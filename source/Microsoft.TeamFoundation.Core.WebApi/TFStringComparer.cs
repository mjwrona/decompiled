// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TFStringComparer
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  public class TFStringComparer : VssStringComparer
  {
    private TFStringComparer(StringComparison stringComparison)
      : base(stringComparison)
    {
    }

    public static VssStringComparer AnnotationName => VssStringComparer.s_ordinal;

    public static VssStringComparer BacklogPluralName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer CaseSensitiveFileName => VssStringComparer.s_ordinal;

    public static VssStringComparer ChangeType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ChangeTypeUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer CheckinNoteName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CheckinNoteNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer CommandLineOptionName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CommandLineOptionValue => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer Comment => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer CommentUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer ConflictDescription => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer ConflictDescriptionUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer ConflictType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ConflictTypeUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer FileType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer InstanceId => VssStringComparer.s_ordinal;

    public static VssStringComparer LabelName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer LabelNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer ObjectId => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer PermissionName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer PolicyType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer QuotaName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ShelvesetName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ShelvesetNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer SoapExceptionCode => VssStringComparer.s_ordinal;

    public static VssStringComparer SubscriptionFieldName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer SubscriptionTag => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TeamProjectCollectionName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TeamProjectCollectionNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer TeamProjectName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TeamProjectGuid => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TeamProjectNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer TeamProjectPropertyName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TeamNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer TFSName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TFSNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer VersionControlPath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemQueryName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer WorkItemQueryText => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemStateName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer WorkItemActionName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemTypeName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer BehaviorName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer WorkItemIdentityName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer WorkspaceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkspaceNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer WorkItemFieldType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemUpdate => VssStringComparer.s_ordinal;

    public static VssStringComparer BuildAgent => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BuildAgentUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer BuildControllerName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BuildControllerNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer BuildName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BuildNumber => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BuildStep => VssStringComparer.s_ordinal;

    public static VssStringComparer BuildPlatformFlavor => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BuildTargetName => VssStringComparer.s_ordinal;

    public static VssStringComparer BuildTaskName => VssStringComparer.s_ordinal;

    public static VssStringComparer BuildType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BuildTypeUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer BuildQuality => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BuildQualityUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer InformationType => VssStringComparer.s_ordinal;

    public static VssStringComparer TestCategoryName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TestCategoryNameUI => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer DataType => VssStringComparer.s_ordinal;

    public static VssStringComparer StructureType => VssStringComparer.s_ordinal;

    public static VssStringComparer QueryOperator => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CssActions => VssStringComparer.s_ordinal;

    public static VssStringComparer UpdateAction => VssStringComparer.s_ordinal;

    public static VssStringComparer WorkItemArtifactType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AutoCompleteComboBox => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer WorkItemCategoryName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemCategoryReferenceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BacklogLevelId => TFStringComparer.WorkItemCategoryReferenceName;

    public static VssStringComparer LinkComment => VssStringComparer.s_ordinal;

    public static VssStringComparer ControlType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ControlClassName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DefaultValue => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer IterationName => VssStringComparer.s_currentCulture;

    public static VssStringComparer ListViewItem => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer NodeSpec => VssStringComparer.s_ordinal;

    public static VssStringComparer CssNodeName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer FavoritesNodePath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ProjectCreationContextData => VssStringComparer.s_ordinal;

    public static VssStringComparer TemplateName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WssListElement => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WssTemplate => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WssFilePath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer OfficeVersions => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CssProjectPropertyName => VssStringComparer.s_ordinal;

    public static VssStringComparer FactName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ParserTag => VssStringComparer.s_ordinal;

    public static VssStringComparer StringUtilityComparison => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BoolAppSetting => VssStringComparer.s_ordinal;

    public static VssStringComparer IdentityData => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ProjectString => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CssStructureType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CssXmlNodeName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CssTreeNodeName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer CssXmlNodeInfoUri => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CssTreePathName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer RegistrationEntryType => VssStringComparer.s_ordinal;

    public static VssStringComparer DataGridId => VssStringComparer.s_currentCulture;

    public static VssStringComparer ArtiFactUrl => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AclPermissionEntry => VssStringComparer.s_currentCulture;

    public static VssStringComparer DirectoryEntrySchemaClassName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer RoleMemberName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BaseHierarchyNodeName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer BaseHierarchyNodePath => VssStringComparer.s_ordinal;

    public static VssStringComparer BaseUIHierarchyNodeName => VssStringComparer.s_ordinal;

    public static VssStringComparer CanonicalName => VssStringComparer.s_ordinal;

    public static VssStringComparer CreateDSArg => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer CatalogItemName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer Verb => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ProgId => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer OfficeWorkItemId => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer TfsProtocolUriComponent => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer StoryboardStencilReferenceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer StoryboardLinkPath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AllowedValue => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ProjectUri => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ELeadListObjectName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer HashCode => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemId => VssStringComparer.s_ordinal;

    public static VssStringComparer WorkItemRev => VssStringComparer.s_ordinal;

    public static VssStringComparer WorkItemType => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExcelValidationData => VssStringComparer.s_ordinal;

    public static VssStringComparer ExcelValidationDataIgnoreCase => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkbookName => VssStringComparer.s_ordinal;

    public static VssStringComparer WorksheetName => VssStringComparer.s_ordinal;

    public static VssStringComparer ExcelListName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExcelColumnName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExcelWorkSheetName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExcelNumberFormat => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExcelBannerText => VssStringComparer.s_ordinal;

    public static VssStringComparer WorkItemFieldReferenceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemTypeletReferenceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemTypeReferenceName => TFStringComparer.WorkItemTypeletReferenceName;

    public static VssStringComparer BehaviorReferenceName => TFStringComparer.WorkItemTypeletReferenceName;

    public static VssStringComparer WorkItemLinkTypeReferenceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WorkItemFieldFriendlyName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer StoredQueryName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer StoredQueryText => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer AttachmentName => VssStringComparer.s_ordinal;

    public static VssStringComparer LinkData => VssStringComparer.s_ordinal;

    public static VssStringComparer CssSprocErrors => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer MSProjectDisplayableObjectName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer MSProjectAssignmentName => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer MSProjectFieldName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer MSProjectCellValue => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer CaseInsensitiveArrayList => VssStringComparer.s_currentCultureIgnoreCase;

    public static VssStringComparer ProjMapArgs => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ProcessName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ProcessReferenceName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer WIConverterFieldRefName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ReportItemPath => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer DiagnosticAreaPathName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BoardColumnName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BoardColumnNameCaseSensitive => VssStringComparer.s_ordinal;

    public static VssStringComparer BoardRowName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer BoardName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ProcessTemplatePluginName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExternalConnectionName => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExternalProviderKey => VssStringComparer.s_ordinalIgnoreCase;

    public static VssStringComparer ExternalRepoId => VssStringComparer.s_currentCulture;

    public static VssStringComparer ExternalRepoName => VssStringComparer.s_ordinalIgnoreCase;
  }
}
