// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.JsonConstants
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.Json
{
  internal static class JsonConstants
  {
    internal const string ODataActionsMetadataName = "actions";
    internal const string ODataFunctionsMetadataName = "functions";
    internal const string ODataOperationTitleName = "title";
    internal const string ODataOperationMetadataName = "metadata";
    internal const string ODataOperationTargetName = "target";
    internal const string ODataErrorName = "error";
    internal const string ODataErrorCodeName = "code";
    internal const string ODataErrorMessageName = "message";
    internal const string ODataErrorTargetName = "target";
    internal const string ODataErrorDetailsName = "details";
    internal const string ODataErrorInnerErrorName = "innererror";
    internal const string ODataErrorInnerErrorMessageName = "message";
    internal const string ODataErrorInnerErrorTypeNameName = "type";
    internal const string ODataErrorInnerErrorStackTraceName = "stacktrace";
    internal const string ODataErrorInnerErrorInnerErrorName = "internalexception";
    internal const string ODataDateTimeFormat = "\\/Date({0})\\/";
    internal const string ODataDateTimeOffsetFormat = "\\/Date({0}{1}{2:D4})\\/";
    internal const string ODataDateTimeOffsetPlusSign = "+";
    internal const string ODataServiceDocumentEntitySetsName = "EntitySets";
    internal const string JsonTrueLiteral = "true";
    internal const string JsonFalseLiteral = "false";
    internal const string JsonNullLiteral = "null";
    internal const string StartObjectScope = "{";
    internal const string EndObjectScope = "}";
    internal const string StartArrayScope = "[";
    internal const string EndArrayScope = "]";
    internal const string StartPaddingFunctionScope = "(";
    internal const string EndPaddingFunctionScope = ")";
    internal const string ObjectMemberSeparator = ",";
    internal const string ArrayElementSeparator = ",";
    internal const string NameValueSeparator = ":";
    internal const char QuoteCharacter = '"';
  }
}
