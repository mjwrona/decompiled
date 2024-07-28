// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestSuiteXmlGenerationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestSuiteXmlGenerationHelper
  {
    private TestSuiteDisplayModel m_selectedSuite;
    private TestPlan m_selectedPlan;
    private List<TestSuiteDisplayModel> m_testSuitesHierarchy;
    private ExportHtmlDialogSettings m_dialogSettings;
    private TestSuitesHelper m_suiteHelper;
    private XmlDocument m_xmlDocument;
    private TestManagerRequestContext m_testContext;
    private TestCaseXmlGenerationHelper m_testCaseXmlHelper;
    private List<int> m_suiteAddedToHirearchy = new List<int>();
    private bool m_addTestCase;
    private bool m_addsuite;
    private const string c_rootNodeName = "testSuites";
    private const string c_testSuiteXmlElement = "testSuite";
    private const string c_suitePropertiesXmlElement = "suiteProperties";
    private const string c_propertiesXmlElement = "properties";
    private const string c_requirementXmlElement = "requirement";
    private const string c_allconfigurationsXmlElement = "configurations";
    private const string c_configurationXmlElement = "configuration";
    private const string c_rootSuiteName = "<root>";

    public TestSuiteXmlGenerationHelper(
      TestManagerRequestContext testContext,
      List<TestSuiteDisplayModel> testSuitesHierarchy,
      TestSuiteDisplayModel selectedSuite,
      ExportHtmlDialogSettings dialogSettings,
      TestPlan selectedPlan,
      List<ParametersXmlMap> testCaseParametersXmlMap)
    {
      this.m_testContext = testContext;
      this.m_suiteHelper = new TestSuitesHelper(testContext);
      this.m_testCaseXmlHelper = new TestCaseXmlGenerationHelper(testContext, selectedSuite, dialogSettings, selectedPlan, testCaseParametersXmlMap);
      this.m_selectedPlan = selectedPlan;
      this.m_testSuitesHierarchy = testSuitesHierarchy;
      this.m_selectedSuite = selectedSuite;
      this.m_dialogSettings = dialogSettings;
      this.SetSuiteAndTestCasesCheckboxes();
    }

    public static string GetTestSuiteType(int testSuiteType)
    {
      string testSuiteType1 = string.Empty;
      switch (testSuiteType)
      {
        case 1:
          testSuiteType1 = TestManagementResources.ExportHtmlSuiteTypeQuery;
          break;
        case 2:
          testSuiteType1 = TestManagementResources.ExportHtmlSuiteTypeStatic;
          break;
        case 3:
          testSuiteType1 = TestManagementResources.ExportHtmlSuiteTypeRequirement;
          break;
      }
      return testSuiteType1;
    }

    public XmlDocument GetSuiteXml()
    {
      if (this.m_addsuite)
      {
        this.StoreRequirementIdsLinkedToSuites();
        this.PreFetchTestCaseData();
        this.GenerateSuiteXml();
      }
      return this.m_xmlDocument;
    }

    private Dictionary<int, List<int>> GetSuiteIdToAllTestCaseIdsMap()
    {
      Dictionary<int, List<int>> allTestCaseIdsMap = new Dictionary<int, List<int>>();
      for (int index = 0; index < this.m_testSuitesHierarchy.Count; ++index)
        allTestCaseIdsMap[this.m_testSuitesHierarchy[index].Id] = this.m_testSuitesHierarchy[index].TestCaseIds;
      return allTestCaseIdsMap;
    }

    private List<int> GetAllTestCaseIdsToFetch()
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.m_testSuitesHierarchy.Count; ++index)
      {
        List<int> testCaseIds = this.m_testSuitesHierarchy[index].TestCaseIds;
        intList = intList.Concat<int>((IEnumerable<int>) testCaseIds).ToList<int>();
      }
      return intList.Distinct<int>().ToList<int>();
    }

    private void PreFetchTestCaseData()
    {
      List<int> idsToFetch;
      Dictionary<int, List<int>> suiteIdToTestCaseIdsMap;
      if (this.m_addTestCase)
      {
        idsToFetch = this.GetAllTestCaseIdsToFetch();
        suiteIdToTestCaseIdsMap = this.GetSuiteIdToAllTestCaseIdsMap();
      }
      else
      {
        idsToFetch = new List<int>();
        suiteIdToTestCaseIdsMap = new Dictionary<int, List<int>>();
      }
      this.m_testCaseXmlHelper.PrefetchTestCaseData(suiteIdToTestCaseIdsMap, idsToFetch);
    }

    private void StoreRequirementIdsLinkedToSuites()
    {
      List<int> source = new List<int>();
      for (int index = 0; index < this.m_testSuitesHierarchy.Count; ++index)
      {
        TestSuiteDisplayModel suiteDisplayModel = this.m_testSuitesHierarchy[index];
        if (suiteDisplayModel.SuiteType == TestSuiteEntryType.RequirementTestSuite)
          source.Add(suiteDisplayModel.RequirementId);
      }
      this.m_testCaseXmlHelper.StoreRequirementIdsToGet(source.Distinct<int>().ToList<int>());
    }

    private void GenerateSuiteXml()
    {
      this.m_xmlDocument = new XmlDocument();
      this.m_xmlDocument.AppendChild((XmlNode) this.m_xmlDocument.CreateElement("testSuites"));
      for (int index = 0; index < this.m_testSuitesHierarchy.Count; ++index)
        this.AddSuiteXml(this.m_testSuitesHierarchy[index]);
    }

    private void AddSuiteXml(TestSuiteDisplayModel suite)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement("testSuite");
      XmlElement documentElement = this.m_xmlDocument.DocumentElement;
      element1.SetAttribute("id", suite.Id.ToString());
      element1.SetAttribute("title", suite.Title == "<root>" ? this.m_selectedPlan.Name : suite.Title);
      element1.SetAttribute("url", PublicUrlHelper.GetTestSuiteUrl(this.m_testContext, suite.Id, this.m_selectedPlan.PlanId));
      if (this.m_dialogSettings.isCheckboxChecked("suiteProperties"))
      {
        XmlElement element2 = this.m_xmlDocument.CreateElement("suiteProperties");
        XmlElement element3 = this.m_xmlDocument.CreateElement("properties");
        XmlCreationHelper.AddPopertyTag((XmlNode) element3, this.m_xmlDocument, TestManagementResources.ExportHtmlSuiteStateHeading, suite.Status);
        XmlCreationHelper.AddPopertyTag((XmlNode) element3, this.m_xmlDocument, TestManagementResources.ExportHtmlSuiteTypeHeading, TestSuiteXmlGenerationHelper.GetTestSuiteType((int) suite.SuiteType));
        if (suite.SuiteType == TestSuiteEntryType.RequirementTestSuite)
        {
          string workItemEditUrl = PublicUrlHelper.GetWorkItemEditUrl(this.m_testContext, suite.RequirementId);
          XmlElement element4 = this.m_xmlDocument.CreateElement("requirement");
          element4.SetAttribute("id", suite.RequirementId.ToString());
          element4.SetAttribute("url", workItemEditUrl);
          element3.AppendChild((XmlNode) element4);
        }
        this.AddConfiguration(element2, suite);
        element2.AppendChild((XmlNode) element3);
        element1.AppendChild((XmlNode) element2);
      }
      if (this.m_dialogSettings.isCheckboxChecked("suiteTestCases"))
      {
        XmlDocument testCaseXml = this.m_testCaseXmlHelper.GenerateTestCaseXml(suite.Id, suite.TestCaseIds);
        if (testCaseXml != null && testCaseXml.DocumentElement != null)
        {
          XmlNode newChild = this.m_xmlDocument.ImportNode((XmlNode) testCaseXml.DocumentElement, true);
          element1.AppendChild(newChild);
        }
      }
      XmlElement newChild1 = element1;
      documentElement.AppendChild((XmlNode) newChild1);
    }

    private void AddConfiguration(XmlElement rootNode, TestSuiteDisplayModel suite)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement("configurations");
      if (suite.Configurations != null)
      {
        for (int index = 0; index < suite.Configurations.Count; ++index)
        {
          XmlElement element2 = this.m_xmlDocument.CreateElement("configuration");
          element2.SetAttribute("value", suite.Configurations[index].Name);
          element1.AppendChild((XmlNode) element2);
        }
      }
      rootNode.AppendChild((XmlNode) element1);
    }

    private void SetSuiteAndTestCasesCheckboxes()
    {
      if (this.m_dialogSettings.isCheckboxChecked("suiteTestCases") || this.m_dialogSettings.isCheckboxChecked("testSuite_cb0") || this.m_dialogSettings.isCheckboxChecked("testSuite_cb1") || this.m_dialogSettings.isCheckboxChecked("testSuite_cb2"))
        this.m_addTestCase = true;
      if (!this.m_dialogSettings.isCheckboxChecked("suiteProperties") && !this.m_addTestCase)
        return;
      this.m_addsuite = true;
    }
  }
}
