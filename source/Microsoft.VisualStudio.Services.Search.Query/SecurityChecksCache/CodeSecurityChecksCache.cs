// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache.CodeSecurityChecksCache
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache
{
  public class CodeSecurityChecksCache : 
    SecurityChecksCacheBase<RepoHash>,
    ICodeSecurityChecksCache,
    IDisposable
  {
    private const int HashSize = 20;
    private readonly byte[] m_emptyRepoHash;
    private readonly MD5 m_md5;
    private readonly ReaderWriterLockSlim m_userRepoListCacheLock;
    private bool m_disposedValue;

    protected FriendlyDictionary<byte[], CodeSecurityChecksCache.RepoSetsInfo> RepoHashToRepoInfoDict { get; set; }

    [SuppressMessage("Microsoft.Cryptographic.Standard", "CA5350:MD5CannotBeUsed", Target = "m_md5", Justification = "#1581817")]
    public CodeSecurityChecksCache(
      int securityChecksCacheMaxSize,
      TimeSpan securityCheckCacheExpiration,
      TimeSpan securityCheckCacheCleanupTaskInterval,
      TimeSpan securityChecksCacheInactivityExpiration)
      : base(securityChecksCacheMaxSize, securityCheckCacheExpiration, securityCheckCacheCleanupTaskInterval, securityChecksCacheInactivityExpiration)
    {
      this.RepoHashToRepoInfoDict = new FriendlyDictionary<byte[], CodeSecurityChecksCache.RepoSetsInfo>();
      this.m_userRepoListCacheLock = new ReaderWriterLockSlim();
      this.m_md5 = MD5.Create();
      this.m_emptyRepoHash = this.m_md5.ComputeHash(Encoding.ASCII.GetBytes(string.Empty));
    }

    public virtual void UpdateRepositorySetsCache(
      IVssRequestContext userRequestContext,
      List<byte[]> userRepositorySets)
    {
      this.m_userRepoListCacheLock.EnterWriteLock();
      try
      {
        byte[] repoSetsHash = this.GetRepoSetsHash(userRepositorySets);
        RepoHash userData = (RepoHash) null;
        if (this.TryGetUserData(userRequestContext, out userData) && !((IEnumerable<byte>) userData.Hash).SequenceEqual<byte>((IEnumerable<byte>) repoSetsHash))
          --this.RepoHashToRepoInfoDict[userData.Hash].ReferenceCount;
        userData = new RepoHash(repoSetsHash);
        this.UpdateCacheWithUserInfo(userRequestContext, userData);
        CodeSecurityChecksCache.RepoSetsInfo repoSetsInfo1;
        if (this.RepoHashToRepoInfoDict.TryGetValue(repoSetsHash, out repoSetsInfo1))
        {
          ++repoSetsInfo1.ReferenceCount;
        }
        else
        {
          CodeSecurityChecksCache.RepoSetsInfo repoSetsInfo2 = new CodeSecurityChecksCache.RepoSetsInfo()
          {
            Hash = repoSetsHash,
            ReferenceCount = 1,
            RepositorySetHashes = userRepositorySets
          };
          this.RepoHashToRepoInfoDict[repoSetsHash] = repoSetsInfo2;
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081352, "Query Pipeline", "SecurityChecks", ex);
      }
      finally
      {
        this.m_userRepoListCacheLock.ExitWriteLock();
      }
    }

    public virtual bool TryGetUserRepositorySets(
      IVssRequestContext userRequestContext,
      out List<byte[]> userRepositorySets)
    {
      this.m_userRepoListCacheLock.EnterReadLock();
      try
      {
        RepoHash userData;
        if (!this.TryGetUserData(userRequestContext, out userData))
        {
          userRepositorySets = (List<byte[]>) null;
          return false;
        }
        userRepositorySets = this.RepoHashToRepoInfoDict[userData.Hash].RepositorySetHashes;
        return true;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081356, "Query Pipeline", "SecurityChecks", ex);
        userRepositorySets = (List<byte[]>) null;
        return false;
      }
      finally
      {
        this.m_userRepoListCacheLock.ExitReadLock();
      }
    }

    private byte[] GetRepoSetsHash(List<byte[]> userRepositorySetHashes)
    {
      if (userRepositorySetHashes.Count == 0)
        return this.m_emptyRepoHash;
      Stopwatch stopwatch = Stopwatch.StartNew();
      userRepositorySetHashes.Sort(new Comparison<byte[]>(StructuralComparisons.StructuralComparer.Compare));
      byte[] numArray = new byte[20 * userRepositorySetHashes.Count];
      int dstOffset = 0;
      foreach (Array repositorySetHash in userRepositorySetHashes)
      {
        Buffer.BlockCopy(repositorySetHash, 0, (Array) numArray, dstOffset, 20);
        dstOffset += 20;
      }
      byte[] hash = this.m_md5.ComputeHash(numArray);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityChecksGetRepoListHashTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      return hash;
    }

    public override void ClearCache()
    {
      this.m_userRepoListCacheLock.EnterWriteLock();
      try
      {
        base.ClearCache();
        this.RepoHashToRepoInfoDict.Clear();
      }
      finally
      {
        this.m_userRepoListCacheLock.ExitWriteLock();
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing)
      {
        this.m_md5.Dispose();
        this.m_userRepoListCacheLock.Dispose();
      }
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public class RepoSetsInfo
    {
      public RepoSetsInfo()
      {
        this.RepositorySetHashes = new List<byte[]>();
        this.ReferenceCount = 0;
      }

      public List<byte[]> RepositorySetHashes { get; set; }

      public byte[] Hash { get; set; }

      public int ReferenceCount { get; set; }
    }
  }
}
