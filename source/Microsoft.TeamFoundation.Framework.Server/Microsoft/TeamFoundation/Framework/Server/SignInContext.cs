// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SignInContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SignInContext
  {
    public TeamFoundationHostType HostType { get; set; }

    public Guid HostId { get; set; }

    public Dictionary<string, string> QueryString { get; set; }

    public string MsmRedirectHost { get; set; }

    public string ValidationHash { get; internal set; }

    public string ClientVersion { get; internal set; }

    public string ClientSku { get; internal set; }

    public static string EncodeQueryString(Dictionary<string, string> queryString) => HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) queryString)));

    public static Dictionary<string, string> DecodeQueryString(string base64QueryString) => JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(base64QueryString)));

    public static string ConvertQueryStringToURLSegment(string base64QueryString) => SignInContext.ConvertQueryStringToURLSegment((IDictionary<string, string>) SignInContext.DecodeQueryString(base64QueryString));

    public static string ConvertQueryStringToURLSegment(IDictionary<string, string> queryString)
    {
      if (queryString == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(1024);
      foreach (string key in (IEnumerable<string>) queryString.Keys)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append('&');
        stringBuilder.Append(key).Append('=').Append(Uri.EscapeDataString(queryString[key]));
      }
      return stringBuilder.ToString();
    }
  }
}
