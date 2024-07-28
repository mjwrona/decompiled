// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.IWitProcessTemplateValidatorConfiguration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  public interface IWitProcessTemplateValidatorConfiguration
  {
    IEnumerable<LinkTypeDefinition> SystemLinkTypeWhiteList { get; }

    IEnumerable<string> SystemControlWhiteList { get; }

    IEnumerable<string> BannedRules { get; }

    IEnumerable<string> SystemFieldWithSyncNameChanges { get; }

    IEnumerable<string> RulesProhibitedFields { get; }

    IEnumerable<FieldRuleRestriction> FieldRuleRestrictions { get; }

    IEnumerable<FeatureRequirement> FeatureRequirements { get; }

    IEqualityComparer<string> NameComparer { get; }

    IEnumerable<ProcessFieldDefinition> DefaultSystemFields { get; }

    int MaxWorkItemTypesPerProcess { get; }

    int MaxCustomWorkItemTypesPerProcess { get; }

    int MaxFieldsPerCollection { get; }

    int MaxFieldsPerProcessTemplate { get; }

    int MaxCategoriesPerProcess { get; }

    int MaxGlobalListCountPerProcess { get; }

    int MaxGlobalListItemCountPerProcess { get; }

    int MaxCustomLinkTypes { get; }

    int MaxStatesPerWorkItemType { get; }

    int MaxValuesInSingleRuleValuesList { get; }

    int MaxSyncNameChangesFieldsPerType { get; }

    int MaxFieldsInWorkItemType { get; }

    int MaxCustomFieldsPerWorkItemType { get; }

    int MaxRulesPerWorkItemType { get; }

    int MaxPickListItemsPerList { get; }

    int MaxPickListItemLength { get; }

    int MaxCustomStatesPerWorkItemType { get; }

    int MaxPortfolioBacklogLevels { get; }

    int MaxPickListsPerCollection { get; }

    bool GlobalListsPermitted { get; }

    bool CustomLinkTypesPermitted { get; }

    bool AllRulesPermitted { get; }

    bool NoFieldRuleRestrictions { get; }

    bool CustomControlsPermitted { get; }

    bool IdentitiesWithoutSyncNameChangesPermitted { get; }
  }
}
