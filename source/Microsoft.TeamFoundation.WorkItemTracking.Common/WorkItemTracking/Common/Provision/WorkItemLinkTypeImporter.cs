// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.WorkItemLinkTypeImporter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemLinkTypeImporter
  {
    private IMetadataProvisioningHelper m_metadata;
    private Dictionary<string, WorkItemLinkTypeImporter.WorkItemLinkTypeEntry> m_processedWorkLinkTypes = new Dictionary<string, WorkItemLinkTypeImporter.WorkItemLinkTypeEntry>();
    private const string c_linkTypeElementName = "LinkType";
    private const string c_topologyAttribute = "Topology";

    public WorkItemLinkTypeImporter(IMetadataProvisioningHelper metadata) => this.m_metadata = metadata;

    public XmlDocument Translate(XmlElement workItemLinkTypeDefinition, bool insertsOnly)
    {
      XmlNodeList xmlNodeList = workItemLinkTypeDefinition.SelectNodes("LinkType");
      XmlDocument doc = new XmlDocument();
      XmlNode xmlNode = doc.AppendChild((XmlNode) doc.CreateElement("Package"));
      foreach (XmlElement listElement in xmlNodeList)
      {
        bool isUpdate = this.m_metadata.ExistsWorkItemLinkRefName(listElement.Attributes["ReferenceName"].Value);
        if (!(isUpdate & insertsOnly))
        {
          XmlElement newChild = this.TranslateLinkType(listElement, doc, isUpdate);
          if (newChild != null)
            xmlNode.AppendChild((XmlNode) newChild);
        }
      }
      return xmlNode.ChildNodes.Count <= 0 ? (XmlDocument) null : doc;
    }

    private XmlElement TranslateLinkType(XmlElement listElement, XmlDocument doc, bool isUpdate)
    {
      string str1 = listElement.Attributes["ReferenceName"].Value;
      XmlElement element = doc.CreateElement(isUpdate ? "UpdateWorkItemLinkType" : "InsertWorkItemLinkType");
      element.SetAttribute("ReferenceName", str1);
      string str2 = this.HandleForwardName(listElement, str1, element);
      int topology = this.HandleTopology(listElement, str1, element);
      string str3 = this.HandleReverseName(listElement, str1, str2, topology, element);
      if (isUpdate && TFStringComparer.WorkItemFieldReferenceName.Equals(str2, this.m_metadata.GetWorkItemLinkTypeForwardEndName(str1)) && TFStringComparer.WorkItemFieldReferenceName.Equals(str3, this.m_metadata.GetWorkItemLinkTypeReverseEndName(str1)))
        return (XmlElement) null;
      if (isUpdate && !this.m_metadata.EditAllowForWorkItemLinkType(str1))
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeCannotBeModified", (object) str1));
      this.CheckRefName(str1);
      this.m_processedWorkLinkTypes.Add(str1, new WorkItemLinkTypeImporter.WorkItemLinkTypeEntry(str1, str2, str3));
      return element;
    }

    private string HandleReverseName(
      XmlElement listElement,
      string refName,
      string forwardName,
      int topology,
      XmlElement xout)
    {
      string empty = string.Empty;
      bool flag1 = (topology & 4) == 4;
      string typeReverseEndName;
      if (listElement.HasAttribute("ReverseName"))
      {
        typeReverseEndName = listElement.Attributes["ReverseName"].Value;
        this.CheckFriendlyName(refName, typeReverseEndName);
        xout.SetAttribute("ReverseName", typeReverseEndName);
      }
      else
      {
        if (!this.m_metadata.GetWorkItemLinkTypeReferenceNames().Contains(refName))
        {
          if (flag1)
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeTargetRelationNameRequired", (object) refName));
          return forwardName;
        }
        typeReverseEndName = this.m_metadata.GetWorkItemLinkTypeReverseEndName(refName);
      }
      bool flag2 = TFStringComparer.WorkItemFieldReferenceName.Equals(forwardName, typeReverseEndName);
      if (flag1 & flag2)
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeDirectionalSourceTargetSame", (object) topology.ToString((IFormatProvider) CultureInfo.CurrentCulture), (object) refName));
      else if (!flag1 && !flag2)
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeNonDirectionalSourceTargetDifferent", (object) topology.ToString((IFormatProvider) CultureInfo.CurrentCulture), (object) refName));
      return typeReverseEndName;
    }

    private int HandleTopology(XmlElement listElement, string refName, XmlElement xout)
    {
      bool flag = false;
      if (this.m_metadata.GetWorkItemLinkTypeReferenceNames().Contains(refName))
        flag = true;
      int num;
      if (listElement.HasAttribute("Topology"))
      {
        num = this.TranslateTopology(listElement.Attributes["Topology"].Value);
        if (flag)
        {
          int itemLinkTypeRefName = this.m_metadata.GetTopologyForWorkItemLinkTypeRefName(refName);
          if (num != itemLinkTypeRefName)
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeCannotUpdateTopology", (object) refName));
        }
        else
          xout.SetAttribute("Rules", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      else
      {
        if (!flag)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeTopologyNotSpecified", (object) refName));
        num = this.m_metadata.GetTopologyForWorkItemLinkTypeRefName(refName);
      }
      return num;
    }

    private int TranslateTopology(string topology)
    {
      switch ((int) Enum.Parse(typeof (LinkTopology), topology))
      {
        case 0:
          return 0;
        case 1:
          return 4;
        case 2:
          return 12;
        case 3:
          return 28;
        default:
          return -1;
      }
    }

    private string HandleForwardName(XmlElement listElement, string refName, XmlElement xout)
    {
      string name = (string) null;
      if (listElement.HasAttribute("ForwardName"))
      {
        name = listElement.Attributes["ForwardName"].Value;
        this.CheckFriendlyName(refName, name);
        xout.SetAttribute("ForwardName", name);
      }
      else
      {
        if (this.m_metadata.ExistsWorkItemLinkRefName(refName))
          name = this.m_metadata.GetWorkItemLinkTypeForwardEndName(refName);
        if (string.IsNullOrEmpty(name))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeSourceRelationNameRequired", (object) refName));
      }
      return name;
    }

    private void CheckFriendlyName(string refName, string name)
    {
      if (!ValidationMethods.IsValidLinkTypeName(name))
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidLinkTypeName", (object) name));
      string nameByFriendlyName = this.m_metadata.GetWorkItemReferenceNameByFriendlyName(name);
      if (!string.IsNullOrEmpty(nameByFriendlyName) && !TFStringComparer.WorkItemFieldReferenceName.Equals(nameByFriendlyName, refName) && !this.m_processedWorkLinkTypes.ContainsKey(nameByFriendlyName))
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeNameConflict", (object) refName, (object) nameByFriendlyName, (object) name));
      foreach (WorkItemLinkTypeImporter.WorkItemLinkTypeEntry itemLinkTypeEntry in this.m_processedWorkLinkTypes.Values)
      {
        if (TFStringComparer.WorkItemFieldReferenceName.Equals(itemLinkTypeEntry.ForwardName, name) || TFStringComparer.WorkItemFieldReferenceName.Equals(itemLinkTypeEntry.ReverseName, name))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeNameConflict", (object) refName, (object) itemLinkTypeEntry.ReferenceName, (object) name));
      }
    }

    private void CheckRefName(string refName)
    {
      if (!ValidationMethods.IsValidLinkTypeReferenceNameForImport(refName))
      {
        if (ValidationMethods.IsSystemReferenceName(refName))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidLinkTypeReferenceNameSystem", (object) refName));
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidLinkTypeReferenceName", (object) refName));
      }
      foreach (int field in this.m_metadata.GetFields())
      {
        if (TFStringComparer.WorkItemFieldReferenceName.Equals(this.m_metadata.GetFieldReferenceName(field), refName))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeFieldRefNameConflict", (object) refName, (object) refName));
      }
      foreach (WorkItemLinkTypeImporter.WorkItemLinkTypeEntry itemLinkTypeEntry in this.m_processedWorkLinkTypes.Values)
      {
        if (TFStringComparer.WorkItemFieldReferenceName.Equals(itemLinkTypeEntry.ReferenceName, refName))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinkTypeRefNameConflict", (object) refName, (object) refName));
      }
    }

    private class WorkItemLinkTypeEntry
    {
      public readonly string ReferenceName;
      public readonly string ForwardName;
      public readonly string ReverseName;

      public WorkItemLinkTypeEntry(string refName, string forwardName, string reverseName)
      {
        this.ReferenceName = refName;
        this.ForwardName = forwardName;
        this.ReverseName = reverseName;
      }
    }
  }
}
