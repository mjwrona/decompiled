// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.NewFormXmlSerializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  internal class NewFormXmlSerializer
  {
    private const string c_testStepsFieldReferenceName = "Microsoft.VSTS.TCM.Steps";
    private XmlDocument document = new XmlDocument();
    private Dictionary<string, Control> m_coreControls;
    private Dictionary<string, int> m_controlsInLayoutMap;
    private LinksControlTransformer m_linksControlTransformer = new LinksControlTransformer();
    private ITraceRequest m_tracer;

    internal NewFormXmlSerializer(ITraceRequest tracer = null)
    {
      this.m_tracer = tracer;
      this.m_controlsInLayoutMap = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    }

    internal string Serialize(Layout layout)
    {
      ArgumentUtility.CheckForNull<Layout>(layout, nameof (layout));
      if (this.m_tracer != null)
        this.m_tracer.TraceEnter(909804, "FormLayout", "FormTransformsLayer", nameof (Serialize));
      try
      {
        return this.Transform(layout);
      }
      catch (Exception ex)
      {
        if (this.m_tracer != null)
          this.m_tracer.TraceException(909805, TraceLevel.Error, "FormLayout", "FormTransformsLayer", ex);
        throw;
      }
      finally
      {
        if (this.m_tracer != null)
          this.m_tracer.TraceLeave(909805, "FormLayout", "FormTransformsLayer", nameof (Serialize));
      }
    }

    public string Transform(Layout layout)
    {
      XmlElement element = this.document.CreateElement(ProvisionTags.Form);
      this.m_coreControls = layout.SystemControls.ToDictionary<Control, string, Control>((Func<Control, string>) (x => x.Id), (Func<Control, Control>) (x => x));
      XmlElement layoutElement = this.CreateLayoutElement(element);
      this.AddHeader(layoutElement);
      XmlElement tabGroupElement = this.CreateTabGroupElement(layoutElement, this.FormatMargin(0, 10, 0, 0));
      foreach (Page child in (IEnumerable<Page>) layout.Children)
      {
        bool? visible = child.Visible;
        if (visible.HasValue)
        {
          visible = child.Visible;
          if (!visible.Value)
            continue;
        }
        if (this.HasVisibleControls(child))
        {
          XmlElement groupElement = this.CreateGroupElement(this.CreateTabElement(tabGroupElement, child.Label), string.Empty);
          this.FillSections(child, groupElement);
        }
      }
      this.OptimizeSingleTabGroupLabels(element);
      this.document.AppendChild((XmlNode) element);
      return this.document.OuterXml;
    }

    private void OptimizeSingleTabGroupLabels(XmlElement formElement)
    {
      foreach (XmlElement selectNode in formElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//{0}", (object) ProvisionTags.Tab)))
      {
        if (selectNode.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Control)).Count == 1)
        {
          XmlNodeList xmlNodeList = selectNode.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}[@{1}]", (object) ProvisionTags.Group, (object) ProvisionAttributes.Label));
          if (xmlNodeList.Count == 1 && string.Equals(selectNode.GetAttribute(ProvisionAttributes.Label), ((XmlElement) xmlNodeList[0]).GetAttribute(ProvisionAttributes.Label), StringComparison.OrdinalIgnoreCase))
            xmlNodeList[0].Attributes.RemoveNamedItem(ProvisionAttributes.Label);
        }
      }
    }

    private void AddHeader(XmlElement parent)
    {
      this.AddHeaderControls(this.CreateGroupElement(parent, string.Empty, this.FormatMargin(10, 0, 0, 0)));
      this.AddCoreControls(this.CreateGroupElement(parent, string.Empty, this.FormatMargin(10, 10, 0, 0)));
    }

    private void AddHeaderControls(XmlElement parent)
    {
      XmlElement controlElement1 = this.CreateControlElement(this.CreateColumnElement(parent, 94), "System.Title", WellKnownControlNames.FieldControl, string.Empty, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormTitleWatermark());
      controlElement1.Attributes.Append(this.AddAttribute(controlElement1, ProvisionAttributes.ControlFontSize, ProvisionValues.ControlFontSizeLarge));
      XmlElement controlElement2 = this.CreateControlElement(this.CreateColumnElement(parent, 6), "System.Id", WellKnownControlNames.FieldControl, string.Empty);
      controlElement2.Attributes.Append(this.AddAttribute(controlElement2, ProvisionAttributes.ControlFontSize, ProvisionValues.ControlFontSizeLarge));
    }

    private void AddCoreControls(XmlElement parent)
    {
      XmlElement columnElement1 = this.CreateColumnElement(parent, 30);
      this.CreateControlElement(columnElement1, "System.AssignedTo", WellKnownControlNames.FieldControl, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormAssignedToLabel(), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormAssignedToWatermark(), controlLabelPosition: ProvisionValues.ControlLabelPositionLeft);
      this.CreateControlElement(columnElement1, "System.State", WellKnownControlNames.FieldControl, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormStateLabel(), controlLabelPosition: ProvisionValues.ControlLabelPositionLeft);
      createControlElementBasedOnVisibilityAndLabels(columnElement1, "System.Reason", WellKnownControlNames.FieldControl, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormReasonLabel(), ProvisionValues.ControlLabelPositionLeft);
      XmlElement columnElement2 = this.CreateColumnElement(parent, 40);
      this.CreateControlElement(columnElement2, string.Empty, WellKnownControlNames.LabelControl, string.Empty);
      createControlElementBasedOnVisibilityAndLabels(columnElement2, "System.AreaPath", WellKnownControlNames.ClassificationControl, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormAreaPathLabel(), ProvisionValues.ControlLabelPositionLeft);
      createControlElementBasedOnVisibilityAndLabels(columnElement2, "System.IterationPath", WellKnownControlNames.ClassificationControl, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormIterationLabel(), ProvisionValues.ControlLabelPositionLeft);
      XmlElement columnElement3 = this.CreateColumnElement(parent, 30);
      this.CreateControlElement(columnElement3, string.Empty, WellKnownControlNames.LabelControl, string.Empty);
      this.CreateControlElement(columnElement3, string.Empty, WellKnownControlNames.LabelControl, string.Empty);
      this.CreateControlElement(columnElement3, "System.ChangedDate", WellKnownControlNames.DateControl, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NewFormChangedDateLabel(), readOnly: true, controlLabelPosition: ProvisionValues.ControlLabelPositionLeft);

      void createControlElementBasedOnVisibilityAndLabels(
        XmlElement parentElement,
        string fieldName,
        string type,
        string label,
        string controlLabelPosition)
      {
        Control control;
        if (this.m_coreControls.TryGetValue(fieldName, out control) && ((int) control.Visible ?? 1) == 0)
          return;
        label = control?.Label ?? label;
        this.CreateControlElement(parentElement, fieldName, type, label, controlLabelPosition: controlLabelPosition);
      }
    }

    private void FillSections(Page page, XmlElement parent)
    {
      IDictionary<Section, int> sectionWidths = this.GetSectionWidths(page);
      Section firstSection = (Section) null;
      XmlElement parent1 = (XmlElement) null;
      for (int index = 0; index < page.Children.Count; ++index)
      {
        Section key = page.SectionFromIndex(index);
        int width1 = sectionWidths[key];
        if (width1 > 0)
        {
          if (firstSection == null)
          {
            this.AddControls(this.CreateColumnElement(this.CreateGroupElement(this.CreateColumnElement(parent, width1), string.Empty), 100), (IEnumerable<Group>) key.Children);
            firstSection = key;
          }
          else
          {
            if (parent1 == null)
            {
              int width2 = sectionWidths.Sum<KeyValuePair<Section, int>>((Func<KeyValuePair<Section, int>, int>) (widthPair => widthPair.Key == firstSection ? 0 : widthPair.Value));
              parent1 = this.CreateGroupElement(this.CreateColumnElement(parent, width2), string.Empty, this.FormatMargin(20, 0, 0, 0));
            }
            this.AddControls(this.CreateColumnElement(this.CreateGroupElement(this.CreateColumnElement(parent1, width1), string.Empty), 100), (IEnumerable<Group>) key.Children);
          }
        }
      }
    }

    private void AddControls(XmlElement sectionColumnElement, IEnumerable<Group> groups)
    {
      foreach (Group group in groups)
      {
        if (this.HasVisibleControls(group))
        {
          string label = group.Children.Count != 1 || TFStringComparer.WorkItemTypeReferenceName.Compare(group.Children[0].Id, "Microsoft.VSTS.TCM.Steps") != 0 ? group.Label : string.Empty;
          XmlElement columnElement = this.CreateColumnElement(this.CreateGroupElement(sectionColumnElement, label), 100);
          foreach (Control child in (IEnumerable<Control>) group.Children)
          {
            if (this.IsControlVisible(child))
              this.CreateControlElement(columnElement, child.Id, child.ControlType, !LayoutConstants.WideControls.Contains(child.ControlType) ? child.Label : string.Empty, child.Watermark, child.Metadata, "(0,0,0,10)", child.ReadOnly.HasValue && child.ReadOnly.Value);
          }
        }
      }
    }

    private bool HasVisibleGroups(Section section) => section.Children.Any<Group>((Func<Group, bool>) (group => this.HasVisibleControls(group)));

    private bool HasVisibleControls(Group group) => group.Children.Any<Control>((Func<Control, bool>) (control => this.IsControlVisible(control)));

    private bool HasVisibleControls(Page page) => page.Children.SelectMany<Section, Group>((Func<Section, IEnumerable<Group>>) (section => (IEnumerable<Group>) section.Children)).SelectMany<Group, Control>((Func<Group, IEnumerable<Control>>) (group => (IEnumerable<Control>) group.Children)).Any<Control>((Func<Control, bool>) (control => this.IsControlVisible(control)));

    private bool IsControlVisible(Control control)
    {
      if (control.IsContribution)
        return false;
      return !control.Visible.HasValue || control.Visible.Value;
    }

    private XmlElement CreateLayoutElement(XmlElement parent)
    {
      XmlElement element = this.document.CreateElement(ProvisionTags.Layout);
      element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.LayoutHideReadOnlyEmptyFields, "true"));
      element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.LayoutHideControlBorders, "true"));
      parent.AppendChild((XmlNode) element);
      return element;
    }

    private XmlElement CreateTabGroupElement(XmlElement parent, string margin = null)
    {
      XmlElement element = this.document.CreateElement(ProvisionTags.TabGroup);
      if (!string.IsNullOrWhiteSpace(margin))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.Margin, margin));
      parent.AppendChild((XmlNode) element);
      return element;
    }

    private XmlElement CreateTabElement(XmlElement parent, string label)
    {
      XmlElement element = this.document.CreateElement(ProvisionTags.Tab);
      if (!string.IsNullOrWhiteSpace(label))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.Label, label));
      parent.AppendChild((XmlNode) element);
      return element;
    }

    private XmlElement CreateGroupElement(XmlElement parent, string label, string margin = null)
    {
      XmlElement element = this.document.CreateElement(ProvisionTags.Group);
      if (!string.IsNullOrWhiteSpace(label))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.Label, label));
      if (!string.IsNullOrWhiteSpace(margin))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.Margin, margin));
      parent.AppendChild((XmlNode) element);
      return element;
    }

    private XmlElement CreateColumnElement(XmlElement parent, int width)
    {
      XmlElement element = this.document.CreateElement(ProvisionTags.Column);
      if (!string.IsNullOrWhiteSpace(width.ToString()))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.ColumnPercentWidth, width.ToString()));
      parent.AppendChild((XmlNode) element);
      return element;
    }

    private XmlElement CreateControlElement(
      XmlElement parent,
      string fieldName,
      string type,
      string label,
      string watermark = null,
      string metadata = null,
      string margin = null,
      bool readOnly = false,
      string controlLabelPosition = null)
    {
      XmlElement element = this.document.CreateElement(ProvisionTags.Control);
      controlLabelPosition = string.IsNullOrEmpty(controlLabelPosition) ? ProvisionValues.ControlLabelPositionTop : controlLabelPosition;
      element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.Label, label));
      element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.ControlLabelPosition, controlLabelPosition));
      if (fieldName != null)
      {
        int num1;
        if (this.m_controlsInLayoutMap.TryGetValue(fieldName, out num1))
        {
          int num2;
          this.m_controlsInLayoutMap[fieldName] = num2 = num1 + 1;
          element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.ControlName, fieldName + num2.ToString()));
        }
        else
          this.m_controlsInLayoutMap[fieldName] = 1;
      }
      Control control;
      if (!string.IsNullOrWhiteSpace(fieldName) && this.m_coreControls.TryGetValue(fieldName, out control))
        readOnly = control.ReadOnly.HasValue && control.ReadOnly.Value;
      if (!string.IsNullOrWhiteSpace(fieldName))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.ControlFieldName, fieldName));
      if (!string.IsNullOrWhiteSpace(type))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.ControlType, type));
      if (!string.IsNullOrWhiteSpace(watermark))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.ControlEmptyText, watermark));
      if (!string.IsNullOrWhiteSpace(margin))
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.Margin, margin));
      if (readOnly)
        element.Attributes.Append(this.AddAttribute(element, ProvisionAttributes.ControlReadOnly, "True"));
      if (!string.IsNullOrWhiteSpace(metadata))
      {
        XmlDocument xmlDocument = new XmlDocument();
        try
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          using (StringReader input = new StringReader(metadata))
          {
            using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
              xmlDocument.Load(reader);
          }
          if (!string.IsNullOrEmpty(type) && string.Equals(WellKnownControlNames.LinksControl, type, StringComparison.OrdinalIgnoreCase))
            element.InnerXml = this.m_linksControlTransformer.ConvertLinksMetadataToLegacy(metadata);
          else
            element.InnerXml = metadata;
        }
        catch (XmlException ex)
        {
          if (this.m_tracer != null)
            this.m_tracer.TraceException(909803, TraceLevel.Warning, "FormLayout", "FormTransformsLayer", (Exception) ex);
        }
      }
      parent.AppendChild((XmlNode) element);
      return element;
    }

    private XmlAttribute AddAttribute(XmlElement element, string name, string value)
    {
      XmlAttribute attribute = this.document.CreateAttribute(name);
      attribute.Value = value;
      return attribute;
    }

    private string FormatMargin(int left, int top, int right, int bottom) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1},{2},{3})", (object) left, (object) top, (object) right, (object) bottom);

    private IDictionary<Section, int> GetSectionWidths(Page page)
    {
      IDictionary<Section, int> sectionWidths = this.GetSectionWidths(page, 50, 25);
      int num = sectionWidths.Values.Where<int>((Func<int, bool>) (i => i > 0)).Count<int>();
      switch (num)
      {
        case 0:
          sectionWidths[page.SectionFromIndex(0)] = 100;
          return sectionWidths;
        case 1:
          return this.GetSectionWidths(page, 100, 0);
        case 2:
          return this.GetSectionWidths(page, 67, 33);
        case 3:
          return sectionWidths;
        case 4:
          return this.GetSectionWidths(page, 40, 20);
        default:
          return this.GetSectionWidths(page, 30, 70 / (num - 1));
      }
    }

    private IDictionary<Section, int> GetSectionWidths(
      Page page,
      int firstSectionWidth,
      int remainingSectionWidths)
    {
      Dictionary<Section, int> sectionWidths = new Dictionary<Section, int>();
      bool flag = false;
      for (int index = 0; index < page.Children.Count; ++index)
      {
        Section section = page.SectionFromIndex(index);
        if (!this.HasVisibleGroups(section))
        {
          sectionWidths[section] = 0;
        }
        else
        {
          sectionWidths[section] = !flag ? firstSectionWidth : remainingSectionWidths;
          flag = true;
        }
      }
      return (IDictionary<Section, int>) sectionWidths;
    }
  }
}
