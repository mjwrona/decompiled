// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.FederatedAcsLogon
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FederatedAcsLogon
  {
    private Uri _acsIdentityProviderServiceAddress;
    private EventHandler<AuthenticationCompleteEventArgs> _authenticationComplete;
    private Uri _cookieDomainAndPath;
    private CookieCollection _cookieCollection;
    private EventHandler<EventArgs> _userAgentComplete;
    private Uri _replyToAddress;
    private FederatedAcsLogon.LogonState _state;
    private static Uri windowsLiveCookieDomain = new Uri("https://login.live.com/");
    private const string windowsLiveUISignOutUrl = "https://login.live.com/uilogout.srf";
    private object _syncLock = new object();
    [CLSCompliant(false)]
    public const uint INTERNET_COOKIE_HTTPONLY = 8192;

    public FederatedAcsLogon(
      Uri replyToAddress,
      Uri cookieDomainAndPath,
      Uri acsIdentityProviderServiceAddress)
    {
      this._replyToAddress = replyToAddress;
      this._acsIdentityProviderServiceAddress = acsIdentityProviderServiceAddress;
      this._cookieDomainAndPath = cookieDomainAndPath;
    }

    public event EventHandler<AuthenticationCompleteEventArgs> AuthenticationComplete
    {
      add
      {
        lock (this._syncLock)
          this._authenticationComplete += value;
      }
      remove
      {
        lock (this._syncLock)
          this._authenticationComplete -= value;
      }
    }

    public event EventHandler<EventArgs> UserAgentComplete
    {
      add
      {
        lock (this._syncLock)
          this._userAgentComplete += value;
      }
      remove
      {
        lock (this._syncLock)
          this._userAgentComplete -= value;
      }
    }

    public CookieCollection SsoCookieCollection => this._cookieCollection;

    public FederatedAcsLogon.LogonState State => this._state;

    public void Navigated(Uri address)
    {
      if (!this._replyToAddress.PathAndQuery.Equals(address.PathAndQuery, StringComparison.OrdinalIgnoreCase) && !address.AbsoluteUri.StartsWith("https://login.live.com/uilogout.srf", StringComparison.OrdinalIgnoreCase))
        return;
      this._cookieCollection = FederatedAcsLogon.GetFederatedCookies(address);
      this.SetAuthenticationComplete();
    }

    public void Navigating(Uri address)
    {
      if (!this._replyToAddress.PathAndQuery.Equals(address.PathAndQuery, StringComparison.OrdinalIgnoreCase))
        return;
      this._state = FederatedAcsLogon.LogonState.WaitingForSsoCookie;
      this.SetUserAgentComplete();
    }

    private void SetAuthenticationComplete()
    {
      this._state = FederatedAcsLogon.LogonState.Complete;
      EventHandler<AuthenticationCompleteEventArgs> authenticationComplete;
      lock (this._syncLock)
        authenticationComplete = this._authenticationComplete;
      if (authenticationComplete == null)
        return;
      authenticationComplete((object) this, new AuthenticationCompleteEventArgs(this._cookieCollection));
    }

    private void SetUserAgentComplete()
    {
      EventHandler<EventArgs> userAgentComplete;
      lock (this._syncLock)
        userAgentComplete = this._userAgentComplete;
      if (userAgentComplete == null)
        return;
      userAgentComplete((object) this, new EventArgs());
    }

    public CookieCollection GetFederatedCookies() => FederatedAcsLogon.GetFederatedCookies(this._cookieDomainAndPath);

    public static CookieCollection GetFederatedCookies(Uri cookieDomainAndPath)
    {
      CookieCollection federatedCookies = (CookieCollection) null;
      Cookie cookie1 = FederatedAcsLogon.GetCookieEx(cookieDomainAndPath, "FedAuth").FirstOrDefault<Cookie>();
      if (cookie1 != null)
      {
        federatedCookies = new CookieCollection();
        federatedCookies.Add(cookie1);
        for (int index = 1; index < 50; ++index)
        {
          string cookieName = "FedAuth" + index.ToString();
          Cookie cookie2 = FederatedAcsLogon.GetCookieEx(cookieDomainAndPath, cookieName).FirstOrDefault<Cookie>();
          if (cookie2 != null)
            federatedCookies.Add(cookie2);
          else
            break;
        }
      }
      return federatedCookies;
    }

    public static CookieCollection GetFederatedCookies(CookieCollection cookies)
    {
      CookieCollection federatedCookies = (CookieCollection) null;
      Cookie cookie1 = cookies["FedAuth"];
      if (cookie1 != null)
      {
        federatedCookies = new CookieCollection();
        federatedCookies.Add(cookie1);
        for (int index = 1; index < 50; ++index)
        {
          string name = "FedAuth" + index.ToString();
          Cookie cookie2 = cookies[name];
          if (cookie2 != null)
            federatedCookies.Add(cookie2);
          else
            break;
        }
      }
      return federatedCookies;
    }

    public static CookieCollection GetAllCookies(Uri cookieDomainAndPath)
    {
      CookieCollection allCookies = (CookieCollection) null;
      foreach (Cookie cookie in FederatedAcsLogon.GetCookieEx(cookieDomainAndPath, (string) null))
      {
        if (allCookies == null)
          allCookies = new CookieCollection();
        allCookies.Add(cookie);
      }
      return allCookies;
    }

    public void DeleteFederatedCookies() => FederatedAcsLogon.DeleteFederatedCookies(this._cookieDomainAndPath, true);

    public static void DeleteFederatedCookies(Uri cookieDomainAndPath) => FederatedAcsLogon.DeleteFederatedCookies(cookieDomainAndPath, true);

    public static void DeleteFederatedCookies(Uri cookieDomainAndPath, bool exactDomainMatch)
    {
      CookieCollection federatedCookies = FederatedAcsLogon.GetFederatedCookies(cookieDomainAndPath);
      if (federatedCookies == null)
        return;
      if (exactDomainMatch)
      {
        foreach (Cookie cookie in federatedCookies)
          FederatedAcsLogon.DeleteCookieEx(cookieDomainAndPath, cookie.Name);
      }
      else
      {
        Uri[] allCookieDomains = FederatedAcsLogon.GetAllCookieDomains(cookieDomainAndPath);
        foreach (Cookie cookie in federatedCookies)
        {
          foreach (Uri cookiePath in allCookieDomains)
            FederatedAcsLogon.DeleteCookieEx(cookiePath, cookie.Name);
        }
      }
    }

    public static void DeleteWindowsLiveCookies() => FederatedAcsLogon.DeleteAllCookies(FederatedAcsLogon.windowsLiveCookieDomain);

    public static void DeleteAllCookies(Uri cookieDomainAndPath)
    {
      CookieCollection allCookies = FederatedAcsLogon.GetAllCookies(cookieDomainAndPath);
      if (allCookies == null)
        return;
      foreach (Cookie cookie in allCookies)
        FederatedAcsLogon.DeleteCookieEx(cookieDomainAndPath, cookie.Name);
    }

    private static Uri[] GetAllCookieDomains(Uri cookieDomainAndPath)
    {
      List<Uri> uriList = new List<Uri>();
      string[] strArray = cookieDomainAndPath.Host.Split('.');
      for (int count = Math.Min(strArray.Length, 2); count <= strArray.Length; ++count)
      {
        string str = string.Join(".", strArray, strArray.Length - count, count);
        uriList.Add(new UriBuilder(cookieDomainAndPath)
        {
          Host = str
        }.Uri);
      }
      return uriList.ToArray();
    }

    [DllImport("wininet.dll", SetLastError = true)]
    private static extern bool InternetGetCookieEx(
      string url,
      string cookieName,
      StringBuilder cookieData,
      ref int size,
      uint flags,
      IntPtr reserved);

    [DllImport("wininet.dll", SetLastError = true)]
    private static extern bool InternetSetCookieEx(
      string url,
      string cookieName,
      string cookieData,
      uint flags,
      IntPtr reserved);

    public static bool DeleteCookieEx(Uri cookiePath, string cookieName)
    {
      uint flags = 8192;
      string url = cookiePath.ToString();
      if (!url.EndsWith("/", StringComparison.Ordinal))
        url += "/";
      DateTime dateTime = DateTime.UtcNow.AddYears(-1);
      string cookieData = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}=0;expires={1};path=/;domain={2};httponly", (object) cookieName, (object) dateTime.ToString("R"), (object) cookiePath.Host);
      return FederatedAcsLogon.InternetSetCookieEx(url, (string) null, cookieData, flags, IntPtr.Zero);
    }

    public static List<Cookie> GetCookieEx(Uri cookiePath, string cookieName)
    {
      uint flags = 8192;
      List<Cookie> cookieEx = new List<Cookie>();
      int size = 256;
      StringBuilder cookieData = new StringBuilder(size);
      string url = cookiePath.ToString();
      if (!url.EndsWith("/", StringComparison.Ordinal))
        url += "/";
      if (!FederatedAcsLogon.InternetGetCookieEx(url, cookieName, cookieData, ref size, flags, IntPtr.Zero))
      {
        if (size < 0)
          return cookieEx;
        cookieData = new StringBuilder(size);
        if (!FederatedAcsLogon.InternetGetCookieEx(url, cookieName, cookieData, ref size, flags, IntPtr.Zero))
          return cookieEx;
      }
      if (cookieData.Length > 0)
      {
        string str1 = cookieData.ToString();
        char[] chArray = new char[1]{ ';' };
        foreach (string str2 in str1.Split(chArray))
        {
          char[] separator = new char[1]{ '=' };
          string[] strArray = str2.Split(separator, 2);
          if (strArray.Length == 2)
            cookieEx.Add(new Cookie()
            {
              Name = strArray[0].TrimStart(),
              Value = strArray[1],
              HttpOnly = true
            });
        }
      }
      return cookieEx;
    }

    public enum LogonState
    {
      Initialized,
      WaitingForAcsResponse,
      WaitingForSsoCookie,
      Complete,
    }
  }
}
