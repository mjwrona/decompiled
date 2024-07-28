// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpRequestMessageWrapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HttpRequestMessageWrapper : HttpRequestBase
  {
    private readonly string m_virtualPathRoot;
    private readonly HttpRequestMessage m_httpRequest;
    private NameValueCollection m_headers;
    private NameValueCollection m_serverVariables = new NameValueCollection();

    public HttpRequestMessageWrapper(string virtualPathRoot, HttpRequestMessage httpRequest)
    {
      ArgumentUtility.CheckForNull<string>(virtualPathRoot, nameof (virtualPathRoot));
      ArgumentUtility.CheckForNull<HttpRequestMessage>(httpRequest, nameof (httpRequest));
      this.m_virtualPathRoot = virtualPathRoot;
      this.m_httpRequest = httpRequest;
    }

    public override string ApplicationPath => this.m_virtualPathRoot;

    public override string AppRelativeCurrentExecutionFilePath
    {
      get
      {
        string str = HttpUtility.UrlDecode(this.m_httpRequest.RequestUri.AbsolutePath);
        if (!str.StartsWith(this.m_virtualPathRoot, StringComparison.OrdinalIgnoreCase))
          return (string) null;
        return "~" + (this.m_virtualPathRoot.Length == 1 ? str : str.Substring(this.m_virtualPathRoot.Length)).TrimEnd('/');
      }
    }

    public override string CurrentExecutionFilePath => this.FilePath;

    public override string FilePath
    {
      get
      {
        string str = HttpUtility.UrlDecode(this.m_httpRequest.RequestUri.AbsolutePath);
        if (!str.StartsWith(this.m_virtualPathRoot, StringComparison.OrdinalIgnoreCase))
          return (string) null;
        return str.TrimEnd('/');
      }
    }

    public override NameValueCollection Headers
    {
      get
      {
        if (this.m_headers == null)
        {
          NameValueCollection collection = new NameValueCollection();
          foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) this.m_httpRequest.Headers)
            collection.AddMultiple(header.Key, header.Value);
          this.m_headers = collection;
        }
        return this.m_headers;
      }
    }

    public override string HttpMethod => this.m_httpRequest.Method.ToString();

    public override bool IsLocal
    {
      get
      {
        Lazy<bool> lazy;
        return this.m_httpRequest.Properties.TryGetValue<Lazy<bool>>("MS_IsLocal", out lazy) && lazy.Value;
      }
    }

    public override string Path => HttpUtility.UrlDecode(this.m_httpRequest.RequestUri.AbsolutePath);

    public override string PathInfo => string.Empty;

    public override NameValueCollection QueryString => this.m_httpRequest.RequestUri.ParseQueryString();

    public override string RawUrl => HttpUtility.UrlDecode(this.m_httpRequest.RequestUri.PathAndQuery);

    public override string RequestType => this.m_httpRequest.Method.ToString();

    public override NameValueCollection ServerVariables => this.m_serverVariables;

    public override Uri Url => this.m_httpRequest.RequestUri;

    public override string UserAgent => this.m_httpRequest.Headers.UserAgent.ToString();

    public override string UserHostAddress => string.Empty;

    public override string UserHostName => string.Empty;
  }
}
