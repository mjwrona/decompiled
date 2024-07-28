// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.PlanningToolsFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class PlanningToolsFeature : ProjectFeatureBase, INotifyProjectFeatureProvisioned
  {
    private const string c_sizeFieldRefName = "Microsoft.VSTS.Scheduling.Size";
    private const string c_originalEstimateFieldRefName = "Microsoft.VSTS.Scheduling.OriginalEstimate";

    public PlanningToolsFeature()
      : base(Resources.PlanningToolsFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata) => projectMetadata.GetProcessConfiguration() != null ? ProjectFeatureState.FullyConfigured : ProjectFeatureState.NotConfigured;

    public override void Process(IProjectProvisioningContext context)
    {
      ArgumentUtility.CheckForNull<IProjectProvisioningContext>(context, nameof (context));
      this.EnsureProcessConfigurationSettings(context);
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      this.SetupCategoryStateMapping(context, processConfiguration.RequirementBacklog);
      this.SetupCategoryStateMapping(context, processConfiguration.TaskBacklog);
      if (processConfiguration.BugWorkItems != null)
        this.EnsureCategory(context, processConfiguration.BugWorkItems.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists);
      this.FixCMMIEffortField(context);
    }

    private void SetupCategoryStateMapping(
      IProjectProvisioningContext context,
      BacklogCategoryConfiguration category)
    {
      this.EnsureCategory(context, category.CategoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.Merge, ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists);
      string workItemTypeName = context.GetWorkItemTypeCategory(category.CategoryReferenceName).DefaultWorkItemTypeName;
      WorkItemTypeMetadata workItemType = context.GetWorkItemType(workItemTypeName);
      State[] states = category.States;
      if (states == null)
        return;
      category.States = PlanningToolsFeature.SetupStateMapping(context, workItemType, states);
    }

    private static State[] SetupStateMapping(
      IProjectProvisioningContext context,
      WorkItemTypeMetadata workItem,
      State[] templateStates)
    {
      if (!((IEnumerable<State>) templateStates).Any<State>((Func<State, bool>) (state => state.Type == StateTypeEnum.Proposed)))
        return templateStates;
      string[] array = workItem.States.ToArray<string>();
      if (!((IEnumerable<State>) templateStates).Where<State>((Func<State, bool>) (state => state.Type == StateTypeEnum.Proposed)).Join<State, string, string, string>((IEnumerable<string>) array, (Func<State, string>) (state => state.Value), (Func<string, string>) (witState => witState), (Func<State, string, string>) ((templateState, witState) => witState), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName).Any<string>())
      {
        string initialState = workItem.InitialState;
        templateStates = ((IEnumerable<State>) templateStates).Where<State>((Func<State, bool>) (state => state.Value != initialState)).ToArray<State>();
        ((IEnumerable<State>) templateStates).First<State>((Func<State, bool>) (state => state.Type == StateTypeEnum.Proposed)).Value = initialState;
      }
      return templateStates;
    }

    private void FixCMMIEffortField(IProjectProvisioningContext context)
    {
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      bool changedFieldSetting = false;
      string requirementCategoryReferenceName = processConfiguration.RequirementBacklog.CategoryReferenceName;
      Action<Func<string>, Action<string>> swapFieldSetting = (Action<Func<string>, Action<string>>) ((getFieldSetting, setFieldSetting) =>
      {
        string str = getFieldSetting();
        if (this.FieldExistsOnCategoryWorkItems((IProjectMetadata) context, str, requirementCategoryReferenceName) || !TFStringComparer.WorkItemFieldReferenceName.Equals(str, "Microsoft.VSTS.Scheduling.Size") || !this.FieldExistsOnCategoryWorkItems((IProjectMetadata) context, "Microsoft.VSTS.Scheduling.OriginalEstimate", requirementCategoryReferenceName))
          return;
        setFieldSetting("Microsoft.VSTS.Scheduling.OriginalEstimate");
        changedFieldSetting = true;
      });
      Action<Column[]> action = (Action<Column[]>) (columns =>
      {
        foreach (Column column1 in columns)
        {
          Column column = column1;
          swapFieldSetting((Func<string>) (() => column.FieldName), (Action<string>) (fieldName => column.FieldName = fieldName));
        }
      });
      action(processConfiguration.TaskBacklog.Columns);
      action(processConfiguration.RequirementBacklog.Columns);
      TypeField effortField = ((IEnumerable<TypeField>) processConfiguration.TypeFields).FirstOrDefault<TypeField>((Func<TypeField, bool>) (tf => tf.Type == FieldTypeEnum.Effort));
      if (effortField != null)
        swapFieldSetting((Func<string>) (() => effortField.Name), (Action<string>) (fieldName => effortField.Name = fieldName));
      if (!changedFieldSetting)
        return;
      WorkItemTypeMetadata itemTypeMetadata = context.GetWorkItemTypeCategory(requirementCategoryReferenceName).WorkItemTypeNames.Select<string, WorkItemTypeMetadata>((Func<string, WorkItemTypeMetadata>) (witName => context.GetWorkItemType(witName))).FirstOrDefault<WorkItemTypeMetadata>((Func<WorkItemTypeMetadata, bool>) (wit => wit.GetField("Microsoft.VSTS.Scheduling.Size") == null));
      string name1 = itemTypeMetadata.GetField("Microsoft.VSTS.Scheduling.OriginalEstimate").Name;
      string name2 = context.ProcessTemplate.GetWorkItemType(itemTypeMetadata.Name).GetField("Microsoft.VSTS.Scheduling.Size").Name;
      context.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarning_PlanningTools_CMMIFieldFix, (object) itemTypeMetadata.Name, (object) name2, (object) name1), IssueLevel.Warning));
    }

    void INotifyProjectFeatureProvisioned.OnProvisioned(
      IVssRequestContext requestContext,
      string projectUri)
    {
      requestContext.TraceEnter(1004000, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "PlanningToolsFeature.OnProvisioned");
      try
      {
        CommonStructureProjectInfo projectFromUri = CssUtils.GetProjectFromUri(requestContext, projectUri);
        WebApiTeam defaultTeam = requestContext.GetService<ITeamService>().GetDefaultTeam(requestContext, projectFromUri.GetId());
        TeamConfigurationHelper.SetDefaultSettings(requestContext, projectFromUri.ToProjectInfo(), defaultTeam, TeamAreaAction.UseRoot);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException("PlanningTools feature threw exception while post-configuration custom actions", ex);
        requestContext.TraceException(1004040, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, ex);
      }
      requestContext.TraceLeave(1004001, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "PlanningToolsFeature.OnProvisioned");
    }
  }
}
