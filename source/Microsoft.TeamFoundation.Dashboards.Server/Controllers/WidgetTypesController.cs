// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.WidgetTypesController
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Dashboards.Controllers
{
  [VersionedApiControllerCustomName("Dashboard", "WidgetTypes", 1)]
  public class WidgetTypesController : DashboardApiControllerBase
  {
    public override string TraceArea => "WidgetTypes";

    [ClientExample("GET__dashboard_WidgetTypes.json", null, null, null)]
    [TraceFilter(10017408, 10017409)]
    [HttpGet]
    [ApplyRequestLanguage]
    public WidgetTypesResponse GetWidgetTypes([FromUri(Name = "$scope")] WidgetScope scope)
    {
      IEnumerable<WidgetMetadata> filteredWidgetsMetadata = this.TfsRequestContext.GetService<IWidgetTypesService>().GetFilteredWidgetsMetadata(this.TfsRequestContext, (Func<WidgetMetadata, bool>) (o => o.SupportedScopes.Contains<WidgetScope>(scope)));
      return filteredWidgetsMetadata != null && filteredWidgetsMetadata.Count<WidgetMetadata>() > 0 ? this.CreateWidgetTypesResponse(this.TfsRequestContext, filteredWidgetsMetadata) : throw new NoMetaDataFoundInCatalogException();
    }

    [ClientExample("GET__dashboard_WidgetTypes__widgetId_.json", null, null, null)]
    [TraceFilter(10017406, 10017407)]
    [HttpGet]
    public WidgetMetadataResponse GetWidgetMetadata(string contributionId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(contributionId, nameof (contributionId), "Dashboards");
      IEnumerator<WidgetMetadata> enumerator = this.TfsRequestContext.GetService<IWidgetTypesService>().GetFilteredWidgetsMetadata(this.TfsRequestContext, (Func<WidgetMetadata, bool>) (o => o.ContributionId == contributionId)).GetEnumerator();
      if (enumerator.MoveNext())
        return this.CreateWidgetMetadataResponse(enumerator.Current, contributionId);
      throw new NoMetaDataFoundInCatalogException();
    }
  }
}
