// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentHealthOptionConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [GenerateAllConstants(null)]
  public static class DeploymentHealthOptionConstants
  {
    public const string AllTargetsInParallel = "AllTargetsInParallel";
    public const string HalfOfTargetsInParallel = "HalfOfTargetsInParallel";
    public const string QuarterOfTargetsInParallel = "QuarterOfTargetsInParallel";
    public const string OneTargetAtATime = "OneTargetAtATime";
    public const string Custom = "Custom";
  }
}
