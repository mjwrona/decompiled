// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsPublicProjectRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitLfsPublicProjectRequestRestrictionsAttribute : 
    GitPublicProjectRequestRestrictionsAttribute
  {
    public GitLfsPublicProjectRequestRestrictionsAttribute()
    {
    }

    public GitLfsPublicProjectRequestRestrictionsAttribute(
      AuthenticationMechanisms mechanisms,
      UserAgentFilterType agentFilterType,
      string agentFilter)
      : base(mechanisms, agentFilterType, agentFilter)
    {
    }

    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      string httpMethod = requestContext.WebRequestContextInternal().HttpContext.Request.HttpMethod;
      string str = (string) null;
      object obj = (object) null;
      if (requestContext.Items.TryGetValue("lfsOperation", out obj))
        str = (string) obj;
      string method = HttpMethod.Get.Method;
      return httpMethod.Equals(method) || str == "download" ? base.Allow(requestContext, routeValues) : new AllowPublicAccessResult(false, false, Guid.Empty);
    }
  }
}
