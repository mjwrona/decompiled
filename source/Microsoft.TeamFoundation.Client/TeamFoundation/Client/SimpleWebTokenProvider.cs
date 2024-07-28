// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.SimpleWebTokenProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Specialized;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.VssServiceIdentityTokenProvider instead.", false)]
  internal sealed class SimpleWebTokenProvider : IssuedTokenProvider
  {
    public SimpleWebTokenProvider(
      SimpleWebTokenCredential credential,
      Uri serverUrl,
      Uri signInUrl,
      string realm)
      : base((IssuedTokenCredential) credential, serverUrl, signInUrl)
    {
      this.Realm = realm;
    }

    public SimpleWebTokenCredential Credential => (SimpleWebTokenCredential) base.Credential;

    public override bool GetTokenIsInteractive => false;

    public string Realm { get; private set; }

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (!base.IsAuthenticationChallenge(webResponse) || this.SignInUrl == (Uri) null)
        return false;
      string header = webResponse.Headers["X-TFS-FedAuthRealm"];
      string leftPart = new Uri(webResponse.Headers["X-TFS-FedAuthIssuer"]).GetLeftPart(UriPartial.Authority);
      return this.Realm.Equals(header, StringComparison.OrdinalIgnoreCase) && this.SignInUrl.AbsoluteUri.Equals(leftPart, StringComparison.OrdinalIgnoreCase);
    }

    protected override IssuedToken OnGetToken(IssuedToken failedToken, TimeSpan timeout)
    {
      try
      {
        TeamFoundationTrace.Enter(TraceKeywordSets.Authentication, "SimpleWebTokenProvider.OnGetToken");
        if (string.IsNullOrEmpty(this.Credential.UserName) || string.IsNullOrEmpty(this.Credential.Password))
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Username or Password is null. Returning null.");
          return (IssuedToken) null;
        }
        AcsRetryHelper acsRetryHelper = new AcsRetryHelper(3);
        using (WebClient client = new WebClient())
        {
          try
          {
            client.BaseAddress = this.SignInUrl.AbsoluteUri;
            client.Encoding = TfsRequestSettings.RequestEncoding;
            return (IssuedToken) SimpleWebToken.ExtractToken(acsRetryHelper.Invoke<byte[]>((Func<byte[]>) (() => client.UploadValues(new Uri("WRAPv0.9/", UriKind.Relative), "POST", new NameValueCollection()
            {
              {
                "wrap_name",
                this.Credential.UserName
              },
              {
                "wrap_password",
                this.Credential.Password
              },
              {
                "wrap_scope",
                this.Realm
              }
            }))), TfsRequestSettings.RequestEncoding);
          }
          catch (Exception ex)
          {
            TeamFoundationTrace.TraceException(ex);
            throw;
          }
        }
      }
      finally
      {
        TeamFoundationTrace.Exit(TraceKeywordSets.Authentication, "SimpleWebTokenProvider.OnGetToken");
      }
    }

    protected override IAsyncResult OnBeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      try
      {
        TeamFoundationTrace.Enter(TraceKeywordSets.Authentication, "SimpleWebTokenProvider.OnBeginGetToken");
        if (!string.IsNullOrEmpty(this.Credential.UserName) && !string.IsNullOrEmpty(this.Credential.Password))
          return (IAsyncResult) new SimpleWebTokenProvider.GetTokenOperation(this, timeout, callback, state);
        TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Username or Password is null. Returning null.");
        return (IAsyncResult) new CompletedOperation<IssuedToken>((IssuedToken) null, callback, state);
      }
      finally
      {
        TeamFoundationTrace.Exit(TraceKeywordSets.Authentication, "SimpleWebTokenProvider.OnBeginGetToken");
      }
    }

    protected override IssuedToken OnEndGetToken(IAsyncResult result) => result is SimpleWebTokenProvider.GetTokenOperation ? (IssuedToken) SimpleWebTokenProvider.GetTokenOperation.End(result) : CompletedOperation<IssuedToken>.End(result);

    private new sealed class GetTokenOperation : AsyncOperation
    {
      private WebClient m_client;
      private SimpleWebToken m_token;
      private SimpleWebTokenProvider m_tokenProvider;

      public GetTokenOperation(
        SimpleWebTokenProvider tokenProvider,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_tokenProvider = tokenProvider;
        this.Begin();
      }

      private void Begin()
      {
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          this.m_client = new WebClient();
          this.m_client.BaseAddress = this.m_tokenProvider.SignInUrl.AbsoluteUri;
          this.m_client.Encoding = TfsRequestSettings.RequestEncoding;
          this.m_client.UseDefaultCredentials = false;
          NameValueCollection data = new NameValueCollection();
          data.Add("wrap_name", this.m_tokenProvider.Credential.UserName);
          data.Add("wrap_password", this.m_tokenProvider.Credential.Password);
          data.Add("wrap_scope", this.m_tokenProvider.Realm);
          TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Refreshing the token using URL={0}, Name={1}, Realm={2}", (object) this.m_client.BaseAddress, (object) this.m_tokenProvider.Credential.UserName, (object) this.m_tokenProvider.Realm);
          this.m_client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(SimpleWebTokenProvider.GetTokenOperation.UploadValuesCompletedCallback);
          this.m_client.UploadValuesAsync(new Uri("WRAPv0.9/", UriKind.Relative), "POST", data, (object) this);
          flag = false;
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
          exception = ex;
        }
        finally
        {
          if (flag && this.m_client != null)
          {
            this.m_client.UploadValuesCompleted -= new UploadValuesCompletedEventHandler(SimpleWebTokenProvider.GetTokenOperation.UploadValuesCompletedCallback);
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
        SimpleWebTokenProvider.GetTokenOperation userState = (SimpleWebTokenProvider.GetTokenOperation) e.UserState;
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
            userState.m_client.UploadValuesCompleted -= new UploadValuesCompletedEventHandler(SimpleWebTokenProvider.GetTokenOperation.UploadValuesCompletedCallback);
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
        {
          TeamFoundationTrace.Error(TraceKeywordSets.Authentication, "Exception caught when refreshing token: {0}", (object) e.Error.ToString());
          throw e.Error;
        }
        this.m_token = SimpleWebToken.ExtractToken(e.Result, TfsRequestSettings.RequestEncoding);
        TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Successfully retrieved token: '{0}'", (object) this.m_token);
        return true;
      }

      public static SimpleWebToken End(IAsyncResult result) => AsyncOperation.End<SimpleWebTokenProvider.GetTokenOperation>(result).m_token;
    }
  }
}
