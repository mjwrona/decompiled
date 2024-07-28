// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Controllers.DeploymentProjectApiController
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Net;

namespace Microsoft.Azure.Pipelines.Deployment.Controllers
{
  public abstract class DeploymentProjectApiController : TfsProjectApiController
  {
    public override string TraceArea => "Deployment";

    public override string ActivityLogArea => "Deployment";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<NoteNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<NoteExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<OccurrenceTagExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<OccurrenceNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
