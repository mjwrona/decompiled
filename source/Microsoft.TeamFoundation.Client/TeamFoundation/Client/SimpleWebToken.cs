// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.SimpleWebToken
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.VssServiceIdentityToken instead.", false)]
  [Serializable]
  public sealed class SimpleWebToken : IssuedToken
  {
    private string m_token;
    private const string c_expiresName = "ExpiresOn";
    private static DateTime s_epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public SimpleWebToken(string token)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      this.m_token = token;
      Dictionary<string, string> tokenValues;
      if (!SimpleWebToken.TryGetNameValues(token, out tokenValues))
        return;
      tokenValues.TryGetValue("ExpiresOn", out string _);
    }

    public string Token => this.m_token;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.ServiceIdentity;

    internal override void ApplyTo(HttpWebRequest request)
    {
      request.Headers.Remove(HttpRequestHeader.Authorization);
      request.Headers.Add(HttpRequestHeader.Authorization, "WRAP access_token=\"" + this.m_token + "\"");
      request.ServicePoint.Expect100Continue = false;
    }

    internal static SimpleWebToken ExtractToken(byte[] responseData, Encoding encoding) => new SimpleWebToken(UriUtility.UrlDecode(((IEnumerable<string>) encoding.GetString(responseData).Split('&')).Single<string>((Func<string, bool>) (value => value.StartsWith("wrap_access_token=", StringComparison.OrdinalIgnoreCase))).Split('=')[1], encoding));

    internal static bool TryGetNameValues(string token, out Dictionary<string, string> tokenValues)
    {
      tokenValues = (Dictionary<string, string>) null;
      if (string.IsNullOrEmpty(token))
        return false;
      tokenValues = ((IEnumerable<string>) token.Split('&')).Aggregate<string, Dictionary<string, string>>(new Dictionary<string, string>(), (Func<Dictionary<string, string>, string, Dictionary<string, string>>) ((dict, rawNameValue) =>
      {
        if (rawNameValue == string.Empty)
          return dict;
        string[] strArray = rawNameValue.Split('=');
        if (strArray.Length != 2)
        {
          TeamFoundationTrace.Error("Invalid formEncodedstring - contains a name/value pair missing an = character");
          return dict;
        }
        if (dict.ContainsKey(strArray[0]))
        {
          TeamFoundationTrace.Error("Repeated name/value pair in form");
          return dict;
        }
        dict.Add(UriUtility.UrlDecode(strArray[0]), UriUtility.UrlDecode(strArray[1]));
        return dict;
      }));
      return true;
    }
  }
}
