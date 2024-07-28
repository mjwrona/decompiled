// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueriedAccessControlEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct QueriedAccessControlEntry
  {
    public readonly IdentityDescriptor IdentityDescriptor;
    public readonly int Allow;
    public readonly int Deny;
    public readonly int InheritedAllow;
    public readonly int InheritedDeny;
    public readonly int EffectiveAllow;
    public readonly int EffectiveDeny;

    public QueriedAccessControlEntry(IdentityDescriptor descriptor, int allow, int deny)
    {
      this.IdentityDescriptor = descriptor;
      this.Allow = allow;
      this.Deny = deny;
      this.InheritedAllow = 0;
      this.InheritedDeny = 0;
      this.EffectiveAllow = 0;
      this.EffectiveDeny = 0;
    }

    public QueriedAccessControlEntry(
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      int inheritedAllow,
      int inheritedDeny,
      int effectiveAllow,
      int effectiveDeny)
    {
      this.IdentityDescriptor = descriptor;
      this.Allow = allow;
      this.Deny = deny;
      this.InheritedAllow = inheritedAllow;
      this.InheritedDeny = inheritedDeny;
      this.EffectiveAllow = effectiveAllow;
      this.EffectiveDeny = effectiveDeny;
    }
  }
}
