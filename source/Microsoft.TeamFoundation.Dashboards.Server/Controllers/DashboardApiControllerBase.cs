// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.DashboardApiControllerBase
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Model.RestResponse;
using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Dashboards.Controllers
{
  public abstract class DashboardApiControllerBase : TfsProjectApiController
  {
    private static readonly IDictionary<Type, HttpStatusCode> s_CommonHttpExceptions = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InsufficientPermissionsException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (DashboardEntryExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (DashboardDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (DashboardsDoNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (EmptyDashboardNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DashboardNameLengthExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DashboardDescriptionLengthExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DashboardInvalidPositionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (CreateDashboardWithExistingIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DashboardCountLimitExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (LastDashboardCannotBeDeletedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (WidgetETagConflictException),
        HttpStatusCode.Conflict
      },
      {
        typeof (WidgetCollisionException),
        HttpStatusCode.Conflict
      },
      {
        typeof (WidgetExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (WidgetWithoutIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetSizeNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetTypeDoesNotExistException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (EmptyWidgetNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetNameLengthExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetSettingsLengthExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetTypeLengthExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetSizeDimensionExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetPositionDimensionExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetSettingsVersionInvalidException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (CreateWidgetWithExistingIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (WidgetLimitExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NoMetaDataFoundInCatalogException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      }
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => DashboardApiControllerBase.s_CommonHttpExceptions;

    public override string ActivityLogArea => "Dashboards";

    private string GetGroupUrl(Guid groupId) => this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.DashboardGroup, (object) new
    {
      groupId = groupId
    });

    private string GetDashboardUrl(Guid dashboardId) => this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.Dashboard, (object) new
    {
      dashboardId = dashboardId
    });

    private string GetWidgetUrl(Guid widgetId) => this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.Widget, (object) new
    {
      widgetId = widgetId
    });

    private DashboardGroupEntryResponse CreateDashboardEntryResponseInner(DashboardGroupEntry entry)
    {
      string dashboardUrl = this.GetDashboardUrl(entry.Id.Value);
      return new DashboardGroupEntryResponse(entry, dashboardUrl);
    }

    private WidgetResponse CreateWidgetResponseInner(Widget widget)
    {
      string widgetUrl = this.GetWidgetUrl(widget.Id.Value);
      return new WidgetResponse(widget, widgetUrl);
    }

    protected DashboardGroup CreateDashboardGroupResponse(
      Guid groupId,
      DashboardGroup dashboardGroup)
    {
      DashboardGroupEntryResponse[] array = dashboardGroup.DashboardEntries.Select<DashboardGroupEntry, DashboardGroupEntryResponse>(new Func<DashboardGroupEntry, DashboardGroupEntryResponse>(this.CreateDashboardEntryResponseInner)).ToArray<DashboardGroupEntryResponse>();
      string groupUrl = this.GetGroupUrl(groupId);
      DashboardGroup response = new DashboardGroup((IEnumerable<DashboardGroupEntry>) array, groupUrl);
      response.AddLinks((IEnumerable<DashboardGroupEntryResponse>) array);
      return response;
    }

    protected DashboardGroupEntryResponse CreateDashboardEntryResponse(
      Guid groupId,
      Dashboard entry)
    {
      return this.CreateDashboardEntryResponse(groupId, new DashboardGroupEntry(entry));
    }

    protected DashboardGroupEntryResponse CreateDashboardEntryResponse(
      Guid groupId,
      DashboardGroupEntry entry)
    {
      DashboardGroupEntryResponse entryResponseInner = this.CreateDashboardEntryResponseInner(entry);
      entryResponseInner.AddLinks(this.GetGroupUrl(groupId));
      return entryResponseInner;
    }

    protected DashboardResponse CreateDashboardResponse(
      Guid groupId,
      Dashboard entry,
      bool includeWidgets = true)
    {
      return this.CreateDashboardResponse(groupId, new DashboardGroupEntry(entry), includeWidgets);
    }

    protected DashboardResponse CreateDashboardResponse(
      Guid groupId,
      DashboardGroupEntry entry,
      bool includeWidgets = true)
    {
      IEnumerable<WidgetResponse> widgetResponses = (IEnumerable<WidgetResponse>) null;
      if (includeWidgets)
      {
        widgetResponses = (IEnumerable<WidgetResponse>) entry.Widgets.Select<Widget, WidgetResponse>(new Func<Widget, WidgetResponse>(this.CreateWidgetResponseInner)).ToArray<WidgetResponse>();
        IEnumerable<WidgetMetadata> metaDataList = this.GetMetaDataList(widgetResponses);
        bool memberClaim = this.GetMemberClaim();
        foreach (WidgetResponse widgetResponse in widgetResponses)
        {
          widgetResponse.SetMetaData(this.GetMetaData(metaDataList, widgetResponse.ContributionId));
          if (!memberClaim && !widgetResponse.IsEnabled)
            widgetResponse.Settings = (string) null;
        }
      }
      DashboardResponse response = new DashboardResponse(widgetResponses, entry, this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.Dashboard, (object) new
      {
      }));
      response.AddLinks(this.GetGroupUrl(groupId), widgetResponses);
      return response;
    }

    private WidgetMetadata GetMetaData(
      IEnumerable<WidgetMetadata> metaDataList,
      string contributionId)
    {
      IEnumerable<WidgetMetadata> source = metaDataList.Where<WidgetMetadata>((Func<WidgetMetadata, bool>) (m => m.ContributionId == contributionId));
      return source == null || source.Count<WidgetMetadata>() == 0 ? WidgetMetadata.CreateDisabledInstance(contributionId) : source.First<WidgetMetadata>();
    }

    private IEnumerable<WidgetMetadata> GetMetaDataList(IEnumerable<WidgetResponse> responses)
    {
      IWidgetTypesService service = this.TfsRequestContext.GetService<IWidgetTypesService>();
      IEnumerable<string> contributionIdsInResponse = responses.Select<WidgetResponse, string>((Func<WidgetResponse, string>) (r => r.ContributionId));
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Func<WidgetMetadata, bool> filter = (Func<WidgetMetadata, bool>) (m => contributionIdsInResponse.Contains<string>(m.ContributionId));
      return service.GetFilteredWidgetsMetadata(tfsRequestContext, filter) ?? throw new NoMetaDataFoundInCatalogException();
    }

    private IEnumerable<WidgetMetadata> GetMetaDataList(WidgetResponse response) => this.GetMetaDataList((IEnumerable<WidgetResponse>) new List<WidgetResponse>()
    {
      response
    });

    private bool GetMemberClaim() => this.TfsRequestContext.GetService<IContributionClaimService>().HasClaim(this.TfsRequestContext, "Member");

    protected WidgetResponse CreateWidgetResponse(Guid groupId, Guid dashboardId, Widget widget)
    {
      WidgetResponse response = widget != null ? this.CreateWidgetResponseInner(widget) : throw new NoMetaDataFoundInCatalogException();
      string dashboardUrl = this.GetDashboardUrl(dashboardId);
      string groupUrl = this.GetGroupUrl(groupId);
      int num = this.GetMemberClaim() ? 1 : 0;
      IEnumerable<WidgetMetadata> metaDataList = this.GetMetaDataList(response);
      response.SetMetaData(this.GetMetaData(metaDataList, response.ContributionId));
      if (num == 0 && !response.IsEnabled)
        response.Settings = (string) null;
      response.AddLinks(groupUrl, dashboardUrl);
      return response;
    }

    protected WidgetTypesResponse CreateWidgetTypesResponse(
      IVssRequestContext requestContext,
      IEnumerable<WidgetMetadata> widgetCatalog)
    {
      WidgetTypesResponse response = new WidgetTypesResponse(widgetCatalog, this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.WidgetTypes, (object) null));
      string marketPlaceRootUrl = GalleryHelper.TryGetMarketPlaceRootUrl(requestContext);
      if (marketPlaceRootUrl != null)
        response.AddLinks(marketPlaceRootUrl);
      return response;
    }

    protected WidgetMetadataResponse CreateWidgetMetadataResponse(
      WidgetMetadata widgetMetadata,
      string contributionId)
    {
      return new WidgetMetadataResponse(widgetMetadata, this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.WidgetTypes, (object) new
      {
        contributionId = contributionId
      }));
    }
  }
}
