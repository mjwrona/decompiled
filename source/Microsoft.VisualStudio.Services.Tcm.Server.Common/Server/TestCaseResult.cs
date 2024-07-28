// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server.Results;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  [SoapInclude(typeof (TestResult))]
  [CLSCompliant(false)]
  public class TestCaseResult : TestResult, IAreaUriProperty
  {
    protected byte m_state;
    protected int m_resolutionStateId;
    protected string m_computerName;
    protected byte m_priority = byte.MaxValue;
    protected string m_testCaseTitle;
    protected string m_testCaseArea;
    protected string m_testCaseAreaUri;
    protected int m_testCaseRevision;
    protected int m_afnStripId;
    protected int m_resetCount;
    protected int m_testCaseId;
    protected int m_configurationId;
    protected int m_testPointId = int.MinValue;
    protected byte m_failureType;
    protected string m_automatedTestName;
    protected string m_automatedTestStorage;
    protected string m_automatedTestType;
    protected string m_automatedTestTypeId;
    protected string m_automatedTestId;
    protected int m_revision;
    protected int m_testSuiteId;
    protected int m_testPlanId;
    protected string m_testSuiteName;
    protected const string c_manualOrUnknownTest = "Manual or unknown";
    internal const byte UnspecifiedPriority = 255;

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TestCaseId
    {
      get => this.m_testCaseId;
      set => this.m_testCaseId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int ConfigurationId
    {
      get => this.m_configurationId;
      set => this.m_configurationId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ConfigurationName { get; set; }

    [XmlAttribute]
    [DefaultValue(-2147483648)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TestPointId
    {
      get => this.m_testPointId;
      set => this.m_testPointId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (TestResultState))]
    public byte State
    {
      get => this.m_state;
      set => this.m_state = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public byte FailureType
    {
      get => this.m_failureType;
      set => this.m_failureType = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int ResolutionStateId
    {
      get => this.m_resolutionStateId;
      set => this.m_resolutionStateId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ComputerName
    {
      get => this.m_computerName;
      set => this.m_computerName = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid Owner { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid RunBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string RunByName { get; set; }

    [XmlAttribute]
    [DefaultValue(255)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public byte Priority
    {
      get => this.m_priority;
      set => this.m_priority = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string TestCaseTitle
    {
      get => this.m_testCaseTitle;
      set => this.m_testCaseTitle = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(SqlFieldName = "TestCaseAreaPath")]
    public string TestCaseArea
    {
      get => this.m_testCaseArea;
      set => this.m_testCaseArea = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BuildType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string LayoutUid { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int Attempt { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public bool IsSystemIssue { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ExceptionType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Locale { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BucketUid { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BucketingSystem { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int ExecutionNumber { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TopologyId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string TestCaseAreaUri
    {
      get => this.m_testCaseAreaUri;
      set => this.m_testCaseAreaUri = value;
    }

    [XmlIgnore]
    public string AreaUri
    {
      get => this.TestCaseAreaUri;
      set => this.TestCaseAreaUri = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TestCaseRevision
    {
      get => this.m_testCaseRevision;
      set => this.m_testCaseRevision = value;
    }

    [ClientProperty(ClientVisibility.Private)]
    public int AfnStripId
    {
      get => this.m_afnStripId;
      set => this.m_afnStripId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int ResetCount
    {
      get => this.m_resetCount;
      set => this.m_resetCount = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AutomatedTestName
    {
      get => this.m_automatedTestName;
      set => this.m_automatedTestName = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AutomatedTestStorage
    {
      get => this.m_automatedTestStorage;
      set => this.m_automatedTestStorage = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AutomatedTestType
    {
      get => this.m_automatedTestType;
      set => this.m_automatedTestType = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AutomatedTestTypeId
    {
      get => this.m_automatedTestTypeId;
      set => this.m_automatedTestTypeId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AutomatedTestId
    {
      get => this.m_automatedTestId;
      set => this.m_automatedTestId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [QueryMapping]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BuildNumber { get; set; }

    [XmlIgnore]
    [QueryMapping(SqlFieldName = "DataspaceId")]
    internal string TeamProject => (string) null;

    [XmlIgnore]
    [DefaultValue(0)]
    internal int DataRowCount { get; set; }

    [XmlIgnore]
    [QueryMapping]
    public int TestPlanId
    {
      get => this.m_testPlanId;
      set => this.m_testPlanId = value;
    }

    [XmlIgnore]
    public int TestSuiteId
    {
      get => this.m_testSuiteId;
      set => this.m_testSuiteId = value;
    }

    [XmlIgnore]
    public string SuiteName
    {
      get => this.m_testSuiteName;
      set => this.m_testSuiteName = value;
    }

    [XmlIgnore]
    public int SequenceId { get; set; }

    public int TestCaseReferenceId { get; set; }

    public TestExtensionField StackTrace { get; set; }

    public List<TestExtensionField> CustomFields { get; set; }

    public BuildConfiguration BuildReference { get; set; }

    public ReleaseReference ReleaseReference { get; set; }

    public FailingSince FailingSince { get; set; }

    public bool IsRerun { get; set; }

    public int SubResultCount { get; set; }

    public ResultGroupType ResultGroupType { get; set; }

    public byte TestPhase { get; set; }

    public List<TestResultDimension> Dimensions { get; set; }

    public List<Link<ResultLinkType>> Links { get; set; }

    public string TestRunTitle { get; set; }

    public int AreaId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestCaseResult ({0}, {1}) State={2}", (object) this.TestRunId, (object) this.TestResultId, (object) this.State);

    private static List<TestCaseResult> ProcessQueryResults(
      TestManagementRequestContext context,
      List<TestCaseResultIdentifier> ids,
      out List<TestCaseResultIdentifier> excessIds,
      int pageSize,
      Guid projectId,
      bool testSessionProperties = false)
    {
      return TestCaseResult.ProcessQueryResults(context, ids, out excessIds, pageSize, projectId, false, out List<TestActionResult> _, out List<TestResultParameter> _, out List<TestResultAttachment> _, testSessionProperties);
    }

    private static List<TestCaseResult> ProcessQueryResults(
      TestManagementRequestContext context,
      List<TestCaseResultIdentifier> ids,
      out List<TestCaseResultIdentifier> excessIds,
      int pageSize,
      Guid projectId,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments,
      bool testSessionProperties = false)
    {
      context.SecurityManager.FilterViewWorkItemOptional<TestCaseResultIdentifier>(context, (IList<TestCaseResultIdentifier>) ids);
      int count = ids.Count;
      int num = count < pageSize ? count : pageSize;
      context.TraceVerbose("BusinessLayer", "TestCaseResult.ProcessQueryResults - collection size = {0}, fetch size = {1}", (object) count, (object) num);
      List<TestCaseResultIdentifier> range = ids.GetRange(0, num);
      (bool IsCrossRunsQuery, int TestRunId) runsQueryDetails = TestCaseResult.GetCrossRunsQueryDetails(range);
      if (pageSize == 0)
      {
        excessIds = ids;
        actionResults = (List<TestActionResult>) null;
        parameters = (List<TestResultParameter>) null;
        attachments = (List<TestResultAttachment>) null;
        return new List<TestCaseResult>();
      }
      excessIds = num >= count ? new List<TestCaseResultIdentifier>() : ids.GetRange(num, count - num);
      List<TestCaseResult> results = new List<TestCaseResult>();
      if (testSessionProperties)
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          results = managementDatabase.FetchTestResultsByRunMRX(projectId, runsQueryDetails.TestRunId, range.Select<TestCaseResultIdentifier, int>((Func<TestCaseResultIdentifier, int>) (id => id.TestResultId)).ToList<int>(), includeActionResults, out actionResults, out parameters, out attachments);
      }
      else if (runsQueryDetails.IsCrossRunsQuery || !context.IsTcmService)
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          results = managementDatabase.FetchTestResults(range.Select<TestCaseResultIdentifier, TestCaseResultIdAndRev>((Func<TestCaseResultIdentifier, TestCaseResultIdAndRev>) (id => new TestCaseResultIdAndRev(id, 0))).ToArray<TestCaseResultIdAndRev>(), (List<TestCaseResultIdentifier>) null, projectId, includeActionResults, out actionResults, out parameters, out attachments);
      }
      else
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          results = managementDatabase.FetchTestResultsByRun(projectId, runsQueryDetails.TestRunId, range.Select<TestCaseResultIdentifier, int>((Func<TestCaseResultIdentifier, int>) (id => id.TestResultId)).ToList<int>(), includeActionResults, out actionResults, out parameters, out attachments);
      }
      if (TestResultExtensionLogstore.IsExtensionInLogstoreFFEnabled(context))
        TestResultExtensionLogstore.FetchMutlipleResultExFromLogstore(context, ref results, projectId);
      TestCaseResult.UpdateAreaPathForResults(context, results);
      TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
      return TestCaseResult.ResolveUserNames(context, results);
    }

    private static (bool IsCrossRunsQuery, int TestRunId) GetCrossRunsQueryDetails(
      List<TestCaseResultIdentifier> ids)
    {
      int num = 0;
      for (int index = 0; index < ids.Count; ++index)
      {
        if (num == 0)
          num = ids[index].TestRunId;
        else if (num != ids[index].TestRunId)
          return (true, 0);
      }
      return (false, num);
    }

    private static List<TestCaseResult> ProcessQueryResultsWithSuiteDetails(
      TestManagementRequestContext context,
      List<TestCaseResultIdentifier> ids,
      int pageSize,
      Guid projectId)
    {
      context.SecurityManager.FilterViewWorkItemOptional<TestCaseResultIdentifier>(context, (IList<TestCaseResultIdentifier>) ids);
      int count = ids.Count;
      int length = count < pageSize ? count : pageSize;
      context.TraceVerbose("BusinessLayer", "TestCaseResult.ProcessQueryResultsWithSuiteDetails - collection size = {0}, fetch size = {1}", (object) count, (object) length);
      TestCaseResultIdAndRev[] resultIds = new TestCaseResultIdAndRev[length];
      for (int index = 0; index < length; ++index)
        resultIds[index] = new TestCaseResultIdAndRev(ids[index], 0);
      if (pageSize == 0)
        return new List<TestCaseResult>();
      List<TestCaseResult> results = new List<TestCaseResult>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        results = managementDatabase.FetchTestResultsWithSuiteDetails(resultIds, projectId);
      return TestCaseResult.ResolveUserNames(context, results);
    }

    internal static List<TestCaseResult> Fetch(
      TestManagementRequestContext context,
      TestCaseResultIdAndRev[] idsToFetch,
      string projectName,
      bool includeActionResults,
      List<TestCaseResultIdentifier> deletedIds,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      ArgumentUtility.CheckForNull<TestCaseResultIdAndRev[]>(idsToFetch, nameof (idsToFetch), "Test Results");
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseResult.Fetch"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        {
          actionResults = (List<TestActionResult>) null;
          parameters = (List<TestResultParameter>) null;
          attachments = (List<TestResultAttachment>) null;
          return new List<TestCaseResult>();
        }
        context.TestManagementHost.Replicator.UpdateCss(context);
        List<TestCaseResult> results1;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          results1 = managementDatabase.FetchTestResults(idsToFetch, deletedIds, projectFromName.GuidId, includeActionResults, out actionResults, out parameters, out attachments);
        TestCaseResult.UpdateAreaPathForResults(context, results1);
        TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
        List<TestCaseResult> results2 = new List<TestCaseResult>();
        List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
        foreach (TestCaseResult testCaseResult in results1)
        {
          if (testCaseResult.TestCaseId > 0)
            testCaseResultList.Add(testCaseResult);
          else
            results2.Add(testCaseResult);
          testCaseResult.Comment = string.IsNullOrEmpty(testCaseResult.Comment) ? string.Empty : testCaseResult.Comment;
        }
        context.SecurityManager.FilterViewWorkItemOptional<TestCaseResult>(context, (IList<TestCaseResult>) testCaseResultList);
        results2.AddRange((IEnumerable<TestCaseResult>) testCaseResultList);
        return TestCaseResult.ResolveUserNames(context, results2);
      }
    }

    internal static TestCaseResult FetchSingularly(
      TestManagementRequestContext context,
      TestCaseResultIdentifier id,
      string projectName,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseResult.FetchSingularly"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        {
          actionResults = (List<TestActionResult>) null;
          parameters = (List<TestResultParameter>) null;
          attachments = (List<TestResultAttachment>) null;
          return new TestCaseResult();
        }
        context.TestManagementHost.Replicator.UpdateCss(context);
        TestCaseResult result = new TestCaseResult();
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.UseIterationIdInLogStoreAttachmentMapper"))
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            result = managementDatabase.FetchTestResultV2(id, projectFromName.GuidId, includeActionResults, out actionResults, out parameters, out attachments);
        }
        else
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            result = managementDatabase.FetchTestResult(id, projectFromName.GuidId, includeActionResults, out actionResults, out parameters, out attachments);
        }
        if (TestResultExtensionLogstore.ShouldTestExtensionBeStoredInLogstore(context, id.TestRunId, id.TestResultId, projectFromName.GuidId))
          TestResultExtensionLogstore.FetchExtJsonFromLogstoreAndParse(context, ref result, id.TestRunId, projectName);
        List<TestCaseResult> testCaseResultList = new List<TestCaseResult>()
        {
          result
        };
        TestCaseResult.UpdateAreaPathForResults(context, testCaseResultList);
        TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
        if (result.TestCaseId > 0)
          context.SecurityManager.FilterViewWorkItemOptional<TestCaseResult>(context, (IList<TestCaseResult>) testCaseResultList);
        return TestCaseResult.ResolveUserNames(context, testCaseResultList).FirstOrDefault<TestCaseResult>();
      }
    }

    internal static void UpdateConfigurgationNameForResults(
      TestManagementRequestContext context,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String) || results == null)
        return;
      HashSet<int> configIds = new HashSet<int>();
      results.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result =>
      {
        int result1;
        if (result == null || result.Configuration == null || !int.TryParse(result.Configuration.Id, out result1) || result1 <= 0)
          return;
        configIds.Add(result1);
      }));
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        List<Tuple<TestConfiguration, string>> source = managementDatabase.QueryTestConfigurationById(configIds.ToList<int>(), projectFromName.GuidId, false);
        if (!source.Any<Tuple<TestConfiguration, string>>())
          return;
        Dictionary<int, string> resultConfiurationMap = new Dictionary<int, string>();
        foreach (Tuple<TestConfiguration, string> tuple in source)
          resultConfiurationMap.Add(tuple.Item1.Id, tuple.Item1.Name);
        results.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result =>
        {
          int result2;
          if (result == null || result.Configuration == null || !int.TryParse(result.Configuration.Id, out result2) || result2 <= 0)
            return;
          result.Configuration.Name = resultConfiurationMap.GetValueOrDefault<int, string>(result2, (string) null);
        }));
      }
    }

    internal static List<TestCaseResult> QueryByRun(
      TestManagementRequestContext context,
      int testRunId,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments,
      int offset = 0,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomeList = null,
      bool newTestsOnly = false,
      bool testSessionProperties = false)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
      {
        excessIds = new List<TestCaseResultIdentifier>();
        actionResults = (List<TestActionResult>) null;
        parameters = (List<TestResultParameter>) null;
        attachments = (List<TestResultAttachment>) null;
        return new List<TestCaseResult>();
      }
      TestRun testRun = (TestRun) null;
      if (context.IsFeatureEnabled("TestManagement.Server.AvoidUpdateCSSInGetResultsForAutomated"))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testRun = managementDatabase.GetTestRunBasic(projectFromName.GuidId, testRunId);
      }
      if (testRun == null || testRun.TestPlanId > 0)
        context.TestManagementHost.Replicator.UpdateCss(context);
      List<TestCaseResultIdentifier> source = new List<TestCaseResultIdentifier>();
      if (!newTestsOnly)
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          source = managementDatabase.QueryTestResults(testRunId, Guid.Empty, (byte) 0, outcomeList != null ? outcomeList.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, byte>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, byte>) (t => (byte) t)).ToList<byte>() : (List<byte>) null, 0, projectFromName.GuidId, context.IsTcmService);
      }
      else
      {
        TestExtensionFieldDetails extensionFieldDetails = context.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(context, projectFromName.GuidId, scopeFilter: CustomTestFieldScope.TestResult).Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "NewTestEntry", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          source = managementDatabase.QueryNewTestResults(testRunId, Guid.Empty, (byte) 0, outcomeList != null ? outcomeList.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, byte>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, byte>) (t => (byte) t)).ToList<byte>() : (List<byte>) null, 0, projectFromName.GuidId, extensionFieldDetails.Id);
      }
      if (source != null && source.Count > 0)
        source.Sort((Comparison<TestCaseResultIdentifier>) ((id1, id2) => id1.TestResultId.CompareTo(id2.TestResultId)));
      List<TestCaseResultIdentifier> list = source.Skip<TestCaseResultIdentifier>(offset).ToList<TestCaseResultIdentifier>();
      return TestCaseResult.ProcessQueryResults(context, list, out excessIds, pageSize, projectFromName.GuidId, includeActionResults, out actionResults, out parameters, out attachments, testSessionProperties);
    }

    internal static List<TestCaseResultIdentifier> GetTestCaseResultIdsInTestRun(
      TestManagementRequestContext context,
      int testRunId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestCaseResultIdentifier>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryTestResults(testRunId, Guid.Empty, (byte) 0, (List<byte>) null, 0, projectFromName.GuidId, context.IsTcmService);
    }

    internal static List<TestCaseResult> QueryByPoint(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      int pointId)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestCaseResult>();
      context.TestManagementHost.Replicator.UpdateCss(context);
      bool isTcmService = context.IsTcmService;
      List<TestCaseResult> testCaseResultList;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testCaseResultList = managementDatabase.QueryTestResultsByPoint(projectFromName.GuidId, planId, pointId, isTcmService);
      TestCaseResult.UpdateAreaPathForResults(context, testCaseResultList);
      context.SecurityManager.FilterViewWorkItemOptional<TestCaseResult>(context, (IList<TestCaseResult>) testCaseResultList);
      return testCaseResultList;
    }

    internal static List<TestCaseResult> QueryByRunAndState(
      TestManagementRequestContext context,
      int testRunId,
      byte state,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
      {
        excessIds = new List<TestCaseResultIdentifier>();
        return new List<TestCaseResult>();
      }
      context.TestManagementHost.Replicator.UpdateCss(context);
      List<TestCaseResultIdentifier> ids = new List<TestCaseResultIdentifier>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        ids = managementDatabase.QueryTestResults(testRunId, Guid.Empty, state, (List<byte>) null, 0, projectFromName.GuidId, context.IsTcmService);
      return TestCaseResult.ProcessQueryResults(context, ids, out excessIds, pageSize, projectFromName.GuidId);
    }

    internal static List<TestCaseResult> QueryByRunAndOutcome(
      TestManagementRequestContext context,
      int testRunId,
      byte outcome,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      return TestCaseResult.QueryByRunAndOutcomeInternal(context, testRunId, outcome, pageSize, true, out excessIds, projectName, out List<TestCaseResultIdentifier> _);
    }

    internal static List<TestCaseResult> QueryByRunAndOutcomeInternal(
      TestManagementRequestContext context,
      int testRunId,
      byte outcome,
      int pageSize,
      bool processResults,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName,
      out List<TestCaseResultIdentifier> tcIdentifiers)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      tcIdentifiers = new List<TestCaseResultIdentifier>();
      excessIds = new List<TestCaseResultIdentifier>();
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestCaseResult>();
      context.TestManagementHost.Replicator.UpdateCss(context);
      if (!processResults)
        return new List<TestCaseResult>();
      List<TestCaseResultIdentifier> resultIdentifierList1 = new List<TestCaseResultIdentifier>();
      using (TestManagementDatabase managementDatabase1 = TestManagementDatabase.Create(context))
      {
        ref List<TestCaseResultIdentifier> local = ref tcIdentifiers;
        TestManagementDatabase managementDatabase2 = managementDatabase1;
        int testRunId1 = testRunId;
        Guid empty = Guid.Empty;
        List<byte> outcomes = new List<byte>();
        outcomes.Add(outcome);
        Guid guidId = projectFromName.GuidId;
        int num = context.IsTcmService ? 1 : 0;
        List<TestCaseResultIdentifier> resultIdentifierList2 = managementDatabase2.QueryTestResults(testRunId1, empty, (byte) 0, outcomes, 0, guidId, num != 0);
        local = resultIdentifierList2;
      }
      return TestCaseResult.ProcessQueryResults(context, tcIdentifiers, out excessIds, pageSize, projectFromName.GuidId);
    }

    internal static List<TestCaseResult> QueryByRunAndOwner(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
      {
        excessIds = new List<TestCaseResultIdentifier>();
        return new List<TestCaseResult>();
      }
      context.TestManagementHost.Replicator.UpdateCss(context);
      List<TestCaseResultIdentifier> ids = new List<TestCaseResultIdentifier>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        ids = managementDatabase.QueryTestResults(testRunId, owner, (byte) 0, (List<byte>) null, 0, projectFromName.GuidId, context.IsTcmService);
      return TestCaseResult.ProcessQueryResults(context, ids, out excessIds, pageSize, projectFromName.GuidId);
    }

    internal static void Create(
      TestManagementRequestContext context,
      TestCaseResult[] results,
      bool populateDataRowCount,
      string projectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseResult.Create"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        string empty = string.Empty;
        CustomerIntelligenceData cid = new CustomerIntelligenceData();
        context.WorkItemFieldDataHelper.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired(context, projectFromName, results, populateDataRowCount);
        ((IEnumerable<TestCaseResult>) results).ForEach<TestCaseResult>((Action<TestCaseResult>) (result => TestCaseResult.PopulateResultStartAndCompleteDateIfRequired(result)));
        Guid teamFoundationId = context.UserTeamFoundationId;
        bool isTcmService = context.IsTcmService;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        {
          try
          {
            foreach (IGrouping<int, TestCaseResult> results1 in ((IEnumerable<TestCaseResult>) results).GroupBy<TestCaseResult, int>((Func<TestCaseResult, int>) (r => r.TestRunId)))
              managementDatabase.CreateTestResults(projectFromName.GuidId, results1.Key, (IEnumerable<TestCaseResult>) results1, teamFoundationId, true, isTcmService);
          }
          catch (TestManagementValidationException ex)
          {
            string argumentName = ex.GetArgumentName();
            if (string.IsNullOrEmpty(argumentName))
              throw new TestManagementValidationException(ServerResources.CreateTestResults_InvalidData);
            throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CreateTestResults_InvalidData_WithArgument, (object) argumentName));
          }
        }
        if (results == null)
          return;
        string str = results[0].AutomatedTestType != null ? results[0].AutomatedTestType : "Manual or unknown";
        cid.Add("TestRunId", results[0].TestRunId.ToString());
        cid.Add("TestType", str);
        cid.Add("TestResultCount", results.Length.ToString());
        TelemetryLogger.Instance.PublishData(context.RequestContext, "TestResultType", cid);
      }
    }

    internal static Dictionary<int, int[]> Create2(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      TestCaseResult[] results,
      bool populateDataRowCount,
      bool populateTestCaseProperties,
      bool shouldPublishOnlyFailedResults = false,
      bool testSessionProperties = false)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (populateTestCaseProperties)
        context.WorkItemFieldDataHelper.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired(context, projectFromName, results, populateDataRowCount);
      ((IEnumerable<TestCaseResult>) results).ForEach<TestCaseResult>((Action<TestCaseResult>) (result => TestCaseResult.PopulateResultStartAndCompleteDateIfRequired(result)));
      bool skipPassedTestsDetails = context.IsFeatureEnabled("TestManagement.Server.SkipPassedTestsDetails") | shouldPublishOnlyFailedResults;
      int newTestCaseRefRows = -1;
      int[] resultIds;
      TestCaseResult.CreateResults2(context, testRunId, results, projectFromName, skipPassedTestsDetails, true, out int[] _, out newTestCaseRefRows, out resultIds, testSessionProperties);
      TestCaseResult.FireTestCaseResultsCreatedNotification(context, (IList<TestCaseResult>) results, testRunId, projectName);
      Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>();
      dictionary.Add(testRunId, resultIds);
      if (newTestCaseRefRows < 0)
        return dictionary;
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("TestRunId", (double) testRunId);
      cid.Add("TestResultCount", (double) results.Length);
      cid.Add("NewTestCaseRefRows", (double) newTestCaseRefRows);
      TelemetryLogger.Instance.PublishData(context.RequestContext, "NewTestCaseReferences", cid);
      return dictionary;
    }

    internal static void CreateResults2(
      TestManagementRequestContext context,
      int testRunId,
      TestCaseResult[] results,
      GuidAndString projectId,
      bool skipPassedTestsDetails,
      bool updateRunSummary,
      out int[] testCaseRefIds,
      out int newTestCaseRefRows,
      out int[] resultIds,
      bool testSessionProperties = false)
    {
      Guid teamFoundationId = context.UserTeamFoundationId;
      List<int> newTestCaseRefIds;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testCaseRefIds = managementDatabase.CreateTestCaseReference(projectId.GuidId, (IEnumerable<TestCaseResult>) results, teamFoundationId, out newTestCaseRefRows, out newTestCaseRefIds);
      for (int index = 0; index < results.Length; ++index)
        results[index].TestCaseReferenceId = testCaseRefIds[index];
      if (context.IsFeatureEnabled("TestManagement.Server.EnableNewTestResultLogging"))
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectId.GuidId);
        try
        {
          TestResultsSettings testResultsSettings = context.RequestContext.GetService<ITeamFoundationTestManagementTestResultsSettingsService>().GetTestResultsSettings(context, projectFromGuid, TestResultsSettingsType.NewTestLogging);
          context.RequestContext.Trace(1015803, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "FF is set to true, validating project settings and newTestCaseRef count");
          if (testResultsSettings != null)
          {
            bool? logNewTests = (bool?) testResultsSettings.NewTestResultLoggingSettings?.LogNewTests;
            bool flag = true;
            if (logNewTests.GetValueOrDefault() == flag & logNewTests.HasValue && newTestCaseRefIds != null && newTestCaseRefIds.Count<int>() > 0)
            {
              context.RequestContext.Trace(1015803, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "Validated project settings, tagging results");
              TestExtensionFieldDetails newFieldDetails = context.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(context, projectId.GuidId, scopeFilter: CustomTestFieldScope.TestResult).Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "NewTestEntry", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
              if (newFieldDetails != null)
                TestCaseResult.getResultIdsMappedToRefs(results, newTestCaseRefIds, newFieldDetails);
              context.RequestContext.Trace(1015803, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "Tagged results with new refs");
              goto label_18;
            }
          }
          context.RequestContext.Trace(1015803, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "Project setting - {0}; newTestCaseRefId count - {1}", (object) testResultsSettings, (object) newTestCaseRefRows);
        }
        catch (Exception ex)
        {
          context.RequestContext.Trace(1015803, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "Failed to log new test results due to {0}", (object) ex);
        }
      }
