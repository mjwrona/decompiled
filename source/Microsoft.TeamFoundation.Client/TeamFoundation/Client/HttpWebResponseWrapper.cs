// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.HttpWebResponseWrapper
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
  internal struct HttpWebResponseWrapper : IHttpResponse, IHttpHeaders
  {
    private readonly HttpWebResponse m_response;

    public HttpWebResponseWrapper(HttpWebResponse response) => this.m_response = response;

    public IHttpHeaders Headers => (IHttpHeaders) this;

    public HttpStatusCode StatusCode => this.m_response.StatusCode;

    IEnumerable<string> IHttpHeaders.GetValues(string name) => (IEnumerable<string>) this.m_response.Headers.GetValues(name) ?? Enumerable.Empty<string>();

    void IHttpHeaders.SetValue(string name, string value) => throw new NotSupportedException();

    bool IHttpHeaders.TryGetValues(string name, out IEnumerable<string> values)
    {
      values = (IEnumerable<string>) this.m_response.Headers.GetValues(name);
      return values != null;
    }
  }
}
