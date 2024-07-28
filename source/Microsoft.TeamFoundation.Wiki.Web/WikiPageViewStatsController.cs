// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPageViewStatsController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [FeatureEnabled("WebAccess.Wiki.PageViewStats")]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pageViewStats")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WikiPageViewStatsController : WikiApiController
  {
    [HttpPost]
    [ClientResourceOperation(ClientResourceOperationName.CreateOrUpdate)]
    [ClientResponseType(typeof (WikiPageViewStats), null, null)]
    [ClientLocationId("1087b746-5d15-41b9-bea6-14e325e7f880")]
    [ClientInternalUseOnly(false)]
    public HttpResponseMessage CreateOrUpdatePageViewStats(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor wikiVersion,
      [ClientQueryParameter] string path,
      [ClientQueryParameter] string oldPath = null)
    {
      if (string.IsNullOrEmpty(path) || path.Equals("/"))
        throw new InvalidArgumentValueException(nameof (path), Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageWriteOperationInvalidPagePath);
      WikiV2 wikiByIdentifier = WikiV2Helper.GetWikiByIdentifier(this.TfsRequestContext, this.ProjectId, wikiIdentifier);
      if (wikiByIdentifier == null)
        throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound);
      if (wikiVersion == null)
        throw new InvalidArgumentValueException(nameof (wikiVersion), Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageInvalidWikiVersion);
      ITfsGitRepository wikiRepository = this.GetWikiRepository(wikiByIdentifier);
      this.Request.RegisterForDispose((IDisposable) wikiRepository);
      WikiPagesProvider wikiPagesProvider = new WikiPagesProvider();
      WikiPagesOrderReader pagesOrderReader1 = new WikiPagesOrderReader();
      string mappedPath = wikiByIdentifier.MappedPath;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ITfsGitRepository repository = wikiRepository;
      Guid id = wikiByIdentifier.Id;
      string rootPath = mappedPath;
      WikiPagePath wikiPagePath = WikiPagePath.FromWikiPagePath(path);
      GitVersionDescriptor versionDescriptor = wikiVersion;
      WikiPagesOrderReader pagesOrderReader2 = pagesOrderReader1;
      wikiPagesProvider.GetPage(tfsRequestContext, repository, id, rootPath, wikiPagePath, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader2);
      try
      {
        WikiPageViewStats pageViewStats;
        new WikiPageViewStatsProvider().CreateOrUpdatePageViewStats(this.TfsRequestContext, this.ProjectId, wikiByIdentifier.Id, wikiVersion, path, oldPath, out pageViewStats);
        return this.Request.CreateResponse<WikiPageViewStats>(oldPath != null ? HttpStatusCode.OK : HttpStatusCode.Created, pageViewStats);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(15252802, "Wiki", "Service", ex);
        throw;
      }
    }
  }
}
