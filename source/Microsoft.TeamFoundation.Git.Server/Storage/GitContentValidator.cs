// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.GitContentValidator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal class GitContentValidator
  {
    private readonly IVssRequestContext m_rc;
    private readonly ILocationService m_locSvc;
    private readonly IContentValidationService m_contentValidationService;
    private readonly ITfsGitRepository m_repo;

    public GitContentValidator(
      IVssRequestContext rc,
      IContentValidationService contentValidationSvc,
      ILocationService locSvc,
      ITfsGitRepository repo)
    {
      this.m_rc = rc;
      this.m_locSvc = locSvc;
      this.m_contentValidationService = contentValidationSvc;
      this.m_repo = repo;
    }

    public Task ValidatePush(
      Microsoft.VisualStudio.Services.Identity.Identity pusher,
      Guid projectId,
      string pusherIpAddress,
      IEnumerable<Sha1Id> newCommits,
      IEnumerable<TfsGitRefUpdateResult> refUpdates)
    {
      return this.m_contentValidationService.SubmitAsync(this.m_rc, projectId, this.GetKeysForNewBlobs(pusher, pusherIpAddress, newCommits, refUpdates), pusher, pusherIpAddress);
    }

    public IEnumerable<ContentValidationKey> GetKeysForNewBlobs(
      Microsoft.VisualStudio.Services.Identity.Identity pusher,
      string pusherIpAddress,
      IEnumerable<Sha1Id> newCommits,
      IEnumerable<TfsGitRefUpdateResult> refUpdates)
    {
      Uri blobBase = this.m_locSvc.GetResourceUri(this.m_rc, "git", GitWebApiConstants.BlobsLocationId, (object) new
      {
        project = this.m_repo.Key.ProjectId,
        repositoryId = this.m_repo.Key.RepoId
      });
      foreach (Sha1Id newCommit in newCommits)
      {
        foreach (TfsGitDiffEntry tfsGitDiffEntry in this.m_repo.LookupObject<TfsGitCommit>(newCommit).GetManifest(this.m_repo, false, false))
        {
          if ((tfsGitDiffEntry.ChangeType & TfsGitChangeType.Delete) != TfsGitChangeType.Delete)
          {
            string relativePath = tfsGitDiffEntry.RelativePath;
            ContentValidationScanType scanType;
            if (this.ShouldSubmit(relativePath, tfsGitDiffEntry.NewObjectId, out scanType))
              yield return new ContentValidationKey(UriUtility.Combine(blobBase, tfsGitDiffEntry.NewObjectId.ToString(), true).AppendQuery("resolveLfs", "true").AppendQuery("$format", "OctetStream"), scanType, relativePath);
          }
        }
      }
      Queue<TfsGitTree> refUpdateTreesToWalk = new Queue<TfsGitTree>();
      foreach (TfsGitRefUpdateResult gitRefUpdateResult in refUpdates.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (r => r.Succeeded && r.NewObjectId != Sha1Id.Empty)))
      {
        TfsGitObject tag = this.m_repo.LookupObject(gitRefUpdateResult.NewObjectId);
        TfsGitObject peeledObject = (TfsGitObject) (tag as TfsGitTree);
        if (peeledObject != null || tag.PackType == GitPackObjectType.Tag && ((TfsGitTag) tag).TryPeelToNonTag(out peeledObject) && peeledObject.PackType == GitPackObjectType.Tree)
          refUpdateTreesToWalk.Enqueue((TfsGitTree) peeledObject);
      }
      HashSet<Sha1Id> checkedTrees = new HashSet<Sha1Id>();
      while (refUpdateTreesToWalk.Count > 0)
      {
        TfsGitTree currTree = refUpdateTreesToWalk.Dequeue();
        foreach (TreeParser.Entry parserEntry in currTree.GetParserEntries())
        {
          if (parserEntry.PackType == GitPackObjectType.Tree && !checkedTrees.Contains(parserEntry.ObjectId))
            refUpdateTreesToWalk.Enqueue(new TfsGitTree(currTree.ObjectSet, parserEntry.ObjectId));
          else if (parserEntry.PackType == GitPackObjectType.Blob)
          {
            string nameString = parserEntry.GetNameString();
            ContentValidationScanType scanType;
            if (this.ShouldSubmit(nameString, new Sha1Id?(parserEntry.ObjectId), out scanType))
              yield return new ContentValidationKey(UriUtility.Combine(blobBase, parserEntry.ObjectId.ToString(), true), scanType, nameString);
          }
          checkedTrees.Add(currTree.ObjectId);
        }
        currTree = (TfsGitTree) null;
      }
    }

    private bool ShouldSubmit(
      string fileName,
      Sha1Id? objectId,
      out ContentValidationScanType scanType)
    {
      scanType = ContentValidationUtil.GetScanTypeFromFileName(fileName);
      if (scanType == ContentValidationScanType.None)
        return false;
      Sha1Id? nullable = objectId;
      Sha1Id emptyBlobHash = GitServerConstants.EmptyBlobHash;
      if (!nullable.HasValue)
        return true;
      return nullable.HasValue && nullable.GetValueOrDefault() != emptyBlobHash;
    }
  }
}
