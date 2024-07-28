// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseEnvironmentStepTable3
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
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseEnvironmentStepTable3
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[16]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("ActualApproverId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ApproverId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ApproverComment", SqlDbType.NVarChar, 1024L),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionEnvironmentRank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("IsAutomated", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Status", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("StepType", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("TrialNumber", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ReassignedFromStepId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Logs", SqlDbType.NVarChar, 4000L)
    };

    public static void BindReleaseEnvironmentStepTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentStepTableV4", ReleaseEnvironmentStepTable3.GetSqlDataRecords(releaseEnvironmentSteps));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<ReleaseEnvironmentStep> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironmentStep>();
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in rows.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (r => r != null)))
      {
        int ordinal1 = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentStepTable3.SqlMetaData);
        record.SetGuid(ordinal1, releaseEnvironmentStep.ActualApproverId);
        int num1;
        record.SetGuid(num1 = ordinal1 + 1, releaseEnvironmentStep.ApproverId);
        int num2;
        record.SetString(num2 = num1 + 1, string.IsNullOrEmpty(releaseEnvironmentStep.ApproverComment) ? string.Empty : releaseEnvironmentStep.ApproverComment);
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal2;
        int num3 = ordinal2 = num2 + 1;
        DateTime dateTime1 = releaseEnvironmentStep.CreatedOn;
        ref DateTime local1 = ref dateTime1;
        SqlDateTime minValue = SqlDateTime.MinValue;
        DateTime dateTime2 = minValue.Value;
        DateTime createdOn;
        if (local1.CompareTo(dateTime2) >= 0)
        {
          createdOn = releaseEnvironmentStep.CreatedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          createdOn = minValue.Value;
        }
        sqlDataRecord1.SetDateTime(ordinal2, createdOn);
        int num4;
        record.SetInt32(num4 = num3 + 1, releaseEnvironmentStep.DefinitionEnvironmentId);
        int num5;
        record.SetInt32(num5 = num4 + 1, releaseEnvironmentStep.DefinitionEnvironmentRank);
        int num6;
        record.SetInt32(num6 = num5 + 1, releaseEnvironmentStep.ReleaseEnvironmentId);
        int num7;
        record.SetInt32(num7 = num6 + 1, releaseEnvironmentStep.Id);
        int num8;
        record.SetBoolean(num8 = num7 + 1, releaseEnvironmentStep.IsAutomated);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal3;
        int num9 = ordinal3 = num8 + 1;
        dateTime1 = releaseEnvironmentStep.ModifiedOn;
        ref DateTime local2 = ref dateTime1;
        minValue = SqlDateTime.MinValue;
        DateTime dateTime3 = minValue.Value;
        DateTime modifiedOn;
        if (local2.CompareTo(dateTime3) >= 0)
        {
          modifiedOn = releaseEnvironmentStep.ModifiedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          modifiedOn = minValue.Value;
        }
        sqlDataRecord2.SetDateTime(ordinal3, modifiedOn);
        int num10;
        record.SetInt32(num10 = num9 + 1, releaseEnvironmentStep.Rank);
        int num11;
        record.SetByte(num11 = num10 + 1, (byte) releaseEnvironmentStep.Status);
        int num12;
        record.SetInt32(num12 = num11 + 1, (int) releaseEnvironmentStep.StepType);
        int num13;
        record.SetInt32(num13 = num12 + 1, releaseEnvironmentStep.TrialNumber);
        int num14;
        record.SetInt32(num14 = num13 + 1, releaseEnvironmentStep.GroupStepId);
        int num15;
        record.SetNullableString(num15 = num14 + 1, releaseEnvironmentStep.Logs);
        yield return record;
      }
    }
  }
}
