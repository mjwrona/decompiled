// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApis.GuidRouteConstraint
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server.WebApis
{
  public class GuidRouteConstraint : IHttpRouteConstraint
  {
    public bool Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      object obj;
      if (!values.TryGetValue(parameterName, out obj) || obj == null)
        return false;
      return obj is Guid || Guid.TryParse(Convert.ToString(obj, (IFormatProvider) CultureInfo.InvariantCulture), out Guid _);
    }
  }
}
