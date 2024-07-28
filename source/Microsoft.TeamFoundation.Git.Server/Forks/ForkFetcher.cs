// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Forks.ForkFetcher
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Forks
{
  internal sealed class ForkFetcher
  {
    private readonly IVssRequestContext m_rc;
    private readonly IGitForkService m_forkSvc;
    private readonly ITeamFoundationGitRepositoryService m_repoSvc;
    private readonly ITeamFoundationGitRefService m_refSvc;
    private readonly ISettingsService m_settingsSvc;
    private const string c_layer = "ForkFetcher";

    public ForkFetcher(
      IVssRequestContext rc,
      IGitForkService forkSvc,
      ITeamFoundationGitRepositoryService repoSvc,
      ITeamFoundationGitRefService refSvc,
      ISettingsService settingsSvc)
    {
      this.m_rc = rc;
      this.m_forkSvc = forkSvc;
      this.m_repoSvc = repoSvc;
      this.m_refSvc = refSvc;
      this.m_settingsSvc = settingsSvc;
    }

    public void PerformFetch(ForkFetchAsyncOp fetchRequest, ClientTraceData ctData)
    {
      ForkFetchParams parameters = fetchRequest.Parameters;
      RepoKey key1;
      using (ITfsGitRepository repositoryById = this.m_repoSvc.FindRepositoryById(this.m_rc, parameters.TargetRepoId))
        key1 = repositoryById.Key;
      GlobalGitRepositoryKey source = parameters.Source;
      if (source.CollectionId != Guid.Empty && source.CollectionId != this.m_rc.ServiceHost.InstanceId)
        throw new ArgumentException("Cross-collection forking is unsupported.");
      ctData?.Add("SourceRepositoryId", (object) source.RepositoryId);
      ctData?.Add("SourceCollectionId", (object) source.CollectionId);
      this.m_forkSvc.UpdateFetchProgress(this.m_rc, key1, fetchRequest.OperationId, GitAsyncOperationStatus.InProgress, ForkUpdateStep.Started);
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      string str1 = (string) null;
      string refName = (string) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      using (ITfsGitRepository repositoryById1 = this.m_repoSvc.FindRepositoryById(this.m_rc, source.RepositoryId))
      {
        using (ITfsGitRepository repositoryById2 = this.m_repoSvc.FindRepositoryById(this.m_rc, key1.RepoId))
        {
          if (repositoryById1.Key.OdbId != key1.OdbId)
            throw new ArgumentException("Source and target repos must share an ODB");
          List<TfsGitRef> tfsGitRefList;
          if (parameters.SourceToTargetRefs != null)
          {
            tfsGitRefList = repositoryById1.Refs.MatchingNames(parameters.SourceToTargetRefs.Select<SourceToTargetRef, string>((Func<SourceToTargetRef, string>) (r => r.SourceRef)));
            dictionary1 = parameters.SourceToTargetRefs.ToDictionary<SourceToTargetRef, string, string>((Func<SourceToTargetRef, string>) (r => r.SourceRef), (Func<SourceToTargetRef, string>) (r => r.TargetRef));
          }
          else
            tfsGitRefList = repositoryById1.Settings.OptimizedByDefault ? repositoryById1.Refs.Limited() : repositoryById1.Refs.All();
          Dictionary<string, Sha1Id> dictionary2 = this.FilterToNonSystemRefs((IEnumerable<TfsGitRef>) tfsGitRefList).ToDictionary<TfsGitRef, string, Sha1Id>((Func<TfsGitRef, string>) (r => r.Name), (Func<TfsGitRef, Sha1Id>) (r => r.ObjectId));
          ctData?.Add("TotalSourceToTargetRefsCount", (object) (parameters.SourceToTargetRefs != null ? parameters.SourceToTargetRefs.Count : 0));
          ctData?.Add("TotalSourceRefsCount", (object) tfsGitRefList.Count);
          ctData?.Add("RefsCopiedCount", (object) dictionary2.Count);
          if (parameters.CopySourceRepoDefaults)
          {
            string name = tfsGitRefList.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (r => r.IsDefaultBranch))?.Name;
            if (name != null)
            {
              refName = name;
              if (parameters.SourceToTargetRefs != null)
                refName = dictionary1[name];
            }
            str1 = this.m_settingsSvc.GetValue<string>(this.m_rc, SettingsUserScope.User, "Repository", repositoryById1.Key.RepoId.ToString(), "Branches.Compare", (string) null);
            ctData?.Add("DefaultRef", (object) name);
            ctData?.Add("CompareRef", (object) str1);
          }
          ctData?.Add("ReadRefsMs", (object) stopwatch.ElapsedMilliseconds);
          if (dictionary2.Keys.Count == 0)
          {
            this.MarkFinished(key1, fetchRequest.OperationId, GitAsyncOperationStatus.Completed);
          }
          else
          {
            stopwatch.Restart();
            Dictionary<string, Sha1Id> dictionary3 = parameters.SourceToTargetRefs == null ? repositoryById2.Refs.MatchingNames((IEnumerable<string>) dictionary2.Keys).ToDictionary<TfsGitRef, string, Sha1Id>((Func<TfsGitRef, string>) (r => r.Name), (Func<TfsGitRef, Sha1Id>) (r => r.ObjectId)) : repositoryById2.Refs.MatchingNames(parameters.SourceToTargetRefs.Select<SourceToTargetRef, string>((Func<SourceToTargetRef, string>) (r => r.TargetRef))).ToDictionary<TfsGitRef, string, Sha1Id>((Func<TfsGitRef, string>) (r => r.Name), (Func<TfsGitRef, Sha1Id>) (r => r.ObjectId));
            ctData?.Add("MatchRefsMs", (object) stopwatch.ElapsedMilliseconds);
            if (this.m_rc.IsFeatureEnabled("Git.IsolationBitmap.Write"))
            {
              stopwatch.Restart();
              IIsolationBitmapProvider isolationBitmapProvider = GitServerUtils.GetIsolationBitmapProvider(repositoryById2);
              ISet<Sha1Id> odb = (ISet<Sha1Id>) isolationBitmapProvider.GetOdb();
              isolationBitmapProvider.AddOdbObjectsAndSerialize((IEnumerable<Sha1Id>) new BitmapReachableObjectResolver(this.m_rc, GitServerUtils.GetOdb(repositoryById1).ReachabilityProvider).Resolve(repositoryById1, odb ?? (ISet<Sha1Id>) new HashSet<Sha1Id>(), (ISet<Sha1Id>) dictionary2.Select<KeyValuePair<string, Sha1Id>, Sha1Id>((Func<KeyValuePair<string, Sha1Id>, Sha1Id>) (r => r.Value)).ToHashSet<Sha1Id>(), (ICollection<Sha1Id>) Array.Empty<Sha1Id>(), new GitObjectFilter(), false, out ISet<Sha1Id> _, (IObserver<int>) null));
              ctData?.Add("UpdateBitmapMs", (object) stopwatch.ElapsedMilliseconds);
            }
            stopwatch.Restart();
            List<TfsGitRefUpdateRequest> refUpdates = new List<TfsGitRefUpdateRequest>(dictionary3.Keys.Count);
            foreach (string key2 in dictionary2.Keys)
            {
              string str2 = key2;
              if (parameters.SourceToTargetRefs != null)
              {
                if (dictionary1.ContainsKey(key2))
                  str2 = dictionary1[key2];
                else
                  this.m_rc.TraceAlways(1013750, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (ForkFetcher), "Source ref name '{0}' wasn't found in sourceToTargetRefs dictionary. Assuming targetRefName equals sourceRefName.", (object) key2);
              }
              Sha1Id empty;
              if (!dictionary3.TryGetValue(str2, out empty))
                empty = Sha1Id.Empty;
              refUpdates.Add(new TfsGitRefUpdateRequest(str2, empty, dictionary2[key2]));
            }
            TfsGitRefUpdateResultSet refUpdateResultSet = this.m_refSvc.UpdateRefs(this.m_rc, key1.RepoId, refUpdates);
            foreach (TfsGitRefUpdateResult gitRefUpdateResult in refUpdateResultSet.Results.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (r => !r.Succeeded)))
              this.m_rc.Trace(1013730, TraceLevel.Error, GitServerUtils.TraceArea, nameof (ForkFetcher), "Ref update {0} failed: {1} {2}", (object) gitRefUpdateResult.Name, (object) gitRefUpdateResult.Status, (object) gitRefUpdateResult.CustomMessage);
            ctData?.Add("UpdateRefsMs", (object) stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
            if (parameters.CopySourceRepoDefaults)
            {
              if (refName != null)
                repositoryById2.Refs.SetDefault(refName);
              if (str1 != null)
                this.m_settingsSvc.SetValue(this.m_rc, SettingsUserScope.User, "Repository", repositoryById2.Key.RepoId.ToString(), "Branches.Compare", (object) str1);
              ctData?.Add("SetDefaultAndCompareMs", (object) stopwatch.ElapsedMilliseconds);
            }
            this.MarkFinished(key1, fetchRequest.OperationId, refUpdateResultSet.CountFailed > 0 ? GitAsyncOperationStatus.Failed : GitAsyncOperationStatus.Completed);
          }
        }
      }
    }

    private void MarkFinished(
      RepoKey targetRepoKey,
      int asyncOperationId,
      GitAsyncOperationStatus operationStatus)
    {
      this.m_forkSvc.UpdateFetchProgress(this.m_rc, targetRepoKey, asyncOperationId, operationStatus, ForkUpdateStep.Finished);
    }

    public IEnumerable<TfsGitRef> FilterToNonSystemRefs(IEnumerable<TfsGitRef> refs)
    {
      foreach (TfsGitRef nonSystemRef in refs)
      {
        if (!nonSystemRef.Name.StartsWith("refs/pull/", StringComparison.Ordinal))
          yield return nonSystemRef;
      }
    }
  }
}
