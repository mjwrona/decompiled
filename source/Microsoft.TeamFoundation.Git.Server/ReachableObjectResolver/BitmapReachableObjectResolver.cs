// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver.BitmapReachableObjectResolver
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver
{
  internal class BitmapReachableObjectResolver : IReachableObjectResolver
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly ICommitReachabilityProvider m_reachProvider;
    private const string c_layer = "BitmapReachableObjectResolver";

    internal BitmapReachableObjectResolver(
      IVssRequestContext requestContext,
      ICommitReachabilityProvider provider)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ICommitReachabilityProvider>(provider, nameof (provider));
      this.m_requestContext = requestContext;
      this.m_reachProvider = provider;
      this.Stats = new BitmapReachableObjectResolver.Statistics();
    }

    public ISet<Sha1Id> Resolve(
      ITfsGitRepository repository,
      ISet<Sha1Id> haves,
      ISet<Sha1Id> wants1,
      ICollection<Sha1Id> shallows,
      GitObjectFilter filter,
      bool wantsPreExpanded,
      out ISet<Sha1Id> foundHaves,
      IObserver<int> statusObserver)
    {
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<ISet<Sha1Id>>(haves, nameof (haves));
      ArgumentUtility.CheckForNull<ISet<Sha1Id>>(wants1, nameof (wants));
      ArgumentUtility.CheckForNull<ICollection<Sha1Id>>(shallows, nameof (shallows));
      if (shallows.Count > 0)
        return new ObjectDBReachableObjectResolver2(this.m_requestContext).Resolve(repository, haves, wants1, shallows, filter, wantsPreExpanded, out foundHaves, statusObserver);
      BitmapReachableObjectResolver.FilteredHavesAndWants filteredHavesAndWants = this.GetFilteredHavesAndWants(repository, (IEnumerable<Sha1Id>) haves, (IEnumerable<Sha1Id>) wants1);
      ICommitReachabilityProvider reachProvider = this.m_reachProvider;
      this.Stats.TotalNumObjects = reachProvider.ObjectList.Count;
      Stopwatch stopwatch = Stopwatch.StartNew();
      IReadOnlyBitmap<Sha1Id> foundBitmap = (IReadOnlyBitmap<Sha1Id>) reachProvider.GetReachableIndexSet((IEnumerable<Sha1Id>) filteredHavesAndWants.HaveCommits);
      this.Stats.FindHavesMillis = (double) stopwatch.ElapsedMilliseconds;
      stopwatch.Restart();
      IBitmap<Sha1Id> bitmapResult = reachProvider.GetReachableIndexSet((IEnumerable<Sha1Id>) filteredHavesAndWants.WantCommits, (IEnumerable<Sha1Id>) filteredHavesAndWants.HaveCommits, foundBitmap, statusObserver);
      this.Stats.FindWantsMillis = (double) stopwatch.ElapsedMilliseconds;
      foreach (Sha1Id wantedLooseObject in filteredHavesAndWants.WantedLooseObjects)
        bitmapResult.Add(wantedLooseObject);
      ObjectWalker objectWalker = new ObjectWalker();
      Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool> filterAction = (Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool>) null;
      filterAction = this.m_requestContext.IsFeatureEnabled("Git.DisallowShallowAndPartialClone") ? (Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool>) ((id, objectType, wants2) => filter.Include(objectType)) : (Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool>) ((id, objectType, wants3) => filter.Include(id, objectType, wants3));
      HashSet<TfsGitTree> wantTrees = filteredHavesAndWants.WantTrees;
      Func<Sha1Id, GitObjectType, bool> walkCondition = (Func<Sha1Id, GitObjectType, bool>) ((id, objectType) =>
      {
        if (!filterAction(id, objectType, wants1) || foundBitmap.Contains(id) || !bitmapResult.Add(id))
          return false;
        statusObserver?.OnNext(bitmapResult.Count);
        return true;
      });
      objectWalker.Walk((IEnumerable<TfsGitObject>) wantTrees, walkCondition);
      ISet<Sha1Id> hashSet = (ISet<Sha1Id>) bitmapResult.ToHashSet<Sha1Id>();
      ISet<Sha1Id> sha1IdSet = filter.Filter(hashSet, wants1, repository);
      statusObserver?.OnNext(sha1IdSet.Count);
      statusObserver?.OnCompleted();
      foundHaves = (ISet<Sha1Id>) foundBitmap;
      return sha1IdSet;
    }

    public BitmapReachableObjectResolver.Statistics Stats { get; }

    private BitmapReachableObjectResolver.FilteredHavesAndWants GetFilteredHavesAndWants(
      ITfsGitRepository repo,
      IEnumerable<Sha1Id> haves,
      IEnumerable<Sha1Id> wants)
    {
      BitmapReachableObjectResolver.FilteredHavesAndWants filteredHavesAndWants = new BitmapReachableObjectResolver.FilteredHavesAndWants();
      foreach (Sha1Id have in haves)
      {
        TfsGitCommit commit = repo.LookupObject(have).TryResolveToCommit();
        if (commit != null)
          filteredHavesAndWants.HaveCommits.Add(commit.ObjectId);
      }
      foreach (Sha1Id want in wants)
      {
        TfsGitObject tfsGitObject = repo.TryLookupObject(want);
        if (tfsGitObject != null)
        {
          for (; tfsGitObject.ObjectType == GitObjectType.Tag; tfsGitObject = ((TfsGitTag) tfsGitObject).GetReferencedObject())
            filteredHavesAndWants.WantedLooseObjects.Add(tfsGitObject.ObjectId);
          if (tfsGitObject.ObjectType == GitObjectType.Commit)
            filteredHavesAndWants.WantCommits.Add(tfsGitObject.ObjectId);
          else if (tfsGitObject.ObjectType == GitObjectType.Tree)
            filteredHavesAndWants.WantTrees.Add((TfsGitTree) tfsGitObject);
          else if (tfsGitObject.ObjectType == GitObjectType.Blob)
            filteredHavesAndWants.WantedLooseObjects.Add(tfsGitObject.ObjectId);
        }
      }
      return filteredHavesAndWants;
    }

    public class Statistics
    {
      public int TotalNumObjects { get; set; }

      public double FindWantsMillis { get; set; }

      public double FindHavesMillis { get; set; }
    }

    private class FilteredHavesAndWants
    {
      public FilteredHavesAndWants()
      {
        this.HaveCommits = new HashSet<Sha1Id>();
        this.WantCommits = new HashSet<Sha1Id>();
        this.WantTrees = new HashSet<TfsGitTree>();
        this.WantedLooseObjects = new HashSet<Sha1Id>();
      }

      public HashSet<Sha1Id> HaveCommits { get; }

      public HashSet<Sha1Id> WantCommits { get; }

      public HashSet<TfsGitTree> WantTrees { get; }

      public HashSet<Sha1Id> WantedLooseObjects { get; }

      public IEnumerable<Sha1Id> AllCommits => this.HaveCommits.Concat<Sha1Id>((IEnumerable<Sha1Id>) this.WantCommits);
    }
  }
}
