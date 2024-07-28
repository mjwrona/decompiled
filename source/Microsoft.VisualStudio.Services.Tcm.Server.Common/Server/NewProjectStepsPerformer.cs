// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.NewProjectStepsPerformer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class NewProjectStepsPerformer
  {
    internal static void ExecuteSteps(TestManagementRequestContext context, string projectName)
    {
      GuidAndString projectGuidAndUri = context.ProjectServiceHelper.GetProjectGuidAndUri(projectName);
      NewProjectStepsPerformer.CreateDefaultTestExtensionFieldsForProject(context, projectGuidAndUri.GuidId);
      context.RequestContext.GetService<IResultRetentionSettingsService>().CreateResultRetentionSettingsForNewProject(context, projectGuidAndUri);
    }

    public static void InitializeNewProject(
      TestManagementRequestContext context,
      string projectName)
    {
      Guid guidId = context.ProjectServiceHelper.GetProjectGuidAndUri(projectName).GuidId;
      NewProjectStepsPerformer.InitializeNewProject(context, guidId);
    }

    public static void InitializeNewProject(TestManagementRequestContext context, Guid projectId)
    {
      if (NewProjectStepsPerformer.IsSynced(context.RequestContext, projectId))
        return;
      NewProjectStepsPerformer.CreateProject(context, projectId);
      if (context.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        NewProjectStepsPerformer.SyncMetaData(context, projectId);
      NewProjectStepsPerformer.CreateDefaultTestExtensionFieldsForProject(context, projectId);
      NewProjectStepsPerformer.CreateLogStoreProjectSummary(context, projectId);
      NewProjectStepsPerformer.CompleteSync(context.RequestContext, projectId);
    }

    public static void SyncTestFailureTypes(
      TestManagementRequestContext targetRequestContext,
      Guid projectId)
    {
      try
      {
        targetRequestContext.TraceInfo("Framework", "syncing failure types for {0}", (object) projectId);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType> result = targetRequestContext.RequestContext.GetClient<TCMServiceMigrationHttpClient>().GetTestFailureTypesAsync(projectId).Result;
        targetRequestContext.TraceInfo("Framework", string.Format("Found {0} failure types.", (object) 0), (object) result.Count);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(targetRequestContext))
          managementDatabase.SyncTestFailureTypes((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType>) result, projectId);
      }
      catch (Exception ex)
      {
        targetRequestContext.TraceError("Framework", string.Format("Initalizing test failure types failed because of exception: {0} for project {1}", (object) ex.ToString(), (object) projectId));
        throw;
      }
    }

    public static void SyncTestResolutionStates(
      TestManagementRequestContext targetRequestContext,
      Guid projectId)
    {
      try
      {
        targetRequestContext.TraceInfo("Framework", "syncing resolution states for {0}", (object) projectId);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState> result = targetRequestContext.RequestContext.GetClient<TCMServiceMigrationHttpClient>().GetTestResolutionStatesAsync(projectId).Result;
        targetRequestContext.TraceInfo("Framework", string.Format("Found {0} resolution states.", (object) 0), (object) result.Count);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(targetRequestContext))
          managementDatabase.SyncTestResolutionStates((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState>) result, projectId);
      }
      catch (Exception ex)
      {
        targetRequestContext.TraceError("Framework", string.Format("Initalizing test resolution states failed because of exception: {0} for project {1}", (object) ex.ToString(), (object) projectId));
        throw;
      }
    }

    public static void SyncTestSettings(
      TestManagementRequestContext targetRequestContext,
      Guid projectId)
    {
      targetRequestContext.TraceInfo("Framework", "syncing testSettings for {0}", (object) projectId);
      TestManagementHttpClient client = targetRequestContext.RequestContext.GetClient<TestManagementHttpClient>();
      int result1 = NewProjectStepsPerformer.GetTestSettingsContinuationTokenId(targetRequestContext.RequestContext, projectId);
      DataContractConverter contractConverter = new DataContractConverter(targetRequestContext);
      try
      {
        do
        {
          IPagedList<TestSettings2> result2 = client.GetTestSettingsAsync2(projectId, continuationToken: new int?(result1)).Result;
          int.TryParse(result2.ContinuationToken, out result1);
          targetRequestContext.TraceInfo("Framework", "testSettings Count {0} continuationTokenId {1}.", (object) result2.Count, (object) result1);
          IList<TestSettings> fromDataContract = contractConverter.GetTestSettingsListFromDataContract((IList<TestSettings2>) result2);
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(targetRequestContext))
            managementDatabase.SyncTestSettings((IEnumerable<TestSettings>) fromDataContract, projectId);
          NewProjectStepsPerformer.SetTestSettingsContinuationTokenId(targetRequestContext.RequestContext, projectId, result1);
        }
        while (result1 > 0);
      }
      catch (Exception ex)
      {
        targetRequestContext.TraceError("Framework", string.Format("Initalizing testSettings failed because of exception: {0} for project {1}", (object) ex.ToString(), (object) projectId));
        throw;
      }
    }

    public static void CreateDefaultTestExtensionFieldsForProject(
      TestManagementRequestContext requestContext,
      Guid projectId)
    {
      requestContext.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().CreateDefaultTestExtensionFieldsForProject(requestContext, projectId);
    }

    private static void CreateProject(TestManagementRequestContext context, Guid projectId)
    {
      string projectName = context.ProjectServiceHelper.GetProjectName(projectId);
      string projectUri = context.ProjectServiceHelper.GetProjectUri(projectName);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.CreateProject(projectUri, projectId, projectName, 0, out bool _);
      try
      {
        context.RequestContext.GetService<IDataspaceService>().CreateDataspace(context.RequestContext, "Default", projectId, DataspaceState.Active);
      }
      catch (SqlException ex)
      {
        if (ex.Number == 2601)
        {
          context.TraceInfo("Framework", "Caught sql exception number 2601, while creating {0} Dataspace for the Project {1} from {2}", (object) "Default", (object) projectId, (object) new StackTrace());
        }
        else
        {
          context.TraceError("Framework", "Exception occurred while {0} Dataspace creation for the Project {1}. Exception: {2}", (object) "Default", (object) projectId, (object) ex);
          throw;
        }
      }
    }

    private static void SyncMetaData(TestManagementRequestContext context, Guid projectId)
    {
      NewProjectStepsPerformer.SyncTestFailureTypes(context, projectId);
      NewProjectStepsPerformer.SyncTestResolutionStates(context, projectId);
      NewProjectStepsPerformer.SyncTestSettings(context, projectId);
    }

    private static bool IsSynced(IVssRequestContext context, Guid projectId)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      string str = string.Format("/Service/TestManagement/TcmServiceNewProjectSyncCompleted/{0}", (object) projectId);
      IVssRequestContext requestContext = context;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      return service.GetValue<bool>(requestContext, in local, false);
    }

    private static void CompleteSync(IVssRequestContext context, Guid projectId)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      string str = string.Format("/Service/TestManagement/TcmServiceNewProjectSyncCompleted/{0}", (object) projectId);
      IVssRequestContext requestContext = context;
      string path = str;
      service.SetValue<bool>(requestContext, path, true);
    }

    private static int GetTestSettingsContinuationTokenId(
      IVssRequestContext context,
      Guid projectId)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      string str = string.Format("/Service/TestManagement/TestSettingsContinuationTokenId/{0}", (object) projectId);
      IVssRequestContext requestContext = context;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      return service.GetValue<int>(requestContext, in local, 0);
    }

    private static void SetTestSettingsContinuationTokenId(
      IVssRequestContext context,
      Guid projectId,
      int value)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      string str = string.Format("/Service/TestManagement/TestSettingsContinuationTokenId/{0}", (object) projectId);
      IVssRequestContext requestContext = context;
      string path = str;
      int num = value;
      service.SetValue<int>(requestContext, path, num);
    }

    private static void CreateLogStoreProjectSummary(
      TestManagementRequestContext context,
      Guid projectId)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.UpdateLogStoreProjectSummary(projectId, 0L, true);
    }
  }
}
