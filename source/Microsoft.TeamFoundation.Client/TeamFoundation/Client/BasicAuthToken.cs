// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BasicAuthToken
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.VssBasicToken instead.", false)]
  public sealed class BasicAuthToken : IssuedToken
  {
    private ICredentials m_credentials;

    public BasicAuthToken(ICredentials credentials) => this.m_credentials = credentials;

    internal ICredentials Credentials => this.m_credentials;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.Basic;

    internal override void ApplyTo(HttpWebRequest webRequest)
    {
      NetworkCredential credential = this.m_credentials.GetCredential(webRequest.RequestUri, "Basic");
      if (credential == null)
        return;
      webRequest.Headers.Remove(HttpRequestHeader.Authorization);
      webRequest.Headers.Add(HttpRequestHeader.Authorization, BasicAuthToken.FormatBasicAuthHeader(credential));
      webRequest.ServicePoint.Expect100Continue = false;
    }

    private static string FormatBasicAuthHeader(NetworkCredential credential)
    {
      string empty = string.Empty;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Basic {0}", (object) Convert.ToBase64String(TfsRequestSettings.RequestEncoding.GetBytes(string.IsNullOrEmpty(credential.Domain) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) credential.UserName, (object) credential.Password) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}:{2}", (object) credential.Domain, (object) credential.UserName, (object) credential.Password))));
    }
  }
}
