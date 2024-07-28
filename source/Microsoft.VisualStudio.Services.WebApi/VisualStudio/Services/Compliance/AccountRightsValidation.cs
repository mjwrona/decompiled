// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Compliance.AccountRightsValidation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Licensing;
using System;

namespace Microsoft.VisualStudio.Services.Compliance
{
  [Obsolete("This type is no longer used.")]
  public class AccountRightsValidation
  {
    public AccountRightsValidation()
    {
    }

    public AccountRightsValidation(
      ComplianceValidation validation,
      VisualStudioOnlineServiceLevel accountRights)
      : this(validation, accountRights, string.Empty)
    {
    }

    public AccountRightsValidation(
      ComplianceValidation validation,
      VisualStudioOnlineServiceLevel accountRights,
      string accountRightsReason)
      : this(validation, accountRights, accountRightsReason, (AccountEntitlement) null)
    {
    }

    public AccountRightsValidation(
      ComplianceValidation validation,
      VisualStudioOnlineServiceLevel accountRights,
      string accountRightsReason,
      AccountEntitlement accountEntitlement)
    {
      this.AccountRights = accountRights;
      this.ComplianceValidation = validation;
      this.AccountRightsReason = accountRightsReason;
      this.AccountEntitlement = accountEntitlement;
    }

    public AccountEntitlement AccountEntitlement { get; set; }

    public VisualStudioOnlineServiceLevel AccountRights { get; set; }

    public string AccountRightsReason { get; set; }

    public ComplianceValidation ComplianceValidation { get; set; }

    public bool Volatile { get; set; }
  }
}
