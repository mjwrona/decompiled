// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.ProvisionAttributes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ProvisionAttributes
  {
    public static readonly string DefaultNamespace = "xmlns";
    public static readonly string FriendlyName = "name";
    public static readonly string ReferenceName = "refname";
    public static readonly string RenameSafe = "syncnamechanges";
    public static readonly string WorkItemTypeName = "name";
    public static readonly string FieldName = "name";
    public static readonly string FieldReferenceName = "refname";
    public static readonly string GlobalListName = "name";
    public static readonly string GroupReference = "group";
    public static readonly string Application = "application";
    public static readonly string AppVersion = "version";
    public static readonly string ListItemValue = "value";
    public static readonly string FieldType = "type";
    public static readonly string FieldValue = "value";
    public static readonly string OriginalState = "from";
    public static readonly string NewState = "to";
    public static readonly string RuleValue = "value";
    public static readonly string RuleSource = "from";
    public static readonly string FieldReference = "field";
    public static readonly string StateValue = "value";
    public static readonly string Reportable = "reportable";
    public static readonly string FieldReportingName = "reportingname";
    public static readonly string FieldReportingReferenceName = "reportingrefname";
    public static readonly string Formula = "formula";
    public static readonly string ExpandItems = "expanditems";
    public static readonly string FilterItems = "filteritems";
    public static readonly string MatchPattern = "pattern";
    public static readonly string RuleApplyTo = "for";
    public static readonly string RuleDontApplyTo = "not";
    public static readonly string TransitionAllowedFor = "for";
    public static readonly string TransitionProhibitedFor = "not";
    public static readonly string Margin = nameof (Margin);
    public static readonly string Label = nameof (Label);
    public static readonly string Visible = nameof (Visible);
    public static readonly string Height = nameof (Height);
    public static readonly string LayoutTarget = "Target";
    public static readonly string LayoutHideReadOnlyEmptyFields = "HideReadOnlyEmptyFields";
    public static readonly string LayoutShowEmptyReadOnlyFields = "ShowEmptyReadOnlyFields";
    public static readonly string LayoutHideControlBorders = "HideControlBorders";
    public static readonly string ColumnPercentWidth = "PercentWidth";
    public static readonly string ColumnFixedWidth = "FixedWidth";
    public static readonly string ControlFieldName = nameof (FieldName);
    public static readonly string ControlType = "Type";
    public static readonly string ControlName = "Name";
    public static readonly string ControlReadOnly = "ReadOnly";
    public static readonly string ControlLabelPosition = "LabelPosition";
    public static readonly string ControlEmptyText = "EmptyText";
    public static readonly string LinkAttribute = nameof (LinkAttribute);
    public static readonly string LinkRefName = "RefName";
    public static readonly string LinkType = nameof (LinkType);
    public static readonly string FilterOn = nameof (FilterOn);
    public static readonly string FilterType = nameof (FilterType);
    public static readonly string WorkItemType = nameof (WorkItemType);
    public static readonly string Scope = nameof (Scope);
    public static readonly string ContributionId = "Id";
    public static readonly string InstanceId = "instanceid";
    public static readonly string ControlFontSize = nameof (ControlFontSize);
    public static readonly string ExtensionId = "Id";
    public static readonly string ControlHeight = nameof (Height);
    public static readonly string InputId = "Id";
    public static readonly string InputValue = "Value";
    public static readonly string PageLayoutMode = "LayoutMode";
    public static readonly string WebLayoutElementId = "Id";
    public static readonly string WebLayoutLinkType = "Type";
    public static readonly string WebLayoutColumnName = "Name";
    public static readonly string WebLayoutWorkItemTypeFiltersScope = "WorkItemTypeFiltersScope";
    public static readonly string UrlRoot = nameof (UrlRoot);
    public static readonly string UrlPath = nameof (UrlPath);
    public static readonly string ParamValue = "Value";
    public static readonly string ParamType = "Type";
    public static readonly string TargetName = nameof (TargetName);
    public static readonly string ReloadOnParamChange = nameof (ReloadOnParamChange);
    public static readonly string AllowScript = nameof (AllowScript);
    public static readonly string Index = nameof (Index);
    public static readonly string ControlReplacesFieldReferenceName = "Replaces";
  }
}
