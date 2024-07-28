// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VCoreServiceObjective
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VCoreServiceObjective
  {
    public static readonly int MinStorageSizeInGB = 5;
    private DatabaseServiceEdition m_edition;
    private VCoreGeneration m_generation;
    private int m_cores;
    private DatabaseServiceObjective m_objective;

    public DatabaseServiceEdition Edition => this.m_edition;

    public VCoreGeneration Generation => this.m_generation;

    public int Cores => this.m_cores;

    public DatabaseServiceObjective ServiceObjective => this.m_objective;

    public int IncludedStorageInGB => 5;

    public int MaxStorageInGB
    {
      get
      {
        if (this.Edition == DatabaseServiceEdition.HyperScale)
          return 102400;
        if (this.Generation == VCoreGeneration.M)
          return 4096;
        if (this.Generation == VCoreGeneration.Gen5)
        {
          if (this.Cores <= 4)
            return 1024;
          if (this.Cores <= 6)
            return 1536;
          if (this.Cores <= 10)
            return 2048;
          return this.Cores <= 20 ? 3072 : 4096;
        }
        if (this.Edition != DatabaseServiceEdition.GeneralPurpose || this.Cores <= 3)
          return 1024;
        if (this.Cores <= 7)
          return 1536;
        return this.Cores <= 10 ? 3072 : 4096;
      }
    }

    public VCoreServiceObjective(DatabaseServiceObjective serviceObjective)
    {
      this.m_objective = serviceObjective;
      this.m_edition = AzureDatabaseProperties.GetEdition(serviceObjective);
      if (!this.m_edition.IsVCoreBased())
        throw new Exception(string.Format("ServiceObjective {0} is in Edition {1} which is not VCore!", (object) this.m_objective, (object) this.m_edition));
      string[] strArray = this.m_objective.ToString().Split('_');
      if ((strArray.Length != 3 || !EnumUtility.TryParse<VCoreGeneration>(strArray[1], true, out this.m_generation) || !int.TryParse(strArray[2], out this.m_cores)) && (strArray.Length != 4 || !EnumUtility.TryParse<VCoreGeneration>(strArray[2], true, out this.m_generation) || !int.TryParse(strArray[3], out this.m_cores)))
        throw new Exception(string.Format("ServiceObjective {0} is not a valid VCore Sku!", (object) this.m_objective));
    }

    public VCoreServiceObjective(
      DatabaseServiceEdition edition,
      VCoreGeneration generation,
      int cores)
    {
      this.m_edition = edition;
      this.m_generation = generation;
      this.m_cores = cores;
      this.m_objective = EnumUtility.Parse<DatabaseServiceObjective>(this.EditionString + "_" + generation.ToString() + "_" + cores.ToString());
    }

    public override string ToString() => this.ServiceObjective.ToString();

    private string EditionString
    {
      get
      {
        if (this.Edition == DatabaseServiceEdition.BusinessCritical)
          return "BC";
        return this.Edition != DatabaseServiceEdition.GeneralPurpose ? "HS" : "GP";
      }
    }
  }
}
