// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider.CredentialResponse
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider
{
  internal class CredentialResponse
  {
    public readonly ICredentials Credentials;
    public readonly RequestStatus Status;

    public CredentialResponse(RequestStatus status)
      : this((ICredentials) null, status)
    {
    }

    public CredentialResponse(ICredentials credentials, RequestStatus status)
    {
      this.Credentials = (credentials == null || status != RequestStatus.ProviderNotApplicable) && (credentials != null || status != RequestStatus.Success) ? credentials : throw new ArgumentException("Invalid argument combination between 'credentials' and 'status'");
      this.Status = status;
    }
  }
}
