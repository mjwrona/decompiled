// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server.Utility;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class LogStoreHelper
  {
    private const string ContainerNameRegex = "^[a-z0-9]+(-[a-z0-9]+)*$";
    private static Regex _regex = new Regex("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.Compiled, TimeSpan.FromMilliseconds((double) TestResultsConstants.RegexTimeOutInMilliSeconds));
    private static IBuildServiceHelper _buildServiceHelper;
    private static IReleaseServiceHelper _releaseServiceHelper;

    public static ContainerScopeDetails GetContainerScopeDetails(
      TestManagementRequestContext testManagementRequestContext,
      TestLogReference logReference)
    {
      ContainerScopeDetails containerScopeDetails = (ContainerScopeDetails) null;
      if (logReference.Scope == TestLogScope.Build && logReference.BuildId > 0)
        containerScopeDetails = new ContainerScopeDetails()
        {
          BuildId = logReference.BuildId,
          ContainerScope = ContainerScope.Build
        };
      else if (logReference.Scope == TestLogScope.Release && logReference.ReleaseId > 0)
        containerScopeDetails = new ContainerScopeDetails()
        {
          ReleaseId = logReference.ReleaseId,
          ReleaseEnvId = logReference.ReleaseEnvId,
          ContainerScope = ContainerScope.Release
        };
      else if (logReference.Scope == TestLogScope.Run && logReference.RunId > 0)
        containerScopeDetails = new ContainerScopeDetails()
        {
          RunIdId = logReference.RunId,
          ContainerScope = ContainerScope.Run
        };
      if (logReference.BuildId <= 0 && logReference.ReleaseId <= 0 && logReference.RunId <= 0)
        testManagementRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "GetContainerScopeDetails -Invalid buildId {0} ,  releaseId {1} and RunId {2}", (object) logReference.BuildId, (object) logReference.ReleaseId, (object) logReference.RunId);
      return containerScopeDetails;
    }

    public static void ValidateContainerDetails(
      TestManagementRequestContext testManagementRequestContext,
      ContainerScopeDetails containerScopeDetails,
      ProjectInfo projectInfo,
      bool checkPublishingPermission = false,
      bool allowReleaseContainer = false)
    {
      if (containerScopeDetails == null)
        return;
      int releaseDefinitionId = 0;
      int buildDefinitionId = 0;
      if (containerScopeDetails.ContainerScope == ContainerScope.Build)
      {
        Microsoft.TeamFoundation.Build.WebApi.Build build = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
        if (!LogStoreHelper.TryGetBuild(testManagementRequestContext, containerScopeDetails.BuildId, projectInfo.Id, out build))
          throw new TestObjectNotFoundException(testManagementRequestContext.RequestContext, containerScopeDetails.BuildId, ObjectTypes.AssociatedBuild);
        buildDefinitionId = build.Definition.Id;
      }
      else if (containerScopeDetails.ContainerScope == ContainerScope.Release && allowReleaseContainer)
      {
        ReleaseReference releaseRef = (ReleaseReference) null;
        if (!LogStoreHelper.TryGetRelease(testManagementRequestContext, containerScopeDetails.ReleaseId, containerScopeDetails.ReleaseEnvId, projectInfo.Id, out releaseRef))
          throw new TestObjectNotFoundException(testManagementRequestContext.RequestContext, containerScopeDetails.ReleaseId, ObjectTypes.AssociatedRelease);
        releaseDefinitionId = releaseRef.ReleaseDefId;
      }
      else
      {
        if (containerScopeDetails.ContainerScope != ContainerScope.Run)
          throw new TestObjectNotFoundException("ContainerScope: " + containerScopeDetails.ContainerScope.ToString() + " is not supported", ObjectTypes.Other);
        TestRun testRun = (TestRun) null;
        if (!LogStoreHelper.TryGetTestRun(testManagementRequestContext, containerScopeDetails.RunIdId, projectInfo.Name, out testRun))
          throw new TestObjectNotFoundException(testManagementRequestContext.RequestContext, containerScopeDetails.RunIdId, ObjectTypes.TestRun);
        if (testRun.ReleaseReference != null)
          releaseDefinitionId = testRun.ReleaseReference.ReleaseDefId;
        if (testRun.BuildReference != null)
          buildDefinitionId = testRun.BuildReference.BuildDefinitionId;
      }
      if (checkPublishingPermission && (releaseDefinitionId > 0 || buildDefinitionId > 0) && !LogStoreHelper.IsLogStorePublishAllowed(testManagementRequestContext.RequestContext, releaseDefinitionId, buildDefinitionId))
        throw new TestManagementInvalidOperationException("TestLogStore");
    }

    public static string GetContainerName(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails containerScopeDetails)
    {
      string containerName = (string) null;
      if (containerScopeDetails != null)
      {
        Guid? hostId = new Guid?();
        int dataspaceId = 0;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          hostId = new Guid?(requestContext.ServiceHost.InstanceId);
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
            dataspaceId = managementDatabase.GetDataspaceId(projectId);
        }
        else
          requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "Invalid requestconext, container name must be at collection level");
        containerName = LogStoreHelper.CreateContainerName(requestContext, hostId, dataspaceId, containerScopeDetails);
      }
      else
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetContainerName -null check for containerScopeDetails");
      return containerName;
    }

    public static TestLogContainer GetTestContainer(
      IVssRequestContext requestContext,
      string containerName,
      DateTime lastModifiedDateTime)
    {
      TestLogContainer testContainer = new TestLogContainer();
      List<string> list = ((IEnumerable<string>) containerName.Split('-')).ToList<string>();
      if (list.Count != 8)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetTestContainer - Invalid container - {0} name ", (object) containerName);
        return (TestLogContainer) null;
      }
      try
      {
        string input = list[0] + "-" + list[1] + "-" + list[2] + "-" + list[3] + "-" + list[4];
        testContainer.HostId = Guid.Parse(input);
        testContainer.DataspaceId = int.Parse(list[5]);
        testContainer.containerScopeDetails = new ContainerScopeDetails();
        testContainer.LastModifiedDate = lastModifiedDateTime;
        string str = list[6];
        switch (str)
        {
          case "buildid":
            testContainer.containerScopeDetails.ContainerScope = ContainerScope.Build;
            testContainer.containerScopeDetails.BuildId = int.Parse(list[7]);
            break;
          case "releaseid":
            testContainer.containerScopeDetails.ContainerScope = ContainerScope.Release;
            testContainer.containerScopeDetails.ReleaseId = int.Parse(list[7]);
            break;
          case "runid":
            testContainer.containerScopeDetails.ContainerScope = ContainerScope.Run;
            testContainer.containerScopeDetails.RunIdId = int.Parse(list[7]);
            break;
          default:
            requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetTestContainer - Invalid container scope - {0} name, {1} ", (object) containerName, (object) str);
            return (TestLogContainer) null;
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetTestContainer - Invalid container - {0} name, exception {1}", (object) containerName, (object) ex.Message);
        return (TestLogContainer) null;
      }
      return testContainer;
    }

    public static string SanatizeContainerName(
      IVssRequestContext requestContext,
      string containerName)
    {
      IRegexWrapper regexWrapper = (IRegexWrapper) new RegexWrapper(LogStoreHelper._regex);
      try
      {
        if (!string.IsNullOrEmpty(containerName) && containerName.Length > 3 && containerName.Length < 63)
        {
          if (regexWrapper.IsMatch(containerName))
            goto label_5;
        }
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "SanatizeContainerName - Invalid container - {0} name.", (object) containerName);
        return (string) null;
      }
      catch (RegexMatchTimeoutException ex)
      {
        requestContext.Trace(1015793, TraceLevel.Error, "TestManagement", "LogStorage", string.Format("sanitizing of string: '{0}' has failed with exception: {1}", (object) containerName, (object) ex));
        return (string) null;
      }
