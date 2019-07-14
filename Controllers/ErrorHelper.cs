using System;
using System.Collections;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

/// <summary>
/// Logs, formats, and translates messages.
/// </summary>
public static class ErrorHelper {
  private const string _defaultDataDictionaryKey = "Detail";
  private const string _standardIndent = "   ";               // Matches .net exception stack's format.


  /// <summary>
  /// Add to the Data dictionary.
  /// Default Keys will be Detail, Detail2, Detail3, ....
  /// </summary>
  public static void AddData(this Exception ex, object value) {
    AddData(ex, key: _defaultDataDictionaryKey, value: value, autoIncrementKeys: true);
  }


  /// <summary>
  /// Add to the Data dictionary.
  /// </summary>
  public static void AddData(this Exception ex, string key, object value, bool autoIncrementKeys = true) {
    if (!autoIncrementKeys) {
      if (!ex.Data.Contains(key)) {
        ex.Data.Add(key, value);
      }
    }
    else {
      int num = 1;
      var suffix = "";
      while (ex.Data.Contains(key + suffix)) {
        num++;
        suffix = num.ToString();
      }
      ex.Data.Add(key + suffix, value);
    }
  }


  public static int CountDepth(this Exception ex) {
    int depth = 0;
    while (null != (ex = ex.InnerException)) {
      depth++;
    }
    return depth;
  }


  public static string FormatLogEventStack(string text) {
    if (string.IsNullOrEmpty(text)) {
      return "";
    }
    const string existingDelimiter = " => "; // Is it better to capture leading white-space with regex? This works and is faster.
    return text.Replace(existingDelimiter, Environment.NewLine + existingDelimiter);
  }


  /// <summary>
  /// Gets exception's and all nested exceptions's Messages separated by line breaks.
  /// </summary>
  /// <param name="ex"></param>
  /// <returns></returns>
  public static string GetAllExceptionMessages(this Exception ex) {
    return GetExceptionsReport(ex, verbose: false);
  }


  /// <summary>
  /// Formats exception message, type name, stack, etc., for ex and all inner exception messages.
  /// </summary>
  public static string GetExceptionsReport(this Exception exception, bool verbose = true) {
    int depth = 0;
    var rpt = new StringBuilder();
    var ex = exception;
    do {
      rpt.AppendLine($"Exception (Depth {depth}{(ex.InnerException == null ? "-Base" : "")}): {ex.GetType().Name} - {ex.Message}");
      if (verbose) {
        rpt.AppendLine(ex.StackTrace);
      }

      string subRpt = null;    // Get a sub-report if there is a sub-collection to read. Testing ex's type as we go.
      if (  /**/ null != (subRpt = (ex as ReflectionTypeLoadException)?.GetLoaderExceptionsReport())) {
        rpt.AppendLine(subRpt);

      }
      else if (null != (subRpt = (ex as SqlException)?.GetSqlErrorsReport())) {
        rpt.AppendLine(subRpt);

      }
      //else if (null != (subRpt = (ex as DbEntityValidationException)?.GetEntityValidationErrorsReport())) {
      //  rpt.AppendLine(subRpt);
      //}

      if (ex.Data?.Count > 0) {
        rpt.AppendLine(GetDataReport(ex));
      }

      depth++;
      ex = ex.InnerException;
    } while (ex != null);

    return rpt.ToString().TrimEnd();
  }


  /// <summary>
  /// Gets a string of the contents of the given exception's data dictionary separated by line breaks.
  /// Does not look into inner or related exceptions.
  /// </summary>
  /// <returns>
  ///   Data["Detail1"]: content 1
  ///   Data["Detail2"]: content 2
  ///   Data["NamedKey"]: content 3
  ///   Data[weird-non-string-key]: content 4
  /// </returns>
  public static string GetDataReport(Exception ex) {
    var sb = new StringBuilder();

    if (ex.Data?.Count > 0) {
      foreach (DictionaryEntry de in ex.Data) {   // Only considering top level Data for exception for Data by design.
        string keyString = (de.Key is string)
            ? $"\"{de.Key}\""                   // Format with "quotes"
            : $"{de.Key}";                      // Will call ToString() 
        sb.AppendLine($"{_standardIndent}Data[{keyString}]: {de.Value}");
      }
    }

    return sb.ToString().TrimEnd();
  }


  private static string GetLoaderExceptionsReport(this ReflectionTypeLoadException ex) {
    var sb = new StringBuilder();

    int i = 0;
    foreach (var le in ex.LoaderExceptions) {
      sb.AppendLine($"{nameof(ex.LoaderExceptions)}[{i}]: {le.Message}");
      i++;
    }

    return sb.ToString().Trim();
  }


  //private static string GetEntityValidationErrorsReport(this DbEntityValidationException ex) {
  //  var sb = new StringBuilder();

  //  int eveInd = 0;
  //  if (ex != null) {
  //    foreach (var eve in ex.EntityValidationErrors) {
  //      sb.AppendLine($"{nameof(ex.EntityValidationErrors)}[{eveInd}]: {eve.Entry.Entity.GetType().FullName} in state {eve.Entry.State} has the following {nameof(eve.ValidationErrors)}:");
  //      int innerInd = 0;
  //      foreach (var ve in eve.ValidationErrors) {
  //        sb.AppendLine($"{_standardIndent}[{innerInd}]: {{ Property: '{ve.PropertyName}', Value: '{eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName)}', Error: '{ve.ErrorMessage}'}}");
  //        innerInd++;
  //      }
  //      eveInd++;
  //    }
  //    sb.AppendLine();
  //  }

  //  return sb.ToString().TrimEnd();
  //}


  private static string GetSqlErrorsReport(this SqlException sqlException) {
    var sb = new StringBuilder();

    int i = 0;
    foreach (SqlError err in sqlException.Errors) {
      sb.AppendLine($"Errors[{i}]: ({err.Number}) {err.Message}");
    }

    return sb.ToString().TrimEnd();
  }
}
