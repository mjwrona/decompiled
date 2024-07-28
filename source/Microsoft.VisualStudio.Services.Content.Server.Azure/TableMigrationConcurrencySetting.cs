// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableMigrationConcurrencySetting
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TableMigrationConcurrencySetting : IGroupIndexable
  {
    public int GroupIndex { get; set; }

    public int TotalGroups { get; set; }

    public string OverallPrefix { get; private set; }

    public TableMigrationJobLevelConcurrencyType Type { get; private set; }

    public bool IsDistributed => !string.IsNullOrEmpty(this.OverallPrefix) || this.TotalGroups > 1;

    private TableMigrationConcurrencySetting(
      int groupIndex,
      int totalGroups,
      string overallPrefix,
      TableMigrationJobLevelConcurrencyType type)
    {
      this.GroupIndex = groupIndex >= 0 ? groupIndex : 0;
      this.TotalGroups = totalGroups > 1 ? totalGroups : 1;
      this.OverallPrefix = overallPrefix ?? string.Empty;
      this.Type = type;
    }

    public static TableMigrationConcurrencySetting ByStorageAccountGroup(
      int groupIndex,
      int totalGroups)
    {
      return new TableMigrationConcurrencySetting(groupIndex, totalGroups, string.Empty, TableMigrationJobLevelConcurrencyType.StorageAccount);
    }

    public static TableMigrationConcurrencySetting ByOverallPrefix(string overallPrefix) => new TableMigrationConcurrencySetting(0, 1, overallPrefix, TableMigrationJobLevelConcurrencyType.Prefix);

    public static TableMigrationConcurrencySetting None() => new TableMigrationConcurrencySetting(0, 1, string.Empty, TableMigrationJobLevelConcurrencyType.Prefix);

    public override string ToString()
    {
      switch (this.Type)
      {
        case TableMigrationJobLevelConcurrencyType.Prefix:
          return string.Format("{0}='{1}'", (object) this.Type, (object) this.OverallPrefix);
        case TableMigrationJobLevelConcurrencyType.StorageAccount:
          return string.Format("{0}={1}/{2}", (object) this.Type, (object) this.GroupIndex, (object) this.TotalGroups);
        default:
          throw new InvalidOperationException(string.Format("Unrecognized {0}: {1}", (object) "TableMigrationJobLevelConcurrencyType", (object) this.Type));
      }
    }
  }
}
