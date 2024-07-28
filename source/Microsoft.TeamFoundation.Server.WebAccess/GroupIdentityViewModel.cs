// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.GroupIdentityViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class GroupIdentityViewModel : IdentityViewModelBase
  {
    private string m_scope;
    private string m_friendlyDisplayName;
    protected string m_subHeader;
    private const string TeamProjectDomainPrefix = "vstfs:///Classification/TeamProject";

    public GroupIdentityViewModel()
    {
    }

    public GroupIdentityViewModel(TeamFoundationIdentity identity)
      : base(identity)
    {
      this.DisplayName = identity.DisplayName;
      this.Description = identity.GetAttribute(nameof (Description), string.Empty);
      if (this.IsWindowsIdentity)
      {
        this.m_scope = identity.GetAttribute("Domain", string.Empty);
      }
      else
      {
        UserNameUtil.Parse(identity.DisplayName, out this.m_friendlyDisplayName, out string _);
        this.m_scope = identity.GetAttribute("ScopeName", string.Empty);
        this.MemberCount = identity.Members.Count;
        this.MemberCountText = this.MemberCount.ToString();
        string attribute1 = identity.GetAttribute("Domain", string.Empty);
        this.IsProjectLevel = !string.IsNullOrEmpty(attribute1) && attribute1.StartsWith("vstfs:///Classification/TeamProject");
        if (identity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        {
          string attribute2 = identity.GetAttribute("SpecialType", string.Empty);
          this.RestrictEditingMembership = string.Equals(attribute2, "ServiceApplicationGroup", StringComparison.OrdinalIgnoreCase);
          this.RestrictEditingMembership |= string.Equals(attribute2, "EveryoneApplicationGroup", StringComparison.OrdinalIgnoreCase);
        }
        this.IsAadGroup = AadIdentityHelper.IsAadGroup(identity.Descriptor);
      }
      this.IsReadOnly = false;
      this.m_subHeader = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) this.m_scope);
      TfsWebContext tfsWebContext = (TfsWebContext) null;
      if (!this.IsAadGroup || !TfsHelpers.TryGetTfsWebContext(out tfsWebContext))
        return;
      GroupIdentityViewModel.LoadAaDTenantData(this, tfsWebContext.TfsRequestContext);
    }

    internal static void LoadAaDTenantData(
      GroupIdentityViewModel groupIdentityViewModel,
      IVssRequestContext context)
    {
      if (!groupIdentityViewModel.IsAadGroup)
        return;
      groupIdentityViewModel.RestrictEditingMembership = true;
      groupIdentityViewModel.IsReadOnly = true;
      try
      {
        string tenantName = IdentityViewModelBase.GetTenantName(context);
        if (tenantName != null)
        {
          groupIdentityViewModel.m_subHeader = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) tenantName);
          groupIdentityViewModel.m_scope = tenantName;
          groupIdentityViewModel.DisplayName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]\\{1}", (object) tenantName, (object) groupIdentityViewModel.FriendlyDisplayName);
        }
        else
          context.Trace(0, TraceLevel.Error, nameof (GroupIdentityViewModel), nameof (LoadAaDTenantData), "GetTenant() returned null for AAD Group. GroupID : {0}", (object) groupIdentityViewModel.TeamFoundationId);
      }
      catch (Exception ex)
      {
        context.TraceCatch(0, nameof (GroupIdentityViewModel), nameof (LoadAaDTenantData), ex);
      }
    }

    public string Scope => this.m_scope;

    public string Description { get; private set; }

    public override string FriendlyDisplayName => this.m_friendlyDisplayName ?? this.DisplayName;

    public override string IdentityType => "group";

    public bool IsReadOnly { get; set; }

    public bool IsProjectLevel { get; set; }

    public int MemberCount { get; private set; }

    public string MemberCountText { get; private set; }

    public bool IsAadGroup { get; private set; }

    public List<IdentityViewModelBase> Members { get; private set; }

    public bool RestrictEditingMembership { get; set; }

    public override string SubHeader => this.m_subHeader;

    public void PopulateGroupMembers(TfsWebContext webContext)
    {
      TeamFoundationIdentity[] foundationIdentityArray = webContext.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(webContext.TfsRequestContext, this.Identity.Members.ToArray<IdentityDescriptor>(), this.IsWindowsIdentity ? MembershipQuery.None : MembershipQuery.Direct, ReadIdentityOptions.None, (IEnumerable<string>) null);
      this.Members = new List<IdentityViewModelBase>();
      foreach (TeamFoundationIdentity identity in foundationIdentityArray)
      {
        if (identity != null)
        {
          IdentityViewModelBase identityViewModelBase;
          if (this.IsWindowsIdentity)
          {
            identityViewModelBase = !identity.IsContainer ? (IdentityViewModelBase) new UserIdentityViewModel() : (IdentityViewModelBase) new GroupIdentityViewModel();
            identityViewModelBase.TeamFoundationId = identity.TeamFoundationId;
            identityViewModelBase.DisplayName = identity.DisplayName;
          }
          else
            identityViewModelBase = IdentityImageUtility.GetIdentityViewModel(identity);
          this.Members.Add(identityViewModelBase);
        }
      }
      this.Members.Sort();
    }

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["IsWindowsGroup"] = (object) this.IsWindowsIdentity;
      json["IsAadGroup"] = (object) this.IsAadGroup;
      json["Description"] = (object) this.Description;
      json["Scope"] = (object) this.Scope;
      json["MemberCountText"] = (object) this.MemberCountText;
      json["IsTeam"] = (object) false;
      json["IsProjectLevel"] = (object) this.IsProjectLevel;
      if (this.RestrictEditingMembership)
        json["RestrictEditingMembership"] = (object) true;
      return json;
    }
  }
}
