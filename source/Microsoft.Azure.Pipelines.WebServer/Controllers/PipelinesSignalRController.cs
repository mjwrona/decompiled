// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.PipelinesSignalRController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server;
using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Azure.Pipelines.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "signalr")]
  [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.TypeScript)]
  public class PipelinesSignalRController : PipelinesProjectApiController
  {
    [HttpGet]
    public SignalRConnection GetSignedSignalRUrl(int pipelineId, int runId)
    {
      ArgumentUtility.CheckForNonPositiveInt(pipelineId, nameof (pipelineId));
      ArgumentUtility.CheckForNonPositiveInt(runId, nameof (runId));
      if (this.TfsRequestContext.GetService<IExternalRunsService>().GetRun(this.TfsRequestContext, this.ProjectId, pipelineId, runId) == null)
        throw new RunNotFoundException(PipelinesServerResources.RunNotFound((object) runId)).Expected("Pipelines");
      return this.GetClientConverter<SignalRConnectionConverter>().ToWebApiUrl(this.TfsRequestContext, this.ProjectId, pipelineId, runId);
    }
  }
}
