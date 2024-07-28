// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.OperationsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Operations
{
  [VersionedApiControllerCustomName("operations", "operations", 1)]
  public class OperationsController : TfsApiController
  {
    public override string TraceArea => "OperationsApi";

    public override string ActivityLogArea => "Framework";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<OperationUpdateFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<OperationNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<OperationPluginNotFoundException>(HttpStatusCode.NotFound);
    }

    [HttpGet]
    [ClientLocationId("9A1B74B4-2CA8-4A9F-8470-C2F2E6FDC949")]
    public Operation GetOperation(Guid operationId, Guid? pluginId = null) => this.TfsRequestContext.GetService<IOperationsService>().GetOperation(this.TfsRequestContext, pluginId ?? Guid.Empty, operationId);
  }
}
