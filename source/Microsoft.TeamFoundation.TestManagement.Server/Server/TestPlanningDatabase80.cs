// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase80
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase80 : TestPlanningDatabase79
  {
    internal override List<int> GetTestCaseIds(int opId)
    {
      IVssRequestContext requestContext = this.RequestContext;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      TestPlanningDatabase53.FetchSuite fetchSuite = new TestPlanningDatabase53.FetchSuite();
      List<int> testCaseIds = new List<int>();
      try
      {
        this.PrepareStoredProcedure("prc_GetCloneRelationship");
        this.BindInt("@opId", opId);
        this.BindInt("@itemType", 0);
        this.BindInt("@isCloned", 1);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TargetDataspaceId");
        while (reader.Read())
        {
          int num = fetchSuite.bind(requestContext, reader, out int _);
          testCaseIds.Add(num);
        }
        return testCaseIds;
      }
      catch (SqlException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "GetTestCaseCloneOperation: Error {0}", (object) ex.ToString());
        this.MapException(ex);
        throw;
      }
    }

    internal override int BeginTestCaseCloneOperation(
      int sourceTestPlan,
      int destinationTestPlan,
      int sourceSuiteId,
      int targetSuiteId,
      List<int> testCaseIds,
      Guid projectGuid,
      Guid targetProjectGuid,
      CloneTestCaseOptions options,
      Guid createdBy,
      ResultObjectType operationType)
    {
      try
      {
        this.PrepareStoredProcedure("prc_BeginTestCaseCloneOperation");
        this.BindIdTypeTable("@sourceTestCaseIdsTable", (IEnumerable<int>) testCaseIds);
        this.BindInt("@sourcePlanId", sourceTestPlan);
        this.BindInt("@targetPlanId", destinationTestPlan);
        this.BindInt("@sourceSuiteId", sourceSuiteId);
        this.BindInt("@targetSuiteId", targetSuiteId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@targetDataspaceId", this.GetDataspaceId(targetProjectGuid));
        if (options != null && options.RelatedLinkComment != null)
          this.BindString("@linkComment", options.RelatedLinkComment, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        else
          this.BindString("@linkComment", (string) null, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", createdBy);
        if (options != null && !TestPlanningDatabase.IsNullOrEmpty(options.ResolvedFieldDetails))
          this.BindXml("@editFieldDetails", this.DictionaryOfOverriddenFieldDetailsToXml(options.ResolvedFieldDetails));
        else
          this.BindXml("@editFieldDetails", string.Empty);
        this.BindByte("@operationType", (byte) operationType);
        SqlDataReader sqlDataReader = this.ExecuteReader();
        int num = 0;
        while (sqlDataReader.Read())
          num = sqlDataReader.GetInt32(0);
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "BeginTestCaseCloneOperation: OpId is {0}", (object) num);
        return num;
      }
      catch (SqlException ex)
      {
        this.RequestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "BeginTestCaseCloneOperation: Error {0}", (object) ex.ToString());
        this.MapException(ex);
        throw;
      }
    }

    protected string DictionaryOfOverriddenFieldDetailsToXml(
      Dictionary<string, string> overriddenFieldDetails)
    {
      StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      using (XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w))
      {
        xmlTextWriter.WriteStartElement("Fields");
        if (!TestPlanningDatabase.IsNullOrEmpty(overriddenFieldDetails))
        {
          foreach (KeyValuePair<string, string> overriddenFieldDetail in overriddenFieldDetails)
          {
            xmlTextWriter.WriteStartElement("Field");
            xmlTextWriter.WriteAttributeString("Key", overriddenFieldDetail.Key.ToString());
            xmlTextWriter.WriteAttributeString("Value", overriddenFieldDetail.Value);
            xmlTextWriter.WriteEndElement();
          }
        }
        xmlTextWriter.WriteEndElement();
        xmlTextWriter.Flush();
        return w.ToString();
      }
    }

    internal override CloneOperationInformation GetTestCaseCloneOperation(
      int opId,
      out List<Tuple<Guid, Guid, CloneOperationInformation>> projectGuidList)
    {
      IVssRequestContext requestContext = this.RequestContext;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: OpId:{0}", (object) opId);
      CloneOperationInformation caseCloneOperation = new CloneOperationInformation();
      TestPlanningDatabase.FetchCloneInformationColumns informationColumns = new TestPlanningDatabase.FetchCloneInformationColumns();
      try
      {
        this.PrepareStoredProcedure("prc_GetCloneOperation");
        this.BindInt("@opId", opId);
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, ProjectData> dictionary = new Dictionary<int, ProjectData>();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TargetDataspaceId");
        projectGuidList = new List<Tuple<Guid, Guid, CloneOperationInformation>>();
        while (reader.Read())
        {
          int sourceDataspaceId;
          caseCloneOperation = informationColumns.bind(requestContext, reader, out sourceDataspaceId);
          int int32 = sqlColumnBinder.GetInt32((IDataReader) reader);
          Guid dataspaceIdentifier1 = this.GetDataspaceIdentifier(sourceDataspaceId);
          Guid dataspaceIdentifier2 = this.GetDataspaceIdentifier(int32);
          projectGuidList.Add(new Tuple<Guid, Guid, CloneOperationInformation>(dataspaceIdentifier1, dataspaceIdentifier2, caseCloneOperation));
        }
        requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetTestCaseCloneOperation: {0}", (object) caseCloneOperation);
        return caseCloneOperation;
      }
      catch (SqlException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "GetTestCaseCloneOperation: Error {0}", (object) ex.ToString());
        this.MapException(ex);
        throw;
      }
    }

    internal override void CompleteTestCaseCloneOperation(
      TestManagementRequestContext context,
      CloneOperationInformation opInfo,
      int opId)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CompleteTestCaseCloneOperation");
        this.BindInt("@opId", opId);
        this.BindInt("@targetSuiteId", opInfo.ResultObjectId);
        this.ExecuteReader().Read();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override CloneTestCaseOperationInformation GetTestCaseCloneOperationInfo(
      int opId,
      out List<Tuple<Guid, Guid, CloneTestCaseOperationInformation>> projectGuidList)
    {
      IVssRequestContext requestContext = this.RequestContext;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: OpId:{0}", (object) opId);
      CloneTestCaseOperationInformation cloneOperationInfo = new CloneTestCaseOperationInformation();
      TestPlanningDatabase.FetchCloneInformationColumnsForTestCase columnsForTestCase = new TestPlanningDatabase.FetchCloneInformationColumnsForTestCase();
      projectGuidList = new List<Tuple<Guid, Guid, CloneTestCaseOperationInformation>>();
      try
      {
        this.PrepareStoredProcedure("prc_GetCloneOperation");
        this.BindInt("@opId", opId);
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, ProjectData> dictionary = new Dictionary<int, ProjectData>();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TargetDataspaceId");
        while (reader.Read())
        {
          int sourceDataspaceId;
          cloneOperationInfo = columnsForTestCase.bind(requestContext, reader, out sourceDataspaceId);
          int int32 = sqlColumnBinder.GetInt32((IDataReader) reader);
          Guid dataspaceIdentifier1 = this.GetDataspaceIdentifier(sourceDataspaceId);
          Guid dataspaceIdentifier2 = this.GetDataspaceIdentifier(int32);
          projectGuidList.Add(new Tuple<Guid, Guid, CloneTestCaseOperationInformation>(dataspaceIdentifier1, dataspaceIdentifier2, cloneOperationInfo));
        }
        requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetTestCaseCloneOperationInfo: {0}", (object) cloneOperationInfo);
        return cloneOperationInfo;
      }
      catch (SqlException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "GetTestCaseCloneOperationInfo: Error {0}", (object) ex.ToString());
        this.MapException(ex);
        throw;
      }
    }

    internal TestPlanningDatabase80(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase80()
    {
    }
  }
}
