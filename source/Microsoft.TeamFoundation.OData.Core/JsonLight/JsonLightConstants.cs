// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.JsonLightConstants
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.JsonLight
{
  internal static class JsonLightConstants
  {
    internal const string ODataAnnotationNamespacePrefix = "odata.";
    internal const char ODataPropertyAnnotationSeparatorChar = '@';
    internal const string ODataNullPropertyName = "null";
    internal const string ODataNullAnnotationTrueValue = "true";
    internal const string ODataValuePropertyName = "value";
    internal const string ODataErrorPropertyName = "error";
    internal const string ODataSourcePropertyName = "source";
    internal const string ODataTargetPropertyName = "target";
    internal const string ODataRelationshipPropertyName = "relationship";
    internal const string ODataIdPropertyName = "id";
    internal const string ODataDeltaPropertyName = "delta";
    internal const string ODataReasonPropertyName = "reason";
    internal const string ODataReasonChangedValue = "changed";
    internal const string ODataReasonDeletedValue = "deleted";
    internal const string ODataServiceDocumentElementUrlName = "url";
    internal const string ODataServiceDocumentElementTitle = "title";
    internal const string ODataServiceDocumentElementKind = "kind";
    internal const string ODataServiceDocumentElementName = "name";
    internal const string ContextUriSelectQueryOptionName = "$select";
    internal const char ContextUriQueryOptionValueSeparator = '=';
    internal const char ContextUriQueryOptionSeparator = '&';
    internal const char FunctionParameterStart = '(';
    internal const char FunctionParameterEnd = ')';
    internal const string FunctionParameterSeparator = ",";
    internal const char FunctionParameterSeparatorChar = ',';
    internal const string FunctionParameterAssignment = "=@";
    internal const string ServiceDocumentSingletonKindName = "Singleton";
    internal const string ServiceDocumentFunctionImportKindName = "FunctionImport";
    internal const string ServiceDocumentEntitySetKindName = "EntitySet";
    internal const string SimplifiedODataContextPropertyName = "@context";
    internal const string SimplifiedODataIdPropertyName = "@id";
    internal const string SimplifiedODataTypePropertyName = "@type";
    internal const string SimplifiedODataRemovedPropertyName = "@removed";
  }
}
