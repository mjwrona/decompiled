// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseEnvironmentStepTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseEnvironmentStepTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[16]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ActualApproverId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ApproverId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ApproverComment", SqlDbType.NVarChar, 1024L),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentRank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentStepId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("IsAutomated", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Status", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("StepType", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("TrialNumber", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Logs", SqlDbType.NVarChar, 4000L)
    };

    public static void BindReleaseEnvironmentStepTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentStepTableV2", ReleaseEnvironmentStepTable.GetSqlDataRecords(releaseEnvironmentStep));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(ReleaseEnvironmentStep row)
    {
      int ordinal1 = 0;
      SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentStepTable.SqlMetaData);
      record.SetGuid(ordinal1, row.ActualApproverId);
      int num1;
      record.SetGuid(num1 = ordinal1 + 1, row.ApproverId);
      int num2;
      record.SetString(num2 = num1 + 1, string.IsNullOrEmpty(row.ApproverComment) ? string.Empty : row.ApproverComment);
      SqlDataRecord sqlDataRecord1 = record;
      int ordinal2;
      int num3 = ordinal2 = num2 + 1;
      DateTime createdOn1 = row.CreatedOn;
      ref DateTime local1 = ref createdOn1;
      SqlDateTime minValue = SqlDateTime.MinValue;
      DateTime dateTime1 = minValue.Value;
      DateTime createdOn2;
      if (local1.CompareTo(dateTime1) >= 0)
      {
        createdOn2 = row.CreatedOn;
      }
      else
      {
        minValue = SqlDateTime.MinValue;
        createdOn2 = minValue.Value;
      }
      sqlDataRecord1.SetDateTime(ordinal2, createdOn2);
      int num4;
      record.SetInt32(num4 = num3 + 1, row.DefinitionEnvironmentId);
      int num5;
      record.SetInt32(num5 = num4 + 1, row.DefinitionEnvironmentRank);
      int num6;
      record.SetInt32(num6 = num5 + 1, 0);
      int num7;
      record.SetInt32(num7 = num6 + 1, row.ReleaseEnvironmentId);
      int num8;
      record.SetInt32(num8 = num7 + 1, row.Id);
      int num9;
      record.SetBoolean(num9 = num8 + 1, row.IsAutomated);
      SqlDataRecord sqlDataRecord2 = record;
      int ordinal3;
      int num10 = ordinal3 = num9 + 1;
      DateTime modifiedOn1 = row.ModifiedOn;
      ref DateTime local2 = ref modifiedOn1;
      minValue = SqlDateTime.MinValue;
      DateTime dateTime2 = minValue.Value;
      DateTime modifiedOn2;
      if (local2.CompareTo(dateTime2) >= 0)
      {
        modifiedOn2 = row.ModifiedOn;
      }
      else
      {
        minValue = SqlDateTime.MinValue;
        modifiedOn2 = minValue.Value;
      }
      sqlDataRecord2.SetDateTime(ordinal3, modifiedOn2);
      int num11;
      record.SetInt32(num11 = num10 + 1, row.Rank);
      int num12;
      record.SetByte(num12 = num11 + 1, (byte) row.Status);
      int num13;
      record.SetInt32(num13 = num12 + 1, (int) row.StepType);
      int num14;
      record.SetInt32(num14 = num13 + 1, row.TrialNumber);
      int num15;
      record.SetNullableString(num15 = num14 + 1, row.Logs);
      yield return record;
    }
  }
}
