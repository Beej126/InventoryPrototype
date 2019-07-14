#if false
using System.ComponentModel;
using Lims.Attributes;

namespace Lims.Constants {
  [WithTypeScript(EnumStringLiterals = true)]
  public enum AAA {
    /// <summary>
    /// this is a comment for 'a'
    /// </summary>
    [Description("abc Description")]
    ABC,

    TheDDDDDDivision,
    TheLastOne
  }

  /// <summary>
  ///  Comment for BBB
  /// </summary>
  [WithTypeScript(EnumStringLiterals = false)]
  public enum BBB {
    [Description("a Description")]
    A,
    /// <summary>
    /// Comment for b.
    /// It is on 2 lines.
    /// </summary>
    B
  }

  public enum NonTS {
    A, B, C, D
  }

  [WithTypeScript]
  public enum YYY {
    A, B, C, D
  }

  [WithTypeScript(EnumStringLiterals = false)]
  public enum ZZZ {
    A, B, C, D
  }
}
#endif