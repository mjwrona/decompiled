// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPlanXmlGenerationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestPlanXmlGenerationHelper
  {
    private Microsoft.TeamFoundation.TestManagement.Server.TestPlan m_selectedPlan;
    private int m_selectedPlanId;
    private Dictionary<int, TestSuiteModel> m_testSuitesHierarchy;
    private ExportHtmlDialogSettings m_dialogSettings;
    private TestPlansHelper m_planHelper;
    private TestSuitesHelper m_suiteHelper;
    private TestSettingsHelper m_testSettingsHelper;
    private TestEnvironmentsHelper m_testEnvironmentsHelper;
    private XmlDocument m_xmlDocument;
    private TestManagerRequestContext m_testContext;
    private const string c_manualRunsXmlElement = "manualRuns";
    private const string c_automatedRunsXmlElement = "automatedRuns";
    private const string c_buildsXmlElement = "builds";
    private const string c_allConfigurationXmlElement = "configurations";
    private const string c_singleConfigurationXmlElement = "configuration";
    private const string c_planConfigurationXmlElement = "planConfiguration";
    private const string c_rootNodeName = "testPlan";
    private const string c_suiteHierarchyXmlElement = "suiteHierarchy";
    private const string c_suiteXmlElement = "suite";
    private const string c_propertiesXmlElement = "properties";
    private const string c_testPlanSettingsXmlElement = "testPlanSettings";
    private const string c_variablesXmlElement = "variables";
    private const string c_descriptionXmlElement = "description";

    public TestPlanXmlGenerationHelper(TestManagerRequestContext requestContext) => this.m_testContext = requestContext;

    public TestPlanXmlGenerationHelper(
      TestManagerRequestContext testContext,
      ExportHtmlDialogSettings dialogSettings,
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan selectedPlan)
    {
      this.m_testContext = testContext;
      this.m_planHelper = new TestPlansHelper(testContext);
      this.m_suiteHelper = new TestSuitesHelper(testContext);
      this.m_testSettingsHelper = new TestSettingsHelper((TestManagementRequestContext) testContext.TestRequestContext);
      this.m_testEnvironmentsHelper = new TestEnvironmentsHelper((TestManagementRequestContext) testContext.TestRequestContext);
      this.m_selectedPlanId = selectedPlan.PlanId;
      this.m_selectedPlan = selectedPlan;
      this.m_dialogSettings = dialogSettings;
      if (!this.m_dialogSettings.isCheckboxChecked("testPlan_cb1") && !this.m_dialogSettings.isCheckboxChecked("testPlan_cb2"))
        return;
      this.m_testSuitesHierarchy = this.m_suiteHelper.GetSuitesTreeMapForPlan(this.m_selectedPlan.PlanId);
    }

    public XmlDocument GetPlanXml()
    {
      this.GeneratePlanXml();
      return this.m_xmlDocument;
    }

    private void GeneratePlanXml()
    {
      this.m_xmlDocument = new XmlDocument();
      XmlElement element = this.m_xmlDocument.CreateElement("testPlan");
      element.SetAttribute("id", this.m_selectedPlan.PlanId.ToString());
      element.SetAttribute("title", this.m_selectedPlan.Name);
      element.SetAttribute("url", PublicUrlHelper.GetTestPlanUrl(this.m_testContext, this.m_selectedPlanId));
      this.m_xmlDocument.AppendChild((XmlNode) element);
      this.AddTestPlanData();
    }

    private void AddTestPlanData()
    {
      if (!string.IsNullOrEmpty(this.m_selectedPlan.Description))
        this.AddDescription();
      if (this.m_dialogSettings.isCheckboxChecked("testPlan_cb0"))
        this.AddTestPlanProperties();
      if (this.m_dialogSettings.isCheckboxChecked("testPlan_cb1"))
        this.AddSuiteHierarchy();
      if (this.m_dialogSettings.isCheckboxChecked("testPlan_cb3"))
        this.AddTestPlanSettings();
      if (!this.m_dialogSettings.isCheckboxChecked("testPlan_cb2"))
        return;
      this.AddConfigurations();
    }

    private void AddSuiteHierarchy()
    {
      XmlElement documentElement = this.m_xmlDocument.DocumentElement;
      XmlElement element = this.m_xmlDocument.CreateElement("suiteHierarchy");
      if (this.m_testSuitesHierarchy != null && this.m_testSuitesHierarchy.Count > 0)
        this.AddSuite((XmlNode) element, this.m_testSuitesHierarchy[this.m_selectedPlan.RootSuiteId]);
      XmlElement newChild = element;
      documentElement.AppendChild((XmlNode) newChild);
    }

    private void AddSuite(XmlNode rootNode, TestSuiteModel suite)
    {
      XmlElement element = this.m_xmlDocument.CreateElement(nameof (suite));
      element.SetAttribute("id", suite.Id.ToString());
      element.SetAttribute("title", suite.Id == this.m_selectedPlan.RootSuiteId ? this.m_selectedPlan.Name : suite.Title);
      element.SetAttribute("url", PublicUrlHelper.GetTestSuiteUrl(this.m_testContext, suite.Id, this.m_selectedPlanId));
      element.SetAttribute("type", TestSuiteXmlGenerationHelper.GetTestSuiteType((int) suite.SuiteType));
      if (suite.ChildSuiteIds != null)
      {
        for (int index = 0; index < suite.ChildSuiteIds.Count; ++index)
        {
          int childSuiteId = suite.ChildSuiteIds[index];
          this.AddSuite((XmlNode) element, this.m_testSuitesHierarchy[childSuiteId]);
        }
      }
      rootNode.AppendChild((XmlNode) element);
    }

    private void AddDescription()
    {
      XmlElement documentElement = this.m_xmlDocument.DocumentElement;
      XmlElement element1 = this.m_xmlDocument.CreateElement("description");
      XmlElement element2 = this.m_xmlDocument.CreateElement("div");
      element2.InnerXml = HtmlParser.ParseHtml(string.IsNullOrEmpty(this.m_selectedPlan.EncodedDescription) ? string.Empty : this.m_selectedPlan.EncodedDescription).InnerXml;
      element1.AppendChild((XmlNode) element2);
      XmlElement newChild = element1;
      documentElement.AppendChild((XmlNode) newChild);
    }

    private void AddTestPlanProperties()
    {
      XmlElement documentElement = this.m_xmlDocument.DocumentElement;
      XmlElement element = this.m_xmlDocument.CreateElement("properties");
      string userDisplayName = new CachedIdentitiesCollection(this.m_testContext).GetUserDisplayName(this.m_selectedPlan.Owner);
      XmlCreationHelper.AddPopertyTag((XmlNode) element, this.m_xmlDocument, TestManagementResources.AreaPath, this.m_selectedPlan.AreaPath);
      XmlCreationHelper.AddPopertyTag((XmlNode) element, this.m_xmlDocument, TestManagementResources.Iteration, this.m_selectedPlan.Iteration);
      XmlCreationHelper.AddPopertyTag((XmlNode) element, this.m_xmlDocument, TestManagementResources.Owner, string.IsNullOrEmpty(userDisplayName) ? string.Empty : userDisplayName);
      XmlCreationHelper.AddPopertyTag((XmlNode) element, this.m_xmlDocument, TestManagementResources.ExportHtmlSuiteStateHeading, this.m_selectedPlan.Status);
      XmlCreationHelper.AddPopertyTag((XmlNode) element, this.m_xmlDocument, TestManagementResources.StartDate, this.m_selectedPlan.StartDate.ToString("D"));
      XmlCreationHelper.AddPopertyTag((XmlNode) element, this.m_xmlDocument, TestManagementResources.EndDate, this.m_selectedPlan.EndDate.ToString("D"));
      XmlElement newChild = element;
      documentElement.AppendChild((XmlNode) newChild);
    }

    private void AddTestPlanSettings()
    {
      XmlElement documentElement = this.m_xmlDocument.DocumentElement;
      XmlElement element = this.m_xmlDocument.CreateElement("testPlanSettings");
      this.AddManualRuns(element);
      this.AddAutomatedRuns(element);
      this.AddBuilds(element);
      XmlElement newChild = element;
      documentElement.AppendChild((XmlNode) newChild);
    }

    private void AddManualRuns(XmlElement rootNode)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement("manualRuns");
      XmlElement element2 = this.m_xmlDocument.CreateElement("properties");
      string str1 = TestManagementResources.DisplayTextNone;
      string str2 = TestManagementResources.DisplayTextNone;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings = (Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings) null;
      if (this.m_selectedPlan.TestSettingsId != 0)
        testSettings = this.m_testSettingsHelper.GetTestSetting(this.m_testContext.ProjectName, this.m_selectedPlan.TestSettingsId);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment testEnvironment = (Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment) null;
      if (this.m_selectedPlan.ManualTestEnvironmentId != Guid.Empty)
        testEnvironment = this.m_testEnvironmentsHelper.GetTestEnvironment(this.m_testContext.ProjectName, this.m_selectedPlan.ManualTestEnvironmentId);
      if (testSettings != null)
        str1 = testSettings.TestSettingsName;
      if (testEnvironment != null)
        str2 = testEnvironment.EnvironmentName;
      XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.Settings, str1);
      XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.Environment, str2);
      element1.AppendChild((XmlNode) element2);
      rootNode.AppendChild((XmlNode) element1);
    }

    private void AddAutomatedRuns(XmlElement rootNode)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement("automatedRuns");
      XmlElement element2 = this.m_xmlDocument.CreateElement("properties");
      string str1 = TestManagementResources.DisplayTextNone;
      string str2 = TestManagementResources.DisplayTextNone;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings = (Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings) null;
      if (this.m_selectedPlan.AutomatedTestSettingsId != 0)
        testSettings = this.m_testSettingsHelper.GetTestSetting(this.m_testContext.ProjectName, this.m_selectedPlan.AutomatedTestSettingsId);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment testEnvironment = (Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment) null;
      if (this.m_selectedPlan.ManualTestEnvironmentId != Guid.Empty)
        testEnvironment = this.m_testEnvironmentsHelper.GetTestEnvironment(this.m_testContext.ProjectName, this.m_selectedPlan.AutomatedTestEnvironmentId);
      if (testSettings != null)
        str1 = testSettings.TestSettingsName;
      if (testEnvironment != null)
        str2 = testEnvironment.EnvironmentName;
      XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.Settings, str1);
      XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.Environment, str2);
      element1.AppendChild((XmlNode) element2);
      rootNode.AppendChild((XmlNode) element1);
    }

    private void AddBuilds(XmlElement rootNode)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement("builds");
      XmlElement element2 = this.m_xmlDocument.CreateElement("properties");
      string str1 = TestManagementResources.DisplayTextNone;
      string str2 = TestManagementResources.DisplayTextNone;
      string str3 = TestManagementResources.DisplayTextNone;
      if (!string.IsNullOrEmpty(this.m_selectedPlan.BuildDefinition))
        str1 = this.m_selectedPlan.BuildDefinition;
      if (!string.IsNullOrEmpty(this.m_selectedPlan.BuildQuality))
        str2 = this.m_selectedPlan.BuildQuality;
      if (!string.IsNullOrEmpty(this.m_selectedPlan.BuildUri))
        str3 = this.m_selectedPlan.BuildUri;
      XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.Definition, str1);
      XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.Quality, str2);
      XmlCreationHelper.AddPopertyTag((XmlNode) element2, this.m_xmlDocument, TestManagementResources.BuildInUse, str3);
      element1.AppendChild((XmlNode) element2);
      rootNode.AppendChild((XmlNode) element1);
    }

    private void AppendConfigurationsData(XmlElement rootNode, TestConfigurationModel config)
    {
      XmlElement element1 = this.m_xmlDocument.CreateElement("configuration");
      element1.SetAttribute("value", config.Name);
      element1.SetAttribute("id", config.Id.ToString());
      if (config.Variables != null)
      {
        for (int index = 0; index < config.Variables.Count; ++index)
        {
          Microsoft.TeamFoundation.TestManagement.Server.NameValuePair variable = config.Variables[index];
          XmlElement element2 = this.m_xmlDocument.CreateElement("variables");
          element2.SetAttribute("name", variable.Name);
          element2.SetAttribute("value", variable.Value);
          element1.AppendChild((XmlNode) element2);
        }
      }
      rootNode.AppendChild((XmlNode) element1);
    }

    private void AddConfigurations()
    {
      XmlNode documentElement = (XmlNode) this.m_xmlDocument.DocumentElement;
      XmlElement element1 = this.m_xmlDocument.CreateElement("configurations");
      XmlElement element2 = this.m_xmlDocument.CreateElement("planConfiguration");
      List<TestConfigurationModel> configurationDetails = this.getConfigurationDetails(this.GetAllConfigurationIds());
      for (int index = 0; index < configurationDetails.Count; ++index)
        this.AppendConfigurationsData(element2, configurationDetails[index]);
      element1.AppendChild((XmlNode) element2);
      documentElement.AppendChild((XmlNode) element1);
    }

    private List<int> GetAllConfigurationIds()
    {
      HashSet<int> source = new HashSet<int>();
      foreach (KeyValuePair<int, TestSuiteModel> keyValuePair in this.m_testSuitesHierarchy)
      {
        TestSuiteModel testSuiteModel = keyValuePair.Value;
        if (testSuiteModel.Configurations != null)
          source.UnionWith((IEnumerable<int>) testSuiteModel.Configurations);
      }
      return source.ToList<int>();
    }

    public virtual List<TestConfigurationModel> getConfigurationDetails(
      List<int> allConfigurationIds)
    {
      return this.m_planHelper.GetConfigurationDetails(allConfigurationIds, this.m_selectedPlanId);
    }

    public Microsoft.TeamFoundation.TestManagement.Server.TestPlan SelectedPlan
    {
      get => this.m_selectedPlan;
      set => this.m_selectedPlan = value;
    }

    public ExportHtmlDialogSettings DialogSettings
    {
      get => this.m_dialogSettings;
      set => this.m_dialogSettings = value;
    }

    public Dictionary<int, TestSuiteModel> TestSuitesHierarchy
    {
      get => this.m_testSuitesHierarchy;
      set => this.m_testSuitesHierarchy = value;
    }
  }
}
