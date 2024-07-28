// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.OAuthTokenProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.WebApi.VssOAuthTokenProvider instead.", false)]
  internal sealed class OAuthTokenProvider : IssuedTokenProvider
  {
    public OAuthTokenProvider(OAuthTokenCredential credential, Uri serverUrl, Uri signInUrl)
      : base((IssuedTokenCredential) credential, serverUrl, signInUrl)
    {
    }

    public OAuthTokenCredential Credential => (OAuthTokenCredential) base.Credential;

    public override bool GetTokenIsInteractive => false;

    protected override IssuedToken OnGetToken(IssuedToken failedToken, TimeSpan timeout)
    {
      if (string.IsNullOrEmpty(this.Credential.ClientId) || string.IsNullOrEmpty(this.Credential.ClientSecret))
        return (IssuedToken) null;
      if (!(failedToken is OAuthTokenContainer oauthTokenContainer) || oauthTokenContainer.RefreshToken == null)
        return (IssuedToken) null;
      WebClient webClient = new WebClient();
      webClient.BaseAddress = this.SignInUrl.AbsoluteUri;
      webClient.Encoding = TfsRequestSettings.RequestEncoding;
      webClient.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(this.Credential.ClientId + ":" + this.Credential.ClientSecret)));
      NameValueCollection data = new NameValueCollection();
      if (oauthTokenContainer.RefreshToken.TokenType == OAuthTokenType.AuthenticationCode)
      {
        data.Add("grant_type", "authorization_code");
        data.Add("code", oauthTokenContainer.RefreshToken.Token);
      }
      else if (oauthTokenContainer.RefreshToken.TokenType == OAuthTokenType.RefreshToken)
      {
        data.Add("grant_type", "refresh_token");
        data.Add("refresh_token", oauthTokenContainer.RefreshToken.Token);
      }
      OAuthTokenContainer tokens = OAuthTokenContainer.ExtractTokens(webClient.UploadValues(new Uri("_oauth/accesstoken/", UriKind.Relative), "POST", data), TfsRequestSettings.RequestEncoding);
      if (this.Credential.TokensReceived != null)
        this.Credential.TokensReceived(tokens);
      return (IssuedToken) tokens;
    }

    protected override IAsyncResult OnBeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return !string.IsNullOrEmpty(this.Credential.ClientId) && !string.IsNullOrEmpty(this.Credential.ClientSecret) && failedToken is OAuthTokenContainer ? (IAsyncResult) new OAuthTokenProvider.GetTokenOperation(this, (OAuthTokenContainer) failedToken, timeout, callback, state) : (IAsyncResult) new CompletedOperation<IssuedToken>((IssuedToken) null, callback, state);
    }

    protected override IssuedToken OnEndGetToken(IAsyncResult result) => result is OAuthTokenProvider.GetTokenOperation ? (IssuedToken) OAuthTokenProvider.GetTokenOperation.End(result) : CompletedOperation<IssuedToken>.End(result);

    private new sealed class GetTokenOperation : AsyncOperation
    {
      private WebClient m_client;
      private OAuthTokenContainer m_failedToken;
      private OAuthTokenContainer m_token;
      private OAuthTokenProvider m_tokenProvider;

      public GetTokenOperation(
        OAuthTokenProvider tokenProvider,
        OAuthTokenContainer failedToken,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_tokenProvider = tokenProvider;
        this.m_failedToken = failedToken;
        this.Begin();
      }

      private void Begin()
      {
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          if (this.m_failedToken.RefreshToken != null)
          {
            this.m_client = new WebClient();
            this.m_client.BaseAddress = this.m_tokenProvider.SignInUrl.AbsoluteUri;
            this.m_client.Encoding = TfsRequestSettings.RequestEncoding;
            this.m_client.UseDefaultCredentials = false;
            this.m_client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(this.m_tokenProvider.Credential.ClientId + ":" + this.m_tokenProvider.Credential.ClientSecret)));
            NameValueCollection data = new NameValueCollection();
            if (this.m_failedToken.RefreshToken.TokenType == OAuthTokenType.AuthenticationCode)
            {
              data.Add("grant_type", "authorization_code");
              data.Add("code", this.m_failedToken.RefreshToken.Token);
            }
            else if (this.m_failedToken.RefreshToken.TokenType == OAuthTokenType.RefreshToken)
            {
              data.Add("grant_type", "refresh_token");
              data.Add("refresh_token", this.m_failedToken.RefreshToken.Token);
            }
            this.m_client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(OAuthTokenProvider.GetTokenOperation.UploadValuesCompletedCallback);
            this.m_client.UploadValuesAsync(new Uri("_oauth/accesstoken/", UriKind.Relative), "POST", data, (object) this);
            flag = false;
          }
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        finally
        {
          if (flag && this.m_client != null)
          {
            this.m_client.UploadValuesCompleted -= new UploadValuesCompletedEventHandler(OAuthTokenProvider.GetTokenOperation.UploadValuesCompletedCallback);
            this.m_client.Dispose();
          }
        }
        if (!flag)
          return;
        this.Complete(true, exception);
      }

      private static void UploadValuesCompletedCallback(
        object sender,
        UploadValuesCompletedEventArgs e)
      {
        OAuthTokenProvider.GetTokenOperation userState = (OAuthTokenProvider.GetTokenOperation) e.UserState;
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          flag = userState.CompleteUploadValues(e);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        finally
        {
          if (flag && userState.m_client != null)
          {
            userState.m_client.UploadValuesCompleted -= new UploadValuesCompletedEventHandler(OAuthTokenProvider.GetTokenOperation.UploadValuesCompletedCallback);
            userState.m_client.Dispose();
          }
        }
        if (!flag)
          return;
        userState.Complete(false, exception);
      }

      private bool CompleteUploadValues(UploadValuesCompletedEventArgs e)
      {
        if (e.Error != null)
          throw e.Error;
        this.m_token = OAuthTokenContainer.ExtractTokens(e.Result, TfsRequestSettings.RequestEncoding);
        if (this.m_tokenProvider.Credential.TokensReceived != null)
          this.m_tokenProvider.Credential.TokensReceived(this.m_token);
        return true;
      }

      public static OAuthTokenContainer End(IAsyncResult result) => AsyncOperation.End<OAuthTokenProvider.GetTokenOperation>(result).m_token;
    }
  }
}
