// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupMembershipChangeSqlNotification
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [XmlType("ArrayOfGroupMembershipChange")]
  public class GroupMembershipChangeSqlNotification
  {
    public GroupMembershipChangeSqlNotification()
    {
    }

    public GroupMembershipChangeSqlNotification(GroupMembershipChangeMessage message)
    {
      this.SequenceId = message != null ? message.SequenceId : 0L;
      GroupMembershipChangeMessage.Change[] changes = message.Changes;
      this.Changes = new GroupMembershipChangeSqlNotification.Change[changes != null ? changes.Length : 0];
      for (int index = 0; index < this.Changes.Length; ++index)
        this.Changes[index] = new GroupMembershipChangeSqlNotification.Change()
        {
          ContainerId = message.Changes[index].ContainerId,
          MemberId = message.Changes[index].MemberId,
          Active = message.Changes[index].Active
        };
    }

    [XmlAttribute("SequenceId")]
    public long SequenceId { get; set; }

    [XmlElement("GroupMembershipChange")]
    public GroupMembershipChangeSqlNotification.Change[] Changes { get; set; }

    public struct Change
    {
      [XmlAttribute("cid")]
      public Guid ContainerId { get; set; }

      [XmlAttribute("mid")]
      public Guid MemberId { get; set; }

      [XmlIgnore]
      public bool Active { get; set; }

      [XmlAttribute("active")]
      public string ActiveString
      {
        get => !this.Active ? "0" : "1";
        set => this.Active = XmlConvert.ToBoolean(value);
      }
    }
  }
}
