// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DefinitionEnvironmentTable
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
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
  public static class DefinitionEnvironmentTable
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[9]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("AgentPoolId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Demands", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData3 = new Microsoft.SqlServer.Server.SqlMetaData[10]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("AgentPoolId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Demands", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData4 = new Microsoft.SqlServer.Server.SqlMetaData[11]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("AgentPoolId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("QueueId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Demands", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData5 = new Microsoft.SqlServer.Server.SqlMetaData[13]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("QueueId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("Demands", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ExecutionPolicies", SqlDbType.NVarChar, 2048L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData6 = new Microsoft.SqlServer.Server.SqlMetaData[14]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("QueueId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("Demands", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Workflow", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ExecutionPolicies", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("Schedules", SqlDbType.NVarChar, 4000L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData7 = new Microsoft.SqlServer.Server.SqlMetaData[13]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ExecutionPolicies", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("Schedules", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetentionPolicy", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetainBuild", SqlDbType.Bit)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData8 = new Microsoft.SqlServer.Server.SqlMetaData[14]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ExecutionPolicies", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("Schedules", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetentionPolicy", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetainBuild", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData9 = new Microsoft.SqlServer.Server.SqlMetaData[15]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ExecutionPolicies", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("Schedules", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetentionPolicy", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetainBuild", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData10 = new Microsoft.SqlServer.Server.SqlMetaData[16]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ExecutionPolicies", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("Schedules", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetentionPolicy", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetainBuild", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Gates", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData11 = new Microsoft.SqlServer.Server.SqlMetaData[17]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("RunOptions", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("ApprovalOptions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("GuidId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Id", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Name", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("OwnerId", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("Rank", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Variables", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("VariableGroups", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Conditions", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("ExecutionPolicies", SqlDbType.NVarChar, 2048L),
      new Microsoft.SqlServer.Server.SqlMetaData("Schedules", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetentionPolicy", SqlDbType.NVarChar, 4000L),
      new Microsoft.SqlServer.Server.SqlMetaData("RetainBuild", SqlDbType.Bit),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier),
      new Microsoft.SqlServer.Server.SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("Gates", SqlDbType.NVarChar, -1L)
    };

    public static void BindDefinitionEnvironmentTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV2", DefinitionEnvironmentTable.GetSqlDataRecords(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment environment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData);
        record.SetInt32(ordinal, 0);
        int num1;
        record.SetNullableString(num1 = ordinal + 1, environment.GetCompatDemands());
        int num2;
        record.SetGuid(num2 = num1 + 1, environment.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, environment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(environment.Name) ? string.Empty : environment.Name);
        int num5;
        record.SetGuid(num5 = num4 + 1, environment.OwnerId);
        int num6;
        record.SetInt32(num6 = num5 + 1, environment.Rank);
        int num7;
        record.SetString(num7 = num6 + 1, string.IsNullOrEmpty(environment.GetCompatWorkflow()) ? string.Empty : environment.GetCompatWorkflow());
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(environment.Variables);
        int num8;
        record.SetString(num8 = num7 + 1, ServerModelUtility.ToString((object) dictionary));
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV3", DefinitionEnvironmentTable.GetSqlDataRecords3(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords3(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment environment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData3);
        record.SetInt32(ordinal, 0);
        int num1;
        record.SetString(num1 = ordinal + 1, ServerModelUtility.ToString((object) environment.GetCompatEnvironmentOptions()));
        int num2;
        record.SetNullableString(num2 = num1 + 1, environment.GetCompatDemands());
        int num3;
        record.SetGuid(num3 = num2 + 1, environment.GuidId);
        int num4;
        record.SetInt32(num4 = num3 + 1, environment.Id);
        int num5;
        record.SetString(num5 = num4 + 1, string.IsNullOrEmpty(environment.Name) ? string.Empty : environment.Name);
        int num6;
        record.SetGuid(num6 = num5 + 1, environment.OwnerId);
        int num7;
        record.SetInt32(num7 = num6 + 1, environment.Rank);
        int num8;
        record.SetString(num8 = num7 + 1, string.IsNullOrEmpty(environment.GetCompatWorkflow()) ? string.Empty : environment.GetCompatWorkflow());
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(environment.Variables);
        int num9;
        record.SetString(num9 = num8 + 1, ServerModelUtility.ToString((object) dictionary));
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable4(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV4", DefinitionEnvironmentTable.GetSqlDataRecords4(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords4(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment environment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData4);
        record.SetInt32(ordinal, 0);
        int num1;
        record.SetInt32(num1 = ordinal + 1, environment.GetCompatQueueId());
        int num2;
        record.SetString(num2 = num1 + 1, ServerModelUtility.ToString((object) environment.GetCompatEnvironmentOptions()));
        int num3;
        record.SetNullableString(num3 = num2 + 1, environment.GetCompatDemands());
        int num4;
        record.SetGuid(num4 = num3 + 1, environment.GuidId);
        int num5;
        record.SetInt32(num5 = num4 + 1, environment.Id);
        int num6;
        record.SetString(num6 = num5 + 1, string.IsNullOrEmpty(environment.Name) ? string.Empty : environment.Name);
        int num7;
        record.SetGuid(num7 = num6 + 1, environment.OwnerId);
        int num8;
        record.SetInt32(num8 = num7 + 1, environment.Rank);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrEmpty(environment.GetCompatWorkflow()) ? string.Empty : environment.GetCompatWorkflow());
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(environment.Variables);
        int num10;
        record.SetString(num10 = num9 + 1, ServerModelUtility.ToString((object) dictionary));
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable5(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV5", DefinitionEnvironmentTable.GetSqlDataRecords5(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords5(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData5);
        record.SetInt32(ordinal, definitionEnvironment.GetCompatQueueId());
        int num1;
        record.SetString(num1 = ordinal + 1, ServerModelUtility.ToString((object) definitionEnvironment.GetCompatEnvironmentOptions()));
        string str1 = ServerModelUtility.ToString((object) definitionEnvironment.GetPreAndPostApprovalOptionsFromDefinitionEnvironment());
        int num2;
        record.SetString(num2 = num1 + 1, str1);
        int num3;
        record.SetNullableString(num3 = num2 + 1, definitionEnvironment.GetCompatDemands());
        int num4;
        record.SetGuid(num4 = num3 + 1, definitionEnvironment.GuidId);
        int num5;
        record.SetInt32(num5 = num4 + 1, definitionEnvironment.Id);
        int num6;
        record.SetString(num6 = num5 + 1, string.IsNullOrEmpty(definitionEnvironment.Name) ? string.Empty : definitionEnvironment.Name);
        int num7;
        record.SetGuid(num7 = num6 + 1, definitionEnvironment.OwnerId);
        int num8;
        record.SetInt32(num8 = num7 + 1, definitionEnvironment.Rank);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrEmpty(definitionEnvironment.GetCompatWorkflow()) ? string.Empty : definitionEnvironment.GetCompatWorkflow());
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(definitionEnvironment.Variables);
        int num10;
        record.SetString(num10 = num9 + 1, ServerModelUtility.ToString((object) dictionary));
        string str2 = definitionEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Conditions);
        int num11;
        record.SetNullableString(num11 = num10 + 1, str2);
        string str3 = ServerModelUtility.ToString((object) definitionEnvironment.ExecutionPolicy);
        int num12;
        record.SetString(num12 = num11 + 1, string.IsNullOrWhiteSpace(str3) ? string.Empty : str3);
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable6(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV6", DefinitionEnvironmentTable.GetSqlDataRecords6(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords6(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData6);
        record.SetInt32(ordinal, definitionEnvironment.GetCompatQueueId());
        int num1;
        record.SetString(num1 = ordinal + 1, ServerModelUtility.ToString((object) definitionEnvironment.GetCompatEnvironmentOptions()));
        string str1 = ServerModelUtility.ToString((object) definitionEnvironment.GetPreAndPostApprovalOptionsFromDefinitionEnvironment());
        int num2;
        record.SetString(num2 = num1 + 1, str1);
        string compatWorkflow = definitionEnvironment.GetCompatWorkflow();
        int num3;
        record.SetNullableString(num3 = num2 + 1, definitionEnvironment.GetCompatDemands());
        int num4;
        record.SetGuid(num4 = num3 + 1, definitionEnvironment.GuidId);
        int num5;
        record.SetInt32(num5 = num4 + 1, definitionEnvironment.Id);
        int num6;
        record.SetString(num6 = num5 + 1, string.IsNullOrEmpty(definitionEnvironment.Name) ? string.Empty : definitionEnvironment.Name);
        int num7;
        record.SetGuid(num7 = num6 + 1, definitionEnvironment.OwnerId);
        int num8;
        record.SetInt32(num8 = num7 + 1, definitionEnvironment.Rank);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrEmpty(compatWorkflow) ? string.Empty : compatWorkflow);
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(definitionEnvironment.Variables);
        int num10;
        record.SetString(num10 = num9 + 1, ServerModelUtility.ToString((object) dictionary));
        string str2 = definitionEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Conditions);
        int num11;
        record.SetNullableString(num11 = num10 + 1, str2);
        string str3 = ServerModelUtility.ToString((object) definitionEnvironment.ExecutionPolicy);
        int num12;
        record.SetString(num12 = num11 + 1, string.IsNullOrWhiteSpace(str3) ? string.Empty : str3);
        string str4 = definitionEnvironment.Schedules == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Schedules);
        int num13;
        record.SetNullableString(num13 = num12 + 1, str4);
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable7(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV7", DefinitionEnvironmentTable.GetSqlDataRecords7(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords7(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData7);
        record.SetString(ordinal, ServerModelUtility.ToString((object) definitionEnvironment.EnvironmentOptions));
        string str1 = ServerModelUtility.ToString((object) definitionEnvironment.GetPreAndPostApprovalOptionsFromDefinitionEnvironment());
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetGuid(num2 = num1 + 1, definitionEnvironment.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, definitionEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(definitionEnvironment.Name) ? string.Empty : definitionEnvironment.Name);
        int num5;
        record.SetGuid(num5 = num4 + 1, definitionEnvironment.OwnerId);
        int num6;
        record.SetInt32(num6 = num5 + 1, definitionEnvironment.Rank);
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(definitionEnvironment.Variables);
        int num7;
        record.SetString(num7 = num6 + 1, ServerModelUtility.ToString((object) dictionary));
        string str2 = definitionEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Conditions);
        int num8;
        record.SetNullableString(num8 = num7 + 1, str2);
        string str3 = ServerModelUtility.ToString((object) definitionEnvironment.ExecutionPolicy);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrWhiteSpace(str3) ? string.Empty : str3);
        string str4 = definitionEnvironment.Schedules == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Schedules);
        int num10;
        record.SetNullableString(num10 = num9 + 1, str4);
        string str5 = (string) null;
        if (definitionEnvironment.RetentionPolicy != null)
          str5 = JsonConvert.SerializeObject((object) new ShallowEnvironmentRetentionPolicy()
          {
            DaysToKeep = definitionEnvironment.RetentionPolicy.DaysToKeep,
            ReleasesToKeep = definitionEnvironment.RetentionPolicy.ReleasesToKeep
          });
        int num11;
        record.SetNullableString(num11 = num10 + 1, str5);
        bool flag = str5 == null || definitionEnvironment.RetentionPolicy.RetainBuild;
        int num12;
        record.SetBoolean(num12 = num11 + 1, flag);
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable8(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV8", DefinitionEnvironmentTable.GetSqlDataRecords8(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords8(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData8);
        record.SetString(ordinal, ServerModelUtility.ToString((object) definitionEnvironment.EnvironmentOptions));
        string str1 = ServerModelUtility.ToString((object) definitionEnvironment.GetPreAndPostApprovalOptionsFromDefinitionEnvironment());
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetGuid(num2 = num1 + 1, definitionEnvironment.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, definitionEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(definitionEnvironment.Name) ? string.Empty : definitionEnvironment.Name);
        int num5;
        record.SetGuid(num5 = num4 + 1, definitionEnvironment.OwnerId);
        int num6;
        record.SetInt32(num6 = num5 + 1, definitionEnvironment.Rank);
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(definitionEnvironment.Variables);
        int num7;
        record.SetString(num7 = num6 + 1, ServerModelUtility.ToString((object) dictionary));
        string str2 = definitionEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Conditions);
        int num8;
        record.SetNullableString(num8 = num7 + 1, str2);
        string str3 = ServerModelUtility.ToString((object) definitionEnvironment.ExecutionPolicy);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrWhiteSpace(str3) ? string.Empty : str3);
        string str4 = definitionEnvironment.Schedules == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Schedules);
        int num10;
        record.SetNullableString(num10 = num9 + 1, str4);
        string str5 = (string) null;
        if (definitionEnvironment.RetentionPolicy != null)
          str5 = JsonConvert.SerializeObject((object) new ShallowEnvironmentRetentionPolicy()
          {
            DaysToKeep = definitionEnvironment.RetentionPolicy.DaysToKeep,
            ReleasesToKeep = definitionEnvironment.RetentionPolicy.ReleasesToKeep
          });
        int num11;
        record.SetNullableString(num11 = num10 + 1, str5);
        bool flag = str5 == null || definitionEnvironment.RetentionPolicy.RetainBuild;
        int num12;
        record.SetBoolean(num12 = num11 + 1, flag);
        int num13;
        record.SetNullableGuid(num13 = num12 + 1, new Guid?());
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable9(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV9", DefinitionEnvironmentTable.GetSqlDataRecords9(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords9(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData9);
        record.SetString(ordinal, ServerModelUtility.ToString((object) definitionEnvironment.EnvironmentOptions));
        string str1 = ServerModelUtility.ToString((object) definitionEnvironment.GetPreAndPostApprovalOptionsFromDefinitionEnvironment());
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetGuid(num2 = num1 + 1, definitionEnvironment.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, definitionEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(definitionEnvironment.Name) ? string.Empty : definitionEnvironment.Name);
        int num5;
        record.SetGuid(num5 = num4 + 1, definitionEnvironment.OwnerId);
        int num6;
        record.SetInt32(num6 = num5 + 1, definitionEnvironment.Rank);
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(definitionEnvironment.Variables);
        int num7;
        record.SetString(num7 = num6 + 1, ServerModelUtility.ToString((object) dictionary));
        string str2 = definitionEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Conditions);
        int num8;
        record.SetNullableString(num8 = num7 + 1, str2);
        string str3 = ServerModelUtility.ToString((object) definitionEnvironment.ExecutionPolicy);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrWhiteSpace(str3) ? string.Empty : str3);
        string str4 = definitionEnvironment.Schedules == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Schedules);
        int num10;
        record.SetNullableString(num10 = num9 + 1, str4);
        string str5 = (string) null;
        if (definitionEnvironment.RetentionPolicy != null)
          str5 = JsonConvert.SerializeObject((object) new ShallowEnvironmentRetentionPolicy()
          {
            DaysToKeep = definitionEnvironment.RetentionPolicy.DaysToKeep,
            ReleasesToKeep = definitionEnvironment.RetentionPolicy.ReleasesToKeep
          });
        int num11;
        record.SetNullableString(num11 = num10 + 1, str5);
        bool flag = str5 == null || definitionEnvironment.RetentionPolicy.RetainBuild;
        int num12;
        record.SetBoolean(num12 = num11 + 1, flag);
        int num13;
        record.SetNullableGuid(num13 = num12 + 1, new Guid?());
        int num14;
        record.SetNullableString(num14 = num13 + 1, ServerModelUtility.ToString((object) definitionEnvironment.ProcessParameters));
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable10(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV10", DefinitionEnvironmentTable.GetSqlDataRecords10(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords10(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData10);
        record.SetString(ordinal, ServerModelUtility.ToString((object) definitionEnvironment.EnvironmentOptions));
        string str1 = ServerModelUtility.ToString((object) definitionEnvironment.GetPreAndPostApprovalOptionsFromDefinitionEnvironment());
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetGuid(num2 = num1 + 1, definitionEnvironment.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, definitionEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(definitionEnvironment.Name) ? string.Empty : definitionEnvironment.Name);
        int num5;
        record.SetGuid(num5 = num4 + 1, definitionEnvironment.OwnerId);
        int num6;
        record.SetInt32(num6 = num5 + 1, definitionEnvironment.Rank);
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(definitionEnvironment.Variables);
        int num7;
        record.SetString(num7 = num6 + 1, ServerModelUtility.ToString((object) dictionary));
        string str2 = definitionEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Conditions);
        int num8;
        record.SetNullableString(num8 = num7 + 1, str2);
        string str3 = ServerModelUtility.ToString((object) definitionEnvironment.ExecutionPolicy);
        int num9;
        record.SetString(num9 = num8 + 1, string.IsNullOrWhiteSpace(str3) ? string.Empty : str3);
        string str4 = definitionEnvironment.Schedules == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Schedules);
        int num10;
        record.SetNullableString(num10 = num9 + 1, str4);
        string str5 = (string) null;
        if (definitionEnvironment.RetentionPolicy != null)
          str5 = JsonConvert.SerializeObject((object) new ShallowEnvironmentRetentionPolicy()
          {
            DaysToKeep = definitionEnvironment.RetentionPolicy.DaysToKeep,
            ReleasesToKeep = definitionEnvironment.RetentionPolicy.ReleasesToKeep
          });
        int num11;
        record.SetNullableString(num11 = num10 + 1, str5);
        bool flag = str5 == null || definitionEnvironment.RetentionPolicy.RetainBuild;
        int num12;
        record.SetBoolean(num12 = num11 + 1, flag);
        int num13;
        record.SetNullableGuid(num13 = num12 + 1, new Guid?());
        int num14;
        record.SetNullableString(num14 = num13 + 1, ServerModelUtility.ToString((object) definitionEnvironment.ProcessParameters));
        int num15;
        record.SetNullableString(num15 = num14 + 1, ServerModelUtility.ToString((object) definitionEnvironment.GetDefinitionGates()));
        yield return record;
      }
    }

    public static void BindDefinitionEnvironmentTable11(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_DefinitionEnvironmentTableV11", DefinitionEnvironmentTable.GetSqlDataRecords11(definitionEnvironments));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlDataRecords11(
      IEnumerable<DefinitionEnvironment> rows)
    {
      rows = rows ?? Enumerable.Empty<DefinitionEnvironment>();
      foreach (DefinitionEnvironment definitionEnvironment in rows.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(DefinitionEnvironmentTable.SqlMetaData11);
        record.SetString(ordinal, ServerModelUtility.ToString((object) definitionEnvironment.EnvironmentOptions));
        string str1 = ServerModelUtility.ToString((object) definitionEnvironment.GetPreAndPostApprovalOptionsFromDefinitionEnvironment());
        int num1;
        record.SetString(num1 = ordinal + 1, str1);
        int num2;
        record.SetGuid(num2 = num1 + 1, definitionEnvironment.GuidId);
        int num3;
        record.SetInt32(num3 = num2 + 1, definitionEnvironment.Id);
        int num4;
        record.SetString(num4 = num3 + 1, string.IsNullOrEmpty(definitionEnvironment.Name) ? string.Empty : definitionEnvironment.Name);
        int num5;
        record.SetGuid(num5 = num4 + 1, definitionEnvironment.OwnerId);
        int num6;
        record.SetInt32(num6 = num5 + 1, definitionEnvironment.Rank);
        IDictionary<string, ConfigurationVariableValue> dictionary = VariablesUtility.ReplaceSecretVariablesWithNull(definitionEnvironment.Variables);
        int num7;
        record.SetString(num7 = num6 + 1, ServerModelUtility.ToString((object) dictionary));
        int num8;
        record.SetString(num8 = num7 + 1, ServerModelUtility.ToString((object) definitionEnvironment.VariableGroups));
        string str2 = definitionEnvironment.Conditions == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Conditions);
        int num9;
        record.SetNullableString(num9 = num8 + 1, str2);
        string str3 = ServerModelUtility.ToString((object) definitionEnvironment.ExecutionPolicy);
        int num10;
        record.SetString(num10 = num9 + 1, string.IsNullOrWhiteSpace(str3) ? string.Empty : str3);
        string str4 = definitionEnvironment.Schedules == null ? (string) null : JsonConvert.SerializeObject((object) definitionEnvironment.Schedules);
        int num11;
        record.SetNullableString(num11 = num10 + 1, str4);
        string str5 = (string) null;
        if (definitionEnvironment.RetentionPolicy != null)
          str5 = JsonConvert.SerializeObject((object) new ShallowEnvironmentRetentionPolicy()
          {
            DaysToKeep = definitionEnvironment.RetentionPolicy.DaysToKeep,
            ReleasesToKeep = definitionEnvironment.RetentionPolicy.ReleasesToKeep
          });
        int num12;
        record.SetNullableString(num12 = num11 + 1, str5);
        bool flag = str5 == null || definitionEnvironment.RetentionPolicy.RetainBuild;
        int num13;
        record.SetBoolean(num13 = num12 + 1, flag);
        int num14;
        record.SetNullableGuid(num14 = num13 + 1, new Guid?());
        int num15;
        record.SetNullableString(num15 = num14 + 1, ServerModelUtility.ToString((object) definitionEnvironment.ProcessParameters));
        int num16;
        record.SetNullableString(num16 = num15 + 1, ServerModelUtility.ToString((object) definitionEnvironment.GetDefinitionGates()));
        yield return record;
      }
    }
  }
}