label_5:
      return containerName.ToLowerInvariant();
    }

    public static string GetContainerName(
      IVssRequestContext requestContext,
      TestLogContainer testLogContainer)
    {
      return testLogContainer != null ? LogStoreHelper.CreateContainerName(requestContext, new Guid?(testLogContainer.HostId), testLogContainer.DataspaceId, testLogContainer.containerScopeDetails) : (string) null;
    }

    public static bool TryGetTestRun(
      TestManagementRequestContext testManagementRequestContext,
      int testRunId,
      string projectName,
      out TestRun testRun)
    {
      testRun = (TestRun) null;
      TestRun[] array = TestRun.Query(testManagementRequestContext, testRunId, Guid.Empty, string.Empty, projectName).ToArray();
      if (array.Length == 0)
        return false;
      testRun = ((IEnumerable<TestRun>) array).First<TestRun>();
      return true;
    }

    public static bool TryGetBuild(
      TestManagementRequestContext testManagementRequestContext,
      int buildId,
      Guid projectId,
      out Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      build = LogStoreHelper.BuildServiceHelper.QueryBuildById(testManagementRequestContext.RequestContext, projectId, buildId, false);
      return build != null;
    }

    private static bool TryGetRelease(
      TestManagementRequestContext testManagementRequestContext,
      int releaseId,
      int releaseEnvId,
      Guid projectId,
      out ReleaseReference releaseRef)
    {
      releaseRef = LogStoreHelper.ReleaseServiceHelper.QueryReleaseReferenceById(testManagementRequestContext.RequestContext, new GuidAndString(string.Empty, projectId), releaseId, releaseEnvId);
      return releaseRef != null;
    }

    private static bool IsLogStorePublishAllowed(
      IVssRequestContext requestContext,
      int releaseDefinitionId,
      int buildDefinitionId)
    {
      if (!requestContext.IsFeatureEnabled("TestManagement.Server.EnableRampPlanForTestLogStore"))
        return true;
      string query = (string) null;
      if (releaseDefinitionId > 0)
        query = "/Service/TestManagement/Settings/TcmLogStoreEnablementRoot/ReleaseDefinition/" + releaseDefinitionId.ToString();
      else if (buildDefinitionId > 0)
        query = "/Service/TestManagement/Settings/TcmLogStoreEnablementRoot/BuildDefinition/" + buildDefinitionId.ToString();
      return string.IsNullOrEmpty(query) || requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) query, false);
    }

    private static string CreateContainerName(
      IVssRequestContext requestContext,
      Guid? hostId,
      int dataspaceId,
      ContainerScopeDetails containerScopeDetails)
    {
      string containerName = (string) null;
      if (containerScopeDetails != null)
      {
        if (hostId.HasValue && dataspaceId != 0)
        {
          if (containerScopeDetails.ContainerScope == ContainerScope.Build && containerScopeDetails.BuildId > 0)
            containerName = string.Format("{0}-{1}-buildid-{2}", (object) hostId, (object) dataspaceId, (object) containerScopeDetails.BuildId);
          else if (containerScopeDetails.ContainerScope == ContainerScope.Release && containerScopeDetails.ReleaseId > 0)
            containerName = string.Format("{0}-{1}-releaseid-{2}", (object) hostId, (object) dataspaceId, (object) containerScopeDetails.ReleaseId);
          else if (containerScopeDetails.ContainerScope == ContainerScope.Run && containerScopeDetails.RunIdId > 0)
            containerName = string.Format("{0}-{1}-runid-{2}", (object) hostId, (object) dataspaceId, (object) containerScopeDetails.RunIdId);
          containerName = containerName?.ToLowerInvariant();
        }
      }
      else
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "CreateContainerName -null check for containerScopeDetails");
      return containerName;
    }

    internal static IBuildServiceHelper BuildServiceHelper
    {
      get => LogStoreHelper._buildServiceHelper ?? (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
      set => LogStoreHelper._buildServiceHelper = value;
    }

    internal static IReleaseServiceHelper ReleaseServiceHelper
    {
      get => LogStoreHelper._releaseServiceHelper ?? (IReleaseServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.ReleaseServiceHelper();
      set => LogStoreHelper._releaseServiceHelper = value;
    }
  }
}
