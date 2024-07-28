// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ProcessConfigurationHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class ProcessConfigurationHelper
  {
    internal static TestManagementProcessConfig GetProcessConfiguration(
      TestManagementRequestContext context,
      string projectUri,
      string witCategoryRefName)
    {
      context.TraceEnter("BusinessLayer", "ProcessConfigurationHelper.GetProcessConfiguration");
      TestManagementProcessConfig processConfiguration = (TestManagementProcessConfig) null;
      try
      {
        processConfiguration = ProcessConfigurationHelper.GetTestManagementProcessConfig(context.RequestContext, projectUri);
      }
      catch (Exception ex)
      {
        context.TraceException("BusinessLayer", ex);
      }
      if (processConfiguration == null)
      {
        processConfiguration = (TestManagementProcessConfig) null;
        context.TraceWarning("BusinessLayer", "Did not find processconfig for project {0}", (object) projectUri);
      }
      else if (string.Equals(witCategoryRefName, WitCategoryRefName.TestPlan) && processConfiguration.TestPlanStates == null)
        processConfiguration = (TestManagementProcessConfig) null;
      else if (string.Equals(witCategoryRefName, WitCategoryRefName.TestSuite) && processConfiguration.TestSuiteStates == null)
        processConfiguration = (TestManagementProcessConfig) null;
      context.TraceLeave("BusinessLayer", "ProcessConfigurationHelper.GetProcessConfiguration");
      return processConfiguration;
    }

    internal static TestManagementProcessConfig GetTestManagementProcessConfig(
      IVssRequestContext requestContext,
      string projectUri)
    {
      IProjectConfigurationService service = requestContext.GetService<IProjectConfigurationService>();
      if (service == null)
        return (TestManagementProcessConfig) null;
      ProjectProcessConfiguration processSettings = service.GetProcessSettings(requestContext, projectUri, false);
      TestManagementProcessConfig tcmProcessConfig = new TestManagementProcessConfig();
      ProcessConfigurationHelper.PopulateTestPlanStatesOfTcmProcessConfig(processSettings, tcmProcessConfig);
      ProcessConfigurationHelper.PopulateTestSuiteStatesOfTcmProcessConfig(processSettings, tcmProcessConfig);
      return tcmProcessConfig;
    }

    private static void PopulateTestSuiteStatesOfTcmProcessConfig(
      ProjectProcessConfiguration projectProcessConfig,
      TestManagementProcessConfig tcmProcessConfig)
    {
      if (projectProcessConfig.TestSuiteWorkItems == null)
        return;
      tcmProcessConfig.TestSuiteStates = projectProcessConfig.TestSuiteWorkItems.States;
    }

    private static void PopulateTestPlanStatesOfTcmProcessConfig(
      ProjectProcessConfiguration projectProcessConfig,
      TestManagementProcessConfig tcmProcessConfig)
    {
      if (projectProcessConfig.TestPlanWorkItems == null)
        return;
      tcmProcessConfig.TestPlanStates = projectProcessConfig.TestPlanWorkItems.States;
    }

    internal static void ValidateProcessConfigurationForCategory(
      TestManagementRequestContext context,
      string projectName,
      string projectUri,
      string witCategoryRefName)
    {
      context.TraceEnter("BusinessLayer", "ProcessConfigurationHelper.ValidateProcessConfigurationForCategory");
      List<WorkItemTypeCategory> itemTypeCategories = context.RequestContext.GetService<IWitHelper>().GetWorkItemTypeCategories(context.RequestContext, projectName, (IEnumerable<string>) new List<string>()
      {
        witCategoryRefName
      });
      if (itemTypeCategories == null || itemTypeCategories.Count != 1)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidWitCategory, (object) witCategoryRefName));
      foreach (WorkItemTypeReference workItemType in itemTypeCategories[0].WorkItemTypes)
        ProcessConfigurationHelper.ValidateProcessConfiguration(context, projectUri, projectName, workItemType.Name, witCategoryRefName, true);
      context.TraceLeave("BusinessLayer", "ProcessConfigurationHelper.ValidateProcessConfigurationForCategory");
    }

    internal static void ValidateProcessConfiguration(
      TestManagementRequestContext context,
      string projectUri,
      string projectName,
      string witType,
      string witCategoryRefName,
      bool validateStartState)
    {
      context.TraceEnter("BusinessLayer", "ProcessConfigurationHelper.ValidateProcessConfiguration");
      TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem(context, witCategoryRefName);
      tcmWorkItem.ValidateProcessConfigMappingExistsForAllWorkItemStates(context, projectUri, projectName, witType);
      if (validateStartState)
        tcmWorkItem.ValidateDefaultStateMapsToInProgressMetaState(context, projectUri, projectName, witType);
      context.TraceLeave("BusinessLayer", "ProcessConfigurationHelper.ValidateProcessConfiguration");
    }

    public static IList<string> GetWorkItemStates(
      TestManagementRequestContext context,
      string projectUri,
      byte tcmState,
      string witCategoryRefName)
    {
      context.TraceEnter("BusinessLayer", "ProcessConfigurationHelper.GetWorkItemStates");
      TestManagementProcessConfig processConfiguration = ProcessConfigurationHelper.GetProcessConfiguration(context, projectUri, witCategoryRefName);
      TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem(context, witCategoryRefName);
      IList<string> workItemStates = tcmState != (byte) 0 ? tcmWorkItem.GetWorkItemState(context, processConfiguration, tcmState) : tcmWorkItem.GetWorkItemStates(context, processConfiguration);
      context.TraceLeave("BusinessLayer", "ProcessConfigurationHelper.GetWorkItemStates");
      return workItemStates;
    }

    public static string GetWorkItemStateFromProcessConfiguration(
      TestManagementRequestContext context,
      string projectUri,
      byte tcmState,
      bool autoResolveMultiStateMappingAmbiguity,
      string witCategoryRefName)
    {
      context.TraceEnter("BusinessLayer", "ProcessConfigurationHelper.GetWorkItemStateFromProcessConfiguration");
      TestManagementProcessConfig processConfiguration1 = ProcessConfigurationHelper.GetProcessConfiguration(context, projectUri, witCategoryRefName);
      string processConfiguration2 = TCMWorkItemBase.CreateTCMWorkItem(context, witCategoryRefName).GetWorkItemStateFromProcessConfiguration(context, processConfiguration1, tcmState, autoResolveMultiStateMappingAmbiguity);
      context.TraceInfo("BusinessLayer", "category:{0} state value:{1}", (object) witCategoryRefName, (object) processConfiguration2);
      context.TraceLeave("BusinessLayer", "ProcessConfigurationHelper.GetWorkItemStateFromProcessConfiguration");
      return processConfiguration2;
    }

    internal static void ValidateProcessConfiguration(
      TestManagementRequestContext context,
      string teamProjectName)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, teamProjectName);
      ProcessConfigurationHelper.ValidateProcessConfigurationForCategory(context, teamProjectName, projectUriFromName, WitCategoryRefName.TestPlan);
      ProcessConfigurationHelper.ValidateProcessConfigurationForCategory(context, teamProjectName, projectUriFromName, WitCategoryRefName.TestSuite);
    }
  }
}
