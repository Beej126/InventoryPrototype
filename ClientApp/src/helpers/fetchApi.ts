import * as toastr from 'toastr';
import { ApiResponse } from '../models/ApiResponse';
import { URLSearchParams_appendAll } from './urlSearchParams'

const previousToastHandles: any[string] = [];

type ValidHttpVerbs = 'GET' | 'POST' | 'PUT' | 'DELETE';

export async function fetchApi<TIn, TOut>(
  controller: string,
  action: string | null = null,
  inData?: TIn | null,
  verb: ValidHttpVerbs = 'GET',
  toastErrors: boolean = true
): Promise<ApiResponse<TOut>> {

  // one reason for using FormData (over simple pojo) is that it handles binary file upload piece (in Document Storage)
  // ReSharper disable once TypeGuardProducesNeverType
  const isFormData = inData instanceof FormData;

  const previousToast = previousToastHandles[controller];
  //clear previous error
  if (previousToast && previousToast[0].className.indexOf('error') > -1) {
    previousToast.hide(); //skip the fade out since the user knows they've initiated another action
    toastr.clear(previousToast);
  }
  //only show another info toast (e.g. "loading") if we don't already have one visible from a previous call on this same page
  const progressToast = !previousToast ? toastr.info(getProgressText(verb), '', { progressBar: true }) : null;

  const headers = new Headers();
  //FormData automatically gets sent as Content-Type: multipart/form-data
  if (!isFormData) {
    headers.set('Accept', 'application/json');
    headers.set('Content-Type', 'application/json');
  }
  let apiResp: ApiResponse<TOut>;
  try {
    let url = new URL(baseUrl.trimEnd('/') + '/api/' + controller + '/' + (action?action:''));

    let body: any = null;
    if (verb === 'GET') { if (inData) { URLSearchParams_appendAll(url.searchParams, inData);} }
    else body = isFormData ? inData as any as FormData : JSON.stringify(inData); //some tutorials show this as simply assigning object but that results in [Object object] for me

    const response = await fetch(url.toString(), {
      body: body,
      credentials: 'include',
      headers,
      method: verb,
      mode: 'cors'
    });
    const apiResponse = await response;
    apiResp = await apiResponse.json() as any as ApiResponse<TOut>;
  }
  catch (e) {
    apiResp = new ApiResponse<TOut>();
    apiResp.success = false;
    apiResp.errorMessages.push(`${e.message}`);
    if (e.message === 'Failed to fetch') {
      apiResp.errorMessages.push('<br/><br/>Probably need to restart Controller API');
    }
  }

  if (progressToast) {
    progressToast.hide(); //skip the fade out
    toastr.clear(progressToast);
  }

  if (apiResp.success) {
    if (verb !== 'GET') {
      previousToastHandles[controller] = toastr.success(getDefaultSuccessText(verb));
    }
  }

  else {
    let errMsg = apiResp.errorMessages[0];
    if (apiResp.errorMessages.length > 1) { errMsg = apiResp.errorMessages.map((_) => `<li>${_}</li>`).join('') || getDefaultFailureText(verb); } 
    if (toastErrors) {
      previousToastHandles[controller] = toastr.error(errMsg, 'Error' + (apiResp.errorMessages.length>1?'s':''), { timeOut: 0, toastClass: 'toast-nowrap', closeButton: true });
    }
    // throw-on-error convention...
    // facilitates coding promise.then(...) as only executing when successful
    // and coding for promise.except() if needed
    // versus checking for success as a status property
    // throw new Error(errMsg);
  }

  return apiResp;
}

function getProgressText(verb: ValidHttpVerbs) {
  // By convention POST is a 'load' and PUT is a save, etc.
  switch (verb) {
    case 'GET':
      return 'Loading...';
    case 'POST':
    case 'PUT':
      return 'Saving...';
    case 'DELETE':
      return 'Deleting...';
    default:
      //const exhaustiveCheck: never = verb; //TODO: somehow this was thinking 'DELETE' was possible
      return '';
  }
}

function getDefaultSuccessText(verb: ValidHttpVerbs) {
  // By convention POST is a 'load' and PUT is a save, etc.
  switch (verb) {
    case 'GET':
      return '';
    case 'POST':
    case 'PUT':
      return 'Save successful.';
    case 'DELETE':
      return 'Delete successful.';
    default:
      //const exhaustiveCheck: never = verb;
      return '';
  }
}

function getDefaultFailureText(verb: ValidHttpVerbs) {
  // By convention POST is a 'load' and PUT is a save, etc.
  switch (verb) {
    case 'GET':
    case 'POST':
      return 'An error occured while loading.';
    case 'PUT':
      return 'An error occured while saving.';
    case 'DELETE':
      return 'An error occured while deleting.';
    default:
      //const exhaustiveCheck: never = verb;
      return '';
  }
}


declare global {
  interface FormData {
    /**
     * serialize JS object
     */
    fromObject(data: object): FormData;
  }
}

// Brent: from what i'm seeing we can't add this as first class constructor due to how it's ts interface is declared...
// String's interface as another example actually has a little more wiggle room...
// String is declared as a var from StringConstructor interface
// this allows us to add to those methods at static class interface level (via ts "interface merging") versus the instance interface
// ...so we do the ol' JS prototype way...
// and then we just use the prototype function as a constructor
// or we could instantiate first and use formObj.fromObject but that seems sorta backwards
//  e.g. var payload = FormData.prototype.fromObject(jsobject);
/*
FormData.prototype.fromObject = function (data: object) {
  const formData = new FormData();
  serialize(formData, null, data);
  return formData;
};

// adapted from: https://stackoverflow.com/a/43101878
function serialize(formData: FormData, key: string | null, data: object): void {
  if (Array.isArray(data)) {
    data.forEach((val) => {
      serialize(formData, `${key || ""}[]`, val);
    });
  } else if (data instanceof Object && !(data instanceof File)) {
    typedKeys(data).forEach((propName) => {
      serialize(formData, key ? `${key}[${propName}]` : propName, data[propName]);
    });
  } else {
    if (data) { formData.append(key!, data); }
  }
}
*/
