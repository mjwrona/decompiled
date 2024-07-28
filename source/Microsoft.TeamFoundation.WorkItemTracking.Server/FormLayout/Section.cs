// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Section
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  [DataContract]
  public class Section : LayoutNodeContainer<Group>, IXmlSerializable
  {
    public const string SectionIdFormat = "Section{0}";

    public static Section CreateInheritPlaceholderNode() => new Section("$inherited");

    public Section(string id) => this.Id = id;

    public Section()
    {
    }

    [DataMember(Name = "groups", EmitDefaultValue = false)]
    public override IList<Group> Children { get; protected set; }

    public override int? Rank
    {
      get
      {
        for (int index = 1; index < WebLayoutXmlHelper.MaxNumberOfSections + 1; ++index)
        {
          if (string.Equals(this.Id, string.Format("Section{0}", (object) index), StringComparison.OrdinalIgnoreCase))
            return new int?(index - 1);
        }
        return new int?();
      }
      set
      {
      }
    }

    public Section Clone()
    {
      Section section = new Section();
      section.Id = this.Id;
      section.FromInheritedLayout = this.FromInheritedLayout;
      section.Overridden = this.Overridden;
      section.Children = (IList<Group>) this.Children.Select<Group, Group>((Func<Group, Group>) (x => x.Clone())).ToList<Group>();
      section.Rank = this.Rank;
      if (this.Contribution != null)
        section.Contribution = this.Contribution.Clone();
      section.Visible = this.Visible;
      return section;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader) => throw new NotImplementedException();

    public void WriteXml(XmlWriter writer)
    {
      if (this.Children == null || !this.Children.Any<Group>())
        return;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (Group));
      XmlSerializer serializer = TeamFoundationSerializationUtility.CreateSerializer(typeof (Group), new XmlRootAttribute(ProvisionTags.GroupContribution));
      foreach (Group child in (IEnumerable<Group>) this.Children)
      {
        if (!child.IsContribution)
          xmlSerializer.Serialize(writer, (object) child);
        else
          serializer.Serialize(writer, (object) child);
      }
    }
  }
}
