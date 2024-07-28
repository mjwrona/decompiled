// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlCumulativeUpdateDownloader
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public sealed class SqlCumulativeUpdateDownloader : VssfFileDownloader
  {
    private const string c_downloadLatestUpdateWebPage2022 = "https://www.microsoft.com/en-us/download/details.aspx?id=105013";
    private const string c_downloadLatestUpdateWebPage2019 = "https://www.microsoft.com/en-sg/download/details.aspx?id=100809";
    private readonly SqlExpressVersion m_sqlVersion;

    public SqlCumulativeUpdateDownloader(SqlExpressVersion sqlVersion)
      : base()
    {
      this.m_sqlVersion = sqlVersion;
      if (sqlVersion != SqlExpressVersion.SqlExpress2022 && sqlVersion != SqlExpressVersion.SqlExpress2019)
        throw new ArgumentException("Only SQL Server 2022 and 2019 are supported", nameof (sqlVersion));
    }

    public string Download(
      string downloadDirectory,
      DownloadProgressChangedEventHandler onDownloadProgressChanged,
      ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(downloadDirectory, nameof (downloadDirectory));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info("Downloading the latest cumulative Update for SQL Server");
      SqlServerUpdateInfo result = this.GetUpdateInfoAsync().GetAwaiter().GetResult();
      string fileName = Path.Combine(downloadDirectory, result.Url.Substring(result.Url.LastIndexOf('/') + 1));
      this.Download(new Uri(result.Url), fileName, onDownloadProgressChanged, true, logger);
      return fileName;
    }

    public async Task<SqlServerUpdateInfo> GetUpdateInfoAsync()
    {
      SqlCumulativeUpdateDownloader updateDownloader = this;
      string requestUri = updateDownloader.m_sqlVersion == SqlExpressVersion.SqlExpress2022 ? "https://www.microsoft.com/en-us/download/details.aspx?id=105013" : "https://www.microsoft.com/en-sg/download/details.aspx?id=100809";
      int result1;
      if (updateDownloader.m_sqlVersion == SqlExpressVersion.SqlExpress2022)
      {
        using (HttpClient httpClient = new HttpClient((HttpMessageHandler) new VssHttpRetryMessageHandler(new VssHttpRetryOptions()
        {
          MaxRetries = 3
        }, (HttpMessageHandler) new HttpClientHandler()
        {
          AllowAutoRedirect = false
        })))
        {
          using (HttpResponseMessage result2 = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://go.microsoft.com/fwlink/?LinkId=2260110")).GetAwaiter().GetResult())
          {
            if (result2.StatusCode == HttpStatusCode.Found)
            {
              if (result2.Headers.Location.ToString().IndexOf("bing.", StringComparison.OrdinalIgnoreCase) < 0)
              {
                Uri location = result2.Headers.Location;
                NameValueCollection queryString = HttpUtility.ParseQueryString(location.Query);
                string str = location.AbsoluteUri.ToString();
                string url = str.Substring(0, str.IndexOf('?'));
                int kb = int.Parse(queryString["kb"]);
                result1 = int.Parse(queryString["size"]);
                string datePublished = queryString["published"];
                string version = queryString["version"];
                string description = queryString["description"];
                return new SqlServerUpdateInfo(url, description, version, kb, result1, datePublished);
              }
            }
          }
        }
      }
      HttpClient httpClient1 = updateDownloader.HttpClientFactory.CreateHttpClient(3, 30);
      using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri))
      {
        using (HttpResponseMessage responseMessage = await httpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead))
        {
          responseMessage.EnsureSuccessStatusCode();
          string str = await responseMessage.Content.ReadAsStringAsync();
          Match match = Regex.Match(str, "https:\\/\\/download\\.microsoft\\.com\\/download\\/[0-9A-Fa-f\\/-]*\\/(?<fn>SQLServer20\\d\\d-KB(?<kb>\\d+)-x64\\.exe)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(10.0));
          string url = match.Success ? match.Value : throw new PageScrapeException("Could not find download url.");
          int kb = int.Parse(match.Groups["kb"].Value);
          string attribute1 = SqlCumulativeUpdateDownloader.GetAttribute(str, "downloadDescription");
          string attribute2 = SqlCumulativeUpdateDownloader.GetAttribute(str, "version");
          string attribute3 = SqlCumulativeUpdateDownloader.GetAttribute(str, "datePublished");
          int.TryParse(SqlCumulativeUpdateDownloader.GetAttribute(str, "size"), out result1);
          return new SqlServerUpdateInfo(url, attribute1, attribute2, kb, result1, attribute3);
        }
      }
    }

    private static string GetAttribute(string page, string attributeName)
    {
      Match match = Regex.Match(page, "\"" + attributeName + "\":\"(?<desc>[^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(10.0));
      return match.Success ? match.Groups[1].Value : (string) null;
    }
  }
}
