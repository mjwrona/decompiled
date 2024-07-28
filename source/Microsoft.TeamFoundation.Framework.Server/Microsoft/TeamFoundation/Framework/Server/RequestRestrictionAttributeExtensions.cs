// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestRestrictionAttributeExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RequestRestrictionAttributeExtensions
  {
    public static RequestRestrictionsAttribute ApplyRequestRestrictions(
      this IEnumerable<RequestRestrictionsAttribute> allRestrictions,
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      List<RequestRestrictionsAttribute> list = allRestrictions.Where<RequestRestrictionsAttribute>((Func<RequestRestrictionsAttribute, bool>) (_ => _.MatchUserAgent(requestContext.UserAgent))).ToList<RequestRestrictionsAttribute>();
      if (list.Count > 2)
        requestContext.Trace(134217727, TraceLevel.Error, nameof (RequestRestrictionAttributeExtensions), nameof (RequestRestrictionAttributeExtensions), string.Format("Found {0} restrictions: {1}", (object) list.Count, (object) string.Join(",", list.Select<RequestRestrictionsAttribute, string>((Func<RequestRestrictionsAttribute, string>) (_ => _.ToString())).ToArray<string>())));
      RequestRestrictionsAttribute restrictionsAttribute = list.Where<RequestRestrictionsAttribute>((Func<RequestRestrictionsAttribute, bool>) (_ => _.AgentFilter != null)).FirstOrDefault<RequestRestrictionsAttribute>() ?? list.FirstOrDefault<RequestRestrictionsAttribute>();
      restrictionsAttribute?.ApplyRequestRestrictions(requestContext, routeValues);
      return restrictionsAttribute;
    }
  }
}
