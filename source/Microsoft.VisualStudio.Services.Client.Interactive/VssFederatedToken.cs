// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssFederatedToken
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Client
{
  [Serializable]
  public sealed class VssFederatedToken : IssuedToken
  {
    private CookieCollection m_cookies;
    private const string c_cookieHeader = "Cookie";

    public VssFederatedToken(CookieCollection cookies)
    {
      ArgumentUtility.CheckForNull<CookieCollection>(cookies, nameof (cookies));
      this.m_cookies = cookies;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public CookieCollection CookieCollection => this.m_cookies;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.Federated;

    internal override void ApplyTo(IHttpRequest request)
    {
      IEnumerable<string> values = request.Headers.GetValues("Cookie");
      request.Headers.SetValue("Cookie", this.GetHeaderValue(values));
    }

    private string GetHeaderValue(IEnumerable<string> cookieHeaders)
    {
      List<string> values = new List<string>();
      if (cookieHeaders != null)
      {
        foreach (string cookieHeader in cookieHeaders)
          values.AddRange(((IEnumerable<string>) cookieHeader.Split(';')).Select<string, string>((Func<string, string>) (x => x.Trim())));
      }
      values.RemoveAll((Predicate<string>) (x => string.IsNullOrEmpty(x)));
      foreach (Cookie cookie1 in this.m_cookies)
      {
        Cookie cookie = cookie1;
        values.RemoveAll((Predicate<string>) (x => string.Equals(x.Substring(0, x.IndexOf('=')), cookie.Name, StringComparison.OrdinalIgnoreCase)));
        values.Add(cookie.Name + "=" + cookie.Value);
      }
      return string.Join("; ", (IEnumerable<string>) values);
    }
  }
}
