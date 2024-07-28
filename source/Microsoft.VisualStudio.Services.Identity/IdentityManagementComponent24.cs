// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent24
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent24 : IdentityManagementComponent23
  {
    public override int UpgradeIdentitiesToTargetResourceVersion(
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      this.TraceEnter(47011200, nameof (UpgradeIdentitiesToTargetResourceVersion));
      try
      {
        this.PrepareStoredProcedure("prc_UpgradeIdentitiesToTargetResourceVersion");
        this.BindInt("@targetResourceVersion ", targetResourceVersion);
        this.BindInt("@updateBatchSize", updateBatchSize);
        this.BindInt("@maxNumberOfIdentitiesToUpdate", maxNumberOfIdentitiesToUpdate);
        return (int) this.ExecuteNonQuery(true);
      }
      finally
      {
        this.TraceLeave(47011209, nameof (UpgradeIdentitiesToTargetResourceVersion));
      }
    }

    public override int DowngradeIdentitiesToTargetResourceVersion(
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      this.TraceEnter(47011200, nameof (DowngradeIdentitiesToTargetResourceVersion));
      try
      {
        this.PrepareStoredProcedure("prc_DowngradeIdentitiesToTargetResourceVersion");
        this.BindInt("@targetResourceVersion ", targetResourceVersion);
        this.BindInt("@updateBatchSize", updateBatchSize);
        this.BindInt("@maxNumberOfIdentitiesToUpdate", maxNumberOfIdentitiesToUpdate);
        return (int) this.ExecuteNonQuery(true);
      }
      finally
      {
        this.TraceLeave(47011209, nameof (DowngradeIdentitiesToTargetResourceVersion));
      }
    }
  }
}
