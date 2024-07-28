// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesets3Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(3.0)]
  [ClientGroupByResource("changesets")]
  [VersionedApiControllerCustomName(Area = "tfvc", ResourceName = "changesets", ResourceVersion = 3)]
  public class TfvcChangesets3Controller : TfvcChangesets2Controller
  {
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
    public override HttpResponseMessage GetChangesets(
      [ClientIgnore] string maxChangeCount = "0",
      [ClientIgnore] bool includeDetails = false,
      [ClientIgnore] bool includeWorkItems = false,
      int? maxCommentLength = null,
      [ClientIgnore] bool includeSourceRename = false,
      [ClientIgnore, FromUri(Name = "path")] string path = "",
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$orderby")] string orderby = "id",
      [ModelBinder] TfvcChangesetSearchCriteria searchCriteria = null)
    {
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(30.0);
      return this.GenerateChangesetsResponse(maxCommentLength, path, skip, top, orderby, searchCriteria);
    }
  }
}
