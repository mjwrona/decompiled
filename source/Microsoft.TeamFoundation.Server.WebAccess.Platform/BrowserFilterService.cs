// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.BrowserFilterService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class BrowserFilterService : IVssFrameworkService
  {
    public const string RegistryRoot = "/Configuration/WebAccess/BrowserFilter";
    private const string c_defaultBrowserBlockList = "IE/7-/Trident;IEMobile/7-;Safari/3-;Chrome/7-;Opera/9.79-;Firefox/3.4-";
    private const string c_defaultBrowserWarnList = "IE/10-";
    private ReaderWriterLock m_cacheLock = new ReaderWriterLock();
    private BrowserFilterMode m_mode = BrowserFilterMode.BlockList;
    private string m_blockedMessage = string.Empty;
    private string m_unsupportedMessage = string.Empty;
    private List<BrowserFilterEntry> m_blockEntries = new List<BrowserFilterEntry>();
    private List<BrowserFilterEntry> m_warnEntries = new List<BrowserFilterEntry>();

    private void ValidateHostType(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.ValidateHostType(systemRequestContext);
      this.LoadEntries(systemRequestContext);
      systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/WebAccess/BrowserFilter/...");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadEntries(requestContext);
    }

    public BrowserFilterMode GetMode()
    {
      this.m_cacheLock.AcquireReaderLock(-1);
      try
      {
        return this.m_mode;
      }
      finally
      {
        this.m_cacheLock.ReleaseReaderLock();
      }
    }

    public string GetBlockedMessage()
    {
      this.m_cacheLock.AcquireReaderLock(-1);
      try
      {
        return this.m_blockedMessage;
      }
      finally
      {
        this.m_cacheLock.ReleaseReaderLock();
      }
    }

    public string GetUnsupportedMessage()
    {
      this.m_cacheLock.AcquireReaderLock(-1);
      try
      {
        return this.m_unsupportedMessage;
      }
      finally
      {
        this.m_cacheLock.ReleaseReaderLock();
      }
    }

    public IEnumerable<BrowserFilterEntry> GetEntries()
    {
      this.m_cacheLock.AcquireReaderLock(-1);
      try
      {
        return (IEnumerable<BrowserFilterEntry>) this.m_blockEntries.ToArray();
      }
      finally
      {
        this.m_cacheLock.ReleaseReaderLock();
      }
    }

    public BrowserSupportLevel GetSupportLevel(HttpRequestBase request)
    {
      BrowserVersion browserVersion = BrowserVersion.FromRequest(request);
      this.m_cacheLock.AcquireReaderLock(-1);
      try
      {
        if (this.m_mode == BrowserFilterMode.Disabled)
          return BrowserSupportLevel.Allowed;
        foreach (BrowserFilterEntry blockEntry in this.m_blockEntries)
        {
          if (blockEntry.IsMatch(browserVersion.BrowserName, browserVersion.MajorVersion, browserVersion.MinorVersion, request.UserAgent))
            return this.m_mode == BrowserFilterMode.AllowList ? BrowserSupportLevel.Allowed : BrowserSupportLevel.Blocked;
        }
        foreach (BrowserFilterEntry warnEntry in this.m_warnEntries)
        {
          if (warnEntry.IsMatch(browserVersion.BrowserName, browserVersion.MajorVersion, browserVersion.MinorVersion, request.UserAgent))
            return BrowserSupportLevel.UnSupported;
        }
        return this.m_mode != BrowserFilterMode.AllowList ? BrowserSupportLevel.Allowed : BrowserSupportLevel.Blocked;
      }
      finally
      {
        this.m_cacheLock.ReleaseReaderLock();
      }
    }

    private void LoadEntries(IVssRequestContext requestContext)
    {
      this.m_cacheLock.AcquireWriterLock(-1);
      try
      {
        this.m_blockEntries.Clear();
        this.m_warnEntries.Clear();
        Dictionary<string, string> dictionary = requestContext.GetService<CachedRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/WebAccess/BrowserFilter/...").ToDictionary<RegistryEntry, string, string>((Func<RegistryEntry, string>) (entry => entry.Path), (Func<RegistryEntry, string>) (entry => entry.Value));
        BrowserFilterMode defaultValue = BrowserFilterMode.BlockList;
        this.m_mode = defaultValue;
        string s;
        if (dictionary.TryGetValue("/Configuration/WebAccess/BrowserFilter/Mode", out s) && !string.IsNullOrEmpty(s))
        {
          int result;
          this.m_mode = !int.TryParse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result) ? s.ParseEnumDefault<BrowserFilterMode>(defaultValue) : (!Enum.IsDefined(typeof (BrowserFilterMode), (object) result) ? defaultValue : (BrowserFilterMode) result);
        }
        if (!dictionary.TryGetValue("/Configuration/WebAccess/BrowserFilter/Message", out this.m_blockedMessage))
          this.m_blockedMessage = WACommonResources.NotSupportedBrowserMessage;
        TeamFoundationExecutionEnvironment executionEnvironment;
        if (!dictionary.TryGetValue("/Configuration/WebAccess/BrowserFilter/WarnMessage", out this.m_unsupportedMessage))
        {
          executionEnvironment = requestContext.ExecutionEnvironment;
          this.m_unsupportedMessage = !executionEnvironment.IsHostedDeployment ? WACommonResources.UnsupportedBrowserWarningMessage : WACommonResources.UnsupportedBrowserWarningMessageHosted;
        }
        string filterString1;
        if (!dictionary.TryGetValue("/Configuration/WebAccess/BrowserFilter/List", out filterString1))
        {
          if (this.m_mode == BrowserFilterMode.BlockList)
          {
            executionEnvironment = requestContext.ExecutionEnvironment;
            if (executionEnvironment.IsHostedDeployment)
            {
              filterString1 = "IE/7-/Trident;IEMobile/7-;Safari/3-;Chrome/7-;Opera/9.79-;Firefox/3.4-";
              goto label_12;
            }
          }
          filterString1 = (string) null;
        }
label_12:
        this.AddBrowserEntries(requestContext, (IList<BrowserFilterEntry>) this.m_blockEntries, filterString1);
        string filterString2;
        if (!dictionary.TryGetValue("/Configuration/WebAccess/BrowserFilter/WarnList", out filterString2))
          filterString2 = "IE/10-";
        this.AddBrowserEntries(requestContext, (IList<BrowserFilterEntry>) this.m_warnEntries, filterString2);
        string str;
        if (!dictionary.TryGetValue("/Configuration/WebAccess/BrowserFilter/RegexMatches", out str) || string.IsNullOrEmpty(str))
          return;
        foreach (string pattern in str.Split<string>(';', StringSplitOptions.RemoveEmptyEntries))
        {
          try
          {
            Regex regex = new Regex(pattern);
            this.m_blockEntries.Add(new BrowserFilterEntry()
            {
              RegexMatch = true,
              Browser = pattern
            });
          }
          catch (ArgumentException ex)
          {
            TeamFoundationEventLog.Default.Log(requestContext, ex.Message, ex.ToReadableStackTrace(), TeamFoundationEventId.ConfigurationError, EventLogEntryType.Warning);
            requestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, (Exception) ex);
          }
        }
      }
      finally
      {
        this.m_cacheLock.ReleaseWriterLock();
      }
    }

    private void AddBrowserEntries(
      IVssRequestContext requestContext,
      IList<BrowserFilterEntry> entryList,
      string filterString)
    {
      if (string.IsNullOrEmpty(filterString))
        return;
      foreach (string browser in filterString.Split<string>(';', StringSplitOptions.RemoveEmptyEntries))
      {
        try
        {
          BrowserFilterEntry listEntry = BrowserFilterService.ParseListEntry(browser);
          entryList.Add(listEntry);
        }
        catch (FormatException ex)
        {
          TeamFoundationEventLog.Default.Log(requestContext, ex.Message, ex.ToReadableStackTrace(), TeamFoundationEventId.ConfigurationError, EventLogEntryType.Warning);
          requestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, (Exception) ex);
        }
        catch (OverflowException ex)
        {
          TeamFoundationEventLog.Default.Log(requestContext, ex.Message, ex.ToReadableStackTrace(), TeamFoundationEventId.ConfigurationError, EventLogEntryType.Warning);
          requestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, (Exception) ex);
        }
      }
    }

    public static bool IsBrowserIE8OrOlder(HttpRequestBase request)
    {
      BrowserVersion browserVersion = BrowserVersion.FromRequest(request);
      return browserVersion.IsInternetExplorer && browserVersion.MajorVersion <= 8;
    }

    public static bool IsBrowserIE9OrOlder(HttpRequestBase request)
    {
      BrowserVersion browserVersion = BrowserVersion.FromRequest(request);
      return browserVersion.IsInternetExplorer && browserVersion.MajorVersion <= 9;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsBrowserIE9OrOlder(HttpBrowserCapabilitiesBase browser) => string.Equals(browser.Browser, "IE") && browser.MajorVersion <= 9;

    public static BrowserFilterEntry ParseListEntry(string browser)
    {
      string[] strArray1 = browser.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
      BrowserFilterEntry listEntry = new BrowserFilterEntry()
      {
        Browser = strArray1[0]
      };
      if (strArray1.Length > 1)
      {
        string str = strArray1[1];
        bool? nullable = new bool?();
        if (str.EndsWith("-"))
        {
          nullable = new bool?(true);
          str = str.TrimEnd('-');
        }
        else if (str.EndsWith("+"))
        {
          nullable = new bool?(false);
          str = str.TrimEnd('+');
        }
        string[] strArray2 = str.Split(new char[1]{ '.' }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray2.Length != 0)
        {
          listEntry.MajorVersion = new int?(int.Parse(strArray2[0], (IFormatProvider) NumberFormatInfo.InvariantInfo));
          listEntry.Below = nullable;
          if (strArray2.Length > 1)
          {
            string s = strArray2[1];
            if (strArray2.Length > 2)
              s = s + "." + strArray2[2];
            listEntry.MinorVersion = new double?(double.Parse(s, (IFormatProvider) NumberFormatInfo.InvariantInfo));
          }
        }
      }
      if (strArray1.Length > 2)
        listEntry.Exception = strArray1[2];
      return listEntry;
    }
  }
}
