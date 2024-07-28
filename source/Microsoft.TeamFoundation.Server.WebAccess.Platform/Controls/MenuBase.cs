// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public abstract class MenuBase : ControlBase
  {
    public const string ClassBowtie = "bowtie-menus";
    private MenuItemCollection m_children;

    protected MenuBase(string tagName)
      : base(tagName)
    {
    }

    public MenuBase Parent { get; set; }

    public MenuOwner Owner
    {
      get
      {
        MenuOwner owner = this as MenuOwner;
        for (MenuBase parent = this.Parent; owner == null && parent != null; parent = parent.Parent)
          owner = parent as MenuOwner;
        return owner;
      }
    }

    public bool IsOwner => this.Equals((object) this.Owner);

    public MenuItemCollection ChildItems
    {
      get
      {
        if (this.m_children == null)
          this.m_children = this.FillChildItems();
        return this.m_children;
      }
    }

    public bool RenderGrandchildren { get; set; }

    protected bool HasChildren => this.m_children != null && this.m_children.Count > 0;

    public bool IsBowtieMenu => this.CssClass != null && this.CssClass.Contains("bowtie-menus");

    protected abstract MenuItemCollection FillChildItems();
  }
}
