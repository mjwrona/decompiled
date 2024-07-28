// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.GlobalListsCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GlobalListsCollection
  {
    private IMetadataProvisioningHelper m_metadata;
    private Dictionary<string, HashSet<string>> m_newLists;
    private Dictionary<string, ListItem> m_existingLists;
    private Dictionary<UpdatePackageField, HashSet<int>> m_pendingValidation;

    public GlobalListsCollection(IMetadataProvisioningHelper provisioning)
    {
      this.m_metadata = provisioning;
      this.m_newLists = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) this.m_metadata.ServerStringComparer);
      this.m_pendingValidation = new Dictionary<UpdatePackageField, HashSet<int>>();
    }

    public void Add(XmlElement listElement, bool throwOnDuplicateGloballist = false)
    {
      string attribute = listElement.GetAttribute(ProvisionAttributes.GlobalListName);
      if (this.m_newLists.ContainsKey(attribute))
      {
        if (!throwOnDuplicateGloballist)
          return;
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateGlobalList", (object) attribute));
      }
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) this.m_metadata.ServerStringComparer);
      foreach (XmlNode childNode in listElement.ChildNodes)
      {
        if (childNode is XmlElement xmlElement)
        {
          string str = xmlElement.GetAttribute(ProvisionAttributes.ListItemValue).Trim();
          if (stringSet.Contains(str))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateListItem", (object) str));
          stringSet.Add(str);
        }
      }
      this.m_newLists[attribute] = stringSet;
    }

    public void SubmitChanges(UpdatePackage batch)
    {
      MetaID listID1 = batch.InsertConstant("299f07ef-6201-41b3-90fc-03eeb3977587", (MetaID) null);
      foreach (KeyValuePair<string, HashSet<string>> newList in this.m_newLists)
      {
        string constant1 = "*" + newList.Key;
        MetaID listID2 = batch.InsertConstant(constant1, listID1);
        foreach (string constant2 in newList.Value)
          batch.InsertConstant(constant2, listID2);
      }
    }

    public void CheckItemsValidity(string listName, UpdatePackageField field)
    {
      HashSet<string> stringSet;
      if (this.m_newLists.TryGetValue(listName, out stringSet))
      {
        foreach (string str in stringSet)
          field.NormalizeValue(str);
      }
      else
      {
        ListItem listItem;
        if (this.ExistingLists.TryGetValue(listName, out listItem))
        {
          HashSet<int> intSet;
          if (!this.m_pendingValidation.TryGetValue(field, out intSet))
          {
            intSet = new HashSet<int>();
            this.m_pendingValidation.Add(field, intSet);
          }
          intSet.Add(listItem.ConstId);
        }
        else
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorGlobalListNotFound", (object) listName));
      }
    }

    public void CheckPendingItemsValidity()
    {
      List<int> list = this.m_pendingValidation.Values.SelectMany<HashSet<int>, int>((Func<HashSet<int>, IEnumerable<int>>) (x => (IEnumerable<int>) x)).Distinct<int>().ToList<int>();
      if (!list.Any<int>())
        return;
      IDictionary<int, IEnumerable<ListItem>> dictionary = this.m_metadata.ExpandSetsOneLevel((IEnumerable<int>) list);
      foreach (KeyValuePair<UpdatePackageField, HashSet<int>> keyValuePair in this.m_pendingValidation)
      {
        foreach (int key in keyValuePair.Value)
          this.ValidateListItems(keyValuePair.Key, dictionary[key].Select<ListItem, string>((Func<ListItem, string>) (x => x.DisplayName)));
      }
    }

    private void ValidateListItems(UpdatePackageField field, IEnumerable<string> items)
    {
      foreach (string str in items)
        field.NormalizeValue(str);
    }

    private Dictionary<string, ListItem> ExistingLists
    {
      get
      {
        if (this.m_existingLists == null)
        {
          this.m_existingLists = new Dictionary<string, ListItem>();
          int id;
          if (this.m_metadata.FindConstByFullName("299f07ef-6201-41b3-90fc-03eeb3977587", false, out id))
          {
            foreach (ListItem listItem in this.m_metadata.ExpandSetsOneLevel(Enumerable.Repeat<int>(id, 1))[id])
            {
              if (listItem.DisplayName.Length > 0 && listItem.DisplayName[0] == '*')
                this.m_existingLists[listItem.DisplayName.Remove(0, 1)] = listItem;
            }
          }
        }
        return this.m_existingLists;
      }
    }
  }
}
