// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationDateTimePicker
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  public class TeamFoundationDateTimePicker : DateTimePicker
  {
    private Color m_alternateBackgroundColor;
    private bool m_useAlternateBackgroundColor;

    public TeamFoundationDateTimePicker()
    {
      this.m_alternateBackgroundColor = Control.DefaultBackColor;
      this.m_useAlternateBackgroundColor = false;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 20 && this.Enabled && this.UseAlternateBackgroundColor)
      {
        using (Graphics graphics = this.CreateGraphics())
        {
          Rectangle rect = new Rectangle(new Point(0, 0), new Size(this.Width, this.Height));
          using (Brush brush = (Brush) new SolidBrush(this.AlternateBackgroundColor))
            graphics.FillRectangle(brush, rect);
          m.Result = (IntPtr) 1;
        }
      }
      else
        base.WndProc(ref m);
    }

    [Browsable(true)]
    [Description("Enables use of the alternate background color for the date time picker control")]
    [Category("Team Foundation")]
    public bool UseAlternateBackgroundColor
    {
      get => this.m_useAlternateBackgroundColor;
      set
      {
        if (this.m_useAlternateBackgroundColor == value)
          return;
        this.m_useAlternateBackgroundColor = value;
        if (!this.Visible || !this.Enabled || !this.m_useAlternateBackgroundColor)
          return;
        this.Invalidate();
      }
    }

    [Browsable(true)]
    [Description("Sets the alternate background color for the date time picker control")]
    [Category("Team Foundation")]
    public Color AlternateBackgroundColor
    {
      get => this.m_alternateBackgroundColor;
      set
      {
        if (!(this.m_alternateBackgroundColor != value))
          return;
        this.m_alternateBackgroundColor = value;
        if (!this.Visible || !this.Enabled || !this.UseAlternateBackgroundColor)
          return;
        this.Invalidate();
      }
    }
  }
}
