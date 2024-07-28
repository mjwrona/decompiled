// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssBasicToken
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Globalization;
using System.Net;

namespace Microsoft.VisualStudio.Services.Common
{
  public sealed class VssBasicToken : IssuedToken
  {
    private readonly ICredentials m_credentials;

    public VssBasicToken(ICredentials credentials) => this.m_credentials = credentials;

    internal ICredentials Credentials => this.m_credentials;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.Basic;

    internal override void ApplyTo(IHttpRequest request)
    {
      NetworkCredential credential = this.m_credentials.GetCredential(request.RequestUri, "Basic");
      if (credential == null)
        return;
      request.Headers.SetValue("Authorization", "Basic " + VssBasicToken.FormatBasicAuthHeader(credential));
    }

    private static string FormatBasicAuthHeader(NetworkCredential credential)
    {
      string empty = string.Empty;
      return Convert.ToBase64String(VssHttpRequestSettings.Encoding.GetBytes(string.IsNullOrEmpty(credential.Domain) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) credential.UserName, (object) credential.Password) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}:{2}", (object) credential.Domain, (object) credential.UserName, (object) credential.Password)));
    }
  }
}
