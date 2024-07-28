// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CookieToken
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Client.VssFederatedToken instead.", false)]
  [Serializable]
  public sealed class CookieToken : IssuedToken
  {
    private CookieCollection m_cookies;

    public CookieToken(CookieCollection cookies)
    {
      ArgumentUtility.CheckForNull<CookieCollection>(cookies, nameof (cookies));
      this.m_cookies = cookies;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public CookieCollection CookieCollection => this.m_cookies;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.Federated;

    internal override void RequestUserData(HttpWebRequest webRequest)
    {
      if (this.IsAuthenticated || this.FromStorage)
        return;
      webRequest.Headers.Add("X-VSS-UserData", string.Empty);
    }

    internal override void GetUserData(HttpWebResponse webResponse)
    {
      string header = webResponse.Headers["X-VSS-UserData"];
      if (string.IsNullOrWhiteSpace(header))
        return;
      string[] strArray = header.Split(':');
      if (strArray.Length < 2)
        return;
      this.UserId = Guid.Parse(strArray[0]);
      this.UserName = strArray[1];
    }

    internal override void ApplyTo(HttpWebRequest webRequest)
    {
      if (webRequest.CookieContainer == null)
        webRequest.CookieContainer = new CookieContainer();
      CookieCollection cookies = webRequest.CookieContainer.GetCookies(webRequest.RequestUri) ?? new CookieCollection();
      cookies.Add(this.m_cookies);
      webRequest.CookieContainer.Add(webRequest.RequestUri, cookies);
      if (this.m_cookies.Count <= 0)
        return;
      webRequest.ServicePoint.Expect100Continue = false;
    }
  }
}
