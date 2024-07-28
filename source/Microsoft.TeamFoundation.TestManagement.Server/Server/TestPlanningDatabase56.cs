// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase56
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase56 : TestPlanningDatabase55
  {
    internal TestPlanningDatabase56(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase56()
    {
    }

    public override UpdatedProperties CreateSuiteEntries(
      Guid projectGuid,
      UpdatedProperties parentProps,
      IEnumerable<TestCaseAndOwner> testCases,
      int toIndex,
      out List<int> configurationIds,
      out List<string> configurationNames,
      TestSuiteSource type,
      List<TestPointAssignment> testCaseConfigurationPair = null)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateSuiteEntries");
        if (testCaseConfigurationPair == null)
          testCaseConfigurationPair = new List<TestPointAssignment>();
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindInt("@toIndex", toIndex);
        this.BindInt("@clientType", (int) (byte) type);
        this.BindTestCaseAndOwnerTypeTable("@testCaseAndOwnerTable", testCases);
        this.BindTestPointAssignmentTypeTable("testCaseConfigurationTable", (IEnumerable<TestPointAssignment>) testCaseConfigurationPair);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        SqlDataReader reader = this.ExecuteReader();
        configurationIds = new List<int>();
        configurationNames = new List<string>();
        TestPlanningDatabase.ConfigurationIdAndNameColumns idAndNameColumns = new TestPlanningDatabase.ConfigurationIdAndNameColumns();
        while (reader.Read())
        {
          configurationIds.Add(idAndNameColumns.ConfigurationId.GetInt32((IDataReader) reader));
          configurationNames.Add(idAndNameColumns.ConfigurationName.GetString((IDataReader) reader, true));
        }
        return parentProps;
      }
      catch (SqlException ex)
      {
        this.HandleDuplicateSuiteEntryError(ex);
        throw;
      }
    }

    public override void SyncTestPointsForTestCaseConfigurations(
      TestManagementRequestContext context,
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<int> testCaseIds,
      List<int> configurationIds,
      List<TestPointAssignment> testCaseConfigurationPair = null)
    {
      try
      {
        context.TraceEnter("Database", "SuiteDatabase.SyncTestPointsForTestCaseConfigurations");
        if (testCaseConfigurationPair == null)
          testCaseConfigurationPair = new List<TestPointAssignment>();
        testCaseConfigurationPair = testCaseConfigurationPair != null ? testCaseConfigurationPair : new List<TestPointAssignment>();
        this.PrepareStoredProcedure("prc_SyncTestPointsForTestCaseConfigurations");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.BindIdTypeTable("@testCaseIds", (IEnumerable<int>) testCaseIds);
        this.BindIdTypeTable("@configurationIds", (IEnumerable<int>) configurationIds);
        this.BindTestPointAssignmentTypeTable("@testCaseConfigurationTable", (IEnumerable<TestPointAssignment>) testCaseConfigurationPair);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        context.TraceLeave("Database", "SuiteDatabase.SyncTestPointsForTestCaseConfigurations");
        this.MapException(ex);
        throw;
      }
    }

    internal override CloneTestSuiteOperationInformation GetSuiteCloneOperation(
      int opId,
      out List<Tuple<Guid, Guid, int, CloneTestSuiteOperationInformation>> projectsSuiteIdsList,
      out Dictionary<string, string> resolvedFieldDetails)
    {
      resolvedFieldDetails = new Dictionary<string, string>();
      IVssRequestContext requestContext = this.RequestContext;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: OpId:{0}", (object) opId);
      CloneTestSuiteOperationInformation suiteCloneOperation = new CloneTestSuiteOperationInformation();
      TestPlanningDatabase56.FetchCloneInformationColumnsForSuite informationColumnsForSuite = new TestPlanningDatabase56.FetchCloneInformationColumnsForSuite();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("SourceSuiteParentSuiteId");
      projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneTestSuiteOperationInformation>>();
      try
      {
        this.PrepareStoredProcedure("prc_GetCloneOperation");
        this.BindInt("@opId", opId);
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, ProjectData> dictionary = new Dictionary<int, ProjectData>();
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("TargetDataspaceId");
        while (reader.Read())
        {
          int sourceDataspaceId;
          suiteCloneOperation = informationColumnsForSuite.bind(requestContext, reader, out sourceDataspaceId, out resolvedFieldDetails);
          int int32_1 = sqlColumnBinder2.GetInt32((IDataReader) reader);
          Guid dataspaceIdentifier1 = this.GetDataspaceIdentifier(sourceDataspaceId);
          Guid dataspaceIdentifier2 = this.GetDataspaceIdentifier(int32_1);
          int int32_2 = sqlColumnBinder1.GetInt32((IDataReader) reader, -1);
          projectsSuiteIdsList.Add(new Tuple<Guid, Guid, int, CloneTestSuiteOperationInformation>(dataspaceIdentifier1, dataspaceIdentifier2, int32_2, suiteCloneOperation));
        }
        requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: {0}", (object) suiteCloneOperation);
        return suiteCloneOperation;
      }
      catch (SqlException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "GetCloneOperation: Error {0}", (object) ex.ToString());
        this.MapException(ex);
        throw;
      }
    }

    protected new class FetchCloneInformationColumnsForSuite
    {
      private SqlColumnBinder OpId = new SqlColumnBinder(nameof (OpId));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder ResultObjectType = new SqlColumnBinder(nameof (ResultObjectType));
      private SqlColumnBinder LinkComment = new SqlColumnBinder(nameof (LinkComment));
      private SqlColumnBinder EditFieldDetails = new SqlColumnBinder(nameof (EditFieldDetails));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder CreatedBy = new SqlColumnBinder(nameof (CreatedBy));
      private SqlColumnBinder CompletionDate = new SqlColumnBinder(nameof (CompletionDate));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Message = new SqlColumnBinder(nameof (Message));
      private SqlColumnBinder TotalTestCaseCount = new SqlColumnBinder(nameof (TotalTestCaseCount));
      private SqlColumnBinder TotalRequirementsCount = new SqlColumnBinder(nameof (TotalRequirementsCount));
      private SqlColumnBinder TestCasesCloned = new SqlColumnBinder(nameof (TestCasesCloned));
      private SqlColumnBinder RequirementsCloned = new SqlColumnBinder(nameof (RequirementsCloned));
      private SqlColumnBinder SharedStepsCloned = new SqlColumnBinder(nameof (SharedStepsCloned));
      private SqlColumnBinder ResultObjectName = new SqlColumnBinder(nameof (ResultObjectName));
      private SqlColumnBinder ResultObjectId = new SqlColumnBinder(nameof (ResultObjectId));
      private SqlColumnBinder TargetSuiteId = new SqlColumnBinder(nameof (TargetSuiteId));
      private SqlColumnBinder SourceObjectName = new SqlColumnBinder(nameof (SourceObjectName));
      private SqlColumnBinder SourceObjectId = new SqlColumnBinder(nameof (SourceObjectId));
      private SqlColumnBinder TargetPlanName = new SqlColumnBinder(nameof (TargetPlanName));
      private SqlColumnBinder TargetPlanId = new SqlColumnBinder(nameof (TargetPlanId));
      private SqlColumnBinder SourcePlanName = new SqlColumnBinder(nameof (SourcePlanName));
      private SqlColumnBinder SourcePlanId = new SqlColumnBinder(nameof (SourcePlanId));
      private SqlColumnBinder CopyAllSuites = new SqlColumnBinder(nameof (CopyAllSuites));
      private SqlColumnBinder CopyRecursively = new SqlColumnBinder(nameof (CopyRecursively));
      private SqlColumnBinder CloneRequirements = new SqlColumnBinder(nameof (CloneRequirements));

      internal CloneTestSuiteOperationInformation bind(
        IVssRequestContext context,
        SqlDataReader reader,
        out int sourceDataspaceId,
        out Dictionary<string, string> resolvedFieldDetails)
      {
        CloneTestSuiteOperationInformation operationInformation = new CloneTestSuiteOperationInformation();
        CloneOperationCommonResponse operationCommonResponse = new CloneOperationCommonResponse();
        operationCommonResponse.opId = this.OpId.GetInt32((IDataReader) reader);
        operationCommonResponse.creationDate = this.ResultObjectType.GetByte((IDataReader) reader) != (byte) 1 ? this.CreationDate.GetDateTime((IDataReader) reader) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CloneOperationNotFoundForSuite, (object) operationCommonResponse.opId)).Expected("Test Management");
        operationCommonResponse.completionDate = this.CompletionDate.GetDateTime((IDataReader) reader, DateTime.MaxValue);
        operationCommonResponse.message = this.Message.GetString((IDataReader) reader, true);
        operationCommonResponse.state = (Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationState) this.State.GetByte((IDataReader) reader);
        operationCommonResponse.cloneStatistics = new CloneStatistics()
        {
          ClonedRequirementsCount = this.RequirementsCloned.GetInt32((IDataReader) reader),
          ClonedSharedStepsCount = this.SharedStepsCloned.GetInt32((IDataReader) reader),
          ClonedTestCasesCount = this.TestCasesCloned.GetInt32((IDataReader) reader),
          TotalRequirementsCount = this.TotalRequirementsCount.GetInt32((IDataReader) reader),
          TotalTestCasesCount = this.TotalTestCaseCount.GetInt32((IDataReader) reader)
        };
        Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions cloneOptions = new Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions();
        cloneOptions.CopyAllSuites = this.CopyAllSuites.GetBoolean((IDataReader) reader);
        cloneOptions.CopyAncestorHierarchy = this.CopyRecursively.GetByte((IDataReader) reader) == (byte) 0;
        cloneOptions.RelatedLinkComment = this.LinkComment.GetString((IDataReader) reader, true);
        string workItemType;
        resolvedFieldDetails = TestPlanningDatabase.XmlToDictionaryOfOverriddenFieldDetails(context, this.EditFieldDetails.GetString((IDataReader) reader, true), out workItemType);
        cloneOptions.DestinationWorkItemType = workItemType;
        TestSuiteReferenceWithProject referenceWithProject1 = new TestSuiteReferenceWithProject();
        referenceWithProject1.Id = this.SourceObjectId.GetInt32((IDataReader) reader);
        referenceWithProject1.Name = this.SourceObjectName.GetString((IDataReader) reader, true) ?? string.Empty;
        int int32 = this.ResultObjectId.GetInt32((IDataReader) reader);
        if (int32 != 0)
        {
          TestSuiteReferenceWithProject referenceWithProject2 = new TestSuiteReferenceWithProject();
          referenceWithProject2.Id = int32;
          referenceWithProject2.Name = this.ResultObjectName.GetString((IDataReader) reader, true) ?? string.Empty;
          operationInformation.clonedTestSuite = referenceWithProject2;
        }
        TestSuiteReferenceWithProject referenceWithProject3 = new TestSuiteReferenceWithProject();
        referenceWithProject3.Id = this.TargetSuiteId.GetInt32((IDataReader) reader);
        cloneOptions.CloneRequirements = this.CloneRequirements.GetBoolean((IDataReader) reader);
        operationCommonResponse.creationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        operationInformation.cloneOperationResponse = operationCommonResponse;
        operationInformation.cloneOptions = cloneOptions;
        operationInformation.sourceTestSuite = referenceWithProject1;
        operationInformation.destinationTestSuite = referenceWithProject3;
        sourceDataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return operationInformation;
      }
    }
  }
}
