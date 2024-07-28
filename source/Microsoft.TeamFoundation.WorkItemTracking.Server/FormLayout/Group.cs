// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group
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
  public class Group : LayoutNodeContainer<Control>, IXmlSerializable
  {
    [DataMember(Name = "label", EmitDefaultValue = false)]
    public string Label { get; set; }

    [DataMember(Name = "controls", EmitDefaultValue = false)]
    public override IList<Control> Children { get; protected set; }

    [DataMember(Name = "height", EmitDefaultValue = false)]
    public int? Height { get; set; }

    public Group Clone(bool cloneChildren = true)
    {
      Group group = new Group();
      group.Id = this.Id;
      group.FromInheritedLayout = this.FromInheritedLayout;
      group.Overridden = this.Overridden;
      group.Label = this.Label;
      group.Rank = this.Rank;
      group.Visible = this.Visible;
      group.Height = this.Height;
      if (this.Contribution != null)
        group.Contribution = this.Contribution.Clone();
      if (cloneChildren)
        group.Children = (IList<Control>) this.Children.Select<Control, Control>((Func<Control, Control>) (x => x.Clone())).ToList<Control>();
      else
        group.Children = (IList<Control>) new List<Control>();
      return group;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader) => throw new NotImplementedException();

    public void WriteXml(XmlWriter writer)
    {
      if (!string.IsNullOrEmpty(this.Label))
        writer.WriteAttributeString(ProvisionAttributes.Label, this.Label);
      if (this.Contribution == null)
      {
        if (this.Children == null || !this.Children.Any<Control>())
          return;
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (Control));
        XmlSerializer serializer = TeamFoundationSerializationUtility.CreateSerializer(typeof (Control), new XmlRootAttribute(ProvisionTags.ControlContribution));
        foreach (Control child in (IEnumerable<Control>) this.Children)
        {
          if (!child.IsContribution)
            xmlSerializer.Serialize(writer, (object) child);
          else
            serializer.Serialize(writer, (object) child);
        }
      }
      else
      {
        writer.WriteAttributeString(ProvisionAttributes.ContributionId, this.Contribution.ContributionId);
        if (this.Visible.HasValue)
        {
          bool? visible = this.Visible;
          bool flag = false;
          if (visible.GetValueOrDefault() == flag & visible.HasValue)
            writer.WriteAttributeString(ProvisionAttributes.Visible, XmlConvert.ToString(this.Visible.Value));
        }
        int? height1 = this.Height;
        if (!height1.HasValue)
          return;
        height1 = this.Height;
        if (height1.Value <= 0)
          return;
        XmlWriter xmlWriter = writer;
        string height2 = ProvisionAttributes.Height;
        height1 = this.Height;
        string str = XmlConvert.ToString(height1.Value);
        xmlWriter.WriteAttributeString(height2, str);
      }
    }
  }
}
