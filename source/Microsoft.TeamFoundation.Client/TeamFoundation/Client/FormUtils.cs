// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.FormUtils
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FormUtils
  {
    public static bool HandleContextMenuDispatch(Control parent, Message m)
    {
      bool flag1 = false;
      bool flag2 = (int) (long) m.LParam == -1;
      int x = NativeMethods.Util.SignedLOWORD(m.LParam);
      int y = NativeMethods.Util.SignedHIWORD(m.LParam);
      Control control1 = Control.FromHandle(NativeMethods.GetFocus());
      if (control1 != null)
      {
        Point positionForKeyboard = FormUtils.ComputeContextMenuPositionForKeyboard(control1, x, y, flag2);
        flag1 = FormUtils.HandleContextMenuDispatch(parent, control1, positionForKeyboard, flag2);
      }
      if (!flag1)
      {
        Control control2 = Control.FromHandle(m.WParam);
        if (control2 != null && control2 != control1)
        {
          Point positionForKeyboard = FormUtils.ComputeContextMenuPositionForKeyboard(control2, x, y, flag2);
          flag1 = FormUtils.HandleContextMenuDispatch(parent, control2, positionForKeyboard, flag2);
        }
      }
      return flag1;
    }

    public static bool HandleContextMenuDispatch(
      Control parent,
      Control control,
      Point point,
      bool isKeyboardInvoke)
    {
      bool flag = false;
      for (Control control1 = control; !flag && control1 != null && control1 != parent.Parent; control1 = control1.Parent)
      {
        if (control1 is IContextMenuDispatch contextMenuDispatch)
          flag = contextMenuDispatch.ShowContextMenuForControl(control, point, isKeyboardInvoke);
      }
      return flag;
    }

    public static Point ComputeContextMenuPositionForKeyboard(
      Control control,
      int x,
      int y,
      bool keyboardInvoke)
    {
      Point positionForKeyboard;
      if (keyboardInvoke)
      {
        switch (control)
        {
          case TreeView _:
            TreeView treeView = (TreeView) control;
            TreeNode selectedNode = treeView.SelectedNode;
            positionForKeyboard = selectedNode == null || !selectedNode.IsVisible ? new Point(0, 0) : FormUtils.AdjustToClientRect(selectedNode.Bounds.Left, selectedNode.Bounds.Top + selectedNode.Bounds.Height, treeView.ClientRectangle);
            break;
          case ListView _:
            ListView listView = (ListView) control;
            Rectangle clientRectangle = listView.ClientRectangle;
            ListViewItem focusedItem = listView.FocusedItem;
            positionForKeyboard = focusedItem == null || !focusedItem.Bounds.IntersectsWith(clientRectangle) ? new Point(0, 0) : FormUtils.AdjustToClientRect(focusedItem.Position.X, focusedItem.Bounds.Top + focusedItem.Bounds.Height, clientRectangle);
            break;
          default:
            positionForKeyboard = new Point(control.Width / 2, control.Height / 2);
            break;
        }
      }
      else
        positionForKeyboard = control.PointToClient(new Point(x, y));
      return positionForKeyboard;
    }

    public static Point AdjustToClientRect(int menuX, int menuY, Rectangle clientRect)
    {
      menuX = menuX > clientRect.Right ? clientRect.Right - 50 : menuX;
      menuY = menuY > clientRect.Bottom ? clientRect.Bottom - 50 : menuY;
      menuX = menuX < 0 ? 0 : menuX;
      menuY = menuY < 0 ? 0 : menuY;
      return new Point(menuX, menuY);
    }

    internal static bool IsTypeOrDerived(Type type, Type typeCompare)
    {
      bool flag;
      for (flag = false; type != (Type) null && !flag; type = type.BaseType)
      {
        if (type == typeCompare)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    internal static bool IsControlType(Type type) => FormUtils.IsTypeOrDerived(type, typeof (Control));

    [Conditional("DEBUG")]
    internal static void CheckControlConstructorCalls()
    {
    }

    internal static bool IsDesignMode(Control ctl)
    {
      bool flag;
      for (flag = false; !flag && ctl != null; ctl = ctl.Parent)
      {
        ISite site = ctl.Site;
        if (site != null)
          flag = site.DesignMode;
      }
      return flag;
    }

    public static bool DoAnyWishToCancel<T>(EventHandler<T> handlers, object sender, T e) where T : CancelEventArgs
    {
      if (handlers == null)
        return false;
      foreach (EventHandler<T> invocation in handlers.GetInvocationList())
      {
        invocation(sender, e);
        if (e.Cancel)
          return true;
      }
      return false;
    }

    public static Control ActiveLeafControl(ContainerControl control)
    {
      Control activeControl;
      while ((activeControl = control.ActiveControl) != null)
      {
        control = activeControl as ContainerControl;
        if (control == null)
          return activeControl;
      }
      return (Control) control;
    }

    public static bool IsMnemonicMsg(Message msg)
    {
      char wparam = (char) (int) msg.WParam;
      return msg.Msg == 260 && Control.ModifierKeys == Keys.Alt && wparam >= '!' && wparam < 'a';
    }

    public static bool IsWindowNavigationMsg(Message msg)
    {
      char wparam = (char) (int) msg.WParam;
      return msg.Msg == 256 && (Control.ModifierKeys & Keys.Control) == Keys.Control && wparam == '\t';
    }
  }
}
