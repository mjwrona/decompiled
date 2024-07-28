// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.IssuedTokenCredential
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  [Serializable]
  public abstract class IssuedTokenCredential
  {
    [NonSerialized]
    private TaskScheduler m_scheduler;
    [NonSerialized]
    private IVssCredentialPrompt m_prompt;
    [NonSerialized]
    private IVssCredentialStorage m_storage;

    protected IssuedTokenCredential(IssuedToken initialToken) => this.InitialToken = initialToken;

    public abstract VssCredentialsType CredentialType { get; }

    internal IssuedToken InitialToken { get; set; }

    internal TaskScheduler Scheduler
    {
      get => this.m_scheduler;
      set => this.m_scheduler = value;
    }

    internal IVssCredentialPrompt Prompt
    {
      get => this.m_prompt;
      set => this.m_prompt = value;
    }

    internal IVssCredentialStorage Storage
    {
      get => this.m_storage;
      set => this.m_storage = value;
    }

    internal Uri TokenStorageUrl { get; set; }

    internal IssuedTokenProvider CreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response,
      IssuedToken failedToken)
    {
      if (response != null && !this.IsAuthenticationChallenge(response))
        throw new InvalidOperationException();
      if (this.InitialToken == null && this.Storage != null)
      {
        if (this.TokenStorageUrl == (Uri) null)
          throw new InvalidOperationException("The TokenStorageUrl property must have a value if the Storage property is set on this instance of " + this.GetType().Name + ".");
        this.InitialToken = this.Storage.RetrieveToken(this.TokenStorageUrl, this.CredentialType);
      }
      IssuedTokenProvider tokenProvider = this.OnCreateTokenProvider(serverUrl, response);
      if (tokenProvider != null)
        tokenProvider.TokenStorageUrl = this.TokenStorageUrl;
      if (tokenProvider != null && this.InitialToken != null && this.InitialToken != failedToken)
        tokenProvider.CurrentToken = this.InitialToken;
      return tokenProvider;
    }

    internal virtual string GetAuthenticationChallenge(IHttpResponse webResponse)
    {
      IEnumerable<string> values;
      return !webResponse.Headers.TryGetValues("WWW-Authenticate", out values) ? string.Empty : string.Join(", ", values);
    }

    public abstract bool IsAuthenticationChallenge(IHttpResponse webResponse);

    protected abstract IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response);
  }
}
