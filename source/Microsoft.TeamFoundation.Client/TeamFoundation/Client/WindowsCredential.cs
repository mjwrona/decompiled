// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WindowsCredential
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.WindowsCredential instead.", false)]
  public sealed class WindowsCredential : IssuedTokenCredential
  {
    public WindowsCredential()
      : this(true)
    {
    }

    public WindowsCredential(bool useDefaultCredentials)
      : this(useDefaultCredentials ? CredentialCache.DefaultCredentials : (ICredentials) null)
    {
    }

    public WindowsCredential(ICredentials credentials)
      : this(credentials, Environment.UserInteractive ? (ICredentialsProvider) new UICredentialsProvider() : (ICredentialsProvider) null)
    {
    }

    public WindowsCredential(ICredentials credentials, ICredentialsProvider credentialsProvider)
    {
      this.Credentials = credentials;
      this.Provider = credentialsProvider;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ICredentials Credentials { get; set; }

    internal ICredentialsProvider Provider { get; set; }

    protected override VssCredentialsType CredentialType => VssCredentialsType.Windows;

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (webResponse == null)
        return true;
      return webResponse.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(webResponse.Headers[HttpResponseHeader.WwwAuthenticate]);
    }

    internal override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      HttpWebResponse response)
    {
      if (this.Credentials != null)
        this.InitialToken = (IssuedToken) new WindowsToken(this.Credentials);
      return (IssuedTokenProvider) new WindowsTokenProvider(this, serverUrl);
    }
  }
}
