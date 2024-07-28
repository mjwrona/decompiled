// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountEntitlementEvent
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class AccountEntitlementEvent
  {
    public AccountEntitlementEvent(
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      License accountLicense,
      AccountEntitlementEventKind kind,
      Guid userId)
    {
      this.AccountId = accountId;
      this.Identity = identity;
      this.UserId = userId;
      this.AccountLicense = accountLicense;
      this.Kind = kind;
    }

    public Guid AccountId { get; private set; }

    public Guid UserId { get; private set; }

    public AccountEntitlementEventKind Kind { get; private set; }

    public License AccountLicense { get; private set; }

    public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; private set; }
  }
}
