using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;

public class ApiExceptionFilter : ExceptionFilterAttribute {
  public override void OnException(ExceptionContext context) {
    DebugShowException(context.Exception, verbose: true);

    string[] messages = null;

    var baseException = context.Exception.GetBaseException();
    messages = new[] {baseException.Message};
#if !DEBUG
      //if PROD, be careful not to expose database errors as a security precaution
      messages = new[] {"An unhandled error occurred."};

      //TODO: Brent: I'm thinking it'd be convenient to pop some end user exceptions from stored procedures
      //      I'm thinking we might be able to selectively allow those through by 'tagging' them
      //      with a specific Oracle Error number range
#endif
    //ModelState exceptions are specifically intended to be end user consumption
    if (baseException is ModelStateException modelException) {
      messages = new[] { modelException.Message };
    }

    var apiResponse = new InventoryPrototype.Models.ApiResponse<string> {
      ErrorMessages = messages
    };
    context.HttpContext.Response.StatusCode = 400 /*bad request*/;
    context.Result = new JsonResult(apiResponse);

    base.OnException(context);
  }


  [Conditional("DEBUG")]
  private static void DebugShowException(Exception exception, bool verbose) {
    if (verbose) {
      Debug.WriteLine(" -------------------- Exception Messages -------------------- ");
      Debug.WriteLine(exception.GetAllExceptionMessages());
      Debug.WriteLine(" -------------------- Exception Report -------------------- ");
      Debug.WriteLine(exception.GetExceptionsReport());
      Debug.WriteLine("");
    }
    else {
      Debug.Fail(exception.GetAllExceptionMessages());
    }
  }
}
