// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.WindowsCredential
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Common
{
  public sealed class WindowsCredential : IssuedTokenCredential
  {
    private ICredentials m_credentials;

    public WindowsCredential()
      : this(true)
    {
    }

    public WindowsCredential(bool useDefaultCredentials)
      : this(useDefaultCredentials ? CredentialCache.DefaultCredentials : (ICredentials) null)
    {
      this.UseDefaultCredentials = useDefaultCredentials;
    }

    public WindowsCredential(ICredentials credentials)
      : this((WindowsToken) null)
    {
      this.m_credentials = credentials;
      this.UseDefaultCredentials = credentials == CredentialCache.DefaultCredentials;
    }

    public WindowsCredential(WindowsToken initialToken)
      : base((IssuedToken) initialToken)
    {
    }

    public ICredentials Credentials
    {
      get => this.m_credentials;
      set
      {
        this.m_credentials = value;
        this.UseDefaultCredentials = this.Credentials == CredentialCache.DefaultCredentials;
      }
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.Windows;

    public bool UseDefaultCredentials { get; private set; }

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse) => webResponse != null && (webResponse.StatusCode == HttpStatusCode.Unauthorized && webResponse.Headers.GetValues("WWW-Authenticate").Any<string>((Func<string, bool>) (x => WindowsCredential.AuthenticationSchemeValid(x))) || webResponse.StatusCode == HttpStatusCode.ProxyAuthenticationRequired && webResponse.Headers.GetValues("Proxy-Authenticate").Any<string>((Func<string, bool>) (x => WindowsCredential.AuthenticationSchemeValid(x))));

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      if (response == null)
        return (IssuedTokenProvider) null;
      if (this.m_credentials != null)
        this.InitialToken = (IssuedToken) new WindowsToken(this.m_credentials);
      return (IssuedTokenProvider) new WindowsTokenProvider(this, serverUrl);
    }

    private static bool AuthenticationSchemeValid(string authenticateHeader) => authenticateHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase) || authenticateHeader.StartsWith("Digest", StringComparison.OrdinalIgnoreCase) || authenticateHeader.StartsWith("Negotiate", StringComparison.OrdinalIgnoreCase) || authenticateHeader.StartsWith("Ntlm", StringComparison.OrdinalIgnoreCase);
  }
}
