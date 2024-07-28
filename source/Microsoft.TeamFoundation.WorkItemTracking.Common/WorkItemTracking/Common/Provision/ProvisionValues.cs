// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.ProvisionValues
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ProvisionValues
  {
    public static readonly string TypesNamespace = "http://schemas.microsoft.com/VisualStudio/2008/workitemtracking/typedef";
    public static readonly string GlobalListsNamespace = "http://schemas.microsoft.com/VisualStudio/2005/workitemtracking/globallists";
    public static readonly string CategoriesNamespace = "http://schemas.microsoft.com/VisualStudio/2008/workitemtracking/categories";
    public static readonly string Application = "Work item type editor";
    public static readonly string AppVersion = "1.0";
    public static readonly string SourceValue = "value";
    public static readonly string SourceField = "field";
    public static readonly string SourceClock = "clock";
    public static readonly string SourceCurrentUser = "currentuser";
    public static readonly string SourceGuid = "guid";
    public static readonly string LayoutTargetNewForm = "NewForm";
    public static readonly string LayoutTargetWeb = "Web";
    public static readonly string FieldTypeString = "String";
    public static readonly string FieldTypeInteger = "Integer";
    public static readonly string FieldTypeDateTime = "DateTime";
    public static readonly string FieldTypePlainText = "PlainText";
    public static readonly string FieldTypeHtml = "HTML";
    public static readonly string FieldTypeDouble = "Double";
    public static readonly string FieldTypeTreePath = "TreePath";
    public static readonly string FieldTypeHistory = "History";
    public static readonly string FieldTypeGuid = "GUID";
    public static readonly string FieldTypeBoolean = "Boolean";
    public static readonly string FieldTypeIdentity = "Identity";
    public static readonly string ReportingMeasure = "measure";
    public static readonly string ReportingDimension = "dimension";
    public static readonly string ReportingDetail = "detail";
    public static readonly string FormulaSum = "sum";
    public static readonly string FormulaCount = "count";
    public static readonly string FormulaDistinctCount = "distinctcount";
    public static readonly string FormulaAvg = "avg";
    public static readonly string FormulaMin = "min";
    public static readonly string FormulaMax = "max";
    public static readonly string ExcludeGroups = "excludegroups";
    public static readonly string ConstScopeInstance = "[team foundation]\\";
    public static readonly string ConstScopeGlobal = "[global]\\";
    public static readonly string ConstScopeProject = "[project]\\";
    public static readonly string ControlFontSizeLarge = "large";
    public static readonly string ControlLabelPositionTop = "Top";
    public static readonly string ControlLabelPositionLeft = "Left";
    public static readonly string ColumnTargetDescription = "System.Links.Description";
    public static readonly string ColumnLinkComment = "System.Links.Comment";
    public static readonly string ColumnLinkType = "System.Links.LinkType";
    public static readonly string Include = "include";
    public static readonly string Exclude = "exclude";
    public static readonly string IncludeAll = "includeAll";
    public static readonly string ExcludeAll = "excludeAll";
    public static readonly string ForwardName = "forwardname";
    public static readonly string ReverseName = "reversename";
    public static readonly string Project = "project";
    public static readonly string All = "all";
    public static readonly string ParamTypeValueOriginal = "Original";
    public static readonly string ParamMacroProcessGuidanceUrl = "@ProcessGuidance";
    public static readonly string ParamMacroPortal = "@PortalPage";
    public static readonly string ParamMacroReportManagerUrl = "@ReportManagerUrl";
    public static readonly string ParamMacroReportServiceSiteUrl = "@ReportServiceSiteUrl";
    public static readonly string ParamMacroMe = "@Me";
    public static readonly string WebLayoutIncludeAllLinkTypes = "System.Links.IncludeAll";
    public static readonly string ForwardNameSuffix = "-Forward";
    public static readonly string ReverseNameSuffix = "-Reverse";
  }
}
