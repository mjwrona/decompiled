// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BorderPanel
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  public class BorderPanel : Panel
  {
    private Color m_borderColor = SystemColors.ControlDark;
    private ButtonBorderStyle m_borderStyle = ButtonBorderStyle.Solid;
    private Padding m_borderPadding = Padding.Empty;
    private BorderPanel.Sides m_borderSides = BorderPanel.Sides.All;
    private bool m_useInnerColor;
    private Color m_innerColor = SystemColors.Control;

    public BorderPanel()
    {
      this.ResizeRedraw = true;
      this.m_borderColor = UIHost.Colors.BorderColor;
    }

    public Color BorderColor
    {
      get => this.m_borderColor;
      set => this.m_borderColor = value;
    }

    public ButtonBorderStyle BorderStyle
    {
      get => this.m_borderStyle;
      set => this.m_borderStyle = value;
    }

    public Padding BorderPadding
    {
      get => this.m_borderPadding;
      set => this.m_borderPadding = value;
    }

    public BorderPanel.Sides BorderSides
    {
      get => this.m_borderSides;
      set => this.m_borderSides = value;
    }

    public Color InnerColor
    {
      get => this.m_innerColor;
      set => this.m_innerColor = value;
    }

    public bool UseInnerColor
    {
      get => this.m_useInnerColor;
      set => this.m_useInnerColor = value;
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
      base.OnPaintBackground(pevent);
      if (this.m_borderSides == BorderPanel.Sides.None && !this.m_useInnerColor)
        return;
      Rectangle rectangle;
      ref Rectangle local = ref rectangle;
      int left1 = this.ClientRectangle.Left;
      Padding borderPadding = this.BorderPadding;
      int left2 = borderPadding.Left;
      int x = left1 + left2;
      int top1 = this.ClientRectangle.Top;
      borderPadding = this.BorderPadding;
      int top2 = borderPadding.Top;
      int y = top1 + top2;
      int width1 = this.ClientRectangle.Width;
      borderPadding = this.BorderPadding;
      int left3 = borderPadding.Left;
      int num1 = width1 - left3;
      borderPadding = this.BorderPadding;
      int right = borderPadding.Right;
      int width2 = num1 - right - 1;
      int height1 = this.ClientRectangle.Height;
      borderPadding = this.BorderPadding;
      int top3 = borderPadding.Top;
      int num2 = height1 - top3;
      borderPadding = this.BorderPadding;
      int bottom = borderPadding.Bottom;
      int height2 = num2 - bottom - 1;
      local = new Rectangle(x, y, width2, height2);
      if (this.m_useInnerColor)
      {
        using (Brush brush = (Brush) new SolidBrush(this.InnerColor))
          pevent.Graphics.FillRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Width + 1, rectangle.Height + 1);
      }
      if (this.m_borderSides == BorderPanel.Sides.None)
        return;
      using (Pen pen = new Pen(this.BorderColor))
      {
        if ((this.m_borderSides & BorderPanel.Sides.Top) == BorderPanel.Sides.Top)
          pevent.Graphics.DrawLine(pen, new Point(rectangle.Left, rectangle.Top), new Point(rectangle.Right, rectangle.Top));
        if ((this.m_borderSides & BorderPanel.Sides.Right) == BorderPanel.Sides.Right)
          pevent.Graphics.DrawLine(pen, new Point(rectangle.Right, rectangle.Top), new Point(rectangle.Right, rectangle.Bottom));
        if ((this.m_borderSides & BorderPanel.Sides.Bottom) == BorderPanel.Sides.Bottom)
          pevent.Graphics.DrawLine(pen, new Point(rectangle.Right, rectangle.Bottom), new Point(rectangle.Left, rectangle.Bottom));
        if ((this.m_borderSides & BorderPanel.Sides.Left) != BorderPanel.Sides.Left)
          return;
        pevent.Graphics.DrawLine(pen, new Point(rectangle.Left, rectangle.Bottom), new Point(rectangle.Left, rectangle.Top));
      }
    }

    [Flags]
    public enum Sides
    {
      None = 0,
      Top = 1,
      Bottom = 2,
      Left = 4,
      Right = 8,
      All = Right | Left | Bottom | Top, // 0x0000000F
    }
  }
}
