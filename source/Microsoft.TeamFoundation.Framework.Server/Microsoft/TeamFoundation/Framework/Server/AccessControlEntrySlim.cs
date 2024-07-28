// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControlEntrySlim
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct AccessControlEntrySlim : IAccessControlEntry
  {
    public IdentityDescriptor Descriptor;
    public int Allow;
    public int Deny;

    public AccessControlEntrySlim(IdentityDescriptor descriptor, int allow, int deny)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      this.Descriptor = descriptor;
      this.Allow = allow;
      this.Deny = deny;
    }

    public AccessControlEntrySlim(IAccessControlEntry ace)
    {
      ArgumentUtility.CheckForNull<IAccessControlEntry>(ace, nameof (ace));
      this.Descriptor = ace.Descriptor;
      this.Allow = ace.Allow;
      this.Deny = ace.Deny;
    }

    public bool IsEmpty => this.Allow == 0 && this.Deny == 0;

    IdentityDescriptor IAccessControlEntry.Descriptor
    {
      get => this.Descriptor;
      set => this.Descriptor = value;
    }

    int IAccessControlEntry.Allow
    {
      get => this.Allow;
      set => this.Allow = value;
    }

    int IAccessControlEntry.Deny
    {
      get => this.Deny;
      set => this.Deny = value;
    }

    bool IAccessControlEntry.IncludesExtendedInfo => false;

    int IAccessControlEntry.InheritedAllow => 0;

    int IAccessControlEntry.InheritedDeny => 0;

    int IAccessControlEntry.EffectiveAllow => 0;

    int IAccessControlEntry.EffectiveDeny => 0;

    IAccessControlEntry IAccessControlEntry.Clone() => (IAccessControlEntry) new AccessControlEntrySlim(this.Descriptor, this.Allow, this.Deny);

    public Microsoft.VisualStudio.Services.Security.AccessControlEntry ToRestContractType() => new Microsoft.VisualStudio.Services.Security.AccessControlEntry(this.Descriptor, this.Allow, this.Deny, (AceExtendedInformation) null);
  }
}
