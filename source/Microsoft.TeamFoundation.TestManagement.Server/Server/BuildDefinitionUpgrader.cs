// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BuildDefinitionUpgrader
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class BuildDefinitionUpgrader
  {
    private TfsTestManagementRequestContext m_tcmRequestContext;
    private MigrationLogger m_logger;
    private string m_teamProjectName;
    private Guid m_projectGuid;
    private const string TestPlanIdAttributeName = "TestPlanId";
    private const string LabWorkFlowNameSpaceAttribute = "xmlns:mtlwa";
    private const string RunTestDetailsXPath = "//mtlwa:LabWorkflowDetails/mtlwa:LabWorkflowDetails.TestParameters/mtlwa:RunTestDetails";
    private const string TestSuitesIdListNodeName = "mtlwa:RunTestDetails.TestSuiteIdList";

    internal BuildDefinitionUpgrader(
      TfsTestManagementRequestContext tcmRequestContext,
      MigrationLogger logger,
      string projectName,
      Guid projectGuid)
    {
      this.m_tcmRequestContext = tcmRequestContext;
      this.m_logger = logger;
      this.m_teamProjectName = projectName;
      this.m_projectGuid = projectGuid;
    }

    internal void Perform()
    {
      Dictionary<int, int> suiteTcmIdWitIdMap = (Dictionary<int, int>) null;
      Dictionary<int, int> planTcmIdWitIdMap = (Dictionary<int, int>) null;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_tcmRequestContext))
        planTcmIdWitIdMap = planningDatabase.FetchTcmIdWitIdMap((TestManagementRequestContext) this.m_tcmRequestContext, this.m_projectGuid, out suiteTcmIdWitIdMap);
      IVssRequestContext requestContext = this.m_tcmRequestContext.RequestContext;
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      foreach (BuildDefinition buildDefinition in this.GetBuildDefinitions(requestContext, service))
      {
        try
        {
          this.UpgradeBuildDefinition(requestContext, service, buildDefinition, planTcmIdWitIdMap, suiteTcmIdWitIdMap);
        }
        catch (Exception ex)
        {
          this.m_logger.Log(TraceLevel.Warning, string.Format("Ignoring buildDefinition {0} for upgrade. Exception Details: {1}", (object) buildDefinition.Name, (object) ex.ToString()));
        }
      }
    }

    private BuildDefinition[] GetBuildDefinitions(
      IVssRequestContext requestContext,
      TeamFoundationBuildService buildService)
    {
      string str = BuildPath.Root(this.m_teamProjectName, "*");
      return buildService.QueryBuildDefinitions(requestContext, new BuildDefinitionSpec()
      {
        FullPath = str
      }).Definitions.ToArray();
    }

    private void UpgradeBuildDefinition(
      IVssRequestContext requestContext,
      TeamFoundationBuildService buildService,
      BuildDefinition definition,
      Dictionary<int, int> planTcmIdWitIdMap,
      Dictionary<int, int> suiteTcmIdWitIdMap)
    {
      this.m_logger.Log(TraceLevel.Info, string.Format("Trying to upgrade buildDefinition {0}. Process Parameters {1}", (object) definition.Name, (object) definition.ProcessParameters));
      string processParameters = definition.ProcessParameters;
      if (string.IsNullOrWhiteSpace(processParameters))
      {
        this.m_logger.Log(TraceLevel.Info, string.Format("Ignoring build definition upgrade as process parameters are empty"));
      }
      else
      {
        XmlDocument xmlDocument = XmlUtility.LoadXmlDocumentFromString(processParameters);
        XmlAttribute attribute = xmlDocument.DocumentElement.Attributes["xmlns:mtlwa"];
        if (attribute == null)
        {
          this.m_logger.Log(TraceLevel.Info, string.Format("Ignoring build definition upgrade as LabWorkFlowNameSpaceAttribute does not exist"));
        }
        else
        {
          XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
          nsmgr.AddNamespace(attribute.LocalName, attribute.Value);
          XmlNode runTestDetailsNode = xmlDocument.SelectSingleNode("//mtlwa:LabWorkflowDetails/mtlwa:LabWorkflowDetails.TestParameters/mtlwa:RunTestDetails", nsmgr);
          if (runTestDetailsNode == null)
            this.m_logger.Log(TraceLevel.Info, string.Format("Ignoring build definition upgrade as runTestDetailsNode does not exist"));
          else if (!this.ReplaceTestPlanId(planTcmIdWitIdMap, runTestDetailsNode, definition.FullPath))
          {
            this.m_logger.Log(TraceLevel.Info, string.Format("Ignoring build definition upgrade due to failure in replacing test plan id"));
          }
          else
          {
            XmlNode xmlNode = runTestDetailsNode.SelectSingleNode("mtlwa:RunTestDetails.TestSuiteIdList", nsmgr);
            if (xmlNode == null || xmlNode.FirstChild == null)
              this.m_logger.Log(TraceLevel.Info, "Ignoring build definition upgrade as testSuiteIdsNode or its child node does not exist");
            else if (!this.ReplaceTestSuiteIds(suiteTcmIdWitIdMap, xmlNode.FirstChild, definition.FullPath))
            {
              this.m_logger.Log(TraceLevel.Info, string.Format("Ignoring build definition upgrade due to failure in replacing test suite id"));
            }
            else
            {
              definition.ProcessParameters = xmlDocument.OuterXml;
              this.m_logger.Log(TraceLevel.Info, string.Format("Updating buildDefinition process parameters {0}", (object) definition.ProcessParameters));
              buildService.UpdateBuildDefinitions(requestContext, (IList<BuildDefinition>) new BuildDefinition[1]
              {
                definition
              });
              this.m_logger.Log(TraceLevel.Info, string.Format("Finished upgrading buildDefinition {0}", (object) definition.Name));
            }
          }
        }
      }
    }

    private bool ReplaceTestSuiteIds(
      Dictionary<int, int> suiteTcmIdWitIdMap,
      XmlNode testSuiteIdListNode,
      string definitionName)
    {
      if (testSuiteIdListNode.ChildNodes == null)
      {
        this.m_logger.Log(TraceLevel.Info, "testSuiteIdListNode does not have child nodes");
        return false;
      }
      foreach (XmlNode childNode in testSuiteIdListNode.ChildNodes)
      {
        string innerText = childNode.InnerText;
        int witId;
        if (!this.GetWitId(suiteTcmIdWitIdMap, innerText, out witId))
        {
          this.m_logger.Log(TraceLevel.Info, "Test suite id is not valid");
          return false;
        }
        if (witId > 0)
          this.m_logger.Log(TraceLevel.Info, string.Format("Replacing SuiteId {0} with {1}", (object) innerText, (object) witId));
        else
          this.m_logger.Log(TraceLevel.Warning, string.Format("TestSuite {0} asociated with buildDefinition {1} does not exist. Making testSuiteId zero", (object) innerText, (object) definitionName));
        childNode.InnerText = witId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      return true;
    }

    private bool ReplaceTestPlanId(
      Dictionary<int, int> planTcmIdWitIdMap,
      XmlNode runTestDetailsNode,
      string definitionName)
    {
      XmlAttribute attribute = runTestDetailsNode.Attributes["TestPlanId"];
      if (attribute == null)
      {
        this.m_logger.Log(TraceLevel.Info, "testPlanAttrribute does not exist");
        return false;
      }
      string tcmIdString = attribute.Value;
      int witId;
      if (!this.GetWitId(planTcmIdWitIdMap, tcmIdString, out witId))
      {
        this.m_logger.Log(TraceLevel.Info, "Test plan id is not valid");
        return false;
      }
      if (witId > 0)
        this.m_logger.Log(TraceLevel.Info, string.Format("Replacing TestPlanId {0} with {1}", (object) tcmIdString, (object) witId));
      else
        this.m_logger.Log(TraceLevel.Warning, string.Format("TestPlan {0} asociated with buildDefinition {1} does not exist. Making testplanId zero", (object) tcmIdString, (object) definitionName));
      runTestDetailsNode.Attributes["TestPlanId"].Value = witId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return true;
    }

    private bool GetWitId(Dictionary<int, int> tcmIdWitIdMap, string tcmIdString, out int witId)
    {
      int result = 0;
      witId = 0;
      if (!int.TryParse(tcmIdString, out result))
      {
        this.m_logger.Log(TraceLevel.Info, string.Format("Unable to parse {0} to integer", (object) tcmIdString));
        return false;
      }
      if (!tcmIdWitIdMap.TryGetValue(result, out witId))
        this.m_logger.Log(TraceLevel.Info, string.Format("Unable to find {0} in tcmIdWitIdMap ", (object) result));
      return true;
    }
  }
}
