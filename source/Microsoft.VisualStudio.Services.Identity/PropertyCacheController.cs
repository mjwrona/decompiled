// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PropertyCacheController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Cache", ResourceName = "Properties")]
  public class PropertyCacheController : TfsApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpPlatformPropertyCacheServiceExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PropertyCacheServiceNotAvailableException),
        HttpStatusCode.InternalServerError
      }
    };

    [HttpPut]
    public string Cache(object data) => this.TfsRequestContext.GetService<PlatformPropertyCacheService>().Cache<object>(this.TfsRequestContext, data);

    [HttpGet]
    public object Get(string cacheKey) => this.TfsRequestContext.GetService<IPropertyCacheService>().Get<object>(this.TfsRequestContext, cacheKey);

    [HttpDelete]
    public void Delete(string cacheKey) => this.TfsRequestContext.GetService<IPropertyCacheService>().Delete(this.TfsRequestContext, cacheKey);

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PropertyCacheController.s_httpPlatformPropertyCacheServiceExceptions;
  }
}
