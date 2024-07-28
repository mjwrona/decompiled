// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssBasicCredential
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Common
{
  public sealed class VssBasicCredential : FederatedCredential
  {
    public VssBasicCredential()
      : this((VssBasicToken) null)
    {
    }

    public VssBasicCredential(string userName, string password)
      : this(new VssBasicToken((ICredentials) new NetworkCredential(userName, password)))
    {
    }

    public VssBasicCredential(ICredentials initialToken)
      : this(new VssBasicToken(initialToken))
    {
    }

    public VssBasicCredential(VssBasicToken initialToken)
      : base((IssuedToken) initialToken)
    {
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.Basic;

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse) => webResponse != null && (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Unauthorized) && webResponse.Headers.GetValues("WWW-Authenticate").Any<string>((Func<string, bool>) (x => x.StartsWith("Basic", StringComparison.OrdinalIgnoreCase)));

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      bool result;
      if (serverUrl.Scheme != "https" && (!bool.TryParse(Environment.GetEnvironmentVariable("VSS_ALLOW_UNSAFE_BASICAUTH") ?? "false", out result) || !result))
        throw new InvalidOperationException(CommonResources.BasicAuthenticationRequiresSsl());
      return (IssuedTokenProvider) new BasicAuthTokenProvider(this, serverUrl);
    }
  }
}
