using System;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class WithTypeScriptAttribute : Attribute {

  /// <summary>
  /// Specifies the optional base class for this typescript projection.
  /// </summary>
  public string BaseClass { get; set; }

  /// <summary>
  /// When on an enum, specifies use string literals for values.
  /// The description if present or else name will be used as the value.
  /// Otherwise the numeric values are used.
  /// </summary>
  public bool EnumStringLiterals { get; set; } = false;

  /// <summary>
  /// When a property has a complex type, use this on the class to specify the full import statement.
  /// </summary>
  /// <example>
  ///  [WithTypeScript(Import = "import { MessageLevel } from '../constants/MessageLevel';")]
  ///  [WithTypeScript(Import = "import { Emoji } from '../constants/Emoji';")]
  ///  public class Message {
  ///    public MessageLevel Level { get; set; }
  ///    public Emoji Emoji { get; set; }
  ///    public string Text { get; set; }
  ///  }
  /// </example>
  public string Import { get; set; }

  /// <summary>
  /// Set true to include "changeStatus" enum property on the TS class
  /// <see cref="Lims.Api.Constants.ChangeStatus"/>
  /// </summary>
  /// <example>
  ///   [WithTypeScript(ChangeStatus = true)]
  ///   public class MatrixDef {...}
  /// </example>
  public bool ChangeStatus { get; set; } = false;
}
