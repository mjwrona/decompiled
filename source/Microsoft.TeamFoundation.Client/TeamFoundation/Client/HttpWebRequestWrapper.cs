// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.HttpWebRequestWrapper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  internal struct HttpWebRequestWrapper : IHttpRequest, IHttpHeaders
  {
    private readonly HttpWebRequest m_request;

    public HttpWebRequestWrapper(HttpWebRequest request) => this.m_request = request;

    public IHttpHeaders Headers => (IHttpHeaders) this;

    public Uri RequestUri => this.m_request.RequestUri;

    public IDictionary<string, object> Properties => throw new NotSupportedException();

    IEnumerable<string> IHttpHeaders.GetValues(string name) => (IEnumerable<string>) this.m_request.Headers.GetValues(name) ?? Enumerable.Empty<string>();

    void IHttpHeaders.SetValue(string name, string value) => this.m_request.Headers[name] = value;

    bool IHttpHeaders.TryGetValues(string name, out IEnumerable<string> values)
    {
      values = (IEnumerable<string>) this.m_request.Headers.GetValues(name);
      return values != null;
    }
  }
}
