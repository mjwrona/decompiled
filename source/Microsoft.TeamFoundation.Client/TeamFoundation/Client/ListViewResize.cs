// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ListViewResize
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ListViewResize : ListView
  {
    private ListViewResize.AutoHeadersSizingEnum m_autoHeadersSizing;
    private IComparer m_listViewSorter;
    private int m_updateDepth;
    private int m_verticalNonClient;
    private int m_horizontalNonClient;
    private int m_itemHeight;
    private int m_lastClientWidth;

    public void BeginAddUpdate()
    {
      ++this.m_updateDepth;
      if (this.m_updateDepth != 1)
        return;
      this.BeginUpdate();
      this.m_listViewSorter = this.ListViewItemSorter;
      this.ListViewItemSorter = (IComparer) null;
    }

    public void EndAddUpdate()
    {
      if (--this.m_updateDepth > 0)
        return;
      this.m_updateDepth = 0;
      this.ListViewItemSorter = this.m_listViewSorter;
      this.m_listViewSorter = (IComparer) null;
      this.EndUpdate();
    }

    public event EventHandler Scroll;

    public event EventHandler<ListViewColumnSizedEventArgs> ColumnSized;

    protected override void CreateHandle()
    {
      base.CreateHandle();
      IntPtr hWnd = Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) this, this.Handle), 4127, (IntPtr) 0, (IntPtr) 0);
      if (!(hWnd != (IntPtr) 0))
        return;
      IntPtr windowLong = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetWindowLong(hWnd, -16);
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetWindowLong(hWnd, -16, (IntPtr) ((int) windowLong | 4));
    }

    [Browsable(true)]
    [Description("Set the criteria by which we size the right most column.")]
    [Category("Team Foundation")]
    [DefaultValue(ListViewResize.AutoHeadersSizingEnum.FitContent)]
    public ListViewResize.AutoHeadersSizingEnum AutoHeadersSizing
    {
      get => this.m_autoHeadersSizing;
      set => this.m_autoHeadersSizing = value;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override unsafe void WndProc(ref Message m)
    {
      if (m.Msg == 78)
      {
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.NMHEADER* lparam = (Microsoft.TeamFoundation.Common.Internal.NativeMethods.NMHEADER*) (void*) m.LParam;
        if (lparam->nmhdr.code == -306 || lparam->nmhdr.code == -326)
        {
          if (lparam->iItem == this.Columns.Count - 1)
          {
            m.Result = (IntPtr) 1;
            return;
          }
          this.OnColumnSized(lparam->iItem, ((Microsoft.TeamFoundation.Common.Internal.NativeMethods.HDITEM*) (void*) lparam->pItem)->cxy);
        }
        else if (lparam->nmhdr.code == -301 || lparam->nmhdr.code == -321)
        {
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.HDITEM* pItem = (Microsoft.TeamFoundation.Common.Internal.NativeMethods.HDITEM*) (void*) lparam->pItem;
          if ((pItem->mask & 1) != 0)
          {
            if (lparam->iItem != this.Columns.Count - 1)
            {
              if (this.m_autoHeadersSizing == ListViewResize.AutoHeadersSizingEnum.FitContent)
                this.Columns[this.Columns.Count - 1].Width = -2;
              else
                this.SizeLastColumn(m.HWnd, this.ClientRectangle.Width);
            }
            this.OnColumnSized(lparam->iItem, pItem->cxy);
          }
        }
      }
      else if (m.Msg == 71 || m.Msg == 275)
      {
        if (this.View == View.Details && this.TopItem != null)
        {
          ListViewItem topItem = this.TopItem;
          base.WndProc(ref m);
          if (this.TopItem == topItem)
            return;
          this.OnScroll();
          return;
        }
      }
      else
      {
        if (m.Msg == 277 || m.Msg == 276 || m.Msg == 522)
        {
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) this, this.Handle), 11, (IntPtr) 0, (IntPtr) 0);
          base.WndProc(ref m);
          this.OnScroll();
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) this, this.Handle), 11, (IntPtr) 1, (IntPtr) 0);
          this.Invalidate();
          return;
        }
        if (this.m_autoHeadersSizing == ListViewResize.AutoHeadersSizingEnum.FitClientArea)
        {
          if (m.Msg == 70)
          {
            Microsoft.TeamFoundation.Common.Internal.NativeMethods.WINDOWPOS* lparam = (Microsoft.TeamFoundation.Common.Internal.NativeMethods.WINDOWPOS*) (void*) m.LParam;
            if ((lparam->flags & 1) == 0)
              this.UpdateColumns(m.HWnd, this.Items.Count, lparam->cx, lparam->cy);
          }
          else if (m.Msg == 4103 || m.Msg == 4173)
          {
            int num = (int) Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) null, m.HWnd), 4100, IntPtr.Zero, IntPtr.Zero);
            this.UpdateColumns(m.HWnd, num + 1, this.Width, this.Height);
          }
          else
          {
            if (m.Msg == 4104)
            {
              base.WndProc(ref m);
              this.UpdateColumns(m.HWnd, this.Items.Count, this.Width, this.Height);
              return;
            }
            if (m.Msg == 4105)
            {
              base.WndProc(ref m);
              this.UpdateColumns(m.HWnd, 0, this.Width, this.Height);
              return;
            }
          }
        }
      }
      base.WndProc(ref m);
    }

    private void UpdateColumns(IntPtr hwnd, int itemCount, int width, int height)
    {
      if (this.m_verticalNonClient == 0)
      {
        HandleRef handleRef = new HandleRef((object) null, hwnd);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.RECT rect1;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetWindowRect(handleRef, out rect1);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.RECT rect2;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetClientRect(handleRef, out rect2);
        this.m_verticalNonClient = rect1.bottom - rect1.top - (rect2.bottom - rect2.top);
        this.m_horizontalNonClient = rect1.right - rect1.left - (rect2.right - rect2.left);
        IntPtr handle = Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(handleRef, 4127, IntPtr.Zero, IntPtr.Zero);
        if (handle != IntPtr.Zero)
        {
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) null, handle), 4615, IntPtr.Zero, ref rect2);
          this.m_verticalNonClient += rect2.bottom - rect2.top;
        }
      }
      if (this.m_itemHeight == 0)
      {
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.RECT rect = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.RECT();
        rect.left = 0;
        if ((int) Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) null, hwnd), 4110, (IntPtr) 0, ref rect) != 0)
          this.m_itemHeight = rect.bottom - rect.top;
      }
      width -= this.m_horizontalNonClient;
      height -= this.m_verticalNonClient;
      if (itemCount * this.m_itemHeight > height)
        width -= SystemInformation.VerticalScrollBarWidth;
      if (width == this.m_lastClientWidth)
        return;
      this.SizeLastColumn(hwnd, width);
      this.m_lastClientWidth = width;
    }

    private void SizeLastColumn(IntPtr hwnd, int width)
    {
      int index = this.Columns.Count - 1;
      if (index < 0)
        return;
      int num = 0;
      HandleRef hwnd1 = new HandleRef((object) null, hwnd);
      for (int column = 0; column < index; ++column)
        num += Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetListViewColumnWidth(hwnd1, column);
      this.Columns[index].Width = width - (num + index);
    }

    protected virtual void OnColumnSized(int column, int width)
    {
      EventHandler<ListViewColumnSizedEventArgs> columnSized = this.ColumnSized;
      if (columnSized == null)
        return;
      columnSized((object) this, new ListViewColumnSizedEventArgs(column, width));
    }

    protected virtual void OnScroll()
    {
      EventHandler scroll = this.Scroll;
      if (scroll == null)
        return;
      scroll((object) this, EventArgs.Empty);
    }

    public virtual bool AnyItemsChecked
    {
      get
      {
        bool anyItemsChecked = false;
        for (int index = 0; index < this.Items.Count; ++index)
        {
          if (this.Items[index].Checked)
          {
            anyItemsChecked = true;
            break;
          }
        }
        return anyItemsChecked;
      }
    }

    public enum AutoHeadersSizingEnum
    {
      FitContent,
      FitClientArea,
    }
  }
}
