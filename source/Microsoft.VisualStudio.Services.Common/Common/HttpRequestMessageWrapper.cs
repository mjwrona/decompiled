// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.HttpRequestMessageWrapper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Common
{
  internal struct HttpRequestMessageWrapper : IHttpRequest, IHttpHeaders
  {
    private readonly HttpRequestMessage m_request;

    public HttpRequestMessageWrapper(HttpRequestMessage request) => this.m_request = request;

    public IHttpHeaders Headers => (IHttpHeaders) this;

    public Uri RequestUri => this.m_request.RequestUri;

    public IDictionary<string, object> Properties => this.m_request.Properties;

    IEnumerable<string> IHttpHeaders.GetValues(string name)
    {
      IEnumerable<string> values;
      if (!this.m_request.Headers.TryGetValues(name, out values))
        values = Enumerable.Empty<string>();
      return values;
    }

    void IHttpHeaders.SetValue(string name, string value)
    {
      this.m_request.Headers.Remove(name);
      this.m_request.Headers.Add(name, value);
    }

    bool IHttpHeaders.TryGetValues(string name, out IEnumerable<string> values) => this.m_request.Headers.TryGetValues(name, out values);
  }
}
