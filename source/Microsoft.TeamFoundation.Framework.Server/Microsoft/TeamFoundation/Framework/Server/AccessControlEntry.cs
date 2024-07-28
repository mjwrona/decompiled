// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControlEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AccessControlEntry : IAccessControlEntry
  {
    public AccessControlEntrySlim BaseEntry;
    public int InheritedAllow;
    public int InheritedDeny;
    public int EffectiveAllow;
    public int EffectiveDeny;
    public bool IncludesExtendedInfo;

    public AccessControlEntry()
    {
    }

    public AccessControlEntry(AccessControlEntrySlim slimAce) => this.BaseEntry = slimAce;

    public AccessControlEntry(IdentityDescriptor descriptor, int allow, int deny)
      : this(new AccessControlEntrySlim(descriptor, allow, deny))
    {
    }

    public AccessControlEntry(
      AccessControlEntrySlim slimAce,
      int inheritedAllow,
      int inheritedDeny,
      int effectiveAllow,
      int effectiveDeny)
      : this(slimAce)
    {
      this.InheritedAllow = inheritedAllow;
      this.InheritedDeny = inheritedDeny;
      this.EffectiveAllow = effectiveAllow;
      this.EffectiveDeny = effectiveDeny;
      this.IncludesExtendedInfo = true;
    }

    public AccessControlEntry(
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      int inheritedAllow,
      int inheritedDeny,
      int effectiveAllow,
      int effectiveDeny)
      : this(new AccessControlEntrySlim(descriptor, allow, deny), inheritedAllow, inheritedDeny, effectiveAllow, effectiveDeny)
    {
    }

    public AccessControlEntry(IAccessControlEntry ace)
      : this(ace.Descriptor, ace.Allow, ace.Deny)
    {
      if (!ace.IncludesExtendedInfo)
        return;
      this.InheritedAllow = ace.InheritedAllow;
      this.InheritedDeny = ace.InheritedDeny;
      this.EffectiveAllow = ace.EffectiveAllow;
      this.EffectiveDeny = ace.EffectiveDeny;
      this.IncludesExtendedInfo = true;
    }

    public IdentityDescriptor Descriptor
    {
      get => this.BaseEntry.Descriptor;
      set => this.BaseEntry = this.BaseEntry with
      {
        Descriptor = value
      };
    }

    public int Allow
    {
      get => this.BaseEntry.Allow;
      set => this.BaseEntry = this.BaseEntry with
      {
        Allow = value
      };
    }

    public int Deny
    {
      get => this.BaseEntry.Deny;
      set => this.BaseEntry = this.BaseEntry with
      {
        Deny = value
      };
    }

    public bool IsEmpty => this.BaseEntry.IsEmpty;

    bool IAccessControlEntry.IncludesExtendedInfo => this.IncludesExtendedInfo;

    int IAccessControlEntry.InheritedAllow => this.InheritedAllow;

    int IAccessControlEntry.InheritedDeny => this.InheritedDeny;

    int IAccessControlEntry.EffectiveAllow => this.EffectiveAllow;

    int IAccessControlEntry.EffectiveDeny => this.EffectiveDeny;

    IAccessControlEntry IAccessControlEntry.Clone() => (IAccessControlEntry) new AccessControlEntry(this.Descriptor, this.Allow, this.Deny, this.InheritedAllow, this.InheritedDeny, this.EffectiveAllow, this.EffectiveDeny);

    public Microsoft.VisualStudio.Services.Security.AccessControlEntry ToRestContractType() => new Microsoft.VisualStudio.Services.Security.AccessControlEntry(this.Descriptor, this.Allow, this.Deny, new AceExtendedInformation(this.InheritedAllow, this.InheritedDeny, this.EffectiveAllow, this.EffectiveDeny));
  }
}
