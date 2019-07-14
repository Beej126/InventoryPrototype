${ 
  //**********************************************************************
  //BIG TIP!! be careful not to use "crazy" stuff like single quotes
  //or multiline comments here in the top helper code section
  //***********************************************************************

  // Enable extension methods by adding using Typewriter.Extensions.*
  using Typewriter.Extensions.Types;
  using System.Text.RegularExpressions;
  using System.Collections;
  using System.Text;

  Template(Settings settings) {
    //from: https://github.com/frhagn/Typewriter/issues/106#issuecomment-208827766
    settings.OutputFilenameFactory = (file) => {
      try {
        var firstClass = ClassesInFile(file).FirstOrDefault();
        var className = (firstClass?.ContainingClass ?? firstClass)?.Name;
        var interfaceName = file.Interfaces.FirstOrDefault()?.Name;
        return $"../ClientApp/src/models/{className??interfaceName}.ts";
      }
      catch (Exception) {
        // BA: this is a super low brow attempt at error visibility
        // i.e. if something blew up, this file will be present
        // the typewriter exception reporting can be pretty invisible at times
        return $"../ClientApp/template_error.tsx";
      }
    };
  }

  // debug trick, "printf" lihves!! https://github.com/frhagn/Typewriter/issues/121#issuecomment-231323983
  // concatenate anything you need into this debugInfo variable from your custom methods
  // and then throw $PrintDebugInfo into the main template space below to view in the output window
  // static string debugInfo = "";
  // string PrintDebugInfo(File f) => debugInfo; 

  // nested class support: https://github.com/frhagn/Typewriter/issues/134#issuecomment-253771122
  bool ClassFilter(Class c) => //Regex.IsMatch(c.Name, "Dto$|Command$");
    c.Attributes.Any(a => a.Name == "WithTypeScript");
  IEnumerable<Class> ClassesInFile(File f) {
    var classesInFile = f.Classes.Where(ClassFilter)
      // BA: subtle tweak on the original github logic ref''d above...  
      // i want to pull a NESTED *Command class who''s PARENT does NOT match the filter
      // therefore starting with f.Classes versus classes
      .Concat(f.Classes.SelectMany(c => c.NestedClasses).Where(ClassFilter)).ToList();
    //BA: see imports comment below: AllClassesCached.Concat(classesInFile);
    //debugInfo += $"{f.Name} result: " + string.Join("\r\n", ClassesInFile.Select(c=>c.Name)) + "\r\n";
    return classesInFile;
  }

  // Custom extension methods can be used in the template by adding a $ prefix e.g. $LoudName
  string LoudName(Property property) {
    return property.Name.ToUpperInvariant();
  }

  //tack on the not null operator
  //e.g. data!: T;
  string PropertyNameEx(Property p) {
    return p.name + ((p.Attributes.Any(a => a.Name == "NotNullable") || !p.Type.IsNullable) && TypeDefault(p) == "" ? "!" :"");
  }

  string TypeConverter(Property p) {
    var result = p.Type.Name;
    if (p.Attributes.Any(a => a.Name == "Nullable") || p.Type.IsNullable) {
      result += " | null";
    }
    //only using properties of type IFormFile for name safety on the client so actual type doesn''t really matter
    if (result == "IFormFile") { result = "any"; } 
    return result;
  }

  //create a list of generic parms, e.g. <T>, by classname so that we can compare to properties later
	static HashSet<string> GenericParameters = new HashSet<string>();
	string TypeParameter(TypeParameter tp) {
		GenericParameters.Add($"{tp.Parent}-{tp.Name}");
		return $"<{tp.Name}>";
	}

  string TypeDefault(Property p) {
		//if this is a generic parameter type e.g. <T> then allow it to be null since thats in parity with C#
    //if (GenericParameters.Contains($"{p.Parent}-{p.Type.Name}")) {
		//	return " | null = " + p.Type.Default();
		//}

    if (p.Type.IsEnum) {
      return $" = {p.Type.Name}.{p.Type.Constants[0].name}";
    }
    if (p.Type.Name == "string") {
      return " = ''";
    }
    if (p.Type.IsEnumerable) {
      return " = []";
    }
    if (p.Attributes.Any(a => a.Name == "Nullable") || p.Type.IsNullable) {
      return " = null";
    }
    if (p.Type.Default() == "null" && p.Attributes.Any(a => a.Name == "NotNullable")) {
      return "";
    }

    return " = " + p.Type.Default();
  }

  string GrabToggleValue(string textualProperties, string posNeg) {
    if (!textualProperties.Contains($"{posNeg}Display")) {
      var fullmatch = Regex.Match(textualProperties, $"{posNeg}Value:\\s*'(.*?)'");
      var shorthand = fullmatch.Groups[1].Value;
      var value = Regex.Match(shorthand, "\\((.*?)\\)").Groups[1].Value;
      return textualProperties = textualProperties.Replace(fullmatch.Value, $"{posNeg}Value: '{value}'") +
        $", {posNeg}Display: '{shorthand.Replace("(","").Replace(")","")}'";
    }
    return textualProperties;
  }

  //attempting to make this mapping logic somewhat generic so we can load up multiple case statements on the same return
  //fyi, i''ve submitted this request for 3rd party assembly support for stuff like camelCasing:
  //https://github.com/frhagn/Typewriter/issues/232#issue-269180636
  string attrib(Attribute a) {
    //BA: the template goes haywire if you use a real apostrophe in this section
    var val = a?.Value?.Replace((char)34, (char)39);

    switch (a.name) {
      case "hiddenField":
      case "ignore":
      case "required": //boolean
        return $"{a.name}: true";
      case "stringLength": //renaming c# attrib to something more satisfying
        return $"maxLen: {val}";
      case "display":
        var nameval = val.Split('=');
        return $"{nameval[0].ToLowerInvariant().Trim(' ')}" + (nameval.Length > 1 ? $": {nameval[1].Trim(' ').Replace("=", ":")}" : "");
      case "toggle": //bag of properties
        val = val.Replace(" =", "=").Replace("=", ":");
        //lowercase first letter of each property name
        val = Regex.Replace(val, "(^|,)\\s*([A-Z])", m => m.Groups[0].Value.ToLower());
        val = GrabToggleValue(val, "pos");
        val = GrabToggleValue(val, "neg");
        return $"{a.name}: {{{val}}}";
      case "displayWidth": //numeric value vs string as default
        return $"{a.name}: {val}";
      default: //BA:shifting to ignore by default
        return null; //$"{a.name}: \"{val}\"";
      /*
      case "resultColumn":
      case "computedColumn":
      case "column":
        return null; //ignore these attributes
      */
    }
  }
  
  string Meta(Property p) {
    var attribs = p.Attributes.Select(a => attrib(a)).Where(s=>s!=null).ToArray();
    var attribsString = string.Join(", ", attribs).TrimStart(',');
    return attribs.Length > 0 ? $"\r\n  @meta({{ {attribsString} }})\r\n" : "\r\n";
  }

  string NestedTop(Class c) {
    if (c.ContainingClass != null) {
      return $"export namespace {c.ContainingClass} {{\r\n";
    }
    return "";
  }

  string NestedBottom(Class c) {
    if (c.ContainingClass != null) {
      return $"\r\n}}\r\n";
    }
    return "";
  }

  // BA: this "automatic" approach won''t fly since typewriter naturally might be
  // emitting a class-file with properties of another type that hasn''t been covered yet
  // so there''s no obvious way to identify those now that we don''t name them with an overt "dto" style "flag"
  // please keep this commented code around a little bit longer in case inspiration strikes :)
  //
  // static List<Class> AllClassesCached = new List<Class>();
  //
  // string Imports(File f) {
  //  var classes = ClassesInFile(f);
  //  return string.Concat(
  //    // starting with all classes flagged by Typescript attribute...
  //    classes
  //      // get all the properties on the class...
  //      .SelectMany(c => c.Properties)
  //      // where the property type corresponds to a class in the list to be projected
  //      // startsWith performs a sql style "like" to encompass stuff like arrays of type (e.g. MatrixDef[])
  //      .Where(p => AllClassesCached.Any(c=>p.Type.Name.StartsWith(c.Name)))
  //      .Select(p => String.Format(
  //        "\r\nimport {{ {0} }} from '../models/{0}';", 
  //        p.Type.Name.TrimEnd(new [] {'[',']'})
  //      )
  //    )
  //  );
  //}

  string Imports(File f) {
    var imports = ClassesInFile(f)
        .SelectMany(c=>c.Attributes)
        .Where(a=>a.Value?.ToLower().StartsWith("import") ?? false)
        //(?-i) means case insensitivity, who knew?! ;)
        .Select(a=>Regex.Match(a.Value,"(?i)import\\s*=\\s*\"(.*?)\"").Groups[1].Value)
        .ToArray();
    return imports.Length>0 ? "\r\n" + string.Join("\r\n", imports) + "\r\n" : "";
  }

  string Source(File f) => f.FullName.Substring(Math.Max(0,f.FullName.IndexOf("InventoryPrototype"))); 

}/************************************************************************
 *** This file is generated. Don't edit manually.
 *** Source: $Source
 *** Template: TypewriterTemplates\ClassProjector.tst
 ************************************************************************/
/* tslint:disable */

import { meta } from '../helpers/meta'; $Imports$Classes(c => ClassFilter(c) || c.NestedClasses.Any(ClassFilter))[]
$ClassesInFile[$NestedTop
export class $Name$TypeParameters[$TypeParameter]$BaseClass[ extends $Name] {
  public static readonly _className: string = '$Name';$Properties[
$Meta  public $PropertyNameEx: $TypeConverter$TypeDefault;]
}
$NestedBottom
$Interfaces(*Dto)[
export interface $Name$TypeParameters {
$Properties[  $name: $Type;]
}]]