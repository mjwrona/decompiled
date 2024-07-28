// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitDataFileProviderService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitDataFileProviderService : VssCacheBase, IVssFrameworkService
  {
    private object m_lock;
    private Dictionary<GitDataFileProviderService.EntryKey, RefCount<GitDataFile>> m_entries;
    private PriorityQueue<GitDataFileProviderService.EntryKey, long> m_lruQueue;
    private Dictionary<GitDataFileProviderService.EntryKey, object> m_retrieveLocks;
    private TeamFoundationTask m_collectIncrementalTask;
    private object m_lockDeletableFiles;
    private HashSet<GitDataFileProviderService.EntryKey> m_deletableFiles;
    private int m_daysBeforeFilesAreDeletable;
    private int m_intervalsBeforeFilesAreDeletable;
    private const string c_layer = "GitDataFileProviderService";
    private const int c_collectLruMillis = 60000;
    private long m_collectLruSpanTicks;
    private static readonly long s_lruUpdatePeriodTicks = 2L * Stopwatch.Frequency;
    private static readonly RegistryQuery s_registryQuery = new RegistryQuery("/Configuration/Caching/MemoryCache/GitDataFileProviderService/InactivityInterval");

    public void ServiceStart(IVssRequestContext systemRC)
    {
      systemRC.CheckProjectCollectionRequestContext();
      IVssRegistryService service = systemRC.GetService<IVssRegistryService>();
      service.RegisterNotification(systemRC, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, in GitDataFileProviderService.s_registryQuery);
      systemRC.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRC, "Default", StorageUtils.ClearDeletableFilesNotificationId, new SqlNotificationCallback(this.OnClearDeletableFilesForOdb), false);
      this.m_lock = new object();
      this.m_entries = new Dictionary<GitDataFileProviderService.EntryKey, RefCount<GitDataFile>>((IEqualityComparer<GitDataFileProviderService.EntryKey>) EqualityComparer<GitDataFileProviderService.EntryKey>.Default);
      this.m_lruQueue = new PriorityQueue<GitDataFileProviderService.EntryKey, long>((IEqualityComparer<GitDataFileProviderService.EntryKey>) EqualityComparer<GitDataFileProviderService.EntryKey>.Default, (IComparer<long>) Comparer<long>.Default);
      this.m_retrieveLocks = new Dictionary<GitDataFileProviderService.EntryKey, object>();
      this.m_lockDeletableFiles = new object();
      this.m_deletableFiles = new HashSet<GitDataFileProviderService.EntryKey>((IEqualityComparer<GitDataFileProviderService.EntryKey>) EqualityComparer<GitDataFileProviderService.EntryKey>.Default);
      this.m_daysBeforeFilesAreDeletable = Math.Max(service.GetValue<int>(systemRC, (RegistryQuery) "/Service/Git/Settings/NumDaysBeforeFilesAreDeletable", 7), 1);
      this.m_intervalsBeforeFilesAreDeletable = Math.Max(service.GetValue<int>(systemRC, (RegistryQuery) "/Service/Git/Settings/NumberOfIntervalsBeforeFilesAreDeletable", 2), 1);
      Interlocked.Exchange(ref this.m_collectLruSpanTicks, this.GetExpirationTicksFromRegistry(systemRC));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.m_collectIncrementalTask = new TeamFoundationTask(GitDataFileProviderService.\u003C\u003EO.\u003C0\u003E__CollectIncremental ?? (GitDataFileProviderService.\u003C\u003EO.\u003C0\u003E__CollectIncremental = new TeamFoundationTaskCallback(GitDataFileProviderService.CollectIncremental)), (object) this, 60000);
      systemRC.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(systemRC.ServiceHost.InstanceId, this.m_collectIncrementalTask);
    }

    public void ServiceEnd(IVssRequestContext systemRC)
    {
      systemRC.GetService<IVssRegistryService>().UnregisterNotification(systemRC, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      systemRC.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRC, "Default", StorageUtils.ClearDeletableFilesNotificationId, new SqlNotificationCallback(this.OnClearDeletableFilesForOdb), false);
      systemRC.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(systemRC.ServiceHost.InstanceId, this.m_collectIncrementalTask);
      this.m_collectIncrementalTask = (TeamFoundationTask) null;
      if (this.m_lock != null)
      {
        bool lockTaken = false;
        try
        {
          Monitor.TryEnter(this.m_lock, ref lockTaken);
          if (!lockTaken)
          {
            string message = "GitDataFileProviderService.ServiceEnd: !lockWasTaken";
            systemRC.Trace(1013559, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), message);
          }
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(this.m_lock);
        }
        this.Clear();
        this.m_lock = (object) null;
      }
      int count = this.m_entries.Count;
      if (count != 0)
      {
        string message = string.Format("{0}.{1}: {2} == {3}", (object) nameof (GitDataFileProviderService), (object) nameof (ServiceEnd), (object) "count", (object) count);
        systemRC.Trace(1013397, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), message);
      }
      this.m_entries = (Dictionary<GitDataFileProviderService.EntryKey, RefCount<GitDataFile>>) null;
      this.m_retrieveLocks = (Dictionary<GitDataFileProviderService.EntryKey, object>) null;
      this.m_lruQueue = (PriorityQueue<GitDataFileProviderService.EntryKey, long>) null;
      this.m_lockDeletableFiles = (object) null;
      this.m_deletableFiles = (HashSet<GitDataFileProviderService.EntryKey>) null;
    }

    public IGitDataFileProvider Create(
      IVssRequestContext rc,
      OdbId odbId,
      ITfsGitBlobProvider blobPrv,
      Lazy<GitOdbSettings> odbSettings)
    {
      return (IGitDataFileProvider) new GitDataFileProviderService.Provider(this, rc, odbId, blobPrv, odbSettings);
    }

    private static void CollectIncremental(IVssRequestContext rc, object thisAsObject)
    {
      rc.TraceEnter(1013136, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), nameof (CollectIncremental));
      ((GitDataFileProviderService) thisAsObject).Collect();
      rc.TraceLeave(1013137, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), nameof (CollectIncremental));
    }

    private RefCount<GitDataFile>.Handle Lookup(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      string resourceId)
    {
      GitDataFileProviderService.EntryKey key = new GitDataFileProviderService.EntryKey(odbId, resourceId);
      RefCount<GitDataFile>.Handle handle1 = (RefCount<GitDataFile>.Handle) null;
      GitDataFileProviderService.EntryKey entryKey = new GitDataFileProviderService.EntryKey(odbId, string.Empty);
      rc.TraceEnter(1013923, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "CheckDeletableFiles." + resourceId);
      try
      {
        lock (this.m_lockDeletableFiles)
        {
          if (!this.m_deletableFiles.Contains(entryKey))
          {
            this.m_deletableFiles.UnionWith(new SqlGitKnownFilesProvider(rc, odbId).ReadInterval(new DateTime?(DateTime.UtcNow.AddDays((double) -this.m_daysBeforeFilesAreDeletable)), new int?(this.m_intervalsBeforeFilesAreDeletable)).Select<KeyValuePair<string, KnownFileIntervalData>, GitDataFileProviderService.EntryKey>((Func<KeyValuePair<string, KnownFileIntervalData>, GitDataFileProviderService.EntryKey>) (df => new GitDataFileProviderService.EntryKey(odbId, df.Key))));
            this.m_deletableFiles.Add(entryKey);
          }
          if (this.m_deletableFiles.Contains(key))
          {
            rc.TraceAlways(1013742, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), FormattableString.Invariant(FormattableStringFactory.Create("Git File Is Deletable and In Use.  OdbId: {0} ", (object) odbId)) + FormattableString.Invariant(FormattableStringFactory.Create("FileName: {0}", (object) resourceId)));
            throw new GitFileIsDeletableException(odbId, resourceId);
          }
        }
      }
      finally
      {
        rc.TraceLeave(1013924, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "CheckDeletableFiles." + resourceId);
      }
      object obj1 = (object) null;
      bool lockTaken = false;
      try
      {
        bool flag = true;
        while (true)
        {
          object obj2 = (object) null;
          rc.TraceEnter(1013925, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "AttemptToGetEntryFromCache." + resourceId);
          try
          {
            lock (this.m_lock)
            {
              RefCount<GitDataFile> refCount;
              if (this.m_entries.TryGetValue(key, out refCount))
              {
                this.UpdateLruIfOld(key);
                handle1 = refCount.AcquireHandle();
              }
              else if (!this.m_retrieveLocks.TryGetValue(key, out obj2))
              {
                this.m_retrieveLocks.Add(key, obj1 = new object());
                Monitor.Enter(obj1, ref lockTaken);
                goto label_36;
              }
            }
          }
          finally
          {
            rc.TraceLeave(1013926, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "AttemptToGetEntryFromCache." + resourceId);
          }
          if (flag)
          {
            if (handle1 != null)
              this.NotifyCacheLookupSucceeded();
            else
              this.NotifyCacheLookupFailed();
            flag = false;
          }
          if (handle1 == null)
          {
            this.NotifyCacheItemsReplaced(1, new MemoryCacheOperationStatistics(0, 0L));
            rc.TraceEnter(1013927, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "WaitingToGetEntry." + resourceId);
            try
            {
              lock (obj2)
                ;
            }
            finally
            {
              rc.TraceLeave(1013928, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "WaitingToGetEntry." + resourceId);
            }
          }
          else
            break;
        }
        RefCount<GitDataFile>.Handle handle2 = handle1;
        handle1 = (RefCount<GitDataFile>.Handle) null;
        return handle2;
label_36:
        this.NotifyCacheLookupFailed();
        GitDataFile target = GitDataFileUtil.Retrieve(rc, blobProvider, odbId, resourceId);
        try
        {
          rc.TraceEnter(1013929, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "CacheResult." + resourceId);
          try
          {
            lock (this.m_lock)
            {
              this.m_lruQueue.EnqueueOrUpdate(key, Stopwatch.GetTimestamp());
              RefCount<GitDataFile> refCount = new RefCount<GitDataFile>(target);
              this.m_entries.Add(key, refCount);
              target = (GitDataFile) null;
              handle1 = refCount.AcquireHandle();
            }
          }
          finally
          {
            rc.TraceLeave(1013930, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "CacheResult." + resourceId);
          }
          this.NotifyCacheItemsAdded(1, new MemoryCacheOperationStatistics(1, 0L));
          RefCount<GitDataFile>.Handle handle3 = handle1;
          handle1 = (RefCount<GitDataFile>.Handle) null;
          return handle3;
        }
        finally
        {
          target?.Dispose();
        }
      }
      finally
      {
        if (obj1 != null)
        {
          rc.TraceEnter(1013931, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "RemoveLock");
          try
          {
            lock (this.m_lock)
            {
              if (lockTaken)
                Monitor.Exit(obj1);
              this.m_retrieveLocks.Remove(key);
            }
          }
          finally
          {
            rc.TraceLeave(1013932, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), "RemoveLock");
          }
        }
        handle1?.Dispose();
      }
    }

    private void Collect() => this.Collect(Interlocked.Read(ref this.m_collectLruSpanTicks));

    internal void Clear() => this.Collect(-1L);

    private void Collect(long spanTicksToKeep)
    {
      long num = Stopwatch.GetTimestamp() - spanTicksToKeep;
      List<GitDataFile> gitDataFileList = new List<GitDataFile>();
      lock (this.m_lock)
      {
        GitDataFileProviderService.EntryKey key;
        while (this.m_lruQueue.TryDequeueBeforeThreshold(num, out key))
        {
          RefCount<GitDataFile> entry = this.m_entries[key];
          if (!entry.IsAlive)
          {
            this.m_entries.Remove(key);
            gitDataFileList.Add(entry.DangerousTarget);
          }
          else
            this.m_lruQueue.EnqueueOrUpdate(key, num);
        }
      }
      if (gitDataFileList.Count != 0)
      {
        MemoryCacheOperationStatistics stats = new MemoryCacheOperationStatistics(-gitDataFileList.Count, 0L);
        this.NotifyCacheItemsInvalidated(gitDataFileList.Count, stats);
      }
      foreach (GitDataFile gitDataFile in gitDataFileList)
      {
        gitDataFile.Dispose();
        if (!string.IsNullOrEmpty(gitDataFile.Path))
        {
          try
          {
            File.SetLastAccessTime(gitDataFile.Path, DateTime.Now);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceCatchRaw(1013398, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (GitDataFileProviderService), ex);
          }
        }
      }
    }

    private void UpdateLruIfOld(GitDataFileProviderService.EntryKey key)
    {
      long priority;
      if (!this.m_lruQueue.TryGetPriority(key, out priority))
        return;
      long timestamp = Stopwatch.GetTimestamp();
      if (priority >= timestamp - GitDataFileProviderService.s_lruUpdatePeriodTicks)
        return;
      this.m_lruQueue.EnqueueOrUpdate(key, timestamp);
    }

    private void OnRegistryChanged(IVssRequestContext rc, RegistryEntryCollection changedEntries) => Interlocked.Exchange(ref this.m_collectLruSpanTicks, this.GetExpirationTicksFromRegistry(rc));

    private void OnClearDeletableFilesForOdb(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.ClearFileBlacklistForOdb(OdbId.Parse(eventData));
    }

    public void ClearFileBlacklistForOdb(OdbId odbId)
    {
      lock (this.m_lockDeletableFiles)
        this.m_deletableFiles.RemoveWhere((Predicate<GitDataFileProviderService.EntryKey>) (x => x.OdbId == odbId));
    }

    private long GetExpirationTicksFromRegistry(IVssRequestContext rc) => GitDataFileProviderService.GetStopWatchTicks(rc.GetService<IVssRegistryService>().GetValue<TimeSpan>(rc, in GitDataFileProviderService.s_registryQuery, true, TimeSpan.FromMilliseconds(60000.0)));

    private static long GetStopWatchTicks(TimeSpan span) => Convert.ToInt64(span.TotalSeconds * (double) Stopwatch.Frequency);

    private class Provider : IGitDataFileProvider, IBufferStreamFactory
    {
      private readonly GitDataFileProviderService m_outer;
      private readonly IVssRequestContext m_rc;
      private readonly OdbId m_odbId;
      private readonly ITfsGitBlobProvider m_blobPrv;
      private readonly Lazy<GitOdbSettings> m_odbSettings;

      public Provider(
        GitDataFileProviderService outer,
        IVssRequestContext rc,
        OdbId odbId,
        ITfsGitBlobProvider blobPrv,
        Lazy<GitOdbSettings> odbSettings)
      {
        this.m_outer = outer;
        this.m_rc = rc;
        this.m_odbId = odbId;
        this.m_blobPrv = blobPrv;
        this.m_odbSettings = odbSettings;
      }

      public Stream GetMemoryMappedStream(string resourceId, long offset, long length)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
        RefCount<GitDataFile>.Handle handle = this.m_outer.Lookup(this.m_rc, this.m_blobPrv, this.m_odbId, resourceId);
        try
        {
          SafeBufferStream memoryMappedStream = handle.Target.CreateMemoryMappedStream(offset, length);
          RefCount<GitDataFile>.Handle handleForLambda = handle;
          memoryMappedStream.PushActionOnDispose((Action) (() => handleForLambda.Dispose()));
          handle = (RefCount<GitDataFile>.Handle) null;
          return (Stream) memoryMappedStream;
        }
        finally
        {
          handle?.Dispose();
        }
      }

      public Stream GetStream(string resourceId, long offset, long length)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
        RefCount<GitDataFile>.Handle handle = this.m_outer.Lookup(this.m_rc, this.m_blobPrv, this.m_odbId, resourceId);
        try
        {
          GitRestrictedStream stream = handle.Target.CreateStream(offset, length);
          RefCount<GitDataFile>.Handle handleForLambda = handle;
          stream.PushActionOnDispose((Action) (() => handleForLambda.Dispose()));
          handle = (RefCount<GitDataFile>.Handle) null;
          return (Stream) stream;
        }
        finally
        {
          handle?.Dispose();
        }
      }

      public Stream GetBufferStream(long length) => (Stream) new BufferStream(GitServerUtils.GetCacheDirectory(this.m_rc, this.m_odbId.Value), length, this.m_odbSettings.Value.MaxMemoryStreamBytes, this.m_odbSettings.Value.MaxMemoryMappedFileBytes);
    }

    private struct EntryKey : IEquatable<GitDataFileProviderService.EntryKey>
    {
      public EntryKey(OdbId odbId, string resourceId)
      {
        this.OdbId = odbId;
        this.ResourceId = resourceId;
      }

      public bool Equals(GitDataFileProviderService.EntryKey other) => this.OdbId == other.OdbId && this.ResourceId == other.ResourceId;

      public override bool Equals(object obj) => obj is GitDataFileProviderService.EntryKey other && this.Equals(other);

      public override int GetHashCode() => HashCodeUtil.GetHashCode<OdbId, string>(this.OdbId, this.ResourceId);

      public OdbId OdbId { get; }

      public string ResourceId { get; }
    }
  }
}
