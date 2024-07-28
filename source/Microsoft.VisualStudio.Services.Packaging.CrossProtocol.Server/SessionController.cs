// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.SessionController
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientGroupByResource("Provenance")]
  [VersionedApiControllerCustomName(Area = "Provenance", ResourceName = "Session")]
  [TaskYieldOnException]
  public class SessionController : TfsProjectApiController
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions { get; } = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (VssPropertyValidationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ProvenanceSessionAlreadyExistsException),
        HttpStatusCode.Conflict
      }
    };

    [HttpPost]
    [ClientSwaggerOperationId("CreateSession")]
    [ClientResponseType(typeof (SessionResponse), null, null)]
    public async Task<HttpResponseMessage> CreateSession(
      string protocol,
      [FromBody] SessionRequest sessionRequest)
    {
      SessionController sessionController = this;
      if (sessionRequest == null || string.IsNullOrWhiteSpace(sessionRequest.Feed))
        throw new VssPropertyValidationException("Feed", Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ArgumentRequired((object) "Feed"));
      return await new SessionCreationHandlerBootstrapper(sessionController.TfsRequestContext).Bootstrap().ThenDelegateTo<RawSessionRequest, SessionResponse, HttpResponseMessage>((IAsyncHandler<SessionResponse, HttpResponseMessage>) new JsonSerializingHttpResponseHandler<SessionResponse>()).Handle(new RawSessionRequest(protocol, sessionController.ProjectId, sessionRequest));
    }
  }
}
