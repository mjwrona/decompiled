// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySequenceIdController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "MaxSequenceId")]
  [ClientInclude(~RestClientLanguages.Swagger2)]
  public class IdentitySequenceIdController : TfsApiController
  {
    private const string s_area = "IdentitySequenceId";
    private const string s_layer = "IdentitySequenceIdController";
    private static readonly Dictionary<Type, HttpStatusCode> httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      }
    };

    public override string ActivityLogArea => "Identities";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitySequenceIdController.httpExceptions;

    [HttpGet]
    [ClientResponseType(typeof (long), null, null)]
    public HttpResponseMessage GetMaxSequenceId()
    {
      this.TfsRequestContext.TraceEnter(80540, "IdentitySequenceId", nameof (IdentitySequenceIdController), nameof (GetMaxSequenceId));
      try
      {
        return this.Request.CreateResponse<long>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IdentityStatisticsService>().GetMaxSequenceId(this.TfsRequestContext));
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(80549, "IdentitySequenceId", nameof (IdentitySequenceIdController), nameof (GetMaxSequenceId));
      }
    }
  }
}
