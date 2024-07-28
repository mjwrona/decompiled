// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.VssfHttpClientFactory
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class VssfHttpClientFactory : IVssfHttpClientFactory
  {
    public HttpClient CreateHttpClient(int maxRetries, int timeout) => new HttpClient((HttpMessageHandler) new VssHttpRetryMessageHandler(new VssHttpRetryOptions()
    {
      MaxRetries = maxRetries
    }, (HttpMessageHandler) new HttpClientHandler()))
    {
      Timeout = TimeSpan.FromSeconds((double) timeout)
    };
  }
}
