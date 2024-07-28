// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CodeMigrator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class CodeMigrator
  {
    private readonly ITraceRequest m_tracer;
    private readonly IReachableObjectResolver m_ror;
    private readonly Func<ITfsGitRepository, IGitPackWriter> m_getPackWriter;
    private readonly Func<RepoKey, RepoKey, IProgress<ReceivePackStep>> m_getPushProgress;
    private readonly Func<RepoKey, List<TfsGitRefUpdateRequest>, TfsGitRefUpdateResultSet> m_updateRefs;

    public CodeMigrator(
      ITraceRequest tracer,
      IReachableObjectResolver ror,
      Func<ITfsGitRepository, IGitPackWriter> getPackWriter,
      Func<RepoKey, RepoKey, IProgress<ReceivePackStep>> getPushProgress,
      Func<RepoKey, List<TfsGitRefUpdateRequest>, TfsGitRefUpdateResultSet> updateRefs)
    {
      this.m_tracer = tracer;
      this.m_ror = ror;
      this.m_getPackWriter = getPackWriter;
      this.m_getPushProgress = getPushProgress;
      this.m_updateRefs = updateRefs;
    }

    public void PerformPartialMigrate(
      ITfsGitRepository source,
      ITfsGitRepository target,
      RepoMigrationRequest migrationRequest)
    {
      TfsGitRef[] targetParents = (TfsGitRef[]) null;
      Dictionary<string, Sha1Id> toMigrate = (Dictionary<string, Sha1Id>) null;
      Dictionary<string, Sha1Id> createdCommits = (Dictionary<string, Sha1Id>) null;
      Dictionary<string, BranchMigrationData> toRename = new Dictionary<string, BranchMigrationData>();
      IProgress<ReceivePackStep> pushWatcher = this.m_getPushProgress(source.Key, target.Key);
      try
      {
        this.ParseRefDataFromRequest(source, target, migrationRequest, ref targetParents, ref toMigrate, ref toRename);
        createdCommits = this.CreateCommits(source, target, targetParents, toMigrate, toRename, pushWatcher);
        ISet<Sha1Id> deltaToPush = this.CalculateDeltaToPush(source, target, createdCommits);
        this.IngestUpdates(source, target, createdCommits, toMigrate, toRename, deltaToPush, pushWatcher);
      }
      finally
      {
        this.UpdateProgressRefs(source, target, createdCommits, toMigrate, toRename);
      }
    }

    private void ParseRefDataFromRequest(
      ITfsGitRepository source,
      ITfsGitRepository target,
      RepoMigrationRequest migrationRequest,
      ref TfsGitRef[] targetParents,
      ref Dictionary<string, Sha1Id> toMigrate,
      ref Dictionary<string, BranchMigrationData> toRename)
    {
      targetParents = new TfsGitRef[migrationRequest.ParentRefNames.Length];
      for (int index = 0; index < migrationRequest.ParentRefNames.Length; ++index)
      {
        targetParents[index] = target.Refs.MatchingName(migrationRequest.ParentRefNames[index]);
        if (targetParents[index] == null)
          throw new ArgumentException(migrationRequest.ParentRefNames[index] + " has not yet been migrated and must be before any leaf branches can be moved.");
      }
      foreach (string str in migrationRequest.BranchesToMigrate)
      {
        BranchMigrationData branchMigrationData = MigrationBranchParser.Parse(str, migrationRequest.MigrationBranchPrefix);
        toRename[str] = branchMigrationData;
      }
      List<TfsGitRef> source1 = source.Refs.MatchingNames((IEnumerable<string>) migrationRequest.BranchesToMigrate);
      List<string> list = source1.Where<TfsGitRef>((Func<TfsGitRef, bool>) (r => !r.IsLockedById.HasValue)).Select<TfsGitRef, string>((Func<TfsGitRef, string>) (r => r.Name)).ToList<string>();
      if (list.Count > 0)
        this.SkipOrThrow("The following refs are unlocked and cannot be migrated: " + string.Join(", ", (IEnumerable<string>) list), migrationRequest);
      toMigrate = source1.ToDictionary<TfsGitRef, string, Sha1Id>((Func<TfsGitRef, string>) (r => r.Name), (Func<TfsGitRef, Sha1Id>) (r => r.ObjectId));
      if (toMigrate.Count != ((IEnumerable<string>) migrationRequest.BranchesToMigrate).Count<string>())
        throw new ArgumentException("Requested refs don't exist: " + string.Join(", ", ((IEnumerable<string>) migrationRequest.BranchesToMigrate).Except<string>((IEnumerable<string>) toMigrate.Keys)));
      this.TraceMigrationStatus(source.Key, target.Key, string.Format("Creating {0} migration commits in source", (object) toMigrate.Keys.Count));
    }

    private Dictionary<string, Sha1Id> CreateCommits(
      ITfsGitRepository source,
      ITfsGitRepository target,
      TfsGitRef[] targetParents,
      Dictionary<string, Sha1Id> toMigrate,
      Dictionary<string, BranchMigrationData> toRename,
      IProgress<ReceivePackStep> pushWatcher)
    {
      Dictionary<string, Sha1Id> commits = new Dictionary<string, Sha1Id>();
      Sha1Id[] array = ((IEnumerable<TfsGitRef>) targetParents).Select<TfsGitRef, Sha1Id>((Func<TfsGitRef, Sha1Id>) (x => x.ObjectId)).ToArray<Sha1Id>();
      Stream tempPackStream;
      PackAndRefIngester packAndRefIngester = source.CreatePackAndRefIngester(out tempPackStream);
      using (tempPackStream)
      {
        using (GitPackSerializer gitPackSerializer = new GitPackSerializer(tempPackStream, toMigrate.Count))
        {
          foreach (string key in toMigrate.Keys)
          {
            TfsGitRef tfsGitRef1 = source.Refs.MatchingName(toRename[key].SourceName);
            Sha1Id sha1Id = tfsGitRef1 != null ? tfsGitRef1.ObjectId : toMigrate[key];
            string str1 = tfsGitRef1?.Name ?? key;
            var data = new
            {
              ProjectId = source.Key.ProjectId,
              RepoId = source.Key.RepoId,
              CommitId = sha1Id
            };
            Sha1Id objectId = source.LookupObject<TfsGitCommit>(toMigrate[key]).GetTree().ObjectId;
            string identityString = IdentityAndDate.CreateIdentityString("Azure DevOps Repo Migrator", "noreply@dev.azure.com", DateTime.UtcNow, TimeSpan.Zero);
            string str2 = str1.Substring("refs/heads/".Length);
            byte[] commitBytes;
            Sha1Id commit = CommitBuilder.CreateCommit((IEnumerable<Sha1Id>) array, objectId, identityString, identityString, "Automated migration of branch: " + str2 + "\n\nData: " + JsonConvert.SerializeObject((object) data), out commitBytes);
            using (MemoryStream sourceStream = new MemoryStream(commitBytes, false))
              gitPackSerializer.AddInflatedStreamWithTypeAndSize((Stream) sourceStream, GitPackObjectType.Commit, (long) commitBytes.Length);
            string refName = CodeMigrator.InProgressRefName(target.Key, toRename[key].TargetName);
            TfsGitRef tfsGitRef2 = source.Refs.MatchingName(refName);
            packAndRefIngester.AddRefUpdateRequest(refName, tfsGitRef2 == null ? Sha1Id.Empty : tfsGitRef2.ObjectId, commit);
            commits.Add(key, commit);
          }
          gitPackSerializer.Complete();
          tempPackStream.Position = 0L;
          TfsGitRefUpdateResultSet srcUpdateResults = packAndRefIngester.Ingest(pushWatcher);
          CodeMigrator.CheckRefUpdateForFailures(source.Key, srcUpdateResults);
        }
      }
      return commits;
    }

    private ISet<Sha1Id> CalculateDeltaToPush(
      ITfsGitRepository source,
      ITfsGitRepository target,
      Dictionary<string, Sha1Id> createdCommits)
    {
      List<TfsGitRef> source1 = source.Refs.MatchingNames((IEnumerable<string>) new string[1]
      {
        CodeMigrator.CompletedPrefix(target.Key.RepoId)
      }, GitRefSearchType.StartsWith);
      this.TraceMigrationStatus(source.Key, target.Key, string.Format("Using {0} completed refs for reachability calculation", (object) source1.Count));
      ISet<Sha1Id> deltaToPush = this.m_ror.Resolve(source, (ISet<Sha1Id>) new HashSet<Sha1Id>(source1.Select<TfsGitRef, Sha1Id>((Func<TfsGitRef, Sha1Id>) (r => r.ObjectId))), (ISet<Sha1Id>) createdCommits.Values.ToHashSet<Sha1Id>(), (ICollection<Sha1Id>) Array.Empty<Sha1Id>(), new GitObjectFilter(), false, out ISet<Sha1Id> _, (IObserver<int>) null);
      this.TraceMigrationStatus(source.Key, target.Key, string.Format("Pushing {0} objects to target", (object) deltaToPush.Count));
      return deltaToPush;
    }

    private void IngestUpdates(
      ITfsGitRepository source,
      ITfsGitRepository target,
      Dictionary<string, Sha1Id> createdCommits,
      Dictionary<string, Sha1Id> toMigrate,
      Dictionary<string, BranchMigrationData> toRename,
      ISet<Sha1Id> deltaToPushToTarget,
      IProgress<ReceivePackStep> pushWatcher)
    {
      Stream tempPackStream;
      PackAndRefIngester packAndRefIngester = target.CreatePackAndRefIngester(out tempPackStream);
      TfsGitRefUpdateResultSet srcUpdateResults;
      using (tempPackStream)
      {
        foreach (string key in toMigrate.Keys)
        {
          Sha1Id createdCommit = createdCommits[key];
          packAndRefIngester.AddRefUpdateRequest(toRename[key].TargetName, Sha1Id.Empty, createdCommit);
        }
        this.m_getPackWriter(source).Write(deltaToPushToTarget, tempPackStream);
        tempPackStream.Position = 0L;
        srcUpdateResults = packAndRefIngester.Ingest(pushWatcher, false);
      }
      CodeMigrator.CheckRefUpdateForFailures(target.Key, srcUpdateResults);
    }

    private void UpdateProgressRefs(
      ITfsGitRepository source,
      ITfsGitRepository target,
      Dictionary<string, Sha1Id> createdCommits,
      Dictionary<string, Sha1Id> toMigrate,
      Dictionary<string, BranchMigrationData> toRename)
    {
      if (createdCommits == null || toMigrate == null)
        return;
      List<TfsGitRefUpdateRequest> refUpdateRequestList = new List<TfsGitRefUpdateRequest>(toMigrate.Keys.Count * 2);
      foreach (string key in toMigrate.Keys)
      {
        string refName = CodeMigrator.CompletedRefName(target.Key, toRename[key].TargetName);
        TfsGitRef tfsGitRef = source.Refs.MatchingName(refName);
        Sha1Id createdCommit = createdCommits[key];
        refUpdateRequestList.Add(new TfsGitRefUpdateRequest(CodeMigrator.CompletedRefName(target.Key, toRename[key].TargetName), tfsGitRef != null ? tfsGitRef.ObjectId : Sha1Id.Empty, createdCommit));
        refUpdateRequestList.Add(new TfsGitRefUpdateRequest(CodeMigrator.InProgressRefName(target.Key, toRename[key].TargetName), createdCommit, Sha1Id.Empty));
      }
      CodeMigrator.CheckRefUpdateForFailures(source.Key, this.m_updateRefs(source.Key, refUpdateRequestList));
    }

    private static void CheckRefUpdateForFailures(
      RepoKey repoKey,
      TfsGitRefUpdateResultSet srcUpdateResults)
    {
      if (srcUpdateResults.CountFailed > 0)
      {
        string str = string.Join(", ", srcUpdateResults.Results.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (r => r.Status != 0)).Select(r => new
        {
          Name = r.Name,
          Status = r.Status,
          RejectedBy = r.RejectedBy,
          CustomMessage = r.CustomMessage
        }));
        throw new InvalidOperationException(string.Format("Repo {0}: Ref update failed: {1}", (object) repoKey, (object) str));
      }
    }

    public static string CompletedPrefix(Guid targetRepoId) => string.Format("refs/repo_migrate/target/{0}/completed/", (object) targetRepoId);

    private static string CompletedRefName(RepoKey targetKey, string migratedRef) => CodeMigrator.CompletedPrefix(targetKey.RepoId) + migratedRef;

    public static string InProgressPrefix(Guid targetRepoId) => string.Format("refs/repo_migrate/target/{0}/in_progress/", (object) targetRepoId);

    private static string InProgressRefName(RepoKey targetKey, string migratingRef) => CodeMigrator.InProgressPrefix(targetKey.RepoId) + migratingRef;

    private void SkipOrThrow(string message, RepoMigrationRequest migrationRequest)
    {
      if (!migrationRequest.SkipValidationsForTestingOnly)
        throw new ArgumentException(message);
      this.m_tracer.TraceAlways(1013883, TraceLevel.Info, GitServerUtils.TraceArea, nameof (CodeMigrator), "Skipped Validation: " + message);
    }

    private void TraceMigrationStatus(RepoKey source, RepoKey target, string message) => this.m_tracer.TraceAlways(1013852, TraceLevel.Info, GitServerUtils.TraceArea, nameof (CodeMigrator), string.Format("Repo {0} to {1}: {2}", (object) source, (object) target, (object) message));
  }
}
