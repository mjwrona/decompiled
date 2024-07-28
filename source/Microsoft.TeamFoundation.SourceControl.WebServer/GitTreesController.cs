// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitTreesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitTreesController : GitApiController
  {
    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__trees__objectId_.json", "Non-recursive Example", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__trees__objectId__recursive-true.json", "Recursive Example", null, null)]
    [ClientResponseType(typeof (GitTreeRef), null, null)]
    [ClientResponseType(typeof (Stream), "GetTreeZip", "application/zip")]
    [ClientLocationId("729F6437-6F92-44EC-8BEE-273A7111063C")]
    public HttpResponseMessage GetTree(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      string sha1,
      string projectId = null,
      bool recursive = false,
      [ClientIgnore] int? depth = null,
      string fileName = null,
      [FromUri(Name = "$format"), ClientInclude(RestClientLanguages.Swagger2)] string format = null)
    {
      RequestMediaType requestMediaType = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip,
        RequestMediaType.None
      }).First<RequestMediaType>();
      recursive = recursive || requestMediaType == RequestMediaType.Zip || depth.HasValue;
      ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId);
      this.AddDisposableResource((IDisposable) tfsGitRepository);
      TfsGitTree tree = tfsGitRepository.LookupObject<TfsGitTree>(GitCommitUtility.ParseSha1Id(sha1));
      string str = tree.ObjectId.ToString();
      GitTreeRef gitTreeRef = new GitTreeRef()
      {
        ObjectId = str,
        TreeEntries = (IEnumerable<GitTreeEntryRef>) this.GetTreeEntries(tfsGitRepository.Key, tree, recursive, depth ?? int.MaxValue),
        Size = tree.GetLength(),
        Url = this.Url.RestLink(this.TfsRequestContext, GitWebApiConstants.TreesLocationId, (object) new
        {
          repositoryId = repositoryId,
          sha1 = str
        })
      };
      if (requestMediaType == RequestMediaType.Zip)
      {
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) GitFileUtility.GetZipPushStreamContent(tfsGitRepository, gitTreeRef);
        response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment((fileName ?? gitTreeRef.ObjectId) + ".zip");
        return response;
      }
      gitTreeRef.Links = gitTreeRef.GetTreeReferenceLinks(this.TfsRequestContext, tfsGitRepository.Key);
      return this.Request.CreateResponse<GitTreeRef>(HttpStatusCode.OK, gitTreeRef);
    }

    private List<GitTreeEntryRef> GetTreeEntries(
      RepoKey repoKey,
      TfsGitTree tree,
      bool recursive,
      int depth)
    {
      return recursive ? tree.GetTreeEntriesRecursive(depth).Select<TreeEntryAndPath, GitTreeEntryRef>((Func<TreeEntryAndPath, GitTreeEntryRef>) (x => x.Entry.ToGitTreeEntryShallow(this.TfsRequestContext, repoKey, this.Url, x.RelativePath))).ToList<GitTreeEntryRef>() : tree.GetTreeEntries().Select<TfsGitTreeEntry, GitTreeEntryRef>((Func<TfsGitTreeEntry, GitTreeEntryRef>) (x => x.ToGitTreeEntryShallow(this.TfsRequestContext, repoKey, this.Url))).ToList<GitTreeEntryRef>();
    }
  }
}
