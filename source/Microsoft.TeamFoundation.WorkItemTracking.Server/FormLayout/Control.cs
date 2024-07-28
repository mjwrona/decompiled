// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control
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
  public class Control : LayoutNode, IXmlSerializable
  {
    [DataMember(Name = "label", EmitDefaultValue = false)]
    public string Label { get; set; }

    public string Name { get; set; }

    [DataMember(Name = "controlType", EmitDefaultValue = false)]
    public string ControlType { get; set; }

    [DataMember(Name = "height", EmitDefaultValue = false)]
    public int? Height { get; set; }

    [DataMember(Name = "readonly", EmitDefaultValue = false)]
    public bool? ReadOnly { get; set; }

    [DataMember(Name = "replacesFieldReferenceName", EmitDefaultValue = false)]
    public string ReplacesFieldReferenceName { get; set; }

    [DataMember(Name = "watermark", EmitDefaultValue = false)]
    public string Watermark { get; set; }

    [DataMember(Name = "metadata", EmitDefaultValue = false)]
    public string Metadata { get; set; }

    public Control Clone()
    {
      Control control = new Control();
      control.Id = this.Id;
      control.FromInheritedLayout = this.FromInheritedLayout;
      control.Overridden = this.Overridden;
      control.Label = this.Label;
      control.ControlType = this.ControlType;
      control.Rank = this.Rank;
      control.ReadOnly = this.ReadOnly;
      control.ReplacesFieldReferenceName = this.ReplacesFieldReferenceName;
      control.Watermark = this.Watermark;
      control.Metadata = this.Metadata;
      control.Visible = this.Visible;
      if (this.Contribution != null)
        control.Contribution = this.Contribution.Clone();
      control.Height = this.Height;
      control.Name = this.Name;
      return control;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader) => throw new NotImplementedException();

    public void WriteXml(XmlWriter writer)
    {
      if (!string.IsNullOrEmpty(this.Label))
        writer.WriteAttributeString(ProvisionAttributes.Label, this.Label);
      if (this.Contribution == null)
      {
        if (!string.IsNullOrEmpty(this.ControlType))
          writer.WriteAttributeString(ProvisionAttributes.ControlType, this.ControlType);
        bool flag1 = false;
        if (!string.IsNullOrEmpty(this.ControlType) && (WebLayoutXmlHelper.HeaderControls.Contains(this.ControlType) || this.ControlType.Equals(WellKnownControlNames.AssociatedAutomationControl, StringComparison.OrdinalIgnoreCase) || this.ControlType.Equals(WellKnownControlNames.DeploymentsControl, StringComparison.OrdinalIgnoreCase)))
          flag1 = true;
        if (!flag1 && !string.IsNullOrEmpty(this.Id))
          writer.WriteAttributeString(ProvisionAttributes.ControlFieldName, this.Id);
        if (!string.IsNullOrEmpty(this.Name))
          writer.WriteAttributeString(ProvisionAttributes.ControlName, this.Name);
        if (!string.IsNullOrEmpty(this.Watermark))
          writer.WriteAttributeString(ProvisionAttributes.ControlEmptyText, this.Watermark);
        bool? visible1 = this.ReadOnly;
        if (visible1.HasValue)
        {
          visible1 = this.ReadOnly;
          if (visible1.Value)
            writer.WriteAttributeString(ProvisionAttributes.ControlReadOnly, "True");
        }
        if (!string.IsNullOrEmpty(this.ReplacesFieldReferenceName))
          writer.WriteAttributeString(ProvisionAttributes.ControlReplacesFieldReferenceName, this.ReplacesFieldReferenceName);
        int? height = this.Height;
        if (height.HasValue)
        {
          height = this.Height;
          if (height.Value > 0)
          {
            XmlWriter xmlWriter = writer;
            string controlHeight = ProvisionAttributes.ControlHeight;
            height = this.Height;
            string str = height.Value.ToString();
            xmlWriter.WriteAttributeString(controlHeight, str);
          }
        }
        visible1 = this.Visible;
        if (visible1.HasValue)
        {
          visible1 = this.Visible;
          bool flag2 = false;
          if (visible1.GetValueOrDefault() == flag2 & visible1.HasValue)
          {
            XmlWriter xmlWriter = writer;
            string visible2 = ProvisionAttributes.Visible;
            visible1 = this.Visible;
            string str = XmlConvert.ToString(visible1.Value);
            xmlWriter.WriteAttributeString(visible2, str);
          }
        }
        writer.WriteRaw(this.Metadata);
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
        if (height1.HasValue)
        {
          height1 = this.Height;
          if (height1.Value > 0)
          {
            XmlWriter xmlWriter = writer;
            string height2 = ProvisionAttributes.Height;
            height1 = this.Height;
            string str = XmlConvert.ToString(height1.Value);
            xmlWriter.WriteAttributeString(height2, str);
          }
        }
        if (this.Contribution.Inputs == null || !this.Contribution.Inputs.Any<KeyValuePair<string, object>>())
          return;
        writer.WriteStartElement(ProvisionTags.ControlContributionInputs);
        foreach (KeyValuePair<string, object> input in (IEnumerable<KeyValuePair<string, object>>) this.Contribution.Inputs)
        {
          writer.WriteStartElement(ProvisionTags.ControlContributionInput);
          writer.WriteAttributeString(ProvisionAttributes.InputId, input.Key);
          writer.WriteAttributeString(ProvisionAttributes.InputValue, input.Value.ToString());
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
    }

    public override int GetHashCode() => this.Id.ToLower().GetHashCode();

    public override bool Equals(object obj) => obj is Control control && string.Equals(control.Id, this.Id, StringComparison.OrdinalIgnoreCase);
  }
}
