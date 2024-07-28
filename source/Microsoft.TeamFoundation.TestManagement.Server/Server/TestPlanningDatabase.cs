// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_ServerTestSuiteTypeTable = new SqlMetaData[9]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Title", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("QueryString", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SourceSuiteId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ServerTestSuiteTypeTable2 = new SqlMetaData[10]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Title", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("QueryString", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Status", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SourceSuiteId", SqlDbType.Int),
      new SqlMetaData("AreaUri", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_TestExternalLinkTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("LinkId", SqlDbType.Int),
      new SqlMetaData("Uri", SqlDbType.VarChar, 512L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_TestSuiteEntryTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("TestCaseId", SqlDbType.Int),
      new SqlMetaData("ChildSuiteId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestCaseAndOwnerTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Owner", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] TestManagement_typ_UpdatePointStateAndTesterTypeTable = new SqlMetaData[10]
    {
      new SqlMetaData("PointId", SqlDbType.Int),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("AssignedTo", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("ResetToActive", SqlDbType.Bit),
      new SqlMetaData("TestCaseId", SqlDbType.Int),
      new SqlMetaData("ConfigurationId", SqlDbType.Int),
      new SqlMetaData("LastTestRunId", SqlDbType.Int),
      new SqlMetaData("LastTestResultId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_SuiteTestCasesTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("SuiteId", SqlDbType.Int),
      new SqlMetaData("TestCaseId", SqlDbType.Int),
      new SqlMetaData("Owner", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_SuitesRepopulateInfoTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("SuiteId", SqlDbType.Int),
      new SqlMetaData("UpdateEntries", SqlDbType.Bit),
      new SqlMetaData("LastError", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_ChildSuiteEntryTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("SuiteId", SqlDbType.Int),
      new SqlMetaData("ChildSuiteId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TestPointAssignmentTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("TestCaseId", SqlDbType.Int),
      new SqlMetaData("ConfigurationId", SqlDbType.Int),
      new SqlMetaData("AssignedTo", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_SessionWorkItemLinkTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("SessionId", SqlDbType.Int),
      new SqlMetaData("WorkItemUri", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_SessionWorkItemExploredTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("WorkItemId", SqlDbType.Int),
      new SqlMetaData("ExploredStartTime", SqlDbType.DateTime),
      new SqlMetaData("ExploredEndTime", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_PlanIdToBuildDefinitionTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("PlanId", SqlDbType.Int),
      new SqlMetaData("BuildDefinitionId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_NameValuePairTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Value", SqlDbType.NVarChar, 256L)
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
    private static readonly SqlMetaData[] typ_IdPairTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("SourceId", SqlDbType.Int),
      new SqlMetaData("TargetId", SqlDbType.Int)
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
    private static readonly SqlMetaData[] typ_NameTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_TestMethodTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Container", SqlDbType.NVarChar, 1024L)
    };
    private static readonly SqlMetaData[] TestManagement_typ_TinyIntTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("ByteValue", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] Typ_Int32TypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
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
    private static readonly SqlMetaData[] typ_UpdateTestPointOutcomeTypeTable = new SqlMetaData[10]
    {
      new SqlMetaData("PlanId", SqlDbType.Int),
      new SqlMetaData("PointId", SqlDbType.Int),
      new SqlMetaData("FailureType", SqlDbType.TinyInt),
      new SqlMetaData("LastTestRunId", SqlDbType.Int),
      new SqlMetaData("LastTestResultId", SqlDbType.Int),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LastResultState", SqlDbType.TinyInt),
      new SqlMetaData("LastResultOutcome", SqlDbType.TinyInt),
      new SqlMetaData("LastResolutionStateId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_PointResults3Table = new SqlMetaData[11]
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
      new SqlMetaData("LastFailureType", SqlDbType.TinyInt),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier)
    };
    private const int DefaultMaxLength = 256;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[107]
    {
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase>(32),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase2>(33),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase3>(34),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase4>(35),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase5>(36),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase6>(37),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase7>(38),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase8>(39),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase9>(40),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase10>(41),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase11>(42),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase12>(43),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase13>(44),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase14>(45),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase15>(46),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase16>(47),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase17>(48),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase18>(49),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase19>(50),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase20>(51),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase21>(52),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase22>(53),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase23>(54),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase24>(55),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase24>(56),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase24>(57),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase25>(58),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase26>(59),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase27>(60),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase28>(61),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase29>(62),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase29>(63),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase30>(64),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase31>(65),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase32>(66),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase33>(67),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase34>(68),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase35>(69),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase36>(70),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase37>(71),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase37>(72),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase38>(73),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase39>(74),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase40>(75),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase41>(76),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase41_1>(77),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase42>(78),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase43>(79),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase44>(80),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase45>(81),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase46>(82),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase47>(83),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase48>(84),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase49>(85),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase50>(86),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase51>(87),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase52>(88),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase53>(89),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase54>(90),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase55>(91),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase56>(92),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase57>(93),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase58>(94),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase59>(95),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase60>(96),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase61>(97),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase62>(98),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase63>(99),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase64>(100),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase65>(101),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase66>(102),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase67>(103),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase68>(104),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase69>(105),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase70>(106),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase71>(107),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase72>(108),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase73>(109),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase74>(110),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase75>(111),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase76>(112),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase77>(113),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase78>(114),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase79>(115),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase80>(116),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase81>(117),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase82>(118),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase83>(119),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase84>(120),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase85>(121),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase86>(122),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase87>(123),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase88>(124),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase89>(125),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase90>(126),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase91>((int) sbyte.MaxValue),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase92>(128),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase93>(129),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase94>(130),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase95>(131),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase96>(132),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase97>(133),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase98>(134),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase99>(135),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase100>(136),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase101>(137),
      (IComponentCreator) new ComponentCreator<TestPlanningDatabase102>(138)
    }, "TestManagement");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static Dictionary<string, string> s_TcmDynamicSqlBatchStatementsMap;
    internal static readonly int c_SqlErrorNumber_PKViolation = 2627;
    internal static readonly int c_SqlErrorNumber_IndexKeyViolation = 2601;
    internal const string c_TestResultSprocPrefix = "TestResult.";
    internal const string c_TestManagementSprocPrefix = "TestManagement.";
    private List<TestPlan> s_emptyPlanList = new List<TestPlan>();
    private const int WORKITEM_URI_PARSE = 35;

    protected SqlParameter BindServerTestSuiteTypeTable(
      string parameterName,
      IEnumerable<ServerTestSuite> suites)
    {
      suites = suites ?? Enumerable.Empty<ServerTestSuite>();
      return this.BindTable(parameterName, "typ_ServerTestSuiteTypeTable", this.BindServerTestSuiteTypeTableRows(suites));
    }

    private IEnumerable<SqlDataRecord> BindServerTestSuiteTypeTableRows(
      IEnumerable<ServerTestSuite> suites)
    {
      foreach (ServerTestSuite suite in suites)
      {
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_ServerTestSuiteTypeTable);
        record.SetInt32(0, suite.Id);
        record.SetString(1, suite.Title);
        record.SetString(2, suite.Description ?? string.Empty);
        if (!string.IsNullOrEmpty(suite.ConvertedQueryString))
          record.SetStringPreserveNull(3, suite.ConvertedQueryString);
        else
          record.SetStringPreserveNull(3, suite.QueryString);
        record.SetString(4, suite.Status);
        record.SetInt32(5, suite.Revision);
        record.SetDateTime(6, suite.LastUpdated);
        record.SetGuid(7, suite.LastUpdatedBy);
        record.SetInt32(8, suite.SourceSuiteId);
        yield return record;
      }
    }

    protected SqlParameter BindServerTestSuiteTypeTable2(
      string parameterName,
      IEnumerable<ServerTestSuite> suites)
    {
      suites = suites ?? Enumerable.Empty<ServerTestSuite>();
      IEnumerable<SqlDataRecord> rows = this.BindServerTestSuiteTypeTable2Rows(suites);
      return rows != null ? this.BindTable(parameterName, "typ_ServerTestSuiteTypeTable2", rows) : this.BindTable(parameterName, "typ_ServerTestSuiteTypeTable2", rows, false);
    }

    private IEnumerable<SqlDataRecord> BindServerTestSuiteTypeTable2Rows(
      IEnumerable<ServerTestSuite> suites)
    {
      foreach (ServerTestSuite suite in suites)
      {
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_ServerTestSuiteTypeTable2);
        record.SetInt32(0, suite.Id);
        record.SetString(1, suite.Title);
        record.SetString(2, suite.Description ?? string.Empty);
        if (!string.IsNullOrEmpty(suite.ConvertedQueryString))
          record.SetStringPreserveNull(3, suite.ConvertedQueryString);
        else
          record.SetStringPreserveNull(3, suite.QueryString);
        record.SetString(4, suite.Status);
        record.SetInt32(5, suite.Revision);
        record.SetDateTime(6, suite.LastUpdated);
        record.SetGuid(7, suite.LastUpdatedBy);
        record.SetInt32(8, suite.SourceSuiteId);
        record.SetString(9, suite.AreaUri ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindTestExternalLinkTypeTable(
      string parameterName,
      IEnumerable<TestExternalLink> links)
    {
      links = links ?? Enumerable.Empty<TestExternalLink>();
      return this.BindTable(parameterName, "typ_TestExternalLinkTypeTable", this.BindTestExternalLinkTypeTableRows(links));
    }

    private IEnumerable<SqlDataRecord> BindTestExternalLinkTypeTableRows(
      IEnumerable<TestExternalLink> links)
    {
      foreach (TestExternalLink link in links)
      {
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_TestExternalLinkTypeTable);
        record.SetInt32(0, link.LinkId);
        record.SetStringPreserveNull(1, link.Uri);
        record.SetString(2, link.Description ?? string.Empty);
        yield return record;
      }
    }

    protected SqlParameter BindTestSuiteEntryTypeTable(
      string parameterName,
      IEnumerable<TestSuiteEntry> entries)
    {
      entries = entries ?? Enumerable.Empty<TestSuiteEntry>();
      return this.BindTable(parameterName, "typ_TestSuiteEntryTypeTable", this.BindTestSuiteEntryTypeTableRows(entries));
    }

    private IEnumerable<SqlDataRecord> BindTestSuiteEntryTypeTableRows(
      IEnumerable<TestSuiteEntry> entries)
    {
      foreach (TestSuiteEntry entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_TestSuiteEntryTypeTable);
        if (entry.IsTestCaseEntry)
        {
          sqlDataRecord.SetInt32(0, entry.EntryId);
          sqlDataRecord.SetInt32(1, 0);
        }
        else
        {
          sqlDataRecord.SetInt32(0, 0);
          sqlDataRecord.SetInt32(1, entry.EntryId);
        }
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestCaseAndSuiteTypeTable(
      string parameterName,
      Dictionary<int, List<int>> testCases)
    {
      testCases = testCases ?? new Dictionary<int, List<int>>();
      return this.BindTable(parameterName, "typ_TestSuiteEntryTypeTable", this.BindTestCaseAndSuiteTypeTableRows(testCases));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseAndSuiteTypeTableRows(
      Dictionary<int, List<int>> testCases)
    {
      foreach (int suiteId in testCases.Keys)
      {
        foreach (int num in testCases[suiteId])
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_TestSuiteEntryTypeTable);
          sqlDataRecord.SetInt32(0, num);
          sqlDataRecord.SetInt32(1, suiteId);
          yield return sqlDataRecord;
        }
      }
    }

    protected SqlParameter BindTestCaseAndOwnerTypeTable(
      string parameterName,
      IEnumerable<TestCaseAndOwner> entries)
    {
      entries = entries ?? Enumerable.Empty<TestCaseAndOwner>();
      return this.BindTable(parameterName, "typ_TestCaseAndOwnerTypeTable", this.BindTestCaseAndOwnerTypeTableRows(entries));
    }

    private IEnumerable<SqlDataRecord> BindTestCaseAndOwnerTypeTableRows(
      IEnumerable<TestCaseAndOwner> entries)
    {
      foreach (TestCaseAndOwner entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_TestCaseAndOwnerTypeTable);
        sqlDataRecord.SetInt32(0, entry.Id);
        sqlDataRecord.SetGuid(1, entry.Owner);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindTestManagement_UpdatePointStateAndTesterTypeTable(
      string parameterName,
      IEnumerable<UpdatePointStateAndTester> entries)
    {
      entries = entries ?? Enumerable.Empty<UpdatePointStateAndTester>();
      return this.BindTable(parameterName, "TestManagement.typ_UpdatePointStateAndTesterTypeTable", this.BindTestManagement_UpdatePointStateAndTesterTypeTableRows(entries));
    }

    private IEnumerable<SqlDataRecord> BindTestManagement_UpdatePointStateAndTesterTypeTableRows(
      IEnumerable<UpdatePointStateAndTester> entries)
    {
      foreach (UpdatePointStateAndTester entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.TestManagement_typ_UpdatePointStateAndTesterTypeTable);
        sqlDataRecord.SetInt32(0, entry.PointId);
        sqlDataRecord.SetByte(1, entry.State);
        sqlDataRecord.SetGuid(2, entry.AssignedTo);
        sqlDataRecord.SetGuid(3, entry.LastUpdatedBy);
        sqlDataRecord.SetInt32(4, entry.Revision);
        sqlDataRecord.SetBoolean(5, entry.ResetToActive);
        sqlDataRecord.SetInt32(6, entry.TestCaseId);
        sqlDataRecord.SetInt32(7, entry.ConfigurationId);
        sqlDataRecord.SetInt32(8, entry.LastTestRunId);
        sqlDataRecord.SetInt32(9, entry.LastTestResultId);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindSuiteTestCasesTypeTable(
      string parameterName,
      Dictionary<int, List<TestCaseAndOwner>> entries)
    {
      entries = entries ?? new Dictionary<int, List<TestCaseAndOwner>>();
      return this.BindTable(parameterName, "typ_SuiteTestCasesTypeTable", this.BindSuiteTestCasesTypeTableRows(entries));
    }

    private IEnumerable<SqlDataRecord> BindSuiteTestCasesTypeTableRows(
      Dictionary<int, List<TestCaseAndOwner>> entries)
    {
      foreach (KeyValuePair<int, List<TestCaseAndOwner>> entry1 in entries)
      {
        KeyValuePair<int, List<TestCaseAndOwner>> entry = entry1;
        foreach (TestCaseAndOwner testCaseAndOwner in entry.Value)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_SuiteTestCasesTypeTable);
          sqlDataRecord.SetInt32(0, entry.Key);
          sqlDataRecord.SetInt32(1, testCaseAndOwner.Id);
          sqlDataRecord.SetGuid(2, testCaseAndOwner.Owner);
          yield return sqlDataRecord;
        }
        entry = new KeyValuePair<int, List<TestCaseAndOwner>>();
      }
    }

    protected SqlParameter BindSuitesRepopulateInfoTypeTable(
      string parameterName,
      List<SuiteRepopulateInfo> entries)
    {
      entries = entries ?? new List<SuiteRepopulateInfo>();
      return this.BindTable(parameterName, "typ_SuitesRepopulateInfoTypeTable", this.BindSuitesRepopulateInfoTypeTableRows(entries));
    }

    private IEnumerable<SqlDataRecord> BindSuitesRepopulateInfoTypeTableRows(
      List<SuiteRepopulateInfo> entries)
    {
      foreach (SuiteRepopulateInfo entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_SuitesRepopulateInfoTypeTable);
        sqlDataRecord.SetInt32(0, entry.SuiteId);
        sqlDataRecord.SetBoolean(1, entry.UpdateEntries);
        sqlDataRecord.SetString(2, entry.LastError ?? string.Empty);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindChildSuiteEntryTypeTable(
      string parameterName,
      Dictionary<int, List<int>> entries)
    {
      entries = entries ?? new Dictionary<int, List<int>>();
      return this.BindTable(parameterName, "typ_ChildSuiteEntryTypeTable", this.BindChildSuiteEntryTypeTableRows(entries));
    }

    private IEnumerable<SqlDataRecord> BindChildSuiteEntryTypeTableRows(
      Dictionary<int, List<int>> entries)
    {
      foreach (KeyValuePair<int, List<int>> entry1 in entries)
      {
        KeyValuePair<int, List<int>> entry = entry1;
        foreach (int num in entry.Value)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_ChildSuiteEntryTypeTable);
          sqlDataRecord.SetInt32(0, entry.Key);
          sqlDataRecord.SetInt32(1, num);
          yield return sqlDataRecord;
        }
        entry = new KeyValuePair<int, List<int>>();
      }
    }

    protected SqlParameter BindTestPointAssignmentTypeTable(
      string parameterName,
      IEnumerable<TestPointAssignment> assignments)
    {
      assignments = assignments ?? Enumerable.Empty<TestPointAssignment>();
      return this.BindTable(parameterName, "typ_TestPointAssignmentTypeTable", this.BindTestPointAssignmentTypeTableRows(assignments));
    }

    private IEnumerable<SqlDataRecord> BindTestPointAssignmentTypeTableRows(
      IEnumerable<TestPointAssignment> assignments)
    {
      foreach (TestPointAssignment assignment in assignments)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_TestPointAssignmentTypeTable);
        sqlDataRecord.SetInt32(0, assignment.TestCaseId);
        sqlDataRecord.SetInt32(1, assignment.ConfigurationId);
        sqlDataRecord.SetGuid(2, assignment.AssignedTo);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindSessionWorkItemLinkTypeTable(
      string parameterName,
      IEnumerable<KeyValuePair<int, string>> links)
    {
      links = links ?? Enumerable.Empty<KeyValuePair<int, string>>();
      return this.BindTable(parameterName, "typ_SessionWorkItemLinkTypeTable", this.BindSessionWorkItemLinkTypeTableRows(links));
    }

    private IEnumerable<SqlDataRecord> BindSessionWorkItemLinkTypeTableRows(
      IEnumerable<KeyValuePair<int, string>> links)
    {
      foreach (KeyValuePair<int, string> link in links)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_SessionWorkItemLinkTypeTable);
        sqlDataRecord.SetInt32(0, link.Key);
        TcmTrace.TraceAndDebugAssert("Database", !string.IsNullOrEmpty(link.Value), "The db does not take null values.");
        sqlDataRecord.SetString(1, link.Value);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindSessionWorkItemExploredLinkTypeTable(
      string parameterName,
      IEnumerable<TestSessionExploredWorkItemReference> workItemsExplored)
    {
      workItemsExplored = workItemsExplored ?? Enumerable.Empty<TestSessionExploredWorkItemReference>();
      return this.BindTable(parameterName, "typ_SessionWorkItemExploredTypeTable", this.BindSessionWorkItemExploredLinkTypeTableRows(workItemsExplored));
    }

    private IEnumerable<SqlDataRecord> BindSessionWorkItemExploredLinkTypeTableRows(
      IEnumerable<TestSessionExploredWorkItemReference> workItemsExplored)
    {
      foreach (TestSessionExploredWorkItemReference workItemReference in workItemsExplored)
      {
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_SessionWorkItemExploredTypeTable);
        record.SetInt32(0, workItemReference.Id);
        record.SetDateTimePreserveNull(1, workItemReference.StartTime);
        record.SetDateTimePreserveNull(2, workItemReference.EndTime);
        yield return record;
      }
    }

    protected SqlParameter BindPlanIdToBuildDefinitionTypeTable(
      string parameterName,
      Dictionary<int, List<int>> buildDefIdToPlanIdsMap)
    {
      buildDefIdToPlanIdsMap = buildDefIdToPlanIdsMap ?? new Dictionary<int, List<int>>();
      return this.BindTable(parameterName, "typ_PlanIdToBuildDefinitionTypeTable", this.BindPlanIdToBuildDefinitionTypeTableRows(buildDefIdToPlanIdsMap));
    }

    private IEnumerable<SqlDataRecord> BindPlanIdToBuildDefinitionTypeTableRows(
      Dictionary<int, List<int>> buildDefIdToPlanIdsMap)
    {
      foreach (int buildDefinitionId in buildDefIdToPlanIdsMap.Keys)
      {
        foreach (int num in buildDefIdToPlanIdsMap[buildDefinitionId])
        {
          SqlDataRecord definitionTypeTableRow = new SqlDataRecord(TestPlanningDatabase.typ_PlanIdToBuildDefinitionTypeTable);
          definitionTypeTableRow.SetInt32(0, num);
          definitionTypeTableRow.SetInt32(1, buildDefinitionId);
          yield return definitionTypeTableRow;
        }
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_NameValuePairTypeTable);
        sqlDataRecord.SetString(0, pair.Name ?? string.Empty);
        sqlDataRecord.SetString(1, pair.Value ?? string.Empty);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_IdTypeTable);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_KeyValuePairInt32DateTimeTable);
        sqlDataRecord.SetInt32(0, key);
        sqlDataRecord.SetDateTime(1, idToDateMap[key]);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_IdPairTypeTable);
        sqlDataRecord.SetInt32(0, idMap.Key);
        sqlDataRecord.SetInt32(1, idMap.Value);
        yield return sqlDataRecord;
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_IdAndRevTypeTable);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_UpdatedPropertiesTypeTable);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_PropertyValuePairTypeTable);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_IntStringPairTypeTable);
        sqlDataRecord.SetInt32(0, pair.Key);
        sqlDataRecord.SetString(1, pair.Value ?? string.Empty);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_NameTypeTable);
        sqlDataRecord.SetString(0, name ?? string.Empty);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.typ_TestMethodTypeTable);
        sqlDataRecord.SetString(0, testMethod.Name);
        sqlDataRecord.SetString(1, testMethod.Container);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.TestManagement_typ_TinyIntTypeTable);
        sqlDataRecord.SetByte(0, state);
        yield return sqlDataRecord;
      }
    }

    internal byte[] GetSHA256Hash(string name)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(name);
      using (SHA256Managed shA256Managed = new SHA256Managed())
        return shA256Managed.ComputeHash(bytes);
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
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase.Typ_Int32TypeTable);
        sqlDataRecord.SetInt32(0, id);
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
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_BuildRefTable);
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
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_BuildRefTable2);
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
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_BuildRefTable3);
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

    protected SqlParameter BindUpdateTestPointOutcomeTypeTable(
      string parameterName,
      IEnumerable<TestPointOutcomeUpdateFromTestResultRequest> results)
    {
      results = results ?? Enumerable.Empty<TestPointOutcomeUpdateFromTestResultRequest>();
      return this.BindTable(parameterName, "typ_UpdateTestPointOutcomeTypeTable", this.BindUpdateTestPointOutcomeTypeTableRows(results));
    }

    private IEnumerable<SqlDataRecord> BindUpdateTestPointOutcomeTypeTableRows(
      IEnumerable<TestPointOutcomeUpdateFromTestResultRequest> results)
    {
      foreach (TestPointOutcomeUpdateFromTestResultRequest result in results)
      {
        SqlDataRecord record1 = new SqlDataRecord(TestPlanningDatabase.typ_UpdateTestPointOutcomeTypeTable);
        record1.SetInt32(0, result.TestPlanId);
        record1.SetInt32(1, result.TestPointId);
        record1.SetNullableByte(2, result.FailureType);
        record1.SetInt32(3, result.TestRunId);
        record1.SetInt32(4, result.TestResultId);
        record1.SetDateTime(5, result.LastUpdated);
        record1.SetGuid(6, result.LastUpdatedBy);
        SqlDataRecord record2 = record1;
        byte? state = result.State;
        int? nullable1 = state.HasValue ? new int?((int) state.GetValueOrDefault()) : new int?();
        int num1 = 0;
        byte? nullable2 = nullable1.GetValueOrDefault() == num1 & nullable1.HasValue ? new byte?((byte) 1) : result.State;
        record2.SetNullableByte(7, nullable2);
        SqlDataRecord record3 = record1;
        byte? outcome = result.Outcome;
        nullable1 = outcome.HasValue ? new int?((int) outcome.GetValueOrDefault()) : new int?();
        int num2 = 0;
        byte? nullable3 = nullable1.GetValueOrDefault() == num2 & nullable1.HasValue ? new byte?((byte) 1) : result.Outcome;
        record3.SetNullableByte(8, nullable3);
        record1.SetInt32(9, result.ResolutionStateId < 0 ? 0 : result.ResolutionStateId);
        yield return record1;
      }
    }

    protected SqlParameter BindPointResults3Table(
      string parameterName,
      IEnumerable<PointsResults2> pointResults2)
    {
      pointResults2 = pointResults2 ?? Enumerable.Empty<PointsResults2>();
      return this.BindTable(parameterName, "typ_PointResultsTable3", this.BindPointResults3TypeTableRows(pointResults2));
    }

    private IEnumerable<SqlDataRecord> BindPointResults3TypeTableRows(
      IEnumerable<PointsResults2> pointResults2)
    {
      foreach (PointsResults2 pointsResults2 in pointResults2)
      {
        SqlDataRecord record = new SqlDataRecord(TestPlanningDatabase.typ_PointResults3Table);
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
        record.SetNullableGuid(10, pointsResults2.LastUpdatedBy);
        yield return record;
      }
    }

    internal virtual UpdatedProperties CreateBugFieldMapping(
      Guid projectGuid,
      BugFieldMapping bugFieldMapping,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_CreateBugFieldMapping");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindString("@fieldMapping", bugFieldMapping.FieldMapping, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedProperties bugFieldMapping1 = reader.Read() ? new TestPlanningDatabase.BugFieldMappingColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateBugFieldMapping");
      bugFieldMapping1.LastUpdatedBy = updatedBy;
      return bugFieldMapping1;
    }

    internal virtual UpdatedProperties UpdateBugFieldMapping(
      Guid projectGuid,
      BugFieldMapping bugFieldMapping,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_UpdateBugFieldMapping");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindString("@fieldMapping", bugFieldMapping.FieldMapping, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindInt("@revision", bugFieldMapping.Revision);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedProperties updatedProperties = reader.Read() ? new TestPlanningDatabase.BugFieldMappingColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateBugFieldMapping");
      updatedProperties.LastUpdatedBy = updatedBy;
      return updatedProperties;
    }

    internal virtual BugFieldMapping QueryBugFieldMapping(Guid projectGuid)
    {
      BugFieldMapping bugFieldMapping = (BugFieldMapping) null;
      this.PrepareStoredProcedure("prc_QueryBugFieldMapping");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
        bugFieldMapping = new TestPlanningDatabase.QueryBugFieldMappingColumns().Bind(reader);
      return bugFieldMapping;
    }

    internal virtual TestArtifactsAssociatedItemsModel QueryTestCaseAssociatedTestArtifacts(
      TestManagementRequestContext context,
      Guid projectGuid,
      int testCaseId,
      bool isTcmService)
    {
      context.TraceEnter("Database", "TestCaseDatabase.QueryTestCaseAssociatedTestArtifacts");
      string storedProcedure = "prc_QueryTestCaseAssociatedTestArtifactsV2";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@testCaseId", testCaseId);
      TestArtifactsAssociatedItemsModel associatedItemsModel = this.GetTestArtifactsAssociatedItemsModel(context, storedProcedure);
      context.TraceLeave("Database", "TestCaseDatabase.QueryTestCaseAssociatedTestArtifacts");
      return associatedItemsModel;
    }

    internal virtual List<int> GetTestCaseIds(int opId) => throw new NotImplementedException();

    internal virtual int BeginTestCaseCloneOperation(
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
      throw new NotImplementedException();
    }

    internal virtual CloneOperationInformation GetTestCaseCloneOperation(
      int opId,
      out List<Tuple<Guid, Guid, CloneOperationInformation>> projectGuidList)
    {
      throw new NotImplementedException();
    }

    internal virtual void CompleteTestCaseCloneOperation(
      TestManagementRequestContext context,
      CloneOperationInformation opInfo,
      int opId)
    {
      throw new NotImplementedException();
    }

    internal virtual CloneTestCaseOperationInformation GetTestCaseCloneOperationInfo(
      int opId,
      out List<Tuple<Guid, Guid, CloneTestCaseOperationInformation>> projectGuidList)
    {
      throw new NotImplementedException();
    }

    internal virtual List<Microsoft.TeamFoundation.TestManagement.Server.Charting.TestFieldData> GetTestExecutionReport(
      string planName,
      int suiteId,
      List<KeyValuePair<int, string>> dimensionList)
    {
      this.RequestContext.TraceEnter("Database", "ReportDatabase.GetTestExecutionReport");
      List<Microsoft.TeamFoundation.TestManagement.Server.Charting.TestFieldData> testExecutionReport = new List<Microsoft.TeamFoundation.TestManagement.Server.Charting.TestFieldData>();
      try
      {
        this.PrepareDynamicProcedure("prc_GetTestExecutionReport");
        this.BindString("@planName", planName, 512, false, SqlDbType.NVarChar);
        this.BindInt("@suiteId", suiteId);
        this.BindNameTypeTable("@dimensions", (IEnumerable<string>) dimensionList.Select<KeyValuePair<int, string>, string>((System.Func<KeyValuePair<int, string>, string>) (m => m.Value)).ToList<string>());
        SqlDataReader reader = this.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Dictionary<string, object> dimensions = new Dictionary<string, object>();
            foreach (string str in dimensionList.OrderBy<KeyValuePair<int, string>, int>((System.Func<KeyValuePair<int, string>, int>) (x => x.Key)).Select<KeyValuePair<int, string>, string>((System.Func<KeyValuePair<int, string>, string>) (x => x.Value)))
            {
              object obj = new SqlColumnBinder(str).GetObject((IDataReader) reader);
              dimensions[str] = obj;
            }
            long int64 = Convert.ToInt64(new SqlColumnBinder("AggTestsCount").GetInt32((IDataReader) reader));
            testExecutionReport.Add(new Microsoft.TeamFoundation.TestManagement.Server.Charting.TestFieldData(dimensions, int64));
          }
        }
        return testExecutionReport;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ReportDatabase.GetTestExecutionReport");
      }
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestRunSummaryReport(
      Guid projectGuid,
      int runId,
      List<string> dimensionList,
      bool getRecentRun = false)
    {
      this.RequestContext.TraceEnter("Database", "ReportDatabase.GetTestRunSummaryReport");
      Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData runSummaryReport = new Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData();
      try
      {
        this.PrepareStoredProcedure("TestResult.prc_GetTestRunSummaryReport");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@runId", runId);
        this.BindBoolean("@getRecentRun", getRecentRun);
        this.BindNameTypeTable("@dimensions", (IEnumerable<string>) dimensionList);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Dictionary<string, object> dimensionValues = new Dictionary<string, object>();
            foreach (string dimension in dimensionList)
            {
              object dimensionValue = new SqlColumnBinder(dimension).GetObject((IDataReader) reader);
              object obj = (object) Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData.MapDimensionValue(this.RequestContext, dimension, dimensionValue);
              dimensionValues[dimension] = obj;
            }
            long int64 = Convert.ToInt64(new SqlColumnBinder("AggTestsCount").GetInt32((IDataReader) reader));
            runSummaryReport.AddReportDatarow(dimensionValues, int64);
          }
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ReportDatabase.GetTestRunSummaryReport");
      }
      return runSummaryReport;
    }

    internal virtual List<Microsoft.TeamFoundation.TestManagement.Server.Charting.TestAuthoringDetails> GetTestAuthoringChartDetails(
      string planName,
      int suiteId)
    {
      return new List<Microsoft.TeamFoundation.TestManagement.Server.Charting.TestAuthoringDetails>();
    }

    internal void RegisterTestController(TestController controller)
    {
      this.PrepareStoredProcedure("prc_RegisterTestController");
      this.BindString("@name", controller.Name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", controller.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@groupId", controller.GroupId, 256, false, SqlDbType.NVarChar);
      this.BindNameValuePairTypeTable("@propertiesTable", (IEnumerable<NameValuePair>) controller.Properties);
      this.ExecuteNonQuery();
    }

    internal void UnregisterTestController(string controllerName)
    {
      this.PrepareStoredProcedure("prc_UnregisterTestController");
      this.BindString("@name", controllerName, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal void UpdateTestController(TestController controller)
    {
      this.PrepareStoredProcedure("prc_UpdateTestController");
      this.BindString("@name", controller.Name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", controller.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@groupId", controller.GroupId, 256, false, SqlDbType.NVarChar);
      this.BindNameValuePairTypeTable("@propertiesTable", (IEnumerable<NameValuePair>) controller.Properties);
      this.ExecuteNonQuery();
    }

    internal void UpdateTestControllerHeartbeatTime(string controllerName)
    {
      this.PrepareStoredProcedure("prc_UpdateTestControllerHeartbeat");
      this.BindString("@name", controllerName, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal List<TestController> QueryTestControllers(string queryValue, bool isNameValue)
    {
      this.PrepareStoredProcedure("prc_QueryTestControllers");
      this.BindString("@queryValue", queryValue, 256, true, SqlDbType.NVarChar);
      this.BindBoolean("@isNameValue", isNameValue);
      SqlDataReader reader = this.ExecuteReader();
      List<TestController> testControllerList = new List<TestController>();
      Dictionary<string, TestController> dictionary = new Dictionary<string, TestController>();
      TestPlanningDatabase.QueryTestControllerColumns controllerColumns = new TestPlanningDatabase.QueryTestControllerColumns();
      while (reader.Read())
      {
        TestController testController = controllerColumns.Bind(reader);
        testControllerList.Add(testController);
        dictionary[testController.Name] = testController;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestControllers");
      TestController testController1 = (TestController) null;
      string controllerName = (string) null;
      TestPlanningDatabase.QueryTestControllerPropertyColumns controllerPropertyColumns = new TestPlanningDatabase.QueryTestControllerPropertyColumns();
      while (reader.Read())
      {
        NameValuePair nameValuePair = controllerPropertyColumns.bind(reader, out controllerName);
        if (dictionary.TryGetValue(controllerName, out testController1))
          testController1.Properties.Add(nameValuePair);
      }
      return testControllerList;
    }

    internal void RegisterDataCollector(DataCollectorInformation collector)
    {
      this.PrepareStoredProcedure("prc_RegisterDataCollector");
      this.BindString("@typeUri", collector.TypeUri, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@id", collector.Id);
      this.BindString("@description", collector.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@defaultConfiguration", collector.DefaultConfiguration == null ? string.Empty : collector.DefaultConfiguration.OuterXml, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@configurationEditorConfiguration", collector.ConfigurationEditorConfiguration == null ? string.Empty : collector.ConfigurationEditorConfiguration.OuterXml, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindNameValuePairTypeTable("@propertiesTable", (IEnumerable<NameValuePair>) collector.Properties);
      this.ExecuteNonQuery();
    }

    internal void UnregisterDataCollector(string typeUri)
    {
      this.PrepareStoredProcedure("prc_UnregisterDataCollector");
      this.BindString("@typeUri", typeUri, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal void UpdateDataCollector(DataCollectorInformation collector)
    {
      this.PrepareStoredProcedure("prc_UpdateDataCollector");
      this.BindString("@typeUri", collector.TypeUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", collector.Description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@defaultConfiguration", TestPlanningDatabase.XmlNodeToProperty(collector.DefaultConfiguration), int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@configurationEditorConfiguration", TestPlanningDatabase.XmlNodeToProperty(collector.ConfigurationEditorConfiguration), int.MaxValue, false, SqlDbType.NVarChar);
      this.BindNameValuePairTypeTable("@propertiesTable", (IEnumerable<NameValuePair>) collector.Properties);
      this.ExecuteNonQuery();
    }

    internal List<DataCollectorInformation> QueryDataCollectors(string queryValue)
    {
      this.PrepareStoredProcedure("prc_QueryDataCollectors");
      this.BindStringPreserveNull("@queryValue", queryValue, 256, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      List<DataCollectorInformation> collectorInformationList = new List<DataCollectorInformation>();
      Dictionary<string, DataCollectorInformation> dictionary = new Dictionary<string, DataCollectorInformation>();
      TestPlanningDatabase.QueryDataCollectorColumns collectorColumns = new TestPlanningDatabase.QueryDataCollectorColumns();
      while (reader.Read())
      {
        DataCollectorInformation collectorInformation = collectorColumns.Bind(reader);
        collectorInformationList.Add(collectorInformation);
        dictionary[collectorInformation.TypeUri] = collectorInformation;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryDataCollectors");
      DataCollectorInformation collectorInformation1 = (DataCollectorInformation) null;
      string typeUri = (string) null;
      TestPlanningDatabase.QueryDataCollectorPropertyColumns collectorPropertyColumns = new TestPlanningDatabase.QueryDataCollectorPropertyColumns();
      while (reader.Read())
      {
        NameValuePair nameValuePair = collectorPropertyColumns.bind(reader, out typeUri);
        if (dictionary.TryGetValue(typeUri, out collectorInformation1))
          collectorInformation1.Properties.Add(nameValuePair);
      }
      return collectorInformationList;
    }

    private static string XmlNodeToProperty(XmlNode node) => node != null ? node.OuterXml : string.Empty;

    public virtual IEnumerable<int> GetTestPointdIdsForOutcomeSync(int planId, int[] testPointIds) => (IEnumerable<int>) new List<int>();

    public virtual void UpdateTestPointOutcome(
      Guid projectGuid,
      IList<TestPointOutcomeUpdateFromTestResultRequest> results)
    {
    }

    public virtual void SyncPointResultToPoint(List<PointsResults2> pointResults)
    {
    }

    public virtual DateTime GetLeastMigrationStartDate() => throw new NotImplementedException();

    internal virtual bool CleanDeletedTestPoints(int waitDaysForCleanup, int deletionBatchSize) => true;

    internal virtual IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry> GetSuiteEntries(
      Guid projectId,
      int suiteId)
    {
      return (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry>();
    }

    internal virtual IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry> ReorderSuiteEntries(
      Guid projectId,
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      return (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry>();
    }

    static TestPlanningDatabase()
    {
      TestPlanningDatabase.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      TestPlanningDatabase.RegisterException(550002, typeof (TestObjectNotFoundException));
      TestPlanningDatabase.RegisterException(550017, typeof (TestObjectNotFoundException));
      TestPlanningDatabase.RegisterException(550003, typeof (TestObjectInUseException));
      TestPlanningDatabase.RegisterException(550004, typeof (TestObjectUpdatedException));
      TestPlanningDatabase.RegisterException(550005, typeof (TestManagementInvalidOperationException));
      TestPlanningDatabase.RegisterException(550008, typeof (TestManagementInvalidOperationException));
      TestPlanningDatabase.RegisterException(550006, typeof (TestSuiteInvalidOperationException));
      TestPlanningDatabase.RegisterException(550007, typeof (TestSuiteInvalidOperationException));
      TestPlanningDatabase.RegisterException(550009, typeof (TestManagementValidationException));
      TestPlanningDatabase.RegisterException(550010, typeof (TestSuiteInvalidOperationException));
      TestPlanningDatabase.RegisterException(550011, typeof (TestSuiteInvalidOperationException));
      TestPlanningDatabase.RegisterException(550012, typeof (TestManagementInvalidOperationException));
      TestPlanningDatabase.RegisterException(550013, typeof (TestManagementValidationException));
      TestPlanningDatabase.RegisterException(550021, typeof (TestManagementConflictingOperation));
      TestPlanningDatabase.RegisterException(550025, typeof (TestManagementInvalidOperationException));
      TestPlanningDatabase.RegisterException(550029, typeof (TestSuiteInvalidOperationException));
      TestPlanningDatabase.RegisterException(550018, typeof (TestManagementValidationException));
      TestPlanningDatabase.RegisterException(550019, typeof (TestManagementValidationException));
      TestPlanningDatabase.RegisterException(550026, typeof (TestManagementInvalidOperationException));
      TestPlanningDatabase.RegisterException(550030, typeof (TestManagementInvalidOperationException));
      TestPlanningDatabase.RegisterException(550027, typeof (TestManagementInvalidOperationException));
      TestPlanningDatabase.InitializeTestManagementDynamicSprocsMap();
    }

    public TestPlanningDatabase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal TestPlanningDatabase(string connectionString, int partitionId)
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
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap = new Dictionary<string, string>(13);
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QuerySessions2"] = TestPlanningDynamicSqlBatchStatements.dynprc_QuerySessions2;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QuerySuites"] = TestPlanningDynamicSqlBatchStatements.dynprc_QuerySuites;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryPoints"] = TestPlanningDynamicSqlBatchStatements.dynprc_QueryPoints;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryPoints2"] = TestPlanningDynamicSqlBatchStatements.dynprc_QueryPoints2;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryPoints3"] = TestPlanningDynamicSqlBatchStatements.dynprc_QueryPoints3;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryPointsWithLastResults"] = TestPlanningDynamicSqlBatchStatements.dynprc_QueryPointsWithLastResults;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryPointsWithLastResults2"] = TestPlanningDynamicSqlBatchStatements.dynprc_QueryPointsWithLastResults2;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QuerySuitePointCounts"] = TestPlanningDynamicSqlBatchStatements.dynprc_QuerySuitePointCounts;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryTestPointStatistics"] = TestPlanningDynamicSqlBatchStatements.dynprc_QueryTestPointStatistics;
      TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap["prc_QueryTestPointStatisticsByPivot"] = TestPlanningDynamicSqlBatchStatements.dynprc_QueryTestPointStatisticsByPivot;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TestPlanningDatabase.s_sqlExceptionFactories;

    public static TestPlanningDatabase Create(TestManagementRequestContext requestContext) => TestPlanningDatabase.Create(requestContext.RequestContext);

    public static TestPlanningDatabase Create(IVssRequestContext requestContext) => requestContext.CreateComponent<TestPlanningDatabase>("TestManagement");

    public static TestPlanningDatabase CreateReadReplicaAwareComponent(
      IVssRequestContext requestContext)
    {
      DatabaseConnectionType databaseConnectionType = requestContext.RouteThroughReadReplica() ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default;
      return requestContext.CreateComponent<TestPlanningDatabase>("TestManagement", new DatabaseConnectionType?(databaseConnectionType));
    }

    private static void RegisterException(int sqlErrorCode, Type exceptionType) => TestPlanningDatabase.s_sqlExceptionFactories.Add(sqlErrorCode, new SqlExceptionFactory(exceptionType));

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

    internal virtual TcmCommonStructureNodeInfo GetRootArea(string projectName)
    {
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(this.RequestContext);
      return managementRequestContext.CSSHelper.GetRootNodes(managementRequestContext.ProjectServiceHelper.GetProjectUri(projectName)).Find((Predicate<TcmCommonStructureNodeInfo>) (r => TFStringComparer.StructureType.Equals("ProjectModelHierarchy", r.StructureType)));
    }

    internal virtual string EscapeQuotes(string str) => str.Replace("'", "''");

    public override void Dispose()
    {
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    internal virtual void PrepareDynamicProcedure(string sprocName)
    {
      string sqlBatchStatements = TestPlanningDatabase.s_TcmDynamicSqlBatchStatementsMap[sprocName];
      this.PrepareSqlBatch(sqlBatchStatements.Length, true);
      this.AddStatement(sqlBatchStatements, 0, true, true);
    }

    internal static long GetDurationFromStartAndCompleteDates(
      DateTime startDate,
      DateTime completeDate)
    {
      return !startDate.Equals(new DateTime()) && !completeDate.Equals(new DateTime()) ? (completeDate - startDate).Ticks : 0L;
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

    internal virtual List<ImpactedPoint> QueryImpactedPointsForPlan(
      Guid projectGuid,
      int planId,
      string planName,
      string buildDefinitionUri,
      DateTime buildStartDate)
    {
      this.PrepareStoredProcedure("prc_QueryImpactedPointsForPlan");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@planId", planId);
      this.BindTestPlanName(planName);
      this.BindString("@buildDefinitionUri", buildDefinitionUri, 256, false, SqlDbType.NVarChar);
      this.BindDateTime("@buildStartDate", buildStartDate);
      SqlDataReader reader = this.ExecuteReader();
      List<ImpactedPoint> impactedPointList = new List<ImpactedPoint>();
      TestPlanningDatabase.ImpactedPointsColumns impactedPointsColumns = new TestPlanningDatabase.ImpactedPointsColumns();
      while (reader.Read())
        impactedPointList.Add(impactedPointsColumns.Bind(reader));
      return impactedPointList;
    }

    protected virtual void BindTestPlanName(string planName) => this.BindString("@planName", planName, 256, false, SqlDbType.NVarChar);

    internal void CreateTestBuild(
      string buildUri,
      string buildDefinitionUri,
      string projectUri,
      DateTime startTime)
    {
      this.PrepareStoredProcedure("prc_CreateTestBuild");
      this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@buildDefinitionUri", buildDefinitionUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      this.BindDateTime("@startTime", startTime);
      this.BindInt("@buildId", 0);
      this.ExecuteNonQuery();
    }

    internal virtual BlockedPointProperties BlockTestPoint2(
      Guid projectGuid,
      TestPoint point,
      TestCaseResult result,
      Guid updatedBy)
    {
      return new BlockedPointProperties();
    }

    internal virtual Dictionary<int, TestPoint> QuerySuitePoints(
      int planId,
      List<byte> pointStates,
      List<byte> pointOutcomes,
      List<Guid> assignedTesters,
      List<int> configurationIds,
      int minPointId,
      int batchSize,
      out int maxPointId)
    {
      maxPointId = 0;
      return new Dictionary<int, TestPoint>();
    }

    internal virtual Dictionary<int, byte> QueryTestCasesInPlans(Guid projectGuid)
    {
      Dictionary<int, byte> dictionary = new Dictionary<int, byte>();
      this.PrepareStoredProcedure("prc_QueryTestCasesInPlans");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.QueryTestCasesInPlansColumns casesInPlansColumns = new TestPlanningDatabase.QueryTestCasesInPlansColumns();
      while (reader.Read())
        dictionary[casesInPlansColumns.TestCaseId.GetInt32((IDataReader) reader)] = (byte) 0;
      return dictionary;
    }

    internal virtual TestPlan UpdateTestPlan(
      TestManagementRequestContext context,
      Guid projectGuid,
      string auditUser,
      TestPlan plan,
      Guid updatedBy,
      TestExternalLink[] links,
      string oldTitle,
      int suiteRevision)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.UpdateTestPlan");
        Validator.CheckStartEndDatesInOrder(plan.StartDate, plan.EndDate);
        this.PrepareStoredProcedure("prc_UpdatePlan");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", plan.PlanId);
        this.BindStringPreserveNull("@oldName", oldTitle, 256, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@name", plan.Name, 256, SqlDbType.NVarChar);
        this.BindByte("@planState", plan.State);
        this.BindStringPreserveNull("@buildUri", plan.BuildUri, 64, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildDefinition", plan.BuildDefinition, 260, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildQuality", plan.BuildQuality, 256, SqlDbType.NVarChar);
        this.BindInt("@manualTestSettingsId", plan.TestSettingsId);
        this.BindInt("@automatedTestSettingsId", plan.AutomatedTestSettingsId);
        this.BindGuid("@manualTestEnvironmentId", plan.ManualTestEnvironmentId);
        this.BindGuid("@automatedTestEnvironmentId", plan.AutomatedTestEnvironmentId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindDateTime("@lastUpdated", plan.LastUpdated);
        this.BindInt("@suiteRevision", suiteRevision);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_UpdatePlan");
        TestPlanningDatabase.UpdateTestPlansColumns testPlansColumns = new TestPlanningDatabase.UpdateTestPlansColumns();
        plan.PreviousBuildUri = testPlansColumns.PreviousBuildUri.GetString((IDataReader) reader, true);
        plan.BuildTakenDate = testPlansColumns.BuildTakenDate.GetDateTime((IDataReader) reader);
        plan.LastUpdatedBy = updatedBy;
        return plan;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.UpdateTestPlan");
      }
    }

    internal virtual UpdatedProperties UpdateTestPoint(
      Guid projectGuid,
      TestPoint point,
      Guid updatedBy,
      bool updateRunResultsInTCM,
      bool considerUnassignedTesters = false,
      bool ResetToActive = false)
    {
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_UpdateTestPoint");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", point.PlanId);
        this.BindInt("@pointId", point.PointId);
        this.BindStringPreserveNull("@comment", point.Comment, 1048576, SqlDbType.NVarChar);
        if (considerUnassignedTesters)
          this.BindGuid("@assignedTo", point.AssignedTo);
        else
          this.BindGuidPreserveNull("@assignedTo", point.AssignedTo);
        this.BindInt("@revision", point.Revision);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@state", point.State, (byte) 0);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestPlanningDatabase.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestPoint");
        updatedProperties.LastUpdatedBy = updatedBy;
        return updatedProperties;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual BlockedPointProperties BlockTestPoint(
      Guid projectGuid,
      TestPoint point,
      TestCaseResult result,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("TestManagement.prc_BlockTestPoint");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@pointId", point.PointId);
      this.BindInt("@planId", point.PlanId);
      this.BindInt("@testRunId", result.TestRunId);
      this.BindInt("@testCaseId", result.TestCaseId);
      this.BindByte("@priority", result.Priority);
      this.BindString("@testCaseTitle", result.TestCaseTitle, 256, true, SqlDbType.NVarChar);
      this.BindString("@testCaseArea", result.TestCaseAreaUri, 256, true, SqlDbType.NVarChar);
      this.BindInt("@testCaseRevision", result.TestCaseRevision);
      this.BindString("@automatedTestName", result.AutomatedTestName, 256, false, SqlDbType.NVarChar);
      this.BindString("@automatedTestStorage", result.AutomatedTestStorage, 256, false, SqlDbType.NVarChar);
      this.BindString("@automatedTestType", result.AutomatedTestType, 256, false, SqlDbType.NVarChar);
      this.BindString("@automatedTestId", result.AutomatedTestId, 64, false, SqlDbType.NVarChar);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindInt("@revision", point.Revision);
      SqlDataReader reader = this.ExecuteReader();
      BlockedPointProperties blockedPointProperties = reader.Read() ? new TestPlanningDatabase.UpdatedPropertyColumns().bindBlockedTestPointProperties(reader) : throw new UnexpectedDatabaseResultException("prc_BlockTestPoint");
      blockedPointProperties.LastTestRunId = result.TestRunId;
      blockedPointProperties.LastUpdatedBy = updatedBy;
      return blockedPointProperties;
    }

    internal virtual List<TestPoint> QueryTestPoints(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryTestPoints"))
      {
        this.PrepareDynamicProcedure("prc_QueryPoints");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        SqlDataReader reader = this.ExecuteReader();
        List<TestPoint> testPointList = new List<TestPoint>();
        TestPlanningDatabase.QueryTestPointColumns testPointColumns = new TestPlanningDatabase.QueryTestPointColumns();
        while (reader.Read())
          testPointList.Add(testPointColumns.bind(reader));
        return testPointList;
      }
    }

    internal List<TestPointStatistic> QueryTestPointStatistics(
      string whereClause,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryTestPointStatistics"))
      {
        this.PrepareDynamicProcedure("prc_QueryTestPointStatistics");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        SqlDataReader reader = this.ExecuteReader();
        List<TestPointStatistic> testPointStatisticList = new List<TestPointStatistic>();
        TestPlanningDatabase.TestPointStatisticColumns statisticColumns = new TestPlanningDatabase.TestPointStatisticColumns();
        while (reader.Read())
          testPointStatisticList.Add(statisticColumns.Bind(reader));
        return testPointStatisticList;
      }
    }

    internal virtual List<TestPoint> QueryTestPointsWithLastResults(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      return this.QueryTestPoints(whereClause, orderBy, displayNameInGroupList);
    }

    internal List<TestPointStatisticPivotItem> QueryTestPointStatisticsPerPivot(
      string whereClause,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      TestPointStatisticsQueryPivotType pivotType)
    {
      this.PrepareDynamicProcedure("prc_QueryTestPointStatisticsByPivot");
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      this.BindString("@pivot", this.FindSqlAttributeForPivot(pivotType), int.MaxValue, false, SqlDbType.NVarChar);
      List<TestPointStatisticPivotItem> statisticPivotItemList = new List<TestPointStatisticPivotItem>();
      SqlDataReader reader = this.ExecuteReader();
      if (pivotType == TestPointStatisticsQueryPivotType.Suite)
        return this.ConvertToListOfTestPointStatisticsPivotITem(new TestPlanningDatabase.TestPointStatisticColumnsBySuiteId().BindByPivot(reader));
      return pivotType == TestPointStatisticsQueryPivotType.Tester ? this.ConvertToListOfTestPointStatisticsPivotITem(new TestPlanningDatabase.TestPointStatisticColumnsByTester().BindByPivot(reader)) : (List<TestPointStatisticPivotItem>) null;
    }

    private List<TestPointStatisticPivotItem> ConvertToListOfTestPointStatisticsPivotITem(
      Dictionary<string, List<TestPointStatistic>> statistics)
    {
      List<TestPointStatisticPivotItem> statisticsPivotItem = new List<TestPointStatisticPivotItem>();
      if (statistics != null)
      {
        foreach (KeyValuePair<string, List<TestPointStatistic>> statistic in statistics)
          statisticsPivotItem.Add(new TestPointStatisticPivotItem()
          {
            Pivot = statistic.Key,
            Statistics = statistic.Value
          });
      }
      return statisticsPivotItem;
    }

    private string FindSqlAttributeForPivot(TestPointStatisticsQueryPivotType pivotType)
    {
      if (pivotType == TestPointStatisticsQueryPivotType.Suite)
        return "SuiteId";
      if (pivotType == TestPointStatisticsQueryPivotType.Tester)
        return "AssignedTo";
      throw new ArgumentOutOfRangeException(nameof (pivotType));
    }

    internal virtual List<int> FetchTestCaseIds(Guid projectId, int suiteId)
    {
      this.PrepareStoredProcedure("prc_FetchTestCasesFromSuiteRecursive");
      this.BindInt("@suiteId", suiteId);
      List<int> intList = new List<int>();
      SqlDataReader sqlDataReader = this.ExecuteReader();
      while (sqlDataReader.Read())
        intList.Add(sqlDataReader.GetInt32(0));
      return intList;
    }

    internal virtual TestArtifactsAssociatedItemsModel QueryTestPlanAssociatedTestArtifacts(
      TestManagementRequestContext context,
      Guid projectGuid,
      int testPlanId,
      bool isTcmService,
      int pointQueryLimit)
    {
      context.TraceEnter("Database", "TestPlanDatabase.QueryTestPlanAssociatedTestArtifacts");
      this.PrepareStoredProcedure("prc_QueryTestPlanAssociatedTestArtifacts");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@testPlanId", testPlanId);
      TestArtifactsAssociatedItemsModel associatedItemsModel = this.GetTestArtifactsAssociatedItemsModel(context, "prc_QueryTestPlanAssociatedTestArtifacts");
      context.TraceLeave("Database", "TestPlanDatabase.QueryTestPlanAssociatedTestArtifacts");
      return associatedItemsModel;
    }

    protected TestArtifactsAssociatedItemsModel GetTestArtifactsAssociatedItemsModel(
      TestManagementRequestContext context,
      string storedProcedure)
    {
      TestArtifactsAssociatedItemsModel associatedItemsModel = new TestArtifactsAssociatedItemsModel();
      try
      {
        SqlDataReader reader = this.ExecuteReader();
        TestPlanningDatabase.TestArtifactsAssociatedItems artifactsAssociatedItems = new TestPlanningDatabase.TestArtifactsAssociatedItems();
        if (!reader.Read())
          return associatedItemsModel;
        associatedItemsModel.SuitesCount = artifactsAssociatedItems.SuitesCount.GetInt32((IDataReader) reader);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
        if (reader.Read())
        {
          int int32 = artifactsAssociatedItems.PointCount.GetInt32((IDataReader) reader, -1, -1);
          if (int32 == -1)
            int32 = artifactsAssociatedItems.PointHistoryCount.GetInt32((IDataReader) reader, -1, -1);
          associatedItemsModel.PointCount = int32;
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(storedProcedure);
        if (reader.Read())
          associatedItemsModel.TestResultsCount = artifactsAssociatedItems.TestResultsCount.GetInt32((IDataReader) reader);
        if (reader.NextResult())
        {
          if (reader.Read())
            associatedItemsModel.TestPlanId = new int?(artifactsAssociatedItems.TestPlanId.GetInt32((IDataReader) reader));
        }
      }
      catch (TestObjectNotFoundException ex)
      {
        context.TraceWarning("Database", string.Format("No associated artifacts found for the given test artifact, exception: {0}", (object) ex.Message));
      }
      return associatedItemsModel;
    }

    internal virtual List<TestPoint> FetchTestPoints(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds)
    {
      return this.FetchTestPoints(projectGuid, planId, idsToFetch, deletedIds, false);
    }

    protected virtual List<TestPoint> FetchTestPoints(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds,
      bool includeSuiteName)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.FetchTestPoints"))
      {
        this.PrepareStoredProcedure("prc_FetchTestPoints");
        this.BindIdAndRevTypeTable("@idsTable", (IEnumerable<IdAndRev>) idsToFetch);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", planId);
        List<TestPoint> testPointList = new List<TestPoint>();
        TestPlanningDatabase.FetchTestPointsColumns testPointsColumns = new TestPlanningDatabase.FetchTestPointsColumns();
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
          testPointList.Add(testPointsColumns.bind(reader, includeSuiteName));
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_FetchTestPoints");
        new TestPlanningDatabase.IdsPropertyColumns().bind(reader, deletedIds);
        return testPointList;
      }
    }

    internal virtual TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints)
    {
      return (TestPlanHubData) null;
    }

    internal virtual TestPlanHubData FetchTestPlanHubData(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool includeTestPoints,
      int configuration,
      Guid tester)
    {
      return (TestPlanHubData) null;
    }

    internal virtual List<TestPoint> FetchTestPointsWithLastResults(
      Guid projectGuid,
      int planId,
      IdAndRev[] idsToFetch,
      List<int> deletedIds)
    {
      return this.FetchTestPoints(projectGuid, planId, idsToFetch, deletedIds);
    }

    internal virtual List<TestPoint> QueryTestPointHistory(int testPointId, Guid projectGuid)
    {
      List<TestPoint> testPointList = new List<TestPoint>();
      this.PrepareStoredProcedure("TestManagement.prc_QueryPointHistory");
      this.BindInt("@testPointId", testPointId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.QueryTestPointHistoryColumns pointHistoryColumns = new TestPlanningDatabase.QueryTestPointHistoryColumns();
      if (reader.HasRows)
      {
        while (reader.Read())
          testPointList.Add(pointHistoryColumns.bind(reader));
      }
      return testPointList;
    }

    internal virtual List<Guid> QueryTesters(Guid projectGuid, int planId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryTesters"))
      {
        this.PrepareStoredProcedure("prc_QueryTesters");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", planId);
        List<Guid> guidList = new List<Guid>();
        SqlDataReader reader = this.ExecuteReader();
        TestPlanningDatabase.FetchTester fetchTester = new TestPlanningDatabase.FetchTester();
        while (reader.Read())
          guidList.Add(fetchTester.bind(reader));
        return guidList;
      }
    }

    internal virtual bool? IsSuiteOrderMigratedForPlan(Guid projectGuid, int planId) => new bool?();

    internal virtual Dictionary<int, string> QueryConfigurations(Guid projectGuid, int planId) => new Dictionary<int, string>();

    internal virtual List<int> FetchValidTestPlanIds(
      TestManagementRequestContext context,
      List<int> witPlanIds,
      Guid projectGuid,
      bool excludeOrphanPlans)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "Database", "TestPlanDataBase.FetchValidTestPlanIds"))
        {
          context.TraceEnter("Database", "TestPlanDatabase.FetchValidTestPlanIds");
          if (witPlanIds.Count == 0)
            return new List<int>();
          this.PrepareStoredProcedure("prc_FetchValidTestPlanIds");
          this.BindIdTypeTable("@idsTable", (IEnumerable<int>) witPlanIds);
          this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
          this.BindBoolean("@excludeOrphanPlans", excludeOrphanPlans);
          HashSet<int> intSet = new HashSet<int>();
          SqlDataReader reader = this.ExecuteReader();
          SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("PlanId");
          while (reader.Read())
          {
            int int32 = sqlColumnBinder.GetInt32((IDataReader) reader);
            intSet.Add(int32);
          }
          List<int> intList = new List<int>();
          foreach (int witPlanId in witPlanIds)
          {
            if (intSet.Contains(witPlanId))
              intList.Add(witPlanId);
          }
          return intList;
        }
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.FetchValidTestPlanIds");
      }
    }

    internal virtual List<TestPlan> FetchTestPlans(
      TestManagementRequestContext context,
      List<int> deletedIds,
      Guid projectGuid,
      bool fetchSuitesMetaData,
      IdAndRev[] idsToFetch,
      List<TestPlan> witPlans,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap,
      bool excludePlansWithNoRootSuite = true)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "Database", "TestPlanDataBase.FetchTestPlans"))
        {
          context.TraceEnter("Database", "TestPlanDatabase.FetchTestPlans");
          this.PrepareStoredProcedure("prc_FetchTestPlans");
          List<int> ids = new List<int>(witPlans.Select<TestPlan, int>((System.Func<TestPlan, int>) (p => p.PlanId)));
          this.BindIdTypeTable("@idsTable", (IEnumerable<int>) ids);
          this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
          this.BindBoolean("@fetchSuitesMetaData", fetchSuitesMetaData);
          Dictionary<int, TestPlan> dictionary = new Dictionary<int, TestPlan>(witPlans.Count);
          Dictionary<int, TestPlan> workItemDictionary = this.GetPlanWorkItemDictionary(witPlans);
          List<ServerTestSuite> source = (List<ServerTestSuite>) null;
          SqlDataReader reader = this.ExecuteReader();
          TestPlanningDatabase.FetchTestPlansColumns testPlansColumns = new TestPlanningDatabase.FetchTestPlansColumns();
          projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
          while (reader.Read())
          {
            TestPlan tcmPlan = testPlansColumns.bind(reader);
            TestPlan witPlan;
            if (workItemDictionary.TryGetValue(tcmPlan.PlanId, out witPlan))
            {
              TestPlanningDatabase.PopulatePropertiesFromPlanWorkItem(tcmPlan, witPlan);
              dictionary.Add(tcmPlan.PlanId, tcmPlan);
            }
          }
          if (fetchSuitesMetaData)
            source = reader.NextResult() ? this.ReadSuiteMetaData(context, "prc_FetchTestPlans", reader, false, out projectsSuitesMap) : throw new UnexpectedDatabaseResultException("prc_FetchTestPlans");
          List<TestPlan> testPlanList = new List<TestPlan>();
          foreach (int key in ids)
          {
            TestPlan plan;
            if (dictionary.TryGetValue(key, out plan))
            {
              if (fetchSuitesMetaData)
                plan.SuitesMetaData = source.Where<ServerTestSuite>((System.Func<ServerTestSuite, bool>) (s => s.PlanId == plan.PlanId)).ToList<ServerTestSuite>();
              testPlanList.Add(plan);
            }
            else if (!excludePlansWithNoRootSuite)
            {
              TestPlan tcmPlan = new TestPlan();
              TestPlan witPlan;
              if (workItemDictionary.TryGetValue(key, out witPlan))
              {
                tcmPlan.PlanId = key;
                TestPlanningDatabase.PopulatePropertiesFromPlanWorkItem(tcmPlan, witPlan);
                testPlanList.Add(tcmPlan);
              }
            }
          }
          return testPlanList;
        }
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.FetchTestPlans");
      }
    }

    protected Dictionary<int, TestPlan> GetPlanWorkItemDictionary(List<TestPlan> witPlans)
    {
      Dictionary<int, TestPlan> workItemDictionary = new Dictionary<int, TestPlan>();
      foreach (TestPlan witPlan in witPlans)
      {
        if (!workItemDictionary.ContainsKey(witPlan.PlanId))
          workItemDictionary[witPlan.PlanId] = witPlan;
      }
      return workItemDictionary;
    }

    protected static void PopulatePropertiesFromPlanWorkItem(TestPlan tcmPlan, TestPlan witPlan)
    {
      tcmPlan.Name = witPlan.Name;
      tcmPlan.Owner = witPlan.Owner;
      tcmPlan.OwnerName = witPlan.OwnerName;
      tcmPlan.StartDate = witPlan.StartDate;
      tcmPlan.EndDate = witPlan.EndDate;
      tcmPlan.Description = witPlan.Description;
      tcmPlan.EncodedDescription = witPlan.EncodedDescription;
      tcmPlan.Revision = witPlan.Revision;
      tcmPlan.LastUpdated = witPlan.LastUpdated;
      tcmPlan.LastUpdatedBy = witPlan.LastUpdatedBy;
      tcmPlan.LastUpdatedByName = witPlan.LastUpdatedByName;
      tcmPlan.AreaPath = witPlan.AreaPath;
      tcmPlan.AreaUri = witPlan.AreaUri;
      tcmPlan.Iteration = witPlan.Iteration;
      tcmPlan.State = witPlan.State;
      tcmPlan.Status = witPlan.Status;
    }

    internal Dictionary<int, SuitePointCount> QuerySuitePointCounts(
      TestManagementRequestContext context,
      string whereClause,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      using (PerfManager.Measure(context.RequestContext, "Database", "TestPlanDatabase.QuerySuitePointCounts"))
      {
        this.PrepareDynamicProcedure("prc_QuerySuitePointCounts");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, SuitePointCount> dictionary = new Dictionary<int, SuitePointCount>();
        byte[] buffer = new byte[256];
        while (reader.Read())
        {
          SuitePointCount suitePointCount = (SuitePointCount) null;
          TestPlanningDatabase.QuerySuitePointCountsColumns pointCountsColumns = new TestPlanningDatabase.QuerySuitePointCountsColumns();
          int bytes = pointCountsColumns.SuitePath.GetBytes((IDataReader) reader, 0L, buffer, 0, buffer.Length);
          for (int index = 0; index < bytes; index += 4)
          {
            int key = (int) buffer[index] << 24 | (int) buffer[index + 1] << 16 | (int) buffer[index + 2] << 8 | (int) buffer[index + 3];
            context.TraceAndDebugAssert("Database", key > 0, "Invalid suite ID");
            if (!dictionary.TryGetValue(key, out suitePointCount))
            {
              suitePointCount = new SuitePointCount();
              suitePointCount.SuiteId = key;
              dictionary.Add(key, suitePointCount);
            }
          }
          suitePointCount.PointCount = pointCountsColumns.PointCount.GetInt32((IDataReader) reader);
        }
        return dictionary;
      }
    }

    internal virtual Dictionary<int, SuitePointCount> QuerySuitePointCounts2(
      TestManagementRequestContext context,
      Guid projectGuid,
      int planId,
      List<string> suiteStates,
      List<byte> pointStates,
      List<byte> pointOutcomes,
      List<Guid> assignedTesters,
      List<int> configurationIds)
    {
      return new Dictionary<int, SuitePointCount>();
    }

    internal virtual List<int> FetchPlanIdsContainingCloneHistory(
      IVssRequestContext context,
      Guid projectGuid,
      List<int> planIds,
      bool fetchAllPlans)
    {
      this.PrepareStoredProcedure("prc_FetchPlanIdsContainingCloneHistory");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindIdTypeTable("@planIdsTable", (IEnumerable<int>) planIds.ToArray<int>());
      this.BindBoolean("@fetchAllPlans", fetchAllPlans);
      List<int> intList = new List<int>();
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.FetchCloneInformationColumns informationColumns = new TestPlanningDatabase.FetchCloneInformationColumns();
      TestPlanningDatabase.PlanIdColumn planIdColumn = new TestPlanningDatabase.PlanIdColumn();
      while (reader.Read())
        intList.Add(planIdColumn.PlanId.GetInt32((IDataReader) reader));
      return intList;
    }

    internal virtual List<CloneOperationInformation> FetchCloneInformationForTestPlans(
      IVssRequestContext context,
      Guid projectGuid,
      int planId,
      out List<Tuple<Guid, Guid, int, CloneOperationInformation>> projectsSuiteIdsList)
    {
      int dataspaceId = this.GetDataspaceId(projectGuid);
      this.PrepareStoredProcedure("prc_FetchCloneInformationForTestPlans");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@planId", planId);
      List<CloneOperationInformation> operationInformationList = new List<CloneOperationInformation>();
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.FetchCloneInformationColumns informationColumns = new TestPlanningDatabase.FetchCloneInformationColumns();
      projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneOperationInformation>>();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("SourceSuiteParentSuiteId");
      while (reader.Read())
      {
        int sourceDataspaceId;
        CloneOperationInformation operationInformation = informationColumns.bind(context, reader, out sourceDataspaceId);
        Guid dataspaceIdentifier = this.GetDataspaceIdentifier(sourceDataspaceId);
        int int32 = sqlColumnBinder.GetInt32((IDataReader) reader, -1);
        projectsSuiteIdsList.Add(new Tuple<Guid, Guid, int, CloneOperationInformation>(dataspaceIdentifier, projectGuid, int32, operationInformation));
        operationInformationList.Add(operationInformation);
      }
      return operationInformationList;
    }

    internal virtual TestPlan CreateTestPlanInternal(
      TestManagementRequestContext context,
      Guid projectGuid,
      TestPlan plan,
      Guid updatedBy,
      TestExternalLink[] links,
      TestPlanSource type)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.CreateTestPlanInternal");
        Validator.CheckStartEndDatesInOrder(plan.StartDate, plan.EndDate);
        this.PrepareStoredProcedure("prc_CreatePlan");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", plan.PlanId);
        this.BindByte("@type", (byte) type);
        this.BindByte("@planState", plan.State);
        this.BindString("@buildUri", plan.BuildUri, 64, false, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildDefinition", plan.BuildDefinition, 260, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildQuality", plan.BuildQuality, 256, SqlDbType.NVarChar);
        this.BindInt("@testSettingsId", plan.TestSettingsId);
        this.BindInt("@automatedTestSettingsId", plan.AutomatedTestSettingsId);
        this.BindGuid("@manualTestEnvironmentId", plan.ManualTestEnvironmentId);
        this.BindGuid("@automatedTestEnvironmentId", plan.AutomatedTestEnvironmentId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@rootSuiteId", plan.RootSuiteId);
        this.BindString("@rootSuiteStatus", plan.RootSuiteStatus, 256, false, SqlDbType.NVarChar);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_CreatePlan");
        TestPlanningDatabase.CreateTestPlanColumns createTestPlanColumns = new TestPlanningDatabase.CreateTestPlanColumns();
        plan.RootSuiteId = createTestPlanColumns.RootSuiteId.GetInt32((IDataReader) reader);
        TestPlanningDatabase.UpdateTestPlansColumns testPlansColumns = new TestPlanningDatabase.UpdateTestPlansColumns();
        plan.BuildTakenDate = testPlansColumns.BuildTakenDate.GetDateTime((IDataReader) reader);
        plan.LastUpdatedBy = updatedBy;
        return plan;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.CreateTestPlanInternal");
      }
    }

    internal virtual TestPlan CreateTestPlan(
      TestManagementRequestContext context,
      Guid projectGuid,
      TestPlan plan,
      Guid updatedBy,
      TestExternalLink[] links,
      TestPlanSource type)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.CreateTestPlan");
        return this.CreateTestPlanInternal(context, projectGuid, plan, updatedBy, links, type);
      }
      catch (TestObjectNotFoundException ex)
      {
        if (ex.ObjectType == ObjectTypes.TestConfiguration)
        {
          this.CreateDefaultTestConfiguration(context, projectGuid);
          return this.CreateTestPlanInternal(context, projectGuid, plan, updatedBy, links, type);
        }
        throw;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.CreateTestPlan");
      }
    }

    internal virtual TestConfiguration CreateDefaultTestConfiguration(
      TestManagementRequestContext context,
      Guid projectGuid,
      string configurationTitle = null)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.CreateDefaultTestConfiguration");
        string nameFromProjectGuid = Validator.GetProjectNameFromProjectGuid(context.RequestContext, projectGuid);
        return new TestConfiguration()
        {
          Name = (string.IsNullOrEmpty(configurationTitle) ? string.Format(ServerResources.DefaultConfigurationCreated, (object) DateTime.UtcNow) : configurationTitle),
          IsDefault = true,
          State = (byte) 1,
          AreaPath = this.GetRootArea(nameFromProjectGuid).Path
        }.Create(context, nameFromProjectGuid);
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.CreateDefaultTestConfiguration");
      }
    }

    internal virtual void QueueDeleteTestPlan(
      Guid projectGuid,
      int testPlanId,
      bool canDeleteResults,
      Guid updatedBy)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.QueueDeleteTestPlan");
        int dataspaceId = projectGuid.Equals(Guid.Empty) ? 0 : this.GetDataspaceId(projectGuid);
        this.PrepareStoredProcedure("prc_QueueDeleteTestPlan");
        this.BindInt("@dataspaceId", dataspaceId);
        this.BindInt("@testPlanId", testPlanId);
        this.BindBoolean("@canDeleteResults", canDeleteResults);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.QueueDeleteTestPlan");
      }
    }

    internal virtual bool CleanDeletedTestPlans()
    {
      this.PrepareStoredProcedure("prc_DeleteTestPlan");
      return (int) this.ExecuteScalar() == 0;
    }

    internal virtual string QueryPlanAreaUriBySuite(int projectId, int suiteId) => throw new NotImplementedException();

    internal void RemoveDeletedEnvironmentIds(IEnumerable<Guid> environmentIds)
    {
      this.PrepareStoredProcedure("prc_RemoveDeletedEnvironments");
      this.BindGuidTable("idsTable", environmentIds);
      this.ExecuteNonQuery();
    }

    internal virtual List<int> FetchDeletedTestPlanIds(Guid projectGuid, List<int> idsToFetch) => new List<int>();

    internal virtual List<TestPoint> GetPointsByQuery(
      Guid projectGuid,
      int[] testCaseIds,
      string[] configurations,
      Guid[] testers,
      int skip,
      int top,
      bool includeSuiteName = false)
    {
      return new List<TestPoint>();
    }

    public virtual List<TestPoint> QueryTestPoints(Dictionary<string, List<object>> parametersMap) => new List<TestPoint>();

    public virtual List<TestPoint> QueryTestPointsWithLastResults(
      Dictionary<string, List<object>> parametersMap)
    {
      return new List<TestPoint>();
    }

    public virtual List<TestPointStatistic> QueryTestPointStatistics(
      Dictionary<string, List<object>> parametersMap)
    {
      return new List<TestPointStatistic>();
    }

    public virtual Dictionary<Guid, Dictionary<string, List<int>>> GetPlansHavingBuildDefinitionNamesWithoutId(
      int top)
    {
      return new Dictionary<Guid, Dictionary<string, List<int>>>();
    }

    public virtual void PopulateBuildDefinitionIdInPlan(
      Guid projectGuid,
      Dictionary<int, List<int>> planIds)
    {
    }

    internal virtual bool CleanDeletedTestPlans(
      int waitDaysForCleanup,
      int planArtifactsDeletionBatchSize)
    {
      return true;
    }

    internal virtual Guid QueueDeleteTestPlan(Guid projectGuid, int testPlanId, Guid updatedBy)
    {
      this.QueueDeleteTestPlan(projectGuid, testPlanId, true, updatedBy);
      return projectGuid;
    }

    internal virtual void BulkUpdateTestPointStateAndTester(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool updateRunResultsInTCM,
      List<UpdatePointStateAndTester> updatePointStateAndTesters)
    {
      throw new NotImplementedException();
    }

    internal virtual void BulkUpdateOfTestPointStateAndTester(
      Guid projectGuid,
      int planId,
      int suiteId,
      bool updateRunResultsInTCM,
      List<UpdatePointStateAndTester> updatePointStateAndTesters)
    {
      throw new NotImplementedException();
    }

    internal virtual List<TestPointRecord> QueryTestPointsByOutcomeMigrationDate(
      int batchSize,
      TestPointWatermark fromWatermark,
      out TestPointWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      toWatermark = fromWatermark;
      return new List<TestPointRecord>();
    }

    internal virtual List<TestPointHistoryRecord> QueryTestPointHistoryByWatermarkDate(
      int batchSize,
      TestPointHistoryWatermark fromWatermark,
      out TestPointHistoryWatermark toWatermark,
      TestArtifactSource dataSource)
    {
      toWatermark = fromWatermark;
      return new List<TestPointHistoryRecord>();
    }

    internal virtual List<TestPlanRecord> QueryTestPlansByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      toDate = fromDate;
      return new List<TestPlanRecord>();
    }

    internal virtual bool RestoreSoftDeletedTestPlan(
      Guid projectId,
      int testPlanId,
      bool restoreWorkItem,
      bool restoreForTestPlan)
    {
      return false;
    }

    internal virtual bool RestoreSoftDeletedTestSuite(
      Guid projectId,
      int testPlanId,
      int testSuiteIdToRecover,
      bool restoreWorkItem,
      bool restoreForTestPlan)
    {
      return false;
    }

    internal virtual Session CreateSession(
      Guid projectGuid,
      Session session,
      Guid updatedBy,
      int source)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateSession");
        this.BindString("@title", session.Title, 256, false, SqlDbType.NVarChar);
        this.BindGuid("@owner", session.Owner);
        this.BindByte("@state", session.State);
        this.BindString("@buildUri", session.BuildUri, 256, true, SqlDbType.NVarChar);
        this.BindString("@buildNumber", session.BuildNumber, 260, true, SqlDbType.NVarChar);
        this.BindString("@buildPlatform", session.BuildPlatform, 256, true, SqlDbType.NVarChar);
        this.BindString("@buildFlavor", session.BuildFlavor, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@testPlanId", session.TestPlanId);
        this.BindString("@controller", session.Controller, 256, true, SqlDbType.NVarChar);
        this.BindInt("@testSettingsId", session.TestSettingsId);
        this.BindInt("@publicTestSettingsId", session.PublicTestSettingsId);
        this.BindGuid("@testEnvironmentId", session.TestEnvironmentId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindLong("@duration", session.Duration);
        this.BindString("@sprint", session.Sprint, 64, true, SqlDbType.NVarChar);
        this.BindString("@computerName", session.ComputerName, 64, true, SqlDbType.NVarChar);
        this.BindInt("@userStoryId", session.UserStoryId);
        this.BindInt("@charterId", session.CharterId);
        this.BindInt("@feedbackId", session.FeedbackId);
        this.BindInt("@configurationId", session.ConfigurationId);
        this.BindString("@configurationName", session.ConfigurationName, 256, true, SqlDbType.NVarChar);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new TestPlanningDatabase.CreateSessionColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateSession");
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual TestSession CreateTestSession(TestSession session, string teamField) => new TestSession();

    internal virtual void CreateAssociatedWorkItemsForTestSession(
      int sessionId,
      TestSessionWorkItemReference[] workItemsFilled)
    {
    }

    internal virtual void CreateExploredWorkItemsForTestSession(
      TestSession session,
      List<TestSessionExploredWorkItemReference> workItemsExplored)
    {
    }

    internal virtual Dictionary<int, TestSession> QueryTestSession(
      TfsTestManagementRequestContext testmanagementRequestContext,
      string projectId,
      List<int> sessionIds,
      List<int> sourceList,
      List<int> stateList,
      ref List<int> workItemRefListForSession,
      ref Dictionary<int, List<TestSessionWorkItemReference>> sessionIdToListOfWorkItemRef,
      ref List<int> exploredItemRefListForSession,
      ref Dictionary<int, List<TestSessionExploredWorkItemReference>> sessionIdToListOfExploredItemRef)
    {
      return new Dictionary<int, TestSession>();
    }

    internal virtual List<int> GetSessionIdsOfTeam(
      string projectId,
      int period,
      Guid sessionOwner,
      List<int> sourceList,
      List<int> stateList,
      bool isTeamFieldAreaPath,
      List<string> teamFieldsOfTeam)
    {
      return new List<int>();
    }

    internal virtual UpdatedSessionProperties UpdateSession(
      Session session,
      Guid updatedBy,
      Guid projectGuid)
    {
      this.PrepareStoredProcedure("prc_UpdateSession");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@sessionId", session.SessionId);
      this.BindString("@title", session.Title, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@owner", session.Owner);
      this.BindByte("@state", session.State);
      this.BindString("@controller", session.Controller, 256, true, SqlDbType.NVarChar);
      this.BindInt("@testSettingsId", session.TestSettingsId);
      this.BindInt("@publicTestSettingsId", session.PublicTestSettingsId);
      this.BindGuid("@testEnvironmentId", session.TestEnvironmentId);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindStringPreserveNull("@comment", session.Comment, 1048576, SqlDbType.NVarChar);
      this.BindNullableDateTime("@dateStarted", session.StartDate);
      this.BindNullableDateTime("@dateCompleted", session.CompleteDate);
      this.BindLong("@duration", session.Duration);
      this.BindString("@sprint", session.Sprint, 64, true, SqlDbType.NVarChar);
      this.BindString("@computerName", session.ComputerName, 64, true, SqlDbType.NVarChar);
      this.BindInt("@userStoryId", session.UserStoryId);
      this.BindInt("@charterId", session.CharterId);
      this.BindInt("@feedbackId", session.FeedbackId);
      this.BindInt("@configurationId", session.ConfigurationId);
      this.BindString("@configurationName", session.ConfigurationName, 256, true, SqlDbType.NVarChar);
      this.BindInt("@revision", session.Revision);
      this.BindXml("@notes", this.NotesToXml(session.Notes));
      this.BindXml("@bookmarks", this.BookMarksToXml(session.Bookmarks));
      SqlDataReader reader = this.ExecuteReader();
      UpdatedSessionProperties sessionProperties = reader.Read() ? new TestPlanningDatabase.UpdatedSessionPropertyColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateSession");
      sessionProperties.LastUpdatedBy = updatedBy;
      return sessionProperties;
    }

    internal virtual TestSession UpdateTestSession(TestSession session) => new TestSession();

    internal virtual void QueueDeleteSession(Guid projectGuid, int sessionId, Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_QueueDeleteSession");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@sessionId", sessionId);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.ExecuteNonQuery();
    }

    internal virtual List<Session> QuerySessions(
      int sessionId,
      Guid owner,
      string buildUri,
      Guid projectGuid,
      out Dictionary<Guid, List<Session>> projetcsMap)
    {
      List<Session> sessionList = new List<Session>();
      this.PrepareStoredProcedure("prc_QuerySessions");
      this.BindNullableInt("@sessionId", sessionId, 0);
      this.BindGuidPreserveNull("@owner", owner);
      this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.QuerySessionColumns querySessionColumns = new TestPlanningDatabase.QuerySessionColumns();
      Dictionary<int, Guid> dictionary = new Dictionary<int, Guid>();
      projetcsMap = new Dictionary<Guid, List<Session>>();
      while (reader.Read())
      {
        int dataspaceId;
        Session session = querySessionColumns.Bind(this.RequestContext, reader, out dataspaceId);
        if (dictionary.ContainsKey(dataspaceId))
        {
          Guid key = dictionary[dataspaceId];
          projetcsMap[key].Add(session);
        }
        else
        {
          Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
          dictionary[dataspaceId] = dataspaceIdentifier;
          projetcsMap[dataspaceIdentifier] = new List<Session>();
          projetcsMap[dataspaceIdentifier].Add(session);
        }
        sessionList.Add(session);
      }
      return sessionList;
    }

    internal virtual List<Session> QuerySessions(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      out Dictionary<Guid, List<Session>> projetcsMap)
    {
      List<Session> sessionList = new List<Session>();
      this.PrepareDynamicProcedure("prc_QuerySessions2");
      this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.QuerySessionColumns querySessionColumns = new TestPlanningDatabase.QuerySessionColumns();
      Dictionary<int, Guid> dictionary = new Dictionary<int, Guid>();
      projetcsMap = new Dictionary<Guid, List<Session>>();
      while (reader.Read())
      {
        int dataspaceId;
        Session session = querySessionColumns.Bind(this.RequestContext, reader, out dataspaceId);
        if (dictionary.ContainsKey(dataspaceId))
        {
          Guid key = dictionary[dataspaceId];
          projetcsMap[key].Add(session);
        }
        else
        {
          Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
          dictionary[dataspaceId] = dataspaceIdentifier;
          projetcsMap[dataspaceIdentifier] = new List<Session>();
          projetcsMap[dataspaceIdentifier].Add(session);
        }
        sessionList.Add(session);
      }
      return sessionList;
    }

    internal void CreateAssociatedWorkItems(int[] identifiers, string[] workItemUris)
    {
      this.PrepareStoredProcedure("prc_CreateAssociatedWorkItemsForSession");
      this.BindSessionWorkItemLinkTypeTable("@workItemLinksTable", (IEnumerable<KeyValuePair<int, string>>) this.GetListOfUris<int>(identifiers, workItemUris));
      this.ExecuteNonQuery();
    }

    internal void DeleteAssociatedWorkItems(int[] identifiers, string[] workItemUris)
    {
      this.PrepareStoredProcedure("prc_DeleteAssociatedWorkItemsForSession");
      this.BindSessionWorkItemLinkTypeTable("@workItemLinksTable", (IEnumerable<KeyValuePair<int, string>>) this.GetListOfUris<int>(identifiers, workItemUris));
      this.ExecuteNonQuery();
    }

    internal virtual Dictionary<int, List<string>> QueryAssociatedWorkItemsForSessions(
      Guid projectId,
      int[] sessionIds)
    {
      Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
      this.PrepareStoredProcedure("prc_QueryAssociatedWorkItemsForSession");
      this.BindIdTypeTable("@sessionIdsTable", (IEnumerable<int>) sessionIds);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("WorkItemUri");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("SessionId");
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

    internal virtual Guid GetProjectForSession(int sessionId)
    {
      this.PrepareStoredProcedure("prc_GetProjectForSession");
      this.BindNullableInt("@sessionId", sessionId, 0);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? this.GetDataspaceIdentifier(new SqlColumnBinder("DataspaceId").GetInt32((IDataReader) reader)) : Guid.Empty;
    }

    internal virtual Dictionary<int, TestSession> QueryTestSession(
      TfsTestManagementRequestContext testmanagementRequestContext,
      string projectId,
      List<int> sessionIds,
      ref List<int> workItemRefListForSession,
      ref Dictionary<int, List<TestSessionWorkItemReference>> sessionIdToListOfWorkItemRef,
      ref List<int> exploredItemRefListForSession,
      ref Dictionary<int, List<TestSessionExploredWorkItemReference>> sessionIdToListOfExploredItemRef)
    {
      Dictionary<int, TestSession> dictionary = new Dictionary<int, TestSession>();
      workItemRefListForSession = new List<int>();
      sessionIdToListOfWorkItemRef = new Dictionary<int, List<TestSessionWorkItemReference>>();
      exploredItemRefListForSession = new List<int>();
      sessionIdToListOfExploredItemRef = new Dictionary<int, List<TestSessionExploredWorkItemReference>>();
      try
      {
        this.PrepareStoredProcedure("prc_QueryTestSession");
        this.BindInt("@dataspaceId", this.GetDataspaceId(new Guid(projectId)));
        this.BindIdTypeTable("@sessionIdTable", (IEnumerable<int>) sessionIds);
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestSession testSession = new TestPlanningDatabase.QueryTestSessionColumns().bind((TestManagementRequestContext) testmanagementRequestContext, reader);
          dictionary[testSession.Id] = testSession;
        }
        if (reader.NextResult())
          new TestPlanningDatabase.ReadAssociatedWorkItems().bind((TestManagementRequestContext) testmanagementRequestContext, reader, ref workItemRefListForSession, ref sessionIdToListOfWorkItemRef);
        if (reader.NextResult())
          new TestPlanningDatabase.ReadExploredWorkItems().bind((TestManagementRequestContext) testmanagementRequestContext, reader, ref exploredItemRefListForSession, ref sessionIdToListOfExploredItemRef);
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      return dictionary;
    }

    internal bool CleanDeletedSessions()
    {
      this.PrepareStoredProcedure("prc_DeleteSession");
      return (int) this.ExecuteScalar() == 0;
    }

    protected string NotesToXml(List<SessionNote> notesList)
    {
      string empty = string.Empty;
      if (notesList != null && notesList.Count > 0)
      {
        using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          using (XmlTextWriter writer = new XmlTextWriter((TextWriter) w))
          {
            writer.WriteStartDocument();
            writer.WriteStartElement("notes");
            foreach (SessionNote notes in notesList)
            {
              writer.WriteStartElement("n");
              this.BindXmlInt(writer, "t", notes.CreationTime);
              this.BindXmlString(writer, "c", notes.Comment ?? string.Empty);
              writer.WriteEndElement();
            }
            writer.WriteEndDocument();
          }
          empty = w.ToString();
        }
      }
      return empty;
    }

    protected static void XmlToNotes(
      IVssRequestContext context,
      string xml,
      List<SessionNote> notes)
    {
      if (string.IsNullOrEmpty(xml))
        return;
      try
      {
        using (XmlReader safeReader = Microsoft.TeamFoundation.TestManagement.Common.Internal.XmlUtility.CreateSafeReader((TextReader) new StringReader(xml)))
        {
          for (bool flag = safeReader.ReadToFollowing("n"); flag; flag = safeReader.ReadToNextSibling("n"))
          {
            SessionNote sessionNote = new SessionNote();
            if (safeReader.MoveToAttribute("t"))
              sessionNote.CreationTime = safeReader.ReadContentAsLong();
            if (safeReader.MoveToAttribute("c"))
              sessionNote.Comment = safeReader.ReadContentAsString();
            notes.Add(sessionNote);
          }
        }
      }
      catch (XmlException ex)
      {
        context.TraceException("Database", (Exception) ex);
      }
    }

    protected string BookMarksToXml(List<SessionBookmark> bookmarkList)
    {
      string empty = string.Empty;
      if (bookmarkList != null && bookmarkList.Count > 0)
      {
        using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          using (XmlTextWriter writer = new XmlTextWriter((TextWriter) w))
          {
            writer.WriteStartDocument();
            writer.WriteStartElement("bookmarks");
            foreach (SessionBookmark bookmark in bookmarkList)
            {
              writer.WriteStartElement("b");
              this.BindXmlInt(writer, "t", bookmark.CreationTime);
              this.BindXmlString(writer, "c", bookmark.Comment ?? string.Empty);
              this.BindXmlString(writer, "u", bookmark.ArtifactUri);
              writer.WriteEndElement();
            }
            writer.WriteEndDocument();
          }
          empty = w.ToString();
        }
      }
      return empty;
    }

    protected static void XmlToBookmarks(
      IVssRequestContext context,
      string xml,
      List<SessionBookmark> bookmarks)
    {
      if (string.IsNullOrEmpty(xml))
        return;
      try
      {
        using (XmlReader safeReader = Microsoft.TeamFoundation.TestManagement.Common.Internal.XmlUtility.CreateSafeReader((TextReader) new StringReader(xml)))
        {
          for (bool flag = safeReader.ReadToFollowing("b"); flag; flag = safeReader.ReadToNextSibling("b"))
          {
            SessionBookmark sessionBookmark = new SessionBookmark();
            if (safeReader.MoveToAttribute("t"))
              sessionBookmark.CreationTime = safeReader.ReadContentAsLong();
            if (safeReader.MoveToAttribute("c"))
              sessionBookmark.Comment = safeReader.ReadContentAsString();
            if (safeReader.MoveToAttribute("u"))
              sessionBookmark.ArtifactUri = safeReader.ReadContentAsString();
            bookmarks.Add(sessionBookmark);
          }
        }
      }
      catch (XmlException ex)
      {
        context.TraceException("Database", (Exception) ex);
      }
    }

    internal List<KeyValuePair<TcmProperty, int>> FetchAllProperties()
    {
      this.PrepareStoredProcedure("prc_GetAllTcmProperties");
      return new TestPlanningDatabase.PropertyColumn().Bind(this.ExecuteReader());
    }

    internal int GetPropertyValue(TcmProperty property)
    {
      this.PrepareStoredProcedure("prc_GetTcmPropertyValue");
      this.BindInt("@propertyId", (int) property);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new TestPlanningDatabase.PropertyColumn().BindValue(reader) : 0;
    }

    internal void UpdateProperties(List<KeyValuePair<TcmProperty, int>> properties)
    {
      this.PrepareStoredProcedure("prc_UpdateTcmProperties");
      this.BindPropertyValuePairTypeTable("@propertyTable", (IEnumerable<KeyValuePair<TcmProperty, int>>) properties);
      this.ExecuteReader();
    }

    internal virtual Dictionary<string, int> QuerySqmData(int noOfDays)
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      this.PrepareStoredProcedure("prc_GetTcmSqmData");
      this.BindInt("timeDurationInDays", noOfDays);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        TestPlanningDatabase.SqmData sqmData = new TestPlanningDatabase.SqmData();
        while (reader.Read())
          dictionary.Add(sqmData.CloneOperationsCount.Key, sqmData.CloneOperationsCount.SqlKey.GetInt32((IDataReader) reader));
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.CloneOperationsAcrossProjectsCount.Key, sqmData.CloneOperationsAcrossProjectsCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.ClonePlanOperationsCount.Key, sqmData.ClonePlanOperationsCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.StaticTestSuitesCount.Key, sqmData.StaticTestSuitesCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.QueryBasedTestSuitesCount.Key, sqmData.QueryBasedTestSuitesCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.RequirementBasedTestSuitesCount.Key, sqmData.RequirementBasedTestSuitesCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.TotalProjectsCount.Key, sqmData.TotalProjectsCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.ResolutionStatesCustomizedProjectsCount.Key, sqmData.ResolutionStatesCustomizedProjectsCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.FailureTypeCustomizedProjectsCount.Key, sqmData.FailureTypeCustomizedProjectsCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.PlansCreatedFromMtm.Key, sqmData.PlansCreatedFromMtm.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.PlansCreatedFromWeb.Key, sqmData.PlansCreatedFromWeb.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.SuitesCreatedFromMtm.Key, sqmData.SuitesCreatedFromMtm.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.SuitesCreatedFromWeb.Key, sqmData.SuitesCreatedFromWeb.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.TestCasesAddedFromMtm.Key, sqmData.TestCasesAddedFromMtm.SqlKey.GetInt32((IDataReader) reader, 0));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.TestCasesAddedFromWeb.Key, sqmData.TestCasesAddedFromWeb.SqlKey.GetInt32((IDataReader) reader, 0));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.ManualTestRunsCreatedFromMtmCount.Key, sqmData.ManualTestRunsCreatedFromMtmCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.ManualTestRunsCreatedFromWebCount.Key, sqmData.ManualTestRunsCreatedFromWebCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.MTRRunsInitiatedFromWebCount.Key, sqmData.MTRRunsInitiatedFromWebCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.XTSessionsCount.Key, sqmData.XTSessionsCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.FBSessionsCount.Key, sqmData.FBSessionsCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.UsersCreatingRunsFromWebCount.Key, sqmData.UsersCreatingRunsFromWebCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.UsersCreatingRunsFromMtmCount.Key, sqmData.UsersCreatingRunsFromMtmCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.UsersCreatingPlansFromWebCount.Key, sqmData.UsersCreatingPlansFromWebCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.UsersCreatingPlansFromMtmCount.Key, sqmData.UsersCreatingPlansFromMtmCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.UsersUsingXTCount.Key, sqmData.UsersUsingXTCount.SqlKey.GetInt32((IDataReader) reader));
        }
        if (this.MoreResultsAvailable(reader))
        {
          while (reader.Read())
            dictionary.Add(sqmData.TotalDTARunsQueued.Key, sqmData.TotalDTARunsQueued.SqlKey.GetInt32((IDataReader) reader));
        }
      }
      return dictionary;
    }

    internal List<TestRunLabSQMData> QueryTestRunLabSQMData(int timeDurationInDays)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestRunLabSQMData");
      this.BindInt("@timeDurationInDays", timeDurationInDays);
      SqlDataReader reader = this.ExecuteReader();
      List<TestRunLabSQMData> testRunLabSqmDataList = new List<TestRunLabSQMData>();
      TestPlanningDatabase.QueryTestRunLabSQMDataColumns labSqmDataColumns = new TestPlanningDatabase.QueryTestRunLabSQMDataColumns();
      while (reader.Read())
      {
        TestRunLabSQMData testRunLabSqmData = labSqmDataColumns.bind(reader);
        testRunLabSqmDataList.Add(testRunLabSqmData);
      }
      return testRunLabSqmDataList;
    }

    internal virtual UpdatedProperties CreateSuite(
      TestManagementRequestContext context,
      ref UpdatedProperties parent,
      ServerTestSuite child,
      Guid updatedBy,
      IEnumerable<TestCaseAndOwner> entries,
      int toIndex,
      Guid projectGuid,
      TestSuiteSource type)
    {
      try
      {
        context.TraceAndDebugAssert("Database", parent.Id != 0, "Cannot create root suite");
        this.PrepareStoredProcedure("prc_CreateSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parent.Id);
        this.BindInt("@suiteId", child.Id);
        this.BindInt("@suiteRevision", child.Revision);
        this.BindInt("@toIndex", toIndex);
        this.BindString("@title", child.Title, 256, false, SqlDbType.NVarChar);
        this.BindString("@description", child.Description, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindByte("@type", child.SuiteType);
        this.BindByte("@clientType", (byte) type);
        this.BindString("@query", child.ConvertedQueryString, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindInt("@requirementId", child.RequirementId);
        this.BindString("@witStatus", child.Status, 256, false, SqlDbType.NVarChar);
        this.BindBoolean("@inheritConfigs", child.InheritDefaultConfigurations);
        this.BindIdTypeTable("@configIdsTable", (IEnumerable<int>) child.DefaultConfigurations);
        this.BindTestCaseAndOwnerTypeTable("@testCaseAndOwnerTable", entries);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindDateTime("@lastUpdated", child.LastUpdated);
        this.BindInt("@parentRevision", parent.Revision);
        object obj = this.ExecuteScalar() ?? (object) child.Id;
        return new UpdatedProperties() { Id = (int) obj };
      }
      catch (SqlException ex)
      {
        this.HandleDuplicateSuiteEntryError(ex);
        throw;
      }
    }

    internal virtual CloneTestPlanOperationInformation GetPlanCloneOperation(
      int opId,
      out List<Tuple<Guid, Guid, int, CloneTestPlanOperationInformation>> projectsSuiteIdsList,
      out Dictionary<string, string> resolvedFieldDetails)
    {
      throw new NotImplementedException();
    }

    internal virtual CloneTestSuiteOperationInformation GetSuiteCloneOperation(
      int opId,
      out List<Tuple<Guid, Guid, int, CloneTestSuiteOperationInformation>> projectsSuiteIdsList,
      out Dictionary<string, string> resolvedFieldDetails)
    {
      throw new NotImplementedException();
    }

    internal virtual void SyncSuites(List<ServerTestSuite> suites) => throw new NotImplementedException();

    internal virtual List<int> Getsuites(int opId) => throw new NotImplementedException();

    internal virtual int BeginCloneOperation(
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
        if (options != null && options.CopyAncestorHierarchy)
          this.BindBoolean("@copyRecursively", false);
        else
          this.BindBoolean("@copyRecursively", true);
        this.BindGuid("@createdBy", createdBy);
        this.BindByte("@operationType", (byte) operationType);
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

    protected static bool IsNullOrEmpty(Dictionary<string, string> dictionary) => dictionary == null && dictionary.Count > 0;

    protected string DictionaryOfOverriddenFieldDetailsToXml(
      Dictionary<string, string> overriddenFieldDetails,
      string destinationWorkItemType)
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
        if (!string.IsNullOrEmpty(destinationWorkItemType))
        {
          xmlTextWriter.WriteStartElement("Field");
          xmlTextWriter.WriteAttributeString("Key", WorkItemFieldRefNames.WorkItemType);
          xmlTextWriter.WriteAttributeString("Value", destinationWorkItemType);
          xmlTextWriter.WriteEndElement();
        }
        xmlTextWriter.WriteEndElement();
        xmlTextWriter.Flush();
        return w.ToString();
      }
    }

    protected static Dictionary<string, string> XmlToDictionaryOfOverriddenFieldDetails(
      IVssRequestContext requestContext,
      string xml,
      out string workItemType)
    {
      Dictionary<string, string> overriddenFieldDetails = (Dictionary<string, string>) null;
      workItemType = (string) null;
      if (!string.IsNullOrEmpty(xml))
      {
        overriddenFieldDetails = new Dictionary<string, string>();
        try
        {
          using (XmlReader safeReader = Microsoft.TeamFoundation.TestManagement.Common.Internal.XmlUtility.CreateSafeReader((TextReader) new StringReader(xml)))
          {
            for (bool flag = safeReader.ReadToFollowing("Field"); flag; flag = safeReader.ReadToNextSibling("Field"))
            {
              string fieldId;
              string fieldValue;
              TestPlanningDatabase.ReadFieldIdAndValueFromXml(safeReader, out fieldId, out fieldValue);
              if (string.CompareOrdinal(fieldId, WorkItemFieldRefNames.WorkItemType) == 0)
                workItemType = fieldValue;
              else
                overriddenFieldDetails.Add(fieldId, fieldValue);
            }
          }
        }
        catch (XmlException ex)
        {
          requestContext.TraceException("Database", (Exception) ex);
        }
      }
      return overriddenFieldDetails;
    }

    protected List<int> GetConfigurationsFromDb()
    {
      HashSet<int> source = new HashSet<int>();
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("ConfigurationId");
      while (reader.Read())
      {
        int int32 = sqlColumnBinder.GetInt32((IDataReader) reader);
        source.Add(int32);
      }
      return source.ToList<int>();
    }

    private static void ReadFieldIdAndValueFromXml(
      XmlReader reader,
      out string fieldId,
      out string fieldValue)
    {
      fieldId = (string) null;
      fieldValue = string.Empty;
      int num = reader.IsEmptyElement ? 1 : 0;
      if (!reader.HasAttributes)
        return;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case "Key":
            fieldId = Microsoft.VisualStudio.Services.Common.Internal.XmlUtility.StringFromXmlAttribute(reader);
            continue;
          case "Value":
            fieldValue = Microsoft.VisualStudio.Services.Common.Internal.XmlUtility.StringFromXmlAttribute(reader);
            continue;
          default:
            continue;
        }
      }
    }

    internal int GetAllSuitesToClone(
      int opId,
      out List<int> suitesToClone,
      out HashSet<int> suitesToUpdateRelationship,
      out bool cloneRootSuite)
    {
      try
      {
        this.PrepareStoredProcedure("prc_GetAllSuitesToClone");
        this.BindInt("@opId", opId);
        suitesToClone = new List<int>();
        suitesToUpdateRelationship = new HashSet<int>();
        int allSuitesToClone = 0;
        cloneRootSuite = false;
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("SuiteId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("CloneRootSuite");
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
          suitesToClone.Add(sqlColumnBinder1.GetInt32((IDataReader) reader));
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_GetAllSuitesToClone");
        while (reader.Read())
          suitesToUpdateRelationship.Add(sqlColumnBinder1.GetInt32((IDataReader) reader));
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_GetAllSuitesToClone");
        while (reader.Read())
        {
          allSuitesToClone = sqlColumnBinder1.GetInt32((IDataReader) reader);
          cloneRootSuite = sqlColumnBinder2.GetBoolean((IDataReader) reader);
        }
        return allSuitesToClone;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual int CompleteCloneOperation(
      TestManagementRequestContext context,
      CloneOperationInformation opInfo,
      int opId,
      List<ServerTestSuite> clonedSuites,
      UpdatedProperties targetSuiteProp,
      TcmCommonStructureNodeInfo targetProjectRootAreaUri)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CompleteCloneOperation");
        if (targetProjectRootAreaUri != null)
          this.BindString("@targetProjectRootAreaUri", targetProjectRootAreaUri.Uri, 256, false, SqlDbType.NVarChar);
        else
          this.BindString("@targetProjectRootAreaUri", string.Empty, 256, false, SqlDbType.NVarChar);
        this.BindInt("@opId", opId);
        this.BindString("@sourcePlanName", opInfo.SourcePlanName, 256, false, SqlDbType.NVarChar);
        this.BindServerTestSuiteTypeTable("@clonedSuites", (IEnumerable<ServerTestSuite>) clonedSuites);
        this.BindInt("@targetSuiteId", targetSuiteProp.Id);
        this.BindInt("@targetSuiteRevision", targetSuiteProp.Revision);
        this.BindDateTime("@targetSuiteLastUpdated", targetSuiteProp.LastUpdated);
        this.BindGuid("@targetSuiteLastUpdatedBy", targetSuiteProp.LastUpdatedBy);
        SqlDataReader sqlDataReader = this.ExecuteReader();
        sqlDataReader.Read();
        object[] values = new object[sqlDataReader.FieldCount];
        sqlDataReader.GetValues(values);
        return (int) values[0];
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal Dictionary<int, int> GetCloneRelationship(
      int opId,
      CloneItemType itemType,
      bool isCloned)
    {
      try
      {
        this.PrepareStoredProcedure("prc_GetCloneRelationship");
        this.BindInt("@opId", opId);
        this.BindByte("@itemType", (byte) itemType);
        this.BindBoolean("@isCloned", isCloned);
        Dictionary<int, int> cloneRelationship = new Dictionary<int, int>();
        SqlDataReader sqlDataReader = this.ExecuteReader();
        while (sqlDataReader.Read())
        {
          object[] values = new object[sqlDataReader.FieldCount];
          sqlDataReader.GetValues(values);
          int key = (int) values[0];
          int num = values[1] == DBNull.Value ? 0 : (int) values[1];
          cloneRelationship[key] = num;
        }
        return cloneRelationship;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual void UpdateCloneRelationship(
      int opId,
      Dictionary<int, int> idsMap,
      CloneItemType itemType,
      bool incrementItemCount = true)
    {
      try
      {
        this.PrepareStoredProcedure("prc_UpdateCloneRelationship");
        this.BindInt("@opId", opId);
        this.BindIdPairTypeTable("@idsMap", (IEnumerable<KeyValuePair<int, int>>) idsMap);
        this.BindByte("@itemType", (byte) itemType);
        this.BindBoolean("@incrementItemCount", incrementItemCount);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual CloneOperationInformation GetCloneOperation(
      int opId,
      out List<Tuple<Guid, Guid, int, CloneOperationInformation>> projectsSuiteIdsList)
    {
      IVssRequestContext requestContext = this.RequestContext;
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: OpId:{0}", (object) opId);
      CloneOperationInformation cloneOperation = new CloneOperationInformation();
      TestPlanningDatabase.FetchCloneInformationColumns informationColumns = new TestPlanningDatabase.FetchCloneInformationColumns();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("SourceSuiteParentSuiteId");
      projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneOperationInformation>>();
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
          cloneOperation = informationColumns.bind(requestContext, reader, out sourceDataspaceId);
          int int32_1 = sqlColumnBinder2.GetInt32((IDataReader) reader);
          Guid dataspaceIdentifier1 = this.GetDataspaceIdentifier(sourceDataspaceId);
          Guid dataspaceIdentifier2 = this.GetDataspaceIdentifier(int32_1);
          int int32_2 = sqlColumnBinder1.GetInt32((IDataReader) reader, -1);
          projectsSuiteIdsList.Add(new Tuple<Guid, Guid, int, CloneOperationInformation>(dataspaceIdentifier1, dataspaceIdentifier2, int32_2, cloneOperation));
        }
        requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "GetCloneOperation: {0}", (object) cloneOperation);
        return cloneOperation;
      }
      catch (SqlException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "Database", "GetCloneOperation: Error {0}", (object) ex.ToString());
        this.MapException(ex);
        throw;
      }
    }

    internal void UpdateCloneOperationState(int opId, CloneOperationState state, string message)
    {
      CloneOperationInformation operationInformation = new CloneOperationInformation();
      try
      {
        this.PrepareStoredProcedure("prc_UpdateCloneOperation");
        this.BindInt("@opId", opId);
        this.BindByte("@state", (byte) state);
        this.BindStringPreserveNull("@message", message, 1024, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual void CloneAfnStrip(
      int opId,
      int cloneTestCaseId,
      int tfsFileId,
      long uncompressedLength,
      string comment,
      bool changeCounterInterval = false)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "CloneAfnStrip: OpId {0}, TestCaseId {1}, FileId {2}, Length {3}", (object) opId, (object) cloneTestCaseId, (object) tfsFileId, (object) uncompressedLength);
      CloneOperationInformation operationInformation = new CloneOperationInformation();
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_CloneAfnStrip");
        this.BindInt("@opId", opId);
        this.BindInt("@cloneTestCaseId", cloneTestCaseId);
        this.BindInt("@tfsFileId", tfsFileId);
        this.BindLong("@uncompressedLength", uncompressedLength);
        this.BindStringPreserveNull("@cloneTitle", comment, -1, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal void DeleteOldCloneOperation(int olderThanMinutes)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "DeleteOldCloneOperation: deleting jobs older thann {0} minutes", (object) olderThanMinutes);
      try
      {
        this.PrepareStoredProcedure("prc_DeleteOldCloneOperation");
        this.BindInt("@olderThanMinutes", olderThanMinutes);
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "DeleteOldCloneOperation: {0} jobs were deleted", (object) (int) this.ExecuteScalar());
      }
      catch (SqlException ex)
      {
        this.RequestContext.TraceException("Database", (Exception) ex);
        this.MapException(ex);
        throw;
      }
    }

    public virtual void UpdateSuiteTesters(
      TestManagementRequestContext context,
      Guid projectGuid,
      int suiteId,
      Guid[] testers)
    {
      try
      {
        context.TraceEnter("Database", "SuiteDatabase.UpdateSuiteTesters");
        this.PrepareStoredProcedure("prc_UpdateSuiteTesters");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@suiteId", suiteId);
        this.BindGuidTable("@testerIdTable", (IEnumerable<Guid>) testers);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        context.TraceLeave("Database", "SuiteDatabase.UpdateSuiteTesters");
        throw;
      }
    }

    public virtual List<Guid> GetTestersAssignedToSuite(
      TestManagementRequestContext context,
      Guid projectGuid,
      int suiteId)
    {
      this.PrepareStoredProcedure("prc_GetTestersAssignedToSuite");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@suiteId", suiteId);
      List<Guid> testersAssignedToSuite = new List<Guid>();
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("Tester");
      while (reader.Read())
        testersAssignedToSuite.Add(sqlColumnBinder.GetGuid((IDataReader) reader));
      return testersAssignedToSuite;
    }

    public virtual void SyncTestPointsForSuiteTesters(
      TestManagementRequestContext context,
      Guid projectGuid,
      UpdatedProperties parentProps)
    {
      try
      {
        context.TraceEnter("Database", "SuiteDatabase.SyncTestPointsForSuiteTesters");
        this.PrepareStoredProcedure("prc_SyncTestPointsForSuiteTesters");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        context.TraceLeave("Database", "SuiteDatabase.SyncTestPointsForSuiteTesters");
        this.MapException(ex);
        throw;
      }
    }

    public virtual void SyncTestPoints(
      TestManagementRequestContext context,
      Guid projectGuid,
      UpdatedProperties parentProps)
    {
      this.SyncTestPointsForSuiteConfigurations(context, projectGuid, parentProps);
      this.SyncTestPointsForSuiteTesters(context, projectGuid, parentProps);
    }

    public virtual void SyncTestPointsForSuiteConfigurations(
      TestManagementRequestContext context,
      Guid projectGuid,
      UpdatedProperties parentProps)
    {
      try
      {
        context.TraceEnter("Database", "SuiteDatabase.SyncTestPointsForSuiteConfigurations");
        this.PrepareStoredProcedure("prc_SyncTestPointsForSuiteConfigurations");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        context.TraceLeave("Database", "SuiteDatabase.SyncTestPointsForSuiteConfigurations");
        this.MapException(ex);
        throw;
      }
    }

    public virtual void SyncTestPointsForTestCaseConfigurations(
      TestManagementRequestContext context,
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<int> testCaseIds,
      List<int> configurationIds,
      List<TestPointAssignment> testCaseConfigurationPair = null)
    {
      try
      {
        if (testCaseConfigurationPair != null)
          throw new NotImplementedException();
        context.TraceEnter("Database", "SuiteDatabase.SyncTestPointsForTestCaseConfigurations");
        this.PrepareStoredProcedure("prc_SyncTestPointsForTestCaseConfigurations");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.BindIdTypeTable("@testCaseIds", (IEnumerable<int>) testCaseIds);
        this.BindIdTypeTable("@configurationIds", (IEnumerable<int>) configurationIds);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        context.TraceLeave("Database", "SuiteDatabase.SyncTestPointsForTestCaseConfigurations");
        this.MapException(ex);
        throw;
      }
    }

    public virtual UpdatedProperties CreateSuiteEntries(
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
        if (testCaseConfigurationPair != null)
          throw new NotImplementedException();
        this.PrepareStoredProcedure("prc_CreateSuiteEntries");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindInt("@toIndex", toIndex);
        this.BindInt("@clientType", (int) (byte) type);
        this.BindTestCaseAndOwnerTypeTable("@testCaseAndOwnerTable", testCases);
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

    internal virtual Dictionary<int, ServerTestSuite> FetchTestSuites(
      TestManagementRequestContext context,
      IdAndRev[] suiteIds,
      List<int> deletedIds,
      bool includeTesters,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "Database", "SuiteDatabase.FetchTestSuites"))
        {
          context.TraceEnter("Database", "TestPlanningDatabase.FetchTestSuites");
          this.PrepareStoredProcedure("prc_FetchTestSuites");
          this.BindIdAndRevTypeTable("@idsTable", (IEnumerable<IdAndRev>) suiteIds);
          SqlDataReader reader = this.ExecuteReader();
          List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
          TestPlanningDatabase.FetchTestSuitesColumns testSuitesColumns = new TestPlanningDatabase.FetchTestSuitesColumns();
          Dictionary<int, Guid> dictionary1 = new Dictionary<int, Guid>();
          projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
          while (reader.Read())
          {
            int dataspaceId;
            ServerTestSuite serverTestSuite = testSuitesColumns.bind(reader, out dataspaceId);
            if (dictionary1.ContainsKey(dataspaceId))
            {
              Guid key = dictionary1[dataspaceId];
              projectsSuitesMap[key].Add(serverTestSuite);
            }
            else
            {
              Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
              dictionary1[dataspaceId] = dataspaceIdentifier;
              projectsSuitesMap[dataspaceIdentifier] = new List<ServerTestSuite>();
              projectsSuitesMap[dataspaceIdentifier].Add(serverTestSuite);
            }
            serverTestSuiteList.Add(serverTestSuite);
          }
          Dictionary<int, ServerTestSuite> dictionary2 = new Dictionary<int, ServerTestSuite>();
          foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
          {
            serverTestSuite.ServerEntries.Clear();
            dictionary2[serverTestSuite.Id] = serverTestSuite;
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
          new TestPlanningDatabase.IdsPropertyColumns().bind(reader, deletedIds);
          if (dictionary2.Count > 0)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
            TestSuiteEntry lastEntry = (TestSuiteEntry) null;
            List<TestPointAssignment> points = new List<TestPointAssignment>();
            TestPlanningDatabase.TestSuiteEntryColumns suiteEntryColumns = new TestPlanningDatabase.TestSuiteEntryColumns();
            HashSet<TestSuiteEntry> testSuiteEntrySet = new HashSet<TestSuiteEntry>();
            while (reader.Read())
            {
              TestSuiteEntry testSuiteEntry = suiteEntryColumns.bind(reader, lastEntry, points);
              ServerTestSuite serverTestSuite;
              if (testSuiteEntry != lastEntry && dictionary2.TryGetValue(testSuiteEntry.ParentSuiteId, out serverTestSuite))
              {
                if (!testSuiteEntry.IsTestCaseEntry)
                  testSuiteEntrySet.Add(testSuiteEntry);
                serverTestSuite.ServerEntries.Add(testSuiteEntry);
                if (testSuiteEntry.EntryType == (byte) 1)
                  ++serverTestSuite.TestCaseCount;
              }
              lastEntry = testSuiteEntry;
            }
            if (lastEntry != null && points.Count > 0)
              lastEntry.PointAssignments = points.ToArray();
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
            TestPlanningDatabase.TestSuiteConfigurationColumns configurationColumns = new TestPlanningDatabase.TestSuiteConfigurationColumns();
            while (reader.Read())
            {
              int suiteId;
              int configurationId;
              string configurationName;
              configurationColumns.bind(reader, out suiteId, out configurationId, out configurationName);
              ServerTestSuite serverTestSuite;
              if (dictionary2.TryGetValue(suiteId, out serverTestSuite))
              {
                serverTestSuite.DefaultConfigurations.Add(configurationId);
                serverTestSuite.DefaultConfigurationNames.Add(configurationName);
              }
            }
          }
          return dictionary2;
        }
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanningDatabase.FetchTestSuites");
      }
    }

    internal virtual List<ServerTestSuite> FetchTestSuitesForTestCase(
      TestManagementRequestContext context,
      int testCaseId,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap)
    {
      this.PrepareStoredProcedure("prc_FetchTestSuitesForTestCase");
      this.BindInt("@testCaseId", testCaseId);
      SqlDataReader reader = this.ExecuteReader();
      List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
      TestPlanningDatabase.FetchTestSuitesColumns testSuitesColumns = new TestPlanningDatabase.FetchTestSuitesColumns();
      Dictionary<int, Guid> dictionary1 = new Dictionary<int, Guid>();
      projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
      while (reader.Read())
      {
        int dataspaceId;
        ServerTestSuite serverTestSuite = testSuitesColumns.bind(reader, out dataspaceId);
        if (dictionary1.ContainsKey(dataspaceId))
        {
          Guid key = dictionary1[dataspaceId];
          projectsSuitesMap[key].Add(serverTestSuite);
        }
        else
        {
          Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
          dictionary1[dataspaceId] = dataspaceIdentifier;
          projectsSuitesMap[dataspaceIdentifier] = new List<ServerTestSuite>();
          projectsSuitesMap[dataspaceIdentifier].Add(serverTestSuite);
        }
        serverTestSuiteList.Add(serverTestSuite);
      }
      Dictionary<int, ServerTestSuite> dictionary2 = new Dictionary<int, ServerTestSuite>();
      foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
      {
        serverTestSuite.ServerEntries.Clear();
        dictionary2[serverTestSuite.Id] = serverTestSuite;
      }
      return new List<ServerTestSuite>((IEnumerable<ServerTestSuite>) dictionary2.Values);
    }

    internal virtual List<ServerTestSuite> FetchTestSuitesForPlan(
      TestManagementRequestContext context,
      Guid projectGuid,
      int planId,
      int rootSuiteId,
      bool includeOnlyL1,
      bool includeTester,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "Database", "SuiteDatabase.FetchTestSuitesForPlan"))
        {
          context.TraceEnter("Database", "SuiteDatabase.FetchTestSuitesForPlan");
          this.PrepareStoredProcedure("prc_FetchTestSuitesForPlan");
          this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
          this.BindInt("@planId", planId);
          SqlDataReader reader = this.ExecuteReader();
          return this.ReadSuiteMetaData(context, "prc_FetchTestSuitesForPlan", reader, false, out projectsSuitesMap);
        }
      }
      finally
      {
        context.TraceLeave("Database", "SuiteDatabase.FetchTestSuitesForPlan");
      }
    }

    internal virtual List<ServerTestSuite> ReadSuiteMetaData(
      TestManagementRequestContext context,
      string StoredProcedure,
      SqlDataReader reader,
      bool includeTesters,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanningDatabase.ReadSuiteMetaData");
        Dictionary<int, ServerTestSuite> dictionary1 = new Dictionary<int, ServerTestSuite>();
        List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
        TestPlanningDatabase.FetchTestSuitesColumns testSuitesColumns = new TestPlanningDatabase.FetchTestSuitesColumns();
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
        while (reader.Read())
        {
          int dataspaceId;
          ServerTestSuite serverTestSuite = testSuitesColumns.bind(reader, out dataspaceId);
          if (dictionary2.ContainsKey(dataspaceId))
          {
            Guid key = dictionary2[dataspaceId];
            projectsSuitesMap[key].Add(serverTestSuite);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary2[dataspaceId] = dataspaceIdentifier;
            projectsSuitesMap[dataspaceIdentifier] = new List<ServerTestSuite>();
            projectsSuitesMap[dataspaceIdentifier].Add(serverTestSuite);
          }
          serverTestSuiteList.Add(serverTestSuite);
          dictionary1.Add(serverTestSuite.Id, serverTestSuite);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(StoredProcedure);
        TestPlanningDatabase.FetchTestSuitesTestCountColumns testCountColumns = new TestPlanningDatabase.FetchTestSuitesTestCountColumns();
        while (reader.Read())
        {
          int int32_1 = testCountColumns.SuiteId.GetInt32((IDataReader) reader);
          int int32_2 = testCountColumns.TestCaseCount.GetInt32((IDataReader) reader);
          ServerTestSuite serverTestSuite;
          if (dictionary1.TryGetValue(int32_1, out serverTestSuite))
            serverTestSuite.TestCaseCount = int32_2;
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(StoredProcedure);
        TestPlanningDatabase.TestSuiteConfigurationColumns configurationColumns = new TestPlanningDatabase.TestSuiteConfigurationColumns();
        ServerTestSuite serverTestSuite1;
        while (reader.Read())
        {
          int suiteId;
          int configurationId;
          string configurationName;
          configurationColumns.bind(reader, out suiteId, out configurationId, out configurationName);
          if (dictionary1.TryGetValue(suiteId, out serverTestSuite1))
          {
            serverTestSuite1.DefaultConfigurations.Add(configurationId);
            serverTestSuite1.DefaultConfigurationNames.Add(configurationName);
          }
        }
        if (includeTesters && reader.NextResult())
        {
          TestPlanningDatabase.FetchTestSuitesTesterColumns suitesTesterColumns = new TestPlanningDatabase.FetchTestSuitesTesterColumns();
          while (reader.Read())
          {
            int suiteId;
            Guid tester;
            suitesTesterColumns.bind(reader, out suiteId, out tester);
            if (dictionary1.TryGetValue(suiteId, out serverTestSuite1))
              serverTestSuite1.DefaultTesters.Add(tester);
          }
        }
        return serverTestSuiteList;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanningDatabase.ReadSuiteMetaData");
      }
    }

    internal virtual List<SuiteIdAndType> QueryTestSuites(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.QueryTestSuites");
        this.PrepareDynamicProcedure("prc_QuerySuites");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        return this.PerformSuiteQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.QueryTestSuites");
      }
    }

    internal virtual List<SuiteIdAndType> QuerySuitesByTestCaseId(int testCaseId, Guid projectGuid)
    {
      this.PrepareStoredProcedure("prc_QuerySuitesByTestCaseId");
      this.BindInt("@testCaseId", testCaseId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      return this.PerformSuiteQuery();
    }

    internal virtual List<SuiteIdAndType> PerformSuiteQuery()
    {
      List<SuiteIdAndType> suiteIdAndTypeList = new List<SuiteIdAndType>();
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase.SuiteIdAndTypeColumns idAndTypeColumns = new TestPlanningDatabase.SuiteIdAndTypeColumns();
      while (reader.Read())
        suiteIdAndTypeList.Add(idAndTypeColumns.bind(reader));
      return suiteIdAndTypeList;
    }

    internal virtual List<int> GetAllChildSuites(int testSuiteId, Guid projectGuid)
    {
      try
      {
        this.PrepareStoredProcedure("prc_GetAllChildSuites");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@testSuiteId", testSuiteId);
        SqlDataReader reader = this.ExecuteReader();
        List<int> allChildSuites = new List<int>();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("ChildSuiteId");
        while (reader.Read())
          allChildSuites.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return allChildSuites;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual int CopySuiteEntries(
      TestManagementRequestContext context,
      Guid projectGuid,
      int fromSuiteId,
      List<TestSuiteEntry> entries,
      UpdatedProperties toSuiteUpdatedProperties,
      int toIndex,
      List<ServerTestSuite> newlyCreatedSuites,
      Dictionary<int, int> oldSuiteIdToNewSuiteIdMap,
      out Dictionary<int, int> sourceToTargetSuiteMap)
    {
      try
      {
        context.TraceEnter("Database", "SuiteDatabase.CopySuiteEntries");
        this.PrepareStoredProcedure("prc_CopySuiteEntries");
        this.BindInt("@fromSuiteId", fromSuiteId);
        this.BindInt("@toSuiteId", toSuiteUpdatedProperties.Id);
        this.BindInt("@toSuiteRevision", toSuiteUpdatedProperties.Revision);
        this.BindInt("@toIndex", toIndex);
        this.BindTestSuiteEntryTypeTable("@entriesTable", (IEnumerable<TestSuiteEntry>) entries);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindGuid("@lastUpdatedBy", toSuiteUpdatedProperties.LastUpdatedBy);
        this.BindDateTime("@lastUpdated", toSuiteUpdatedProperties.LastUpdated);
        this.BindServerTestSuiteTypeTable("@newlyCreatedSuites", (IEnumerable<ServerTestSuite>) newlyCreatedSuites);
        this.ExecuteNonQuery();
        sourceToTargetSuiteMap = new Dictionary<int, int>();
        return toSuiteUpdatedProperties.Revision;
      }
      finally
      {
        context.TraceLeave("Database", "SuiteDatabase.CopySuiteEntries");
      }
    }

    internal virtual List<int> GetDescendentSuites(Guid projectGuid, int parentSuiteId)
    {
      try
      {
        this.PrepareStoredProcedure("prc_GetDescendentSuites");
        this.BindInt("@parentSuiteId", parentSuiteId);
        SqlDataReader reader = this.ExecuteReader();
        List<int> descendentSuites = new List<int>();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("SuiteId");
        while (reader.Read())
          descendentSuites.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return descendentSuites;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal virtual List<int> ValidateCopySuiteEntriesAndGetChildSuites(
      Guid projectGuid,
      int fromSuiteId,
      IdAndRev toSuite,
      List<TestSuiteEntry> entries,
      bool skipValidations)
    {
      this.PrepareStoredProcedure("prc_ValidateCopySuiteEntriesAndGetChildSuites");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@fromSuiteId", fromSuiteId);
      this.BindInt("@toSuiteId", toSuite.Id);
      this.BindInt("@toSuiteRevision", toSuite.Revision);
      this.BindTestSuiteEntryTypeTable("@entriesTable", (IEnumerable<TestSuiteEntry>) entries);
      this.BindBoolean("@skipValidations", skipValidations);
      SqlDataReader reader = this.ExecuteReader();
      List<int> childSuites = new List<int>();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("Id");
      while (reader.Read())
        childSuites.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      return childSuites;
    }

    internal virtual void MoveSuiteEntries(
      Guid projectGuid,
      ref UpdatedProperties fromSuiteProps,
      List<TestSuiteEntry> entries,
      ref UpdatedProperties toSuiteProps,
      int toIndex,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_MoveSuiteEntries");
      this.BindInt("@fromSuiteId", fromSuiteProps.Id);
      this.BindInt("@fromSuiteRevision", fromSuiteProps.Revision);
      this.BindDateTime("@fromSuiteLastUpdated", fromSuiteProps.LastUpdated);
      this.BindInt("@toSuiteId", toSuiteProps.Id);
      this.BindInt("@toSuiteRevision", toSuiteProps.Revision);
      this.BindDateTime("@toSuiteLastUpdated", toSuiteProps.LastUpdated);
      this.BindInt("@toIndex", toIndex);
      this.BindTestSuiteEntryTypeTable("@entriesTable", (IEnumerable<TestSuiteEntry>) entries);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.ExecuteReader();
    }

    internal virtual UpdatedProperties DeleteSuiteEntries(
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<TestSuiteEntry> entries,
      out (int, List<int>) planPointIds)
    {
      planPointIds = (-1, (List<int>) null);
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.DeleteSuiteEntries");
        this.PrepareStoredProcedure("prc_DeleteSuiteEntries");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindTestSuiteEntryTypeTable("@entriesTable", (IEnumerable<TestSuiteEntry>) entries);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.ExecuteNonQuery();
        return parentProps;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.DeleteSuiteEntries");
      }
    }

    internal virtual (int PlanId, List<int> PointIds) DestroyTestCase(
      int testCaseId,
      List<UpdatedProperties> suiteProps,
      out Guid projectGuid)
    {
      projectGuid = Guid.Empty;
      this.PrepareStoredProcedure("prc_DestroyTestCase");
      this.BindInt("@testCaseId", testCaseId);
      this.BindUpdatedPropertiesTypeTable("@suitePropTable", (IEnumerable<UpdatedProperties>) suiteProps);
      this.ExecuteNonQuery();
      return (-1, (List<int>) null);
    }

    internal virtual Dictionary<IdAndRev, List<int>> FetchParentsIdAndRev(
      List<int> suitesIds,
      out Guid projectGuid,
      out List<int> plans)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.FetchParentsIdAndRev");
        projectGuid = Guid.Empty;
        plans = new List<int>();
        this.PrepareStoredProcedure("prc_FetchParentsIdAndRev");
        this.BindIdTypeTable("@suiteIdsTable", (IEnumerable<int>) suitesIds);
        Dictionary<IdAndRev, List<int>> dictionary1 = new Dictionary<IdAndRev, List<int>>();
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("ParentId");
        SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Revision");
        SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("SuiteId");
        SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("DataspaceId");
        SqlColumnBinder sqlColumnBinder5 = new SqlColumnBinder("PlanId");
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        while (reader.Read())
        {
          int int32_1 = sqlColumnBinder4.GetInt32((IDataReader) reader);
          if (!dictionary2.TryGetValue(int32_1, out projectGuid))
          {
            projectGuid = this.GetDataspaceIdentifier(int32_1);
            dictionary2[int32_1] = projectGuid;
          }
          int int32_2 = sqlColumnBinder5.GetInt32((IDataReader) reader);
          int int32_3 = sqlColumnBinder1.GetInt32((IDataReader) reader);
          if (int32_3 == 0)
          {
            plans.Add(int32_2);
          }
          else
          {
            int int32_4 = sqlColumnBinder3.GetInt32((IDataReader) reader);
            int int32_5 = sqlColumnBinder2.GetInt32((IDataReader) reader);
            IdAndRev key = new IdAndRev(int32_3, int32_5);
            if (!dictionary1.ContainsKey(key))
              dictionary1.Add(key, new List<int>());
            dictionary1[key].Add(int32_4);
          }
        }
        return dictionary1;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.FetchParentsIdAndRev");
      }
    }

    internal virtual void SyncSuites(Guid projectGuid, List<ServerTestSuite> suites)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.SyncSuites");
        this.PrepareStoredProcedure("prc_SyncSuites");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindServerTestSuiteTypeTable("@suiteMetaDataTable", (IEnumerable<ServerTestSuite>) suites);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.SyncSuites");
      }
    }

    internal virtual List<IdAndRev> FetchSuitesRevision(Guid projectGuid, List<int> suiteIds)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestPlanningDatabase.FetchSuitesRevision");
        this.PrepareStoredProcedure("prc_FetchSuitesRevision");
        this.BindIdTypeTable("@suiteIds", (IEnumerable<int>) suiteIds);
        SqlDataReader reader = this.ExecuteReader();
        List<IdAndRev> idAndRevList = new List<IdAndRev>();
        TestPlanningDatabase.IdAndRevColumns idAndRevColumns = new TestPlanningDatabase.IdAndRevColumns();
        while (reader.Read())
          idAndRevList.Add(idAndRevColumns.bind(reader));
        return idAndRevList;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanningDatabase.FetchSuitesRevision");
      }
    }

    internal virtual UpdatedProperties UpdateSuite(
      Guid projectGuid,
      ServerTestSuite suite,
      Guid updatedBy,
      IEnumerable<TestCaseAndOwner> entries,
      TestSuiteSource type)
    {
      try
      {
        this.PrepareStoredProcedure("prc_UpdateSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", suite.ParentId);
        this.BindInt("@suiteId", suite.Id);
        this.BindStringPreserveNull("@title", suite.Title, 256, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@description", suite.Description, int.MaxValue, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@query", suite.ConvertedQueryString, int.MaxValue, SqlDbType.NVarChar);
        this.BindString("@witStatus", suite.Status, 256, false, SqlDbType.NVarChar);
        this.BindInt("@revision", suite.Revision);
        this.BindTestCaseAndOwnerTypeTable("@testCaseAndOwnerTable", entries);
        this.BindBoolean("@inheritConfigs", suite.InheritDefaultConfigurations);
        this.BindIdTypeTable("@configIdsTable", (IEnumerable<int>) suite.DefaultConfigurations);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindDateTime("@lastUpdated", suite.LastUpdated);
        this.BindByte("@clientType", (byte) type);
        this.BindNullableByte("@queryMigrationState", (byte) suite.QueryMigrationState, (byte) 2);
        this.ExecuteReader();
        return new UpdatedProperties()
        {
          Revision = suite.Revision,
          LastUpdated = suite.LastUpdated,
          LastUpdatedBy = updatedBy
        };
      }
      catch (SqlException ex)
      {
        this.HandleDuplicateSuiteEntryError(ex);
        throw;
      }
    }

    internal virtual void RepopulateSuiteEntries(
      Guid projectGuid,
      int suiteId,
      string lastError,
      IEnumerable<TestCaseAndOwner> entries,
      Guid updatedBy,
      TestSuiteSource type)
    {
      try
      {
        this.PrepareStoredProcedure("prc_RepopulateSuiteEntries");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@suiteId", suiteId);
        this.BindString("@lastError", lastError, 4096, true, SqlDbType.NVarChar);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindBoolean("@updateEntries", entries != null);
        this.BindByte("@clientType", (byte) type);
        this.BindTestCaseAndOwnerTypeTable("@testCaseAndOwnerTable", entries);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.HandleDuplicateSuiteEntryError(ex);
        throw;
      }
    }

    internal virtual List<TestPoint> RepopulateSuitesAndFetchTestPoints(
      Guid projectGuid,
      List<SuiteRepopulateInfo> suitesRepopulateInfo,
      Dictionary<int, List<TestCaseAndOwner>> entries,
      Guid updatedBy,
      TestSuiteSource type)
    {
      try
      {
        this.PrepareStoredProcedure("prc_RepopulateSuitesAndFetchTestPoints");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@clientType", (byte) type);
        this.BindSuiteTestCasesTypeTable("@suiteTestCasesTable", entries);
        this.BindSuitesRepopulateInfoTypeTable("@suiteRepopulateInfo", suitesRepopulateInfo);
        List<TestPoint> testPointList = new List<TestPoint>();
        TestPlanningDatabase.FetchTestPointsColumns testPointsColumns = new TestPlanningDatabase.FetchTestPointsColumns();
        SqlDataReader reader = this.ExecuteReader();
        do
        {
          while (reader.Read())
            testPointList.Add(testPointsColumns.bindTestPointForInlineTest(reader));
        }
        while (reader.NextResult());
        return testPointList;
      }
      catch (SqlException ex)
      {
        this.HandleDuplicateSuiteEntryError(ex);
        throw;
      }
    }

    internal virtual void ReorderSuitesForPlanAsPerSortedTitle(
      Guid projectGuid,
      int planId,
      Dictionary<int, List<int>> entries)
    {
      this.PrepareStoredProcedure("prc_ReorderSuitesForPlanAsPerSortedTitle");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@planId", planId);
      this.BindChildSuiteEntryTypeTable("@orderedSuites", entries);
      this.ExecuteNonQuery();
    }

    internal virtual void SetSuiteEntryConfigurations(
      Guid projectGuid,
      int suiteId,
      IEnumerable<TestCaseAndOwner> testCases,
      IEnumerable<int> configurationIds,
      Guid updatedBy,
      UpdatedProperties ret)
    {
      try
      {
        this.PrepareStoredProcedure("prc_SetSuiteEntryConfigurations");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", suiteId);
        this.BindTestCaseAndOwnerTypeTable("@testCaseAndOwnerTable", testCases);
        this.BindIdTypeTable("@configIdsTable", configurationIds);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindDateTime("@lastUpdated", ret.LastUpdated);
        this.BindInt("@suiteRevision", ret.Revision);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.HandleDuplicateSuiteEntryError(ex);
        throw;
      }
    }

    internal virtual void AssignTestPoints(
      Guid projectGuid,
      int suiteId,
      TestPointAssignment[] assignments,
      Guid updatedBy,
      string auditUser)
    {
      this.PrepareStoredProcedure("prc_AssignTestPoints");
      this.BindInt("@suiteId", suiteId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindTestPointAssignmentTypeTable("@assignmentsTable", (IEnumerable<TestPointAssignment>) assignments);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindString("@auditUser", auditUser, 128, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal virtual List<int> GetInUseConfigurationsForSuite(Guid projectGuid, int suiteId) => new List<int>();

    internal virtual List<int> GetInUseConfigurationsForTestCases(
      Guid projectGuid,
      Dictionary<int, List<int>> testCases)
    {
      return new List<int>();
    }

    internal virtual List<SuiteIdAndType> QueryTestSuites(
      Guid projectGuid,
      Dictionary<string, List<object>> parametersMap)
    {
      return new List<SuiteIdAndType>();
    }

    internal virtual bool CleanDeletedTestSuites(int waitDaysForCleanup) => true;

    internal virtual bool CleanDeletedTestSuites(int waitDaysForCleanup, int deletionBatchSize) => true;

    internal virtual UpdatedProperties DeleteSuiteEntries2(
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<TestSuiteEntry> entries,
      out (int, List<int>) planPointIdsMap)
    {
      planPointIdsMap = (-1, (List<int>) null);
      return new UpdatedProperties();
    }

    internal virtual void DeleteTestPointsForSuites(
      Guid projectGuid,
      List<int> suiteIds,
      int deletionBatchSize)
    {
    }

    internal virtual List<int> DeleteTestPointsAndRunsForSuites(
      Guid projectGuid,
      Guid updatedBy,
      List<int> suiteIds,
      int deletionBatchSize,
      int deleteTestPointsAndRunsForSuitesSprocExecTimeOutInSec)
    {
      return new List<int>();
    }

    internal virtual void FetchTestRunsToBeDeletedForSuites(
      Guid projectGuid,
      List<int> suiteIds,
      int fetchTestRunsToBeDeletedForSuitesSprocExecTimeOutInSec,
      out (int, List<int>) planRunIds)
    {
      planRunIds = (int.MinValue, (List<int>) null);
    }

    internal virtual TestArtifactsAssociatedItemsModel QueryTestSuiteAssociatedTestArtifacts(
      TestManagementRequestContext context,
      Guid projectGuid,
      int testSuiteId,
      bool isTcmService,
      int pointQueryLimit)
    {
      context.TraceEnter("Database", "SuiteDatabase.QueryTestSuiteAssociatedTestArtifacts");
      this.PrepareStoredProcedure("prc_QueryTestSuiteAssociatedTestArtifacts");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@testSuiteId", testSuiteId);
      TestArtifactsAssociatedItemsModel associatedItemsModel = this.GetTestArtifactsAssociatedItemsModel(context, "prc_QueryTestSuiteAssociatedTestArtifacts");
      context.TraceLeave("Database", "SuiteDatabase.QueryTestSuiteAssociatedTestArtifacts");
      return associatedItemsModel;
    }

    protected void HandleDuplicateSuiteEntryError(SqlException e)
    {
      if (e.Number == TestPlanningDatabase.c_SqlErrorNumber_PKViolation && e.Message.Contains("@testCaseAndOwnerTable"))
        throw new TestManagementInvalidOperationException(ServerResources.CannotAddDuplicateTestCaseToSuite);
      this.MapException(e);
    }

    internal virtual void UpdateSuiteSyncStatus(
      Guid projectGuid,
      Dictionary<int, DateTime> suitesLastSyncedMap)
    {
    }

    internal virtual List<ServerTestSuite> FetchSuiteSyncStatus(
      Guid projectGuid,
      List<int> suiteIds)
    {
      return new List<ServerTestSuite>();
    }

    internal virtual List<TestSuiteRecord> QueryTestSuitesByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      toDate = fromDate;
      return new List<TestSuiteRecord>();
    }

    internal virtual Dictionary<int, ServerSuite> FetchServerSuites(
      TestManagementRequestContext context,
      IdAndRev[] suiteIds,
      List<int> deletedIds,
      bool includeTesters,
      out Dictionary<Guid, List<ServerSuite>> projectsSuitesMap)
    {
      throw new NotImplementedException();
    }

    internal virtual List<int> GetAssignedConfigurationsForSuite(Guid projectGuid, int suiteId)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestSuiteDatabase.GetAssignedConfigurationsForSuite"))
      {
        this.PrepareStoredProcedure("prc_QueryInUseConfigurationsForSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", suiteId);
        return this.GetConfigurationsFromDb();
      }
    }

    internal List<string> FetchProjectsApplicableForMigration(
      TfsTestManagementRequestContext context)
    {
      context.TraceEnter("Database", "UpgradeDatabase.FetchProjectsApplicableForMigration");
      List<string> stringList = new List<string>();
      this.PrepareStoredProcedure("prc_FetchProjectsApplicableForMigration");
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("ProjectName");
      while (reader.Read())
        stringList.Add(sqlColumnBinder.GetString((IDataReader) reader, false));
      context.TraceLeave("Database", "UpgradeDatabase.FetchProjectsApplicableForMigration");
      return stringList;
    }

    internal ProjectMigrationDetails GetProjectMigrationDetails(
      TestManagementRequestContext context,
      Guid projectGuid)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.GetProjectMigrationDetails");
        this.PrepareStoredProcedure("prc_GetProjectMigrationDetails");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        SqlDataReader reader = this.ExecuteReader();
        ProjectMigrationDetails migrationDetails = new ProjectMigrationDetails();
        if (reader.Read())
        {
          SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("MigrationState");
          migrationDetails.MigrationState = (UpgradeMigrationState) sqlColumnBinder1.GetInt32((IDataReader) reader);
          SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("MigrationError");
          migrationDetails.MigrationError = sqlColumnBinder2.GetString((IDataReader) reader, true);
        }
        if (reader.NextResult() && reader.Read())
        {
          SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("TotalPlansCount");
          migrationDetails.TotalPlansCount = sqlColumnBinder3.GetInt32((IDataReader) reader, 0);
          SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("MigratedPlansCount");
          migrationDetails.MigratedPlansCount = sqlColumnBinder4.GetInt32((IDataReader) reader, 0);
        }
        return migrationDetails;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.GetProjectMigrationDetails");
      }
    }

    internal List<int> FetchQBSApplicableForMigration(TfsTestManagementRequestContext context)
    {
      try
      {
        context.TraceEnter("Database", "UpgradeDatabase.FetchQBSApplicableForMigration");
        List<int> intList = new List<int>();
        this.PrepareStoredProcedure("prc_FetchQBSApplicableForMigration");
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("SuiteId");
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
        return intList;
      }
      finally
      {
        context.TraceLeave("Database", "UpgradeDatabase.FetchQBSApplicableForMigration");
      }
    }

    internal void UpdateQBSMigrationDetails(
      TestManagementRequestContext context,
      int qbsSuiteId,
      UpgradeMigrationState migrationState)
    {
      try
      {
        context.TraceEnter("Database", "UpgradeDatabase.UpdateQBSMigrationDetails");
        this.PrepareStoredProcedure("prc_UpdateQBSMigrationDetails");
        this.BindInt("@suiteId", qbsSuiteId);
        this.BindNullableByte("@queryMigrationState", (byte) migrationState, (byte) 2);
        this.ExecuteNonQuery();
      }
      finally
      {
        context.TraceLeave("Database", "UpgradeDatabase.UpdateQBSMigrationDetails");
      }
    }

    internal bool CleanDeletedTestPlansBeforeWITMigration()
    {
      this.PrepareStoredProcedure("prc_DeleteTestPlan_BeforeWITMigration");
      return (int) this.ExecuteScalar() == 0;
    }

    internal List<int> DeleteSessionsAssociatedWithDeletedPlans(
      Guid projectGuid,
      Guid lastUpdatedBy)
    {
      this.PrepareStoredProcedure("prc_DeleteSessionsAssociatedWithDeletedPlans");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
      List<int> intList = new List<int>();
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("SessionId");
      while (reader.Read())
        intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      return intList;
    }

    internal void UpdateProjectMigrationDetails(
      TestManagementRequestContext context,
      Guid projectGuid,
      UpgradeMigrationState migrationState,
      string migrationError)
    {
      try
      {
        context.TraceEnter("Database", "UpgradeDatabase.UpdateProjectMigrationDetails");
        this.PrepareStoredProcedure("prc_UpdateProjectMigrationDetails");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@migrationState", (int) migrationState);
        this.BindString("@migrationError", migrationError, int.MaxValue, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      finally
      {
        context.TraceLeave("Database", "UpgradeDatabase.UpdateProjectMigrationDetails");
      }
    }

    internal List<TestPlan> FetchPlansToBeMigratedOnWit(
      TestManagementRequestContext context,
      Guid projectGuid,
      int limit)
    {
      context.TraceEnter("Database", "TestPlanDatabase.FetchPlansToBeMigratedOnWit");
      this.PrepareStoredProcedure("prc_FetchDataToBeMigratedOnWit");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@limit", limit);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<int, TestPlan> dictionary = new Dictionary<int, TestPlan>();
      TestPlanningDatabase.FetchTestPlanDataColumns testPlanDataColumns = new TestPlanningDatabase.FetchTestPlanDataColumns();
      TestPlan testPlan = (TestPlan) null;
      while (reader.Read())
      {
        testPlan = testPlanDataColumns.bind(reader);
        context.TraceVerbose("Database", "Fetched plan details: {0}", (object) testPlan.ToString());
        dictionary.Add(testPlan.PlanId, testPlan);
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_FetchDataToBeMigratedOnWit");
      TestPlanningDatabase.ExternalLinkColumns externalLinkColumns = new TestPlanningDatabase.ExternalLinkColumns();
      while (reader.Read())
      {
        TestExternalLink testExternalLink = externalLinkColumns.bind(reader);
        if (dictionary.TryGetValue(testExternalLink.PlanId, out testPlan) && testPlan != null)
        {
          if (testPlan.Links == null)
            testPlan.Links = new List<TestExternalLink>();
          testPlan.Links.Add(testExternalLink);
        }
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_FetchDataToBeMigratedOnWit");
      TestPlanningDatabase.FetchTestSuitesColumnsForWitCreation columnsForWitCreation = new TestPlanningDatabase.FetchTestSuitesColumnsForWitCreation();
      List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
      while (reader.Read())
      {
        ServerTestSuite serverTestSuite = columnsForWitCreation.bind(reader);
        if (dictionary.TryGetValue(serverTestSuite.PlanId, out testPlan) && testPlan != null)
        {
          if (testPlan.SuitesMetaData == null)
            testPlan.SuitesMetaData = new List<ServerTestSuite>();
          testPlan.SuitesMetaData.Add(serverTestSuite);
        }
      }
      context.TraceLeave("Database", "TestPlanDatabase.FetchPlansToBeMigratedOnWit");
      return dictionary.Values.ToList<TestPlan>();
    }

    internal bool UpdateWitProperties(Guid projectGuid, TestPlan plan)
    {
      this.PrepareStoredProcedure("prc_UpdateWitProperties");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@tcmPlanId", plan.SourcePlanIdPreUpgrade);
      this.BindInt("@witPlanId", plan.PlanWorkItem.Id);
      this.BindServerTestSuiteTypeTable("@suiteMetaDataTable", (IEnumerable<ServerTestSuite>) plan.SuitesMetaData);
      bool flag = false;
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
        flag = Convert.ToBoolean(new SqlColumnBinder("IsPlanOffline").GetInt32((IDataReader) reader));
      return flag;
    }

    internal void UpdateTcmArtifactsAfterMigrationOnWit(
      TestManagementRequestContext context,
      MigrationLogger logger,
      Guid projectGuid,
      int tcmId,
      int witId,
      int limit)
    {
      int currentStepId = 0;
      do
      {
        logger.Log(TraceLevel.Info, string.Format("Replacing testplan and test suite identifiers with work item identifiers in referencing TCM artifacts. CurrentStepId: {0}", (object) currentStepId));
      }
      while (this.UpdateTcmArtifactsAfterMigrationOnWit(context, projectGuid, tcmId, witId, currentStepId, limit, out currentStepId) != UpgradeMigrationState.Completed);
    }

    private UpgradeMigrationState UpdateTcmArtifactsAfterMigrationOnWit(
      TestManagementRequestContext context,
      Guid projectGuid,
      int tcmId,
      int witId,
      int startingStep,
      int limit,
      out int currentStepId)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.UpdateTcmArtifactsAfterMigrationOnWit");
        this.PrepareStoredProcedure("prc_UpdateTcmArtifactsAfterMigrationOnWit");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@tcmId", tcmId);
        this.BindInt("@witId", witId);
        this.BindInt("@startingStep", startingStep);
        this.BindInt("@limit", limit);
        SqlDataReader reader = this.ExecuteReader();
        UpgradeMigrationState upgradeMigrationState = UpgradeMigrationState.NotStarted;
        currentStepId = 0;
        if (reader.Read())
        {
          upgradeMigrationState = (UpgradeMigrationState) new SqlColumnBinder("MigrationState").GetInt32((IDataReader) reader);
          if (upgradeMigrationState == UpgradeMigrationState.InProgress)
          {
            SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("StartingStepId");
            currentStepId = sqlColumnBinder.GetInt32((IDataReader) reader);
          }
        }
        return upgradeMigrationState;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.UpdateTcmArtifactsAfterMigrationOnWit");
      }
    }

    internal virtual void UpdateCloneOperationInformationAfterMigrationOnWit(
      TestManagementRequestContext context,
      Guid projectGuid)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.UpdateCloneOperationInformationAfterMigrationOnWit");
        this.PrepareStoredProcedure("prc_UpdateCloneOperationInformationAfterMigrationOnWit");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.ExecuteNonQuery();
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.UpdateCloneOperationInformationAfterMigrationOnWit");
      }
    }

    internal Dictionary<int, int> FetchTcmIdWitIdMap(
      TestManagementRequestContext context,
      Guid projectGuid,
      out Dictionary<int, int> suiteTcmIdWitIdMap)
    {
      this.PrepareStoredProcedure("prc_FetchTcmIdWitIdMap");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      suiteTcmIdWitIdMap = new Dictionary<int, int>();
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("PlanId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("SuiteId");
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("WitId");
      while (reader.Read())
      {
        int int32_1 = sqlColumnBinder1.GetInt32((IDataReader) reader);
        int int32_2 = sqlColumnBinder3.GetInt32((IDataReader) reader);
        dictionary[int32_1] = int32_2;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_FetchTcmIdWitIdMap");
      while (reader.Read())
      {
        int int32_3 = sqlColumnBinder2.GetInt32((IDataReader) reader);
        int int32_4 = sqlColumnBinder3.GetInt32((IDataReader) reader);
        suiteTcmIdWitIdMap[int32_3] = int32_4;
      }
      return dictionary;
    }

    internal void CreateIndexesBeforePlanMigration()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("preupd_MigrateTestPlansOnPrem.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
    }

    internal void DropIndexesAfterPlanMigration()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("postupd_MigrateTestPlansOnPrem.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
    }

    protected class BugFieldMappingColumns
    {
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));

      internal UpdatedProperties bind(SqlDataReader reader) => new UpdatedProperties()
      {
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader)
      };
    }

    protected class QueryBugFieldMappingColumns
    {
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder CreatedBy = new SqlColumnBinder(nameof (CreatedBy));
      private SqlColumnBinder FieldMapping = new SqlColumnBinder(nameof (FieldMapping));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));

      internal BugFieldMapping Bind(SqlDataReader reader) => new BugFieldMapping()
      {
        CreatedDate = this.CreatedDate.GetDateTime((IDataReader) reader),
        CreatedBy = this.CreatedBy.GetGuid((IDataReader) reader, false),
        FieldMapping = this.FieldMapping.GetString((IDataReader) reader, false),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false),
        Revision = this.Revision.GetInt32((IDataReader) reader)
      };
    }

    private class QueryTestControllerColumns
    {
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder GroupId = new SqlColumnBinder(nameof (GroupId));
      private SqlColumnBinder TimeSinceLastHeartbeat = new SqlColumnBinder(nameof (TimeSinceLastHeartbeat));

      internal TestController Bind(SqlDataReader reader) => new TestController()
      {
        Name = this.Name.GetString((IDataReader) reader, false),
        Description = this.Description.GetString((IDataReader) reader, true),
        GroupId = this.GroupId.GetString((IDataReader) reader, true),
        TimeSinceLastHeartbeat = this.TimeSinceLastHeartbeat.GetInt32((IDataReader) reader, 0)
      };
    }

    private class QueryTestControllerPropertyColumns
    {
      private SqlColumnBinder ControllerName = new SqlColumnBinder(nameof (ControllerName));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Value = new SqlColumnBinder(nameof (Value));

      internal NameValuePair bind(SqlDataReader reader, out string controllerName)
      {
        NameValuePair nameValuePair = new NameValuePair();
        controllerName = this.ControllerName.GetString((IDataReader) reader, false);
        nameValuePair.Name = this.Name.GetString((IDataReader) reader, false);
        nameValuePair.Value = this.Value.GetString((IDataReader) reader, true);
        return nameValuePair;
      }
    }

    private class QueryDataCollectorColumns
    {
      private SqlColumnBinder TypeUri = new SqlColumnBinder(nameof (TypeUri));
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder DefaultConfiguration = new SqlColumnBinder(nameof (DefaultConfiguration));
      private SqlColumnBinder ConfigurationEditorConfiguration = new SqlColumnBinder(nameof (ConfigurationEditorConfiguration));

      internal DataCollectorInformation Bind(SqlDataReader reader) => new DataCollectorInformation()
      {
        TypeUri = this.TypeUri.GetString((IDataReader) reader, false),
        Id = this.Id.GetGuid((IDataReader) reader),
        Description = this.Description.GetString((IDataReader) reader, true),
        DefaultConfiguration = this.PropertyToXmlNode(this.DefaultConfiguration.GetString((IDataReader) reader, true)),
        ConfigurationEditorConfiguration = this.PropertyToXmlNode(this.ConfigurationEditorConfiguration.GetString((IDataReader) reader, true))
      };

      private XmlNode PropertyToXmlNode(string xmlString) => !string.IsNullOrEmpty(xmlString) ? (XmlNode) Microsoft.TeamFoundation.TestManagement.Common.Internal.XmlUtility.LoadXmlDocumentFromString(xmlString).DocumentElement : (XmlNode) null;
    }

    private class QueryDataCollectorPropertyColumns
    {
      private SqlColumnBinder TypeUri = new SqlColumnBinder(nameof (TypeUri));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Value = new SqlColumnBinder(nameof (Value));

      internal NameValuePair bind(SqlDataReader reader, out string typeUri)
      {
        NameValuePair nameValuePair = new NameValuePair();
        typeUri = this.TypeUri.GetString((IDataReader) reader, false);
        nameValuePair.Name = this.Name.GetString((IDataReader) reader, false);
        nameValuePair.Value = this.Value.GetString((IDataReader) reader, true);
        return nameValuePair;
      }
    }

    protected class ImpactedPointsColumns
    {
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder SuiteTitle = new SqlColumnBinder(nameof (SuiteTitle));

      internal ImpactedPoint Bind(SqlDataReader reader) => new ImpactedPoint()
      {
        BuildUri = this.BuildUri.GetString((IDataReader) reader, false),
        TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader),
        PointId = this.PointId.GetInt32((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        SuiteName = this.SuiteTitle.GetString((IDataReader) reader, false)
      };
    }

    protected class QueryTestCasesInPlansColumns
    {
      internal SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
    }

    protected class UpdateTestPlansColumns
    {
      internal SqlColumnBinder PreviousBuildUri = new SqlColumnBinder(nameof (PreviousBuildUri));
      internal SqlColumnBinder BuildTakenDate = new SqlColumnBinder(nameof (BuildTakenDate));
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
    }

    protected class TestPointStatisticColumns
    {
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));
      private SqlColumnBinder PointCount = new SqlColumnBinder(nameof (PointCount));

      internal TestPointStatistic Bind(SqlDataReader reader) => new TestPointStatistic()
      {
        TestPointState = this.State.GetByte((IDataReader) reader),
        ResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
        ResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
        FailureType = this.FailureType.GetByte((IDataReader) reader, (byte) 0),
        ResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader),
        Count = this.PointCount.GetInt32((IDataReader) reader)
      };
    }

    protected class UpdatedPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));
      internal SqlColumnBinder IsRunCompleted = new SqlColumnBinder(nameof (IsRunCompleted));
      internal SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      internal SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));

      internal BlockedPointProperties bindBlockedTestPointProperties(SqlDataReader reader)
      {
        BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
        blockedPointProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        blockedPointProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        blockedPointProperties.LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader);
        return blockedPointProperties;
      }

      internal UpdatedProperties bindUpdatedProperties(SqlDataReader reader) => new UpdatedProperties()
      {
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader)
      };
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

    private class TestPointStatisticColumnsBySuiteId
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));
      private SqlColumnBinder PointCount = new SqlColumnBinder(nameof (PointCount));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));

      internal TestPointStatistic Bind(SqlDataReader reader) => new TestPointStatistic()
      {
        TestPointState = this.State.GetByte((IDataReader) reader),
        ResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
        ResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
        FailureType = this.FailureType.GetByte((IDataReader) reader, (byte) 0),
        ResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader),
        Count = this.PointCount.GetInt32((IDataReader) reader)
      };

      internal Dictionary<string, List<TestPointStatistic>> BindByPivot(SqlDataReader reader)
      {
        Dictionary<string, List<TestPointStatistic>> dictionary = new Dictionary<string, List<TestPointStatistic>>();
        while (reader.Read())
        {
          string key = this.SuiteId.GetInt32((IDataReader) reader).ToString();
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, new List<TestPointStatistic>()
            {
              this.Bind(reader)
            });
          else
            dictionary[key].Add(this.Bind(reader));
        }
        return dictionary;
      }
    }

    private class TestPointStatisticColumnsByTester
    {
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));
      private SqlColumnBinder PointCount = new SqlColumnBinder(nameof (PointCount));

      internal TestPointStatistic Bind(SqlDataReader reader) => new TestPointStatistic()
      {
        TestPointState = this.State.GetByte((IDataReader) reader),
        ResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
        ResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
        FailureType = this.FailureType.GetByte((IDataReader) reader, (byte) 0),
        ResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader),
        Count = this.PointCount.GetInt32((IDataReader) reader)
      };

      internal Dictionary<string, List<TestPointStatistic>> BindByPivot(SqlDataReader reader)
      {
        Dictionary<string, List<TestPointStatistic>> dictionary = new Dictionary<string, List<TestPointStatistic>>();
        while (reader.Read())
        {
          string key = this.AssignedTo.GetGuid((IDataReader) reader, false).ToString();
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, new List<TestPointStatistic>()
            {
              this.Bind(reader)
            });
          else
            dictionary[key].Add(this.Bind(reader));
        }
        return dictionary;
      }
    }

    private class ExternalLinkColumns
    {
      private SqlColumnBinder LinkId = new SqlColumnBinder(nameof (LinkId));
      private SqlColumnBinder Uri = new SqlColumnBinder(nameof (Uri));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));

      internal TestExternalLink bind(SqlDataReader reader) => new TestExternalLink()
      {
        LinkId = this.LinkId.GetInt32((IDataReader) reader),
        Description = this.Description.GetString((IDataReader) reader, false),
        Uri = this.Uri.GetString((IDataReader) reader, false)
      };
    }

    protected class TestArtifactsAssociatedItems
    {
      internal SqlColumnBinder SuitesCount = new SqlColumnBinder(nameof (SuitesCount));
      internal SqlColumnBinder PointHistoryCount = new SqlColumnBinder(nameof (PointHistoryCount));
      internal SqlColumnBinder PointCount = new SqlColumnBinder(nameof (PointCount));
      internal SqlColumnBinder TestResultsCount = new SqlColumnBinder(nameof (TestResultsCount));
      internal SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
    }

    internal class FetchTestPlanHubDataColumnsBase
    {
      internal virtual TestPlanHubData bind(
        SqlDataReader reader,
        string storedProcedure,
        bool includeTestPoints)
      {
        return new TestPlanHubData();
      }
    }

    private class QuerySuitePointCountsColumns
    {
      internal SqlColumnBinder SuitePath = new SqlColumnBinder(nameof (SuitePath));
      internal SqlColumnBinder PointCount = new SqlColumnBinder(nameof (PointCount));
    }

    protected class FetchTester
    {
      internal SqlColumnBinder Tester = new SqlColumnBinder("AssignedTo");

      internal Guid bind(SqlDataReader reader) => this.Tester.GetGuid((IDataReader) reader);
    }

    private class FetchTestPlanDataColumns
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
      private SqlColumnBinder EndDate = new SqlColumnBinder(nameof (EndDate));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder Iteration = new SqlColumnBinder(nameof (Iteration));
      private SqlColumnBinder AreaPath = new SqlColumnBinder(nameof (AreaPath));
      private SqlColumnBinder AreaUri = new SqlColumnBinder(nameof (AreaUri));
      private SqlColumnBinder BuildDefinition = new SqlColumnBinder(nameof (BuildDefinition));
      private SqlColumnBinder BuildQuality = new SqlColumnBinder(nameof (BuildQuality));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder PreviousBuildUri = new SqlColumnBinder(nameof (PreviousBuildUri));
      private SqlColumnBinder BuildTakenDate = new SqlColumnBinder(nameof (BuildTakenDate));
      private SqlColumnBinder TestSettingsId = new SqlColumnBinder(nameof (TestSettingsId));
      private SqlColumnBinder AutomatedTestSettingsId = new SqlColumnBinder(nameof (AutomatedTestSettingsId));
      private SqlColumnBinder ManualTestEnvironmentId = new SqlColumnBinder(nameof (ManualTestEnvironmentId));
      private SqlColumnBinder AutomatedTestEnvironmentId = new SqlColumnBinder(nameof (AutomatedTestEnvironmentId));
      private SqlColumnBinder RootSuiteId = new SqlColumnBinder(nameof (RootSuiteId));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder ProjectName = new SqlColumnBinder(nameof (ProjectName));
      private SqlColumnBinder WitId = new SqlColumnBinder(nameof (WitId));
      private SqlColumnBinder MigrationState = new SqlColumnBinder(nameof (MigrationState));

      internal TestPlan bind(SqlDataReader reader)
      {
        TestPlan testPlan = new TestPlan()
        {
          PlanId = this.PlanId.GetInt32((IDataReader) reader)
        };
        testPlan.SourcePlanIdPreUpgrade = testPlan.PlanId;
        testPlan.Name = this.Name.GetString((IDataReader) reader, false);
        testPlan.Description = this.Description.GetString((IDataReader) reader, false);
        testPlan.Owner = this.Owner.GetGuid((IDataReader) reader, false);
        testPlan.StartDate = this.StartDate.GetDateTime((IDataReader) reader);
        testPlan.EndDate = this.EndDate.GetDateTime((IDataReader) reader);
        testPlan.State = this.State.GetByte((IDataReader) reader);
        string databasePath1 = this.AreaPath.GetString((IDataReader) reader, true);
        if (databasePath1 != null)
          testPlan.AreaPath = DBPath.DatabaseToUserPath(databasePath1, false, false);
        testPlan.AreaUri = this.AreaUri.GetString((IDataReader) reader, true);
        string databasePath2 = this.Iteration.GetString((IDataReader) reader, true);
        if (databasePath2 != null)
          testPlan.Iteration = DBPath.DatabaseToUserPath(databasePath2, false, false);
        testPlan.BuildDefinition = this.BuildDefinition.GetString((IDataReader) reader, true);
        testPlan.BuildQuality = this.BuildQuality.GetString((IDataReader) reader, true);
        testPlan.BuildUri = this.BuildUri.GetString((IDataReader) reader, false);
        testPlan.PreviousBuildUri = this.PreviousBuildUri.GetString((IDataReader) reader, true);
        testPlan.BuildTakenDate = this.BuildTakenDate.GetDateTime((IDataReader) reader);
        testPlan.TestSettingsId = this.TestSettingsId.GetInt32((IDataReader) reader);
        testPlan.AutomatedTestSettingsId = this.AutomatedTestSettingsId.GetInt32((IDataReader) reader);
        testPlan.ManualTestEnvironmentId = this.ManualTestEnvironmentId.GetGuid((IDataReader) reader, false);
        testPlan.AutomatedTestEnvironmentId = this.AutomatedTestEnvironmentId.GetGuid((IDataReader) reader, false);
        testPlan.Revision = this.Revision.GetInt32((IDataReader) reader);
        testPlan.RootSuiteId = this.RootSuiteId.GetInt32((IDataReader) reader);
        testPlan.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testPlan.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testPlan.PlanWorkItem.TeamProjectName = this.ProjectName.GetString((IDataReader) reader, true);
        testPlan.PlanWorkItem.Id = this.WitId.GetInt32((IDataReader) reader);
        testPlan.MigrationState = (UpgradeMigrationState) this.MigrationState.GetInt32((IDataReader) reader);
        return testPlan;
      }
    }

    protected class FetchTestPlansColumns
    {
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder BuildDefinition = new SqlColumnBinder(nameof (BuildDefinition));
      private SqlColumnBinder BuildQuality = new SqlColumnBinder(nameof (BuildQuality));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder PreviousBuildUri = new SqlColumnBinder(nameof (PreviousBuildUri));
      private SqlColumnBinder BuildTakenDate = new SqlColumnBinder(nameof (BuildTakenDate));
      private SqlColumnBinder TestSettingsId = new SqlColumnBinder(nameof (TestSettingsId));
      private SqlColumnBinder AutomatedTestSettingsId = new SqlColumnBinder(nameof (AutomatedTestSettingsId));
      private SqlColumnBinder ManualTestEnvironmentId = new SqlColumnBinder(nameof (ManualTestEnvironmentId));
      private SqlColumnBinder AutomatedTestEnvironmentId = new SqlColumnBinder(nameof (AutomatedTestEnvironmentId));
      private SqlColumnBinder RootSuiteId = new SqlColumnBinder(nameof (RootSuiteId));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder ReleaseDefinitionId = new SqlColumnBinder(nameof (ReleaseDefinitionId));
      private SqlColumnBinder ReleaseEnvDefinitionId = new SqlColumnBinder(nameof (ReleaseEnvDefinitionId));

      internal TestPlan bind(SqlDataReader reader)
      {
        TestPlan testPlan = new TestPlan();
        testPlan.PlanId = this.PlanId.GetInt32((IDataReader) reader);
        testPlan.BuildDefinition = this.BuildDefinition.GetString((IDataReader) reader, true);
        testPlan.BuildQuality = this.BuildQuality.GetString((IDataReader) reader, true);
        testPlan.BuildUri = this.BuildUri.GetString((IDataReader) reader, false);
        testPlan.PreviousBuildUri = this.PreviousBuildUri.GetString((IDataReader) reader, true);
        testPlan.BuildTakenDate = this.BuildTakenDate.GetDateTime((IDataReader) reader);
        testPlan.TestSettingsId = this.TestSettingsId.GetInt32((IDataReader) reader);
        testPlan.AutomatedTestSettingsId = this.AutomatedTestSettingsId.GetInt32((IDataReader) reader);
        testPlan.ManualTestEnvironmentId = this.ManualTestEnvironmentId.GetGuid((IDataReader) reader, false);
        testPlan.AutomatedTestEnvironmentId = this.AutomatedTestEnvironmentId.GetGuid((IDataReader) reader, false);
        testPlan.RootSuiteId = this.RootSuiteId.GetInt32((IDataReader) reader);
        testPlan.BuildDefinitionId = this.BuildDefinitionId.ColumnExists((IDataReader) reader) ? this.BuildDefinitionId.GetInt32((IDataReader) reader) : 0;
        int int32_1 = this.ReleaseDefinitionId.ColumnExists((IDataReader) reader) ? this.ReleaseDefinitionId.GetInt32((IDataReader) reader) : 0;
        int int32_2 = this.ReleaseEnvDefinitionId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvDefinitionId.GetInt32((IDataReader) reader) : 0;
        if (int32_1 > 0 && int32_2 > 0)
          testPlan.ReleaseEnvDef = new ReleaseEnvironmentDefinition()
          {
            ReleaseDefinitionId = int32_1,
            ReleaseEnvDefinitionId = int32_2
          };
        return testPlan;
      }
    }

    private class QueryTestPointColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));

      internal TestPoint bind(SqlDataReader reader) => new TestPoint()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader)
      };
    }

    protected class FetchTestPointsColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder LastTestRunId = new SqlColumnBinder(nameof (LastTestRunId));
      private SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));
      private SqlColumnBinder SuiteName = new SqlColumnBinder(nameof (SuiteName));
      private SqlColumnBinder SequenceNumber = new SqlColumnBinder(nameof (SequenceNumber));
      private SqlColumnBinder LastResetToActive = new SqlColumnBinder(nameof (LastResetToActive));

      internal TestPoint bind(SqlDataReader reader, bool includeSuiteName = false)
      {
        TestPoint testPoint = new TestPoint();
        testPoint.PointId = this.PointId.GetInt32((IDataReader) reader);
        testPoint.PlanId = this.PlanId.GetInt32((IDataReader) reader);
        testPoint.SuiteId = this.SuiteId.GetInt32((IDataReader) reader);
        testPoint.ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader);
        testPoint.ConfigurationName = this.ConfigurationName.GetString((IDataReader) reader, true);
        testPoint.State = this.State.GetByte((IDataReader) reader);
        testPoint.FailureType = this.FailureType.GetByte((IDataReader) reader, (byte) 0);
        testPoint.LastTestRunId = this.LastTestRunId.GetInt32((IDataReader) reader);
        testPoint.LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader);
        testPoint.LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0);
        testPoint.LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0);
        testPoint.TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader);
        testPoint.Comment = this.Comment.GetString((IDataReader) reader, true);
        testPoint.AssignedTo = this.AssignedTo.GetGuid((IDataReader) reader, false);
        testPoint.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testPoint.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testPoint.Revision = this.Revision.GetInt32((IDataReader) reader);
        testPoint.LastResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader, 0);
        testPoint.LastResetToActive = this.LastResetToActive.ColumnExists((IDataReader) reader) ? this.LastResetToActive.GetDateTime((IDataReader) reader) : DateTime.MinValue;
        testPoint.SequenceNumber = this.SequenceNumber.GetInt32((IDataReader) reader, -1, -1);
        if (includeSuiteName)
          testPoint.SuiteName = this.SuiteName.GetString((IDataReader) reader, true);
        return testPoint;
      }

      internal TestPoint bindTestPointForInlineTest(SqlDataReader reader) => new TestPoint()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        PlanId = this.PlanId.GetInt32((IDataReader) reader),
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
        LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
        TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader),
        LastResetToActive = this.LastResetToActive.ColumnExists((IDataReader) reader) ? this.LastResetToActive.GetDateTime((IDataReader) reader) : DateTime.MinValue,
        LastUpdated = this.LastUpdated.ColumnExists((IDataReader) reader) ? this.LastUpdated.GetDateTime((IDataReader) reader) : DateTime.MinValue,
        SequenceNumber = this.SequenceNumber.GetInt32((IDataReader) reader, -1, -1)
      };
    }

    protected class QueryTestPointHistoryColumns
    {
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder FailureType = new SqlColumnBinder(nameof (FailureType));
      private SqlColumnBinder LastTestRunId = new SqlColumnBinder(nameof (LastTestRunId));
      private SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      private SqlColumnBinder LastResultState = new SqlColumnBinder(nameof (LastResultState));
      private SqlColumnBinder LastResultOutcome = new SqlColumnBinder(nameof (LastResultOutcome));
      private SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastResolutionStateId = new SqlColumnBinder(nameof (LastResolutionStateId));

      internal TestPoint bind(SqlDataReader reader) => new TestPoint()
      {
        PointId = this.PointId.GetInt32((IDataReader) reader),
        PlanId = this.PlanId.GetInt32((IDataReader) reader),
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader),
        State = this.State.GetByte((IDataReader) reader),
        FailureType = this.FailureType.GetByte((IDataReader) reader, (byte) 0),
        LastTestRunId = this.LastTestRunId.GetInt32((IDataReader) reader),
        LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader),
        LastResultState = this.LastResultState.GetByte((IDataReader) reader, (byte) 0),
        LastResultOutcome = this.LastResultOutcome.GetByte((IDataReader) reader, (byte) 0),
        TestCaseId = this.TestCaseId.GetInt32((IDataReader) reader),
        Comment = this.Comment.GetString((IDataReader) reader, true),
        AssignedTo = this.AssignedTo.GetGuid((IDataReader) reader, false),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader),
        LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false),
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastResolutionStateId = this.LastResolutionStateId.GetInt32((IDataReader) reader, 0)
      };
    }

    protected class PlanIdColumn
    {
      internal SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
    }

    protected class FetchCloneInformationColumns
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

      internal CloneOperationInformation bind(
        IVssRequestContext context,
        SqlDataReader reader,
        out int sourceDataspaceId)
      {
        CloneOperationInformation operationInformation = new CloneOperationInformation();
        operationInformation.OpId = this.OpId.GetInt32((IDataReader) reader);
        operationInformation.ResultObjectType = (ResultObjectType) this.ResultObjectType.GetByte((IDataReader) reader);
        operationInformation.LinkComment = this.LinkComment.GetString((IDataReader) reader, true);
        string workItemType;
        operationInformation.EditFieldDetails = TestPlanningDatabase.XmlToDictionaryOfOverriddenFieldDetails(context, this.EditFieldDetails.GetString((IDataReader) reader, true), out workItemType);
        operationInformation.DestinationWorkItemType = workItemType;
        operationInformation.State = (CloneOperationState) this.State.GetByte((IDataReader) reader);
        operationInformation.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        operationInformation.TeamFoundationUserId = this.CreatedBy.GetGuid((IDataReader) reader);
        operationInformation.CompletionDate = this.CompletionDate.GetDateTime((IDataReader) reader, DateTime.MaxValue);
        operationInformation.Message = this.Message.GetString((IDataReader) reader, true);
        operationInformation.TotalTestCaseCount = this.TotalTestCaseCount.GetInt32((IDataReader) reader);
        operationInformation.TotalRequirementsCount = this.TotalRequirementsCount.GetInt32((IDataReader) reader);
        operationInformation.ClonedTestCaseCount = this.TestCasesCloned.GetInt32((IDataReader) reader);
        operationInformation.ClonedRequirementsCount = this.RequirementsCloned.GetInt32((IDataReader) reader);
        operationInformation.ClonedSharedStepCount = this.SharedStepsCloned.GetInt32((IDataReader) reader);
        operationInformation.ResultObjectName = this.ResultObjectName.GetString((IDataReader) reader, true) ?? string.Empty;
        operationInformation.ResultObjectId = this.ResultObjectId.GetInt32((IDataReader) reader, -1);
        operationInformation.SourceObjectName = this.SourceObjectName.GetString((IDataReader) reader, true) ?? string.Empty;
        operationInformation.SourceObjectId = this.SourceObjectId.GetInt32((IDataReader) reader);
        operationInformation.TargetPlanName = this.TargetPlanName.GetString((IDataReader) reader, false);
        operationInformation.TargetPlanId = this.TargetPlanId.GetInt32((IDataReader) reader);
        operationInformation.SourcePlanName = this.SourcePlanName.GetString((IDataReader) reader, true) ?? string.Empty;
        operationInformation.SourcePlanId = this.SourcePlanId.GetInt32((IDataReader) reader);
        sourceDataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return operationInformation;
      }
    }

    protected class FetchCloneInformationColumnsForTestCase
    {
      private SqlColumnBinder OpId = new SqlColumnBinder(nameof (OpId));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder ResultObjectType = new SqlColumnBinder(nameof (ResultObjectType));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LinkComment = new SqlColumnBinder(nameof (LinkComment));
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

      internal CloneTestCaseOperationInformation bind(
        IVssRequestContext context,
        SqlDataReader reader,
        out int sourceDataspaceId)
      {
        CloneTestCaseOperationInformation operationInformation = new CloneTestCaseOperationInformation();
        CloneOperationCommonResponse operationCommonResponse = new CloneOperationCommonResponse();
        operationCommonResponse.opId = this.OpId.GetInt32((IDataReader) reader);
        switch ((ResultObjectType) this.ResultObjectType.GetByte((IDataReader) reader))
        {
          case ResultObjectType.TestSuite:
          case ResultObjectType.TestPlan:
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CloneOperationNotFoundForTestCase, (object) operationCommonResponse.opId)).Expected("Test Management");
          default:
            operationCommonResponse.creationDate = this.CreationDate.GetDateTime((IDataReader) reader);
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
            Microsoft.TeamFoundation.TestManagement.WebApi.CloneTestCaseOptions cloneTestCaseOptions = new Microsoft.TeamFoundation.TestManagement.WebApi.CloneTestCaseOptions();
            cloneTestCaseOptions.RelatedLinkComment = this.LinkComment.GetString((IDataReader) reader, true);
            SourceTestSuiteResponse testSuiteResponse = new SourceTestSuiteResponse();
            testSuiteResponse.Id = this.SourceObjectId.GetInt32((IDataReader) reader);
            TestSuiteReferenceWithProject referenceWithProject = new TestSuiteReferenceWithProject();
            referenceWithProject.Id = this.ResultObjectId.GetInt32((IDataReader) reader);
            operationCommonResponse.creationDate = this.CreationDate.GetDateTime((IDataReader) reader);
            operationInformation.cloneOperationResponse = operationCommonResponse;
            operationInformation.cloneOptions = cloneTestCaseOptions;
            operationInformation.sourceTestSuite = testSuiteResponse;
            operationInformation.destinationTestSuite = referenceWithProject;
            sourceDataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
            return operationInformation;
        }
      }
    }

    protected class CreateTestPlanColumns
    {
      internal SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder RootSuiteId = new SqlColumnBinder(nameof (RootSuiteId));
      internal SqlColumnBinder BuildTakenDate = new SqlColumnBinder(nameof (BuildTakenDate));
    }

    private class QueryAreaUriColumns
    {
      internal SqlColumnBinder AreaUri = new SqlColumnBinder(nameof (AreaUri));
    }

    private class QuerySessionColumns
    {
      private SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder Controller = new SqlColumnBinder(nameof (Controller));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder Guid = new SqlColumnBinder(nameof (Guid));
      private SqlColumnBinder TestSettingsId = new SqlColumnBinder(nameof (TestSettingsId));
      private SqlColumnBinder PublicTestSettingsId = new SqlColumnBinder(nameof (PublicTestSettingsId));
      private SqlColumnBinder TestEnvironmentId = new SqlColumnBinder(nameof (TestEnvironmentId));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder BugsCount = new SqlColumnBinder(nameof (BugsCount));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder Sprint = new SqlColumnBinder(nameof (Sprint));
      private SqlColumnBinder ComputerName = new SqlColumnBinder(nameof (ComputerName));
      private SqlColumnBinder UserStoryId = new SqlColumnBinder(nameof (UserStoryId));
      private SqlColumnBinder CharterId = new SqlColumnBinder(nameof (CharterId));
      private SqlColumnBinder FeedbackId = new SqlColumnBinder(nameof (FeedbackId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder SessionNotes = new SqlColumnBinder("Notes");
      private SqlColumnBinder SessionBookmarks = new SqlColumnBinder("Bookmarks");
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));

      internal Session Bind(IVssRequestContext context, SqlDataReader reader, out int dataspaceId)
      {
        Session session = new Session();
        session.SessionId = this.SessionId.GetInt32((IDataReader) reader);
        session.Title = this.Title.GetString((IDataReader) reader, false);
        session.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        session.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        session.Owner = this.Owner.GetGuid((IDataReader) reader, false);
        session.State = this.State.GetByte((IDataReader) reader);
        session.BuildUri = this.BuildUri.GetString((IDataReader) reader, true);
        session.BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
        session.StartDate = this.StartDate.GetDateTime((IDataReader) reader);
        session.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        session.Controller = this.Controller.GetString((IDataReader) reader, true);
        session.TestPlanId = this.TestPlanId.GetInt32((IDataReader) reader);
        session.TestSettingsId = this.TestSettingsId.GetInt32((IDataReader) reader);
        session.PublicTestSettingsId = this.PublicTestSettingsId.GetInt32((IDataReader) reader);
        session.TestEnvironmentId = this.TestEnvironmentId.GetGuid((IDataReader) reader);
        session.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        session.Revision = this.Revision.GetInt32((IDataReader) reader);
        session.Comment = this.Comment.GetString((IDataReader) reader, true);
        session.BugsCount = this.BugsCount.GetInt32((IDataReader) reader);
        session.Duration = this.Duration.GetInt64((IDataReader) reader);
        session.Sprint = this.Sprint.GetString((IDataReader) reader, true);
        session.ComputerName = this.ComputerName.GetString((IDataReader) reader, true);
        session.UserStoryId = this.UserStoryId.GetInt32((IDataReader) reader);
        session.CharterId = this.CharterId.GetInt32((IDataReader) reader);
        session.FeedbackId = this.FeedbackId.GetInt32((IDataReader) reader);
        session.ConfigurationId = this.ConfigurationId.GetInt32((IDataReader) reader);
        session.ConfigurationName = this.ConfigurationName.GetString((IDataReader) reader, true);
        TestPlanningDatabase.XmlToNotes(context, this.SessionNotes.GetString((IDataReader) reader, true), session.Notes);
        TestPlanningDatabase.XmlToBookmarks(context, this.SessionBookmarks.GetString((IDataReader) reader, true), session.Bookmarks);
        session.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader);
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return session;
      }
    }

    internal class CreateSessionColumns
    {
      internal SqlColumnBinder sessionId = new SqlColumnBinder("SessionId");
      internal SqlColumnBinder revision = new SqlColumnBinder("Revision");

      internal Session bind(SqlDataReader reader)
      {
        Session session = new Session();
        session.SessionId = this.sessionId.GetInt32((IDataReader) reader);
        session.Revision = this.revision.GetInt32((IDataReader) reader);
        return session;
      }
    }

    protected class UpdatedSessionPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder IsSessionStarted = new SqlColumnBinder(nameof (IsSessionStarted));

      internal UpdatedSessionProperties bind(SqlDataReader reader)
      {
        UpdatedSessionProperties sessionProperties = new UpdatedSessionProperties();
        sessionProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        sessionProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        sessionProperties.IsSessionStarted = this.IsSessionStarted.GetBoolean((IDataReader) reader);
        return sessionProperties;
      }
    }

    internal class QueryTestSessionColumns
    {
      private SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder Source = new SqlColumnBinder(nameof (Source));
      private SqlColumnBinder FeedbackId = new SqlColumnBinder(nameof (FeedbackId));

      internal TestSession bind(TestManagementRequestContext context, SqlDataReader reader)
      {
        TestSession testSession = new TestSession();
        testSession.Id = this.SessionId.GetInt32((IDataReader) reader);
        testSession.Title = this.Title.GetString((IDataReader) reader, false);
        testSession.StartDate = this.CreationDate.GetDateTime((IDataReader) reader);
        Guid guid = this.Owner.GetGuid((IDataReader) reader, false);
        testSession.Owner = new IdentityRef()
        {
          Id = guid.ToString()
        };
        testSession.State = (TestSessionState) this.State.GetByte((IDataReader) reader);
        testSession.EndDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        testSession.Revision = this.Revision.GetInt32((IDataReader) reader);
        testSession.Source = (TestSessionSource) this.Source.GetInt32((IDataReader) reader);
        if (this.FeedbackId.ColumnExists((IDataReader) reader))
          testSession.PropertyBag.AddOrUpdateProperties("FeedbackRequestId", this.FeedbackId.GetInt32((IDataReader) reader).ToString());
        return testSession;
      }
    }

    internal class ReadAssociatedWorkItems
    {
      private SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      private SqlColumnBinder WorkItemUri = new SqlColumnBinder(nameof (WorkItemUri));

      internal void bind(
        TestManagementRequestContext context,
        SqlDataReader reader,
        ref List<int> workItemRefListForSession,
        ref Dictionary<int, List<TestSessionWorkItemReference>> sessionIdToListOfWorkItemRef)
      {
        int result = 0;
        List<TestSessionWorkItemReference> workItemReferenceList = new List<TestSessionWorkItemReference>();
        while (reader.Read())
        {
          TestSessionWorkItemReference workItemReference = new TestSessionWorkItemReference();
          int int32 = this.SessionId.GetInt32((IDataReader) reader);
          int.TryParse(this.WorkItemUri.GetString((IDataReader) reader, false).Substring(35), out result);
          workItemReference.Id = result;
          if (int32 > 0 && result > 0)
            workItemRefListForSession.Add(workItemReference.Id);
          sessionIdToListOfWorkItemRef.TryGetValue(int32, out workItemReferenceList);
          if (workItemReferenceList == null)
            workItemReferenceList = new List<TestSessionWorkItemReference>();
          if (int32 > 0 && result > 0)
          {
            workItemReferenceList.Add(workItemReference);
            sessionIdToListOfWorkItemRef[int32] = workItemReferenceList;
          }
        }
      }
    }

    internal class ReadExploredWorkItems
    {
      private SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder ExploredStartTime = new SqlColumnBinder(nameof (ExploredStartTime));
      private SqlColumnBinder ExploredEndTime = new SqlColumnBinder(nameof (ExploredEndTime));

      internal void bind(
        TestManagementRequestContext context,
        SqlDataReader reader,
        ref List<int> exploredItemRefListForSession,
        ref Dictionary<int, List<TestSessionExploredWorkItemReference>> sessionIdToListOfExploredItemRef)
      {
        List<TestSessionExploredWorkItemReference> workItemReferenceList = new List<TestSessionExploredWorkItemReference>();
        while (reader.Read())
        {
          TestSessionExploredWorkItemReference workItemReference = new TestSessionExploredWorkItemReference();
          int int32 = this.SessionId.GetInt32((IDataReader) reader);
          workItemReference.Id = this.WorkItemId.GetInt32((IDataReader) reader);
          workItemReference.StartTime = this.ExploredStartTime.GetDateTime((IDataReader) reader);
          workItemReference.EndTime = this.ExploredEndTime.GetDateTime((IDataReader) reader);
          exploredItemRefListForSession.Add(workItemReference.Id);
          sessionIdToListOfExploredItemRef.TryGetValue(int32, out workItemReferenceList);
          if (workItemReferenceList == null)
            workItemReferenceList = new List<TestSessionExploredWorkItemReference>();
          if (int32 > 0 && workItemReference.Id > 0)
          {
            workItemReferenceList.Add(workItemReference);
            sessionIdToListOfExploredItemRef[int32] = workItemReferenceList;
          }
        }
      }
    }

    private class PropertyColumn
    {
      private SqlColumnBinder PropertyId = new SqlColumnBinder(nameof (PropertyId));
      private SqlColumnBinder Value = new SqlColumnBinder(nameof (Value));

      internal List<KeyValuePair<TcmProperty, int>> Bind(SqlDataReader reader)
      {
        List<KeyValuePair<TcmProperty, int>> keyValuePairList = new List<KeyValuePair<TcmProperty, int>>();
        while (reader.Read())
        {
          KeyValuePair<TcmProperty, int> keyValuePair = new KeyValuePair<TcmProperty, int>((TcmProperty) this.PropertyId.GetInt32((IDataReader) reader), this.Value.GetInt32((IDataReader) reader));
          keyValuePairList.Add(keyValuePair);
        }
        return keyValuePairList;
      }

      internal int BindValue(SqlDataReader reader) => this.Value.GetInt32((IDataReader) reader);
    }

    private class SqmData
    {
      public TestPlanningDatabase.SqlColumn CloneOperationsCount = new TestPlanningDatabase.SqlColumn(nameof (CloneOperationsCount));
      public TestPlanningDatabase.SqlColumn CloneOperationsAcrossProjectsCount = new TestPlanningDatabase.SqlColumn(nameof (CloneOperationsAcrossProjectsCount));
      public TestPlanningDatabase.SqlColumn ClonePlanOperationsCount = new TestPlanningDatabase.SqlColumn(nameof (ClonePlanOperationsCount));
      public TestPlanningDatabase.SqlColumn StaticTestSuitesCount = new TestPlanningDatabase.SqlColumn(nameof (StaticTestSuitesCount));
      public TestPlanningDatabase.SqlColumn QueryBasedTestSuitesCount = new TestPlanningDatabase.SqlColumn(nameof (QueryBasedTestSuitesCount));
      public TestPlanningDatabase.SqlColumn RequirementBasedTestSuitesCount = new TestPlanningDatabase.SqlColumn(nameof (RequirementBasedTestSuitesCount));
      public TestPlanningDatabase.SqlColumn TotalProjectsCount = new TestPlanningDatabase.SqlColumn(nameof (TotalProjectsCount));
      public TestPlanningDatabase.SqlColumn ResolutionStatesCustomizedProjectsCount = new TestPlanningDatabase.SqlColumn(nameof (ResolutionStatesCustomizedProjectsCount));
      public TestPlanningDatabase.SqlColumn FailureTypeCustomizedProjectsCount = new TestPlanningDatabase.SqlColumn(nameof (FailureTypeCustomizedProjectsCount));
      public TestPlanningDatabase.SqlColumn PlansCreatedFromMtm = new TestPlanningDatabase.SqlColumn(nameof (PlansCreatedFromMtm));
      public TestPlanningDatabase.SqlColumn PlansCreatedFromWeb = new TestPlanningDatabase.SqlColumn(nameof (PlansCreatedFromWeb));
      public TestPlanningDatabase.SqlColumn SuitesCreatedFromMtm = new TestPlanningDatabase.SqlColumn(nameof (SuitesCreatedFromMtm));
      public TestPlanningDatabase.SqlColumn SuitesCreatedFromWeb = new TestPlanningDatabase.SqlColumn(nameof (SuitesCreatedFromWeb));
      public TestPlanningDatabase.SqlColumn TestCasesAddedFromMtm = new TestPlanningDatabase.SqlColumn(nameof (TestCasesAddedFromMtm));
      public TestPlanningDatabase.SqlColumn TestCasesAddedFromWeb = new TestPlanningDatabase.SqlColumn(nameof (TestCasesAddedFromWeb));
      public TestPlanningDatabase.SqlColumn ManualTestRunsCreatedFromMtmCount = new TestPlanningDatabase.SqlColumn(nameof (ManualTestRunsCreatedFromMtmCount));
      public TestPlanningDatabase.SqlColumn ManualTestRunsCreatedFromWebCount = new TestPlanningDatabase.SqlColumn(nameof (ManualTestRunsCreatedFromWebCount));
      public TestPlanningDatabase.SqlColumn MTRRunsInitiatedFromWebCount = new TestPlanningDatabase.SqlColumn(nameof (MTRRunsInitiatedFromWebCount));
      public TestPlanningDatabase.SqlColumn XTSessionsCount = new TestPlanningDatabase.SqlColumn(nameof (XTSessionsCount));
      public TestPlanningDatabase.SqlColumn FBSessionsCount = new TestPlanningDatabase.SqlColumn(nameof (FBSessionsCount));
      public TestPlanningDatabase.SqlColumn UsersCreatingRunsFromWebCount = new TestPlanningDatabase.SqlColumn(nameof (UsersCreatingRunsFromWebCount));
      public TestPlanningDatabase.SqlColumn UsersCreatingRunsFromMtmCount = new TestPlanningDatabase.SqlColumn(nameof (UsersCreatingRunsFromMtmCount));
      public TestPlanningDatabase.SqlColumn UsersCreatingPlansFromWebCount = new TestPlanningDatabase.SqlColumn(nameof (UsersCreatingPlansFromWebCount));
      public TestPlanningDatabase.SqlColumn UsersCreatingPlansFromMtmCount = new TestPlanningDatabase.SqlColumn(nameof (UsersCreatingPlansFromMtmCount));
      public TestPlanningDatabase.SqlColumn UsersUsingXTCount = new TestPlanningDatabase.SqlColumn(nameof (UsersUsingXTCount));
      public TestPlanningDatabase.SqlColumn TotalDTARunsQueued = new TestPlanningDatabase.SqlColumn(nameof (TotalDTARunsQueued));
    }

    protected class SqlColumn
    {
      public string Key;
      public SqlColumnBinder SqlKey;

      public SqlColumn(string inputKey)
      {
        this.Key = inputKey;
        this.SqlKey = new SqlColumnBinder(this.Key);
      }
    }

    private class QueryTestRunLabSQMDataColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestEnvironmentId = new SqlColumnBinder(nameof (TestEnvironmentId));
      private SqlColumnBinder TestcaseCount = new SqlColumnBinder(nameof (TestcaseCount));
      private SqlColumnBinder TotalTimeSpent = new SqlColumnBinder(nameof (TotalTimeSpent));

      internal TestRunLabSQMData bind(SqlDataReader reader) => new TestRunLabSQMData()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestEnvironmentId = new Guid(this.TestEnvironmentId.GetString((IDataReader) reader, false)),
        TestcaseCount = this.TestcaseCount.GetInt32((IDataReader) reader),
        TimeSpent = this.TotalTimeSpent.GetInt32((IDataReader) reader)
      };
    }

    private class GetOldestDynamicSuiteColumns
    {
      internal SqlColumnBinder MaxAgeSeconds = new SqlColumnBinder(nameof (MaxAgeSeconds));
    }

    internal class FetchTestSuitesTesterColumns
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder Tester = new SqlColumnBinder(nameof (Tester));

      internal void bind(SqlDataReader reader, out int suiteId, out Guid tester)
      {
        suiteId = this.SuiteId.GetInt32((IDataReader) reader, 0);
        tester = this.Tester.GetGuid((IDataReader) reader, true);
      }
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

    internal class FetchTestSuitesTestCountColumns
    {
      public SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      public SqlColumnBinder TestCaseCount = new SqlColumnBinder(nameof (TestCaseCount));
    }

    protected class FetchTestSuitesColumns
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder ParentSuiteId = new SqlColumnBinder(nameof (ParentSuiteId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder SuiteType = new SqlColumnBinder(nameof (SuiteType));
      private SqlColumnBinder RequirementId = new SqlColumnBinder(nameof (RequirementId));
      private SqlColumnBinder Query = new SqlColumnBinder(nameof (Query));
      private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
      private SqlColumnBinder InheritConfigs = new SqlColumnBinder(nameof (InheritConfigs));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastPopulated = new SqlColumnBinder(nameof (LastPopulated));
      private SqlColumnBinder LastError = new SqlColumnBinder(nameof (LastError));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder LastSynced = new SqlColumnBinder(nameof (LastSynced));

      internal virtual ServerTestSuite bind(SqlDataReader reader, out int dataspaceId)
      {
        ServerTestSuite serverTestSuite = new ServerTestSuite();
        serverTestSuite.Id = this.SuiteId.GetInt32((IDataReader) reader);
        serverTestSuite.PlanId = this.PlanId.GetInt32((IDataReader) reader);
        serverTestSuite.ParentId = this.ParentSuiteId.GetInt32((IDataReader) reader);
        serverTestSuite.Title = this.Title.GetString((IDataReader) reader, false);
        serverTestSuite.Description = this.Description.GetString((IDataReader) reader, false);
        serverTestSuite.QueryString = this.Query.GetString((IDataReader) reader, true);
        serverTestSuite.ConvertedQueryString = this.Query.GetString((IDataReader) reader, true);
        serverTestSuite.Status = this.Status.GetString((IDataReader) reader, false);
        serverTestSuite.InheritDefaultConfigurations = this.InheritConfigs.GetBoolean((IDataReader) reader);
        serverTestSuite.Revision = this.Revision.GetInt32((IDataReader) reader);
        serverTestSuite.LastPopulated = this.LastPopulated.GetDateTime((IDataReader) reader);
        serverTestSuite.LastError = this.LastError.GetString((IDataReader) reader, true);
        serverTestSuite.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        serverTestSuite.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        serverTestSuite.RequirementId = this.RequirementId.GetInt32((IDataReader) reader);
        serverTestSuite.SuiteType = this.SuiteType.GetByte((IDataReader) reader);
        serverTestSuite.LastSynced = this.LastSynced.ColumnExists((IDataReader) reader) ? this.LastSynced.GetDateTime((IDataReader) reader) : DateTime.MinValue;
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return serverTestSuite;
      }
    }

    protected class TestSuiteEntryColumns
    {
      private SqlColumnBinder ParentSuiteId = new SqlColumnBinder(nameof (ParentSuiteId));
      private SqlColumnBinder ChildId = new SqlColumnBinder(nameof (ChildId));
      private SqlColumnBinder EntryType = new SqlColumnBinder(nameof (EntryType));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));

      internal TestSuiteEntry bind(
        SqlDataReader reader,
        TestSuiteEntry lastEntry,
        List<TestPointAssignment> points)
      {
        int int32_1 = this.ParentSuiteId.GetInt32((IDataReader) reader);
        int int32_2 = this.ChildId.GetInt32((IDataReader) reader);
        byte num = this.EntryType.GetByte((IDataReader) reader);
        int int32_3 = this.ConfigurationId.GetInt32((IDataReader) reader, 0);
        Guid guid = this.AssignedTo.GetGuid((IDataReader) reader, true);
        string str = this.ConfigurationName.GetString((IDataReader) reader, true);
        if (lastEntry != null)
        {
          if (lastEntry.ParentSuiteId == int32_1 && lastEntry.EntryId == int32_2 && (int) lastEntry.EntryType == (int) num && int32_3 != 0)
          {
            points.Add(new TestPointAssignment()
            {
              TestCaseId = int32_2,
              ConfigurationId = int32_3,
              ConfigurationName = str,
              AssignedTo = guid
            });
            return lastEntry;
          }
          if (points.Count > 0)
          {
            lastEntry.PointAssignments = points.ToArray();
            points.Clear();
          }
        }
        TestSuiteEntry testSuiteEntry = new TestSuiteEntry();
        testSuiteEntry.ParentSuiteId = int32_1;
        testSuiteEntry.EntryId = int32_2;
        testSuiteEntry.EntryType = num;
        if (int32_3 <= 0)
          return testSuiteEntry;
        points.Add(new TestPointAssignment()
        {
          TestCaseId = int32_2,
          ConfigurationId = int32_3,
          ConfigurationName = str,
          AssignedTo = guid
        });
        return testSuiteEntry;
      }
    }

    protected class ConfigurationIdAndNameColumns
    {
      internal SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      internal SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
    }

    protected class TestSuiteConfigurationColumns
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));

      internal void bind(
        SqlDataReader reader,
        out int suiteId,
        out int configurationId,
        out string configurationName)
      {
        suiteId = this.SuiteId.GetInt32((IDataReader) reader);
        configurationId = this.ConfigurationId.GetInt32((IDataReader) reader);
        configurationName = this.ConfigurationName.GetString((IDataReader) reader, true);
      }
    }

    private class SuiteIdAndTypeColumns
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder SuiteType = new SqlColumnBinder(nameof (SuiteType));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder RequirementId = new SqlColumnBinder(nameof (RequirementId));

      internal SuiteIdAndType bind(SqlDataReader reader) => new SuiteIdAndType()
      {
        SuiteId = this.SuiteId.GetInt32((IDataReader) reader),
        SuiteType = this.SuiteType.GetByte((IDataReader) reader),
        PlanId = this.PlanId.GetInt32((IDataReader) reader),
        Revision = this.Revision.GetInt32((IDataReader) reader),
        RequirementId = this.RequirementId.GetInt32((IDataReader) reader)
      };
    }

    private class FetchTestSuitesColumnsForWitCreation
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder SuiteType = new SqlColumnBinder(nameof (SuiteType));
      private SqlColumnBinder Query = new SqlColumnBinder(nameof (Query));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));

      internal ServerTestSuite bind(SqlDataReader reader) => new ServerTestSuite()
      {
        Id = this.SuiteId.GetInt32((IDataReader) reader),
        Title = this.Title.GetString((IDataReader) reader, false),
        Description = this.Description.GetString((IDataReader) reader, false),
        QueryString = this.Query.GetString((IDataReader) reader, true),
        State = this.State.GetByte((IDataReader) reader),
        SuiteType = this.SuiteType.GetByte((IDataReader) reader),
        LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false),
        PlanId = this.PlanId.GetInt32((IDataReader) reader)
      };
    }
  }
}
