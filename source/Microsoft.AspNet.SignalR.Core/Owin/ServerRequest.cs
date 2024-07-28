// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Owin.ServerRequest
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Owin
{
  public class ServerRequest : IRequest
  {
    private INameValueCollection _queryString;
    private INameValueCollection _headers;
    private IDictionary<string, Cookie> _cookies;
    private IPrincipal _user;
    private readonly OwinRequest _request;

    public ServerRequest(IDictionary<string, object> environment)
    {
      this._request = new OwinRequest(environment);
      this._user = this._request.User;
    }

    public Uri Url => this._request.Uri;

    public string LocalPath => (this._request.PathBase + this._request.Path).Value;

    public INameValueCollection QueryString => LazyInitializer.EnsureInitialized<INameValueCollection>(ref this._queryString, (Func<INameValueCollection>) (() => (INameValueCollection) new ReadableStringCollectionWrapper(this._request.Query)));

    public INameValueCollection Headers => LazyInitializer.EnsureInitialized<INameValueCollection>(ref this._headers, (Func<INameValueCollection>) (() => (INameValueCollection) new ReadableStringCollectionWrapper((IReadableStringCollection) this._request.Headers)));

    public IDictionary<string, Cookie> Cookies => LazyInitializer.EnsureInitialized<IDictionary<string, Cookie>>(ref this._cookies, (Func<IDictionary<string, Cookie>>) (() =>
    {
      Dictionary<string, Cookie> cookies = new Dictionary<string, Cookie>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> cookie in this._request.Cookies)
      {
        if (!cookies.ContainsKey(cookie.Key))
          cookies.Add(cookie.Key, new Cookie(cookie.Key, cookie.Value));
      }
      return (IDictionary<string, Cookie>) cookies;
    }));

    public IPrincipal User => this._user;

    public IDictionary<string, object> Environment => this._request.Environment;

    public async Task<INameValueCollection> ReadForm() => (INameValueCollection) new ReadableStringCollectionWrapper((IReadableStringCollection) await this._request.ReadFormAsync().PreserveCulture<IFormCollection>());
  }
}
