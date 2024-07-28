// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MalformedRequestCleanupAuthenticationModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MalformedRequestCleanupAuthenticationModule : VssfAuthenticationHttpModuleBase
  {
    private const string PowerBIUserAgent = "Microsoft.Data.Mashup";
    private const string AuthorizationHeader = "Authorization";

    public override void OnAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      if (httpContext == null || httpContext.Request?.UserAgent == null || httpContext?.Request?.Headers == null || !httpContext.Request.UserAgent.StartsWith("Microsoft.Data.Mashup", StringComparison.OrdinalIgnoreCase))
        return;
      string[] values = httpContext.Request.Headers.GetValues("Authorization");
      if (values == null || values.Length == 0)
        return;
      List<string> list = ((IEnumerable<string>) values).ToList<string>();
      if (list.RemoveAll((Predicate<string>) (value => value.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(value.Substring("Bearer".Length)))) <= 0)
        return;
      if (list.Count == 0)
        httpContext.Request.Headers.Remove("Authorization");
      else
        httpContext.Request.Headers.Set("Authorization", string.Join(",", (IEnumerable<string>) list));
    }
  }
}
