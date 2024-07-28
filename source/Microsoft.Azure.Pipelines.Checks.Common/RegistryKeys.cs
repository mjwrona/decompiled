// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.RegistryKeys
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  public static class RegistryKeys
  {
    public static readonly string RegistrySettingsPath = "/Pipelines/Policy/Settings/";
    public static readonly string ApprovalsPath = RegistryKeys.RegistrySettingsPath + "Approvals/";
    public static readonly string QueryApprovalsCount = RegistryKeys.ApprovalsPath + nameof (QueryApprovalsCount);
  }
}
