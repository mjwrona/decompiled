// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WindowsToken
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.WindowsToken instead.", false)]
  public sealed class WindowsToken : IssuedToken, ICredentials
  {
    internal WindowsToken(ICredentials credentials) => this.Credentials = credentials;

    public ICredentials Credentials { get; private set; }

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.Windows;

    internal override void ApplyTo(HttpWebRequest webRequest)
    {
      if (this.Credentials == null)
        return;
      webRequest.Credentials = (ICredentials) this;
      webRequest.ConnectionGroupName = TfsHttpRequestHelpers.GetConnectionGroupName(webRequest.RequestUri, this.Credentials);
      webRequest.UnsafeAuthenticatedConnectionSharing = true;
    }

    NetworkCredential ICredentials.GetCredential(Uri uri, string authType) => this.Credentials == null ? (NetworkCredential) null : this.Credentials.GetCredential(uri, authType);
  }
}
