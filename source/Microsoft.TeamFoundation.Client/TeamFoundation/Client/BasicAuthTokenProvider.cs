// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BasicAuthTokenProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.VssBasicTokenProvider instead.", false)]
  internal sealed class BasicAuthTokenProvider : IssuedTokenProvider
  {
    private IUICredentialsProvider m_provider;

    public BasicAuthTokenProvider(BasicAuthCredential credential, Uri serverUrl)
      : base((IssuedTokenCredential) credential, serverUrl, serverUrl)
    {
      this.m_provider = (IUICredentialsProvider) new UICredentialsProvider(CachedCredentialsType.Basic);
    }

    public BasicAuthCredential Credential => (BasicAuthCredential) base.Credential;

    public override bool GetTokenIsInteractive => this.CurrentToken == null;

    protected override IssuedToken OnGetToken(IssuedToken failedToken, TimeSpan timeout)
    {
      ICredentials failedCredentials = (ICredentials) null;
      if (failedToken != null && failedToken is BasicAuthToken)
        failedCredentials = ((BasicAuthToken) failedToken).Credentials;
      ICredentials credentials = this.m_provider.GetCredentials(this.SignInUrl, failedCredentials);
      return credentials == null ? (IssuedToken) null : (IssuedToken) new BasicAuthToken(credentials);
    }

    protected override IAsyncResult OnBeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new CompletedOperation<IssuedToken>(this.OnGetToken(failedToken, timeout), callback, state);
    }

    protected override IssuedToken OnEndGetToken(IAsyncResult result) => CompletedOperation<IssuedToken>.End(result);
  }
}
