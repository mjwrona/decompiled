// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Staging.StageShardInvalidController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.Staging
{
  [ValidateRequestFromServicePrincipal]
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Analytics", ResourceName = "Invalid")]
  public class StageShardInvalidController : TfsApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Go)]
    public HttpResponseMessage InvalidateShard(
      string table,
      int providerShard,
      StageShardInvalid shard)
    {
      this.TfsRequestContext.GetService<IStageService>().InvalidateProviderShard(this.TfsRequestContext, table, providerShard, shard.InvalidFields, shard.DisableCurrentStream, shard.KeysOnly);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
