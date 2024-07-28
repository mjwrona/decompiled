// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BasicAuthCredential
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.VssBasicCredential instead.", false)]
  public sealed class BasicAuthCredential : FederatedCredential
  {
    public BasicAuthCredential()
      : this((BasicAuthToken) null)
    {
    }

    public BasicAuthCredential(ICredentials initialToken)
      : this(new BasicAuthToken(initialToken))
    {
    }

    public BasicAuthCredential(BasicAuthToken initialToken)
      : base((IssuedToken) initialToken)
    {
    }

    protected override VssCredentialsType CredentialType => VssCredentialsType.Basic;

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (webResponse == null || webResponse.StatusCode != HttpStatusCode.Found && webResponse.StatusCode != HttpStatusCode.Found && webResponse.StatusCode != HttpStatusCode.Unauthorized)
        return false;
      string header = webResponse.Headers[HttpResponseHeader.WwwAuthenticate];
      return !string.IsNullOrEmpty(header) && header.IndexOf("Basic", 0, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    internal override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      HttpWebResponse response)
    {
      if (serverUrl.Scheme != Uri.UriSchemeHttps)
        throw new InvalidOperationException(ClientResources.BasicAuthenticationRequiresSsl());
      return (IssuedTokenProvider) new BasicAuthTokenProvider(this, serverUrl);
    }
  }
}
