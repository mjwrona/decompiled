// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemProcessValidatorRegistryKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WorkItemProcessValidatorRegistryKey
  {
    public const string Root = "/Service/WorkItemTracking/Settings/Validator/";
    public const string All = "/Service/WorkItemTracking/Settings/Validator/*";
    public const string GlobalListsPermitted = "/Service/WorkItemTracking/Settings/Validator/GlobalListsPermitted";
    public const string CustomLinkTypesPermitted = "/Service/WorkItemTracking/Settings/Validator/CustomLinkTypesPermitted";
    public const string AllRulesPermitted = "/Service/WorkItemTracking/Settings/Validator/AllRulesPermitted";
    public const string NoFieldRuleRestrictions = "/Service/WorkItemTracking/Settings/Validator/NoFieldRuleRestrictions";
    public const string CustomControlsPermitted = "/Service/WorkItemTracking/Settings/Validator/CustomControlsPermitted";
    public const string IdentitiesWithoutSyncNameChangesPermitted = "/Service/WorkItemTracking/Settings/Validator/IdentitiesWithoutSyncNameChangesPermitted";
    public const string WorkItemTypesPerProcess = "/Service/WorkItemTracking/Settings/Validator/WorkItemTypesPerProcess";
    public const string MaxCustomWorkItemTypesPerProcess = "/Service/WorkItemTracking/Settings/Validator/MaxCustomWorkItemTypesPerProcess";
    public const string MaxFieldsPerCollection = "/Service/WorkItemTracking/Settings/Validator/MaxFieldsPerCollection";
    public const string MaxFieldsPerProcessTemplate = "/Service/WorkItemTracking/Settings/Validator/MaxFieldsPerProcessTemplate";
    public const string CategoriesPerProcess = "/Service/WorkItemTracking/Settings/Validator/CategoriesPerProcess";
    public const string GlobalListCountPerProcess = "/Service/WorkItemTracking/Settings/Validator/GlobalListCountPerProcess";
    public const string GlobaListItemCountPerProcess = "/Service/WorkItemTracking/Settings/Validator/GlobaListItemCountPerProcess";
    public const string CustomLinkTypes = "/Service/WorkItemTracking/Settings/Validator/CustomLinkTypes";
    public const string StatesPerWorkItemType = "/Service/WorkItemTracking/Settings/Validator/StatesPerWorkItemType";
    public const string ValuesInSingleRuleValuesList = "/Service/WorkItemTracking/Settings/Validator/ValuesInSingleRuleValuesList";
    public const string SyncNameChangesFieldsPerType = "/Service/WorkItemTracking/Settings/Validator/SyncNameChangesFieldsPerType";
    public const string FieldsInWorkItemType = "/Service/WorkItemTracking/Settings/Validator/FieldsInWorkItemType";
    public const string MaxCustomFieldsPerWorkItemType = "/Service/WorkItemTracking/Settings/Validator/MaxCustomFieldsPerWorkItemType";
    public const string RulesPerWorkItemType = "/Service/WorkItemTracking/Settings/Validator/RulesPerWorkItemType";
    public const string MaxPickListItemsPerList = "/Service/WorkItemTracking/Settings/Validator/MaxPickListItemsPerList";
    public const string MaxPickListItemLength = "/Service/WorkItemTracking/Settings/Validator/MaxPickListItemLength";
    public const string MaxCustomStatesPerWorkItemType = "/Service/WorkItemTracking/Settings/Validator/MaxCustomStatesPerWorkItemType";
    public const string MaxPortfolioBacklogLevels = "/Service/WorkItemTracking/Settings/Validator/MaxPortfolioBacklogLevels";
    public const string MaxPickListsPerCollection = "/Service/WorkItemTracking/Settings/Validator/MaxPickListsPerCollection";
  }
}
