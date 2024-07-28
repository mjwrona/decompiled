// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountMembershipEvent
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Account
{
  public class AccountMembershipEvent
  {
    public AccountMembershipEvent(Guid accountId, Guid memberId, AccountMembershipEventKind kind)
    {
      this.AccountId = accountId;
      this.AccountMemberId = memberId;
      this.Kind = kind;
    }

    public Guid AccountId { get; private set; }

    public Guid AccountMemberId { get; private set; }

    public AccountMembershipEventKind Kind { get; private set; }
  }
}
