// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.QueryItemUpdatePackageHelpers
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class QueryItemUpdatePackageHelpers
  {
    private IQueryProvisioningHelper m_provisioning;

    public QueryItemUpdatePackageHelpers(IQueryProvisioningHelper provisioning) => this.m_provisioning = provisioning;

    public XmlDocument Translate()
    {
      IEnumerable<Guid> dirtyQueryItems = this.m_provisioning.GetDirtyQueryItems();
      if (dirtyQueryItems == null || dirtyQueryItems.Count<Guid>() == 0)
        return (XmlDocument) null;
      XmlDocument xmlDoc = new XmlDocument();
      XmlElement element = xmlDoc.CreateElement("Package");
      xmlDoc.AppendChild((XmlNode) element);
      foreach (Guid guid in dirtyQueryItems)
      {
        if (this.m_provisioning.IsQueryDeleted(guid))
          this.GenerateDeleteItemXml(xmlDoc, element, guid);
        else if (this.m_provisioning.IsQueryNew(guid))
          this.GenerateNewItemXml(xmlDoc, element, guid);
        else
          this.GenerateUpdateItemXml(xmlDoc, element, guid);
      }
      return xmlDoc;
    }

    private void GenerateDeleteItemXml(XmlDocument xmlDoc, XmlElement rootNode, Guid itemId) => QueryItemHelper.Delete(rootNode, itemId);

    private void GenerateNewItemXml(XmlDocument xmlDoc, XmlElement rootNode, Guid itemId)
    {
      string ownerIdentifier = this.m_provisioning.GetOwnerIdentifier(itemId);
      string identityType = this.m_provisioning.GetIdentityType(itemId);
      string queryText = this.m_provisioning.GetQueryText(itemId);
      QueryItemHelper.Create(rootNode, itemId, this.m_provisioning.GetParentId(itemId), this.m_provisioning.GetName(itemId), queryText, ownerIdentifier, identityType);
      this.GenerateUpdateAclXml(xmlDoc, rootNode, itemId);
    }

    private void GenerateUpdateAclXml(XmlDocument xmlDoc, XmlElement rootNode, Guid itemId)
    {
      if (!this.m_provisioning.IsAccessControlListDirty(itemId))
        return;
      XmlElement element1 = xmlDoc.CreateElement("SetQueryItemAccessControlList");
      element1.SetAttribute("QueryID", itemId.ToString());
      element1.SetAttribute("InheritPermissions", this.m_provisioning.GetInheritPermissions(itemId).ToString());
      rootNode.AppendChild((XmlNode) element1);
      foreach (QueryAccessControlEntry accessControlEntry in this.m_provisioning.GetAccessControlEntries(itemId))
      {
        XmlElement element2 = xmlDoc.CreateElement("AccessControlEntry");
        element2.SetAttribute("IdentityType", accessControlEntry.IdentityType);
        element2.SetAttribute("Identifier", accessControlEntry.Identifier);
        element2.SetAttribute("Allow", accessControlEntry.Allow.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        element2.SetAttribute("Deny", accessControlEntry.Deny.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        element1.AppendChild((XmlNode) element2);
      }
    }

    private void GenerateUpdateItemXml(XmlDocument xmlDoc, XmlElement rootNode, Guid id)
    {
      if (this.m_provisioning.IsQueryDirtyShallow(id))
      {
        Guid parentId = this.m_provisioning.GetParentId(id, true);
        string name = this.m_provisioning.GetName(id, true);
        string ownerIdentifier = this.m_provisioning.GetOwnerIdentifier(id, true);
        string identityType = this.m_provisioning.GetIdentityType(id, true);
        string queryText = this.m_provisioning.GetQueryText(id, true);
        QueryItemHelper.Update(rootNode, id, parentId, name, queryText, ownerIdentifier, identityType);
      }
      this.GenerateUpdateAclXml(xmlDoc, rootNode, id);
    }
  }
}
