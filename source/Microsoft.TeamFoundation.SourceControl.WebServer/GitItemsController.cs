// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitItemsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Events;
using Microsoft.TeamFoundation.Git.Server.LinkedContent;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitItemsController : GitApiController
  {
    private const int c_defaultTop = 100;
    private const long c_maxItemContentBytes = 5242880;

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__items_scopePath-_filePath_.json", "Get metadata", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__items_scopePath-_filePath__download-true.json", "Download", null, null)]
    [ClientResponseType(typeof (GitItem), null, null)]
    [ClientResponseType(typeof (Stream), "GetItemZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetItemText", "text/plain")]
    [ClientResponseType(typeof (Stream), "GetItemContent", "application/octet-stream")]
    [ClientLocationId("FB93C0DB-47ED-4A31-8C20-47552878FB44")]
    [PublicProjectRequestRestrictions]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage GetItem(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientQueryParameter] string path,
      [ClientIgnore] string projectId = null,
      string scopePath = null,
      [ClientParameterType(typeof (VersionControlRecursionType), false)] string recursionLevel = "none",
      bool includeContentMetadata = false,
      bool latestProcessedChange = false,
      bool download = false,
      [FromUri(Name = "$format"), ClientInclude(RestClientLanguages.Swagger2)] string format = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null,
      bool includeContent = false,
      bool resolveLfs = false,
      bool sanitize = false)
    {
      if (string.IsNullOrWhiteSpace(path))
        return this.GetItems(repositoryId, projectId, scopePath, recursionLevel, includeContentMetadata, latestProcessedChange, download, format: format, versionDescriptor: versionDescriptor);
      if (!string.IsNullOrWhiteSpace(scopePath))
        throw new ArgumentException(Resources.Get("ErrorItemAndScopePaths")).Expected("git");
      VersionControlRecursionType recursionType = GitItemsController.CheckRecursionLevel(recursionLevel);
      if (recursionType != VersionControlRecursionType.None)
        throw new ArgumentException(Resources.Get("ErrorItemPathWithRecursion")).Expected("git");
      return this.GetItemsFromPath(repositoryId, projectId, path, true, recursionType, includeContent, includeContentMetadata, resolveLfs, latestProcessedChange, download, true, format, versionDescriptor, sanitize);
    }

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__items_scopePath-_folderPath_.json", "For a path", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__items_scopePath-_folderPath__recursionLevel-OneLevel.json", "Single level of recursion", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__items_scopePath-_folderPath__recursionLevel-Full_includeContentMetadata-true.json", "Full recursion and with content metadata", null, null)]
    [ClientResponseType(typeof (List<GitItem>), null, null)]
    [ClientLocationId("FB93C0DB-47ED-4A31-8C20-47552878FB44")]
    [PublicProjectRequestRestrictions]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage GetItems(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      string scopePath = null,
      [ClientParameterType(typeof (VersionControlRecursionType), false)] string recursionLevel = "none",
      bool includeContentMetadata = false,
      bool latestProcessedChange = false,
      bool download = false,
      bool includeLinks = false,
      [FromUri(Name = "$format"), ClientInclude(RestClientLanguages.Swagger2)] string format = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null,
      bool zipForUnix = false)
    {
      VersionControlRecursionType recursionType = GitItemsController.CheckRecursionLevel(recursionLevel);
      string path = scopePath;
      if (string.IsNullOrWhiteSpace(path))
        path = "/";
      return this.GetItemsFromPath(repositoryId, projectId, path, false, recursionType, false, includeContentMetadata, false, latestProcessedChange, download, includeLinks, format, versionDescriptor, zipForUnix: zipForUnix);
    }

    private HttpResponseMessage GetItemsFromPath(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId,
      string path,
      bool singleItemMode,
      VersionControlRecursionType recursionType,
      bool includeContent,
      bool includeContentMetadata,
      bool resolveLfs,
      bool getLatestChanges,
      bool provideDownload,
      bool provideLinks,
      [FromUri(Name = "$format"), ClientInclude(RestClientLanguages.Swagger2)] string format,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null,
      bool sanitize = false,
      bool zipForUnix = false)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Git.Items.ZipForUnix"))
        zipForUnix = false;
      RequestMediaType requestMediaType = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip,
        RequestMediaType.Text,
        RequestMediaType.OctetStream,
        RequestMediaType.None
      }).First<RequestMediaType>();
      CacheControlHeaderValue controlHeaderValue = (CacheControlHeaderValue) null;
      if (versionDescriptor.VersionType == GitVersionType.Commit)
        controlHeaderValue = GitApiController.GetCacheControl();
      if (requestMediaType == RequestMediaType.Zip)
        provideDownload = true;
      if (zipForUnix)
      {
        if (requestMediaType != RequestMediaType.Zip)
          throw new InvalidArgumentValueException(nameof (zipForUnix), Resources.Get("InvalidParameters"));
        this.TfsRequestContext.TraceAlways(1013903, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitItemsController), "Get git items' zip archive with linux permissions for repository: " + repositoryId + " and path: " + path);
      }
      GitItemDescriptor gitItemDescriptor = new GitItemDescriptor();
      gitItemDescriptor.Path = path;
      int num;
      switch (requestMediaType)
      {
        case RequestMediaType.Zip:
          num = 120;
          break;
        case RequestMediaType.Text:
          num = 0;
          break;
        default:
          num = (int) recursionType;
          break;
      }
      gitItemDescriptor.RecursionLevel = (VersionControlRecursionType) num;
      gitItemDescriptor.Version = versionDescriptor.Version;
      gitItemDescriptor.VersionType = versionDescriptor.VersionType;
      gitItemDescriptor.VersionOptions = versionDescriptor.VersionOptions;
      GitItemDescriptor itemDescriptor = gitItemDescriptor;
      ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId);
      this.AddDisposableResource((IDisposable) tfsGitRepository);
      if (versionDescriptor.VersionType == GitVersionType.Branch && tfsGitRepository.Refs.GetDefaultOrAny() == null)
        throw new GitItemNotFoundException(Resources.Format("ErrorNoBranchesFormat", (object) tfsGitRepository.Name));
      long scanBytes = 0;
      if (includeContentMetadata & singleItemMode && (requestMediaType == RequestMediaType.Text || requestMediaType == RequestMediaType.Json & includeContent))
        scanBytes = 5242880L;
      IEnumerable<ITfsLinkedContentResolver> contentResolvers = GitLinkedContentUtility.GetTfsLinkedContentResolvers(this.TfsRequestContext, resolveLfs);
      GitItemsCollection gitItemsCollection = GitItemUtility.RetrieveItemModels(this.TfsRequestContext, this.Url, tfsGitRepository, itemDescriptor, includeContentMetadata, provideLinks, scanBytes, contentResolvers);
      ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
      switch (requestMediaType)
      {
        case RequestMediaType.Json:
          if (getLatestChanges)
            GitItemUtility.PopulateLastChangedCommits(this.TfsRequestContext, this.Url, tfsGitRepository, gitItemsCollection, itemDescriptor.RecursionLevel);
          if (singleItemMode)
          {
            GitItem gitItem = gitItemsCollection[0];
            ContentRangeHeaderValue contentRange = (ContentRangeHeaderValue) null;
            if (includeContent)
              gitItem.Content = this.GetItemContent(gitItem, tfsGitRepository, contentResolvers, out contentRange);
            if (provideLinks)
              gitItem.Links = gitItem.GetItemReferenceLinks(this.TfsRequestContext, this.Url, tfsGitRepository.Key);
            gitItem.SetSecuredObject(repositoryReadOnly);
            HttpResponseMessage response = this.Request.CreateResponse<GitItem>(HttpStatusCode.OK, gitItem);
            response.Headers.CacheControl = controlHeaderValue;
            if (contentRange != null)
            {
              response.Content.Headers.ContentRange = contentRange;
              response.StatusCode = HttpStatusCode.PartialContent;
            }
            this.TfsRequestContext.UpdateTimeToFirstPage();
            return response;
          }
          foreach (GitItem gitItem in (List<GitItem>) gitItemsCollection)
          {
            if (provideLinks)
              gitItem.Links = gitItem.GetItemReferenceLinks(this.TfsRequestContext, this.Url, tfsGitRepository.Key);
          }
          gitItemsCollection.SetSecuredObject<GitItem>(repositoryReadOnly);
          HttpResponseMessage response1 = this.Request.CreateResponse<GitItemsCollection>(HttpStatusCode.OK, gitItemsCollection);
          response1.Headers.CacheControl = controlHeaderValue;
          this.TfsRequestContext.UpdateTimeToFirstPage();
          return response1;
        case RequestMediaType.Zip:
          HttpResponseMessage response2 = this.Request.CreateResponse(HttpStatusCode.OK);
          string extensionFromPath = VersionControlFileUtility.GetFileExtensionFromPath(gitItemsCollection[0].Path);
          bool sanitizeSVG = "svg".Equals(extensionFromPath, StringComparison.OrdinalIgnoreCase) & sanitize && this.TfsRequestContext.IsFeatureEnabled("Git.Items.SanitizeSVG");
          gitItemsCollection.SetSecuredObject<GitItem>(repositoryReadOnly);
          switch (requestMediaType)
          {
            case RequestMediaType.None:
            case RequestMediaType.OctetStream:
              if (sanitizeSVG)
              {
                this.UpdateResponseWithStringContent(response2, gitItemsCollection[0], tfsGitRepository, contentResolvers, true);
                break;
              }
              response2.Content = (HttpContent) new VssServerStreamContent(gitItemsCollection[0].GitObjectType == GitObjectType.Commit ? GitFileUtility.GetSubmoduleContentStream(gitItemsCollection[0]) : GitFileUtility.GetFileContentStream(tfsGitRepository, GitCommitUtility.ParseSha1Id(gitItemsCollection[0].ObjectId), contentResolvers), (object) gitItemsCollection[0]);
              break;
            case RequestMediaType.Zip:
              response2.Content = (HttpContent) GitFileUtility.GetZipPushStreamContent(tfsGitRepository, gitItemsCollection, contentResolvers, repositoryReadOnly, zipForUnix);
              break;
            case RequestMediaType.Text:
              this.UpdateResponseWithStringContent(response2, gitItemsCollection[0], tfsGitRepository, contentResolvers, sanitizeSVG);
              break;
            default:
              throw new InvalidArgumentValueException(Resources.Get("UnsupportedMediaType"));
          }
          string contentType = MimeMapper.GetContentType(extensionFromPath);
          string str1 = string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType;
          string responseContentType = MediaTypeFormatUtility.GetSafeResponseContentType(sanitizeSVG ? "image/svg+xml" : str1);
          if (sanitizeSVG)
            response2.Headers.Add("Content-Security-Policy", "script-src 'none';");
          provideDownload = provideDownload || responseContentType == "application/octet-stream";
          provideDownload |= sanitizeSVG;
          if (provideDownload)
          {
            string fileName = (gitItemsCollection[0].Path == "/" ? tfsGitRepository.Name : GitFileUtility.GetFileNameFromPath(gitItemsCollection[0].Path)) + (requestMediaType == RequestMediaType.Zip ? ".zip" : string.Empty);
            response2.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName);
            this.PublishDownloadEvent(this.TfsRequestContext, tfsGitRepository, versionDescriptor);
          }
          string str2 = (string) null;
          if (this.TfsRequestContext.IsFeatureEnabled("Wiki.DetectTextFileCharset"))
          {
            FileContentMetadata contentMetadata = gitItemsCollection[0].ContentMetadata;
            ContentViewerType contentViewerType = MimeMapper.GetContentViewerType(extensionFromPath, responseContentType);
            if (contentMetadata != null && contentMetadata.Encoding >= 0)
            {
              if (contentViewerType == ContentViewerType.Text)
              {
                try
                {
                  str2 = Encoding.GetEncoding(contentMetadata.Encoding).HeaderName;
                }
                catch (ArgumentException ex)
                {
                }
                catch (NotSupportedException ex)
                {
                }
              }
            }
          }
          response2.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType)
          {
            CharSet = str2
          };
          response2.Headers.CacheControl = controlHeaderValue;
          this.TfsRequestContext.UpdateTimeToFirstPage();
          return response2;
        default:
          if (gitItemsCollection[0].GitObjectType != GitObjectType.Tree)
            goto case RequestMediaType.Zip;
          else
            goto case RequestMediaType.Json;
      }
    }

    private void PublishDownloadEvent(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor)
    {
      if (requestContext.IsFeatureEnabled("SourceControl.Git.DisableRepositoryDownloadEvent"))
      {
        requestContext.TraceAlways(10139367, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitItemsController), "FeatureFlag SourceControl.Git.DisableRepositoryDownloadEvent is on: skipping ServiceHook notifications");
      }
      else
      {
        GitDownloadEvent data = new GitDownloadEvent()
        {
          ProjectId = repository.Key.ProjectId,
          RepositoryId = repository.Key.RepoId,
          UserId = requestContext.GetUserId(),
          DownloadDate = DateTime.UtcNow
        };
        if (versionDescriptor.VersionType == GitVersionType.Branch)
          data.Branch = versionDescriptor.Version;
        VssNotificationEvent theEvent = new VssNotificationEvent((object) data)
        {
          EventType = "ms.vss-code.git-branch-download-event"
        };
        theEvent.AddScope(VssNotificationEvent.ScopeNames.Project, repository.Key.ProjectId);
        theEvent.AddScope(VssNotificationEvent.ScopeNames.Repository, repository.Key.RepoId);
        requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
      }
    }

    private static VersionControlRecursionType CheckRecursionLevel(string recursionLevel)
    {
      VersionControlRecursionType result;
      if (!System.Enum.TryParse<VersionControlRecursionType>(recursionLevel, true, out result))
        throw new ArgumentException(Resources.Format("InvalidQueryParam", (object) recursionLevel, (object) nameof (recursionLevel), (object) string.Join(",", System.Enum.GetNames(typeof (VersionControlRecursionType))))).Expected("git");
      return result;
    }

    private void UpdateResponseWithStringContent(
      HttpResponseMessage response,
      GitItem item,
      ITfsGitRepository repo,
      IEnumerable<ITfsLinkedContentResolver> linkedContentResolvers,
      bool sanitizeSVG)
    {
      ContentRangeHeaderValue contentRange;
      string str = this.GetItemContent(item, repo, linkedContentResolvers, out contentRange);
      if (sanitizeSVG)
      {
        if (contentRange != null)
          throw new InvalidSvgException(Resources.Get("InvalidSvgLargeFile"));
        str = SanitizeSvgUtility.Sanitize(this.TfsRequestContext, str);
      }
      response.Content = (HttpContent) new VssServerStringContent(str, (object) item);
      if (contentRange == null)
        return;
      response.Content.Headers.ContentRange = contentRange;
      response.StatusCode = HttpStatusCode.PartialContent;
    }

    private string GetItemContent(
      GitItem item,
      ITfsGitRepository repository,
      IEnumerable<ITfsLinkedContentResolver> linkedContentResolvers,
      out ContentRangeHeaderValue contentRange)
    {
      long from;
      long to;
      this.GetRequestedByteRange(out from, out to);
      FileContentMetadata fileContentMetadata = item.ContentMetadata ?? GitItemUtility.GetContentMetadata(this.TfsRequestContext, repository, item, false, (string) null, to, linkedContentResolvers);
      string itemContent;
      using (RangeStream contentStream = new RangeStream(new Lazy<Stream>((Func<Stream>) (() => item.GitObjectType != GitObjectType.Commit ? GitFileUtility.GetFileContentStream(repository, GitCommitUtility.ParseSha1Id(item.ObjectId), linkedContentResolvers) : GitFileUtility.GetSubmoduleContentStream(item))), from, (int) (to - from)))
      {
        itemContent = VersionControlFileReader.ReadFileContent((Stream) contentStream, fileContentMetadata.Encoding);
        contentRange = contentStream.Truncated ? new ContentRangeHeaderValue(from, from + (long) contentStream.BytesRead) : (ContentRangeHeaderValue) null;
      }
      return itemContent;
    }

    private void GetRequestedByteRange(out long from, out long to)
    {
      from = 0L;
      to = 5242880L;
      RangeHeaderValue range = this.Request.Headers.Range;
      if (range != null)
      {
        RangeItemHeaderValue rangeItemHeaderValue = range.Ranges.FirstOrDefault<RangeItemHeaderValue>();
        if (rangeItemHeaderValue != null)
        {
          ref long local1 = ref from;
          long? nullable = rangeItemHeaderValue.From;
          long num1 = nullable ?? from;
          local1 = num1;
          ref long local2 = ref to;
          nullable = rangeItemHeaderValue.To;
          long num2 = nullable ?? to;
          local2 = num2;
        }
      }
      if (to - from <= 5242880L)
        return;
      to = from + 5242880L;
    }

    [HttpGet]
    [ClientIgnore]
    [ClientResponseType(typeof (IPagedList<GitItem>), null, null)]
    [ClientLocationId("FB93C0DB-47ED-4A31-8C20-47552878FB44")]
    public HttpResponseMessage GetItemsPaged(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [FromUri(Name = "$top")] int top,
      [ClientIgnore] string projectId = null,
      string scopePath = null,
      string continuationToken = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null)
    {
      if (string.IsNullOrWhiteSpace(scopePath))
        throw new InvalidArgumentValueException(Resources.Format("ScopePathRequiredForPagingItems", (object) nameof (scopePath)));
      GitVersionParser.ValidateVersionDescriptor(versionDescriptor);
      if (versionDescriptor.VersionOptions == GitVersionOptions.PreviousChange)
        throw new ArgumentException(Resources.Get("ErrorPreviousChangeRecursive"));
      if (top == 0)
        top = 100;
      HttpResponseMessage itemsListUnderPath = this.GetPagedItemsListUnderPath(repositoryId, projectId, scopePath, versionDescriptor, top, continuationToken);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return itemsListUnderPath;
    }

    private HttpResponseMessage GetPagedItemsListUnderPath(
      string repositoryId,
      string projectId,
      string scopePath,
      GitVersionDescriptor versionDescriptor,
      int top,
      string continuationToken)
    {
      CacheControlHeaderValue controlHeaderValue = (CacheControlHeaderValue) null;
      if (versionDescriptor.VersionType == GitVersionType.Commit)
        controlHeaderValue = GitApiController.GetCacheControl();
      GitItemsContinuationToken continuationToken1 = (GitItemsContinuationToken) null;
      GitItemsCollection gitItemsCollection1 = new GitItemsCollection();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitCommit commitFromVersion = GitVersionParser.GetCommitFromVersion(this.TfsRequestContext, tfsGitRepository, ref scopePath, versionDescriptor);
        TfsGitTree tree = commitFromVersion.GetTree();
        NormalizedGitPath normalizedGitPath = new NormalizedGitPath(scopePath);
        GitItemsContinuationToken token = (GitItemsContinuationToken) null;
        if (continuationToken != null && !GitItemsContinuationToken.TryParseContinuationToken(tree.ObjectId, scopePath, continuationToken, out token))
          return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
        TfsGitTreeEntry treeEntry = (TfsGitTreeEntry) null;
        TfsGitObject tfsGitObject = normalizedGitPath.IsRoot ? (TfsGitObject) tree : GitItemUtility.FindMember(this.TfsRequestContext, tree, ref scopePath, out treeEntry);
        if (treeEntry != null && treeEntry.ObjectType == GitObjectType.Commit)
        {
          string path = ".gitmodules";
          tfsGitObject = GitItemUtility.FindMember(this.TfsRequestContext, tree, ref path, out treeEntry);
        }
        if (tfsGitObject == null)
        {
          string commitId = commitFromVersion.ObjectId.ToString();
          return this.Request.CreateErrorResponse((Exception) new GitItemNotFoundException(scopePath, tfsGitRepository.Name, versionDescriptor.ToString(), commitId), (IHttpController) this);
        }
        if (tfsGitObject.ObjectType != GitObjectType.Tree && token != null)
          return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Get("ContinuationTokenForBlobScopeNotSupported")), (IHttpController) this);
        if (tfsGitObject.ObjectType != GitObjectType.Tree)
        {
          GitItemsCollection gitItemsCollection2 = gitItemsCollection1;
          GitItem gitItem = new GitItem();
          gitItem.Path = normalizedGitPath.ToString();
          gitItem.ObjectId = tfsGitObject.ObjectId.ToString();
          gitItem.GitObjectType = tfsGitObject.ObjectType;
          gitItemsCollection2.Add(gitItem);
          HttpResponseMessage response = this.Request.CreateResponse<GitItemsCollection>(HttpStatusCode.OK, gitItemsCollection1);
          response.Headers.CacheControl = controlHeaderValue;
          return response;
        }
        int num = top;
        if (token == null && normalizedGitPath.IsRoot)
        {
          GitItemsCollection gitItemsCollection3 = gitItemsCollection1;
          GitItem gitItem = new GitItem();
          gitItem.Path = scopePath;
          gitItem.ObjectId = tree.ObjectId.ToString();
          gitItem.GitObjectType = tree.ObjectType;
          gitItemsCollection3.Add(gitItem);
          --num;
          if (num == 0)
            continuationToken1 = new GitItemsContinuationToken(tree.ObjectId, normalizedGitPath, normalizedGitPath, tree.ObjectType);
        }
        if (num > 0)
        {
          foreach (GitItem retrievePagedItemModel in GitItemUtility.RetrievePagedItemModels(this.TfsRequestContext, tree, normalizedGitPath, token))
          {
            gitItemsCollection1.Add(retrievePagedItemModel);
            --num;
            if (num == 0)
            {
              continuationToken1 = new GitItemsContinuationToken(tree.ObjectId, normalizedGitPath, new NormalizedGitPath(retrievePagedItemModel.Path), retrievePagedItemModel.GitObjectType);
              break;
            }
          }
        }
      }
      HttpResponseMessage response1 = this.Request.CreateResponse<GitItemsCollection>(HttpStatusCode.OK, gitItemsCollection1);
      response1.Headers.CacheControl = controlHeaderValue;
      if (continuationToken1 != null)
        response1.Headers.Add("x-ms-continuationtoken", continuationToken1.ToString());
      return response1;
    }
  }
}
