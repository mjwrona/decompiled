// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LinksControlTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public class LinksControlTransformer
  {
    private readonly string[] c_linkColumns = new string[4]
    {
      "System.Id",
      "System.Title",
      "System.AssignedTo",
      "System.WorkItemType"
    };
    private readonly string[] c_defaultColumns = new string[3]
    {
      "System.State",
      "System.CreatedDate",
      ProvisionValues.ColumnLinkComment
    };

    public XmlElement ConvertToWebLayout(
      XmlElement legacyLinksControl,
      Func<string, XmlElement> createElement)
    {
      XmlElement webLayout = createElement(legacyLinksControl.Name);
      webLayout.SetAttribute(ProvisionAttributes.ControlName, legacyLinksControl.GetAttribute(ProvisionAttributes.ControlName));
      webLayout.SetAttribute(ProvisionAttributes.ControlType, legacyLinksControl.GetAttribute(ProvisionAttributes.ControlType));
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinksControlOptions);
      if (legacyLinksControl.SelectNodes(xpath).Count == 1)
      {
        XmlElement newChild = createElement(ProvisionTags.LinksControlOptions);
        webLayout.AppendChild((XmlNode) newChild);
        string scope;
        XmlElement workitemTypeFilters = this.ConvertToWebLayoutWorkitemTypeFilters(legacyLinksControl, createElement, out scope);
        if (!string.IsNullOrEmpty(scope))
          newChild.SetAttribute(ProvisionAttributes.WebLayoutWorkItemTypeFiltersScope, scope);
        if (workitemTypeFilters != null)
          newChild.AppendChild((XmlNode) workitemTypeFilters);
        XmlElement layoutLinkFilters = this.ConvertToWebLayoutLinkFilters(legacyLinksControl, createElement);
        if (layoutLinkFilters != null)
          newChild.AppendChild((XmlNode) layoutLinkFilters);
        XmlElement layoutLinkColumns = this.ConvertToWebLayoutLinkColumns(legacyLinksControl, createElement);
        if (layoutLinkColumns != null)
          newChild.AppendChild((XmlNode) layoutLinkColumns);
      }
      return webLayout;
    }

    internal string ConvertLinksMetadata(string metadata)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element = xmlDocument.CreateElement("control");
      element.InnerXml = metadata;
      return this.ConvertToWebLayout(element, new Func<string, XmlElement>(xmlDocument.CreateElement)).InnerXml;
    }

    private XmlElement ConvertToWebLayoutLinkColumns(
      XmlElement input,
      Func<string, XmlElement> createElement)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinkColumns);
      XmlNodeList xmlNodeList = input.SelectNodes(xpath);
      if (xmlNodeList.Count != 1)
        return (XmlElement) null;
      XmlElement layoutLinkColumns = createElement(ProvisionTags.WebLayoutLinkColumns);
      List<string> stringList = new List<string>();
      HashSet<string> collection = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
      collection.AddRange<string, HashSet<string>>((IEnumerable<string>) new string[4]
      {
        "System.Id",
        "System.Title",
        "System.AssignedTo",
        "System.WorkItemType"
      });
      foreach (XmlElement xmlElement in xmlNodeList[0].ChildNodes.OfType<XmlElement>())
      {
        if (!string.IsNullOrEmpty(xmlElement.Attributes[ProvisionAttributes.LinkAttribute]?.Value))
        {
          stringList.Add(xmlElement.Attributes[ProvisionAttributes.LinkAttribute].Value);
        }
        else
        {
          string str = xmlElement.Attributes[ProvisionAttributes.LinkRefName].Value;
          if (!collection.Contains(str))
            stringList.Add(str);
        }
      }
      foreach (string str in stringList)
      {
        XmlElement newChild = createElement(ProvisionTags.WebLayoutLinkColumn);
        newChild.SetAttribute(ProvisionAttributes.WebLayoutColumnName, str);
        layoutLinkColumns.AppendChild((XmlNode) newChild);
      }
      return layoutLinkColumns;
    }

    private XmlElement ConvertToWebLayoutLinkFilters(
      XmlElement input,
      Func<string, XmlElement> createElement)
    {
      IEnumerable<string> strings1 = this.TransformLegacyWorkItemLinkfilters(input);
      IEnumerable<string> strings2 = this.TransformLegacyExternalLinkfilters(input);
      if (strings1 == null && strings2 == null)
        return (XmlElement) null;
      if (strings1 == null)
        strings1 = (IEnumerable<string>) new string[1]
        {
          ProvisionValues.WebLayoutIncludeAllLinkTypes
        };
      else if (strings2 == null)
        strings2 = (IEnumerable<string>) new string[1]
        {
          ProvisionValues.WebLayoutIncludeAllLinkTypes
        };
      XmlElement layoutLinkFilters = createElement(ProvisionTags.WebLayoutLinkFilters);
      foreach (string str in strings1)
      {
        XmlElement newChild = createElement(ProvisionTags.WebLayoutWorkItemLinkFilter);
        newChild.SetAttribute(ProvisionAttributes.WebLayoutLinkType, str);
        layoutLinkFilters.AppendChild((XmlNode) newChild);
      }
      foreach (string str in strings2)
      {
        XmlElement newChild = createElement(ProvisionTags.WebLayoutExternalLinkFilter);
        newChild.SetAttribute(ProvisionAttributes.WebLayoutLinkType, str);
        layoutLinkFilters.AppendChild((XmlNode) newChild);
      }
      return layoutLinkFilters;
    }

    private IEnumerable<string> TransformLegacyExternalLinkfilters(XmlElement input)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.ExternalLinkFilters);
      XmlNodeList xmlNodeList = input.SelectNodes(xpath);
      if (xmlNodeList.Count != 1)
        return (IEnumerable<string>) null;
      XmlNode xmlNode1 = xmlNodeList[0];
      string x = xmlNode1.Attributes[ProvisionAttributes.FilterType].Value;
      if (VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.IncludeAll) || VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.Exclude))
        return (IEnumerable<string>) null;
      if (VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.ExcludeAll))
        return Enumerable.Empty<string>();
      List<string> stringList = new List<string>();
      if (!VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.Include))
        return (IEnumerable<string>) stringList;
      foreach (XmlNode xmlNode2 in xmlNode1.ChildNodes.OfType<XmlElement>())
      {
        string str = xmlNode2.Attributes[ProvisionAttributes.LinkType].Value;
        stringList.Add(str);
      }
      return (IEnumerable<string>) stringList;
    }

    private IEnumerable<string> TransformLegacyWorkItemLinkfilters(XmlElement input)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WorkItemLinkFilters);
      XmlNodeList xmlNodeList = input.SelectNodes(xpath);
      if (xmlNodeList.Count != 1)
        return (IEnumerable<string>) null;
      XmlNode xmlNode = xmlNodeList[0];
      string x1 = xmlNode.Attributes[ProvisionAttributes.FilterType].Value;
      if (VssStringComparer.XmlAttributeValue.Equals(x1, ProvisionValues.ExcludeAll))
        return Enumerable.Empty<string>();
      if (VssStringComparer.XmlAttributeValue.Equals(x1, ProvisionValues.IncludeAll) || VssStringComparer.XmlAttributeValue.Equals(x1, ProvisionValues.Exclude))
        return (IEnumerable<string>) null;
      List<string> stringList = new List<string>();
      if (VssStringComparer.XmlAttributeValue.Equals(x1, ProvisionValues.Include))
      {
        foreach (XmlElement xmlElement in xmlNode.ChildNodes.OfType<XmlElement>())
        {
          string str = xmlElement.Attributes[ProvisionAttributes.LinkType].Value;
          string x2 = xmlElement.Attributes[ProvisionAttributes.FilterOn]?.Value;
          if (VssStringComparer.XmlAttributeValue.Equals(x2, ProvisionValues.ForwardName))
            stringList.Add(str + ProvisionValues.ForwardNameSuffix);
          else if (VssStringComparer.XmlAttributeValue.Equals(x2, ProvisionValues.ReverseName))
            stringList.Add(str + ProvisionValues.ReverseNameSuffix);
          else
            stringList.Add(str);
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private XmlElement ConvertToWebLayoutWorkitemTypeFilters(
      XmlElement input,
      Func<string, XmlElement> createElement,
      out string scope)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WorkItemTypeFilters);
      XmlNodeList xmlNodeList = input.SelectNodes(xpath);
      scope = (string) null;
      if (xmlNodeList.Count != 1)
        return (XmlElement) null;
      XmlNode xmlNode = xmlNodeList[0];
      scope = xmlNode.Attributes[ProvisionAttributes.Scope]?.Value;
      string x = xmlNode.Attributes[ProvisionAttributes.FilterType]?.Value;
      List<string> stringList = new List<string>();
      if (VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.IncludeAll) || VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.Exclude))
        stringList = (List<string>) null;
      else if (VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.Include))
      {
        stringList = new List<string>();
        foreach (XmlElement xmlElement in xmlNode.ChildNodes.OfType<XmlElement>())
          stringList.Add(xmlElement.Attributes[ProvisionAttributes.WorkItemType].Value);
      }
      if (stringList == null)
        return (XmlElement) null;
      XmlElement workitemTypeFilters = createElement(ProvisionTags.WebLayoutWorkItemTypeFilters);
      foreach (string str in stringList)
      {
        XmlElement newChild = createElement(ProvisionTags.Filter);
        newChild.SetAttribute(ProvisionAttributes.WorkItemType, str);
        workitemTypeFilters.AppendChild((XmlNode) newChild);
      }
      return workitemTypeFilters;
    }

    public XmlElement ConvertToLegacy(
      XmlElement webLayoutLinksControl,
      Func<string, XmlElement> createElement)
    {
      XmlElement legacy = createElement(webLayoutLinksControl.Name);
      legacy.SetAttribute(ProvisionAttributes.ControlName, webLayoutLinksControl.GetAttribute(ProvisionAttributes.ControlName));
      legacy.SetAttribute(ProvisionAttributes.ControlType, webLayoutLinksControl.GetAttribute(ProvisionAttributes.ControlType));
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinksControlOptions);
      if (webLayoutLinksControl.SelectNodes(xpath).Count == 1)
      {
        XmlElement newChild1 = createElement(ProvisionTags.LinksControlOptions);
        legacy.AppendChild((XmlNode) newChild1);
        XmlElement workitemTypeFilters = this.ConvertToLegacyWorkitemTypeFilters(webLayoutLinksControl, createElement);
        if (workitemTypeFilters != null)
          newChild1.AppendChild((XmlNode) workitemTypeFilters);
        bool hasWorkItemLinkFilters = false;
        IEnumerable<XmlElement> legacyLinkFilters = this.ConvertToLegacyLinkFilters(webLayoutLinksControl, createElement, out hasWorkItemLinkFilters);
        if (legacyLinkFilters != null)
        {
          foreach (XmlElement newChild2 in legacyLinkFilters)
            newChild1.AppendChild((XmlNode) newChild2);
        }
        XmlElement legacyLinkColumns = this.ConvertToLegacyLinkColumns(webLayoutLinksControl, createElement, hasWorkItemLinkFilters);
        if (legacyLinkColumns != null)
          newChild1.AppendChild((XmlNode) legacyLinkColumns);
      }
      return legacy;
    }

    public void InjectLinkTypeFilter(Layout layout, LinkFilterType linkFilterType)
    {
      if (!layout.Children.Any<Page>())
        return;
      Section linkSection = this.GetLinkSection(layout);
      Group group = (Group) null;
      switch (linkFilterType)
      {
        case LinkFilterType.GitHub:
          group = WellKnownProcessLayout.GetDevelopmentLinksControlGroup(linkSection);
          break;
        case LinkFilterType.RemoteLinks:
        case LinkFilterType.GitHubIssue:
          group = WellKnownProcessLayout.GetRelatedWorkItemLinksControlGroup(linkSection);
          break;
      }
      if (group == null || group.Children.Count <= 0)
        return;
      Control control = group.Children.First<Control>();
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element1 = xmlDocument.CreateElement("control");
      XmlDocumentFragment documentFragment = xmlDocument.CreateDocumentFragment();
      documentFragment.InnerXml = control.Metadata;
      element1.AppendChild((XmlNode) documentFragment);
      XmlNodeList xmlNodeList = element1.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutLinkFilters));
      if (xmlNodeList.Count != 1)
        return;
      XmlNode xmlNode = xmlNodeList[0];
      switch (linkFilterType)
      {
        case LinkFilterType.GitHub:
          bool flag1 = false;
          foreach (XmlElement childNode in xmlNode.ChildNodes)
          {
            string attribute = childNode.GetAttribute(ProvisionAttributes.WebLayoutLinkType);
            if (attribute == "GitHub Pull Request" || attribute == "GitHub Commit")
            {
              flag1 = true;
              break;
            }
          }
          if (!flag1)
          {
            XmlElement element2 = xmlDocument.CreateElement(ProvisionTags.WebLayoutExternalLinkFilter);
            element2.SetAttribute(ProvisionAttributes.WebLayoutLinkType, "GitHub Pull Request");
            xmlNode.AppendChild((XmlNode) element2);
            XmlElement element3 = xmlDocument.CreateElement(ProvisionTags.WebLayoutExternalLinkFilter);
            element3.SetAttribute(ProvisionAttributes.WebLayoutLinkType, "GitHub Commit");
            xmlNode.AppendChild((XmlNode) element3);
            break;
          }
          break;
        case LinkFilterType.RemoteLinks:
        case LinkFilterType.GitHubIssue:
          bool flag2 = false;
          bool flag3 = false;
          foreach (XmlElement childNode in xmlNode.ChildNodes)
          {
            string attribute = childNode.GetAttribute(ProvisionAttributes.WebLayoutLinkType);
            if (attribute == "System.LinkTypes.Remote.Dependency" || attribute == "System.LinkTypes.Remote.Related")
              flag2 = true;
            if (attribute == "GitHub Issue")
              flag3 = true;
          }
          if (!flag2 && linkFilterType == LinkFilterType.RemoteLinks)
          {
            XmlElement element4 = xmlDocument.CreateElement(ProvisionTags.WebLayoutWorkItemLinkFilter);
            element4.SetAttribute(ProvisionAttributes.WebLayoutLinkType, "System.LinkTypes.Remote.Related");
            xmlNode.AppendChild((XmlNode) element4);
            XmlElement element5 = xmlDocument.CreateElement(ProvisionTags.WebLayoutWorkItemLinkFilter);
            element5.SetAttribute(ProvisionAttributes.WebLayoutLinkType, "System.LinkTypes.Remote.Dependency");
            xmlNode.AppendChild((XmlNode) element5);
          }
          if (!flag3 && linkFilterType == LinkFilterType.GitHubIssue)
          {
            XmlElement element6 = xmlDocument.CreateElement(ProvisionTags.WebLayoutExternalLinkFilter);
            element6.SetAttribute(ProvisionAttributes.WebLayoutLinkType, "GitHub Issue");
            xmlNode.AppendChild((XmlNode) element6);
            break;
          }
          break;
      }
      control.Metadata = element1.InnerXml;
    }

    internal string ConvertLinksMetadataToLegacy(string metadata)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element = xmlDocument.CreateElement("control");
      element.InnerXml = metadata;
      return this.ConvertToLegacy(element, new Func<string, XmlElement>(xmlDocument.CreateElement)).InnerXml;
    }

    private Section GetLinkSection(Layout layout) => layout.Children.FirstOrDefault<Page>((Func<Page, bool>) (page => page.PageType == PageType.Custom && !page.IsContribution))?.FindDescendant<Section>(string.Format("Section{0}", (object) 3));

    private XmlElement ConvertToLegacyLinkColumns(
      XmlElement input,
      Func<string, XmlElement> createElement,
      bool hasWorkItemLinkFilters)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutLinkColumns);
      XmlNodeList xmlNodeList = input.SelectNodes(xpath);
      List<string> source = new List<string>((IEnumerable<string>) this.c_linkColumns);
      if (xmlNodeList.Count == 0 || xmlNodeList[0].ChildNodes.Count == 0)
      {
        source.AddRange((IEnumerable<string>) this.c_defaultColumns);
      }
      else
      {
        foreach (XmlNode xmlNode in xmlNodeList[0].ChildNodes.OfType<XmlElement>())
        {
          string str = xmlNode.Attributes[ProvisionAttributes.WebLayoutColumnName].Value;
          if (!source.Contains<string>(str, (IEqualityComparer<string>) VssStringComparer.XmlAttributeValue))
            source.Add(str);
        }
      }
      if (!hasWorkItemLinkFilters)
      {
        HashSet<string> allowedColumns = new HashSet<string>((IEnumerable<string>) new string[3]
        {
          "System.Id",
          "System.Title",
          ProvisionValues.ColumnLinkComment
        }, (IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
        source.RemoveAll((Predicate<string>) (columnName => !allowedColumns.Contains(columnName)));
      }
      XmlElement legacyLinkColumns = createElement(ProvisionTags.LinkColumns);
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) new string[3]
      {
        ProvisionValues.ColumnTargetDescription,
        ProvisionValues.ColumnLinkComment,
        ProvisionValues.ColumnLinkType
      }, (IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
      foreach (string str in source)
      {
        XmlElement newChild = createElement(ProvisionTags.LinkColumn);
        if (stringSet.Contains(str))
          newChild.SetAttribute(ProvisionAttributes.LinkAttribute, str);
        else
          newChild.SetAttribute(ProvisionAttributes.LinkRefName, str);
        legacyLinkColumns.AppendChild((XmlNode) newChild);
      }
      return legacyLinkColumns;
    }

    private IEnumerable<XmlElement> ConvertToLegacyLinkFilters(
      XmlElement input,
      Func<string, XmlElement> createElement,
      out bool hasWorkItemLinkFilters)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutLinkFilters);
      XmlNodeList xmlNodeList = input.SelectNodes(xpath);
      hasWorkItemLinkFilters = true;
      if (xmlNodeList.Count != 1)
        return (IEnumerable<XmlElement>) null;
      XmlNode xmlNode = xmlNodeList[0];
      List<string> stringList = new List<string>();
      Dictionary<string, int> dictionary = new Dictionary<string, int>((IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
      bool flag1 = false;
      bool flag2 = false;
      hasWorkItemLinkFilters = false;
      foreach (XmlElement xmlElement in xmlNode.ChildNodes.OfType<XmlElement>())
      {
        string x = xmlElement.Attributes[ProvisionAttributes.WebLayoutLinkType].Value;
        if (VssStringComparer.XmlNodeName.Compare(xmlElement.Name, ProvisionTags.WebLayoutExternalLinkFilter) == 0)
        {
          if (VssStringComparer.XmlAttributeValue.Compare(x, ProvisionValues.WebLayoutIncludeAllLinkTypes) == 0)
            flag1 = true;
          else
            stringList.Add(x);
        }
        else
        {
          hasWorkItemLinkFilters = true;
          if (VssStringComparer.XmlAttributeValue.Compare(x, ProvisionValues.WebLayoutIncludeAllLinkTypes) == 0)
          {
            flag2 = true;
          }
          else
          {
            int num = 2;
            int length = x.IndexOf("-Forward", StringComparison.InvariantCultureIgnoreCase);
            if (length >= 0)
            {
              num = 1;
            }
            else
            {
              length = x.IndexOf("-Reverse", StringComparison.InvariantCultureIgnoreCase);
              if (length >= 0)
                num = 0;
            }
            string key = length >= 0 ? x.Substring(0, length) : x;
            dictionary[key] = !dictionary.ContainsKey(key) ? num : 2;
          }
        }
      }
      XmlElement xmlElement1 = createElement(ProvisionTags.ExternalLinkFilters);
      if (flag1)
        xmlElement1 = (XmlElement) null;
      else if (stringList.Count == 0)
      {
        xmlElement1.SetAttribute(ProvisionAttributes.FilterType, ProvisionValues.ExcludeAll);
      }
      else
      {
        xmlElement1.SetAttribute(ProvisionAttributes.FilterType, ProvisionValues.Include);
        foreach (string str in stringList)
        {
          XmlElement newChild = createElement(ProvisionTags.Filter);
          newChild.SetAttribute(ProvisionAttributes.LinkType, str);
          xmlElement1.AppendChild((XmlNode) newChild);
        }
      }
      XmlElement xmlElement2 = createElement(ProvisionTags.WorkItemLinkFilters);
      if (flag2)
        xmlElement2 = (XmlElement) null;
      else if (dictionary.Count == 0)
      {
        xmlElement2.SetAttribute(ProvisionAttributes.FilterType, ProvisionValues.ExcludeAll);
      }
      else
      {
        xmlElement2.SetAttribute(ProvisionAttributes.FilterType, ProvisionValues.Include);
        foreach (KeyValuePair<string, int> keyValuePair in dictionary)
        {
          string key = keyValuePair.Key;
          XmlElement newChild = createElement(ProvisionTags.Filter);
          newChild.SetAttribute(ProvisionAttributes.LinkType, key);
          if (keyValuePair.Value == 1)
            newChild.SetAttribute(ProvisionAttributes.FilterOn, ProvisionValues.ForwardName);
          else if (keyValuePair.Value == 0)
            newChild.SetAttribute(ProvisionAttributes.FilterOn, ProvisionValues.ReverseName);
          xmlElement2.AppendChild((XmlNode) newChild);
        }
      }
      return ((IEnumerable<XmlElement>) new XmlElement[2]
      {
        xmlElement2,
        xmlElement1
      }).Where<XmlElement>((Func<XmlElement, bool>) (item => item != null));
    }

    private XmlElement ConvertToLegacyWorkitemTypeFilters(
      XmlElement input,
      Func<string, XmlElement> createElement)
    {
      string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinksControlOptions);
      string str = (string) null;
      XmlNodeList xmlNodeList1 = input.SelectNodes(xpath1);
      if (xmlNodeList1.Count > 0)
        str = xmlNodeList1[0].Attributes[ProvisionAttributes.WebLayoutWorkItemTypeFiltersScope]?.Value;
      string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutWorkItemTypeFilters);
      XmlNodeList xmlNodeList2 = input.SelectNodes(xpath2);
      XmlElement workitemTypeFilters = createElement(ProvisionTags.WorkItemTypeFilters);
      if (!string.IsNullOrEmpty(str))
        workitemTypeFilters.SetAttribute(ProvisionAttributes.Scope, str);
      if (xmlNodeList2.Count != 1)
      {
        if (string.IsNullOrEmpty(str))
          return (XmlElement) null;
        workitemTypeFilters.SetAttribute(ProvisionAttributes.FilterType, ProvisionValues.IncludeAll);
        return workitemTypeFilters;
      }
      IEnumerable<XmlElement> source = xmlNodeList2[0].ChildNodes.OfType<XmlElement>();
      if (!source.Any<XmlElement>())
      {
        workitemTypeFilters.SetAttribute(ProvisionAttributes.FilterType, ProvisionValues.ExcludeAll);
      }
      else
      {
        workitemTypeFilters.SetAttribute(ProvisionAttributes.FilterType, ProvisionValues.Include);
        foreach (XmlElement xmlElement in source)
        {
          XmlElement newChild = createElement(ProvisionTags.Filter);
          newChild.SetAttribute(ProvisionAttributes.WorkItemType, xmlElement.Attributes[ProvisionAttributes.WorkItemType].Value);
          workitemTypeFilters.AppendChild((XmlNode) newChild);
        }
      }
      return workitemTypeFilters;
    }
  }
}
