// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.HostAuthorizationDecision
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class HostAuthorizationDecision
  {
    public HostAuthorizationError HostAuthorizationError { get; set; }

    public Guid HostAuthorizationId { get; set; }

    public Guid RegistrationId { get; set; }

    public Uri SetupUri { get; set; }

    public string RegistrationName { get; set; }

    public ClientType ClientType { get; set; }

    public bool HasError => this.HostAuthorizationError != 0;
  }
}
