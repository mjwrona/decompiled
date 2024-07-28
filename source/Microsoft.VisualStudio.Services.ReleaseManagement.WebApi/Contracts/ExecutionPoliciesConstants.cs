// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ExecutionPoliciesConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  public static class ExecutionPoliciesConstants
  {
    public const int ConcurrencyInfinite = 0;
    public const int ConcurrencyOnlyOne = 1;
    public const int QueueDepthInfinite = 0;
    public const int QueueDepthLastReleaseOnly = 1;
  }
}
