// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.QueryPackageNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct QueryPackageNames
  {
    public const string QueryAsOf = "AsOf";
    public const string QueryEver = "Ever";
    public const string GroupOperator = "GroupOperator";
    public const string OperatorAnd = "and";
    public const string OperatorOr = "or";
    public const string OperatorAndNot = "andnot";
    public const string OperatorOrNot = "ornot";
    public const string Expression = "Expression";
    public const string Group = "Group";
    public const string Operator = "Operator";
    public const string FieldType = "FieldType";
    public const string ValueTypeString = "String";
    public const string ValueTypeNumber = "Number";
    public const string ValueTypeDouble = "Double";
    public const string ValueTypeDateTime = "DateTime";
    public const string ValueTypeBoolean = "Boolean";
    public const string ValueTypeGuid = "Guid";
    public const string ValueTypeColumn = "Column";
    public const string ExpandConstant = "ExpandConstant";
    public const string ExpressionOperatorEquals = "equals";
    public const string ExpressionOperatorNotEquals = "notequals";
    public const string ExpressionOperatorLess = "less";
    public const string ExpressionOperatorLessEquals = "equalsless";
    public const string ExpressionOperatorGreater = "greater";
    public const string ExpressionOperatorGreaterEquals = "equalsgreater";
    public const string ExpressionOperatorEver = "ever";
    public const string ExpressionOperatorContains = "contains";
    public const string ExpressionOperatorNotContains = "notcontains";
    public const string ExpressionOperatorContainsWords = "containswords";
    public const string ExpressionOperatorNotContainsWords = "notcontainswords";
    public const string ExpressionOperatorUnder = "under";
    public const string ExpressionOperatorNotUnder = "notunder";
    public const string ExpressionOperatorEverContains = "evercontains";
    public const string ExpressionOperatorNeverContains = "nevercontains";
    public const string ExpressionOperatorEverContainsWords = "evercontainswords";
    public const string ExpressionOperatorNeverContainsWords = "nevercontainswords";
    public const string ExpressionOperatorFTContains = "ftcontains";
    public const string ExpressionOperatorNotFTContains = "notftcontains";
    public const string ExpressionOperatorEverFTContains = "everftcontains";
    public const string ExpressionOperatorNeverFTContains = "neverftcontains";
    public const string QueryLinksElement = "LinksQuery";
    public const string QueryLinksLeftQueryElement = "LeftQuery";
    public const string QueryLinksLeftQueryPrefix = "lhs";
    public const string QueryLinksLinkQueryElement = "LinkQuery";
    public const string QueryLinksLinkQueryPrefix = "links";
    public const string QueryLinksLinkTypeQueryPrefix = "linktypes";
    public const string QueryLinksRightQueryElement = "RightQuery";
    public const string QueryLinksRightQueryPrefix = "rhs";
    public const string QueryLinksRecursiveAttribute = "RecursionID";
    public const string QueryLinksReturnMatchingParentsAttribute = "ReturnMatchingParents";
    public const string QueryLinksTypeAttribute = "Type";
    public const string QueryLinksMustContainType = "mustcontain";
    public const string QueryLinksMayContainType = "maycontain";
    public const string QueryLinksDoesNotContainType = "doesnotcontain";
    public const int FieldsTypeMaskDataType = 240;
    public const int FieldTypeMaskFlagsAndDataType = 255;
    public const int FieldsTypeMaskSubType = 3840;
    public const int FieldsTypeMaskReadOnly = 1;
    public const int FieldsTypeMaskIgnore = 2;
    public const int FieldsTypeMaskPerson = 8;
    public const int FieldsTypeKeyword = 16;
    public const int FieldsTypeInteger = 32;
    public const int FieldsTypeDateTime = 48;
    public const int FieldsTypeDouble = 240;
    public const int FieldsTypeLongText = 64;
    public const int FieldsTypeFiles = 80;
    public const int FieldsTypeTreeNode = 160;
    public const int FieldsTypeBit = 224;
    public const int FieldsTypeGuid = 208;
    public const int TypeTreePath = 272;
    public const int TypeHistory = 320;
    public const int TypeHtml = 576;
    public const int TypeAttachment = 336;
    public const int TypeUrl = 592;
    public const int TypeBisUri = 850;
    public const int TypeUnc = 1104;
    public const int TypeRelatedLinks = 176;
    public const int TypeTreeNodeName = 528;
    public const int TypeTreeNodeType = 784;
    public const int TypePerson = 24;
    public const int FieldsTypeTreeId = 288;
    public const int SubTypePickList = 1280;
    public const int ExpandConstantFlag = 1;
    public const int UseFieldTypeFlag = 2;
  }
}
