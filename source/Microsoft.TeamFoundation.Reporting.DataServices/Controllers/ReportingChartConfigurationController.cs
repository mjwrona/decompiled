// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Controllers.ReportingChartConfigurationController
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Controllers
{
  public class ReportingChartConfigurationController : ChartingProjectControllerBase
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_HttpExceptions = new Dictionary<Type, HttpStatusCode>((IDictionary<Type, HttpStatusCode>) ChartingProjectControllerBase.s_CommonHttpExceptions)
    {
      {
        typeof (ChartConfigurationAlreadyCreatedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (TransformOptionsAlreadyCreatedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (InvalidChartConfigurationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidColorConfigurationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidChartGroupException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TooManyChartsPerGroupException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TooManyColorsPerChartException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TransformOptionsDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ChartConfigurationDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ChartScopeProviderNotFoundException),
        HttpStatusCode.NotFound
      }
    };
    private IChartConfigurationService m_chartConfigurationService;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ReportingChartConfigurationController.s_HttpExceptions;

    public override string TraceArea => "ChartConfiguration";

    public IChartConfigurationService ChartConfigurationService
    {
      get
      {
        if (this.m_chartConfigurationService == null)
          this.m_chartConfigurationService = this.TfsRequestContext.GetService<IChartConfigurationService>();
        return this.m_chartConfigurationService;
      }
    }

    [TraceFilter(1017000, 1017010)]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public IEnumerable<ChartConfigurationResponse> GetChartConfigurations(
      string scope,
      string groupKey)
    {
      return (IEnumerable<ChartConfigurationResponse>) this.ChartConfigurationService.GetChartConfigurationGroup(this.TfsRequestContext, this.ProjectId, scope, groupKey).Select<ChartConfiguration, ChartConfigurationResponse>(new Func<ChartConfiguration, ChartConfigurationResponse>(this.CreateResponse)).ToArray<ChartConfigurationResponse>();
    }

    [TraceFilter(1017041, 1017050)]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public ChartConfigurationResponse GetChartConfiguration(Guid id) => this.CreateResponse(this.ChartConfigurationService.GetChartConfiguration(this.TfsRequestContext, this.ProjectId, id));

    [TraceFilter(1017011, 1017020)]
    [HttpPost]
    public ChartConfigurationResponse CreateChartConfiguration(ChartConfiguration chartConfiguration)
    {
      if (chartConfiguration == null)
        throw new InvalidChartConfigurationException(ReportingResources.NoValidChartConfigurationDetected());
      if (chartConfiguration.ChartId.HasValue)
      {
        Guid? chartId = chartConfiguration.ChartId;
        Guid empty = Guid.Empty;
        if ((chartId.HasValue ? (chartId.HasValue ? (chartId.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          throw new ChartConfigurationAlreadyCreatedException(chartConfiguration.ChartId.Value);
      }
      return this.CreateResponse(this.ChartConfigurationService.SaveChartConfiguration(this.TfsRequestContext, this.ProjectId, chartConfiguration));
    }

    [TraceFilter(1017021, 1017030)]
    [HttpPut]
    public ChartConfigurationResponse ReplaceChartConfiguration(
      ChartConfiguration chartConfiguration)
    {
      if (chartConfiguration == null)
        throw new InvalidChartConfigurationException(ReportingResources.NoValidChartConfigurationDetected());
      if (chartConfiguration.ChartId.HasValue)
      {
        Guid? chartId = chartConfiguration.ChartId;
        Guid empty = Guid.Empty;
        if ((chartId.HasValue ? (chartId.HasValue ? (chartId.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
          return this.CreateResponse(this.ChartConfigurationService.SaveChartConfiguration(this.TfsRequestContext, this.ProjectId, chartConfiguration));
      }
      throw new ChartConfigurationDoesNotExistException(Guid.Empty);
    }

    [TraceFilter(1017031, 1017040)]
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteChartConfiguration(Guid id)
    {
      this.ChartConfigurationService.DeleteChartConfiguration(this.TfsRequestContext, this.ProjectId, id);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    private ChartConfigurationResponse CreateResponse(ChartConfiguration addedConfig) => new ChartConfigurationResponse(addedConfig, this.Url.RestLink(this.TfsRequestContext, ReportingLocationIds.ChartConfiguration, (object) new
    {
      id = addedConfig.ChartId.ToString()
    }));
  }
}
