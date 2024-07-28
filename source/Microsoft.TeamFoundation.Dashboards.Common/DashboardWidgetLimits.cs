// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Common.DashboardWidgetLimits
// Assembly: Microsoft.TeamFoundation.Dashboards.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A36A0DA7-561F-410D-8C02-3A33CBE61F1D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Dashboards.Common
{
  [GenerateAllConstants(null)]
  public class DashboardWidgetLimits
  {
    public const int MaxDashboardNameLength = 32;
    public const int MaxDashboardDescriptionLength = 128;
    public const int MaxWidgetNameLength = 256;
    public const int MaxWidgetTypeLength = 1000;
    public const int MaxWidgetArtifactIdLength = 256;
    public const int MaxWidgetSettingsVersionLength = 256;
    public const int MaxWidgetArtifactContextLength = 512;
    public const int MaxWidgetLegacySettingsLength = 4000;
    public const int MaxWidgetSettingsLength = 16000;
    public const int MinRequiredRefreshInterval = 30;
    public const int MaxRowSpan = 10;
    public const int MaxColumnSpan = 10;
  }
}
