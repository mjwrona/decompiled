// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApis.ReadConsistencyLevelHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.WebApis
{
  internal class ReadConsistencyLevelHandler : DelegatingHandler
  {
    private VssReadConsistencyLevel? m_readConsistencyLevel;
    private string m_readConsistencyLevelString;

    public ReadConsistencyLevelHandler(VssReadConsistencyLevel? readConsistencyLevel)
    {
      this.m_readConsistencyLevel = readConsistencyLevel;
      ref VssReadConsistencyLevel? local = ref this.m_readConsistencyLevel;
      this.m_readConsistencyLevelString = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (this.m_readConsistencyLevel.HasValue)
        request.Headers.Add("X-VSS-ReadConsistencyLevel", this.m_readConsistencyLevelString);
      return base.SendAsync(request, cancellationToken);
    }
  }
}
