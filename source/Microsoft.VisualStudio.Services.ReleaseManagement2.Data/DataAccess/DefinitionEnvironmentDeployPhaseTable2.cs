// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DefinitionEnvironmentDeployPhaseTable2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class DefinitionEnvironmentDeployPhaseTable2
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[8]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("PhaseType", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("DeploymentInput", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData3 = new Microsoft.SqlServer.Server.SqlMetaData[9]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("PhaseType", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("DeploymentInput", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier)
    };

    public static void BindDefinitionEnvironmentDeployPhaseTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DeployPhase> deployPhases)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentDeployPhaseTable2", DefinitionEnvironmentDeployPhaseTable2.GetSqlRecords(deployPhases));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlRecords(IEnumerable<DeployPhase> rows)
    {
      rows = rows ?? Enumerable.Empty<DeployPhase>();
      foreach (DeployPhase deployPhase in rows.Where<DeployPhase>((System.Func<DeployPhase, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentDeployPhaseTable2.SqlMetaData);
        record.SetInt32(ordinal, deployPhase.ReleaseDefinitionId);
        int num1;
        record.SetInt32(num1 = ordinal + 1, deployPhase.DefinitionEnvironmentId);
        int num2;
        record.SetGuid(num2 = num1 + 1, deployPhase.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, deployPhase.Rank);
        int num4;
        record.SetString(num4 = num3 + 1, deployPhase.Name);
        int num5;
        record.SetInt32(num5 = num4 + 1, (int) deployPhase.PhaseType);
        int num6;
        record.SetString(num6 = num5 + 1, deployPhase.Workflow);
        string str = deployPhase.DeploymentInput != null ? deployPhase.DeploymentInput.ToString(Formatting.None) : (string) null;
        int num7;
        record.SetNullableString(num7 = num6 + 1, str);
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentDeployPhaseTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DeployPhase> deployPhases)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentDeployPhaseTable3", DefinitionEnvironmentDeployPhaseTable2.GetSqlRecords3(deployPhases));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlRecords3(IEnumerable<DeployPhase> rows)
    {
      rows = rows ?? Enumerable.Empty<DeployPhase>();
      foreach (DeployPhase deployPhase in rows.Where<DeployPhase>((System.Func<DeployPhase, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentDeployPhaseTable2.SqlMetaData3);
        record.SetInt32(ordinal, deployPhase.ReleaseDefinitionId);
        int num1;
        record.SetInt32(num1 = ordinal + 1, deployPhase.DefinitionEnvironmentId);
        int num2;
        record.SetGuid(num2 = num1 + 1, deployPhase.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, deployPhase.Rank);
        int num4;
        record.SetString(num4 = num3 + 1, deployPhase.Name);
        int num5;
        record.SetInt32(num5 = num4 + 1, (int) deployPhase.PhaseType);
        int num6;
        record.SetString(num6 = num5 + 1, deployPhase.Workflow);
        string str = deployPhase.DeploymentInput != null ? deployPhase.DeploymentInput.ToString(Formatting.None) : (string) null;
        int num7;
        record.SetNullableString(num7 = num6 + 1, str);
        int num8;
        record.SetNullableGuid(num8 = num7 + 1, new Guid?());
        yield return record;
      }
    }
  }
}
