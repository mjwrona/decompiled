// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAadToken
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Client
{
  [Serializable]
  public class VssAadToken : IssuedToken
  {
    private readonly bool allowDialog;
    private readonly string accessToken;
    private readonly string accessTokenType;
    private readonly IPublicClientApplication app;
    private readonly VssAadCredential userCredential;
    private readonly Func<string[], AuthenticationResult> tokenAcquisitionDelegate;

    public VssAadToken(string accessTokenType, string accessToken)
    {
      this.FromStorage = true;
      if (!string.IsNullOrWhiteSpace(accessToken) && !string.IsNullOrWhiteSpace(accessTokenType))
        this.Authenticated();
      this.accessToken = accessToken;
      this.accessTokenType = accessTokenType;
    }

    public VssAadToken(
      PublicClientApplicationBuilder clientBuilder,
      VssAadCredential userCredential = null,
      bool allowDialog = false)
    {
      this.FromStorage = true;
      HttpClientFactoryWithUserAgent factoryWithUserAgent = new HttpClientFactoryWithUserAgent();
      this.app = clientBuilder.WithHttpClientFactory((IMsalHttpClientFactory) factoryWithUserAgent).Build();
      this.userCredential = userCredential;
      this.allowDialog = allowDialog;
    }

    public VssAadToken(
      Func<string[], AuthenticationResult> tokenAcquisitionDelegate)
    {
      this.FromStorage = true;
      this.tokenAcquisitionDelegate = tokenAcquisitionDelegate;
    }

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.Aad;

    public AuthenticationResult AcquireToken(string[] scopes = null)
    {
      if (this.app == null && this.tokenAcquisitionDelegate == null)
        return (AuthenticationResult) null;
      if (scopes == null)
        scopes = VssAadSettings.DefaultScopes;
      AuthenticationResult authenticationResult = (AuthenticationResult) null;
      for (int index = 0; index < 3; ++index)
      {
        try
        {
          if (this.tokenAcquisitionDelegate != null)
            authenticationResult = this.tokenAcquisitionDelegate(scopes);
          else if (this.userCredential == null && !this.allowDialog)
          {
            List<IAccount> list = this.app.GetAccountsAsync().SyncResultConfigured<IEnumerable<IAccount>>().ToList<IAccount>();
            bool flag = true;
            authenticationResult = !(list.Count == 0 & flag) ? this.app.AcquireTokenSilent((IEnumerable<string>) scopes, list.FirstOrDefault<IAccount>()).ExecuteAsync().SyncResultConfigured<AuthenticationResult>() : this.app.AcquireTokenSilent((IEnumerable<string>) scopes, PublicClientApplication.OperatingSystemAccount).ExecuteAsync().SyncResultConfigured<AuthenticationResult>();
          }
          else
            authenticationResult = this.app.AcquireTokenByUsernamePassword((IEnumerable<string>) scopes, this.userCredential?.Username, this.userCredential?.Password).ExecuteAsync().SyncResultConfigured<AuthenticationResult>();
          if (authenticationResult != null)
            break;
        }
        catch (Exception ex)
        {
        }
      }
      return authenticationResult;
    }

    internal override void ApplyTo(IHttpRequest request)
    {
      AuthenticationResult authenticationResult = this.AcquireToken();
      if (authenticationResult != null)
      {
        request.Headers.SetValue("Authorization", authenticationResult.CreateAuthorizationHeader() ?? "");
      }
      else
      {
        if (string.IsNullOrEmpty(this.accessTokenType) || string.IsNullOrEmpty(this.accessToken))
          return;
        request.Headers.SetValue("Authorization", this.accessTokenType + " " + this.accessToken);
      }
    }
  }
}
