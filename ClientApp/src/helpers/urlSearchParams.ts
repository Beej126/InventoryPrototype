import moment from 'moment';

/*
export { };

declare global {
  interface URLSearchParams {
    appendAll(objToAppend: any): void;
  }
}

if (!URLSearchParams.prototype.appendAll) {
  URLSearchParams.prototype.appendAll = function(objToAppend): void {
    Object.keys(objToAppend).forEach((k) => {
      const val = objToAppend[k];
      if (val !== null && typeof val !== 'undefined') {
        let valString: string;
        if (val instanceof Date) {
          valString = moment(val).format((moment as any).HTML5_FMT.DATE);
        } else {
          valString = val.toString();
        }
        this.append(k, valString);
      }
    });
  };
}
*/


/* brent: there was an issue with the previous prototype extension approach here, ** only running on IE/polyfill **
  the error was 'URLSearchParams is undefined' on the line with: URLSearchParams.prototype.appendAll
  i guess it's a conflict between how typescript declares this as an interface and the polyfill just does a var
}*/

export function URLSearchParams_appendAll(urlSearchParams: URLSearchParams, objToAppend: any): void {
  Object.keys(objToAppend).forEach((k) => {
    const val = objToAppend[k];
    if (val !== null && typeof val !== 'undefined') {
      let valString: string;
      if (val instanceof Date) {
        valString = moment(val).format(moment.HTML5_FMT.DATE);
      } else {
        valString = val.toString();
      }
      urlSearchParams.append(k, valString);
    }
  });
}
