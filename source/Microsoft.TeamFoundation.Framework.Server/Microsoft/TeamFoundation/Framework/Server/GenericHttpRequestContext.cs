// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GenericHttpRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class GenericHttpRequestContext : WebRequestContext
  {
    public GenericHttpRequestContext(
      VssServiceHost serviceHost,
      RequestContextType requestContextType,
      HttpContextBase httpContext,
      LockHelper helper,
      TimeSpan timeout)
      : base((IVssServiceHost) serviceHost, requestContextType, httpContext, helper, timeout)
    {
      this.IsTracked = true;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GenericHttpRequestContext[{0} {1}, RemoteComputer: {2}, RemoteIPAddress: {3}, RemotePort: {4}, UserAgent: {5}]", (object) this.HttpMethod, (object) this.RequestUri, (object) this.RemoteComputer, (object) this.RemoteIPAddress, (object) this.RemotePort, (object) this.UserAgent);
  }
}
