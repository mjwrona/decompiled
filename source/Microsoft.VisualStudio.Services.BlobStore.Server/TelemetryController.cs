// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.TelemetryController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelineartifactstelemetry", ResourceName = "aiinstrumentationkey", ResourceVersion = 1)]
  [SetActivityLogAnonymousIdentifier]
  public sealed class TelemetryController : BlobControllerBase
  {
    private const string AIInstrumentationKey = "de94fe98-207d-447b-b9a0-9502d93c6e11";

    [HttpGet]
    [ControllerMethodTraceFilter(5790002)]
    public ApplicationInsightsInfo GetAppInsightsKey() => new ApplicationInsightsInfo("de94fe98-207d-447b-b9a0-9502d93c6e11");
  }
}
