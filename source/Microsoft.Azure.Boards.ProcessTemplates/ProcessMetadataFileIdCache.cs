// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessMetadataFileIdCache
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessMetadataFileIdCache : IVssFrameworkService
  {
    private Dictionary<string, int> m_processMetadataFileIds;
    private int m_processMetadataFileIdsVersion;

    public bool TryGetProcessMetadataFileId(
      IVssRequestContext requestContext,
      Guid processTypeId,
      string resourceType,
      string resourceName,
      out int fileId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceType, nameof (resourceType));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      fileId = 0;
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_processMetadataFileIds == null)
      {
        using (ProcessMetadataFilesComponent metadataFilesComponent = this.CreateProcessMetadataFilesComponent(deploymentContext))
        {
          int? processMetadataFileId = metadataFilesComponent.GetProcessMetadataFileId(processTypeId, resourceType.ToString(), resourceName);
          if (!processMetadataFileId.HasValue)
            return false;
          fileId = processMetadataFileId.Value;
          return true;
        }
      }
      else
      {
        if (this.m_processMetadataFileIds.TryGetValue(ProcessMetadataFileIdCache.GetCacheKey(processTypeId, resourceType, resourceName), out fileId))
          return true;
        requestContext.Trace(100052007, TraceLevel.Error, nameof (ProcessMetadataFileIdCache), nameof (TryGetProcessMetadataFileId), "Couldn't find fileId in cache. key: " + ProcessMetadataFileIdCache.GetCacheKey(processTypeId, resourceType, resourceName) + ", LocalCache: " + string.Join(",", (IEnumerable<string>) this.m_processMetadataFileIds.Keys));
        return false;
      }
    }

    public int GetFileIdsCacheVersion() => this.m_processMetadataFileIdsVersion;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.Trace(100052011, TraceLevel.Info, nameof (ProcessMetadataFileIdCache), nameof (ServiceStart), "ProcessMetadataFileIdCache service is started.");
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_processMetadataFileIdsVersion = 0;
      this.RefreshCache(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.ProcessMetadataFileChanged, new SqlNotificationCallback(this.OnProcessMetadataFileModified), false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.Trace(100052012, TraceLevel.Info, nameof (ProcessMetadataFileIdCache), nameof (ServiceEnd), "ProcessMetadataFileIdCache service is ending");
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.ProcessMetadataFileChanged, new SqlNotificationCallback(this.OnProcessMetadataFileModified), true);
    }

    public void RefreshCache(IVssRequestContext requestContext)
    {
      IEnumerable<ProcessMetadataFileEntry> processMetadataFiles;
      using (ProcessMetadataFilesComponent metadataFilesComponent = this.CreateProcessMetadataFilesComponent(requestContext))
        processMetadataFiles = metadataFilesComponent.GetAllProcessMetadataFiles();
      if (processMetadataFiles == null)
        return;
      if (!processMetadataFiles.Any<ProcessMetadataFileEntry>())
        requestContext.Trace(100052007, TraceLevel.Error, nameof (ProcessMetadataFileIdCache), nameof (RefreshCache), "ProcessMetadataFilesComponent didn't return any response.");
      Dictionary<string, int> newFileIds = new Dictionary<string, int>();
      foreach (ProcessMetadataFileEntry metadataFileEntry in processMetadataFiles)
      {
        string cacheKey = ProcessMetadataFileIdCache.GetCacheKey(metadataFileEntry.ProcessTypeId, metadataFileEntry.ResourceType, metadataFileEntry.ResourceName);
        newFileIds.Add(cacheKey, metadataFileEntry.FileId);
      }
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Current count of fileids in cache", (object) this.m_processMetadataFileIds?.Count);
      properties.Add("Current Cache version", (double) this.m_processMetadataFileIdsVersion);
      this.UpdateCacheVersion(newFileIds);
      this.m_processMetadataFileIds = newFileIds;
      properties.Add("Updated count of fileids in cache", (object) this.m_processMetadataFileIds?.Count);
      properties.Add("Updated Cache version", (double) this.m_processMetadataFileIdsVersion);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessMetadataFileIdCache), nameof (RefreshCache), properties);
    }

    private void OnProcessMetadataFileModified(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.RefreshCache(requestContext.To(TeamFoundationHostType.Deployment));
    }

    private void UpdateCacheVersion(Dictionary<string, int> newFileIds)
    {
      if (this.m_processMetadataFileIds == null)
        return;
      int num = 0;
      foreach (KeyValuePair<string, int> newFileId in newFileIds)
      {
        if (!this.m_processMetadataFileIds.ContainsKey(newFileId.Key) || this.m_processMetadataFileIds[newFileId.Key] != newFileId.Value)
          num = 1;
      }
      this.m_processMetadataFileIdsVersion += num;
    }

    private ProcessMetadataFilesComponent CreateProcessMetadataFilesComponent(
      IVssRequestContext deploymentContext)
    {
      return deploymentContext.CreateComponent<ProcessMetadataFilesComponent>();
    }

    public static string GetCacheKey(Guid processTypeId, string resourceType, string resourceName) => string.Join(";", new string[3]
    {
      processTypeId.ToString(),
      resourceType,
      resourceName
    });
  }
}
