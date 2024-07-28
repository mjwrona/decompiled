// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.BrowserVersion
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public struct BrowserVersion
  {
    private const string c_unknownBrowserName = "Unknown";
    private const string c_internetExplorerName = "IE";
    private const string c_tridentVersionPrefix = "Trident/";

    public string BrowserName { get; set; }

    public int MajorVersion { get; set; }

    public double MinorVersion { get; set; }

    public bool IsInternetExplorer { get; set; }

    public static BrowserVersion FromRequest(HttpRequestBase request)
    {
      BrowserVersion browserVersion = new BrowserVersion();
      try
      {
        HttpBrowserCapabilitiesBase browser = request.Browser;
        browserVersion.BrowserName = browser.Browser;
        browserVersion.MajorVersion = browser.MajorVersion;
        browserVersion.MinorVersion = browser.MinorVersion;
        browserVersion.IsInternetExplorer = string.Equals(browserVersion.BrowserName, "IE", StringComparison.OrdinalIgnoreCase);
        if (browserVersion.IsInternetExplorer)
        {
          if (browserVersion.MajorVersion == 7)
          {
            if (!string.IsNullOrEmpty(request.UserAgent))
            {
              int num1 = request.UserAgent.IndexOf("Trident/");
              if (num1 >= 0)
              {
                int startIndex = num1 + "Trident/".Length;
                int num2 = request.UserAgent.IndexOf('.', startIndex);
                if (num2 > startIndex)
                {
                  int result;
                  if (int.TryParse(request.UserAgent.Substring(startIndex, num2 - startIndex), out result))
                    browserVersion.MajorVersion = result + 4;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Warning, "WebAccess", TfsTraceLayers.Controller, nameof (FromRequest), ex);
        browserVersion.BrowserName = "Unknown";
        browserVersion.MajorVersion = 0;
        browserVersion.MinorVersion = 0.0;
      }
      return browserVersion;
    }
  }
}
