// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsObjectsHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [GvfsPublicProjectRequestRestrictions]
  internal class GvfsObjectsHandler : GvfsHttpHandler<DepthAndObjectIds>
  {
    private const string c_layer = "GvfsObjectsHandler";

    public GvfsObjectsHandler()
    {
    }

    public GvfsObjectsHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override TimeSpan Timeout => TimeSpan.FromHours(72.0);

    protected override string Layer => nameof (GvfsObjectsHandler);

    internal override void ProcessPost(RepoNameKey nameKey, DepthAndObjectIds request)
    {
      using (ITfsGitRepository repositoryByName = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(this.RequestContext, nameKey.ProjectName, nameKey.RepositoryName))
      {
        HttpResponseBase response = this.HandlerHttpContext.Response;
        response.StatusCode = 200;
        HashSet<Sha1Id> sha1IdSet;
        try
        {
          sha1IdSet = this.ProcessRequestedObjects(repositoryByName, request);
        }
        catch (GitObjectDoesNotExistException ex)
        {
          this.RequestContext.TraceCatch(1013567, GitServerUtils.TraceArea, nameof (GvfsObjectsHandler), (Exception) ex);
          this.WriteTextResponse(HttpStatusCode.NotFound, ex.Message);
          return;
        }
        ITfsGitContentDB<TfsGitObjectLocation> contentDb = (ITfsGitContentDB<TfsGitObjectLocation>) GitServerUtils.GetContentDB(repositoryByName);
        if (sha1IdSet.Count > 1)
        {
          this.SendPackToResponse(response, contentDb, (ISet<Sha1Id>) sha1IdSet);
        }
        else
        {
          response.ContentType = GvfsServerConstants.LooseObjectResponseContentType;
          new GitLooseObjectWriter(this.RequestContext.RequestTracer, repositoryByName).Write(response.OutputStream, sha1IdSet.First<Sha1Id>(), (Action) (() =>
          {
            this.ResponseStarted = true;
            this.RequestContext.UpdateTimeToFirstPage();
          }));
        }
      }
    }

    private HashSet<Sha1Id> ProcessRequestedObjects(
      ITfsGitRepository repo,
      DepthAndObjectIds request)
    {
      using (this.RequestContext.TraceBlock(1013549, 1013550, GitServerUtils.TraceArea, nameof (GvfsObjectsHandler), nameof (ProcessRequestedObjects)))
      {
        HashSet<Sha1Id> commits = new HashSet<Sha1Id>();
        HashSet<Sha1Id> objectsToSend = new HashSet<Sha1Id>();
        foreach (Sha1Id objectId in (IEnumerable<Sha1Id>) request.ObjectIds)
        {
          switch (repo.LookupObjectType(objectId))
          {
            case GitObjectType.Bad:
              throw new GitObjectDoesNotExistException(objectId);
            case GitObjectType.Commit:
              commits.Add(objectId);
              continue;
            case GitObjectType.Tree:
            case GitObjectType.Blob:
            case GitObjectType.Tag:
              objectsToSend.Add(objectId);
              continue;
            default:
              throw new InvalidGitObjectTypeException(objectId);
          }
        }
        if (commits.Count > 0)
        {
          foreach (Sha1Id objectId in this.ExpandCommitDepth(repo, (IEnumerable<Sha1Id>) commits, request.CommitDepth))
          {
            objectsToSend.Add(objectId);
            this.ExpandTreeEntries(repo.LookupObject<TfsGitCommit>(objectId).GetTree(), objectsToSend);
          }
        }
        return objectsToSend;
      }
    }

    private IEnumerable<Sha1Id> ExpandCommitDepth(
      ITfsGitRepository repo,
      IEnumerable<Sha1Id> commits,
      int commitDepth)
    {
      using (this.RequestContext.TraceBlock(1013557, 1013558, GitServerUtils.TraceArea, nameof (GvfsObjectsHandler), nameof (ExpandCommitDepth)))
      {
        --commitDepth;
        return commitDepth <= 0 ? commits : new AncestralGraphAlgorithm<int, Sha1Id>().GetReachable((IDirectedGraph<int, Sha1Id>) repo.GetCommitGraph(commits), commits, maxDistance: commitDepth);
      }
    }

    private void ExpandTreeEntries(TfsGitTree rootTree, HashSet<Sha1Id> objectsToSend)
    {
      using (this.RequestContext.TraceBlock(1013551, 1013552, GitServerUtils.TraceArea, nameof (GvfsObjectsHandler), nameof (ExpandTreeEntries)))
      {
        Stack<TfsGitTree> source = new Stack<TfsGitTree>();
        source.Push(rootTree);
        while (source.Any<TfsGitTree>())
        {
          TfsGitTree tfsGitTree = source.Pop();
          if (objectsToSend.Add(tfsGitTree.ObjectId))
          {
            foreach (TfsGitTree tree in tfsGitTree.GetTrees())
              source.Push(tree);
          }
        }
      }
    }

    private void SendPackToResponse(
      HttpResponseBase response,
      ITfsGitContentDB<TfsGitObjectLocation> storage,
      ISet<Sha1Id> objectsToSend)
    {
      using (this.RequestContext.TraceBlock(1013553, 1013554, GitServerUtils.TraceArea, nameof (GvfsObjectsHandler), nameof (SendPackToResponse)))
      {
        response.ContentType = GvfsServerConstants.PackResponseContentType;
        bool isTarpit;
        using (GitPackWriterThrottler throttler = new GitPackWriterThrottler(this.RequestContext, objectsToSend.Count, out isTarpit))
        {
          GitPackWriter gitPackWriter = new GitPackWriter(storage, throttler);
          try
          {
            gitPackWriter.Write(objectsToSend, response.OutputStream, (ISet<Sha1Id>) null, (Action) (() =>
            {
              this.RequestContext.UpdateTimeToFirstPage();
              this.ResponseStarted = true;
            }), (Predicate<ObjectIdAndGitPackIndexEntry>) null);
          }
          catch (HttpException ex)
          {
            throw new RequestCanceledException(FrameworkResources.RequestCanceledErrorWithReason((object) ex.Message), (Exception) ex);
          }
        }
      }
    }
  }
}
