// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IssuedTokenCredential
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.IssuedTokenCredential instead.", false)]
  [Serializable]
  public abstract class IssuedTokenCredential
  {
    [NonSerialized]
    private SynchronizationContext m_syncContext;
    [NonSerialized]
    private TfsClientCredentialStorage m_tokenStorage;

    internal IssuedTokenCredential()
    {
    }

    internal IssuedTokenCredential(IssuedToken initialToken) => this.InitialToken = initialToken;

    internal IssuedToken InitialToken { get; set; }

    internal SynchronizationContext SyncContext
    {
      get => this.m_syncContext;
      set => this.m_syncContext = value;
    }

    protected abstract VssCredentialsType CredentialType { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsClientCredentialStorage TokenStorage
    {
      get => this.m_tokenStorage;
      set => this.m_tokenStorage = value;
    }

    internal IssuedTokenProvider CreateTokenProvider(
      Uri serverUrl,
      HttpWebResponse response,
      IssuedToken failedToken)
    {
      IssuedTokenProvider tokenProvider = response == null || this.IsAuthenticationChallenge(response) ? this.OnCreateTokenProvider(serverUrl, response) : throw new InvalidOperationException();
      if (tokenProvider != null && this.InitialToken != null && this.InitialToken != failedToken)
        tokenProvider.CurrentToken = this.InitialToken;
      return tokenProvider;
    }

    internal abstract bool IsAuthenticationChallenge(HttpWebResponse webResponse);

    internal abstract IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      HttpWebResponse response);
  }
}
