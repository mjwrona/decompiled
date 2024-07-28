// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Staging.StageController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.Staging
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Analytics", ResourceName = "Stage")]
  [ValidateRequestFromServicePrincipal]
  public class StageController : TfsApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<StageStreamDisabledException>(HttpStatusCode.Gone);
      exceptionMap.AddStatusCode<StageTableInMaintenanceException>(HttpStatusCode.ServiceUnavailable);
      exceptionMap.AddStatusCode<StageStreamNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<StageStreamThrottledException>((HttpStatusCode) 429);
      exceptionMap.AddStatusCode<InvalidAccessException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<AnalyticsStagingUnavailableException>(HttpStatusCode.ServiceUnavailable);
    }

    [HttpPost]
    [ClientResponseType(typeof (IngestResult), null, null)]
    [ClientRequestBodyIsStream]
    [ClientRequestContentMediaType("application/json")]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Go)]
    public HttpResponseMessage StageRecords(string table, int providerShard, int stream)
    {
      using (Stream result = this.Request.Content.ReadAsStreamAsync().Result)
        return this.CreateResponse<IngestResult>(this.TfsRequestContext.GetService<IStageService>().StageRecords(this.TfsRequestContext, table, providerShard, stream, result), HttpStatusCode.Accepted);
    }

    [HttpGet]
    [ClientResponseType(typeof (StageProviderShardInfo), null, null)]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Go)]
    public HttpResponseMessage GetShard(string table, int providerShard) => this.CreateResponse<StageProviderShardInfo>(this.TfsRequestContext.GetService<IStageService>().GetShard(this.TfsRequestContext, table, providerShard));

    [HttpGet]
    [ClientResponseType(typeof (StageTableInfo), null, null)]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Go)]
    public HttpResponseMessage GetTable(string table) => this.CreateResponse<StageTableInfo>(this.TfsRequestContext.GetService<IStageService>().GetTable(this.TfsRequestContext, table));

    [HttpPut]
    [ClientResponseType(typeof (StageProviderShardInfo), null, null)]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Go)]
    public HttpResponseMessage CreateShard(string table, int providerShard) => this.CreateResponse<StageProviderShardInfo>(this.TfsRequestContext.GetService<IStageService>().CreateShard(this.TfsRequestContext, table, providerShard), HttpStatusCode.Created);

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Go)]
    public HttpResponseMessage DeleteShard(string table, int providerShard)
    {
      this.TfsRequestContext.GetService<IStageService>().DeleteShard(this.TfsRequestContext, table, providerShard);
      return this.CreateResponse(HttpStatusCode.Accepted);
    }
  }
}
