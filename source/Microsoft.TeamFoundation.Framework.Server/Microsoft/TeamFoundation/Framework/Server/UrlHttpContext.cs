// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlHttpContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UrlHttpContext : HttpContextBase
  {
    private UrlHttpContext.UrlHttpRequest m_httpRequest;
    private UrlHttpContext.ShimHttpResponse m_httpResponse;
    private Hashtable m_items;

    public UrlHttpContext(string virtualPathRoot, Uri requestUri, string clientAddress)
    {
      this.m_httpRequest = new UrlHttpContext.UrlHttpRequest(virtualPathRoot, requestUri, clientAddress);
      this.m_httpResponse = new UrlHttpContext.ShimHttpResponse();
      this.m_items = new Hashtable();
    }

    public override HttpRequestBase Request => (HttpRequestBase) this.m_httpRequest;

    public override HttpResponseBase Response => (HttpResponseBase) this.m_httpResponse;

    public override IDictionary Items => (IDictionary) this.m_items;

    private class ShimHttpResponse : HttpResponseBase
    {
      private NameValueCollection m_headers;
      private HttpCookieCollection m_cookies;
      private bool m_headersWritten;

      public override string ApplyAppPathModifier(string virtualPath) => virtualPath;

      public override void AddHeader(string name, string value) => this.Headers.Add(name, value);

      public override CancellationToken ClientDisconnectedToken => CancellationToken.None;

      public override NameValueCollection Headers
      {
        get
        {
          if (this.m_headers == null)
            this.m_headers = new NameValueCollection();
          return this.m_headers;
        }
      }

      public override HttpCookieCollection Cookies
      {
        get
        {
          if (this.m_cookies == null)
            this.m_cookies = new HttpCookieCollection();
          return this.m_cookies;
        }
      }

      public override bool HeadersWritten => this.m_headersWritten;
    }

    private class UrlHttpRequest : HttpRequestBase
    {
      private readonly string m_virtualPathRoot;
      private readonly Uri m_requestUri;
      private readonly string m_userHostAddress;
      private NameValueCollection m_headers = new NameValueCollection();
      private NameValueCollection m_serverVariables = new NameValueCollection();
      private HttpCookieCollection m_cookies;

      public UrlHttpRequest(string virtualPathRoot, Uri requestUri, string clientAddress)
      {
        ArgumentUtility.CheckForNull<string>(virtualPathRoot, nameof (virtualPathRoot));
        ArgumentUtility.CheckForNull<Uri>(requestUri, "httpRequest");
        this.m_virtualPathRoot = virtualPathRoot;
        this.m_requestUri = requestUri;
        this.m_userHostAddress = clientAddress;
      }

      public override string AppRelativeCurrentExecutionFilePath
      {
        get
        {
          string absolutePath = this.m_requestUri.AbsolutePath;
          if (!absolutePath.StartsWith(this.m_virtualPathRoot, StringComparison.OrdinalIgnoreCase))
            return (string) null;
          return "~" + (this.m_virtualPathRoot.Length == 1 ? absolutePath : absolutePath.Substring(this.m_virtualPathRoot.Length)).TrimEnd('/');
        }
      }

      public override string FilePath
      {
        get
        {
          string absolutePath = this.m_requestUri.AbsolutePath;
          if (!absolutePath.StartsWith(this.m_virtualPathRoot, StringComparison.OrdinalIgnoreCase))
            return (string) null;
          return absolutePath.TrimEnd('/');
        }
      }

      public override ReadEntityBodyMode ReadEntityBodyMode => ReadEntityBodyMode.Bufferless;

      public override string ApplicationPath => this.m_virtualPathRoot;

      public override string CurrentExecutionFilePath => this.FilePath;

      public override NameValueCollection Headers => this.m_headers;

      public override string HttpMethod => "GET";

      public override bool IsLocal => false;

      public override string Path => this.m_requestUri.AbsolutePath;

      public override string PathInfo => string.Empty;

      public override NameValueCollection QueryString => this.m_requestUri.ParseQueryString();

      public override string RawUrl => this.m_requestUri.PathAndQuery;

      public override string RequestType => "GET";

      public override NameValueCollection ServerVariables => this.m_serverVariables;

      public override CancellationToken TimedOutToken => CancellationToken.None;

      public override Uri Url => this.m_requestUri;

      public override string UserAgent => string.Empty;

      public override string UserHostAddress => this.m_userHostAddress;

      public override string UserHostName => string.Empty;

      public override Uri UrlReferrer => this.m_requestUri;

      public override HttpCookieCollection Cookies
      {
        get
        {
          if (this.m_cookies == null)
            this.m_cookies = new HttpCookieCollection();
          return this.m_cookies;
        }
      }
    }
  }
}
