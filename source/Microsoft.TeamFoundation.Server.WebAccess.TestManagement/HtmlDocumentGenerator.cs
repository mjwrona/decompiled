// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.HtmlDocumentGenerator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class HtmlDocumentGenerator : TestHelperBase
  {
    private XmlDocument m_xmlDocument;
    private TestSuitesHelper m_suiteHelper;
    private TestPlan m_selectedPlan;
    private TestManagerRequestContext m_testContext;
    private ColumnSettingsHelper m_columnSettingsHelper;
    private TestHubUserSettings m_testHubUserSettings;
    private TestSuiteDisplayModel m_selectedSuite;
    private TestPointGridDisplayColumn[] m_columnOptions;
    private List<TestSuiteDisplayModel> m_testSuitesHierarchyForSuite;
    private ExportHtmlDialogSettings m_dialogSettings;
    private TestPlanXmlGenerationHelper m_testPlanXmlGenerator;
    private TestSuiteXmlGenerationHelper m_testSuiteXmlGenerator;
    private List<ParametersXmlMap> m_parametersXmlMap;
    private string m_xsltFileText;
    private const string c_exportToHtmlSendMailJobName = "ExportToHtmlSendMailJob";
    private const string c_exportToHtmlSendMailJobExtensionName = "Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ExportToHtmlSendMailJob";
    private const string c_planAndSuitesElement = "planAndSuites";

    internal HtmlDocumentGenerator(
      TestManagerRequestContext testContext,
      int planId,
      int suiteId,
      string parametersXml,
      string xsltFileText,
      List<string> dialogSettings)
      : base(testContext)
    {
      this.m_testContext = testContext;
      this.m_suiteHelper = new TestSuitesHelper(testContext);
      this.m_testHubUserSettings = new TestHubUserSettings(testContext);
      this.m_columnSettingsHelper = new ColumnSettingsHelper(testContext);
      this.m_dialogSettings = new ExportHtmlDialogSettings(dialogSettings);
      bool includeChildSuite = !this.m_dialogSettings.isCheckboxChecked("suiteOnly");
      this.m_selectedPlan = this.FetchTestPlan(planId);
      this.m_testSuitesHierarchyForSuite = this.m_suiteHelper.GetSuiteHierarchy(suiteId, includeChildSuite);
      this.m_selectedSuite = this.m_testSuitesHierarchyForSuite.Where<TestSuiteDisplayModel>((Func<TestSuiteDisplayModel, bool>) (s => s.Id == suiteId)).ToList<TestSuiteDisplayModel>().First<TestSuiteDisplayModel>();
      if (this.m_dialogSettings.isCheckboxChecked("testPlan"))
        this.m_testPlanXmlGenerator = new TestPlanXmlGenerationHelper(testContext, this.m_dialogSettings, this.m_selectedPlan);
      this.m_columnOptions = this.m_columnSettingsHelper.GetDisplayColumns(this.m_testHubUserSettings.GetColumnOptions()).ToArray();
      this.m_xsltFileText = xsltFileText;
      this.m_parametersXmlMap = JsonConvert.DeserializeObject<List<ParametersXmlMap>>(parametersXml);
      this.m_testSuiteXmlGenerator = new TestSuiteXmlGenerationHelper(testContext, this.m_testSuitesHierarchyForSuite, this.m_selectedSuite, this.m_dialogSettings, this.m_selectedPlan, this.m_parametersXmlMap);
    }

    public string GetHtmlReport()
    {
      using (XmlReader input = XmlReader.Create((TextReader) new StringReader(this.GetReportXml())))
      {
        using (XmlReader xsltReader = XsltTransformationHelper.GetXsltReader(this.m_testContext, this.m_xsltFileText))
        {
          XslCompiledTransform compiledTransform = new XslCompiledTransform();
          compiledTransform.Load(xsltReader);
          StringWriter w = new StringWriter();
          using (XmlTextWriter results = new XmlTextWriter((TextWriter) w))
          {
            compiledTransform.Transform(input, (XsltArgumentList) null, (XmlWriter) results);
            return w.ToString();
          }
        }
      }
    }

    public static void ScheduleSendMailJob(
      TestManagerRequestContext testContext,
      int planId,
      int suiteid,
      string parametersXml,
      string notes,
      Microsoft.TeamFoundation.Server.WebAccess.Mail.MailMessage message)
    {
      TeamFoundationJobService service1 = testContext.TfsRequestContext.GetService<TeamFoundationJobService>();
      string xsltFile = XsltTransformationHelper.GetXsltFile(testContext);
      ExportHtmlDialogSettings htmlDialogSettings = new ExportHtmlDialogSettings(testContext);
      WebApiTeam team = testContext.Team;
      Guid teamId = team != null ? team.Id : Guid.Empty;
      ExportToHtmSendMailJobDataModel objectToSerialize = new ExportToHtmSendMailJobDataModel(planId, suiteid, parametersXml, notes, testContext.ProjectName, teamId, message, testContext.CurrentProjectGuid, xsltFile, htmlDialogSettings.GetDialogSettings());
      System.Net.Mail.MailMessage mailMessage = message.CreateMailMessage(testContext.TfsRequestContext, testContext.TfsWebContext.IsHosted, out List<Guid> _);
      TeamFoundationMailService service2 = testContext.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationMailService>();
      string empty = string.Empty;
      string htmlReport = new HtmlDocumentGenerator(testContext, objectToSerialize.PlanId, objectToSerialize.SuiteId, objectToSerialize.ParametersXml, objectToSerialize.XsltFileText, objectToSerialize.DialogSettings).GetHtmlReport();
      string str = empty + objectToSerialize.NotesText + htmlReport;
      mailMessage.Body = str;
      IVssRequestContext tfsRequestContext1 = testContext.TfsRequestContext;
      System.Net.Mail.MailMessage message1 = mailMessage;
      service2.ValidateMessage(tfsRequestContext1, message1);
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      IVssRequestContext tfsRequestContext2 = testContext.TfsRequestContext;
      XmlNode jobData = xml;
      service1.QueueOneTimeJob(tfsRequestContext2, "ExportToHtmlSendMailJob", "Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ExportToHtmlSendMailJob", jobData, true);
    }

    private string GetReportXml()
    {
      this.m_xmlDocument = new XmlDocument();
      XmlElement element = this.m_xmlDocument.CreateElement("planAndSuites");
      this.m_xmlDocument.AppendChild((XmlNode) element);
      if (this.m_dialogSettings.isCheckboxChecked("testPlan"))
      {
        XmlDocument planXml = this.m_testPlanXmlGenerator.GetPlanXml();
        if (planXml != null)
        {
          XmlNode newChild = this.m_xmlDocument.ImportNode((XmlNode) planXml.DocumentElement, true);
          element.AppendChild(newChild);
        }
      }
      XmlDocument suiteXml = this.m_testSuiteXmlGenerator.GetSuiteXml();
      if (suiteXml != null)
      {
        XmlNode newChild = this.m_xmlDocument.ImportNode((XmlNode) suiteXml.DocumentElement, true);
        element.AppendChild(newChild);
      }
      this.m_xmlDocument.AppendChild((XmlNode) element);
      return this.m_xmlDocument.OuterXml;
    }

    private TestPlan FetchTestPlan(int testPlanId)
    {
      List<TestPlan> source = TestPlan.Fetch((TestManagementRequestContext) this.m_testContext.TestRequestContext, new IdAndRev[1]
      {
        new IdAndRev() { Id = testPlanId }
      }, new List<int>(), this.m_testContext.ProjectName);
      return source != null ? source.FirstOrDefault<TestPlan>() : (TestPlan) null;
    }
  }
}
