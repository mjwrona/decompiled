// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase53
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
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
  public class TestPlanningDatabase53 : TestPlanningDatabase52
  {
    internal TestPlanningDatabase53(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase53()
    {
    }

    internal override int BeginCloneOperation(
      List<int> sourceSuiteIds,
      int targetSuiteId,
      Guid projectGuid,
      Guid targetProjectGuid,
      CloneOptions options,
      Guid createdBy,
      ResultObjectType operationType,
      bool changeCounterInterval = false)
    {
      try
      {
        this.PrepareStoredProcedure("prc_BeginCloneOperation");
        this.BindIdTypeTable("@sourceSuiteIdsTable", (IEnumerable<int>) sourceSuiteIds);
        this.BindInt("@targetSuiteId", targetSuiteId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@targetDataspaceId", this.GetDataspaceId(targetProjectGuid));
        if (options != null && options.RelatedLinkComment != null)
          this.BindString("@linkComment", options.RelatedLinkComment, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        else
          this.BindString("@linkComment", (string) null, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (options != null && (!TestPlanningDatabase.IsNullOrEmpty(options.ResolvedFieldDetails) || !string.IsNullOrEmpty(options.DestinationWorkItemType)))
          this.BindXml("@editFieldDetails", this.DictionaryOfOverriddenFieldDetailsToXml(options.ResolvedFieldDetails, options.DestinationWorkItemType));
        else
          this.BindXml("@editFieldDetails", string.Empty);
        if (options != null && options.CloneRequirements)
          this.BindBoolean("@cloneRequirements", options.CloneRequirements);
        else
          this.BindBoolean("@cloneRequirements", false);
        if (options != null && options.CopyAllSuites)
          this.BindBoolean("@copyAllSuites", options.CopyAllSuites);
        else
          this.BindBoolean("@copyAllSuites", false);
        if (options != null && options.CopyAncestorHierarchy)
          this.BindBoolean("@copyRecursively", false);
        else
          this.BindBoolean("@copyRecursively", true);
        this.BindGuid("@createdBy", createdBy);
        this.BindByte("@operationType", (byte) operationType);
        this.BindBoolean("@changeCounterInterval", changeCounterInterval);
        SqlDataReader sqlDataReader = this.ExecuteReader();
        int num = 0;
        while (sqlDataReader.Read())
          num = sqlDataReader.GetInt32(0);
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "BeginCloneOperation: OpId iss {0}", (object) num);
        return num;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override List<int> Getsuites(int opId)
    {
      IVssRequestContext requestContext = this.RequestContext;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      TestPlanningDatabase53.FetchSuite fetchSuite = new TestPlanningDatabase53.FetchSuite();
      List<int> intList = new List<int>();
      try
      {
        this.PrepareStoredProcedure("prc_GetCloneRelationship");
        this.BindInt("@opId", opId);
        this.BindInt("@itemType", 3);
        this.BindInt("@isCloned", 1);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TargetDataspaceId");
        while (reader.Read())
        {
          int num = fetchSuite.bind(requestContext, reader, out int _);
          intList.Add(num);
        }
        return intList;
      }
      catch (SqlException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "GetCloneOperation: Error {0}", (object) ex.ToString());
        this.MapException(ex);
        throw;
      }
    }

    internal override CloneTestPlanOperationInformation GetPlanCloneOperation(
      int opId,
      out List<Tuple<Guid, Guid, int, CloneTestPlanOperationInformation>> projectsSuiteIdsList,
      out Dictionary<string, string> resolvedFieldDetails)
    {
      resolvedFieldDetails = new Dictionary<string, string>();
      IVssRequestContext requestContext = this.RequestContext;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: OpId:{0}", (object) opId);
      CloneTestPlanOperationInformation planCloneOperation = new CloneTestPlanOperationInformation();
      TestPlanningDatabase53.FetchCloneInformationColumnsForTest informationColumnsForTest = new TestPlanningDatabase53.FetchCloneInformationColumnsForTest();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("SourceSuiteParentSuiteId");
      projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneTestPlanOperationInformation>>();
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
          planCloneOperation = informationColumnsForTest.bind(requestContext, reader, out sourceDataspaceId, out resolvedFieldDetails);
          int int32_1 = sqlColumnBinder2.GetInt32((IDataReader) reader);
          Guid dataspaceIdentifier1 = this.GetDataspaceIdentifier(sourceDataspaceId);
          Guid dataspaceIdentifier2 = this.GetDataspaceIdentifier(int32_1);
          int int32_2 = sqlColumnBinder1.GetInt32((IDataReader) reader, -1);
          projectsSuiteIdsList.Add(new Tuple<Guid, Guid, int, CloneTestPlanOperationInformation>(dataspaceIdentifier1, dataspaceIdentifier2, int32_2, planCloneOperation));
        }
        requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: {0}", (object) planCloneOperation);
        return planCloneOperation;
      }
      catch (SqlException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "GetCloneOperation: Error {0}", (object) ex.ToString());
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
      TestPlanningDatabase53.FetchCloneInformationColumnsForSuite informationColumnsForSuite = new TestPlanningDatabase53.FetchCloneInformationColumnsForSuite();
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

    protected class FetchSuite
    {
      private SqlColumnBinder OpId = new SqlColumnBinder(nameof (OpId));
      private SqlColumnBinder SourceId = new SqlColumnBinder(nameof (SourceId));
      private SqlColumnBinder ItemType = new SqlColumnBinder(nameof (ItemType));
      private SqlColumnBinder TargetId = new SqlColumnBinder(nameof (TargetId));

      internal int bind(
        IVssRequestContext context,
        SqlDataReader reader,
        out int sourceDataspaceId)
      {
        int int32 = this.SourceId.GetInt32((IDataReader) reader);
        sourceDataspaceId = 1;
        return int32;
      }
    }

    protected class FetchCloneInformationColumnsForTest
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
      private SqlColumnBinder SourceObjectName = new SqlColumnBinder(nameof (SourceObjectName));
      private SqlColumnBinder SourceObjectId = new SqlColumnBinder(nameof (SourceObjectId));
      private SqlColumnBinder TargetPlanName = new SqlColumnBinder(nameof (TargetPlanName));
      private SqlColumnBinder TargetPlanId = new SqlColumnBinder(nameof (TargetPlanId));
      private SqlColumnBinder SourcePlanName = new SqlColumnBinder(nameof (SourcePlanName));
      private SqlColumnBinder SourcePlanId = new SqlColumnBinder(nameof (SourcePlanId));
      private SqlColumnBinder CopyAllSuites = new SqlColumnBinder(nameof (CopyAllSuites));
      private SqlColumnBinder CopyRecursively = new SqlColumnBinder(nameof (CopyRecursively));
      private SqlColumnBinder CloneRequirements = new SqlColumnBinder(nameof (CloneRequirements));

      internal CloneTestPlanOperationInformation bind(
        IVssRequestContext context,
        SqlDataReader reader,
        out int sourceDataspaceId,
        out Dictionary<string, string> resolvedFieldDetails)
      {
        CloneTestPlanOperationInformation operationInformation = new CloneTestPlanOperationInformation();
        CloneOperationCommonResponse operationCommonResponse = new CloneOperationCommonResponse();
        operationCommonResponse.opId = this.OpId.GetInt32((IDataReader) reader);
        operationCommonResponse.creationDate = this.ResultObjectType.GetByte((IDataReader) reader) != (byte) 0 ? this.CreationDate.GetDateTime((IDataReader) reader) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CloneOperationNotFoundForPlan, (object) operationCommonResponse.opId)).Expected("Test Management");
        operationCommonResponse.state = (Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationState) this.State.GetByte((IDataReader) reader);
        operationCommonResponse.completionDate = this.CompletionDate.GetDateTime((IDataReader) reader, DateTime.MaxValue);
        operationCommonResponse.message = this.Message.GetString((IDataReader) reader, true);
        operationCommonResponse.cloneStatistics = new CloneStatistics()
        {
          ClonedRequirementsCount = this.RequirementsCloned.GetInt32((IDataReader) reader),
          ClonedSharedStepsCount = this.SharedStepsCloned.GetInt32((IDataReader) reader),
          ClonedTestCasesCount = this.TestCasesCloned.GetInt32((IDataReader) reader),
          TotalRequirementsCount = this.TotalRequirementsCount.GetInt32((IDataReader) reader),
          TotalTestCasesCount = this.TotalTestCaseCount.GetInt32((IDataReader) reader)
        };
        Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions cloneOptions = new Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions();
        bool? nullableBoolean = this.CopyAllSuites.GetNullableBoolean((IDataReader) reader);
        cloneOptions.CopyAllSuites = nullableBoolean.HasValue && nullableBoolean.Value;
        cloneOptions.CopyAncestorHierarchy = this.CopyRecursively.GetByte((IDataReader) reader) == (byte) 0;
        SourceTestplanResponse testplanResponse = new SourceTestplanResponse();
        testplanResponse.Id = this.SourcePlanId.GetInt32((IDataReader) reader);
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan testPlan1 = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan();
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan testPlan2 = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan()
        {
          Id = this.TargetPlanId.GetInt32((IDataReader) reader)
        };
        cloneOptions.CloneRequirements = this.CloneRequirements.GetBoolean((IDataReader) reader);
        cloneOptions.RelatedLinkComment = this.LinkComment.GetString((IDataReader) reader, true);
        string workItemType;
        resolvedFieldDetails = TestPlanningDatabase.XmlToDictionaryOfOverriddenFieldDetails(context, this.EditFieldDetails.GetString((IDataReader) reader, true), out workItemType);
        cloneOptions.DestinationWorkItemType = workItemType;
        operationInformation.sourceTestPlan = testplanResponse;
        operationInformation.destinationTestPlan = testPlan2;
        operationCommonResponse.creationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        operationInformation.cloneOperationResponse = operationCommonResponse;
        operationInformation.cloneOptions = cloneOptions;
        sourceDataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return operationInformation;
      }
    }

    protected class FetchCloneInformationColumnsForSuite
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
