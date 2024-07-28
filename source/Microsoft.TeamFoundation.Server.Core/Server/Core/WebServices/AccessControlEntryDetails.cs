// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.AccessControlEntryDetails
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  public class AccessControlEntryDetails
  {
    public AccessControlEntryDetails()
    {
    }

    public AccessControlEntryDetails(string token, IAccessControlEntry ace)
    {
      this.Token = token;
      this.Descriptor = ace.Descriptor;
      this.Allow = ace.Allow;
      this.Deny = ace.Deny;
      if (!ace.IncludesExtendedInfo)
        return;
      this.ExtendedInformation = new AceExtendedInformation()
      {
        InheritedAllow = ace.InheritedAllow,
        InheritedDeny = ace.InheritedDeny,
        EffectiveAllow = ace.EffectiveAllow,
        EffectiveDeny = ace.EffectiveDeny
      };
    }

    [XmlAttribute]
    public int Allow { get; set; }

    [XmlAttribute]
    public int Deny { get; set; }

    [XmlAttribute]
    public string Token { get; set; }

    [XmlElement("SerializableDescriptor")]
    public IdentityDescriptor Descriptor { get; set; }

    public AceExtendedInformation ExtendedInformation { get; set; }

    public IAccessControlEntry ToAccessControlEntry() => this.ExtendedInformation == null ? (IAccessControlEntry) new AccessControlEntry(this.Descriptor, this.Allow, this.Deny) : (IAccessControlEntry) new AccessControlEntry(this.Descriptor, this.Allow, this.Deny, this.ExtendedInformation.InheritedAllow, this.ExtendedInformation.InheritedDeny, this.ExtendedInformation.EffectiveAllow, this.ExtendedInformation.EffectiveDeny);
  }
}
