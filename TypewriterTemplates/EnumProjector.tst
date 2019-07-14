${
  // Enable extension methods by adding using Typewriter.Extensions.*
  using Typewriter.Extensions.Types;
  using System.ComponentModel;

  // Uncomment the constructor to change template settings.
  Template(Settings settings) {

    //from: https://github.com/frhagn/Typewriter/issues/106#issuecomment-208827766
    settings.OutputFilenameFactory = file => {
      try {
        var fileName = file.Enums.FirstOrDefault()?.Name; 
        return $"../ClientApp/constants/{fileName}.ts"; //todo: could not change .ts via webpack.config.js rule??? => https://github.com/s-panferov/awesome-typescript-loader/issues/404#issuecomment-305075186
      }
      catch (Exception) {
        return $"../ClientApp/template_error.tsx";
      }
    };
  }

  string Description(EnumValue v) =>
    v.Attributes.FirstOrDefault(a => a.Name == "Description")?.Value ?? v.Name;
  
  string StringName(string name) =>
    name + "String";

  bool IsProjection(Enum o) =>
    o.Attributes.Any(a => a.Name == "WithTypeScript");
    
  bool EnumStringLiteralsTest(Enum o, bool answer) =>
    o.Attributes.Any(a => a.Name == "WithTypeScript" && ((a.Value?.Contains($"EnumStringLiterals = true") ?? false) == answer));

  bool EnumStringLiterals(Enum o) => 
    EnumStringLiteralsTest(o, true);

  bool EnumNumbers(Enum o) => 
    EnumStringLiteralsTest(o, false);
    
  string InnerCommentMaybe(DocComment comment) =>
    FormatMaybe("\r\n  // {0}", comment.ToString());

  string OuterCommentMaybe(DocComment comment) =>
    FormatMaybe("\r\n// {0}", comment.ToString());

  string FormatMaybe(string format, string content) =>
    string.IsNullOrEmpty(content) ? "" : string.Format(format, content);

}/***********************************************
*** This file is codeGen'd. Don't edit manually.
************************************************/
$Enums($EnumNumbers)[$DocComment[$OuterCommentMaybe]
export enum $Name {$Values[$DocComment[$InnerCommentMaybe]
  $name = $Value,]
}
]$Enums($EnumStringLiterals)[$DocComment[$OuterCommentMaybe]
export enum $Name {$Values[$DocComment[$InnerCommentMaybe]
  $name = '$Description',]
}
]