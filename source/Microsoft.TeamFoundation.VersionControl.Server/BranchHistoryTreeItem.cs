// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BranchHistoryTreeItem
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class BranchHistoryTreeItem : IBranchHistoryTreeItem
  {
    public BranchRelative Relative;
    public List<BranchHistoryTreeItem> Children;
    [XmlAttribute("lvl")]
    [DefaultValue(0)]
    public int Level;
    public BranchHistoryTreeItem Parent;

    public BranchHistoryTreeItem() => this.Children = new List<BranchHistoryTreeItem>();

    internal BranchHistoryTreeItem(
      BranchHistoryTreeItem ourParent,
      BranchRelative ourRelative,
      int ourLevel)
    {
      this.Parent = ourParent;
      this.Relative = ourRelative;
      this.Level = ourLevel;
      this.Children = new List<BranchHistoryTreeItem>();
    }

    void IBranchHistoryTreeItem.AddChild(IBranchHistoryTreeItem newChild) => this.Children.Add((BranchHistoryTreeItem) newChild);

    internal BranchHistoryTreeItem GetRequestedItem()
    {
      if (this.Relative.IsRequestedItem)
        return this;
      foreach (BranchHistoryTreeItem child in this.Children)
      {
        BranchHistoryTreeItem requestedItem = child.GetRequestedItem();
        if (requestedItem != null)
          return requestedItem;
      }
      return (BranchHistoryTreeItem) null;
    }

    IBranchHistoryTreeItem IBranchHistoryTreeItem.GetRequestedItem() => (IBranchHistoryTreeItem) this.GetRequestedItem();

    IBranchRelative IBranchHistoryTreeItem.Relative
    {
      get => (IBranchRelative) this.Relative;
      set => this.Relative = (BranchRelative) value;
    }

    ICollection IBranchHistoryTreeItem.Children => (ICollection) this.Children;

    int IBranchHistoryTreeItem.Level
    {
      get => this.Level;
      set => this.Level = value;
    }

    IBranchHistoryTreeItem IBranchHistoryTreeItem.Parent
    {
      get => (IBranchHistoryTreeItem) this.Parent;
      set => this.Parent = (BranchHistoryTreeItem) value;
    }
  }
}
