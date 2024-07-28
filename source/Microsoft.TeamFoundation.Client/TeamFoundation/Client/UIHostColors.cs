// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.UIHostColors
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;
using System.Drawing;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UIHostColors
  {
    public virtual Color BorderColor => SystemColors.ControlDark;

    public virtual Color DefaultInfoBackColor => SystemColors.Info;

    public virtual Color InfoBackColor => SystemColors.Info;

    public virtual Color InfoForeColor => SystemColors.InfoText;

    public virtual Color ControlBackColor => SystemColors.Window;

    public virtual Color ControlForeColor => SystemColors.WindowText;

    public virtual Color ReadOnlyControlBackColor => this.ControlBackColor;

    public virtual Color ReadOnlyControlForeColor => this.GrayTextColor;

    public virtual Color InvalidControlBackColor => SystemColors.Info;

    public virtual Color InvalidControlForeColor => SystemColors.InfoText;

    public virtual Color ErrorControlBackColor => this.ControlBackColor;

    public virtual Color ErrorControlForeColor => this.GrayTextColor;

    public virtual Color ProgressFillColor => SystemColors.Highlight;

    public virtual Color QueryBuilderGroupedBackColor => SystemColors.Window;

    public virtual Color QueryBuilderGroupedLineColor => SystemColors.GrayText;

    public virtual Color SortedColumnBackColor => SystemColors.Control;

    public virtual Color SortedColumnForeColor => SystemColors.ControlText;

    public virtual Color GrayTextColor => SystemColors.GrayText;

    public virtual Color GridForeColor => SystemColors.WindowText;

    public virtual Color GridBackColor => SystemColors.Window;

    public virtual Color GridHeaderForeColor => SystemColors.ControlText;

    public virtual Color GridHeaderBackColor => SystemColors.Control;

    public virtual Color GridRowHeaderSelectionBackColor => SystemColors.ControlDark;

    public virtual Color GridRowHeaderSelectionForeColor => SystemColors.ControlLightLight;

    public virtual Color GridUnfocusedSelectedRowBackColor => SystemColors.Control;

    public virtual Color GridUnfocusedSelectedRowForeColor => SystemColors.ControlText;

    public virtual Color GridTargetRowBackColor => SystemColors.Info;

    public virtual Color GridTargetRowForeColor => SystemColors.InfoText;

    public virtual Color GridSummaryRowBackColor => SystemColors.Window;

    public virtual Color GridSummaryRowForeColor => SystemColors.WindowText;

    public virtual Color GridLineColor => SystemColors.ControlLight;

    public virtual Color HighlightForeColor => SystemColors.HighlightText;

    public virtual Color HighlightBackColor => SystemColors.Highlight;

    public virtual Color FrameColor => SystemColors.WindowFrame;

    public virtual Color FormBackColor => SystemColors.Control;

    public virtual Color FormForeColor => SystemColors.ControlText;

    public virtual Color FormDarkColor => SystemColors.ControlDark;

    public virtual Color FormDarkDarkColor => SystemColors.ControlDarkDark;

    public virtual Color FormLightColor => SystemColors.ControlLight;

    public virtual Color VisualSurfaceSubItemBorder => Color.FromArgb(119, 153, 181);

    public virtual Color VisualSurfaceSubItemGradientBegin => Color.FromArgb(198, 212, 223);

    public virtual Color VisualSurfaceSubItemGradientEnd => Color.FromArgb(184, 204, 215);

    public virtual Color VisualSurfaceSubItemText => Color.FromArgb(89, 89, 89);

    public virtual Color VisualSurfaceMainItemBorder => Color.FromArgb(81, 118, 42);

    public virtual Color VisualSurfaceMainItemGradientBegin => Color.FromArgb(183, 207, 128);

    public virtual Color VisualSurfaceMainItemGradientEnd => Color.FromArgb(159, 184, 97);

    public virtual Color VisualSurfaceMainItemText => Color.FromArgb(0, 0, 0);

    public virtual Color VisualSurfaceOutline => Color.FromArgb(89, 89, 89);

    public virtual Color ActiveTabForeground => this.ControlForeColor;

    public virtual Color TabForeground => this.FormDarkDarkColor;

    public virtual Color TabBackground => this.FormBackColor;

    public virtual Color TabHoverForeground => UIHostColors.CombineColors(this.TabBackground, 50, this.TabForeground, 50);

    public virtual Color HotTrackColor => SystemColors.HotTrack;

    public static Color CombineColors(Color c1, int a1, Color c2, int a2) => Color.FromArgb(((int) c1.A * a1 + (int) c2.A * a2) / 100, ((int) c1.R * a1 + (int) c2.R * a2) / 100, ((int) c1.G * a1 + (int) c2.G * a2) / 100, ((int) c1.B * a1 + (int) c2.B * a2) / 100);
  }
}
