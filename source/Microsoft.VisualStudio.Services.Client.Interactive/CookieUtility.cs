// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.CookieUtility
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Client
{
  internal static class CookieUtility
  {
    public static readonly string AcsMetadataRetrievalExceptionText = "Unable to retrieve ACS Metadata from '{0}'";
    public static readonly string FedAuthCookieName = "FedAuth";
    public static readonly string WindowsLiveSignOutUrl = "https://login.live.com/uilogout.srf";
    public static readonly Uri WindowsLiveCookieDomain = new Uri("https://login.live.com/");
    public const uint INTERNET_COOKIE_HTTPONLY = 8192;

    public static CookieCollection GetFederatedCookies(Uri cookieDomainAndPath)
    {
      CookieCollection federatedCookies = (CookieCollection) null;
      Cookie cookie1 = CookieUtility.GetCookieEx(cookieDomainAndPath, CookieUtility.FedAuthCookieName).FirstOrDefault<Cookie>();
      if (cookie1 != null)
      {
        federatedCookies = new CookieCollection();
        federatedCookies.Add(cookie1);
        for (int index = 1; index < 50; ++index)
        {
          string cookieName = CookieUtility.FedAuthCookieName + index.ToString();
          Cookie cookie2 = CookieUtility.GetCookieEx(cookieDomainAndPath, cookieName).FirstOrDefault<Cookie>();
          if (cookie2 != null)
            federatedCookies.Add(cookie2);
          else
            break;
        }
      }
      return federatedCookies;
    }

    public static CookieCollection GetFederatedCookies(string[] token)
    {
      CookieCollection federatedCookies = (CookieCollection) null;
      if (token != null && token.Length != 0 && token[0] != null)
      {
        federatedCookies = new CookieCollection();
        federatedCookies.Add(new Cookie(CookieUtility.FedAuthCookieName, token[0]));
        for (int index = 1; index < token.Length; ++index)
        {
          string name = CookieUtility.FedAuthCookieName + index.ToString();
          if (token[index] != null)
            federatedCookies.Add(new Cookie(name, token[index])
            {
              HttpOnly = true
            });
          else
            break;
        }
      }
      return federatedCookies;
    }

    public static CookieCollection GetFederatedCookies(IHttpResponse webResponse)
    {
      CookieCollection federatedCookies = (CookieCollection) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (webResponse.Headers.TryGetValues("Set-Cookie", out values))
      {
        foreach (string str1 in values)
        {
          if (str1 != null && str1.StartsWith(CookieUtility.FedAuthCookieName, StringComparison.OrdinalIgnoreCase))
          {
            string str2 = ((IEnumerable<string>) str1.Split(';')).FirstOrDefault<string>();
            int length = str2.IndexOf('=');
            if (length > 0 && length < str2.Length - 1)
            {
              string name = str2.Substring(0, length);
              string str3 = str2.Substring(length + 1);
              federatedCookies = federatedCookies ?? new CookieCollection();
              federatedCookies.Add(new Cookie(name, str3));
            }
          }
        }
      }
      return federatedCookies;
    }

    public static CookieCollection GetAllCookies(Uri cookieDomainAndPath)
    {
      CookieCollection allCookies = (CookieCollection) null;
      foreach (Cookie cookie in CookieUtility.GetCookieEx(cookieDomainAndPath, (string) null))
      {
        if (allCookies == null)
          allCookies = new CookieCollection();
        allCookies.Add(cookie);
      }
      return allCookies;
    }

    public static void DeleteFederatedCookies(Uri cookieDomainAndPath)
    {
      CookieCollection federatedCookies = CookieUtility.GetFederatedCookies(cookieDomainAndPath);
      if (federatedCookies == null)
        return;
      foreach (Cookie cookie in federatedCookies)
        CookieUtility.DeleteCookieEx(cookieDomainAndPath, cookie.Name);
    }

    public static void DeleteWindowsLiveCookies() => CookieUtility.DeleteAllCookies(CookieUtility.WindowsLiveCookieDomain);

    public static void DeleteAllCookies(Uri cookieDomainAndPath)
    {
      CookieCollection allCookies = CookieUtility.GetAllCookies(cookieDomainAndPath);
      if (allCookies == null)
        return;
      foreach (Cookie cookie in allCookies)
        CookieUtility.DeleteCookieEx(cookieDomainAndPath, cookie.Name);
    }

    [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool InternetGetCookieEx(
      string url,
      string cookieName,
      StringBuilder cookieData,
      ref int size,
      uint flags,
      IntPtr reserved);

    [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
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
      return CookieUtility.InternetSetCookieEx(url, (string) null, cookieData, flags, IntPtr.Zero);
    }

    public static bool SetCookiesEx(Uri cookiePath, CookieCollection cookies)
    {
      string url = cookiePath.ToString();
      if (!url.EndsWith("/", StringComparison.Ordinal))
        url += "/";
      bool flag = true;
      foreach (Cookie cookie in cookies)
      {
        DateTime expires = cookie.Expires;
        if (expires.Year != 1)
        {
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          object[] objArray = new object[4]
          {
            (object) cookie.Value,
            (object) cookie.Path,
            (object) cookie.Domain,
            null
          };
          expires = cookie.Expires;
          objArray[3] = (object) expires.ToString("ddd, dd-MMM-yyyy HH:mm:ss 'GMT'");
          string cookieData = string.Format((IFormatProvider) invariantCulture, "{0}; path={1}; domain={2}; expires={3}; httponly", objArray);
          flag &= CookieUtility.InternetSetCookieEx(url, cookie.Name, cookieData, 8192U, IntPtr.Zero);
        }
      }
      return flag;
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
      if (!CookieUtility.InternetGetCookieEx(url, cookieName, cookieData, ref size, flags, IntPtr.Zero))
      {
        if (size < 0)
          return cookieEx;
        cookieData = new StringBuilder(size);
        if (!CookieUtility.InternetGetCookieEx(url, cookieName, cookieData, ref size, flags, IntPtr.Zero))
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
  }
}
