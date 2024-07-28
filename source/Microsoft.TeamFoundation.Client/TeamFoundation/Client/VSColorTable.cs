// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.VSColorTable
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VSColorTable : ProfessionalColorTable
  {
    private const string blueColorScheme = "NormalColor";
    private const string oliveColorScheme = "HomeStead";
    private const string silverColorScheme = "Metallic";
    private Dictionary<VSColorTable.KnownColors, System.Drawing.Color> vsRGB;

    private Dictionary<VSColorTable.KnownColors, System.Drawing.Color> ColorTable
    {
      get
      {
        if (this.vsRGB == null)
        {
          this.vsRGB = new Dictionary<VSColorTable.KnownColors, System.Drawing.Color>(209);
          this.InitTanLunaColors(ref this.vsRGB);
        }
        return this.vsRGB;
      }
    }

    internal static string ColorScheme => VSColorTable.DisplayInformation.ColorScheme;

    private bool UseBaseColorTable
    {
      get
      {
        int num = VSColorTable.ColorScheme == "HomeStead" || VSColorTable.ColorScheme == "NormalColor" ? (UIHost.IsVistaOrNewer ? 1 : 0) : 1;
        if (num == 0)
          return num != 0;
        if (this.vsRGB == null)
          return num != 0;
        this.vsRGB.Clear();
        this.vsRGB = (Dictionary<VSColorTable.KnownColors, System.Drawing.Color>) null;
        return num != 0;
      }
    }

    internal System.Drawing.Color FromKnownColor(VSColorTable.KnownColors color) => this.ColorTable[color];

    public override System.Drawing.Color ButtonPressedBorder => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver) : base.ButtonPressedBorder;

    public override System.Drawing.Color ButtonSelectedBorder => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver) : base.ButtonSelectedBorder;

    public override System.Drawing.Color ButtonCheckedGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradSelectedBegin) : base.ButtonCheckedGradientBegin;

    public override System.Drawing.Color ButtonCheckedGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradSelectedMiddle) : base.ButtonCheckedGradientMiddle;

    public override System.Drawing.Color ButtonCheckedGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradSelectedEnd) : base.ButtonCheckedGradientEnd;

    public override System.Drawing.Color ButtonSelectedGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseOverBegin) : base.ButtonSelectedGradientBegin;

    public override System.Drawing.Color ButtonSelectedGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseOverMiddle) : base.ButtonSelectedGradientMiddle;

    public override System.Drawing.Color ButtonSelectedGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseOverEnd) : base.ButtonSelectedGradientEnd;

    public override System.Drawing.Color ButtonPressedGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseDownBegin) : base.ButtonPressedGradientBegin;

    public override System.Drawing.Color ButtonPressedGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseDownMiddle) : base.ButtonPressedGradientMiddle;

    public override System.Drawing.Color ButtonPressedGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseDownEnd) : base.ButtonPressedGradientEnd;

    public override System.Drawing.Color CheckBackground => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBCtlBkgdSelected) : base.CheckBackground;

    public override System.Drawing.Color CheckSelectedBackground => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver) : base.CheckSelectedBackground;

    public override System.Drawing.Color CheckPressedBackground => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver) : base.CheckPressedBackground;

    public override System.Drawing.Color GripDark => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBDragHandle) : base.GripDark;

    public override System.Drawing.Color GripLight => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBDragHandleShadow) : base.GripLight;

    public override System.Drawing.Color ImageMarginGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradVertBegin) : base.ImageMarginGradientBegin;

    public override System.Drawing.Color ImageMarginGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradVertMiddle) : base.ImageMarginGradientMiddle;

    public override System.Drawing.Color ImageMarginGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradVertEnd) : base.ImageMarginGradientEnd;

    public override System.Drawing.Color ImageMarginRevealedGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin) : base.ImageMarginRevealedGradientBegin;

    public override System.Drawing.Color ImageMarginRevealedGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle) : base.ImageMarginRevealedGradientMiddle;

    public override System.Drawing.Color ImageMarginRevealedGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd) : base.ImageMarginRevealedGradientEnd;

    public override System.Drawing.Color MenuStripGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin) : base.MenuStripGradientBegin;

    public override System.Drawing.Color MenuStripGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd) : base.MenuStripGradientEnd;

    public override System.Drawing.Color MenuItemSelected => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver) : base.MenuItemSelected;

    public override System.Drawing.Color MenuItemBorder => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBMenuBdrOuter) : base.MenuItemBorder;

    public override System.Drawing.Color MenuItemSelectedGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseOverBegin) : base.MenuItemSelectedGradientBegin;

    public override System.Drawing.Color MenuItemSelectedGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMouseOverEnd) : base.MenuItemSelectedGradientEnd;

    public override System.Drawing.Color MenuItemPressedGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdBegin) : base.MenuItemPressedGradientBegin;

    public override System.Drawing.Color MenuItemPressedGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle) : base.MenuItemPressedGradientMiddle;

    public override System.Drawing.Color MenuItemPressedGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdEnd) : base.MenuItemPressedGradientEnd;

    public override System.Drawing.Color RaftingContainerGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin) : base.RaftingContainerGradientBegin;

    public override System.Drawing.Color RaftingContainerGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd) : base.RaftingContainerGradientEnd;

    public override System.Drawing.Color SeparatorDark => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBSplitterLine) : base.SeparatorDark;

    public override System.Drawing.Color SeparatorLight => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBSplitterLineLight) : base.SeparatorLight;

    public override System.Drawing.Color ToolStripBorder => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBShadow) : base.ToolStripBorder;

    public override System.Drawing.Color ToolStripDropDownBackground => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBMenuBkgd) : base.ToolStripDropDownBackground;

    public override System.Drawing.Color ToolStripGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradVertBegin) : base.ToolStripGradientBegin;

    public override System.Drawing.Color ToolStripGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradVertMiddle) : base.ToolStripGradientMiddle;

    public override System.Drawing.Color ToolStripGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradVertEnd) : base.ToolStripGradientEnd;

    public override System.Drawing.Color OverflowButtonGradientBegin => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradOptionsBegin) : base.OverflowButtonGradientBegin;

    public override System.Drawing.Color OverflowButtonGradientMiddle => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradOptionsMiddle) : base.OverflowButtonGradientMiddle;

    public override System.Drawing.Color OverflowButtonGradientEnd => !this.UseBaseColorTable ? this.FromKnownColor(VSColorTable.KnownColors.msocbvcrCBGradOptionsEnd) : base.OverflowButtonGradientEnd;

    internal void InitTanLunaColors(
      ref Dictionary<VSColorTable.KnownColors, System.Drawing.Color> rgbTable)
    {
      rgbTable[VSColorTable.KnownColors.msocbvcrCBBkgd] = System.Drawing.Color.FromArgb(239, 237, 222);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBDragHandle] = System.Drawing.Color.FromArgb(193, 190, 179);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBSplitterLine] = System.Drawing.Color.FromArgb(197, 194, 184);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBTitleBkgd] = System.Drawing.Color.FromArgb(172, 168, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBTitleText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBBdrOuterFloating] = System.Drawing.Color.FromArgb(146, 143, 130);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBBdrOuterDocked] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBTearOffHandle] = System.Drawing.Color.FromArgb(239, 237, 222);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBTearOffHandleMouseOver] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBkgd] = System.Drawing.Color.FromArgb(239, 237, 222);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlText] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlTextDisabled] = System.Drawing.Color.FromArgb(180, 177, 163);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBkgdMouseDown] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBdrMouseDown] = System.Drawing.Color.FromArgb(75, 75, 111);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlTextMouseDown] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBkgdSelected] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBdrSelected] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = System.Drawing.Color.FromArgb(75, 75, 111);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlBkgdLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBCtlTextLight] = System.Drawing.Color.FromArgb(128, 128, 128);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMainMenuBkgd] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuBkgd] = System.Drawing.Color.FromArgb(252, 252, 249);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuCtlText] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuCtlTextDisabled] = System.Drawing.Color.FromArgb(197, 194, 184);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuBdrOuter] = System.Drawing.Color.FromArgb(138, 134, 122);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuIconBkgd] = System.Drawing.Color.FromArgb(239, 237, 222);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] = System.Drawing.Color.FromArgb(230, 227, 210);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuSplitArrow] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPBkgd] = System.Drawing.Color.FromArgb(246, 244, 236);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPTitleBkgdActive] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPTitleBkgdInactive] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPTitleTextActive] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPTitleTextInactive] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPBdrOuterFloating] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPBdrOuterDocked] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlBdrDisabled] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlTextDisabled] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlBkgdDisabled] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPGroupline] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrSBBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOBBkgdBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOBBkgdBdrContrast] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOABBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGDHeaderBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGDHeaderBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGDHeaderCellBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGDHeaderSeeThroughSelection] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGDHeaderCellBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGDHeaderCellBkgdSelected] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBSplitterLineLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBShadow] = System.Drawing.Color.FromArgb(146, 146, 118);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBOptionsButtonShadow] = System.Drawing.Color.FromArgb(238, 238, 244);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPNavBarBkgnd] = System.Drawing.Color.FromArgb(197, 194, 184);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPBdrInnerDocked] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBLabelBkgnd] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBIconDisabledLight] = System.Drawing.Color.FromArgb(247, 245, 249);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBIconDisabledDark] = System.Drawing.Color.FromArgb(122, 121, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBLowColorIconDisabled] = System.Drawing.Color.FromArgb(180, 177, 163);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin] = System.Drawing.Color.FromArgb(229, 229, 215);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd] = System.Drawing.Color.FromArgb(251, 250, 247);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradVertBegin] = System.Drawing.Color.FromArgb(252, 252, 249);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradVertMiddle] = System.Drawing.Color.FromArgb(246, 244, 236);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradVertEnd] = System.Drawing.Color.FromArgb(176, 176, 147);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsBegin] = System.Drawing.Color.FromArgb(243, 242, 240);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsMiddle] = System.Drawing.Color.FromArgb(226, 225, 219);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsEnd] = System.Drawing.Color.FromArgb(146, 146, 118);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = System.Drawing.Color.FromArgb(252, 252, 249);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = System.Drawing.Color.FromArgb(246, 244, 236);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = System.Drawing.Color.FromArgb(247, 246, 239);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = System.Drawing.Color.FromArgb(242, 240, 228);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = System.Drawing.Color.FromArgb(230, 227, 210);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsSelectedBegin] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsSelectedEnd] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradSelectedBegin] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradSelectedMiddle] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradSelectedEnd] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMouseOverBegin] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMouseOverMiddle] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMouseOverEnd] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMouseDownBegin] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMouseDownMiddle] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBGradMouseDownEnd] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[VSColorTable.KnownColors.msocbvcrNetLookBkgnd] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBMenuShadow] = System.Drawing.Color.FromArgb(252, 252, 249);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBDockSeparatorLine] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBDropDownArrow] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKGridlines] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKGroupText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKGroupLine] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKGroupShaded] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKGroupNested] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKIconBar] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKFlagNone] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKFolderbarLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKFolderbarDark] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKFolderbarText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBButtonLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBButtonDark] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBSelectedButtonLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBSelectedButtonDark] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBHoverButtonLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBHoverButtonDark] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBPressedButtonLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBPressedButtonDark] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBDarkOutline] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBSplitterDark] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBActionDividerLine] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBLabelText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKWBFoldersBackground] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKTodayIndicatorLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKTodayIndicatorDark] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKInfoBarBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKInfoBarText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOLKPreviewPaneLabelText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrHyperlink] = System.Drawing.Color.FromArgb(0, 61, 178);
      rgbTable[VSColorTable.KnownColors.msocbvcrHyperlinkFollowed] = System.Drawing.Color.FromArgb(170, 0, 170);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGWorkspaceBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerActiveBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerInactiveBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerTabStopTicks] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerTabBoxBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrXLFormulaBarBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrCBDragHandleShadow] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[VSColorTable.KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelected] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPSlideBdrInactiveSelected] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPSlideBdrMouseOver] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDlgGroupBoxText] = System.Drawing.Color.FromArgb(7, 70, 213);
      rgbTable[VSColorTable.KnownColors.msocbvcrScrollbarBkgd] = System.Drawing.Color.FromArgb(246, 244, 236);
      rgbTable[VSColorTable.KnownColors.msocbvcrListHeaderArrow] = System.Drawing.Color.FromArgb(156, 154, 143);
      rgbTable[VSColorTable.KnownColors.msocbvcrDisabledHighlightedText] = System.Drawing.Color.FromArgb(187, 206, 236);
      rgbTable[VSColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrDisabledFocuslessHighlightedText] = System.Drawing.Color.FromArgb(172, 168, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPCtlTextMouseDown] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPTextDisabled] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPInfoTipBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrWPInfoTipText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWActiveTabText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWInactiveTabText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWTabBkgdMouseOver] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWTabTextMouseOver] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWTabBkgdMouseDown] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDWTabTextMouseDown] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPLightBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPDarkBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupContentLightBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupContentDarkBkgd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupContentText] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupContentTextDisabled] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPGroupline] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrGSPHyperlink] = System.Drawing.Color.FromArgb((int) byte.MaxValue, 51, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBkgd] = System.Drawing.Color.FromArgb(212, 212, 226);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabText] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdr] = System.Drawing.Color.FromArgb(118, 116, 146);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrDark] = System.Drawing.Color.FromArgb(186, 185, 206);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBkgdSelected] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabTextSelected] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrSelected] = System.Drawing.Color.FromArgb(124, 124, 148);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBkgdMouseDown] = System.Drawing.Color.FromArgb(154, 183, 228);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabTextMouseDown] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrMouseDown] = System.Drawing.Color.FromArgb(75, 75, 111);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrLightMouseDown] = System.Drawing.Color.FromArgb(75, 75, 111);
      rgbTable[VSColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseDown] = System.Drawing.Color.FromArgb(75, 75, 111);
      rgbTable[VSColorTable.KnownColors.msocbvcrToastGradBegin] = System.Drawing.Color.FromArgb(246, 244, 236);
      rgbTable[VSColorTable.KnownColors.msocbvcrToastGradEnd] = System.Drawing.Color.FromArgb(179, 178, 204);
      rgbTable[VSColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = System.Drawing.Color.FromArgb(236, 233, 216);
      rgbTable[VSColorTable.KnownColors.msocbvcrJotNavUIGradEnd] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[VSColorTable.KnownColors.msocbvcrJotNavUIText] = System.Drawing.Color.FromArgb(0, 0, 0);
      rgbTable[VSColorTable.KnownColors.msocbvcrJotNavUIBdr] = System.Drawing.Color.FromArgb(172, 168, 153);
      rgbTable[VSColorTable.KnownColors.msocbvcrPlacesBarBkgd] = System.Drawing.Color.FromArgb(224, 223, 227);
      rgbTable[VSColorTable.KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[VSColorTable.KnownColors.msocbvcrPubWebDocScratchPageBkgd] = System.Drawing.Color.FromArgb(193, 210, 238);
    }

    private static class DisplayInformation
    {
      [ThreadStatic]
      private static string colorScheme;

      static DisplayInformation()
      {
        SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(VSColorTable.DisplayInformation.OnUserPreferenceChanged);
        VSColorTable.DisplayInformation.SetScheme();
      }

      internal static string ColorScheme => VSColorTable.DisplayInformation.colorScheme;

      private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) => VSColorTable.DisplayInformation.SetScheme();

      private static void SetScheme()
      {
        if (VisualStyleRenderer.IsSupported)
          VSColorTable.DisplayInformation.colorScheme = VisualStyleInformation.ColorScheme;
        else
          VSColorTable.DisplayInformation.colorScheme = (string) null;
      }
    }

    internal enum KnownColors
    {
      msocbvcrCBBdrOuterDocked = 0,
      msocbvcrCBBdrOuterFloating = 1,
      msocbvcrCBBkgd = 2,
      msocbvcrCBCtlBdrMouseDown = 3,
      msocbvcrCBCtlBdrMouseOver = 4,
      msocbvcrCBCtlBdrSelected = 5,
      msocbvcrCBCtlBdrSelectedMouseOver = 6,
      msocbvcrCBCtlBkgd = 7,
      msocbvcrCBCtlBkgdLight = 8,
      msocbvcrCBCtlBkgdMouseDown = 9,
      msocbvcrCBCtlBkgdMouseOver = 10, // 0x0000000A
      msocbvcrCBCtlBkgdSelected = 11, // 0x0000000B
      msocbvcrCBCtlBkgdSelectedMouseOver = 12, // 0x0000000C
      msocbvcrCBCtlText = 13, // 0x0000000D
      msocbvcrCBCtlTextDisabled = 14, // 0x0000000E
      msocbvcrCBCtlTextLight = 15, // 0x0000000F
      msocbvcrCBCtlTextMouseDown = 16, // 0x00000010
      msocbvcrCBCtlTextMouseOver = 17, // 0x00000011
      msocbvcrCBDockSeparatorLine = 18, // 0x00000012
      msocbvcrCBDragHandle = 19, // 0x00000013
      msocbvcrCBDragHandleShadow = 20, // 0x00000014
      msocbvcrCBDropDownArrow = 21, // 0x00000015
      msocbvcrCBGradMainMenuHorzBegin = 22, // 0x00000016
      msocbvcrCBGradMainMenuHorzEnd = 23, // 0x00000017
      msocbvcrCBGradMenuIconBkgdDroppedBegin = 24, // 0x00000018
      msocbvcrCBGradMenuIconBkgdDroppedEnd = 25, // 0x00000019
      msocbvcrCBGradMenuIconBkgdDroppedMiddle = 26, // 0x0000001A
      msocbvcrCBGradMenuTitleBkgdBegin = 27, // 0x0000001B
      msocbvcrCBGradMenuTitleBkgdEnd = 28, // 0x0000001C
      msocbvcrCBGradMouseDownBegin = 29, // 0x0000001D
      msocbvcrCBGradMouseDownEnd = 30, // 0x0000001E
      msocbvcrCBGradMouseDownMiddle = 31, // 0x0000001F
      msocbvcrCBGradMouseOverBegin = 32, // 0x00000020
      msocbvcrCBGradMouseOverEnd = 33, // 0x00000021
      msocbvcrCBGradMouseOverMiddle = 34, // 0x00000022
      msocbvcrCBGradOptionsBegin = 35, // 0x00000023
      msocbvcrCBGradOptionsEnd = 36, // 0x00000024
      msocbvcrCBGradOptionsMiddle = 37, // 0x00000025
      msocbvcrCBGradOptionsMouseOverBegin = 38, // 0x00000026
      msocbvcrCBGradOptionsMouseOverEnd = 39, // 0x00000027
      msocbvcrCBGradOptionsMouseOverMiddle = 40, // 0x00000028
      msocbvcrCBGradOptionsSelectedBegin = 41, // 0x00000029
      msocbvcrCBGradOptionsSelectedEnd = 42, // 0x0000002A
      msocbvcrCBGradOptionsSelectedMiddle = 43, // 0x0000002B
      msocbvcrCBGradSelectedBegin = 44, // 0x0000002C
      msocbvcrCBGradSelectedEnd = 45, // 0x0000002D
      msocbvcrCBGradSelectedMiddle = 46, // 0x0000002E
      msocbvcrCBGradVertBegin = 47, // 0x0000002F
      msocbvcrCBGradVertEnd = 48, // 0x00000030
      msocbvcrCBGradVertMiddle = 49, // 0x00000031
      msocbvcrCBIconDisabledDark = 50, // 0x00000032
      msocbvcrCBIconDisabledLight = 51, // 0x00000033
      msocbvcrCBLabelBkgnd = 52, // 0x00000034
      msocbvcrCBLowColorIconDisabled = 53, // 0x00000035
      msocbvcrCBMainMenuBkgd = 54, // 0x00000036
      msocbvcrCBMenuBdrOuter = 55, // 0x00000037
      msocbvcrCBMenuBkgd = 56, // 0x00000038
      msocbvcrCBMenuCtlText = 57, // 0x00000039
      msocbvcrCBMenuCtlTextDisabled = 58, // 0x0000003A
      msocbvcrCBMenuIconBkgd = 59, // 0x0000003B
      msocbvcrCBMenuIconBkgdDropped = 60, // 0x0000003C
      msocbvcrCBMenuShadow = 61, // 0x0000003D
      msocbvcrCBMenuSplitArrow = 62, // 0x0000003E
      msocbvcrCBOptionsButtonShadow = 63, // 0x0000003F
      msocbvcrCBShadow = 64, // 0x00000040
      msocbvcrCBSplitterLine = 65, // 0x00000041
      msocbvcrCBSplitterLineLight = 66, // 0x00000042
      msocbvcrCBTearOffHandle = 67, // 0x00000043
      msocbvcrCBTearOffHandleMouseOver = 68, // 0x00000044
      msocbvcrCBTitleBkgd = 69, // 0x00000045
      msocbvcrCBTitleText = 70, // 0x00000046
      msocbvcrDisabledFocuslessHighlightedText = 71, // 0x00000047
      msocbvcrDisabledHighlightedText = 72, // 0x00000048
      msocbvcrDlgGroupBoxText = 73, // 0x00000049
      msocbvcrDocTabBdr = 74, // 0x0000004A
      msocbvcrDocTabBdrDark = 75, // 0x0000004B
      msocbvcrDocTabBdrDarkMouseDown = 76, // 0x0000004C
      msocbvcrDocTabBdrDarkMouseOver = 77, // 0x0000004D
      msocbvcrDocTabBdrLight = 78, // 0x0000004E
      msocbvcrDocTabBdrLightMouseDown = 79, // 0x0000004F
      msocbvcrDocTabBdrLightMouseOver = 80, // 0x00000050
      msocbvcrDocTabBdrMouseDown = 81, // 0x00000051
      msocbvcrDocTabBdrMouseOver = 82, // 0x00000052
      msocbvcrDocTabBdrSelected = 83, // 0x00000053
      msocbvcrDocTabBkgd = 84, // 0x00000054
      msocbvcrDocTabBkgdMouseDown = 85, // 0x00000055
      msocbvcrDocTabBkgdMouseOver = 86, // 0x00000056
      msocbvcrDocTabBkgdSelected = 87, // 0x00000057
      msocbvcrDocTabText = 88, // 0x00000058
      msocbvcrDocTabTextMouseDown = 89, // 0x00000059
      msocbvcrDocTabTextMouseOver = 90, // 0x0000005A
      msocbvcrDocTabTextSelected = 91, // 0x0000005B
      msocbvcrDWActiveTabBkgd = 92, // 0x0000005C
      msocbvcrDWActiveTabText = 93, // 0x0000005D
      msocbvcrDWActiveTabTextDisabled = 94, // 0x0000005E
      msocbvcrDWInactiveTabBkgd = 95, // 0x0000005F
      msocbvcrDWInactiveTabText = 96, // 0x00000060
      msocbvcrDWTabBkgdMouseDown = 97, // 0x00000061
      msocbvcrDWTabBkgdMouseOver = 98, // 0x00000062
      msocbvcrDWTabTextMouseDown = 99, // 0x00000063
      msocbvcrDWTabTextMouseOver = 100, // 0x00000064
      msocbvcrFocuslessHighlightedBkgd = 101, // 0x00000065
      msocbvcrFocuslessHighlightedText = 102, // 0x00000066
      msocbvcrGDHeaderBdr = 103, // 0x00000067
      msocbvcrGDHeaderBkgd = 104, // 0x00000068
      msocbvcrGDHeaderCellBdr = 105, // 0x00000069
      msocbvcrGDHeaderCellBkgd = 106, // 0x0000006A
      msocbvcrGDHeaderCellBkgdSelected = 107, // 0x0000006B
      msocbvcrGDHeaderSeeThroughSelection = 108, // 0x0000006C
      msocbvcrGSPDarkBkgd = 109, // 0x0000006D
      msocbvcrGSPGroupContentDarkBkgd = 110, // 0x0000006E
      msocbvcrGSPGroupContentLightBkgd = 111, // 0x0000006F
      msocbvcrGSPGroupContentText = 112, // 0x00000070
      msocbvcrGSPGroupContentTextDisabled = 113, // 0x00000071
      msocbvcrGSPGroupHeaderDarkBkgd = 114, // 0x00000072
      msocbvcrGSPGroupHeaderLightBkgd = 115, // 0x00000073
      msocbvcrGSPGroupHeaderText = 116, // 0x00000074
      msocbvcrGSPGroupline = 117, // 0x00000075
      msocbvcrGSPHyperlink = 118, // 0x00000076
      msocbvcrGSPLightBkgd = 119, // 0x00000077
      msocbvcrHyperlink = 120, // 0x00000078
      msocbvcrHyperlinkFollowed = 121, // 0x00000079
      msocbvcrJotNavUIBdr = 122, // 0x0000007A
      msocbvcrJotNavUIGradBegin = 123, // 0x0000007B
      msocbvcrJotNavUIGradEnd = 124, // 0x0000007C
      msocbvcrJotNavUIGradMiddle = 125, // 0x0000007D
      msocbvcrJotNavUIText = 126, // 0x0000007E
      msocbvcrListHeaderArrow = 127, // 0x0000007F
      msocbvcrNetLookBkgnd = 128, // 0x00000080
      msocbvcrOABBkgd = 129, // 0x00000081
      msocbvcrOBBkgdBdr = 130, // 0x00000082
      msocbvcrOBBkgdBdrContrast = 131, // 0x00000083
      msocbvcrOGMDIParentWorkspaceBkgd = 132, // 0x00000084
      msocbvcrOGRulerActiveBkgd = 133, // 0x00000085
      msocbvcrOGRulerBdr = 134, // 0x00000086
      msocbvcrOGRulerBkgd = 135, // 0x00000087
      msocbvcrOGRulerInactiveBkgd = 136, // 0x00000088
      msocbvcrOGRulerTabBoxBdr = 137, // 0x00000089
      msocbvcrOGRulerTabBoxBdrHighlight = 138, // 0x0000008A
      msocbvcrOGRulerTabStopTicks = 139, // 0x0000008B
      msocbvcrOGRulerText = 140, // 0x0000008C
      msocbvcrOGTaskPaneGroupBoxHeaderBkgd = 141, // 0x0000008D
      msocbvcrOGWorkspaceBkgd = 142, // 0x0000008E
      msocbvcrOLKFlagNone = 143, // 0x0000008F
      msocbvcrOLKFolderbarDark = 144, // 0x00000090
      msocbvcrOLKFolderbarLight = 145, // 0x00000091
      msocbvcrOLKFolderbarText = 146, // 0x00000092
      msocbvcrOLKGridlines = 147, // 0x00000093
      msocbvcrOLKGroupLine = 148, // 0x00000094
      msocbvcrOLKGroupNested = 149, // 0x00000095
      msocbvcrOLKGroupShaded = 150, // 0x00000096
      msocbvcrOLKGroupText = 151, // 0x00000097
      msocbvcrOLKIconBar = 152, // 0x00000098
      msocbvcrOLKInfoBarBkgd = 153, // 0x00000099
      msocbvcrOLKInfoBarText = 154, // 0x0000009A
      msocbvcrOLKPreviewPaneLabelText = 155, // 0x0000009B
      msocbvcrOLKTodayIndicatorDark = 156, // 0x0000009C
      msocbvcrOLKTodayIndicatorLight = 157, // 0x0000009D
      msocbvcrOLKWBActionDividerLine = 158, // 0x0000009E
      msocbvcrOLKWBButtonDark = 159, // 0x0000009F
      msocbvcrOLKWBButtonLight = 160, // 0x000000A0
      msocbvcrOLKWBDarkOutline = 161, // 0x000000A1
      msocbvcrOLKWBFoldersBackground = 162, // 0x000000A2
      msocbvcrOLKWBHoverButtonDark = 163, // 0x000000A3
      msocbvcrOLKWBHoverButtonLight = 164, // 0x000000A4
      msocbvcrOLKWBLabelText = 165, // 0x000000A5
      msocbvcrOLKWBPressedButtonDark = 166, // 0x000000A6
      msocbvcrOLKWBPressedButtonLight = 167, // 0x000000A7
      msocbvcrOLKWBSelectedButtonDark = 168, // 0x000000A8
      msocbvcrOLKWBSelectedButtonLight = 169, // 0x000000A9
      msocbvcrOLKWBSplitterDark = 170, // 0x000000AA
      msocbvcrOLKWBSplitterLight = 171, // 0x000000AB
      msocbvcrPlacesBarBkgd = 172, // 0x000000AC
      msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd = 173, // 0x000000AD
      msocbvcrPPOutlineThumbnailsPaneTabBdr = 174, // 0x000000AE
      msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd = 175, // 0x000000AF
      msocbvcrPPOutlineThumbnailsPaneTabText = 176, // 0x000000B0
      msocbvcrPPSlideBdrActiveSelected = 177, // 0x000000B1
      msocbvcrPPSlideBdrActiveSelectedMouseOver = 178, // 0x000000B2
      msocbvcrPPSlideBdrInactiveSelected = 179, // 0x000000B3
      msocbvcrPPSlideBdrMouseOver = 180, // 0x000000B4
      msocbvcrPubPrintDocScratchPageBkgd = 181, // 0x000000B5
      msocbvcrPubWebDocScratchPageBkgd = 182, // 0x000000B6
      msocbvcrSBBdr = 183, // 0x000000B7
      msocbvcrScrollbarBkgd = 184, // 0x000000B8
      msocbvcrToastGradBegin = 185, // 0x000000B9
      msocbvcrToastGradEnd = 186, // 0x000000BA
      msocbvcrWPBdrInnerDocked = 187, // 0x000000BB
      msocbvcrWPBdrOuterDocked = 188, // 0x000000BC
      msocbvcrWPBdrOuterFloating = 189, // 0x000000BD
      msocbvcrWPBkgd = 190, // 0x000000BE
      msocbvcrWPCtlBdr = 191, // 0x000000BF
      msocbvcrWPCtlBdrDefault = 192, // 0x000000C0
      msocbvcrWPCtlBdrDisabled = 193, // 0x000000C1
      msocbvcrWPCtlBkgd = 194, // 0x000000C2
      msocbvcrWPCtlBkgdDisabled = 195, // 0x000000C3
      msocbvcrWPCtlText = 196, // 0x000000C4
      msocbvcrWPCtlTextDisabled = 197, // 0x000000C5
      msocbvcrWPCtlTextMouseDown = 198, // 0x000000C6
      msocbvcrWPGroupline = 199, // 0x000000C7
      msocbvcrWPInfoTipBkgd = 200, // 0x000000C8
      msocbvcrWPInfoTipText = 201, // 0x000000C9
      msocbvcrWPNavBarBkgnd = 202, // 0x000000CA
      msocbvcrWPText = 203, // 0x000000CB
      msocbvcrWPTextDisabled = 204, // 0x000000CC
      msocbvcrWPTitleBkgdActive = 205, // 0x000000CD
      msocbvcrWPTitleBkgdInactive = 206, // 0x000000CE
      msocbvcrWPTitleTextActive = 207, // 0x000000CF
      msocbvcrWPTitleTextInactive = 208, // 0x000000D0
      lastKnownColor = 209, // 0x000000D1
      msocbvcrXLFormulaBarBkgd = 209, // 0x000000D1
    }
  }
}
