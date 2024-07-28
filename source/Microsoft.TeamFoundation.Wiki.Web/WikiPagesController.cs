// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPagesController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
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
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pages")]
  public class WikiPagesController : WikiApiController
  {
    [HttpGet]
    [ClientResourceOperation(ClientResourceOperationName.Get)]
    [ClientResponseType(typeof (WikiPageResponse), null, null)]
    [ClientResponseType(typeof (Stream), "GetPageZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetPageText", "text/plain")]
    [ClientLocationId("25D3FBC7-FE3D-46CB-B5A5-0B6F79CAF27B")]
    [TraceFilter(15250200, 15250299)]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_wiki_page_JSON.json", "Get page as JSON", null, null)]
    [ClientExample("GET_wiki_page_JSON_withContent.json", "Get page as JSON with content", null, null)]
    [ClientExample("GET_wiki_page_text.json", "Get page as text", null, null)]
    [ClientExample("GET_wiki_page_JSON_withRecursionLevel.json", "Get page as JSON with recursion level", null, null)]
    public HttpResponseMessage GetPage(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [ClientQueryParameter] string path = "/",
      [ClientParameterType(typeof (VersionControlRecursionType), false)] string recursionLevel = "none",
      [FromUri(Name = "$format"), ClientIgnore] string format = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null,
      [ClientQueryParameter] bool includeContent = false)
    {
      path = Microsoft.TeamFoundation.Wiki.Server.PathHelper.NormalizePath(path);
      if (string.IsNullOrEmpty(path))
        throw new InvalidArgumentValueException(nameof (path), Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageReadOperationInvalidPagePath);
      return this.GetPageFromPath(this.FetchWiki(wikiIdentifier), path, recursionLevel, includeContent, versionDescriptor);
    }

    [HttpGet]
    [ClientResourceOperation(ClientResourceOperationName.Get)]
    [ClientResponseType(typeof (WikiPageResponse), null, null)]
    [ClientResponseType(typeof (Stream), "GetPageByIdZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetPageByIdText", "text/plain")]
    [ClientLocationId("CEDDCF75-1068-452D-8B13-2D4D76E1F970")]
    [TraceFilter(15250200, 15250299)]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_wiki_pageById_JSON.json", "Get page by id as JSON", null, null)]
    [ClientExample("GET_wiki_pageById_JSON_withContent.json", "Get page by id as JSON with content", null, null)]
    [ClientExample("GET_wiki_pageById_text.json", "Get page as text", null, null)]
    [ClientExample("GET_wiki_pageById_JSON_withRecursionLevel.json", "Get page by id as JSON with recursion level", null, null)]
    public HttpResponseMessage GetPageById(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int id,
      [ClientParameterType(typeof (VersionControlRecursionType), false)] string recursionLevel = "none",
      [ClientQueryParameter] bool includeContent = false)
    {
      this.ValidateId(id);
      WikiV2 wiki = this.FetchWiki(wikiIdentifier);
      GitVersionDescriptor version;
      string versionFromPageId = this.GetPageReadablePathAndVersionFromPageId(this.TfsRequestContext, this.ProjectId, wiki, id, out version);
      return this.GetPageFromPath(wiki, versionFromPageId, recursionLevel, includeContent, version);
    }

    [HttpPut]
    [ClientResourceOperation(ClientResourceOperationName.CreateOrUpdate)]
    [ClientResponseCode(HttpStatusCode.Created, "Page created. Created page's version is populated in the ETag response header.", false)]
    [ClientResponseCode(HttpStatusCode.OK, "Page edited. Edited page's version is populated in the ETag response header.", false)]
    [ClientResponseType(typeof (WikiPageResponse), null, null)]
    [ClientHeaderParameter("If-Match", typeof (string), "Version", "Version of the page on which the change is to be made. Mandatory for `Edit` scenario. To be populated in the If-Match header of the request.", false, false)]
    [ClientLocationId("25D3FBC7-FE3D-46CB-B5A5-0B6F79CAF27B")]
    [TraceFilter(15250200, 15250299)]
    [ClientExample("PUT_wiki_page_add.json", "Add a page", null, null)]
    [ClientExample("PUT_wiki_page_edit.json", "Edit a page", null, null)]
    public HttpResponseMessage CreateOrUpdatePage(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [ClientQueryParameter] string path,
      [FromBody] WikiPageCreateOrUpdateParameters parameters,
      [FromUri] string comment = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null)
    {
      path = Microsoft.TeamFoundation.Wiki.Server.PathHelper.NormalizePath(path);
      if (string.IsNullOrEmpty(path) || path.Equals("/"))
        throw new InvalidArgumentValueException(nameof (path), Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageWriteOperationInvalidPagePath);
      foreach (string reservedCharacter in PathConstants.PageNameReservedCharacters)
      {
        if (path.Contains(reservedCharacter))
          throw new InvalidArgumentValueException(nameof (path), string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageNameHasReservedCharacters, (object) string.Join(", ", PathConstants.PageNameReservedCharacters)));
      }
      string pageContent = parameters?.Content ?? string.Empty;
      WikiV2 wiki = this.FetchWiki(wikiIdentifier);
      return this.CreateOrUpdatePageByPath(path, comment, pageContent, wiki, versionDescriptor, nameof (CreateOrUpdatePage));
    }

    [HttpPatch]
    [ClientResourceOperation(ClientResourceOperationName.Update)]
    [ClientResponseCode(HttpStatusCode.OK, "Page edited. Edited page's version is populated in the ETag response header.", false)]
    [ClientResponseType(typeof (WikiPageResponse), null, null)]
    [ClientHeaderParameter("If-Match", typeof (string), "Version", "Version of the page on which the change is to be made. Mandatory for `Edit` scenario. To be populated in the If-Match header of the request.", false, false)]
    [ClientLocationId("CEDDCF75-1068-452D-8B13-2D4D76E1F970")]
    [TraceFilter(15250200, 15250299)]
    [ClientExample("PATCH_wiki_pageById_edit.json", "Edit a page by Id", null, null)]
    public HttpResponseMessage UpdatePageById(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int id,
      [FromBody] WikiPageCreateOrUpdateParameters parameters,
      [FromUri] string comment = null)
    {
      this.ValidateId(id);
      WikiV2 wiki = this.FetchWiki(wikiIdentifier);
      GitVersionDescriptor version;
      string versionFromPageId = this.GetPageReadablePathAndVersionFromPageId(this.TfsRequestContext, this.ProjectId, wiki, id, out version);
      string pageContent = parameters?.Content ?? string.Empty;
      return this.CreateOrUpdatePageByPath(versionFromPageId, comment, pageContent, wiki, version, nameof (UpdatePageById));
    }

    [HttpDelete]
    [ClientResourceOperation(ClientResourceOperationName.Delete)]
    [ClientResponseCode(HttpStatusCode.OK, "Page deleted.", false)]
    [ClientResponseType(typeof (WikiPageResponse), null, null)]
    [ClientLocationId("25D3FBC7-FE3D-46CB-B5A5-0B6F79CAF27B")]
    [TraceFilter(15250200, 15250299)]
    [ClientExample("DELETE_wiki_page.json", null, null, null)]
    public HttpResponseMessage DeletePage(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [ClientQueryParameter] string path,
      [FromUri] string comment = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null)
    {
      path = Microsoft.TeamFoundation.Wiki.Server.PathHelper.NormalizePath(path);
      if (string.IsNullOrEmpty(path) || path.Equals("/"))
        throw new InvalidArgumentValueException(nameof (path), Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageWriteOperationInvalidPagePath);
      WikiV2 wiki = this.FetchWiki(wikiIdentifier);
      return this.DeletePageByPath(path, comment, wiki, versionDescriptor, "Delete Page");
    }

    [HttpDelete]
    [ClientResourceOperation(ClientResourceOperationName.Delete)]
    [ClientResponseCode(HttpStatusCode.OK, "Page deleted.", false)]
    [ClientResponseType(typeof (WikiPageResponse), null, null)]
    [ClientLocationId("CEDDCF75-1068-452D-8B13-2D4D76E1F970")]
    [TraceFilter(15250200, 15250299)]
    [ClientExample("DELETE_wiki_pageById.json", null, null, null)]
    public HttpResponseMessage DeletePageById([FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier, [FromUri] int id, [FromUri] string comment = null)
    {
      this.ValidateId(id);
      WikiV2 wiki = this.FetchWiki(wikiIdentifier);
      GitVersionDescriptor version;
      return this.DeletePageByPath(this.GetPageReadablePathAndVersionFromPageId(this.TfsRequestContext, this.ProjectId, wiki, id, out version), comment, wiki, version, nameof (DeletePageById), new int?(id));
    }

    private void ValidateId(int id)
    {
      if (id <= 0)
        throw new InvalidArgumentValueException(nameof (id), Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageInvalidPageId);
    }

    private WikiV2 FetchWiki(string wikiIdentifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(wikiIdentifier, nameof (wikiIdentifier));
      return WikiV2Helper.GetWikiByIdentifier(this.TfsRequestContext, this.ProjectId, wikiIdentifier) ?? throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound);
    }

    private string GetPageReadablePathAndVersionFromPageId(
      IVssRequestContext requestContext,
      Guid projectId,
      WikiV2 wiki,
      int pageId,
      out GitVersionDescriptor version)
    {
      WikiPageIdDetails pageIdDetails = new WikiPageIdDetailsProvider().GetPageIdDetails(requestContext, projectId, wiki, pageId);
      version = pageIdDetails.WikiVersion;
      return Microsoft.TeamFoundation.Wiki.Server.PathHelper.GetPageReadablePathFromUnReadablePath(Path.ChangeExtension(pageIdDetails.GitFriendlyPagePath, (string) null));
    }

    private HttpResponseMessage GetPageFromPath(
      WikiV2 wiki,
      string readablePagePath,
      string recursionLevel,
      bool includeContentInJson,
      GitVersionDescriptor versionDescriptor = null)
    {
      List<RequestMediaType> supportedTypes;
      if (readablePagePath.Equals("/"))
        supportedTypes = new List<RequestMediaType>()
        {
          RequestMediaType.Json,
          RequestMediaType.Zip,
          RequestMediaType.None
        };
      else
        supportedTypes = new List<RequestMediaType>()
        {
          RequestMediaType.Json,
          RequestMediaType.Zip,
          RequestMediaType.Text,
          RequestMediaType.None
        };
      VersionControlRecursionType result;
      if (!System.Enum.TryParse<VersionControlRecursionType>(recursionLevel, true, out result))
        throw new ArgumentException(Microsoft.TeamFoundation.Wiki.Web.Resources.GetPageInvalidRecursionLevelError);
      ITfsGitRepository wikiRepository = this.GetWikiRepository(wiki);
      this.Request.RegisterForDispose((IDisposable) wikiRepository);
      if (versionDescriptor == null || string.IsNullOrEmpty(versionDescriptor.Version))
        versionDescriptor = wiki.Versions.ToList<GitVersionDescriptor>()[0];
      List<RequestMediaType> prioritizedAcceptHeaders = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, supportedTypes);
      RequestMediaType requestMediaType1 = prioritizedAcceptHeaders != null ? prioritizedAcceptHeaders.First<RequestMediaType>() : RequestMediaType.None;
      RequestMediaType requestMediaType2 = requestMediaType1 == RequestMediaType.None ? RequestMediaType.Json : requestMediaType1;
      int num1;
      switch (requestMediaType2)
      {
        case RequestMediaType.Zip:
          num1 = 120;
          break;
        case RequestMediaType.Text:
          num1 = 0;
          break;
        default:
          num1 = (int) result;
          break;
      }
      VersionControlRecursionType recursionType = (VersionControlRecursionType) num1;
      WikiPagesProvider wikiPagesProvider = new WikiPagesProvider();
      WikiPage wikiPage = (WikiPage) null;
      int num2;
      switch (requestMediaType2)
      {
        case RequestMediaType.Json:
          num2 = includeContentInJson ? 1 : 0;
          break;
        case RequestMediaType.Text:
          num2 = 1;
          break;
        default:
          num2 = 0;
          break;
      }
      bool includeContent = num2 != 0;
      bool includePageId = requestMediaType2 == RequestMediaType.Json;
      using (WikiPagesOrderReader pagesOrderReader = new WikiPagesOrderReader())
        wikiPage = wikiPagesProvider.GetPage(this.TfsRequestContext, wikiRepository, wiki.Id, wiki.MappedPath, WikiPagePath.FromWikiPagePath(readablePagePath), versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader, recursionType, includeContent, includePageId);
      if (includePageId)
      {
        string str = "PagesController: GetPage";
        this.TfsRequestContext.TraceAlways(15250001, TraceLevel.Info, "Wiki", str, string.Format("Wiki id:{0}, version:{1} Page id present: {2}", (object) wiki.Id, (object) versionDescriptor.Version, (object) wikiPage.Id.HasValue));
        TelemetryUtil.EmitHasPageIdCiDataIfNeeded(this.TfsRequestContext, wiki, versionDescriptor, wikiPage, str, "Wiki", "Service");
      }
      HttpResponseMessage response1 = this.Request.CreateResponse(HttpStatusCode.OK);
      switch (requestMediaType2)
      {
        case RequestMediaType.None:
        case RequestMediaType.OctetStream:
          response1.Headers.ETag = new EntityTagHeaderValue("\"" + wikiPage.InternalItem?.ObjectId + "\"");
          return response1;
        case RequestMediaType.Zip:
          GitItemsCollection gitItems = new GitItemsCollection();
          gitItems.AddRange((IEnumerable<GitItem>) PageHelper.FlattenWikiPage(wikiPage).Select<WikiPage, GitItem>((Func<WikiPage, GitItem>) (page => page.InternalItem)).ToList<GitItem>());
          response1.Content = (HttpContent) GitFileUtility.GetZipPushStreamContent(wikiRepository, gitItems, GitSecuredObjectFactory.CreateRepositoryReadOnly(wikiRepository.Key));
          string str1 = readablePagePath.Equals("/") || wikiPage.InternalItem?.Path == null ? wikiRepository.Name : Path.GetFileNameWithoutExtension(wikiPage.InternalItem?.Path);
          response1.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
          {
            FileName = str1 + ".zip"
          };
          goto case RequestMediaType.None;
        case RequestMediaType.Text:
          wikiPage.SetSecuredObject(GitSecuredObjectFactory.CreateRepositoryReadOnly(wikiRepository.Key));
          response1.Content = (HttpContent) new VssServerStringContent(wikiPage.Content, (object) wikiPage);
          goto case RequestMediaType.None;
        default:
          ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(wikiRepository.Key);
          wikiPage.SetSecuredObject(repositoryReadOnly);
          HttpResponseMessage response2 = this.Request.CreateResponse<WikiPage>(HttpStatusCode.OK, wikiPage);
          response2.Headers.ETag = new EntityTagHeaderValue("\"" + wikiPage.InternalItem?.ObjectId + "\"");
          return response2;
      }
    }

    private WikiPageChange GetValidPageChange(string pageFilePath, string content, string comment)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content), Microsoft.TeamFoundation.Wiki.Web.Resources.InvalidParametersOrNull);
      WikiPageChange validPageChange = new WikiPageChange();
      validPageChange.Path = WikiPagePath.FromWikiPagePath(pageFilePath);
      validPageChange.NewPath = (WikiPagePath) null;
      validPageChange.NewOrder = new int?();
      validPageChange.Content = content;
      validPageChange.Comment = !string.IsNullOrEmpty(comment) ? comment : (string) null;
      return validPageChange;
    }

    private HttpResponseMessage CreateWikiPageResponse(
      ITfsGitRepository repository,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion,
      WikiPageChange pageChange,
      string previousHeadCommit,
      int? pageId,
      string callerName,
      out WikiPage wikiPage)
    {
      WikiPagePath path = pageChange.Path;
      HttpStatusCode statusCode;
      switch (pageChange.ChangeType)
      {
        case WikiChangeType.Add:
          statusCode = HttpStatusCode.Created;
          break;
        default:
          statusCode = HttpStatusCode.OK;
          break;
      }
      string str;
      if (pageChange.ChangeType == WikiChangeType.Delete)
      {
        ref WikiPage local = ref wikiPage;
        WikiPagesProvider wikiPagesProvider = new WikiPagesProvider();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        ITfsGitRepository repository1 = repository;
        Guid id = wiki.Id;
        string mappedPath = wiki.MappedPath;
        WikiPagePath wikiPagePath = path;
        GitVersionDescriptor versionDescriptor = new GitVersionDescriptor();
        versionDescriptor.VersionType = GitVersionType.Commit;
        versionDescriptor.Version = previousHeadCommit;
        WikiPagesOrderReader pagesOrderReader = new WikiPagesOrderReader();
        // ISSUE: explicit non-virtual call
        WikiPage page = __nonvirtual (wikiPagesProvider.GetPage(tfsRequestContext, repository1, id, mappedPath, wikiPagePath, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader));
        local = page;
        str = "";
      }
      else
      {
        wikiPage = new WikiPagesProvider().GetPage(this.TfsRequestContext, repository, wiki.Id, wiki.MappedPath, path, wikiVersion, (IWikiPagesOrderReader) new WikiPagesOrderReader(), includeContent: pageChange.ChangeType == WikiChangeType.Add || pageChange.ChangeType == WikiChangeType.Edit, includePageId: !pageId.HasValue);
        if (pageId.HasValue)
          wikiPage.Id = new int?(pageId.Value);
        this.TfsRequestContext.TraceAlways(15250001, TraceLevel.Info, "Wiki", "PagesController: " + callerName, string.Format("Wiki id:{0}, version:{1} Page id present: {2}", (object) wiki.Id, (object) wikiVersion.Version, (object) wikiPage.Id.HasValue));
        TelemetryUtil.EmitHasPageIdCiDataIfNeeded(this.TfsRequestContext, wiki, wikiVersion, wikiPage, callerName, "Wiki", "Service");
        str = wikiPage.InternalItem.ObjectId;
      }
      HttpResponseMessage response = this.Request.CreateResponse<WikiPage>(statusCode, wikiPage);
      response.Headers.ETag = new EntityTagHeaderValue("\"" + str + "\"");
      return response;
    }

    private HttpResponseMessage CreateOrUpdatePageByPath(
      string path,
      string comment,
      string pageContent,
      WikiV2 wiki,
      GitVersionDescriptor versionDescriptor,
      string callerName)
    {
      WikiPageChange pageChange = this.GetValidPageChange(path, pageContent, comment);
      versionDescriptor = this.ValidateAndGetWikiVersion(wiki, versionDescriptor);
      ITfsGitRepository repository = this.GetWikiRepository(wiki);
      this.Request.RegisterForDispose((IDisposable) repository);
      if (repository.IsInMaintenance)
        throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
      try
      {
        ETaggedPageActionPerformer pageActionPerformer = new ETaggedPageActionPerformer(repository, path, (Func<string>) (() => WikiGitHelper.GetWikiItemGitObjectId(this.TfsRequestContext, repository, versionDescriptor, Microsoft.TeamFoundation.Wiki.Server.PathHelper.GetPageFilePath(path, wiki.MappedPath).GetPagePathWithExtension())), (Func<TfsGitRefUpdateResultSet>) (() =>
        {
          int pageDepth = Microsoft.TeamFoundation.Wiki.Server.PathHelper.GetPageDepth(path);
          pageChange.ChangeType = WikiChangeType.Add;
          pageChange.NewOrder = new int?(pageDepth <= 1 ? 1 : 0);
          pageChange.Comment = pageChange.Comment ?? string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.PageAddedDefaultComment, (object) pageChange.Path);
          return this.PerformActionOnWikiPage(repository, wiki.MappedPath, pageChange, versionDescriptor);
        }), (Func<TfsGitRefUpdateResultSet>) (() =>
        {
          pageChange.ChangeType = WikiChangeType.Edit;
          pageChange.Comment = pageChange.Comment ?? string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.PageEditedDefaultComment, (object) pageChange.Path);
          return this.PerformActionOnWikiPage(repository, wiki.MappedPath, pageChange, versionDescriptor);
        }));
        if (this.GetRefFromVersionDescriptor(repository, versionDescriptor) == null)
          throw new InvalidArgumentValueException("Version", string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiVersionInvalidOrDoesNotExist, (object) versionDescriptor.Version));
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        HttpRequestMessage request = this.Request;
        TfsGitRefUpdateResultSet refUpdateResultSet = pageActionPerformer.PerformAction(tfsRequestContext, request);
        int? pageId;
        this.TryProcessPushAndGetPageId(wiki, repository, versionDescriptor, refUpdateResultSet.PushId.Value, pageChange, 15250401, out pageId);
        WikiPage wikiPage;
        HttpResponseMessage wikiPageResponse = this.CreateWikiPageResponse(repository, wiki, versionDescriptor, pageChange, (string) null, pageId, callerName, out wikiPage);
        this.FollowPageIfRequired(wiki, wikiPage, pageChange);
        return wikiPageResponse;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(15252800, "Wiki", "Service", ex);
        throw;
      }
    }

    private void FollowPageIfRequired(WikiV2 wiki, WikiPage wikiPage, WikiPageChange pageChange)
    {
      using (TimedCiEvent ciEvent = new TimedCiEvent(this.TfsRequestContext, "Wiki", "Service"))
      {
        if (pageChange.ChangeType != WikiChangeType.Add)
          return;
        if (wikiPage.Id.HasValue)
        {
          NotificationUtil.FollowPage(this.TfsRequestContext, wiki.ProjectId, wiki.Id, wikiPage.Id.Value, true, "Service", ciEvent);
        }
        else
        {
          this.TfsRequestContext.TraceAlways(15250811, TraceLevel.Warning, "Wiki", "Service", "Could not follow to the newly created page as page id is not available.");
          ciEvent.Properties.AddOrIncrement("PageIdNotFound", 1L);
        }
      }
    }

    private HttpResponseMessage DeletePageByPath(
      string path,
      string comment,
      WikiV2 wiki,
      GitVersionDescriptor versionDescriptor,
      string callerName,
      int? pageId = null)
    {
      ITfsGitRepository wikiRepository = this.GetWikiRepository(wiki);
      this.Request.RegisterForDispose((IDisposable) wikiRepository);
      if (wikiRepository.IsInMaintenance)
        throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
      WikiPageChange wikiPageChange1 = new WikiPageChange();
      wikiPageChange1.Path = WikiPagePath.FromWikiPagePath(path);
      wikiPageChange1.ChangeType = WikiChangeType.Delete;
      wikiPageChange1.Comment = comment ?? string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.PageDeletedDefaultComment, (object) path);
      WikiPageChange wikiPageChange2 = wikiPageChange1;
      versionDescriptor = this.ValidateAndGetWikiVersion(wiki, versionDescriptor);
      string previousHeadCommit = (this.GetRefFromVersionDescriptor(wikiRepository, versionDescriptor) ?? throw new InvalidArgumentValueException("Version", string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiVersionInvalidOrDoesNotExist, (object) versionDescriptor.Version))).ObjectId.ToString();
      this.PerformActionOnWikiPage(wikiRepository, wiki.MappedPath, wikiPageChange2, versionDescriptor);
      return this.CreateWikiPageResponse(wikiRepository, wiki, versionDescriptor, wikiPageChange2, previousHeadCommit, pageId, callerName, out WikiPage _);
    }
  }
}
