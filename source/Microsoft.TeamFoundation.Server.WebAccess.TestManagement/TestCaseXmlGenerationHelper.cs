// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestCaseXmlGenerationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestCaseXmlGenerationHelper
  {
    private TestSuiteDisplayModel m_selectedSuite;
    private Microsoft.TeamFoundation.TestManagement.Server.TestPlan m_selectedPlan;
    private ExportHtmlDialogSettings m_dialogSettings;
    private TestSuitesHelper m_suiteHelper;
    private TestPlansHelper m_planHelper;
    private PointsHelper m_pointsHelper;
    private ColumnSettingsHelper m_columnSettingsHelper;
    private XmlDocument m_xmlDocument;
    private TestManagerRequestContext m_testContext;
    private List<TestPointGridDisplayColumn> m_columnOptions;
    private TestHubUserSettings m_testHubUserSettings;
    private List<string> m_testCaseFields;
    private List<int> m_requirementIds;
    private List<ParametersXmlMap> m_parametersMap;
    private Dictionary<int, List<TestCaseIdToTestPointsMap>> m_testPointsForAllTestCasesOfSuite = new Dictionary<int, List<TestCaseIdToTestPointsMap>>();
    private Dictionary<int, TestCaseModel> m_testCaseMap = new Dictionary<int, TestCaseModel>();
    private Dictionary<int, SharedStepWorkItem> m_sharedStepCache = new Dictionary<int, SharedStepWorkItem>();
    private Dictionary<int, LinkedWorkItem> m_linkedWorkItemsInfoCache = new Dictionary<int, LinkedWorkItem>();
    public const string c_rootNodeName = "testCases";
    private const string c_testCaseXmlElement = "testCase";
    private const string c_propertiesXmlElement = "properties";
    private const string c_summaryXmlElement = "summary";
    private const string c_attachmentLinksClass = "test-step-attachment-links";
    private const string c_attachmentClass = "test-step-attachment";
    private const string c_attachmentNamesClass = "test-step-attachment-name";
    private const string c_attachmentSizeClass = "test-step-attachment-size";
    private const string c_testStepXmlElement = "testStep";
    private const string c_stepAttachmentsXmlElement = "stepAttachments";
    private const string c_testStepActionXmlElement = "testStepAction";
    private const string c_testStepExpectedXmlElement = "testStepExpected";
    private const string c_testStepsXmlElement = "testSteps";
    private const string c_latestTestOutcomesXmlElement = "latestTestOutcomes";
    private const string c_testResultXmlElement = "testResult";
    private const string c_automationXmlElement = "automation";
    private const string c_linksXmlElement = "links";
    private const string c_linkXmlElement = "link";
    private const string c_linksAndAttachmentsXmlElement = "linksAndAttachments";
    private const string c_attachmentsXmlElement = "attachments";
    private const string c_attachmentXmlElement = "attachment";
    private const string c_sharedParameterXmlElement = "sharedParameterDataSet";
    private const string c_defaultDuration = "0";

    public TestCaseXmlGenerationHelper(TestManagerRequestContext testContext) => this.m_testContext = testContext;

    public TestCaseXmlGenerationHelper(
      TestManagerRequestContext testContext,
      TestSuiteDisplayModel selectedSuite,
      ExportHtmlDialogSettings dialogSettings,
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan selectedPlan,
      List<ParametersXmlMap> testCaseParametersXmlMap)
    {
      this.m_testContext = testContext;
      this.m_suiteHelper = new TestSuitesHelper(testContext);
      this.m_planHelper = new TestPlansHelper(testContext);
      this.m_pointsHelper = new PointsHelper((TestManagementRequestContext) this.m_testContext.TestRequestContext);
      this.m_columnSettingsHelper = new ColumnSettingsHelper(testContext);
      this.m_testHubUserSettings = new TestHubUserSettings(testContext);
      this.m_selectedPlan = selectedPlan;
      this.m_selectedSuite = selectedSuite;
      this.m_dialogSettings = dialogSettings;
      this.m_parametersMap = testCaseParametersXmlMap;
      this.m_columnOptions = this.m_columnSettingsHelper.GetDisplayColumns(this.m_testHubUserSettings.GetColumnOptions());
    }

    public XmlDocument GenerateTestCaseXml(int suiteId, List<int> testCaseIds)
    {
      this.m_xmlDocument = new XmlDocument();
      this.PushTestCaseData(suiteId, testCaseIds);
      return this.m_xmlDocument;
    }

    public void PrefetchTestCaseData(
      Dictionary<int, List<int>> suiteIdToTestCaseIdsMap,
      List<int> idsToFetch)
    {
      if (idsToFetch != null && idsToFetch.Count > 0)
      {
        this.GetPointsWithLatestTestResult(suiteIdToTestCaseIdsMap);
        this.ParseAndCacheTestCase(this.GetTestCaseWorkItems(idsToFetch));
      }
      else
        this.GetLinkedWorkItems(new List<TestCaseModel>());
    }

    public void StoreRequirementIdsToGet(List<int> requirementIds) => this.m_requirementIds = requirementIds;

    private void PushTestCaseData(int suiteId, List<int> testCaseIds)
    {
      if (testCaseIds == null || testCaseIds.Count <= 0)
        return;
      XmlElement element1 = this.m_xmlDocument.CreateElement("testCases");
      XmlElement xmlElement1 = element1;
      int num = testCaseIds.Count;
      string str1 = num.ToString();
      xmlElement1.SetAttribute("count", str1);
      for (int index = 0; index < testCaseIds.Count; ++index)
      {
        TestCaseModel testCase = this.GetTestCase(testCaseIds[index]);
        string workItemEditUrl = this.getWorkItemEditUrl(testCase.Id);
        XmlElement element2 = this.m_xmlDocument.CreateElement("testCase");
        XmlElement xmlElement2 = element2;
        num = testCase.Id;
        string str2 = num.ToString();
        xmlElement2.SetAttribute("id", str2);
        element2.SetAttribute("title", testCase.Title.ToString());
        element2.SetAttribute("url", workItemEditUrl);
        this.AddTestCaseProperties(testCase, element2);
        this.AddTestCaseSummary(testCase, element2);
        this.AddTestSteps(testCase, element2);
        if (this.m_dialogSettings.isCheckboxChecked("testSuite_cb0"))
          this.AddParameters(testCase, element2);
        if (this.m_dialogSettings.isCheckboxChecked("testSuite_cb1"))
          this.AddLinksAndAttachmentsData(testCase, element2);
        if (this.m_dialogSettings.isCheckboxChecked("testSuite_cb2"))
          this.AddAutomation(testCase, element2);
        if (this.m_dialogSettings.isCheckboxChecked("testSuite_cb3") && suiteId >= 0 && this.m_testPointsForAllTestCasesOfSuite.ContainsKey(suiteId))
        {
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> fromTestPointsMap = this.GetTestPointsFromTestPointsMap(this.m_testPointsForAllTestCasesOfSuite[suiteId], testCase.Id);
          this.AddLatestTestOutcome(this.PopulateResultFieldsMap(), fromTestPointsMap, element2);
        }
        element1.AppendChild((XmlNode) element2);
      }
      this.m_xmlDocument.AppendChild((XmlNode) element1);
    }

    internal virtual string getWorkItemEditUrl(int testCaseId) => PublicUrlHelper.GetWorkItemEditUrl(this.m_testContext, testCaseId);

    internal virtual TestCaseModel GetTestCase(int testCaseId) => this.m_testCaseMap[testCaseId];

    private void AddParameters(TestCaseModel testCase, XmlElement rootNode)
    {
      string parametersXml = this.GetParametersXml(testCase);
      if (string.IsNullOrEmpty(parametersXml))
        return;
      XmlDocument safeXmlDocument = this.GetSafeXmlDocument(parametersXml);
      foreach (XmlElement selectNode in safeXmlDocument.DocumentElement.SelectNodes("sharedParameterDataSet"))
      {
        XmlAttribute attribute = selectNode.Attributes["id"];
        string str = string.Empty;
        int result = 0;
        if (attribute != null && int.TryParse(attribute.Value.ToString(), out result))
          str = PublicUrlHelper.GetSharedParameterUrl(this.m_testContext, result.ToString());
        selectNode.SetAttribute("url", str);
      }
      XmlNode newChild = this.m_xmlDocument.ImportNode((XmlNode) safeXmlDocument.DocumentElement, true);
      rootNode.AppendChild(newChild);
    }

    private string GetParametersXml(TestCaseModel testCase)
    {
      for (int index = 0; index < this.m_parametersMap.Count; ++index)
      {
        if (this.m_parametersMap[index].testCaseId == testCase.Id)
          return this.m_parametersMap[index].parametersXml;
      }
      return string.Empty;
    }

    private XmlDocument GetSafeXmlDocument(string parametersXml)
    {
      XmlDocument safeXmlDocument = new XmlDocument();
      using (StringReader input = new StringReader(parametersXml))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        }))
          safeXmlDocument.Load(reader);
      }
      return safeXmlDocument;
    }

    private Dictionary<string, string> PopulateResultFieldsMap() => new Dictionary<string, string>()
    {
      {
        "configurationName",
        TestManagementResources.TestPointGridColumnConfiguration
      },
      {
        "runBy",
        TestManagementResources.TestResultsPaneRunByColumnHeader
      },
      {
        "duration",
        TestManagementResources.TestResultsPaneDurationColumnHeaderTooltip
      },
      {
        "outcome",
        TestManagementResources.TestPointGridColumnOutcome
      },
      {
        "resultDate",
        TestManagementResources.QueryColumnNameDateCompleted
      },
      {
        "tester",
        TestManagementResources.TesterFilter
      },
      {
        "buildNumber",
        TestManagementResources.BuildNumber
      }
    };

    private void AddLatestTestOutcome(
      Dictionary<string, string> resultFieldColumnMap,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testResultsData,
      XmlElement rootNode)
    {
      XmlElement xmlElement = (XmlElement) null;
      if (testResultsData == null)
        return;
      for (int index = 0; index < testResultsData.Count; ++index)
      {
        if (index == 0)
        {
          xmlElement = this.m_xmlDocument.CreateElement("latestTestOutcomes");
          rootNode.AppendChild((XmlNode) xmlElement);
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint testResult = testResultsData[index];
        this.AddTestResultData(xmlElement, resultFieldColumnMap, testResult, index.ToString());
      }
    }

    private void AddTestResultData(
      XmlElement rootNode,
      Dictionary<string, string> resultFieldColumnMap,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint testResult,
      string indexString)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement(nameof (testResult));
      XmlElement element2 = this.m_xmlDocument.CreateElement("properties");
      List<string> stringList = new List<string>()
      {
        "outcome",
        "tester",
        "configurationName",
        "runBy",
        "resultDate",
        "duration",
        "buildNumber"
      };
      Dictionary<string, PointPropertyValue> dictionary = this.PopulateResultFieldValuesMap(testResult);
      for (int index = 0; index < stringList.Count; ++index)
      {
        if (dictionary.ContainsKey(stringList[index]) && resultFieldColumnMap.ContainsKey(stringList[index]))
        {
          string resultFieldColumn = resultFieldColumnMap[stringList[index]];
          PointPropertyValue pointPropertyValue = dictionary[stringList[index]] ?? new PointPropertyValue();
          if (string.IsNullOrEmpty(pointPropertyValue.Value))
            pointPropertyValue.Value = string.Empty;
          XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, resultFieldColumn, pointPropertyValue.Value, pointPropertyValue.Url);
        }
      }
      element1.AppendChild((XmlNode) element2);
      rootNode.AppendChild((XmlNode) element1);
    }

    private Dictionary<string, PointPropertyValue> PopulateResultFieldValuesMap(Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint testPoint)
    {
      Dictionary<string, PointPropertyValue> dictionary = new Dictionary<string, PointPropertyValue>();
      CachedIdentitiesCollection identitiesCollection = new CachedIdentitiesCollection(this.m_testContext);
      if (testPoint.Configuration != null)
        dictionary["configurationName"] = this.InitializePointPropertyValue(testPoint.Configuration.Name);
      if (testPoint.LastResultDetails != null)
      {
        if (testPoint.LastResultDetails.RunBy != null)
        {
          string userDisplayName = identitiesCollection.GetUserDisplayName(new Guid(testPoint.LastResultDetails.RunBy.Id));
          dictionary["runBy"] = this.InitializePointPropertyValue(string.IsNullOrEmpty(userDisplayName) ? string.Empty : userDisplayName);
        }
        float num = (float) (testPoint.LastResultDetails.Duration / 10000000L);
        string str = "0";
        if ((double) num > 0.0)
          str = num.ToString();
        dictionary["duration"] = this.InitializePointPropertyValue(str);
        DateTime dateCompleted = testPoint.LastResultDetails.DateCompleted;
        dictionary["resultDate"] = this.InitializePointPropertyValue(testPoint.LastResultDetails.DateCompleted.ToString("D"));
      }
      string url = string.Empty;
      if (testPoint.LastResult != null && testPoint.LastTestRun != null)
        url = PublicUrlHelper.GetTestRunUrl(this.m_testContext, testPoint.LastTestRun.Id, testPoint.LastResult.Id);
      string localizedString = TestOutcomeConverter.GetLocalizedString(testPoint.Outcome);
      dictionary["outcome"] = this.InitializePointPropertyValue(localizedString, url);
      dictionary["buildNumber"] = this.InitializePointPropertyValue(testPoint.LastRunBuildNumber);
      if (testPoint.AssignedTo != null)
      {
        string userDisplayName = identitiesCollection.GetUserDisplayName(new Guid(testPoint.AssignedTo.Id));
        dictionary["tester"] = this.InitializePointPropertyValue(string.IsNullOrEmpty(userDisplayName) ? string.Empty : userDisplayName);
      }
      return dictionary;
    }

    private PointPropertyValue InitializePointPropertyValue(string value, string url = "") => new PointPropertyValue()
    {
      Value = value,
      Url = url
    };

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetTestPointsFromTestPointsMap(
      List<TestCaseIdToTestPointsMap> testcasesResults,
      int testCaseId)
    {
      if (testcasesResults != null)
      {
        List<TestCaseIdToTestPointsMap> list = testcasesResults.Where<TestCaseIdToTestPointsMap>((Func<TestCaseIdToTestPointsMap, bool>) (t => t.IsTestCaseIdEqual(testCaseId))).ToList<TestCaseIdToTestPointsMap>();
        if (list != null && list.Count > 0)
          return list[0].testPoints;
      }
      return new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
    }

    private void AddAutomation(TestCaseModel testCase, XmlElement rootNode)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = testCase.WorkItem;
      XmlElement element1 = this.m_xmlDocument.CreateElement("automation");
      object fieldValue1 = workItem.GetFieldValue(this.m_testContext.TfsRequestContext, WorkItemFieldNames.TestName);
      object fieldValue2 = workItem.GetFieldValue(this.m_testContext.TfsRequestContext, WorkItemFieldNames.TestType);
      object fieldValue3 = workItem.GetFieldValue(this.m_testContext.TfsRequestContext, WorkItemFieldNames.Storage);
      if (fieldValue1 != null || fieldValue2 != null || fieldValue3 != null)
      {
        XmlElement element2 = this.m_xmlDocument.CreateElement("properties");
        if (fieldValue1 != null)
          XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.AutomatedTestName, HtmlParser.ParseHtml(fieldValue1.ToString()).InnerText);
        if (fieldValue3 != null)
          XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.AutomatedTestStorage, HtmlParser.ParseHtml(fieldValue3.ToString()).InnerText);
        if (fieldValue2 != null)
          XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.AutomatedTestType, HtmlParser.ParseHtml(fieldValue2.ToString()).InnerText);
      }
      rootNode.AppendChild((XmlNode) element1);
    }

    private List<WorkItemResourceLinkInfo> GetAttachmentsForTestCase(TestCaseModel testCase)
    {
      List<TestActionModel> testSteps = testCase.GetTestSteps();
      List<WorkItemResourceLinkInfo> list = testCase.WorkItem.ResourceLinks.ToList<WorkItemResourceLinkInfo>();
      List<WorkItemResourceLinkInfo> attachmentsForTestCase = new List<WorkItemResourceLinkInfo>();
      List<WorkItemResourceLinkInfo> resourceLinkInfoList = new List<WorkItemResourceLinkInfo>();
      for (int index = 0; index < testSteps.Count; ++index)
      {
        if (testSteps[index] is TestStepModel testStepModel)
          resourceLinkInfoList.AddRange((IEnumerable<WorkItemResourceLinkInfo>) testStepModel.Attachments);
      }
      for (int index = 0; index < list.Count; ++index)
      {
        if (resourceLinkInfoList.IndexOf(list[index]) == -1)
          attachmentsForTestCase.Add(list[index]);
      }
      return attachmentsForTestCase;
    }

    private void AddLinksData(TestCaseModel testCase, XmlElement rootNode)
    {
      bool flag = false;
      XmlElement newChild = (XmlElement) null;
      List<WorkItemLinkInfo> workItemLinkInfoList = testCase.WorkItem.WorkItemLinks == null ? new List<WorkItemLinkInfo>() : testCase.WorkItem.WorkItemLinks.ToList<WorkItemLinkInfo>();
      for (int index = 0; index < workItemLinkInfoList.Count; ++index)
      {
        if (!flag)
        {
          newChild = this.m_xmlDocument.CreateElement("links");
          rootNode.AppendChild((XmlNode) newChild);
          flag = true;
        }
        WorkItemLinkInfo workItemLinkInfo = workItemLinkInfoList[index];
        int targetId = workItemLinkInfo.TargetId;
        MDWorkItemLinkType linkTypeById = this.m_testContext.TfsRequestContext.GetService<WorkItemTrackingLinkService>().GetLinkTypeById(this.m_testContext.TfsRequestContext, workItemLinkInfo.LinkType);
        LinkedWorkItem linkedWorkItem = this.m_linkedWorkItemsInfoCache[targetId];
        if (linkedWorkItem != null)
        {
          XmlElement element = this.m_xmlDocument.CreateElement("link");
          element.SetAttribute("id", linkedWorkItem.Id.ToString());
          element.SetAttribute("workItemType", linkedWorkItem.Type);
          element.SetAttribute("title", linkedWorkItem.Title);
          element.SetAttribute("type", linkTypeById.ReverseEnd.Name);
          element.SetAttribute("url", PublicUrlHelper.GetWorkItemEditUrl(this.m_testContext, linkedWorkItem.Id));
          newChild.AppendChild((XmlNode) element);
        }
      }
    }

    private void AddLinksAndAttachmentsData(TestCaseModel testCase, XmlElement rootNode)
    {
      List<WorkItemResourceLinkInfo> attachmentsForTestCase = this.GetAttachmentsForTestCase(testCase);
      List<WorkItemLinkInfo> list = testCase.WorkItem.WorkItemLinks.ToList<WorkItemLinkInfo>();
      if ((attachmentsForTestCase == null || attachmentsForTestCase.Count <= 0) && (list == null || list.Count <= 0))
        return;
      XmlElement element1 = this.m_xmlDocument.CreateElement("linksAndAttachments");
      this.AddLinksData(testCase, element1);
      if (attachmentsForTestCase != null && attachmentsForTestCase.Count > 0)
      {
        XmlElement element2 = this.m_xmlDocument.CreateElement("attachments");
        for (int index = 0; index < attachmentsForTestCase.Count; ++index)
        {
          XmlElement element3 = this.m_xmlDocument.CreateElement("attachment");
          WorkItemResourceLinkInfo resourceLinkInfo = attachmentsForTestCase[index];
          string name = resourceLinkInfo.Name;
          string str1 = string.Format("{0}{1}", (object) Math.Ceiling((double) resourceLinkInfo.ResourceSize / 1024.0), (object) TestManagementResources.DisplayTextKilobyte);
          string attachmentUrl = PublicUrlHelper.GetAttachmentUrl(this.m_testContext, name, resourceLinkInfo.Location);
          string str2 = resourceLinkInfo.ResourceCreatedDate.ToString("D");
          string comment = resourceLinkInfo.Comment;
          element3.SetAttribute("name", name);
          element3.SetAttribute("size", str1);
          element3.SetAttribute("url", attachmentUrl);
          element3.SetAttribute("date", str2);
          element3.SetAttribute("comments", comment);
          element2.AppendChild((XmlNode) element3);
        }
        element1.AppendChild((XmlNode) element2);
      }
      rootNode.AppendChild((XmlNode) element1);
    }

    internal virtual void AddTestCaseProperties(TestCaseModel testCase, XmlElement rootNode)
    {
      XmlElement element = this.m_xmlDocument.CreateElement("properties");
      for (int index = 0; index < this.m_columnOptions.Count; ++index)
      {
        if (this.m_columnOptions[index].Name != "System.Id" && this.m_columnOptions[index].Name != "System.Title")
        {
          object fieldValue = testCase.WorkItem.GetFieldValue(this.m_testContext.TfsRequestContext, this.m_columnOptions[index].Name);
          if (fieldValue != null)
            XmlCreationHelper.AddPopertyTag((XmlNode) element, this.m_xmlDocument, this.m_columnOptions[index].Text, fieldValue.ToString());
        }
      }
      rootNode.AppendChild((XmlNode) element);
    }

    private void AddTestCaseSummary(TestCaseModel testCase, XmlElement rootNode)
    {
      object descriptionField = this.getTestCaseDescriptionField(testCase);
      if (descriptionField == null)
        return;
      XmlElement element = this.m_xmlDocument.CreateElement("summary");
      element.AppendChild((XmlNode) this.WrapInDiv(descriptionField.ToString()));
      rootNode.AppendChild((XmlNode) element);
    }

    internal virtual object getTestCaseDescriptionField(TestCaseModel testCase) => testCase.WorkItem.GetFieldValue(this.m_testContext.TfsRequestContext, "System.Description");

    private void CreateAttachmentList(XmlElement rootNode, TestStepModel testStep)
    {
      if (testStep.Attachments == null || testStep.Attachments.Count <= 0)
        return;
      XmlElement element1 = this.m_xmlDocument.CreateElement("div");
      element1.SetAttribute("class", "test-step-attachment-links");
      for (int index = 0; index < testStep.Attachments.Count; ++index)
      {
        WorkItemResourceLinkInfo attachment = testStep.Attachments[index];
        string name = attachment.Name;
        string str = string.Format(TestManagementResources.TestStepAttachmentSizeFormat, (object) Math.Ceiling((double) attachment.ResourceSize / 1024.0));
        string attachmentUrl = PublicUrlHelper.GetAttachmentUrl(this.m_testContext, name, attachment.Location);
        XmlElement element2 = this.m_xmlDocument.CreateElement("div");
        element1.SetAttribute("class", "test-step-attachment");
        XmlElement element3 = this.m_xmlDocument.CreateElement("a");
        element3.SetAttribute("target", "_blank");
        element3.SetAttribute("class", "test-step-attachment-name");
        element3.SetAttribute("href", attachmentUrl);
        element3.InnerText = name;
        XmlElement element4 = this.m_xmlDocument.CreateElement("span");
        element4.SetAttribute("class", "test-step-attachment-size");
        element4.InnerText = str;
        element2.AppendChild((XmlNode) element3);
        element2.AppendChild((XmlNode) element4);
        element1.AppendChild((XmlNode) element2);
      }
      rootNode.AppendChild((XmlNode) element1);
    }

    private void AddTestStepAttachements(XmlElement rootNode, TestStepModel testStep)
    {
      XmlElement element = this.m_xmlDocument.CreateElement("stepAttachments");
      this.CreateAttachmentList(element, testStep);
      rootNode.AppendChild((XmlNode) element);
    }

    private void AddTestStepData(
      XmlElement rootNode,
      TestStepModel testStep,
      int index,
      int sharedStepIndex = -1)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement(nameof (testStep));
      XmlElement element2 = this.m_xmlDocument.CreateElement("testStepAction");
      XmlElement element3 = this.m_xmlDocument.CreateElement("testStepExpected");
      string str = sharedStepIndex != -1 ? sharedStepIndex.ToString() + "." + index.ToString() : index.ToString();
      element2.AppendChild((XmlNode) this.WrapInDiv(testStep.Action));
      element3.AppendChild((XmlNode) this.WrapInDiv(testStep.ExpectedResult));
      element1.SetAttribute(nameof (index), str);
      this.AddTestStepAttachements(element1, testStep);
      element1.AppendChild((XmlNode) element2);
      element1.AppendChild((XmlNode) element3);
      rootNode.AppendChild((XmlNode) element1);
    }

    private XmlElement WrapInDiv(string text)
    {
      XmlElement element = this.m_xmlDocument.CreateElement("div");
      element.InnerXml = HtmlParser.ParseHtml(string.IsNullOrEmpty(text) ? string.Empty : text).InnerXml;
      return element;
    }

    internal virtual void AddTestSteps(TestCaseModel testCase, XmlElement rootNode)
    {
      testCase.ProcessTestStepAttachments(this.m_testContext);
      List<TestActionModel> testSteps = testCase.GetTestSteps();
      int num = 0;
      if (testSteps == null || testSteps.Count <= 0)
        return;
      XmlElement element = this.m_xmlDocument.CreateElement("testSteps");
      for (int index = 0; index < testSteps.Count; ++index)
      {
        TestActionModel testStep = testSteps[index];
        if (testStep is SharedStepModel sharedTestStep)
          this.AddSharedStepData(element, sharedTestStep, ++num);
        else
          this.AddTestStepData(element, testStep as TestStepModel, ++num);
      }
      rootNode.AppendChild((XmlNode) element);
    }

    private void AddSharedStepData(XmlElement rootNode, SharedStepModel sharedTestStep, int index)
    {
      SharedStepWorkItem sharedStepWorkItem = this.m_sharedStepCache[sharedTestStep.Refe];
      if (sharedStepWorkItem == null)
        return;
      XmlElement element1 = this.m_xmlDocument.CreateElement("testStep");
      XmlElement element2 = this.m_xmlDocument.CreateElement("testStepAction");
      element1.SetAttribute(nameof (index), index.ToString());
      element2.AppendChild((XmlNode) this.WrapInDiv(sharedStepWorkItem.Title));
      element1.AppendChild((XmlNode) element2);
      rootNode.AppendChild((XmlNode) element1);
      for (int index1 = 0; index1 < sharedStepWorkItem.TestSteps.Count; ++index1)
      {
        if (sharedStepWorkItem.TestSteps[index1] is TestStepModel testStep)
          this.AddTestStepData(rootNode, testStep, index1 + 1, index);
      }
    }

    private List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> GetTestCaseWorkItems(
      List<int> workItemIds)
    {
      this.m_testCaseFields = this.m_columnSettingsHelper.GetWorkItemFields((IEnumerable<TestPointGridDisplayColumn>) this.m_columnOptions.ToArray());
      this.m_testCaseFields.Add(WorkItemFieldNames.Actions);
      this.m_testCaseFields.Add("System.Description");
      return this.m_testContext.TfsRequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItems(this.m_testContext.TfsRequestContext, (IEnumerable<int>) workItemIds).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>();
    }

    private void ParseAndCacheTestCase(List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> workitems)
    {
      List<TestCaseModel> testCases = new List<TestCaseModel>();
      if (workitems == null || workitems.Count <= 0)
        return;
      for (int index = 0; index < workitems.Count; ++index)
      {
        TestCaseModel testCaseModel = new TestCaseModel(this.m_testContext, workitems[index]);
        this.m_testCaseMap[workitems[index].Id] = testCaseModel;
        testCases.Add(testCaseModel);
      }
      this.CacheSharedStepWits(this.GetSharedSteps(testCases));
      this.GetLinkedWorkItems(testCases);
    }

    private void CacheLinkedWorkItemsInfo(List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workitems)
    {
      if (workitems == null || workitems.Count <= 0)
        return;
      for (int index = 0; index < workitems.Count; ++index)
      {
        object obj1;
        workitems[index].Fields.TryGetValue(CoreFieldReferenceNames.Title, out obj1);
        object obj2;
        workitems[index].Fields.TryGetValue(CoreFieldReferenceNames.WorkItemType, out obj2);
        LinkedWorkItem linkedWorkItem = new LinkedWorkItem()
        {
          Id = workitems[index].Id.Value,
          Title = Convert.ToString(obj1),
          Type = Convert.ToString(obj2)
        };
        this.m_linkedWorkItemsInfoCache[linkedWorkItem.Id] = linkedWorkItem;
      }
    }

    private List<int> GetAllLinkedWorkItemIds(List<TestCaseModel> testCases)
    {
      List<int> linkedWorkItemIds = new List<int>();
      if (this.m_dialogSettings.isCheckboxChecked("testSuite_cb1") && testCases != null && testCases.Count > 0)
      {
        for (int index1 = 0; index1 < testCases.Count; ++index1)
        {
          TestCaseModel testCase = testCases[index1];
          List<WorkItemLinkInfo> workItemLinkInfoList = testCase.WorkItem.WorkItemLinks == null ? new List<WorkItemLinkInfo>() : testCase.WorkItem.WorkItemLinks.ToList<WorkItemLinkInfo>();
          for (int index2 = 0; index2 < workItemLinkInfoList.Count; ++index2)
          {
            WorkItemLinkInfo workItemLinkInfo = workItemLinkInfoList[index2];
            linkedWorkItemIds.Add(workItemLinkInfo.TargetId);
          }
        }
      }
      return linkedWorkItemIds;
    }

    private void GetLinkedWorkItems(List<TestCaseModel> testCases)
    {
      List<int> linkedWorkItemIds = this.GetAllLinkedWorkItemIds(testCases);
      List<string> fields = new List<string>()
      {
        "System.Id",
        "System.Title",
        "System.WorkItemType"
      };
      if (linkedWorkItemIds.Count <= 0)
        return;
      this.CacheLinkedWorkItemsInfo(this.m_testContext.TfsRequestContext.GetService<IWitHelper>().GetWorkItems(this.m_testContext.TfsRequestContext, linkedWorkItemIds, fields).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>());
    }

    private List<int> GetSharedStepsIdsToGet(List<TestCaseModel> testCases)
    {
      List<int> source = new List<int>();
      if (testCases != null && testCases.Count > 0)
      {
        for (int index1 = 0; index1 < testCases.Count; ++index1)
        {
          List<TestActionModel> testSteps = testCases[index1].GetTestSteps();
          if (testSteps != null && testSteps.Count > 0)
          {
            for (int index2 = 0; index2 < testSteps.Count; ++index2)
            {
              if (testSteps[index2] is SharedStepModel sharedStepModel)
              {
                int refe = sharedStepModel.Refe;
                source.Add(refe);
              }
            }
          }
        }
      }
      return source.Distinct<int>().ToList<int>();
    }

    private SharedStepWorkItem GetSharedStepFromWorkItem(Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem)
    {
      int id = workItem.Id;
      int revision = workItem.Revision;
      object fieldValue1 = workItem.GetFieldValue(this.m_testContext.TfsRequestContext, "System.Title");
      object fieldValue2 = workItem.GetFieldValue(this.m_testContext.TfsRequestContext, WorkItemFieldNames.Actions);
      return new SharedStepWorkItem()
      {
        Id = id,
        Revision = revision,
        Title = fieldValue1 == null ? string.Empty : fieldValue1.ToString(),
        TestSteps = TestStepXmlParserHelper.GetTestStepsArray(fieldValue2 == null ? string.Empty : fieldValue2.ToString()),
        WorkItem = workItem
      };
    }

    private List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> GetSharedSteps(
      List<TestCaseModel> testCases)
    {
      List<int> sharedStepsIdsToGet = this.GetSharedStepsIdsToGet(testCases);
      return this.m_testContext.TfsRequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItems(this.m_testContext.TfsRequestContext, (IEnumerable<int>) sharedStepsIdsToGet).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>();
    }

    private void CacheSharedStepWits(List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> workitems)
    {
      if (workitems == null || workitems.Count <= 0)
        return;
      for (int index = 0; index < workitems.Count; ++index)
      {
        SharedStepWorkItem stepFromWorkItem = this.GetSharedStepFromWorkItem(workitems[index]);
        this.m_sharedStepCache[stepFromWorkItem.Id] = stepFromWorkItem;
      }
    }

    private void AddTestPointsToTheCorrespondingTestCaseAndSuite(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPoints,
      int suiteId,
      int caseId)
    {
      if (!this.m_testPointsForAllTestCasesOfSuite.ContainsKey(suiteId))
      {
        this.m_testPointsForAllTestCasesOfSuite[suiteId] = new List<TestCaseIdToTestPointsMap>();
      }
      else
      {
        List<TestCaseIdToTestPointsMap> list = this.m_testPointsForAllTestCasesOfSuite[suiteId].Where<TestCaseIdToTestPointsMap>((Func<TestCaseIdToTestPointsMap, bool>) (map => map.IsTestCaseIdEqual(caseId))).ToList<TestCaseIdToTestPointsMap>();
        if (list != null && list.Count > 0)
        {
          list[0].testPoints = list[0].testPoints.Concat<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>) testPoints).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
          return;
        }
      }
      this.m_testPointsForAllTestCasesOfSuite[suiteId].AddRange((IEnumerable<TestCaseIdToTestPointsMap>) new List<TestCaseIdToTestPointsMap>()
      {
        new TestCaseIdToTestPointsMap()
        {
          testCaseId = caseId,
          testPoints = testPoints
        }
      });
    }

    private void InsertPointsIntoTestPointsMap(
      int suiteId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPointsWithLastResults,
      Dictionary<int, List<int>> suiteIdToTestCaseIdsMap)
    {
      List<int> suiteIdToTestCaseIds = suiteIdToTestCaseIdsMap[suiteId];
      int count = suiteIdToTestCaseIds.Count;
      if (testPointsWithLastResults == null)
        return;
      for (int index1 = 0; index1 < count; ++index1)
      {
        int caseId = suiteIdToTestCaseIds[index1];
        for (int index2 = 0; index2 < testPointsWithLastResults.Count; ++index2)
        {
          if (testPointsWithLastResults[index2].TestCase.Id == caseId.ToString((IFormatProvider) CultureInfo.InvariantCulture))
            this.AddTestPointsToTheCorrespondingTestCaseAndSuite(new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>()
            {
              testPointsWithLastResults[index2]
            }, suiteId, caseId);
        }
      }
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetPointsContainingLastResultDetails(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPointsWithResults)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> lastResultDetails = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
      if (testPointsWithResults != null && testPointsWithResults.Count > 0)
        lastResultDetails = testPointsWithResults.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint, bool>) (point => point.LastResultDetails != null)).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
      return lastResultDetails;
    }

    private void GetPointsWithLatestTestResult(Dictionary<int, List<int>> suiteIdToTestCaseIdsMap)
    {
      if (!this.m_dialogSettings.isCheckboxChecked("testSuite_cb3"))
        return;
      foreach (KeyValuePair<int, List<int>> suiteIdToTestCaseIds in suiteIdToTestCaseIdsMap)
      {
        int key = suiteIdToTestCaseIds.Key;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> lastResultDetails = this.GetPointsContainingLastResultDetails(this.m_pointsHelper.GetPoints(this.m_testContext.ProjectName, this.m_selectedPlan.PlanId, key, string.Empty, string.Empty, string.Empty, string.Empty, false, 0, int.MaxValue));
        this.InsertPointsIntoTestPointsMap(key, lastResultDetails, suiteIdToTestCaseIdsMap);
      }
    }

    public ExportHtmlDialogSettings DialogSettings
    {
      get => this.m_dialogSettings;
      set => this.m_dialogSettings = value;
    }
  }
}
