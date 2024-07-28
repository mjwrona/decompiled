// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.TeamFoundationProxyRepositoryService
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  internal class TeamFoundationProxyRepositoryService : IVssFrameworkService
  {
    private ReaderWriterLock m_configLock = new ReaderWriterLock();
    private Dictionary<string, DateTime> m_failedServers = new Dictionary<string, DateTime>((IEqualityComparer<string>) VssStringComparer.Guid);
    private Dictionary<string, string> m_instances = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.Guid);
    private Dictionary<string, Repository> m_proxyRepositories = new Dictionary<string, Repository>((IEqualityComparer<string>) VssStringComparer.Guid);
    private HashSet<string> m_duplicatedServers = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.Guid);
    private HashSet<string> m_duplicatedRepositories = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.Guid);

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.InitializeServers(systemRequestContext.GetService<TeamFoundationFileCacheService>().Configuration);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      foreach (Repository repository in this.m_proxyRepositories.Values)
        repository.Dispose();
      this.m_proxyRepositories.Clear();
    }

    internal Repository GetRepository(
      IProxyConfiguration configuration,
      RepositoryInformation repositoryInformation)
    {
      if (!configuration.IsRemoteConfiguration)
        throw new NotSupportedException();
      Repository repository;
      try
      {
        this.m_configLock.AcquireReaderLock(-1);
        if (this.m_duplicatedServers.Contains(repositoryInformation.InstanceId))
          throw new DuplicatedServerException(ProxyResources.Format("DuplicateServer", (object) repositoryInformation.InstanceId));
        if (this.m_duplicatedRepositories.Contains(repositoryInformation.RepositoryId))
          throw new DuplicatedServerException(ProxyResources.Format("DuplicateServer", (object) repositoryInformation.RepositoryId));
        if (this.m_proxyRepositories.TryGetValue(repositoryInformation.RepositoryId, out repository))
          return repository;
      }
      finally
      {
        if (this.m_configLock.IsReaderLockHeld)
          this.m_configLock.ReleaseReaderLock();
      }
      try
      {
        this.m_configLock.AcquireWriterLock(-1);
        DateTime dateTime;
        if (this.m_failedServers.TryGetValue(repositoryInformation.RepositoryId, out dateTime))
        {
          if (DateTime.UtcNow.Subtract(dateTime).TotalSeconds < 3600.0)
            goto label_22;
        }
        this.m_failedServers.Remove(repositoryInformation.RepositoryId);
        if (this.m_proxyRepositories.TryGetValue(repositoryInformation.RepositoryId, out repository))
          return repository;
        string uriString;
        if (!string.IsNullOrEmpty(repositoryInformation.InstanceId) && this.m_instances.TryGetValue(repositoryInformation.InstanceId, out uriString))
        {
          this.RegisterServer(configuration, new Uri(new Uri(uriString), repositoryInformation.CollectionPath), true);
          if (this.m_proxyRepositories.TryGetValue(repositoryInformation.RepositoryId, out repository))
            return repository;
        }
        this.m_failedServers[repositoryInformation.RepositoryId] = DateTime.UtcNow;
      }
      finally
      {
        if (this.m_configLock.IsWriterLockHeld)
          this.m_configLock.ReleaseWriterLock();
      }
label_22:
      throw new Microsoft.TeamFoundation.Framework.Server.UnknownRepositoryException(ProxyResources.Get("UnknownRepository"));
    }

    private void InitializeServers(ProxyConfiguration configuration)
    {
      foreach (Uri serverUri in (IEnumerable<Uri>) configuration.ServerUris)
        this.RegisterServer((IProxyConfiguration) configuration, serverUri, false);
    }

    private void RegisterServer(
      IProxyConfiguration configuration,
      Uri serverUri,
      bool collectionServer)
    {
      TeamFoundationTrace.Verbose("Adding Server {0}", (object) serverUri.ToString());
      VssCredentials credentials = VssClientCredentials.LoadCachedCredentials(CredentialsStorageRegistryKeywords.Proxy, serverUri, false);
      if (!collectionServer)
      {
        try
        {
          TfsConfigurationServer configurationServer = new TfsConfigurationServer(serverUri, credentials);
          configurationServer.Authenticate();
          if (!configurationServer.IsHostedServer)
          {
            this.SaveServer(configurationServer.InstanceId.ToString(), serverUri);
            return;
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
        }
      }
      try
      {
        TfsTeamProjectCollection teamProjectCollection = new TfsTeamProjectCollection(serverUri, credentials);
        teamProjectCollection.Authenticate();
        string str = teamProjectCollection.InstanceId.ToString();
        Repository repository = RepositoryFactory.CreateRepository(teamProjectCollection, Path.Combine(configuration.CacheRoot, str));
        if (this.SaveRepository(str, repository))
          configuration.RegisterRepository(str, serverUri.ToString());
        else
          configuration.UnRegisterRepository(str);
      }
      catch (Exception ex)
      {
        string message = ProxyResources.Format("ErrorLoadingCollection", (object) serverUri.ToString(), (object) ex.Message);
        EventLogEntryType level = !(ex is Microsoft.TeamFoundation.Framework.Client.TeamFoundationSecurityServiceException) ? EventLogEntryType.Warning : EventLogEntryType.Error;
        TeamFoundationEventLog.Default.LogException(message, ex, TeamFoundationEventId.DefaultExceptionEventId, level);
      }
    }

    internal bool SaveServer(string instanceId, Uri serverUri)
    {
      if (!this.m_duplicatedServers.Contains(instanceId) && !this.m_instances.ContainsKey(instanceId))
      {
        this.m_instances[instanceId] = serverUri.AbsoluteUri;
        return true;
      }
      this.m_duplicatedServers.Add(instanceId);
      this.m_instances.Remove(instanceId);
      TeamFoundationEventLog.Default.Log(ProxyResources.Format("DuplicateServerUri", (object) serverUri), TeamFoundationEventId.ProxyDuplicatedServerUri, EventLogEntryType.Error);
      return false;
    }

    internal bool SaveRepository(string repositoryId, Repository repository)
    {
      if (!this.m_duplicatedRepositories.Contains(repositoryId) && !this.m_proxyRepositories.ContainsKey(repositoryId))
      {
        this.m_proxyRepositories[repositoryId] = repository;
        return true;
      }
      this.m_duplicatedRepositories.Add(repositoryId);
      this.m_proxyRepositories.Remove(repositoryId);
      TeamFoundationEventLog.Default.Log(ProxyResources.Format("DuplicateServerUri", (object) repositoryId), TeamFoundationEventId.ProxyDuplicatedServerUri, EventLogEntryType.Error);
      return false;
    }
  }
}
