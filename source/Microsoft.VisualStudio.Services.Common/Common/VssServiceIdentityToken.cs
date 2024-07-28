// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssServiceIdentityToken
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common
{
  [Serializable]
  public sealed class VssServiceIdentityToken : IssuedToken
  {
    private string m_token;
    private const string c_expiresName = "ExpiresOn";

    public VssServiceIdentityToken(string token)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      this.m_token = token;
      Dictionary<string, string> tokenValues;
      if (!VssServiceIdentityToken.TryGetNameValues(token, out tokenValues))
        return;
      tokenValues.TryGetValue("ExpiresOn", out string _);
    }

    public string Token => this.m_token;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.ServiceIdentity;

    internal override void ApplyTo(IHttpRequest request) => request.Headers.SetValue("Authorization", "WRAP access_token=\"" + this.m_token + "\"");

    internal static VssServiceIdentityToken ExtractToken(string responseValue) => new VssServiceIdentityToken(UriUtility.UrlDecode(((IEnumerable<string>) responseValue.Split('&')).Single<string>((Func<string, bool>) (value => value.StartsWith("wrap_access_token=", StringComparison.OrdinalIgnoreCase))).Split('=')[1], VssHttpRequestSettings.Encoding));

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
        if (strArray.Length != 2 || dict.ContainsKey(strArray[0]))
          return dict;
        dict.Add(UriUtility.UrlDecode(strArray[0]), UriUtility.UrlDecode(strArray[1]));
        return dict;
      }));
      return true;
    }
  }
}
