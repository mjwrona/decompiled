// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.InformationBar
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  public class InformationBar : BorderPanel
  {
    private LinkLabel m_labelText;
    private Label m_progressLabel;
    private int m_maxHeight;
    private int m_progressBarValue;
    private ProgressBar m_progressBar;
    private Panel m_progressPanel;
    private InformationBar.IconType m_iconType;
    private PictureBox m_icon;
    private Color m_warningBackColor;
    private Color m_warningForeColor;
    private Color m_infoBackColor;
    private Color m_infoForeColor;
    private Color m_linkForeColor;
    private static ImageList s_images;
    private static object s_imagesLock = new object();
    private static Dictionary<int, Image> s_imagesCache;
    private bool m_resizing;
    private bool m_wrapText = true;
    private const string cInformationBarImages = "InformationBarImages";

    static InformationBar()
    {
      Bitmap bitmap = (Bitmap) ClientResources.Manager.GetObject("InformationBarImages");
      bitmap.MakeTransparent();
      InformationBar.s_images = new ImageList();
      InformationBar.s_images.ImageSize = new Size(16, 16);
      InformationBar.s_images.Images.AddStrip((Image) bitmap);
      InformationBar.s_imagesCache = new Dictionary<int, Image>();
    }

    public InformationBar()
    {
      this.Padding = new Padding(1, 1, 1, 1).LogicalToDeviceUnits();
      this.m_labelText = new LinkLabel();
      this.m_labelText.Name = nameof (m_labelText);
      this.m_labelText.Font = SystemFonts.DefaultFont;
      this.m_labelText.Dock = DockStyle.Fill;
      this.m_labelText.TextAlign = ContentAlignment.TopLeft;
      this.m_labelText.AutoSize = true;
      this.m_labelText.Padding = new Padding(3, 3, 0, 3).LogicalToDeviceUnits();
      this.m_labelText.UseMnemonic = false;
      this.m_labelText.AutoEllipsis = true;
      this.m_labelText.LinkClicked += new LinkLabelLinkClickedEventHandler(this.OnLabelLinkClicked);
      this.m_labelText.Links.Clear();
      this.m_progressBar = new ProgressBar();
      this.m_progressBar.Name = "InformationBarProgress";
      this.m_progressBar.Dock = DockStyle.Top;
      this.m_progressBar.Height = DpiHelper.LogicalToDeviceUnitsY(16);
      this.m_progressBar.Minimum = 0;
      this.m_progressBar.Maximum = 100;
      this.m_progressBar.Step = 1;
      this.m_progressLabel = new Label();
      this.m_progressLabel.Name = nameof (m_progressLabel);
      this.m_progressLabel.Dock = DockStyle.Left;
      this.m_progressLabel.TextAlign = ContentAlignment.TopLeft;
      this.m_progressLabel.AutoSize = true;
      this.m_progressLabel.Padding = new Padding(6, 1, 0, 3).LogicalToDeviceUnits();
      this.m_progressPanel = new Panel();
      this.m_progressPanel.Dock = DockStyle.Right;
      this.m_progressPanel.Size = new Size(this.m_progressBar.Width, this.Height);
      this.m_progressPanel.Padding = new Padding(0, 2, 0, 0).LogicalToDeviceUnits();
      this.m_progressPanel.Visible = false;
      this.m_progressPanel.Controls.Add((Control) this.m_progressBar);
      this.m_progressPanel.Controls.Add((Control) this.m_progressLabel);
      this.m_progressPanel.Resize += new EventHandler(this.m_progressPanel_Resize);
      this.m_icon = new PictureBox();
      this.m_icon.Name = "InformationBarIcon";
      this.m_icon.Dock = DockStyle.Left;
      this.m_icon.Visible = false;
      this.m_icon.SizeMode = PictureBoxSizeMode.Normal;
      this.m_icon.Size = new Size(21, 21).LogicalToDeviceUnits();
      this.m_icon.Padding = new Padding(3, 2, 0, 0).LogicalToDeviceUnits();
      this.m_icon.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.Controls.Add((Control) this.m_labelText);
      this.Controls.Add((Control) this.m_progressPanel);
      this.Controls.Add((Control) this.m_icon);
      this.Height = DpiHelper.LogicalToDeviceUnitsY(23);
      this.MinimumSize = new Size(100, 23).LogicalToDeviceUnits();
      this.InitControlColors();
    }

    private void m_progressPanel_Resize(object sender, EventArgs e) => this.m_progressPanel.Width = this.m_progressLabel.Width + DpiHelper.LogicalToDeviceUnitsX(100);

    private void InitControlColors()
    {
      this.UseInnerColor = true;
      this.m_infoBackColor = UIHost.Colors.InfoBackColor;
      this.m_infoForeColor = UIHost.Colors.InfoForeColor;
      this.m_warningBackColor = UIHost.Colors.InfoBackColor;
      this.m_warningForeColor = UIHost.Colors.InfoForeColor;
      this.m_linkForeColor = this.m_labelText.LinkColor;
      this.UpdateControlColors();
    }

    private void UpdateControlColors()
    {
      this.ForeColor = this.Icon == InformationBar.IconType.Warning || this.Icon == InformationBar.IconType.Error ? this.WarningForeColor : this.InfoForeColor;
      this.m_labelText.LinkColor = this.LinkForeColor;
      this.m_labelText.ActiveLinkColor = this.LinkForeColor;
      this.m_labelText.VisitedLinkColor = this.LinkForeColor;
      this.m_progressBar.ForeColor = UIHost.Colors.ProgressFillColor;
      Color color = this.Icon == InformationBar.IconType.Warning || this.Icon == InformationBar.IconType.Error ? this.WarningBackColor : this.InfoBackColor;
      this.m_labelText.BackColor = color;
      this.m_progressBar.BackColor = color;
      this.m_progressPanel.BackColor = color;
      this.m_icon.BackColor = color;
      this.InnerColor = color;
    }

    public void StartMarquee(int speed)
    {
      this.m_progressPanel.Visible = true;
      this.m_progressBar.Style = ProgressBarStyle.Marquee;
      this.m_progressBar.MarqueeAnimationSpeed = speed;
      this.AdjustHeightToText();
    }

    public void StopMarquee()
    {
      this.m_progressBar.Style = ProgressBarStyle.Blocks;
      this.m_progressPanel.Visible = false;
      this.AdjustHeightToText();
    }

    private void OnLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      if (this.LabelLinkClicked == null)
        return;
      this.LabelLinkClicked(sender, e);
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (this.m_resizing)
        return;
      this.m_resizing = true;
      try
      {
        if (!this.IsHandleCreated)
          return;
        this.BeginInvoke((Delegate) (() => this.AdjustHeightToText()));
      }
      finally
      {
        this.m_resizing = false;
      }
    }

    private void AdjustHeightToText()
    {
      try
      {
        this.Update();
        int num = this.PreferredSize.Height;
        if (this.m_maxHeight > 0 && num > this.m_maxHeight)
          num = this.m_maxHeight;
        this.Height = num;
        this.m_progressPanel.Height = Math.Min(this.m_progressPanel.Height, this.Height - this.Padding.Vertical);
        this.m_progressBar.Height = Math.Min(this.m_progressBar.Height, this.m_progressPanel.Height - this.m_progressPanel.Padding.Vertical);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceAndDebugFailException(ex);
      }
    }

    private void UpdateProgress()
    {
      if (this.m_progressBarValue >= 0 && this.m_progressBarValue <= 100)
        this.m_progressBar.Value = this.m_progressBarValue;
      if (this.m_progressBarValue < 0 && this.m_progressPanel.Visible)
        this.m_progressPanel.Visible = false;
      else if (this.m_progressBarValue >= 0 && !this.m_progressPanel.Visible)
        this.m_progressPanel.Visible = true;
      this.AdjustHeightToText();
    }

    private void UpdateIcon()
    {
      switch (this.m_iconType)
      {
        case InformationBar.IconType.Warning:
        case InformationBar.IconType.Error:
        case InformationBar.IconType.Info:
          this.m_icon.Image = InformationBar.GetImageFromCache((int) this.m_iconType);
          this.m_icon.Visible = true;
          break;
        default:
          this.m_icon.Visible = false;
          this.m_icon.Image = InformationBar.GetImageFromCache(0);
          break;
      }
      this.AdjustHeightToText();
      this.UpdateControlColors();
      this.Invalidate();
    }

    private static Image GetImageFromCache(int index)
    {
      Image image = (Image) null;
      if (!InformationBar.s_imagesCache.TryGetValue(index, out image))
      {
        lock (InformationBar.s_imagesLock)
        {
          if (!InformationBar.s_imagesCache.TryGetValue(index, out image))
          {
            image = InformationBar.s_images.Images[index];
            DpiHelper.LogicalToDeviceUnits(ref image, DpiHelper.ImageScalingMode);
            InformationBar.s_imagesCache[index] = image;
          }
        }
      }
      return image;
    }

    public Color WarningBackColor
    {
      get => this.m_warningBackColor;
      set
      {
        this.m_warningBackColor = value;
        this.UpdateControlColors();
      }
    }

    public Color WarningForeColor
    {
      get => this.m_warningForeColor;
      set
      {
        this.m_warningForeColor = value;
        this.UpdateControlColors();
      }
    }

    public Color InfoBackColor
    {
      get => this.m_infoBackColor;
      set
      {
        this.m_infoBackColor = value;
        this.UpdateControlColors();
      }
    }

    public Color InfoForeColor
    {
      get => this.m_infoForeColor;
      set
      {
        this.m_infoForeColor = value;
        this.UpdateControlColors();
      }
    }

    public Color LinkForeColor
    {
      get => this.m_linkForeColor;
      set
      {
        this.m_linkForeColor = value;
        this.UpdateControlColors();
      }
    }

    public override string Text
    {
      get => this.m_labelText.Text;
      set
      {
        if (this.m_labelText.Text != value)
          this.m_labelText.Links.Clear();
        this.m_labelText.Text = value;
        this.AdjustHeightToText();
      }
    }

    public string ProgressText
    {
      get => this.m_progressLabel.Text;
      set
      {
        this.m_progressLabel.Text = value;
        this.m_progressPanel_Resize((object) null, EventArgs.Empty);
        this.AdjustHeightToText();
      }
    }

    public InformationBar.IconType Icon
    {
      get => this.m_iconType;
      set
      {
        if (this.m_iconType == value)
          return;
        this.m_iconType = value;
        this.UpdateIcon();
      }
    }

    public int ProgressBarValue
    {
      get => this.m_progressBarValue;
      set
      {
        this.m_progressBarValue = value;
        this.UpdateProgress();
      }
    }

    public new Size PreferredSize
    {
      get
      {
        int height = (!this.m_wrapText ? this.m_labelText.GetPreferredSize(new Size(int.MaxValue, 1)) : this.m_labelText.GetPreferredSize(new Size(this.m_labelText.Width, int.MaxValue))).Height;
        Padding padding = this.Padding;
        int top = padding.Top;
        int num = height + top;
        padding = this.Padding;
        int bottom = padding.Bottom;
        return new Size(this.Width, Math.Max(num + bottom, this.MinimumSize.Height));
      }
    }

    public int AutoGrowMaxHeight
    {
      get => this.m_maxHeight;
      set => this.m_maxHeight = value;
    }

    public bool WrapText
    {
      get => this.m_wrapText;
      set
      {
        this.m_wrapText = value;
        this.AdjustHeightToText();
      }
    }

    public LinkLabel.LinkCollection LabelLinks => this.m_labelText.Links;

    public event LinkLabelLinkClickedEventHandler LabelLinkClicked;

    internal Panel ProgressBarPanelInternal => this.m_progressPanel;

    public enum IconType
    {
      None,
      Warning,
      Error,
      Info,
    }
  }
}
