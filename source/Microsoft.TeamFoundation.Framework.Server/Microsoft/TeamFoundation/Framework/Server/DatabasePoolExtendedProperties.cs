// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePoolExtendedProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePoolExtendedProperties
  {
    public DatabasePoolExtendedProperties(
      string poolName,
      bool isExternal,
      bool isStaging,
      bool isImport,
      bool isExport,
      bool canUpgrade,
      bool canFailover,
      bool canUseAvailabilityZone,
      bool canRightSize,
      string dbPartitionSplitDataspaceCategory = "")
    {
      this.PoolName = poolName;
      this.IsExternal = isExternal;
      this.IsStaging = isStaging;
      this.IsImport = isImport;
      this.IsExport = isExport;
      this.CanUpgrade = canUpgrade;
      this.CanFailover = canFailover;
      this.CanUseAvailabilityZone = canUseAvailabilityZone;
      this.CanRightSize = canRightSize;
      this.DbPartitionSplitDataspaceCategory = dbPartitionSplitDataspaceCategory;
    }

    public static DatabasePoolExtendedProperties Default => new DatabasePoolExtendedProperties("", false, false, false, false, false, false, false, false);

    public string PoolName { get; }

    public string DbPartitionSplitDataspaceCategory { get; }

    public bool IsExternal { get; }

    public bool IsStaging { get; }

    public bool IsImport { get; }

    public bool IsExport { get; }

    public bool CanUpgrade { get; }

    public bool CanFailover { get; }

    public bool CanUseAvailabilityZone { get; }

    public bool CanRightSize { get; }
  }
}
