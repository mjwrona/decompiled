// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WinformsStyler
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WinformsStyler
  {
    public virtual void Style(ToolStrip toolStrip) => this.Style(toolStrip, true, true);

    public virtual void Style(ToolStrip toolStrip, bool preferVsipColors) => this.Style(toolStrip, preferVsipColors, true);

    public virtual void Style(
      ToolStrip toolStrip,
      bool preferVsipColors,
      bool overrideButtonSelectedColor)
    {
      toolStrip.Renderer = (ToolStripRenderer) new ToolStripProfessionalRenderer((ProfessionalColorTable) new VSColorTable())
      {
        RoundedEdges = false
      };
      toolStrip.ForeColor = UIHost.Colors.FormForeColor;
      toolStrip.ImageScalingSize = this.GetImageScalingSize();
    }

    public virtual void Style(ContextMenuStrip contextMenu)
    {
      contextMenu.Renderer = (ToolStripRenderer) new ToolStripProfessionalRenderer((ProfessionalColorTable) new VSColorTable())
      {
        RoundedEdges = false
      };
      contextMenu.ImageScalingSize = this.GetImageScalingSize();
    }

    public virtual void Style(DataGridView grid) => this.Style(grid, false);

    public virtual void Style(DataGridView grid, bool highlightColumnHeaders)
    {
      grid.EnableHeadersVisualStyles = false;
      grid.BackgroundColor = UIHost.Colors.GridBackColor;
      grid.ForeColor = UIHost.Colors.GridForeColor;
      grid.GridColor = UIHost.Colors.GridLineColor;
      grid.ColumnHeadersDefaultCellStyle.BackColor = UIHost.Colors.GridHeaderBackColor;
      grid.ColumnHeadersDefaultCellStyle.ForeColor = UIHost.Colors.GridHeaderForeColor;
      grid.RowHeadersDefaultCellStyle.BackColor = UIHost.Colors.GridHeaderBackColor;
      grid.RowHeadersDefaultCellStyle.ForeColor = UIHost.Colors.GridHeaderForeColor;
      grid.RowHeadersDefaultCellStyle.SelectionBackColor = UIHost.Colors.GridRowHeaderSelectionBackColor;
      grid.RowHeadersDefaultCellStyle.SelectionForeColor = UIHost.Colors.GridRowHeaderSelectionForeColor;
      grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UIHost.Colors.GridRowHeaderSelectionBackColor;
      grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UIHost.Colors.GridRowHeaderSelectionForeColor;
      grid.DefaultCellStyle.ForeColor = UIHost.Colors.GridForeColor;
      grid.DefaultCellStyle.BackColor = UIHost.Colors.GridBackColor;
      grid.DefaultCellStyle.SelectionBackColor = UIHost.Colors.HighlightBackColor;
      grid.DefaultCellStyle.SelectionForeColor = UIHost.Colors.HighlightForeColor;
      grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      if (!highlightColumnHeaders)
        return;
      grid.CellMouseEnter -= new DataGridViewCellEventHandler(this.grid_CellMouseEnter);
      grid.CellMouseLeave -= new DataGridViewCellEventHandler(this.grid_CellMouseLeave);
      this.UpdateGridHeaderCells(grid);
      grid.CellMouseEnter += new DataGridViewCellEventHandler(this.grid_CellMouseEnter);
      grid.CellMouseLeave += new DataGridViewCellEventHandler(this.grid_CellMouseLeave);
    }

    protected Size GetImageScalingSize() => new Size(16, 16).LogicalToDeviceUnits();

    private void UpdateGridHeaderCells(DataGridView grid)
    {
      foreach (DataGridViewColumn column in (BaseCollection) grid.Columns)
      {
        column.HeaderCell.Style.BackColor = UIHost.Colors.GridHeaderBackColor;
        column.HeaderCell.Style.ForeColor = UIHost.Colors.GridHeaderForeColor;
      }
    }

    private void grid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        if (!(sender is DataGridView dataGridView) || e.RowIndex != -1 || e.ColumnIndex == -1)
          return;
        DataGridViewColumn column = dataGridView.Columns[e.ColumnIndex];
        if (string.IsNullOrEmpty(column.HeaderText))
          return;
        column.HeaderCell.Style.BackColor = UIHost.Colors.GridHeaderBackColor;
        column.HeaderCell.Style.ForeColor = UIHost.Colors.GridHeaderForeColor;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        if (!(sender is DataGridView dataGridView) || e.RowIndex != -1 || e.ColumnIndex == -1)
          return;
        DataGridViewColumn column = dataGridView.Columns[e.ColumnIndex];
        if (string.IsNullOrEmpty(column.HeaderText))
          return;
        column.HeaderCell.Style.BackColor = UIHost.Colors.HighlightBackColor;
        column.HeaderCell.Style.ForeColor = UIHost.Colors.HighlightForeColor;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    public virtual void Style<T>(CustomCombo<T> combo, bool preferVsipColors) where T : Control, IComboHostedControl
    {
    }

    public virtual void Style(TreeView treeView)
    {
      if (!UIHost.IsVistaOrNewer || !Application.RenderWithVisualStyles)
        return;
      treeView.ShowLines = false;
      treeView.HotTracking = true;
      int lParam = (int) NativeMethods.SendMessage(treeView.Handle, 4397, (IntPtr) 0, (IntPtr) 0) | 4 | 64;
      NativeMethods.SendMessage(treeView.Handle, 4396, (IntPtr) 0, (IntPtr) lParam);
      NativeMethods.SetWindowTheme(treeView.Handle, "explorer", (string) null);
    }
  }
}
