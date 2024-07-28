// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseDefinitionTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseDefinitionTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[7]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData2 = new Microsoft.SqlServer.Server.SqlMetaData[12]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("name", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("createdBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("createdOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("modifiedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("modifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("variableGroups", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("releaseNameFormat", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("type", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("buildDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("path", SqlDbType.NVarChar, 400L),
      new Microsoft.SqlServer.Server.SqlMetaData("guidId", SqlDbType.UniqueIdentifier)
    };

    public static void BindReleaseDefinitionTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      ReleaseDefinition releaseDefinition)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseDefinitionTableV2", ReleaseDefinitionTable.GetSqlDataRecords(releaseDefinition));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(ReleaseDefinition row)
    {
      int ordinal = 0;
      SqlDataRecord rec = new SqlDataRecord(ReleaseDefinitionTable.SqlMetaData);
      rec.SetGuid(ordinal, row.CreatedBy);
      int num1;
      rec.SetNullableDateTime(num1 = ordinal + 1, row.CreatedOn);
      int num2;
      rec.SetInt32(num2 = num1 + 1, row.Id);
      int num3;
      rec.SetGuid(num3 = num2 + 1, row.ModifiedBy);
      int num4;
      rec.SetNullableDateTime(num4 = num3 + 1, row.ModifiedOn);
      int num5;
      rec.SetString(num5 = num4 + 1, string.IsNullOrEmpty(row.Name) ? string.Empty : row.Name);
      IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(row.Variables);
      int num6;
      rec.SetString(num6 = num5 + 1, ServerModelUtility.ToString((object) dictionary));
      yield return rec;
    }

    public static void BindPipelineDefinition(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseDefinition> releaseDefinitions)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_PipelineDefinition", ReleaseDefinitionTable.GetSqlDataRecords(releaseDefinitions));
    }

    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<ReleaseDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseDefinition>();
      foreach (ReleaseDefinition releaseDefinition in rows.Where<ReleaseDefinition>((System.Func<ReleaseDefinition, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseDefinitionTable.SqlMetaData2);
        sqlDataRecord.SetString(ordinal, string.IsNullOrEmpty(releaseDefinition.Name) ? string.Empty : releaseDefinition.Name);
        int num1;
        sqlDataRecord.SetGuid(num1 = ordinal + 1, releaseDefinition.CreatedBy);
        int num2;
        sqlDataRecord.SetNullableDateTime(num2 = num1 + 1, releaseDefinition.CreatedOn);
        int num3;
        sqlDataRecord.SetGuid(num3 = num2 + 1, releaseDefinition.ModifiedBy);
        int num4;
        sqlDataRecord.SetNullableDateTime(num4 = num3 + 1, releaseDefinition.ModifiedOn);
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseDefinition.Variables);
        int num5;
        sqlDataRecord.SetString(num5 = num4 + 1, ServerModelUtility.ToString((object) dictionary));
        int num6;
        sqlDataRecord.SetString(num6 = num5 + 1, ServerModelUtility.ToString((object) releaseDefinition.VariableGroups));
        int num7;
        sqlDataRecord.SetString(num7 = num6 + 1, releaseDefinition.ReleaseNameFormat);
        int num8;
        sqlDataRecord.SetInt32(num8 = num7 + 1, 0);
        int num9;
        sqlDataRecord.SetString(num9 = num8 + 1, string.IsNullOrWhiteSpace(releaseDefinition.Path) ? "\\" : PathHelper.UserToDBPath(releaseDefinition.Path));
        int num10;
        sqlDataRecord.SetNullableGuid(num10 = num9 + 1, new Guid?());
        yield return sqlDataRecord;
      }
    }
  }
}
