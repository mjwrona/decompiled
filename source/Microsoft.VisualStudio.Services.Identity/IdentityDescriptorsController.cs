// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorsController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "descriptors")]
  public class IdentityDescriptorsController : IdentitiesControllerBase
  {
    public const string TraceLayer = "IdentityDescriptorsController";
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (IdentityDescriptorNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidGetDescriptorRequestException),
        HttpStatusCode.BadRequest
      }
    };

    [HttpGet]
    [TraceFilter(6307330, 6307331)]
    public IdentityDescriptor GetDescriptorById(Guid id, bool isMasterId = false)
    {
      IIdentityIdentifierConversionService service = this.TfsRequestContext.GetService<IIdentityIdentifierConversionService>();
      IdentityDescriptor identityDescriptor = !isMasterId ? service.GetDescriptorByLocalId(this.TfsRequestContext, id) : service.GetDescriptorByMasterId(this.TfsRequestContext, id);
      return !(identityDescriptor == (IdentityDescriptor) null) ? identityDescriptor : throw new IdentityDescriptorNotFoundException(id, isMasterId);
    }

    public override string TraceArea => "Identities";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentityDescriptorsController.s_httpExceptions;

    public override string ActivityLogArea => "Identities";
  }
}
