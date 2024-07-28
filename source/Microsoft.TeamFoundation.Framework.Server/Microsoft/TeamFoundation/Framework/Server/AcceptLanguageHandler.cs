// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AcceptLanguageHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AcceptLanguageHandler : DelegatingHandler
  {
    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      for (CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture; !string.IsNullOrEmpty(cultureInfo?.Name); cultureInfo = cultureInfo.Parent)
      {
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo.Name));
        if (cultureInfo.Parent == cultureInfo)
          break;
      }
      return base.SendAsync(request, cancellationToken);
    }
  }
}
