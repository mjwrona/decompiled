// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

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
  public class Page : LayoutNodeContainer<Section>, IXmlSerializable
  {
    public static Page CreateInheritPlaceholderNode()
    {
      Page inheritPlaceholderNode = new Page();
      inheritPlaceholderNode.Id = "$inherited";
      inheritPlaceholderNode.Children.Add(Section.CreateInheritPlaceholderNode());
      return inheritPlaceholderNode;
    }

    public Page()
      : this(false)
    {
    }

    public Page(bool initialize)
    {
      this.Children = (IList<Section>) new List<Section>();
      if (initialize)
      {
        for (int index = 1; index <= WebLayoutXmlHelper.MaxNumberOfSections; ++index)
          this.Children.Add(new Section(string.Format("Section{0}", (object) index)));
      }
      this.PageType = PageType.Custom;
      this.LayoutMode = PageLayoutMode.FirstColumnWide;
    }

    [DataMember(Name = "layoutMode")]
    public PageLayoutMode LayoutMode { get; set; }

    [DataMember(Name = "label", EmitDefaultValue = false)]
    public string Label { get; set; }

    [DataMember(Name = "pageType")]
    public PageType PageType { get; set; }

    [DataMember(Name = "locked", EmitDefaultValue = false)]
    public bool? Locked { get; set; }

    [DataMember(Name = "sections", EmitDefaultValue = false)]
    public override IList<Section> Children { get; protected set; }

    public Page Clone()
    {
      Page page = new Page();
      page.Id = this.Id;
      page.FromInheritedLayout = this.FromInheritedLayout;
      page.Overridden = this.Overridden;
      page.Label = this.Label;
      page.PageType = this.PageType;
      page.Locked = this.Locked;
      page.Rank = this.Rank;
      page.Visible = this.Visible;
      page.LayoutMode = this.LayoutMode;
      if (this.Contribution != null)
        page.Contribution = this.Contribution.Clone();
      page.Children = (IList<Section>) this.Children.Select<Section, Section>((Func<Section, Section>) (x => x.Clone())).ToList<Section>();
      return page;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader) => throw new NotImplementedException();

    public void WriteXml(XmlWriter writer)
    {
      if (!string.IsNullOrEmpty(this.Label))
        writer.WriteAttributeString(ProvisionAttributes.Label, this.Label);
      if (!this.IsContribution)
        writer.WriteAttributeString(ProvisionAttributes.PageLayoutMode, this.LayoutMode.ToString());
      if (this.Contribution == null)
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (Section));
        foreach (Section child in (IEnumerable<Section>) this.Children)
        {
          if (child.Children != null && child.Children.Any<Group>())
            xmlSerializer.Serialize(writer, (object) child);
        }
      }
      else
      {
        writer.WriteAttributeString(ProvisionAttributes.ContributionId, this.Contribution.ContributionId);
        if (!this.Visible.HasValue)
          return;
        bool? visible = this.Visible;
        bool flag = false;
        if (!(visible.GetValueOrDefault() == flag & visible.HasValue))
          return;
        writer.WriteAttributeString(ProvisionAttributes.Visible, XmlConvert.ToString(this.Visible.Value));
      }
    }
  }
}
