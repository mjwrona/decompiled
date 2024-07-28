// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsPublicProjectRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GvfsPublicProjectRequestRestrictionsAttribute : 
    GitPublicProjectRequestRestrictionsAttribute
  {
    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      string httpMethod = requestContext.WebRequestContextInternal().HttpContext.Request.HttpMethod;
      return httpMethod.Equals(HttpMethod.Get.Method) || httpMethod.Equals(HttpMethod.Post.Method) ? base.Allow(requestContext, routeValues) : new AllowPublicAccessResult(false, false, Guid.Empty);
    }
  }
}
