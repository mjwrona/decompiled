// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IdentityDescriptorData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public sealed class IdentityDescriptorData
  {
    public IdentityDescriptorData()
    {
    }

    internal IdentityDescriptorData(IdentityDescriptor descriptor)
    {
      this.IdentityType = descriptor.IdentityType;
      this.Identifier = descriptor.Identifier;
    }

    [XmlAttribute("identityType")]
    public string IdentityType { get; set; }

    [XmlAttribute("identifier")]
    public string Identifier { get; set; }
  }
}
