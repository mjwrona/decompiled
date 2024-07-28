// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.Authorization
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class Authorization
  {
    public Guid AuthorizationId { get; set; }

    public Uri RedirectUri { get; set; }

    public Guid IdentityId { get; set; }

    public string Scopes { get; set; }

    public DateTimeOffset ValidFrom { get; set; }

    public DateTimeOffset ValidTo { get; set; }

    public DateTimeOffset AccessIssued { get; set; }

    public bool IsAccessUsed { get; set; }

    public bool IsValid { get; set; }

    public Guid RegistrationId { get; set; }

    public string Audience { get; set; }

    public string Source { get; set; }
  }
}
