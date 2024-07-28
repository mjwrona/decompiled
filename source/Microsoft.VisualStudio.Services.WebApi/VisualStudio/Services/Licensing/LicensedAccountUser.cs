// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensedAccountUser
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class LicensedAccountUser
  {
    public LicensedAccountUser(Guid accountId, Guid userId)
    {
      this.AccountId = accountId;
      this.UserId = userId;
      this.AvailableLicenses = new List<AccountUserLicense>();
    }

    public LicensedAccountUser(
      Guid accountId,
      Guid userId,
      List<AccountUserLicense> availableRights)
    {
      this.AccountId = accountId;
      this.UserId = userId;
      this.AvailableLicenses = availableRights;
    }

    public Guid AccountId { get; set; }

    public Guid UserId { get; set; }

    public List<AccountUserLicense> AvailableLicenses { get; set; }
  }
}
