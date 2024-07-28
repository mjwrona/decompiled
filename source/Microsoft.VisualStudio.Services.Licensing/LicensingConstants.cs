// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingConstants
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicensingConstants
  {
    public const string TraceArea = "Licensing";
    public const string LicensingConfigurationServiceName = "Licensing_Config";
    public const string LicensingPartitionServiceName = "Licensing_Partition";
    public const string ExtensionLicensingServiceName = "ExtensionLicensing";
    public const int MinimumRightUpdateThresholdInHours = 8;
    public const int MaximumRightUpdateThresholdInHours = 12;
    public const int DefaultAccountEntitlementSearchMaxPageSize = 100;
  }
}
