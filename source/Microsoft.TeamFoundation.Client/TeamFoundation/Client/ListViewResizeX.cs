// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ListViewResizeX
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ListViewResizeX : ListViewResize
  {
    private bool m_inDoubleClickCheckHack;
    private bool m_doubleClickDoesCheck;

    public ListViewResizeX() => this.m_doubleClickDoesCheck = true;

    private unsafe void WmReflectNotify(ref Message m)
    {
      if (this.DoubleClickDoesCheck || !this.CheckBoxes || ((NativeMethods.NMHDR*) (void*) m.LParam)->code != -3)
        return;
      this.m_inDoubleClickCheckHack = true;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 8270)
        this.WmReflectNotify(ref m);
      else if (m.Msg == 4114 && this.m_inDoubleClickCheckHack)
      {
        this.m_inDoubleClickCheckHack = false;
        m.Result = (IntPtr) -1;
        return;
      }
      base.WndProc(ref m);
    }

    [Browsable(true)]
    [Description("When CheckBoxes is true, this controls whether or not double clicking will toggle the check.")]
    [Category("Team Foundation")]
    [DefaultValue(true)]
    public bool DoubleClickDoesCheck
    {
      get => this.m_doubleClickDoesCheck;
      set => this.m_doubleClickDoesCheck = value;
    }
  }
}
