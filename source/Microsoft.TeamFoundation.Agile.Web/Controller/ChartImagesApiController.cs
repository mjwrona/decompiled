// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.ChartImagesApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.Charts;
using Microsoft.Azure.Boards.Charts.Exceptions;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "chartimages")]
  [ControllerApiVersion(5.1)]
  public class ChartImagesApiController : BoardsApiControllerBase
  {
    private const int DEFAULT_CHART_WIDTH = 140;
    private const int DEFAULT_CHART_HEIGHT = 40;
    private const int DEFAULT_ITERATIONS_NUMBER = 5;
    private const string c_imageContentType = "image/png";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ChartDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<IterationNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<NoIterationsSetForTeamException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidChartDimensionsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BurndownWorkItemLimitExceededException>(HttpStatusCode.RequestEntityTooLarge);
    }

    [HttpGet]
    [ClientResponseType(typeof (StreamContent), null, null)]
    [ClientLocationId("4ee4d042-64fa-4202-8ca6-dae1ab888985")]
    public HttpResponseMessage GetBoardChartImage(
      string board,
      string name,
      [FromUri] int width = 140,
      [FromUri] int height = 40,
      [FromUri] bool showDetails = false,
      [FromUri] string title = null)
    {
      string levelIdByNameOrId = this.GetBoardBacklogLevelIdByNameOrId(this.TfsRequestContext.GetService<BoardService>(), board);
      Stream boardChartImage;
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "ChartImagesApiController.CumulativeFlow.GenerateBoardChartImage"))
        boardChartImage = this.TfsRequestContext.GetService<IBoardsChartService>().GenerateBoardChartImage(this.TfsRequestContext, name, this.AgileSettings, this.ProjectId, this.Team, levelIdByNameOrId, width, height, showDetails, title);
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(boardChartImage);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
      return response;
    }

    [HttpGet]
    [ClientResponseType(typeof (StreamContent), null, null)]
    [ClientLocationId("89436dcf-a56b-4f72-a42e-2afef39c88a5")]
    public HttpResponseMessage GetIterationsChartImage(
      string name,
      [FromUri] int iterationsNumber = 5,
      [FromUri] int width = 140,
      [FromUri] int height = 40,
      [FromUri] bool showDetails = false,
      [FromUri] string title = null)
    {
      Stream iterationsChartImage = this.TfsRequestContext.GetService<IBoardsChartService>().GenerateIterationsChartImage(this.TfsRequestContext, name, this.AgileSettings, this.ProjectId, iterationsNumber, width, height, showDetails, title);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(iterationsChartImage);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
      return response;
    }

    [HttpGet]
    [ClientResponseType(typeof (StreamContent), null, null)]
    [ClientLocationId("8b94efc1-e022-469d-80aa-8d2ba1c21449")]
    public HttpResponseMessage GetIterationChartImage(
      Guid iterationId,
      string name,
      [FromUri] int width = 140,
      [FromUri] int height = 40,
      [FromUri] bool showDetails = false,
      [FromUri] string title = null)
    {
      Stream iterationChartImage = this.TfsRequestContext.GetService<IBoardsChartService>().GenerateIterationChartImage(this.TfsRequestContext, name, this.AgileSettings, this.ProjectId, iterationId, this.Team, width, height, showDetails, title);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(iterationChartImage);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
      return response;
    }
  }
}
