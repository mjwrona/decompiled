// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Controllers.ApiTeamChartController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Charts;
using Microsoft.Azure.Boards.Charts.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Serializers;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Utils.Performance;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiTeamChartController : AgileAreaController
  {
    private const string c_imageContentType = "image/png";

    [HttpGet]
    [TfsTraceFilter(220201, 220209)]
    [SamplePerformanceData]
    public ActionResult BurnDown(string iterationPath, [ModelBinder(typeof (CustomJsonModelBinder<ChartOptionInvalidJsonException>))] ChartOptions chartOptions)
    {
      if (string.IsNullOrEmpty(iterationPath))
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, AgileControlsServerResources.IterationPathIsNull);
      try
      {
        return (ActionResult) this.File(this.TfsRequestContext.GetService<BoardsChartService>().GenerateIterationChartImage(this.TfsRequestContext, "burndown", this.Settings, this.TfsWebContext.Project.Id, this.GetIterationIdForPath(iterationPath), this.Team, chartOptions.Width, chartOptions.Height, chartOptions.ShowDetails, chartOptions.Title), "image/png");
      }
      catch (InvalidChartDimensionsException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, ex.Message);
      }
      catch (BurndownWorkItemLimitExceededException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.RequestEntityTooLarge, ex.Message);
      }
      catch (NoIterationsSetForTeamException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, ex.Message);
      }
      catch (IterationPathNotFoundException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.NotFound, ex.Message);
      }
    }

    [HttpGet]
    [TfsTraceFilter(220211, 220219)]
    [SamplePerformanceData]
    public ActionResult Velocity(int? iterationsNumber, [ModelBinder(typeof (CustomJsonModelBinder<ChartOptionInvalidJsonException>))] ChartOptions chartOptions)
    {
      if (!iterationsNumber.HasValue)
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, AgileControlsServerResources.InvalidVelocityChartIterations);
      try
      {
        return (ActionResult) this.File(this.TfsRequestContext.GetService<BoardsChartService>().GenerateIterationsChartImage(this.TfsRequestContext, "velocity", this.Settings, this.TfsWebContext.Project.Id, iterationsNumber.Value, chartOptions.Width, chartOptions.Height, chartOptions.ShowDetails, chartOptions.Title), "image/png");
      }
      catch (InvalidChartDimensionsException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, ex.Message);
      }
      catch (NoIterationsSetForTeamException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, ex.Message);
      }
    }

    [HttpGet]
    [TfsTraceFilter(220241, 220249)]
    [SamplePerformanceData]
    public ActionResult CumulativeFlow([ModelBinder(typeof (CustomJsonModelBinder<ChartOptionInvalidJsonException>))] ChartOptions chartOptions, string hubCategoryRefName)
    {
      Stream boardChartImage;
      using (WebPerformanceTimerHelpers.StartMeasure((WebContext) this.TfsWebContext, "ApiTeamChartController.CumulativeFlow.GenerateCumulativeFlowDiagram"))
      {
        try
        {
          boardChartImage = this.TfsRequestContext.GetService<BoardsChartService>().GenerateBoardChartImage(this.TfsRequestContext, "cumulativeFlow", this.Settings, this.TfsWebContext.Project.Id, this.Team, hubCategoryRefName, chartOptions.Width, chartOptions.Height, chartOptions.ShowDetails, chartOptions.Title);
        }
        catch (InvalidChartDimensionsException ex)
        {
          return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, ex.Message);
        }
      }
      WebPerformanceTimerHelpers.SendCustomerIntelligenceData((WebContext) this.TfsWebContext);
      return (ActionResult) this.File(boardChartImage, "image/png");
    }

    private Guid GetIterationIdForPath(string iterationPath) => (this.GetIterationNodeForPath(iterationPath) ?? throw new IterationPathNotFoundException(iterationPath)).CssNodeId;
  }
}
