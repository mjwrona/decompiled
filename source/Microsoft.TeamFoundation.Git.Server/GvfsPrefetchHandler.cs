// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsPrefetchHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [GvfsPublicProjectRequestRestrictions]
  internal class GvfsPrefetchHandler : GvfsHttpHandler
  {
    private readonly IGvfsHandlerDependencyFactory m_depoFactory;
    private readonly Func<ITfsGitRepository, IVssRequestContext, bool, IReachableObjectResolver> m_reachabilityFactory;
    private static readonly byte[] s_prefetchPacksHeader = new byte[6]
    {
      (byte) 71,
      (byte) 80,
      (byte) 82,
      (byte) 69,
      (byte) 32,
      (byte) 1
    };
    private static readonly double s_fullPackCuttoffSeconds = TimeSpan.FromDays(30.0).TotalSeconds;
    private static readonly double s_minSecondsBetweenRequests = TimeSpan.FromHours(1.0).TotalSeconds;

    public GvfsPrefetchHandler()
    {
      this.m_depoFactory = GvfsHandlerDependencyFactory.Instance;
      this.m_reachabilityFactory = (Func<ITfsGitRepository, IVssRequestContext, bool, IReachableObjectResolver>) ((repo, rc, full) =>
      {
        ICommitReachabilityProvider reachabilityProvider = GitServerUtils.GetOdb(repo).ReachabilityProvider;
        return !full || reachabilityProvider == null || rc.IsFeatureEnabled("Git.Bitmap.DoNotUseOnPrefetch") ? (IReachableObjectResolver) new ObjectDBReachableObjectResolver2(rc) : (IReachableObjectResolver) new BitmapReachableObjectResolver(rc, reachabilityProvider);
      });
    }

    public GvfsPrefetchHandler(
      HttpContextBase context,
      IGvfsHandlerDependencyFactory depFactory,
      Func<ITfsGitRepository, IVssRequestContext, bool, IReachableObjectResolver> reachabilityFactory)
      : base(context)
    {
      this.m_depoFactory = depFactory;
      this.m_reachabilityFactory = reachabilityFactory;
    }

    protected override TimeSpan Timeout => TimeSpan.FromHours(24.0);

    protected override string Layer => nameof (GvfsPrefetchHandler);

    internal override void ProcessGet(RepoNameKey nameKey)
    {
      long? latestPackTimestamp = GvfsUtil.GetLatestPackTimestamp(this.HandlerHttpContext.Request, this.RequestContext.ServiceName);
      ClientTraceData eventData = new ClientTraceData();
      using (ITfsGitRepository repository = this.m_depoFactory.GetRepository(this.RequestContext, nameKey))
      {
        long totalSeconds = (long) (DateTime.UtcNow - GitServerConstants.UtcEpoch).TotalSeconds;
        long? nullable1;
        double? nullable2;
        int num1;
        if (latestPackTimestamp.HasValue)
        {
          long num2 = totalSeconds;
          nullable1 = latestPackTimestamp;
          nullable2 = nullable1.HasValue ? new double?((double) (num2 - nullable1.GetValueOrDefault())) : new double?();
          double packCuttoffSeconds = GvfsPrefetchHandler.s_fullPackCuttoffSeconds;
          num1 = nullable2.GetValueOrDefault() > packCuttoffSeconds & nullable2.HasValue ? 1 : 0;
        }
        else
          num1 = 1;
        bool flag = num1 != 0;
        long num3 = totalSeconds;
        nullable1 = latestPackTimestamp;
        nullable2 = nullable1.HasValue ? new double?((double) (num3 - nullable1.GetValueOrDefault())) : new double?();
        double secondsBetweenRequests = GvfsPrefetchHandler.s_minSecondsBetweenRequests;
        ISet<Sha1Id> toSend;
        if (nullable2.GetValueOrDefault() < secondsBetweenRequests & nullable2.HasValue)
          toSend = (ISet<Sha1Id>) new HashSet<Sha1Id>();
        else if (flag && this.RequestContext.IsFeatureEnabled("Git.GVFS.DisableReachablePrefetch"))
        {
          ContentDB contentDb = GitServerUtils.GetContentDB(repository);
          toSend = (ISet<Sha1Id>) new HashSet<Sha1Id>(contentDb.Index.GetObjectIdsByType(GitPackObjectType.Commit).Concat<Sha1Id>(contentDb.Index.GetObjectIdsByType(GitPackObjectType.Tree)));
          eventData.AddGvfsPrefetchCtData(repository, latestPackTimestamp, totalSeconds, 0, 0, toSend.Count);
        }
        else
        {
          HashSet<Sha1Id> wants;
          HashSet<Sha1Id> haves;
          if (flag)
          {
            wants = new HashSet<Sha1Id>(repository.Refs.OptimizedRefHeads().Select<TfsGitRef, Sha1Id>((Func<TfsGitRef, Sha1Id>) (x => x.ObjectId)));
            haves = new HashSet<Sha1Id>();
          }
          else
          {
            GitServerConstants.UtcEpoch.AddSeconds((double) latestPackTimestamp.Value);
            List<Sha1Id> sha1IdList1 = new List<Sha1Id>();
            List<Sha1Id> sha1IdList2 = new List<Sha1Id>();
            IGvfsPrefetchHavesWantsProvider havesWantsProvider = this.m_depoFactory.GetPrefetchHavesWantsProvider(this.RequestContext, repository);
            eventData.Add("PrefetchHavesWantsSource", (object) havesWantsProvider.GetType().Name);
            (haves, wants) = havesWantsProvider.ReadHavesWantsSince(latestPackTimestamp.Value);
          }
          toSend = this.m_reachabilityFactory(repository, this.RequestContext, flag).Resolve(repository, (ISet<Sha1Id>) haves, (ISet<Sha1Id>) wants, (ICollection<Sha1Id>) new HashSet<Sha1Id>(), new GitObjectFilter(), false, out ISet<Sha1Id> _, (IObserver<int>) null);
          eventData.AddGvfsPrefetchCtData(repository, latestPackTimestamp, totalSeconds, haves.Count, wants.Count, toSend.Count);
        }
        this.WriteResponse(eventData, repository, totalSeconds, toSend);
        eventData.PublishGvfsPrefetchCtData(this.RequestContext);
      }
    }

    private void WriteResponse(
      ClientTraceData eventData,
      ITfsGitRepository repo,
      long newPackTimestamp,
      ISet<Sha1Id> toSend)
    {
      HttpResponseBase response = this.HandlerHttpContext.Response;
      response.StatusCode = 200;
      this.ResponseStarted = true;
      response.OutputStream.Write(GvfsPrefetchHandler.s_prefetchPacksHeader, 0, GvfsPrefetchHandler.s_prefetchPacksHeader.Length);
      byte[] numArray = new byte[26];
      if (toSend.Count == 0)
      {
        response.OutputStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
      }
      else
      {
        Array.Copy((Array) BitConverter.GetBytes((ushort) 1), (Array) numArray, 2);
        Array.Copy((Array) BitConverter.GetBytes(newPackTimestamp), 0, (Array) numArray, 2, 8);
        Array.Copy((Array) BitConverter.GetBytes(long.MaxValue), 0, (Array) numArray, 10, 8);
        Array.Copy((Array) BitConverter.GetBytes(-1L), 0, (Array) numArray, 18, 8);
        response.OutputStream.Write(numArray, 0, numArray.Length);
        bool isTarpit;
        using (GitPackWriterThrottler throttler = new GitPackWriterThrottler(this.RequestContext, toSend.Count, out isTarpit))
          this.m_depoFactory.GetPackWriter(this.RequestContext, repo, throttler, eventData).Write(toSend, response.OutputStream, beforeStreamingResponse: new Action(((VssRequestContextExtensions) this.RequestContext).UpdateTimeToFirstPage), objectsToIncludeFilter: (Predicate<ObjectIdAndGitPackIndexEntry>) (x => x.ObjectType != GitPackObjectType.Blob));
      }
    }
  }
}