label_18:
      resultIds = TestCaseResult.CreateInternal(context, projectId, testRunId, (IEnumerable<TestCaseResult>) results, skipPassedTestsDetails, updateRunSummary);
      if (!testSessionProperties)
        return;
      TestCaseResult.CreateInternalOneMRX(context, projectId.GuidId, testRunId, (IEnumerable<TestCaseResult>) results, resultIds);
    }

    internal static void CreateInternalOneMRX(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      int[] resultIds)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.CreateTestResultOneMRX(projectId, results, resultIds);
    }

    internal static int[] CreateInternal(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      bool skipPassedTestsDetails,
      bool updateRunSummary)
    {
      int[] numArray1 = (int[]) null;
      try
      {
        Guid teamFoundationId = context.UserTeamFoundationId;
        bool isTcmService = context.IsTcmService;
        if (skipPassedTestsDetails)
        {
          IEnumerable<TestCaseResult> results1 = results.Where<TestCaseResult>((Func<TestCaseResult, bool>) (r => r.Outcome != (byte) 2));
          int passedTestsCount = results.Count<TestCaseResult>((Func<TestCaseResult, bool>) (r => r.Outcome == (byte) 2));
          int[] numArray2 = (int[]) null;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            numArray2 = managementDatabase.CreateTestResultsExtension2(projectId.GuidId, testRunId, results1, passedTestsCount, teamFoundationId, isTcmService);
          numArray1 = new int[results.Count<TestCaseResult>()];
          int index1 = 0;
          int index2 = 0;
          foreach (TestResult result in results)
          {
            if (result.Outcome == (byte) 2)
            {
              numArray1[index2] = -1;
            }
            else
            {
              numArray1[index2] = numArray2[index1];
              ++index1;
            }
            ++index2;
          }
        }
        else
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            numArray1 = managementDatabase.CreateTestResultsExtension2(projectId.GuidId, testRunId, results, 0, teamFoundationId, isTcmService);
        }
      }
      catch (TestManagementValidationException ex)
      {
        string argumentName = ex.GetArgumentName();
        if (string.IsNullOrEmpty(argumentName))
          throw new TestManagementValidationException(ServerResources.CreateTestResults_InvalidData);
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CreateTestResults_InvalidData_WithArgument, (object) argumentName));
      }
      if (results != null && results.Any<TestCaseResult>())
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData();
        TestCaseResult testCaseResult = results.First<TestCaseResult>();
        string str = testCaseResult.AutomatedTestType != null ? testCaseResult.AutomatedTestType : "Manual or unknown";
        cid.Add("TestRunId", (double) testRunId);
        cid.Add("TestType", str);
        cid.Add("TestResultCount", results.Count<TestCaseResult>().ToString());
        TelemetryLogger.Instance.PublishData(context.RequestContext, "TestResultType", cid);
      }
      return numArray1;
    }

    private static void PopulateResultStartAndCompleteDateIfRequired(TestCaseResult result)
    {
      if (result == null || result.State != (byte) 5)
        return;
      bool flag1 = DateTime.Equals(result.DateStarted, new DateTime());
      bool flag2 = DateTime.Equals(result.DateCompleted, new DateTime());
      if (!flag1 & flag2)
        result.DateCompleted = result.DateStarted.AddTicks(result.Duration);
      else if (flag1 && !flag2)
        result.DateStarted = result.DateCompleted.AddTicks(-result.Duration);
      else if (flag1 & flag2)
      {
        result.DateStarted = DateTime.UtcNow;
        result.DateCompleted = result.DateStarted.AddTicks(result.Duration);
      }
      if (result.DateCompleted < result.DateStarted)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultCompletedDateGreaterThanStartDate));
    }

    private static void PopulateResultCommentAndErrorMessageIfRequired(
      TestManagementRequestContext context,
      TestCaseResult result,
      IList<TestExtensionFieldDetails> resultExFieldDetails)
    {
      List<TestExtensionField> testExtensionFieldList = new List<TestExtensionField>();
      if (!string.IsNullOrEmpty(result?.Comment))
      {
        List<TestExtensionField> customFields = result.CustomFields;
        if ((customFields != null ? customFields.FirstOrDefault<TestExtensionField>((Func<TestExtensionField, bool>) (r => string.Equals(r.Field.Name, "Comment", StringComparison.OrdinalIgnoreCase))) : (TestExtensionField) null) == null)
        {
          TestExtensionFieldDetails extensionFieldDetails = resultExFieldDetails.Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "Comment", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
          if (extensionFieldDetails != null)
          {
            int forResultComment = TestManagementServiceUtility.GetMaxLengthForResultComment(context.RequestContext);
            string str = result.Comment.Substring(0, result.Comment.Length > forResultComment ? forResultComment : result.Comment.Length);
            testExtensionFieldList.Add(new TestExtensionField()
            {
              Field = extensionFieldDetails,
              Value = (object) str
            });
          }
        }
      }
      if (!string.IsNullOrEmpty(result?.ErrorMessage))
      {
        List<TestExtensionField> customFields = result.CustomFields;
        if ((customFields != null ? customFields.FirstOrDefault<TestExtensionField>((Func<TestExtensionField, bool>) (r => string.Equals(r.Field.Name, "ErrorMessage", StringComparison.OrdinalIgnoreCase))) : (TestExtensionField) null) == null)
        {
          TestExtensionFieldDetails extensionFieldDetails = resultExFieldDetails.Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "ErrorMessage", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
          if (extensionFieldDetails != null)
            testExtensionFieldList.Add(new TestExtensionField()
            {
              Field = extensionFieldDetails,
              Value = (object) result.ErrorMessage
            });
        }
      }
      if (!testExtensionFieldList.Any<TestExtensionField>())
        return;
      if (result.CustomFields != null)
        result.CustomFields.AddRange((IEnumerable<TestExtensionField>) testExtensionFieldList);
      else
        result.CustomFields = testExtensionFieldList;
    }

    private static void DedupResultCustomFields(
      TestManagementRequestContext context,
      TestCaseResult result)
    {
      if (result == null || result.CustomFields == null || !result.CustomFields.Any<TestExtensionField>())
        return;
      int count1 = result.CustomFields.Count;
      IEnumerable<IGrouping<string, TestExtensionField>> source = result.CustomFields.GroupBy<TestExtensionField, string>((Func<TestExtensionField, string>) (x => x.Field.Name));
      result.CustomFields = source.Select<IGrouping<string, TestExtensionField>, TestExtensionField>((Func<IGrouping<string, TestExtensionField>, TestExtensionField>) (y => y.FirstOrDefault<TestExtensionField>())).ToList<TestExtensionField>();
      int count2 = result.CustomFields.Count;
      if (count1 == count2)
        return;
      string str = string.Join(",", source.Where<IGrouping<string, TestExtensionField>>((Func<IGrouping<string, TestExtensionField>, bool>) (x => x.Count<TestExtensionField>() > 1)).Select<IGrouping<string, TestExtensionField>, string>((Func<IGrouping<string, TestExtensionField>, string>) (y => y.Key)));
      context.TraceError("BusinessLayer", "Duplicate Custom Fields with following Name: {0}", (object) str);
    }

    private static void CheckActionResults(
      TestActionResult[] actionResults,
      int testRunId,
      int testResultId)
    {
      if (actionResults == null)
        return;
      foreach (TestActionResult actionResult in actionResults)
      {
        if (actionResult.TestRunId != testRunId || actionResult.TestResultId != testResultId)
          throw new InvalidResultUpdateRequestException(ServerResources.InvalidTestActionResultForUpdate);
      }
    }

    private static void CheckParameters(
      TestResultParameter[] parameters,
      int testRunId,
      int testResultId)
    {
      if (parameters == null)
        return;
      foreach (TestResultParameter parameter in parameters)
      {
        if (parameter.TestRunId != testRunId || parameter.TestResultId != testResultId)
          throw new InvalidResultUpdateRequestException(ServerResources.InvalidParameterForUpdate);
      }
    }

    private static void CheckAttachments(
      TestResultAttachment[] attachments,
      int testRunId,
      int testResultId)
    {
      if (attachments == null)
        return;
      foreach (TestResultAttachment attachment in attachments)
      {
        if (attachment.TestRunId != testRunId || attachment.TestResultId != testResultId)
          throw new InvalidResultUpdateRequestException(ServerResources.InvalidAttachmentForUpdate);
      }
    }

    private static void CheckAttachmentIdentities(
      TestResultAttachmentIdentity[] attachments,
      int testRunId,
      int testResultId)
    {
      if (attachments == null)
        return;
      foreach (TestResultAttachmentIdentity attachment in attachments)
      {
        if (attachment.TestRunId != testRunId || attachment.TestResultId != testResultId)
          throw new InvalidResultUpdateRequestException(ServerResources.InvalidAttachmentForUpdate);
      }
    }

    internal static TestCaseResult FindArtifact(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      ref TestRunArtifactInfo info)
    {
      info.UpdateRun(testRunId, context);
      if (info.HasViewRunPermission)
      {
        context.TestManagementHost.Replicator.UpdateCss(context);
        List<TestCaseResult> results = new List<TestCaseResult>();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          results = managementDatabase.FetchTestResults(new TestCaseResultIdAndRev[1]
          {
            new TestCaseResultIdAndRev(new TestCaseResultIdentifier(testRunId, testResultId), -1)
          }, (List<TestCaseResultIdentifier>) null, info.Project.GuidId);
        TestCaseResult.UpdateAreaPathForResults(context, results);
        if (results.Count == 1 && context.SecurityManager.CanViewTestResult(context, results[0].AreaUri))
        {
          TestCaseResult.ResolveUserNames(context, results);
          return results[0];
        }
      }
      return (TestCaseResult) null;
    }

    internal static ResultUpdateResponse[] Update(
      TestManagementRequestContext context,
      ResultUpdateRequest[] requests,
      string projectName,
      bool autoComputeTestRunState = true)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      requests = requests ?? Array.Empty<ResultUpdateRequest>();
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      IList<TestExtensionFieldDetails> resultExFieldDetails = context.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(context, projectFromName.GuidId, scopeFilter: CustomTestFieldScope.TestResult | CustomTestFieldScope.System);
      foreach (ResultUpdateRequest request in requests)
      {
        if (request.TestCaseResult != null && (request.TestRunId != request.TestCaseResult.TestRunId || request.TestResultId != request.TestCaseResult.TestResultId))
          throw new InvalidResultUpdateRequestException(ServerResources.InvalidTestCaseResultForUpdate);
        TestCaseResult.CheckActionResults(request.ActionResults, request.TestRunId, request.TestResultId);
        TestCaseResult.CheckActionResults(request.ActionResultDeletes, request.TestRunId, request.TestResultId);
        TestCaseResult.CheckParameters(request.Parameters, request.TestRunId, request.TestResultId);
        TestCaseResult.CheckParameters(request.ParameterDeletes, request.TestRunId, request.TestResultId);
        TestCaseResult.CheckAttachments(request.Attachments, request.TestRunId, request.TestResultId);
        TestCaseResult.CheckAttachmentIdentities(request.AttachmentDeletes, request.TestRunId, request.TestResultId);
        TestCaseResult.PopulateResultStartAndCompleteDateIfRequired(request.TestCaseResult);
        TestCaseResult.PopulateResultCommentAndErrorMessageIfRequired(context, request.TestCaseResult, resultExFieldDetails);
        if (!context.IsFeatureEnabled("TestManagement.Server.DisableResultDedupCustomField"))
          TestCaseResult.DedupResultCustomFields(context, request.TestCaseResult);
      }
      context.TraceVerbose("BusinessLayer", "TestCaseResult.Update - validation done");
      ResultUpdateResponse[] resultUpdateResponseArray = new ResultUpdateResponse[requests.Length];
      List<ResultUpdateResponse> resultUpdateResponseList = (List<ResultUpdateResponse>) null;
      Dictionary<TestCaseResultIdentifier, ResultUpdateResponse> dictionary1 = new Dictionary<TestCaseResultIdentifier, ResultUpdateResponse>();
      List<TestResultAttachment> attachments1 = new List<TestResultAttachment>();
      List<TestResultAttachmentIdentity> attachments2 = new List<TestResultAttachmentIdentity>();
      Dictionary<TestCaseResultIdentifier, List<int>> dictionary2 = new Dictionary<TestCaseResultIdentifier, List<int>>();
      Guid teamFoundationId = context.UserTeamFoundationId;
      bool isTcmService = context.IsTcmService;
      foreach (IGrouping<int, ResultUpdateRequest> resultsForUpdate in ((IEnumerable<ResultUpdateRequest>) requests).GroupBy<ResultUpdateRequest, int>((Func<ResultUpdateRequest, int>) (r => r.TestRunId)))
      {
        try
        {
          string iterationUri;
          Guid runProjGuid;
          TestRun run;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            resultUpdateResponseList = managementDatabase.UpdateTestResults2(projectFromName.GuidId, resultsForUpdate.Key, (IEnumerable<ResultUpdateRequest>) resultsForUpdate, teamFoundationId, autoComputeTestRunState, true, isTcmService, out iterationUri, out runProjGuid, out run);
          IList<TestCaseResult> testCaseResultList = (IList<TestCaseResult>) new List<TestCaseResult>();
          foreach (ResultUpdateRequest resultUpdateRequest in (IEnumerable<ResultUpdateRequest>) resultsForUpdate)
          {
            if (resultUpdateRequest.TestCaseResult != null)
              testCaseResultList.Add(resultUpdateRequest.TestCaseResult);
            if (resultUpdateRequest.Attachments != null && ((IEnumerable<TestResultAttachment>) resultUpdateRequest.Attachments).Any<TestResultAttachment>())
              attachments1.AddRange((IEnumerable<TestResultAttachment>) resultUpdateRequest.Attachments);
            if (resultUpdateRequest.AttachmentDeletes != null && ((IEnumerable<TestResultAttachmentIdentity>) resultUpdateRequest.AttachmentDeletes).Any<TestResultAttachmentIdentity>())
              attachments2.AddRange((IEnumerable<TestResultAttachmentIdentity>) resultUpdateRequest.AttachmentDeletes);
          }
          if (testCaseResultList.Any<TestCaseResult>())
            TestCaseResult.FireTestCaseResultsUpdatedNotification(context, testCaseResultList, projectName);
          if (run != null)
          {
            TestRun.UpdateProjectDataForRun(context, runProjGuid, run);
            TestRun.UpdateIterationPathForRun(context, iterationUri, run);
            TestRun.FireRunCompletedNotification(context, run);
            TestRun.LogRunCompletedTelemetry(context, run);
          }
        }
        catch (TestObjectUpdatedException ex)
        {
          resultUpdateResponseList = new List<ResultUpdateResponse>();
          foreach (ResultUpdateRequest resultUpdateRequest in (IEnumerable<ResultUpdateRequest>) resultsForUpdate)
            resultUpdateResponseList.Add(new ResultUpdateResponse()
            {
              Revision = -1,
              TestResultId = resultUpdateRequest.TestResultId
            });
        }
        foreach (ResultUpdateResponse resultUpdateResponse1 in resultUpdateResponseList)
        {
          Dictionary<TestCaseResultIdentifier, ResultUpdateResponse> dictionary3 = dictionary1;
          TestCaseResultIdentifier key = new TestCaseResultIdentifier();
          key.TestRunId = resultsForUpdate.Key;
          key.TestResultId = resultUpdateResponse1.TestResultId;
          ResultUpdateResponse resultUpdateResponse2 = resultUpdateResponse1;
          dictionary3.Add(key, resultUpdateResponse2);
        }
      }
      context.TraceVerbose("BusinessLayer", "TestCaseResult.Update - results updated");
      if (attachments1.Count > 0)
      {
        bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          dictionary2 = managementDatabase.CreateAttachments(projectFromName.GuidId, (IEnumerable<TestResultAttachment>) attachments1, false, changeCounterInterval);
      }
      if (attachments2.Count > 0)
      {
        try
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            managementDatabase.DeleteAttachments(projectFromName.GuidId, (IEnumerable<TestResultAttachmentIdentity>) attachments2);
        }
        catch (Exception ex)
        {
          context.TraceError("BusinessLayer", "TestCaseResult.Update threw excpetion - Exception message: {0}", (object) ex.Message);
        }
      }
      context.TraceVerbose("BusinessLayer", "TestCaseResult.Update - attachments update");
      int index = 0;
      foreach (ResultUpdateRequest request in requests)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier()
        {
          TestRunId = request.TestRunId,
          TestResultId = request.TestResultId
        };
        resultUpdateResponseArray[index] = dictionary1[key];
        resultUpdateResponseArray[index].LastUpdatedByName = context.UserTeamFoundationName;
        resultUpdateResponseArray[index].MaxReservedSubResultId = dictionary1[key].MaxReservedSubResultId;
        if (dictionary2.ContainsKey(key))
          resultUpdateResponseArray[index].AttachmentIds = dictionary2[key].ToArray();
        ++index;
      }
      return resultUpdateResponseArray;
    }

    internal static void UpdateExtensionFields(
      TestManagementRequestContext context,
      TestCaseResult[] results,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      results = results ?? Array.Empty<TestCaseResult>();
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      context.TraceVerbose("BusinessLayer", "TestCaseResult.UpdateExtensions - validation done");
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        foreach (IGrouping<int, TestCaseResult> results1 in ((IEnumerable<TestCaseResult>) results).GroupBy<TestCaseResult, int>((Func<TestCaseResult, int>) (r => r.TestRunId)))
        {
          try
          {
            managementDatabase.UpdateTestResultsExtension(projectFromName.GuidId, results1.Key, (IEnumerable<TestCaseResult>) results1, teamFoundationId);
          }
          catch (TestObjectUpdatedException ex)
          {
            context.TraceException("BusinessLayer", (Exception) ex);
          }
        }
        context.TraceVerbose("BusinessLayer", "TestCaseResult.UpdateExtensions - extensions updated");
      }
    }

    internal static TestCaseResult[] Reset(
      TestManagementRequestContext context,
      TestCaseResultIdentifier[] ids,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      TestCaseResult[] results = new TestCaseResult[ids.Length];
      TestRun[] testRunArray = new TestRun[ids.Length];
      Guid teamFoundationId = context.UserTeamFoundationId;
      bool isTcmService = context.IsTcmService;
      for (int index = 0; index < ids.Length; ++index)
      {
        string iterationUri;
        Guid runProjGuid;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        {
          TestRun run;
          results[index] = managementDatabase.ResetTestResult(projectFromName.GuidId, ids[index].TestRunId, ids[index].TestResultId, teamFoundationId, out iterationUri, out runProjGuid, out run, isTcmService);
          testRunArray[index] = run;
        }
        TestRun.UpdateProjectDataForRun(context, runProjGuid, testRunArray[index]);
        TestRun.UpdateIterationPathForRun(context, iterationUri, testRunArray[index]);
        if (results[index] != null)
          TestCaseResult.UpdateAreaPathForResults(context, new List<TestCaseResult>()
          {
            results[index]
          });
      }
      for (int index = 0; index < ids.Length; ++index)
      {
        TestCaseResult.FireTestCaseResultChangedNotification(context, ids[index], projectName);
        if (testRunArray[index] != null)
          TestRun.FireRunStartedNotification(context, testRunArray[index]);
      }
      TestCaseResult.ResolveUserNames(context, (IEnumerable<TestCaseResult>) results);
      return results;
    }

    internal static TestCaseResult FindInMultipleProjects(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      out string projectName)
    {
      TestRunArtifactInfo info = new TestRunArtifactInfo();
      TestCaseResult artifact = TestCaseResult.FindArtifact(context, testRunId, testResultId, ref info);
      projectName = info.ProjectName;
      return artifact;
    }

    internal static TestCaseResult FindArtifact(
      TestManagementRequestContext context,
      string toolSpecificId,
      ref TestRunArtifactInfo info)
    {
      int testRunId;
      int testResultId;
      return !ArtifactHelper.ParseTestCaseResultId(toolSpecificId, out testRunId, out testResultId) ? (TestCaseResult) null : TestCaseResult.FindArtifact(context, testRunId, testResultId, ref info);
    }

    internal static List<TestCaseResult> Query(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseResult.Query"))
      {
        ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), "Test Results");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        {
          excessIds = new List<TestCaseResultIdentifier>();
          return new List<TestCaseResult>();
        }
        AreaPathQueryTranslator pathQueryTranslator = new AreaPathQueryTranslator(context, query);
        query.QueryText = pathQueryTranslator.TranslateQuery();
        int lazyInitialization;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
        string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
        string orderClause = pathQueryTranslator.GenerateOrderClause();
        List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
        List<TestCaseResultIdentifier> ids = new List<TestCaseResultIdentifier>();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          ids = managementDatabase.QueryTestResults2(whereClause, orderClause, valueLists);
        return TestCaseResult.ProcessQueryResults(context, ids, out excessIds, pageSize, projectFromName.GuidId);
      }
    }

    internal static List<TestCaseResult> QueryWithSuiteDetails(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      int pageSize)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), "Test Results");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestCaseResult>();
      AreaPathQueryTranslator pathQueryTranslator = new AreaPathQueryTranslator(context, query);
      query.QueryText = pathQueryTranslator.TranslateQuery();
      int lazyInitialization;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
      string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
      string orderClause = pathQueryTranslator.GenerateOrderClause();
      List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
      List<TestCaseResultIdentifier> ids = new List<TestCaseResultIdentifier>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        ids = managementDatabase.QueryTestResults2(whereClause, orderClause, valueLists);
      return TestCaseResult.ProcessQueryResultsWithSuiteDetails(context, ids, pageSize, projectFromName.GuidId);
    }

    internal static List<TestCaseResult> QueryTestResultsForTestCaseId(
      TestManagementRequestContext context,
      string projectName,
      int testCaseId,
      int pageSize)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestCaseResult>();
      List<TestCaseResultIdentifier> ids = new List<TestCaseResultIdentifier>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        ids = managementDatabase.QueryTestResultsForTestCaseId(projectFromName.GuidId, testCaseId, pageSize);
      return TestCaseResult.ProcessQueryResultsWithSuiteDetails(context, ids, pageSize, projectFromName.GuidId);
    }

    internal static void CreateAssociatedWorkItems(
      TestManagementRequestContext context,
      TestCaseResultIdentifier[] identifiers,
      string[] workItemUris,
      string projectName = null)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseResult.CreateAssociatedWorkItems"))
      {
        GuidAndString guidAndString = new GuidAndString(string.Empty, Guid.Empty);
        if (!string.IsNullOrEmpty(projectName))
          guidAndString = Validator.CheckAndGetProjectFromName(context, projectName);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.CreateAssociatedWorkItems(identifiers, workItemUris, guidAndString.GuidId);
      }
    }

    internal static void DeleteAssociatedWorkItems(
      TestManagementRequestContext context,
      TestCaseResultIdentifier[] identifiers,
      string[] workItemUris)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DeleteAssociatedWorkItems(identifiers, workItemUris);
    }

    private static void UpdateAreaPathForResults(
      TestManagementRequestContext context,
      List<TestCaseResult> results)
    {
      if (results == null || !results.Any<TestCaseResult>())
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (TestCaseResult result in results)
      {
        if (!string.IsNullOrEmpty(result.TestCaseAreaUri))
          dictionary[result.TestCaseAreaUri] = string.Empty;
      }
      List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(dictionary.Keys.ToList<string>());
      if (nodes == null || !nodes.Any<TcmCommonStructureNodeInfo>())
        return;
      foreach (TcmCommonStructureNodeInfo structureNodeInfo in nodes)
        dictionary[structureNodeInfo.Uri] = structureNodeInfo.Path;
      foreach (TestCaseResult result in results)
      {
        if (!string.IsNullOrEmpty(result.TestCaseAreaUri))
        {
          if (dictionary.ContainsKey(result.TestCaseAreaUri))
            result.TestCaseArea = dictionary[result.TestCaseAreaUri];
          else
            context.TraceWarning("BusinessLayer", "Could not find area for area uri {0}", (object) result.TestCaseAreaUri);
        }
      }
    }

    private static void FireTestCaseResultChangedNotification(
      TestManagementRequestContext context,
      TestCaseResultIdentifier testCaseResultIdentifier,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestCaseResultChangedNotification(testCaseResultIdentifier, projectName));
    }

    private static void FireTestCaseResultsCreatedNotification(
      TestManagementRequestContext context,
      IList<TestCaseResult> testCaseResults,
      int testRunId,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestCaseResultsCreatedNotification(testCaseResults, testRunId, projectName));
    }

    private static void FireTestCaseResultsUpdatedNotification(
      TestManagementRequestContext context,
      IList<TestCaseResult> testCaseResults,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestCaseResultsUpdatedNotification(testCaseResults, projectName));
    }

    private static List<TestCaseResult> ResolveUserNames(
      TestManagementRequestContext context,
      List<TestCaseResult> results)
    {
      TestCaseResult.ResolveUserNames(context, results.AsEnumerable<TestCaseResult>());
      return results;
    }

    private static void ResolveUserNames(
      TestManagementRequestContext context,
      IEnumerable<TestCaseResult> results)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (TestCaseResult result in results)
      {
        source.Add(result.Owner);
        source.Add(result.RunBy);
        source.Add(result.LastUpdatedBy);
      }
      Guid[] array = source != null ? ((IEnumerable<Guid>) source.ToArray<Guid>()).Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty)).ToArray<Guid>() : (Guid[]) null;
      Dictionary<Guid, Tuple<string, string>> userGuidCache = IdentityHelper.ResolveIdentitiesEx(context, (IList<Guid>) array);
      foreach (TestCaseResult result in results)
      {
        string distinctDisplayName = TestCaseResult.GetDistinctDisplayName(context, result.Owner, userGuidCache);
        if (!string.IsNullOrEmpty(distinctDisplayName))
          result.OwnerName = distinctDisplayName;
        result.RunByName = TestCaseResult.GetDistinctDisplayName(context, result.RunBy, userGuidCache);
        result.LastUpdatedByName = TestCaseResult.GetDistinctDisplayName(context, result.LastUpdatedBy, userGuidCache);
      }
    }

    private static string GetDistinctDisplayName(
      TestManagementRequestContext context,
      Guid userGuid,
      Dictionary<Guid, Tuple<string, string>> userGuidCache)
    {
      Tuple<string, string> tuple;
      if (userGuidCache.TryGetValue(userGuid, out tuple))
        return tuple != null ? IdentityHelper.GetDistinctDisplayName(tuple.Item1, tuple.Item2) : (string) null;
      context.TraceAndDebugAssert("BusinessLayer", tuple == null, "No user found corresponding to Id " + userGuid.ToString());
      return string.Empty;
    }

    private static void getResultIdsMappedToRefs(
      TestCaseResult[] results,
      List<int> refIds,
      TestExtensionFieldDetails newFieldDetails)
    {
      for (int i = 0; i < ((IEnumerable<TestCaseResult>) results).Count<TestCaseResult>(); i++)
      {
        if (refIds.Any<int>((Func<int, bool>) (x => x == results[i].TestCaseReferenceId)))
        {
          if (results[i].CustomFields == null)
            results[i].CustomFields = new List<TestExtensionField>();
          results[i].CustomFields.Add(new TestExtensionField()
          {
            Field = newFieldDetails,
            Value = (object) 1
          });
        }
      }
    }
  }
}
