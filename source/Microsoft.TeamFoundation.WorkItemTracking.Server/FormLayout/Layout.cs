// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Layout
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Newtonsoft.Json;
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
  [XmlRoot("WebLayout")]
  public class Layout : LayoutNodeContainer<Page>, IXmlSerializable
  {
    public Layout()
    {
      this.SystemControls = new HashSet<Control>();
      this.Extensions = (IList<Extension>) new List<Extension>();
      this.OrphanedNodes = new List<Group>();
    }

    public Layout(Layout layout)
      : this()
    {
      this.Id = layout.Id;
    }

    [DataMember(Name = "extensions", EmitDefaultValue = false)]
    public IList<Extension> Extensions { get; protected set; }

    [DataMember(Name = "pages", EmitDefaultValue = false)]
    public override IList<Page> Children { get; protected set; }

    [DataMember(Name = "systemControls", EmitDefaultValue = false)]
    public HashSet<Control> SystemControls { get; protected set; }

    [JsonIgnore]
    public override int? Rank { get; set; }

    [DataMember(Name = "showEmptyReadOnlyFields", EmitDefaultValue = false)]
    public bool? ShowEmptyReadOnlyFields { get; set; }

    [JsonIgnore]
    public List<Group> OrphanedNodes { get; protected set; }

    public Layout Clone()
    {
      Layout layout = new Layout();
      layout.Id = this.Id;
      layout.FromInheritedLayout = this.FromInheritedLayout;
      layout.Extensions = (IList<Extension>) this.Extensions.Select<Extension, Extension>((Func<Extension, Extension>) (ext => ext.Clone())).ToList<Extension>();
      layout.Children = (IList<Page>) this.Children.Select<Page, Page>((Func<Page, Page>) (x => x.Clone())).ToList<Page>();
      layout.SystemControls = new HashSet<Control>(this.SystemControls.Select<Control, Control>((Func<Control, Control>) (x => x.Clone())));
      layout.ShowEmptyReadOnlyFields = this.ShowEmptyReadOnlyFields;
      layout.OrphanedNodes = this.OrphanedNodes.Select<Group, Group>((Func<Group, Group>) (x => x.Clone())).ToList<Group>();
      return layout;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader) => throw new NotImplementedException();

    public void WriteXml(XmlWriter writer)
    {
      if (this.ShowEmptyReadOnlyFields.HasValue)
        writer.WriteAttributeString(ProvisionAttributes.LayoutShowEmptyReadOnlyFields, XmlConvert.ToString(this.ShowEmptyReadOnlyFields.Value));
      if (this.Extensions != null && this.Extensions.Any<Extension>())
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (Extension));
        writer.WriteStartElement(ProvisionTags.Extensions);
        foreach (Extension extension in (IEnumerable<Extension>) this.Extensions)
          xmlSerializer.Serialize(writer, (object) extension);
        writer.WriteEndElement();
      }
      if (this.SystemControls != null && this.SystemControls.Any<Control>())
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (Control));
        writer.WriteStartElement(ProvisionTags.SystemControls);
        foreach (Control systemControl in this.SystemControls)
          xmlSerializer.Serialize(writer, (object) systemControl);
        writer.WriteEndElement();
      }
      if (this.Children == null || !this.Children.Any<Page>())
        return;
      XmlSerializer xmlSerializer1 = new XmlSerializer(typeof (Page));
      XmlSerializer serializer = TeamFoundationSerializationUtility.CreateSerializer(typeof (Page), new XmlRootAttribute(ProvisionTags.PageContribution));
      foreach (Page child in (IEnumerable<Page>) this.Children)
      {
        if (!child.IsContribution)
          xmlSerializer1.Serialize(writer, (object) child);
        else
          serializer.Serialize(writer, (object) child);
      }
    }
  }
}
