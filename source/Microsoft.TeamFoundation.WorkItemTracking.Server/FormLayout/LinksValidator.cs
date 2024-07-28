// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LinksValidator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public static class LinksValidator
  {
    public const string IntegratedInBuildLink = "Integrated in build";
    public const string FoundInBuildLink = "Found in build";
    public const string GitHubPullRequest = "GitHub Pull Request";
    public const string GitHubCommit = "GitHub Commit";
    public const string GitHubIssue = "GitHub Issue";

    public static void Validate(
      IMetadataProvisioningHelper metadataProvisiongHelper,
      XmlNodeList linksControlNodes)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.XmlNodeName);
      string empty = string.Empty;
      for (int i = 0; i < linksControlNodes.Count; ++i)
      {
        XmlElement linksControlNode = (XmlElement) linksControlNodes[i];
        string attribute1 = linksControlNode.GetAttribute(ProvisionAttributes.ControlType);
        XmlElement parentNode = (XmlElement) linksControlNode.ParentNode;
        if (parentNode.ChildNodes.Count > 1)
        {
          string message = InternalsResourceStrings.Format("MultipleControlsInLinksGroupException", (object) parentNode.GetAttribute(ProvisionAttributes.Label));
          metadataProvisiongHelper.ThrowValidationException(message);
        }
        if (!string.IsNullOrEmpty(attribute1) && VssStringComparer.XmlAttributeValue.Equals(WellKnownControlNames.LinksControl, attribute1))
        {
          string attribute2 = linksControlNode.GetAttribute(ProvisionAttributes.ControlName);
          string key = attribute2 == null ? string.Empty : attribute2;
          if (dictionary.ContainsKey(key))
          {
            string message = InternalsResourceStrings.Format("LinksControlDuplicateName", (object) key);
            metadataProvisiongHelper.ThrowValidationException(message);
          }
          dictionary[key] = (object) null;
          string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutLinkFilters);
          XmlNodeList xmlNodeList1 = linksControlNode.SelectNodes(xpath1);
          if (xmlNodeList1.Count == 1)
            LinksValidator.ValidateWebLayoutLinkFilters(xmlNodeList1[0], metadataProvisiongHelper);
          string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutLinkColumns);
          XmlNodeList xmlNodeList2 = linksControlNode.SelectNodes(xpath2);
          if (xmlNodeList2.Count == 1)
            LinksValidator.ValidateLinkColumns(xmlNodeList2[0], metadataProvisiongHelper);
          string xpath3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutWorkItemTypeFilters);
          XmlNodeList xmlNodeList3 = linksControlNode.SelectNodes(xpath3);
          if (xmlNodeList3.Count == 1)
            LinksValidator.ValidateWorkItemTypeFilters(xmlNodeList3[0], metadataProvisiongHelper);
        }
      }
    }

    private static void ValidateWorkItemTypeFilters(
      XmlNode node,
      IMetadataProvisioningHelper metadata)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Filter);
      XmlNodeList xmlNodeList = node.SelectNodes(xpath);
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.HtmlAttributeValue);
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        string key = xmlNode.Attributes[ProvisionAttributes.WorkItemType].Value.Trim();
        if (dictionary.ContainsKey(key))
          metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlDuplicateWITFilterName", (object) key));
        dictionary[key] = (object) null;
      }
    }

    private static void ValidateLinkColumns(XmlNode node, IMetadataProvisioningHelper metadata)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutLinkColumn);
      XmlNodeList xmlNodeList = node.SelectNodes(xpath);
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        if (xmlNode.Attributes.Count != 1)
          metadata.ThrowValidationException(InternalsResourceStrings.Format("WebLayoutLinksControlLinkColumnInvalidAttributes"));
        string name = xmlNode.Attributes[0].Name;
        if (!string.Equals(ProvisionAttributes.WebLayoutColumnName, name, StringComparison.InvariantCultureIgnoreCase))
          metadata.ThrowValidationException(InternalsResourceStrings.Format("WebLayoutLinksControlLinkColumnInvalidAttributes"));
        string key = xmlNode.Attributes[0].Value;
        if (dictionary.ContainsKey(key))
          metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlLinkColumnDuplicateColumn", (object) key));
        dictionary[key] = (object) null;
      }
    }

    private static void ValidateWebLayoutLinkFilters(
      XmlNode node,
      IMetadataProvisioningHelper metadata)
    {
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) metadata.GetWorkItemLinkTypeReferenceNames(), (IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
      LinksValidator.ValidateWebLayoutLinks(node, metadata, ProvisionTags.WebLayoutWorkItemLinkFilter);
      LinksValidator.ValidateWebLayoutLinks(node, metadata, ProvisionTags.WebLayoutExternalLinkFilter);
    }

    private static void ValidateWebLayoutLinks(
      XmlNode node,
      IMetadataProvisioningHelper metadata,
      string linkTagName)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) linkTagName);
      XmlNodeList xmlNodeList = node.SelectNodes(xpath);
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
      bool isExternal = VssStringComparer.XmlNodeName.Equals(linkTagName, ProvisionTags.WebLayoutExternalLinkFilter);
      HashSet<string> allExternalLinkTypes = (HashSet<string>) null;
      if (isExternal)
        allExternalLinkTypes = new HashSet<string>((IEnumerable<string>) metadata.GetRegisteredLinkTypes(), (IEqualityComparer<string>) VssStringComparer.XmlAttributeValue);
      bool flag = false;
      Func<string, bool> func = (Func<string, bool>) (name => !isExternal || allExternalLinkTypes.Contains(name));
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        string str1 = xmlNode.Attributes[ProvisionAttributes.WebLayoutLinkType].Value;
        if (flag)
        {
          string str2;
          if (!isExternal)
            str2 = InternalsResourceStrings.Format("ErrorDuplicateWorkItemLink", (object) str1);
          else
            str2 = InternalsResourceStrings.Format("ErrorDuplicateExternalLink", (object) str1);
          string message = str2;
          metadata.ThrowValidationException(message);
        }
        flag = VssStringComparer.XmlAttributeValue.Equals(str1, ProvisionValues.WebLayoutIncludeAllLinkTypes);
        if (dictionary.ContainsKey(str1))
          metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlDuplicateFilterName", (object) str1));
        else if (!flag && !func(str1))
        {
          if (LinksValidator.DataImportWhitelistedLinkTypes.Contains(str1))
            metadata.ThrowValidationException(InternalsResourceStrings.Format("DataImportWhitelistedLinksControlUnknownFilterName", (object) str1));
          else
            metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlUnknownFilterName", (object) str1));
        }
        dictionary[str1] = (object) null;
      }
    }

    public static HashSet<string> DataImportWhitelistedLinkTypes => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "Integrated in build",
      "Found in build",
      "GitHub Pull Request",
      "GitHub Commit",
      "GitHub Issue"
    };
  }
}
