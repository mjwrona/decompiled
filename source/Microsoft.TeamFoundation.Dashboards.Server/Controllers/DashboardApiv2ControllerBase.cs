// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Controllers.DashboardApiv2ControllerBase
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Dashboards.Model.RestResponse;
using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Dashboards.Controllers
{
  public abstract class DashboardApiv2ControllerBase : TfsTeamApiController
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
        typeof (WidgetConflictOnCopyException),
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
      },
      {
        typeof (WidgetServiceUnavailableException),
        HttpStatusCode.InternalServerError
      }
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => DashboardApiv2ControllerBase.s_CommonHttpExceptions;

    public override string ActivityLogArea => "Dashboards";

    internal string GetDashboardUrl(Guid? dashboardId) => this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.DashboardV2, (object) new
    {
      dashboardId = dashboardId
    });

    internal string GetWidgetUrl(Guid widgetId) => this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.WidgetV2, (object) new
    {
      widgetId = widgetId
    });

    private DashboardGroupEntryResponse CreateDashboardEntryResponseInner(DashboardGroupEntry entry)
    {
      string dashboardUrl = this.GetDashboardUrl(new Guid?(entry.Id.Value));
      return new DashboardGroupEntryResponse(entry, dashboardUrl);
    }

    private WidgetResponse CreateWidgetResponseInner(Widget widget)
    {
      string widgetUrl = this.GetWidgetUrl(widget.Id.Value);
      WidgetResponse widgetResponseInner = new WidgetResponse(widget, widgetUrl);
      widgetResponseInner.SetSecuredObject((ISecuredObject) widget);
      return widgetResponseInner;
    }

    protected DashboardGroup CreateDashboardGroupResponse(
      Guid groupId,
      DashboardGroup dashboardGroup)
    {
      DashboardGroupEntryResponse[] array = dashboardGroup.DashboardEntries.Select<DashboardGroupEntry, DashboardGroupEntryResponse>(new Func<DashboardGroupEntry, DashboardGroupEntryResponse>(this.CreateDashboardEntryResponseInner)).ToArray<DashboardGroupEntryResponse>();
      string dashboardUrl = this.GetDashboardUrl(new Guid?());
      DashboardGroup response = new DashboardGroup((IEnumerable<DashboardGroupEntry>) array, dashboardGroup.Permission, dashboardUrl);
      response.TeamDashboardPermission = dashboardGroup.TeamDashboardPermission;
      response.AddLinks((IEnumerable<DashboardGroupEntryResponse>) array);
      response.SetSecuredObject((ISecuredObject) dashboardGroup);
      return response;
    }

    protected DashboardGroupEntryResponse CreateDashboardEntryResponse(
      Guid groupId,
      DashboardGroupEntry entry)
    {
      DashboardGroupEntryResponse entryResponseInner = this.CreateDashboardEntryResponseInner(entry);
      entryResponseInner.AddLinks(this.GetDashboardUrl(new Guid?()));
      return entryResponseInner;
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
        bool memberClaim = this.GetMemberClaim();
        IEnumerable<WidgetMetadata> metaDataList = this.GetMetaDataList(widgetResponses);
        foreach (WidgetResponse widgetResponse in widgetResponses)
        {
          widgetResponse.SetMetaData(this.GetMetaData(metaDataList, widgetResponse.ContributionId, (ISecuredObject) widgetResponse));
          if (!memberClaim && !widgetResponse.IsEnabled)
            widgetResponse.Settings = (string) null;
        }
      }
      DashboardResponse response = new DashboardResponse(widgetResponses, entry, this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.DashboardV2, (object) new
      {
      }));
      response.AddLinks(this.GetDashboardUrl(new Guid?()), widgetResponses);
      response.SetSecuredObject((ISecuredObject) entry);
      return response;
    }

    private WidgetMetadata GetMetaData(
      IEnumerable<WidgetMetadata> metaDataList,
      string contributionId,
      ISecuredObject securedObject)
    {
      IEnumerable<WidgetMetadata> source = metaDataList.Where<WidgetMetadata>((Func<WidgetMetadata, bool>) (m => m.ContributionId == contributionId));
      return (source == null || source.Count<WidgetMetadata>() == 0 ? WidgetMetadata.CreateDisabledInstance(contributionId) : source.First<WidgetMetadata>()).CloneWithSecuredObject(securedObject);
    }

    private IEnumerable<WidgetMetadata> GetMetaDataList(IEnumerable<WidgetResponse> responses)
    {
      IWidgetTypesService service = this.TfsRequestContext.GetService<IWidgetTypesService>();
      IEnumerable<string> contributionIdsInResponse = responses.Select<WidgetResponse, string>((Func<WidgetResponse, string>) (r => r.ContributionId));
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Func<WidgetMetadata, bool> filter = (Func<WidgetMetadata, bool>) (m => contributionIdsInResponse.Contains<string>(m.ContributionId));
      return service.GetFilteredWidgetsMetadata(tfsRequestContext, filter) ?? throw new NoMetaDataFoundInCatalogException();
    }

    private IEnumerable<WidgetMetadata> GetMetaDataList(IEnumerable<Widget> widgets)
    {
      IWidgetTypesService service = this.TfsRequestContext.GetService<IWidgetTypesService>();
      IEnumerable<string> contributionIdsInResponse = widgets.Select<Widget, string>((Func<Widget, string>) (r => r.ContributionId));
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
      WidgetResponse widgetResponseInner = this.CreateWidgetResponseInner(widget);
      string dashboardUrl1 = this.GetDashboardUrl(new Guid?(dashboardId));
      string dashboardUrl2 = this.GetDashboardUrl(new Guid?());
      IEnumerable<WidgetMetadata> metaDataList = this.GetMetaDataList(widgetResponseInner);
      int num = this.GetMemberClaim() ? 1 : 0;
      widgetResponseInner.SetMetaData(this.GetMetaData(metaDataList, widgetResponseInner.ContributionId, (ISecuredObject) widget));
      widgetResponseInner.AddLinks(dashboardUrl2, dashboardUrl1);
      if (num == 0 && !widgetResponseInner.IsEnabled)
        widgetResponseInner.Settings = (string) null;
      return widgetResponseInner;
    }

    protected WidgetTypesResponse CreateWidgetTypesResponse(
      IEnumerable<WidgetMetadata> widgetCatalog)
    {
      return new WidgetTypesResponse(widgetCatalog, this.Url.RestLink(this.TfsRequestContext, DashboardsLocationIds.WidgetTypes, (object) null));
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

    protected void PopulateMetadata(List<Widget> widgets)
    {
      IEnumerable<WidgetMetadata> metaDataList = this.GetMetaDataList((IEnumerable<Widget>) widgets.ToArray());
      bool memberClaim = this.GetMemberClaim();
      foreach (Widget widget in widgets)
      {
        widget.SetMetaData(this.GetMetaData(metaDataList, widget.ContributionId, (ISecuredObject) widget));
        if (!memberClaim && !widget.IsEnabled)
          widget.Settings = (string) null;
      }
    }
  }
}
