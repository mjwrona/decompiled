// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitBlobsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitBlobsController : GitApiController
  {
    private const int c_filenameMaxLength = 260;
    private const string c_fileExtensionZip = ".zip";

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__blobs__objectId_.json", null, null, null)]
    [ClientResponseType(typeof (GitBlobRef), null, null)]
    [ClientResponseType(typeof (Stream), "GetBlobZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetBlobContent", "application/octet-stream")]
    [ClientLocationId("7B28E929-2C99-405D-9C5C-6167A06E6816")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetBlob(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      string sha1,
      [ClientIgnore] string projectId = null,
      bool download = false,
      string fileName = null,
      [FromUri(Name = "$format"), ClientInclude(RestClientLanguages.Swagger2)] string format = null,
      bool resolveLfs = false)
    {
      RequestMediaType requestMediaType = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip,
        RequestMediaType.Text,
        RequestMediaType.OctetStream,
        RequestMediaType.None
      }).First<RequestMediaType>();
      fileName = this.TrimAndValidateFilename(fileName);
      bool flag = download || requestMediaType == RequestMediaType.Zip;
      ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId);
      this.AddDisposableResource((IDisposable) tfsGitRepository);
      TfsGitBlob tfsGitBlob = this.LookupBlob(tfsGitRepository, sha1);
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      long length;
      if (GitFileUtility.TryDetectFileEncodingAndLength(tfsGitRepository, tfsGitBlob.ObjectId, 0, 0L, out length) == -1 && requestMediaType != RequestMediaType.Json)
        requestMediaType = RequestMediaType.OctetStream;
      if (resolveLfs && requestMediaType != RequestMediaType.OctetStream)
        throw new ArgumentException(Resources.Get("CanOnlyResolveLfsToOctetStream")).Expected("git");
      string str = tfsGitBlob.ObjectId.ToString();
      ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
      if (requestMediaType != RequestMediaType.Json)
      {
        response.Content = requestMediaType == RequestMediaType.Zip ? (HttpContent) GitFileUtility.GetZipPushStreamContent(tfsGitRepository, tfsGitBlob.ObjectId, repositoryReadOnly) : (HttpContent) new VssServerStreamContent(GitFileUtility.GetFileContentStream(tfsGitRepository, tfsGitBlob.ObjectId, GitLinkedContentUtility.GetTfsLinkedContentResolvers(this.TfsRequestContext, resolveLfs)), (object) repositoryReadOnly);
        string mediaType = "application/octet-stream";
        if (!string.IsNullOrEmpty(fileName))
        {
          string contentType = MimeMapper.GetContentType(VersionControlFileUtility.GetFileExtensionFromPath(fileName));
          mediaType = MediaTypeFormatUtility.GetSafeResponseContentType(string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType);
          flag = flag || mediaType == "application/octet-stream";
        }
        if (flag)
        {
          string fileName1 = fileName ?? str + (requestMediaType == RequestMediaType.Zip ? ".zip" : string.Empty);
          response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName1);
        }
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        this.TfsRequestContext.UpdateTimeToFirstPage();
        return response;
      }
      GitBlobRef gitBlob = new GitBlobRef()
      {
        ObjectId = str,
        Size = length,
        Url = this.Url.RestLink(this.TfsRequestContext, GitWebApiConstants.BlobsLocationId, (object) new
        {
          project = tfsGitRepository.Key.ProjectId,
          repositoryId = repositoryId,
          sha1 = str
        })
      };
      gitBlob.Links = gitBlob.GetBlobReferenceLinks(this.TfsRequestContext, tfsGitRepository.Key, repositoryReadOnly);
      gitBlob.SetSecuredObject(repositoryReadOnly);
      return this.Request.CreateResponse<GitBlobRef>(HttpStatusCode.OK, gitBlob);
    }

    [HttpPost]
    [ClientLocationId("7B28E929-2C99-405D-9C5C-6167A06E6816")]
    [ClientResponseType(typeof (Stream), "GetBlobsZip", "application/zip")]
    public HttpResponseMessage GetBlobs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      List<string> blobIds,
      [ClientIgnore] string projectId = null,
      string filename = null)
    {
      if (MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Zip,
        RequestMediaType.None
      }).FirstOrDefault<RequestMediaType>() != RequestMediaType.Zip)
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.Get("MustAcceptZip"));
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) blobIds, nameof (blobIds), this.TfsRequestContext.ServiceName);
      filename = this.TrimAndValidateFilename(filename);
      ITfsGitRepository repo = this.GetTfsGitRepository(repositoryId, projectId);
      this.AddDisposableResource((IDisposable) repo);
      List<TfsGitBlob> list = blobIds.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Select<string, TfsGitBlob>((Func<string, TfsGitBlob>) (blobId => this.LookupBlob(repo, blobId))).ToList<TfsGitBlob>();
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) GitFileUtility.GetZipPushStreamContent(repo, (IList<TfsGitBlob>) list);
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(filename ?? list[0].ObjectId.ToString() + ".zip");
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }

    private TfsGitBlob LookupBlob(ITfsGitRepository repo, string blobId)
    {
      try
      {
        return repo.LookupObject<TfsGitBlob>(GitCommitUtility.ParseSha1Id(blobId));
      }
      catch (GitUnexpectedObjectTypeException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex).Expected("git");
      }
      catch (GitObjectDoesNotExistException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex).Expected("git");
      }
    }

    private string TrimAndValidateFilename(string filename)
    {
      if (filename != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(filename, nameof (filename), this.TfsRequestContext.ServiceName);
        filename = filename.Trim();
        ArgumentUtility.CheckForOutOfRange(filename.Length, nameof (filename), 1, 260, this.TfsRequestContext.ServiceName);
      }
      return filename;
    }
  }
}
