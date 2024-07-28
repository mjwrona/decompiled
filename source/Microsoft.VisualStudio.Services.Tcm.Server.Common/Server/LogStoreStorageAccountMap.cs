// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreStorageAccountMap
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreStorageAccountMap
  {
    public LogStoreStorageAccountMap(int storageAccountIndex, bool isReadOnly)
    {
      this.StorageAccountConnectionIndex = storageAccountIndex;
      this.IsReadOnly = isReadOnly;
    }

    public int StorageAccountConnectionIndex { get; }

    public bool IsReadOnly { get; }

    public static int GetStorageAccountConnectionIndexOfArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails scopeDetails)
    {
      using (PerfManager.Measure(requestContext, "LogStorage", TraceUtils.GetActionName("LogStoreStorageAccountMap.GetStorageAccountConnectionIndexOfArtifact", "Tcm")))
      {
        int artifactId = LogStoreStorageAccountMap.GetArtifactId(scopeDetails);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          return managementDatabase.GetLogStoreArtifactStorageAccount(projectId, scopeDetails.ContainerScope, artifactId);
      }
    }

    public static int CreateNewStorageAccountConnectionIndexOfArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails scopeDetails,
      long logStoreConnectionCounter)
    {
      using (PerfManager.Measure(requestContext, "LogStorage", TraceUtils.GetActionName("LogStoreStorageAccountMap.CreateNewStorageAccountConnectionIndexOfArtifact", "Tcm")))
      {
        int artifactId = LogStoreStorageAccountMap.GetArtifactId(scopeDetails);
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        int connectionIndexOfArtifact;
        if (!requestContext.IsFeatureEnabled("TestManagement.Server.EnableShardingForTestLogStore"))
        {
          connectionIndexOfArtifact = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreDefaultStorageAccountIndex", true, 0);
          requestContext.TraceVerbose("LogStorage", string.Format("LogStoreStorageAccountMap.CreateNewStorageAccountConnectionIndexOfArtifact : {0}", (object) 0), (object) connectionIndexOfArtifact);
        }
        else
          connectionIndexOfArtifact = LogStoreStorageAccountMap.GetNewStorageAccountConnectionIndexOfArtifact(requestContext, projectId, scopeDetails, logStoreConnectionCounter);
        try
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
            managementDatabase.CreateLogStoreArtifactStorageAccountMap(projectId, scopeDetails.ContainerScope, artifactId, connectionIndexOfArtifact);
          requestContext.TraceVerbose("LogStorage", string.Format("LogStoreStorageAccountMap.CreateNewStorageAccountConnectionIndexOfArtifact - Using shard at index 0 for artifactId:{0} and Scope: {1}", (object) LogStoreStorageAccountMap.GetArtifactId(scopeDetails), (object) scopeDetails.ContainerScope.ToString()));
        }
        catch (TestManagementInvalidOperationException ex)
        {
          requestContext.TraceException("LogStorage", (Exception) ex);
        }
        return connectionIndexOfArtifact;
      }
    }

    private static int GetArtifactId(ContainerScopeDetails scopeDetails)
    {
      switch (scopeDetails.ContainerScope)
      {
        case ContainerScope.Build:
          return scopeDetails.BuildId;
        case ContainerScope.Run:
          return scopeDetails.RunIdId;
        case ContainerScope.Release:
          return scopeDetails.ReleaseId;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "ContainerScope"));
      }
    }

    private static int GetNewStorageAccountConnectionIndexOfArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails scopeDetails,
      long logStoreConnectionCounter)
    {
      using (PerfManager.Measure(requestContext, "LogStorage", TraceUtils.GetActionName("LogStoreStorageAccountMap.GetNewStorageAccountConnectionIndexOfArtifact", "Tcm")))
      {
        List<LogStoreStorageAccountMap> storeStorageAccount = LogStoreStorageAccountMap.GetLogStoreStorageAccount(requestContext, projectId);
        if (storeStorageAccount.Count == 0)
        {
          requestContext.TraceVerbose("LogStorage", "LogStoreStorageAccountMap-GetNewStorageAccountConnectionIndexOfArtifact - Did not find log store storage account mapping for project {0}", (object) projectId);
          storeStorageAccount = LogStoreStorageAccountMap.GetLogStoreStorageAccount(requestContext, Guid.Empty);
        }
        if (storeStorageAccount.Count == 0)
        {
          requestContext.TraceVerbose("LogStorage", "LogStoreStorageAccountMap-GetNewStorageAccountConnectionIndexOfArtifact - No storage account dedicated for projectId: " + projectId.ToString() + " hence defaulting to 0 index");
          return 0;
        }
        if (LogStoreStorageAccountMap.CheckIfNeedToFallBackToDefaultShard(requestContext, scopeDetails, projectId))
        {
          requestContext.TraceVerbose("LogStorage", string.Format("LogStoreStorageAccountMap-GetNewStorageAccountConnectionIndexOfArtifact - Falling to default shard at index 0 for this artifactId:{0} and Scope: {1}", (object) LogStoreStorageAccountMap.GetArtifactId(scopeDetails), (object) scopeDetails.ContainerScope.ToString()));
          return 0;
        }
        int index = (int) logStoreConnectionCounter % storeStorageAccount.Count;
        return storeStorageAccount[index].StorageAccountConnectionIndex;
      }
    }

    private static bool CheckIfNeedToFallBackToDefaultShard(
      IVssRequestContext requestContext,
      ContainerScopeDetails containerScopeDetails,
      Guid projectId)
    {
      int artifactId = LogStoreStorageAccountMap.GetArtifactId(containerScopeDetails);
      string query = string.Format("/Service/TestManagement/Settings/TcmLogStoreShardFallbackMax/{0}/{1}", (object) projectId, (object) containerScopeDetails.ContainerScope.ToString());
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) query, -1);
      return artifactId > 0 && artifactId < num;
    }

    private static List<LogStoreStorageAccountMap> GetLogStoreStorageAccount(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      using (PerfManager.Measure(requestContext, "LogStorage", TraceUtils.GetActionName("LogStoreStorageAccountMap.GetLogStoreStorageAccount", "Tcm")))
      {
        List<LogStoreStorageAccountMap> storeStorageAccount;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          storeStorageAccount = managementDatabase.GetLogStoreStorageAccount(projectId);
        return storeStorageAccount.Where<LogStoreStorageAccountMap>((Func<LogStoreStorageAccountMap, bool>) (x => !x.IsReadOnly)).ToList<LogStoreStorageAccountMap>();
      }
    }
  }
}
