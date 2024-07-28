// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkIdentitiesController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Identity")]
  [ClientInclude(~RestClientLanguages.Swagger2)]
  public class FrameworkIdentitiesController : TfsApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpIdentityServiceExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      }
    };

    [HttpPut]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Identity.Identity), null, null)]
    public HttpResponseMessage CreateIdentity(FrameworkIdentityInfo frameworkIdentityInfo)
    {
      ArgumentUtility.CheckForNull<FrameworkIdentityInfo>(frameworkIdentityInfo, nameof (frameworkIdentityInfo));
      return this.Request.CreateResponse<Microsoft.VisualStudio.Services.Identity.Identity>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IdentityService>().CreateFrameworkIdentity(this.TfsRequestContext, frameworkIdentityInfo.IdentityType, frameworkIdentityInfo.Role, frameworkIdentityInfo.Identifier, frameworkIdentityInfo.DisplayName));
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) FrameworkIdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
