// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessControlEntry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class AccessControlEntry
  {
    protected AccessControlEntry()
      : this((IdentityDescriptor) null, 0, 0)
    {
    }

    public AccessControlEntry(IdentityDescriptor descriptor, int allow, int deny)
      : this(descriptor, allow, deny, (AceExtendedInformation) null)
    {
    }

    internal AccessControlEntry(
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      AceExtendedInformation extendedInfo)
    {
      this.Allow = allow;
      this.Deny = deny;
      this.Descriptor = descriptor;
      this.ExtendedInfo = extendedInfo;
    }

    public int Allow { get; set; }

    public int Deny { get; set; }

    public IdentityDescriptor Descriptor { get; set; }

    public AceExtendedInformation ExtendedInfo { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public bool IsEmpty => this.Allow == 0 && this.Deny == 0;

    internal AccessControlEntry Clone() => new AccessControlEntry()
    {
      Allow = this.Allow,
      Deny = this.Deny,
      Descriptor = this.Descriptor,
      ExtendedInfo = this.ExtendedInfo
    };
  }
}
