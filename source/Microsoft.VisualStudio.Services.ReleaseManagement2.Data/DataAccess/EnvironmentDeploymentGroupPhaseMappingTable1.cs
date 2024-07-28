// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.EnvironmentDeploymentGroupPhaseMappingTable1
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class EnvironmentDeploymentGroupPhaseMappingTable1
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[3]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("EnvironmentName", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("DeploymentGroupId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Tags", SqlDbType.NVarChar, -1L)
    };

    public static void BindEnvironmentDeploymentGroupPhaseMappingTable1(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IList<EnvironmentDeploymentGroupPhaseMapping> environmentDeploymentGroupPhaseMappings,
      IDictionary<int, string> environmentIdVsName)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_EnvironmentDeploymentGroupPhaseMappingTableV1", EnvironmentDeploymentGroupPhaseMappingTable1.GetEnvironmentDeploymentGroupPhaseMappingSqlDataRecords((IEnumerable<EnvironmentDeploymentGroupPhaseMapping>) environmentDeploymentGroupPhaseMappings, environmentIdVsName));
    }

    private static IEnumerable<SqlDataRecord> GetEnvironmentDeploymentGroupPhaseMappingSqlDataRecords(
      IEnumerable<EnvironmentDeploymentGroupPhaseMapping> rows,
      IDictionary<int, string> environmentIdVsName)
    {
      rows = rows ?? Enumerable.Empty<EnvironmentDeploymentGroupPhaseMapping>();
      foreach (EnvironmentDeploymentGroupPhaseMapping groupPhaseMapping in rows.Where<EnvironmentDeploymentGroupPhaseMapping>((System.Func<EnvironmentDeploymentGroupPhaseMapping, bool>) (r => r != null)))
      {
        string empty = string.Empty;
        if (environmentIdVsName.TryGetValue(groupPhaseMapping.EnvironmentId, out empty))
        {
          int ordinal = 0;
          SqlDataRecord mappingSqlDataRecord = new SqlDataRecord(EnvironmentDeploymentGroupPhaseMappingTable1.SqlMetaData);
          mappingSqlDataRecord.SetString(ordinal, empty);
          int num1;
          mappingSqlDataRecord.SetInt32(num1 = ordinal + 1, groupPhaseMapping.DeploymentGroupId);
          int num2;
          if (groupPhaseMapping.Tags != null)
            mappingSqlDataRecord.SetString(num2 = num1 + 1, groupPhaseMapping.Tags);
          else
            mappingSqlDataRecord.SetDBNull(num2 = num1 + 1);
          yield return mappingSqlDataRecord;
        }
      }
    }
  }
}
