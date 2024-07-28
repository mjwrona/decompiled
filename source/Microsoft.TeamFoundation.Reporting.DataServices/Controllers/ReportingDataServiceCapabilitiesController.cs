// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Controllers.ReportingDataServiceCapabilitiesController
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Controllers
{
  [ApplyRequestLanguage]
  public class ReportingDataServiceCapabilitiesController : ChartingCollectionControllerBase
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_HttpExceptions = new Dictionary<Type, HttpStatusCode>((IDictionary<Type, HttpStatusCode>) ChartingCollectionControllerBase.s_CommonHttpExceptions)
    {
      {
        typeof (ChartScopeProviderNotFoundException),
        HttpStatusCode.NotFound
      }
    };

    public override string TraceArea => "DataServiceCapabilities";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ReportingDataServiceCapabilitiesController.s_HttpExceptions;

    [TraceFilter(1017921, 1017922)]
    [HttpGet]
    public DataServiceCapabilitiesResponse GetDataServiceCapabilities(string scope) => this.CreateResponse(this.TfsRequestContext.GetService<IDataServiceCapabilitiesService>().GetDataServiceCapabilities(this.TfsRequestContext, scope));

    private DataServiceCapabilitiesResponse CreateResponse(DataServiceCapabilities capabilities) => new DataServiceCapabilitiesResponse(capabilities, this.Url.RestLink(this.TfsRequestContext, ReportingLocationIds.DataServiceCapabilities, (object) new
    {
      scope = capabilities.Scope
    }));
  }
}
