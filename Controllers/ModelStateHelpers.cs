using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

public class ModelStateException : Exception {
  public string[] ErrorMessages { get; private set; }

  public ModelStateException(string message)
    : base(message) {
    ErrorMessages = new string[] { message };
  }

  public ModelStateException(string[] messages)
    : base(messages == null ? "" : string.Join(" ", messages)) {
    ErrorMessages = messages;
  }
}

public static class ModelStateHelpers {

  public static void ThrowIfInvalid(this ModelStateDictionary modelState) {
    if (!modelState.IsValid) {
      var messages = modelState
        .Values
        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
        .Distinct()
        .ToArray();
      throw new ModelStateException(messages);
    }
  }
}
