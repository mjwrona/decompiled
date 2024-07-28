// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.ProvisionTags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ProvisionTags
  {
    public static readonly string Root = "WITD";
    public static readonly string WorkItemType = "WORKITEMTYPE";
    public static readonly string Description = "DESCRIPTION";
    public static readonly string GlobalLists = "GLOBALLISTS";
    public static readonly string FieldDefinitions = "FIELDS";
    public static readonly string FieldDefinition = "FIELD";
    public static readonly string Workflow = "WORKFLOW";
    public static readonly string States = "STATES";
    public static readonly string State = "STATE";
    public static readonly string Transitions = "TRANSITIONS";
    public static readonly string Transition = "TRANSITION";
    public static readonly string Reasons = "REASONS";
    public static readonly string Reason = "REASON";
    public static readonly string DefaultReason = "DEFAULTREASON";
    public static readonly string FieldReferences = "FIELDS";
    public static readonly string FieldReference = "FIELD";
    public static readonly string Form = "FORM";
    public static readonly string GlobalWorkflow = "GLOBALWORKFLOW";
    public static readonly string HelpTextRule = "HELPTEXT";
    public static readonly string RequiredRule = "REQUIRED";
    public static readonly string ReadOnlyRule = "READONLY";
    public static readonly string NotSameAsRule = "NOTSAMEAS";
    public static readonly string CopyRule = "COPY";
    public static readonly string DefaultRule = "DEFAULT";
    public static readonly string ServerDefaultRule = "SERVERDEFAULT";
    public static readonly string CannotLoseValueRule = "CANNOTLOSEVALUE";
    public static readonly string AllowExistingValueRule = "ALLOWEXISTINGVALUE";
    public static readonly string EmptyRule = "EMPTY";
    public static readonly string ValidUserRule = "VALIDUSER";
    public static readonly string FrozenRule = "FROZEN";
    public static readonly string MatchRule = "MATCH";
    public static readonly string AllowedValuesRule = "ALLOWEDVALUES";
    public static readonly string SuggestedValuesRule = "SUGGESTEDVALUES";
    public static readonly string ProhibitedValuesRule = "PROHIBITEDVALUES";
    public static readonly string GlobalList = "GLOBALLIST";
    public static readonly string ListItem = "LISTITEM";
    public static readonly string GroupItem = "GROUPITEM";
    public static readonly string UserList = "USERLIST";
    public static readonly string WhenCondition = "WHEN";
    public static readonly string WhenNotCondition = "WHENNOT";
    public static readonly string WhenChangedCondition = "WHENCHANGED";
    public static readonly string WhenNotChangedCondition = "WHENNOTCHANGED";
    public static readonly string Actions = "ACTIONS";
    public static readonly string Action = "ACTION";
    public static readonly string Layout = nameof (Layout);
    public static readonly string TabGroup = nameof (TabGroup);
    public static readonly string Tab = nameof (Tab);
    public static readonly string Group = nameof (Group);
    public static readonly string Column = nameof (Column);
    public static readonly string Control = nameof (Control);
    public static readonly string LinksControlOptions = nameof (LinksControlOptions);
    public static readonly string WorkItemLinkFilters = nameof (WorkItemLinkFilters);
    public static readonly string ExternalLinkFilters = nameof (ExternalLinkFilters);
    public static readonly string WorkItemTypeFilters = nameof (WorkItemTypeFilters);
    public static readonly string LinkColumns = nameof (LinkColumns);
    public static readonly string LinkColumn = nameof (LinkColumn);
    public static readonly string Filter = nameof (Filter);
    public const string WebLayout = "WebLayout";
    public static readonly string Page = nameof (Page);
    public static readonly string Section = nameof (Section);
    public static readonly string WebLayoutLinkFilters = "LinkFilters";
    public static readonly string WebLayoutWorkItemLinkFilter = "WorkItemLinkFilter";
    public static readonly string WebLayoutExternalLinkFilter = "ExternalLinkFilter";
    public static readonly string WebLayoutWorkItemTypeFilters = nameof (WorkItemTypeFilters);
    public static readonly string WebLayoutLinkColumns = "Columns";
    public static readonly string WebLayoutLinkColumn = nameof (Column);
    public static readonly string PageContribution = nameof (PageContribution);
    public static readonly string GroupContribution = nameof (GroupContribution);
    public static readonly string ControlContribution = nameof (ControlContribution);
    public static readonly string ControlContributionInputs = "Inputs";
    public static readonly string ControlContributionInput = "Input";
    public static readonly string Extensions = nameof (Extensions);
    public static readonly string Extension = nameof (Extension);
    public static readonly string SystemControls = nameof (SystemControls);
    public static readonly string Categories = "CATEGORIES";
    public static readonly string Category = "CATEGORY";
    public static readonly string WorkItemTypeReference = "WORKITEMTYPE";
    public static readonly string DefaultWorkItemTypeReference = "DEFAULTWORKITEMTYPE";
    public static readonly string TypePrefix = "witd";
    public static readonly string GlobalListsPrefix = "gl";
    public static readonly string CategoriesPrefix = "cat";
    public static readonly string LinkElement = "Link";
    public static readonly string ParamElement = "Param";
    public static readonly string LabelElement = "LabelText";
    public static readonly string TextElement = "Text";
    public static readonly string ContentElement = "Content";
    public static readonly string WebpageControlOptions = nameof (WebpageControlOptions);
  }
}
