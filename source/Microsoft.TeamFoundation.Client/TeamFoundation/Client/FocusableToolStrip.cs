// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.FocusableToolStrip
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FocusableToolStrip : ToolStrip
  {
    private SelectToolStripLabel m_selector;

    public FocusableToolStrip()
    {
      this.m_selector = new SelectToolStripLabel();
      this.Items.Insert(0, (ToolStripItem) this.m_selector);
    }

    private ToolStripItem GetSelectedItem()
    {
      ToolStripItem selectedItem = (ToolStripItem) null;
      for (int index = 0; index < this.DisplayedItems.Count; ++index)
      {
        if (this.DisplayedItems[index].Selected)
        {
          selectedItem = this.DisplayedItems[index];
          break;
        }
      }
      return selectedItem;
    }

    public void SelectToolStrip() => this.m_selector.SelectToolStrip();

    [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
    protected override bool ProcessDialogKey(Keys keyData)
    {
      if (keyData != Keys.Return)
        return base.ProcessDialogKey(keyData);
      this.GetSelectedItem()?.PerformClick();
      base.ProcessDialogKey(Keys.Escape);
      return true;
    }
  }
}
