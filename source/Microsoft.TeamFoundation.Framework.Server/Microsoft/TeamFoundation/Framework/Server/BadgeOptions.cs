// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BadgeOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct BadgeOptions
  {
    public BadgeOptions(
      BadgeLogo logoType,
      string leftText = "",
      string rightText = "",
      string iconFill = "#fff",
      string leftForeground = "#fff",
      string leftBackground = "#555555",
      string rightForeground = "#fff",
      string rightBackground = "")
      : this()
    {
      this.LogoType = logoType;
      this.LeftText = leftText;
      this.RightText = rightText;
      this.IconFill = iconFill;
      this.LeftForeground = leftForeground;
      this.LeftBackground = leftBackground;
      this.RightForeground = rightForeground;
      this.RightBackground = rightBackground;
    }

    public BadgeLogo LogoType { get; }

    public string LeftText { get; set; }

    public string RightText { get; set; }

    public string IconFill { get; set; }

    public string LeftForeground { get; set; }

    public string LeftBackground { get; set; }

    public string RightForeground { get; set; }

    public string RightBackground { get; set; }

    public static class DefaultColors
    {
      public const string LeftBackground = "#555555";
      public const string RightBackgroundFailed = "#F34235";
      public const string RightBackgroundPartiallySucceeded = "#FEC006";
      public const string RightBackgroundSucceeded = "#4EC820";
      public const string RightBackgroundNone = "#BBBBBB";
      public const string RightBackgroundNoDefinition = "#007ACC";
      public const string RightBackgroundNoRuns = "#4da2db";
      public const string IconFillLight = "#fff";
      public const string ForegroundLight = "#fff";
      public const string ForegroundDark = "#000";
    }
  }
}
