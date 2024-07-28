// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WindowsTokenProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.WindowsTokenProvider instead.", false)]
  internal sealed class WindowsTokenProvider : IssuedTokenProvider
  {
    public WindowsTokenProvider(WindowsCredential credential, Uri serverUrl)
      : base((IssuedTokenCredential) credential, serverUrl, serverUrl)
    {
    }

    public WindowsCredential Credential => (WindowsCredential) base.Credential;

    public override bool GetTokenIsInteractive => this.Credential.Provider is IUICredentialsProvider && this.CurrentToken == null;

    protected override IssuedToken OnGetToken(IssuedToken failedToken, TimeSpan timeout)
    {
      ICredentials failedCredentials = (ICredentials) null;
      if (failedToken != null && failedToken is WindowsToken)
        failedCredentials = ((WindowsToken) failedToken).Credentials;
      ICredentials credentials = (ICredentials) null;
      if (this.Credential.Provider != null)
      {
        credentials = this.Credential.Provider.GetCredentials(this.SignInUrl, failedCredentials);
        if (credentials != null)
          this.Credential.Credentials = credentials;
      }
      return credentials == null ? (IssuedToken) null : (IssuedToken) new WindowsToken(credentials);
    }

    protected override IAsyncResult OnBeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new CompletedOperation(callback, state);
    }

    protected override IssuedToken OnEndGetToken(IAsyncResult result)
    {
      CompletedOperation.End(result);
      return (IssuedToken) null;
    }
  }
}
