// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ActionPriorityConstants
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class ActionPriorityConstants
  {
    public const int ManifestExcludePriority = 0;
    public const int ManifestExcludePropertyPriority = 1;
    public const int CustomOptOutPriority = 2;
    public const int ManifestOptOutPriority = 3;
    public const int ManifestThrottlingPriority = 4;
    public const int ManifestHashedPriority = 5;
    public const int ManifestPiiPriority = 6;
    public const int ManifestCredScanPriority = 7;
    public const int MetricPriority = 50;
    public const int SettingPriority = 60;
    public const int CredScanPriority = 90;
    public const int PiiPriority = 100;
    public const int EnforceAIRestrictionPriority = 200;
    public const int ComplexPropertyPriority = 250;
    public const int SuppressEmptyPostPropertyPriority = 300;
    public const int ThrottlingPriority = 1000;
    public const int ManifestRoutePriority = 10000;
  }
}
