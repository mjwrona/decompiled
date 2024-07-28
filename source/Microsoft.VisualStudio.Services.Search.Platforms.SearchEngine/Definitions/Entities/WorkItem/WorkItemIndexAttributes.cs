// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemIndexAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public static class WorkItemIndexAttributes
  {
    private const string KeywordAnalyzer = "keyword";
    private const string PathTokenizer = "pathTokenizer";
    private const string ContentAnalyzer = "contentAnalyzer";
    private const string ContentTokenizer = "contentTokenizer";
    private const string CustomTokenFilter = "customtokenfilter";
    private const int PositionOffsetGapValue = 100;
    private const string WorkItemIndexRoutingAllocationConstant = "index.routing.allocation.include.entity.workitem";
    private static readonly IProperty s_stringFieldMappingv5;
    private static readonly IProperty s_integerFieldMapping;
    private static readonly IProperty s_integerAsStringFieldMappingv5;
    private static readonly IProperty s_identityFieldMapping;
    private static readonly IProperty s_realFieldMapping;
    private static readonly IProperty s_dateTimeFieldMapping;
    private static readonly IProperty s_pathFieldMappingv5;
    private static readonly IProperty s_htmlFieldMappingv5;

    public static IndexSettings GetWorkItemIndexSettings(
      IVssRequestContext requestContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval,
      int localeId = 0)
    {
      WorkItemIndexAnalyzers itemIndexAnalyzers = WorkItemIndexAnalyzersProvider.GetWorkItemIndexAnalyzers(localeId);
      string[] strArray = new string[2]
      {
        "collectionId",
        "projectId"
      };
      IndexSettings indexSettings1 = new IndexSettings();
      indexSettings1.RefreshInterval = (Time) refreshInterval;
      indexSettings1.NumberOfReplicas = new int?(numReplicas);
      indexSettings1.NumberOfShards = new int?(numPrimaries);
      Analysis analysis = new Analysis();
      analysis.Analyzers = (IAnalyzers) new Analyzers(itemIndexAnalyzers.AnalyzersMap);
      Tokenizers tokenizers = new Tokenizers();
      tokenizers["pathTokenizer"] = (ITokenizer) new PathHierarchyTokenizer()
      {
        Delimiter = new char?('\\')
      };
      analysis.Tokenizers = (ITokenizers) tokenizers;
      analysis.TokenFilters = itemIndexAnalyzers.TokenFilters;
      indexSettings1.Analysis = (IAnalysis) analysis;
      indexSettings1.Sorting = (ISortingSettings) new SortingSettings()
      {
        Fields = (Fields) strArray
      };
      IndexSettings indexSettings2 = indexSettings1;
      indexSettings2.Add("index.mapping.total_fields.limit", (object) 100000);
      WorkItemIndexAttributes.AddPatternTokenizerForNon_EnglishCharactesrs(indexSettings2, requestContext);
      if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        indexSettings2.Add("index.routing.allocation.include.entity.workitem", (object) true);
      return indexSettings2;
    }

    private static void AddPatternTokenizerForNon_EnglishCharactesrs(
      IndexSettings indexSettings,
      IVssRequestContext requestContext)
    {
      indexSettings.Analysis.Analyzers.Add("contentAnalyzer", (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "contentTokenizer",
        Filter = (IEnumerable<string>) new string[1]
        {
          "customtokenfilter"
        }
      });
      indexSettings.Analysis.Tokenizers.Add("contentTokenizer", (ITokenizer) new PatternTokenizer()
      {
        Pattern = "(\\w+)|([^\\w\\s]?)",
        Group = new int?(0)
      });
      indexSettings.Analysis.TokenFilters.Add("customtokenfilter", (ITokenFilter) new LengthTokenFilter()
      {
        Min = new int?(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MinValueForLengthTokenFilter", 1)),
        Max = new int?(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MinValueForLengthTokenFilter", 20000))
      });
    }

    public static ITypeMapping GetWorkItemIndexMappings(
      IVssRequestContext requestContext,
      int indexVersion = 0,
      int localeId = 0)
    {
      TypeMapping itemIndexMappings = new TypeMapping();
      itemIndexMappings.Meta = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["version"] = (object) indexVersion
      };
      itemIndexMappings.DateDetection = new bool?(false);
      TypeMapping typeMapping1 = itemIndexMappings;
      Properties properties1 = new Properties();
      PropertyName key1 = (PropertyName) "item";
      KeywordProperty keywordProperty = new KeywordProperty();
      keywordProperty.Name = (PropertyName) "item";
      keywordProperty.Index = new bool?(false);
      properties1[key1] = (IProperty) keywordProperty;
      properties1[(PropertyName) "contractType"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "collectionId"] = (IProperty) new KeywordProperty();
      PropertyName key2 = (PropertyName) "collectionName";
      TextProperty textProperty1 = new TextProperty();
      textProperty1.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty1.Norms = new bool?(false);
      Properties properties2 = new Properties();
      properties2[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      textProperty1.Fields = (IProperties) properties2;
      properties1[key2] = (IProperty) textProperty1;
      properties1[(PropertyName) "projectId"] = (IProperty) new KeywordProperty();
      PropertyName key3 = (PropertyName) "projectName";
      TextProperty textProperty2 = new TextProperty();
      textProperty2.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty2.Norms = new bool?(false);
      Properties properties3 = new Properties();
      properties3[(PropertyName) "raw"] = (IProperty) new KeywordProperty()
      {
        EagerGlobalOrdinals = new bool?(true)
      };
      textProperty2.Fields = (IProperties) properties3;
      properties1[key3] = (IProperty) textProperty2;
      properties1[(PropertyName) "indexedTimeStamp"] = (IProperty) new DateProperty()
      {
        Format = "epoch_second"
      };
      PropertyName key4 = (PropertyName) "fields";
      ObjectProperty objectProperty = new ObjectProperty();
      Properties properties4 = new Properties();
      properties4[(PropertyName) WorkItemIndexedField.FromWitField("System.ChangedDate", WorkItemContract.FieldType.DateTime).ContractFieldName] = WorkItemIndexAttributes.s_dateTimeFieldMapping;
      objectProperty.Properties = (IProperties) properties4;
      properties1[key4] = (IProperty) objectProperty;
      typeMapping1.Properties = (IProperties) properties1;
      TypeMapping typeMapping2 = itemIndexMappings;
      DynamicTemplateContainer templateContainer1 = new DynamicTemplateContainer();
      DynamicTemplateContainer templateContainer2 = templateContainer1;
      DynamicTemplate dynamicTemplate1 = new DynamicTemplate();
      dynamicTemplate1.PathMatch = WorkItemIndexAttributes.GetPathMatch("fields", WorkItemContract.ContractFieldNames.WorkItemType);
      TextProperty textProperty3 = new TextProperty();
      textProperty3.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty3.PositionIncrementGap = new int?(100);
      textProperty3.Analyzer = "minimallyStemmedFullTextAnalyzer";
      Properties properties5 = new Properties();
      properties5[(PropertyName) "raw"] = (IProperty) new KeywordProperty()
      {
        EagerGlobalOrdinals = new bool?(true)
      };
      textProperty3.Fields = (IProperties) properties5;
      dynamicTemplate1.Mapping = (IProperty) textProperty3;
      templateContainer2["workItemType"] = (IDynamicTemplate) dynamicTemplate1;
      DynamicTemplateContainer templateContainer3 = templateContainer1;
      DynamicTemplate dynamicTemplate2 = new DynamicTemplate();
      dynamicTemplate2.PathMatch = WorkItemIndexAttributes.GetPathMatch("fields", WorkItemContract.ContractFieldNames.State);
      TextProperty textProperty4 = new TextProperty();
      textProperty4.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty4.Analyzer = "minimallyStemmedFullTextAnalyzer";
      textProperty4.PositionIncrementGap = new int?(100);
      Properties properties6 = new Properties();
      properties6[(PropertyName) "raw"] = (IProperty) new KeywordProperty()
      {
        EagerGlobalOrdinals = new bool?(true)
      };
      textProperty4.Fields = (IProperties) properties6;
      dynamicTemplate2.Mapping = (IProperty) textProperty4;
      templateContainer3["workItemState"] = (IDynamicTemplate) dynamicTemplate2;
      DynamicTemplateContainer templateContainer4 = templateContainer1;
      DynamicTemplate dynamicTemplate3 = new DynamicTemplate();
      dynamicTemplate3.PathMatch = WorkItemIndexAttributes.GetPathMatch("fields", WorkItemContract.ContractFieldNames.AssignedToIdentity);
      TextProperty textProperty5 = new TextProperty();
      textProperty5.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty5.PositionIncrementGap = new int?(100);
      textProperty5.Analyzer = "unstemmedFullTextAnalyzer";
      Properties properties7 = new Properties();
      properties7[(PropertyName) "raw"] = (IProperty) new KeywordProperty()
      {
        EagerGlobalOrdinals = new bool?(true)
      };
      textProperty5.Fields = (IProperties) properties7;
      dynamicTemplate3.Mapping = (IProperty) textProperty5;
      templateContainer4["workItemAssignedToIdentity"] = (IDynamicTemplate) dynamicTemplate3;
      DynamicTemplateContainer templateContainer5 = templateContainer1;
      DynamicTemplate dynamicTemplate4 = new DynamicTemplate();
      dynamicTemplate4.PathMatch = WorkItemIndexAttributes.GetPathMatch("fields", WorkItemContract.ContractFieldNames.AssignedToName);
      TextProperty textProperty6 = new TextProperty();
      textProperty6.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty6.PositionIncrementGap = new int?(100);
      textProperty6.Analyzer = "unstemmedFullTextAnalyzer";
      Properties properties8 = new Properties();
      properties8[(PropertyName) "raw"] = (IProperty) new KeywordProperty()
      {
        EagerGlobalOrdinals = new bool?(true)
      };
      properties8[(PropertyName) "pattern"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "contentAnalyzer"
      };
      textProperty6.Fields = (IProperties) properties8;
      dynamicTemplate4.Mapping = (IProperty) textProperty6;
      templateContainer5["workItemAssignedToName"] = (IDynamicTemplate) dynamicTemplate4;
      DynamicTemplateContainer templateContainer6 = templateContainer1;
      DynamicTemplate dynamicTemplate5 = new DynamicTemplate();
      dynamicTemplate5.PathMatch = WorkItemIndexAttributes.GetPathMatch("fields", WorkItemContract.ContractFieldNames.Tags);
      TextProperty textProperty7 = new TextProperty();
      textProperty7.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty7.PositionIncrementGap = new int?(100);
      textProperty7.Analyzer = "minimallyStemmedFullTextAnalyzer";
      Properties properties9 = new Properties();
      properties9[(PropertyName) "stemmed"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "lightStemmedFullTextAnalyzer"
      };
      properties9[(PropertyName) "pattern"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "contentAnalyzer"
      };
      textProperty7.Fields = (IProperties) properties9;
      dynamicTemplate5.Mapping = (IProperty) textProperty7;
      templateContainer6["workItemTags"] = (IDynamicTemplate) dynamicTemplate5;
      DynamicTemplateContainer templateContainer7 = templateContainer1;
      DynamicTemplate dynamicTemplate6 = new DynamicTemplate();
      dynamicTemplate6.PathMatch = WorkItemIndexAttributes.GetPathMatch("fields", WorkItemContract.ContractFieldNames.Title);
      TextProperty textProperty8 = new TextProperty();
      textProperty8.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty8.PositionIncrementGap = new int?(100);
      textProperty8.Analyzer = "minimallyStemmedFullTextAnalyzer";
      Properties properties10 = new Properties();
      properties10[(PropertyName) "stemmed"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "lightStemmedFullTextAnalyzer"
      };
      properties10[(PropertyName) "pattern"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "contentAnalyzer"
      };
      textProperty8.Fields = (IProperties) properties10;
      dynamicTemplate6.Mapping = (IProperty) textProperty8;
      templateContainer7["title"] = (IDynamicTemplate) dynamicTemplate6;
      templateContainer1["NonAnalyzedTextFields"] = (IDynamicTemplate) new DynamicTemplate()
      {
        PathMatch = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.*", (object) "nonAnalyzedFields"),
        Mapping = (requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? (IProperty) IcuCollationKeywordPropertyProvider.GetIcuCollationKeywordProperty(localeId) : (IProperty) new KeywordProperty())
      };
      string platformFieldTypeString1 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.String];
      templateContainer1[platformFieldTypeString1] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.String).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_stringFieldMappingv5
      };
      string platformFieldTypeString2 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.IntegerAsString];
      templateContainer1[platformFieldTypeString2] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.IntegerAsString).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_integerAsStringFieldMappingv5
      };
      string platformFieldTypeString3 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Identity];
      templateContainer1[platformFieldTypeString3] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Identity).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_identityFieldMapping
      };
      string platformFieldTypeString4 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Name];
      templateContainer1[platformFieldTypeString4] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Name).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_identityFieldMapping
      };
      string platformFieldTypeString5 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Integer];
      templateContainer1[platformFieldTypeString5] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Integer).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_integerFieldMapping
      };
      string platformFieldTypeString6 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Real];
      templateContainer1[platformFieldTypeString6] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Real).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_realFieldMapping
      };
      string platformFieldTypeString7 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.DateTime];
      templateContainer1[platformFieldTypeString7] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.DateTime).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_dateTimeFieldMapping
      };
      string platformFieldTypeString8 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Path];
      templateContainer1[platformFieldTypeString8] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Path).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_pathFieldMappingv5
      };
      string platformFieldTypeString9 = WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Html];
      templateContainer1[platformFieldTypeString9] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Html).ContractFieldName,
        Mapping = WorkItemIndexAttributes.s_htmlFieldMappingv5
      };
      string key5 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.String]));
      templateContainer1[key5] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.String).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_stringFieldMappingv5
      };
      string key6 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Integer]));
      templateContainer1[key6] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Integer).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_integerFieldMapping
      };
      string key7 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Real]));
      templateContainer1[key7] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Real).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_realFieldMapping
      };
      string key8 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.DateTime]));
      templateContainer1[key8] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.DateTime).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_dateTimeFieldMapping
      };
      string key9 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Path]));
      templateContainer1[key9] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Path).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_pathFieldMappingv5
      };
      string key10 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Html]));
      templateContainer1[key10] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Html).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_htmlFieldMappingv5
      };
      string key11 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Name]));
      templateContainer1[key11] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Name).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_identityFieldMapping
      };
      string key12 = FormattableString.Invariant(FormattableStringFactory.Create("composite_{0}", (object) WorkItemContract.PlatformFieldTypeStringMap[WorkItemContract.FieldType.Identity]));
      templateContainer1[key12] = (IDynamicTemplate) new DynamicTemplate()
      {
        Match = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Identity).CompositeContractFieldName,
        Mapping = WorkItemIndexAttributes.s_identityFieldMapping
      };
      DynamicTemplateContainer templateContainer8 = templateContainer1;
      typeMapping2.DynamicTemplates = (IDynamicTemplateContainer) templateContainer8;
      return (ITypeMapping) itemIndexMappings;
    }

    private static string GetPathMatch(string platformFieldName, string fieldName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) platformFieldName, (object) fieldName);

    static WorkItemIndexAttributes()
    {
      TextProperty textProperty1 = new TextProperty();
      textProperty1.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty1.Analyzer = "minimallyStemmedFullTextAnalyzer";
      textProperty1.PositionIncrementGap = new int?(100);
      Properties properties1 = new Properties();
      properties1[(PropertyName) "stemmed"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "lightStemmedFullTextAnalyzer"
      };
      properties1[(PropertyName) "pattern"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "contentAnalyzer"
      };
      textProperty1.Fields = (IProperties) properties1;
      WorkItemIndexAttributes.s_stringFieldMappingv5 = (IProperty) textProperty1;
      WorkItemIndexAttributes.s_integerFieldMapping = (IProperty) new NumberProperty(NumberType.Integer);
      WorkItemIndexAttributes.s_integerAsStringFieldMappingv5 = (IProperty) new KeywordProperty();
      WorkItemIndexAttributes.s_identityFieldMapping = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100),
        Analyzer = "unstemmedFullTextAnalyzer"
      };
      WorkItemIndexAttributes.s_realFieldMapping = (IProperty) new NumberProperty(NumberType.Float);
      WorkItemIndexAttributes.s_dateTimeFieldMapping = (IProperty) new DateProperty()
      {
        Format = "date_optional_time"
      };
      TextProperty textProperty2 = new TextProperty();
      textProperty2.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty2.Analyzer = "unstemmedFullTextAnalyzer";
      textProperty2.PositionIncrementGap = new int?(100);
      Properties properties2 = new Properties();
      properties2[(PropertyName) "lower"] = (IProperty) new TextProperty()
      {
        Analyzer = "pathAnalyzer",
        SearchAnalyzer = "keyword",
        Norms = new bool?(false)
      };
      textProperty2.Fields = (IProperties) properties2;
      WorkItemIndexAttributes.s_pathFieldMappingv5 = (IProperty) textProperty2;
      TextProperty textProperty3 = new TextProperty();
      textProperty3.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty3.Analyzer = "minimallyStemmedFullTextAnalyzer";
      textProperty3.PositionIncrementGap = new int?(100);
      Properties properties3 = new Properties();
      properties3[(PropertyName) "stemmed"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "lightStemmedFullTextAnalyzer"
      };
      properties3[(PropertyName) "pattern"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "contentAnalyzer"
      };
      textProperty3.Fields = (IProperties) properties3;
      WorkItemIndexAttributes.s_htmlFieldMappingv5 = (IProperty) textProperty3;
    }

    public static class FieldNameSuffix
    {
      public const string Raw = "raw";
      public const string Lower = "lower";
      public const string Stemmed = "stemmed";
      public const string Pattern = "pattern";
    }
  }
}
