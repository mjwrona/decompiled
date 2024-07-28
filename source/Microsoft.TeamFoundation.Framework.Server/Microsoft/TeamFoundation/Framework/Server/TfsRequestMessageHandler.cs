// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsRequestMessageHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TfsRequestMessageHandler : DelegatingHandler
  {
    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (!request.Properties.ContainsKey(TfsApiPropertyKeys.TfsRequestContext))
        request.Properties.Add(TfsApiPropertyKeys.TfsRequestContext, (object) request.GetIVssRequestContext());
      string methodOverrideHeader = request.GetHttpMethodFromMethodOverrideHeader();
      if (methodOverrideHeader != null)
        request.Method = new HttpMethod(methodOverrideHeader);
      return base.SendAsync(request, cancellationToken);
    }

    public string GetHeader(HttpHeaders headers, string key)
    {
      IEnumerable<string> values;
      return !headers.TryGetValues(key, out values) ? (string) null : values.First<string>();
    }
  }
}
