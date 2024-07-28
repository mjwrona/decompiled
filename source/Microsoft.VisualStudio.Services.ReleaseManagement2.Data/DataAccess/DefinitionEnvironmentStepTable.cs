// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DefinitionEnvironmentStepTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class DefinitionEnvironmentStepTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[8]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ApproverId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("IsAutomated", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("IsNotificationOn", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("StepType", SqlDbType.TinyInt)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData2 = new Microsoft.SqlServer.Server.SqlMetaData[9]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ApproverId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("IsAutomated", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("IsNotificationOn", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("StepType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier)
    };

    public static void BindDefinitionEnvironmentStepTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironmentStep> definitionEnvironmentSteps)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentStepTable", DefinitionEnvironmentStepTable.SetRecord(definitionEnvironmentSteps));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> SetRecord(IEnumerable<DefinitionEnvironmentStep> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironmentStep>();
      foreach (DefinitionEnvironmentStep definitionEnvironmentStep in rows.Where<DefinitionEnvironmentStep>((System.Func<DefinitionEnvironmentStep, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DefinitionEnvironmentStepTable.SqlMetaData);
        if (definitionEnvironmentStep.ApproverId != Guid.Empty)
          sqlDataRecord.SetGuid(ordinal, definitionEnvironmentStep.ApproverId);
        else
          sqlDataRecord.SetDBNull(ordinal);
        int num1;
        sqlDataRecord.SetInt32(num1 = ordinal + 1, definitionEnvironmentStep.DefinitionEnvironmentId);
        int num2;
        sqlDataRecord.SetGuid(num2 = num1 + 1, definitionEnvironmentStep.GuidId);
        int num3;
        sqlDataRecord.SetInt32(num3 = num2 + 1, definitionEnvironmentStep.Id);
        int num4;
        sqlDataRecord.SetBoolean(num4 = num3 + 1, definitionEnvironmentStep.IsAutomated);
        int num5;
        sqlDataRecord.SetBoolean(num5 = num4 + 1, definitionEnvironmentStep.IsNotificationOn);
        int num6;
        sqlDataRecord.SetInt32(num6 = num5 + 1, definitionEnvironmentStep.Rank);
        int num7;
        sqlDataRecord.SetByte(num7 = num6 + 1, (byte) definitionEnvironmentStep.StepType);
        yield return sqlDataRecord;
      }
    }

    public static void BindDefinitionEnvironmentStepTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironmentStep> definitionEnvironmentSteps)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentStepTable2", DefinitionEnvironmentStepTable.SetRecord2(definitionEnvironmentSteps));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> SetRecord2(IEnumerable<DefinitionEnvironmentStep> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironmentStep>();
      foreach (DefinitionEnvironmentStep definitionEnvironmentStep in rows.Where<DefinitionEnvironmentStep>((System.Func<DefinitionEnvironmentStep, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentStepTable.SqlMetaData2);
        if (definitionEnvironmentStep.ApproverId != Guid.Empty)
          record.SetGuid(ordinal, definitionEnvironmentStep.ApproverId);
        else
          record.SetDBNull(ordinal);
        int num1;
        record.SetInt32(num1 = ordinal + 1, definitionEnvironmentStep.DefinitionEnvironmentId);
        int num2;
        record.SetGuid(num2 = num1 + 1, definitionEnvironmentStep.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, definitionEnvironmentStep.Id);
        int num4;
        record.SetBoolean(num4 = num3 + 1, definitionEnvironmentStep.IsAutomated);
        int num5;
        record.SetBoolean(num5 = num4 + 1, definitionEnvironmentStep.IsNotificationOn);
        int num6;
        record.SetInt32(num6 = num5 + 1, definitionEnvironmentStep.Rank);
        int num7;
        record.SetByte(num7 = num6 + 1, (byte) definitionEnvironmentStep.StepType);
        int num8;
        record.SetNullableGuid(num8 = num7 + 1, new Guid?());
        yield return record;
      }
    }
  }
}
