// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityChangedData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [XmlType("data")]
  public class IdentityChangedData
  {
    [XmlAttribute("iseq")]
    public int IdentitySequenceId { get; set; }

    [XmlAttribute("gseq")]
    public int GroupSequenceId { get; set; }

    [XmlAttribute("dct")]
    public DescriptorChangeType DescriptorChangeType { get; set; }

    [XmlElement("dc")]
    public DescriptorChange[] DescriptorChanges { get; set; }

    [XmlElement("gsvc")]
    public GroupScopeVisibiltyChangeInfo[] GroupScopeVisibiltyChanges { get; set; }

    [XmlElement("gsc")]
    public GroupScopeChange[] GroupScopeChanges { get; set; }

    [XmlElement("mc")]
    public MembershipChangeInfo[] MembershipChanges { get; set; }

    [XmlIgnore]
    public List<Guid> DescriptorChangeIds
    {
      get
      {
        DescriptorChange[] descriptorChanges = this.DescriptorChanges;
        return descriptorChanges == null ? (List<Guid>) null : ((IEnumerable<DescriptorChange>) descriptorChanges).Select<DescriptorChange, Guid>((Func<DescriptorChange, Guid>) (dc => dc.MasterId)).ToList<Guid>();
      }
    }

    [XmlIgnore]
    public List<Guid> GroupScopeChangeIds
    {
      get
      {
        GroupScopeChange[] groupScopeChanges = this.GroupScopeChanges;
        return groupScopeChanges == null ? (List<Guid>) null : ((IEnumerable<GroupScopeChange>) groupScopeChanges).Select<GroupScopeChange, Guid>((Func<GroupScopeChange, Guid>) (gsc => gsc.ScopeId)).ToList<Guid>();
      }
    }
  }
}
