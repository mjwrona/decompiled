// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMessageBusData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [XmlType("data")]
  public class IdentityMessageBusData
  {
    [XmlElement("dc")]
    public Guid[] DescriptorChanges { get; set; }

    [XmlElement("dcm")]
    public Guid[] DescriptorChangesWithMasterId { get; set; }

    [XmlElement("ic")]
    public Guid[] IdentityChanges { get; set; }

    [XmlElement("gc")]
    public Guid[] GroupChanges { get; set; }

    [XmlElement("mc")]
    public MembershipChangeInfo[] MembershipChanges { get; set; }

    [XmlAttribute("dct")]
    public DescriptorChangeType DescriptorChangeType { get; set; }

    [XmlElement("gsvc")]
    public GroupScopeVisibiltyChangeInfo[] GroupScopeVisibiltyChanges { get; set; }

    [XmlAttribute("gsvmc")]
    public Guid[] GroupScopeVisibilityMajorChanges { get; set; }

    [XmlAttribute("iseq")]
    public long IdentitySequenceId { get; set; }

    [XmlAttribute("gseq")]
    public long GroupSequenceId { get; set; }
  }
}
