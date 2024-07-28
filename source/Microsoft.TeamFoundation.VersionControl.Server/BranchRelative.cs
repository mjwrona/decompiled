// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BranchRelative
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class BranchRelative : IBranchRelative
  {
    private Item m_branchToItem;
    private Item m_branchFromItem;
    private int m_relativeToItemId;
    private int m_relativeFromItemId;
    private bool m_isRequestedItem;
    private ChangeType m_branchToChangeType;

    [XmlAttribute("reltoid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int RelativeToItemId
    {
      get => this.m_relativeToItemId;
      set => this.m_relativeToItemId = value;
    }

    [XmlAttribute("relfromid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int RelativeFromItemId
    {
      get => this.m_relativeFromItemId;
      set => this.m_relativeFromItemId = value;
    }

    [XmlAttribute("reqstd")]
    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool IsRequestedItem
    {
      get => this.m_isRequestedItem;
      set => this.m_isRequestedItem = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Item BranchFromItem
    {
      get => this.m_branchFromItem;
      set => this.m_branchFromItem = value;
    }

    object IBranchRelative.BranchFromItem
    {
      get => (object) this.BranchFromItem;
      set => this.BranchFromItem = (Item) value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Item BranchToItem
    {
      get => this.m_branchToItem;
      set => this.m_branchToItem = value;
    }

    [XmlAttribute("bctype")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int BranchToChangeTypeEx
    {
      get => (int) this.m_branchToChangeType;
      set
      {
      }
    }

    internal ChangeType BranchToChangeType
    {
      get => this.m_branchToChangeType;
      set => this.m_branchToChangeType = value;
    }

    object IBranchRelative.BranchToItem
    {
      get => (object) this.BranchToItem;
      set => this.BranchToItem = (Item) value;
    }

    internal static BranchRelative[][] QueryBranches(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      ItemSpec[] items,
      VersionSpec version)
    {
      List<BranchRelative[]> branchRelativeArrayList = new List<BranchRelative[]>();
      Dictionary<int, List<long>> dictionary = new Dictionary<int, List<long>>();
      if (version == null)
        version = (VersionSpec) new LatestVersionSpec();
      for (int index = 0; index < items.Length; ++index)
      {
        ItemSpec itemSpec = items[index];
        DeletedState deletedState = itemSpec.DeletionId != 0 ? DeletedState.Deleted : DeletedState.NonDeleted;
        bool flag = false;
        List<Item> objList1 = new List<Item>();
        Func<VersionedItemComponent, ResultCollection> func;
        if (itemSpec.isServerItem || itemSpec.DeletionId != 0)
        {
          switch (version)
          {
            case WorkspaceVersionSpec _:
              Workspace versionWorkspace = ((WorkspaceVersionSpec) version).findWorkspace(versionControlRequestContext);
              flag = true;
              func = (Func<VersionedItemComponent, ResultCollection>) (db => db.QueryWorkspaceItems(versionWorkspace, itemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace), itemSpec.RecursionType, ItemType.Any, deletedState != 0, 0));
              break;
            case LabelVersionSpec _:
              LabelVersionSpec lv = version as LabelVersionSpec;
              func = (Func<VersionedItemComponent, ResultCollection>) (db => db.QueryLabelItems(lv.Label, ItemPathPair.FromServerItem(lv.Scope), itemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace), itemSpec.RecursionType, ItemType.Any, deletedState, 0));
              break;
            default:
              int versionChangeset = version.ToChangeset(versionControlRequestContext.RequestContext);
              func = (Func<VersionedItemComponent, ResultCollection>) (db => db.QueryItems(itemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace), versionChangeset, itemSpec.RecursionType, deletedState, ItemType.Any, 0));
              break;
          }
        }
        else
        {
          flag = true;
          func = (Func<VersionedItemComponent, ResultCollection>) (db => db.QueryWorkspaceItemsLocal(workspace, itemSpec.toLocalItem(versionControlRequestContext.RequestContext, workspace), itemSpec.RecursionType, ItemType.Any, false, 0, versionControlRequestContext.MaxSupportedServerPathLength));
        }
        using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
        {
          ResultCollection resultCollection = func(versionedItemComponent);
          ObjectBinder<DeterminedItem> current = resultCollection.GetCurrent<DeterminedItem>();
          current.MoveNext();
          string queryPath = current.Current.QueryPath;
          string filePattern = current.Current.FilePattern;
          resultCollection.NextResult();
          if (flag)
          {
            foreach (WorkspaceItem workspaceItem in resultCollection.GetCurrent<WorkspaceItem>().Items)
              objList1.Add((Item) workspaceItem);
          }
          else
            objList1 = resultCollection.GetCurrent<Item>().Items;
        }
        List<Item> objList2 = new List<Item>();
        foreach (Item obj in objList1)
        {
          if (itemSpec.postMatch(obj.ServerItem) && obj.MatchDeletedState(deletedState) && obj.HasPermission(versionControlRequestContext, VersionedItemPermissions.Read))
            objList2.Add(obj);
        }
        if (objList2.Count == 0)
        {
          branchRelativeArrayList.Add(Array.Empty<BranchRelative>());
        }
        else
        {
          int num1 = 1;
          foreach (Item obj in objList2)
          {
            List<BranchRelative> branchRelativeList = new List<BranchRelative>();
            dictionary.Clear();
            List<BranchRelative> items1;
            using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
              items1 = versionedItemComponent.QueryBranches(workspace, obj.ItemPathPair, obj.ChangesetId).GetCurrent<BranchRelative>().Items;
            foreach (BranchRelative branchRelative in items1)
            {
              if (branchRelative.BranchFromItem == null)
              {
                branchRelative.RelativeFromItemId = 0;
              }
              else
              {
                List<long> longList;
                if (dictionary.TryGetValue(branchRelative.BranchFromItem.ItemId, out longList))
                {
                  int num2 = 0;
                  int num3 = (branchRelative.BranchToChangeType & ChangeType.Rename) == ChangeType.Rename ? branchRelative.BranchFromItem.ChangesetId - 1 : branchRelative.BranchFromItem.ChangesetId;
                  foreach (long num4 in longList)
                  {
                    if (num4 >> 32 <= (long) num3)
                      num2 = (int) num4;
                    else
                      break;
                  }
                  branchRelative.RelativeFromItemId = num2;
                }
              }
              branchRelative.RelativeToItemId = num1++;
              if (!dictionary.ContainsKey(branchRelative.BranchToItem.ItemId))
                dictionary[branchRelative.BranchToItem.ItemId] = new List<long>();
              dictionary[branchRelative.BranchToItem.ItemId].Add((long) branchRelative.BranchToItem.ChangesetId << 32 | (long) branchRelative.RelativeToItemId & (long) uint.MaxValue);
              if (branchRelative.BranchFromItem != null && !branchRelative.BranchFromItem.HasPermission(versionControlRequestContext, VersionedItemPermissions.Read))
                branchRelative.BranchFromItem = (Item) null;
              if (!branchRelative.BranchToItem.HasPermission(versionControlRequestContext, VersionedItemPermissions.Read))
                branchRelative.BranchToItem = (Item) null;
              branchRelativeList.Add(branchRelative);
            }
            branchRelativeArrayList.Add(branchRelativeList.ToArray());
          }
        }
      }
      return branchRelativeArrayList.ToArray();
    }

    private delegate ResultCollection QueryItemFunc(VersionedItemComponent vic);
  }
}
