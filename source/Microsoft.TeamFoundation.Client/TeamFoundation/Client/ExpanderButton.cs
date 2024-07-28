// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ExpanderButton
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Microsoft.TeamFoundation.Client
{
  internal class ExpanderButton : System.Windows.Forms.CheckBox
  {
    private IContainer components;
    private bool m_expanded;
    private VisualStyleRenderer m_plusMinusRenderer;
    private System.Windows.Forms.ToolTip m_tooltip;
    private string m_tooltipText;
    private const int m_boxHeight = 15;
    private const int m_boxWidth = 15;
    private const int m_paddingBetweenBoxAndText = 4;
    private const int m_offSet = 4;

    public ExpanderButton()
    {
      this.InitializeComponent();
      this.m_tooltip = new System.Windows.Forms.ToolTip();
      this.AccessibleRole = AccessibleRole.OutlineButton;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.m_tooltip != null)
        {
          this.m_tooltip.Dispose();
          this.m_tooltip = (System.Windows.Forms.ToolTip) null;
        }
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    [Category("Appearance")]
    [Browsable(true)]
    [DefaultValue(true)]
    public bool Expanded
    {
      get => this.m_expanded;
      set => this.Expand(value);
    }

    [Browsable(true)]
    public event EventHandler<CancelEventArgs> ExpandEvent;

    public event EventHandler<CancelEventArgs> ContractEvent;

    private bool OnExpand()
    {
      EventHandler<CancelEventArgs> expandEvent = this.ExpandEvent;
      if (expandEvent == null)
        return true;
      CancelEventArgs e = new CancelEventArgs(false);
      expandEvent((object) this, e);
      return !e.Cancel;
    }

    private bool OnContract() => !FormUtils.DoAnyWishToCancel<CancelEventArgs>(this.ContractEvent, (object) this, new CancelEventArgs(false));

    private void Expand(bool bExpand)
    {
      if (bExpand)
      {
        if (!this.OnExpand())
          return;
      }
      else if (!this.OnContract())
        return;
      this.m_expanded = bExpand;
      this.AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
      this.Invalidate();
    }

    private StringFormat TextFormat
    {
      get
      {
        StringFormat stringFormat = new StringFormat();
        return new StringFormat()
        {
          Alignment = StringAlignment.Near,
          LineAlignment = StringAlignment.Center,
          HotkeyPrefix = !this.ShowKeyboardCues ? HotkeyPrefix.Hide : HotkeyPrefix.Show
        };
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      int height1 = 15;
      int width1 = height1;
      Rectangle bounds = new Rectangle(0, (this.Size.Height - height1) / 2, width1, height1);
      ButtonState state = ButtonState.Flat;
      using (SolidBrush solidBrush1 = new SolidBrush(this.BackColor))
      {
        Graphics graphics = e.Graphics;
        SolidBrush solidBrush2 = solidBrush1;
        Size size = this.Size;
        int width2 = size.Width;
        size = this.Size;
        int height2 = size.Height;
        graphics.FillRectangle((Brush) solidBrush2, 0, 0, width2, height2);
      }
      using (new SolidBrush(this.ForeColor))
      {
        Rectangle textRect;
        Size textSize;
        TextFormatFlags format;
        this.MeasureText(e.Graphics, out textRect, out textSize, out Size _, out format);
        if (this.Enabled)
          TextRenderer.DrawText((IDeviceContext) e.Graphics, this.Text, this.Font, textRect, this.ForeColor, format);
        else
          TextRenderer.DrawText((IDeviceContext) e.Graphics, this.Text, this.Font, textRect, SystemColors.GrayText, format);
        if (this.Focused)
        {
          Rectangle rectangle = new Rectangle(textRect.Left - 1, (this.Size.Height - textSize.Height) / 2, textSize.Width, textSize.Height);
          ControlPaint.DrawFocusRectangle(e.Graphics, rectangle);
        }
      }
      if (!this.Enabled)
        state |= ButtonState.Inactive;
      if (Application.RenderWithVisualStyles)
      {
        if (this.m_plusMinusRenderer == null)
          this.m_plusMinusRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
        this.m_plusMinusRenderer.SetParameters(this.m_expanded ? VisualStyleElement.TreeView.Glyph.Opened : VisualStyleElement.TreeView.Glyph.Closed);
        this.m_plusMinusRenderer.DrawBackground((IDeviceContext) e.Graphics, bounds);
      }
      else
      {
        ControlPaint.DrawCheckBox(e.Graphics, bounds.X, bounds.Y, bounds.Width, bounds.Height, state);
        int x = 4;
        Point pt1_1 = new Point(x, bounds.Y + bounds.Height / 2);
        Point pt1_2 = new Point(width1 / 2, bounds.Y + x);
        Point pt2_1 = new Point(width1 - x - 1, bounds.Y + bounds.Height / 2);
        Point pt2_2 = new Point(width1 / 2, bounds.Y + bounds.Height - x - 1);
        using (Pen pen = new Pen(this.ForeColor))
        {
          e.Graphics.DrawLine(pen, pt1_1, pt2_1);
          if (this.m_expanded)
            return;
          e.Graphics.DrawLine(pen, pt1_2, pt2_2);
        }
      }
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      using (Graphics graphics = this.CreateGraphics())
      {
        Size textSize;
        Size unboundTextSize;
        this.MeasureText(graphics, out Rectangle _, out textSize, out unboundTextSize, out TextFormatFlags _);
        this.m_tooltipText = textSize.Width < unboundTextSize.Width ? this.StripHotKey(this.Text) : string.Empty;
      }
    }

    private void MeasureText(
      Graphics g,
      out Rectangle textRect,
      out Size textSize,
      out Size unboundTextSize,
      out TextFormatFlags format)
    {
      int height1 = 15;
      int width1 = height1;
      Rectangle rectangle1 = new Rectangle(0, (this.Size.Height - height1) / 2, width1, height1);
      int num = 4;
      int x = rectangle1.Left + rectangle1.Width + num;
      ref Rectangle local = ref textRect;
      Point location = new Point(x, 0);
      Size size1 = this.Size;
      int width2 = size1.Width - x;
      size1 = this.Size;
      int height2 = size1.Height;
      Size size2 = new Size(width2, height2);
      Rectangle rectangle2 = new Rectangle(location, size2);
      local = rectangle2;
      format = TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
      textSize = TextRenderer.MeasureText((IDeviceContext) g, this.Text, this.Font, new Size(textRect.Width, textRect.Height), format);
      TextFormatFlags flags = TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
      unboundTextSize = TextRenderer.MeasureText((IDeviceContext) g, this.Text, this.Font, new Size(0, textRect.Height), flags);
    }

    private string StripHotKey(string text)
    {
      StringBuilder stringBuilder = new StringBuilder(text.Length);
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] != '&')
          stringBuilder.Append(text[index]);
        else if (index + 1 < text.Length && text[index + 1] == '&')
        {
          stringBuilder.Append('&');
          ++index;
        }
      }
      return stringBuilder.ToString();
    }

    protected override void OnMouseHover(EventArgs e)
    {
      base.OnMouseHover(e);
      if (string.IsNullOrEmpty(this.m_tooltipText))
        return;
      this.m_tooltip.Show(this.m_tooltipText, (IWin32Window) this);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      this.m_tooltip.Hide((IWin32Window) this);
    }

    private void ExpanderButton_Click(object sender, EventArgs e)
    {
      this.Expand(!this.Expanded);
      this.Checked = false;
    }

    protected override AccessibleObject CreateAccessibilityInstance() => (AccessibleObject) new ExpanderButton.ExpanderButtonAccessibilityObject(this);

    private void InitializeComponent()
    {
      this.AutoCheck = false;
      this.Click += new EventHandler(this.ExpanderButton_Click);
    }

    private class ExpanderButtonAccessibilityObject : System.Windows.Forms.CheckBox.CheckBoxAccessibleObject
    {
      public ExpanderButtonAccessibilityObject(ExpanderButton owner)
        : base((Control) owner)
      {
      }

      public override AccessibleStates State => base.State & ~AccessibleStates.Checked | ((this.Owner as ExpanderButton).Expanded ? AccessibleStates.Expanded : AccessibleStates.Collapsed);
    }
  }
}
