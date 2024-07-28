// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseEnvironmentTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Table is the correct term here")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ReleaseEnvironmentTable
  {
    private static readonly SqlMetaData[] SqlMetaData6 = new SqlMetaData[13]
    {
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("DefinitionDemands", SqlDbType.NVarChar, -1L),
      new SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Rank", SqlDbType.Int),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] SqlMetaData7 = new SqlMetaData[11]
    {
      new SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Rank", SqlDbType.Int),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DeployPhaseSnapshots", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] SqlMetaData8 = new SqlMetaData[11]
    {
      new SqlMetaData("RunOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Rank", SqlDbType.Int),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DeployPhaseSnapshots", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] SqlMetaData9 = new SqlMetaData[13]
    {
      new SqlMetaData("RunOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Rank", SqlDbType.Int),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DeployPhaseSnapshots", SqlDbType.NVarChar, -1L),
      new SqlMetaData("BuildId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] SqlMetaData10 = new SqlMetaData[14]
    {
      new SqlMetaData("RunOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Rank", SqlDbType.Int),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DeployPhaseSnapshots", SqlDbType.NVarChar, -1L),
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] SqlMetaData11 = new SqlMetaData[15]
    {
      new SqlMetaData("RunOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Rank", SqlDbType.Int),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DeployPhaseSnapshots", SqlDbType.NVarChar, -1L),
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Gates", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] SqlMetaData12 = new SqlMetaData[16]
    {
      new SqlMetaData("RunOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("DefinitionEnvironmentId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Rank", SqlDbType.Int),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new SqlMetaData("VariableGroups", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DeployPhaseSnapshots", SqlDbType.NVarChar, -1L),
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Gates", SqlDbType.NVarChar, -1L)
    };

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parameterName", Justification = "deprecated code")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "releaseEnvironments", Justification = "deprecated code")]
    public static void BindReleaseEnvironmentTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parameterName", Justification = "deprecated code")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "releaseEnvironments", Justification = "deprecated code")]
    public static void BindReleaseEnvironmentTable4(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parameterName", Justification = "deprecated code")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "releaseEnvironments", Justification = "deprecated code")]
    public static void BindReleaseEnvironmentTable5(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
    }

    public static void BindReleaseEnvironmentTable6(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentTableV6", ReleaseEnvironmentTable.GetSqlDataRecords6(releaseEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords6(
      IEnumerable<ReleaseEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironment>();
      foreach (ReleaseEnvironment releaseEnvironment in rows.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentTable.SqlMetaData6);
        record.SetInt32(ordinal, releaseEnvironment.GetCompatQueueId());
        int num1;
        record.SetString(num1 = ordinal + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetCompatEnvironmentOptions()));
        string str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetPreAndPostApprovalOptionsFromReleaseEnvironment());
        string compatWorkflow = releaseEnvironment.GetCompatWorkflow();
        int num2;
        record.SetString(num2 = num1 + 1, str1);
        int num3;
        record.SetNullableString(num3 = num2 + 1, releaseEnvironment.GetCompatDemands());
        int num4;
        record.SetInt32(num4 = num3 + 1, releaseEnvironment.DefinitionEnvironmentId);
        int num5;
        record.SetInt32(num5 = num4 + 1, releaseEnvironment.Id);
        int num6;
        record.SetString(num6 = num5 + 1, string.IsNullOrEmpty(releaseEnvironment.Name) ? string.Empty : releaseEnvironment.Name);
        int num7;
        record.SetInt32(num7 = num6 + 1, releaseEnvironment.Rank);
        int num8;
        record.SetInt32(num8 = num7 + 1, releaseEnvironment.ReleaseId);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrEmpty(compatWorkflow) ? string.Empty : compatWorkflow);
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseEnvironment.Variables);
        int num10;
        record.SetString(num10 = num9 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) dictionary));
        string str2 = releaseEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) releaseEnvironment.Conditions);
        int num11;
        record.SetNullableString(num11 = num10 + 1, str2);
        int num12;
        record.SetGuid(num12 = num11 + 1, releaseEnvironment.OwnerId);
        yield return record;
      }
    }

    public static void BindReleaseEnvironmentTable7(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentTableV7", ReleaseEnvironmentTable.GetSqlDataRecords7(releaseEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords7(
      IEnumerable<ReleaseEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironment>();
      foreach (ReleaseEnvironment releaseEnvironment in rows.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentTable.SqlMetaData7);
        string str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetPreAndPostApprovalOptionsFromReleaseEnvironment());
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseEnvironment.Variables);
        string str2 = releaseEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) releaseEnvironment.Conditions);
        record.SetString(ordinal, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.EnvironmentOptions));
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetInt32(num2 = num1 + 1, releaseEnvironment.DefinitionEnvironmentId);
        int num3;
        record.SetInt32(num3 = num2 + 1, releaseEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(releaseEnvironment.Name) ? string.Empty : releaseEnvironment.Name);
        int num5;
        record.SetInt32(num5 = num4 + 1, releaseEnvironment.Rank);
        int num6;
        record.SetInt32(num6 = num5 + 1, releaseEnvironment.ReleaseId);
        int num7;
        record.SetString(num7 = num6 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) dictionary));
        int num8;
        record.SetNullableString(num8 = num7 + 1, str2);
        int num9;
        record.SetGuid(num9 = num8 + 1, releaseEnvironment.OwnerId);
        int num10;
        record.SetString(num10 = num9 + 1, ReleaseEnvironmentTable.GetDeployPhaseSnapshotsString(releaseEnvironment));
        yield return record;
      }
    }

    public static void BindReleaseEnvironmentTable8(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentTableV8", ReleaseEnvironmentTable.GetSqlDataRecords8(releaseEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords8(
      IEnumerable<ReleaseEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironment>();
      foreach (ReleaseEnvironment releaseEnvironment in rows.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentTable.SqlMetaData8);
        string str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetPreAndPostApprovalOptionsFromReleaseEnvironment());
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseEnvironment.Variables);
        string str2 = releaseEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) releaseEnvironment.Conditions);
        record.SetString(ordinal, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.EnvironmentOptions));
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetInt32(num2 = num1 + 1, releaseEnvironment.DefinitionEnvironmentId);
        int num3;
        record.SetInt32(num3 = num2 + 1, releaseEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(releaseEnvironment.Name) ? string.Empty : releaseEnvironment.Name);
        int num5;
        record.SetInt32(num5 = num4 + 1, releaseEnvironment.Rank);
        int num6;
        record.SetInt32(num6 = num5 + 1, releaseEnvironment.ReleaseId);
        int num7;
        record.SetString(num7 = num6 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) dictionary));
        int num8;
        record.SetNullableString(num8 = num7 + 1, str2);
        int num9;
        record.SetGuid(num9 = num8 + 1, releaseEnvironment.OwnerId);
        int num10;
        record.SetString(num10 = num9 + 1, ReleaseEnvironmentTable.GetDeployPhaseSnapshotsString(releaseEnvironment));
        yield return record;
      }
    }

    public static void BindReleaseEnvironmentTable9(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentTableV9", ReleaseEnvironmentTable.GetSqlDataRecords9(releaseEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords9(
      IEnumerable<ReleaseEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironment>();
      foreach (ReleaseEnvironment releaseEnvironment in rows.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentTable.SqlMetaData9);
        string str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetPreAndPostApprovalOptionsFromReleaseEnvironment());
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseEnvironment.Variables);
        string str2 = releaseEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) releaseEnvironment.Conditions);
        record.SetString(ordinal, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.EnvironmentOptions));
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetInt32(num2 = num1 + 1, releaseEnvironment.DefinitionEnvironmentId);
        int num3;
        record.SetInt32(num3 = num2 + 1, releaseEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(releaseEnvironment.Name) ? string.Empty : releaseEnvironment.Name);
        int num5;
        record.SetByte(num5 = num4 + 1, (byte) releaseEnvironment.Status);
        int num6;
        record.SetInt32(num6 = num5 + 1, releaseEnvironment.Rank);
        int num7;
        record.SetInt32(num7 = num6 + 1, releaseEnvironment.ReleaseId);
        int num8;
        record.SetString(num8 = num7 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) dictionary));
        int num9;
        record.SetNullableString(num9 = num8 + 1, str2);
        int num10;
        record.SetGuid(num10 = num9 + 1, releaseEnvironment.OwnerId);
        int num11;
        record.SetString(num11 = num10 + 1, ReleaseEnvironmentTable.GetDeployPhaseSnapshotsString(releaseEnvironment));
        int num12;
        record.SetInt32(num12 = num11 + 1, 0);
        yield return record;
      }
    }

    public static void BindReleaseEnvironmentTable10(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentTableV10", ReleaseEnvironmentTable.GetSqlDataRecords10(releaseEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords10(
      IEnumerable<ReleaseEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironment>();
      foreach (ReleaseEnvironment releaseEnvironment in rows.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentTable.SqlMetaData10);
        string str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetPreAndPostApprovalOptionsFromReleaseEnvironment());
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseEnvironment.Variables);
        string str2 = releaseEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) releaseEnvironment.Conditions);
        record.SetString(ordinal, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.EnvironmentOptions));
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetInt32(num2 = num1 + 1, releaseEnvironment.DefinitionEnvironmentId);
        int num3;
        record.SetInt32(num3 = num2 + 1, releaseEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(releaseEnvironment.Name) ? string.Empty : releaseEnvironment.Name);
        int num5;
        record.SetByte(num5 = num4 + 1, (byte) releaseEnvironment.Status);
        int num6;
        record.SetInt32(num6 = num5 + 1, releaseEnvironment.Rank);
        int num7;
        record.SetInt32(num7 = num6 + 1, releaseEnvironment.ReleaseId);
        int num8;
        record.SetString(num8 = num7 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) dictionary));
        int num9;
        record.SetNullableString(num9 = num8 + 1, str2);
        int num10;
        record.SetGuid(num10 = num9 + 1, releaseEnvironment.OwnerId);
        int num11;
        record.SetString(num11 = num10 + 1, ReleaseEnvironmentTable.GetDeployPhaseSnapshotsString(releaseEnvironment));
        int num12;
        record.SetInt32(num12 = num11 + 1, 0);
        int num13;
        record.SetNullableString(num13 = num12 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.ProcessParameters));
        yield return record;
      }
    }

    public static void BindReleaseEnvironmentTable11(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentTableV11", ReleaseEnvironmentTable.GetSqlDataRecords11(releaseEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords11(
      IEnumerable<ReleaseEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironment>();
      foreach (ReleaseEnvironment releaseEnvironment in rows.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentTable.SqlMetaData11);
        string str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetPreAndPostApprovalOptionsFromReleaseEnvironment());
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseEnvironment.Variables);
        string str2 = releaseEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) releaseEnvironment.Conditions);
        record.SetString(ordinal, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.EnvironmentOptions));
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetInt32(num2 = num1 + 1, releaseEnvironment.DefinitionEnvironmentId);
        int num3;
        record.SetInt32(num3 = num2 + 1, releaseEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(releaseEnvironment.Name) ? string.Empty : releaseEnvironment.Name);
        int num5;
        record.SetByte(num5 = num4 + 1, (byte) releaseEnvironment.Status);
        int num6;
        record.SetInt32(num6 = num5 + 1, releaseEnvironment.Rank);
        int num7;
        record.SetInt32(num7 = num6 + 1, releaseEnvironment.ReleaseId);
        int num8;
        record.SetString(num8 = num7 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) dictionary));
        int num9;
        record.SetNullableString(num9 = num8 + 1, str2);
        int num10;
        record.SetGuid(num10 = num9 + 1, releaseEnvironment.OwnerId);
        int num11;
        record.SetString(num11 = num10 + 1, ReleaseEnvironmentTable.GetDeployPhaseSnapshotsString(releaseEnvironment));
        int num12;
        record.SetInt32(num12 = num11 + 1, 0);
        int num13;
        record.SetNullableString(num13 = num12 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.ProcessParameters));
        int num14;
        record.SetNullableString(num14 = num13 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetDefinitionGates()));
        yield return record;
      }
    }

    public static void BindReleaseEnvironmentTable12(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseEnvironmentTableV12", ReleaseEnvironmentTable.GetSqlDataRecords12(releaseEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords12(
      IEnumerable<ReleaseEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironment>();
      foreach (ReleaseEnvironment releaseEnvironment in rows.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseEnvironmentTable.SqlMetaData12);
        string str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetPreAndPostApprovalOptionsFromReleaseEnvironment());
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(releaseEnvironment.Variables);
        IList<VariableGroup> variableGroupList = VariableGroupUtility.ClearSecrets(releaseEnvironment.VariableGroups);
        string str2 = releaseEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) releaseEnvironment.Conditions);
        if (str2.Length > 4000)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MaxPropertyLengthExceeded, (object) "conditionsString", (object) 4000.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
        record.SetString(ordinal, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.EnvironmentOptions));
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetInt32(num2 = num1 + 1, releaseEnvironment.DefinitionEnvironmentId);
        int num3;
        record.SetInt32(num3 = num2 + 1, releaseEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(releaseEnvironment.Name) ? string.Empty : releaseEnvironment.Name);
        int num5;
        record.SetByte(num5 = num4 + 1, (byte) releaseEnvironment.Status);
        int num6;
        record.SetInt32(num6 = num5 + 1, releaseEnvironment.Rank);
        int num7;
        record.SetInt32(num7 = num6 + 1, releaseEnvironment.ReleaseId);
        int num8;
        record.SetString(num8 = num7 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) dictionary));
        int num9;
        record.SetString(num9 = num8 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) variableGroupList));
        int num10;
        record.SetNullableString(num10 = num9 + 1, str2);
        int num11;
        record.SetGuid(num11 = num10 + 1, releaseEnvironment.OwnerId);
        int num12;
        record.SetString(num12 = num11 + 1, JsonConvert.SerializeObject((object) releaseEnvironment.DeploymentSnapshot));
        int num13;
        record.SetInt32(num13 = num12 + 1, 0);
        int num14;
        record.SetNullableString(num14 = num13 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.ProcessParameters));
        int num15;
        record.SetNullableString(num15 = num14 + 1, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) releaseEnvironment.GetDefinitionGates()));
        yield return record;
      }
    }

    private static string GetDeployPhaseSnapshotsString(ReleaseEnvironment environment) => JsonConvert.SerializeObject((object) environment.GetDesignerDeployPhaseSnapshots());
  }
}
