// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.GitRepoSecurityChecksService
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  public class GitRepoSecurityChecksService : IDisposable
  {
    private readonly object m_gitSecurityNamespaceLock;
    private readonly object m_searchSecurityNamespaceLock;
    private IVssSecurityNamespace m_gitSecurityNamespace;
    private bool m_disposedValue;

    protected ReaderWriterLockSlim RepoSecuritySetsLock { get; private set; }

    protected IVssSecurityNamespace SearchSecurityNamespace { get; private set; }

    public GitRepoSecurityChecksService()
    {
      this.m_gitSecurityNamespaceLock = new object();
      this.m_searchSecurityNamespaceLock = new object();
      this.RepoSecuritySetsLock = new ReaderWriterLockSlim();
    }

    protected internal Dictionary<byte[], List<GitRepositoryData>> RepositorySecuritySets { get; set; }

    internal void SetSecurityData(
      IVssRequestContext requestContext,
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      requestContext.Items["searchServiceSecurityNamespaceGuidKey"] = (object) namespaceId;
      requestContext.Items["searchServiceSecurityPermissionKey"] = (object) requiredPermissions;
      requestContext.Items["searchServiceSecurityTokenKey"] = (object) token;
    }

    public virtual bool GitRepoHasReadPermission(
      IVssRequestContext requestContext,
      GitRepositoryData repo)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081345, "Query Pipeline", "SecurityChecks", "HasReadPermission");
      try
      {
        string securable = GitUtils.CalculateSecurable(repo.ProjectId, repo.Id, (string) null);
        int num = this.m_gitSecurityNamespace.HasPermission(requestContext, securable, 2, false) ? 1 : 0;
        if (num != 0)
          this.SetSecurityData(requestContext, GitConstants.GitSecurityNamespaceId, 2, securable);
        return num != 0;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081346, "Query Pipeline", "SecurityChecks", "HasReadPermission");
      }
    }

    protected byte[] GenerateUniqueHash()
    {
      byte[] data = new byte[RepositoryConstants.SecurityHashLength];
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
        cryptoServiceProvider.GetBytes(data);
      return data;
    }

    protected bool IsInvalidSecurityHash(byte[] hash) => hash == null || hash.Length != RepositoryConstants.SecurityHashLength || ((IEnumerable<byte>) hash).All<byte>((Func<byte, bool>) (b => b == (byte) 0));

    protected void LoadGitSecurityNamespace(IVssRequestContext collectionRequestContext)
    {
      if (this.m_gitSecurityNamespace != null)
        return;
      lock (this.m_gitSecurityNamespaceLock)
      {
        if (this.m_gitSecurityNamespace != null)
          return;
        SecurityChecksUtils.LoadRemoteSecurityNamespace(collectionRequestContext, GitConstants.GitSecurityNamespaceId);
        this.m_gitSecurityNamespace = SecurityChecksUtils.GetSecurityNamespace(collectionRequestContext, GitConstants.GitSecurityNamespaceId);
      }
    }

    protected void GetAllUserAccessibleRepositories(
      IVssRequestContext userRequestContext,
      string projectIdentifier,
      int numberOfReposToCheckInASecuritySet,
      out List<byte[]> allAccessibleRepoSetHashList,
      out List<GitRepositoryData> accessibleRepos,
      out int numRssWithMismatchedAccess,
      out int numExceptions)
    {
      numRssWithMismatchedAccess = 0;
      numExceptions = 0;
      accessibleRepos = new List<GitRepositoryData>();
      allAccessibleRepoSetHashList = new List<byte[]>();
      try
      {
        this.RepoSecuritySetsLock.EnterReadLock();
        using (Dictionary<byte[], List<GitRepositoryData>>.KeyCollection.Enumerator enumerator = this.RepositorySecuritySets.Keys.GetEnumerator())
        {
label_13:
          while (enumerator.MoveNext())
          {
            byte[] current = enumerator.Current;
            List<GitRepositoryData> repositorySecuritySet = this.RepositorySecuritySets[current];
            int count = repositorySecuritySet.Count;
            try
            {
              GitRepositoryData repo1 = repositorySecuritySet.First<GitRepositoryData>();
              if (this.GitRepoHasReadPermission(userRequestContext, repo1))
              {
                accessibleRepos.AddRange(this.FilterGitReposByProject((IEnumerable<GitRepositoryData>) repositorySecuritySet, projectIdentifier));
                allAccessibleRepoSetHashList.Add(current);
              }
              else
              {
                Random random = new Random();
                int num = 1;
                while (true)
                {
                  if (num < numberOfReposToCheckInASecuritySet)
                  {
                    if (num < count)
                    {
                      int index = count > numberOfReposToCheckInASecuritySet ? random.Next(1, count) : num;
                      GitRepositoryData repo2 = repositorySecuritySet.ElementAt<GitRepositoryData>(index);
                      if (!this.GitRepoHasReadPermission(userRequestContext, repo2))
                        ++num;
                      else
                        break;
                    }
                    else
                      goto label_13;
                  }
                  else
                    goto label_13;
                }
                ++numRssWithMismatchedAccess;
                accessibleRepos.AddRange(this.FilterGitReposByProject((IEnumerable<GitRepositoryData>) repositorySecuritySet, projectIdentifier));
                allAccessibleRepoSetHashList.Add(current);
              }
            }
            catch (Exception ex)
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1081340, "Query Pipeline", "SecurityChecks", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Ignoring exception during access checks: {0}\n{1}", (object) ex.Message, (object) ex.StackTrace));
              ++numExceptions;
            }
          }
        }
      }
      finally
      {
        this.RepoSecuritySetsLock.ExitReadLock();
      }
    }

    protected void LoadSearchSecurityNamespace(IVssRequestContext requestContext)
    {
      if (this.SearchSecurityNamespace != null)
        return;
      lock (this.m_searchSecurityNamespaceLock)
      {
        if (this.SearchSecurityNamespace != null)
          return;
        this.SearchSecurityNamespace = SecurityChecksUtils.GetSecurityNamespace(requestContext, CommonConstants.SearchSecurityNamespaceGuid);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IEnumerable<GitRepositoryData> FilterGitReposByProject(
      IEnumerable<GitRepositoryData> repoList,
      string projectIdentifier)
    {
      if (projectIdentifier == null)
        return repoList;
      Guid projectGuid;
      bool isProjectGuid = Guid.TryParse(projectIdentifier, out projectGuid);
      return repoList.Where<GitRepositoryData>((Func<GitRepositoryData, bool>) (r =>
      {
        if (isProjectGuid && r.ProjectId == projectGuid)
          return true;
        return !isProjectGuid && string.Equals(r.ProjectName, projectIdentifier, StringComparison.OrdinalIgnoreCase);
      })).Select<GitRepositoryData, GitRepositoryData>((Func<GitRepositoryData, GitRepositoryData>) (r => r));
    }

    protected void TryGettingUserAccessibleReposFromCache(
      IVssRequestContext userRequestContext,
      ICodeSecurityChecksCache m_securityCache,
      string projectIdentifier,
      double SecurityChecksCacheHitKpi,
      double SecurityChecksCacheMissKpi,
      out List<byte[]> allAccessibleRepoSetHashList,
      out List<GitRepositoryData> accessibleRepos,
      out bool isCacheHit)
    {
      isCacheHit = false;
      accessibleRepos = new List<GitRepositoryData>();
      allAccessibleRepoSetHashList = new List<byte[]>();
      if (m_securityCache.TryGetUserRepositorySets(userRequestContext, out allAccessibleRepoSetHashList))
      {
        try
        {
          List<GitRepositoryData> repoList = new List<GitRepositoryData>();
          this.RepoSecuritySetsLock.EnterReadLock();
          foreach (byte[] key in allAccessibleRepoSetHashList)
          {
            List<GitRepositoryData> collection;
            if (this.RepositorySecuritySets.TryGetValue(key, out collection))
              repoList.AddRange((IEnumerable<GitRepositoryData>) collection);
          }
          accessibleRepos.AddRange(this.FilterGitReposByProject((IEnumerable<GitRepositoryData>) repoList, projectIdentifier));
          isCacheHit = true;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityChecksCacheHit", "Query Pipeline", SecurityChecksCacheHitKpi);
        }
        finally
        {
          this.RepoSecuritySetsLock.ExitReadLock();
        }
      }
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityChecksCacheHit", "Query Pipeline", SecurityChecksCacheMissKpi);
    }

    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<RepoSecuritySetsLock>k__BackingField", Justification = "False positive: Dispose is happening through the property.")]
    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && this.RepoSecuritySetsLock != null)
      {
        this.RepoSecuritySetsLock.Dispose();
        this.RepoSecuritySetsLock = (ReaderWriterLockSlim) null;
      }
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
