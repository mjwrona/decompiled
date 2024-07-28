// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public static class BuildExtensions
  {
    public static MvcHtmlString SecurityOptions(this HtmlHelper htmlHelper) => htmlHelper.JsonIsland((object) new
    {
      permissionSet = BuildSecurity.BuildNamespaceId,
      separator = BuildSecurity.NamespaceSeparator,
      projectGuid = htmlHelper.ViewContext.RequestContext.TfsWebContext().CurrentProjectGuid
    }, (object) new{ @class = "options" });

    private static PivotFilter CreateDropdownPivotFilter(
      string name,
      IEnumerable<PivotFilterItem> items)
    {
      return new PivotFilter(name, items)
      {
        Behavior = PivotFilterBehavior.Dropdown
      };
    }

    public static MvcHtmlString QueuedUserFilter(this HtmlHelper htmlHelper)
    {
      PivotFilter dropdownPivotFilter = BuildExtensions.CreateDropdownPivotFilter(BuildServerResources.BuildFilterBy, (IEnumerable<PivotFilterItem>) new PivotFilterItem[2]
      {
        new PivotFilterItem(BuildServerResources.BuildFilterAnyone, (object) "anyone")
        {
          Selected = true
        },
        new PivotFilterItem(BuildServerResources.BuildFilterMe, (object) "me")
        {
          Selected = false
        }
      });
      return htmlHelper.Control<PivotFilter>(dropdownPivotFilter, (object) new
      {
        @class = "build-pane-filter builds-queued queued-build-user"
      });
    }

    public static MvcHtmlString CompletedUserFilter(this HtmlHelper htmlHelper)
    {
      PivotFilter dropdownPivotFilter = BuildExtensions.CreateDropdownPivotFilter(BuildServerResources.BuildFilterBy, (IEnumerable<PivotFilterItem>) new PivotFilterItem[2]
      {
        new PivotFilterItem(BuildServerResources.BuildFilterAnyone, (object) "anyone")
        {
          Selected = true
        },
        new PivotFilterItem(BuildServerResources.BuildFilterMe, (object) "me")
        {
          Selected = false
        }
      });
      return htmlHelper.Control<PivotFilter>(dropdownPivotFilter, (object) new
      {
        @class = "build-pane-filter builds-completed completed-build-user"
      });
    }

    public static MvcHtmlString StatusFilter(this HtmlHelper htmlHelper)
    {
      PivotFilter dropdownPivotFilter = BuildExtensions.CreateDropdownPivotFilter(BuildServerResources.BuildFilterStatus, (IEnumerable<PivotFilterItem>) new PivotFilterItem[4]
      {
        new PivotFilterItem(BuildServerResources.BuildFilterAny, (object) QueueStatus.All)
        {
          Selected = true
        },
        new PivotFilterItem(BuildServerResources.BuildFilterInProgress, (object) QueueStatus.InProgress)
        {
          Selected = false
        },
        new PivotFilterItem(BuildServerResources.BuildFilterQueued, (object) QueueStatus.Queued)
        {
          Selected = false
        },
        new PivotFilterItem(BuildServerResources.BuildFilterPostponed, (object) QueueStatus.Postponed)
        {
          Selected = false
        }
      });
      return htmlHelper.Control<PivotFilter>(dropdownPivotFilter, (object) new
      {
        @class = "build-pane-filter builds-queued build-status"
      });
    }

    public static MvcHtmlString BuildDateFilter(this HtmlHelper htmlHelper)
    {
      PivotFilter dropdownPivotFilter = BuildExtensions.CreateDropdownPivotFilter(BuildServerResources.BuildFilterDate, (IEnumerable<PivotFilterItem>) new PivotFilterItem[7]
      {
        new PivotFilterItem(BuildServerResources.BuildFilterAny, (object) "any")
        {
          Selected = false
        },
        new PivotFilterItem(BuildServerResources.BuildFilterToday, (object) string.Empty)
        {
          Selected = true
        },
        new PivotFilterItem(BuildServerResources.BuildFilterLast24Hours, (object) "l24h")
        {
          Selected = false
        },
        new PivotFilterItem(BuildServerResources.BuildFilterLast48Hours, (object) "l48h")
        {
          Selected = false
        },
        new PivotFilterItem(BuildServerResources.BuildFilterLast7Days, (object) "l7d")
        {
          Selected = false
        },
        new PivotFilterItem(BuildServerResources.BuildFilterLast14Days, (object) "l14d")
        {
          Selected = false
        },
        new PivotFilterItem(BuildServerResources.BuildFilterLast28Days, (object) "l28d")
        {
          Selected = false
        }
      });
      return htmlHelper.Control<PivotFilter>(dropdownPivotFilter, (object) new
      {
        @class = "build-pane-filter builds-completed completed-build-date"
      });
    }

    public static MvcHtmlString BuildQualityFilter(
      this HtmlHelper htmlHelper,
      List<string> buildQualities)
    {
      List<PivotFilterItem> items = new List<PivotFilterItem>();
      items.Add(new PivotFilterItem(BuildServerResources.BuildFilterAny, (object) string.Empty)
      {
        Selected = true
      });
      foreach (string text in (IEnumerable<string>) buildQualities.OrderBy<string, string>((Func<string, string>) (q => q), (IComparer<string>) StringComparer.OrdinalIgnoreCase))
        items.Add(new PivotFilterItem(text, (object) text)
        {
          Selected = false
        });
      PivotFilter dropdownPivotFilter = BuildExtensions.CreateDropdownPivotFilter(BuildServerResources.BuildFilterQuality, (IEnumerable<PivotFilterItem>) items);
      return htmlHelper.Control<PivotFilter>(dropdownPivotFilter, (object) new
      {
        @class = "build-pane-filter builds-completed completed-build-quality"
      });
    }

    public static MvcHtmlString BuildControllerFilter(this HtmlHelper htmlHelper)
    {
      PivotFilter dropdownPivotFilter = BuildExtensions.CreateDropdownPivotFilter(BuildServerResources.BuildFilterControllers, (IEnumerable<PivotFilterItem>) new List<PivotFilterItem>()
      {
        new PivotFilterItem(BuildServerResources.BuildFilterAny, (object) string.Empty)
        {
          Selected = true
        }
      });
      return htmlHelper.Control<PivotFilter>(dropdownPivotFilter, (object) new
      {
        @class = "build-pane-filter builds-queued build-controllers"
      });
    }

    public static MvcHtmlString BuildDetailOptions(
      this HtmlHelper htmlHelper,
      BuildDetailViewModel model)
    {
      return htmlHelper.JsonIsland((object) model.ToJson(), (object) new
      {
        @class = "build-detail-options"
      });
    }

    public static MvcHtmlString BuildQualities(
      this HtmlHelper htmlHelper,
      IEnumerable<string> buildQualities)
    {
      HtmlHelper htmlHelper1 = htmlHelper;
      JsObject data = new JsObject();
      data.Add("qualities", (object) buildQualities);
      var htmlAttributes = new{ @class = "build-qualities" };
      return htmlHelper1.JsonIsland((object) data, (object) htmlAttributes);
    }

    public static MvcHtmlString BuildReportExtensions(
      this HtmlHelper htmlHelper,
      BuildViewModel model)
    {
      HtmlHelper htmlHelper1 = htmlHelper;
      JsObject data = new JsObject();
      data.Add("summarySections", (object) model.SummarySections);
      data.Add("logNodes", (object) model.LogNodes);
      var htmlAttributes = new
      {
        @class = "build-extensions"
      };
      return htmlHelper1.JsonIsland((object) data, (object) htmlAttributes);
    }

    public static MvcHtmlString ValidTimeZones(this HtmlHelper htmlHelper)
    {
      HtmlHelper htmlHelper1 = htmlHelper;
      JsObject data = new JsObject();
      data.Add("validTimeZones", (object) TimeZoneInfo.GetSystemTimeZones());
      var htmlAttributes = new
      {
        @class = "valid-time-zones"
      };
      return htmlHelper1.JsonIsland((object) data, (object) htmlAttributes);
    }

    public static JsObject ToJson(this BuildDeployment model)
    {
      JsObject json = new JsObject();
      json.Add("name", (object) model.Deployment.Number);
      json.Add("uri", (object) model.Deployment.Uri);
      json.Add("definition", (object) model.DeploymentDefinitionName);
      json.Add("quality", (object) model.Deployment.Quality);
      json.Add("status", (object) model.Deployment.Status);
      json.Add("date", (object) model.Deployment.FinishTime);
      json.Add("reason", (object) model.Deployment.Reason);
      json.Add("retain", (object) model.Deployment.KeepForever);
      json.Add("changesets", (object) model.Source.ChangeSet.Select<ChangesetDisplayInformation, int>((Func<ChangesetDisplayInformation, int>) (x => x.ChangesetId)));
      json.Add("requestedBy", (object) model.Deployment.RequestedFor.Select<RequestedForDisplayInformation, Guid>((Func<RequestedForDisplayInformation, Guid>) (x => x.TeamFoundationId)));
      return json;
    }

    public static MvcHtmlString BuildAdminViewOptions(
      this HtmlHelper htmlHelper,
      BuildSettingsAdminViewModel model)
    {
      return htmlHelper.RestApiJsonIsland((object) model, (object) new
      {
        @class = "options"
      });
    }

    public static MvcHtmlString BuildAgentQueueOptions(
      this HtmlHelper htmlHelper,
      AgentQueueAdminViewModel model)
    {
      return htmlHelper.RestApiJsonIsland((object) model, (object) new
      {
        @class = "options"
      });
    }
  }
}
