// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WorkItemExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class WorkItemExtensions
  {
    private static string _DEFAULT_GRID_CLASS = "query-result-grid";

    public static MvcHtmlString WorkItemViewOptions(this HtmlHelper htmlHelper) => htmlHelper.JsonIsland((object) new
    {
      queryItemPermissionSet = QueryItemSecurityConstants.NamespaceGuid
    }, (object) new{ @class = "options" });

    public static MvcHtmlString WorkItemFormOptions(
      this HtmlHelper htmlHelper,
      WorkItemFormModel model)
    {
      IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext;
      HtmlHelper htmlHelper1 = htmlHelper;
      JsObject data = new JsObject();
      data.Add("action", (object) model.Action);
      data.Add("id", (object) model.Id);
      data.Add("witd", (object) model.WorkItemType);
      data.Add("initialValues", (object) model.InitialValues);
      data.Add("sourceView", (object) model.SourceView);
      var htmlAttributes = new{ @class = "options" };
      return htmlHelper1.JsonIsland((object) data, (object) htmlAttributes);
    }

    public static MvcHtmlString QueryResultGrid(
      this HtmlHelper htmlHelper,
      QueryResultModel queryModel)
    {
      return WorkItemExtensions.QueryResultGrid(htmlHelper, queryModel, (IDictionary<string, object>) null, WorkItemExtensions._DEFAULT_GRID_CLASS);
    }

    public static MvcHtmlString QueryResultGrid(
      this HtmlHelper htmlHelper,
      QueryResultModel queryModel,
      object htmlAttributes)
    {
      return WorkItemExtensions.QueryResultGrid(htmlHelper, queryModel, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes), WorkItemExtensions._DEFAULT_GRID_CLASS);
    }

    public static MvcHtmlString QueryResultGrid(
      this HtmlHelper htmlHelper,
      QueryResultModel queryModel,
      IDictionary<string, object> htmlAttributes)
    {
      return WorkItemExtensions.QueryResultGrid(htmlHelper, queryModel, htmlAttributes, WorkItemExtensions._DEFAULT_GRID_CLASS);
    }

    public static MvcHtmlString QueryResultGrid(
      this HtmlHelper htmlHelper,
      QueryResultModel queryModel,
      object htmlAttributes,
      string gridClass)
    {
      return WorkItemExtensions.QueryResultGrid(htmlHelper, queryModel, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes), gridClass);
    }

    public static MvcHtmlString QueryResultGrid(
      this HtmlHelper htmlHelper,
      QueryResultModel queryModel,
      IDictionary<string, object> htmlAttributes,
      string gridClass)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.MergeAttributes<string, object>(htmlAttributes, true);
      tagBuilder.AddCssClass(gridClass);
      StringBuilder stringBuilder = new StringBuilder();
      if (queryModel != null)
      {
        if (queryModel.IncludeContextInfo)
          stringBuilder.AppendLine(htmlHelper.TfsWebContext().ToHtmlString());
        stringBuilder.AppendLine(htmlHelper.DataContractJsonIsland<QueryResultModel>(queryModel, (object) new
        {
          @class = "options"
        }).ToHtmlString());
      }
      tagBuilder.InnerHtml = stringBuilder.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static MvcHtmlString WorkItemFinderOptions(
      this HtmlHelper htmlHelper,
      WorkItemFinderModel model)
    {
      HtmlHelper htmlHelper1 = htmlHelper;
      JsObject data = new JsObject();
      JsObject jsObject = new JsObject();
      jsObject.Add("contextMenu", (object) model.ShowContextMenu);
      data.Add("gutter", (object) jsObject);
      data.Add("showContextMenu", (object) model.ShowContextMenu);
      data.Add("allowMultiSelect", (object) model.AllowMultipleSelection);
      data.Add("initialSelection", (object) false);
      var htmlAttributes = new{ @class = "options" };
      return htmlHelper1.JsonIsland((object) data, (object) htmlAttributes);
    }

    public static MvcHtmlString TeamSettingsData(
      this HtmlHelper htmlHelper,
      TeamWITSettingsModel teamSettings)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      ArgumentUtility.CheckForNull<TeamWITSettingsModel>(teamSettings, nameof (teamSettings));
      return htmlHelper.DataContractJsonIsland<TeamWITSettingsModel>(teamSettings, (object) new
      {
        @class = "team-settings-data"
      }, true);
    }

    public static MvcHtmlString WorkItemRequestMetadata(this HtmlHelper htmlHelper) => htmlHelper.ViewData["UnfollowResult"] != null ? htmlHelper.JsonIsland(htmlHelper.ViewData["UnfollowResult"], (object) new
    {
      @class = "work-item-unfollow-data"
    }) : MvcHtmlString.Empty;

    public static MvcHtmlString WorkItemTypesETag(this HtmlHelper htmlHelper)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      if (tfsWebContext.Project == null)
        return MvcHtmlString.Empty;
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "WorkItemExtensions.WorkItemTypesETag"))
      {
        try
        {
          return htmlHelper.DataContractJsonIsland<string>(EtagHelper.GetWorkItemTypesETag(tfsWebContext.TfsRequestContext, tfsWebContext.Project.Id), (object) new
          {
            @class = "workitemtypes-etag"
          });
        }
        catch (Exception ex)
        {
          tfsWebContext.TfsRequestContext.TraceException(290005, "WebAccess", TfsTraceLayers.Controller, ex);
        }
        return MvcHtmlString.Empty;
      }
    }
  }
}
