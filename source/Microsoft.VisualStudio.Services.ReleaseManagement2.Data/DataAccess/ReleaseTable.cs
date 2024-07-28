// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseTable
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
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseTable
  {
    private const int DescriptionLength = 256;
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[13]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Definition", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Description", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Reason", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("Status", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("TargetEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData3 = new Microsoft.SqlServer.Server.SqlMetaData[12]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Definition", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Description", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Status", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("TargetEnvironmentId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData4 = new Microsoft.SqlServer.Server.SqlMetaData[13]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Description", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Status", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("Reason", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("VariableGroups", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseNameFormat", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildId", SqlDbType.Int)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData5 = new Microsoft.SqlServer.Server.SqlMetaData[14]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Description", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Status", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("Reason", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("VariableGroups", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseNameFormat", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildNumberRevision", SqlDbType.Int)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData6 = new Microsoft.SqlServer.Server.SqlMetaData[15]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Description", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("Status", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("Reason", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ModifiedOn", SqlDbType.DateTime),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("VariableGroups", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseNameFormat", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ReleaseDefinitionRevision", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("BuildNumberRevision", SqlDbType.Int)
    };

    public static void BindReleaseTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      Release release)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTable", ReleaseTable.GetSqlDataRecords(release));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(Release row)
    {
      int ordinal = 0;
      SqlDataRecord record = new SqlDataRecord(ReleaseTable.SqlMetaData);
      DateTime dateTime1 = row.CreatedOn;
      SqlDateTime minValue;
      DateTime createdOn;
      if (dateTime1.CompareTo(SqlDateTime.MinValue.Value) >= 0)
      {
        createdOn = row.CreatedOn;
      }
      else
      {
        minValue = SqlDateTime.MinValue;
        createdOn = minValue.Value;
      }
      DateTime dateTime2 = createdOn;
      dateTime1 = row.ModifiedOn;
      ref DateTime local = ref dateTime1;
      minValue = SqlDateTime.MinValue;
      DateTime dateTime3 = minValue.Value;
      DateTime modifiedOn;
      if (local.CompareTo(dateTime3) >= 0)
      {
        modifiedOn = row.ModifiedOn;
      }
      else
      {
        minValue = SqlDateTime.MinValue;
        modifiedOn = minValue.Value;
      }
      DateTime dateTime4 = modifiedOn;
      record.SetGuid(ordinal, row.CreatedBy);
      int num1;
      record.SetDateTime(num1 = ordinal + 1, dateTime2);
      int num2;
      record.SetNullableString(num2 = num1 + 1, ReleaseDefinitionSnapshotUtility.ToJson(row.DefinitionSnapshot));
      int num3;
      record.SetString(num3 = num2 + 1, string.IsNullOrEmpty(row.Description) ? string.Empty : row.Description);
      int num4;
      record.SetInt32(num4 = num3 + 1, row.Id);
      int num5;
      record.SetGuid(num5 = num4 + 1, row.ModifiedBy);
      int num6;
      record.SetDateTime(num6 = num5 + 1, dateTime4);
      int num7;
      record.SetString(num7 = num6 + 1, string.IsNullOrEmpty(row.Name) ? string.Empty : row.Name);
      int num8;
      record.SetInt32(num8 = num7 + 1, row.ReleaseDefinitionId);
      int num9;
      record.SetByte(num9 = num8 + 1, (byte) row.Reason);
      int num10;
      record.SetByte(num10 = num9 + 1, (byte) row.Status);
      int num11;
      record.SetInt32(num11 = num10 + 1, row.TargetEnvironmentId);
      IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(row.Variables);
      int num12;
      record.SetString(num12 = num11 + 1, ServerModelUtility.ToString((object) dictionary));
      yield return record;
    }

    public static void BindReleaseTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      Release release)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTableV3", ReleaseTable.GetSqlDataRecords3(release));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords3(Release row)
    {
      int ordinal = 0;
      SqlDataRecord record = new SqlDataRecord(ReleaseTable.SqlMetaData3);
      DateTime dateTime1 = row.CreatedOn;
      SqlDateTime minValue;
      DateTime createdOn;
      if (dateTime1.CompareTo(SqlDateTime.MinValue.Value) >= 0)
      {
        createdOn = row.CreatedOn;
      }
      else
      {
        minValue = SqlDateTime.MinValue;
        createdOn = minValue.Value;
      }
      DateTime dateTime2 = createdOn;
      dateTime1 = row.ModifiedOn;
      ref DateTime local = ref dateTime1;
      minValue = SqlDateTime.MinValue;
      DateTime dateTime3 = minValue.Value;
      DateTime modifiedOn;
      if (local.CompareTo(dateTime3) >= 0)
      {
        modifiedOn = row.ModifiedOn;
      }
      else
      {
        minValue = SqlDateTime.MinValue;
        modifiedOn = minValue.Value;
      }
      DateTime dateTime4 = modifiedOn;
      record.SetGuid(ordinal, row.CreatedBy);
      int num1;
      record.SetDateTime(num1 = ordinal + 1, dateTime2);
      int num2;
      record.SetNullableString(num2 = num1 + 1, ReleaseDefinitionSnapshotUtility.ToJson(row.DefinitionSnapshot));
      int num3;
      record.SetString(num3 = num2 + 1, string.IsNullOrEmpty(row.Description) ? string.Empty : row.Description);
      int num4;
      record.SetInt32(num4 = num3 + 1, row.Id);
      int num5;
      record.SetGuid(num5 = num4 + 1, row.ModifiedBy);
      int num6;
      record.SetDateTime(num6 = num5 + 1, dateTime4);
      int num7;
      record.SetString(num7 = num6 + 1, string.IsNullOrEmpty(row.Name) ? string.Empty : row.Name);
      int num8;
      record.SetInt32(num8 = num7 + 1, row.ReleaseDefinitionId);
      int num9;
      record.SetByte(num9 = num8 + 1, (byte) row.Status);
      int num10;
      record.SetInt32(num10 = num9 + 1, row.TargetEnvironmentId);
      IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(row.Variables);
      int num11;
      record.SetString(num11 = num10 + 1, ServerModelUtility.ToString((object) dictionary));
      yield return record;
    }

    public static void BindReleaseTable4(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Release> releases)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTableV4", ReleaseTable.GetSqlDataRecords4(releases));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords4(IEnumerable<Release> rows)
    {
      rows = rows ?? Enumerable.Empty<Release>();
      foreach (Release release in rows.Where<Release>((System.Func<Release, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseTable.SqlMetaData4);
        SqlDateTime minValue;
        DateTime createdOn;
        if (release.CreatedOn.CompareTo(SqlDateTime.MinValue.Value) >= 0)
        {
          createdOn = release.CreatedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          createdOn = minValue.Value;
        }
        DateTime dateTime1 = createdOn;
        DateTime modifiedOn1 = release.ModifiedOn;
        ref DateTime local = ref modifiedOn1;
        minValue = SqlDateTime.MinValue;
        DateTime dateTime2 = minValue.Value;
        DateTime modifiedOn2;
        if (local.CompareTo(dateTime2) >= 0)
        {
          modifiedOn2 = release.ModifiedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          modifiedOn2 = minValue.Value;
        }
        DateTime dateTime3 = modifiedOn2;
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(release.Variables);
        record.SetString(ordinal, string.IsNullOrEmpty(release.Name) ? string.Empty : release.Name);
        int num1;
        record.SetString(num1 = ordinal + 1, string.IsNullOrEmpty(release.Description) ? string.Empty : release.Description);
        int num2;
        record.SetByte(num2 = num1 + 1, (byte) release.Status);
        int num3;
        record.SetByte(num3 = num2 + 1, (byte) release.Reason);
        int num4;
        record.SetGuid(num4 = num3 + 1, release.CreatedBy);
        int num5;
        record.SetDateTime(num5 = num4 + 1, dateTime1);
        int num6;
        record.SetGuid(num6 = num5 + 1, release.ModifiedBy);
        int num7;
        record.SetDateTime(num7 = num6 + 1, dateTime3.ToUtcDateTime());
        int num8;
        record.SetString(num8 = num7 + 1, ServerModelUtility.ToString((object) dictionary));
        int num9;
        record.SetString(num9 = num8 + 1, ServerModelUtility.ToString((object) release.VariableGroups));
        int num10;
        record.SetNullableString(num10 = num9 + 1, release.ReleaseNameFormat);
        int num11;
        record.SetInt32(num11 = num10 + 1, 0);
        int num12;
        record.SetInt32(num12 = num11 + 1, 0);
        yield return record;
      }
    }

    public static void BindReleaseTable5(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Release> releases)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTableV5", ReleaseTable.GetSqlDataRecords5(releases));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords5(IEnumerable<Release> rows)
    {
      rows = rows ?? Enumerable.Empty<Release>();
      foreach (Release release in rows.Where<Release>((System.Func<Release, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseTable.SqlMetaData5);
        SqlDateTime minValue;
        DateTime createdOn;
        if (release.CreatedOn.CompareTo(SqlDateTime.MinValue.Value) >= 0)
        {
          createdOn = release.CreatedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          createdOn = minValue.Value;
        }
        DateTime dateTime1 = createdOn;
        DateTime modifiedOn1 = release.ModifiedOn;
        ref DateTime local = ref modifiedOn1;
        minValue = SqlDateTime.MinValue;
        DateTime dateTime2 = minValue.Value;
        DateTime modifiedOn2;
        if (local.CompareTo(dateTime2) >= 0)
        {
          modifiedOn2 = release.ModifiedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          modifiedOn2 = minValue.Value;
        }
        DateTime dateTime3 = modifiedOn2;
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(release.Variables);
        record.SetString(ordinal, string.IsNullOrEmpty(release.Name) ? string.Empty : release.Name);
        int num1;
        record.SetString(num1 = ordinal + 1, string.IsNullOrEmpty(release.Description) ? string.Empty : release.Description);
        int num2;
        record.SetByte(num2 = num1 + 1, (byte) release.Status);
        int num3;
        record.SetByte(num3 = num2 + 1, (byte) release.Reason);
        int num4;
        record.SetGuid(num4 = num3 + 1, release.CreatedBy);
        int num5;
        record.SetDateTime(num5 = num4 + 1, dateTime1);
        int num6;
        record.SetGuid(num6 = num5 + 1, release.ModifiedBy);
        int num7;
        record.SetDateTime(num7 = num6 + 1, dateTime3.ToUtcDateTime());
        int num8;
        record.SetString(num8 = num7 + 1, ServerModelUtility.ToString((object) dictionary));
        int num9;
        record.SetString(num9 = num8 + 1, ServerModelUtility.ToString((object) release.VariableGroups));
        int num10;
        record.SetNullableString(num10 = num9 + 1, release.ReleaseNameFormat);
        int num11;
        record.SetInt32(num11 = num10 + 1, 0);
        int num12;
        record.SetInt32(num12 = num11 + 1, 0);
        int num13;
        record.SetInt32(num13 = num12 + 1, release.DefinitionSnapshotRevision);
        yield return record;
      }
    }

    public static void BindReleaseTable6(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Release> releases)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTableV6", ReleaseTable.GetSqlDataRecords6(releases));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords6(IEnumerable<Release> rows)
    {
      rows = rows ?? Enumerable.Empty<Release>();
      foreach (Release release in rows.Where<Release>((System.Func<Release, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseTable.SqlMetaData6);
        SqlDateTime minValue;
        DateTime createdOn;
        if (release.CreatedOn.CompareTo(SqlDateTime.MinValue.Value) >= 0)
        {
          createdOn = release.CreatedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          createdOn = minValue.Value;
        }
        DateTime dateTime1 = createdOn;
        DateTime modifiedOn1 = release.ModifiedOn;
        ref DateTime local = ref modifiedOn1;
        minValue = SqlDateTime.MinValue;
        DateTime dateTime2 = minValue.Value;
        DateTime modifiedOn2;
        if (local.CompareTo(dateTime2) >= 0)
        {
          modifiedOn2 = release.ModifiedOn;
        }
        else
        {
          minValue = SqlDateTime.MinValue;
          modifiedOn2 = minValue.Value;
        }
        DateTime dateTime3 = modifiedOn2;
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(release.Variables);
        record.SetString(ordinal, string.IsNullOrEmpty(release.Name) ? string.Empty : release.Name);
        int num1;
        record.SetString(num1 = ordinal + 1, string.IsNullOrEmpty(release.Description) ? string.Empty : release.Description);
        int num2;
        record.SetByte(num2 = num1 + 1, (byte) release.Status);
        int num3;
        record.SetByte(num3 = num2 + 1, (byte) release.Reason);
        int num4;
        record.SetGuid(num4 = num3 + 1, release.CreatedBy);
        int num5;
        record.SetDateTime(num5 = num4 + 1, dateTime1);
        int num6;
        record.SetGuid(num6 = num5 + 1, release.ModifiedBy);
        int num7;
        record.SetDateTime(num7 = num6 + 1, dateTime3.ToUtcDateTime());
        int num8;
        record.SetString(num8 = num7 + 1, ServerModelUtility.ToString((object) dictionary));
        int num9;
        record.SetString(num9 = num8 + 1, ServerModelUtility.ToString((object) release.VariableGroups));
        int num10;
        record.SetNullableString(num10 = num9 + 1, release.ReleaseNameFormat);
        int num11;
        record.SetInt32(num11 = num10 + 1, release.ReleaseDefinitionRevision);
        int num12;
        record.SetInt32(num12 = num11 + 1, 0);
        int num13;
        record.SetInt32(num13 = num12 + 1, 0);
        int num14;
        record.SetInt32(num14 = num13 + 1, release.DefinitionSnapshotRevision);
        yield return record;
      }
    }
  }
}
