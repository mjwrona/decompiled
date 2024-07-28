// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountEntitlementClaim
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [JsonObject(MemberSerialization.OptIn)]
  internal class AccountEntitlementClaim : ILicenseClaim
  {
    public AccountEntitlementClaim(
      AccountEntitlement accountEntitlement,
      DateTimeOffset validationDate)
    {
      this.AccountEntitlement = accountEntitlement;
      this.ValidationDate = validationDate;
    }

    [JsonProperty]
    public AccountEntitlement AccountEntitlement { get; private set; }

    public string ClaimValue => this.AccountEntitlement.Serialize<AccountEntitlement>(true);

    [JsonProperty]
    public DateTimeOffset ValidationDate { get; private set; }
  }
}
