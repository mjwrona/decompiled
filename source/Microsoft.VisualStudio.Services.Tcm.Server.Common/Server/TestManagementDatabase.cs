// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  public class TestManagementDatabase : 
    TeamFoundationSqlResourceComponent,
    IBuildReferenceDatabase,
    ITestExtensibilityDatabase,
    IPlannedTestMetadataDatabase,
    ITraceabilityDatabase,
    IReleaseReferenceDatabase,
    IResultRetentionSettingsDatabase,
    ITestReportsDatabase
  {
    private static readonly SqlMetaData[] typ_ReleaseRef2Table = new SqlMetaData[12]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("ReleaseRefId", SqlDbType.Int),
      new SqlMetaData("ReleaseUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseEnvUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvId", SqlDbType.Int),
      new SqlMetaData("ReleaseDefId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvDefId", SqlDbType.Int),
      new SqlMetaData("Attempt", SqlDbType.Int),
      new SqlMetaData("ReleaseName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ReleaseCreationDate", SqlDbType.DateTime2),
      new SqlMetaData("EnvironmentCreationDate", SqlDbType.DateTime2)
    };
    private static readonly SqlMetaData[] typ_BuildRef3Table = new SqlMetaData[16]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("BuildConfigurationId", SqlDbType.Int),
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildPlatform", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildFlavor", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("SourceVersion", SqlDbType.NVarChar, 326L),
      new SqlMetaData("BuildSystem", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CoverageId", SqlDbType.Int),
      new SqlMetaData("BuildDeleted", SqlDbType.Bit),
      new SqlMetaData("RepoId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RepoType", SqlDbType.NVarChar, 40L)
    };
    private static readonly SqlMetaData[] typ_TestRunContext2Table = new SqlMetaData[5]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunContextId", SqlDbType.Int),
      new SqlMetaData("BuildRefId", SqlDbType.Int),
      new SqlMetaData("ReleaseRefId", SqlDbType.Int),
      new SqlMetaData("SourceWorkflow", SqlDbType.NVarChar, 128L)
    };
    private static readonly SqlMetaData[] typ_TestCaseReference2Table = new SqlMetaData[19]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestCaseId", SqlDbType.Int),
      new SqlMetaData("TestPointId", SqlDbType.Int),
      new SqlMetaData("ConfigurationId", SqlDbType.Int),
      new SqlMetaData("TestCaseRefId", SqlDbType.Int),
      new SqlMetaData("AutomatedTestName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("AutomatedTestStorage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("AutomatedTestType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("AutomatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("TestCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TestCaseRevision", SqlDbType.Int),
      new SqlMetaData("Priority", SqlDbType.TinyInt),
      new SqlMetaData("Owner", SqlDbType.NVarChar, 256L),
      new SqlMetaData("AreaId", SqlDbType.Int),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LastRefTestRunDate", SqlDbType.DateTime),
      new SqlMetaData("AutomatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("AutomatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] typ_TestResult2Table2 = new SqlMetaData[20]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestCaseRefId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("Outcome", SqlDbType.TinyInt),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("DateStarted", SqlDbType.DateTime),
      new SqlMetaData("DateCompleted", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("RunBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ComputerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("FailureType", SqlDbType.TinyInt),
      new SqlMetaData("ResolutionStateId", SqlDbType.Int),
      new SqlMetaData("Owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ResetCount", SqlDbType.Int),
      new SqlMetaData("AfnStripId", SqlDbType.Int),
      new SqlMetaData("EffectivePointState", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_TestMessageLogEntry2Table = new SqlMetaData[6]
    {
      new SqlMetaData("TestMessageLogId", SqlDbType.Int),
      new SqlMetaData("EntryId", SqlDbType.Int),
      new SqlMetaData("LogUser", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DateCreated", SqlDbType.DateTime),
      new SqlMetaData("LogLevel", SqlDbType.TinyInt),
      new SqlMetaData("Message", SqlDbType.NVarChar, 2048L)
    };
    private static readonly SqlMetaData[] typ_TestRun3Table2 = new SqlMetaData[42]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("Title", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("Owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("IncompleteTests", SqlDbType.Int),
      new SqlMetaData("TestPlanId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 260L),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ErrorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("StartDate", SqlDbType.DateTime),
      new SqlMetaData("CompleteDate", SqlDbType.DateTime),
      new SqlMetaData("PostProcessState", SqlDbType.TinyInt),
      new SqlMetaData("DueDate", SqlDbType.DateTime),
      new SqlMetaData("Controller", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TestMessageLogId", SqlDbType.Int),
      new SqlMetaData("LegacySharePath", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("TestSettingsId", SqlDbType.Int),
      new SqlMetaData("BuildConfigurationId", SqlDbType.Int),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Type", SqlDbType.TinyInt),
      new SqlMetaData("CoverageId", SqlDbType.Int),
      new SqlMetaData("IsAutomated", SqlDbType.Bit),
      new SqlMetaData("TestEnvironmentId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Version", SqlDbType.Int),
      new SqlMetaData("PublicTestSettingsId", SqlDbType.Int),
      new SqlMetaData("IsBvt", SqlDbType.Bit),
      new SqlMetaData("Comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("TotalTests", SqlDbType.Int),
      new SqlMetaData("PassedTests", SqlDbType.Int),
      new SqlMetaData("NotApplicableTests", SqlDbType.Int),
      new SqlMetaData("UnanalyzedTests", SqlDbType.Int),
      new SqlMetaData("IsMigrated", SqlDbType.Bit),
      new SqlMetaData("ReleaseUri", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("ReleaseEnvironmentUri", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("TestRunContextId", SqlDbType.Int),
      new SqlMetaData("MaxReservedResultId", SqlDbType.Int),
      new SqlMetaData("DeletedOn", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_TestRunSummary2Table = new SqlMetaData[10]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunStatsId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestRunContextId", SqlDbType.Int),
      new SqlMetaData("TestRunCompletedDate", SqlDbType.DateTime),
      new SqlMetaData("TestOutcome", SqlDbType.TinyInt),
      new SqlMetaData("ResultCount", SqlDbType.Int),
      new SqlMetaData("ResultDuration", SqlDbType.BigInt),
      new SqlMetaData("RunDuration", SqlDbType.BigInt),
      new SqlMetaData("IsRerun", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_TestRunSummary3Table = new SqlMetaData[10]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunStatsId", SqlDbType.BigInt),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestRunContextId", SqlDbType.Int),
      new SqlMetaData("TestRunCompletedDate", SqlDbType.DateTime),
      new SqlMetaData("TestOutcome", SqlDbType.TinyInt),
      new SqlMetaData("ResultCount", SqlDbType.Int),
      new SqlMetaData("ResultDuration", SqlDbType.BigInt),
      new SqlMetaData("RunDuration", SqlDbType.BigInt),
      new SqlMetaData("IsRerun", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_TestResultsEx2Table2 = new SqlMetaData[11]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("IntValue", SqlDbType.Int),
      new SqlMetaData("FloatValue", SqlDbType.Float),
      new SqlMetaData("BitValue", SqlDbType.Bit),
      new SqlMetaData("DateTimeValue", SqlDbType.DateTime),
      new SqlMetaData("GuidValue", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StringValue", SqlDbType.NVarChar, 4000L)
    };
    private static readonly SqlMetaData[] typ_TestCaseMetadata2Table = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestMetadataId", SqlDbType.Int),
      new SqlMetaData("Container", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 1024L)
    };
    private static readonly SqlMetaData[] typ_RequirementsToTestsMapping2Table = new SqlMetaData[8]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("WorkItemId", SqlDbType.Int),
      new SqlMetaData("TestMetadataId", SqlDbType.Int),
      new SqlMetaData("CreationDate", SqlDbType.DateTime2),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DeletionDate", SqlDbType.DateTime2),
      new SqlMetaData("DeletedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsMigratedToWIT", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_TestResultReset2Table2 = new SqlMetaData[7]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("DateModified", SqlDbType.DateTime),
      new SqlMetaData("AuditIdentity", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TestResultRV", SqlDbType.Binary, 8L)
    };
    private static readonly SqlMetaData[] typ_TestActionResult2Table = new SqlMetaData[14]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("Outcome", SqlDbType.TinyInt),
      new SqlMetaData("ErrorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("DateStarted", SqlDbType.DateTime),
      new SqlMetaData("DateCompleted", SqlDbType.DateTime),
      new SqlMetaData("Duration", SqlDbType.BigInt),
      new SqlMetaData("SharedStepId", SqlDbType.Int),
      new SqlMetaData("SharedStepRevision", SqlDbType.Int),
      new SqlMetaData("Comment", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_TestRunEx2Table = new SqlMetaData[10]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("IntValue", SqlDbType.Int),
      new SqlMetaData("FloatValue", SqlDbType.Float),
      new SqlMetaData("BitValue", SqlDbType.Bit),
      new SqlMetaData("DateTimeValue", SqlDbType.DateTime),
      new SqlMetaData("GuidValue", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StringValue", SqlDbType.NVarChar, 4000L)
    };
    private static readonly SqlMetaData[] typ_TestRunExtended2Table = new SqlMetaData[10]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("Substate", SqlDbType.TinyInt),
      new SqlMetaData("SourceFilter", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("TestCaseFilter", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("TestEnvironmentUrl", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("AutEnvironmentUrl", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("CsmContent", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("CsmParameters", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("CsmContent", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_TestParameter2Table = new SqlMetaData[10]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L),
      new SqlMetaData("ParameterName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("DateModified", SqlDbType.DateTime),
      new SqlMetaData("DataType", SqlDbType.TinyInt),
      new SqlMetaData("Expected", SqlDbType.VarBinary, -1L),
      new SqlMetaData("Actual", SqlDbType.VarBinary, -1L)
    };
    private static readonly SqlMetaData[] typ_Coverage2Table = new SqlMetaData[5]
    {
      new SqlMetaData("CoverageId", SqlDbType.Int),
      new SqlMetaData("DataCreated", SqlDbType.DateTime),
      new SqlMetaData("DateModified", SqlDbType.DateTime),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("LastError", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_CodeCoverageSummary2Table = new SqlMetaData[6]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("BuildConfigurationId", SqlDbType.Int),
      new SqlMetaData("Label", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Position", SqlDbType.Int),
      new SqlMetaData("Total", SqlDbType.Int),
      new SqlMetaData("Covered", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ModuleCoverage2Table = new SqlMetaData[14]
    {
      new SqlMetaData("CoverageId", SqlDbType.Int),
      new SqlMetaData("ModuleId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Signature", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SignatureAge", SqlDbType.Int),
      new SqlMetaData("LinesCovered", SqlDbType.Int),
      new SqlMetaData("LinesPartiallyCovered", SqlDbType.Int),
      new SqlMetaData("LinesNotCovered", SqlDbType.Int),
      new SqlMetaData("BlocksCovered", SqlDbType.Int),
      new SqlMetaData("BlocksNotCovered", SqlDbType.Int),
      new SqlMetaData("BlockCount", SqlDbType.Int),
      new SqlMetaData("BlockDataLength", SqlDbType.Int),
      new SqlMetaData("BlockData", SqlDbType.VarBinary, -1L),
      new SqlMetaData("CoverageFileUrl", SqlDbType.NVarChar, 512L)
    };
    private static readonly SqlMetaData[] typ_FunctionCoverage2Table = new SqlMetaData[12]
    {
      new SqlMetaData("CoverageId", SqlDbType.Int),
      new SqlMetaData("ModuleId", SqlDbType.Int),
      new SqlMetaData("FunctionId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SourceFile", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Class", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Namespace", SqlDbType.NVarChar, 256L),
      new SqlMetaData("LinesCovered", SqlDbType.Int),
      new SqlMetaData("LinesPartiallyCovered", SqlDbType.Int),
      new SqlMetaData("LinesNotCovered", SqlDbType.Int),
      new SqlMetaData("BlocksCovered", SqlDbType.Int),
      new SqlMetaData("BlocksNotCovered", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TCMPropertyBag2Table = new SqlMetaData[4]
    {
      new SqlMetaData("ArtifactType", SqlDbType.Int),
      new SqlMetaData("ArtifactId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Value", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_PointResults2Table = new SqlMetaData[10]
    {
      new SqlMetaData("PointId", SqlDbType.Int),
      new SqlMetaData("PlanId", SqlDbType.Int),
      new SqlMetaData("ChangeNumber", SqlDbType.Int),
      new SqlMetaData("LastTestRunId", SqlDbType.Int),
      new SqlMetaData("LastTestResultId", SqlDbType.Int),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastResultState", SqlDbType.TinyInt),
      new SqlMetaData("LastResultOutcome", SqlDbType.TinyInt),
      new SqlMetaData("LastResolutionStateId", SqlDbType.Int),
      new SqlMetaData("LastFailureType", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_PointReference2Table = new SqlMetaData[2]
    {
      new SqlMetaData("PointId", SqlDbType.Int),
      new SqlMetaData("PlanId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestCaseResultTypeTable = new SqlMetaData[14]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_TestCaseResult2TypeTable = new SqlMetaData[14]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_TestCaseResult3TypeTable = new SqlMetaData[26]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResultTypeTable = new SqlMetaData[27]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResult2TypeTable = new SqlMetaData[15]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("testCaseRefId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResult3TypeTable = new SqlMetaData[29]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResult4TypeTable = new SqlMetaData[29]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResult5TypeTable = new SqlMetaData[29]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseAreaId", SqlDbType.Int),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_PlannedMetadataIdTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("testPlanId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_AutomatedTestDetailsTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] typ_TestCaseResultForUpdateTypeTable = new SqlMetaData[17]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("pointId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("revision", SqlDbType.Int),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_TestCaseResultUpdateTypeTable = new SqlMetaData[18]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("pointId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("revision", SqlDbType.Int),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_TestCaseResultUpdate2TypeTable = new SqlMetaData[22]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("pointId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("revision", SqlDbType.Int),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResultUpdate3TypeTable = new SqlMetaData[25]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("pointId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("revision", SqlDbType.Int),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResultUpdateTypeTable = new SqlMetaData[23]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("pointId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("revision", SqlDbType.Int),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] TestResult_TestCaseResultFieldValueHashTypeTable = new SqlMetaData[5]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("subResultId", SqlDbType.Int),
      new SqlMetaData("fieldId", SqlDbType.Int),
      new SqlMetaData("fieldValueHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResultUpdate2TypeTable = new SqlMetaData[25]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("pointId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("revision", SqlDbType.Int),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseResultUpdate2_2TypeTable = new SqlMetaData[25]
    {
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("pointId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("state", SqlDbType.TinyInt),
      new SqlMetaData("outcome", SqlDbType.TinyInt),
      new SqlMetaData("resolutionStateId", SqlDbType.Int),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("errorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("computerName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("dateStarted", SqlDbType.DateTime),
      new SqlMetaData("dateCompleted", SqlDbType.DateTime),
      new SqlMetaData("duration", SqlDbType.BigInt),
      new SqlMetaData("revision", SqlDbType.Int),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("runBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("failureType", SqlDbType.TinyInt),
      new SqlMetaData("automatedTestTypeId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("ownerName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] typ_TestResultAttachmentTypeTable = new SqlMetaData[9]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("iterationId", SqlDbType.Int),
      new SqlMetaData("actionPath", SqlDbType.VarChar, 256L),
      new SqlMetaData("fileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("attachmentType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("tmiRunId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("sessionId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestResultAttachmentTypeTable2 = new SqlMetaData[10]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("iterationId", SqlDbType.Int),
      new SqlMetaData("actionPath", SqlDbType.VarChar, 256L),
      new SqlMetaData("fileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("attachmentType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("tmiRunId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("sessionId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestResultAttachmentTypeTable3 = new SqlMetaData[11]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("iterationId", SqlDbType.Int),
      new SqlMetaData("actionPath", SqlDbType.VarChar, 256L),
      new SqlMetaData("subResultId", SqlDbType.Int),
      new SqlMetaData("fileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("attachmentType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("tmiRunId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("sessionId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestResultAttachmentTypeTable4 = new SqlMetaData[14]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("iterationId", SqlDbType.Int),
      new SqlMetaData("actionPath", SqlDbType.VarChar, 256L),
      new SqlMetaData("subResultId", SqlDbType.Int),
      new SqlMetaData("tfsFileId", SqlDbType.Int),
      new SqlMetaData("fileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("attachmentType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("tmiRunId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("sessionId", SqlDbType.Int),
      new SqlMetaData("isComplete", SqlDbType.Bit),
      new SqlMetaData("uncompressedLength", SqlDbType.BigInt),
      new SqlMetaData("Id", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestResultAttachmentTypeTable5 = new SqlMetaData[9]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("SubResultId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("FileName", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("AttachmentType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("IsComplete", SqlDbType.Bit),
      new SqlMetaData("UncompressedLength", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_LogStoreAttachmentTable = new SqlMetaData[5]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("subResultId", SqlDbType.Int),
      new SqlMetaData("fileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("attachmentType", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_LogStoreAttachmentMappingTable = new SqlMetaData[6]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("subResultId", SqlDbType.Int),
      new SqlMetaData("fileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("attachmentType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("UncompressedLength", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_LogStoreAttachmentMappingV2Table = new SqlMetaData[8]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("subResultId", SqlDbType.Int),
      new SqlMetaData("fileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("attachmentType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("UncompressedLength", SqlDbType.BigInt),
      new SqlMetaData("iterationId", SqlDbType.Int),
      new SqlMetaData("actionPath", SqlDbType.VarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_TestResultAttachmentIdentityTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("attachmentId", SqlDbType.Int),
      new SqlMetaData("sessionId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_NameValuePairTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Value", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_TestMessageLogEntryTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("LogUser", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DateCreated", SqlDbType.DateTime),
      new SqlMetaData("LogLevel", SqlDbType.TinyInt),
      new SqlMetaData("Message", SqlDbType.NVarChar, 2048L)
    };
    private static readonly SqlMetaData[] typ_IdTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairInt32DateTimeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] TestManagement_typ_TinyIntTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("ByteValue", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_IdPairTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("SourceId", SqlDbType.Int),
      new SqlMetaData("TargetId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_IdToTitleTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Title", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_IdAndRevTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Revision", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_UpdatedPropertiesTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_PropertyValuePairTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("PropertyId", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_IntStringPairTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Number", SqlDbType.Int),
      new SqlMetaData("Data", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_TestRunSummaryByOutcomeTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("TestOutcome", SqlDbType.TinyInt),
      new SqlMetaData("ResultCount", SqlDbType.Int),
      new SqlMetaData("Duration", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_NameTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_TestCaseResultWorkItemLinkTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("WorkItemUri", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_TestCaseResultIdAndRevTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("Revision", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestActionResultTypeTable = new SqlMetaData[11]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L),
      new SqlMetaData("Outcome", SqlDbType.TinyInt),
      new SqlMetaData("ErrorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("DateStarted", SqlDbType.DateTime),
      new SqlMetaData("DateCompleted", SqlDbType.DateTime),
      new SqlMetaData("Duration", SqlDbType.BigInt),
      new SqlMetaData("SharedStepId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestActionResultUpdate2TypeTable = new SqlMetaData[12]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L),
      new SqlMetaData("Outcome", SqlDbType.TinyInt),
      new SqlMetaData("ErrorMessage", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("DateStarted", SqlDbType.DateTime),
      new SqlMetaData("DateCompleted", SqlDbType.DateTime),
      new SqlMetaData("Duration", SqlDbType.BigInt),
      new SqlMetaData("SharedStepId", SqlDbType.Int),
      new SqlMetaData("SharedStepRevision", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestActionResultUpdateTypeTable = new SqlMetaData[12]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L),
      new SqlMetaData("Outcome", SqlDbType.TinyInt),
      new SqlMetaData("ErrorMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Comment", SqlDbType.NVarChar, -1L),
      new SqlMetaData("DateStarted", SqlDbType.DateTime),
      new SqlMetaData("DateCompleted", SqlDbType.DateTime),
      new SqlMetaData("Duration", SqlDbType.BigInt),
      new SqlMetaData("SharedStepId", SqlDbType.Int),
      new SqlMetaData("SharedStepRevision", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_BranchToFlakinessStateTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("IsFlaky", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_TestActionResultForDeleteTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L)
    };
    private static readonly SqlMetaData[] typ_TestResultParameterTypeTable = new SqlMetaData[8]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L),
      new SqlMetaData("ParameterName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("DataType", SqlDbType.TinyInt),
      new SqlMetaData("Expected", SqlDbType.VarBinary, -1L),
      new SqlMetaData("Actual", SqlDbType.VarBinary, -1L)
    };
    private static readonly SqlMetaData[] typ_TestResultParameterForDeleteTypeTable = new SqlMetaData[5]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("IterationId", SqlDbType.Int),
      new SqlMetaData("ActionPath", SqlDbType.VarChar, 512L),
      new SqlMetaData("ParameterName", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_BuildRefTable = new SqlMetaData[10]
    {
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildPlatform", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildFlavor", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("RepositoryId", SqlDbType.Int),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("SourceVersion", SqlDbType.NVarChar, 326L)
    };
    private static readonly SqlMetaData[] typ_BuildRefTable2 = new SqlMetaData[11]
    {
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildPlatform", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildFlavor", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("RepositoryId", SqlDbType.Int),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("SourceVersion", SqlDbType.NVarChar, 326L),
      new SqlMetaData("BuildSystem", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_BuildRefTable3 = new SqlMetaData[12]
    {
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildPlatform", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildFlavor", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("SourceVersion", SqlDbType.NVarChar, 326L),
      new SqlMetaData("BuildSystem", SqlDbType.NVarChar, 256L),
      new SqlMetaData("RepoId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RepoType", SqlDbType.NVarChar, 40L)
    };
    private static readonly SqlMetaData[] typ_BuildRefTable4 = new SqlMetaData[13]
    {
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildPlatform", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildFlavor", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDefinitionId", SqlDbType.Int),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("SourceVersion", SqlDbType.NVarChar, 326L),
      new SqlMetaData("BuildSystem", SqlDbType.NVarChar, 256L),
      new SqlMetaData("RepoId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RepoType", SqlDbType.NVarChar, 40L),
      new SqlMetaData("TargetBranchName", SqlDbType.NVarChar, 400L)
    };
    private static readonly SqlMetaData[] typ_PipelineRefTable = new SqlMetaData[6]
    {
      new SqlMetaData("StageName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("StageAttempt", SqlDbType.Int),
      new SqlMetaData("PhaseName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("PhaseAttempt", SqlDbType.Int),
      new SqlMetaData("JobName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("JobAttempt", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_BranchNameTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L)
    };
    private static readonly SqlMetaData[] typ_BuildUriToDeletionState = new SqlMetaData[2]
    {
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDeleted", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_ReleaseRefTable = new SqlMetaData[2]
    {
      new SqlMetaData("ReleaseUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseEnvUri", SqlDbType.NVarChar, 2048L)
    };
    private static readonly SqlMetaData[] typ_ReleaseRefTable2 = new SqlMetaData[7]
    {
      new SqlMetaData("ReleaseUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseEnvUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvId", SqlDbType.Int),
      new SqlMetaData("ReleaseDefId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvDefId", SqlDbType.Int),
      new SqlMetaData("Attempt", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ReleaseRefTable3 = new SqlMetaData[8]
    {
      new SqlMetaData("ReleaseUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseEnvUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvId", SqlDbType.Int),
      new SqlMetaData("ReleaseDefId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvDefId", SqlDbType.Int),
      new SqlMetaData("Attempt", SqlDbType.Int),
      new SqlMetaData("ReleaseName", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_ReleaseRefTable4 = new SqlMetaData[10]
    {
      new SqlMetaData("ReleaseUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseEnvUri", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvId", SqlDbType.Int),
      new SqlMetaData("ReleaseDefId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvDefId", SqlDbType.Int),
      new SqlMetaData("Attempt", SqlDbType.Int),
      new SqlMetaData("ReleaseName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ReleaseCreationDate", SqlDbType.DateTime2),
      new SqlMetaData("EnvironmentCreationDate", SqlDbType.DateTime2)
    };
    private static readonly SqlMetaData[] typ_TestExtensionFieldValuesTable = new SqlMetaData[9]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("FieldId", SqlDbType.Int),
      new SqlMetaData("IntValue", SqlDbType.Int),
      new SqlMetaData("FloatValue", SqlDbType.Float),
      new SqlMetaData("DateTimeValue", SqlDbType.DateTime),
      new SqlMetaData("GuidValue", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BitValue", SqlDbType.Bit),
      new SqlMetaData("StringValue", SqlDbType.NVarChar, 4000L)
    };
    private static readonly SqlMetaData[] typ_TestExtensionFieldsTable = new SqlMetaData[5]
    {
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("FieldType", SqlDbType.TinyInt),
      new SqlMetaData("IsRunScoped", SqlDbType.Bit),
      new SqlMetaData("IsResultScoped", SqlDbType.Bit),
      new SqlMetaData("IsSystemField", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_TestMethodTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Container", SqlDbType.NVarChar, 1024L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseReferenceTypeTable = new SqlMetaData[13]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.NVarChar, 256L),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] Typ_Int32TypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] Typ_Int64TypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] TestResult_typ_FailingSinceDetailsTable = new SqlMetaData[3]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("failingSince", SqlDbType.NVarChar, 4000L)
    };
    private static readonly SqlMetaData[] typ_TestResultIdentifierRecordTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("TestCaseRefId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] TestResult_typ_FailureCountForTestRunTypeTable = new SqlMetaData[5]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("NewFailures", SqlDbType.Int),
      new SqlMetaData("NewFailedResults", SqlDbType.VarChar, SqlMetaData.Max),
      new SqlMetaData("ExistingFailures", SqlDbType.Int),
      new SqlMetaData("ExistingFailedResults", SqlDbType.VarChar, SqlMetaData.Max)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseReference2TypeTable = new SqlMetaData[15]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.NVarChar, 256L),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseReference3TypeTable = new SqlMetaData[15]
    {
      new SqlMetaData("orderIndex", SqlDbType.Int),
      new SqlMetaData("testCaseId", SqlDbType.Int),
      new SqlMetaData("testPointId", SqlDbType.Int),
      new SqlMetaData("configurationId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.NVarChar, 256L),
      new SqlMetaData("priority", SqlDbType.TinyInt),
      new SqlMetaData("testCaseTitle", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseArea", SqlDbType.NVarChar, 256L),
      new SqlMetaData("testCaseRevision", SqlDbType.Int),
      new SqlMetaData("automatedTestName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestStorage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("automatedTestType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("automatedTestId", SqlDbType.NVarChar, 64L),
      new SqlMetaData("automatedTestNameHash", SqlDbType.VarBinary, 32L),
      new SqlMetaData("automatedTestStorageHash", SqlDbType.VarBinary, 32L)
    };
    private static readonly SqlMetaData[] TestResult_typ_TestCaseReferenceUpdatableFieldsTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("testRunId", SqlDbType.Int),
      new SqlMetaData("testResultId", SqlDbType.Int),
      new SqlMetaData("owner", SqlDbType.NVarChar, 256L),
      new SqlMetaData("priority", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_TestSettingsTypeTable = new SqlMetaData[13]
    {
      new SqlMetaData("SettingsId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("Settings", SqlDbType.Xml),
      new SqlMetaData("AreaId", SqlDbType.Int),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("IsPublic", SqlDbType.Bit),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsAutomated", SqlDbType.Bit),
      new SqlMetaData("MachineRoles", SqlDbType.Xml)
    };
    private static readonly SqlMetaData[] typ_ModuleCoverageTypeTable = new SqlMetaData[12]
    {
      new SqlMetaData("ModuleId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Signature", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SignatureAge", SqlDbType.Int),
      new SqlMetaData("LinesCovered", SqlDbType.Int),
      new SqlMetaData("LinesPartiallyCovered", SqlDbType.Int),
      new SqlMetaData("LinesNotCovered", SqlDbType.Int),
      new SqlMetaData("BlocksCovered", SqlDbType.Int),
      new SqlMetaData("BlocksNotCovered", SqlDbType.Int),
      new SqlMetaData("BlockCount", SqlDbType.Int),
      new SqlMetaData("BlockDataLength", SqlDbType.Int),
      new SqlMetaData("BlockData", SqlDbType.VarBinary, -1L)
    };
    private static readonly SqlMetaData[] typ_ModuleCoverageTypeTable1 = new SqlMetaData[13]
    {
      new SqlMetaData("ModuleId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Signature", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SignatureAge", SqlDbType.Int),
      new SqlMetaData("LinesCovered", SqlDbType.Int),
      new SqlMetaData("LinesPartiallyCovered", SqlDbType.Int),
      new SqlMetaData("LinesNotCovered", SqlDbType.Int),
      new SqlMetaData("BlocksCovered", SqlDbType.Int),
      new SqlMetaData("BlocksNotCovered", SqlDbType.Int),
      new SqlMetaData("BlockCount", SqlDbType.Int),
      new SqlMetaData("BlockDataLength", SqlDbType.Int),
      new SqlMetaData("BlockData", SqlDbType.VarBinary, -1L),
      new SqlMetaData("CoverageFileUrl", SqlDbType.NVarChar, 512L)
    };
    private static readonly SqlMetaData[] typ_FunctionCoverageTypeTable = new SqlMetaData[11]
    {
      new SqlMetaData("ModuleId", SqlDbType.Int),
      new SqlMetaData("FunctionId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SourceFile", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Class", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Namespace", SqlDbType.NVarChar, 256L),
      new SqlMetaData("LinesCovered", SqlDbType.Int),
      new SqlMetaData("LinesPartiallyCovered", SqlDbType.Int),
      new SqlMetaData("LinesNotCovered", SqlDbType.Int),
      new SqlMetaData("BlocksCovered", SqlDbType.Int),
      new SqlMetaData("BlocksNotCovered", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_CodeCoverageSummaryTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("Label", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Position", SqlDbType.Int),
      new SqlMetaData("Total", SqlDbType.Int),
      new SqlMetaData("Covered", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestAuthoringDetailsTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("PointId", SqlDbType.Int),
      new SqlMetaData("SuiteId", SqlDbType.Int),
      new SqlMetaData("ConfigurationId", SqlDbType.Int),
      new SqlMetaData("TesterId", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_TestAuthoringDetails2TypeTable = new SqlMetaData[9]
    {
      new SqlMetaData("PointId", SqlDbType.Int),
      new SqlMetaData("SuiteId", SqlDbType.Int),
      new SqlMetaData("ConfigurationId", SqlDbType.Int),
      new SqlMetaData("TesterId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("Priority", SqlDbType.TinyInt),
      new SqlMetaData("RunBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsAutomated", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_TestExtentionFieldIdArrayTable = new SqlMetaData[1]
    {
      new SqlMetaData("FieldId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestLinksTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("EntryType", SqlDbType.TinyInt),
      new SqlMetaData("LinkType", SqlDbType.TinyInt),
      new SqlMetaData("LinkUrl", SqlDbType.NVarChar, 512L)
    };
    private static readonly SqlMetaData[] typ_SessionTimelineTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("EntryType", SqlDbType.TinyInt),
      new SqlMetaData("TimelineType", SqlDbType.TinyInt),
      new SqlMetaData("TimelineDisplay", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("TimestampUTC", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_GroupLayoutTypeTable = new SqlMetaData[5]
    {
      new SqlMetaData("Type", SqlDbType.NVarChar, 20L),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Uid", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StartTime", SqlDbType.DateTime),
      new SqlMetaData("EndTime", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_JobLayoutTypeTable = new SqlMetaData[6]
    {
      new SqlMetaData("ParentUid", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Type", SqlDbType.NVarChar, 20L),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Uid", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StartTime", SqlDbType.DateTime),
      new SqlMetaData("EndTime", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_LayoutPropertiesTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("LayoutUid", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PropertyName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("PropertyValue", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    private static readonly SqlMetaData[] typ_TestResultDimensionTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 50L),
      new SqlMetaData("Value", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    private static readonly SqlMetaData[] typ_FailureBucketMRXTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("BucketUid", SqlDbType.NVarChar, 128L),
      new SqlMetaData("BucketingSystem", SqlDbType.NVarChar, 64L)
    };
    private static readonly SqlMetaData[] typ_TestResultExMRXTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("IsSystemIssue", SqlDbType.Bit),
      new SqlMetaData("ExceptionType", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    private static readonly SqlMetaData[] typ_TestResultOneMRXTypeTable = new SqlMetaData[8]
    {
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("TestRunId", SqlDbType.Int),
      new SqlMetaData("ExecutionNumber", SqlDbType.Int),
      new SqlMetaData("Attempt", SqlDbType.Int),
      new SqlMetaData("Locale", SqlDbType.NVarChar, 64L),
      new SqlMetaData("BuildType", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("TestPhase", SqlDbType.TinyInt),
      new SqlMetaData("TopologyId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_LinkResultTypeTable = new SqlMetaData[6]
    {
      new SqlMetaData("TestResultId", SqlDbType.Int),
      new SqlMetaData("ParentType", SqlDbType.TinyInt),
      new SqlMetaData("LinkType", SqlDbType.TinyInt),
      new SqlMetaData("LinkUrl", SqlDbType.NVarChar, 512L),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("OperationType", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_TestSession_TestRunsTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("TestRunId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_EnvironmentTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("ProcessorArchitecture", SqlDbType.NVarChar, 10L),
      new SqlMetaData("ConfigurationName", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[107]
    {
      (IComponentCreator) new ComponentCreator<TestManagementDatabase>(32),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase2>(33),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase3>(34),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase4>(35),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase5>(36),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase6>(37),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase7>(38),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase8>(39),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase9>(40),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase10>(41),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase11>(42),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase12>(43),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase13>(44),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase14>(45),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase15>(46),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase16>(47),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase17>(48),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase18>(49),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase19>(50),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase20>(51),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase21>(52),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase22>(53),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase23>(54),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase24>(55),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase24>(56),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase24>(57),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase25>(58),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase26>(59),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase27>(60),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase28>(61),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase29>(62),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase29>(63),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase30>(64),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase31>(65),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase32>(66),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase33>(67),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase34>(68),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase35>(69),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase36>(70),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase37>(71),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase37>(72),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase38>(73),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase39>(74),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase40>(75),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase41>(76),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase41_1>(77),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase42>(78),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase43>(79),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase44>(80),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase45>(81),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase46>(82),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase47>(83),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase48>(84),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase49>(85),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase50>(86),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase51>(87),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase52>(88),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase53>(89),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase54>(90),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase55>(91),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase56>(92),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase57>(93),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase58>(94),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase59>(95),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase60>(96),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase61>(97),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase62>(98),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase63>(99),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase64>(100),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase65>(101),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase66>(102),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase67>(103),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase68>(104),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase69>(105),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase70>(106),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase71>(107),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase72>(108),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase73>(109),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase74>(110),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase75>(111),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase76>(112),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase77>(113),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase78>(114),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase79>(115),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase80>(116),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase81>(117),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase82>(118),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase83>(119),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase84>(120),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase85>(121),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase86>(122),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase87>(123),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase88>(124),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase89>(125),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase90>(126),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase91>((int) sbyte.MaxValue),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase92>(128),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase93>(129),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase94>(130),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase95>(131),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase96>(132),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase97>(133),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase98>(134),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase99>(135),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase100>(136),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase101>(137),
      (IComponentCreator) new ComponentCreator<TestManagementDatabase102>(138)
    }, "TestManagement");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static Dictionary<string, string> s_TcmDynamicSqlBatchStatementsMap;
    internal static readonly int c_SqlErrorNumber_PKViolation = 2627;
    internal static readonly int c_SqlErrorNumber_IndexKeyViolation = 2601;
    internal const string c_TestResultSprocPrefix = "TestResult.";
    internal const string c_TestManagementSprocPrefix = "TestManagement.";

    internal virtual Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachments(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments,
      bool areSessionAttachments,
      bool changeCounterInterval = false)
    {
      this.AssignTempIdToAttachments(attachments, changeCounterInterval);
      this.PrepareStoredProcedure("prc_CreateAttachments");
      this.BindTestResultAttachmentTypeTable2("@attachments", attachments);
      this.BindBoolean("@areSessionAttachments", areSessionAttachments);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateAttachments");
      List<int> intList1 = new List<int>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        intList1.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<int>> attachments1 = new Dictionary<TestCaseResultIdentifier, List<int>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<int> intList2;
        if (!attachments1.TryGetValue(key, out intList2))
        {
          intList2 = new List<int>();
          attachments1.Add(key, intList2);
        }
        intList2.Add(intList1[num++]);
      }
      return attachments1;
    }

    protected void AssignTempIdToAttachments(
      IEnumerable<TestResultAttachment> attachments,
      bool changeCounterInterval = false,
      int startGap = 0)
    {
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        if (changeCounterInterval)
        {
          attachment.Id = num * 2 + startGap;
          ++num;
        }
        else
          attachment.Id = startGap + num++;
      }
    }

    internal virtual int GetAttachmentTfsFileId(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int sessionId,
      int attachmentId)
    {
      this.PrepareStoredProcedure("prc_GetAttachmentTfsFileId");
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@attachmentId", attachmentId);
      using (SqlDataReader reader = this.ExecuteReader())
        return reader.Read() ? new TestManagementDatabase.GetAttachmentTfsFileIdColumns().TfsFileId.GetInt32((IDataReader) reader, 0) : throw new TestObjectNotFoundException(this.RequestContext, attachmentId, ObjectTypes.Attachment);
    }

    internal virtual bool GetAppendAttachmentStatusForUploadedFile(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int sessionId,
      int fileId,
      int attachmentId,
      bool isFileUploadComplete,
      int defaultAfnStripFlag,
      long uncompressedLength)
    {
      this.PrepareStoredProcedure("TestResult.prc_AppendAttachment");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@tfsFileId", fileId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@attachmentId", attachmentId);
      this.BindBoolean("@isComplete", isFileUploadComplete);
      this.BindInt("@sessionId", sessionId);
      this.BindBoolean("@defaultAfnStripFlag", defaultAfnStripFlag != 0);
      this.BindLong("@uncompressedLength", uncompressedLength);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new TestManagementDatabase.AppendAttachmentColumns().CoverageChanged.GetBoolean((IDataReader) reader, false) : throw new UnexpectedDatabaseResultException("prc_AppendAttachment");
    }

    internal virtual bool GetAppendAttachmentStatusForUploadedFileV2(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int sessionId,
      int attachmentId,
      bool isFileUploadComplete)
    {
      return false;
    }

    internal virtual bool GetAppendAttachmentStatusForUploadedFileInLogStore(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int sessionId,
      int attachmentId,
      bool isFileUploadComplete)
    {
      return false;
    }

    internal virtual void DeleteAttachments(
      Guid projectId,
      IEnumerable<TestResultAttachmentIdentity> attachments)
    {
      this.PrepareStoredProcedure("prc_DeleteAttachments");
      this.BindTestResultAttachmentIdentityTypeTable("@attachments", attachments);
      this.ExecuteNonQuery();
    }

    internal virtual List<string> DeleteAttachmentsFromLogStoreMapper(
      Guid projectId,
      IEnumerable<TestResultAttachmentIdentity> attachments)
    {
      return new List<string>();
    }

    internal virtual List<int> CreateTestAttachmentsInLogStoreMapper(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      return new List<int>();
    }

    internal virtual void UpdateAttachmentInLogStoreAttachmentMapper(
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId,
      long uncompressedLength,
      string filename)
    {
      throw new NotImplementedException();
    }

    internal virtual List<TestResultAttachment> CreateLogStoreTestAttachmentIdMappings(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      return new List<TestResultAttachment>();
    }

    internal virtual TestResultAttachment GetLogStoreTestAttachmentIdMapping(
      Guid projectId,
      int runId,
      int resultId,
      int subResultId,
      int attachmentId)
    {
      return new TestResultAttachment();
    }

    internal virtual Guid? GetProjectForAttachment(int attachmentId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "AttachmentDatabase.GetProjectForAttachment");
        this.PrepareStoredProcedure("prc_GetProjectForAttachment");
        this.BindNullableInt("@attachmentId", attachmentId, 0);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new Guid?(this.GetDataspaceIdentifier(new SqlColumnBinder("DataspaceId").GetInt32((IDataReader) reader))) : new Guid?();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "AttachmentDatabase.GetProjectForAttachment");
      }
    }

    protected void GetAttachmentOwnerIds(
      TestManagementRequestContext context,
      SqlDataReader reader,
      out int testRunId,
      out int sessionId,
      out int testResultId)
    {
      testRunId = 0;
      sessionId = 0;
      testResultId = 0;
      if (!reader.Read())
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeletedAttachmentArtifact, (object) testRunId, (object) testResultId), ObjectTypes.Attachment);
      TestManagementDatabase.QueryAttachmentOwnerIdColumns attachmentOwnerIdColumns = new TestManagementDatabase.QueryAttachmentOwnerIdColumns();
      testRunId = attachmentOwnerIdColumns.TestRunId.GetInt32((IDataReader) reader, 0);
      sessionId = attachmentOwnerIdColumns.SessionId.ColumnExists((IDataReader) reader) ? attachmentOwnerIdColumns.SessionId.GetInt32((IDataReader) reader, 0) : 0;
      testResultId = attachmentOwnerIdColumns.TestResultId.GetInt32((IDataReader) reader, 0);
    }

    internal virtual List<TestResultAttachment> QueryAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int attachmentId,
      bool getSiblingAttachments,
      out string areaUri)
    {
      int lazyInitialization = !projectId.Equals(Guid.Empty) ? this.GetDataspaceIdWithLazyInitialization(projectId) : 0;
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      areaUri = string.Empty;
      this.PrepareStoredProcedure("TestResult.prc_QueryAttachment");
      this.BindInt("@dataspaceId", lazyInitialization);
      this.BindInt("@attachmentId", attachmentId);
      this.BindBoolean("@getSiblingAttachments", getSiblingAttachments);
      SqlDataReader reader = this.ExecuteReader();
      int testRunId;
      int testResultId;
      this.GetAttachmentOwnerIds(context, reader, out testRunId, out int _, out testResultId);
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryAttachment");
      if (testResultId > 0)
        areaUri = this.GetResultAreaUri(reader, testRunId, testResultId);
      if (testRunId > 0 && !reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryAttachment");
      TestManagementDatabase.QueryAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryAttachmentsColumns();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      return resultAttachmentList;
    }

    internal virtual List<TestResultAttachment> QueryAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int sessionId,
      out string areaUri,
      int subResultId = 0)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      areaUri = string.Empty;
      this.PrepareStoredProcedure("TestResult.prc_QueryAttachments");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindNullableInt("@attachmentId", attachmentId, 0);
      this.BindInt("@sessionId", sessionId);
      SqlDataReader reader = this.ExecuteReader();
      if (testResultId > 0)
        areaUri = this.GetResultAreaUri(reader, testRunId, testResultId);
      if (testRunId > 0 && !reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryAttachments");
      TestManagementDatabase.QueryAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryAttachmentsColumns();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      return resultAttachmentList;
    }

    internal virtual List<TestResultAttachment> QueryAttachments2(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareDynamicProcedure("prc_QueryAttachments2");
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryAttachmentsColumns2 attachmentsColumns2 = new TestManagementDatabase.QueryAttachmentsColumns2();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns2.Bind(reader));
      return resultAttachmentList;
    }

    internal virtual List<TestResultAttachment> QueryIterationAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      string attachmentType,
      out string areaUri)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareStoredProcedure("TestResult.prc_QueryIterationAttachments");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindString("@attachmentType", attachmentType, 64, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      areaUri = this.GetResultAreaUri(reader, testRunId, testResultId);
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryIterationAttachments");
      TestManagementDatabase.QueryAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryAttachmentsColumns();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      return resultAttachmentList;
    }

    internal virtual void UpdateDefaultStrip(
      Guid projectId,
      int testRunId,
      int testResultId,
      int testCaseId)
    {
      this.PrepareStoredProcedure("prc_UpdateDefaultStrip");
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@testCaseId", testCaseId);
      this.ExecuteNonQuery();
    }

    internal virtual TestResultAttachment QueryDefaultStrip(
      int testCaseId,
      Guid projectId,
      out string areaUri)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QueryDefaultStrip");
      this.BindInt("@testCaseId", testCaseId);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
        return new TestManagementDatabase.QueryDefaultStripsColumns().Bind(reader, out areaUri);
      areaUri = (string) null;
      return (TestResultAttachment) null;
    }

    internal virtual List<bool> CheckActionRecordingExists(int[] testCaseIds)
    {
      this.PrepareStoredProcedure("prc_CheckActionRecordingExists");
      this.BindIdTypeTable("@testCaseIdsTable", (IEnumerable<int>) testCaseIds);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("value");
      if (!reader.Read())
        return (List<bool>) null;
      List<bool> boolList = new List<bool>();
      do
      {
        boolList.Add(sqlColumnBinder.GetInt32((IDataReader) reader) > 0);
      }
      while (reader.Read());
      return boolList;
    }

    internal virtual List<int> QueryAttachmentFilesToDelete(int count, out Guid batch)
    {
      this.PrepareStoredProcedure("prc_QueryAttachmentFilesToDelete");
      this.BindInt("@count", count);
      SqlDataReader reader = this.ExecuteReader();
      List<int> delete = new List<int>();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TfsFileId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Batch");
      while (reader.Read())
        delete.Add(sqlColumnBinder1.GetInt32((IDataReader) reader));
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryAttachmentFilesToDelete");
      batch = reader.Read() ? sqlColumnBinder2.GetGuid((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_QueryAttachmentFilesToDelete");
      return delete;
    }

    internal virtual void CommitDeletedAttachmentFilesToDelete(Guid batch)
    {
      this.PrepareStoredProcedure("prc_CommitDeletedAttachmentFilesToDelete");
      this.BindGuid("@batchID", batch);
      this.ExecuteNonQuery();
    }

    public virtual List<TestResultAttachment> QueryAttachments3(
      Guid projectId,
      Dictionary<string, List<object>> parametersMap)
    {
      return new List<TestResultAttachment>();
    }

    internal virtual Guid? GetProjectForAttachment(
      int attachmentId,
      out int testRunId,
      out int sessionId)
    {
      testRunId = 0;
      sessionId = 0;
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "AttachmentDatabase.GetProjectForAttachment");
        this.PrepareStoredProcedure("prc_GetProjectForAttachment");
        this.BindNullableInt("@attachmentId", attachmentId, 0);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          return new Guid?();
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("DataspaceId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("TestRunId");
        SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("SessionId");
        testRunId = sqlColumnBinder2.ColumnExists((IDataReader) reader) ? sqlColumnBinder2.GetInt32((IDataReader) reader) : 0;
        sessionId = sqlColumnBinder3.ColumnExists((IDataReader) reader) ? sqlColumnBinder3.GetInt32((IDataReader) reader) : 0;
        return new Guid?(this.GetDataspaceIdentifier(sqlColumnBinder1.GetInt32((IDataReader) reader)));
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "AttachmentDatabase.GetProjectForAttachment");
      }
    }

    internal virtual Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachmentsWithFileId(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments,
      bool changeCounterInterval = false)
    {
      return new Dictionary<TestCaseResultIdentifier, List<int>>();
    }

    internal virtual AfnStrip CreateAfnStrip(
      Guid projectId,
      int tfsFileId,
      AfnStrip afnStrip,
      Guid createdBy,
      bool changeCounterInterval = false)
    {
      return (AfnStrip) null;
    }

    internal virtual List<TestResultAttachment> QueryAttachmentsById(
      TestManagementRequestContext context,
      Guid projectId,
      int attachmentId,
      bool getSiblingAttachments)
    {
      return (List<TestResultAttachment>) null;
    }

    public virtual void DeleteTestBuild(
      Guid projectGuid,
      string[] buildUris,
      Guid lastUpdatedBy,
      bool deleteOnlyAutomatedRuns,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("prc_DeleteTestBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
      this.BindString("@buildUri", buildUris[0], 256, false, SqlDbType.NVarChar);
      this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
      this.ExecuteNonQuery();
    }

    public virtual void MarkTestBuildDeleted(Guid projectGuid, string[] buildUris)
    {
    }

    public virtual BuildConfiguration GetBuildConfigurationIdFromFlavorAndPlatform(
      Guid projectId,
      int buildId,
      string platform,
      string flavor)
    {
      return (BuildConfiguration) null;
    }

    public virtual BuildConfiguration QueryBuildConfigurationById(
      int buildConfigurationid,
      out Guid projectId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "BuildReferenceDatabase.QueryBuildConfigurationById");
        projectId = Guid.Empty;
        BuildConfiguration buildConfiguration = new BuildConfiguration();
        this.PrepareStoredProcedure("prc_QueryBuildConfigurationById");
        this.BindInt("@buildConfigurationId", buildConfigurationid);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
        {
          int dataspaceId;
          buildConfiguration = new TestManagementDatabase.QueryBuildConfigurationsColumns().bind(reader, out dataspaceId);
          projectId = this.GetDataspaceIdentifier(dataspaceId);
        }
        return buildConfiguration;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "BuildReferenceDatabase.QueryBuildConfigurationById");
      }
    }

    public virtual BuildConfiguration QueryBuildConfigurationById2(
      int buildConfigurationid,
      Guid projectId)
    {
      return (BuildConfiguration) null;
    }

    public virtual IList<string> QueryBuildsByProject(
      Guid projectId,
      bool? queryDeletedBuild,
      int batchSize)
    {
      return (IList<string>) new List<string>();
    }

    public virtual void UpdateBuildDeletionState(
      Guid projectId,
      Dictionary<string, bool> buildUriToDeletionStatus)
    {
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestRunSummaryReport2(
      Guid projectGuid,
      int runId,
      List<string> dimensionList)
    {
      return (Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData) null;
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestExecutionReport2(
      Guid projectId,
      int planId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensionList)
    {
      return (Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData) null;
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestExecutionReport3(
      Guid projectId,
      int planId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensionList)
    {
      return (Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData) null;
    }

    public virtual int QueryTestPullRequestBuildId(
      Guid projectGuid,
      Guid repositoryId,
      int pullRequestId,
      int pullRequestIterationId)
    {
      return 0;
    }

    public virtual void AddOrUpdateTestPullRequestBuilds(
      Guid projectGuid,
      Guid repositoryId,
      int pullRequestId,
      int pullRequestIterationId,
      int buildId)
    {
    }

    public virtual void AddOrUpdateCodeCoverageSummaryWithStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus status)
    {
      this.AddOrUpdateCodeCoverageSummary(projectGuid, buildRef, coverageData);
    }

    public virtual void AddOrUpdateCoverageDetailedSummaryWithStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus status,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      this.AddOrUpdateCoverageDetailedSummary(projectGuid, buildRef, coverageData);
    }

    public virtual void AddOrUpdateCodeCoverageSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CoverageSummaryStatus status)
    {
    }

    public virtual void AddOrUpdateCoverageDetailedSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CoverageSummaryStatus status,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
    }

    internal virtual CoverageSummaryStatusResult QueryCodeCoverageSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef)
    {
      return new CoverageSummaryStatusResult()
      {
        SummaryStatus = CoverageSummaryStatus.None,
        RequestedDate = DateTime.UtcNow,
        CoverageDetailedSummaryStatus = CoverageDetailedSummaryStatus.None
      };
    }

    internal virtual CoverageSummaryStatusResult QueryCoverageDetailedSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef)
    {
      return new CoverageSummaryStatusResult()
      {
        SummaryStatus = CoverageSummaryStatus.None,
        RequestedDate = DateTime.UtcNow,
        CoverageDetailedSummaryStatus = CoverageDetailedSummaryStatus.None
      };
    }

    internal virtual CodeCoverageSummary QueryCodeCoverageSummary(
      Guid projectGuid,
      string buildUri,
      string deltaBuildUri)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.QueryCodeCoverageSummary");
        CodeCoverageSummary codeCoverageSummary = new CodeCoverageSummary();
        this.PrepareStoredProcedure("prc_QueryCodeCoverageSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
        this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
        this.BindString("@deltaBuildUri", deltaBuildUri, 256, true, SqlDbType.NVarChar);
        List<CodeCoverageData> source = new List<CodeCoverageData>();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          while (reader.Read())
          {
            string buildFlavor;
            string buildPlatform;
            CodeCoverageStatistics coverageStatistics = new TestManagementDatabase.CodeCoverageSummaryColumns().Bind(reader, out buildPlatform, out buildFlavor);
            CodeCoverageData codeCoverageData1 = source.FirstOrDefault<CodeCoverageData>((System.Func<CodeCoverageData, bool>) (data => buildFlavor.Equals(data.BuildFlavor) && buildPlatform.Equals(data.BuildPlatform)));
            if (codeCoverageData1 == null)
            {
              CodeCoverageData codeCoverageData2 = new CodeCoverageData()
              {
                CoverageStats = (IList<CodeCoverageStatistics>) new List<CodeCoverageStatistics>(),
                BuildPlatform = buildPlatform,
                BuildFlavor = buildFlavor
              };
              codeCoverageData2.CoverageStats.Add(coverageStatistics);
              source.Add(codeCoverageData2);
            }
            else
              codeCoverageData1.CoverageStats.Add(coverageStatistics);
          }
        }
        codeCoverageSummary.CoverageData = (IList<CodeCoverageData>) source;
        codeCoverageSummary.BuildUri = buildUri;
        codeCoverageSummary.DeltaBuildUri = deltaBuildUri;
        return codeCoverageSummary;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.QueryCodeCoverageSummary");
      }
    }

    internal virtual CodeCoverageSummary QueryCoverageDetailedSummary(
      Guid projectGuid,
      string buildUri,
      string deltaBuildUri)
    {
      return (CodeCoverageSummary) null;
    }

    internal virtual void AddOrUpdateCodeCoverageSummary(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummary");
        this.PrepareStoredProcedure("prc_AddOrUpdateCodeCoverageSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        List<BuildConfiguration> builds;
        if (buildRef == null)
        {
          builds = (List<BuildConfiguration>) null;
        }
        else
        {
          builds = new List<BuildConfiguration>();
          builds.Add(buildRef);
        }
        this.BindBuildRefTypeTable("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
        this.BindCoverageSummaryTypeTable("@coverageStatsDataTable", (IEnumerable<CodeCoverageStatistics>) coverageData.CoverageStats);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummary");
      }
    }

    internal virtual void AddOrUpdateCoverageDetailedSummary(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData)
    {
    }

    internal virtual List<CoverageChangeExtension> QueryCoverageChanges(
      Dictionary<int, Guid> projectGuids)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryCoverageChanges");
      SqlDataReader reader = this.ExecuteReader();
      List<CoverageChangeExtension> coverageChangeExtensionList = new List<CoverageChangeExtension>();
      TestManagementDatabase.CoverageChangeColumns coverageChangeColumns = new TestManagementDatabase.CoverageChangeColumns();
      Dictionary<int, string> dictionary = new Dictionary<int, string>();
      while (reader.Read())
      {
        int dataspaceId;
        CoverageChangeExtension coverageChangeExtension = new CoverageChangeExtension(coverageChangeColumns.Bind(reader, out dataspaceId), dataspaceId);
        if (!projectGuids.ContainsKey(coverageChangeExtension.dataSpaceId))
          projectGuids.Add(dataspaceId, this.GetDataspaceIdentifier(coverageChangeExtension.dataSpaceId));
        coverageChangeExtensionList.Add(coverageChangeExtension);
      }
      return coverageChangeExtensionList;
    }

    public virtual List<BuildCoverage> QueryBuildCoverage(
      Guid projectGuid,
      string buildUri,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      this.PrepareStoredProcedure("prc_QueryBuildCoverage");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindString("@buildUri", buildUri, 64, false, SqlDbType.NVarChar);
      this.BindInt("@flags", (int) flags);
      SqlDataReader reader = this.ExecuteReader();
      SortedDictionary<int, Coverage> coverageById = new SortedDictionary<int, Coverage>();
      TestManagementDatabase.BuildCoverageColumns buildCoverageColumns = new TestManagementDatabase.BuildCoverageColumns();
      while (reader.Read())
      {
        BuildCoverage buildCoverage = buildCoverageColumns.Bind(reader);
        coverageById.Add(buildCoverage.Id, (Coverage) buildCoverage);
      }
      this.ReadCoverage(flags, "prc_QueryBuildCoverage", reader, coverageById);
      List<BuildCoverage> buildCoverageList = new List<BuildCoverage>(coverageById.Values.Count);
      foreach (BuildCoverage buildCoverage in coverageById.Values)
        buildCoverageList.Add(buildCoverage);
      return buildCoverageList;
    }

    internal virtual List<TestRunCoverage> QueryTestRunCoverage(
      Guid projectGuid,
      int testRunId,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunCoverage");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@flags", (int) flags);
      SqlDataReader reader = this.ExecuteReader();
      SortedDictionary<int, Coverage> coverageById = new SortedDictionary<int, Coverage>();
      TestManagementDatabase.TestRunCoverageColumns runCoverageColumns = new TestManagementDatabase.TestRunCoverageColumns();
      while (reader.Read())
      {
        TestRunCoverage testRunCoverage = runCoverageColumns.Bind(reader);
        coverageById.Add(testRunCoverage.Id, (Coverage) testRunCoverage);
      }
      this.ReadCoverage(flags, "prc_QueryTestRunCoverage", reader, coverageById);
      List<TestRunCoverage> testRunCoverageList = new List<TestRunCoverage>(coverageById.Values.Count);
      foreach (TestRunCoverage testRunCoverage in coverageById.Values)
        testRunCoverageList.Add(testRunCoverage);
      return testRunCoverageList;
    }

    protected virtual void ReadCoverage(
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags,
      string storedProcedure,
      SqlDataReader reader,
      SortedDictionary<int, Coverage> coverageById)
    {
      if ((flags & Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.Modules) == (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) 0)
        return;
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      TestManagementDatabase.ModuleCoverageColumns moduleCoverageColumns = new TestManagementDatabase.ModuleCoverageColumns();
      while (reader.Read())
      {
        ModuleCoverage moduleCoverage = moduleCoverageColumns.Bind(reader, flags);
        coverageById[moduleCoverage.CoverageId].Modules.Add(moduleCoverage);
      }
      if ((flags & Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.Functions) == (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) 0)
        return;
      SortedDictionary<int, SortedDictionary<int, ModuleCoverage>> sortedDictionary1 = new SortedDictionary<int, SortedDictionary<int, ModuleCoverage>>();
      foreach (Coverage coverage in coverageById.Values)
      {
        SortedDictionary<int, ModuleCoverage> sortedDictionary2 = new SortedDictionary<int, ModuleCoverage>();
        foreach (ModuleCoverage module in coverage.Modules)
          sortedDictionary2.Add(module.ModuleId, module);
        sortedDictionary1.Add(coverage.Id, sortedDictionary2);
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      TestManagementDatabase.FunctionCoverageColumns functionCoverageColumns = new TestManagementDatabase.FunctionCoverageColumns();
      while (reader.Read())
      {
        FunctionCoverage functionCoverage = functionCoverageColumns.Bind(reader);
        sortedDictionary1[functionCoverage.CoverageId][functionCoverage.ModuleId].Functions.Add(functionCoverage);
      }
    }

    public virtual void UpdateBuildCoverage(
      int buildConfigurationId,
      Coverage coverage,
      int coverageChangeId,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_UpdateBuildCoverage");
      this.BindInt("@buildConfigurationId", buildConfigurationId);
      this.BindByte("@state", coverage.State);
      this.BindInt("@coverageChangeId", coverageChangeId);
      this.BindStringPreserveNull("@errorLog", coverage.LastError, int.MaxValue, SqlDbType.NVarChar);
      this.BindModuleCoverageTypeTable("@moduleInfo", (IEnumerable<ModuleCoverage>) coverage.Modules);
      this.BindFunctionCoverageTypeTable("@functionInfo", (IEnumerable<FunctionCoverage>) this.GetFunctionCoverages(coverage));
      this.ExecuteNonQuery();
    }

    internal virtual void UpdateTestRunCoverage(int testRunId, Coverage coverage, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_UpdateTestRunCoverage");
      this.BindInt("@testRunId", testRunId);
      this.BindByte("@state", coverage.State);
      this.BindStringPreserveNull("@errorLog", coverage.LastError, int.MaxValue, SqlDbType.NVarChar);
      this.BindModuleCoverageTypeTable("@moduleInfo", (IEnumerable<ModuleCoverage>) coverage.Modules);
      this.BindFunctionCoverageTypeTable("@functionInfo", (IEnumerable<FunctionCoverage>) this.GetFunctionCoverages(coverage));
      this.ExecuteNonQuery();
    }

    protected List<FunctionCoverage> GetFunctionCoverages(Coverage coverage)
    {
      List<FunctionCoverage> functionCoverages = new List<FunctionCoverage>();
      coverage.Modules.ForEach((Action<ModuleCoverage>) (module =>
      {
        module.Functions.ForEach((Action<FunctionCoverage>) (function => function.ModuleId = module.ModuleId));
        functionCoverages.AddRange((IEnumerable<FunctionCoverage>) module.Functions);
      }));
      return functionCoverages;
    }

    public virtual void CreateBuildConfiguration(Guid projectId, BuildConfiguration buildRef)
    {
    }

    public virtual IList<TestExtensionFieldDetails> AddExtensionFields(
      IVssRequestContext context,
      Guid projectId,
      IList<TestExtensionFieldDetails> fieldsToAdd)
    {
      try
      {
        context.TraceEnter(0, "TestManagement", "Database", "TestExtensibilityDatabase.AddFields");
        List<TestExtensionFieldDetails> extensionFieldDetailsList = new List<TestExtensionFieldDetails>();
        this.PrepareStoredProcedure("prc_AddTestExtensionFields");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestExtensionFieldsTypeTable("@fieldsTable", (IEnumerable<TestExtensionFieldDetails>) fieldsToAdd);
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestManagementDatabase.TestExtensionFieldsColumns extensionFieldsColumns = new TestManagementDatabase.TestExtensionFieldsColumns();
          extensionFieldDetailsList.Add(extensionFieldsColumns.bind(reader));
        }
        return (IList<TestExtensionFieldDetails>) extensionFieldDetailsList;
      }
      finally
      {
        context.TraceLeave(0, "TestManagement", "Database", "TestExtensibilityDatabase.AddFields");
      }
    }

    public virtual IList<TestExtensionFieldDetails> QueryExtensionFields(
      IVssRequestContext context,
      Guid projectId,
      bool isRunScoped,
      bool isResultScoped,
      bool isSystemScoped)
    {
      try
      {
        context.TraceEnter(0, "TestManagement", "Database", "TestExtensibilityDatabase.QueryFields");
        List<TestExtensionFieldDetails> extensionFieldDetailsList = new List<TestExtensionFieldDetails>();
        this.PrepareStoredProcedure("prc_QueryTestExtensionFields");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindBoolean("@isRunScoped", isRunScoped);
        this.BindBoolean("@isResultScoped", isResultScoped);
        this.BindBoolean("@isSystemScoped", isSystemScoped);
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestManagementDatabase.TestExtensionFieldsColumns extensionFieldsColumns = new TestManagementDatabase.TestExtensionFieldsColumns();
          extensionFieldDetailsList.Add(extensionFieldsColumns.bind(reader));
        }
        return (IList<TestExtensionFieldDetails>) extensionFieldDetailsList;
      }
      finally
      {
        context.TraceLeave(0, "TestManagement", "Database", "TestExtensibilityDatabase.QueryFields");
      }
    }

    public virtual void CreateLogStoreStorageAccountMap(
      Guid projectId,
      int storageAccountConnectionIndex)
    {
      this.PrepareStoredProcedure("prc_CreateLogStoreStorageAccountMap");
      this.BindInt("@dataspaceId", projectId == Guid.Empty ? 0 : this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@storageAccountConnectionIndex", storageAccountConnectionIndex);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateLogStoreContainerStateField(
      Guid projectId,
      int testRunId,
      int fieldId,
      int fieldValue,
      int newFieldValue,
      int batchSize)
    {
    }

    public virtual List<LogStoreStorageAccountMap> GetLogStoreStorageAccount(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetLogStoreStorageAccount");
      this.BindInt("@dataspaceId", projectId == Guid.Empty ? 0 : this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("StorageAccountConnectionIndex");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("IsReadOnly");
      List<LogStoreStorageAccountMap> storeStorageAccount = new List<LogStoreStorageAccountMap>();
      while (reader.Read())
      {
        LogStoreStorageAccountMap storageAccountMap = new LogStoreStorageAccountMap(sqlColumnBinder1.GetInt32((IDataReader) reader), sqlColumnBinder2.GetBoolean((IDataReader) reader, false));
        storeStorageAccount.Add(storageAccountMap);
      }
      return storeStorageAccount;
    }

    public virtual void CreateLogStoreArtifactStorageAccountMap(
      Guid projectId,
      ContainerScope artifactScope,
      int artifactId,
      int storageAccountConnectionIndex)
    {
      this.PrepareStoredProcedure("prc_CreateLogStoreArtifactStorageAccountMap");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindByte("@artifactScope", (byte) artifactScope);
      this.BindInt("@artifactId", artifactId);
      this.BindInt("@storageAccountConnectionIndex", storageAccountConnectionIndex);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteLogStoreArtifactStorageAccountMap(
      Guid projectId,
      ContainerScope artifactScope,
      int artifactId)
    {
      this.PrepareStoredProcedure("prc_DeleteLogStoreArtifactStorageAccountMap");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindByte("@artifactScope", (byte) artifactScope);
      this.BindInt("@artifactId", artifactId);
      this.ExecuteNonQuery();
    }

    public virtual int GetLogStoreArtifactStorageAccount(
      Guid projectId,
      ContainerScope artifactScope,
      int artifactId)
    {
      this.PrepareStoredProcedure("prc_GetLogStoreArtifactStorageAccount");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindByte("@artifactScope", (byte) artifactScope);
      this.BindInt("@artifactId", artifactId);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("StorageAccountConnectionIndex");
      int artifactStorageAccount = -1;
      if (reader.Read())
        artifactStorageAccount = sqlColumnBinder.GetInt32((IDataReader) reader, -1, -1);
      return artifactStorageAccount;
    }

    public virtual long UpdateLogStoreProjectSummary(
      Guid projectId,
      long blobSize,
      bool isinitialize = false)
    {
      return 0;
    }

    public virtual long UpdateLogStoreContentSizeByRelease(
      Guid projectId,
      int releaseId,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      return 0;
    }

    internal virtual List<TestResultAttachment> QueryLogStoreAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      return new List<TestResultAttachment>();
    }

    internal virtual Dictionary<TestCaseResultIdentifier, List<long>> CreateLogStoreAttachmentMapping(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      return new Dictionary<TestCaseResultIdentifier, List<long>>();
    }

    internal virtual Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachmentMappingV2(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      return new Dictionary<TestCaseResultIdentifier, List<int>>();
    }

    internal virtual List<TestResultAttachment> QueryAttachmentsV2(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      return new List<TestResultAttachment>();
    }

    internal virtual List<TestResultAttachment> QueryUploadedAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      return new List<TestResultAttachment>();
    }

    internal virtual Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachmentMapping(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      return new Dictionary<TestCaseResultIdentifier, List<int>>();
    }

    internal virtual List<TestResultAttachment> QueryAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      return new List<TestResultAttachment>();
    }

    internal virtual List<TestResultAttachment> QueryIterationAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int iterationId)
    {
      return new List<TestResultAttachment>();
    }

    public virtual void DeleteTestPlanData(Guid projectGuid, int testPlanId)
    {
    }

    public virtual void DeleteTestPointData(Guid projectGuid, int testPlanId, List<int> pointIds)
    {
    }

    public virtual List<int> DeleteTestSuiteRunsData(
      Guid projectGuid,
      Guid updatedBy,
      int testPlanId,
      List<int> runIds,
      int deleteTestSuiteRunsDataSprocExecTimeOutInSec)
    {
      return new List<int>();
    }

    public virtual List<PointLastResult> FilterPointsOnOutcome(
      Guid projectGuid,
      int planId,
      List<int> pointIds,
      List<byte> pointOutcomes,
      List<byte> resultStates)
    {
      return new List<PointLastResult>();
    }

    public virtual List<PointLastResult> FilterPointsOnOutcome2(
      Guid projectGuid,
      int planId,
      List<int> pointIds,
      List<byte> pointOutcomes,
      List<byte> resultStates)
    {
      return new List<PointLastResult>();
    }

    protected SqlParameter BindReleaseRef2TypeTable(
      string parameterName,
      IEnumerable<ReleaseReference2> releaseRefs,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      releaseRefs = releaseRefs ?? Enumerable.Empty<ReleaseReference2>();
      return this.BindTable(parameterName, "typ_ReleaseRef2Table", this.BindReleaseRef2TypeTableRows(releaseRefs, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindReleaseRef2TypeTableRows(
      IEnumerable<ReleaseReference2> releaseRefs,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (ReleaseReference2 releaseRef in releaseRefs)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ReleaseRef2Table);
        record.SetInt32(0, projectGuidToDataspaceMap[releaseRef.ProjectId]);
        record.SetInt32(1, releaseRef.ReleaseRefId);
        record.SetNullableStringAsEmpty(2, releaseRef.ReleaseUri);
        record.SetNullableStringAsEmpty(3, releaseRef.ReleaseEnvUri);
        record.SetInt32(4, releaseRef.ReleaseId);
        record.SetInt32(5, releaseRef.ReleaseEnvId);
        record.SetInt32(6, releaseRef.ReleaseDefId);
        record.SetInt32(7, releaseRef.ReleaseEnvDefId);
        record.SetInt32(8, releaseRef.Attempt);
        record.SetNullableStringAsEmpty(9, releaseRef.ReleaseName);
        record.SetNullableDateTime(10, releaseRef.ReleaseCreationDate);
        record.SetNullableDateTime(11, releaseRef.EnvironmentCreationDate);
        yield return record;
      }
    }

    protected SqlParameter BindBuildRef2TypeTable(
      string parameterName,
      IEnumerable<BuildReference2> builds,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      builds = builds ?? Enumerable.Empty<BuildReference2>();
      return this.BindTable(parameterName, "typ_BuildRef3Table", this.BindBuildRef2TypeTableRows(builds, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindBuildRef2TypeTableRows(
      IEnumerable<BuildReference2> builds,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (BuildReference2 build in builds)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_BuildRef3Table);
        record.SetInt32(0, projectGuidToDataspaceMap[build.ProjectId]);
        record.SetInt32(1, build.BuildConfigurationId);
        record.SetInt32(2, build.BuildId);
        record.SetNullableStringAsEmpty(3, build.BuildUri);
        record.SetNullableStringAsEmpty(4, build.BuildNumber);
        record.SetNullableStringAsEmpty(5, build.BuildPlatform);
        record.SetNullableStringAsEmpty(6, build.BuildFlavor);
        record.SetInt32(7, build.BuildDefinitionId);
        record.SetNullableDateTime(8, build.CreatedDate);
        record.SetNullableStringAsEmpty(9, build.BranchName);
        record.SetNullableStringAsEmpty(10, build.SourceVersion);
        record.SetNullableStringAsEmpty(11, build.BuildSystem);
        record.SetNullableInt32(12, build.CoverageId);
        record.SetBoolean(13, build.BuildDeleted);
        record.SetNullableStringAsEmpty(14, build.RepoId);
        record.SetNullableStringAsEmpty(15, build.RepoType);
        yield return record;
      }
    }

    protected SqlParameter BindTestRunContext2TypeTable(
      string parameterName,
      IEnumerable<TestRunContext2> testRunContexts,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testRunContexts = testRunContexts ?? Enumerable.Empty<TestRunContext2>();
      return this.BindTable(parameterName, "typ_TestRunContext2Table", this.BindTestRunContext2TypeTableRows(testRunContexts, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestRunContext2TypeTableRows(
      IEnumerable<TestRunContext2> testRunContexts,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestRunContext2 testRunContext in testRunContexts)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestRunContext2Table);
        record.SetInt32(0, projectGuidToDataspaceMap[testRunContext.ProjectId]);
        record.SetInt32(1, testRunContext.TestRunContextId);
        record.SetInt32(2, testRunContext.BuildRefId);
        record.SetInt32(3, testRunContext.ReleaseRefId);
        record.SetNullableStringAsEmpty(4, testRunContext.SourceWorkflow);
        yield return record;
      }
    }

    protected SqlParameter BindTestCaseReference2TypeTable(
      string parameterName,
      IEnumerable<TestCaseReference2> testCaseReferences,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testCaseReferences = testCaseReferences ?? Enumerable.Empty<TestCaseReference2>();
      return this.BindTable(parameterName, "typ_TestCaseReference2Table", this.BindTestCaseReference2TypeTableRows(testCaseReferences, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseReference2TypeTableRows(
      IEnumerable<TestCaseReference2> testCaseReferences,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestCaseReference2 testCaseReference in testCaseReferences)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestCaseReference2Table);
        record.SetInt32(0, projectGuidToDataspaceMap[testCaseReference.ProjectId]);
        record.SetInt32(1, testCaseReference.TestCaseId);
        record.SetInt32(2, testCaseReference.TestPointId);
        record.SetInt32(3, testCaseReference.ConfigurationId);
        record.SetInt32(4, testCaseReference.TestCaseRefId);
        record.SetString(5, testCaseReference.AutomatedTestName);
        record.SetString(6, testCaseReference.AutomatedTestStorage);
        record.SetString(7, testCaseReference.AutomatedTestType);
        record.SetStringPreserveNull(8, testCaseReference.AutomatedTestId);
        record.SetStringPreserveNull(9, testCaseReference.TestCaseTitle);
        record.SetInt32(10, testCaseReference.TestCaseRevision);
        record.SetByte(11, testCaseReference.Priority);
        record.SetString(12, testCaseReference.Owner);
        record.SetInt32(13, testCaseReference.AreaId);
        record.SetDateTime(14, testCaseReference.CreationDate);
        record.SetGuid(15, testCaseReference.CreatedBy);
        record.SetDateTime(16, testCaseReference.LastRefTestRunDate);
        record.SetBytesPreserveNull(17, testCaseReference.AutomatedTestNameHash);
        record.SetBytesPreserveNull(18, testCaseReference.AutomatedTestStorageHash);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult2TypeTable(
      string parameterName,
      IEnumerable<TestResult2> testResults,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testResults = testResults ?? Enumerable.Empty<TestResult2>();
      return this.BindTable(parameterName, "typ_TestResult2Table2", this.BindTestResult2TypeTableRows(testResults, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestResult2TypeTableRows(
      IEnumerable<TestResult2> testResults,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestResult2 testResult in testResults)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResult2Table2);
        record.SetInt32(0, projectGuidToDataspaceMap[testResult.ProjectId]);
        record.SetInt32(1, testResult.TestRunId);
        record.SetInt32(2, testResult.TestCaseRefId);
        record.SetInt32(3, testResult.TestResultId);
        record.SetDateTime(4, testResult.CreationDate);
        record.SetDateTime(5, testResult.LastUpdated);
        record.SetByte(6, testResult.Outcome);
        record.SetByte(7, testResult.State);
        record.SetInt32(8, testResult.Revision);
        record.SetNullableDateTime(9, testResult.DateStarted);
        record.SetNullableDateTime(10, testResult.DateCompleted);
        record.SetNullableGuid(11, testResult.LastUpdatedBy);
        record.SetNullableGuid(12, testResult.RunBy);
        record.SetStringPreserveNull(13, testResult.ComputerName);
        record.SetNullableByte(14, testResult.FailureType);
        record.SetNullableInt32(15, testResult.ResolutionStateId);
        record.SetNullableGuid(16, testResult.Owner);
        record.SetNullableInt32(17, testResult.ResetCount);
        record.SetNullableInt32(18, testResult.AfnStripId);
        record.SetNullableByte(19, testResult.EffectivePointState);
        yield return record;
      }
    }

    protected SqlParameter BindTestMessageLogEntry2TypeTable(
      string parameterName,
      IEnumerable<TestMessageLogEntry2> testMessageLogEntry)
    {
      testMessageLogEntry = testMessageLogEntry ?? Enumerable.Empty<TestMessageLogEntry2>();
      return this.BindTable(parameterName, "typ_TestMessageLogEntry2Table", this.BindTestMessageLogEntry2TypeTableRows(testMessageLogEntry));
    }

    private IEnumerable<SqlDataRecord> BindTestMessageLogEntry2TypeTableRows(
      IEnumerable<TestMessageLogEntry2> testMessageLogEntry)
    {
      foreach (TestMessageLogEntry2 messageLogEntry2 in testMessageLogEntry)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestMessageLogEntry2Table);
        record.SetInt32(0, messageLogEntry2.TestMessageLogId);
        record.SetInt32(1, messageLogEntry2.EntryId);
        record.SetGuid(2, messageLogEntry2.LogUser);
        record.SetDateTime(3, messageLogEntry2.DateCreated);
        record.SetByte(4, messageLogEntry2.LogLevel);
        record.SetStringPreserveNull(5, messageLogEntry2.Message);
        yield return record;
      }
    }

    protected SqlParameter BindTestRun2TypeTable(
      string parameterName,
      IEnumerable<TestRun2> testRun,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testRun = testRun ?? Enumerable.Empty<TestRun2>();
      return this.BindTable(parameterName, "typ_TestRun3Table2", this.BindTestRun2TypeTableRows(testRun, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestRun2TypeTableRows(
      IEnumerable<TestRun2> testRuns,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestRun2 testRun in testRuns)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestRun3Table2);
        record.SetInt32(0, projectGuidToDataspaceMap[testRun.ProjectId]);
        record.SetInt32(1, testRun.TestRunId);
        record.SetString(2, testRun.Title);
        record.SetDateTime(3, testRun.CreationDate);
        record.SetDateTime(4, testRun.LastUpdated);
        record.SetGuid(5, testRun.Owner);
        record.SetByte(6, testRun.State);
        record.SetInt32(7, testRun.IncompleteTests);
        record.SetInt32(8, testRun.TestPlanId);
        record.SetNullableInt32(9, testRun.IterationId);
        record.SetStringPreserveNull(10, testRun.DropLocation);
        record.SetStringPreserveNull(11, testRun.BuildNumber);
        record.SetString(12, testRun.ErrorMessage);
        record.SetNullableDateTime(13, testRun.StartDate);
        record.SetNullableDateTime(14, testRun.CompleteDate);
        record.SetByte(15, testRun.PostProcessState);
        record.SetNullableDateTime(16, testRun.DueDate);
        record.SetStringPreserveNull(17, testRun.Controller);
        record.SetInt32(18, testRun.TestMessageLogId);
        record.SetString(19, testRun.LegacySharePath);
        record.SetInt32(20, testRun.TestSettingsId);
        record.SetInt32(21, testRun.BuildConfigurationId);
        record.SetInt32(22, testRun.Revision);
        record.SetGuid(23, testRun.LastUpdatedBy);
        record.SetByte(24, testRun.Type);
        record.SetNullableInt32(25, testRun.CoverageId);
        record.SetBoolean(26, testRun.IsAutomated);
        record.SetGuid(27, testRun.TestEnvironmentId);
        record.SetInt32(28, testRun.Version);
        record.SetInt32(29, testRun.PublicTestSettingsId);
        record.SetBoolean(30, testRun.IsBvt);
        record.SetString(31, testRun.Comment);
        record.SetInt32(32, testRun.TotalTests);
        record.SetInt32(33, testRun.PassedTests);
        record.SetInt32(34, testRun.NotApplicableTests);
        record.SetInt32(35, testRun.UnanalyzedTests);
        record.SetBoolean(36, testRun.IsMigrated);
        record.SetStringPreserveNull(37, testRun.ReleaseUri);
        record.SetStringPreserveNull(38, testRun.ReleaseEnvironmentUri);
        record.SetInt32(39, testRun.TestRunContextId);
        record.SetNullableInt32(40, testRun.MaxReservedResultId);
        record.SetNullableDateTime(41, testRun.DeletedOn);
        yield return record;
      }
    }

    protected SqlParameter BindTestRunSummary2TypeTable(
      string parameterName,
      IEnumerable<TestRunSummary2> testRunSummary,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testRunSummary = testRunSummary ?? Enumerable.Empty<TestRunSummary2>();
      return this.BindTable(parameterName, "typ_TestRunSummary2Table", this.BindTestRunSummary2TypeTableRows(testRunSummary, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestRunSummary2TypeTableRows(
      IEnumerable<TestRunSummary2> testRuns,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestRunSummary2 testRun in testRuns)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestRunSummary2Table);
        sqlDataRecord.SetInt32(0, projectGuidToDataspaceMap[testRun.ProjectId]);
        sqlDataRecord.SetInt32(1, (int) testRun.TestRunStatsId);
        sqlDataRecord.SetInt32(2, testRun.TestRunId);
        sqlDataRecord.SetInt32(3, testRun.TestRunContextId);
        sqlDataRecord.SetDateTime(4, testRun.TestRunCompletedDate);
        sqlDataRecord.SetByte(5, testRun.TestOutcome);
        sqlDataRecord.SetInt32(6, testRun.ResultCount);
        sqlDataRecord.SetInt64(7, testRun.ResultDuration);
        sqlDataRecord.SetInt64(8, testRun.RunDuration);
        sqlDataRecord.SetBoolean(9, testRun.IsRerun);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestRunSummary3TypeTable(
      string parameterName,
      IEnumerable<TestRunSummary2> testRunSummary,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testRunSummary = testRunSummary ?? Enumerable.Empty<TestRunSummary2>();
      return this.BindTable(parameterName, "typ_TestRunSummary3Table", this.BindTestRunSummary3TypeTableRows(testRunSummary, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestRunSummary3TypeTableRows(
      IEnumerable<TestRunSummary2> testRuns,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestRunSummary2 testRun in testRuns)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestRunSummary3Table);
        sqlDataRecord.SetInt32(0, projectGuidToDataspaceMap[testRun.ProjectId]);
        sqlDataRecord.SetInt64(1, testRun.TestRunStatsId);
        sqlDataRecord.SetInt32(2, testRun.TestRunId);
        sqlDataRecord.SetInt32(3, testRun.TestRunContextId);
        sqlDataRecord.SetDateTime(4, testRun.TestRunCompletedDate);
        sqlDataRecord.SetByte(5, testRun.TestOutcome);
        sqlDataRecord.SetInt32(6, testRun.ResultCount);
        sqlDataRecord.SetInt64(7, testRun.ResultDuration);
        sqlDataRecord.SetInt64(8, testRun.RunDuration);
        sqlDataRecord.SetBoolean(9, testRun.IsRerun);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResultsEx2TypeTable(
      string parameterName,
      IEnumerable<TestResultsEx2> testResultsEx,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testResultsEx = testResultsEx ?? Enumerable.Empty<TestResultsEx2>();
      return this.BindTable(parameterName, "typ_TestResultsEx2Table2", this.BindTestResultsEx2TypeTableRows(testResultsEx, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestResultsEx2TypeTableRows(
      IEnumerable<TestResultsEx2> testResultsEx,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestResultsEx2 testResultsEx2 in testResultsEx)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultsEx2Table2);
        record.SetInt32(0, projectGuidToDataspaceMap[testResultsEx2.ProjectId]);
        record.SetInt32(1, testResultsEx2.TestRunId);
        record.SetInt32(2, testResultsEx2.TestResultId);
        record.SetString(3, testResultsEx2.FieldName);
        record.SetDateTime(4, testResultsEx2.CreationDate);
        record.SetNullableInt32(5, testResultsEx2.IntValue);
        record.SetNullableDouble(6, testResultsEx2.FloatValue);
        record.SetNullableBoolean(7, testResultsEx2.BitValue);
        record.SetNullableDateTime(8, testResultsEx2.DateTimeValue);
        record.SetNullableGuid(9, testResultsEx2.GuidValue);
        record.SetStringPreserveNull(10, testResultsEx2.StringValue);
        yield return record;
      }
    }

    protected SqlParameter BindTestCaseMetadata2TypeTable(
      string parameterName,
      IEnumerable<TestCaseMetadata2> testCases,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testCases = testCases ?? Enumerable.Empty<TestCaseMetadata2>();
      return this.BindTable(parameterName, "typ_TestCaseMetadata2Table", this.BindTestCaseMetadata2TypeTableRows(testCases, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseMetadata2TypeTableRows(
      IEnumerable<TestCaseMetadata2> testCases,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestCaseMetadata2 testCase in testCases)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestCaseMetadata2Table);
        sqlDataRecord.SetInt32(0, projectGuidToDataspaceMap[testCase.ProjectId]);
        sqlDataRecord.SetInt32(1, testCase.TestMetadataId);
        sqlDataRecord.SetString(2, testCase.Container);
        sqlDataRecord.SetString(3, testCase.Name);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindRequirementsToTestsMapping2TypeTable(
      string parameterName,
      IEnumerable<RequirementsToTestsMapping2> requirementsToTestsMapping,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      requirementsToTestsMapping = requirementsToTestsMapping ?? Enumerable.Empty<RequirementsToTestsMapping2>();
      return this.BindTable(parameterName, "typ_RequirementsToTestsMapping2Table", this.BindRequirementsToTestsMapping2TypeTableRows(requirementsToTestsMapping, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindRequirementsToTestsMapping2TypeTableRows(
      IEnumerable<RequirementsToTestsMapping2> requirementsToTestsMapping,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (RequirementsToTestsMapping2 requirementsToTestsMapping2 in requirementsToTestsMapping)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_RequirementsToTestsMapping2Table);
        record.SetInt32(0, projectGuidToDataspaceMap[requirementsToTestsMapping2.ProjectId]);
        record.SetInt32(1, requirementsToTestsMapping2.WorkItemId);
        record.SetInt32(2, requirementsToTestsMapping2.TestMetadataId);
        record.SetDateTime(3, requirementsToTestsMapping2.CreationDate);
        record.SetGuid(4, requirementsToTestsMapping2.CreatedBy);
        record.SetNullableDateTime(5, requirementsToTestsMapping2.DeletionDate);
        record.SetNullableGuid(6, requirementsToTestsMapping2.DeletedBy);
        record.SetBoolean(7, requirementsToTestsMapping2.IsMigratedToWIT);
        yield return record;
      }
    }

    protected SqlParameter BindTestResultReset2Table(
      string parameterName,
      IEnumerable<TestResultReset2> testResultResets,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testResultResets = testResultResets ?? Enumerable.Empty<TestResultReset2>();
      return this.BindTable(parameterName, "typ_TestResultReset2Table2", this.BindTestResultReset2TypeTableRows(testResultResets, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestResultReset2TypeTableRows(
      IEnumerable<TestResultReset2> testResultResets,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestResultReset2 testResultReset in testResultResets)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultReset2Table2);
        record.SetInt32(0, projectGuidToDataspaceMap[testResultReset.ProjectId]);
        record.SetInt32(1, testResultReset.TestRunId);
        record.SetInt32(2, testResultReset.TestResultId);
        record.SetInt32(3, testResultReset.Revision);
        record.SetDateTime(4, testResultReset.DateModified);
        record.SetGuid(5, testResultReset.AuditIdentity);
        record.SetNullableBinary(6, testResultReset.TestResultRV);
        yield return record;
      }
    }

    protected SqlParameter BindTestActionResult2Table(
      string parameterName,
      IEnumerable<TestActionResult2> testActionResults)
    {
      testActionResults = testActionResults ?? Enumerable.Empty<TestActionResult2>();
      return this.BindTable(parameterName, "typ_TestActionResult2Table", this.BindTestActionResult2TypeTableRows(testActionResults));
    }

    private IEnumerable<SqlDataRecord> BindTestActionResult2TypeTableRows(
      IEnumerable<TestActionResult2> testActionResults)
    {
      foreach (TestActionResult2 testActionResult in testActionResults)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestActionResult2Table);
        record.SetInt32(0, testActionResult.TestRunId);
        record.SetInt32(1, testActionResult.TestResultId);
        record.SetInt32(2, testActionResult.IterationId);
        record.SetString(3, testActionResult.ActionPath);
        record.SetDateTime(4, testActionResult.CreationDate);
        record.SetDateTime(5, testActionResult.LastUpdated);
        record.SetByte(6, testActionResult.Outcome);
        record.SetString(7, testActionResult.ErrorMessage);
        record.SetNullableDateTime(8, testActionResult.DateStarted);
        record.SetNullableDateTime(9, testActionResult.DateCompleted);
        record.SetInt64(10, testActionResult.Duration);
        record.SetNullableInt32(11, testActionResult.SharedStepId);
        record.SetNullableInt32(12, testActionResult.SharedStepRevision);
        record.SetString(13, testActionResult.Comment);
        yield return record;
      }
    }

    protected SqlParameter BindTestRunEx2Table(
      string parameterName,
      IEnumerable<TestRunEx2> testRunEx,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testRunEx = testRunEx ?? Enumerable.Empty<TestRunEx2>();
      return this.BindTable(parameterName, "typ_TestRunEx2Table2", this.BindTestRunEx2TypeTableRows(testRunEx, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestRunEx2TypeTableRows(
      IEnumerable<TestRunEx2> testRunExs,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestRunEx2 testRunEx2 in testRunExs)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestRunEx2Table);
        record.SetInt32(0, projectGuidToDataspaceMap[testRunEx2.ProjectId]);
        record.SetInt32(1, testRunEx2.TestRunId);
        record.SetString(2, testRunEx2.FieldName);
        record.SetDateTime(3, testRunEx2.CreatedDate);
        record.SetNullableInt32(4, testRunEx2.IntValue);
        record.SetNullableDouble(5, testRunEx2.FloatValue);
        record.SetNullableBoolean(6, testRunEx2.BitValue);
        record.SetNullableDateTime(7, testRunEx2.DateTimeValue);
        record.SetNullableGuid(8, testRunEx2.GuidValue);
        record.SetStringPreserveNull(9, testRunEx2.StringValue);
        yield return record;
      }
    }

    protected SqlParameter BindTestRunExtended2Table(
      string parameterName,
      IEnumerable<TestRunExtended2> testRunExtended,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      testRunExtended = testRunExtended ?? Enumerable.Empty<TestRunExtended2>();
      return this.BindTable(parameterName, "typ_TestRunExtended2Table", this.BindTestRunExtended2TypeTableRows(testRunExtended, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindTestRunExtended2TypeTableRows(
      IEnumerable<TestRunExtended2> testRuns,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (TestRunExtended2 testRun in testRuns)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestRunExtended2Table);
        record.SetInt32(0, projectGuidToDataspaceMap[testRun.ProjectId]);
        record.SetInt32(1, testRun.TestRunId);
        record.SetNullableByte(2, testRun.Substate);
        record.SetStringPreserveNull(3, testRun.SourceFilter);
        record.SetStringPreserveNull(4, testRun.TestCaseFilter);
        record.SetStringPreserveNull(5, testRun.TestEnvironmentUrl);
        record.SetStringPreserveNull(6, testRun.AutEnvironmentUrl);
        record.SetStringPreserveNull(7, testRun.CsmContent);
        record.SetStringPreserveNull(8, testRun.CsmParameters);
        record.SetStringPreserveNull(9, testRun.SubscriptionName);
        yield return record;
      }
    }

    protected SqlParameter BindTestParameter2Table(
      string parameterName,
      IEnumerable<TestParameter2> testParameters)
    {
      testParameters = testParameters ?? Enumerable.Empty<TestParameter2>();
      return this.BindTable(parameterName, "typ_TestParameter2Table", this.BindTestParameter2TypeTableRows(testParameters));
    }

    private IEnumerable<SqlDataRecord> BindTestParameter2TypeTableRows(
      IEnumerable<TestParameter2> testParameters)
    {
      foreach (TestParameter2 testParameter in testParameters)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestParameter2Table);
        record.SetInt32(0, testParameter.TestRunId);
        record.SetInt32(1, testParameter.TestResultId);
        record.SetInt32(2, testParameter.IterationId);
        record.SetString(3, testParameter.ActionPath);
        record.SetString(4, testParameter.ParameterName);
        record.SetDateTime(5, testParameter.CreationDate);
        record.SetDateTime(6, testParameter.DateModified);
        record.SetByte(7, testParameter.DataType);
        record.SetBytesPreserveNull(8, testParameter.Expected);
        record.SetBytesPreserveNull(9, testParameter.Actual);
        yield return record;
      }
    }

    protected SqlParameter BindCoverage2Table(
      string parameterName,
      IEnumerable<Coverage2> coverage2)
    {
      coverage2 = coverage2 ?? Enumerable.Empty<Coverage2>();
      return this.BindTable(parameterName, "typ_Coverage2Table", this.BindCoverage2TypeTableRows(coverage2));
    }

    private IEnumerable<SqlDataRecord> BindCoverage2TypeTableRows(IEnumerable<Coverage2> coverage2)
    {
      foreach (Coverage2 coverage2_1 in coverage2)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_Coverage2Table);
        record.SetInt32(0, coverage2_1.CoverageId);
        record.SetDateTime(1, coverage2_1.DateCreated);
        record.SetDateTime(2, coverage2_1.DateModified);
        record.SetByte(3, coverage2_1.State);
        record.SetStringPreserveNull(4, coverage2_1.LastError);
        yield return record;
      }
    }

    protected SqlParameter BindCodeCoverageSummary2Table(
      string parameterName,
      IEnumerable<CodeCoverageSummary2> codeCoverageSummary2s,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      codeCoverageSummary2s = codeCoverageSummary2s ?? Enumerable.Empty<CodeCoverageSummary2>();
      return this.BindTable(parameterName, "typ_CodeCoverageSummary2Table", this.BindCodeCoverageSummary2TypeTableRows(codeCoverageSummary2s, projectGuidToDataspaceMap));
    }

    private IEnumerable<SqlDataRecord> BindCodeCoverageSummary2TypeTableRows(
      IEnumerable<CodeCoverageSummary2> codeCoverageSummary2s,
      Dictionary<Guid, int> projectGuidToDataspaceMap)
    {
      foreach (CodeCoverageSummary2 coverageSummary2 in codeCoverageSummary2s)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_CodeCoverageSummary2Table);
        sqlDataRecord.SetInt32(0, projectGuidToDataspaceMap[coverageSummary2.ProjectId]);
        sqlDataRecord.SetInt32(1, coverageSummary2.BuildConfigurationId);
        sqlDataRecord.SetString(2, coverageSummary2.Label);
        sqlDataRecord.SetInt32(3, coverageSummary2.Position);
        sqlDataRecord.SetInt32(4, coverageSummary2.Total);
        sqlDataRecord.SetInt32(5, coverageSummary2.Covered);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindModuleCoverage2Table(
      string parameterName,
      IEnumerable<ModuleCoverage2> moduleCoverage2s)
    {
      moduleCoverage2s = moduleCoverage2s ?? Enumerable.Empty<ModuleCoverage2>();
      return this.BindTable(parameterName, "typ_ModuleCoverage2Table", this.BindModuleCoverage2TypeTableRows(moduleCoverage2s));
    }

    private IEnumerable<SqlDataRecord> BindModuleCoverage2TypeTableRows(
      IEnumerable<ModuleCoverage2> moduleCoverage2s)
    {
      foreach (ModuleCoverage2 moduleCoverage2 in moduleCoverage2s)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ModuleCoverage2Table);
        record.SetInt32(0, moduleCoverage2.CoverageId);
        record.SetInt32(1, moduleCoverage2.ModuleId);
        record.SetString(2, moduleCoverage2.Name);
        record.SetNullableGuid(3, moduleCoverage2.Signature);
        record.SetNullableInt32(4, moduleCoverage2.SignatureAge);
        record.SetInt32(5, moduleCoverage2.LinesCovered);
        record.SetInt32(6, moduleCoverage2.LinesPartiallyCovered);
        record.SetInt32(7, moduleCoverage2.LinesNotCovered);
        record.SetInt32(8, moduleCoverage2.BlocksCovered);
        record.SetInt32(9, moduleCoverage2.BlocksNotCovered);
        record.SetInt32(10, moduleCoverage2.BlockCount);
        record.SetInt32(11, moduleCoverage2.BlockDataLength);
        record.SetBytesPreserveNull(12, moduleCoverage2.BlockData);
        record.SetStringPreserveNull(13, moduleCoverage2.CoverageFileUrl);
        yield return record;
      }
    }

    protected SqlParameter BindFunctionCoverage2Table(
      string parameterName,
      IEnumerable<FunctionCoverage2> functionCoverage2s)
    {
      functionCoverage2s = functionCoverage2s ?? Enumerable.Empty<FunctionCoverage2>();
      return this.BindTable(parameterName, "typ_FunctionCoverage2Table", this.BindFunctionCoverage2TypeTableRows(functionCoverage2s));
    }

    private IEnumerable<SqlDataRecord> BindFunctionCoverage2TypeTableRows(
      IEnumerable<FunctionCoverage2> functionCoverage2s)
    {
      foreach (FunctionCoverage2 functionCoverage2 in functionCoverage2s)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_FunctionCoverage2Table);
        record.SetInt32(0, functionCoverage2.CoverageId);
        record.SetInt32(1, functionCoverage2.ModuleId);
        record.SetInt32(2, functionCoverage2.FunctionId);
        record.SetString(3, functionCoverage2.Name);
        record.SetStringPreserveNull(4, functionCoverage2.SourceFile);
        record.SetStringPreserveNull(5, functionCoverage2.Class);
        record.SetStringPreserveNull(6, functionCoverage2.Namespace);
        record.SetInt32(7, functionCoverage2.LinesCovered);
        record.SetInt32(8, functionCoverage2.LinesPartiallyCovered);
        record.SetInt32(9, functionCoverage2.LinesNotCovered);
        record.SetInt32(10, functionCoverage2.BlocksCovered);
        record.SetInt32(11, functionCoverage2.BlocksNotCovered);
        yield return record;
      }
    }

    protected SqlParameter BindTCMPropertyBag2Table(
      string parameterName,
      IEnumerable<TCMPropertyBag2> tcmPropertyBag2)
    {
      tcmPropertyBag2 = tcmPropertyBag2 ?? Enumerable.Empty<TCMPropertyBag2>();
      return this.BindTable(parameterName, "typ_TCMPropertyBag2Table", this.BindTCMPropertyBag2TypeTableRows(tcmPropertyBag2));
    }

    private IEnumerable<SqlDataRecord> BindTCMPropertyBag2TypeTableRows(
      IEnumerable<TCMPropertyBag2> tcmPropertyBag2)
    {
      foreach (TCMPropertyBag2 tcmPropertyBag2_1 in tcmPropertyBag2)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TCMPropertyBag2Table);
        record.SetInt32(0, tcmPropertyBag2_1.ArtifactType);
        record.SetInt32(1, tcmPropertyBag2_1.ArtifactId);
        record.SetString(2, tcmPropertyBag2_1.Name);
        record.SetStringPreserveNull(3, tcmPropertyBag2_1.Value);
        yield return record;
      }
    }

    protected SqlParameter BindPointResults2Table(
      string parameterName,
      IEnumerable<PointsResults2> pointResults2)
    {
      pointResults2 = pointResults2 ?? Enumerable.Empty<PointsResults2>();
      return this.BindTable(parameterName, "typ_PointResultsTable2", this.BindPointResults2TypeTableRows(pointResults2));
    }

    private IEnumerable<SqlDataRecord> BindPointResults2TypeTableRows(
      IEnumerable<PointsResults2> pointResults2)
    {
      foreach (PointsResults2 pointsResults2 in pointResults2)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_PointResults2Table);
        record.SetInt32(0, pointsResults2.PointId);
        record.SetInt32(1, pointsResults2.PlanId);
        record.SetInt32(2, pointsResults2.ChangeNumber);
        record.SetInt32(3, pointsResults2.LastTestRunId);
        record.SetInt32(4, pointsResults2.LastTestResultId);
        record.SetDateTime(5, pointsResults2.LastUpdated);
        record.SetNullableByte(6, pointsResults2.LastResultState);
        record.SetNullableByte(7, pointsResults2.LastResultOutcome);
        record.SetInt32(8, pointsResults2.LastResolutionStateId);
        record.SetNullableByte(9, pointsResults2.LastFailureType);
        yield return record;
      }
    }

    protected SqlParameter BindPointReference2Table(
      string parameterName,
      IEnumerable<PointsReference2> pointsReferences2)
    {
      pointsReferences2 = pointsReferences2 ?? Enumerable.Empty<PointsReference2>();
      return this.BindTable(parameterName, "typ_PointReference2Table", this.BindPointReference2TypeTableRows(pointsReferences2));
    }

    private IEnumerable<SqlDataRecord> BindPointReference2TypeTableRows(
      IEnumerable<PointsReference2> pointsReferences2)
    {
      foreach (PointsReference2 pointsReference2 in pointsReferences2)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_PointReference2Table);
        sqlDataRecord.SetInt32(0, pointsReference2.PointId);
        sqlDataRecord.SetInt32(1, pointsReference2.PlanId);
        yield return sqlDataRecord;
      }
    }

    internal virtual void SyncReleaseRefs(IEnumerable<ReleaseReference2> releaseRefs)
    {
      this.PrepareStoredProcedure("prc_SyncReleaseRefs");
      IEnumerable<Guid> projectIds = releaseRefs.Select<ReleaseReference2, Guid>((System.Func<ReleaseReference2, Guid>) (x => x.ProjectId));
      this.BindReleaseRef2TypeTable("@releaseRefs", releaseRefs, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<ReleaseReference2> FetchReleaseRefs(
      int waterMarkDataspaceId,
      int waterMarkReleaseRefId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchReleaseRefs");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkReleaseRefId", waterMarkReleaseRefId);
      this.BindInt("@batchSize", batchSize);
      List<ReleaseReference2> releaseReference2List = new List<ReleaseReference2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.ReleaseRefsColumns releaseRefsColumns = new TestManagementDatabase.ReleaseRefsColumns();
      while (reader.Read())
        releaseReference2List.Add(releaseRefsColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return releaseReference2List;
    }

    internal virtual void SyncBuildRefs(IEnumerable<BuildReference2> buildRefs)
    {
      this.PrepareStoredProcedure("prc_SyncBuildRefs");
      IEnumerable<Guid> projectIds = buildRefs.Select<BuildReference2, Guid>((System.Func<BuildReference2, Guid>) (x => x.ProjectId));
      this.BindBuildRef2TypeTable("@buildRefs", buildRefs, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<BuildReference2> FetchBuildRefs(
      int waterMarkDataspaceId,
      int waterMarkBuildRefId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchBuildRefs");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkBuildRefId", waterMarkBuildRefId);
      this.BindInt("@batchSize", batchSize);
      List<BuildReference2> buildReference2List = new List<BuildReference2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.BuildRefsColumns buildRefsColumns = new TestManagementDatabase.BuildRefsColumns();
      while (reader.Read())
        buildReference2List.Add(buildRefsColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return buildReference2List;
    }

    internal virtual void SyncTestRunContexts(IEnumerable<TestRunContext2> testRunContexts)
    {
      this.PrepareStoredProcedure("prc_SyncTestRunContexts");
      IEnumerable<Guid> projectIds = testRunContexts.Select<TestRunContext2, Guid>((System.Func<TestRunContext2, Guid>) (x => x.ProjectId));
      this.BindTestRunContext2TypeTable("@testRunContexts", testRunContexts, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestRunContext2> FetchTestRunContexts(
      int waterMarkDataspaceId,
      int waterMarkTestRunContextId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestRunContexts");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunContextId", waterMarkTestRunContextId);
      this.BindInt("@batchSize", batchSize);
      List<TestRunContext2> testRunContext2List = new List<TestRunContext2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestRunContextColumns runContextColumns = new TestManagementDatabase.TestRunContextColumns();
      while (reader.Read())
        testRunContext2List.Add(runContextColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testRunContext2List;
    }

    internal virtual void SyncTestMessageLogs(IEnumerable<TestMessageLog2> testMessageLogs)
    {
      this.PrepareStoredProcedure("prc_SyncTestMessageLogs");
      this.BindIdTypeTable("@testMessageLogs", testMessageLogs.Select<TestMessageLog2, int>((System.Func<TestMessageLog2, int>) (x => x.TestMessageLogId)));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestMessageLog2> FetchTestMessageLogs(
      int waterMarkTestMessageLogId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestMessageLogs");
      this.BindInt("@waterMarkTestMessageLogId", waterMarkTestMessageLogId);
      this.BindInt("@batchSize", batchSize);
      List<TestMessageLog2> testMessageLog2List = new List<TestMessageLog2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestMessageLogColumns messageLogColumns = new TestManagementDatabase.TestMessageLogColumns();
      while (reader.Read())
        testMessageLog2List.Add(messageLogColumns.bind(reader));
      return testMessageLog2List;
    }

    internal virtual void SyncTestCaseReferences(IEnumerable<TestCaseReference2> testCaseReferences)
    {
      this.PrepareStoredProcedure("prc_SyncTestCaseReferences");
      IEnumerable<Guid> projectIds = testCaseReferences.Select<TestCaseReference2, Guid>((System.Func<TestCaseReference2, Guid>) (x => x.ProjectId));
      this.BindTestCaseReference2TypeTable("@testCaseRefs", testCaseReferences, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestCaseReference2> FetchTestCaseReferences(
      int waterMarkDataspaceId,
      int waterMarkTestCaseRefId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestCaseReferences");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestCaseRefId", waterMarkTestCaseRefId);
      this.BindInt("@batchSize", batchSize);
      List<TestCaseReference2> testCaseReference2List = new List<TestCaseReference2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestCaseReferenceColumns referenceColumns = new TestManagementDatabase.TestCaseReferenceColumns();
      while (reader.Read())
        testCaseReference2List.Add(referenceColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testCaseReference2List;
    }

    internal virtual void SyncTestResults(IEnumerable<TestResult2> testResults)
    {
      this.PrepareStoredProcedure("prc_SyncTestResults");
      IEnumerable<Guid> projectIds = testResults.Select<TestResult2, Guid>((System.Func<TestResult2, Guid>) (x => x.ProjectId));
      this.BindTestResult2TypeTable("@testResults", testResults, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestResult2> FetchTestResults(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestResults");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@waterMarkTestResultId", waterMarkTestResultId);
      this.BindInt("@batchSize", batchSize);
      List<TestResult2> testResult2List = new List<TestResult2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestResultColumns testResultColumns = new TestManagementDatabase.TestResultColumns();
      while (reader.Read())
        testResult2List.Add(testResultColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testResult2List;
    }

    internal virtual void SyncTestMessageLogEntry(
      IEnumerable<TestMessageLogEntry2> testMessageLogEntry)
    {
      this.PrepareStoredProcedure("prc_SyncTestMessageLogEntry");
      this.BindTestMessageLogEntry2TypeTable("@testMessageLogEntry", testMessageLogEntry);
      this.ExecuteNonQuery();
    }

    internal virtual List<TestMessageLogEntry2> FetchTestMessageLogEntry(
      int waterMarkLogId,
      int waterMarkEntryId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestMessageLogEntry");
      this.BindInt("@waterMarkLogId", waterMarkLogId);
      this.BindInt("@waterMarkEntryId", waterMarkEntryId);
      this.BindInt("@batchSize", batchSize);
      List<TestMessageLogEntry2> messageLogEntry2List = new List<TestMessageLogEntry2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestMessageLogEntryColumns messageLogEntryColumns = new TestManagementDatabase.TestMessageLogEntryColumns();
      while (reader.Read())
        messageLogEntry2List.Add(messageLogEntryColumns.bind(reader));
      return messageLogEntry2List;
    }

    internal virtual void SyncTestRuns(IEnumerable<TestRun2> testRuns)
    {
      this.PrepareStoredProcedure("prc_SyncTestRuns");
      IEnumerable<Guid> projectIds = testRuns.Select<TestRun2, Guid>((System.Func<TestRun2, Guid>) (x => x.ProjectId));
      this.BindTestRun2TypeTable("@testRuns", testRuns, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestRun2> FetchTestRuns(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestRuns");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@batchSize", batchSize);
      List<TestRun2> testRun2List = new List<TestRun2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestRunColumns testRunColumns = new TestManagementDatabase.TestRunColumns();
      while (reader.Read())
        testRun2List.Add(testRunColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testRun2List;
    }

    internal virtual void SyncTestRuns2(IEnumerable<TestRun2> testRuns)
    {
      this.PrepareStoredProcedure("prc_SyncTestRuns2");
      IEnumerable<Guid> projectIds = testRuns.Select<TestRun2, Guid>((System.Func<TestRun2, Guid>) (x => x.ProjectId));
      this.BindTestRun2TypeTable("@testRuns", testRuns, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestRun2> FetchTestRuns2(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestRuns2");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@batchSize", batchSize);
      List<TestRun2> testRun2List = new List<TestRun2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestRunColumns testRunColumns = new TestManagementDatabase.TestRunColumns();
      while (reader.Read())
        testRun2List.Add(testRunColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testRun2List;
    }

    internal virtual void SyncTestRunSummary(IEnumerable<TestRunSummary2> testRunSummary)
    {
      this.PrepareStoredProcedure("prc_SyncTestRunSummary");
      IEnumerable<Guid> projectIds = testRunSummary.Select<TestRunSummary2, Guid>((System.Func<TestRunSummary2, Guid>) (x => x.ProjectId));
      this.BindTestRunSummary2TypeTable("@testRunSummary", testRunSummary, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestRunSummary2> FetchTestRunSummary(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestRunSummary");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@batchSize", batchSize);
      List<TestRunSummary2> testRunSummary2List = new List<TestRunSummary2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestRunSummaryColumns runSummaryColumns = new TestManagementDatabase.TestRunSummaryColumns();
      while (reader.Read())
        testRunSummary2List.Add(runSummaryColumns.Bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testRunSummary2List;
    }

    internal virtual void SyncTestResultsEx(IEnumerable<TestResultsEx2> testResultsEx)
    {
      this.PrepareStoredProcedure("prc_SyncTestResultsEx");
      IEnumerable<Guid> projectIds = testResultsEx.Select<TestResultsEx2, Guid>((System.Func<TestResultsEx2, Guid>) (x => x.ProjectId));
      this.BindTestResultsEx2TypeTable("@testResultsEx", testResultsEx, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestResultsEx2> FetchTestResultsEx(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int waterMarkFieldId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestResultsEx");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@waterMarkTestResultId", waterMarkTestResultId);
      this.BindInt("@waterMarkFieldId", waterMarkFieldId);
      this.BindInt("@batchSize", batchSize);
      List<TestResultsEx2> testResultsEx2List = new List<TestResultsEx2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestResultsExColumns resultsExColumns = new TestManagementDatabase.TestResultsExColumns();
      while (reader.Read())
        testResultsEx2List.Add(resultsExColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testResultsEx2List;
    }

    internal virtual void SyncTestCaseMetadata(IEnumerable<TestCaseMetadata2> testCase)
    {
      this.PrepareStoredProcedure("prc_SyncTestCaseMetadata");
      IEnumerable<Guid> projectIds = testCase.Select<TestCaseMetadata2, Guid>((System.Func<TestCaseMetadata2, Guid>) (x => x.ProjectId));
      this.BindTestCaseMetadata2TypeTable("@testCaseMetaData", testCase, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestCaseMetadata2> FetchTestCaseMetadata(
      int waterMarkDataspaceId,
      int waterMarkTestMetadataId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestCaseMetadata");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestMetadataId", waterMarkTestMetadataId);
      this.BindInt("@batchSize", batchSize);
      List<TestCaseMetadata2> testCaseMetadata2List = new List<TestCaseMetadata2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestCaseMetadataColumns caseMetadataColumns = new TestManagementDatabase.TestCaseMetadataColumns();
      while (reader.Read())
        testCaseMetadata2List.Add(caseMetadataColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testCaseMetadata2List;
    }

    internal virtual void SyncRequirementsToTestsMapping(
      IEnumerable<RequirementsToTestsMapping2> requirementMapping)
    {
      this.PrepareStoredProcedure("prc_SyncRequirementsToTestsMapping");
      IEnumerable<Guid> projectIds = requirementMapping.Select<RequirementsToTestsMapping2, Guid>((System.Func<RequirementsToTestsMapping2, Guid>) (x => x.ProjectId));
      this.BindRequirementsToTestsMapping2TypeTable("@requirementsMapping", requirementMapping, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<RequirementsToTestsMapping2> FetchRequirementsToTestsMapping(
      int waterMarkDataspaceId,
      int waterMarkWorkItemId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchRequirementsToTestsMapping");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkWorkItemId", waterMarkWorkItemId);
      this.BindInt("@batchSize", batchSize);
      List<RequirementsToTestsMapping2> testsMapping = new List<RequirementsToTestsMapping2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.RequirementsToTestsMappingColumns testsMappingColumns = new TestManagementDatabase.RequirementsToTestsMappingColumns();
      while (reader.Read())
        testsMapping.Add(testsMappingColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testsMapping;
    }

    internal virtual void SyncTestResultReset(IEnumerable<TestResultReset2> testResultResets)
    {
      this.PrepareStoredProcedure("prc_SyncTestResultReset");
      IEnumerable<Guid> projectIds = testResultResets.Select<TestResultReset2, Guid>((System.Func<TestResultReset2, Guid>) (x => x.ProjectId));
      this.BindTestResultReset2Table("@testResultReset", testResultResets, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestResultReset2> FetchTestResultReset(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int waterMarkRevision,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestResultReset");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@waterMarkTestResultId", waterMarkTestResultId);
      this.BindInt("@waterMarkRevision", waterMarkRevision);
      this.BindInt("@batchSize", batchSize);
      List<TestResultReset2> testResultReset2List = new List<TestResultReset2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestResultResetColumns resultResetColumns = new TestManagementDatabase.TestResultResetColumns();
      while (reader.Read())
        testResultReset2List.Add(resultResetColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testResultReset2List;
    }

    internal virtual void SyncTestActionResult(IEnumerable<TestActionResult2> testActionResults)
    {
      this.PrepareStoredProcedure("prc_SyncTestActionResult");
      this.BindTestActionResult2Table("@testActionResult", testActionResults);
      this.ExecuteNonQuery();
    }

    internal virtual List<TestActionResult2> FetchTestActionResult(
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int waterMarkIterationId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestActionResult");
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@waterMarkTestResultId", waterMarkTestResultId);
      this.BindInt("@waterMarkIterationId", waterMarkIterationId);
      this.BindInt("@batchSize", batchSize);
      List<TestActionResult2> testActionResult2List = new List<TestActionResult2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestActionResultColumns actionResultColumns = new TestManagementDatabase.TestActionResultColumns();
      while (reader.Read())
        testActionResult2List.Add(actionResultColumns.bind(reader));
      return testActionResult2List;
    }

    internal virtual void SyncTestRunEx(IEnumerable<TestRunEx2> testRuns)
    {
      this.PrepareStoredProcedure("prc_SyncTestRunEx");
      IEnumerable<Guid> projectIds = testRuns.Select<TestRunEx2, Guid>((System.Func<TestRunEx2, Guid>) (x => x.ProjectId));
      this.BindTestRunEx2Table("@testRunEx", testRuns, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestRunEx2> FetchTestRunEx(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int waterMarkFieldId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestRunEx");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@waterMarkFieldId", waterMarkFieldId);
      this.BindInt("@batchSize", batchSize);
      List<TestRunEx2> testRunEx2List = new List<TestRunEx2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestRunExColumns testRunExColumns = new TestManagementDatabase.TestRunExColumns();
      while (reader.Read())
        testRunEx2List.Add(testRunExColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testRunEx2List;
    }

    internal virtual void SyncTestRunExtended(IEnumerable<TestRunExtended2> testRuns)
    {
      this.PrepareStoredProcedure("prc_SyncTestRunExtended");
      IEnumerable<Guid> projectIds = testRuns.Select<TestRunExtended2, Guid>((System.Func<TestRunExtended2, Guid>) (x => x.ProjectId));
      this.BindTestRunExtended2Table("@testRunExtended", testRuns, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<TestRunExtended2> FetchTestRunExtended(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestRunExtended");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@batchSize", batchSize);
      List<TestRunExtended2> testRunExtended2List = new List<TestRunExtended2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestRunExtendedColumns runExtendedColumns = new TestManagementDatabase.TestRunExtendedColumns();
      while (reader.Read())
        testRunExtended2List.Add(runExtendedColumns.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return testRunExtended2List;
    }

    internal virtual void SyncTestParameter(IEnumerable<TestParameter2> testParameters)
    {
      this.PrepareStoredProcedure("prc_SyncTestParameter");
      this.BindTestParameter2Table("@testParameter", testParameters);
      this.ExecuteNonQuery();
    }

    internal virtual List<TestParameter2> FetchTestParameter(
      int waterMarkTestRunId,
      int waterMarkResultId,
      int waterMarkIterationId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTestParameter");
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@waterMarkTestResultId", waterMarkResultId);
      this.BindInt("@waterMarkIterationId", waterMarkIterationId);
      this.BindInt("@batchSize", batchSize);
      List<TestParameter2> testParameter2List = new List<TestParameter2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TestParameterColumns parameterColumns = new TestManagementDatabase.TestParameterColumns();
      while (reader.Read())
        testParameter2List.Add(parameterColumns.bind(reader));
      return testParameter2List;
    }

    internal virtual void SyncCoverage(IEnumerable<Coverage2> coverage2)
    {
      this.PrepareStoredProcedure("prc_SyncCoverage");
      this.BindCoverage2Table("@codeCoverages", coverage2);
      this.ExecuteNonQuery();
    }

    internal virtual List<Coverage2> FetchCoverage(int waterMarkCoverageId, int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchCoverage");
      this.BindInt("@waterMarkCoverageId", waterMarkCoverageId);
      this.BindInt("@batchSize", batchSize);
      List<Coverage2> coverage2List = new List<Coverage2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.Coverage2Columns coverage2Columns = new TestManagementDatabase.Coverage2Columns();
      while (reader.Read())
        coverage2List.Add(coverage2Columns.bind(reader));
      return coverage2List;
    }

    internal virtual void SyncCodeCoverageSummary(
      IEnumerable<CodeCoverageSummary2> codeCoverageSummary2s)
    {
      this.PrepareStoredProcedure("prc_SyncCodeCoverageSummary");
      IEnumerable<Guid> projectIds = codeCoverageSummary2s.Select<CodeCoverageSummary2, Guid>((System.Func<CodeCoverageSummary2, Guid>) (x => x.ProjectId));
      this.BindCodeCoverageSummary2Table("@codeCoverageSummary", codeCoverageSummary2s, this.getDataspaceMap(projectIds));
      this.ExecuteNonQuery();
    }

    internal virtual List<CodeCoverageSummary2> FetchCodeCoverageSummary(
      int waterMarkDataspaceId,
      int waterMarkBuildConfigurationId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchCodeCoverageSummary");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkBuildConfigurationId", waterMarkBuildConfigurationId);
      this.BindInt("@batchSize", batchSize);
      List<CodeCoverageSummary2> coverageSummary2List = new List<CodeCoverageSummary2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.CodeCoverageSummaryColumns2 coverageSummaryColumns2 = new TestManagementDatabase.CodeCoverageSummaryColumns2();
      while (reader.Read())
        coverageSummary2List.Add(coverageSummaryColumns2.bind(reader, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return coverageSummary2List;
    }

    internal virtual void SyncModuleCoverage(IEnumerable<ModuleCoverage2> moduleCoverage2s)
    {
      this.PrepareStoredProcedure("prc_SyncModuleCoverage");
      this.BindModuleCoverage2Table("@moduleCoverage", moduleCoverage2s);
      this.ExecuteNonQuery();
    }

    internal virtual List<ModuleCoverage2> FetchModuleCoverage(
      int waterMarkCoverageId,
      int waterMarkModuleId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchModuleCoverage");
      this.BindInt("@waterMarkCoverageId", waterMarkCoverageId);
      this.BindInt("@waterMarkModuleId", waterMarkModuleId);
      this.BindInt("@batchSize", batchSize);
      List<ModuleCoverage2> moduleCoverage2List = new List<ModuleCoverage2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.ModuleCoverage2Columns coverage2Columns = new TestManagementDatabase.ModuleCoverage2Columns();
      while (reader.Read())
        moduleCoverage2List.Add(coverage2Columns.bind(reader));
      return moduleCoverage2List;
    }

    internal virtual void SyncFunctionCoverage(IEnumerable<FunctionCoverage2> functionCoverage2s)
    {
      this.PrepareStoredProcedure("prc_SyncFunctionCoverage");
      this.BindFunctionCoverage2Table("@functionCoverage", functionCoverage2s);
      this.ExecuteNonQuery();
    }

    internal virtual List<FunctionCoverage2> FetchFunctionCoverage(
      int waterMarkCoverageId,
      int waterMarkModuleId,
      int waterMarkFunctionId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchFunctionCoverage");
      this.BindInt("@waterMarkCoverageId", waterMarkCoverageId);
      this.BindInt("@waterMarkModuleId", waterMarkModuleId);
      this.BindInt("@waterMarkFunctionId", waterMarkFunctionId);
      this.BindInt("@batchSize", batchSize);
      List<FunctionCoverage2> functionCoverage2List = new List<FunctionCoverage2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.FunctionCoverageColumns2 coverageColumns2 = new TestManagementDatabase.FunctionCoverageColumns2();
      while (reader.Read())
        functionCoverage2List.Add(coverageColumns2.bind(reader));
      return functionCoverage2List;
    }

    internal virtual void SyncTCMPropertyBag(IEnumerable<TCMPropertyBag2> tcmPropertyBags)
    {
      this.PrepareStoredProcedure("prc_SyncTCMPropertyBag");
      this.BindTCMPropertyBag2Table("@tcmpropertybag", tcmPropertyBags);
      this.ExecuteNonQuery();
    }

    internal virtual List<TCMPropertyBag2> FetchTCMPropertyBag(
      int waterMarkArtifactType,
      int waterMarkArtifactId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchTCMPropertyBag");
      this.BindInt("@waterMarkArtifactType", waterMarkArtifactType);
      this.BindInt("@waterMarkArtifactId", waterMarkArtifactId);
      this.BindInt("@batchSize", batchSize);
      List<TCMPropertyBag2> tcmPropertyBag2List = new List<TCMPropertyBag2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.TCMPropertyBagColumns2 propertyBagColumns2 = new TestManagementDatabase.TCMPropertyBagColumns2();
      while (reader.Read())
        tcmPropertyBag2List.Add(propertyBagColumns2.bind(reader));
      return tcmPropertyBag2List;
    }

    internal virtual void SyncPointResults(IEnumerable<PointsResults2> pointResults)
    {
      this.PrepareStoredProcedure("prc_SyncPointResults");
      this.BindPointResults2Table("@testPointResults", pointResults);
      this.ExecuteNonQuery();
    }

    internal virtual List<PointsResults2> FetchPointResults(
      int waterMarkPlanId,
      int waterMarkPointId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("prc_FetchPointResults");
      this.BindInt("@waterMarkPlanId", waterMarkPlanId);
      this.BindInt("@waterMarkPointId", waterMarkPointId);
      this.BindInt("@batchSize", batchSize);
      List<PointsResults2> pointsResults2List = new List<PointsResults2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.PointsResultsColumns2 pointsResultsColumns2 = new TestManagementDatabase.PointsResultsColumns2();
      while (reader.Read())
        pointsResults2List.Add(pointsResultsColumns2.bind(reader));
      return pointsResults2List;
    }

    internal virtual List<PointsResults2> FetchPointResults(
      IEnumerable<PointsReference2> pointReferences)
    {
      this.PrepareStoredProcedure("prc_FetchPointResults2");
      this.BindPointReference2Table("@pointReferences", pointReferences);
      List<PointsResults2> pointsResults2List = new List<PointsResults2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.PointsResultsColumns2 pointsResultsColumns2 = new TestManagementDatabase.PointsResultsColumns2();
      while (reader.Read())
        pointsResults2List.Add(pointsResultsColumns2.bind(reader));
      return pointsResults2List;
    }

    internal virtual void SyncPointOutcome(List<PointsResults2> pointResults)
    {
      this.PrepareStoredProcedure("prc_SyncPointOutcome");
      this.BindPointResults2Table("@testPointResults", (IEnumerable<PointsResults2>) pointResults);
      this.ExecuteNonQuery();
    }

    internal virtual void BackfillPointHistoryOutcome(
      int fromPlanId,
      int fromPointId,
      int fromChangeNumber,
      int batchSize,
      out int columnsUpdated,
      out int planId,
      out int pointId,
      out int changeNumber)
    {
      this.PrepareStoredProcedure("TestManagement.prc_BackfillPointHistoryOutcome");
      this.BindInt("@fromPlanId", fromPlanId);
      this.BindInt("@fromPointId", fromPointId);
      this.BindInt("@fromChangeNumber", fromChangeNumber);
      this.BindInt("@batchSize", batchSize);
      List<PointsResults2> pointsResults2List = new List<PointsResults2>();
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.PointHistoryOutcomeBackfillColumns outcomeBackfillColumns = new TestManagementDatabase.PointHistoryOutcomeBackfillColumns();
      if (reader.Read())
      {
        outcomeBackfillColumns.bind(reader, out columnsUpdated, out planId, out pointId, out changeNumber);
      }
      else
      {
        columnsUpdated = 0;
        planId = 0;
        pointId = int.MinValue;
        changeNumber = 0;
      }
    }

    internal Dictionary<Guid, int> getDataspaceMap(IEnumerable<Guid> projectIds)
    {
      Dictionary<Guid, int> dataspaceMap = new Dictionary<Guid, int>();
      foreach (Guid guid in projectIds.Distinct<Guid>())
        dataspaceMap[guid] = this.GetDataspaceIdWithLazyInitialization(guid);
      return dataspaceMap;
    }

    public virtual void AddRequirementToTestLinks(
      GuidAndString projectId,
      int workItemId,
      List<TestMethod> testMethods,
      Guid createdBy)
    {
    }

    public virtual List<int> QueryLinkedRequirementsForTest(Guid projectId, TestMethod testMethod) => new List<int>();

    public virtual void DeleteRequirementToTestLink(
      GuidAndString projectId,
      int workItemId,
      TestMethod testMethod,
      Guid deletedBy)
    {
    }

    public virtual void RestoreRequirementToTestLink(
      GuidAndString projectId,
      int workItemId,
      Guid updatedBy)
    {
    }

    public virtual void DestroyRequirementToTestLink(IEnumerable<int> workItemIds, int batchSize)
    {
    }

    public virtual void SyncRequirementTestLinks(
      Guid projectId,
      int workItemId,
      IEnumerable<int> testCaseRefIds,
      Guid updatedBy)
    {
    }

    public virtual Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForBuild(
      GuidAndString projectId,
      List<int> workItemIds,
      BuildConfiguration buildConfigurationInfo,
      string sourceWorkflow,
      int runIdThreshold = 0)
    {
      return new Dictionary<int, TestSummaryForWorkItem>();
    }

    public virtual Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForBuild2(
      GuidAndString projectId,
      List<int> testCaseRefIds,
      BuildConfiguration buildConfigurationInfo,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0)
    {
      return new Dictionary<int, AggregatedDataForResultTrend>();
    }

    public virtual Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForRelease(
      GuidAndString projectId,
      List<int> workItemIds,
      int releaseDefinitionId,
      int releaseEnvironmentDefId,
      string sourceWorkflow,
      int runIdThreshold = 0)
    {
      return new Dictionary<int, TestSummaryForWorkItem>();
    }

    public virtual Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForRelease2(
      GuidAndString projectId,
      List<int> testCaseRefIds,
      int releaseDefinitionId,
      int releaseEnvironmentDefId,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0)
    {
      return new Dictionary<int, AggregatedDataForResultTrend>();
    }

    internal virtual Dictionary<Guid, List<TestNameRequirementAssociation>> QueryNonMigratedTestToRequirementLinks(
      int batchSize)
    {
      return new Dictionary<Guid, List<TestNameRequirementAssociation>>();
    }

    internal virtual void UpdateTestToRequirementLinkMigrationStatus(
      Guid projectGuid,
      List<TestNameRequirementAssociation> associations)
    {
    }

    public virtual void DeleteTestReleases(
      Guid projectGuid,
      List<int> releaseIds,
      Guid lastUpdatedBy,
      int testRunDeletionBatchSize,
      bool isTcmService)
    {
    }

    public virtual ReleaseReference GetReleaseRef(
      Guid projectGuid,
      int releaseId,
      int releaseEnvironmentId)
    {
      return (ReleaseReference) null;
    }

    protected SqlParameter BindTestCaseResultTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "typ_TestCaseResultTypeTable", this.BindTestCaseResultTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResultTypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResultTypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestCaseId);
        record.SetInt32(2, result.TestPointId);
        record.SetInt32(3, result.ConfigurationId);
        record.SetGuid(4, result.Owner);
        record.SetByte(5, result.Priority);
        record.SetString(6, result.TestCaseTitle ?? string.Empty);
        record.SetString(7, result.TestCaseAreaUri ?? string.Empty);
        record.SetInt32(8, result.TestCaseRevision);
        record.SetString(9, result.AutomatedTestName ?? string.Empty);
        record.SetString(10, result.AutomatedTestStorage ?? string.Empty);
        record.SetString(11, result.AutomatedTestType ?? string.Empty);
        record.SetStringPreserveNull(12, result.AutomatedTestTypeId);
        record.SetString(13, result.AutomatedTestId ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindTestCaseResult2TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "typ_TestCaseResult2TypeTable", this.BindTestCaseResult2TypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResult2TypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResult2TypeTable);
        record.SetInt32(0, indexValue++);
        record.SetInt32(1, result.TestCaseId);
        record.SetInt32(2, result.TestPointId);
        record.SetInt32(3, result.ConfigurationId);
        record.SetGuid(4, result.Owner);
        record.SetByte(5, result.Priority);
        record.SetString(6, result.TestCaseTitle ?? string.Empty);
        record.SetString(7, result.TestCaseAreaUri ?? string.Empty);
        record.SetInt32(8, result.TestCaseRevision);
        record.SetString(9, result.AutomatedTestName ?? string.Empty);
        record.SetString(10, result.AutomatedTestStorage ?? string.Empty);
        record.SetString(11, result.AutomatedTestType ?? string.Empty);
        record.SetStringPreserveNull(12, result.AutomatedTestTypeId);
        record.SetString(13, result.AutomatedTestId ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindTestCaseResult3TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId = false)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "typ_TestCaseResult3TypeTable", this.BindTestCaseResult3TypeTableRows(results, ignoreResultId));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResult3TypeTableRows(
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId)
    {
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext);
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResult3TypeTable);
        int num = indexValue++;
        record.SetInt32(0, num);
        record.SetInt32(1, ignoreResultId ? num : result.TestResultId);
        record.SetInt32(2, result.TestCaseId);
        record.SetInt32(3, result.TestPointId);
        record.SetInt32(4, result.ConfigurationId);
        record.SetGuid(5, result.Owner);
        record.SetByte(6, result.Priority);
        record.SetString(7, result.TestCaseTitle ?? string.Empty);
        record.SetString(8, result.TestCaseAreaUri ?? string.Empty);
        record.SetInt32(9, result.TestCaseRevision);
        record.SetString(10, result.AutomatedTestName ?? string.Empty);
        record.SetString(11, result.AutomatedTestStorage ?? string.Empty);
        record.SetString(12, result.AutomatedTestType ?? string.Empty);
        record.SetString(13, result.AutomatedTestTypeId ?? string.Empty);
        record.SetString(14, result.AutomatedTestId ?? string.Empty);
        record.SetByte(15, result.State == (byte) 0 ? (byte) 1 : result.State);
        record.SetByte(16, result.Outcome == (byte) 0 ? (byte) 1 : result.Outcome);
        record.SetInt32(17, result.ResolutionStateId < 0 ? 0 : result.ResolutionStateId);
        record.SetByte(18, result.FailureType);
        record.SetString(19, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : string.Empty);
        record.SetString(20, result.ErrorMessage ?? string.Empty);
        record.SetString(21, result.ComputerName ?? string.Empty);
        record.SetDateTimePreserveNull(22, result.DateStarted);
        record.SetDateTimePreserveNull(23, result.DateCompleted);
        record.SetInt64(24, result.Duration);
        record.SetGuid(25, result.RunBy);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResultTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId = false)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResultTypeTable", this.BindTestResult_TestCaseResultTypeTableRows(results, ignoreResultId));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResultTypeTableRows(
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId)
    {
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext);
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResultTypeTable);
        int num = indexValue++;
        record.SetInt32(0, num);
        record.SetInt32(1, ignoreResultId ? num : result.TestResultId);
        record.SetInt32(2, result.TestCaseId);
        record.SetInt32(3, result.TestPointId);
        record.SetInt32(4, result.ConfigurationId);
        record.SetGuid(5, result.Owner);
        record.SetByte(6, result.Priority);
        record.SetString(7, result.TestCaseTitle ?? string.Empty);
        record.SetString(8, result.TestCaseAreaUri ?? string.Empty);
        record.SetInt32(9, result.TestCaseRevision);
        record.SetString(10, result.AutomatedTestName ?? string.Empty);
        record.SetString(11, result.AutomatedTestStorage ?? string.Empty);
        record.SetString(12, result.AutomatedTestType ?? string.Empty);
        record.SetString(13, result.AutomatedTestTypeId ?? string.Empty);
        record.SetString(14, result.AutomatedTestId ?? string.Empty);
        record.SetByte(15, result.State == (byte) 0 ? (byte) 1 : result.State);
        record.SetByte(16, result.Outcome == (byte) 0 ? (byte) 1 : result.Outcome);
        record.SetInt32(17, result.ResolutionStateId < 0 ? 0 : result.ResolutionStateId);
        record.SetByte(18, result.FailureType);
        record.SetString(19, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : string.Empty);
        record.SetString(20, result.ErrorMessage ?? string.Empty);
        record.SetString(21, result.ComputerName ?? string.Empty);
        record.SetDateTimePreserveNull(22, result.DateStarted);
        record.SetDateTimePreserveNull(23, result.DateCompleted);
        record.SetInt64(24, result.Duration);
        record.SetGuid(25, result.RunBy);
        record.SetString(26, result.OwnerName ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResult2TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId = false)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResult2TypeTable", this.BindTestResult_TestCaseResult2TypeTableRows(results, ignoreResultId));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResult2TypeTableRows(
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId)
    {
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResult2TypeTable);
        int num = indexValue++;
        record.SetInt32(0, num);
        record.SetInt32(1, ignoreResultId ? num : result.TestResultId);
        record.SetInt32(2, result.TestCaseReferenceId);
        record.SetInt32(3, result.TestPointId);
        record.SetInt32(4, result.ConfigurationId);
        record.SetGuid(5, result.Owner);
        record.SetByte(6, result.State == (byte) 0 ? (byte) 1 : result.State);
        record.SetByte(7, result.Outcome == (byte) 0 ? (byte) 1 : result.Outcome);
        record.SetInt32(8, result.ResolutionStateId < 0 ? 0 : result.ResolutionStateId);
        record.SetByte(9, result.FailureType);
        record.SetString(10, result.ComputerName ?? string.Empty);
        record.SetDateTimePreserveNull(11, result.DateStarted);
        record.SetDateTimePreserveNull(12, result.DateCompleted);
        record.SetInt64(13, result.Duration);
        record.SetGuid(14, result.RunBy);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResult3TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId = false)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResult3TypeTable", this.BindTestResult_TestCaseResult3TypeTableRows(results, ignoreResultId));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResult3TypeTableRows(
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId)
    {
      TestManagementDatabase managementDatabase = this;
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(managementDatabase.RequestContext);
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResult3TypeTable);
        int num = indexValue++;
        record.SetInt32(0, num);
        record.SetInt32(1, ignoreResultId ? num : result.TestResultId);
        record.SetInt32(2, result.TestCaseId);
        record.SetInt32(3, result.TestPointId);
        record.SetInt32(4, result.ConfigurationId);
        record.SetGuid(5, result.Owner);
        record.SetByte(6, result.Priority);
        record.SetString(7, result.TestCaseTitle ?? string.Empty);
        record.SetString(8, result.TestCaseAreaUri ?? string.Empty);
        record.SetInt32(9, result.TestCaseRevision);
        record.SetString(10, result.AutomatedTestName ?? string.Empty);
        record.SetString(11, result.AutomatedTestStorage ?? string.Empty);
        record.SetString(12, result.AutomatedTestType ?? string.Empty);
        record.SetString(13, result.AutomatedTestTypeId ?? string.Empty);
        record.SetString(14, result.AutomatedTestId ?? string.Empty);
        record.SetByte(15, result.State == (byte) 0 ? (byte) 1 : result.State);
        record.SetByte(16, result.Outcome == (byte) 0 ? (byte) 1 : result.Outcome);
        record.SetInt32(17, result.ResolutionStateId < 0 ? 0 : result.ResolutionStateId);
        record.SetByte(18, result.FailureType);
        record.SetString(19, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : string.Empty);
        record.SetString(20, result.ErrorMessage ?? string.Empty);
        record.SetString(21, result.ComputerName ?? string.Empty);
        record.SetDateTimePreserveNull(22, result.DateStarted);
        record.SetDateTimePreserveNull(23, result.DateCompleted);
        record.SetInt64(24, result.Duration);
        record.SetGuid(25, result.RunBy);
        record.SetString(26, result.OwnerName ?? string.Empty);
        record.SetBytes(27, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        record.SetBytes(28, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResult4TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId = false)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResult4TypeTable", this.BindTestResult_TestCaseResult4TypeTableRows(results, ignoreResultId));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResult4TypeTableRows(
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId)
    {
      TestManagementDatabase managementDatabase = this;
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(managementDatabase.RequestContext);
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResult4TypeTable);
        int num = indexValue++;
        record.SetInt32(0, num);
        record.SetInt32(1, ignoreResultId ? num : result.TestResultId);
        record.SetInt32(2, result.TestCaseId);
        record.SetInt32(3, result.TestPointId);
        record.SetInt32(4, result.ConfigurationId);
        record.SetGuid(5, result.Owner);
        record.SetByte(6, result.Priority);
        record.SetString(7, result.TestCaseTitle ?? string.Empty);
        record.SetString(8, result.TestCaseAreaUri ?? string.Empty);
        record.SetInt32(9, result.TestCaseRevision);
        record.SetString(10, result.AutomatedTestName ?? string.Empty);
        record.SetString(11, result.AutomatedTestStorage ?? string.Empty);
        record.SetString(12, result.AutomatedTestType ?? string.Empty);
        record.SetString(13, result.AutomatedTestTypeId ?? string.Empty);
        record.SetString(14, result.AutomatedTestId ?? string.Empty);
        record.SetByte(15, result.State == (byte) 0 ? (byte) 1 : result.State);
        record.SetByte(16, result.Outcome == (byte) 0 ? (byte) 1 : result.Outcome);
        record.SetInt32(17, result.ResolutionStateId < 0 ? 0 : result.ResolutionStateId);
        record.SetByte(18, result.FailureType);
        record.SetString(19, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : string.Empty);
        record.SetString(20, result.ErrorMessage ?? string.Empty);
        record.SetString(21, result.ComputerName ?? string.Empty);
        record.SetDateTimePreserveNull(22, result.DateStarted);
        record.SetDateTimePreserveNull(23, result.DateCompleted);
        record.SetInt64(24, result.Duration);
        record.SetGuid(25, result.RunBy);
        record.SetString(26, result.OwnerName ?? string.Empty);
        record.SetBytes(27, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        record.SetBytes(28, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResult5TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId = false)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResult5TypeTable", this.BindTestResult_TestCaseResult5TypeTableRows(results, ignoreResultId));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResult5TypeTableRows(
      IEnumerable<TestCaseResult> results,
      bool ignoreResultId)
    {
      TestManagementDatabase managementDatabase = this;
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(managementDatabase.RequestContext);
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResult5TypeTable);
        int num = indexValue++;
        record.SetInt32(0, num);
        record.SetInt32(1, ignoreResultId ? num : result.TestResultId);
        record.SetInt32(2, result.TestCaseId);
        record.SetInt32(3, result.TestPointId);
        record.SetInt32(4, result.ConfigurationId);
        record.SetGuid(5, result.Owner);
        record.SetByte(6, result.Priority);
        record.SetString(7, result.TestCaseTitle ?? string.Empty);
        record.SetInt32(8, result.AreaId);
        record.SetInt32(9, result.TestCaseRevision);
        record.SetString(10, result.AutomatedTestName ?? string.Empty);
        record.SetString(11, result.AutomatedTestStorage ?? string.Empty);
        record.SetString(12, result.AutomatedTestType ?? string.Empty);
        record.SetString(13, result.AutomatedTestTypeId ?? string.Empty);
        record.SetString(14, result.AutomatedTestId ?? string.Empty);
        record.SetByte(15, result.State == (byte) 0 ? (byte) 1 : result.State);
        record.SetByte(16, result.Outcome == (byte) 0 ? (byte) 1 : result.Outcome);
        record.SetInt32(17, result.ResolutionStateId < 0 ? 0 : result.ResolutionStateId);
        record.SetByte(18, result.FailureType);
        record.SetString(19, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : string.Empty);
        record.SetString(20, result.ErrorMessage ?? string.Empty);
        record.SetString(21, result.ComputerName ?? string.Empty);
        record.SetDateTimePreserveNull(22, result.DateStarted);
        record.SetDateTimePreserveNull(23, result.DateCompleted);
        record.SetInt64(24, result.Duration);
        record.SetGuid(25, result.RunBy);
        record.SetString(26, result.OwnerName ?? string.Empty);
        record.SetBytes(27, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        record.SetBytes(28, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_PlannedMetadataIdTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_PlannedMetadataIdTypeTable", this.BindTestResult_PlannedMetadataIdTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_PlannedMetadataIdTypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_PlannedMetadataIdTypeTable);
        sqlDataRecord.SetInt32(0, result.TestPlanId);
        sqlDataRecord.SetInt32(1, result.TestPointId);
        sqlDataRecord.SetInt32(2, result.ConfigurationId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter Bind_AutomatedTestDetailsTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "dbo.typ_AutomatedTestDetailsTypeTable", this.Bind_AutomatedTestDetailsTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> Bind_AutomatedTestDetailsTypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_AutomatedTestDetailsTypeTable);
        sqlDataRecord.SetInt32(0, result.TestRunId);
        sqlDataRecord.SetInt32(1, result.TestResultId);
        sqlDataRecord.SetBytes(2, 0L, this.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        sqlDataRecord.SetBytes(3, 0L, this.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestCaseResultForUpdateTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "typ_TestCaseResultForUpdateTypeTable", this.BindTestCaseResultForUpdateTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResultForUpdateTypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext);
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResultForUpdateTypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestPointId);
        record.SetGuidPreserveNull(2, result.Owner);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        record.SetBytePreserveNull(4, result.State, (byte) 0);
        record.SetBytePreserveNull(5, result.Outcome, (byte) 0);
        record.SetIntPreserveNull(6, result.ResolutionStateId, -1);
        record.SetStringPreserveNull(7, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : result.Comment);
        record.SetStringPreserveNull(8, result.ErrorMessage);
        record.SetStringPreserveNull(9, result.ComputerName);
        record.SetDateTimePreserveNull(10, result.DateStarted);
        record.SetDateTimePreserveNull(11, result.DateCompleted);
        record.SetLongPreserveNull(12, result.Duration, 0L);
        record.SetInt32(13, result.Revision);
        record.SetGuidPreserveNull(14, result.RunBy);
        record.SetBytePreserveNull(15, result.FailureType, (byte) 5);
        record.SetStringPreserveNull(16, result.AutomatedTestTypeId);
        yield return record;
      }
    }

    protected SqlParameter BindTestCaseResultUpdateTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "typ_TestCaseResultUpdateTypeTable", this.BindTestCaseResultUpdateTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResultUpdateTypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext);
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResultUpdateTypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestPointId);
        record.SetGuidPreserveNull(2, result.Owner);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        record.SetBytePreserveNull(4, result.State, (byte) 0);
        record.SetBytePreserveNull(5, result.Outcome, (byte) 0);
        record.SetIntPreserveNull(6, result.ResolutionStateId, -1);
        record.SetStringPreserveNull(7, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : result.Comment);
        record.SetStringPreserveNull(8, result.ErrorMessage);
        record.SetStringPreserveNull(9, result.ComputerName);
        record.SetDateTimePreserveNull(10, result.DateStarted);
        record.SetDateTimePreserveNull(11, result.DateCompleted);
        record.SetLongPreserveNull(12, result.Duration, 0L);
        record.SetInt32(13, result.Revision);
        record.SetInt32(14, result.TestCaseRevision);
        record.SetGuidPreserveNull(15, result.RunBy);
        record.SetBytePreserveNull(16, result.FailureType, (byte) 5);
        record.SetStringPreserveNull(17, result.AutomatedTestTypeId);
        yield return record;
      }
    }

    protected SqlParameter BindTestCaseResultUpdate2TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "typ_TestCaseResultUpdate2TypeTable", this.BindTestCaseResultUpdate2TypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResultUpdate2TypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext);
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResultUpdate2TypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestPointId);
        record.SetGuidPreserveNull(2, result.Owner);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        record.SetBytePreserveNull(4, result.State, (byte) 0);
        record.SetBytePreserveNull(5, result.Outcome, (byte) 0);
        record.SetIntPreserveNull(6, result.ResolutionStateId, -1);
        record.SetStringPreserveNull(7, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : result.Comment);
        record.SetStringPreserveNull(8, result.ErrorMessage);
        record.SetStringPreserveNull(9, result.ComputerName);
        record.SetDateTimePreserveNull(10, result.DateStarted);
        record.SetDateTimePreserveNull(11, result.DateCompleted);
        record.SetLongPreserveNull(12, result.Duration, 0L);
        record.SetInt32(13, result.Revision);
        record.SetInt32(14, result.TestCaseRevision);
        record.SetGuidPreserveNull(15, result.RunBy);
        record.SetBytePreserveNull(16, result.FailureType, (byte) 5);
        record.SetStringPreserveNull(17, result.AutomatedTestTypeId);
        record.SetStringPreserveNull(18, result.AutomatedTestName);
        record.SetStringPreserveNull(19, result.AutomatedTestStorage);
        record.SetStringPreserveNull(20, result.AutomatedTestType);
        record.SetStringPreserveNull(21, result.AutomatedTestId);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResultUpdate3TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResultUpdate3TypeTable", this.BindTestResult_TestCaseResultUpdate3TypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResultUpdate3TypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      TestManagementDatabase managementDatabase = this;
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(managementDatabase.RequestContext);
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResultUpdate3TypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestPointId);
        record.SetGuid(2, result.Owner, result.OwnerName == null);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        record.SetBytePreserveNull(4, result.State, (byte) 0);
        record.SetBytePreserveNull(5, result.Outcome, (byte) 0);
        record.SetIntPreserveNull(6, result.ResolutionStateId, -1);
        record.SetStringPreserveNull(7, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : result.Comment);
        record.SetStringPreserveNull(8, result.ErrorMessage);
        record.SetStringPreserveNull(9, result.ComputerName);
        record.SetDateTimePreserveNull(10, result.DateStarted);
        record.SetDateTimePreserveNull(11, result.DateCompleted);
        record.SetLongPreserveNull(12, result.Duration, 0L);
        record.SetInt32(13, result.Revision);
        record.SetIntPreserveNull(14, result.TestCaseRevision, 0);
        record.SetGuidPreserveNull(15, result.RunBy);
        record.SetBytePreserveNull(16, result.FailureType, (byte) 5);
        record.SetStringPreserveNull(17, result.AutomatedTestTypeId);
        record.SetStringPreserveNull(18, result.AutomatedTestName);
        record.SetStringPreserveNull(19, result.AutomatedTestStorage);
        record.SetStringPreserveNull(20, result.AutomatedTestType);
        record.SetStringPreserveNull(21, result.AutomatedTestId);
        record.SetString(22, result.OwnerName ?? string.Empty);
        record.SetBytes(23, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        record.SetBytes(24, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResultUpdateTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResultUpdateTypeTable", this.BindTestResult_TestCaseResultUpdateTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResultUpdateTypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext);
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResultUpdateTypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestPointId);
        record.SetGuidPreserveNull(2, result.Owner);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        record.SetBytePreserveNull(4, result.State, (byte) 0);
        record.SetBytePreserveNull(5, result.Outcome, (byte) 0);
        record.SetIntPreserveNull(6, result.ResolutionStateId, -1);
        record.SetStringPreserveNull(7, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : result.Comment);
        record.SetStringPreserveNull(8, result.ErrorMessage);
        record.SetStringPreserveNull(9, result.ComputerName);
        record.SetDateTimePreserveNull(10, result.DateStarted);
        record.SetDateTimePreserveNull(11, result.DateCompleted);
        record.SetLongPreserveNull(12, result.Duration, 0L);
        record.SetInt32(13, result.Revision);
        record.SetIntPreserveNull(14, result.TestCaseRevision, 0);
        record.SetGuidPreserveNull(15, result.RunBy);
        record.SetBytePreserveNull(16, result.FailureType, (byte) 5);
        record.SetStringPreserveNull(17, result.AutomatedTestTypeId);
        record.SetStringPreserveNull(18, result.AutomatedTestName);
        record.SetStringPreserveNull(19, result.AutomatedTestStorage);
        record.SetStringPreserveNull(20, result.AutomatedTestType);
        record.SetStringPreserveNull(21, result.AutomatedTestId);
        record.SetString(22, result.OwnerName ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResultFieldValueHashTypeTable(
      string parameterName,
      IEnumerable<TestResultField> results)
    {
      results = results ?? Enumerable.Empty<TestResultField>();
      return this.BindTable(parameterName, "TestResult.typ_TestResultsFieldHashTable", this.BindTestResult_TestCaseResultFieldValueHashTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResultFieldValueHashTypeTableRows(
      IEnumerable<TestResultField> results)
    {
      foreach (TestResultField result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_TestCaseResultFieldValueHashTypeTable);
        sqlDataRecord.SetInt32(0, result.TestRunId);
        sqlDataRecord.SetInt32(1, result.TestResultId);
        sqlDataRecord.SetInt32(2, result.TestSubResultId);
        sqlDataRecord.SetInt32(3, result.FieldId);
        sqlDataRecord.SetBytes(4, 0L, this.GetSHA256Hash(result.FieldValue ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResultUpdate2TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResultUpdate2TypeTable", this.BindTestResult_TestCaseResultUpdate2TypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResultUpdate2TypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      TestManagementDatabase managementDatabase = this;
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(managementDatabase.RequestContext);
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResultUpdate2TypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestPointId);
        record.SetGuid(2, result.Owner, result.OwnerName == null);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        record.SetBytePreserveNull(4, result.State, (byte) 0);
        record.SetBytePreserveNull(5, result.Outcome, (byte) 0);
        record.SetIntPreserveNull(6, result.ResolutionStateId, -1);
        record.SetStringPreserveNull(7, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : result.Comment);
        record.SetStringPreserveNull(8, result.ErrorMessage);
        record.SetStringPreserveNull(9, result.ComputerName);
        record.SetDateTimePreserveNull(10, result.DateStarted);
        record.SetDateTimePreserveNull(11, result.DateCompleted);
        record.SetLongPreserveNull(12, result.Duration, 0L);
        record.SetInt32(13, result.Revision);
        record.SetIntPreserveNull(14, result.TestCaseRevision, 0);
        record.SetGuidPreserveNull(15, result.RunBy);
        record.SetBytePreserveNull(16, result.FailureType, (byte) 5);
        record.SetStringPreserveNull(17, result.AutomatedTestTypeId);
        record.SetStringPreserveNull(18, result.AutomatedTestName);
        record.SetStringPreserveNull(19, result.AutomatedTestStorage);
        record.SetStringPreserveNull(20, result.AutomatedTestType);
        record.SetStringPreserveNull(21, result.AutomatedTestId);
        record.SetString(22, result.OwnerName ?? string.Empty);
        record.SetBytes(23, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        record.SetBytes(24, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseResultUpdate2_2TypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseResultUpdate2TypeTable", this.BindTestResult_TestCaseResultUpdate2_2TypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResultUpdate2_2TypeTableRows(
      IEnumerable<TestCaseResult> results)
    {
      TestManagementDatabase managementDatabase = this;
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(managementDatabase.RequestContext);
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseResultUpdate2_2TypeTable);
        record.SetInt32(0, result.TestResultId);
        record.SetInt32(1, result.TestPointId);
        record.SetGuid(2, result.Owner, result.OwnerName == null);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        record.SetBytePreserveNull(4, result.State, (byte) 0);
        record.SetBytePreserveNull(5, result.Outcome, (byte) 0);
        record.SetIntPreserveNull(6, result.ResolutionStateId, -1);
        record.SetStringPreserveNull(7, !string.IsNullOrEmpty(result.Comment) ? result.Comment.Substring(0, result.Comment.Length > maxCommentSize ? maxCommentSize : result.Comment.Length) : result.Comment);
        record.SetStringPreserveNull(8, result.ErrorMessage);
        record.SetStringPreserveNull(9, result.ComputerName);
        record.SetDateTimePreserveNull(10, result.DateStarted);
        record.SetDateTimePreserveNull(11, result.DateCompleted);
        record.SetLongPreserveNull(12, result.Duration, 0L);
        record.SetInt32(13, result.Revision);
        record.SetIntPreserveNull(14, result.TestCaseRevision, 0);
        record.SetGuidPreserveNull(15, result.RunBy);
        record.SetBytePreserveNull(16, result.FailureType, (byte) 5);
        record.SetStringPreserveNull(17, result.AutomatedTestTypeId);
        record.SetStringPreserveNull(18, result.AutomatedTestName);
        record.SetStringPreserveNull(19, result.AutomatedTestStorage);
        record.SetStringPreserveNull(20, result.AutomatedTestType);
        record.SetStringPreserveNull(21, result.AutomatedTestId);
        record.SetStringPreserveNull(22, result.OwnerName);
        record.SetBytes(23, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        record.SetBytes(24, 0L, managementDatabase.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return record;
      }
    }

    protected SqlParameter BindTestResultAttachmentTypeTable(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_TestResultAttachmentTypeTable", this.BindTestResultAttachmentTypeTableRows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindTestResultAttachmentTypeTableRows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestResultAttachmentTypeTable);
        sqlDataRecord.SetInt32(0, attachment.TestRunId);
        sqlDataRecord.SetInt32(1, attachment.TestResultId);
        sqlDataRecord.SetInt32(2, attachment.IterationId);
        sqlDataRecord.SetString(3, attachment.ActionPath ?? string.Empty);
        sqlDataRecord.SetString(4, attachment.FileName ?? string.Empty);
        sqlDataRecord.SetString(5, attachment.Comment ?? string.Empty);
        sqlDataRecord.SetString(6, attachment.AttachmentType ?? string.Empty);
        sqlDataRecord.SetGuid(7, attachment.TmiRunId);
        sqlDataRecord.SetInt32(8, attachment.SessionId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResultAttachmentTypeTable2(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_TestResultAttachmentTypeTable2", this.BindTestResultAttachmentTypeTable2Rows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindTestResultAttachmentTypeTable2Rows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestResultAttachmentTypeTable2);
        sqlDataRecord.SetInt32(0, attachment.TestRunId);
        sqlDataRecord.SetInt32(1, attachment.TestResultId);
        sqlDataRecord.SetInt32(2, attachment.IterationId);
        sqlDataRecord.SetString(3, attachment.ActionPath ?? string.Empty);
        sqlDataRecord.SetString(4, attachment.FileName ?? string.Empty);
        sqlDataRecord.SetString(5, attachment.Comment ?? string.Empty);
        sqlDataRecord.SetString(6, attachment.AttachmentType ?? string.Empty);
        sqlDataRecord.SetGuid(7, attachment.TmiRunId);
        sqlDataRecord.SetInt32(8, attachment.SessionId);
        sqlDataRecord.SetInt32(9, attachment.Id);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResultAttachmentTypeTable3(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_TestResultAttachmentTypeTable3", this.BindTestResultAttachmentTypeTable3Rows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindTestResultAttachmentTypeTable3Rows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultAttachmentTypeTable3);
        record.SetInt32(0, attachment.TestRunId);
        record.SetInt32(1, attachment.TestResultId);
        record.SetInt32(2, attachment.IterationId);
        record.SetNullableStringAsEmpty(3, attachment.ActionPath);
        record.SetInt32(4, attachment.SubResultId);
        record.SetNullableStringAsEmpty(5, attachment.FileName);
        record.SetNullableStringAsEmpty(6, attachment.Comment);
        record.SetNullableStringAsEmpty(7, attachment.AttachmentType);
        record.SetGuid(8, attachment.TmiRunId);
        record.SetInt32(9, attachment.SessionId);
        record.SetInt32(10, attachment.Id);
        yield return record;
      }
    }

    protected SqlParameter BindTestResultAttachmentTypeTable4(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_TestResultAttachmentTypeTable4", this.BindTestResultAttachmentTypeTable4Rows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindTestResultAttachmentTypeTable4Rows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultAttachmentTypeTable4);
        record.SetInt32(0, attachment.TestRunId);
        record.SetInt32(1, attachment.TestResultId);
        record.SetInt32(2, attachment.IterationId);
        record.SetNullableStringAsEmpty(3, attachment.ActionPath);
        record.SetInt32(4, attachment.SubResultId);
        record.SetInt32(5, attachment.FileId);
        record.SetNullableStringAsEmpty(6, attachment.FileName);
        record.SetNullableStringAsEmpty(7, attachment.Comment);
        record.SetNullableStringAsEmpty(8, attachment.AttachmentType);
        record.SetGuid(9, attachment.TmiRunId);
        record.SetInt32(10, attachment.SessionId);
        record.SetBoolean(11, attachment.IsComplete);
        record.SetInt64(12, attachment.Length);
        record.SetInt32(13, attachment.Id);
        yield return record;
      }
    }

    protected SqlParameter BindTestResultAttachmentTypeTable5(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_TestResultAttachmentTypeTable5", this.BindTestResultAttachmentTypeTable5Rows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindTestResultAttachmentTypeTable5Rows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultAttachmentTypeTable5);
        record.SetInt32(0, attachment.TestRunId);
        record.SetInt32(1, attachment.TestResultId);
        record.SetInt32(2, attachment.SubResultId);
        record.SetInt32(3, attachment.Id);
        record.SetNullableStringAsEmpty(4, attachment.FileName);
        record.SetNullableStringAsEmpty(5, attachment.AttachmentType);
        record.SetDateTime(6, attachment.CreationDate);
        record.SetBoolean(7, attachment.IsComplete);
        record.SetInt64(8, attachment.Length);
        yield return record;
      }
    }

    protected SqlParameter BindLogStoreAttachmentTable(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_LogStoreAttachmentTypeTable", this.BindLogStoreAttachmentTableRows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindLogStoreAttachmentTableRows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_LogStoreAttachmentTable);
        record.SetInt32(0, attachment.TestRunId);
        record.SetInt32(1, attachment.TestResultId);
        record.SetInt32(2, attachment.SubResultId);
        record.SetNullableStringAsEmpty(3, attachment.FileName);
        record.SetNullableStringAsEmpty(4, attachment.AttachmentType);
        yield return record;
      }
    }

    protected SqlParameter BindLogStoreAttachmentMappingTable(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_LogStoreAttachmentTypeMappingTable", this.BindLogStoreAttachmentMappingTableRows(attachments));
    }

    protected SqlParameter BindLogStoreAttachmentMappingV2Table(
      string parameterName,
      IEnumerable<TestResultAttachment> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachment>();
      return this.BindTable(parameterName, "typ_LogStoreAttachmentTypeMappingV2Table", this.BindLogStoreAttachmentMappingV2TableRows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindLogStoreAttachmentMappingTableRows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_LogStoreAttachmentMappingTable);
        record.SetInt32(0, attachment.TestRunId);
        record.SetInt32(1, attachment.TestResultId);
        record.SetInt32(2, attachment.SubResultId);
        record.SetNullableStringAsEmpty(3, attachment.FileName);
        record.SetNullableStringAsEmpty(4, attachment.AttachmentType);
        record.SetInt64(5, attachment.Length);
        yield return record;
      }
    }

    private IEnumerable<SqlDataRecord> BindLogStoreAttachmentMappingV2TableRows(
      IEnumerable<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_LogStoreAttachmentMappingV2Table);
        record.SetInt32(0, attachment.TestRunId);
        record.SetInt32(1, attachment.TestResultId);
        record.SetInt32(2, attachment.SubResultId);
        record.SetNullableStringAsEmpty(3, attachment.FileName);
        record.SetNullableStringAsEmpty(4, attachment.AttachmentType);
        record.SetInt64(5, attachment.Length);
        record.SetInt32(6, attachment.IterationId);
        record.SetString(7, attachment.ActionPath ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindTestResultAttachmentIdentityTypeTable(
      string parameterName,
      IEnumerable<TestResultAttachmentIdentity> attachments)
    {
      attachments = attachments ?? Enumerable.Empty<TestResultAttachmentIdentity>();
      return this.BindTable(parameterName, "typ_TestResultAttachmentIdentityTypeTable", this.BindTestResultAttachmentIdentityTypeTableRows(attachments));
    }

    private IEnumerable<SqlDataRecord> BindTestResultAttachmentIdentityTypeTableRows(
      IEnumerable<TestResultAttachmentIdentity> attachments)
    {
      foreach (TestResultAttachmentIdentity attachment in attachments)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestResultAttachmentIdentityTypeTable);
        sqlDataRecord.SetInt32(0, attachment.TestRunId);
        sqlDataRecord.SetInt32(1, attachment.TestResultId);
        sqlDataRecord.SetInt32(2, attachment.AttachmentId);
        sqlDataRecord.SetInt32(3, attachment.SessionId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindNameValuePairTypeTable(
      string parameterName,
      IEnumerable<NameValuePair> pairs)
    {
      pairs = pairs ?? Enumerable.Empty<NameValuePair>();
      return this.BindTable(parameterName, "typ_NameValuePairTypeTable", this.BindNameValuePairTypeTableRows(pairs));
    }

    private IEnumerable<SqlDataRecord> BindNameValuePairTypeTableRows(
      IEnumerable<NameValuePair> pairs)
    {
      foreach (NameValuePair pair in pairs)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_NameValuePairTypeTable);
        sqlDataRecord.SetString(0, pair.Name ?? string.Empty);
        sqlDataRecord.SetString(1, pair.Value ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestMessageLogEntryTypeTable(
      string parameterName,
      IEnumerable<TestMessageLogEntry> entries)
    {
      entries = entries ?? Enumerable.Empty<TestMessageLogEntry>();
      return this.BindTable(parameterName, "typ_TestMessageLogEntryTypeTable", this.BindTestMessageLogEntryTypeTableRows(entries));
    }

    private IEnumerable<SqlDataRecord> BindTestMessageLogEntryTypeTableRows(
      IEnumerable<TestMessageLogEntry> entries)
    {
      foreach (TestMessageLogEntry entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestMessageLogEntryTypeTable);
        sqlDataRecord.SetGuid(0, entry.LogUser);
        sqlDataRecord.SetDateTime(1, entry.DateCreated.ToUniversalTime());
        sqlDataRecord.SetByte(2, entry.LogLevel);
        sqlDataRecord.SetString(3, entry.Message ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindIdTypeTable(string parameterName, IEnumerable<int> ids)
    {
      ids = ids ?? Enumerable.Empty<int>();
      return this.BindTable(parameterName, "typ_IdTypeTable", this.BindIdTypeTableRows(ids));
    }

    private IEnumerable<SqlDataRecord> BindIdTypeTableRows(IEnumerable<int> ids)
    {
      foreach (int id in ids)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_IdTypeTable);
        sqlDataRecord.SetInt32(0, id);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindKeyValuePairInt32DateTimeTableRows(
      string parameterName,
      Dictionary<int, DateTime> idToDateMap)
    {
      idToDateMap = idToDateMap ?? new Dictionary<int, DateTime>();
      return this.BindTable(parameterName, "typ_KeyValuePairInt32DateTimeTable", this.BindKeyValuePairInt32DateTimeTableRows(idToDateMap));
    }

    private IEnumerable<SqlDataRecord> BindKeyValuePairInt32DateTimeTableRows(
      Dictionary<int, DateTime> idToDateMap)
    {
      foreach (int key in idToDateMap.Keys)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_KeyValuePairInt32DateTimeTable);
        sqlDataRecord.SetInt32(0, key);
        sqlDataRecord.SetDateTime(1, idToDateMap[key]);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestManagement_TinyIntTypeTable(
      string parameterName,
      IEnumerable<byte> states)
    {
      states = states ?? Enumerable.Empty<byte>();
      return this.BindTable(parameterName, "TestManagement.typ_TinyIntTypeTable", this.BindTestManagement_TinyIntTypeTableRows(states));
    }

    private IEnumerable<SqlDataRecord> BindTestManagement_TinyIntTypeTableRows(
      IEnumerable<byte> states)
    {
      foreach (byte state in states)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestManagement_typ_TinyIntTypeTable);
        sqlDataRecord.SetByte(0, state);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindIdPairTypeTable(
      string parameterName,
      IEnumerable<KeyValuePair<int, int>> idMaps)
    {
      idMaps = idMaps ?? Enumerable.Empty<KeyValuePair<int, int>>();
      return this.BindTable(parameterName, "typ_IdPairTypeTable", this.BindIdPairTypeTableRows(idMaps));
    }

    private IEnumerable<SqlDataRecord> BindIdPairTypeTableRows(
      IEnumerable<KeyValuePair<int, int>> idMaps)
    {
      foreach (KeyValuePair<int, int> idMap in idMaps)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_IdPairTypeTable);
        sqlDataRecord.SetInt32(0, idMap.Key);
        sqlDataRecord.SetInt32(1, idMap.Value);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindIdToTitleTypeTable(
      string parameterName,
      IEnumerable<KeyValuePair<int, string>> pairs)
    {
      pairs = pairs ?? Enumerable.Empty<KeyValuePair<int, string>>();
      return this.BindTable(parameterName, "typ_IdToTitleTypeTable", this.BindIdToTitleTypeTableRows(pairs));
    }

    private IEnumerable<SqlDataRecord> BindIdToTitleTypeTableRows(
      IEnumerable<KeyValuePair<int, string>> pairs)
    {
      foreach (KeyValuePair<int, string> pair in pairs)
      {
        SqlDataRecord titleTypeTableRow = new SqlDataRecord(TestManagementDatabase.typ_IdToTitleTypeTable);
        titleTypeTableRow.SetInt32(0, pair.Key);
        titleTypeTableRow.SetString(1, pair.Value ?? string.Empty);
        yield return titleTypeTableRow;
      }
    }

    protected SqlParameter BindIdAndRevTypeTable(string parameterName, IEnumerable<IdAndRev> ids)
    {
      ids = ids ?? Enumerable.Empty<IdAndRev>();
      return this.BindTable(parameterName, "typ_IdAndRevTypeTable", this.BindIdAndRevTypeTableRows(ids));
    }

    private IEnumerable<SqlDataRecord> BindIdAndRevTypeTableRows(IEnumerable<IdAndRev> ids)
    {
      foreach (IdAndRev id in ids)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_IdAndRevTypeTable);
        sqlDataRecord.SetInt32(0, id.Id);
        sqlDataRecord.SetInt32(1, id.Revision);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindUpdatedPropertiesTypeTable(
      string parameterName,
      IEnumerable<UpdatedProperties> props)
    {
      props = props ?? Enumerable.Empty<UpdatedProperties>();
      return this.BindTable(parameterName, "typ_UpdatedPropertiesTypeTable", this.BindUpdatedPropertiesTypeTableRows(props));
    }

    private IEnumerable<SqlDataRecord> BindUpdatedPropertiesTypeTableRows(
      IEnumerable<UpdatedProperties> props)
    {
      foreach (UpdatedProperties prop in props)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_UpdatedPropertiesTypeTable);
        sqlDataRecord.SetInt32(0, prop.Id);
        sqlDataRecord.SetInt32(1, prop.Revision);
        sqlDataRecord.SetDateTime(2, prop.LastUpdated);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindPropertyValuePairTypeTable(
      string parameterName,
      IEnumerable<KeyValuePair<TcmProperty, int>> pairs)
    {
      pairs = pairs ?? Enumerable.Empty<KeyValuePair<TcmProperty, int>>();
      return this.BindTable(parameterName, "typ_PropertyValuePairTypeTable", this.BindPropertyValuePairTypeTableRows(pairs));
    }

    private IEnumerable<SqlDataRecord> BindPropertyValuePairTypeTableRows(
      IEnumerable<KeyValuePair<TcmProperty, int>> pairs)
    {
      foreach (KeyValuePair<TcmProperty, int> pair in pairs)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_PropertyValuePairTypeTable);
        sqlDataRecord.SetInt32(0, (int) pair.Key);
        sqlDataRecord.SetInt32(1, pair.Value);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindIntStringPairTypeTable(
      string parameterName,
      IEnumerable<KeyValuePair<int, string>> pairs)
    {
      pairs = pairs ?? Enumerable.Empty<KeyValuePair<int, string>>();
      return this.BindTable(parameterName, "typ_IntStringPairTypeTable", this.BindIntStringPairTypeTableRows(pairs));
    }

    private IEnumerable<SqlDataRecord> BindIntStringPairTypeTableRows(
      IEnumerable<KeyValuePair<int, string>> pairs)
    {
      foreach (KeyValuePair<int, string> pair in pairs)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_IntStringPairTypeTable);
        sqlDataRecord.SetInt32(0, pair.Key);
        sqlDataRecord.SetString(1, pair.Value ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestRunSummaryByOutcomeTypeTable(
      string parameterName,
      IEnumerable<RunSummaryByOutcome> runSummaryByOutcomes)
    {
      runSummaryByOutcomes = runSummaryByOutcomes ?? Enumerable.Empty<RunSummaryByOutcome>();
      return this.BindTable(parameterName, "TestResult.typ_TestRunSummaryByOutcomeTypeTable", this.BindTestRunSummaryByOutcomeTypeTableRows(runSummaryByOutcomes));
    }

    private IEnumerable<SqlDataRecord> BindTestRunSummaryByOutcomeTypeTableRows(
      IEnumerable<RunSummaryByOutcome> runSummaryByOutcomes)
    {
      foreach (RunSummaryByOutcome summaryByOutcome in runSummaryByOutcomes)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestRunSummaryByOutcomeTypeTable);
        sqlDataRecord.SetByte(0, summaryByOutcome.TestOutcome == Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified ? (byte) 1 : (byte) summaryByOutcome.TestOutcome);
        sqlDataRecord.SetInt32(1, summaryByOutcome.ResultCount);
        sqlDataRecord.SetInt64(2, summaryByOutcome.ResultDuration);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindNameTypeTable(string parameterName, IEnumerable<string> names)
    {
      names = names ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "typ_NameTypeTable", this.BindNameTypeTableRows(names));
    }

    private IEnumerable<SqlDataRecord> BindNameTypeTableRows(IEnumerable<string> names)
    {
      foreach (string name in names)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_NameTypeTable);
        sqlDataRecord.SetString(0, name ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestCaseResultWorkItemLinkTypeTable(
      string parameterName,
      IEnumerable<KeyValuePair<TestCaseResultIdentifier, string>> links)
    {
      links = links ?? Enumerable.Empty<KeyValuePair<TestCaseResultIdentifier, string>>();
      return this.BindTable(parameterName, "typ_TestCaseResultWorkItemLinkTypeTable", this.BindTestCaseResultWorkItemLinkTypeTableRows(links));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResultWorkItemLinkTypeTableRows(
      IEnumerable<KeyValuePair<TestCaseResultIdentifier, string>> links)
    {
      foreach (KeyValuePair<TestCaseResultIdentifier, string> link in links)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResultWorkItemLinkTypeTable);
        sqlDataRecord.SetInt32(0, link.Key.TestResultId);
        sqlDataRecord.SetInt32(1, link.Key.TestRunId);
        TcmTrace.TraceAndDebugAssert("Database", !string.IsNullOrEmpty(link.Value), "The db does not take null values.");
        sqlDataRecord.SetString(2, link.Value);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestCaseResultIdAndRevTypeTable(
      string parameterName,
      IEnumerable<TestCaseResultIdAndRev> ids)
    {
      ids = ids ?? Enumerable.Empty<TestCaseResultIdAndRev>();
      return this.BindTable(parameterName, "typ_TestCaseResultIdAndRevTypeTable", this.BindTestCaseResultIdAndRevTypeTableRows(ids));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseResultIdAndRevTypeTableRows(
      IEnumerable<TestCaseResultIdAndRev> ids)
    {
      foreach (TestCaseResultIdAndRev id in ids)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestCaseResultIdAndRevTypeTable);
        sqlDataRecord.SetInt32(0, id.Id.TestRunId);
        sqlDataRecord.SetInt32(1, id.Id.TestResultId);
        sqlDataRecord.SetInt32(2, id.Revision);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestActionResultTypeTable(
      string parameterName,
      IEnumerable<TestActionResult> actionResults)
    {
      actionResults = actionResults ?? Enumerable.Empty<TestActionResult>();
      return this.BindTable(parameterName, "typ_TestActionResultTypeTable", this.BindTestActionResultTypeTableRows(actionResults));
    }

    private IEnumerable<SqlDataRecord> BindTestActionResultTypeTableRows(
      IEnumerable<TestActionResult> actionResults)
    {
      foreach (TestActionResult actionResult in actionResults)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestActionResultTypeTable);
        record.SetInt32(0, actionResult.TestRunId);
        record.SetInt32(1, actionResult.TestResultId);
        record.SetInt32(2, actionResult.IterationId);
        record.SetString(3, actionResult.ActionPath ?? string.Empty);
        record.SetBytePreserveNull(4, actionResult.Outcome, (byte) 0);
        record.SetString(5, actionResult.ErrorMessage ?? string.Empty);
        record.SetString(6, actionResult.Comment ?? string.Empty);
        record.SetDateTimePreserveNull(7, actionResult.DateStarted);
        record.SetDateTimePreserveNull(8, actionResult.DateCompleted);
        record.SetInt64(9, actionResult.Duration);
        record.SetInt32(10, actionResult.SetId);
        yield return record;
      }
    }

    protected SqlParameter BindTestActionResultUpdate2TypeTable(
      string parameterName,
      IEnumerable<TestActionResult> actionResults)
    {
      actionResults = actionResults ?? Enumerable.Empty<TestActionResult>();
      return this.BindTable(parameterName, "typ_TestActionResultUpdate2TypeTable", this.BindTestActionResultUpdate2TypeTableRows(actionResults));
    }

    private IEnumerable<SqlDataRecord> BindTestActionResultUpdate2TypeTableRows(
      IEnumerable<TestActionResult> actionResults)
    {
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext);
      foreach (TestActionResult actionResult in actionResults)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestActionResultUpdate2TypeTable);
        record.SetInt32(0, actionResult.TestRunId);
        record.SetInt32(1, actionResult.TestResultId);
        record.SetInt32(2, actionResult.IterationId);
        record.SetString(3, actionResult.ActionPath ?? string.Empty);
        record.SetBytePreserveNull(4, actionResult.Outcome, (byte) 0);
        record.SetString(5, actionResult.ErrorMessage ?? string.Empty);
        record.SetString(6, !string.IsNullOrEmpty(actionResult.Comment) ? (actionResult.Comment.Length > maxCommentSize ? actionResult.Comment.Substring(0, maxCommentSize) : actionResult.Comment) : string.Empty);
        record.SetDateTimePreserveNull(7, actionResult.DateStarted);
        record.SetDateTimePreserveNull(8, actionResult.DateCompleted);
        record.SetInt64(9, actionResult.Duration);
        record.SetInt32(10, actionResult.SetId);
        record.SetInt32(11, actionResult.SetRevision);
        yield return record;
      }
    }

    protected SqlParameter BindTestActionResultUpdateTypeTable(
      string parameterName,
      IEnumerable<TestActionResult> actionResults)
    {
      actionResults = actionResults ?? Enumerable.Empty<TestActionResult>();
      return this.BindTable(parameterName, "typ_TestActionResultUpdateTypeTable", this.BindTestActionResultUpdateTypeTableRows(actionResults));
    }

    private IEnumerable<SqlDataRecord> BindTestActionResultUpdateTypeTableRows(
      IEnumerable<TestActionResult> actionResults)
    {
      TestManagementDatabase managementDatabase = this;
      int maxCommentSize = TestManagementServiceUtility.GetMaxLengthForResultComment(managementDatabase.RequestContext);
      foreach (TestActionResult actionResult in actionResults)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestActionResultUpdateTypeTable);
        record.SetInt32(0, actionResult.TestRunId);
        record.SetInt32(1, actionResult.TestResultId);
        record.SetInt32(2, actionResult.IterationId);
        record.SetString(3, actionResult.ActionPath ?? string.Empty);
        record.SetBytePreserveNull(4, actionResult.Outcome, (byte) 0);
        record.SetString(5, managementDatabase.GetTruncatedErrorMessage(actionResult.ErrorMessage));
        record.SetString(6, !string.IsNullOrEmpty(actionResult.Comment) ? (actionResult.Comment.Length > maxCommentSize ? actionResult.Comment.Substring(0, maxCommentSize) : actionResult.Comment) : string.Empty);
        record.SetDateTimePreserveNull(7, actionResult.DateStarted);
        record.SetDateTimePreserveNull(8, actionResult.DateCompleted);
        record.SetInt64(9, actionResult.Duration);
        record.SetInt32(10, actionResult.SetId);
        record.SetInt32(11, actionResult.SetRevision);
        yield return record;
      }
    }

    private string GetTruncatedErrorMessage(string errorMessage)
    {
      if (string.IsNullOrEmpty(errorMessage))
        return string.Empty;
      string truncatedErrorMessage = errorMessage;
      if (errorMessage.Length > 512)
        truncatedErrorMessage = errorMessage.Substring(0, 512);
      return truncatedErrorMessage;
    }

    protected SqlParameter BindTestDataCleanupIds(
      string parameterName,
      List<TestArtifactDataspaceIdMap> param)
    {
      param = param ?? new List<TestArtifactDataspaceIdMap>();
      return this.BindTable(parameterName, "typ_IdPairTypeTable", this.BindTestDataSpaceBuildIDTypeTableRows(param));
    }

    private IEnumerable<SqlDataRecord> BindTestDataSpaceBuildIDTypeTableRows(
      List<TestArtifactDataspaceIdMap> paramList)
    {
      foreach (TestArtifactDataspaceIdMap artifactDataspaceIdMap in paramList)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_IdPairTypeTable);
        sqlDataRecord.SetInt32(0, artifactDataspaceIdMap.ArtifactId);
        sqlDataRecord.SetInt32(1, artifactDataspaceIdMap.DataSpaceId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindIdsForMarkUnMarkFlakiness(
      string parameterName,
      List<TestBranchFlakinesStateMap> param)
    {
      param = param ?? new List<TestBranchFlakinesStateMap>();
      return this.BindTable(parameterName, "typ_BranchToFlakinessStateTypeTable", this.BindTestBranchNameFlakinessStateTypeTableRows(param));
    }

    private IEnumerable<SqlDataRecord> BindTestBranchNameFlakinessStateTypeTableRows(
      List<TestBranchFlakinesStateMap> paramList)
    {
      foreach (TestBranchFlakinesStateMap flakinesStateMap in paramList)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_BranchToFlakinessStateTypeTable);
        sqlDataRecord.SetString(0, flakinesStateMap.BranchName ?? string.Empty);
        sqlDataRecord.SetBoolean(1, flakinesStateMap.IsFlaky);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestActionResultForDeleteTypeTable(
      string parameterName,
      IEnumerable<TestActionResult> actionResults)
    {
      actionResults = actionResults ?? Enumerable.Empty<TestActionResult>();
      return this.BindTable(parameterName, "typ_TestActionResultForDeleteTypeTable", this.BindTestActionResultForDeleteTypeTableRows(actionResults));
    }

    private IEnumerable<SqlDataRecord> BindTestActionResultForDeleteTypeTableRows(
      IEnumerable<TestActionResult> actionResults)
    {
      foreach (TestActionResult actionResult in actionResults)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestActionResultForDeleteTypeTable);
        sqlDataRecord.SetInt32(0, actionResult.TestRunId);
        sqlDataRecord.SetInt32(1, actionResult.TestResultId);
        sqlDataRecord.SetInt32(2, actionResult.IterationId);
        sqlDataRecord.SetString(3, actionResult.ActionPath ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResultParameterTypeTable(
      string parameterName,
      IEnumerable<TestResultParameter> parameters)
    {
      parameters = parameters ?? Enumerable.Empty<TestResultParameter>();
      return this.BindTable(parameterName, "typ_TestResultParameterTypeTable", this.BindTestResultParameterTypeTableRows(parameters));
    }

    private IEnumerable<SqlDataRecord> BindTestResultParameterTypeTableRows(
      IEnumerable<TestResultParameter> parameters)
    {
      foreach (TestResultParameter parameter in parameters)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultParameterTypeTable);
        record.SetInt32(0, parameter.TestRunId);
        record.SetInt32(1, parameter.TestResultId);
        record.SetInt32(2, parameter.IterationId);
        record.SetString(3, parameter.ActionPath ?? string.Empty);
        record.SetString(4, parameter.ParameterName ?? string.Empty);
        record.SetByte(5, parameter.DataType);
        record.SetBytesPreserveNull(6, parameter.Expected);
        record.SetBytesPreserveNull(7, parameter.Actual);
        yield return record;
      }
    }

    protected SqlParameter BindTestResultParameterForDeleteTypeTable(
      string parameterName,
      IEnumerable<TestResultParameter> parameters)
    {
      parameters = parameters ?? Enumerable.Empty<TestResultParameter>();
      return this.BindTable(parameterName, "typ_TestResultParameterForDeleteTypeTable", this.BindTestResultParameterForDeleteTypeTableRows(parameters));
    }

    private IEnumerable<SqlDataRecord> BindTestResultParameterForDeleteTypeTableRows(
      IEnumerable<TestResultParameter> parameters)
    {
      foreach (TestResultParameter parameter in parameters)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestResultParameterForDeleteTypeTable);
        sqlDataRecord.SetInt32(0, parameter.TestRunId);
        sqlDataRecord.SetInt32(1, parameter.TestResultId);
        sqlDataRecord.SetInt32(2, parameter.IterationId);
        sqlDataRecord.SetString(3, parameter.ActionPath ?? string.Empty);
        sqlDataRecord.SetString(4, parameter.ParameterName ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindBuildRefTypeTable(
      string parameterName,
      IEnumerable<BuildConfiguration> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildConfiguration>();
      return this.BindTable(parameterName, "typ_BuildRefTable", this.BindBuildRefTypeTableRows(builds));
    }

    private IEnumerable<SqlDataRecord> BindBuildRefTypeTableRows(
      IEnumerable<BuildConfiguration> builds)
    {
      foreach (BuildConfiguration build in builds)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_BuildRefTable);
        record.SetInt32(0, build.BuildId);
        record.SetString(1, build.BuildUri);
        record.SetString(2, build.BuildNumber);
        record.SetString(3, build.BuildPlatform);
        record.SetString(4, build.BuildFlavor);
        record.SetInt32(5, build.BuildDefinitionId);
        record.SetNullableDateTime(6, build.CreatedDate);
        record.SetNullableInt32(7, new int?(0));
        record.SetStringPreserveNull(8, build.BranchName);
        record.SetNullableStringAsEmpty(9, build.SourceVersion);
        yield return record;
      }
    }

    protected SqlParameter BindBuildRefTypeTable2(
      string parameterName,
      IEnumerable<BuildConfiguration> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildConfiguration>();
      return this.BindTable(parameterName, "typ_BuildRefTable2", this.BindBuildRefTypeTable2Rows(builds));
    }

    private IEnumerable<SqlDataRecord> BindBuildRefTypeTable2Rows(
      IEnumerable<BuildConfiguration> builds)
    {
      foreach (BuildConfiguration build in builds)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_BuildRefTable2);
        record.SetInt32(0, build.BuildId);
        record.SetNullableStringAsEmpty(1, build.BuildUri);
        record.SetNullableStringAsEmpty(2, build.BuildNumber);
        record.SetNullableStringAsEmpty(3, build.BuildPlatform);
        record.SetNullableStringAsEmpty(4, build.BuildFlavor);
        record.SetInt32(5, build.BuildDefinitionId);
        record.SetDateTime(6, build.CreatedDate);
        record.SetInt32(7, 0);
        record.SetNullableStringAsEmpty(8, build.BranchName);
        record.SetNullableStringAsEmpty(9, build.SourceVersion);
        record.SetNullableStringAsEmpty(10, build.BuildSystem);
        yield return record;
      }
    }

    protected SqlParameter BindBuildRefTypeTable3(
      string parameterName,
      IEnumerable<BuildConfiguration> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildConfiguration>();
      return this.BindTable(parameterName, "typ_BuildRefTable3", this.BindBuildRefTypeTable3Rows(builds));
    }

    private IEnumerable<SqlDataRecord> BindBuildRefTypeTable3Rows(
      IEnumerable<BuildConfiguration> builds)
    {
      foreach (BuildConfiguration build in builds)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_BuildRefTable3);
        record.SetInt32(0, build.BuildId);
        record.SetNullableStringAsEmpty(1, build.BuildUri);
        record.SetNullableStringAsEmpty(2, build.BuildNumber);
        record.SetNullableStringAsEmpty(3, build.BuildPlatform);
        record.SetNullableStringAsEmpty(4, build.BuildFlavor);
        record.SetInt32(5, build.BuildDefinitionId);
        record.SetDateTime(6, build.CreatedDate);
        record.SetNullableStringAsEmpty(7, build.BranchName);
        record.SetNullableStringAsEmpty(8, build.SourceVersion);
        record.SetNullableStringAsEmpty(9, build.BuildSystem);
        record.SetNullableStringAsEmpty(10, build.RepositoryId);
        record.SetNullableStringAsEmpty(11, build.RepositoryType);
        yield return record;
      }
    }

    protected SqlParameter BindBuildRefTypeTable4(
      string parameterName,
      IEnumerable<BuildConfiguration> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildConfiguration>();
      return this.BindTable(parameterName, "typ_BuildRefTable4", this.BindBuildRefTypeTable4Rows(builds));
    }

    private IEnumerable<SqlDataRecord> BindBuildRefTypeTable4Rows(
      IEnumerable<BuildConfiguration> builds)
    {
      foreach (BuildConfiguration build in builds)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_BuildRefTable4);
        record.SetInt32(0, build.BuildId);
        record.SetNullableStringAsEmpty(1, build.BuildUri);
        record.SetNullableStringAsEmpty(2, build.BuildNumber);
        record.SetNullableStringAsEmpty(3, build.BuildPlatform);
        record.SetNullableStringAsEmpty(4, build.BuildFlavor);
        record.SetInt32(5, build.BuildDefinitionId);
        record.SetDateTime(6, build.CreatedDate);
        record.SetNullableStringAsEmpty(7, build.BranchName);
        record.SetNullableStringAsEmpty(8, build.SourceVersion);
        record.SetNullableStringAsEmpty(9, build.BuildSystem);
        record.SetNullableStringAsEmpty(10, build.RepositoryId);
        record.SetNullableStringAsEmpty(11, build.RepositoryType);
        record.SetNullableStringAsEmpty(12, build.TargetBranchName);
        yield return record;
      }
    }

    protected SqlParameter BindPipelineRefTable(
      string parameterName,
      IEnumerable<PipelineReference> pipelineReferences)
    {
      pipelineReferences = pipelineReferences ?? Enumerable.Empty<PipelineReference>();
      return this.BindTable(parameterName, "typ_PipelineRefTable", this.BindPipelineRefTableRows(pipelineReferences));
    }

    private IEnumerable<SqlDataRecord> BindPipelineRefTableRows(
      IEnumerable<PipelineReference> pipelineReferences)
    {
      foreach (PipelineReference pipelineReference in pipelineReferences)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_PipelineRefTable);
        record.SetNullableStringAsEmpty(0, pipelineReference.StageReference.StageName);
        record.SetInt32(1, pipelineReference.StageReference.Attempt);
        record.SetNullableStringAsEmpty(2, pipelineReference.PhaseReference.PhaseName);
        record.SetInt32(3, pipelineReference.PhaseReference.Attempt);
        record.SetNullableStringAsEmpty(4, pipelineReference.JobReference.JobName);
        record.SetInt32(5, pipelineReference.JobReference.Attempt);
        yield return record;
      }
    }

    protected SqlParameter BindBranchNameTypeTable(
      string parameterName,
      IEnumerable<string> branchNames)
    {
      branchNames = branchNames ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "typ_BranchNameTypeTable", this.BindBranchNameTypeTableRows(branchNames));
    }

    private IEnumerable<SqlDataRecord> BindBranchNameTypeTableRows(IEnumerable<string> branchNames)
    {
      foreach (string branchName in branchNames)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_BranchNameTypeTable);
        record.SetNullableStringAsEmpty(0, branchName);
        yield return record;
      }
    }

    protected SqlParameter BindBuildUriToDeletionState(
      string parameterName,
      IEnumerable<KeyValuePair<string, bool>> buildStates)
    {
      buildStates = buildStates ?? Enumerable.Empty<KeyValuePair<string, bool>>();
      return this.BindTable(parameterName, "typ_BuildUriToDeletionState", this.BindBuildUriToDeletionStateRows(buildStates));
    }

    private IEnumerable<SqlDataRecord> BindBuildUriToDeletionStateRows(
      IEnumerable<KeyValuePair<string, bool>> buildStates)
    {
      foreach (KeyValuePair<string, bool> buildState in buildStates)
      {
        SqlDataRecord deletionStateRow = new SqlDataRecord(TestManagementDatabase.typ_BuildUriToDeletionState);
        deletionStateRow.SetString(0, buildState.Key);
        deletionStateRow.SetBoolean(1, buildState.Value);
        yield return deletionStateRow;
      }
    }

    protected SqlParameter BindReleaseRefTypeTable(
      string parameterName,
      IEnumerable<ReleaseReference> releases)
    {
      releases = releases ?? Enumerable.Empty<ReleaseReference>();
      return this.BindTable(parameterName, "typ_ReleaseRefTable", this.BindReleaseRefTypeTableRows(releases));
    }

    private IEnumerable<SqlDataRecord> BindReleaseRefTypeTableRows(
      IEnumerable<ReleaseReference> releases)
    {
      foreach (ReleaseReference release in releases)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ReleaseRefTable);
        record.SetNullableStringAsEmpty(0, release.ReleaseUri);
        record.SetNullableStringAsEmpty(1, release.ReleaseEnvUri);
        yield return record;
      }
    }

    protected SqlParameter BindReleaseRefTypeTable2(
      string parameterName,
      IEnumerable<ReleaseReference> releases)
    {
      releases = releases ?? Enumerable.Empty<ReleaseReference>();
      return this.BindTable(parameterName, "typ_ReleaseRefTable2", this.BindReleaseRefTypeTableRows2(releases));
    }

    private IEnumerable<SqlDataRecord> BindReleaseRefTypeTableRows2(
      IEnumerable<ReleaseReference> releases)
    {
      foreach (ReleaseReference release in releases)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ReleaseRefTable2);
        record.SetNullableStringAsEmpty(0, release.ReleaseUri);
        record.SetNullableStringAsEmpty(1, release.ReleaseEnvUri);
        record.SetInt32(2, release.ReleaseId);
        record.SetInt32(3, release.ReleaseEnvId);
        record.SetInt32(4, release.ReleaseDefId);
        record.SetInt32(5, release.ReleaseEnvDefId);
        record.SetInt32(6, release.Attempt);
        yield return record;
      }
    }

    protected SqlParameter BindReleaseRefTypeTable3(
      string parameterName,
      IEnumerable<ReleaseReference> releases)
    {
      releases = releases ?? Enumerable.Empty<ReleaseReference>();
      return this.BindTable(parameterName, "typ_ReleaseRefTable3", this.BindReleaseRefTypeTableRows3(releases));
    }

    private IEnumerable<SqlDataRecord> BindReleaseRefTypeTableRows3(
      IEnumerable<ReleaseReference> releases)
    {
      foreach (ReleaseReference release in releases)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ReleaseRefTable3);
        record.SetNullableStringAsEmpty(0, release.ReleaseUri);
        record.SetNullableStringAsEmpty(1, release.ReleaseEnvUri);
        record.SetInt32(2, release.ReleaseId);
        record.SetInt32(3, release.ReleaseEnvId);
        record.SetInt32(4, release.ReleaseDefId);
        record.SetInt32(5, release.ReleaseEnvDefId);
        record.SetInt32(6, release.Attempt);
        record.SetNullableStringAsEmpty(7, release.ReleaseName);
        yield return record;
      }
    }

    protected SqlParameter BindReleaseRefTypeTable4(
      string parameterName,
      IEnumerable<ReleaseReference> releases)
    {
      releases = releases ?? Enumerable.Empty<ReleaseReference>();
      return this.BindTable(parameterName, "typ_ReleaseRefTable4", this.BindReleaseRefTypeTableRows4(releases));
    }

    private IEnumerable<SqlDataRecord> BindReleaseRefTypeTableRows4(
      IEnumerable<ReleaseReference> releases)
    {
      foreach (ReleaseReference release in releases)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ReleaseRefTable4);
        record.SetNullableStringAsEmpty(0, release.ReleaseUri);
        record.SetNullableStringAsEmpty(1, release.ReleaseEnvUri);
        record.SetInt32(2, release.ReleaseId);
        record.SetInt32(3, release.ReleaseEnvId);
        record.SetInt32(4, release.ReleaseDefId);
        record.SetInt32(5, release.ReleaseEnvDefId);
        record.SetInt32(6, release.Attempt);
        record.SetNullableStringAsEmpty(7, release.ReleaseName);
        record.SetDateTime(8, release.ReleaseCreationDate);
        record.SetDateTime(9, release.EnvironmentCreationDate);
        yield return record;
      }
    }

    protected SqlParameter BindTestExtensionFieldValuesTypeTable(
      string parameterName,
      IEnumerable<Tuple<int, int, TestExtensionField>> fields)
    {
      fields = fields ?? Enumerable.Empty<Tuple<int, int, TestExtensionField>>();
      return this.BindTable(parameterName, "typ_TestExtensionFieldValuesTable", this.BindTestExtensionFieldValuesTableRows(fields));
    }

    private IEnumerable<SqlDataRecord> BindTestExtensionFieldValuesTableRows(
      IEnumerable<Tuple<int, int, TestExtensionField>> fields)
    {
      foreach (Tuple<int, int, TestExtensionField> field in fields)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestExtensionFieldValuesTable);
        sqlDataRecord.SetInt32(0, field.Item1);
        sqlDataRecord.SetInt32(1, field.Item2);
        sqlDataRecord.SetInt32(2, field.Item3.Field.Id);
        for (int ordinal = 3; ordinal < 9; ++ordinal)
          sqlDataRecord.SetDBNull(ordinal);
        switch (field.Item3.Field.Type)
        {
          case SqlDbType.Bit:
            if (field.Item3.Value is bool)
            {
              sqlDataRecord.SetBoolean(7, (bool) field.Item3.Value);
              break;
            }
            sqlDataRecord.SetBoolean(7, Convert.ToBoolean(field.Item3.Value));
            break;
          case SqlDbType.DateTime:
            if (field.Item3.Value is DateTime)
            {
              sqlDataRecord.SetDateTime(5, (DateTime) field.Item3.Value);
              break;
            }
            sqlDataRecord.SetDateTime(5, DateTime.SpecifyKind(Convert.ToDateTime(field.Item3.Value), DateTimeKind.Utc));
            break;
          case SqlDbType.Float:
            if (field.Item3.Value is double)
            {
              sqlDataRecord.SetDouble(4, (double) field.Item3.Value);
              break;
            }
            sqlDataRecord.SetDouble(4, Convert.ToDouble(field.Item3.Value));
            break;
          case SqlDbType.Int:
            if (field.Item3.Value is int)
            {
              sqlDataRecord.SetInt32(3, (int) field.Item3.Value);
              break;
            }
            sqlDataRecord.SetInt32(3, Convert.ToInt32(field.Item3.Value));
            break;
          case SqlDbType.UniqueIdentifier:
            if (field.Item3.Value is Guid)
            {
              sqlDataRecord.SetGuid(6, (Guid) field.Item3.Value);
              break;
            }
            sqlDataRecord.SetGuid(6, new Guid(field.Item3.Value.ToString()));
            break;
          default:
            if (field.Item3.Value is string)
            {
              sqlDataRecord.SetString(8, (string) field.Item3.Value);
              break;
            }
            sqlDataRecord.SetString(8, Convert.ToString(field.Item3.Value, (IFormatProvider) CultureInfo.InvariantCulture));
            break;
        }
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestExtensionFieldsTypeTable(
      string parameterName,
      IEnumerable<TestExtensionFieldDetails> fields)
    {
      fields = fields ?? Enumerable.Empty<TestExtensionFieldDetails>();
      return this.BindTable(parameterName, "typ_TestExtensionFieldsTable", this.BindTestExtensionFieldsTableRows(fields));
    }

    private IEnumerable<SqlDataRecord> BindTestExtensionFieldsTableRows(
      IEnumerable<TestExtensionFieldDetails> fields)
    {
      foreach (TestExtensionFieldDetails field in fields)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestExtensionFieldsTable);
        sqlDataRecord.SetString(0, field.Name);
        sqlDataRecord.SetByte(1, (byte) field.Type);
        sqlDataRecord.SetBoolean(2, field.IsRunScoped);
        sqlDataRecord.SetBoolean(3, field.IsResultScoped);
        sqlDataRecord.SetBoolean(4, field.IsSystemField);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestMethodTypeTable(
      string parameterName,
      IEnumerable<TestMethod> testMethods)
    {
      testMethods = testMethods ?? Enumerable.Empty<TestMethod>();
      return this.BindTable(parameterName, "TestResult.typ_TestMethodTypeTable", this.BindTestMethodTypeTableRows(testMethods));
    }

    private IEnumerable<SqlDataRecord> BindTestMethodTypeTableRows(
      IEnumerable<TestMethod> testMethods)
    {
      foreach (TestMethod testMethod in testMethods)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestMethodTypeTable);
        sqlDataRecord.SetString(0, testMethod.Name);
        sqlDataRecord.SetString(1, testMethod.Container);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReferenceTypeTableForCreate(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReferenceTypeTable", this.BindTestResult_TestCaseReferenceTypeTableRowsForCreate(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReferenceTypeTableRowsForCreate(
      IEnumerable<TestCaseResult> results)
    {
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReferenceTypeTable);
        int num = indexValue++;
        sqlDataRecord.SetInt32(0, num);
        sqlDataRecord.SetInt32(1, result.TestCaseId);
        sqlDataRecord.SetInt32(2, result.TestPointId);
        sqlDataRecord.SetInt32(3, result.ConfigurationId);
        sqlDataRecord.SetString(4, result.OwnerName ?? string.Empty);
        sqlDataRecord.SetByte(5, result.Priority);
        sqlDataRecord.SetString(6, result.TestCaseTitle ?? string.Empty);
        sqlDataRecord.SetString(7, result.TestCaseAreaUri ?? string.Empty);
        sqlDataRecord.SetInt32(8, result.TestCaseRevision);
        sqlDataRecord.SetString(9, result.AutomatedTestName ?? string.Empty);
        sqlDataRecord.SetString(10, result.AutomatedTestStorage ?? string.Empty);
        sqlDataRecord.SetString(11, result.AutomatedTestType ?? string.Empty);
        sqlDataRecord.SetString(12, result.AutomatedTestId ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReferenceTypeTableForUpdate(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReferenceTypeTable", this.BindTestResult_TestCaseReferenceTypeTableRowsForUpdate(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReferenceTypeTableRowsForUpdate(
      IEnumerable<TestCaseResult> results)
    {
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReferenceTypeTable);
        sqlDataRecord.SetInt32(0, result.TestCaseReferenceId);
        sqlDataRecord.SetString(4, result.OwnerName ?? string.Empty);
        sqlDataRecord.SetByte(5, result.Priority);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindInt32TypeTable(string parameterName, IEnumerable<int> ids)
    {
      ids = ids ?? Enumerable.Empty<int>();
      return this.BindTable(parameterName, "dbo.typ_Int32Table", this.BindInt32TypeTableRows(ids));
    }

    private IEnumerable<SqlDataRecord> BindInt32TypeTableRows(IEnumerable<int> ids)
    {
      foreach (int id in ids)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.Typ_Int32TypeTable);
        sqlDataRecord.SetInt32(0, id);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindInt64TypeTable(string parameterName, IEnumerable<long> ids)
    {
      ids = ids ?? Enumerable.Empty<long>();
      return this.BindTable(parameterName, "dbo.typ_Int64Table", this.BindInt64TypeTableRows(ids));
    }

    private IEnumerable<SqlDataRecord> BindInt64TypeTableRows(IEnumerable<long> ids)
    {
      foreach (long id in ids)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.Typ_Int64TypeTable);
        sqlDataRecord.SetInt64(0, id);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_FailingSinceDetailsTable(
      string parameterName,
      Dictionary<int, string> failingSinceDetails)
    {
      failingSinceDetails = failingSinceDetails ?? new Dictionary<int, string>();
      return this.BindTable(parameterName, "TestResult.typ_FailingSinceDetailsTable", this.BindTestResult_FailingSinceDetailsTableRows(failingSinceDetails));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_FailingSinceDetailsTableRows(
      Dictionary<int, string> failingSinceDetails)
    {
      foreach (int key in failingSinceDetails.Keys)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_FailingSinceDetailsTable);
        sqlDataRecord.SetInt32(0, 0);
        sqlDataRecord.SetInt32(1, key);
        sqlDataRecord.SetString(2, failingSinceDetails[key] ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_FailingSinceDetailsTable(
      string parameterName,
      Dictionary<int, Dictionary<int, string>> failingSinceDetails)
    {
      failingSinceDetails = failingSinceDetails ?? new Dictionary<int, Dictionary<int, string>>();
      return this.BindTable(parameterName, "TestResult.typ_FailingSinceDetailsTable", this.BindTestResult_FailingSinceDetailsTableRows(failingSinceDetails));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_FailingSinceDetailsTableRows(
      Dictionary<int, Dictionary<int, string>> failingSinceDetails)
    {
      foreach (int runId in failingSinceDetails.Keys)
      {
        foreach (int key in (IEnumerable<int>) failingSinceDetails[runId]?.Keys ?? Enumerable.Empty<int>())
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_FailingSinceDetailsTable);
          sqlDataRecord.SetInt32(0, runId);
          sqlDataRecord.SetInt32(1, key);
          sqlDataRecord.SetString(2, failingSinceDetails[runId][key] ?? string.Empty);
          yield return sqlDataRecord;
        }
      }
    }

    protected SqlParameter BindTestResultIdentifierRecordTypeTable(
      string parameterName,
      List<TestResultIdentifierRecord> resultsRecords)
    {
      resultsRecords = resultsRecords ?? new List<TestResultIdentifierRecord>();
      return this.BindTable(parameterName, "typ_TestResultIdentifierRecordTypeTable", this.BindTestResultIdentifierRecordTypeTableRows(resultsRecords));
    }

    private IEnumerable<SqlDataRecord> BindTestResultIdentifierRecordTypeTableRows(
      List<TestResultIdentifierRecord> resultsRecords)
    {
      foreach (TestResultIdentifierRecord resultsRecord in resultsRecords)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestResultIdentifierRecordTypeTable);
        sqlDataRecord.SetInt32(0, resultsRecord.TestRunId);
        sqlDataRecord.SetInt32(1, resultsRecord.TestResultId);
        sqlDataRecord.SetInt32(2, resultsRecord.TestCaseReferenceId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_FailureCountForTestRunTypeTable(
      string parameterName,
      Dictionary<int, ResultInsights> runToInsights)
    {
      runToInsights = runToInsights ?? new Dictionary<int, ResultInsights>();
      return this.BindTable(parameterName, "TestResult.typ_FailureCountForTestRunTypeTable", this.BindTestResult_FailureCountForTestRunTableRows(runToInsights));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_FailureCountForTestRunTableRows(
      Dictionary<int, ResultInsights> runToInsights)
    {
      foreach (int key in runToInsights.Keys)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(TestManagementDatabase.TestResult_typ_FailureCountForTestRunTypeTable);
        sqlDataRecord1.SetInt32(0, key);
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        ResultInsights runToInsight1 = runToInsights[key];
        int newFailures = runToInsight1 != null ? runToInsight1.NewFailures : 0;
        sqlDataRecord2.SetInt32(1, newFailures);
        sqlDataRecord1.SetString(2, runToInsights[key]?.NewFailedResults ?? string.Empty);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        ResultInsights runToInsight2 = runToInsights[key];
        int existingFailures = runToInsight2 != null ? runToInsight2.ExistingFailures : 0;
        sqlDataRecord3.SetInt32(3, existingFailures);
        sqlDataRecord1.SetString(4, runToInsights[key]?.ExistingFailedResults ?? string.Empty);
        yield return sqlDataRecord1;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReference2TypeTableForCreate(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference2TypeTable", this.BindTestResult_TestCaseReference2TypeTableRowsForCreate(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference2TypeTableRowsForCreate(
      IEnumerable<TestCaseResult> results)
    {
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference2TypeTable);
        int num = indexValue++;
        sqlDataRecord.SetInt32(0, num);
        sqlDataRecord.SetInt32(1, result.TestCaseId);
        sqlDataRecord.SetInt32(2, result.TestPointId);
        sqlDataRecord.SetInt32(3, result.ConfigurationId);
        sqlDataRecord.SetString(4, result.OwnerName ?? string.Empty);
        sqlDataRecord.SetByte(5, result.Priority);
        sqlDataRecord.SetString(6, result.TestCaseTitle ?? string.Empty);
        sqlDataRecord.SetString(7, result.TestCaseAreaUri ?? string.Empty);
        sqlDataRecord.SetInt32(8, result.TestCaseRevision);
        sqlDataRecord.SetString(9, result.AutomatedTestName ?? string.Empty);
        sqlDataRecord.SetString(10, result.AutomatedTestStorage ?? string.Empty);
        sqlDataRecord.SetString(11, result.AutomatedTestType ?? string.Empty);
        sqlDataRecord.SetString(12, result.AutomatedTestId ?? string.Empty);
        sqlDataRecord.SetBytes(13, 0L, this.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        sqlDataRecord.SetBytes(14, 0L, this.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    internal byte[] GetSHA256Hash(string name)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(name);
      using (SHA256Managed shA256Managed = new SHA256Managed())
        return shA256Managed.ComputeHash(bytes);
    }

    protected SqlParameter BindTestResult_TestCaseReference2TypeTableForUpdate(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference2TypeTable", this.BindTestResult_TestCaseReference2TypeTableRowsForUpdate(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference2TypeTableRowsForUpdate(
      IEnumerable<TestCaseResult> results)
    {
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference2TypeTable);
        sqlDataRecord.SetInt32(0, result.TestCaseReferenceId);
        sqlDataRecord.SetString(4, result.OwnerName ?? string.Empty);
        sqlDataRecord.SetByte(5, result.Priority);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReference2TypeTableForPopulateHash(
      string parameterName,
      IEnumerable<TestCaseReference> testRefs)
    {
      testRefs = testRefs ?? Enumerable.Empty<TestCaseReference>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference2TypeTable", this.BindTestResult_TestCaseReference2TypeTableRowsForPopulateHash(testRefs));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference2TypeTableRowsForPopulateHash(
      IEnumerable<TestCaseReference> testRefs)
    {
      foreach (TestCaseReference testRef in testRefs)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference2TypeTable);
        sqlDataRecord.SetInt32(0, testRef.TestCaseReferenceId);
        sqlDataRecord.SetBytes(13, 0L, this.GetSHA256Hash(testRef.AutomatedTestName ?? string.Empty), 0, 32);
        sqlDataRecord.SetBytes(14, 0L, this.GetSHA256Hash(testRef.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReference2HashTypeTable(
      string parameterName,
      IEnumerable<string> valuesToHash)
    {
      valuesToHash = valuesToHash ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference2TypeTable", this.BindTestResult_TestCaseReference2HashTypeTableRows(valuesToHash));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference2HashTypeTableRows(
      IEnumerable<string> valuesToHash)
    {
      foreach (string str in valuesToHash)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference2TypeTable);
        sqlDataRecord.SetBytes(13, 0L, this.GetSHA256Hash(str ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReference3TypeTableForCreate(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference3TypeTable", this.BindTestResult_TestCaseReference3TypeTableRowsForCreate(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference3TypeTableRowsForCreate(
      IEnumerable<TestCaseResult> results)
    {
      int indexValue = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference3TypeTable);
        int num = indexValue++;
        sqlDataRecord.SetInt32(0, num);
        sqlDataRecord.SetInt32(1, result.TestCaseId);
        sqlDataRecord.SetInt32(2, result.TestPointId);
        sqlDataRecord.SetInt32(3, result.ConfigurationId);
        sqlDataRecord.SetString(4, result.OwnerName ?? string.Empty);
        sqlDataRecord.SetByte(5, result.Priority);
        sqlDataRecord.SetString(6, result.TestCaseTitle ?? string.Empty);
        sqlDataRecord.SetString(7, result.TestCaseAreaUri ?? string.Empty);
        sqlDataRecord.SetInt32(8, result.TestCaseRevision);
        sqlDataRecord.SetString(9, result.AutomatedTestName ?? string.Empty);
        sqlDataRecord.SetString(10, result.AutomatedTestStorage ?? string.Empty);
        sqlDataRecord.SetString(11, result.AutomatedTestType ?? string.Empty);
        sqlDataRecord.SetString(12, result.AutomatedTestId ?? string.Empty);
        sqlDataRecord.SetBytes(13, 0L, this.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 0, 32);
        sqlDataRecord.SetBytes(14, 0L, this.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReference3TypeTableForUpdate(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference3TypeTable", this.BindTestResult_TestCaseReference3TypeTableRowsForUpdate(results));
    }

    protected SqlParameter BindTestResult_TestCaseReference4TypeTableForUpdate(
      string parameterName,
      IEnumerable<TestCaseResult> results)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReferenceUpdatableFieldsTypeTable", this.BindTestResult_TestCaseReference4TypeTableRowsForUpdate(results));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference3TypeTableRowsForUpdate(
      IEnumerable<TestCaseResult> results)
    {
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference3TypeTable);
        sqlDataRecord.SetInt32(0, result.TestCaseReferenceId);
        sqlDataRecord.SetString(4, result.OwnerName ?? string.Empty);
        sqlDataRecord.SetByte(5, result.Priority);
        yield return sqlDataRecord;
      }
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference4TypeTableRowsForUpdate(
      IEnumerable<TestCaseResult> results)
    {
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReferenceUpdatableFieldsTypeTable);
        record.SetInt32(0, result.TestRunId);
        record.SetInt32(1, result.TestResultId);
        record.SetStringPreserveNull(2, result.OwnerName);
        record.SetBytePreserveNull(3, result.Priority, byte.MaxValue);
        yield return record;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReference3TypeTableForPopulateHash(
      string parameterName,
      IEnumerable<TestCaseReference> testRefs)
    {
      testRefs = testRefs ?? Enumerable.Empty<TestCaseReference>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference3TypeTable", this.BindTestResult_TestCaseReference3TypeTableRowsForPopulateHash(testRefs));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference3TypeTableRowsForPopulateHash(
      IEnumerable<TestCaseReference> testRefs)
    {
      foreach (TestCaseReference testRef in testRefs)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference3TypeTable);
        sqlDataRecord.SetInt32(0, testRef.TestCaseReferenceId);
        sqlDataRecord.SetBytes(13, 0L, this.GetSHA256Hash(testRef.AutomatedTestName ?? string.Empty), 0, 32);
        sqlDataRecord.SetBytes(14, 0L, this.GetSHA256Hash(testRef.AutomatedTestStorage ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestResult_TestCaseReference3HashTypeTable(
      string parameterName,
      IEnumerable<string> valuesToHash)
    {
      valuesToHash = valuesToHash ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "TestResult.typ_TestCaseReference3TypeTable", this.BindTestResult_TestCaseReference3HashTypeTableRows(valuesToHash));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseReference3HashTypeTableRows(
      IEnumerable<string> valuesToHash)
    {
      foreach (string str in valuesToHash)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.TestResult_typ_TestCaseReference3TypeTable);
        sqlDataRecord.SetBytes(13, 0L, this.GetSHA256Hash(str ?? string.Empty), 0, 32);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestSettings_TestSettingsTypeTable(
      string parameterName,
      IEnumerable<TestSettings> testSettings)
    {
      testSettings = testSettings ?? Enumerable.Empty<TestSettings>();
      return this.BindTable(parameterName, "typ_TestSettingsTable", this.BindTestResult_TestCaseResultUpdate3TypeTableRows(testSettings));
    }

    private IEnumerable<SqlDataRecord> BindTestResult_TestCaseResultUpdate3TypeTableRows(
      IEnumerable<TestSettings> testSettings)
    {
      foreach (TestSettings testSetting in testSettings)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestSettingsTypeTable);
        record.SetInt32(0, testSetting.Id);
        record.SetString(1, testSetting.Name);
        record.SetString(2, testSetting.Description);
        record.SetGuidPreserveNull(3, testSetting.CreatedBy);
        record.SetDateTimePreserveNull(4, testSetting.CreatedDate);
        record.SetString(5, testSetting.Settings);
        record.SetInt32(6, testSetting.AreaId);
        record.SetInt32(7, testSetting.Revision);
        record.SetBoolean(8, testSetting.IsPublic);
        record.SetDateTimePreserveNull(9, testSetting.LastUpdated);
        record.SetGuidPreserveNull(10, testSetting.LastUpdatedBy);
        record.SetBoolean(11, testSetting.IsAutomated);
        record.SetSqlXml(12, new SqlXml(TestManagementDatabase.ToStream((object) testSetting.MachineRoles, typeof (TestSettingsMachineRole[]))));
        yield return record;
      }
    }

    internal static Stream ToStream(object obj, Type type)
    {
      DataContractSerializer contractSerializer = new DataContractSerializer(type);
      MemoryStream stream = new MemoryStream();
      MemoryStream memoryStream = stream;
      object graph = obj;
      contractSerializer.WriteObject((Stream) memoryStream, graph);
      stream.Flush();
      stream.Position = 0L;
      return (Stream) stream;
    }

    protected SqlParameter BindModuleCoverageTypeTable(
      string parameterName,
      IEnumerable<ModuleCoverage> modules)
    {
      modules = modules ?? Enumerable.Empty<ModuleCoverage>();
      return this.BindTable(parameterName, "typ_ModuleCoverageTypeTable", this.BindModuleCoverageTypeTableRows(modules));
    }

    private IEnumerable<SqlDataRecord> BindModuleCoverageTypeTableRows(
      IEnumerable<ModuleCoverage> modules)
    {
      foreach (ModuleCoverage module in modules)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ModuleCoverageTypeTable);
        record.SetInt32(0, module.ModuleId);
        TcmTrace.IfNullThenTraceAndDebugFail("Database", module.Name, "module.Name");
        record.SetString(1, module.Name);
        record.SetGuidPreserveNull(2, module.Signature);
        record.SetInt32(3, module.SignatureAge);
        record.SetInt32(4, module.Statistics.LinesCovered);
        record.SetInt32(5, module.Statistics.LinesPartiallyCovered);
        record.SetInt32(6, module.Statistics.LinesNotCovered);
        record.SetInt32(7, module.Statistics.BlocksCovered);
        record.SetInt32(8, module.Statistics.BlocksNotCovered);
        record.SetInt32(9, module.BlockCount);
        record.SetInt32(10, module.BlockData.Length);
        record.SetBytesPreserveNull(11, module.BlockData);
        yield return record;
      }
    }

    protected SqlParameter BindModuleCoverageTypeTable1(
      string parameterName,
      IEnumerable<ModuleCoverage> modules)
    {
      modules = modules ?? Enumerable.Empty<ModuleCoverage>();
      return this.BindTable(parameterName, "typ_ModuleCoverageTypeTable1", this.BindModuleCoverageTypeTableRows1(modules));
    }

    private IEnumerable<SqlDataRecord> BindModuleCoverageTypeTableRows1(
      IEnumerable<ModuleCoverage> modules)
    {
      foreach (ModuleCoverage module in modules)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_ModuleCoverageTypeTable1);
        record.SetInt32(0, module.ModuleId);
        TcmTrace.IfNullThenTraceAndDebugFail("Database", module.Name, "module.Name");
        record.SetString(1, module.Name);
        record.SetGuidPreserveNull(2, module.Signature);
        record.SetInt32(3, module.SignatureAge);
        record.SetInt32(4, module.Statistics.LinesCovered);
        record.SetInt32(5, module.Statistics.LinesPartiallyCovered);
        record.SetInt32(6, module.Statistics.LinesNotCovered);
        record.SetInt32(7, module.Statistics.BlocksCovered);
        record.SetInt32(8, module.Statistics.BlocksNotCovered);
        record.SetInt32(9, module.BlockCount);
        record.SetInt32(10, module.BlockData.Length);
        record.SetBytesPreserveNull(11, module.BlockData);
        record.SetString(12, module.CoverageFileUrl ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindFunctionCoverageTypeTable(
      string parameterName,
      IEnumerable<FunctionCoverage> functions)
    {
      functions = functions ?? Enumerable.Empty<FunctionCoverage>();
      return this.BindTable(parameterName, "typ_FunctionCoverageTypeTable", this.BindFunctionCoverageTypeTableRows(functions));
    }

    private IEnumerable<SqlDataRecord> BindFunctionCoverageTypeTableRows(
      IEnumerable<FunctionCoverage> functions)
    {
      foreach (FunctionCoverage function in functions)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_FunctionCoverageTypeTable);
        record.SetInt32(0, function.ModuleId);
        record.SetInt32(1, function.FunctionId);
        TcmTrace.IfNullThenTraceAndDebugFail("Database", function.Name, "function.Name");
        record.SetString(2, function.Name);
        record.SetStringPreserveNull(3, function.SourceFile);
        record.SetStringPreserveNull(4, function.Class);
        record.SetStringPreserveNull(5, function.Namespace);
        record.SetInt32(6, function.Statistics.LinesCovered);
        record.SetInt32(7, function.Statistics.LinesPartiallyCovered);
        record.SetInt32(8, function.Statistics.LinesNotCovered);
        record.SetInt32(9, function.Statistics.BlocksCovered);
        record.SetInt32(10, function.Statistics.BlocksNotCovered);
        yield return record;
      }
    }

    protected SqlParameter BindCoverageSummaryTypeTable(
      string parameterName,
      IEnumerable<CodeCoverageStatistics> codeCoverageStats)
    {
      codeCoverageStats = codeCoverageStats ?? Enumerable.Empty<CodeCoverageStatistics>();
      return this.BindTable(parameterName, "typ_CodeCoverageSummaryTypeTable", this.BindCoverageSummaryTypeTableRows(codeCoverageStats));
    }

    private IEnumerable<SqlDataRecord> BindCoverageSummaryTypeTableRows(
      IEnumerable<CodeCoverageStatistics> codeCoverageStats)
    {
      foreach (CodeCoverageStatistics codeCoverageStat in codeCoverageStats)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_CodeCoverageSummaryTypeTable);
        sqlDataRecord.SetString(0, codeCoverageStat.Label);
        sqlDataRecord.SetInt32(1, codeCoverageStat.Position);
        sqlDataRecord.SetInt32(2, codeCoverageStat.Total);
        sqlDataRecord.SetInt32(3, codeCoverageStat.Covered);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestAuthoringDetailsTypeTableTable(
      string parameterName,
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails)
    {
      testAuthoringDetails = testAuthoringDetails ?? Enumerable.Empty<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails>();
      return this.BindTable(parameterName, "TestResult.typ_TestAuthoringDetailsTypeTable", this.BindTestAuthoringDetailsTypeTableRows(testAuthoringDetails));
    }

    private IEnumerable<SqlDataRecord> BindTestAuthoringDetailsTypeTableRows(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails)
    {
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails testAuthoringDetail in testAuthoringDetails)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestAuthoringDetailsTypeTable);
        sqlDataRecord.SetInt32(0, testAuthoringDetail.PointId);
        sqlDataRecord.SetInt32(1, testAuthoringDetail.SuiteId);
        sqlDataRecord.SetInt32(2, testAuthoringDetail.ConfigurationId);
        sqlDataRecord.SetGuid(3, testAuthoringDetail.TesterId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestAuthoringDetails2TypeTableTable(
      string parameterName,
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails)
    {
      testAuthoringDetails = testAuthoringDetails ?? Enumerable.Empty<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails>();
      return this.BindTable(parameterName, "TestResult.typ_TestAuthoringDetails2TypeTable", this.BindTestAuthoringDetails2TypeTableRows(testAuthoringDetails));
    }

    private IEnumerable<SqlDataRecord> BindTestAuthoringDetails2TypeTableRows(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails)
    {
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails testAuthoringDetail in testAuthoringDetails)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestAuthoringDetails2TypeTable);
        record.SetInt32(0, testAuthoringDetail.PointId);
        record.SetInt32(1, testAuthoringDetail.SuiteId);
        record.SetInt32(2, testAuthoringDetail.ConfigurationId);
        record.SetGuid(3, testAuthoringDetail.TesterId);
        record.SetByte(4, (byte) testAuthoringDetail.State);
        record.SetDateTime(5, testAuthoringDetail.LastUpdated);
        record.SetNullableByte(6, testAuthoringDetail.Priority);
        record.SetNullableGuid(7, testAuthoringDetail.RunBy);
        record.SetNullableBoolean(8, testAuthoringDetail.IsAutomated);
        yield return record;
      }
    }

    protected SqlParameter BindTestExtentionFieldIdArrayTableTable(
      string parameterName,
      IEnumerable<int> fieldIds)
    {
      fieldIds = fieldIds ?? Enumerable.Empty<int>();
      return this.BindTable(parameterName, "dbo.typ_TestExtentionFieldIdArrayTable", this.BindTestExtentionFieldIdArrayTableRows(fieldIds));
    }

    private IEnumerable<SqlDataRecord> BindTestExtentionFieldIdArrayTableRows(
      IEnumerable<int> fieldIds)
    {
      foreach (int fieldId in fieldIds)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestExtentionFieldIdArrayTable);
        sqlDataRecord.SetInt32(0, fieldId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestLinksTypeTable(
      string parameterName,
      IEnumerable<Link<SessionLinkType>> sessionLinks)
    {
      sessionLinks = sessionLinks ?? Enumerable.Empty<Link<SessionLinkType>>();
      return this.BindTable(parameterName, "OneMRX.typ_TestLinksTypeTable", this.BindTestLinksTypeTableRows(sessionLinks));
    }

    private IEnumerable<SqlDataRecord> BindTestLinksTypeTableRows(
      IEnumerable<Link<SessionLinkType>> links)
    {
      foreach (Link<SessionLinkType> link in links)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestLinksTypeTable);
        sqlDataRecord.SetByte(0, (byte) 1);
        sqlDataRecord.SetByte(1, (byte) link.Type);
        sqlDataRecord.SetString(2, link.Url);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindSessionTimelineTypeTable(
      string parameterName,
      IEnumerable<Timeline<SessionTimelineType>> timelines)
    {
      timelines = timelines ?? Enumerable.Empty<Timeline<SessionTimelineType>>();
      return this.BindTable(parameterName, "OneMRX.typ_SessionTimelineTypeTable", this.BindSessionTimelineTypeTableRows(timelines));
    }

    private IEnumerable<SqlDataRecord> BindSessionTimelineTypeTableRows(
      IEnumerable<Timeline<SessionTimelineType>> timelines)
    {
      foreach (Timeline<SessionTimelineType> timeline in timelines)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_SessionTimelineTypeTable);
        record.SetByte(0, (byte) 1);
        record.SetByte(1, (byte) timeline.Type);
        record.SetNullableString(2, timeline.Display);
        record.SetDateTime(3, timeline.TimestampUTC);
        yield return record;
      }
    }

    protected SqlParameter BindGroupLayoutTypeTable(
      string parameterName,
      IEnumerable<Layout> groupLayout)
    {
      groupLayout = groupLayout ?? Enumerable.Empty<Layout>();
      return this.BindTable(parameterName, "OneMRX.typ_GroupLayoutTypeTable2", this.BindGroupLayoutTypeTableRows(groupLayout));
    }

    private IEnumerable<SqlDataRecord> BindGroupLayoutTypeTableRows(IEnumerable<Layout> groupLayout)
    {
      foreach (Layout layout in groupLayout)
      {
        if (layout.Uid != Guid.Empty)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_GroupLayoutTypeTable);
          sqlDataRecord.SetString(0, layout.Type);
          sqlDataRecord.SetString(1, layout.Name);
          sqlDataRecord.SetGuid(2, layout.Uid);
          sqlDataRecord.SetDateTime(3, layout.StartTimeUTC);
          sqlDataRecord.SetDateTime(4, layout.EndTimeUTC);
          yield return sqlDataRecord;
        }
      }
    }

    protected SqlParameter BindJobLayoutTypeTable(
      string parameterName,
      IEnumerable<Layout> groupLayout)
    {
      groupLayout = groupLayout ?? Enumerable.Empty<Layout>();
      return this.BindTable(parameterName, "OneMRX.typ_JobLayoutTypeTable2", this.BindJobLayoutTypeTableRows(groupLayout));
    }

    private IEnumerable<SqlDataRecord> BindJobLayoutTypeTableRows(IEnumerable<Layout> groupLayout) => (IEnumerable<SqlDataRecord>) groupLayout.Where<Layout>((System.Func<Layout, bool>) (group => group.Children != null && group.Children.Any<Layout>())).SelectMany<Layout, SqlDataRecord>((System.Func<Layout, IEnumerable<SqlDataRecord>>) (group => group.Children.Where<Layout>((System.Func<Layout, bool>) (job => job != null && job.Uid != Guid.Empty)).Select<Layout, SqlDataRecord>((System.Func<Layout, SqlDataRecord>) (job =>
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_JobLayoutTypeTable);
      sqlDataRecord.SetGuid(0, group.Uid);
      sqlDataRecord.SetString(1, job.Type);
      sqlDataRecord.SetString(2, job.Name);
      sqlDataRecord.SetGuid(3, job.Uid);
      sqlDataRecord.SetDateTime(4, job.StartTimeUTC);
      sqlDataRecord.SetDateTime(5, job.EndTimeUTC);
      return sqlDataRecord;
    })))).ToList<SqlDataRecord>();

    protected SqlParameter BindGroupLayoutPropertiesTypeTable(
      string parameterName,
      IEnumerable<Layout> groupLayout)
    {
      groupLayout = groupLayout ?? Enumerable.Empty<Layout>();
      return this.BindTable(parameterName, "OneMRX.typ_GroupLayoutPropertiesTypeTable", this.BindLayoutPropertiesTypeTableRows(groupLayout, TestManagementDatabase.typ_LayoutPropertiesTypeTable));
    }

    protected SqlParameter BindJobLayoutPropertiesTypeTable(
      string parameterName,
      IEnumerable<Layout> groupLayout)
    {
      groupLayout = groupLayout ?? Enumerable.Empty<Layout>();
      return this.BindTable(parameterName, "OneMRX.typ_JobLayoutPropertiesTypeTable", this.BindLayoutPropertiesTypeTableRows(groupLayout.SelectMany<Layout, Layout>((System.Func<Layout, IEnumerable<Layout>>) (group => (IEnumerable<Layout>) group.Children)), TestManagementDatabase.typ_LayoutPropertiesTypeTable));
    }

    private IEnumerable<SqlDataRecord> BindLayoutPropertiesTypeTableRows(
      IEnumerable<Layout> layouts,
      SqlMetaData[] metaData)
    {
      return layouts == null ? (IEnumerable<SqlDataRecord>) null : layouts.Where<Layout>((System.Func<Layout, bool>) (layout => layout != null && layout.Properties != null)).SelectMany<Layout, KeyValuePair<string, string>, SqlDataRecord>((System.Func<Layout, IEnumerable<KeyValuePair<string, string>>>) (layout =>
      {
        Dictionary<string, string> properties1 = layout.Properties;
        return properties1 == null ? (IEnumerable<KeyValuePair<string, string>>) null : properties1.Where<KeyValuePair<string, string>>((System.Func<KeyValuePair<string, string>, bool>) (properties => properties.Key != null));
      }), (Func<Layout, KeyValuePair<string, string>, SqlDataRecord>) ((layout, properties) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(metaData);
        sqlDataRecord.SetGuid(0, layout.Uid);
        sqlDataRecord.SetString(1, properties.Key);
        sqlDataRecord.SetString(2, properties.Value);
        return sqlDataRecord;
      }));
    }

    protected SqlParameter BindDimensionsTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "OneMRX.typ_DimensionsTypeTable", this.BindDimensionsTableRows(results, resultIds));
    }

    private IEnumerable<SqlDataRecord> BindDimensionsTableRows(
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      int i = 0;
      foreach (TestCaseResult result in results)
      {
        foreach (TestResultDimension dimension in result.Dimensions)
        {
          SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultDimensionTypeTable);
          record.SetInt32(0, resultIds[i]);
          record.SetInt32(1, result.TestRunId);
          record.SetStringPreserveNull(2, dimension.Name);
          record.SetStringPreserveNull(3, dimension.Value);
          yield return record;
        }
        ++i;
      }
    }

    protected SqlParameter BindFailureBucketTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "OneMRX.typ_FailureBucketMRXTypeTable", this.BindFailureBucketTableRows(results, resultIds));
    }

    private IEnumerable<SqlDataRecord> BindFailureBucketTableRows(
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      int i = 0;
      foreach (TestCaseResult result in results)
      {
        if (result.BucketUid != null && result.BucketingSystem != null)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_FailureBucketMRXTypeTable);
          sqlDataRecord.SetInt32(0, resultIds[i]);
          sqlDataRecord.SetString(1, result.BucketUid);
          sqlDataRecord.SetString(2, result.BucketingSystem);
          yield return sqlDataRecord;
        }
        ++i;
      }
    }

    protected SqlParameter BindTestResultExOneMRXTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "OneMRX.typ_TestResultExMRXTypeTable", this.BindTestResultExOneMRXTableRows(results, resultIds));
    }

    private IEnumerable<SqlDataRecord> BindTestResultExOneMRXTableRows(
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      int i = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultExMRXTypeTable);
        record.SetInt32(0, resultIds[i]);
        record.SetInt32(1, result.TestRunId);
        record.SetBoolean(2, result.IsSystemIssue);
        record.SetStringPreserveNull(3, result.ExceptionType);
        ++i;
        yield return record;
      }
    }

    protected SqlParameter BindTestResultOneMRXTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "OneMRX.typ_TestResultOneMRXTypeTable", this.BindTestResultOneMRXTableRows(results, resultIds));
    }

    private IEnumerable<SqlDataRecord> BindTestResultOneMRXTableRows(
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      int i = 0;
      foreach (TestCaseResult result in results)
      {
        SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_TestResultOneMRXTypeTable);
        record.SetInt32(0, resultIds[i]);
        record.SetInt32(1, result.TestRunId);
        record.SetInt32(2, result.ExecutionNumber);
        record.SetInt32(3, result.Attempt);
        record.SetStringPreserveNull(4, result.Locale);
        record.SetStringPreserveNull(5, result.BuildType);
        record.SetByte(6, result.TestPhase);
        record.SetInt32(7, result.TopologyId);
        ++i;
        yield return record;
      }
    }

    protected SqlParameter BindLinkTypeTable(
      string parameterName,
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      results = results ?? Enumerable.Empty<TestCaseResult>();
      return this.BindTable(parameterName, "OneMRX.typ_LinkMRXTypeTable", this.BindLinkTypeTableRows(results, resultIds));
    }

    private IEnumerable<SqlDataRecord> BindLinkTypeTableRows(
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      int i = 0;
      foreach (TestCaseResult result in results)
      {
        foreach (Link<ResultLinkType> link in result.Links)
        {
          SqlDataRecord record = new SqlDataRecord(TestManagementDatabase.typ_LinkResultTypeTable);
          record.SetInt32(0, resultIds[i]);
          record.SetByte(1, (byte) 0);
          record.SetByte(2, (byte) link.Type);
          record.SetStringPreserveNull(3, link.Url);
          record.SetStringPreserveNull(4, link.DisplayName);
          record.SetByte(5, (byte) link.OperationType);
          yield return record;
        }
        ++i;
      }
    }

    protected SqlParameter BindTestSessionTestRunsTypeTable(
      string parameterName,
      IEnumerable<int> testRunIds)
    {
      testRunIds = testRunIds ?? Enumerable.Empty<int>();
      return this.BindTable(parameterName, "OneMRX.typ_TestSession_TestRunsTypeTable", this.BindTestSessionTestRunsTypeTableTableRows(testRunIds));
    }

    private IEnumerable<SqlDataRecord> BindTestSessionTestRunsTypeTableTableRows(
      IEnumerable<int> testRunIds)
    {
      foreach (int testRunId in testRunIds)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_TestSession_TestRunsTypeTable);
        sqlDataRecord.SetInt32(0, testRunId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindEnvironmentTypeTable(
      string parameterName,
      IEnumerable<TestSessionEnvironment> environments)
    {
      environments = environments ?? Enumerable.Empty<TestSessionEnvironment>();
      return this.BindTable(parameterName, "OneMRX.typ_EnvironmentTypeTable", this.BindEnvironmentTypeTableRows(environments));
    }

    private IEnumerable<SqlDataRecord> BindEnvironmentTypeTableRows(
      IEnumerable<TestSessionEnvironment> environments)
    {
      foreach (TestSessionEnvironment environment in environments)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementDatabase.typ_EnvironmentTypeTable);
        sqlDataRecord.SetString(0, environment.ProcessorArchitecture);
        sqlDataRecord.SetString(1, environment.DisplayName);
        yield return sqlDataRecord;
      }
    }

    static TestManagementDatabase()
    {
      TestManagementDatabase.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      TestManagementDatabase.RegisterException(550002, typeof (TestObjectNotFoundException));
      TestManagementDatabase.RegisterException(550017, typeof (TestObjectNotFoundException));
      TestManagementDatabase.RegisterException(550003, typeof (TestObjectInUseException));
      TestManagementDatabase.RegisterException(550004, typeof (TestObjectUpdatedException));
      TestManagementDatabase.RegisterException(550005, typeof (TestManagementInvalidOperationException));
      TestManagementDatabase.RegisterException(550008, typeof (TestManagementInvalidOperationException));
      TestManagementDatabase.RegisterException(550006, typeof (TestSuiteInvalidOperationException));
      TestManagementDatabase.RegisterException(550007, typeof (TestSuiteInvalidOperationException));
      TestManagementDatabase.RegisterException(550009, typeof (TestManagementValidationException));
      TestManagementDatabase.RegisterException(550010, typeof (TestSuiteInvalidOperationException));
      TestManagementDatabase.RegisterException(550011, typeof (TestSuiteInvalidOperationException));
      TestManagementDatabase.RegisterException(550012, typeof (TestManagementInvalidOperationException));
      TestManagementDatabase.RegisterException(550013, typeof (TestManagementValidationException));
      TestManagementDatabase.RegisterException(550021, typeof (TestManagementConflictingOperation));
      TestManagementDatabase.RegisterException(550025, typeof (TestManagementInvalidOperationException));
      TestManagementDatabase.RegisterException(550029, typeof (TestSuiteInvalidOperationException));
      TestManagementDatabase.RegisterException(550018, typeof (TestManagementValidationException));
      TestManagementDatabase.RegisterException(550019, typeof (TestManagementValidationException));
      TestManagementDatabase.RegisterException(550026, typeof (TestManagementInvalidOperationException));
      TestManagementDatabase.RegisterException(550030, typeof (TestManagementInvalidOperationException));
      TestManagementDatabase.RegisterException(550027, typeof (TestManagementInvalidOperationException));
      TestManagementDatabase.RegisterException(550031, typeof (TestObjectNotFoundException));
      TestManagementDatabase.InitializeTestManagementDynamicSprocsMap();
    }

    public TestManagementDatabase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal TestManagementDatabase(string connectionString, int partitionId)
      : this()
    {
      this.Initialize(SqlConnectionInfoFactory.Create(connectionString), 3600, 20, 1, 1, (ITFLogger) null, (CircuitBreakerDatabaseProperties) null);
      this.PartitionId = partitionId;
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      base.Initialize(requestContext, databaseCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    protected override void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      base.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, serviceVersion, logger, circuitBreakerProperties);
    }

    private static void InitializeTestManagementDynamicSprocsMap()
    {
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap = new Dictionary<string, string>(13);
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryConfigurations"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryConfigurations;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryTestSettings"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestSettings;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryObjectsCount"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryObjectsCount;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryAttachments2"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryAttachments2;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["TestResult.prc_QueryAttachments2"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryAttachments2_UnifyingView;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["TestResult.prc_QueryTestResults2"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestResults2_UnifyingViews;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryTestRunIds"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestRunIds;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryTestRuns2"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestRuns2;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryTestRuns2_V2"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestRuns2_V2;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryTestRuns2_V3"] = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestRuns2_V3;
      TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_GetTestExecutionReport"] = TestManagementDynamicSqlBatchStatements.dynprc_GetTestExecutionReport;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TestManagementDatabase.s_sqlExceptionFactories;

    public static TestManagementDatabase Create(TestManagementRequestContext requestContext) => TestManagementDatabase.Create(requestContext.RequestContext);

    public static TestManagementDatabase Create(IVssRequestContext requestContext) => requestContext.CreateComponent<TestManagementDatabase>("TestManagement");

    public static TestManagementDatabase CreateReadReplicaAwareComponent(
      IVssRequestContext requestContext)
    {
      DatabaseConnectionType databaseConnectionType = requestContext.RouteThroughReadReplica() ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default;
      return requestContext.CreateComponent<TestManagementDatabase>("TestManagement", new DatabaseConnectionType?(databaseConnectionType));
    }

    private static void RegisterException(int sqlErrorCode, Type exceptionType) => TestManagementDatabase.s_sqlExceptionFactories.Add(sqlErrorCode, new SqlExceptionFactory(exceptionType));

    protected bool MoreResultsAvailable(SqlDataReader reader)
    {
      try
      {
        return reader.NextResult();
      }
      catch (SqlException ex)
      {
        this.HandleException((Exception) ex);
        throw;
      }
    }

    internal virtual string EscapeQuotes(string str) => str.Replace("'", "''");

    public override void Dispose()
    {
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    internal virtual void PrepareDynamicProcedure(string sprocName)
    {
      string sqlBatchStatements = TestManagementDatabase.s_TcmDynamicSqlBatchStatementsMap[sprocName];
      this.PrepareSqlBatch(sqlBatchStatements.Length, true);
      this.AddStatement(sqlBatchStatements, 0, true, true);
    }

    internal static long GetDurationFromStartAndCompleteDates(
      DateTime startDate,
      DateTime completeDate)
    {
      return !startDate.Equals(new DateTime()) && !completeDate.Equals(new DateTime()) ? (completeDate - startDate).Ticks : 0L;
    }

    internal virtual int GetDataspaceIdWithLazyInitialization(Guid projectId) => this.GetDataspaceId(projectId, true);

    public virtual void QueueDeleteRunsByRetentionSettings(
      TestManagementRequestContext context,
      Guid projectId,
      DateTime currentUtcDate,
      Guid deletedBy,
      int runsDeletionBatchSize,
      int automatedTestRetentionDuration,
      int manualTestRetentionDuration,
      out int automatedRunsDeleted,
      out int manualRunsDeleted,
      bool isTcmService,
      bool isOnpremService,
      int queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec)
    {
      automatedRunsDeleted = 0;
      manualRunsDeleted = 0;
    }

    public virtual ResultRetentionSettings CreateResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId,
      ResultRetentionSettings settings)
    {
      return settings;
    }

    public virtual ResultRetentionSettings GetResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId)
    {
      return (ResultRetentionSettings) null;
    }

    public virtual ResultRetentionSettings UpdateResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId,
      ResultRetentionSettings settings)
    {
      return settings;
    }

    public virtual (int TotalBuildSignalsToDelete, int TotalReleaseSignalsToDelete) DeleteInprogressTestResultSignals(
      Guid projectId,
      int batchSize,
      int retentionDays)
    {
      return (0, 0);
    }

    public virtual List<int> QueryAndUpdateBuildInProgressTestSignals(
      IEnumerable<Guid> projectIds,
      int batchSize)
    {
      return new List<int>();
    }

    public virtual List<(int ReleaseId, int EnvironmentId)> QueryAndUpdateReleaseInProgressTestSignals(
      IEnumerable<Guid> projectId,
      int batchSize)
    {
      return new List<(int, int)>();
    }

    public virtual void AddOrUpdateBuildInProgressTestSignal(Guid projectId, int buildId)
    {
    }

    public virtual void AddOrUpdateReleaseInProgressTestSignal(
      Guid projectId,
      int releaseId,
      int environmentId)
    {
    }

    public virtual void CreateTestRunCompletedEntry(
      Guid projectId,
      int testRunId,
      int buildId,
      int releaseId,
      int releaseEnvId,
      bool isSimilarResultsEnabled = false)
    {
    }

    public virtual void DeleteTestRunInsightsCalculationEntry(Guid projectId, List<int> testRunId)
    {
    }

    public virtual void GetTestRunIdsForInsightsCalculation(
      int batchSize,
      out Dictionary<Guid, Dictionary<int, List<int>>> buildTestRunIds,
      out Dictionary<Guid, Dictionary<int, List<int>>> releaseTestRunIds)
    {
      buildTestRunIds = (Dictionary<Guid, Dictionary<int, List<int>>>) null;
      releaseTestRunIds = (Dictionary<Guid, Dictionary<int, List<int>>>) null;
    }

    public virtual void GetTestRunsForFailureFailureBucketing(
      int batchSize,
      out Dictionary<Guid, List<int>> runIds)
    {
      runIds = (Dictionary<Guid, List<int>>) null;
    }

    public virtual void DeleteRunCompletedEntriesForSimilarResultsJob(
      Guid projectId,
      List<int> testRunId)
    {
    }

    public virtual void InsertSimilarTestResults(Guid projectId, List<TestResultField> results)
    {
    }

    public virtual List<TestResultField> QueryFieldsValuesForBucketing(
      Guid projectId,
      List<int> testRunIds)
    {
      return new List<TestResultField>();
    }

    public virtual List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QuerySimilarTestResults(
      Guid projectId,
      int testRunId,
      int testCaseResultId,
      int testSubResultId,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      return new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
    }

    public virtual void DeleteBuildInProgressTestSignal(Guid projectId, int buildId)
    {
    }

    public virtual void DeleteReleaseInProgressTestSignal(
      Guid projectId,
      int releaseId,
      int environmentId)
    {
    }

    public virtual List<TestCaseResult> GetTestCaseResultsByPointIds(
      Guid projectId,
      int planId,
      List<int> pointIds)
    {
      return new List<TestCaseResult>();
    }

    public virtual List<TestCaseResult> GetTestCaseResultsByPointIds2(
      Guid projectId,
      int planId,
      List<int> pointIds)
    {
      return new List<TestCaseResult>();
    }

    public virtual int SoftDeleteStaleTcmJobDefinitions(int batchSize, List<string> jobNames) => 0;

    public virtual TestRunContextBackfillStatus BackfillTestRunContextId(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int batchSize = 10000)
    {
      return new TestRunContextBackfillStatus();
    }

    public virtual void CreateTestResults(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      Guid updatedBy,
      bool updateRunSummary,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_CreateTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindTestResult_TestCaseResult4TypeTable("@testresultsTable", results);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindTestExtensionFieldValuesTypeTable("@additionalFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, results, false));
      this.BindInt("@testResultStartId", 0);
      this.BindBoolean("@updateRunSummary", updateRunSummary);
      this.ExecuteNonQuery();
    }

    public virtual void CreateTestResultOneMRX(
      Guid projectId,
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "OneMRX.CreateTestResultOneMRX"))
        {
          this.PrepareStoredProcedure("OneMRX.prc_CreateTestResultsMRX");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindTestResultOneMRXTypeTable("@testResultMRXTable", results, resultIds);
          this.BindDimensionsTypeTable("@dimensionsTable", results, resultIds);
          this.BindFailureBucketTypeTable("@failureBucketOneMRXTable", results, resultIds);
          this.BindTestResultExOneMRXTypeTable("@testResultExMRXTable", results, resultIds);
          this.BindLinkTypeTable("@linkTable", results, resultIds);
          this.ExecuteNonQuery();
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CreateTestResultOneMRX");
      }
    }

    public virtual long CreateTestSession(Guid projectId, OneMRXSession session)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "OneMRX.CreateTestSession"))
        {
          this.PrepareStoredProcedure("OneMRX.prc_CreateTestSession");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindString("@type", session.Type, (int) byte.MaxValue, true, SqlDbType.NVarChar);
          this.BindString("@name", session.Name, (int) byte.MaxValue, true, SqlDbType.NVarChar);
          this.BindGuid("@uid", session.Uid);
          this.BindNullableDateTime("@startTimeUTC", session.StartTimeUTC);
          this.BindNullableDateTime("@endTimeUTC", session.EndTimeUTC);
          this.BindByte("@state", (byte) session.State);
          this.BindByte("@result", (byte) session.Result);
          this.BindGuidPreserveNull("@sourceSessionID", session.Source.SessionId);
          this.BindString("@sourceOriginSystem", session.Source.OriginSystem, (int) byte.MaxValue, true, SqlDbType.NVarChar);
          this.BindGuid("@sourceTenantID", session.Source.TenantId);
          this.BindString("@sourceTenantName", session.Source.TenantName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
          this.BindTestLinksTypeTable("@SessionLinks", (IEnumerable<Link<SessionLinkType>>) session.Source.Links);
          this.BindSessionTimelineTypeTable("@SessionTimeline", (IEnumerable<Timeline<SessionTimelineType>>) session.Timeline);
          this.BindGroupLayoutTypeTable("@GroupLayout", (IEnumerable<Layout>) session.Layout);
          this.BindJobLayoutTypeTable("@JobLayout", (IEnumerable<Layout>) session.Layout);
          this.BindGroupLayoutPropertiesTypeTable("@GroupLayoutProperties", (IEnumerable<Layout>) session.Layout);
          this.BindJobLayoutPropertiesTypeTable("@JobLayoutProperties", (IEnumerable<Layout>) session.Layout);
          this.BindTestSessionTestRunsTypeTable("@TestSessionTestRunsMap", (IEnumerable<int>) session.TestRuns);
          SqlDataReader reader = this.ExecuteReader();
          return reader.Read() ? new TestManagementDatabase.CreateTestSessionColumns().bind(reader) : throw new UnexpectedDatabaseResultException("OneMRX.prc_CreateTestSession");
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public virtual void CreateConfigurationAndEnvironment(
      Guid projectId,
      IList<TestSessionEnvironment> environments)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "OneMRX.CreateEnvironmentAndConfiguration"))
        {
          this.PrepareStoredProcedure("OneMRX.prc_CreateEnvironmentAndConfiguration");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindEnvironmentTypeTable("@EnvironmentTable", (IEnumerable<TestSessionEnvironment>) environments);
          this.ExecuteNonQuery();
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public List<OneMRXSession> GetTestSessionByBuildId(
      Guid projectId,
      int buildId,
      int skip = 0,
      int top = 2147483647)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "OneMRX.GetTestSessionByBuildId"))
        {
          this.PrepareStoredProcedure("OneMRX.prc_GetSessionMetadataByBuildId");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindInt("@buildId", buildId);
          this.BindInt("@skip", skip);
          this.BindInt("@top", top);
          SqlDataReader reader = this.ExecuteReader();
          List<OneMRXSession> source = new List<OneMRXSession>();
          int num = 0;
          while (reader.Read())
          {
            TestManagementDatabase.QuerySessionMetadataColumns sessionMetadataColumns = new TestManagementDatabase.QuerySessionMetadataColumns();
            source.Add(sessionMetadataColumns.bind(reader));
            ++num;
          }
          return source.Skip<OneMRXSession>(skip).Take<OneMRXSession>(top).ToList<OneMRXSession>();
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public Dictionary<long, List<int>> GetTestRunByTestSession(
      Guid projectId,
      List<long> sessionIds)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "OneMRX.GetTestSessionByBuildId"))
        {
          this.PrepareStoredProcedure("OneMRX.prc_GetTestRunByTestSession");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindInt64TypeTable("@idsTable", (IEnumerable<long>) sessionIds);
          Dictionary<long, List<int>> testSessionRunMap = new Dictionary<long, List<int>>();
          using (SqlDataReader reader = this.ExecuteReader())
          {
            while (reader.Read())
              new TestManagementDatabase.QuerySessionTestRunColumns().Bind(reader, testSessionRunMap);
          }
          return testSessionRunMap;
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public List<Layout> GetLayoutBySessionId(Guid projectId, Guid sessionId, int skip = 0, int top = 2147483647)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "OneMRX.CreateTestSession"))
        {
          this.PrepareStoredProcedure("OneMRX.prc_GetSessionLayoutByTestSessionId");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindGuid("@SessionId", sessionId);
          this.BindInt("@Skip", skip);
          this.BindInt("@Top", top);
          SqlDataReader reader = this.ExecuteReader();
          List<Layout> layoutList = new List<Layout>();
          Dictionary<long, Layout> dictionary = new Dictionary<long, Layout>();
          int num = 0;
          while (reader.Read())
          {
            long int64 = reader.GetInt64(0);
            TestManagementDatabase.QuerySessionLayoutColumns sessionLayoutColumns = new TestManagementDatabase.QuerySessionLayoutColumns();
            dictionary.Add(int64, sessionLayoutColumns.bind(reader));
            ++num;
          }
          if (reader.NextResult())
          {
            while (reader.Read())
            {
              long int64 = reader.GetInt64(1);
              TestManagementDatabase.QuerySessionLayoutColumns sessionLayoutColumns = new TestManagementDatabase.QuerySessionLayoutColumns();
              Layout layout;
              dictionary.TryGetValue(int64, out layout);
              if (layout != null)
              {
                if (layout.Children == null)
                  layout.Children = new List<Layout>();
                layout.Children.Add(sessionLayoutColumns.bind(reader));
              }
            }
          }
          return dictionary.Values.ToList<Layout>().Skip<Layout>(skip).Take<Layout>(top).ToList<Layout>();
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public virtual int[] CreateTestResultsExtension2(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      int passedTestsCount,
      Guid updatedBy,
      bool isTcmService)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.CreateTestResultsExtension2");
        this.PrepareStoredProcedure("TestResult.prc_CreateTestResultsExt2");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindTestResult_TestCaseResult2TypeTable("@testresultsTable", results, true);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindTestExtensionFieldValuesTypeTable("@additionalFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, results, true));
        this.BindInt("@passedTestsCount", passedTestsCount);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestResultId");
        List<int> intList = new List<int>(results.Count<TestCaseResult>());
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader, 0));
        intList.Sort();
        return intList.ToArray();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CreateTestResultsExtension2");
      }
    }

    public virtual int[] CreateTestCaseReference(
      Guid projectId,
      IEnumerable<TestCaseResult> results,
      Guid updatedBy,
      out int newTestCaseReferences,
      out List<int> newTestCaseRefIds)
    {
      newTestCaseReferences = -1;
      newTestCaseRefIds = (List<int>) null;
      return (int[]) null;
    }

    public virtual int DeleteStaleTestCaseReference(
      int batchSize,
      int staleTestCaseRefSprocExecTimeOutInSec)
    {
      return 0;
    }

    public virtual int DeleteStaleTestCaseReferenceByProject(
      int batchSize,
      int staleTestCaseRefSprocExecTimeOutInSec,
      Guid projectId,
      int daysOlderToCleanupStaleTestCaseRef)
    {
      return 0;
    }

    public virtual List<TestCaseReference> QueryTestCaseReference(
      Guid projectId,
      List<string> automatedTestNames,
      List<int> testCaseIds,
      int planId,
      List<int> suiteIds,
      List<int> pointIds)
    {
      return (List<TestCaseReference>) null;
    }

    public virtual void UpdateTestCaseReference(Guid projectId, IEnumerable<TestCaseResult> results)
    {
    }

    public virtual void UpdateTestCaseReference2(
      Guid projectId,
      IEnumerable<TestCaseResult> results)
    {
    }

    public virtual void UpdateTestResultsExtension(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      Guid updatedBy)
    {
    }

    public virtual List<TestRun> QueryTestRunsbyFilters(
      Guid projectId,
      QueryTestRunsFilter filters,
      int top,
      int batchSize,
      out int minNextBatchRunId,
      out DateTime minNextBatchLastUpdated)
    {
      minNextBatchRunId = -1;
      minNextBatchLastUpdated = DateTime.MaxValue;
      return new List<TestRun>();
    }

    public virtual Dictionary<TestArtifactDataspaceIdMap, bool> QueryBuildIdsPresent(
      List<TestArtifactDataspaceIdMap> paramList)
    {
      return new Dictionary<TestArtifactDataspaceIdMap, bool>();
    }

    public virtual Dictionary<TestArtifactDataspaceIdMap, bool> QueryRunIdsPresent(
      List<TestArtifactDataspaceIdMap> dataSpaceRunIds)
    {
      return new Dictionary<TestArtifactDataspaceIdMap, bool>();
    }

    internal virtual List<TestCaseResultIdentifier> QueryTestResultsForTestCaseId(
      Guid projectId,
      int testCaseId,
      int top)
    {
      return new List<TestCaseResultIdentifier>();
    }

    public virtual List<TestRun> GetTestRunIdsWithoutInsightsForBuild(Guid projectId, int buildId) => new List<TestRun>();

    public virtual List<TestRun> GetTestRunIdsWithoutInsightsForRelease(
      Guid projectId,
      int releaseId)
    {
      return new List<TestRun>();
    }

    public virtual List<TestCaseResult> GetTestResultsByFQDN(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      int buildId,
      string sourceWorkflow,
      List<TestCaseReference> testRefs)
    {
      return new List<TestCaseResult>();
    }

    public virtual Dictionary<int, List<RunSummaryByOutcome>> QueryTestRunsOutComeSummary(
      Guid projectId,
      IList<int> testRunIds)
    {
      return new Dictionary<int, List<RunSummaryByOutcome>>();
    }

    public virtual List<TestCaseResult> FetchTestResultsByRun(
      Guid projectId,
      int testRunId,
      List<int> resultIds,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      actionResults = (List<TestActionResult>) null;
      parameters = (List<TestResultParameter>) null;
      attachments = (List<TestResultAttachment>) null;
      return (List<TestCaseResult>) null;
    }

    public virtual List<TestCaseResult> FetchTestResultsByRunMRX(
      Guid projectId,
      int testRunId,
      List<int> resultIds,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      actionResults = (List<TestActionResult>) null;
      parameters = (List<TestResultParameter>) null;
      attachments = (List<TestResultAttachment>) null;
      return (List<TestCaseResult>) null;
    }

    internal virtual List<ResultUpdateResponse> UpdateTestResults2(
      Guid projectId,
      int testRunId,
      IEnumerable<ResultUpdateRequest> resultsForUpdate,
      Guid updatedBy,
      bool autoComputeTestRunState,
      bool updateRunSummary,
      bool isTcmService,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run)
    {
      run = (TestRun) null;
      iterationUri = string.Empty;
      runProjGuid = Guid.Empty;
      return (List<ResultUpdateResponse>) null;
    }

    internal virtual int CreateTestResolutionState(TestResolutionState state, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_CreateTestResolutionState");
      this.BindString("@name", state.Name, 256, true, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new TestManagementDatabase.CreateTestResolutionStateColumns().stateId.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_CreateTestResolutionState");
    }

    internal virtual int CreateTestFailureType(TestFailureType failureType, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_CreateTestFailureType");
      this.BindString("@name", failureType.Name, 256, true, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new TestManagementDatabase.CreateTestFailureTypeColumns().failureTypeId.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_CreateTestFailureType");
    }

    internal virtual List<TestFailureType> ImportFailureTypes(
      TestFailureType[] failureTypes,
      Guid projectId,
      Guid userIdentity)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.ImportFailureTypes");
        this.PrepareStoredProcedure("TestManagement.prc_ImportFailureTypes");
        List<string> names = new List<string>();
        List<TestFailureType> failureTypes1 = new List<TestFailureType>();
        int lazyInitialization = this.GetDataspaceIdWithLazyInitialization(projectId);
        foreach (TestFailureType failureType in failureTypes)
          names.Add(failureType.Name);
        this.BindNameTypeTable("@targetFailureTypes", (IEnumerable<string>) names);
        this.BindInt("@dataspaceId", lazyInitialization);
        this.BindGuid("@userIdentity", userIdentity);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.QueryTestFailureTypesColumns failureTypesColumns = new TestManagementDatabase.QueryTestFailureTypesColumns();
        while (reader.Read())
          failureTypes1.Add(failureTypesColumns.bind(reader));
        if (failureTypes1.Count != 0)
          throw new TestObjectInUseException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.FailureTypesAreInUse, (object) this.FailureTypesListToString((IList<TestFailureType>) failureTypes1)));
        List<TestFailureType> testFailureTypeList = new List<TestFailureType>();
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            testFailureTypeList.Add(failureTypesColumns.bind(reader));
        }
        return testFailureTypeList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.ImportFailureTypes");
      }
    }

    public virtual List<TestResultFailureType> QueryTestResultFailureTypes(
      int failureTypeId,
      Guid projectId)
    {
      return new List<TestResultFailureType>();
    }

    public virtual TestResultFailureType CreateTestResultFailureType(
      string failureType,
      Guid projectId)
    {
      return new TestResultFailureType();
    }

    public virtual bool DeleteTestResultFailureType(
      Guid projectId,
      int failureTypeId,
      Guid auditIdentity)
    {
      return false;
    }

    public virtual void UpdateTestRunExtension(
      Guid projectId,
      IEnumerable<Tuple<int, TestExtensionField>> extensionFields)
    {
    }

    public virtual int DeleteFlakyTestData(
      Guid projectId,
      int batchSize,
      int deleteFlakyTestDataSprocExecTimeOutInSec)
    {
      return 0;
    }

    public virtual long UpdateLogStoreContentSizeByBuild(
      Guid projectId,
      int buildId,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      return 0;
    }

    public virtual long UpdateLogStoreContentSizeByRuns(
      Guid projectId,
      List<int> runIds,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      return 0;
    }

    public virtual UpdatedRunProperties UpdateTestRunWithCustomFields(
      Guid projectId,
      TestRun run,
      Guid updatedBy,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool skipRunStateTransitionCheck = false)
    {
      return this.UpdateTestRun(projectId, run, updatedBy, releaseRef, buildRef, skipRunStateTransitionCheck);
    }

    protected string FailureTypesListToString(IList<TestFailureType> failureTypes)
    {
      StringBuilder stringBuilder = new StringBuilder(failureTypes.Count);
      bool flag = true;
      foreach (TestFailureType failureType in (IEnumerable<TestFailureType>) failureTypes)
      {
        if (!flag)
        {
          stringBuilder.Append(", ");
          stringBuilder.Append(failureType.Name);
        }
        else
        {
          stringBuilder.Append(failureType.Name);
          flag = false;
        }
      }
      return stringBuilder.ToString();
    }

    internal virtual List<TestResolutionState> ImportResolutionStates(
      TestResolutionState[] states,
      Guid projectId,
      Guid userIdentity)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.ImportResolutionStates");
        this.PrepareStoredProcedure("TestManagement.prc_ImportResolutionStates");
        List<string> names = new List<string>();
        List<TestResolutionState> inUseResolutionStates = new List<TestResolutionState>();
        int lazyInitialization = this.GetDataspaceIdWithLazyInitialization(projectId);
        foreach (TestResolutionState state in states)
          names.Add(state.Name);
        this.BindNameTypeTable("@targetResolutionStates", (IEnumerable<string>) names);
        this.BindInt("@dataspaceId", lazyInitialization);
        this.BindGuid("@userIdentity", userIdentity);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.QueryTestResolutionStatesColumns resolutionStatesColumns = new TestManagementDatabase.QueryTestResolutionStatesColumns();
        while (reader.Read())
          inUseResolutionStates.Add(resolutionStatesColumns.bind(reader));
        if (inUseResolutionStates.Count != 0)
          throw new TestObjectInUseException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ResolutionStatesAreInUse, (object) this.ToStringResolutionStates(inUseResolutionStates)));
        List<TestResolutionState> testResolutionStateList = new List<TestResolutionState>();
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            testResolutionStateList.Add(resolutionStatesColumns.bind(reader));
        }
        return testResolutionStateList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.ImportResolutionStates");
      }
    }

    protected string ToStringResolutionStates(List<TestResolutionState> inUseResolutionStates)
    {
      StringBuilder stringBuilder = new StringBuilder(inUseResolutionStates.Count);
      bool flag = true;
      foreach (TestResolutionState useResolutionState in inUseResolutionStates)
      {
        if (!flag)
        {
          stringBuilder.Append(", ");
          stringBuilder.Append(useResolutionState.Name);
        }
        else
        {
          stringBuilder.Append(useResolutionState.Name);
          flag = false;
        }
      }
      return stringBuilder.ToString();
    }

    public virtual List<int> CreateLogEntriesForRun(
      Guid projectId,
      int testRunId,
      int testMessageLogId,
      TestMessageLogEntry[] entries)
    {
      List<int> logEntriesForRun = new List<int>(entries.Length);
      this.PrepareStoredProcedure("prc_CreateTestMessageLogEntriesForRun");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testMessageLogId", testMessageLogId);
      this.BindTestMessageLogEntryTypeTable("@entriesTable", (IEnumerable<TestMessageLogEntry>) entries);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("EntryId");
      while (reader.Read())
        logEntriesForRun.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      return logEntriesForRun;
    }

    public virtual List<TestMessageLogEntry> QueryLogEntriesForRun(
      Guid projectId,
      int testRunId,
      int testMessageLogId)
    {
      List<TestMessageLogEntry> testMessageLogEntryList = new List<TestMessageLogEntry>();
      this.PrepareStoredProcedure("prc_QueryTestMessageLogEntriesForRun");
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testMessageLogId", testMessageLogId);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryTestMessageLogentryColumns messageLogentryColumns = new TestManagementDatabase.QueryTestMessageLogentryColumns();
      while (reader.Read())
        testMessageLogEntryList.Add(messageLogentryColumns.bind(reader));
      return testMessageLogEntryList;
    }

    internal virtual void SaveMaxRVValueBeforeTestResultSchemaMigration()
    {
    }

    internal virtual void CleanDeletedTestRuns2(
      Guid projectId,
      List<int> runIds,
      int resultsDeletionBatchSize,
      int cleanDeletedTestRunsSprocExecTimeOutInSec,
      int reuseTestRunIdThreshold)
    {
    }

    internal virtual List<RetainedResultsDistribution> QueryRetainedResultsDistribution(
      Guid projectId,
      int retainedBeyondDays,
      int queryRetainedResultsDistributionSprocExecTimeOutInSec)
    {
      return new List<RetainedResultsDistribution>();
    }

    internal virtual List<int> QuerySoftDeletedRuns(
      Guid projectId,
      int waitDaysForCleanup,
      int runsDeletionBatchSize,
      DateTime deleteStartDate)
    {
      return new List<int>();
    }

    internal virtual TestRun GetTestRunBasic(Guid projectid, int runId) => (TestRun) null;

    internal virtual void CleanDeletedTestRunDimensions(
      Guid projectId,
      int maxDimensionRowsToDelete,
      int deletionBatchSize,
      int waitDurationForCleanup,
      out int? deletedTestCaseRefs,
      int cleanDeletedTestRunDimensionsSprocExecTimeOutInSec)
    {
      deletedTestCaseRefs = new int?();
    }

    public virtual UpdatedProperties AbortTestRun(
      Guid projectId,
      int testRunId,
      int revision,
      TestRunAbortOptions options,
      byte substate,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_AbortTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@revision", revision);
        this.BindInt("@options", (int) options);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@substate", substate);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_AbortTestRun");
        updatedProperties.LastUpdatedBy = updatedBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return updatedProperties;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
      }
    }

    public virtual UpdatedProperties CancelTestRun(
      Guid projectId,
      int testRunId,
      Guid canceledBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.CancelTestRun");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_CancelTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindGuid("@canceledBy", canceledBy);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_CancelTestRun");
        updatedProperties.LastUpdatedBy = canceledBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return updatedProperties;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CancelTestRun");
      }
    }

    public virtual List<TestRun> QueryTestRuns(
      int testRunId,
      Guid owner,
      string buildUri,
      Guid projectId,
      out Dictionary<int, string> iterationMap,
      out Dictionary<Guid, List<TestRun>> projectsRunsMap,
      int planId = -1,
      int skip = 0,
      int top = 2147483647,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
        Dictionary<int, TestRun> dictionary1 = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("prc_QueryTestRuns");
        this.BindNullableInt("@testRunId", testRunId, 0);
        this.BindGuidPreserveNull("@owner", owner);
        this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        TestManagementDatabase.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        iterationMap = new Dictionary<int, string>();
        projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        while (reader.Read())
        {
          int dataspaceId;
          string iterationUri;
          TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          if (!string.IsNullOrEmpty(iterationUri))
            iterationMap[testRun.TestRunId] = iterationUri;
          if (dictionary2.ContainsKey(dataspaceId))
          {
            Guid key = dictionary2[dataspaceId];
            projectsRunsMap[key].Add(testRun);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary2[dataspaceId] = dataspaceIdentifier;
            projectsRunsMap[dataspaceIdentifier] = new List<TestRun>();
            projectsRunsMap[dataspaceIdentifier].Add(testRun);
          }
          dictionary1.Add(testRun.TestRunId, testRun);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRuns");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary1.ContainsKey(tuple.Item1))
          {
            TestRun testRun = dictionary1[tuple.Item1];
            testRun.CustomFields = testRun.CustomFields ?? new List<TestExtensionField>();
            testRun.CustomFields.Add(tuple.Item2);
          }
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
      }
    }

    public virtual List<TestRun> QueryTestRuns3(
      Guid projectId,
      int testRunId,
      Guid owner,
      string buildUri,
      int planId,
      int skip,
      int top,
      bool isTcmService = false)
    {
      return new List<TestRun>();
    }

    public virtual List<TestRun> QueryTestRuns2(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      out Dictionary<int, string> iterationMap,
      out Dictionary<Guid, List<TestRun>> projectsRunsMap)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns2");
        Dictionary<int, TestRun> dictionary1 = new Dictionary<int, TestRun>();
        this.PrepareDynamicProcedure("prc_QueryTestRuns2");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        TestManagementDatabase.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        iterationMap = new Dictionary<int, string>();
        projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        while (reader.Read())
        {
          int dataspaceId;
          string iterationUri;
          TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          if (!string.IsNullOrEmpty(iterationUri))
            iterationMap[testRun.TestRunId] = iterationUri;
          if (dictionary2.ContainsKey(dataspaceId))
          {
            Guid key = dictionary2[dataspaceId];
            projectsRunsMap[key].Add(testRun);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary2[dataspaceId] = dataspaceIdentifier;
            projectsRunsMap[dataspaceIdentifier] = new List<TestRun>();
            projectsRunsMap[dataspaceIdentifier].Add(testRun);
          }
          dictionary1.Add(testRun.TestRunId, testRun);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRuns2");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary1.ContainsKey(tuple.Item1))
          {
            TestRun testRun = dictionary1[tuple.Item1];
            testRun.CustomFields = testRun.CustomFields ?? new List<TestExtensionField>();
            testRun.CustomFields.Add(tuple.Item2);
          }
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns2");
      }
    }

    public virtual List<TestCaseResult> QueryTestResultsByCategory(
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration buildRef,
      string categoryName)
    {
      return new List<TestCaseResult>();
    }

    internal virtual TestRun QueryTestRunByTmiRunId(
      Guid tmiRunId,
      out string iterationUri,
      out Guid runProjGuid)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunByTmiRunId");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_QueryTestRunByTmiRunId");
        this.BindGuid("@tmiRunId", tmiRunId);
        TestManagementDatabase.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          return (TestRun) null;
        int dataspaceId;
        TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
        runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        return testRun;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunByTmiRunId");
      }
    }

    internal virtual TestCaseResult ResetTestResult(
      Guid projectId,
      int testRunId,
      int testResultId,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_ResetTestResult");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@testResultId", testResultId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        SqlDataReader reader = this.ExecuteReader();
        TestCaseResult testCaseResult = reader.Read() ? new TestManagementDatabase.FetchTestResultsColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_ResetTestResult");
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return testCaseResult;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
      }
    }

    private List<Tuple<int, int, TestExtensionField>> GetExtensionFieldsMap(
      int testRunId,
      IEnumerable<TestCaseResult> results,
      bool useOrderIndex)
    {
      List<Tuple<int, int, TestExtensionField>> extensionFieldsMap = new List<Tuple<int, int, TestExtensionField>>();
      int orderIndex = 0;
      foreach (TestCaseResult result1 in results)
      {
        TestCaseResult result = result1;
        if (result.StackTrace != null)
          extensionFieldsMap.Add(new Tuple<int, int, TestExtensionField>(testRunId, useOrderIndex ? orderIndex : result.TestResultId, result.StackTrace));
        if (result.CustomFields != null && result.CustomFields.Any<TestExtensionField>())
          extensionFieldsMap.AddRange(result.CustomFields.Select<TestExtensionField, Tuple<int, int, TestExtensionField>>((System.Func<TestExtensionField, Tuple<int, int, TestExtensionField>>) (f => new Tuple<int, int, TestExtensionField>(testRunId, useOrderIndex ? orderIndex : result.TestResultId, f))));
        orderIndex++;
      }
      return extensionFieldsMap;
    }

    internal virtual void UpdateTestResolutionState(Guid projectId, TestResolutionState state)
    {
      this.PrepareStoredProcedure("prc_UpdateTestResolutionState");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@stateId", state.Id);
      this.BindString("@name", state.Name, 256, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal virtual void UpdateTestFailureType(Guid projectId, TestFailureType failureType)
    {
      this.PrepareStoredProcedure("prc_UpdateTestFailureType");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@failureTypeId", failureType.Id);
      this.BindString("@name", failureType.Name, 256, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal virtual void DeleteTestResolutionState(
      Guid projectId,
      int stateId,
      Guid auditIdentity)
    {
      this.PrepareStoredProcedure("TestManagement.prc_DeleteTestResolutionState");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@stateId", stateId);
      this.BindGuid("@auditIdentity", auditIdentity);
      this.ExecuteNonQuery();
    }

    internal void DeleteTestFailureType(Guid projectId, int failureTypeId, Guid auditIdentity)
    {
      this.PrepareStoredProcedure("TestManagement.prc_DeleteTestFailureType");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@failureTypeId", failureTypeId);
      this.BindGuid("@auditIdentity", auditIdentity);
      this.ExecuteNonQuery();
    }

    public virtual TestRun CreateTestRun(
      Guid projectId,
      TestRun testRun,
      Guid updatedBy,
      bool changeCounterInterval = false,
      bool isTcmService = false,
      bool reuseDeletedTestRunId = false,
      int reuseTestRunIdDays = 2)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "TestResultDatabase.CreateTestRun"))
        {
          this.PrepareStoredProcedure("prc_CreateTestRun");
          this.BindString("@title", testRun.Title, 256, false, SqlDbType.NVarChar);
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindGuid("@owner", testRun.Owner);
          this.BindByte("@state", testRun.State);
          this.BindString("@dropLocation", testRun.DropLocation, 260, true, SqlDbType.NVarChar);
          this.BindInt("@testPlanId", testRun.TestPlanId);
          this.BindNullableDateTime("@dueDate", testRun.DueDate);
          this.BindInt("@iterationId", testRun.IterationId);
          this.BindString("@controller", testRun.Controller, 256, true, SqlDbType.NVarChar);
          this.BindInt("@testMessageLogId", testRun.TestMessageLogId);
          this.BindInt("@testSettingsId", testRun.TestSettingsId);
          this.BindInt("@publicTestSettingsId", testRun.PublicTestSettingsId);
          this.BindGuid("@testEnvironmentId", testRun.TestEnvironmentId);
          this.BindString("@legacySharePath", testRun.LegacySharePath, 1024, false, SqlDbType.NVarChar);
          this.BindBoolean("@isAutomated", testRun.IsAutomated);
          this.BindByte("@type", testRun.Type);
          this.BindGuid("@lastUpdatedBy", updatedBy);
          this.BindInt("@version", testRun.Version);
          List<BuildConfiguration> builds;
          if (testRun.BuildReference == null)
          {
            builds = (List<BuildConfiguration>) null;
          }
          else
          {
            builds = new List<BuildConfiguration>();
            builds.Add(testRun.BuildReference);
          }
          this.BindBuildRefTypeTable("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
          List<ReleaseReference> releases;
          if (testRun.ReleaseReference == null)
          {
            releases = (List<ReleaseReference>) null;
          }
          else
          {
            releases = new List<ReleaseReference>();
            releases.Add(testRun.ReleaseReference);
          }
          this.BindReleaseRefTypeTable("@releaseRefData", (IEnumerable<ReleaseReference>) releases);
          this.BindTestExtensionFieldValuesTypeTable("@additionalFields", testRun.CustomFields != null ? testRun.CustomFields.Select<TestExtensionField, Tuple<int, int, TestExtensionField>>((System.Func<TestExtensionField, Tuple<int, int, TestExtensionField>>) (f => new Tuple<int, int, TestExtensionField>(testRun.TestRunId, 0, f))) : (IEnumerable<Tuple<int, int, TestExtensionField>>) null);
          this.BindString("@sourceWorkflow", string.Empty, 128, false, SqlDbType.NVarChar);
          this.BindBoolean("@isBvt", testRun.IsBvt);
          this.BindString("@testEnvironmentUrl", !testRun.RunHasDtlEnvironment || testRun.DtlTestEnvironment == null ? (string) null : testRun.DtlTestEnvironment.Url, 2048, true, SqlDbType.NVarChar);
          this.BindString("@autEnvironmentUrl", !testRun.RunHasDtlEnvironment || testRun.DtlAutEnvironment == null ? (string) null : testRun.DtlAutEnvironment.Url, 2048, true, SqlDbType.NVarChar);
          this.BindString("@sourceFilter", !testRun.RunHasDtlEnvironment || !testRun.IsAutomated ? (string) null : testRun.Filter.SourceFilter, 1024, true, SqlDbType.NVarChar);
          this.BindString("@TestCaseFilter", !testRun.RunHasDtlEnvironment || !testRun.IsAutomated ? (string) null : testRun.Filter.TestCaseFilter, 2048, true, SqlDbType.NVarChar);
          SqlDataReader reader = this.ExecuteReader();
          return reader.Read() ? new TestManagementDatabase.CreateTestRunColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateTestRun");
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual int CreateProject(
      string projectUri,
      Guid projectId,
      string projectName,
      int sequenceId,
      out bool isNewProject)
    {
      this.PrepareStoredProcedure("prc_CreateProject");
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@projectName", projectName, 256, false, SqlDbType.NVarChar);
      this.BindInt("@sequenceId", sequenceId);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("ProjectId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("IsNewProject");
      int project = reader.Read() ? sqlColumnBinder1.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_CreateProject");
      isNewProject = sqlColumnBinder2.GetBoolean((IDataReader) reader);
      if (!isNewProject)
        return project;
      this.CreateDefaultTestFailureTypes(projectId);
      return project;
    }

    internal void CreateDefaultTestFailureTypes(Guid projectId)
    {
      this.EnsureTestFailureTypeCreated(ServerResources.DefaultFailureType_None, projectId);
      this.EnsureTestFailureTypeCreated(ServerResources.DefaultFailureType_Regression, projectId);
      this.EnsureTestFailureTypeCreated(ServerResources.DefaultFailureType_NewIssue, projectId);
      this.EnsureTestFailureTypeCreated(ServerResources.DefaultFailureType_KnownIssue, projectId);
      this.EnsureTestFailureTypeCreated(ServerResources.DefaultFailureType_Unknown, projectId);
      this.EnsureTestFailureTypeCreated(ServerResources.DefaultFailureType_NullValue, projectId);
    }

    private void EnsureTestFailureTypeCreated(string failureTypeName, Guid projectId)
    {
      try
      {
        this.CreateTestFailureType(new TestFailureType()
        {
          Name = failureTypeName
        }, projectId);
      }
      catch (Exception ex)
      {
        this.RequestContext.Trace(0, TraceLevel.Warning, "TestManagement", "Database", "EnsureTestFailureTypeCreated Warning: {0}", (object) ex.Message);
      }
    }

    internal bool QueueDeleteProject(string projectUri)
    {
      this.PrepareStoredProcedure("prc_QueueDeleteProject");
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("IsCompleted");
      return reader.Read() ? sqlColumnBinder.GetBoolean((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_QueueDeleteProject");
    }

    internal virtual int CleanupProject(
      int resumeStage,
      Guid projectId,
      string projectUri,
      int cleanupProjectSprocExecTimeOutInSec)
    {
      this.PrepareStoredProcedure("TestManagement.prc_CleanupProject", cleanupProjectSprocExecTimeOutInSec);
      this.BindInt("@resumeStage", resumeStage);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      return (int) this.ExecuteScalar();
    }

    internal virtual List<GuidAndString> QueryProjects(
      TestManagementRequestContext context,
      bool queryDeleted)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryProjects");
        return this.QueryProjectsWithDataspaceIds(queryDeleted).Select<KeyValuePair<GuidAndString, int>, GuidAndString>((System.Func<KeyValuePair<GuidAndString, int>, GuidAndString>) (kv => kv.Key)).ToList<GuidAndString>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryProjects");
      }
    }

    internal virtual List<KeyValuePair<GuidAndString, int>> QueryProjectsWithDataspaceIds(
      bool queryDeleted)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryProjectsWithDataspaceIds");
        this.PrepareStoredProcedure("prc_QueryProjects");
        this.BindBoolean("@isDeleted", queryDeleted);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("DataspaceId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("ProjectUri");
        List<KeyValuePair<GuidAndString, int>> keyValuePairList = new List<KeyValuePair<GuidAndString, int>>();
        while (reader.Read())
        {
          int int32 = sqlColumnBinder1.GetInt32((IDataReader) reader);
          string str = sqlColumnBinder2.GetString((IDataReader) reader, false);
          keyValuePairList.Add(new KeyValuePair<GuidAndString, int>(new GuidAndString(str, this.GetDataspaceIdentifier(int32)), int32));
        }
        return keyValuePairList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryProjectsWithDataspaceIds");
      }
    }

    internal Dictionary<string, IdAndString> QueryAreaPaths()
    {
      this.PrepareStoredProcedure("prc_QueryAreaPaths");
      return new TestManagementDatabase.QueryStructuresColumns().bind(this.ExecuteReader());
    }

    internal Dictionary<string, IdAndString> QueryIterations()
    {
      this.PrepareStoredProcedure("prc_QueryIterations");
      return new TestManagementDatabase.QueryStructuresColumns().bind(this.ExecuteReader());
    }

    internal virtual Dictionary<string, IdAndString> CreateArea(
      string areaUri,
      string areaPath,
      string projectUri,
      int sequenceId)
    {
      this.PrepareStoredProcedure("prc_CreateArea");
      this.BindString("@areaUri", areaUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@areaPath", DBPath.UserToDatabasePath(areaPath, false, false), 4000, false, SqlDbType.NVarChar);
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(new Guid(projectUri.Substring(projectUri.Length - 36))));
      this.BindInt("@sequenceId", sequenceId);
      return new TestManagementDatabase.QueryStructuresColumns().bind(this.ExecuteReader());
    }

    internal virtual string GetProjectUriForArea(string areaUri)
    {
      this.PrepareStoredProcedure("prc_GetProjectUriForArea");
      this.BindString("@areaUri", areaUri, 256, false, SqlDbType.NVarChar);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        string projectUriForArea = (string) null;
        if (sqlDataReader.Read())
          projectUriForArea = sqlDataReader.GetString(0);
        return projectUriForArea;
      }
    }

    internal virtual string GetProjectUriForIteration(string iterationUri)
    {
      this.PrepareStoredProcedure("prc_GetProjectUriForIteration");
      this.BindString("@iterationUri", iterationUri, 256, false, SqlDbType.NVarChar);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        string projectUriForIteration = (string) null;
        if (sqlDataReader.Read())
          projectUriForIteration = sqlDataReader.GetString(0);
        return projectUriForIteration;
      }
    }

    internal virtual Dictionary<string, IdAndString> CreateIteration(
      string iterationUri,
      string iteration,
      string projectUri,
      int sequenceId)
    {
      this.PrepareStoredProcedure("prc_CreateIteration");
      this.BindString("@iterationUri", iterationUri, 256, true, SqlDbType.NVarChar);
      this.BindString("@iteration", DBPath.UserToDatabasePath(iteration, false, false), 4000, true, SqlDbType.NVarChar);
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(new Guid(projectUri.Substring(projectUri.Length - 36))));
      this.BindInt("@sequenceId", sequenceId);
      return new TestManagementDatabase.QueryStructuresColumns().bind(this.ExecuteReader());
    }

    internal void UpdateCssNodeUri(string oldUri, string newUri)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateCssNodeUri");
      this.BindString("@oldUri", oldUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@newUri", newUri, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual List<TestResolutionState> QueryTestResolutionStates(int stateId, Guid projectId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResolutionStates");
        List<TestResolutionState> testResolutionStateList = new List<TestResolutionState>();
        int lazyInitialization = this.GetDataspaceIdWithLazyInitialization(projectId);
        this.PrepareStoredProcedure("prc_QueryTestResolutionStates");
        this.BindNullableInt("@stateId", stateId, 0);
        this.BindInt("@dataspaceId", lazyInitialization);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.QueryTestResolutionStatesColumns resolutionStatesColumns = new TestManagementDatabase.QueryTestResolutionStatesColumns();
        while (reader.Read())
          testResolutionStateList.Add(resolutionStatesColumns.bind(reader));
        return testResolutionStateList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResolutionStates");
      }
    }

    public virtual List<TestFailureType> QueryTestFailureTypes(int failureTypeId, Guid projectId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestFailureTypes");
        List<TestFailureType> testFailureTypeList = new List<TestFailureType>();
        int lazyInitialization = this.GetDataspaceIdWithLazyInitialization(projectId);
        this.PrepareStoredProcedure("prc_QueryTestFailureTypes");
        this.BindNullableInt("@failureTypeId", failureTypeId, -1);
        this.BindInt("@dataspaceId", lazyInitialization);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.QueryTestFailureTypesColumns failureTypesColumns = new TestManagementDatabase.QueryTestFailureTypesColumns();
        while (reader.Read())
          testFailureTypeList.Add(failureTypesColumns.bind(reader));
        return testFailureTypeList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestFailureTypes");
      }
    }

    public virtual void QueueDeleteTestRun(
      Guid projectId,
      int testRunId,
      Guid updatedBy,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueueDeleteTestRun");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.ExecuteNonQuery();
    }

    internal bool CleanUnassociatedBuildCoverage()
    {
      this.PrepareStoredProcedure("prc_DeleteUnassociatedBuildCoverage");
      return (int) this.ExecuteScalar() == 0;
    }

    internal virtual void QueryTestActionResults(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      Guid projectId,
      out List<TestActionResult> results,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments,
      out string areaUri)
    {
      results = new List<TestActionResult>();
      parameters = new List<TestResultParameter>();
      attachments = new List<TestResultAttachment>();
      areaUri = string.Empty;
      this.PrepareStoredProcedure("TestResult.prc_QueryTestActionResults");
      this.BindNullableInt("@testRunId", testRunId, 0);
      this.BindNullableInt("@testResultId", testResultId, 0);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      areaUri = this.GetResultAreaUri(reader, testRunId, testResultId);
      this.ReadTestActionResults(results, parameters, attachments, reader, "prc_QueryTestActionResults");
    }

    protected virtual void ReadTestActionResults(
      List<TestActionResult> results,
      List<TestResultParameter> parameters,
      List<TestResultAttachment> attachments,
      SqlDataReader reader,
      string sprocName)
    {
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(sprocName);
      TestManagementDatabase.QueryTestActionResultsColumns actionResultsColumns = new TestManagementDatabase.QueryTestActionResultsColumns();
      while (reader.Read())
        results.Add(actionResultsColumns.bind(reader));
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(sprocName);
      TestManagementDatabase.QueryTestActionResultsColumns2 actionResultsColumns2 = new TestManagementDatabase.QueryTestActionResultsColumns2();
      while (reader.Read())
        parameters.Add(actionResultsColumns2.bind(reader));
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(sprocName);
      TestManagementDatabase.QueryAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryAttachmentsColumns();
      while (reader.Read())
        attachments.Add(attachmentsColumns.Bind(reader));
    }

    internal virtual List<TestCaseResult> FetchTestResults(
      TestCaseResultIdAndRev[] resultIds,
      List<TestCaseResultIdentifier> deletedIds,
      Guid projectId)
    {
      return this.FetchTestResults(resultIds, deletedIds, projectId, false, out List<TestActionResult> _, out List<TestResultParameter> _, out List<TestResultAttachment> _);
    }

    public virtual List<TestCaseResult> FetchTestResults(
      TestCaseResultIdAndRev[] resultIds,
      List<TestCaseResultIdentifier> deletedIds,
      Guid projectId,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
        this.PrepareStoredProcedure("prc_FetchTestResults");
        this.BindBoolean("@includeActionResults", includeActionResults);
        this.BindTestCaseResultIdAndRevTypeTable("@idsTable", (IEnumerable<TestCaseResultIdAndRev>) resultIds);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        Dictionary<TestCaseResultIdentifier, TestCaseResult> dictionary = new Dictionary<TestCaseResultIdentifier, TestCaseResult>(resultIds.Length);
        TestManagementDatabase.FetchTestResultsColumns testResultsColumns1 = new TestManagementDatabase.FetchTestResultsColumns();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          while (reader.Read())
          {
            TestCaseResult testCaseResult = testResultsColumns1.bind(reader);
            testCaseResult.CustomFields = new List<TestExtensionField>();
            dictionary.Add(new TestCaseResultIdentifier(testCaseResult.TestRunId, testCaseResult.TestResultId), testCaseResult);
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_FetchTestResults");
          TestManagementDatabase.FetchTestResultsExColumns resultsExColumns = new TestManagementDatabase.FetchTestResultsExColumns();
          while (reader.Read())
          {
            Tuple<int, int, TestExtensionField> tuple = resultsExColumns.bind(reader);
            TestCaseResultIdentifier key = new TestCaseResultIdentifier(tuple.Item1, tuple.Item2);
            if (dictionary.ContainsKey(key))
            {
              if (string.Equals("StackTrace", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                dictionary[key].StackTrace = tuple.Item3;
              else if (string.Equals("FailingSince", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string json)
                  dictionary[key].FailingSince = JsonUtilities.Deserialize<FailingSince>(json, true);
              }
              else
                dictionary[key].CustomFields.Add(tuple.Item3);
            }
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_FetchTestResults");
          if (deletedIds != null)
          {
            TestManagementDatabase.QueryTestResultsColumns testResultsColumns2 = new TestManagementDatabase.QueryTestResultsColumns();
            while (reader.Read())
              deletedIds.Add(testResultsColumns2.bindDeleted(reader));
          }
          if (includeActionResults)
          {
            actionResults = new List<TestActionResult>();
            parameters = new List<TestResultParameter>();
            attachments = new List<TestResultAttachment>();
            this.ReadTestActionResults(actionResults, parameters, attachments, reader, "prc_FetchTestResults");
          }
          else
          {
            actionResults = (List<TestActionResult>) null;
            parameters = (List<TestResultParameter>) null;
            attachments = (List<TestResultAttachment>) null;
          }
        }
        return dictionary.Values.ToList<TestCaseResult>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
      }
    }

    public virtual TestCaseResult FetchTestResult(
      TestCaseResultIdentifier resultId,
      Guid projectId,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      actionResults = (List<TestActionResult>) null;
      parameters = (List<TestResultParameter>) null;
      attachments = (List<TestResultAttachment>) null;
      return new TestCaseResult();
    }

    internal virtual TestCaseResult FetchTestResultV2(
      TestCaseResultIdentifier resultId,
      Guid projectId,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      actionResults = (List<TestActionResult>) null;
      parameters = (List<TestResultParameter>) null;
      attachments = (List<TestResultAttachment>) null;
      return new TestCaseResult();
    }

    internal virtual List<TestCaseResultIdentifier> QueryTestResults(
      int testRunId,
      Guid owner,
      byte testStatus,
      List<byte> outcomes,
      int afnStripId,
      Guid projectId,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindNullableInt("@testRunId", testRunId, 0);
      this.BindGuidPreserveNull("@owner", owner);
      this.BindNullableByte("@state", testStatus, (byte) 0);
      this.BindNullableByte("@outcome", outcomes == null || !outcomes.Any<byte>() ? (byte) 0 : outcomes.FirstOrDefault<byte>(), (byte) 0);
      this.BindNullableInt("@afnStripId", afnStripId, 0);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
      TestManagementDatabase.QueryTestResultsColumns testResultsColumns = new TestManagementDatabase.QueryTestResultsColumns();
      while (reader.Read())
        resultIdentifierList.Add(testResultsColumns.bind(reader));
      return resultIdentifierList;
    }

    internal virtual List<TestCaseResult> QueryTestResultsByPoint(
      Guid projectId,
      int planId,
      int pointId,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultsByPoint");
        List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByPoint");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@planId", planId);
        this.BindInt("@testPointId", pointId);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
        while (reader.Read())
        {
          TestCaseResult testCaseResult = testResultsColumns.bind(reader);
          testCaseResultList.Add(testCaseResult);
        }
        return testCaseResultList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultsByPoint");
      }
    }

    public virtual List<TestCaseResultIdentifier> QueryTestResults2(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      this.PrepareDynamicProcedure("TestResult.prc_QueryTestResults2");
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
      TestManagementDatabase.QueryTestResultsColumns testResultsColumns = new TestManagementDatabase.QueryTestResultsColumns();
      while (reader.Read())
        resultIdentifierList.Add(testResultsColumns.bind(reader));
      return resultIdentifierList;
    }

    public virtual List<TestCaseResult> QueryTestResultHistory(
      Guid projectId,
      string automatedTestName,
      int testCaseId,
      DateTime maxCompleteDate,
      int historyDays)
    {
      return new List<TestCaseResult>();
    }

    public virtual List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QueryTestResultTrendReport(
      Guid projectId,
      ResultsFilter filter)
    {
      return new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
    }

    internal string GetResultAreaUri(SqlDataReader reader, int testRunId, int testResultId) => reader.Read() ? new TestManagementDatabase.QueryAreaUriColumns().AreaUri.GetString((IDataReader) reader, true) : throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) testRunId, (object) testResultId), ObjectTypes.TestResult);

    internal virtual List<int> QueryTestRunIds(
      string whereClause,
      string orderByClause,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunIds");
      try
      {
        List<int> intList = new List<int>();
        this.PrepareDynamicProcedure("prc_QueryTestRunIds");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderByClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestRunId");
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return intList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunIds");
      }
    }

    internal virtual List<LegacyTestRun> QueryLegacyTestRuns(
      string buildUri,
      string buildPlatform,
      string buildFlavor,
      out Dictionary<Guid, List<LegacyTestRun>> projectsRunsMap)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryLegacyTestRuns");
        List<LegacyTestRun> legacyTestRunList = new List<LegacyTestRun>();
        this.PrepareStoredProcedure("prc_QueryLegacyRunsByBuild");
        this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
        this.BindString("@buildPlatform", buildPlatform, 256, true, SqlDbType.NVarChar);
        this.BindString("@buildFlavor", buildFlavor, 256, true, SqlDbType.NVarChar);
        TestManagementDatabase.QueryLegacyRunsColumns legacyRunsColumns = new TestManagementDatabase.QueryLegacyRunsColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, Guid> dictionary = new Dictionary<int, Guid>();
        projectsRunsMap = new Dictionary<Guid, List<LegacyTestRun>>();
        while (reader.Read())
        {
          int dataspaceId;
          LegacyTestRun legacyTestRun = legacyRunsColumns.bind(reader, out dataspaceId);
          if (dictionary.ContainsKey(dataspaceId))
          {
            Guid key = dictionary[dataspaceId];
            projectsRunsMap[key].Add(legacyTestRun);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary[dataspaceId] = dataspaceIdentifier;
            projectsRunsMap[dataspaceIdentifier] = new List<LegacyTestRun>();
            projectsRunsMap[dataspaceIdentifier].Add(legacyTestRun);
          }
          legacyTestRunList.Add(legacyTestRun);
        }
        return legacyTestRunList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryLegacyTestRuns");
      }
    }

    internal virtual List<IdAndRev> GetRunsAssociatedWithRelease(
      Guid projectId,
      byte state,
      int releaseId,
      int releaseEnvId)
    {
      return new List<IdAndRev>();
    }

    public virtual Dictionary<int, List<TestRunStatistic>> QueryTestRunStatistics(
      List<int> testRunIds,
      Guid projectId,
      bool isTcmService = false,
      bool shouldReturnStatsIfNotComputed = true)
    {
      Dictionary<int, TestResolutionState> resolutionStates = new Dictionary<int, TestResolutionState>();
      List<TestRunStatistic> testRunStatisticList = new List<TestRunStatistic>();
      int lazyInitialization = this.GetDataspaceIdWithLazyInitialization(projectId);
      this.PrepareStoredProcedure("TestResult.prc_QueryTestRunStatistics");
      this.BindInt("@dataspaceId", lazyInitialization);
      this.BindIdTypeTable("@testRunIdTable", (IEnumerable<int>) testRunIds);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryTestResolutionStatesColumns resolutionStatesColumns = new TestManagementDatabase.QueryTestResolutionStatesColumns();
      while (reader.Read())
      {
        TestResolutionState testResolutionState = resolutionStatesColumns.bind(reader);
        resolutionStates.Add(testResolutionState.Id, testResolutionState);
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunStatistics");
      TestManagementDatabase.QueryTestRunStatisticsColumns statisticsColumns = new TestManagementDatabase.QueryTestRunStatisticsColumns();
      while (reader.Read())
      {
        TestRunStatistic testRunStatistic = statisticsColumns.Bind(reader, (IDictionary<int, TestResolutionState>) resolutionStates);
        testRunStatisticList.Add(testRunStatistic);
      }
      Dictionary<int, List<TestRunStatistic>> dictionary = new Dictionary<int, List<TestRunStatistic>>();
      foreach (TestRunStatistic testRunStatistic in testRunStatisticList)
      {
        if (!dictionary.ContainsKey(testRunStatistic.TestRunId))
          dictionary[testRunStatistic.TestRunId] = new List<TestRunStatistic>();
        dictionary[testRunStatistic.TestRunId].Add(testRunStatistic);
      }
      return dictionary;
    }

    internal void UpdateReplicationState(int cssSequenceId, string destroyedWorkItem)
    {
      this.PrepareStoredProcedure("prc_UpdateReplicationState");
      this.BindInt("@cssSequenceId", cssSequenceId);
      this.BindString("@destroyedWorkItem", destroyedWorkItem, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    protected bool GetTestCaseResultsFromResultUpdateRequests(
      IEnumerable<ResultUpdateRequest> resultsForUpdate,
      List<ResultUpdateResponse> responses,
      HashSet<TestCaseResult> results,
      HashSet<TestActionResult> actionsUpdated,
      HashSet<TestActionResult> actionsDeleted,
      HashSet<TestResultParameter> parametersUpdated,
      HashSet<TestResultParameter> parametersDeleted)
    {
      bool resultUpdateRequests = false;
      if (resultsForUpdate != null)
      {
        foreach (ResultUpdateRequest resultUpdateRequest in resultsForUpdate)
        {
          TestCaseResult testCaseResult = resultUpdateRequest.TestCaseResult;
          if (testCaseResult == null)
          {
            responses.Add(new ResultUpdateResponse()
            {
              Revision = -2,
              TestResultId = resultUpdateRequest.TestResultId
            });
          }
          else
          {
            Validator.CheckStartEndDatesInOrder(testCaseResult.DateStarted, testCaseResult.DateCompleted);
            resultUpdateRequests = true;
            results.Add(testCaseResult);
            if (resultUpdateRequest.ActionResults != null)
              actionsUpdated.UnionWith((IEnumerable<TestActionResult>) resultUpdateRequest.ActionResults);
            if (resultUpdateRequest.ActionResultDeletes != null)
              actionsDeleted.UnionWith((IEnumerable<TestActionResult>) resultUpdateRequest.ActionResultDeletes);
            if (resultUpdateRequest.Parameters != null)
              parametersUpdated.UnionWith((IEnumerable<TestResultParameter>) resultUpdateRequest.Parameters);
            if (resultUpdateRequest.ParameterDeletes != null)
              parametersDeleted.UnionWith((IEnumerable<TestResultParameter>) resultUpdateRequest.ParameterDeletes);
          }
        }
      }
      return resultUpdateRequests;
    }

    public virtual UpdatedRunProperties UpdateTestRun(
      Guid projectId,
      TestRun run,
      Guid updatedBy,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool skipRunStateTransitionCheck = false)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestResultDatabase.UpdateTestRun"))
      {
        this.PrepareStoredProcedure("prc_UpdateTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", run.TestRunId);
        this.BindStringPreserveNull("@title", run.Title, 256, SqlDbType.NVarChar);
        this.BindGuidPreserveNull("@owner", run.Owner);
        this.BindByte("@state", run.State, (byte) 0);
        this.BindNullableDateTime("@dueDate", run.DueDate);
        this.BindNullableInt("@iterationId", run.IterationId, 0);
        this.BindStringPreserveNull("@controller", run.Controller, 256, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@errorMessage", run.ErrorMessage, 512, SqlDbType.NVarChar);
        this.BindNullableDateTime("@dateStarted", run.StartDate);
        this.BindNullableDateTime("@dateCompleted", run.CompleteDate);
        this.BindInt("@testMessageLogId", run.TestMessageLogId);
        this.BindInt("@testSettingsId", run.TestSettingsId);
        this.BindInt("@publicTestSettingsId", run.PublicTestSettingsId);
        this.BindGuid("@testEnvironmentId", run.TestEnvironmentId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@postProcessState", run.PostProcessState, (byte) 0);
        this.BindInt("@version", run.Version);
        this.BindInt("@revision", run.Revision);
        this.BindBoolean("@isBvt", run.IsBvt);
        this.BindStringPreserveNull("@comment", run.Comment, 1048576, SqlDbType.NVarChar);
        this.BindByte("@substate", run.Substate, (byte) 0);
        this.BindString("@testEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlTestEnvironment == null ? (string) null : run.DtlTestEnvironment.Url, 2048, true, SqlDbType.NVarChar);
        this.BindString("@autEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlAutEnvironment == null ? (string) null : run.DtlAutEnvironment.Url, 2048, true, SqlDbType.NVarChar);
        this.BindString("@dtlCsmParameters", !run.RunHasDtlEnvironment || run.CsmParameters == null ? (string) null : run.CsmParameters, 2048, true, SqlDbType.NVarChar);
        this.BindBoolean("@skipRunStateTransitionCheck", skipRunStateTransitionCheck);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedRunProperties updatedRunProperties = reader.Read() ? new TestManagementDatabase.UpdatedPropertyColumns().bindUpdatedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestRun");
        updatedRunProperties.LastUpdatedBy = updatedBy;
        return updatedRunProperties;
      }
    }

    internal void QueryReplicationState(out int cssSequenceId, out string destroyedWorkItem)
    {
      this.PrepareStoredProcedure("prc_QueryReplicationState");
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_QueryReplicationState");
      TestManagementDatabase.ReplicationStateColumns replicationStateColumns = new TestManagementDatabase.ReplicationStateColumns();
      cssSequenceId = replicationStateColumns.CssSequenceId.GetInt32((IDataReader) reader, 0);
      destroyedWorkItem = replicationStateColumns.DestroyedWorkItem.GetString((IDataReader) reader, false);
    }

    internal int QueryObjectsCount(
      string whereClause,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      string tableName)
    {
      this.PrepareDynamicProcedure("prc_QueryObjectsCount");
      this.BindString("@tableName", tableName, 64, false, SqlDbType.NVarChar);
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("Count");
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? sqlColumnBinder.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_QueryObjectsCount");
    }

    internal virtual int CreateTestVariable(TestVariable variable, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_CreateVariable");
      this.BindString("@name", variable.Name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", variable.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindStringTable("@valuesTable", (IEnumerable<string>) variable.Values);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("VariableId");
      return reader.Read() ? sqlColumnBinder.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_CreateVariable");
    }

    internal virtual int UpdateTestVariable(Guid projectId, TestVariable variable)
    {
      this.PrepareStoredProcedure("prc_UpdateVariable");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@variableId", variable.Id);
      this.BindString("@name", variable.Name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", variable.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindStringTable("@valuesTable", (IEnumerable<string>) variable.Values);
      this.BindInt("@revision", variable.Revision);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("Revision");
      return reader.Read() ? sqlColumnBinder.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_UpdateVariable");
    }

    internal virtual void DeleteTestVariable(Guid projectId, int variableId, Guid auditIdentity)
    {
      this.PrepareStoredProcedure("prc_DeleteVariable");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@variableId", variableId);
      this.BindGuid("@auditIdentity", auditIdentity);
      this.ExecuteNonQuery();
    }

    internal virtual TestVariable QueryTestVariableById(int variableId, Guid projectId)
    {
      TestVariable testVariable = (TestVariable) null;
      this.PrepareStoredProcedure("prc_QueryVariableById");
      this.BindNullableInt("@variableId", variableId, 0);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
        testVariable = new TestManagementDatabase.QueryTestVariablesColumns().bind(reader);
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryVariableById");
      if (testVariable != null)
      {
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("Value");
        while (reader.Read())
          testVariable.Values.Add(sqlColumnBinder.GetString((IDataReader) reader, false));
      }
      return testVariable;
    }

    internal virtual List<TestVariable> QueryTestVariables(
      Guid projectId,
      int skip,
      int top,
      int watermark)
    {
      Dictionary<int, TestVariable> dictionary = new Dictionary<int, TestVariable>();
      this.PrepareStoredProcedure("prc_QueryVariablesByProject");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryTestVariablesColumns variablesColumns = new TestManagementDatabase.QueryTestVariablesColumns();
      while (reader.Read())
      {
        TestVariable testVariable = variablesColumns.bind(reader);
        dictionary[testVariable.Id] = testVariable;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryVariablesByProject");
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("VariableId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Value");
      while (reader.Read())
      {
        int int32 = sqlColumnBinder1.GetInt32((IDataReader) reader);
        string str = sqlColumnBinder2.GetString((IDataReader) reader, false);
        TestVariable testVariable;
        if (dictionary.TryGetValue(int32, out testVariable))
          testVariable.Values.Add(str);
      }
      List<TestVariable> testVariableList = new List<TestVariable>(dictionary.Count);
      foreach (TestVariable testVariable in dictionary.Values)
        testVariableList.Add(testVariable);
      return testVariableList;
    }

    internal virtual void CreateAssociatedWorkItems(
      TestCaseResultIdentifier[] identifiers,
      string[] workItemUris,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_CreateAssociatedWorkItems");
      this.BindTestCaseResultWorkItemLinkTypeTable("@workItemLinksTable", (IEnumerable<KeyValuePair<TestCaseResultIdentifier, string>>) this.GetListOfUris<TestCaseResultIdentifier>(identifiers, workItemUris));
      this.ExecuteNonQuery();
    }

    internal void DeleteAssociatedWorkItems(
      TestCaseResultIdentifier[] identifiers,
      string[] workItemUris)
    {
      this.PrepareStoredProcedure("prc_DeleteAssociatedWorkItems");
      this.BindTestCaseResultWorkItemLinkTypeTable("@workItemLinksTable", (IEnumerable<KeyValuePair<TestCaseResultIdentifier, string>>) this.GetListOfUris<TestCaseResultIdentifier>(identifiers, workItemUris));
      this.ExecuteNonQuery();
    }

    internal virtual Guid GetProjectForRun(int testRunId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.GetProjectForRun");
        this.PrepareStoredProcedure("prc_GetProjectForRun");
        this.BindNullableInt("@testRunId", testRunId, 0);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? this.GetDataspaceIdentifier(new SqlColumnBinder("DataspaceId").GetInt32((IDataReader) reader)) : Guid.Empty;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.GetProjectForRun");
      }
    }

    internal virtual Dictionary<int, List<string>> QueryAssociatedWorkItems(
      int[] pointIds,
      Guid projectId)
    {
      Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
      this.PrepareStoredProcedure("TestManagement.prc_QueryAssociatedWorkItems");
      this.BindIdTypeTable("@pointIdsTable", (IEnumerable<int>) pointIds);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("WorkItemUri");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("PointId");
      while (reader.Read())
      {
        int int32 = sqlColumnBinder2.GetInt32((IDataReader) reader);
        string str = sqlColumnBinder1.GetString((IDataReader) reader, false);
        List<string> stringList = (List<string>) null;
        if (!dictionary.TryGetValue(int32, out stringList))
        {
          stringList = new List<string>();
          dictionary.Add(int32, stringList);
        }
        stringList.Add(str);
      }
      return dictionary;
    }

    internal virtual List<string> QueryAssociatedWorkItems(int planId, Guid projectId)
    {
      List<string> stringList = new List<string>();
      this.PrepareStoredProcedure("TestManagement.prc_QueryAssociatedWorkItemsForPlan");
      this.BindInt("@planId", planId);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("WorkItemUri");
      while (reader.Read())
        stringList.Add(sqlColumnBinder.GetString((IDataReader) reader, false));
      return stringList;
    }

    public virtual List<TestCaseResult> FetchTestResultsWithSuiteDetails(
      TestCaseResultIdAndRev[] resultIds,
      Guid projectId)
    {
      return this.FetchTestResults(resultIds, (List<TestCaseResultIdentifier>) null, projectId);
    }

    public virtual int FindAndStoreMaxRunIdWithoutInsights() => 0;

    public virtual void DeleteSummaryAndInsightsForXamlBuilds(int batchSize)
    {
    }

    internal virtual List<TestRunRecord> QueryTestRunsByChangedDate(
      int projectId,
      int batchSize,
      string prBranchName,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      toDate = fromDate;
      return new List<TestRunRecord>();
    }

    internal virtual List<TestResultRecord> QueryTestResultsByTestRunChangedDate(
      int projectId,
      int runBatchSize,
      int resultBatchSize,
      string prBranchName,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark,
      TestArtifactSource dataSource,
      bool includeFlakyData = false)
    {
      toWatermark = fromWatermark;
      return new List<TestResultRecord>();
    }

    public virtual List<TestCaseReferenceRecord> QueryTestCaseReferenceByChangedDate(
      int projectId,
      int testCaseRefBatchSize,
      TestCaseReferenceWatermark fromWatermark,
      out TestCaseReferenceWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      toWatermark = new TestCaseReferenceWatermark()
      {
        ChangedDate = SqlDateTime.MinValue.Value,
        TestCaseReferenceId = 0
      };
      return new List<TestCaseReferenceRecord>();
    }

    internal virtual TestResultsGroupsData GetTestResultAutomatedTestStorageAndOwnersByBuild(
      Guid projectId,
      int buildId,
      string publishContext,
      int runIdThreshold = 0)
    {
      return new TestResultsGroupsData();
    }

    internal virtual TestResultsGroupsData GetTestResultAutomatedTestStorageAndOwnersByRelease(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      int runIdThreshold = 0)
    {
      return new TestResultsGroupsData();
    }

    internal virtual TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByBuild(
      Guid projectId,
      int buildId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold = 0)
    {
      return new TestResultsGroupsDataWithWaterMark();
    }

    internal virtual TestResultsGroupsDataWithWaterMark GetTestResultAutomatedTestStorageAndOwnersByRelease(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top,
      int runIdThreshold = 0)
    {
      return new TestResultsGroupsDataWithWaterMark();
    }

    internal virtual List<int> QueryTestRunIdsByChangedDate(
      int projectId,
      int batchSize,
      string prBranchName,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      toDate = fromDate;
      List<TestRunRecord> source = this.QueryTestRunsByChangedDate(projectId, batchSize, prBranchName, fromDate, out toDate, dataSource);
      return source == null || !source.Any<TestRunRecord>() ? new List<int>() : source.Select<TestRunRecord, int>((System.Func<TestRunRecord, int>) (x => x.TestRunId)).ToList<int>();
    }

    internal virtual List<TestCaseResult> QueryTestResultsByBuildOrRelease(
      Guid projectId,
      int buildId,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      IList<byte> runStates,
      bool fetchFailedTestsOnly,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      return new List<TestCaseResult>();
    }

    internal virtual List<TestCaseResult> QueryTestResultsByPipeline(
      Guid projectId,
      PipelineReference pipelineReference,
      IList<byte> runStates,
      bool fetchFailedTestsOnly,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      return new List<TestCaseResult>();
    }

    internal virtual List<TestCaseResult> FetchSuiteAndConfigurationDetails(
      Guid projectId,
      List<TestCaseResult> ids)
    {
      return new List<TestCaseResult>();
    }

    internal virtual List<TestCaseReferenceRecord> GetTestResultsMetaData(
      Guid projectId,
      IList<int> testcaseReferenceIds,
      bool shouldIncludeFlakyDetails = false)
    {
      return new List<TestCaseReferenceRecord>();
    }

    internal virtual void UpdateTestResultsMetaData(
      Guid projectId,
      int testCaseReferenceId,
      int maxBranchForFlakiness,
      List<TestBranchFlakinesStateMap> markFlakyMap,
      List<TestBranchFlakinesStateMap> unMarkFlakyMap)
    {
    }

    internal virtual void MarkTestCaseRefsFlaky(
      Guid projectId,
      List<int> testRunIds,
      List<int> allowedPipelines)
    {
    }

    internal virtual List<TestResultExArchivalRecord> QueryTestResultExtensionsByTestRunChangedDate(
      int dataspaceId,
      int runBatchSize,
      int resultExBatchSize,
      TestResultExArchivalWatermark fromWatermark,
      DateTime maxTestRunUpdatedDate,
      out TestResultExArchivalWatermark toWatermark,
      TestArtifactSource dataSource,
      List<string> fieldNames = null,
      List<int> runStates = null,
      List<int> excludedRunTypes = null)
    {
      toWatermark = new TestResultExArchivalWatermark()
      {
        TestRunUpdatedDate = SqlDateTime.MinValue.Value,
        TestRunId = 0,
        TestResultId = 0
      };
      return new List<TestResultExArchivalRecord>();
    }

    internal virtual List<PointsResults2> QueryManualTestResultsByUpdateDate(
      Guid projectId,
      int runBatchSize,
      int resultBatchSize,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark)
    {
      toWatermark = (TestResultWatermark) null;
      return new List<PointsResults2>();
    }

    protected List<KeyValuePair<T, string>> GetListOfUris<T>(T[] identifiers, string[] workItemUris)
    {
      List<KeyValuePair<T, string>> listOfUris = new List<KeyValuePair<T, string>>();
      if (identifiers == null || workItemUris == null)
        return listOfUris;
      if (identifiers.Length != workItemUris.Length)
        throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ParallelArraySizeMismatch, (object) nameof (identifiers), (object) nameof (workItemUris), (object) identifiers.Length, (object) workItemUris.Length));
      for (int index = 0; index < identifiers.Length; ++index)
        listOfUris.Add(new KeyValuePair<T, string>(identifiers[index], workItemUris[index]));
      return listOfUris;
    }

    private void BindXmlString(XmlTextWriter writer, string attributeName, string value)
    {
      if (value == null)
        return;
      writer.WriteStartAttribute(attributeName);
      writer.WriteValue(value);
    }

    private void BindXmlInt(XmlTextWriter writer, string attributeName, long value)
    {
      writer.WriteStartAttribute(attributeName);
      writer.WriteValue(value);
    }

    protected void BindStringPreserveNull(
      string parameterName,
      string value,
      int length,
      SqlDbType sqlType)
    {
      if (value == null)
        this.BindNullValue(parameterName, sqlType);
      else
        this.BindString(parameterName, value, length, false, sqlType);
    }

    protected void BindGuidPreserveNull(string parameterName, Guid value)
    {
      if (value == Guid.Empty)
        this.BindNullValue(parameterName, SqlDbType.UniqueIdentifier);
      else
        this.BindGuid(parameterName, value);
    }

    protected void BindNullableDateTime(string parameterName, DateTime value)
    {
      if (value > DateTime.MinValue)
      {
        if (value.Kind == DateTimeKind.Local)
          value = value.ToUniversalTime();
        this.BindDateTime(parameterName, value);
      }
      else
        this.BindNullValue(parameterName, SqlDbType.DateTime);
    }

    public virtual int DeleteMigratedTestExtensions(
      Guid projectId,
      int runId,
      int resultId,
      IEnumerable<int> fieldIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteMigratedTestExtensions");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@runId", runId);
      this.BindInt("@resultId", resultId);
      this.BindTestExtentionFieldIdArrayTableTable("@fieldIds", fieldIds);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("DeletedRowCount");
      return reader.Read() ? sqlColumnBinder.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException("prc_DeleteMigratedTestExtensions");
    }

    public virtual List<TestCaseResultIdentifier> QueryNewTestResults(
      int testRunId,
      Guid owner,
      byte testStatus,
      List<byte> outcomes,
      int afnStripId,
      Guid projectId,
      int newFieldId)
    {
      return new List<TestCaseResultIdentifier>();
    }

    public virtual int TcmAdhocCleanupJob(
      int batchSize,
      int minTestRunId,
      int maxTestRunId,
      int runLimit)
    {
      return 0;
    }

    public virtual Dictionary<int, List<TestCaseReference>> GetAutomatedTestCaseReferencesWithoutHash(
      int top)
    {
      return new Dictionary<int, List<TestCaseReference>>();
    }

    public virtual void PopulateAutomatedTestNameHash(
      int dataspaceId,
      List<TestCaseReference> testCaseReferences)
    {
    }

    public virtual List<TestCaseResult> GetPreMigrationAutomatedTestCaseReferencesWithoutHash(
      int top)
    {
      return new List<TestCaseResult>();
    }

    public virtual void PopulatePreMigrationAutomatedTestNameHash(
      List<TestCaseResult> automatedTestDetails)
    {
    }

    internal virtual IEnumerable<KeyValuePair<string, int>> GetTCMServiceMigrationThreshold(
      bool isTCMService = false)
    {
      string storedProcedure = "prc_GetTCMServiceMigrationThreshold";
      this.PrepareStoredProcedure(storedProcedure);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      TestManagementDatabase.MigrationThreshold migrationThreshold1 = new TestManagementDatabase.MigrationThreshold();
      List<KeyValuePair<string, int>> migrationThreshold2 = new List<KeyValuePair<string, int>>();
      migrationThreshold2.Add(new KeyValuePair<string, int>("TestRunThreshold", migrationThreshold1.TestRunThreshold.GetInt32((IDataReader) reader, 0) + 1000000));
      if (!reader.NextResult() || !reader.Read())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      migrationThreshold2.Add(new KeyValuePair<string, int>("TestAttachmentThreshold", migrationThreshold1.TestAttachmentThreshold.GetInt32((IDataReader) reader, 0) + 1000000));
      return (IEnumerable<KeyValuePair<string, int>>) migrationThreshold2;
    }

    public virtual void UpdateTCMServiceMigrationThreshold(
      int testRunIdThreshold,
      int attachmentIdThreshold)
    {
      this.PrepareStoredProcedure("prc_UpdateTCMServiceMigrationThreshold");
      this.BindInt("@testRunIdThreshold", testRunIdThreshold);
      this.BindInt("@attachmentIdThreshold", attachmentIdThreshold);
      this.ExecuteNonQuery();
    }

    internal virtual void SyncTestSettings(IEnumerable<TestSettings> testSettings, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_SyncTestSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestSettings_TestSettingsTypeTable("@testSettings", testSettings);
      this.ExecuteNonQuery();
    }

    internal virtual void SyncTestResolutionStates(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState> states,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_SyncTestResolutionStates");
      this.BindIdToTitleTypeTable("@resolutionStates", states.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState, KeyValuePair<int, string>>((System.Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState, KeyValuePair<int, string>>) (state => new KeyValuePair<int, string>(state.Id, state.Name))));
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.ExecuteNonQuery();
    }

    internal virtual void SyncTestFailureTypes(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType> failureTypes,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_SyncTestFailureTypes");
      this.BindIdToTitleTypeTable("@failureTypes", failureTypes.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType, KeyValuePair<int, string>>((System.Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType, KeyValuePair<int, string>>) (failureType => new KeyValuePair<int, string>(failureType.Id, failureType.Name))));
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.ExecuteNonQuery();
    }

    internal virtual TestConfiguration CreateTestConfiguration(
      TestConfiguration configuration,
      Guid updatedBy,
      Guid projectId)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateConfiguration");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindString("@name", configuration.Name, 256, false, SqlDbType.NVarChar);
        this.BindString("@description", configuration.Description, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindByte("@state", configuration.State);
        this.BindInt("@areaId", configuration.AreaId);
        this.BindBoolean("@isDefault", configuration.IsDefault);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindNameValuePairTypeTable("@valuesTable", (IEnumerable<NameValuePair>) configuration.Values);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new TestManagementDatabase.CreateTestConfigurationColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateConfiguration");
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual void DeleteTestConfiguration(
      Guid projectId,
      int configurationId,
      Guid auditIdentity)
    {
      this.PrepareStoredProcedure("TestManagement.prc_DeleteConfiguration");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@configurationId", configurationId);
      this.BindGuid("@auditIdentity", auditIdentity);
      this.ExecuteNonQuery();
    }

    internal virtual List<Tuple<TestConfiguration, string>> QueryTestConfigurationById(
      List<int> configurationIds,
      Guid projectId,
      bool returnVariables)
    {
      Dictionary<int, Tuple<TestConfiguration, string>> source = new Dictionary<int, Tuple<TestConfiguration, string>>();
      this.PrepareStoredProcedure("prc_QueryConfigurationById");
      this.BindNullableInt("@configurationId", configurationIds.FirstOrDefault<int>(), 0);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
      {
        string areaUri;
        TestConfiguration testConfiguration = new TestManagementDatabase.QueryTestConfigurationsColumns().bind(reader, out areaUri);
        source[testConfiguration.Id] = new Tuple<TestConfiguration, string>(testConfiguration, areaUri);
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestConfigurationById");
      if (source.Any<KeyValuePair<int, Tuple<TestConfiguration, string>>>())
      {
        TestManagementDatabase.QueryTestConfigurationsColumns2 configurationsColumns2 = new TestManagementDatabase.QueryTestConfigurationsColumns2();
        while (reader.Read())
        {
          int id;
          NameValuePair nameValuePair = configurationsColumns2.bind(reader, out id);
          source[id].Item1.Values.Add(nameValuePair);
        }
      }
      return source.Values.ToList<Tuple<TestConfiguration, string>>();
    }

    public virtual List<TestConfiguration> QueryTestConfigurations(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      int planId,
      out List<KeyValuePair<string, TestConfiguration>> areaUris)
    {
      this.PrepareDynamicProcedure("prc_QueryConfigurations");
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      this.BindInt("@planId", planId);
      areaUris = new List<KeyValuePair<string, TestConfiguration>>();
      return this.GetTestConfigurationsFromReader(this.ExecuteReader(), areaUris);
    }

    public virtual List<TestConfiguration> QueryTestConfigurationsWithPaging(
      Guid projectId,
      int skip,
      int top,
      int watermark,
      out List<KeyValuePair<string, TestConfiguration>> areaUris)
    {
      areaUris = new List<KeyValuePair<string, TestConfiguration>>();
      return new List<TestConfiguration>();
    }

    internal virtual UpdatedProperties UpdateTestConfiguration(
      Guid projectId,
      TestConfiguration configuration,
      Guid updatedBy,
      bool updateInUse,
      bool unchangedValues)
    {
      this.PrepareStoredProcedure("TestManagement.prc_UpdateConfiguration");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@configurationId", configuration.Id);
      this.BindString("@name", configuration.Name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", configuration.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindByte("@state", configuration.State);
      this.BindNullableInt("@areaId", configuration.AreaId, 0);
      this.BindBoolean("@isDefault", configuration.IsDefault);
      this.BindInt("@revision", configuration.Revision);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindBoolean("@updateInUse", updateInUse);
      this.BindBoolean("@unchangedValues", unchangedValues);
      this.BindNameValuePairTypeTable("@valuesTable", (IEnumerable<NameValuePair>) configuration.Values);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateConfiguration");
      updatedProperties.LastUpdatedBy = updatedBy;
      return updatedProperties;
    }

    public virtual List<TestConfiguration> QueryTestConfigurations2(
      Guid projectId,
      Dictionary<string, List<object>> parametersMap,
      int planId,
      out List<KeyValuePair<string, TestConfiguration>> areaUris)
    {
      areaUris = (List<KeyValuePair<string, TestConfiguration>>) null;
      return new List<TestConfiguration>();
    }

    protected List<TestConfiguration> GetTestConfigurationsFromReader(
      SqlDataReader reader,
      List<KeyValuePair<string, TestConfiguration>> areaUris)
    {
      TestManagementDatabase.QueryTestConfigurationsColumns configurationsColumns = new TestManagementDatabase.QueryTestConfigurationsColumns();
      List<TestConfiguration> configurationsFromReader = new List<TestConfiguration>();
      Dictionary<int, TestConfiguration> dictionary = new Dictionary<int, TestConfiguration>();
      while (reader.Read())
      {
        string areaUri;
        TestConfiguration testConfiguration = configurationsColumns.bind(reader, out areaUri);
        if (!string.IsNullOrEmpty(areaUri))
          areaUris.Add(new KeyValuePair<string, TestConfiguration>(areaUri, testConfiguration));
        configurationsFromReader.Add(testConfiguration);
        dictionary[testConfiguration.Id] = testConfiguration;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryConfigurations");
      TestManagementDatabase.QueryTestConfigurationsColumns2 configurationsColumns2 = new TestManagementDatabase.QueryTestConfigurationsColumns2();
      while (reader.Read())
      {
        int id;
        NameValuePair nameValuePair = configurationsColumns2.bind(reader, out id);
        TestConfiguration testConfiguration;
        if (dictionary.TryGetValue(id, out testConfiguration))
          testConfiguration.Values.Add(nameValuePair);
      }
      return configurationsFromReader;
    }

    internal virtual List<TestConfigurationRecord> QueryTestConfigurationsByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      toDate = fromDate;
      return new List<TestConfigurationRecord>();
    }

    internal virtual Dictionary<int, string> QueryShallowTestConfigurations(
      Guid projectId,
      List<int> configurationIds)
    {
      return new Dictionary<int, string>();
    }

    public virtual TestResultsDetailsGroupData GetAggregatedTestResultsForBuild4(
      Guid projectId,
      int buildId,
      string buildUri,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter,
      bool isAbortedRunEnabled,
      bool isInProgressRunsEnabled,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress,
      bool isDefaultFilterWithFilteredIndex,
      bool shouldFetchOldTestCaseRefId,
      int runIdThreshold = 0)
    {
      return new TestResultsDetailsGroupData();
    }

    public virtual TestResultsDetailsGroupData GetAggregatedTestResultsForRelease4(
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter,
      bool isAbortedRunEnabled,
      bool isInProgressRunsEnabled,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress,
      bool isDefaultFilterWithFilteredIndex,
      bool shouldFetchOldTestCaseRefId,
      int runIdThreshold = 0)
    {
      return new TestResultsDetailsGroupData();
    }

    public virtual TestResultsDetails GetTestResultsGroupDetails(
      Guid projectId,
      PipelineReference pipelineReference,
      IList<byte> runStates,
      bool shouldIncludeFailedAndAbortedResults,
      bool queryRunSummaryForInProgress)
    {
      return new TestResultsDetails();
    }

    public virtual TestResultHistory QueryTestCaseResultHistory(
      Guid projectId,
      ResultsFilter filter,
      bool isTfvcBranchFilteringEnabled)
    {
      return new TestResultHistory();
    }

    public virtual TestResultHistory QueryTestCaseResultHistory2(
      Guid projectId,
      ResultsFilter filter,
      int runIdThreshold = 0)
    {
      return new TestResultHistory();
    }

    public virtual TestHistoryContinuationTokenAndResults QueryTestHistory(
      Guid projectId,
      TestHistoryQuery filter,
      int continuationTokenMinRunId,
      int continuationTokenMaxRunId,
      int testResultBatchSizeLimit,
      int runIdThreshold = 0)
    {
      return new TestHistoryContinuationTokenAndResults();
    }

    public virtual void UpdateTestRunSummaryAndInsights(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      TestResultsContextType resultsContextType)
    {
    }

    public virtual void UpdateTestRunSummaryAndInsights2(
      GuidAndString projectId,
      BuildConfiguration buildToUpdate,
      BuildConfiguration previousBuild,
      ReleaseReference releaseToUpdate,
      ReleaseReference previousRelease)
    {
    }

    public virtual void UpdateTestRunSummaryForResults(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results)
    {
    }

    public virtual void UpdateFlakinessFieldForResults(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results)
    {
    }

    public virtual RunSummaryAndInsights QueryTestRunSummaryAndInsightsForBuild(
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration buildRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      out bool isBuildOld,
      int rundIdThreshold = 0)
    {
      runsCount = 0;
      isBuildOld = false;
      return new RunSummaryAndInsights();
    }

    public virtual RunSummaryAndInsights QueryTestRunSummaryAndInsightsForRelease(
      GuidAndString projectId,
      string sourceWorkflow,
      ReleaseReference releaseRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      int runIdThreshold = 0)
    {
      runsCount = 0;
      return new RunSummaryAndInsights();
    }

    public virtual Dictionary<ReleaseReference, RunSummaryAndInsights> QueryTestRunSummaryForReleases(
      GuidAndString projectId,
      List<ReleaseReference> releases,
      string categoryName,
      int runIdThreshold = 0)
    {
      return new Dictionary<ReleaseReference, RunSummaryAndInsights>();
    }

    public virtual void FetchTestFailureDetails(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      out List<TestCaseResult> currentFailedResults,
      out Dictionary<int, TestCaseResult> previousFailedResultsMap,
      out int prevTestRunContextId)
    {
      currentFailedResults = (List<TestCaseResult>) null;
      previousFailedResultsMap = (Dictionary<int, TestCaseResult>) null;
      prevTestRunContextId = 0;
    }

    public virtual void FetchTestFailureDetails(
      GuidAndString projectId,
      List<int> testRunIds,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      string sourceWorkflow,
      bool fetchPreviousFailedResults,
      bool shouldByPassFlaky,
      out Dictionary<int, List<TestCaseResult>> currentFailedResults,
      out Dictionary<int, TestCaseResult> previousFailedResultsMap,
      out int prevTestRunContextId)
    {
      currentFailedResults = (Dictionary<int, List<TestCaseResult>>) null;
      previousFailedResultsMap = (Dictionary<int, TestCaseResult>) null;
      prevTestRunContextId = 0;
    }

    public virtual void PublishTestSummaryAndFailureDetails(
      GuidAndString projectId,
      int testRunId,
      ResultInsights resultInsights,
      Dictionary<int, string> failingSinceDetails,
      bool includeFailureDetails)
    {
    }

    public virtual void PublishTestSummaryAndFailureDetails(
      GuidAndString projectId,
      List<int> testRunIds,
      Dictionary<int, ResultInsights> resultInsights,
      Dictionary<int, Dictionary<int, string>> failingSinceDetails,
      List<TestResultIdentifierRecord> flakyResults,
      bool includeFailureDetails,
      bool publishPassCountOnly = false,
      bool shouldPublishFlakiness = false,
      bool shouldByPassFlaky = false)
    {
    }

    public virtual List<TestResultIdentifierRecord> QueryFlakyTestResults(
      Guid projectId,
      List<int> testRunIds)
    {
      return new List<TestResultIdentifierRecord>();
    }

    public virtual void UpdateTestRunSummaryForNonConfigRuns(
      Guid projectId,
      int testRunId,
      IEnumerable<RunSummaryByOutcome> runSummaryByOutcomes)
    {
    }

    public virtual RunSummaryAndInsights QueryTestRunSummaryAndInsightsForPipeline(
      GuidAndString projectId,
      PipelineReference pipelineReference,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount)
    {
      runsCount = 0;
      return new RunSummaryAndInsights();
    }

    public virtual RunSummaryAndResultInsightsInPipeline GetPipelineTestMetrics(
      GuidAndString projectId,
      PipelineReference pipelineReference,
      bool resultsSummaryFlag,
      bool resultsAnalysisFlag,
      bool runSummaryFlag,
      bool groupByNode = false)
    {
      return new RunSummaryAndResultInsightsInPipeline();
    }

    public virtual List<TestCaseResult> GetTestCaseResultsByIds(
      Guid projectId,
      List<TestCaseResultIdentifier> resultIds,
      List<string> fields)
    {
      return new List<TestCaseResult>();
    }

    public virtual List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      return new List<AggregatedDataForResultTrend>();
    }

    public virtual List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      return new List<AggregatedDataForResultTrend>();
    }

    public virtual List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild2(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      return new List<AggregatedDataForResultTrend>();
    }

    public virtual List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease2(
      Guid projectId,
      TestResultTrendFilter filter)
    {
      return new List<AggregatedDataForResultTrend>();
    }

    public virtual List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      return new List<AggregatedDataForResultTrend>();
    }

    public virtual List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      return new List<AggregatedDataForResultTrend>();
    }

    public virtual List<TestSettings> GetTestSettings(
      Guid projectId,
      int top,
      int continuationTokenId)
    {
      throw new NotImplementedException();
    }

    public virtual List<TestSettings> QueryTestSettings2(
      Guid projectId,
      Dictionary<string, List<object>> parametersMap,
      bool omitSettings,
      out List<KeyValuePair<string, TestSettings>> areaUris)
    {
      areaUris = (List<KeyValuePair<string, TestSettings>>) null;
      return new List<TestSettings>();
    }

    public virtual UpdatedProperties CreateIfNotExistsTestSettings(
      TestManagementRequestContext context,
      Guid projectId,
      TestSettings settings,
      Guid updatedBy)
    {
      throw new NotImplementedException();
    }

    public virtual UpdatedProperties CreateTestSettings(
      TestManagementRequestContext context,
      Guid projectId,
      TestSettings settings,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_CreateTestSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@name", settings.Name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", settings.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindXml("@settings", settings.Settings);
      this.BindXml("@machineRoles", TestSettingsMachineRole.ToXml(settings.MachineRoles));
      this.BindInt("@areaId", settings.AreaId);
      this.BindBoolean("@isPublic", settings.IsPublic);
      this.BindBoolean("@isAutomated", settings.IsAutomated);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedProperties testSettings = reader.Read() ? new TestManagementDatabase.CreateTestSettingsColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateTestSettings");
      testSettings.LastUpdatedBy = updatedBy;
      return testSettings;
    }

    public virtual UpdatedProperties UpdateTestSettings(
      Guid projectId,
      TestSettings settings,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_UpdateTestSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@settingsId", settings.Id);
      this.BindStringPreserveNull("@name", settings.Name, 256, SqlDbType.NVarChar);
      this.BindStringPreserveNull("@description", settings.Description, int.MaxValue, SqlDbType.NVarChar);
      this.BindXml("@settings", settings.Settings);
      this.BindXml("@machineRoles", TestSettingsMachineRole.ToXml(settings.MachineRoles));
      this.BindNullableInt("@areaId", settings.AreaId, 0);
      this.BindBoolean("@isAutomated", settings.IsAutomated);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindInt("@revision", settings.Revision);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestSettings");
      updatedProperties.LastUpdatedBy = updatedBy;
      return updatedProperties;
    }

    public virtual void DeleteTestSettings(Guid projectId, int settingsId, Guid auditIdentity)
    {
      this.PrepareStoredProcedure("prc_DeleteTestSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@settingsId", settingsId);
      this.BindGuid("@auditIdentity", auditIdentity);
      this.ExecuteNonQuery();
    }

    public virtual List<TestSettings> QueryTestSettings(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      bool omitSettings,
      out List<KeyValuePair<string, TestSettings>> areaUris)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestSettings");
        List<TestSettings> testSettingsList = new List<TestSettings>();
        this.PrepareDynamicProcedure("prc_QueryTestSettings");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        this.BindBoolean("@omitSettings", omitSettings);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.QueryTestSettingsColumns testSettingsColumns = new TestManagementDatabase.QueryTestSettingsColumns();
        areaUris = new List<KeyValuePair<string, TestSettings>>();
        while (reader.Read())
        {
          string areaUri;
          TestSettings testSettings = testSettingsColumns.Bind(this.RequestContext, reader, out areaUri);
          if (!string.IsNullOrEmpty(areaUri))
            areaUris.Add(new KeyValuePair<string, TestSettings>(areaUri, testSettings));
          testSettingsList.Add(testSettings);
        }
        return testSettingsList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestSettings");
      }
    }

    public virtual TestSettings QueryTestSettingsById(
      Guid projectId,
      int settingsId,
      out string areaUri)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestSettingsById");
        TestSettings testSettings = (TestSettings) null;
        areaUri = (string) null;
        this.PrepareStoredProcedure("prc_QueryTestSettingsById");
        this.BindNullableInt("@settingsId", settingsId, 0);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.QueryTestSettingsColumns testSettingsColumns = new TestManagementDatabase.QueryTestSettingsColumns();
        if (reader.Read())
          testSettings = testSettingsColumns.Bind(this.RequestContext, reader, out areaUri);
        return testSettings;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestSettingsById");
      }
    }

    protected class QueryDefaultStripsColumns
    {
      private SqlColumnBinder AttachmentId = new SqlColumnBinder(nameof (AttachmentId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPath = new SqlColumnBinder(nameof (ActionPath));
      private SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder UncompressedLength = new SqlColumnBinder(nameof (UncompressedLength));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder AttachmentType = new SqlColumnBinder(nameof (AttachmentType));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder IsComplete = new SqlColumnBinder(nameof (IsComplete));
      private SqlColumnBinder AreaUri = new SqlColumnBinder(nameof (AreaUri));
      private SqlColumnBinder FileId = new SqlColumnBinder("TfsFileId");

      internal TestResultAttachment Bind(SqlDataReader reader, out string areaUri)
      {
        TestResultAttachment resultAttachment = new TestResultAttachment();
        resultAttachment.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        resultAttachment.TestResultId = this.TestResultId.GetInt32((IDataReader) reader);
        resultAttachment.Id = this.AttachmentId.GetInt32((IDataReader) reader);
        resultAttachment.IterationId = this.IterationId.GetInt32((IDataReader) reader);
        resultAttachment.ActionPath = this.ActionPath.GetString((IDataReader) reader, true);
        resultAttachment.FileName = this.FileName.GetString((IDataReader) reader, false);
        resultAttachment.Comment = this.Comment.GetString((IDataReader) reader, true);
        resultAttachment.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        resultAttachment.AttachmentType = this.AttachmentType.GetString((IDataReader) reader, false);
        areaUri = this.AreaUri.GetString((IDataReader) reader, true);
        resultAttachment.Length = this.UncompressedLength.GetInt64((IDataReader) reader);
        resultAttachment.IsComplete = this.IsComplete.GetBoolean((IDataReader) reader);
        resultAttachment.FileId = this.FileId.GetInt32((IDataReader) reader);
        return resultAttachment;
      }
    }

    protected class CreateAttachmentColumns
    {
      internal SqlColumnBinder AttachmentId = new SqlColumnBinder(nameof (AttachmentId));
    }

    protected class AppendAttachmentColumns
    {
      internal SqlColumnBinder CoverageChanged = new SqlColumnBinder(nameof (CoverageChanged));
    }

    protected class QueryAttachmentsColumns
    {
      private SqlColumnBinder AttachmentId = new SqlColumnBinder(nameof (AttachmentId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPath = new SqlColumnBinder(nameof (ActionPath));
      private SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      private SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder UncompressedLength = new SqlColumnBinder(nameof (UncompressedLength));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder AttachmentType = new SqlColumnBinder(nameof (AttachmentType));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder IsComplete = new SqlColumnBinder(nameof (IsComplete));
      private SqlColumnBinder TmiRunId = new SqlColumnBinder(nameof (TmiRunId));
      private SqlColumnBinder FileId = new SqlColumnBinder("TfsFileId");
      private SqlColumnBinder SubResultId = new SqlColumnBinder(nameof (SubResultId));

      internal TestResultAttachment Bind(SqlDataReader reader) => new TestResultAttachment()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        Id = this.AttachmentId.GetInt32((IDataReader) reader),
        IterationId = this.IterationId.GetInt32((IDataReader) reader),
        ActionPath = this.ActionPath.GetString((IDataReader) reader, true),
        SessionId = this.SessionId.ColumnExists((IDataReader) reader) ? this.SessionId.GetInt32((IDataReader) reader, 0) : 0,
        FileName = this.FileName.GetString((IDataReader) reader, false),
        Comment = this.Comment.GetString((IDataReader) reader, true),
        Length = this.UncompressedLength.GetInt64((IDataReader) reader),
        IsComplete = this.IsComplete.GetBoolean((IDataReader) reader),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        AttachmentType = this.AttachmentType.GetString((IDataReader) reader, false),
        TmiRunId = this.TmiRunId.GetGuid((IDataReader) reader, true),
        FileId = this.FileId.GetInt32((IDataReader) reader),
        SubResultId = this.SubResultId.ColumnExists((IDataReader) reader) ? this.SubResultId.GetInt32((IDataReader) reader, 0) : 0
      };
    }

    protected class QueryAttachmentsColumns2
    {
      private SqlColumnBinder AttachmentId = new SqlColumnBinder(nameof (AttachmentId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPath = new SqlColumnBinder(nameof (ActionPath));
      private SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder UncompressedLength = new SqlColumnBinder(nameof (UncompressedLength));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder AttachmentType = new SqlColumnBinder(nameof (AttachmentType));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      private SqlColumnBinder IsComplete = new SqlColumnBinder(nameof (IsComplete));
      private SqlColumnBinder TmiRunId = new SqlColumnBinder(nameof (TmiRunId));
      private SqlColumnBinder AreaUri = new SqlColumnBinder(nameof (AreaUri));
      private SqlColumnBinder FileId = new SqlColumnBinder("TfsFileId");

      internal TestResultAttachment Bind(SqlDataReader reader) => new TestResultAttachment()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        Id = this.AttachmentId.GetInt32((IDataReader) reader),
        IterationId = this.IterationId.GetInt32((IDataReader) reader),
        ActionPath = this.ActionPath.GetString((IDataReader) reader, true),
        SessionId = this.SessionId.ColumnExists((IDataReader) reader) ? this.SessionId.GetInt32((IDataReader) reader) : 0,
        FileName = this.FileName.GetString((IDataReader) reader, false),
        Comment = this.Comment.GetString((IDataReader) reader, true),
        Length = this.UncompressedLength.GetInt64((IDataReader) reader),
        IsComplete = this.IsComplete.GetBoolean((IDataReader) reader),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        AttachmentType = this.AttachmentType.GetString((IDataReader) reader, false),
        TmiRunId = this.TmiRunId.GetGuid((IDataReader) reader, true),
        AreaUri = this.AreaUri.GetString((IDataReader) reader, true),
        FileId = this.FileId.GetInt32((IDataReader) reader)
      };
    }

    protected class QueryAttachmentIdMappingColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder SubResultId = new SqlColumnBinder(nameof (SubResultId));
      private SqlColumnBinder AttachmentId = new SqlColumnBinder("Id");
      private SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      private SqlColumnBinder AttachmentType = new SqlColumnBinder(nameof (AttachmentType));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder IsComplete = new SqlColumnBinder(nameof (IsComplete));
      private SqlColumnBinder UncompressedLength = new SqlColumnBinder(nameof (UncompressedLength));

      internal TestResultAttachment Bind(SqlDataReader reader) => new TestResultAttachment()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        SubResultId = this.SubResultId.GetInt32((IDataReader) reader),
        Id = this.AttachmentId.GetInt32((IDataReader) reader),
        FileName = this.FileName.GetString((IDataReader) reader, false),
        AttachmentType = this.AttachmentType.GetString((IDataReader) reader, false),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        IsComplete = this.IsComplete.GetBoolean((IDataReader) reader),
        Length = this.UncompressedLength.GetInt64((IDataReader) reader)
      };
    }

    private class QueryAttachmentOwnerIdColumns
    {
      internal SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      internal SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      internal SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
    }

    protected class QueryAttachmentContentColumns
    {
      internal SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      internal SqlColumnBinder TfsFileId = new SqlColumnBinder(nameof (TfsFileId));
    }

    protected class GetAttachmentTfsFileIdColumns
    {
      internal SqlColumnBinder TfsFileId = new SqlColumnBinder(nameof (TfsFileId));
    }

    private class QueryBuildConfigurationsColumns
    {
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));

      internal BuildConfiguration bind(SqlDataReader reader, out int dataspaceId)
      {
        BuildConfiguration buildConfiguration = new BuildConfiguration();
        buildConfiguration.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        buildConfiguration.BuildUri = this.BuildUri.GetString((IDataReader) reader, false);
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        buildConfiguration.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, false);
        buildConfiguration.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, false);
        return buildConfiguration;
      }
    }

    protected class CodeCoverageSummaryColumns
    {
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder Label = new SqlColumnBinder(nameof (Label));
      private SqlColumnBinder Position = new SqlColumnBinder(nameof (Position));
      private SqlColumnBinder Total = new SqlColumnBinder(nameof (Total));
      private SqlColumnBinder Covered = new SqlColumnBinder(nameof (Covered));
      private SqlColumnBinder IsDeltaAvailable = new SqlColumnBinder(nameof (IsDeltaAvailable));
      private SqlColumnBinder Delta = new SqlColumnBinder(nameof (Delta));

      internal CodeCoverageStatistics Bind(
        SqlDataReader reader,
        out string buildPlatform,
        out string buildFlavor)
      {
        buildPlatform = (string) null;
        buildFlavor = (string) null;
        if (!reader.IsDBNull(0))
          buildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true);
        if (!reader.IsDBNull(1))
          buildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true);
        string str = this.Label.GetString((IDataReader) reader, true);
        int int32_1 = this.Position.GetInt32((IDataReader) reader);
        int int32_2 = this.Total.GetInt32((IDataReader) reader);
        int int32_3 = this.Covered.GetInt32((IDataReader) reader);
        bool boolean = this.IsDeltaAvailable.GetBoolean((IDataReader) reader);
        double num = Math.Round(this.Delta.GetDouble((IDataReader) reader), 2);
        return new CodeCoverageStatistics()
        {
          Label = str,
          Position = int32_1,
          Total = int32_2,
          Covered = int32_3,
          IsDeltaAvailable = boolean,
          Delta = num
        };
      }
    }

    private class CoverageChangeColumns
    {
      private SqlColumnBinder ChangeId = new SqlColumnBinder(nameof (ChangeId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder ChangeType = new SqlColumnBinder(nameof (ChangeType));
      private SqlColumnBinder DateCreated = new SqlColumnBinder(nameof (DateCreated));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));

      internal CoverageChange Bind(SqlDataReader reader, out int dataspaceId)
      {
        BuildConfiguration buildConfiguration = new BuildConfiguration();
        buildConfiguration.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader, 0);
        buildConfiguration.BuildUri = this.BuildUri.GetString((IDataReader) reader, true);
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        buildConfiguration.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true);
        buildConfiguration.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true);
        if (this.BuildDefinitionId.ColumnExists((IDataReader) reader))
          buildConfiguration.BuildDefinitionId = this.BuildDefinitionId.GetInt32((IDataReader) reader, 0);
        if (this.BuildId.ColumnExists((IDataReader) reader))
          buildConfiguration.BuildId = this.BuildId.GetInt32((IDataReader) reader, 0);
        return new CoverageChange()
        {
          CoverageChangeId = this.ChangeId.GetInt32((IDataReader) reader),
          TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
          TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
          ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader, 0),
          BuildConfiguration = buildConfiguration,
          ChangeType = (CoverageChangeType) this.ChangeType.GetByte((IDataReader) reader),
          SessionId = this.SessionId.GetInt32((IDataReader) reader)
        };
      }
    }

    private class BuildCoverageColumns
    {
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastError = new SqlColumnBinder(nameof (LastError));
      private SqlColumnBinder DateCreated = new SqlColumnBinder(nameof (DateCreated));
      private SqlColumnBinder DateModified = new SqlColumnBinder(nameof (DateModified));

      internal BuildCoverage Bind(SqlDataReader reader)
      {
        BuildCoverage buildCoverage = new BuildCoverage()
        {
          Configuration = new BuildConfiguration()
        };
        buildCoverage.Configuration.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        buildCoverage.Configuration.BuildUri = this.BuildUri.GetString((IDataReader) reader, false);
        buildCoverage.Configuration.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, false);
        buildCoverage.Configuration.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, false);
        buildCoverage.Id = this.CoverageId.GetInt32((IDataReader) reader);
        buildCoverage.State = this.State.GetByte((IDataReader) reader);
        buildCoverage.LastError = this.LastError.GetString((IDataReader) reader, true);
        return buildCoverage;
      }
    }

    protected class TestRunCoverageColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastError = new SqlColumnBinder(nameof (LastError));
      private SqlColumnBinder DateCreated = new SqlColumnBinder(nameof (DateCreated));
      private SqlColumnBinder DateModified = new SqlColumnBinder(nameof (DateModified));

      internal TestRunCoverage Bind(SqlDataReader reader)
      {
        TestRunCoverage testRunCoverage = new TestRunCoverage();
        testRunCoverage.Id = this.CoverageId.GetInt32((IDataReader) reader);
        testRunCoverage.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRunCoverage.State = this.State.GetByte((IDataReader) reader);
        testRunCoverage.LastError = this.LastError.GetString((IDataReader) reader, false);
        return testRunCoverage;
      }
    }

    private class ModuleCoverageColumns
    {
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder ModuleId = new SqlColumnBinder(nameof (ModuleId));
      private SqlColumnBinder Signature = new SqlColumnBinder(nameof (Signature));
      private SqlColumnBinder SignatureAge = new SqlColumnBinder(nameof (SignatureAge));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder LinesCovered = new SqlColumnBinder(nameof (LinesCovered));
      private SqlColumnBinder LinesPartiallyCovered = new SqlColumnBinder(nameof (LinesPartiallyCovered));
      private SqlColumnBinder LinesNotCovered = new SqlColumnBinder(nameof (LinesNotCovered));
      private SqlColumnBinder BlocksCovered = new SqlColumnBinder(nameof (BlocksCovered));
      private SqlColumnBinder BlocksNotCovered = new SqlColumnBinder(nameof (BlocksNotCovered));
      private SqlColumnBinder BlockCount = new SqlColumnBinder(nameof (BlockCount));
      private SqlColumnBinder BlockDataLength = new SqlColumnBinder(nameof (BlockDataLength));
      private SqlColumnBinder BlockData = new SqlColumnBinder(nameof (BlockData));

      internal ModuleCoverage Bind(SqlDataReader reader, Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
      {
        ModuleCoverage moduleCoverage = new ModuleCoverage()
        {
          CoverageId = this.CoverageId.GetInt32((IDataReader) reader),
          ModuleId = this.ModuleId.GetInt32((IDataReader) reader),
          Name = this.Name.GetString((IDataReader) reader, false),
          Signature = this.Signature.GetGuid((IDataReader) reader, true),
          SignatureAge = this.SignatureAge.GetInt32((IDataReader) reader, 0),
          Statistics = new CoverageStatistics()
        };
        moduleCoverage.Statistics.LinesCovered = this.LinesCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.LinesPartiallyCovered = this.LinesPartiallyCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.LinesNotCovered = this.LinesNotCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.BlocksCovered = this.BlocksCovered.GetInt32((IDataReader) reader, 0, 0);
        moduleCoverage.Statistics.BlocksNotCovered = this.BlocksNotCovered.GetInt32((IDataReader) reader, 0, 0);
        if ((flags & Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.BlockData) != (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) 0)
        {
          moduleCoverage.BlockCount = this.BlockCount.GetInt32((IDataReader) reader, 0);
          int int32 = this.BlockDataLength.GetInt32((IDataReader) reader, 0);
          byte[] bytes = this.BlockData.GetBytes((IDataReader) reader, true);
          if (int32 != -1 && int32 != bytes.Length)
            Array.Resize<byte>(ref bytes, int32);
          moduleCoverage.BlockData = bytes;
        }
        return moduleCoverage;
      }
    }

    public class FunctionCoverageColumns
    {
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder ModuleId = new SqlColumnBinder(nameof (ModuleId));
      private SqlColumnBinder FunctionId = new SqlColumnBinder(nameof (FunctionId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Namespace = new SqlColumnBinder(nameof (Namespace));
      private SqlColumnBinder Class = new SqlColumnBinder(nameof (Class));
      private SqlColumnBinder SourceFile = new SqlColumnBinder(nameof (SourceFile));
      private SqlColumnBinder LinesCovered = new SqlColumnBinder(nameof (LinesCovered));
      private SqlColumnBinder LinesPartiallyCovered = new SqlColumnBinder(nameof (LinesPartiallyCovered));
      private SqlColumnBinder LinesNotCovered = new SqlColumnBinder(nameof (LinesNotCovered));
      private SqlColumnBinder BlocksCovered = new SqlColumnBinder(nameof (BlocksCovered));
      private SqlColumnBinder BlocksNotCovered = new SqlColumnBinder(nameof (BlocksNotCovered));

      internal FunctionCoverage Bind(SqlDataReader reader)
      {
        FunctionCoverage functionCoverage = new FunctionCoverage()
        {
          CoverageId = this.CoverageId.GetInt32((IDataReader) reader),
          ModuleId = this.ModuleId.GetInt32((IDataReader) reader),
          FunctionId = this.FunctionId.GetInt32((IDataReader) reader),
          Name = this.Name.GetString((IDataReader) reader, false),
          SourceFile = this.SourceFile.GetString((IDataReader) reader, true),
          Class = this.Class.GetString((IDataReader) reader, true),
          Namespace = this.Namespace.GetString((IDataReader) reader, true),
          Statistics = new CoverageStatistics()
        };
        functionCoverage.Statistics.LinesCovered = this.LinesCovered.GetInt32((IDataReader) reader, 0, 0);
        functionCoverage.Statistics.LinesPartiallyCovered = this.LinesPartiallyCovered.GetInt32((IDataReader) reader, 0, 0);
        functionCoverage.Statistics.LinesNotCovered = this.LinesNotCovered.GetInt32((IDataReader) reader, 0, 0);
        functionCoverage.Statistics.BlocksCovered = this.BlocksCovered.GetInt32((IDataReader) reader, 0, 0);
        functionCoverage.Statistics.BlocksNotCovered = this.BlocksNotCovered.GetInt32((IDataReader) reader, 0, 0);
        return functionCoverage;
      }
    }

    private class TestExtensionFieldsColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder FieldName = new SqlColumnBinder(nameof (FieldName));
      private SqlColumnBinder FieldType = new SqlColumnBinder(nameof (FieldType));
      private SqlColumnBinder IsRunScoped = new SqlColumnBinder(nameof (IsRunScoped));
      private SqlColumnBinder IsResultScoped = new SqlColumnBinder(nameof (IsResultScoped));
      private SqlColumnBinder IsSystemField = new SqlColumnBinder(nameof (IsSystemField));

      internal TestExtensionFieldDetails bind(SqlDataReader reader) => new TestExtensionFieldDetails()
      {
        Id = this.FieldId.GetInt32((IDataReader) reader),
        Name = this.FieldName.GetString((IDataReader) reader, false),
        Type = (SqlDbType) this.FieldType.GetByte((IDataReader) reader),
        IsRunScoped = this.IsRunScoped.GetBoolean((IDataReader) reader, false),
        IsResultScoped = this.IsResultScoped.GetBoolean((IDataReader) reader, false),
        IsSystemField = this.IsSystemField.GetBoolean((IDataReader) reader, false)
      };
    }

    protected class QueryLogStoreAttachmentsColumns
    {
      private SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder AttachmentType = new SqlColumnBinder(nameof (AttachmentType));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder SubResultId = new SqlColumnBinder(nameof (SubResultId));
      private SqlColumnBinder FileId = new SqlColumnBinder(nameof (FileId));

      internal TestResultAttachment Bind(SqlDataReader reader) => new TestResultAttachment()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        FileName = this.FileName.GetString((IDataReader) reader, false),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        AttachmentType = this.AttachmentType.GetString((IDataReader) reader, false),
        SubResultId = this.SubResultId.ColumnExists((IDataReader) reader) ? this.SubResultId.GetInt32((IDataReader) reader, 0) : 0,
        FileId = this.FileId.GetInt32((IDataReader) reader, 0, 0)
      };
    }

    protected class QueryLogStoreAttachmentsColumnsV2
    {
      private SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder AttachmentType = new SqlColumnBinder(nameof (AttachmentType));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder SubResultId = new SqlColumnBinder(nameof (SubResultId));
      private SqlColumnBinder FileId = new SqlColumnBinder(nameof (FileId));
      private SqlColumnBinder AttachmentId = new SqlColumnBinder(nameof (AttachmentId));
      private SqlColumnBinder UncompressedLength = new SqlColumnBinder(nameof (UncompressedLength));

      internal TestResultAttachment Bind(SqlDataReader reader) => new TestResultAttachment()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        FileName = this.FileName.GetString((IDataReader) reader, false),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        AttachmentType = this.AttachmentType.GetString((IDataReader) reader, false),
        SubResultId = this.SubResultId.ColumnExists((IDataReader) reader) ? this.SubResultId.GetInt32((IDataReader) reader, 0) : 0,
        FileId = this.FileId.GetInt32((IDataReader) reader, 0, 0),
        Id = this.AttachmentId.GetInt32((IDataReader) reader),
        Length = this.UncompressedLength.GetInt64((IDataReader) reader)
      };
    }

    protected class QueryLogStoreAttachmentsColumnsV3
    {
      private SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder AttachmentType = new SqlColumnBinder(nameof (AttachmentType));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder SubResultId = new SqlColumnBinder(nameof (SubResultId));
      private SqlColumnBinder FileId = new SqlColumnBinder("TfsFileId");
      private SqlColumnBinder AttachmentId = new SqlColumnBinder(nameof (AttachmentId));
      private SqlColumnBinder UncompressedLength = new SqlColumnBinder(nameof (UncompressedLength));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPath = new SqlColumnBinder(nameof (ActionPath));

      internal TestResultAttachment Bind(SqlDataReader reader) => new TestResultAttachment()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        FileName = this.FileName.GetString((IDataReader) reader, false),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        AttachmentType = this.AttachmentType.GetString((IDataReader) reader, false),
        SubResultId = this.SubResultId.ColumnExists((IDataReader) reader) ? this.SubResultId.GetInt32((IDataReader) reader, 0) : 0,
        FileId = this.FileId.GetInt32((IDataReader) reader, 0, 0),
        Id = this.AttachmentId.GetInt32((IDataReader) reader),
        Length = this.UncompressedLength.GetInt64((IDataReader) reader),
        IterationId = this.IterationId.GetInt32((IDataReader) reader),
        ActionPath = this.ActionPath.GetString((IDataReader) reader, true)
      };
    }

    private class ReleaseRefsColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder ReleaseRefId = new SqlColumnBinder(nameof (ReleaseRefId));
      private SqlColumnBinder ReleaseUri = new SqlColumnBinder(nameof (ReleaseUri));
      private SqlColumnBinder ReleaseEnvUri = new SqlColumnBinder(nameof (ReleaseEnvUri));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseDefId = new SqlColumnBinder(nameof (ReleaseDefId));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));
      private SqlColumnBinder Attempt = new SqlColumnBinder(nameof (Attempt));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder ReleaseCreationDate = new SqlColumnBinder(nameof (ReleaseCreationDate));
      private SqlColumnBinder EnvironmentCreationDate = new SqlColumnBinder(nameof (EnvironmentCreationDate));

      internal ReleaseReference2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        ReleaseReference2 releaseReference2 = new ReleaseReference2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        releaseReference2.ProjectId = GetDataspaceIdentifier(int32);
        releaseReference2.ReleaseRefId = this.ReleaseRefId.GetInt32((IDataReader) reader);
        releaseReference2.ReleaseUri = this.ReleaseUri.GetString((IDataReader) reader, true);
        releaseReference2.ReleaseEnvUri = this.ReleaseEnvUri.GetString((IDataReader) reader, true);
        releaseReference2.ReleaseId = this.ReleaseId.GetInt32((IDataReader) reader);
        releaseReference2.ReleaseEnvId = this.ReleaseEnvId.GetInt32((IDataReader) reader);
        releaseReference2.ReleaseDefId = this.ReleaseDefId.GetInt32((IDataReader) reader);
        releaseReference2.ReleaseEnvDefId = this.ReleaseEnvDefId.GetInt32((IDataReader) reader);
        releaseReference2.Attempt = this.Attempt.GetInt32((IDataReader) reader);
        releaseReference2.ReleaseName = this.ReleaseName.GetString((IDataReader) reader, true);
        releaseReference2.ReleaseCreationDate = this.ReleaseCreationDate.GetNullableDateTime((IDataReader) reader);
        releaseReference2.EnvironmentCreationDate = this.EnvironmentCreationDate.GetNullableDateTime((IDataReader) reader);
        return releaseReference2;
      }
    }

    private class BuildRefsColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder SourceVersion = new SqlColumnBinder(nameof (SourceVersion));
      private SqlColumnBinder BuildSystem = new SqlColumnBinder(nameof (BuildSystem));
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder BuildDeleted = new SqlColumnBinder(nameof (BuildDeleted));
      private SqlColumnBinder RepoId = new SqlColumnBinder(nameof (RepoId));
      private SqlColumnBinder RepoType = new SqlColumnBinder(nameof (RepoType));

      internal BuildReference2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        BuildReference2 buildReference2 = new BuildReference2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        buildReference2.ProjectId = GetDataspaceIdentifier(int32);
        buildReference2.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        buildReference2.BuildUri = this.BuildUri.GetString((IDataReader) reader, true);
        buildReference2.BuildId = this.BuildId.GetInt32((IDataReader) reader);
        buildReference2.BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
        buildReference2.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true);
        buildReference2.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true);
        buildReference2.BuildDefinitionId = this.BuildDefinitionId.GetInt32((IDataReader) reader);
        buildReference2.CreatedDate = this.CreatedDate.GetNullableDateTime((IDataReader) reader);
        buildReference2.BranchName = this.BranchName.GetString((IDataReader) reader, true);
        buildReference2.SourceVersion = this.SourceVersion.GetString((IDataReader) reader, true);
        buildReference2.BuildSystem = this.BuildSystem.GetString((IDataReader) reader, true);
        buildReference2.CoverageId = this.CoverageId.GetNullableInt32((IDataReader) reader);
        buildReference2.BuildDeleted = this.BuildDeleted.GetBoolean((IDataReader) reader, false);
        buildReference2.RepoId = this.RepoId.GetString((IDataReader) reader, true);
        buildReference2.RepoType = this.RepoType.GetString((IDataReader) reader, true);
        return buildReference2;
      }
    }

    private class TestRunContextColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder BuildRefId = new SqlColumnBinder(nameof (BuildRefId));
      private SqlColumnBinder ReleaseRefId = new SqlColumnBinder(nameof (ReleaseRefId));
      private SqlColumnBinder SourceWorkflow = new SqlColumnBinder(nameof (SourceWorkflow));

      internal TestRunContext2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestRunContext2 testRunContext2 = new TestRunContext2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testRunContext2.ProjectId = GetDataspaceIdentifier(int32);
        testRunContext2.TestRunContextId = this.TestRunContextId.GetInt32((IDataReader) reader);
        testRunContext2.BuildRefId = this.BuildRefId.GetInt32((IDataReader) reader, 0);
        testRunContext2.ReleaseRefId = this.ReleaseRefId.GetInt32((IDataReader) reader, 0);
        testRunContext2.SourceWorkflow = this.SourceWorkflow.GetString((IDataReader) reader, true);
        return testRunContext2;
      }
    }

    private class TestMessageLogColumns
    {
      private SqlColumnBinder TestMessageLogId = new SqlColumnBinder(nameof (TestMessageLogId));

      internal TestMessageLog2 bind(SqlDataReader reader) => new TestMessageLog2()
      {
        TestMessageLogId = this.TestMessageLogId.GetInt32((IDataReader) reader)
      };
    }

    private class TestCaseReferenceColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder TestPointId = new SqlColumnBinder(nameof (TestPointId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder AutomatedTestType = new SqlColumnBinder(nameof (AutomatedTestType));
      private SqlColumnBinder AutomatedTestId = new SqlColumnBinder(nameof (AutomatedTestId));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private SqlColumnBinder TestCaseRevision = new SqlColumnBinder(nameof (TestCaseRevision));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder AreaId = new SqlColumnBinder(nameof (AreaId));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder CreatedBy = new SqlColumnBinder(nameof (CreatedBy));
      private SqlColumnBinder LastRefTestRunDate = new SqlColumnBinder(nameof (LastRefTestRunDate));
      private SqlColumnBinder AutomatedTestNameHash = new SqlColumnBinder(nameof (AutomatedTestNameHash));
      private SqlColumnBinder AutomatedTestStorageHash = new SqlColumnBinder(nameof (AutomatedTestStorageHash));

      internal TestCaseReference2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestCaseReference2 testCaseReference2 = new TestCaseReference2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testCaseReference2.ProjectId = GetDataspaceIdentifier(int32);
        testCaseReference2.TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader);
        testCaseReference2.TestPointId = this.TestPointId.GetInt32((IDataReader) reader);
        testCaseReference2.ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader);
        testCaseReference2.TestCaseRefId = this.TestCaseRefId.GetInt32((IDataReader) reader);
        testCaseReference2.AutomatedTestName = this.AutomatedTestName.GetString((IDataReader) reader, true);
        testCaseReference2.AutomatedTestStorage = this.AutomatedTestStorage.GetString((IDataReader) reader, true);
        testCaseReference2.AutomatedTestType = this.AutomatedTestType.GetString((IDataReader) reader, true);
        testCaseReference2.AutomatedTestId = this.AutomatedTestId.GetString((IDataReader) reader, true);
        testCaseReference2.TestCaseTitle = this.TestCaseTitle.GetString((IDataReader) reader, true);
        testCaseReference2.TestCaseRevision = this.TestCaseRevision.GetInt32((IDataReader) reader);
        testCaseReference2.Priority = this.Priority.GetByte((IDataReader) reader);
        testCaseReference2.Owner = this.Owner.GetString((IDataReader) reader, true);
        testCaseReference2.AreaId = this.AreaId.GetInt32((IDataReader) reader);
        testCaseReference2.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testCaseReference2.CreatedBy = this.CreatedBy.GetGuid((IDataReader) reader);
        testCaseReference2.LastRefTestRunDate = this.LastRefTestRunDate.GetDateTime((IDataReader) reader);
        testCaseReference2.AutomatedTestNameHash = this.AutomatedTestNameHash.GetBytes((IDataReader) reader, true);
        testCaseReference2.AutomatedTestStorageHash = this.AutomatedTestStorageHash.GetBytes((IDataReader) reader, true);
        return testCaseReference2;
      }
    }

    private class TestResultColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder RunBy = new SqlColumnBinder(nameof (RunBy));
      private SqlColumnBinder ComputerName = new SqlColumnBinder(nameof (ComputerName));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder ResolutionStateId = new SqlColumnBinder(nameof (ResolutionStateId));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder ResetCount = new SqlColumnBinder(nameof (ResetCount));
      private SqlColumnBinder AfnStripId = new SqlColumnBinder(nameof (AfnStripId));
      private SqlColumnBinder EffectivePointState = new SqlColumnBinder(nameof (EffectivePointState));

      internal TestResult2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestResult2 testResult2 = new TestResult2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testResult2.ProjectId = GetDataspaceIdentifier(int32);
        testResult2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testResult2.TestCaseRefId = this.TestCaseRefId.GetInt32((IDataReader) reader);
        testResult2.TestResultId = this.TestResultId.GetInt32((IDataReader) reader);
        testResult2.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testResult2.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testResult2.Outcome = this.Outcome.GetByte((IDataReader) reader);
        testResult2.State = this.State.GetByte((IDataReader) reader);
        testResult2.Revision = this.Revision.GetInt32((IDataReader) reader);
        testResult2.DateStarted = this.DateStarted.GetNullableDateTime((IDataReader) reader);
        testResult2.DateCompleted = this.DateCompleted.GetNullableDateTime((IDataReader) reader);
        testResult2.LastUpdatedBy = this.LastUpdatedBy.GetNullableGuid((IDataReader) reader);
        testResult2.RunBy = this.RunBy.GetNullableGuid((IDataReader) reader);
        testResult2.ComputerName = this.ComputerName.GetString((IDataReader) reader, true);
        testResult2.FailureType = this.FailureType.GetNullableByte((IDataReader) reader);
        testResult2.ResolutionStateId = this.ResolutionStateId.GetNullableInt32((IDataReader) reader);
        testResult2.Owner = this.Owner.GetNullableGuid((IDataReader) reader);
        testResult2.ResetCount = this.ResetCount.GetNullableInt32((IDataReader) reader);
        testResult2.AfnStripId = this.AfnStripId.GetNullableInt32((IDataReader) reader);
        testResult2.EffectivePointState = this.EffectivePointState.GetNullableByte((IDataReader) reader);
        return testResult2;
      }
    }

    private class TestMessageLogEntryColumns
    {
      private SqlColumnBinder TestMessageLogId = new SqlColumnBinder(nameof (TestMessageLogId));
      private SqlColumnBinder EntryId = new SqlColumnBinder(nameof (EntryId));
      private SqlColumnBinder LogUser = new SqlColumnBinder(nameof (LogUser));
      private SqlColumnBinder DateCreated = new SqlColumnBinder(nameof (DateCreated));
      private SqlColumnBinder LogLevel = new SqlColumnBinder(nameof (LogLevel));
      private SqlColumnBinder Message = new SqlColumnBinder(nameof (Message));

      internal TestMessageLogEntry2 bind(SqlDataReader reader) => new TestMessageLogEntry2()
      {
        TestMessageLogId = this.TestMessageLogId.GetInt32((IDataReader) reader),
        EntryId = this.EntryId.GetInt32((IDataReader) reader),
        LogUser = this.LogUser.GetGuid((IDataReader) reader),
        DateCreated = this.DateCreated.GetDateTime((IDataReader) reader),
        LogLevel = this.LogLevel.GetByte((IDataReader) reader),
        Message = this.Message.GetString((IDataReader) reader, true)
      };
    }

    private class TestRunColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder IncompleteTests = new SqlColumnBinder(nameof (IncompleteTests));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder DropLocation = new SqlColumnBinder(nameof (DropLocation));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder PostProcessState = new SqlColumnBinder(nameof (PostProcessState));
      private SqlColumnBinder DueDate = new SqlColumnBinder(nameof (DueDate));
      private SqlColumnBinder Controller = new SqlColumnBinder(nameof (Controller));
      private SqlColumnBinder TestMessageLogId = new SqlColumnBinder(nameof (TestMessageLogId));
      private SqlColumnBinder LegacySharePath = new SqlColumnBinder(nameof (LegacySharePath));
      private SqlColumnBinder TestSettingsId = new SqlColumnBinder(nameof (TestSettingsId));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder IsAutomated = new SqlColumnBinder(nameof (IsAutomated));
      private SqlColumnBinder TestEnvironmentId = new SqlColumnBinder(nameof (TestEnvironmentId));
      private SqlColumnBinder Version = new SqlColumnBinder(nameof (Version));
      private SqlColumnBinder PublicTestSettingsId = new SqlColumnBinder(nameof (PublicTestSettingsId));
      private SqlColumnBinder IsBvt = new SqlColumnBinder(nameof (IsBvt));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      private SqlColumnBinder PassedTests = new SqlColumnBinder(nameof (PassedTests));
      private SqlColumnBinder NotApplicableTests = new SqlColumnBinder(nameof (NotApplicableTests));
      private SqlColumnBinder UnanalyzedTests = new SqlColumnBinder(nameof (UnanalyzedTests));
      private SqlColumnBinder IsMigrated = new SqlColumnBinder(nameof (IsMigrated));
      private SqlColumnBinder ReleaseUri = new SqlColumnBinder(nameof (ReleaseUri));
      private SqlColumnBinder ReleaseEnvironmentUri = new SqlColumnBinder(nameof (ReleaseEnvironmentUri));
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder MaxReservedResultId = new SqlColumnBinder(nameof (MaxReservedResultId));
      private SqlColumnBinder DeletedOn = new SqlColumnBinder(nameof (DeletedOn));

      internal TestRun2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestRun2 testRun2 = new TestRun2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testRun2.ProjectId = GetDataspaceIdentifier(int32);
        testRun2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRun2.Title = this.Title.GetString((IDataReader) reader, false);
        testRun2.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testRun2.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testRun2.Owner = this.Owner.GetGuid((IDataReader) reader);
        testRun2.State = this.State.GetByte((IDataReader) reader);
        testRun2.IncompleteTests = this.IncompleteTests.GetInt32((IDataReader) reader);
        testRun2.TestPlanId = this.TestPlanId.GetInt32((IDataReader) reader);
        testRun2.IterationId = this.IterationId.GetNullableInt32((IDataReader) reader);
        testRun2.DropLocation = this.DropLocation.GetString((IDataReader) reader, true);
        testRun2.BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
        testRun2.ErrorMessage = this.ErrorMessage.GetString((IDataReader) reader, false);
        testRun2.StartDate = this.StartDate.GetNullableDateTime((IDataReader) reader);
        testRun2.CompleteDate = this.CompleteDate.GetNullableDateTime((IDataReader) reader);
        testRun2.PostProcessState = this.PostProcessState.GetByte((IDataReader) reader);
        testRun2.DueDate = this.DueDate.GetNullableDateTime((IDataReader) reader);
        testRun2.Controller = this.Controller.GetString((IDataReader) reader, true);
        testRun2.TestMessageLogId = this.TestMessageLogId.GetInt32((IDataReader) reader);
        testRun2.LegacySharePath = this.LegacySharePath.GetString((IDataReader) reader, false);
        testRun2.TestSettingsId = this.TestSettingsId.GetInt32((IDataReader) reader);
        testRun2.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        testRun2.Revision = this.Revision.GetInt32((IDataReader) reader);
        testRun2.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader);
        testRun2.Type = this.Type.GetByte((IDataReader) reader);
        testRun2.CoverageId = this.CoverageId.GetNullableInt32((IDataReader) reader);
        testRun2.IsAutomated = this.IsAutomated.GetBoolean((IDataReader) reader);
        testRun2.TestEnvironmentId = this.TestEnvironmentId.GetGuid((IDataReader) reader);
        testRun2.Version = this.Version.GetInt32((IDataReader) reader);
        testRun2.PublicTestSettingsId = this.PublicTestSettingsId.GetInt32((IDataReader) reader);
        testRun2.IsBvt = this.IsBvt.GetBoolean((IDataReader) reader);
        testRun2.Comment = this.Comment.GetString((IDataReader) reader, false);
        testRun2.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        testRun2.PassedTests = this.PassedTests.GetInt32((IDataReader) reader);
        testRun2.NotApplicableTests = this.NotApplicableTests.GetInt32((IDataReader) reader);
        testRun2.UnanalyzedTests = this.UnanalyzedTests.GetInt32((IDataReader) reader);
        testRun2.IsMigrated = this.IsMigrated.GetBoolean((IDataReader) reader);
        testRun2.ReleaseUri = this.ReleaseUri.GetString((IDataReader) reader, true);
        testRun2.ReleaseEnvironmentUri = this.ReleaseEnvironmentUri.GetString((IDataReader) reader, true);
        testRun2.TestRunContextId = this.TestRunContextId.GetInt32((IDataReader) reader);
        testRun2.MaxReservedResultId = this.MaxReservedResultId.GetNullableInt32((IDataReader) reader);
        testRun2.DeletedOn = this.DeletedOn.GetNullableDateTime((IDataReader) reader);
        return testRun2;
      }
    }

    private class TestRunSummaryColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunStatsId = new SqlColumnBinder(nameof (TestRunStatsId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder TestRunCompletedDate = new SqlColumnBinder(nameof (TestRunCompletedDate));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder ResultDuration = new SqlColumnBinder(nameof (ResultDuration));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder IsRerun = new SqlColumnBinder(nameof (IsRerun));

      internal TestRunSummary2 Bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestRunSummary2 testRunSummary2 = new TestRunSummary2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testRunSummary2.ProjectId = GetDataspaceIdentifier(int32);
        testRunSummary2.TestRunStatsId = (long) this.TestRunStatsId.GetInt32((IDataReader) reader);
        testRunSummary2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRunSummary2.TestRunContextId = this.TestRunContextId.GetInt32((IDataReader) reader);
        testRunSummary2.TestRunCompletedDate = this.TestRunCompletedDate.GetDateTime((IDataReader) reader);
        testRunSummary2.TestOutcome = this.TestOutcome.GetByte((IDataReader) reader);
        testRunSummary2.ResultCount = this.ResultCount.GetInt32((IDataReader) reader);
        testRunSummary2.ResultDuration = this.ResultDuration.GetInt64((IDataReader) reader, 0L);
        testRunSummary2.RunDuration = this.RunDuration.GetInt64((IDataReader) reader);
        testRunSummary2.IsRerun = this.IsRerun.GetBoolean((IDataReader) reader);
        return testRunSummary2;
      }
    }

    private class TestResultsExColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder FieldName = new SqlColumnBinder(nameof (FieldName));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder IntValue = new SqlColumnBinder(nameof (IntValue));
      private SqlColumnBinder FloatValue = new SqlColumnBinder(nameof (FloatValue));
      private SqlColumnBinder BitValue = new SqlColumnBinder(nameof (BitValue));
      private SqlColumnBinder DateTimeValue = new SqlColumnBinder(nameof (DateTimeValue));
      private SqlColumnBinder GuidValue = new SqlColumnBinder(nameof (GuidValue));
      private SqlColumnBinder StringValue = new SqlColumnBinder(nameof (StringValue));

      internal TestResultsEx2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestResultsEx2 testResultsEx2 = new TestResultsEx2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testResultsEx2.ProjectId = GetDataspaceIdentifier(int32);
        testResultsEx2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testResultsEx2.TestResultId = this.TestResultId.GetInt32((IDataReader) reader);
        testResultsEx2.FieldId = this.FieldId.GetInt32((IDataReader) reader);
        testResultsEx2.FieldName = this.FieldName.GetString((IDataReader) reader, false);
        testResultsEx2.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testResultsEx2.IntValue = this.IntValue.GetNullableInt32((IDataReader) reader);
        testResultsEx2.FloatValue = this.FloatValue.GetNullableDouble((IDataReader) reader);
        testResultsEx2.BitValue = this.BitValue.GetNullableBoolean((IDataReader) reader);
        testResultsEx2.DateTimeValue = this.DateTimeValue.GetNullableDateTime((IDataReader) reader);
        testResultsEx2.GuidValue = this.GuidValue.GetNullableGuid((IDataReader) reader);
        testResultsEx2.StringValue = this.StringValue.GetString((IDataReader) reader, true);
        return testResultsEx2;
      }
    }

    private class TestCaseMetadataColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestMetadataId = new SqlColumnBinder(nameof (TestMetadataId));
      private SqlColumnBinder Container = new SqlColumnBinder(nameof (Container));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));

      internal TestCaseMetadata2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestCaseMetadata2 testCaseMetadata2 = new TestCaseMetadata2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testCaseMetadata2.ProjectId = GetDataspaceIdentifier(int32);
        testCaseMetadata2.TestMetadataId = this.TestMetadataId.GetInt32((IDataReader) reader);
        testCaseMetadata2.Container = this.Container.GetString((IDataReader) reader, false);
        testCaseMetadata2.Name = this.Name.GetString((IDataReader) reader, false);
        return testCaseMetadata2;
      }
    }

    private class RequirementsToTestsMappingColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder TestMetadataId = new SqlColumnBinder(nameof (TestMetadataId));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder CreatedBy = new SqlColumnBinder(nameof (CreatedBy));
      private SqlColumnBinder DeletionDate = new SqlColumnBinder(nameof (DeletionDate));
      private SqlColumnBinder DeletedBy = new SqlColumnBinder(nameof (DeletedBy));
      private SqlColumnBinder IsMigratedToWIT = new SqlColumnBinder(nameof (IsMigratedToWIT));

      internal RequirementsToTestsMapping2 bind(
        SqlDataReader reader,
        System.Func<int, Guid> GetDataspaceIdentifier)
      {
        RequirementsToTestsMapping2 requirementsToTestsMapping2 = new RequirementsToTestsMapping2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        requirementsToTestsMapping2.ProjectId = GetDataspaceIdentifier(int32);
        requirementsToTestsMapping2.WorkItemId = this.WorkItemId.GetInt32((IDataReader) reader);
        requirementsToTestsMapping2.TestMetadataId = this.TestMetadataId.GetInt32((IDataReader) reader);
        requirementsToTestsMapping2.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        requirementsToTestsMapping2.CreatedBy = this.CreatedBy.GetGuid((IDataReader) reader);
        requirementsToTestsMapping2.DeletionDate = this.DeletionDate.GetNullableDateTime((IDataReader) reader);
        requirementsToTestsMapping2.DeletedBy = this.DeletedBy.GetNullableGuid((IDataReader) reader);
        requirementsToTestsMapping2.IsMigratedToWIT = this.IsMigratedToWIT.GetBoolean((IDataReader) reader);
        return requirementsToTestsMapping2;
      }
    }

    private class TestResultResetColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder DateModified = new SqlColumnBinder(nameof (DateModified));
      private SqlColumnBinder AuditIdentity = new SqlColumnBinder(nameof (AuditIdentity));
      private SqlColumnBinder TestResultRV = new SqlColumnBinder(nameof (TestResultRV));

      internal TestResultReset2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestResultReset2 testResultReset2 = new TestResultReset2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testResultReset2.ProjectId = GetDataspaceIdentifier(int32);
        testResultReset2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testResultReset2.TestResultId = this.TestResultId.GetInt32((IDataReader) reader);
        testResultReset2.Revision = this.Revision.GetInt32((IDataReader) reader);
        testResultReset2.DateModified = this.DateModified.GetDateTime((IDataReader) reader);
        testResultReset2.AuditIdentity = this.AuditIdentity.GetGuid((IDataReader) reader);
        testResultReset2.TestResultRV = this.TestResultRV.GetBytes((IDataReader) reader, false);
        return testResultReset2;
      }
    }

    private class TestActionResultColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPath = new SqlColumnBinder(nameof (ActionPath));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder SharedStepId = new SqlColumnBinder(nameof (SharedStepId));
      private SqlColumnBinder SharedStepRevision = new SqlColumnBinder(nameof (SharedStepRevision));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));

      internal TestActionResult2 bind(SqlDataReader reader) => new TestActionResult2()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        IterationId = this.IterationId.GetInt32((IDataReader) reader),
        ActionPath = this.ActionPath.GetString((IDataReader) reader, false),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        Outcome = this.Outcome.GetByte((IDataReader) reader),
        ErrorMessage = this.ErrorMessage.GetString((IDataReader) reader, false),
        DateStarted = this.DateStarted.GetNullableDateTime((IDataReader) reader),
        DateCompleted = this.DateCompleted.GetNullableDateTime((IDataReader) reader),
        Duration = this.Duration.GetInt64((IDataReader) reader),
        SharedStepId = this.SharedStepId.GetNullableInt32((IDataReader) reader),
        SharedStepRevision = this.SharedStepRevision.GetNullableInt32((IDataReader) reader),
        Comment = this.Comment.GetString((IDataReader) reader, false)
      };
    }

    private class TestRunExColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder FieldName = new SqlColumnBinder(nameof (FieldName));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder IntValue = new SqlColumnBinder(nameof (IntValue));
      private SqlColumnBinder FloatValue = new SqlColumnBinder(nameof (FloatValue));
      private SqlColumnBinder BitValue = new SqlColumnBinder(nameof (BitValue));
      private SqlColumnBinder DateTimeValue = new SqlColumnBinder(nameof (DateTimeValue));
      private SqlColumnBinder GuidValue = new SqlColumnBinder(nameof (GuidValue));
      private SqlColumnBinder StringValue = new SqlColumnBinder(nameof (StringValue));

      internal TestRunEx2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestRunEx2 testRunEx2 = new TestRunEx2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testRunEx2.ProjectId = GetDataspaceIdentifier(int32);
        testRunEx2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRunEx2.FieldId = this.FieldId.GetInt32((IDataReader) reader);
        testRunEx2.FieldName = this.FieldName.GetString((IDataReader) reader, false);
        testRunEx2.CreatedDate = this.CreatedDate.GetDateTime((IDataReader) reader);
        testRunEx2.IntValue = this.IntValue.GetNullableInt32((IDataReader) reader);
        testRunEx2.FloatValue = this.FloatValue.GetNullableDouble((IDataReader) reader);
        testRunEx2.BitValue = this.BitValue.GetNullableBoolean((IDataReader) reader);
        testRunEx2.DateTimeValue = this.DateTimeValue.GetNullableDateTime((IDataReader) reader);
        testRunEx2.GuidValue = this.GuidValue.GetNullableGuid((IDataReader) reader);
        testRunEx2.StringValue = this.StringValue.GetString((IDataReader) reader, true);
        return testRunEx2;
      }
    }

    private class TestRunExtendedColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Substate = new SqlColumnBinder(nameof (Substate));
      private SqlColumnBinder SourceFilter = new SqlColumnBinder(nameof (SourceFilter));
      private SqlColumnBinder TestCaseFilter = new SqlColumnBinder(nameof (TestCaseFilter));
      private SqlColumnBinder TestEnvironmentUrl = new SqlColumnBinder(nameof (TestEnvironmentUrl));
      private SqlColumnBinder AutEnvironmentUrl = new SqlColumnBinder(nameof (AutEnvironmentUrl));
      private SqlColumnBinder CsmContent = new SqlColumnBinder(nameof (CsmContent));
      private SqlColumnBinder CsmParameters = new SqlColumnBinder(nameof (CsmParameters));
      private SqlColumnBinder SubscriptionName = new SqlColumnBinder(nameof (SubscriptionName));

      internal TestRunExtended2 bind(SqlDataReader reader, System.Func<int, Guid> GetDataspaceIdentifier)
      {
        TestRunExtended2 testRunExtended2 = new TestRunExtended2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        testRunExtended2.ProjectId = GetDataspaceIdentifier(int32);
        testRunExtended2.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRunExtended2.Substate = this.Substate.GetNullableByte((IDataReader) reader);
        testRunExtended2.SourceFilter = this.SourceFilter.GetString((IDataReader) reader, true);
        testRunExtended2.TestCaseFilter = this.TestCaseFilter.GetString((IDataReader) reader, true);
        testRunExtended2.TestEnvironmentUrl = this.TestEnvironmentUrl.GetString((IDataReader) reader, true);
        testRunExtended2.AutEnvironmentUrl = this.AutEnvironmentUrl.GetString((IDataReader) reader, true);
        testRunExtended2.CsmContent = this.CsmContent.GetString((IDataReader) reader, true);
        testRunExtended2.CsmParameters = this.CsmParameters.GetString((IDataReader) reader, true);
        testRunExtended2.SubscriptionName = this.SubscriptionName.GetString((IDataReader) reader, true);
        return testRunExtended2;
      }
    }

    private class TestParameterColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPath = new SqlColumnBinder(nameof (ActionPath));
      private SqlColumnBinder ParameterName = new SqlColumnBinder(nameof (ParameterName));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder DateModified = new SqlColumnBinder(nameof (DateModified));
      private SqlColumnBinder DataType = new SqlColumnBinder(nameof (DataType));
      private SqlColumnBinder Expected = new SqlColumnBinder(nameof (Expected));
      private SqlColumnBinder Actual = new SqlColumnBinder(nameof (Actual));

      internal TestParameter2 bind(SqlDataReader reader) => new TestParameter2()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        IterationId = this.IterationId.GetInt32((IDataReader) reader),
        ActionPath = this.ActionPath.GetString((IDataReader) reader, false),
        ParameterName = this.ParameterName.GetString((IDataReader) reader, false),
        CreationDate = this.CreationDate.GetDateTime((IDataReader) reader),
        DateModified = this.DateModified.GetDateTime((IDataReader) reader),
        DataType = this.DataType.GetByte((IDataReader) reader),
        Expected = this.Expected.GetBytes((IDataReader) reader, true),
        Actual = this.Actual.GetBytes((IDataReader) reader, true)
      };
    }

    private class Coverage2Columns
    {
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder DateCreated = new SqlColumnBinder(nameof (DateCreated));
      private SqlColumnBinder DateModified = new SqlColumnBinder(nameof (DateModified));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastError = new SqlColumnBinder(nameof (LastError));

      internal Coverage2 bind(SqlDataReader reader) => new Coverage2()
      {
        CoverageId = this.CoverageId.GetInt32((IDataReader) reader),
        DateCreated = this.DateCreated.GetDateTime((IDataReader) reader),
        DateModified = this.DateModified.GetDateTime((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        LastError = this.LastError.GetString((IDataReader) reader, true)
      };
    }

    private class CodeCoverageSummaryColumns2
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder Label = new SqlColumnBinder(nameof (Label));
      private SqlColumnBinder Position = new SqlColumnBinder(nameof (Position));
      private SqlColumnBinder Total = new SqlColumnBinder(nameof (Total));
      private SqlColumnBinder Covered = new SqlColumnBinder(nameof (Covered));

      internal CodeCoverageSummary2 bind(
        SqlDataReader reader,
        System.Func<int, Guid> GetDataspaceIdentifier)
      {
        CodeCoverageSummary2 coverageSummary2 = new CodeCoverageSummary2();
        int int32 = this.DataspaceId.GetInt32((IDataReader) reader);
        coverageSummary2.ProjectId = GetDataspaceIdentifier(int32);
        coverageSummary2.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        coverageSummary2.Label = this.Label.GetString((IDataReader) reader, false);
        coverageSummary2.Position = this.Position.GetInt32((IDataReader) reader);
        coverageSummary2.Total = this.Total.GetInt32((IDataReader) reader);
        coverageSummary2.Covered = this.Covered.GetInt32((IDataReader) reader);
        return coverageSummary2;
      }
    }

    private class ModuleCoverage2Columns
    {
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder ModuleId = new SqlColumnBinder(nameof (ModuleId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Signature = new SqlColumnBinder(nameof (Signature));
      private SqlColumnBinder SignatureAge = new SqlColumnBinder(nameof (SignatureAge));
      private SqlColumnBinder LinesCovered = new SqlColumnBinder(nameof (LinesCovered));
      private SqlColumnBinder LinesPartiallyCovered = new SqlColumnBinder(nameof (LinesPartiallyCovered));
      private SqlColumnBinder LinesNotCovered = new SqlColumnBinder(nameof (LinesNotCovered));
      private SqlColumnBinder BlocksCovered = new SqlColumnBinder(nameof (BlocksCovered));
      private SqlColumnBinder BlocksNotCovered = new SqlColumnBinder(nameof (BlocksNotCovered));
      private SqlColumnBinder BlockCount = new SqlColumnBinder(nameof (BlockCount));
      private SqlColumnBinder BlockData = new SqlColumnBinder(nameof (BlockData));
      private SqlColumnBinder BlockDataLength = new SqlColumnBinder(nameof (BlockDataLength));
      private SqlColumnBinder CoverageFileUrl = new SqlColumnBinder(nameof (CoverageFileUrl));

      internal ModuleCoverage2 bind(SqlDataReader reader) => new ModuleCoverage2()
      {
        CoverageId = this.CoverageId.GetInt32((IDataReader) reader),
        ModuleId = this.ModuleId.GetInt32((IDataReader) reader),
        Name = this.Name.GetString((IDataReader) reader, false),
        Signature = this.Signature.GetNullableGuid((IDataReader) reader),
        SignatureAge = this.SignatureAge.GetNullableInt32((IDataReader) reader),
        LinesCovered = this.LinesCovered.GetInt32((IDataReader) reader),
        LinesPartiallyCovered = this.LinesPartiallyCovered.GetInt32((IDataReader) reader),
        LinesNotCovered = this.LinesNotCovered.GetInt32((IDataReader) reader),
        BlocksCovered = this.BlocksCovered.GetInt32((IDataReader) reader),
        BlocksNotCovered = this.BlocksNotCovered.GetInt32((IDataReader) reader),
        BlockCount = this.BlockCount.GetInt32((IDataReader) reader),
        BlockData = this.BlockData.GetBytes((IDataReader) reader, true),
        BlockDataLength = this.BlockDataLength.GetInt32((IDataReader) reader),
        CoverageFileUrl = this.CoverageFileUrl.GetString((IDataReader) reader, true)
      };
    }

    private class FunctionCoverageColumns2
    {
      private SqlColumnBinder CoverageId = new SqlColumnBinder(nameof (CoverageId));
      private SqlColumnBinder ModuleId = new SqlColumnBinder(nameof (ModuleId));
      private SqlColumnBinder FunctionId = new SqlColumnBinder(nameof (FunctionId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder SourceFile = new SqlColumnBinder(nameof (SourceFile));
      private SqlColumnBinder Class = new SqlColumnBinder(nameof (Class));
      private SqlColumnBinder Namespace = new SqlColumnBinder(nameof (Namespace));
      private SqlColumnBinder LinesCovered = new SqlColumnBinder(nameof (LinesCovered));
      private SqlColumnBinder LinesPartiallyCovered = new SqlColumnBinder(nameof (LinesPartiallyCovered));
      private SqlColumnBinder LinesNotCovered = new SqlColumnBinder(nameof (LinesNotCovered));
      private SqlColumnBinder BlocksCovered = new SqlColumnBinder(nameof (BlocksCovered));
      private SqlColumnBinder BlocksNotCovered = new SqlColumnBinder(nameof (BlocksNotCovered));

      internal FunctionCoverage2 bind(SqlDataReader reader) => new FunctionCoverage2()
      {
        CoverageId = this.CoverageId.GetInt32((IDataReader) reader),
        ModuleId = this.ModuleId.GetInt32((IDataReader) reader),
        FunctionId = this.FunctionId.GetInt32((IDataReader) reader),
        Name = this.Name.GetString((IDataReader) reader, false),
        SourceFile = this.SourceFile.GetString((IDataReader) reader, true),
        Class = this.Class.GetString((IDataReader) reader, true),
        Namespace = this.Namespace.GetString((IDataReader) reader, true),
        LinesCovered = this.LinesCovered.GetInt32((IDataReader) reader),
        LinesPartiallyCovered = this.LinesPartiallyCovered.GetInt32((IDataReader) reader),
        LinesNotCovered = this.LinesNotCovered.GetInt32((IDataReader) reader),
        BlocksCovered = this.BlocksCovered.GetInt32((IDataReader) reader),
        BlocksNotCovered = this.BlocksNotCovered.GetInt32((IDataReader) reader)
      };
    }

    private class TCMPropertyBagColumns2
    {
      private SqlColumnBinder ArtifactType = new SqlColumnBinder(nameof (ArtifactType));
      private SqlColumnBinder ArtifactId = new SqlColumnBinder(nameof (ArtifactId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Value = new SqlColumnBinder(nameof (Value));

      internal TCMPropertyBag2 bind(SqlDataReader reader) => new TCMPropertyBag2()
      {
        ArtifactType = this.ArtifactType.GetInt32((IDataReader) reader),
        ArtifactId = this.ArtifactId.GetInt32((IDataReader) reader),
        Name = this.Name.GetString((IDataReader) reader, false),
        Value = this.Value.GetString((IDataReader) reader, true)
      };
    }

    private class PointsResultsColumns2
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder ChangeNumber = new SqlColumnBinder(nameof (ChangeNumber));
      private SqlColumnBinder LastTestRunId = new SqlColumnBinder(nameof (LastTestRunId));
      private SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));
      private SqlColumnBinder LastFailureType = new SqlColumnBinder(nameof (LastFailureType));

      internal PointsResults2 bind(SqlDataReader reader) => new PointsResults2()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        PlanId = this.PlanId.GetInt32((IDataReader) reader),
        ChangeNumber = this.ChangeNumber.GetInt32((IDataReader) reader),
        LastTestRunId = this.LastTestRunId.GetInt32((IDataReader) reader),
        LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        LastResultState = this.LastResultState.GetNullableByte((IDataReader) reader),
        LastResultOutcome = this.LastResultOutcome.GetNullableByte((IDataReader) reader),
        LastResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader),
        LastFailureType = this.LastFailureType.GetNullableByte((IDataReader) reader)
      };
    }

    private class PointHistoryOutcomeBackfillColumns
    {
      private SqlColumnBinder ColumnsUpdated = new SqlColumnBinder(nameof (ColumnsUpdated));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder ChangeNumber = new SqlColumnBinder(nameof (ChangeNumber));

      internal void bind(
        SqlDataReader reader,
        out int columnsUpdated,
        out int planId,
        out int pointId,
        out int changeNumber)
      {
        columnsUpdated = this.ColumnsUpdated.GetInt32((IDataReader) reader);
        planId = this.PlanId.GetInt32((IDataReader) reader);
        pointId = this.PointId.GetInt32((IDataReader) reader);
        changeNumber = this.ChangeNumber.GetInt32((IDataReader) reader);
      }
    }

    protected class QueryTestMessageLogentryColumns
    {
      private SqlColumnBinder TestMessageLogId = new SqlColumnBinder(nameof (TestMessageLogId));
      private SqlColumnBinder EntryId = new SqlColumnBinder(nameof (EntryId));
      private SqlColumnBinder LogUser = new SqlColumnBinder(nameof (LogUser));
      private SqlColumnBinder DateCreated = new SqlColumnBinder(nameof (DateCreated));
      private SqlColumnBinder LogLevel = new SqlColumnBinder(nameof (LogLevel));
      private SqlColumnBinder Message = new SqlColumnBinder(nameof (Message));

      internal TestMessageLogEntry bind(SqlDataReader reader) => new TestMessageLogEntry()
      {
        TestMessageLogId = this.TestMessageLogId.GetInt32((IDataReader) reader),
        EntryId = this.EntryId.GetInt32((IDataReader) reader),
        LogUser = this.LogUser.GetGuid((IDataReader) reader, false),
        DateCreated = this.DateCreated.GetDateTime((IDataReader) reader),
        LogLevel = this.LogLevel.GetByte((IDataReader) reader),
        Message = this.Message.GetString((IDataReader) reader, true)
      };
    }

    protected class IdAndRevColumns
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));

      internal IdAndRev bind(SqlDataReader reader) => new IdAndRev()
      {
        Id = this.Id.GetInt32((IDataReader) reader),
        Revision = this.Revision.GetInt32((IDataReader) reader)
      };
    }

    protected class QueryAreaUriColumns
    {
      internal SqlColumnBinder AreaUri = new SqlColumnBinder(nameof (AreaUri));
    }

    private class QuerySessionMetadataColumns
    {
      private SqlColumnBinder SessionId = new SqlColumnBinder("TestSessionId");
      private SqlColumnBinder SessionType = new SqlColumnBinder(nameof (SessionType));
      private SqlColumnBinder SessionName = new SqlColumnBinder(nameof (SessionName));
      private SqlColumnBinder SessionUid = new SqlColumnBinder(nameof (SessionUid));
      private SqlColumnBinder SessionStartTimeUTC = new SqlColumnBinder(nameof (SessionStartTimeUTC));
      private SqlColumnBinder SessionEndTimeUTC = new SqlColumnBinder(nameof (SessionEndTimeUTC));
      private SqlColumnBinder SessionState = new SqlColumnBinder(nameof (SessionState));
      private SqlColumnBinder SessionResult = new SqlColumnBinder(nameof (SessionResult));
      private SqlColumnBinder SourceSessionID = new SqlColumnBinder(nameof (SourceSessionID));
      private SqlColumnBinder SourceOriginSystem = new SqlColumnBinder(nameof (SourceOriginSystem));
      private SqlColumnBinder SourceTenantID = new SqlColumnBinder(nameof (SourceTenantID));
      private SqlColumnBinder SourceTenantName = new SqlColumnBinder(nameof (SourceTenantName));

      internal OneMRXSession bind(SqlDataReader reader)
      {
        OneMRXSession oneMrxSession = new OneMRXSession()
        {
          Id = this.SessionId.GetInt64((IDataReader) reader),
          Type = this.SessionType.ColumnExists((IDataReader) reader) ? this.SessionType.GetString((IDataReader) reader, true) : (string) null,
          Name = this.SessionName.ColumnExists((IDataReader) reader) ? this.SessionName.GetString((IDataReader) reader, true) : (string) null,
          Uid = this.SessionUid.ColumnExists((IDataReader) reader) ? this.SessionUid.GetGuid((IDataReader) reader) : Guid.Empty,
          StartTimeUTC = this.SessionStartTimeUTC.ColumnExists((IDataReader) reader) ? this.SessionStartTimeUTC.GetDateTime((IDataReader) reader) : new DateTime(),
          EndTimeUTC = this.SessionEndTimeUTC.ColumnExists((IDataReader) reader) ? this.SessionEndTimeUTC.GetDateTime((IDataReader) reader) : new DateTime(),
          State = this.SessionState.ColumnExists((IDataReader) reader) ? (TestResultsSessionState) this.SessionState.GetByte((IDataReader) reader) : TestResultsSessionState.None,
          Result = this.SessionResult.ColumnExists((IDataReader) reader) ? (SessionResult) this.SessionResult.GetByte((IDataReader) reader) : SessionResult.None,
          Source = new Source()
        };
        oneMrxSession.Source.SessionId = this.SourceSessionID.ColumnExists((IDataReader) reader) ? this.SourceSessionID.GetGuid((IDataReader) reader, true) : Guid.Empty;
        oneMrxSession.Source.OriginSystem = this.SourceOriginSystem.ColumnExists((IDataReader) reader) ? this.SourceOriginSystem.GetString((IDataReader) reader, true) : (string) null;
        oneMrxSession.Source.TenantId = this.SourceTenantID.ColumnExists((IDataReader) reader) ? this.SourceTenantID.GetGuid((IDataReader) reader, true) : Guid.Empty;
        oneMrxSession.Source.TenantName = this.SourceTenantName.ColumnExists((IDataReader) reader) ? this.SourceTenantName.GetString((IDataReader) reader, true) : (string) null;
        return oneMrxSession;
      }
    }

    private class QuerySessionTestRunColumns
    {
      private SqlColumnBinder SessionId = new SqlColumnBinder("TestSessionId");
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));

      internal Dictionary<long, List<int>> Bind(
        SqlDataReader reader,
        Dictionary<long, List<int>> testSessionRunMap)
      {
        long int64 = this.SessionId.GetInt64((IDataReader) reader);
        int int32 = this.TestRunId.GetInt32((IDataReader) reader);
        if (!testSessionRunMap.ContainsKey(int64))
          testSessionRunMap[int64] = new List<int>();
        testSessionRunMap[int64].Add(int32);
        return testSessionRunMap;
      }
    }

    private class QuerySessionLayoutColumns
    {
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Uid = new SqlColumnBinder(nameof (Uid));
      private SqlColumnBinder StartTimeUTC = new SqlColumnBinder(nameof (StartTimeUTC));
      private SqlColumnBinder EndTimeUTC = new SqlColumnBinder(nameof (EndTimeUTC));
      private SqlColumnBinder Properties = new SqlColumnBinder(nameof (Properties));

      internal Layout bind(SqlDataReader reader)
      {
        Layout layout = new Layout();
        layout.Type = this.Type.ColumnExists((IDataReader) reader) ? this.Type.GetString((IDataReader) reader, true) : (string) null;
        layout.Name = this.Name.ColumnExists((IDataReader) reader) ? this.Name.GetString((IDataReader) reader, true) : (string) null;
        layout.Uid = this.Uid.ColumnExists((IDataReader) reader) ? this.Uid.GetGuid((IDataReader) reader) : Guid.Empty;
        layout.StartTimeUTC = this.StartTimeUTC.ColumnExists((IDataReader) reader) ? this.StartTimeUTC.GetDateTime((IDataReader) reader) : new DateTime();
        layout.EndTimeUTC = this.EndTimeUTC.ColumnExists((IDataReader) reader) ? this.EndTimeUTC.GetDateTime((IDataReader) reader) : new DateTime();
        string str = this.Properties.ColumnExists((IDataReader) reader) ? this.Properties.GetString((IDataReader) reader, true) : (string) null;
        if (!string.IsNullOrEmpty(str))
        {
          Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
          layout.Properties = dictionary;
        }
        return layout;
      }
    }

    private class QueryTestRunColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DropLocation = new SqlColumnBinder(nameof (DropLocation));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder PostProcessState = new SqlColumnBinder(nameof (PostProcessState));
      private SqlColumnBinder DueDate = new SqlColumnBinder(nameof (DueDate));
      private SqlColumnBinder IterationUri = new SqlColumnBinder(nameof (IterationUri));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder Controller = new SqlColumnBinder(nameof (Controller));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder TestMessageLogId = new SqlColumnBinder(nameof (TestMessageLogId));
      private SqlColumnBinder Guid = new SqlColumnBinder(nameof (Guid));
      private SqlColumnBinder LegacySharePath = new SqlColumnBinder(nameof (LegacySharePath));
      private SqlColumnBinder TestSettingsId = new SqlColumnBinder(nameof (TestSettingsId));
      private SqlColumnBinder PublicTestSettingsId = new SqlColumnBinder(nameof (PublicTestSettingsId));
      private SqlColumnBinder TestEnvironmentId = new SqlColumnBinder(nameof (TestEnvironmentId));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
      private SqlColumnBinder IsAutomated = new SqlColumnBinder(nameof (IsAutomated));
      private SqlColumnBinder Version = new SqlColumnBinder(nameof (Version));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder IsBvt = new SqlColumnBinder(nameof (IsBvt));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder RV = new SqlColumnBinder(nameof (RV));
      private SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      private SqlColumnBinder IncompleteTests = new SqlColumnBinder(nameof (IncompleteTests));
      private SqlColumnBinder NotApplicableTests = new SqlColumnBinder(nameof (NotApplicableTests));
      private SqlColumnBinder PassedTests = new SqlColumnBinder(nameof (PassedTests));
      private SqlColumnBinder UnanalyzedTests = new SqlColumnBinder(nameof (UnanalyzedTests));
      private SqlColumnBinder BugsCount = new SqlColumnBinder(nameof (BugsCount));
      private SqlColumnBinder SourceFilter = new SqlColumnBinder(nameof (SourceFilter));
      private SqlColumnBinder TestCaseFilter = new SqlColumnBinder(nameof (TestCaseFilter));
      private SqlColumnBinder TestEnvironmentUrl = new SqlColumnBinder(nameof (TestEnvironmentUrl));
      private SqlColumnBinder AutEnvironmentUrl = new SqlColumnBinder(nameof (AutEnvironmentUrl));
      private SqlColumnBinder Substate = new SqlColumnBinder("SubState");
      private SqlColumnBinder CsmContent = new SqlColumnBinder(nameof (CsmContent));
      private SqlColumnBinder CsmParameters = new SqlColumnBinder(nameof (CsmParameters));
      private SqlColumnBinder SubscriptionName = new SqlColumnBinder(nameof (SubscriptionName));
      private SqlColumnBinder ReleaseUri = new SqlColumnBinder(nameof (ReleaseUri));
      private SqlColumnBinder ReleaseEnvironmentUri = new SqlColumnBinder(nameof (ReleaseEnvironmentUri));

      internal TestRun bind(SqlDataReader reader, out int dataspaceId, out string iterationUri)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRun.Title = this.Title.GetString((IDataReader) reader, false);
        testRun.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testRun.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testRun.Owner = this.Owner.GetGuid((IDataReader) reader, false);
        testRun.State = this.State.GetByte((IDataReader) reader);
        testRun.ErrorMessage = this.ErrorMessage.GetString((IDataReader) reader, false);
        testRun.BuildUri = this.BuildUri.GetString((IDataReader) reader, true);
        testRun.DropLocation = this.DropLocation.GetString((IDataReader) reader, true);
        testRun.BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
        testRun.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true);
        testRun.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true);
        testRun.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader, 0);
        testRun.StartDate = this.StartDate.GetDateTime((IDataReader) reader);
        testRun.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        testRun.PostProcessState = this.PostProcessState.GetByte((IDataReader) reader);
        testRun.DueDate = this.DueDate.GetDateTime((IDataReader) reader);
        testRun.IterationId = this.IterationId.GetInt32((IDataReader) reader, 0);
        testRun.Controller = this.Controller.GetString((IDataReader) reader, true);
        testRun.TestPlanId = this.TestPlanId.GetInt32((IDataReader) reader);
        testRun.TestMessageLogId = this.TestMessageLogId.GetInt32((IDataReader) reader);
        testRun.LegacySharePath = this.LegacySharePath.GetString((IDataReader) reader, false);
        testRun.TestSettingsId = this.TestSettingsId.GetInt32((IDataReader) reader);
        testRun.PublicTestSettingsId = this.PublicTestSettingsId.GetInt32((IDataReader) reader);
        testRun.TestEnvironmentId = this.TestEnvironmentId.GetGuid((IDataReader) reader);
        testRun.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testRun.Type = this.Type.GetByte((IDataReader) reader);
        testRun.IsAutomated = this.IsAutomated.GetBoolean((IDataReader) reader);
        testRun.Version = this.Version.GetInt32((IDataReader) reader);
        testRun.Revision = this.Revision.GetInt32((IDataReader) reader);
        testRun.IsBvt = this.IsBvt.GetBoolean((IDataReader) reader);
        testRun.Comment = this.Comment.GetString((IDataReader) reader, false);
        testRun.RowVersion = this.RV.GetBytes((IDataReader) reader, false);
        testRun.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        testRun.IncompleteTests = this.IncompleteTests.GetInt32((IDataReader) reader);
        testRun.NotApplicableTests = this.NotApplicableTests.GetInt32((IDataReader) reader);
        testRun.PassedTests = this.PassedTests.GetInt32((IDataReader) reader);
        testRun.UnanalyzedTests = this.UnanalyzedTests.GetInt32((IDataReader) reader);
        testRun.BugsCount = this.BugsCount.GetInt32((IDataReader) reader);
        testRun.ReleaseUri = this.ReleaseUri.GetString((IDataReader) reader, true);
        testRun.ReleaseEnvironmentUri = this.ReleaseEnvironmentUri.GetString((IDataReader) reader, true);
        testRun.BuildReference = new BuildConfiguration();
        if (((int) testRun.Type & 16) != 0)
        {
          testRun.DtlTestEnvironment = new ShallowReference()
          {
            Url = this.TestEnvironmentUrl.GetString((IDataReader) reader, true)
          };
          testRun.DtlAutEnvironment = new ShallowReference()
          {
            Url = this.AutEnvironmentUrl.GetString((IDataReader) reader, true)
          };
          if (testRun.IsAutomated)
          {
            testRun.Filter = new RunFilter();
            testRun.Filter.SourceFilter = this.SourceFilter.GetString((IDataReader) reader, false);
            testRun.Filter.TestCaseFilter = this.TestCaseFilter.GetString((IDataReader) reader, true);
            testRun.Substate = this.Substate.GetByte((IDataReader) reader);
            testRun.CsmContent = this.CsmContent.GetString((IDataReader) reader, true);
            testRun.CsmParameters = this.CsmParameters.GetString((IDataReader) reader, true);
            testRun.SubscriptionName = this.SubscriptionName.GetString((IDataReader) reader, true);
          }
        }
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        iterationUri = this.IterationUri.GetString((IDataReader) reader, true);
        return testRun;
      }
    }

    protected class CreateTestResolutionStateColumns
    {
      internal SqlColumnBinder stateId = new SqlColumnBinder("StateId");
    }

    protected class CreateTestFailureTypeColumns
    {
      internal SqlColumnBinder failureTypeId = new SqlColumnBinder("FailureTypeId");
    }

    private class CreateTestRunColumns
    {
      internal SqlColumnBinder testRunId = new SqlColumnBinder("TestRunId");
      internal SqlColumnBinder revision = new SqlColumnBinder("Revision");

      internal TestRun bind(SqlDataReader reader)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.testRunId.GetInt32((IDataReader) reader);
        testRun.Revision = this.revision.GetInt32((IDataReader) reader);
        return testRun;
      }
    }

    private class CreateTestSessionColumns
    {
      internal SqlColumnBinder TestSessionId = new SqlColumnBinder(nameof (TestSessionId));

      internal long bind(SqlDataReader reader) => this.TestSessionId.GetInt64((IDataReader) reader);
    }

    internal class QueryStructuresColumns
    {
      internal SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      internal SqlColumnBinder Uri = new SqlColumnBinder(nameof (Uri));
      internal SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));

      internal Dictionary<string, IdAndString> bind(SqlDataReader reader)
      {
        Dictionary<string, IdAndString> dictionary = new Dictionary<string, IdAndString>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        while (reader.Read())
        {
          string userPath = DBPath.DatabaseToUserPath(this.Name.GetString((IDataReader) reader, false), false, false);
          string str = this.Uri.GetString((IDataReader) reader, false);
          int int32 = this.Id.GetInt32((IDataReader) reader);
          if (!dictionary.ContainsKey(userPath))
            dictionary[userPath] = new IdAndString(str, int32);
        }
        return dictionary;
      }
    }

    protected class QueryTestResolutionStatesColumns
    {
      private SqlColumnBinder StateId = new SqlColumnBinder(nameof (StateId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));

      internal TestResolutionState bind(SqlDataReader reader) => new TestResolutionState()
      {
        Id = this.StateId.GetInt32((IDataReader) reader),
        Name = this.Name.GetString((IDataReader) reader, false)
      };
    }

    protected class QueryTestFailureTypesColumns
    {
      private SqlColumnBinder FailureTypeId = new SqlColumnBinder(nameof (FailureTypeId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));

      internal TestFailureType bind(SqlDataReader reader) => new TestFailureType()
      {
        Id = this.FailureTypeId.GetInt32((IDataReader) reader),
        Name = this.Name.GetString((IDataReader) reader, false)
      };
    }

    private class QueryTestActionResultsColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPathCol = new SqlColumnBinder("ActionPath");
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder SharedStepId = new SqlColumnBinder(nameof (SharedStepId));
      private SqlColumnBinder SharedStepRevision = new SqlColumnBinder(nameof (SharedStepRevision));

      internal TestActionResult bind(SqlDataReader reader)
      {
        int int32_1 = this.SharedStepId.GetInt32((IDataReader) reader);
        int int32_2 = this.SharedStepRevision.GetInt32((IDataReader) reader, 0);
        string actionPath = this.ActionPathCol.GetString((IDataReader) reader, false);
        TestActionResult instance = TestActionResultUtils.GetInstance(actionPath, int32_1);
        instance.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        instance.TestResultId = this.TestResultId.GetInt32((IDataReader) reader);
        instance.IterationId = this.IterationId.GetInt32((IDataReader) reader);
        instance.ActionPath = actionPath;
        instance.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        instance.Outcome = this.Outcome.GetByte((IDataReader) reader);
        instance.ErrorMessage = this.ErrorMessage.GetString((IDataReader) reader, false);
        instance.Comment = this.Comment.GetString((IDataReader) reader, false);
        instance.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        instance.DateStarted = this.DateStarted.GetDateTime((IDataReader) reader);
        instance.DateCompleted = this.DateCompleted.GetDateTime((IDataReader) reader);
        instance.Duration = this.Duration.GetInt64((IDataReader) reader);
        instance.SetId = int32_1;
        instance.SetRevision = int32_2;
        return instance;
      }
    }

    protected class QueryTestActionResultsColumns2
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder ActionPathCol = new SqlColumnBinder("ActionPath");
      private SqlColumnBinder ParameterName = new SqlColumnBinder(nameof (ParameterName));
      private SqlColumnBinder DataType = new SqlColumnBinder(nameof (DataType));
      private SqlColumnBinder Expected = new SqlColumnBinder(nameof (Expected));
      private SqlColumnBinder Actual = new SqlColumnBinder(nameof (Actual));

      internal TestResultParameter bind(SqlDataReader reader) => new TestResultParameter()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        IterationId = this.IterationId.GetInt32((IDataReader) reader),
        ActionPath = this.ActionPathCol.GetString((IDataReader) reader, false),
        ParameterName = this.ParameterName.GetString((IDataReader) reader, false),
        DataType = this.DataType.GetByte((IDataReader) reader),
        Expected = this.Expected.GetBytes((IDataReader) reader, true),
        Actual = this.Actual.GetBytes((IDataReader) reader, true)
      };
    }

    protected class FetchTestResultsColumns
    {
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder TestPointId = new SqlColumnBinder(nameof (TestPointId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ResolutionStateId = new SqlColumnBinder(nameof (ResolutionStateId));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private SqlColumnBinder TestCaseArea = new SqlColumnBinder(nameof (TestCaseArea));
      private SqlColumnBinder TestCaseRevision = new SqlColumnBinder(nameof (TestCaseRevision));
      private SqlColumnBinder ComputerName = new SqlColumnBinder(nameof (ComputerName));
      private SqlColumnBinder AfnStripId = new SqlColumnBinder(nameof (AfnStripId));
      private SqlColumnBinder ResetCount = new SqlColumnBinder(nameof (ResetCount));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder AutomatedTestType = new SqlColumnBinder(nameof (AutomatedTestType));
      private SqlColumnBinder AutomatedTestTypeId = new SqlColumnBinder(nameof (AutomatedTestTypeId));
      private SqlColumnBinder AutomatedTestId = new SqlColumnBinder(nameof (AutomatedTestId));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder RunBy = new SqlColumnBinder(nameof (RunBy));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder TestSuiteId = new SqlColumnBinder(nameof (TestSuiteId));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder TestCaseOwner = new SqlColumnBinder(nameof (TestCaseOwner));
      private SqlColumnBinder TestRunTitle = new SqlColumnBinder(nameof (TestRunTitle));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.CreationDate = this.CreationDate.ColumnExists((IDataReader) reader) ? this.CreationDate.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.TestRunId = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestResultId = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestCaseReferenceId = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestCaseId = this.TestCaseId.ColumnExists((IDataReader) reader) ? this.TestCaseId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ConfigurationId = this.ConfigurationId.ColumnExists((IDataReader) reader) ? this.ConfigurationId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ConfigurationName = this.ConfigurationName.ColumnExists((IDataReader) reader) ? this.ConfigurationName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestPointId = this.TestPointId.ColumnExists((IDataReader) reader) ? this.TestPointId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.State = this.State.ColumnExists((IDataReader) reader) ? this.State.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.Outcome = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.ResolutionStateId = this.ResolutionStateId.ColumnExists((IDataReader) reader) ? this.ResolutionStateId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.Comment = this.Comment.ColumnExists((IDataReader) reader) ? this.Comment.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.ErrorMessage = this.ErrorMessage.ColumnExists((IDataReader) reader) ? this.ErrorMessage.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.LastUpdated = this.LastUpdated.ColumnExists((IDataReader) reader) ? this.LastUpdated.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.DateStarted = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.DateCompleted = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.Owner = this.Owner.ColumnExists((IDataReader) reader) ? this.Owner.GetGuid((IDataReader) reader, false) : Guid.Empty;
        testCaseResult.Priority = this.Priority.ColumnExists((IDataReader) reader) ? this.Priority.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.TestCaseTitle = this.TestCaseTitle.ColumnExists((IDataReader) reader) ? this.TestCaseTitle.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestCaseAreaUri = this.TestCaseArea.ColumnExists((IDataReader) reader) ? this.TestCaseArea.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestCaseRevision = this.TestCaseRevision.ColumnExists((IDataReader) reader) ? this.TestCaseRevision.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ComputerName = this.ComputerName.ColumnExists((IDataReader) reader) ? this.ComputerName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AfnStripId = this.AfnStripId.ColumnExists((IDataReader) reader) ? this.AfnStripId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ResetCount = this.ResetCount.ColumnExists((IDataReader) reader) ? this.ResetCount.GetInt32((IDataReader) reader) : 0;
        testCaseResult.FailureType = this.FailureType.ColumnExists((IDataReader) reader) ? this.FailureType.GetByte((IDataReader) reader, (byte) 0) : (byte) 0;
        testCaseResult.AutomatedTestName = this.AutomatedTestName.ColumnExists((IDataReader) reader) ? this.AutomatedTestName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestStorage = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestType = this.AutomatedTestType.ColumnExists((IDataReader) reader) ? this.AutomatedTestType.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestTypeId = this.AutomatedTestTypeId.ColumnExists((IDataReader) reader) ? this.AutomatedTestTypeId.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestId = this.AutomatedTestId.ColumnExists((IDataReader) reader) ? this.AutomatedTestId.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.Revision = this.Revision.ColumnExists((IDataReader) reader) ? this.Revision.GetInt32((IDataReader) reader) : 0;
        testCaseResult.RunBy = this.RunBy.ColumnExists((IDataReader) reader) ? this.RunBy.GetGuid((IDataReader) reader, true) : Guid.Empty;
        testCaseResult.LastUpdatedBy = this.LastUpdatedBy.ColumnExists((IDataReader) reader) ? this.LastUpdatedBy.GetGuid((IDataReader) reader, false) : Guid.Empty;
        testCaseResult.BuildNumber = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestPlanId = this.TestPlanId.ColumnExists((IDataReader) reader) ? this.TestPlanId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestSuiteId = this.TestSuiteId.ColumnExists((IDataReader) reader) ? this.TestSuiteId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.SuiteName = this.SuiteName.ColumnExists((IDataReader) reader) ? this.SuiteName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.OwnerName = this.TestCaseOwner.ColumnExists((IDataReader) reader) ? this.TestCaseOwner.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.Duration = this.Duration.ColumnExists((IDataReader) reader) ? this.Duration.GetInt64((IDataReader) reader) : TestManagementDatabase.GetDurationFromStartAndCompleteDates(testCaseResult.DateStarted, testCaseResult.DateCompleted);
        testCaseResult.Duration = testCaseResult.Duration == 0L ? TestManagementDatabase.GetDurationFromStartAndCompleteDates(testCaseResult.DateStarted, testCaseResult.DateCompleted) : testCaseResult.Duration;
        testCaseResult.TestRunTitle = this.TestRunTitle.ColumnExists((IDataReader) reader) ? this.TestRunTitle.GetString((IDataReader) reader, true) : string.Empty;
        return testCaseResult;
      }
    }

    protected class FetchTestResultsExColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));

      internal Tuple<int, int, TestExtensionField> bind(SqlDataReader reader)
      {
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        TestExtensionField testExtensionField1 = new TestManagementDatabase.FetchTestExColumns().bind(reader);
        int num = int32_2;
        TestExtensionField testExtensionField2 = testExtensionField1;
        return new Tuple<int, int, TestExtensionField>(int32_1, num, testExtensionField2);
      }
    }

    protected class FetchTestRunsExColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));

      internal Tuple<int, TestExtensionField> bind(SqlDataReader reader) => new Tuple<int, TestExtensionField>(this.TestRunId.GetInt32((IDataReader) reader), new TestManagementDatabase.FetchTestExColumns().bind(reader));
    }

    protected class FetchTestExColumns
    {
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder FieldName = new SqlColumnBinder(nameof (FieldName));
      private SqlColumnBinder IntValue = new SqlColumnBinder(nameof (IntValue));
      private SqlColumnBinder FloatValue = new SqlColumnBinder(nameof (FloatValue));
      private SqlColumnBinder BitValue = new SqlColumnBinder(nameof (BitValue));
      private SqlColumnBinder DateTimeValue = new SqlColumnBinder(nameof (DateTimeValue));
      private SqlColumnBinder GuidValue = new SqlColumnBinder(nameof (GuidValue));
      private SqlColumnBinder StringValue = new SqlColumnBinder(nameof (StringValue));

      internal TestExtensionField bind(SqlDataReader reader)
      {
        TestExtensionField testExtensionField = new TestExtensionField()
        {
          Field = new TestExtensionFieldDetails()
        };
        testExtensionField.Field.Id = this.FieldId.GetInt32((IDataReader) reader);
        testExtensionField.Field.Name = this.FieldName.GetString((IDataReader) reader, false);
        if (!this.StringValue.IsNull((IDataReader) reader))
          testExtensionField.Value = (object) this.StringValue.GetString((IDataReader) reader, false);
        else if (!this.IntValue.IsNull((IDataReader) reader))
          testExtensionField.Value = (object) this.IntValue.GetInt32((IDataReader) reader);
        else if (!this.FloatValue.IsNull((IDataReader) reader))
          testExtensionField.Value = (object) this.FloatValue.GetDouble((IDataReader) reader);
        else if (!this.BitValue.IsNull((IDataReader) reader))
          testExtensionField.Value = (object) this.BitValue.GetBoolean((IDataReader) reader);
        else if (!this.DateTimeValue.IsNull((IDataReader) reader))
          testExtensionField.Value = (object) this.DateTimeValue.GetDateTime((IDataReader) reader);
        else if (!this.GuidValue.IsNull((IDataReader) reader))
          testExtensionField.Value = (object) this.GuidValue.GetGuid((IDataReader) reader);
        return testExtensionField;
      }
    }

    protected class QueryTestResultsColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestCaseArea = new SqlColumnBinder(nameof (TestCaseArea));

      internal TestCaseResultIdentifier bind(SqlDataReader reader) => new TestCaseResultIdentifier()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader),
        AreaUri = this.TestCaseArea.ColumnExists((IDataReader) reader) ? this.TestCaseArea.GetString((IDataReader) reader, true) : (string) null
      };

      internal TestCaseResultIdentifier bindDeleted(SqlDataReader reader) => new TestCaseResultIdentifier()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestResultId = this.TestResultId.GetInt32((IDataReader) reader)
      };
    }

    protected class FetchTestResultsColumnsWithSquence
    {
      private SqlColumnBinder Sequence = new SqlColumnBinder(nameof (Sequence));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder TestPointId = new SqlColumnBinder(nameof (TestPointId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ResolutionStateId = new SqlColumnBinder(nameof (ResolutionStateId));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private SqlColumnBinder TestCaseArea = new SqlColumnBinder(nameof (TestCaseArea));
      private SqlColumnBinder TestCaseRevision = new SqlColumnBinder(nameof (TestCaseRevision));
      private SqlColumnBinder ComputerName = new SqlColumnBinder(nameof (ComputerName));
      private SqlColumnBinder AfnStripId = new SqlColumnBinder(nameof (AfnStripId));
      private SqlColumnBinder ResetCount = new SqlColumnBinder(nameof (ResetCount));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder AutomatedTestType = new SqlColumnBinder(nameof (AutomatedTestType));
      private SqlColumnBinder AutomatedTestTypeId = new SqlColumnBinder(nameof (AutomatedTestTypeId));
      private SqlColumnBinder AutomatedTestId = new SqlColumnBinder(nameof (AutomatedTestId));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder RunBy = new SqlColumnBinder(nameof (RunBy));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder TestSuiteId = new SqlColumnBinder(nameof (TestSuiteId));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder TestCaseOwner = new SqlColumnBinder(nameof (TestCaseOwner));
      private SqlColumnBinder TestRunTitle = new SqlColumnBinder(nameof (TestRunTitle));

      internal TestCaseResult bind(SqlDataReader reader)
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.SequenceId = this.Sequence.ColumnExists((IDataReader) reader) ? this.Sequence.GetInt32((IDataReader) reader) : 0;
        testCaseResult.CreationDate = this.CreationDate.ColumnExists((IDataReader) reader) ? this.CreationDate.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.TestRunId = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestResultId = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestCaseReferenceId = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestCaseId = this.TestCaseId.ColumnExists((IDataReader) reader) ? this.TestCaseId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ConfigurationId = this.ConfigurationId.ColumnExists((IDataReader) reader) ? this.ConfigurationId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ConfigurationName = this.ConfigurationName.ColumnExists((IDataReader) reader) ? this.ConfigurationName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestPointId = this.TestPointId.ColumnExists((IDataReader) reader) ? this.TestPointId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.State = this.State.ColumnExists((IDataReader) reader) ? this.State.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.Outcome = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.ResolutionStateId = this.ResolutionStateId.ColumnExists((IDataReader) reader) ? this.ResolutionStateId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.Comment = this.Comment.ColumnExists((IDataReader) reader) ? this.Comment.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.ErrorMessage = this.ErrorMessage.ColumnExists((IDataReader) reader) ? this.ErrorMessage.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.LastUpdated = this.LastUpdated.ColumnExists((IDataReader) reader) ? this.LastUpdated.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.DateStarted = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.DateCompleted = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : new DateTime();
        testCaseResult.Owner = this.Owner.ColumnExists((IDataReader) reader) ? this.Owner.GetGuid((IDataReader) reader, false) : Guid.Empty;
        testCaseResult.Priority = this.Priority.ColumnExists((IDataReader) reader) ? this.Priority.GetByte((IDataReader) reader) : (byte) 0;
        testCaseResult.TestCaseTitle = this.TestCaseTitle.ColumnExists((IDataReader) reader) ? this.TestCaseTitle.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestCaseAreaUri = this.TestCaseArea.ColumnExists((IDataReader) reader) ? this.TestCaseArea.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestCaseRevision = this.TestCaseRevision.ColumnExists((IDataReader) reader) ? this.TestCaseRevision.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ComputerName = this.ComputerName.ColumnExists((IDataReader) reader) ? this.ComputerName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AfnStripId = this.AfnStripId.ColumnExists((IDataReader) reader) ? this.AfnStripId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.ResetCount = this.ResetCount.ColumnExists((IDataReader) reader) ? this.ResetCount.GetInt32((IDataReader) reader) : 0;
        testCaseResult.FailureType = this.FailureType.ColumnExists((IDataReader) reader) ? this.FailureType.GetByte((IDataReader) reader, (byte) 0) : (byte) 0;
        testCaseResult.AutomatedTestName = this.AutomatedTestName.ColumnExists((IDataReader) reader) ? this.AutomatedTestName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestStorage = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestType = this.AutomatedTestType.ColumnExists((IDataReader) reader) ? this.AutomatedTestType.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestTypeId = this.AutomatedTestTypeId.ColumnExists((IDataReader) reader) ? this.AutomatedTestTypeId.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.AutomatedTestId = this.AutomatedTestId.ColumnExists((IDataReader) reader) ? this.AutomatedTestId.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.Revision = this.Revision.ColumnExists((IDataReader) reader) ? this.Revision.GetInt32((IDataReader) reader) : 0;
        testCaseResult.RunBy = this.RunBy.ColumnExists((IDataReader) reader) ? this.RunBy.GetGuid((IDataReader) reader, true) : Guid.Empty;
        testCaseResult.LastUpdatedBy = this.LastUpdatedBy.ColumnExists((IDataReader) reader) ? this.LastUpdatedBy.GetGuid((IDataReader) reader, false) : Guid.Empty;
        testCaseResult.BuildNumber = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.TestPlanId = this.TestPlanId.ColumnExists((IDataReader) reader) ? this.TestPlanId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.TestSuiteId = this.TestSuiteId.ColumnExists((IDataReader) reader) ? this.TestSuiteId.GetInt32((IDataReader) reader) : 0;
        testCaseResult.SuiteName = this.SuiteName.ColumnExists((IDataReader) reader) ? this.SuiteName.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.OwnerName = this.TestCaseOwner.ColumnExists((IDataReader) reader) ? this.TestCaseOwner.GetString((IDataReader) reader, true) : (string) null;
        testCaseResult.Duration = this.Duration.ColumnExists((IDataReader) reader) ? this.Duration.GetInt64((IDataReader) reader) : TestManagementDatabase.GetDurationFromStartAndCompleteDates(testCaseResult.DateStarted, testCaseResult.DateCompleted);
        testCaseResult.Duration = testCaseResult.Duration == 0L ? TestManagementDatabase.GetDurationFromStartAndCompleteDates(testCaseResult.DateStarted, testCaseResult.DateCompleted) : testCaseResult.Duration;
        testCaseResult.TestRunTitle = this.TestRunTitle.ColumnExists((IDataReader) reader) ? this.TestRunTitle.GetString((IDataReader) reader, true) : string.Empty;
        return testCaseResult;
      }
    }

    protected class QueryLegacyRunsColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TmiRunId = new SqlColumnBinder(nameof (TmiRunId));

      internal LegacyTestRun bind(SqlDataReader reader, out int dataspaceId)
      {
        LegacyTestRun legacyTestRun = new LegacyTestRun();
        legacyTestRun.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        legacyTestRun.Title = this.Title.GetString((IDataReader) reader, false);
        legacyTestRun.Owner = this.Owner.GetGuid((IDataReader) reader, false);
        legacyTestRun.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true);
        legacyTestRun.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true);
        legacyTestRun.TmiRunId = this.TmiRunId.GetGuid((IDataReader) reader, false);
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return legacyTestRun;
      }
    }

    protected class QueryTestRunStatisticsColumns
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ResolutionStateId = new SqlColumnBinder(nameof (ResolutionStateId));
      private SqlColumnBinder Count = new SqlColumnBinder(nameof (Count));
      private SqlColumnBinder ResultMetadata = new SqlColumnBinder(nameof (ResultMetadata));

      internal TestRunStatistic Bind(
        SqlDataReader reader,
        IDictionary<int, TestResolutionState> resolutionStates)
      {
        TestRunStatistic testRunStatistic = new TestRunStatistic();
        testRunStatistic.TestRunId = this.Id.GetInt32((IDataReader) reader);
        testRunStatistic.State = this.State.GetByte((IDataReader) reader);
        testRunStatistic.Outcome = this.Outcome.GetByte((IDataReader) reader);
        testRunStatistic.Count = this.Count.GetInt32((IDataReader) reader);
        int int32 = this.ResolutionStateId.GetInt32((IDataReader) reader, 0);
        if (int32 > 0)
          testRunStatistic.ResolutionState = resolutionStates[int32];
        testRunStatistic.ResultMetadata = this.ResultMetadata.ColumnExists((IDataReader) reader) ? this.ResultMetadata.GetByte((IDataReader) reader, (byte) 0) : (byte) 0;
        return testRunStatistic;
      }
    }

    protected class IdsPropertyColumns
    {
      internal SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));

      internal void bind(SqlDataReader reader, List<int> ids)
      {
        if (ids == null)
          return;
        while (reader.Read())
          ids.Add(this.Id.GetInt32((IDataReader) reader));
      }
    }

    protected class UpdateTestResultColumns
    {
      private SqlColumnBinder revision = new SqlColumnBinder("Revision");
      private SqlColumnBinder lastUpdated = new SqlColumnBinder("LastUpdated");
      private SqlColumnBinder lastUpdatedBy = new SqlColumnBinder("LastUpdatedBy");
      private SqlColumnBinder resultId = new SqlColumnBinder("TestResultId");

      internal ResultUpdateResponse bind(SqlDataReader reader) => new ResultUpdateResponse()
      {
        Revision = this.revision.GetInt32((IDataReader) reader),
        LastUpdated = this.lastUpdated.GetDateTime((IDataReader) reader),
        LastUpdatedBy = this.lastUpdatedBy.GetGuid((IDataReader) reader, false),
        TestResultId = this.resultId.GetInt32((IDataReader) reader)
      };
    }

    private class RevisionColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
    }

    internal class ReplicationStateColumns
    {
      internal SqlColumnBinder CssSequenceId = new SqlColumnBinder(nameof (CssSequenceId));
      internal SqlColumnBinder DestroyedWorkItem = new SqlColumnBinder(nameof (DestroyedWorkItem));
    }

    protected class QueryTestVariablesColumns
    {
      private SqlColumnBinder VariableId = new SqlColumnBinder(nameof (VariableId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));

      internal TestVariable bind(SqlDataReader reader) => new TestVariable()
      {
        Id = this.VariableId.GetInt32((IDataReader) reader),
        Name = this.Name.GetString((IDataReader) reader, false),
        Description = this.Description.GetString((IDataReader) reader, false),
        Revision = this.Revision.GetInt32((IDataReader) reader)
      };
    }

    private class UpdatedPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));

      internal UpdatedProperties bindUpdatedProperties(SqlDataReader reader) => new UpdatedProperties()
      {
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader)
      };

      internal BlockedPointProperties bindBlockedTestPointProperties(SqlDataReader reader)
      {
        BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
        blockedPointProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        blockedPointProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        blockedPointProperties.LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader);
        return blockedPointProperties;
      }

      internal UpdatedRunProperties bindUpdatedRunProperties(SqlDataReader reader)
      {
        UpdatedRunProperties updatedRunProperties = new UpdatedRunProperties();
        updatedRunProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        updatedRunProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        updatedRunProperties.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        updatedRunProperties.IsRunStarted = this.IsRunStarted.GetBoolean((IDataReader) reader);
        return updatedRunProperties;
      }
    }

    private class MigrationThreshold
    {
      public SqlColumnBinder TestRunThreshold = new SqlColumnBinder(nameof (TestRunThreshold));
      public SqlColumnBinder TestAttachmentThreshold = new SqlColumnBinder(nameof (TestAttachmentThreshold));
    }

    protected class CreateTestConfigurationColumns
    {
      internal SqlColumnBinder configurationId = new SqlColumnBinder("ConfigurationId");
      internal SqlColumnBinder revision = new SqlColumnBinder("Revision");
      internal SqlColumnBinder lastUpdated = new SqlColumnBinder("LastUpdated");
      internal SqlColumnBinder lastUpdatedBy = new SqlColumnBinder("LastUpdatedBy");

      internal TestConfiguration bind(SqlDataReader reader) => new TestConfiguration()
      {
        Id = this.configurationId.GetInt32((IDataReader) reader),
        Revision = this.revision.GetInt32((IDataReader) reader),
        LastUpdated = this.lastUpdated.GetDateTime((IDataReader) reader),
        LastUpdatedBy = this.lastUpdatedBy.GetGuid((IDataReader) reader, false)
      };
    }

    protected class QueryTestConfigurationsColumns
    {
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder AreaUri = new SqlColumnBinder(nameof (AreaUri));
      private SqlColumnBinder IsDefault = new SqlColumnBinder(nameof (IsDefault));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));

      internal TestConfiguration bind(SqlDataReader reader, out string areaUri)
      {
        TestConfiguration testConfiguration = new TestConfiguration();
        testConfiguration.Id = this.ConfigurationId.GetInt32((IDataReader) reader);
        testConfiguration.Name = this.Name.GetString((IDataReader) reader, false);
        testConfiguration.Description = this.Description.GetString((IDataReader) reader, false);
        testConfiguration.IsDefault = this.IsDefault.GetBoolean((IDataReader) reader);
        testConfiguration.State = this.State.GetByte((IDataReader) reader);
        testConfiguration.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testConfiguration.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testConfiguration.Revision = this.Revision.GetInt32((IDataReader) reader);
        areaUri = this.AreaUri.GetString((IDataReader) reader, true);
        return testConfiguration;
      }
    }

    protected class QueryTestConfigurationsColumns2
    {
      private SqlColumnBinder configurationId = new SqlColumnBinder("ConfigurationId");
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Value = new SqlColumnBinder(nameof (Value));

      internal NameValuePair bind(SqlDataReader reader, out int id)
      {
        NameValuePair nameValuePair = new NameValuePair();
        id = this.configurationId.GetInt32((IDataReader) reader);
        nameValuePair.Name = this.Name.GetString((IDataReader) reader, false);
        nameValuePair.Value = this.Value.GetString((IDataReader) reader, false);
        return nameValuePair;
      }
    }

    protected class CreateTestSettingsColumns
    {
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));

      internal UpdatedProperties bind(SqlDataReader reader) => new UpdatedProperties()
      {
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        Id = this.Id.GetInt32((IDataReader) reader)
      };
    }

    protected class QueryTestSettingsColumns
    {
      private SqlColumnBinder SettingsId = new SqlColumnBinder(nameof (SettingsId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder CreatedBy = new SqlColumnBinder(nameof (CreatedBy));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder Settings = new SqlColumnBinder(nameof (Settings));
      private SqlColumnBinder MachineRoles = new SqlColumnBinder(nameof (MachineRoles));
      private SqlColumnBinder AreaUri = new SqlColumnBinder(nameof (AreaUri));
      private SqlColumnBinder IsPublic = new SqlColumnBinder(nameof (IsPublic));
      private SqlColumnBinder IsAutomated = new SqlColumnBinder(nameof (IsAutomated));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));

      internal TestSettings Bind(
        IVssRequestContext context,
        SqlDataReader reader,
        out string areaUri)
      {
        TestSettings testSettings = new TestSettings();
        testSettings.Id = this.SettingsId.GetInt32((IDataReader) reader);
        testSettings.Name = this.Name.GetString((IDataReader) reader, false);
        testSettings.Description = this.Description.GetString((IDataReader) reader, false);
        testSettings.CreatedBy = this.CreatedBy.GetGuid((IDataReader) reader, false);
        testSettings.CreatedDate = this.CreatedDate.GetDateTime((IDataReader) reader);
        testSettings.Settings = this.Settings.GetString((IDataReader) reader, true);
        testSettings.IsPublic = this.IsPublic.GetBoolean((IDataReader) reader);
        testSettings.IsAutomated = this.IsAutomated.GetBoolean((IDataReader) reader);
        testSettings.Revision = this.Revision.GetInt32((IDataReader) reader);
        testSettings.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testSettings.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testSettings.MachineRoles = TestSettingsMachineRole.FromXml(context, this.MachineRoles.GetString((IDataReader) reader, true));
        areaUri = this.AreaUri.GetString((IDataReader) reader, true);
        return testSettings;
      }
    }
  }
}
