// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.GateConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [GenerateAllConstants(null)]
  internal class GateConstants
  {
    public const int MinimumSamplingIntervalInMinutes = 5;
    public const int DefaultSamplingIntervalInMinutes = 15;
    public const int MaximumSamplingIntervalInMinutes = 1440;
    public const int MinimumStabilizationTimeInMinutes = 0;
    public const int DefaultStabilizationTimeInMinutes = 5;
    public const int MaximumStabilizationTimeInMinutes = 2880;
    public const int MinimumTimeoutInMinutes = 6;
    public const int DefaultJobTimeoutInMinutes = 1440;
    public const int MaximumTimeoutInMinutes = 21600;
    public const int DefaultMinimumSuccessDurationInMinutes = 0;
    public const int MinimumSuccessDurationMinAllowedValueInMinutes = 0;
    public const int MinimumSuccessDurationMaxAllowedValueInMinutes = 2880;
    public const int MinimumSamplingIntervalInMinutesForLongerTimeout = 30;
    public const int CutoffTimeForLongerGatesTimeout = 4320;
  }
}
