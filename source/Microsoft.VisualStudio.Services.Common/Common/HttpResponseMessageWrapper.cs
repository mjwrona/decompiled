// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.HttpResponseMessageWrapper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Common
{
  internal struct HttpResponseMessageWrapper : IHttpResponse, IHttpHeaders
  {
    private readonly HttpResponseMessage m_response;

    public HttpResponseMessageWrapper(HttpResponseMessage response) => this.m_response = response;

    public IHttpHeaders Headers => (IHttpHeaders) this;

    public HttpStatusCode StatusCode => this.m_response.StatusCode;

    IEnumerable<string> IHttpHeaders.GetValues(string name)
    {
      IEnumerable<string> values;
      if (!this.m_response.Headers.TryGetValues(name, out values))
        values = Enumerable.Empty<string>();
      return values;
    }

    void IHttpHeaders.SetValue(string name, string value) => throw new NotSupportedException();

    bool IHttpHeaders.TryGetValues(string name, out IEnumerable<string> values) => this.m_response.Headers.TryGetValues(name, out values);
  }
}
