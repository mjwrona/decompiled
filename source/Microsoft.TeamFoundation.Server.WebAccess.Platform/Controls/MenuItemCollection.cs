// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuItemCollection
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class MenuItemCollection : Collection<MenuItem>
  {
    public MenuItemCollection(Menu parent) => this.Parent = parent;

    public Menu Parent { get; set; }

    protected override void RemoveItem(int index)
    {
      this[index].Parent = (MenuBase) null;
      base.RemoveItem(index);
    }

    protected override void InsertItem(int index, MenuItem item)
    {
      item.Parent = (MenuBase) this.Parent;
      base.InsertItem(index, item);
    }

    protected override void SetItem(int index, MenuItem item)
    {
      item.Parent = (MenuBase) this.Parent;
      base.SetItem(index, item);
    }

    public void AddRange(IEnumerable<MenuItem> items)
    {
      if (items == null)
        return;
      foreach (MenuItem menuItem in items)
        this.Add(menuItem);
    }
  }
}
