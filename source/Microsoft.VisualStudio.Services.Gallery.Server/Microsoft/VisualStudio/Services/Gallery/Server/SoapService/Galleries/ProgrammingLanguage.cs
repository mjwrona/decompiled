// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.ProgrammingLanguage
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  internal static class ProgrammingLanguage
  {
    public const string LanguageNameCSharp = "C#";
    public const string LanguageNameVB = "VB";
    public const string LanguageNameVBScript = "VBS";
    public const string LanguageNameJavaScript = "JavaScript";
    public const string LanguageNameCPlusPlus = "C++";
    public const string LanguageNameFSharp = "F#";
    public const string LanguageNameSql = "SQL";
    public const string LanguageNameHtml = "HTML";
    public const string LanguageNameCss = "CSS";
    public const string LanguageNamePhp = "PHP";
    public const string LanguageNameJava = "Java";
    public const string LanguageNamePerl = "Perl";
    public const string LanguageNamePython = "Python";
    public const string LanguageNameRuby = "Ruby";
    public const string LanguageNamePowerShell = "PowerShell";
    public const string LanguageNamePowerShellWorkflow = "PowerShell Workflow";
    public const string LanguageNameGraphicalRunbook = "Graphical Runbook";
    public const string LanguageNameDelphi = "Delphi";
    public const string LanguageNameXml = "XML";
    public const string LanguageNameXaml = "XAML";
    public const string LanguageNameBash = "Bash/shell";
    public const string LanguageNameActionScript = "ActionScript3";
    public const string LanguageNameKixtart = "Kixtart";
    public const string LanguageNameRexx = "Object REXX";
    public const string LanguageNameWSS = "Windows Shell Script";
    public const string LanguageNameOther = "Other";
    public const string ProjectTypeCloudServiceProject = "CloudServiceProject";
    public const string ProjectTypeCodeSharingProjects = "Code Sharing Projects";
    public const string ProjectTypeCSharp = "CSharp";
    public const string ProjectTypeDnx = "DNX";
    public const string ProjectTypeFabricApplication = "FabricApplication";
    public const string ProjectTypeFSharp = "FSharp";
    public const string ProjectTypeGeneral = "General";
    public const string ProjectTypeJavaScript = "JavaScript";
    public const string ProjectTypeLightSwitch = "LightSwitch";
    public const string ProjectTypePhp = "PHP";
    public const string ProjectTypeTypeScript = "TypeScript";
    public const string ProjectTypeVC = "VC";
    public const string ProjectTypeVisualBasic = "VisualBasic";
    public const string ProjectTypeWeb = "Web";
    public const string ProjectTypeWiX = "WiX";
    public const string VsixTemplateLanguageCSharp = "CSharp";
    public const string VsixTemplateLanguageVB = "VisualBasic";
    public const string VsixTemplateLanguageJavaScript = "Javascript";
    public const string VsixTemplateLanguageCPlusPlus = "VC";
    public const string VsixTemplateLanguageFSharp = "FSharp";
    public const string VsixTemplateLanguageOther = "Other";
    public static IDictionary<string, string> VsLanguageCategoryDisplayMap = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "C#",
        "Visual C#"
      },
      {
        "VB",
        "Visual Basic"
      },
      {
        "F#",
        "Visual F#"
      },
      {
        "C++",
        "Visual C++"
      }
    };
    public static readonly List<string> DefinedProgrammingLanguageNames = new List<string>()
    {
      "CSharp",
      "VisualBasic",
      "Javascript",
      "VC",
      "FSharp"
    };
    public static readonly List<string> OtherLanguageNames = new List<string>()
    {
      "CloudServiceProject",
      "Code Sharing Projects",
      "DNX",
      "FabricApplication",
      "General",
      "LightSwitch",
      "PHP",
      "TypeScript",
      "Web",
      "WiX"
    };
    public static readonly Dictionary<string, string> VsixTemplateLanguageToLanguageName = new Dictionary<string, string>()
    {
      {
        "CSharp",
        ProgrammingLanguage.VsLanguageCategoryDisplayMap["C#"]
      },
      {
        "FSharp",
        ProgrammingLanguage.VsLanguageCategoryDisplayMap["F#"]
      },
      {
        "VisualBasic",
        ProgrammingLanguage.VsLanguageCategoryDisplayMap["VB"]
      },
      {
        "Javascript",
        "JavaScript"
      },
      {
        "VC",
        ProgrammingLanguage.VsLanguageCategoryDisplayMap["C++"]
      },
      {
        "Other",
        "Other"
      }
    };
    public static readonly Dictionary<int, string> ProgrammingLanguageIdToVsixTemplateLanguageMap = new Dictionary<int, string>()
    {
      {
        1,
        "CSharp"
      },
      {
        2,
        "FSharp"
      },
      {
        3,
        "VisualBasic"
      },
      {
        4,
        "Javascript"
      },
      {
        5,
        "VC"
      },
      {
        6,
        "Other"
      }
    };
  }
}
