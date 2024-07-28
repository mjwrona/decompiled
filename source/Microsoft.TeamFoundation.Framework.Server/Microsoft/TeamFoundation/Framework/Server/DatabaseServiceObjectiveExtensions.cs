// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServiceObjectiveExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DatabaseServiceObjectiveExtensions
  {
    private static readonly ConcurrentDictionary<DatabaseServiceObjective, int> s_maxConcurrentRequestsDictionary = new ConcurrentDictionary<DatabaseServiceObjective, int>(1, 32);

    public static DatabaseServiceEdition GetEdition(this DatabaseServiceObjective serviceObjective) => AzureDatabaseProperties.GetEdition(serviceObjective);

    public static int GetMaxConcurrentRequests(this DatabaseServiceObjective serviceObjective)
    {
      int concurrentRequestsImpl;
      if (!DatabaseServiceObjectiveExtensions.s_maxConcurrentRequestsDictionary.TryGetValue(serviceObjective, out concurrentRequestsImpl))
      {
        concurrentRequestsImpl = serviceObjective.GetMaxConcurrentRequestsImpl();
        DatabaseServiceObjectiveExtensions.s_maxConcurrentRequestsDictionary.TryAdd(serviceObjective, concurrentRequestsImpl);
      }
      return concurrentRequestsImpl;
    }

    private static int GetMaxConcurrentRequestsImpl(this DatabaseServiceObjective serviceObjective)
    {
      switch (serviceObjective)
      {
        case DatabaseServiceObjective.Basic:
          return 30;
        case DatabaseServiceObjective.S0:
          return 60;
        case DatabaseServiceObjective.S1:
          return 90;
        case DatabaseServiceObjective.S2:
          return 120;
        case DatabaseServiceObjective.S3:
          return 200;
        case DatabaseServiceObjective.S4:
          return 400;
        case DatabaseServiceObjective.S6:
          return 800;
        case DatabaseServiceObjective.S7:
          return 1600;
        case DatabaseServiceObjective.S9:
          return 3200;
        case DatabaseServiceObjective.S12:
          return 6000;
        case DatabaseServiceObjective.P1:
          return 200;
        case DatabaseServiceObjective.P2:
          return 400;
        case DatabaseServiceObjective.P4:
          return 800;
        case DatabaseServiceObjective.P6:
          return 1600;
        case DatabaseServiceObjective.P11:
          return 2400;
        case DatabaseServiceObjective.P15:
          return 6400;
        default:
          if (!AzureDatabaseProperties.GetEdition(serviceObjective).IsVCoreBased())
            return 500;
          VCoreServiceObjective serviceObjective1 = new VCoreServiceObjective(serviceObjective);
          return serviceObjective1.Generation == VCoreGeneration.Gen4 ? 200 * serviceObjective1.Cores : 100 * serviceObjective1.Cores;
      }
    }

    public static DatabaseServiceObjective ConvertToVCoreBasedServiceObjective(
      this DatabaseServiceObjective serviceObjective,
      VCoreGeneration generation)
    {
      if (generation == VCoreGeneration.Gen5)
      {
        switch (serviceObjective)
        {
          case DatabaseServiceObjective.S0:
          case DatabaseServiceObjective.S1:
          case DatabaseServiceObjective.S2:
          case DatabaseServiceObjective.S3:
            return DatabaseServiceObjective.GP_Gen5_2;
          case DatabaseServiceObjective.S4:
            return DatabaseServiceObjective.GP_Gen5_4;
          case DatabaseServiceObjective.S6:
            return DatabaseServiceObjective.GP_Gen5_6;
          case DatabaseServiceObjective.S7:
            return DatabaseServiceObjective.GP_Gen5_12;
          case DatabaseServiceObjective.S9:
            return DatabaseServiceObjective.GP_Gen5_18;
          case DatabaseServiceObjective.S12:
            return DatabaseServiceObjective.GP_Gen5_32;
          case DatabaseServiceObjective.P1:
            return DatabaseServiceObjective.BC_Gen5_2;
          case DatabaseServiceObjective.P2:
            return DatabaseServiceObjective.BC_Gen5_4;
          case DatabaseServiceObjective.P4:
            return DatabaseServiceObjective.BC_Gen5_6;
          case DatabaseServiceObjective.P6:
            return DatabaseServiceObjective.BC_Gen5_12;
          case DatabaseServiceObjective.P11:
            return DatabaseServiceObjective.BC_Gen5_18;
          case DatabaseServiceObjective.P15:
            return DatabaseServiceObjective.BC_Gen5_40;
          default:
            return serviceObjective;
        }
      }
      else
      {
        switch (serviceObjective)
        {
          case DatabaseServiceObjective.S0:
          case DatabaseServiceObjective.S1:
          case DatabaseServiceObjective.S2:
          case DatabaseServiceObjective.S3:
            return DatabaseServiceObjective.GP_Gen4_1;
          case DatabaseServiceObjective.S4:
            return DatabaseServiceObjective.GP_Gen4_2;
          case DatabaseServiceObjective.S6:
            return DatabaseServiceObjective.GP_Gen4_3;
          case DatabaseServiceObjective.S7:
            return DatabaseServiceObjective.GP_Gen4_6;
          case DatabaseServiceObjective.S9:
            return DatabaseServiceObjective.GP_Gen4_9;
          case DatabaseServiceObjective.S12:
            return DatabaseServiceObjective.GP_Gen4_16;
          case DatabaseServiceObjective.P1:
            return DatabaseServiceObjective.BC_Gen4_1;
          case DatabaseServiceObjective.P2:
            return DatabaseServiceObjective.BC_Gen4_2;
          case DatabaseServiceObjective.P4:
            return DatabaseServiceObjective.BC_Gen4_3;
          case DatabaseServiceObjective.P6:
            return DatabaseServiceObjective.BC_Gen4_6;
          case DatabaseServiceObjective.P11:
            return DatabaseServiceObjective.BC_Gen4_9;
          case DatabaseServiceObjective.P15:
            return DatabaseServiceObjective.BC_Gen4_24;
          default:
            return serviceObjective;
        }
      }
    }

    public static DatabaseServiceObjective ConvertToPremiumServiceObjective(
      this DatabaseServiceObjective serviceObjective)
    {
      switch (serviceObjective)
      {
        case DatabaseServiceObjective.S0:
        case DatabaseServiceObjective.S1:
        case DatabaseServiceObjective.S2:
        case DatabaseServiceObjective.S3:
          return DatabaseServiceObjective.P1;
        case DatabaseServiceObjective.S4:
          return DatabaseServiceObjective.P2;
        case DatabaseServiceObjective.S6:
          return DatabaseServiceObjective.P4;
        case DatabaseServiceObjective.S7:
          return DatabaseServiceObjective.P6;
        case DatabaseServiceObjective.S9:
          return DatabaseServiceObjective.P11;
        case DatabaseServiceObjective.S12:
          return DatabaseServiceObjective.P15;
        default:
          return serviceObjective;
      }
    }

    public static DatabaseServiceObjective ConvertGeneralPurposeToBusinessCritical(
      this DatabaseServiceObjective serviceObjective)
    {
      if (AzureDatabaseProperties.GetEdition(serviceObjective) != DatabaseServiceEdition.GeneralPurpose)
        return serviceObjective;
      VCoreServiceObjective serviceObjective1 = new VCoreServiceObjective(serviceObjective);
      return new VCoreServiceObjective(DatabaseServiceEdition.BusinessCritical, serviceObjective1.Generation, serviceObjective1.Cores).ServiceObjective;
    }

    public static int GetNumberOfCores(this DatabaseServiceObjective serviceObjective)
    {
      if (serviceObjective < DatabaseServiceObjective.GP_Gen4_1)
        return 0;
      int result;
      return int.TryParse(((IEnumerable<string>) serviceObjective.ToString().Split('_')).Last<string>(), out result) ? result : -1;
    }
  }
}
