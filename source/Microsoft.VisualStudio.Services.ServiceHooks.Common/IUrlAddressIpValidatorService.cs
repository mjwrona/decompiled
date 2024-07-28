// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.IUrlAddressIpValidatorService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  [DefaultServiceImplementation(typeof (UrlAddressIpValidatorService))]
  public interface IUrlAddressIpValidatorService : IVssFrameworkService
  {
    void VerifyUrlIsAllowedIPAddress(
      IVssRequestContext requestContext,
      string url,
      bool throwIfInvalidHost = true);

    void ApplyIPAddressAllowedRangeOnHttpRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage message);
  }
}
