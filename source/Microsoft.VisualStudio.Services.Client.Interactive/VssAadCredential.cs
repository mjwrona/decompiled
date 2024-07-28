// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAadCredential
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Client
{
  [Serializable]
  public sealed class VssAadCredential : FederatedCredential
  {
    public VssAadCredential(VssAadToken initialToken)
      : base((IssuedToken) initialToken)
    {
    }

    public VssAadCredential(string username = null, string password = null)
      : base((IssuedToken) null)
    {
      this.Username = username;
      this.Password = password;
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.Aad;

    public string Username { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Password { get; }

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse) => VssFederatedCredential.IsVssFederatedAuthenticationChallenge(webResponse, out bool _).GetValueOrDefault();

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      return response == null && this.InitialToken == null ? (IssuedTokenProvider) null : (IssuedTokenProvider) new VssAadTokenProvider(this);
    }
  }
}
