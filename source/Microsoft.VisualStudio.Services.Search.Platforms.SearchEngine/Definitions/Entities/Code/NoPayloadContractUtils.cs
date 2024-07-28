// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.NoPayloadContractUtils
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.QueryBuilders;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  internal class NoPayloadContractUtils
  {
    public const string HighlighterName = "noPayloadCodeSearchHighlighter";
    public const string HighlighterNameConstantscore = "noPayloadCodeSearchHighlighter_constantscore";
    public const string HighlighterNameV2 = "noPayloadCodeSearchHighlighter_v2";
    public const string HighlighterNameV2Constantscore = "noPayloadCodeSearchHighlighter_v2_constantscore";
    public const string HighlighterNameV3 = "noPayloadCodeSearchHighlighter_v3";
    public const string HighlighterNameV3Constantscore = "noPayloadCodeSearchHighlighter_v3_constantscore";
    public const string LowerCaseNormalizer = "lowerCaseNormalizer";
    public const string LanguageFieldName = "language";
    public const string ClassFieldName = "class";
    public const string DefinitionFieldName = "def";
    public const string ReferenceFieldName = "ref";
    public const string MethodFieldName = "method";
    public const string StringLiteralFieldName = "strlit";
    public const string EnumFieldName = "enum";
    public const string BaseTypeFieldName = "basetype";
    public const string DeclarationFieldName = "decl";
    public const string NamespaceFieldName = "namespace";
    public const string TypeFieldName = "type";
    public const string InterfaceFieldName = "interface";
    public const string CommentFieldName = "comment";
    public const string MacroFieldName = "macro";
    public const string FieldFieldName = "field";
    [StaticSafe]
    internal static readonly FriendlyDictionary<string, string> CodeTermExpresstionType2FieldName = new FriendlyDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "class",
        "class"
      },
      {
        "def",
        "def"
      },
      {
        "ref",
        "ref"
      },
      {
        "func",
        "method"
      },
      {
        "strlit",
        "strlit"
      },
      {
        "enum",
        "enum"
      },
      {
        "basetype",
        "basetype"
      },
      {
        "decl",
        "decl"
      },
      {
        "namespace",
        "namespace"
      },
      {
        "type",
        "type"
      },
      {
        "interface",
        "interface"
      },
      {
        "comment",
        "comment"
      },
      {
        "macro",
        "macro"
      },
      {
        "field",
        "field"
      }
    };
    [StaticSafe]
    internal static readonly IEnumerable<string> KnownFieldsTypeForHighlightHit = (IEnumerable<string>) new List<string>()
    {
      "class",
      "interface",
      "method",
      "field",
      "enum",
      "macro",
      "comment",
      "strlit",
      "namespace",
      "ref"
    };
    private static readonly IReadOnlyDictionary<string, double> s_codeElementToBoostValueMapping = (IReadOnlyDictionary<string, double>) new Dictionary<string, double>()
    {
      {
        "class",
        10000000.0
      },
      {
        "method",
        10000000.0
      },
      {
        "def",
        1000000.0
      },
      {
        "enum",
        1000000.0
      },
      {
        "basetype",
        100000.0
      },
      {
        "namespace",
        100000.0
      },
      {
        "interface",
        100000.0
      },
      {
        "decl",
        100000.0
      },
      {
        "macro",
        100000.0
      },
      {
        "ref",
        50000.0
      },
      {
        "field",
        10000.0
      },
      {
        "type",
        10000000.0
      },
      {
        "strlit",
        1.0
      },
      {
        "comment",
        0.01
      }
    };
    [StaticSafe]
    private static readonly HashSet<string> s_sortingFields = new HashSet<string>()
    {
      "collectionId",
      CodeContractField.CodeSearchFieldDesc.ProjectId.ElasticsearchFieldName(),
      CodeContractField.CodeSearchFieldDesc.RepoName.ElasticsearchFieldName(),
      CodeContractField.CodeSearchFieldDesc.BranchName.ElasticsearchFieldName(),
      CodeContractField.CodeSearchFieldDesc.SortableFilePath.ElasticsearchFieldName()
    };
    [StaticSafe]
    private static readonly HashSet<string> s_codeElementFields = new HashSet<string>(NoPayloadContractUtils.s_codeElementToBoostValueMapping.Keys);

    public NoPayloadContractUtils()
    {
    }

    public NoPayloadContractUtils(
      IDictionary<CodeFileContract.CodeContractQueryableElement, CodeContractField> fields)
    {
      this.Initialize(fields);
    }

    private void Initialize(
      IDictionary<CodeFileContract.CodeContractQueryableElement, CodeContractField> fields)
    {
      fields[CodeFileContract.CodeContractQueryableElement.Default] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.OriginalContent);
      fields[CodeFileContract.CodeContractQueryableElement.CollectionName] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.CollectionNameInNoPayloadMappings);
      fields[CodeFileContract.CodeContractQueryableElement.ProjectName] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.ProjectNameInNoPayloadMappings);
      fields[CodeFileContract.CodeContractQueryableElement.RepoName] = new CodeContractField(CodeContractField.CodeSearchFieldDesc.RepoNameInNoPayloadMappings);
    }

    public Properties AddNonPayloadProperties(
      Properties properties,
      IVssRequestContext requestContext)
    {
      properties.Add((PropertyName) "class", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      Properties properties1 = properties;
      PropertyName name = (PropertyName) "language";
      KeywordProperty keywordProperty = new KeywordProperty();
      keywordProperty.Name = (PropertyName) "language";
      keywordProperty.Index = new bool?(true);
      keywordProperty.Store = new bool?(true);
      keywordProperty.Norms = new bool?(false);
      keywordProperty.DocValues = new bool?(true);
      properties1.Add(name, (IProperty) keywordProperty);
      properties.Add((PropertyName) "def", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "ref", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "method", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "strlit", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "enum", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "basetype", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "decl", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "namespace", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "type", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "interface", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "comment", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "macro", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      properties.Add((PropertyName) "field", (IProperty) new TextProperty()
      {
        Analyzer = "noPayloadCodesearchAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Norms = new bool?(false)
      });
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EagerGlobalOrdinalsEnabledInNoPayloadMapping"))
      {
        HashSet<string> stringSet = new HashSet<string>()
        {
          "projectNameOriginal",
          CodeContractField.CodeSearchFieldDesc.RepoNameOriginal.ElasticsearchFieldName(),
          "language"
        };
        foreach (KeyValuePair<PropertyName, IProperty> property in (IEnumerable<KeyValuePair<PropertyName, IProperty>>) properties)
        {
          if (stringSet.Contains(property.Key.Name))
            ((KeywordProperty) property.Value).EagerGlobalOrdinals = new bool?(true);
        }
      }
      return properties;
    }

    public IndexSettings AddNonPayloadIndexSettings(
      IndexSettings indexSettings,
      ExecutionContext executionContext,
      string sortingConfigKey)
    {
      indexSettings.Analysis.Tokenizers["pathtokenizer"] = (ITokenizer) new PathHierarchyTokenizer()
      {
        Delimiter = new char?(CommonConstants.DirectorySeparatorCharacter)
      };
      if (indexSettings.Analysis.Normalizers == null)
        indexSettings.Analysis.Normalizers = (INormalizers) new Normalizers();
      indexSettings.Analysis.Normalizers.Add("lowerCaseNormalizer", (INormalizer) new CustomNormalizer()
      {
        Filter = (IEnumerable<string>) new List<string>()
        {
          "lowercase"
        }
      });
      string configValue = executionContext.RequestContext.GetConfigValue<string>(sortingConfigKey);
      string[] source = (string[]) null;
      if (!string.IsNullOrWhiteSpace(configValue))
      {
        source = configValue.Split(',');
        if (((IEnumerable<string>) source).ToList<string>().Except<string>((IEnumerable<string>) NoPayloadContractUtils.s_sortingFields).Any<string>())
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} has unsupported list of fields to sort on.", (object) sortingConfigKey)));
      }
      if (source != null)
      {
        IndexSortOrder[] indexSortOrderArray = new IndexSortOrder[source.Length];
        for (int index = 0; index < source.Length; ++index)
          indexSortOrderArray[index] = IndexSortOrder.Ascending;
        indexSettings.Sorting = (ISortingSettings) new SortingSettings()
        {
          Fields = (Fields) source,
          Order = indexSortOrderArray
        };
      }
      return indexSettings;
    }

    internal string CreateCodeElementQueryString(
      string tokenKind,
      string tokenValue,
      string rewriteMethod = "top_terms_boost_100")
    {
      string elementFilterType = this.GetSearchFieldForCodeElementFilterType(tokenKind);
      if (tokenValue.ContainsWhitespaceOrSpecialCharacters())
        return ElasticsearchQueryBuilder.BuildMatchPhraseQueryWithContentAnalyzer(elementFilterType, tokenValue);
      return tokenValue.Contains("?") || tokenValue.Contains("*") ? ElasticsearchQueryBuilder.BuildWildcardQuery(elementFilterType, ElasticsearchQueryBuilder.NormalizeBackslashAndDoubeQuote(tokenValue), rewriteMethod) : ElasticsearchQueryBuilder.BuildTermQuery(elementFilterType, tokenValue);
    }

    internal string GetSearchFieldForCodeElementFilterType(string codeElementType)
    {
      string elementFilterType;
      if (NoPayloadContractUtils.CodeTermExpresstionType2FieldName.TryGetValue(codeElementType, out elementFilterType))
        return elementFilterType;
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} isn't expected as a valid code element filter.", (object) codeElementType)));
    }

    internal List<string> AddTermQueryOnCEFieldWithBoost(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      List<string> stringList = new List<string>();
      string configValue1 = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/CodeElementFieldsToApplyScoringInNoPayloadMapping");
      HashSet<string> stringSet = NoPayloadContractUtils.s_codeElementFields;
      if (!string.IsNullOrWhiteSpace(configValue1))
        stringSet = new HashSet<string>((IEnumerable<string>) configValue1.Split(','));
      bool configValue2 = requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/UseFunctionScoreOnCodeElementsToApplyScoringInNoPayloadMapping", TeamFoundationHostType.Deployment, true);
      foreach (KeyValuePair<string, double> keyValuePair in (IEnumerable<KeyValuePair<string, double>>) NoPayloadContractUtils.s_codeElementToBoostValueMapping)
      {
        if (stringSet.Contains(keyValuePair.Key))
        {
          string str = configValue2 ? CodeFileContract.ConvertToFunctionScoreQueryString(termExpression, keyValuePair.Key, keyValuePair.Value) : ElasticsearchQueryBuilder.BuildTermQueryWithBoost(keyValuePair.Key, termExpression.Value, keyValuePair.Value);
          stringList.Add(str);
        }
      }
      return stringList;
    }

    public string GetTheFieldValueAsString(
      string fieldName,
      Dictionary<string, byte[]> parsedDataFields)
    {
      byte[] sourceArray;
      if (!parsedDataFields.TryGetValue(fieldName, out sourceArray) || sourceArray == null)
        return (string) null;
      char[] destinationArray = new char[sourceArray.Length];
      Array.Copy((Array) sourceArray, (Array) destinationArray, sourceArray.Length);
      return new string(destinationArray);
    }

    internal string CorrectFilePathFilter(string filePath)
    {
      if (filePath.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && filePath.EndsWith("\"", StringComparison.OrdinalIgnoreCase) && filePath.Length > 1)
        filePath = filePath.Substring(1, filePath.Length - 2);
      return FileAttributes.GetNormalizedFilePathForNoPayloadContract(filePath.Trim()).Trim(CommonConstants.DirectorySeparatorCharacter).ToLowerInvariant();
    }

    internal string CorrectFilePath(string filePath) => FileAttributes.GetNormalizedFilePathForNoPayloadContract(filePath).Trim(CommonConstants.DirectorySeparatorCharacter).ToLowerInvariant();

    internal string GetEmptyParsedContent()
    {
      byte[] sourceArray = new byte[14]
      {
        (byte) 2,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        (byte) 15
      };
      char[] destinationArray = new char[sourceArray.Length];
      Array.Copy((Array) sourceArray, (Array) destinationArray, sourceArray.Length);
      return new string(destinationArray);
    }
  }
}
