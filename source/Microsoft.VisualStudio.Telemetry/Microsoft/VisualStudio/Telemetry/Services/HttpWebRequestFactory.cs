// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.HttpWebRequestFactory
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  [ExcludeFromCodeCoverage]
  internal class HttpWebRequestFactory : IHttpWebRequestFactory
  {
    public IHttpWebRequest Create(string url) => (IHttpWebRequest) new HttpWebRequest(url);

    public IHttpWebRequest Create(
      string url,
      IEnumerable<KeyValuePair<string, string>> queryParameters)
    {
      if (queryParameters == null)
        return this.Create(url);
      UriBuilder uriBuilder = new UriBuilder(url);
      string str = string.Join("&", queryParameters.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "=" + Uri.EscapeDataString(x.Value))));
      uriBuilder.Query = uriBuilder.Query == null || uriBuilder.Query.Length <= 1 ? str : uriBuilder.Query.Substring(1) + "&" + str;
      return (IHttpWebRequest) new HttpWebRequest(uriBuilder.ToString());
    }
  }
}
