// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UserIdentityViewModelBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public abstract class UserIdentityViewModelBase : IdentityViewModelBase
  {
    protected UserIdentityViewModelBase()
    {
    }

    protected UserIdentityViewModelBase(TeamFoundationIdentity identity, bool loadAadTenantData = false)
      : base(identity)
    {
      this.DisplayName = identity.DisplayName;
      this.IsAcsAccount = identity.Descriptor.IdentityType.Equals("Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase) || identity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase);
      this.AccountName = identity.GetAttribute("Account", string.Empty);
      this.Domain = identity.GetAttribute(nameof (Domain), string.Empty);
      this.IdentityMetaType = identity.MetaType;
      if (!loadAadTenantData)
        return;
      TfsWebContext tfsWebContext = (TfsWebContext) null;
      if (!TfsHelpers.TryGetTfsWebContext(out tfsWebContext) || tfsWebContext == null || !tfsWebContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || !AadIdentityHelper.IsAadUser(identity.Descriptor))
        return;
      this.LoadAadTenantData(tfsWebContext.TfsRequestContext);
    }

    public string AccountName { get; private set; }

    public string Domain { get; private set; }

    public override string FriendlyDisplayName => this.DisplayName;

    public string FriendlyMetaType => this.IdentityMetaType == IdentityMetaType.ManagedIdentity ? "managed identity" : this.IdentityMetaType.ToString().ToLowerInvariant();

    public IdentityMetaType IdentityMetaType { get; private set; }

    public abstract override string IdentityType { get; }

    public bool IsAcsAccount { get; private set; }

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["Domain"] = (object) this.Domain;
      json["AccountName"] = (object) this.AccountName;
      json["IsWindowsUser"] = (object) this.IsWindowsIdentity;
      return json;
    }

    private void LoadAadTenantData(IVssRequestContext requestContext) => this.Domain = IdentityViewModelBase.GetTenantName(requestContext);
  }
}
