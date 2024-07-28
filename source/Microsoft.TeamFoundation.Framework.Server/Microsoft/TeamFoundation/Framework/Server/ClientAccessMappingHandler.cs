// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientAccessMappingHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location.Server;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ClientAccessMappingHandler : DelegatingHandler
  {
    private readonly string m_clientAccessMappingMonikers;

    internal ClientAccessMappingHandler(IVssRequestContext requestContext)
    {
      string str;
      if (requestContext.RootContext.Items.TryGetValue<string>(RequestContextItemsKeys.ClientAccessMappingMonikers, out str))
        this.m_clientAccessMappingMonikers = str;
      else
        this.m_clientAccessMappingMonikers = requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext)?.Moniker;
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (!string.IsNullOrEmpty(this.m_clientAccessMappingMonikers))
        request.Headers.Add("X-VSS-ClientAccessMapping", this.m_clientAccessMappingMonikers);
      return base.SendAsync(request, cancellationToken);
    }
  }
}
