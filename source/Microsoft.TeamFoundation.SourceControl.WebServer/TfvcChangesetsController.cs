// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesetsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(1.0)]
  public class TfvcChangesetsController : TfvcApiController
  {
    private const string ChangesetsAscendingOrderById = "id asc";

    [ClientExample("GET__tfvc_changesets__id_.json", "GET a changeset", null, null)]
    [ClientExample("GET__tfvc_changesets__id__includeDetails-true.json", "Get a changeset with policy details and check-in notes.", null, null)]
    [ClientExample("GET__tfvc_changesets__id__includeWorkItems-true.json", "Get a changeset with workitems.", null, null)]
    [ClientExample("GET__tfvc_changesets__id__maxChangeCount.json", "Get a changeset with a max number of changes. ", null, null)]
    [HttpGet]
    [ClientResponseType(typeof (TfvcChangeset), null, null)]
    public HttpResponseMessage GetChangeset(
      [ClientParameterType(typeof (int), false)] string id,
      int maxChangeCount = 0,
      bool includeDetails = false,
      bool includeWorkItems = false,
      int? maxCommentLength = null,
      bool includeSourceRename = false,
      [ClientIgnore, FromUri(Name = "path")] string path = "",
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$orderby")] string orderby = "id",
      [ModelBinder] TfvcChangesetSearchCriteria searchCriteria = null)
    {
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(30.0);
      if (string.IsNullOrWhiteSpace(id))
        return this.GenerateChangesetsResponse(maxCommentLength, path, skip, top, orderby, searchCriteria);
      int changesetNumber = VersionSpecCommon.ParseChangesetNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, id);
      int maxChangeCount1 = 0;
      if (maxChangeCount > 0)
        maxChangeCount1 = Math.Min(maxChangeCount, 100);
      TfvcChangeset changesetById = TfvcChangesetUtility.GetChangesetById(this.TfsRequestContext, this.Url, changesetNumber, maxChangeCount1, includeSourceRename, includeDetails, includeWorkItems, maxCommentLength.GetValueOrDefault(2000));
      changesetById.Links = changesetById.GetChangesetsReferenceLinks(this.TfsRequestContext, this.Url);
      return this.Request.CreateResponse<TfvcChangeset>(HttpStatusCode.OK, changesetById);
    }

    [ClientExample("GET__tfvc_changesets.json", "GET list of changesets", null, null)]
    [ClientExample("GET__tfvc_changesets_searchCriteria.itemPath-_path_.json", "GET the changesets that contain changes to the specified item.", null, null)]
    [ClientExample("GET__tfvc_changesets_searchcriteria.author-_author_.json", "GET the changesets filtered by author.", null, null)]
    [ClientExample("GET__tfvc_changesets_fromId-_from__toId-_to_.json", "GET the changesets for a range of changeset IDs.", null, null)]
    [ClientExample("GET__tfvc_changesets_fromDate-_fromDate__toDate-_toDate_.json", "GET the changesets for a range of dates.", null, null)]
    [ClientExample("GET__tfvc_changesets__top-_top___skip-_skip_.json", "GET the changesets a page at a time.", null, null)]
    [ClientExample("GET__tfvc_changesets__orderBy-_order_.json", "GET the changesets ordering by ID ascending.", null, null)]
    [ClientExample("GET__tfvc_changesets_maxCommentLength-_commentLength_.json", "GET changesets with more or less of each comment. By default 80 characters are returned.", null, null)]
    [HttpGet]
    [ClientResponseType(typeof (List<TfvcChangesetRef>), null, null)]
    public virtual HttpResponseMessage GetChangesets(
      string maxChangeCount = "0",
      bool includeDetails = false,
      bool includeWorkItems = false,
      int? maxCommentLength = null,
      bool includeSourceRename = false,
      [ClientIgnore, FromUri(Name = "path")] string path = "",
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$orderby")] string orderby = "id",
      [ModelBinder] TfvcChangesetSearchCriteria searchCriteria = null)
    {
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(30.0);
      return this.GenerateChangesetsResponse(maxCommentLength, path, skip, top, orderby, searchCriteria);
    }

    protected HttpResponseMessage GenerateChangesetsResponse(
      int? maxCommentLength = null,
      string path = "",
      int? skip = null,
      int? top = null,
      string orderby = "id",
      TfvcChangesetSearchCriteria searchCriteria = null)
    {
      int skip1 = Math.Max(0, skip.GetValueOrDefault());
      int top1 = Math.Max(0, top ?? 100);
      if (searchCriteria == null)
        searchCriteria = new TfvcChangesetSearchCriteria();
      if (orderby == null)
        orderby = "id";
      if (string.IsNullOrEmpty(searchCriteria.ItemPath))
        searchCriteria.ItemPath = path;
      if (this.ProjectId != Guid.Empty)
      {
        if (searchCriteria.Mappings.Count == 0)
        {
          searchCriteria.ItemPath = this.ProjectScopedPath(searchCriteria.ItemPath);
        }
        else
        {
          foreach (TfvcMappingFilter mapping in (IEnumerable<TfvcMappingFilter>) searchCriteria.Mappings)
            mapping.ServerPath = this.ProjectScopedPath(mapping.ServerPath);
        }
      }
      bool moreChangesetsAvailable = false;
      bool ascendingOrder = false;
      foreach (string str in orderby.Split(TfvcApiController.s_orderByFieldSeparator))
      {
        if (str.Equals("id asc", StringComparison.OrdinalIgnoreCase))
          ascendingOrder = true;
      }
      HttpResponseMessage response = this.Request.CreateResponse<TfvcChangesetsCollection>(HttpStatusCode.OK, TfvcChangesetUtility.QueryChangesets(this.TfsRequestContext, this.Url, searchCriteria, top1, skip1, maxCommentLength.GetValueOrDefault(80), ascendingOrder, out moreChangesetsAvailable));
      if (moreChangesetsAvailable)
      {
        string str = this.Url.RestLink(this.TfsRequestContext, TfvcConstants.TfvcChangesetsLocationId, (object) new Dictionary<string, object>()
        {
          {
            "$top",
            (object) top1
          },
          {
            "$skip",
            (object) (skip1 + top1)
          },
          {
            "$orderby",
            (object) orderby
          }
        });
        response.Headers.Add("Link", string.Format(Resources.Get("LinkHeaderForNextPage"), (object) str));
      }
      return response;
    }
  }
}
