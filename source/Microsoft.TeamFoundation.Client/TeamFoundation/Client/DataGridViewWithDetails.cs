// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.DataGridViewWithDetails
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DataGridViewWithDetails : DataGridView
  {
    private int m_detailsRowIndex = -2;
    private int m_detailsRowOrigHeight;
    private DataGridViewContentAlignment m_detailsRowOrigAlignment;
    private Panel m_detailsPanel;
    private Panel m_detailsTopGridLine;
    private Timer m_detailsTimer;
    private bool m_detailsInitialExpand;
    private bool m_detailsInitialExpansionIgnored;
    private bool m_detailsAutoExpand;
    private int m_detailsAutoExpandDelay = 400;
    private int m_detailsImageColumnIndex = -2;
    private DataGridViewDetailsImageMode m_detailsImageMode;
    private Image m_detailsExpandedImage;
    private Image m_detailsCollapsedImage;
    private UserControl m_detailsHostedControl;
    private bool m_selectionChanged;
    private bool m_selectionChangeInvalidatesDetails;
    private int m_skipUpdateDetailsPanel;
    private int m_correctRowMinHeight;

    public event EventHandler<DataGridViewDetailsEventArgs> BeforeExpandDetails;

    public event EventHandler<DataGridViewDetailsEventArgs> AfterExpandDetails;

    public event EventHandler<DataGridViewDetailsEventArgs> BeforeCollapseDetails;

    public event EventHandler<DataGridViewDetailsEventArgs> AfterCollapseDetails;

    public DataGridViewWithDetails()
    {
      this.m_detailsPanel = new Panel();
      this.m_detailsPanel.Parent = (Control) this;
      this.m_detailsPanel.Visible = false;
      this.m_detailsPanel.BackColor = SystemColors.Window;
      this.m_detailsTopGridLine = new Panel();
      this.m_detailsTopGridLine.Parent = (Control) this.m_detailsPanel;
      this.m_detailsTopGridLine.Height = this.GridLineHeight;
      this.m_detailsTopGridLine.Dock = DockStyle.Top;
      this.m_detailsTopGridLine.BackColor = this.GridColor;
      this.m_detailsTimer = new Timer();
      if (this.m_detailsAutoExpandDelay != 0)
        this.m_detailsTimer.Interval = this.m_detailsAutoExpandDelay;
      this.m_detailsTimer.Tick += new EventHandler(this.detailsTimer_Tick);
      this.VerticalScrollBar.VisibleChanged += new EventHandler(this.VerticalScrollBar_VisibleChanged);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.VerticalScrollBar.VisibleChanged -= new EventHandler(this.VerticalScrollBar_VisibleChanged);
        if (this.m_detailsPanel != null)
        {
          this.m_detailsPanel.Dispose();
          this.m_detailsPanel = (Panel) null;
        }
        if (this.m_detailsTopGridLine != null)
        {
          this.m_detailsTopGridLine.Dispose();
          this.m_detailsTopGridLine = (Panel) null;
        }
        if (this.m_detailsTimer != null)
        {
          this.m_detailsTimer.Stop();
          this.m_detailsTimer.Dispose();
          this.m_detailsTimer = (Timer) null;
        }
      }
      base.Dispose(disposing);
    }

    [Browsable(true)]
    [Category("Details Panel")]
    [DefaultValue(false)]
    public bool InitialExpandDetails
    {
      get => this.m_detailsInitialExpand;
      set => this.m_detailsInitialExpand = value;
    }

    [Browsable(true)]
    [Category("Details Panel")]
    [DefaultValue(false)]
    public bool SelectionChangeInvalidatesDetails
    {
      get => this.m_selectionChangeInvalidatesDetails;
      set => this.m_selectionChangeInvalidatesDetails = value;
    }

    [Browsable(true)]
    [Category("Details Panel")]
    [DefaultValue(false)]
    public bool AutoExpandDetails
    {
      get => this.m_detailsAutoExpand;
      set
      {
        this.m_detailsAutoExpand = value;
        if (this.m_detailsAutoExpand)
          return;
        this.m_detailsTimer.Stop();
      }
    }

    [Browsable(true)]
    [Category("Details Panel")]
    [DefaultValue(400)]
    public int AutoExpandDetailsDelay
    {
      get => this.m_detailsAutoExpandDelay;
      set
      {
        this.m_detailsAutoExpandDelay = value;
        if (this.m_detailsAutoExpandDelay == 0)
          return;
        this.m_detailsTimer.Interval = value;
      }
    }

    [Browsable(false)]
    [DefaultValue(null)]
    public UserControl DetailsControl
    {
      get => this.m_detailsHostedControl;
      set
      {
        this.CollapseDetails();
        this.m_detailsHostedControl = value;
        if (this.m_detailsHostedControl == null)
          return;
        this.m_detailsHostedControl.AutoSize = false;
        this.m_detailsHostedControl.Parent = (Control) this.m_detailsPanel;
        this.m_detailsHostedControl.Dock = DockStyle.Fill;
      }
    }

    [Browsable(true)]
    [Category("Details Panel")]
    public Color DetailsBackColor
    {
      get => this.m_detailsPanel.BackColor;
      set => this.m_detailsPanel.BackColor = value;
    }

    [Browsable(false)]
    public int DetailsRow => this.m_detailsRowIndex;

    [Browsable(true)]
    [Category("Details Panel")]
    [DefaultValue(-2)]
    public int DetailsImageColumnIndex
    {
      get => this.m_detailsImageColumnIndex;
      set => this.m_detailsImageColumnIndex = value;
    }

    [Browsable(true)]
    [Category("Details Panel")]
    [DefaultValue(DataGridViewDetailsImageMode.None)]
    public DataGridViewDetailsImageMode DetailsImageMode
    {
      get => this.m_detailsImageMode;
      set => this.m_detailsImageMode = value;
    }

    [Browsable(false)]
    public Image DetailsCollapsedImage
    {
      get => this.m_detailsCollapsedImage;
      set => this.m_detailsCollapsedImage = value;
    }

    [Browsable(false)]
    public Image DetailsExpandedImage
    {
      get => this.m_detailsExpandedImage;
      set => this.m_detailsExpandedImage = value;
    }

    protected bool IsExpandDelayed => this.m_detailsTimer != null && this.m_detailsAutoExpandDelay != 0;

    public void ExpandDetails()
    {
      DataGridViewRow currentRow = this.CurrentRow;
      if (currentRow == null)
        return;
      this.ExpandDetails(currentRow.Index);
    }

    public void ExpandDetails(int rowIndex) => this.ExpandDetails(rowIndex, false);

    private void ExpandDetails(int rowIndex, bool userInitiated)
    {
      if (this.m_detailsHostedControl == null || rowIndex < 0 || rowIndex >= this.Rows.Count || rowIndex == this.m_detailsRowIndex && !this.m_selectionChanged)
        return;
      try
      {
        DataGridViewDetailsEventArgs e = new DataGridViewDetailsEventArgs(rowIndex, userInitiated);
        this.OnBeforeExpandDetails(e);
        if (e.Cancel)
        {
          if (!e.CollapseDetails)
            return;
          this.CollapseDetails();
        }
        else
        {
          bool flag = true;
          if (this.m_detailsRowIndex >= 0)
          {
            if (rowIndex != this.m_detailsRowIndex)
              this.CollapseDetails();
            else
              flag = false;
          }
          DataGridViewRow row = this.Rows[rowIndex];
          if (row == null)
            return;
          if (flag)
          {
            this.m_detailsRowIndex = rowIndex;
            this.m_detailsRowOrigHeight = row.Height;
            this.m_detailsRowOrigAlignment = row.DefaultCellStyle.Alignment;
          }
          row.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
          this.UpdateDetailsPanelHeight();
          int scrollingRowIndex = this.FirstDisplayedScrollingRowIndex;
          int columnHeadersHeight = this.ColumnHeadersVisible ? this.ColumnHeadersHeight : 0;
          if (this.m_detailsRowIndex < 0 || this.m_detailsRowIndex <= scrollingRowIndex || this.ClientRectangle.Height <= columnHeadersHeight + this.GridLineHeight)
            return;
          int num1 = 0;
          int num2 = this.m_detailsRowIndex == this.RowCount - 1 ? 1 : 0;
          if (num2 == 0)
            num1 = this.Rows[this.m_detailsRowIndex + 1].Height - this.GetRowDisplayRectangle(this.m_detailsRowIndex + 1, true).Height;
          if (num2 == 0 && num1 == 0)
            return;
          int num3 = this.Rows[this.m_detailsRowIndex].Height - this.GetRowDisplayRectangle(this.m_detailsRowIndex, true).Height + num1;
          if (num3 >= this.ClientRectangle.Height)
          {
            this.FirstDisplayedScrollingRowIndex = this.m_detailsRowIndex;
          }
          else
          {
            if (num3 <= 0)
              return;
            int index = scrollingRowIndex - 1;
            int num4 = 0;
            do
            {
              ++index;
              if (this.Rows[index].Visible)
              {
                int height = this.Rows[index].Height;
                num4 += height;
              }
            }
            while (num4 < num3 && index < this.RowCount - 2 && index < this.m_detailsRowIndex - 1);
            this.FirstDisplayedScrollingRowIndex = index == this.RowCount - 1 ? this.RowCount - 1 : index + 1;
          }
        }
      }
      finally
      {
        this.OnAfterExpandDetails(new DataGridViewDetailsEventArgs(rowIndex, userInitiated));
      }
    }

    public void CollapseDetails() => this.CollapseDetails(false);

    private void CollapseDetails(bool userInitiated)
    {
      if (this.m_detailsRowIndex < 0)
        return;
      try
      {
        DataGridViewDetailsEventArgs e = new DataGridViewDetailsEventArgs(this.m_detailsRowIndex, userInitiated);
        this.OnBeforeCollapseDetails(e);
        if (e.Cancel)
          return;
        DataGridViewRow row = this.Rows[this.m_detailsRowIndex];
        if (row == null)
          return;
        this.m_detailsRowIndex = -2;
        this.m_detailsPanel.Visible = false;
        row.Height = this.m_detailsRowOrigHeight;
        row.DefaultCellStyle.Alignment = this.m_detailsRowOrigAlignment;
      }
      finally
      {
        this.OnAfterCollapseDetails(new DataGridViewDetailsEventArgs(this.m_detailsRowIndex, userInitiated));
      }
    }

    [Browsable(false)]
    internal int DetailsWidth
    {
      get
      {
        int detailsWidth = this.ClientRectangle.Width - this.GridLineWidth - 2 * this.BorderWidth;
        if (this.VerticalScrollBar.Visible)
          detailsWidth -= SystemInformation.VerticalScrollBarWidth;
        return detailsWidth;
      }
    }

    private int BorderWidth
    {
      get
      {
        switch (this.BorderStyle)
        {
          case BorderStyle.FixedSingle:
            return SystemInformation.BorderSize.Width;
          case BorderStyle.Fixed3D:
            return SystemInformation.Border3DSize.Width;
          default:
            return 0;
        }
      }
    }

    private int GridLineWidth
    {
      get
      {
        switch (this.CellBorderStyle)
        {
          case DataGridViewCellBorderStyle.Single:
          case DataGridViewCellBorderStyle.SingleVertical:
            return SystemInformation.BorderSize.Width;
          case DataGridViewCellBorderStyle.Raised:
          case DataGridViewCellBorderStyle.Sunken:
          case DataGridViewCellBorderStyle.RaisedVertical:
          case DataGridViewCellBorderStyle.SunkenVertical:
            return SystemInformation.Border3DSize.Width;
          default:
            return 0;
        }
      }
    }

    private int GridLineHeight
    {
      get
      {
        switch (this.CellBorderStyle)
        {
          case DataGridViewCellBorderStyle.Single:
          case DataGridViewCellBorderStyle.SingleHorizontal:
            return SystemInformation.BorderSize.Height;
          case DataGridViewCellBorderStyle.Raised:
          case DataGridViewCellBorderStyle.Sunken:
          case DataGridViewCellBorderStyle.RaisedHorizontal:
          case DataGridViewCellBorderStyle.SunkenHorizontal:
            return SystemInformation.Border3DSize.Height;
          default:
            return 0;
        }
      }
    }

    protected virtual void OnBeforeExpandDetails(DataGridViewDetailsEventArgs e)
    {
      if (this.BeforeExpandDetails == null)
        return;
      this.BeforeExpandDetails((object) this, e);
    }

    protected virtual void OnAfterExpandDetails(DataGridViewDetailsEventArgs e)
    {
      if (this.AfterExpandDetails == null)
        return;
      this.AfterExpandDetails((object) this, e);
    }

    protected virtual void OnBeforeCollapseDetails(DataGridViewDetailsEventArgs e)
    {
      if (this.BeforeCollapseDetails == null)
        return;
      this.BeforeCollapseDetails((object) this, e);
    }

    protected virtual void OnAfterCollapseDetails(DataGridViewDetailsEventArgs e)
    {
      if (this.AfterCollapseDetails == null)
        return;
      this.AfterCollapseDetails((object) this, e);
    }

    protected override void OnResize(EventArgs e)
    {
      try
      {
        ++this.m_skipUpdateDetailsPanel;
        base.OnResize(e);
      }
      finally
      {
        --this.m_skipUpdateDetailsPanel;
      }
      if (this.DesignMode)
        return;
      this.UpdateDetailsPanelHeight();
    }

    protected override void OnScroll(ScrollEventArgs e)
    {
      base.OnScroll(e);
      if (this.DesignMode)
        return;
      bool flag = false;
      if (e.ScrollOrientation == ScrollOrientation.VerticalScroll && this.HorizontalScrollBar.Visible && this.Rows != null && this.RowCount > 0 && this.Rows[this.RowCount - 1] != null && this.Rows[this.RowCount - 1].Displayed)
        flag = true;
      if (flag)
        this.BeginInvoke((Delegate) new MethodInvoker(((Control) this).Invalidate));
      this.BeginInvoke((Delegate) new MethodInvoker(this.UpdateDetailsPanel));
    }

    private void VerticalScrollBar_VisibleChanged(object sender, EventArgs e) => this.UpdateDetailsPanelHeight();

    protected override void OnVisibleChanged(EventArgs e)
    {
      try
      {
        ++this.m_skipUpdateDetailsPanel;
        base.OnVisibleChanged(e);
      }
      finally
      {
        --this.m_skipUpdateDetailsPanel;
      }
      if (!this.Visible || this.m_detailsRowIndex == -2)
        return;
      this.m_detailsPanel.Visible = true;
      this.UpdateDetailsPanelHeight();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      if (this.DesignMode || !this.m_detailsPanel.Visible)
        return;
      this.m_detailsPanel.Invalidate(new Rectangle(this.m_detailsPanel.PointToClient(this.PointToScreen(e.ClipRectangle.Location)), e.ClipRectangle.Size), true);
      this.m_detailsPanel.Update();
    }

    protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
    {
      base.OnRowsAdded(e);
      if (this.m_correctRowMinHeight <= 0)
        this.CalculateRowMinHeight();
      if (this.m_correctRowMinHeight <= 0)
        return;
      for (int index = 0; index < e.RowCount; ++index)
        this.Rows[e.RowIndex + index].MinimumHeight = this.m_correctRowMinHeight;
    }

    private void CalculateRowMinHeight()
    {
      try
      {
        int val1 = 0;
        if (this.Rows.Count > 0)
          val1 = this.Rows[0].Height;
        this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
        if (this.Rows.Count <= 0)
          return;
        this.m_correctRowMinHeight = Math.Max(val1, this.Rows[0].Height);
      }
      finally
      {
        this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
      }
    }

    private void CalculateDetailsImageRects(
      int rowIndex,
      Rectangle cell,
      out Rectangle expanded,
      out Rectangle collapsed)
    {
      expanded = Rectangle.Empty;
      collapsed = Rectangle.Empty;
      if (rowIndex == this.m_detailsRowIndex)
        cell.Height = this.m_detailsRowOrigHeight;
      if (this.m_detailsExpandedImage != null)
      {
        int deviceUnitsX1 = DpiHelper.LogicalToDeviceUnitsX(this.m_detailsExpandedImage.Width);
        int deviceUnitsX2 = DpiHelper.LogicalToDeviceUnitsX(this.m_detailsExpandedImage.Height);
        expanded.X = Math.Max(cell.X + cell.Width / 2 - deviceUnitsX1 / 2, 0);
        expanded.Y = cell.Y + cell.Height / 2 - deviceUnitsX2 / 2;
        expanded.Size = new Size(deviceUnitsX1, deviceUnitsX2);
      }
      if (this.m_detailsCollapsedImage == null)
        return;
      int deviceUnitsX3 = DpiHelper.LogicalToDeviceUnitsX(this.m_detailsCollapsedImage.Width);
      int deviceUnitsX4 = DpiHelper.LogicalToDeviceUnitsX(this.m_detailsCollapsedImage.Height);
      collapsed.X = Math.Max(cell.X + cell.Width / 2 - deviceUnitsX3 / 2, 0);
      collapsed.Y = cell.Y + cell.Height / 2 - deviceUnitsX4 / 2;
      collapsed.Size = new Size(deviceUnitsX3, deviceUnitsX4);
    }

    protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
    {
      base.OnRowPostPaint(e);
      if (this.DesignMode || this.m_detailsImageColumnIndex <= -2)
        return;
      DataGridViewRow currentRow = this.CurrentRow;
      int num = currentRow != null ? currentRow.Index : -2;
      if (this.m_detailsImageMode != DataGridViewDetailsImageMode.AllRows && (this.m_detailsImageMode != DataGridViewDetailsImageMode.FocusedRow || e.RowIndex != num))
        return;
      Rectangle displayRectangle = this.GetCellDisplayRectangle(this.m_detailsImageColumnIndex, e.RowIndex, false);
      Rectangle expanded;
      Rectangle collapsed;
      this.CalculateDetailsImageRects(e.RowIndex, displayRectangle, out expanded, out collapsed);
      if (e.RowIndex == this.m_detailsRowIndex && this.m_detailsExpandedImage != null)
      {
        e.Graphics.DrawImage(this.m_detailsExpandedImage, expanded.Location);
      }
      else
      {
        if (this.m_detailsCollapsedImage == null)
          return;
        using (Graphics graphics = Graphics.FromHwnd(this.Handle))
          graphics.DrawImage(this.m_detailsCollapsedImage, collapsed.Location);
      }
    }

    protected override void OnGridColorChanged(EventArgs e)
    {
      base.OnGridColorChanged(e);
      this.m_detailsTopGridLine.BackColor = this.GridColor;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      base.OnKeyUp(e);
      if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
        return;
      this.m_selectionChanged = true;
      if (!this.m_detailsAutoExpand || !this.Visible)
        return;
      this.BeginInvoke((Delegate) new MethodInvoker(this.BeginExpandDetails));
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.m_selectionChanged = true;
      if (!this.m_detailsAutoExpand || !this.Visible)
        return;
      this.BeginInvoke((Delegate) new MethodInvoker(this.BeginExpandDetails));
    }

    private void BeginExpandDetails()
    {
      if (this.IsExpandDelayed)
      {
        this.m_detailsTimer.Stop();
        this.m_detailsTimer.Start();
      }
      else
        this.ExpandDetails();
    }

    protected override void OnRowEnter(DataGridViewCellEventArgs e)
    {
      base.OnRowEnter(e);
      if (this.DesignMode || !this.m_detailsAutoExpand || !this.Visible || !this.m_detailsInitialExpand)
        return;
      if (!this.m_detailsInitialExpand && !this.m_detailsInitialExpansionIgnored)
      {
        this.m_detailsInitialExpansionIgnored = true;
      }
      else
      {
        this.m_detailsInitialExpand = false;
        this.BeginInvoke((Delegate) new MethodInvoker(this.BeginExpandDetails));
      }
    }

    protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
    {
      base.OnRowsRemoved(e);
      if (this.RowCount != 0)
        return;
      this.m_detailsInitialExpand = true;
    }

    protected override void OnRowLeave(DataGridViewCellEventArgs e)
    {
      base.OnRowLeave(e);
      if (this.m_detailsImageColumnIndex <= -2)
        return;
      this.InvalidateCell(this.m_detailsImageColumnIndex, e.RowIndex);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      bool flag1 = false;
      if (e.Button == MouseButtons.Left)
      {
        DataGridView.HitTestInfo hitTestInfo = this.HitTest(e.X, e.Y);
        Rectangle displayRectangle = this.GetCellDisplayRectangle(hitTestInfo.ColumnIndex, hitTestInfo.RowIndex, false);
        if (hitTestInfo.RowIndex >= 0)
        {
          bool flag2 = true;
          if (this.m_detailsImageMode == DataGridViewDetailsImageMode.FocusedRow)
          {
            DataGridViewCell currentCell = this.CurrentCell;
            if (currentCell != null && currentCell.RowIndex != hitTestInfo.RowIndex)
              flag2 = false;
          }
          else if (this.m_detailsImageMode == DataGridViewDetailsImageMode.None)
            flag2 = false;
          if (hitTestInfo.ColumnIndex == this.m_detailsImageColumnIndex & flag2)
          {
            Rectangle expanded;
            Rectangle collapsed;
            this.CalculateDetailsImageRects(hitTestInfo.RowIndex, displayRectangle, out expanded, out collapsed);
            if (this.m_detailsRowIndex >= 0 && expanded.Contains(e.Location) || this.m_detailsRowIndex < 0 && collapsed.Contains(e.Location))
            {
              int detailsRowIndex = this.m_detailsRowIndex;
              if (this.m_detailsRowIndex >= 0)
                this.CollapseDetails(true);
              if (hitTestInfo.RowIndex != detailsRowIndex)
              {
                this.Rows[hitTestInfo.RowIndex].Selected = true;
                this.ExpandDetails(hitTestInfo.RowIndex, true);
              }
              flag1 = true;
            }
          }
        }
      }
      if (flag1)
        return;
      base.OnMouseDown(e);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override bool ProcessKeyPreview(ref Message m) => (this.m_detailsRowIndex < 0 || this.m_detailsHostedControl == null || !this.m_detailsHostedControl.ContainsFocus) && base.ProcessKeyPreview(ref m);

    [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
    protected override bool ProcessDialogKey(Keys keyData)
    {
      if ((keyData & Keys.KeyCode) == Keys.Tab && this.m_detailsRowIndex >= 0 && this.m_detailsHostedControl != null && this.m_detailsHostedControl.ContainsFocus)
      {
        if ((keyData & Keys.Modifiers) == Keys.None && this.RowCount > this.m_detailsRowIndex + 1 && this.ColumnCount > 0)
        {
          this.CurrentCell = this[0, this.m_detailsRowIndex + 1];
          this.Focus();
          return true;
        }
        if ((keyData & Keys.Modifiers) == Keys.Shift && this.ColumnCount > 0)
        {
          this.CurrentCell = this[this.ColumnCount - 1, this.m_detailsRowIndex];
          this.Focus();
          return true;
        }
      }
      return ((keyData & Keys.KeyCode) != Keys.Return && ((keyData & Keys.KeyCode) != Keys.C || (keyData & Keys.Modifiers) != Keys.Control) && ((keyData & Keys.KeyCode) != Keys.V || (keyData & Keys.Modifiers) != Keys.Control) && ((keyData & Keys.KeyCode) != Keys.X || (keyData & Keys.Modifiers) != Keys.Control) || this.m_detailsRowIndex < 0 || this.m_detailsHostedControl == null || !this.m_detailsHostedControl.ContainsFocus) && base.ProcessDialogKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Tab && this.m_detailsRowIndex >= 0 && this.m_detailsHostedControl != null)
      {
        DataGridViewCell currentCell = this.CurrentCell;
        if (currentCell != null)
        {
          if (e.Modifiers == Keys.None && currentCell.RowIndex == this.m_detailsRowIndex && currentCell.ColumnIndex == this.ColumnCount - 1)
          {
            this.m_detailsHostedControl.Focus();
            e.Handled = true;
          }
          else if (e.Modifiers == Keys.Shift && currentCell.RowIndex == this.m_detailsRowIndex + 1 && currentCell.ColumnIndex == 0)
          {
            this.m_detailsHostedControl.Focus();
            e.Handled = true;
          }
        }
      }
      if (e.KeyCode == Keys.Return && e.Modifiers == Keys.None)
      {
        DataGridViewCell currentCell = this.CurrentCell;
        if (currentCell != null && currentCell.ColumnIndex == this.m_detailsImageColumnIndex)
        {
          int detailsRowIndex = this.m_detailsRowIndex;
          if (this.m_detailsRowIndex >= 0)
            this.CollapseDetails(true);
          if (currentCell.RowIndex != detailsRowIndex)
            this.ExpandDetails(currentCell.RowIndex, true);
          e.Handled = true;
        }
      }
      if (!e.Handled && this.m_detailsAutoExpand && this.m_detailsRowIndex >= 0 && (e.KeyCode == Keys.Down && this.m_detailsRowIndex != this.RowCount - 1 || e.KeyCode == Keys.Up && this.m_detailsRowIndex != 0))
        this.CollapseDetails(false);
      base.OnKeyDown(e);
    }

    private void detailsTimer_Tick(object sender, EventArgs e)
    {
      this.m_detailsTimer.Stop();
      if (!this.m_detailsAutoExpand || this.CurrentRow == null)
        return;
      this.ExpandDetails(this.CurrentRow.Index);
    }

    public void UpdateDetailsPanelHeight() => this.UpdateDetailsPanel(true);

    private void UpdateDetailsPanel() => this.UpdateDetailsPanel(false);

    private void UpdateDetailsPanel(bool updateRowHeight)
    {
      if (this.m_skipUpdateDetailsPanel != 0 || this.m_detailsHostedControl == null || this.m_detailsRowIndex < 0)
        return;
      DataGridViewRow row = this.Rows[this.m_detailsRowIndex];
      if (row == null)
        return;
      ++this.m_skipUpdateDetailsPanel;
      try
      {
        if (updateRowHeight)
        {
          this.m_detailsPanel.Width = this.DetailsWidth;
          Size preferredSize = this.m_detailsHostedControl.GetPreferredSize(new Size(this.m_detailsPanel.Width, 0));
          row.Height = this.m_detailsRowOrigHeight + preferredSize.Height + this.m_detailsTopGridLine.Height;
        }
        Rectangle rectangle = this.GetRowDisplayRectangle(this.m_detailsRowIndex, true);
        this.m_detailsPanel.Left = this.BorderWidth - this.HorizontalScrollingOffset;
        this.m_detailsPanel.Top = rectangle.Y + this.m_detailsRowOrigHeight - this.GridLineHeight;
        Rectangle displayRectangle = this.GetRowDisplayRectangle(this.m_detailsRowIndex, true);
        if (displayRectangle.Y != rectangle.Y)
        {
          rectangle = displayRectangle;
          this.m_detailsPanel.Top = rectangle.Y + this.m_detailsRowOrigHeight - this.GridLineHeight;
        }
        if (updateRowHeight && this.HorizontalScrollBar.Visible)
          this.m_detailsPanel.Width = this.PreferredSize.Width;
        if (rectangle.Height == 0)
        {
          if (this.FirstDisplayedScrollingRowIndex == this.m_detailsRowIndex)
          {
            this.m_detailsPanel.Visible = true;
            this.m_detailsPanel.Top = this.ColumnHeadersHeight + this.m_detailsRowOrigHeight;
            rectangle.Height = row.Height;
          }
          else
            this.m_detailsPanel.Visible = false;
        }
        else
          this.m_detailsPanel.Visible = true;
        if (rectangle.Height != row.Height)
          rectangle.Height += this.BorderWidth;
        this.m_detailsPanel.Height = rectangle.Height - this.m_detailsRowOrigHeight;
      }
      finally
      {
        --this.m_skipUpdateDetailsPanel;
      }
    }

    protected override AccessibleObject CreateAccessibilityInstance() => (AccessibleObject) new DataGridViewWithDetails.DataGridWithDetailsAccessibleObject(this);

    protected class DataGridWithDetailsAccessibleObject : DataGridView.DataGridViewAccessibleObject
    {
      private Type m_dataGridViewRowAccessibleObjectType;
      private FieldInfo m_dataGridViewRowAccessibleObjectOwner;

      public DataGridWithDetailsAccessibleObject(DataGridViewWithDetails owner)
        : base((DataGridView) owner)
      {
        this.m_dataGridViewRowAccessibleObjectType = Type.GetType("System.Windows.Forms.DataGridViewRow+DataGridViewRowAccessibleObject, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        this.m_dataGridViewRowAccessibleObjectOwner = this.m_dataGridViewRowAccessibleObjectType.GetField(nameof (owner), BindingFlags.Instance | BindingFlags.NonPublic);
      }

      public DataGridViewWithDetails OwnerDataGrid => (DataGridViewWithDetails) this.Owner;

      public override AccessibleObject GetChild(int index)
      {
        AccessibleObject child = base.GetChild(index);
        if (child == null)
          return (AccessibleObject) null;
        if (!(this.m_dataGridViewRowAccessibleObjectType != (Type) null) || !(this.m_dataGridViewRowAccessibleObjectOwner != (FieldInfo) null) || !(child.GetType() == this.m_dataGridViewRowAccessibleObjectType))
          return child;
        return !(this.m_dataGridViewRowAccessibleObjectOwner.GetValue((object) child) is DataGridViewRow row) ? (AccessibleObject) null : (AccessibleObject) new DataGridViewWithDetails.DataGridViewWithDetailsRowAccessibleObject(row, this.OwnerDataGrid);
      }
    }

    protected class DataGridViewWithDetailsRowAccessibleObject : AccessibleObject
    {
      private AccessibleObject m_originalAccObject;
      private DataGridViewWithDetails m_dataGridView;
      private DataGridViewRow m_row;

      public DataGridViewWithDetailsRowAccessibleObject(
        DataGridViewRow row,
        DataGridViewWithDetails dataGridView)
      {
        this.m_originalAccObject = row.AccessibilityObject;
        this.m_row = row;
        this.m_dataGridView = dataGridView;
      }

      public override Rectangle Bounds => this.m_originalAccObject.Bounds;

      public override string Name => this.m_originalAccObject.Name;

      public override AccessibleObject Parent => this.m_originalAccObject.Parent;

      public override AccessibleRole Role => this.m_originalAccObject.Role;

      public override AccessibleStates State => this.m_originalAccObject.State;

      public override string Value => this.m_originalAccObject.Value;

      public override AccessibleObject Navigate(AccessibleNavigation navigationDirection) => this.m_originalAccObject.Navigate(navigationDirection);

      public override AccessibleObject GetSelected() => this.m_originalAccObject.GetSelected();

      public override string DefaultAction => this.m_originalAccObject.DefaultAction;

      public override string Description => this.m_originalAccObject.Description;

      public override string Help => this.m_originalAccObject.Help;

      public override void DoDefaultAction() => this.m_originalAccObject.DoDefaultAction();

      public override bool Equals(object obj) => this.m_originalAccObject.Equals(obj);

      public override int GetHashCode() => this.m_originalAccObject.GetHashCode();

      public override int GetHelpTopic(out string fileName) => this.m_originalAccObject.GetHelpTopic(out fileName);

      public override AccessibleObject HitTest(int x, int y) => this.m_originalAccObject.HitTest(x, y);

      public override string KeyboardShortcut => this.m_originalAccObject.KeyboardShortcut;

      public override void Select(AccessibleSelection flags) => this.m_originalAccObject.Select(flags);

      public override int GetChildCount()
      {
        int childCount = this.m_originalAccObject.GetChildCount();
        if (this.m_row.Index == this.m_dataGridView.DetailsRow)
          ++childCount;
        return childCount;
      }

      public override AccessibleObject GetChild(int index)
      {
        int childCount = this.m_originalAccObject.GetChildCount();
        if (index < childCount)
          return this.m_originalAccObject.GetChild(index);
        return this.m_row.Index == this.m_dataGridView.DetailsRow && this.m_dataGridView.DetailsControl != null && index == childCount ? this.m_dataGridView.DetailsControl.AccessibilityObject : (AccessibleObject) null;
      }

      public override AccessibleObject GetFocused() => this.m_dataGridView != null && this.m_dataGridView.DetailsControl != null && this.m_dataGridView.DetailsControl.ContainsFocus ? this.m_dataGridView.AccessibilityObject.GetFocused() : this.m_originalAccObject.GetFocused();
    }
  }
}
