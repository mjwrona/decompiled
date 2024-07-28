// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataBatchPayloadRewriter
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnalyticsODataBatchPayloadRewriter
  {
    private static readonly Regex RequestLineRegex = new Regex("^(GET)\\W(.*)\\W(HTTP/\\d.\\d)$", RegexOptions.IgnoreCase);
    private static readonly string s_layer = "ODataBatchPayloadRewriter";

    public static async Task<HttpContent> RewriteSubRequests(HttpRequestMessage batchRequest)
    {
      IVssRequestContext requestContext = batchRequest.GetIVssRequestContext();
      if (!AnalyticsODataBatchPayloadRewriter.ShouldRewrite(batchRequest))
        return batchRequest.Content;
      string appVPath = requestContext.To(TeamFoundationHostType.Deployment).GetService<IUrlHostResolutionService>().ApplicationVirtualPath;
      string requestVPath = requestContext.VirtualPath();
      Stream stream = await batchRequest.Content.ReadAsStreamAsync();
      StringBuilder resultBuilder = new StringBuilder();
      using (StreamReader reader = new StreamReader(stream))
      {
        while (!reader.EndOfStream)
        {
          string input = await reader.ReadLineAsync();
          if (input.StartsWith("GET", StringComparison.OrdinalIgnoreCase))
          {
            Match match = AnalyticsODataBatchPayloadRewriter.RequestLineRegex.Match(input);
            if (match.Success)
            {
              string str = AnalyticsODataBatchPayloadRewriter.FixSubRequest(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, appVPath, requestVPath);
              resultBuilder.AppendLine(str);
              requestContext.TraceAlways(12013041, TraceLevel.Info, AnalyticsODataBatchPayloadRewriter.Area, AnalyticsODataBatchPayloadRewriter.Layer, "SubBatchRequest url changed from '{0}' to '{1}'", (object) match.Groups[2].Value, (object) str);
            }
            else
              resultBuilder.AppendLine(input);
          }
          else
            resultBuilder.AppendLine(input);
        }
      }
      StringContent stringContent = new StringContent(resultBuilder.ToString());
      stringContent.Headers.Clear();
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) batchRequest.Content.Headers)
        stringContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
      return (HttpContent) stringContent;
    }

    private static bool ShouldRewrite(HttpRequestMessage batchRequest)
    {
      if (!(batchRequest.Method == HttpMethod.Post))
        return false;
      IVssRequestContext ivssRequestContext = batchRequest.GetIVssRequestContext();
      return (ivssRequestContext != null ? ivssRequestContext.VirtualPath() : (string) null) != "/";
    }

    internal static string FixSubRequest(
      string method,
      string url,
      string httpVersion,
      string appVirtualPath,
      string requestVirtualPath)
    {
      int length = url.Length;
      string a = Uri.EscapeUriString(requestVirtualPath);
      url = new Regex(Regex.Escape(a.TrimEnd('/'))).Replace(url, appVirtualPath.TrimEnd('/'), 1);
      if (length == url.Length && !string.Equals(a, requestVirtualPath))
        url = new Regex(Regex.Escape(requestVirtualPath.TrimEnd('/'))).Replace(url, appVirtualPath.TrimEnd('/'), 1);
      return method + " " + url + " " + httpVersion;
    }

    protected static string Area => "AnalyticsModel";

    protected static string Layer => AnalyticsODataBatchPayloadRewriter.s_layer;
  }
}
