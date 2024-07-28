// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AccessControlEntryData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class AccessControlEntryData
  {
    public AccessControlEntryData() => this.ExtendedInfo = new AceExtendedInformation();

    public AccessControlEntryData(IAccessControlEntry ace)
    {
      this.Allow = ace.Allow;
      this.Deny = ace.Deny;
      this.Descriptor = ace.Descriptor;
      this.ExtendedInfo = new AceExtendedInformation();
      if (!ace.IncludesExtendedInfo)
        return;
      this.ExtendedInfo.InheritedAllow = ace.InheritedAllow;
      this.ExtendedInfo.InheritedDeny = ace.InheritedDeny;
      this.ExtendedInfo.EffectiveAllow = ace.EffectiveAllow;
      this.ExtendedInfo.EffectiveDeny = ace.EffectiveDeny;
    }

    public int Allow { get; set; }

    public int Deny { get; set; }

    internal Guid TeamFoundationId { get; set; }

    [XmlElement("Descriptor")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, PropertyName = "Descriptor")]
    public IdentityDescriptorData DescriptorValue
    {
      get => new IdentityDescriptorData(this.Descriptor);
      set => throw new NotSupportedException();
    }

    [XmlIgnore]
    public IdentityDescriptor Descriptor { get; set; }

    [XmlElement("ExtendedInfo")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, PropertyName = "ExtendedInfo")]
    public AccessControlEntryExtendedData ExtendedInfoValue
    {
      get => new AccessControlEntryExtendedData(this.ExtendedInfo);
      set => throw new NotSupportedException();
    }

    [XmlIgnore]
    public AceExtendedInformation ExtendedInfo { get; set; }
  }
}
