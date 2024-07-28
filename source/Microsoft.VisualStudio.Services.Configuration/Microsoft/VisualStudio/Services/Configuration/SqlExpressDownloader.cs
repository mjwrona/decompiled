// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlExpressDownloader
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SqlExpressDownloader : VssfFileDownloader
  {
    private static readonly Dictionary<string, int> s_langLCID = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "ENU",
        1033
      },
      {
        "DEU",
        1031
      },
      {
        "ESN",
        3082
      },
      {
        "FRA",
        1036
      },
      {
        "ITA",
        1040
      },
      {
        "JPN",
        1041
      },
      {
        "KOR",
        1042
      },
      {
        "RUS",
        1049
      },
      {
        "CHT",
        1028
      },
      {
        "CHS",
        2052
      },
      {
        "PTB",
        1046
      }
    };
    private static string[] s_supportedLanguages;
    private const string c_2016FWLinkFormat = "https://go.microsoft.com/fwlink/?LinkID=809081&clcid=0x{0:X}";
    private const string c_2017FWLinkFormat = "https://go.microsoft.com/fwlink/?LinkID=857946&clcid=0x{0:X}";
    private const string c_2019FWLinkFormat = "https://go.microsoft.com/fwlink/?LinkID=2114409&clcid=0x{0:X}";
    private const string c_2022FWLinkFormat = "https://go.microsoft.com/fwlink/?LinkID=2213259&clcid=0x{0:X}";
    private readonly SqlExpressVersion m_sqlExpressVersion;

    public SqlExpressDownloader(SqlExpressVersion sqlExpressVersion)
      : base()
    {
      this.m_sqlExpressVersion = sqlExpressVersion;
    }

    public bool ValidateDownloadSiteIsReachable(string lang, ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info("Validating that Microsoft Download Center website is reachable. Lang: {0}.", (object) lang);
      string downloadUrl = SqlExpressDownloader.GetDownloadUrl(this.m_sqlExpressVersion, lang);
      logger.Info("Download url: {0}", (object) downloadUrl);
      return this.ValidateDownloadSiteIsReachable(new Uri(downloadUrl), logger);
    }

    public void Download(
      string lang,
      string fileName,
      DownloadProgressChangedEventHandler onDownloadProgressChanged,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info("Downloading SQL Express installer. Lang: {0}.", (object) lang);
      this.Download(new Uri(SqlExpressDownloader.GetDownloadUrl(this.m_sqlExpressVersion, lang)), fileName, onDownloadProgressChanged, true, logger);
    }

    public static string GetDownloadUrl(SqlExpressVersion sqlExpressVersion, string lang)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(lang, nameof (lang));
      string format;
      switch (sqlExpressVersion)
      {
        case SqlExpressVersion.SqlExpress2016:
          format = "https://go.microsoft.com/fwlink/?LinkID=809081&clcid=0x{0:X}";
          break;
        case SqlExpressVersion.SqlExpress2017:
          format = "https://go.microsoft.com/fwlink/?LinkID=857946&clcid=0x{0:X}";
          break;
        case SqlExpressVersion.SqlExpress2019:
          format = "https://go.microsoft.com/fwlink/?LinkID=2114409&clcid=0x{0:X}";
          break;
        case SqlExpressVersion.SqlExpress2022:
          format = "https://go.microsoft.com/fwlink/?LinkID=2213259&clcid=0x{0:X}";
          break;
        default:
          throw new ArgumentException("{sqlExpressVersion} is not supported.", nameof (sqlExpressVersion));
      }
      int num;
      if (!SqlExpressDownloader.s_langLCID.TryGetValue(lang, out num))
        throw new ArgumentException("'" + lang + "' language is not supported.", nameof (lang));
      return string.Format(format, (object) num, (object) num);
    }

    public static string[] SupportedLanguages
    {
      get
      {
        if (SqlExpressDownloader.s_supportedLanguages == null)
          SqlExpressDownloader.s_supportedLanguages = SqlExpressDownloader.s_langLCID.Keys.ToArray<string>();
        return SqlExpressDownloader.s_supportedLanguages;
      }
    }

    public static string GetLanguage(int lcid)
    {
      foreach (KeyValuePair<string, int> keyValuePair in SqlExpressDownloader.s_langLCID)
      {
        if (keyValuePair.Value == lcid)
          return keyValuePair.Key;
      }
      return (string) null;
    }
  }
}
