// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.CodeSearchFilters
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public static class CodeSearchFilters
  {
    public static readonly IReadOnlyList<string> DisplayableCEFilterIds = (IReadOnlyList<string>) new List<string>()
    {
      "arg",
      "basetype",
      "class",
      "comment",
      "decl",
      "def",
      "enum",
      "func",
      "macro",
      "namespace",
      "prop",
      "ref",
      "strlit",
      "struct"
    };
    public static readonly IReadOnlyDictionary<string, CodeSearchFilters.CEFilterAttributes> CEFilterAttributeMap = (IReadOnlyDictionary<string, CodeSearchFilters.CEFilterAttributes>) new Dictionary<string, CodeSearchFilters.CEFilterAttributes>()
    {
      {
        "arg",
        new CodeSearchFilters.CEFilterAttributes("Argument", (IEnumerable<int>) new int[5]
        {
          1,
          57,
          149,
          163,
          205
        })
      },
      {
        "basetype",
        new CodeSearchFilters.CEFilterAttributes("Basetype", (IEnumerable<int>) new int[7]
        {
          2,
          81,
          162,
          174,
          160,
          161,
          206
        })
      },
      {
        "class",
        new CodeSearchFilters.CEFilterAttributes("Class", (IEnumerable<int>) new int[6]
        {
          3,
          4,
          5,
          79,
          173,
          207
        })
      },
      {
        "classdecl",
        new CodeSearchFilters.CEFilterAttributes("Class Declaration", (IEnumerable<int>) new int[2]
        {
          3,
          5
        })
      },
      {
        "classdef",
        new CodeSearchFilters.CEFilterAttributes("Class Definition", (IEnumerable<int>) new int[5]
        {
          4,
          79,
          173,
          171,
          207
        })
      },
      {
        "comment",
        new CodeSearchFilters.CEFilterAttributes("Comment", (IEnumerable<int>) new int[4]
        {
          6,
          130,
          158,
          208
        })
      },
      {
        "ctor",
        new CodeSearchFilters.CEFilterAttributes("Constructor", (IEnumerable<int>) new int[6]
        {
          8,
          9,
          99,
          142,
          176,
          209
        })
      },
      {
        "decl",
        new CodeSearchFilters.CEFilterAttributes("Declaration", (IEnumerable<int>) new int[53]
        {
          1,
          2,
          3,
          5,
          7,
          8,
          10,
          12,
          14,
          15,
          16,
          17,
          18,
          19,
          21,
          25,
          26,
          30,
          35,
          37,
          39,
          41,
          42,
          44,
          45,
          48,
          50,
          109,
          120,
          121,
          66,
          73,
          74,
          96,
          99,
          111,
          112,
          118,
          129,
          152,
          191,
          189,
          190,
          166,
          167,
          169,
          168,
          175,
          172,
          187,
          210,
          211,
          212
        })
      },
      {
        "def",
        new CodeSearchFilters.CEFilterAttributes("Definition", (IEnumerable<int>) new int[52]
        {
          4,
          7,
          9,
          11,
          13,
          14,
          17,
          20,
          22,
          23,
          24,
          31,
          32,
          36,
          40,
          43,
          46,
          49,
          51,
          120,
          121,
          53,
          57,
          66,
          74,
          79,
          82,
          108,
          140,
          141,
          142,
          146,
          118,
          152,
          173,
          172,
          189,
          190,
          185,
          177,
          171,
          178,
          166,
          167,
          169,
          168,
          187,
          207,
          213,
          214,
          209,
          215
        })
      },
      {
        "dtor",
        new CodeSearchFilters.CEFilterAttributes("Destructor", (IEnumerable<int>) new int[4]
        {
          10,
          11,
          112,
          141
        })
      },
      {
        "enum",
        new CodeSearchFilters.CEFilterAttributes("Enumerator", (IEnumerable<int>) new int[9]
        {
          12,
          13,
          14,
          118,
          146,
          189,
          190,
          214,
          216
        })
      },
      {
        "extern",
        new CodeSearchFilters.CEFilterAttributes("Extern", (IEnumerable<int>) new int[2]
        {
          15,
          16
        })
      },
      {
        "field",
        new CodeSearchFilters.CEFilterAttributes("Field", (IEnumerable<int>) new int[7]
        {
          7,
          17,
          120,
          121,
          169,
          168,
          210
        })
      },
      {
        "friend",
        new CodeSearchFilters.CEFilterAttributes("Friend", (IEnumerable<int>) new int[3]
        {
          18,
          19,
          20
        })
      },
      {
        "func",
        new CodeSearchFilters.CEFilterAttributes("Function/Method", (IEnumerable<int>) new int[11]
        {
          21,
          22,
          23,
          35,
          36,
          53,
          139,
          178,
          177,
          175,
          213
        })
      },
      {
        "funcdecl",
        new CodeSearchFilters.CEFilterAttributes("Function Declaration", (IEnumerable<int>) new int[4]
        {
          21,
          35,
          139,
          175
        })
      },
      {
        "funcdef",
        new CodeSearchFilters.CEFilterAttributes("Function Definition", (IEnumerable<int>) new int[7]
        {
          22,
          23,
          36,
          53,
          178,
          177,
          213
        })
      },
      {
        "global",
        new CodeSearchFilters.CEFilterAttributes("Global", (IEnumerable<int>) new int[3]
        {
          24,
          25,
          26
        })
      },
      {
        "header",
        new CodeSearchFilters.CEFilterAttributes("Header", (IEnumerable<int>) new int[2]
        {
          28,
          29
        })
      },
      {
        "interface",
        new CodeSearchFilters.CEFilterAttributes("Interface", (IEnumerable<int>) new int[5]
        {
          30,
          31,
          152,
          172,
          215
        })
      },
      {
        "macro",
        new CodeSearchFilters.CEFilterAttributes("Macro", (IEnumerable<int>) new int[3]
        {
          32,
          33,
          34
        })
      },
      {
        "macrodef",
        new CodeSearchFilters.CEFilterAttributes("Macro Definition", (IEnumerable<int>) new int[2]
        {
          32,
          34
        })
      },
      {
        "macroref",
        new CodeSearchFilters.CEFilterAttributes("Macro Reference", (IEnumerable<int>) new int[1]
        {
          33
        })
      },
      {
        "namespace",
        new CodeSearchFilters.CEFilterAttributes("Namespace", (IEnumerable<int>) new int[6]
        {
          5,
          37,
          38,
          122,
          155,
          217
        })
      },
      {
        "prop",
        new CodeSearchFilters.CEFilterAttributes("Property", (IEnumerable<int>) new int[2]
        {
          66,
          187
        })
      },
      {
        "ref",
        new CodeSearchFilters.CEFilterAttributes("Reference", (IEnumerable<int>) new int[34]
        {
          27,
          28,
          29,
          33,
          34,
          38,
          47,
          54,
          58,
          67,
          72,
          75,
          76,
          77,
          78,
          87,
          93,
          97,
          119,
          138,
          157,
          165,
          163,
          196,
          218,
          205,
          219,
          220,
          221,
          222,
          223,
          224,
          225,
          226
        })
      },
      {
        "strlit",
        new CodeSearchFilters.CEFilterAttributes("String Literal", (IEnumerable<int>) new int[3]
        {
          71,
          180,
          227
        })
      },
      {
        "struct",
        new CodeSearchFilters.CEFilterAttributes("Struct", (IEnumerable<int>) new int[4]
        {
          5,
          42,
          43,
          170
        })
      },
      {
        "structdecl",
        new CodeSearchFilters.CEFilterAttributes("Struct Declaration", (IEnumerable<int>) new int[2]
        {
          5,
          42
        })
      },
      {
        "structdef",
        new CodeSearchFilters.CEFilterAttributes("Struct Definition", (IEnumerable<int>) new int[2]
        {
          43,
          170
        })
      },
      {
        "tmplarg",
        new CodeSearchFilters.CEFilterAttributes("Template Argument", (IEnumerable<int>) new int[1]
        {
          44
        })
      },
      {
        "tmplspec",
        new CodeSearchFilters.CEFilterAttributes("Template Specification", (IEnumerable<int>) new int[1]
        {
          45
        })
      },
      {
        "type",
        new CodeSearchFilters.CEFilterAttributes("Type", (IEnumerable<int>) new int[15]
        {
          41,
          47,
          54,
          58,
          67,
          72,
          93,
          97,
          109,
          119,
          220,
          224,
          223,
          222,
          228
        })
      },
      {
        "typedef",
        new CodeSearchFilters.CEFilterAttributes("Typedef", (IEnumerable<int>) new int[1]
        {
          46
        })
      },
      {
        "union",
        new CodeSearchFilters.CEFilterAttributes("Union", (IEnumerable<int>) new int[2]
        {
          48,
          49
        })
      }
    };
    public static readonly IReadOnlyDictionary<string, string> DisplayableCEFilterIdToNameMap = (IReadOnlyDictionary<string, string>) CodeSearchFilters.DisplayableCEFilterIds.ToDictionary<string, string, string>((Func<string, string>) (filter => filter), (Func<string, string>) (filter => CodeSearchFilters.CEFilterAttributeMap[filter].FriendlyName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public static readonly IReadOnlyDictionary<string, string> CEFilterNameToIdMap = (IReadOnlyDictionary<string, string>) CodeSearchFilters.CEFilterAttributeMap.ToDictionary<KeyValuePair<string, CodeSearchFilters.CEFilterAttributes>, string, string>((Func<KeyValuePair<string, CodeSearchFilters.CEFilterAttributes>, string>) (pair => pair.Value.FriendlyName), (Func<KeyValuePair<string, CodeSearchFilters.CEFilterAttributes>, string>) (pair => pair.Key), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public static readonly ISet<string> CEFilterIds = (ISet<string>) new HashSet<string>(CodeSearchFilters.CEFilterAttributeMap.Keys);
    public static readonly ISet<string> NonCEFilterIds = (ISet<string>) new HashSet<string>((IEnumerable<string>) new string[6]
    {
      CodeFileContract.CodeContractQueryableElement.RepoName.InlineFilterName(),
      CodeFileContract.CodeContractQueryableElement.ProjectName.InlineFilterName(),
      CodeFileContract.CodeContractQueryableElement.FilePath.InlineFilterName(),
      CodeFileContract.CodeContractQueryableElement.FileExtension.InlineFilterName(),
      CodeFileContract.CodeContractQueryableElement.FileName.InlineFilterName(),
      CodeFileContract.CodeContractQueryableElement.Regex.InlineFilterName()
    });
    public static readonly ISet<string> SupportedFilterIds = (ISet<string>) new HashSet<string>(CodeSearchFilters.CEFilterIds.Union<string>((IEnumerable<string>) CodeSearchFilters.NonCEFilterIds), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static CodeSearchFilters()
    {
      foreach (KeyValuePair<string, CodeSearchFilters.CEFilterAttributes> ceFilterAttribute in (IEnumerable<KeyValuePair<string, CodeSearchFilters.CEFilterAttributes>>) CodeSearchFilters.CEFilterAttributeMap)
      {
        IEnumerable<int> tokenIds = ceFilterAttribute.Value.TokenIds;
        foreach (KeyValuePair<string, string> ceFilterIdToName in (IEnumerable<KeyValuePair<string, string>>) CodeSearchFilters.DisplayableCEFilterIdToNameMap)
        {
          IEnumerable<int> source = CodeSearchFilters.CEFilterAttributeMap[ceFilterIdToName.Key].TokenIds.Intersect<int>(tokenIds);
          if (source.Any<int>())
            CodeSearchFilters.CEFilterAttributeMap[ceFilterAttribute.Key].DisplayFilterTokenIdMap.Add(ceFilterIdToName.Key, source);
        }
      }
    }

    public static class CEFilterId
    {
      public const string Argument = "arg";
      public const string Basetype = "basetype";
      public const string Class = "class";
      public const string ClassDeclaration = "classdecl";
      public const string ClassDefinition = "classdef";
      public const string Comment = "comment";
      public const string Constructor = "ctor";
      public const string Declaration = "decl";
      public const string Definition = "def";
      public const string Destructor = "dtor";
      public const string Enumerator = "enum";
      public const string Extern = "extern";
      public const string Field = "field";
      public const string Friend = "friend";
      public const string FunctionOrMethod = "func";
      public const string FunctionOrMethodDeclaration = "funcdecl";
      public const string FunctionOrMethodDefinition = "funcdef";
      public const string Global = "global";
      public const string Header = "header";
      public const string Interface = "interface";
      public const string Macro = "macro";
      public const string MacroDefinition = "macrodef";
      public const string MacroReference = "macroref";
      public const string Namespace = "namespace";
      public const string Property = "prop";
      public const string Reference = "ref";
      public const string StringLiteral = "strlit";
      public const string Struct = "struct";
      public const string StructDeclaration = "structdecl";
      public const string StructDefinition = "structdef";
      public const string TemplateArgument = "tmplarg";
      public const string TemplateSpecification = "tmplspec";
      public const string Type = "type";
      public const string Typedef = "typedef";
      public const string Union = "union";
    }

    public class CEFilterAttributes
    {
      public IDictionary<string, IEnumerable<int>> DisplayFilterTokenIdMap { get; private set; }

      public IEnumerable<int> TokenIds { get; private set; }

      public string FriendlyName { get; private set; }

      internal CEFilterAttributes(string friendlyName, IEnumerable<int> tokenIds)
      {
        this.TokenIds = tokenIds;
        this.DisplayFilterTokenIdMap = (IDictionary<string, IEnumerable<int>>) new Dictionary<string, IEnumerable<int>>();
        this.FriendlyName = friendlyName;
      }
    }
  }
}
