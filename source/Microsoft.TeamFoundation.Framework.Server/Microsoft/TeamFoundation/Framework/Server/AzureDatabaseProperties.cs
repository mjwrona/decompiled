// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AzureDatabaseProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AzureDatabaseProperties
  {
    private const int c_CoreCountMinimum = 4;
    private static readonly Dictionary<DatabaseServiceEdition, AzureDatabaseProperties.DatabaseStorageChoices> s_defaultSizesForEdition = new Dictionary<DatabaseServiceEdition, AzureDatabaseProperties.DatabaseStorageChoices>()
    {
      {
        DatabaseServiceEdition.NotAzure,
        new AzureDatabaseProperties.DatabaseStorageChoices(0, Array.Empty<int>())
      },
      {
        DatabaseServiceEdition.Basic,
        new AzureDatabaseProperties.DatabaseStorageChoices(2, Array.Empty<int>())
      },
      {
        DatabaseServiceEdition.Standard,
        new AzureDatabaseProperties.DatabaseStorageChoices(250, new int[3]
        {
          500,
          750,
          1024
        })
      },
      {
        DatabaseServiceEdition.Premium,
        new AzureDatabaseProperties.DatabaseStorageChoices(500, new int[2]
        {
          750,
          1024
        })
      },
      {
        DatabaseServiceEdition.GeneralPurpose,
        new AzureDatabaseProperties.DatabaseStorageChoices(250, new int[3]
        {
          500,
          750,
          1024
        })
      },
      {
        DatabaseServiceEdition.BusinessCritical,
        new AzureDatabaseProperties.DatabaseStorageChoices(250, new int[3]
        {
          500,
          750,
          1024
        })
      }
    };
    private static readonly Dictionary<DatabaseServiceObjective, AzureDatabaseProperties.DatabaseStorageChoices> s_objectiveSpecificSizes = new Dictionary<DatabaseServiceObjective, AzureDatabaseProperties.DatabaseStorageChoices>()
    {
      {
        DatabaseServiceObjective.S0,
        new AzureDatabaseProperties.DatabaseStorageChoices(250, Array.Empty<int>())
      },
      {
        DatabaseServiceObjective.S1,
        new AzureDatabaseProperties.DatabaseStorageChoices(250, Array.Empty<int>())
      },
      {
        DatabaseServiceObjective.S2,
        new AzureDatabaseProperties.DatabaseStorageChoices(250, Array.Empty<int>())
      },
      {
        DatabaseServiceObjective.P11,
        new AzureDatabaseProperties.DatabaseStorageChoices(1024, new int[1]
        {
          4096
        })
      },
      {
        DatabaseServiceObjective.P15,
        new AzureDatabaseProperties.DatabaseStorageChoices(1024, new int[1]
        {
          4096
        })
      }
    };
    private static readonly DatabaseSku[] s_dtuBasedSkus = new DatabaseSku[15]
    {
      new DatabaseSku(DatabaseServiceObjective.S0, 10, 14.76),
      new DatabaseSku(DatabaseServiceObjective.S1, 20, 29.52),
      new DatabaseSku(DatabaseServiceObjective.S2, 50, 73.82),
      new DatabaseSku(DatabaseServiceObjective.S3, 100, 147.59),
      new DatabaseSku(DatabaseServiceObjective.S4, 200, 147.59),
      new DatabaseSku(DatabaseServiceObjective.S6, 400, 295.18),
      new DatabaseSku(DatabaseServiceObjective.S7, 800, 590.36),
      new DatabaseSku(DatabaseServiceObjective.S9, 1600, 1180.72),
      new DatabaseSku(DatabaseServiceObjective.S12, 3000, 2213.85),
      new DatabaseSku(DatabaseServiceObjective.P1, 125, 457.5),
      new DatabaseSku(DatabaseServiceObjective.P2, 250, 915.01),
      new DatabaseSku(DatabaseServiceObjective.P4, 500, 1830.0),
      new DatabaseSku(DatabaseServiceObjective.P6, 1000, 3660.0),
      new DatabaseSku(DatabaseServiceObjective.P11, 1750, 6887.21),
      new DatabaseSku(DatabaseServiceObjective.P15, 4000, 13361.0)
    };
    private static readonly VCoreServiceObjective[] s_vCoreServiceObjectives = new VCoreServiceObjective[120]
    {
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_1),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_2),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_3),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_4),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_5),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_6),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_7),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_8),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_9),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_10),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_16),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen4_24),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_2),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_4),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_6),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_8),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_10),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_12),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_14),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_16),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_18),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_20),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_24),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_32),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_40),
      new VCoreServiceObjective(DatabaseServiceObjective.GP_Gen5_80),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_1),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_2),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_3),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_4),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_5),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_6),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_7),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_8),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_9),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_10),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_16),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen4_24),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_2),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_4),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_6),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_8),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_10),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_12),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_14),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_16),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_18),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_20),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_24),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_32),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_40),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_Gen5_80),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_1),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_2),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_3),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_4),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_5),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_6),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_7),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_8),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_9),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_10),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_16),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen4_24),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_2),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_4),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_6),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_8),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_10),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_12),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_14),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_16),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_18),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_20),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_24),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_32),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_40),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_S_Gen5_80),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_1),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_2),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_3),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_4),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_5),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_6),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_7),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_8),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_9),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_10),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_16),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen4_24),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_2),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_4),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_6),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_8),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_10),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_12),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_14),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_16),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_18),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_20),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_24),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_32),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_40),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_Gen5_80),
      new VCoreServiceObjective(DatabaseServiceObjective.BC_M_128),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_2),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_4),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_6),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_8),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_10),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_12),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_14),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_16),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_18),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_20),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_24),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_32),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_40),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_80),
      new VCoreServiceObjective(DatabaseServiceObjective.HS_PRMS_128)
    };
    private static readonly Dictionary<DatabaseServiceObjective, DatabaseServiceObjective> s_premiumToVCoreMapping = new Dictionary<DatabaseServiceObjective, DatabaseServiceObjective>()
    {
      {
        DatabaseServiceObjective.S2,
        DatabaseServiceObjective.GP_Gen5_2
      },
      {
        DatabaseServiceObjective.S3,
        DatabaseServiceObjective.GP_Gen5_2
      },
      {
        DatabaseServiceObjective.S4,
        DatabaseServiceObjective.GP_Gen5_4
      },
      {
        DatabaseServiceObjective.S6,
        DatabaseServiceObjective.GP_Gen5_6
      },
      {
        DatabaseServiceObjective.S7,
        DatabaseServiceObjective.GP_Gen5_12
      },
      {
        DatabaseServiceObjective.S9,
        DatabaseServiceObjective.GP_Gen5_18
      },
      {
        DatabaseServiceObjective.S12,
        DatabaseServiceObjective.GP_Gen5_24
      },
      {
        DatabaseServiceObjective.P1,
        DatabaseServiceObjective.BC_Gen5_2
      },
      {
        DatabaseServiceObjective.P2,
        DatabaseServiceObjective.BC_Gen5_4
      },
      {
        DatabaseServiceObjective.P4,
        DatabaseServiceObjective.BC_Gen5_6
      },
      {
        DatabaseServiceObjective.P6,
        DatabaseServiceObjective.BC_Gen5_12
      },
      {
        DatabaseServiceObjective.P11,
        DatabaseServiceObjective.BC_Gen5_18
      },
      {
        DatabaseServiceObjective.P15,
        DatabaseServiceObjective.BC_Gen5_40
      }
    };
    private static readonly Dictionary<DatabaseServiceEdition, DatabaseServiceObjective[]> s_defaultDtuServiceObjectiveBounds = new Dictionary<DatabaseServiceEdition, DatabaseServiceObjective[]>()
    {
      {
        DatabaseServiceEdition.Standard,
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.S3,
          DatabaseServiceObjective.S7
        }
      },
      {
        DatabaseServiceEdition.Premium,
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.P1,
          DatabaseServiceObjective.P15
        }
      }
    };
    private static readonly Dictionary<KeyValuePair<DatabaseServiceEdition, VCoreGeneration>, DatabaseServiceObjective[]> s_defaultVcoreServiceObjectiveBounds = new Dictionary<KeyValuePair<DatabaseServiceEdition, VCoreGeneration>, DatabaseServiceObjective[]>()
    {
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.GeneralPurpose, VCoreGeneration.Gen4),
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.GP_Gen4_1,
          DatabaseServiceObjective.GP_Gen4_10
        }
      },
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.GeneralPurpose, VCoreGeneration.Gen5),
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.GP_Gen5_2,
          DatabaseServiceObjective.GP_Gen5_18
        }
      },
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.HyperScale, VCoreGeneration.Gen4),
        new DatabaseServiceObjective[4]
        {
          DatabaseServiceObjective.HS_Gen4_1,
          DatabaseServiceObjective.HS_Gen4_24,
          DatabaseServiceObjective.HS_S_Gen4_1,
          DatabaseServiceObjective.HS_S_Gen4_24
        }
      },
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.HyperScale, VCoreGeneration.Gen5),
        new DatabaseServiceObjective[4]
        {
          DatabaseServiceObjective.HS_Gen5_2,
          DatabaseServiceObjective.HS_Gen5_40,
          DatabaseServiceObjective.HS_S_Gen5_2,
          DatabaseServiceObjective.HS_S_Gen5_40
        }
      },
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.HyperScale, VCoreGeneration.PRMS),
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.HS_PRMS_2,
          DatabaseServiceObjective.HS_PRMS_40
        }
      },
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.BusinessCritical, VCoreGeneration.Gen4),
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.BC_Gen4_1,
          DatabaseServiceObjective.BC_Gen4_24
        }
      },
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.BusinessCritical, VCoreGeneration.Gen5),
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.BC_Gen5_2,
          DatabaseServiceObjective.BC_Gen5_80
        }
      },
      {
        new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(DatabaseServiceEdition.BusinessCritical, VCoreGeneration.M),
        new DatabaseServiceObjective[2]
        {
          DatabaseServiceObjective.BC_M_128,
          DatabaseServiceObjective.BC_M_128
        }
      }
    };

    public AzureDatabaseProperties(DatabaseServiceObjective objective)
    {
      this.Objective = objective;
      this.Edition = AzureDatabaseProperties.GetEdition(objective);
      if (this.Edition.IsVCoreBased())
      {
        VCoreServiceObjective serviceObjective = new VCoreServiceObjective(objective);
        this.CurrentMaxSizeInMB = serviceObjective.IncludedStorageInGB * 1024;
        this.IncludedStorageInGB = serviceObjective.IncludedStorageInGB;
        this.MaxStoragePossibleInGB = serviceObjective.MaxStorageInGB;
      }
      else
      {
        AzureDatabaseProperties.DatabaseStorageChoices supportedStorageChoices = this.GetSupportedStorageChoices();
        this.CurrentMaxSizeInMB = supportedStorageChoices.IncludedStorageInGB * 1024;
        this.IncludedStorageInGB = supportedStorageChoices.IncludedStorageInGB;
        this.MaxStoragePossibleInGB = supportedStorageChoices.ExtraSizesInGB.Length != 0 ? ((IEnumerable<int>) supportedStorageChoices.ExtraSizesInGB).Max() : supportedStorageChoices.IncludedStorageInGB;
      }
    }

    public AzureDatabaseProperties(string objectiveString)
      : this(AzureDatabaseProperties.GetServiceObjectiveFromString(objectiveString))
    {
    }

    public DatabaseServiceObjective Objective { get; set; }

    public DatabaseServiceEdition Edition { get; set; }

    public int CurrentMaxSizeInMB { get; set; }

    public int SizeInMB { get; set; }

    public int IncludedStorageInGB { get; }

    public int MaxStoragePossibleInGB { get; }

    public override string ToString() => string.Format("Objective: {0}, Edition: {1}, MaxSizeInMB: {2}, SizeInMB: {3}", (object) this.Objective, (object) this.Edition, (object) this.CurrentMaxSizeInMB, (object) this.SizeInMB);

    public bool DoesServiceObjectiveSupportSize(int sizeInGB)
    {
      if (this.Edition.IsVCoreBased())
        return sizeInGB >= VCoreServiceObjective.MinStorageSizeInGB && sizeInGB <= this.MaxStoragePossibleInGB;
      AzureDatabaseProperties.DatabaseStorageChoices supportedStorageChoices = this.GetSupportedStorageChoices();
      if (sizeInGB <= supportedStorageChoices.IncludedStorageInGB)
        return true;
      for (int index = 0; index < supportedStorageChoices.ExtraSizesInGB.Length; ++index)
      {
        if (sizeInGB == supportedStorageChoices.ExtraSizesInGB[index])
          return true;
      }
      return false;
    }

    public bool DoesDatabaseHaveExtraSpace() => this.CurrentMaxSizeInMB > this.IncludedStorageInGB * 1024;

    public (DatabaseServiceObjective, SloAction) GetServiceObjectiveRecommendation(
      SloAction action,
      DatabaseServiceObjective minServiceObjective,
      DatabaseServiceObjective maxServiceObjective)
    {
      SloAction sloAction = action;
      if (this.Edition.IsDtuBased())
      {
        DatabaseSku currentSku = ((IEnumerable<DatabaseSku>) AzureDatabaseProperties.s_dtuBasedSkus).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.Objective == this.Objective)).FirstOrDefault<DatabaseSku>();
        DatabaseSku[] array = ((IEnumerable<DatabaseSku>) AzureDatabaseProperties.s_dtuBasedSkus).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => new AzureDatabaseProperties(s.Objective).Edition == this.Edition)).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.Objective >= minServiceObjective)).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.Objective <= maxServiceObjective)).ToArray<DatabaseSku>();
        if (currentSku != null)
        {
          DatabaseSku databaseSku;
          switch (action)
          {
            case SloAction.Upgrade:
              databaseSku = ((IEnumerable<DatabaseSku>) array).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.DTU > currentSku.DTU)).OrderBy<DatabaseSku, double>((Func<DatabaseSku, double>) (s => s.PricePerMonth)).FirstOrDefault<DatabaseSku>() ?? currentSku;
              break;
            case SloAction.Downgrade:
              databaseSku = ((IEnumerable<DatabaseSku>) array).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.DTU < currentSku.DTU)).OrderByDescending<DatabaseSku, int>((Func<DatabaseSku, int>) (s => s.DTU)).FirstOrDefault<DatabaseSku>() ?? currentSku;
              break;
            default:
              databaseSku = currentSku;
              break;
          }
          if (databaseSku == currentSku)
            sloAction = SloAction.Keep;
          return (databaseSku.Objective, sloAction);
        }
      }
      else if (this.Edition.IsVCoreBased())
      {
        VCoreServiceObjective vCoreServiceObjective = new VCoreServiceObjective(this.Objective);
        VCoreServiceObjective[] array = ((IEnumerable<VCoreServiceObjective>) AzureDatabaseProperties.s_vCoreServiceObjectives).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.Edition == vCoreServiceObjective.Edition)).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.Generation == vCoreServiceObjective.Generation)).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.ServiceObjective >= minServiceObjective)).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.ServiceObjective <= maxServiceObjective)).ToArray<VCoreServiceObjective>();
        VCoreServiceObjective serviceObjective;
        switch (action)
        {
          case SloAction.Upgrade:
            int targetCores = vCoreServiceObjective.Cores * 2;
            if (targetCores > 40 && vCoreServiceObjective.Cores < 40)
              targetCores = 40;
            serviceObjective = ((IEnumerable<VCoreServiceObjective>) array).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.Cores >= targetCores)).OrderBy<VCoreServiceObjective, int>((Func<VCoreServiceObjective, int>) (o => o.Cores)).FirstOrDefault<VCoreServiceObjective>() ?? ((IEnumerable<VCoreServiceObjective>) array).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.Cores > vCoreServiceObjective.Cores)).OrderByDescending<VCoreServiceObjective, int>((Func<VCoreServiceObjective, int>) (o => o.Cores)).FirstOrDefault<VCoreServiceObjective>() ?? vCoreServiceObjective;
            break;
          case SloAction.Downgrade:
            serviceObjective = ((IEnumerable<VCoreServiceObjective>) array).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.Cores < vCoreServiceObjective.Cores)).Where<VCoreServiceObjective>((Func<VCoreServiceObjective, bool>) (o => o.Cores >= 4)).OrderByDescending<VCoreServiceObjective, int>((Func<VCoreServiceObjective, int>) (o => o.Cores)).FirstOrDefault<VCoreServiceObjective>() ?? vCoreServiceObjective;
            break;
          default:
            serviceObjective = vCoreServiceObjective;
            break;
        }
        if (serviceObjective.ServiceObjective == this.Objective)
          sloAction = SloAction.Keep;
        return (serviceObjective.ServiceObjective, sloAction);
      }
      return (this.Objective, SloAction.Keep);
    }

    public DatabaseServiceObjective GetServiceObjectivePremiumPeer(
      DatabaseServiceObjective maxServiceObjective,
      out SloAction actionToTake)
    {
      DatabaseServiceObjective objective = this.Objective;
      if (this.Edition == DatabaseServiceEdition.Standard)
      {
        DatabaseSku currentSku = ((IEnumerable<DatabaseSku>) AzureDatabaseProperties.s_dtuBasedSkus).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.Objective == this.Objective)).FirstOrDefault<DatabaseSku>();
        if (currentSku != null)
        {
          DatabaseSku databaseSku = ((IEnumerable<DatabaseSku>) AzureDatabaseProperties.s_dtuBasedSkus).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => new AzureDatabaseProperties(s.Objective).Edition == DatabaseServiceEdition.Premium)).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.DTU >= currentSku.DTU)).Where<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.Objective <= maxServiceObjective)).OrderBy<DatabaseSku, double>((Func<DatabaseSku, double>) (s => s.PricePerMonth)).FirstOrDefault<DatabaseSku>();
          if (databaseSku != null)
          {
            actionToTake = SloAction.Upgrade;
            objective = databaseSku.Objective;
          }
        }
      }
      actionToTake = SloAction.Keep;
      return objective;
    }

    public DatabaseServiceObjectiveuCompareResult Compare(
      DatabaseServiceObjective objectiveToCompare)
    {
      DatabaseServiceObjectiveuCompareResult objectiveuCompareResult = DatabaseServiceObjectiveuCompareResult.NotComparable;
      AzureDatabaseProperties databaseProperties = new AzureDatabaseProperties(objectiveToCompare);
      if (this.Objective == objectiveToCompare)
        objectiveuCompareResult = DatabaseServiceObjectiveuCompareResult.Equal;
      else if (this.Edition == databaseProperties.Edition)
        objectiveuCompareResult = this.Objective > objectiveToCompare ? DatabaseServiceObjectiveuCompareResult.GreaterThan : DatabaseServiceObjectiveuCompareResult.LessThan;
      else if (this.Edition.IsDtuBased() && databaseProperties.Edition.IsDtuBased())
      {
        DatabaseSku databaseSku1 = ((IEnumerable<DatabaseSku>) AzureDatabaseProperties.s_dtuBasedSkus).FirstOrDefault<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.Objective == this.Objective));
        DatabaseSku databaseSku2 = ((IEnumerable<DatabaseSku>) AzureDatabaseProperties.s_dtuBasedSkus).FirstOrDefault<DatabaseSku>((Func<DatabaseSku, bool>) (s => s.Objective == objectiveToCompare));
        if (databaseSku1 == null || databaseSku2 == null)
          objectiveuCompareResult = DatabaseServiceObjectiveuCompareResult.NotComparable;
        else if (this.Objective > objectiveToCompare && databaseSku1.DTU > databaseSku2.DTU)
          objectiveuCompareResult = DatabaseServiceObjectiveuCompareResult.GreaterThan;
        else if (this.Objective < objectiveToCompare && databaseSku1.DTU < databaseSku2.DTU)
          objectiveuCompareResult = DatabaseServiceObjectiveuCompareResult.LessThan;
      }
      return objectiveuCompareResult;
    }

    internal bool CanGrowDatabase(out int newSizeGB)
    {
      bool flag = false;
      newSizeGB = 0;
      int num = this.CurrentMaxSizeInMB / 1024;
      if (this.Edition.IsVCoreBased())
      {
        if (this.MaxStoragePossibleInGB > num)
        {
          newSizeGB = Math.Min(Math.Max((int) ((double) num * 1.5), num + 10), this.MaxStoragePossibleInGB);
          flag = true;
        }
      }
      else
      {
        if (num < this.IncludedStorageInGB)
        {
          newSizeGB = this.IncludedStorageInGB;
          return true;
        }
        AzureDatabaseProperties.DatabaseStorageChoices supportedStorageChoices = this.GetSupportedStorageChoices();
        for (int index = 0; index < supportedStorageChoices.ExtraSizesInGB.Length; ++index)
        {
          if (supportedStorageChoices.ExtraSizesInGB[index] > num)
          {
            newSizeGB = supportedStorageChoices.ExtraSizesInGB[index];
            return true;
          }
        }
      }
      return flag;
    }

    public static DatabaseServiceEdition GetEdition(DatabaseServiceObjective objective)
    {
      switch (objective)
      {
        case DatabaseServiceObjective.NotAzure:
          return DatabaseServiceEdition.NotAzure;
        case DatabaseServiceObjective.Basic:
          return DatabaseServiceEdition.Basic;
        case DatabaseServiceObjective.S0:
        case DatabaseServiceObjective.S1:
        case DatabaseServiceObjective.S2:
        case DatabaseServiceObjective.S3:
        case DatabaseServiceObjective.S4:
        case DatabaseServiceObjective.S6:
        case DatabaseServiceObjective.S7:
        case DatabaseServiceObjective.S9:
        case DatabaseServiceObjective.S12:
          return DatabaseServiceEdition.Standard;
        case DatabaseServiceObjective.P1:
        case DatabaseServiceObjective.P2:
        case DatabaseServiceObjective.P4:
        case DatabaseServiceObjective.P6:
        case DatabaseServiceObjective.P11:
        case DatabaseServiceObjective.P15:
          return DatabaseServiceEdition.Premium;
        default:
          if (objective.ToString().StartsWith("GP"))
            return DatabaseServiceEdition.GeneralPurpose;
          if (objective.ToString().StartsWith("BC"))
            return DatabaseServiceEdition.BusinessCritical;
          if (objective.ToString().StartsWith("HS"))
            return DatabaseServiceEdition.HyperScale;
          throw new NotSupportedException(string.Format("The database service objective {0} is not supported!", (object) objective));
      }
    }

    public static void GetDefaultServiceObjectiveBounds(
      DatabaseServiceObjective targeServiceObjective,
      out DatabaseServiceObjective defaultMinSlo,
      out DatabaseServiceObjective defaultMaxSlo)
    {
      if (targeServiceObjective.GetEdition().IsDtuBased())
      {
        defaultMinSlo = AzureDatabaseProperties.s_defaultDtuServiceObjectiveBounds[targeServiceObjective.GetEdition()][0];
        defaultMaxSlo = AzureDatabaseProperties.s_defaultDtuServiceObjectiveBounds[targeServiceObjective.GetEdition()][1];
      }
      else if (targeServiceObjective.ToString().Contains("HS_S"))
      {
        VCoreServiceObjective serviceObjective = new VCoreServiceObjective(targeServiceObjective);
        defaultMinSlo = AzureDatabaseProperties.s_defaultVcoreServiceObjectiveBounds[new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(serviceObjective.Edition, serviceObjective.Generation)][2];
        defaultMaxSlo = AzureDatabaseProperties.s_defaultVcoreServiceObjectiveBounds[new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(serviceObjective.Edition, serviceObjective.Generation)][3];
      }
      else
      {
        VCoreServiceObjective serviceObjective = new VCoreServiceObjective(targeServiceObjective);
        defaultMinSlo = AzureDatabaseProperties.s_defaultVcoreServiceObjectiveBounds[new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(serviceObjective.Edition, serviceObjective.Generation)][0];
        defaultMaxSlo = AzureDatabaseProperties.s_defaultVcoreServiceObjectiveBounds[new KeyValuePair<DatabaseServiceEdition, VCoreGeneration>(serviceObjective.Edition, serviceObjective.Generation)][1];
      }
    }

    public static bool GetDatabaseServiceObjectiveBounds(
      DatabaseServiceObjective targeServiceObjective,
      ITeamFoundationDatabaseProperties database,
      out DatabaseServiceObjective lowerBound,
      out DatabaseServiceObjective upperBound)
    {
      bool serviceObjectiveBounds = false;
      DatabaseServiceObjective defaultMinSlo;
      DatabaseServiceObjective defaultMaxSlo;
      AzureDatabaseProperties.GetDefaultServiceObjectiveBounds(targeServiceObjective, out defaultMinSlo, out defaultMaxSlo);
      if (!string.IsNullOrEmpty(database.MinServiceObjective))
      {
        lowerBound = AzureDatabaseProperties.GetServiceObjectiveFromString(database.MinServiceObjective);
        if (AzureDatabaseProperties.EditionOrGenerationDoesnotMatch(lowerBound, targeServiceObjective))
        {
          lowerBound = defaultMinSlo;
          serviceObjectiveBounds = true;
        }
      }
      else
      {
        lowerBound = defaultMinSlo;
        serviceObjectiveBounds = true;
      }
      if (!string.IsNullOrEmpty(database.MaxServiceObjective))
      {
        upperBound = AzureDatabaseProperties.GetServiceObjectiveFromString(database.MaxServiceObjective);
        if (AzureDatabaseProperties.EditionOrGenerationDoesnotMatch(upperBound, targeServiceObjective))
        {
          upperBound = defaultMaxSlo;
          serviceObjectiveBounds = true;
        }
        else if (targeServiceObjective > upperBound)
        {
          upperBound = targeServiceObjective;
          serviceObjectiveBounds = true;
        }
      }
      else
      {
        upperBound = defaultMaxSlo;
        serviceObjectiveBounds = true;
      }
      return serviceObjectiveBounds;
    }

    private static bool EditionOrGenerationDoesnotMatch(
      DatabaseServiceObjective o1,
      DatabaseServiceObjective o2)
    {
      if (o1.GetEdition() != o2.GetEdition())
        return true;
      return o1.GetEdition().IsVCoreBased() && new VCoreServiceObjective(o1).Generation != new VCoreServiceObjective(o2).Generation;
    }

    private AzureDatabaseProperties.DatabaseStorageChoices GetSupportedStorageChoices()
    {
      AzureDatabaseProperties.DatabaseStorageChoices supportedStorageChoices;
      if (!AzureDatabaseProperties.s_objectiveSpecificSizes.TryGetValue(this.Objective, out supportedStorageChoices))
        supportedStorageChoices = AzureDatabaseProperties.s_defaultSizesForEdition[this.Edition];
      return supportedStorageChoices;
    }

    public static DatabaseServiceObjective GetServiceObjectiveFromString(string serviceObjective)
    {
      DatabaseServiceObjective result;
      if (EnumUtility.TryParse<DatabaseServiceObjective>(serviceObjective, true, out result))
        return result;
      throw new NotSupportedException("The database service objective " + serviceObjective + " is not supported!");
    }

    private class DatabaseStorageChoices
    {
      public DatabaseStorageChoices(int includedStorageInGB, params int[] extraSizesInGB)
      {
        this.IncludedStorageInGB = includedStorageInGB;
        this.ExtraSizesInGB = extraSizesInGB;
      }

      public int IncludedStorageInGB { get; private set; }

      public int[] ExtraSizesInGB { get; private set; }
    }
  }
}
