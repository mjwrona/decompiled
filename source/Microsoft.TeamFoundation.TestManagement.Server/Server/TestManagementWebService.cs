// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementWebService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.Server.TcmService;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/07/TCM/TestManagement/01", Description = "Test Management Service", Name = "TestManagementWebService")]
  [ClientService(ComponentName = "TestManagement", RegistrationName = "TestManagement", ServiceName = "TestManagementWebService", CollectionServiceIdentifier = "11c7db2b-ca61-444d-8bef-334c03af68c4")]
  public class TestManagementWebService : BaseTestManagementWebService
  {
    private AttachmentsHelper m_attachmentsHelper;
    private AfnStripsHelper m_afnStripsHelper;
    private TfsTestManagementRequestContext m_tmRequestContext;
    private LegacyResultsHelper m_legacyResultsHelper;
    private Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper m_resultsHelper;
    private const string xtSuffixString = "_XTWeb";

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResultAttachment))]
    public List<TestResultAttachment> QueryDefaultStrips(int[] testCaseIds, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryDefaultStrips), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (testCaseIds), (IList<int>) testCaseIds);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return this.AfnStripsHelper.GetDefaultAfnStrips(projectName, (IList<int>) testCaseIds).Select<AfnStrip, TestResultAttachment>((Func<AfnStrip, TestResultAttachment>) (afnStrip => AfnStripContractConverter.Convert(afnStrip))).ToList<TestResultAttachment>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (bool))]
    public List<bool> CheckActionRecordingExists(int[] testCaseIds, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckActionRecordingExists), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (testCaseIds), (IList<int>) testCaseIds);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return this.AttachmentsHelper.CheckIfActionRecordingExists((TestManagementRequestContext) this.m_tmRequestContext, projectName, (IList<int>) ((IEnumerable<int>) testCaseIds).ToArray<int>());
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResultAttachment))]
    public List<TestResultAttachment> QueryAttachments(ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryAttachments), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return this.AttachmentsHelper.GetTestAttachmentsByQuery((TestManagementRequestContext) this.m_tmRequestContext, query.TeamProjectName, ResultsStoreQueryContractConverter.Convert(query)).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<TestResultAttachment>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResultAttachment))]
    public List<TestResultAttachment> QueryAttachmentsById(
      string projectName,
      int attachmentId,
      bool getSiblingAttachments)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryAttachmentsById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (attachmentId), (object) attachmentId);
        methodInformation.AddParameter(nameof (getSiblingAttachments), (object) getSiblingAttachments);
        this.EnterMethod(methodInformation);
        return this.AttachmentsHelper.GetTestAttachments((TestManagementRequestContext) this.m_tmRequestContext, projectName, attachmentId, getSiblingAttachments).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<TestResultAttachment>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResultAttachment))]
    public List<TestResultAttachment> QueryTestResultAttachments(
      TestCaseResultIdentifier identifier,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultAttachments), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (identifier), (object) identifier);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestCaseResultIdentifier>(identifier, nameof (identifier), "Test Results");
        return this.AttachmentsHelper.GetTestAttachments((TestManagementRequestContext) this.m_tmRequestContext, projectName, identifier.TestRunId, identifier.TestResultId, 0, 0, 0).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<TestResultAttachment>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResultAttachment))]
    public List<TestResultAttachment> QueryTestRunAttachments(int testRunId, string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunAttachments), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return this.AttachmentsHelper.GetTestAttachments((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId, 0, 0, 0, 0).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<TestResultAttachment>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResultAttachment))]
    public List<TestResultAttachment> QuerySessionAttachments(int sessionId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySessionAttachments), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (sessionId), (object) sessionId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return TestResultAttachment.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, 0, sessionId, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdateDefaultStrips(
      DefaultAfnStripBinding[] afnStripBindingList,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateDefaultStrips), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<DefaultAfnStripBinding>(nameof (afnStripBindingList), (IList<DefaultAfnStripBinding>) afnStripBindingList);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        this.AfnStripsHelper.UpdateDefaultStrip((TestManagementRequestContext) this.m_tmRequestContext, projectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>) ((IEnumerable<DefaultAfnStripBinding>) afnStripBindingList).Select<DefaultAfnStripBinding, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>((Func<DefaultAfnStripBinding, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>) (binding => AfnStripContractConverter.Convert(binding))).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>());
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    public AttachmentsHelper AttachmentsHelper
    {
      get
      {
        if (this.m_attachmentsHelper == null)
          this.m_attachmentsHelper = new AttachmentsHelper((TestManagementRequestContext) this.m_tmRequestContext);
        return this.m_attachmentsHelper;
      }
    }

    public AfnStripsHelper AfnStripsHelper
    {
      get
      {
        if (this.m_afnStripsHelper == null)
          this.m_afnStripsHelper = new AfnStripsHelper(this.m_tmRequestContext);
        return this.m_afnStripsHelper;
      }
    }

    [WebMethod]
    public UpdatedProperties CreateBugFieldMapping(
      BugFieldMapping bugFieldMapping,
      string teamProjectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateBugFieldMapping), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (bugFieldMapping), (object) bugFieldMapping);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<BugFieldMapping>(bugFieldMapping, nameof (bugFieldMapping), this.m_tmRequestContext.RequestContext.ServiceName);
        return bugFieldMapping.Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BugFieldMapping QueryBugFieldMapping(string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBugFieldMapping), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return BugFieldMapping.Query((TestManagementRequestContext) this.m_tmRequestContext, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties UpdateBugFieldMapping(
      BugFieldMapping bugFieldMapping,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBugFieldMapping), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (bugFieldMapping), (object) bugFieldMapping);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<BugFieldMapping>(bugFieldMapping, nameof (bugFieldMapping), this.m_tmRequestContext.RequestContext.ServiceName);
        return bugFieldMapping.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteBuild(string projectName, string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        Build.Delete((TestManagementRequestContext) this.m_tmRequestContext, projectName, new string[1]
        {
          buildUri
        });
        if (this.m_tmRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        SoapBuildDeleteEvent payload = new SoapBuildDeleteEvent(Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, buildUri);
        this.m_tmRequestContext.TestManagementHost.PublishToServiceBus((TestManagementRequestContext) this.m_tmRequestContext, "Microsoft.TestManagement.PlannedTestMetaData.Server", PlannedTestMetaDataEventType.SoapBuildDeleted, (object) payload);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestConfiguration CreateTestConfiguration(
      TestConfiguration testConfiguration,
      string teamProjectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("CreateTestConfiguratation", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testConfiguration), (object) testConfiguration);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestConfiguration>(testConfiguration, nameof (testConfiguration), this.m_tmRequestContext.RequestContext.ServiceName);
        return testConfiguration.Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteTestConfiguration(int testConfigurationId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestConfiguration), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testConfigurationId), (object) testConfigurationId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        TestConfiguration.Delete((TestManagementRequestContext) this.m_tmRequestContext, testConfigurationId, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestConfiguration QueryTestConfigurationById(int testConfigurationId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestConfigurationById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testConfigurationId), (object) testConfigurationId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return TestConfiguration.QueryById((TestManagementRequestContext) this.m_tmRequestContext, testConfigurationId, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestConfiguration))]
    public List<TestConfiguration> QueryTestConfigurations(ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestConfigurations), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return TestConfiguration.Query((TestManagementRequestContext) this.m_tmRequestContext, query, 0);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int QueryTestConfigurationsCount(ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestConfigurationsCount), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return TestConfiguration.QueryCount((TestManagementRequestContext) this.m_tmRequestContext, query);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties UpdateTestConfiguration(
      TestConfiguration testConfiguration,
      string projectName,
      bool updateInUse,
      bool unchangedValues)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestConfiguration), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testConfiguration), (object) testConfiguration);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (updateInUse), (object) updateInUse);
        methodInformation.AddParameter(nameof (unchangedValues), (object) unchangedValues);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestConfiguration>(testConfiguration, nameof (testConfiguration), this.m_tmRequestContext.RequestContext.ServiceName);
        return testConfiguration.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, updateInUse, unchangedValues);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int CreateTestVariable(TestVariable variable, string teamProjectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestVariable), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (variable), (object) variable);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestVariable>(variable, nameof (variable), this.m_tmRequestContext.RequestContext.ServiceName);
        return variable.Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int UpdateTestVariable(TestVariable variable, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestVariable), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (variable), (object) variable);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestVariable>(variable, nameof (variable), this.m_tmRequestContext.RequestContext.ServiceName);
        return variable.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteTestVariable(int variableId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestVariable), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (variableId), (object) variableId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        TestVariable.Delete((TestManagementRequestContext) this.m_tmRequestContext, variableId, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestVariable QueryTestVariableById(int variableId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestVariableById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (variableId), (object) variableId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return TestVariable.QueryById((TestManagementRequestContext) this.m_tmRequestContext, variableId, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestVariable))]
    public List<TestVariable> QueryTestVariables(string teamProjectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestVariables), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        return TestVariable.QueryTestVariables((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties CreateTestSettings(TestSettings settings, string teamProjectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestSettings), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (settings), (object) settings);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestSettings>(settings, nameof (settings), this.m_tmRequestContext.RequestContext.ServiceName);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
        return this.m_tmRequestContext.LegacyTcmServiceHelper.TryCreateTestSettingsCompat(this.m_tmRequestContext.RequestContext, projectFromName.GuidId, TestSettingsContractConverter.Convert(settings), out newUpdateProperties) ? UpdatedPropertiesConverter.Convert(newUpdateProperties) : new UpdatedProperties();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties UpdateTestSettings(TestSettings settings, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestSettings), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (settings), (object) settings);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestSettings>(settings, nameof (settings), this.m_tmRequestContext.RequestContext.ServiceName);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
        return this.m_tmRequestContext.LegacyTcmServiceHelper.TryUpdateTestSettings(this.m_tmRequestContext.RequestContext, projectFromName.GuidId, TestSettingsContractConverter.Convert(settings), out newUpdateProperties) ? UpdatedPropertiesConverter.Convert(newUpdateProperties) : new UpdatedProperties();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteTestSettings(int settingsId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestSettings), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (settingsId), (object) settingsId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        this.m_tmRequestContext.TcmServiceHelper.TryDeleteTestSettings(this.m_tmRequestContext.RequestContext, Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, settingsId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestSettings QueryTestSettingsById(int settingsId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestSettingsById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (settingsId), (object) settingsId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        LegacyTestSettings newTestSettings;
        if (!this.m_tmRequestContext.LegacyTcmServiceHelper.TryGetTestSettingsCompatById(this.m_tmRequestContext.RequestContext, Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, settingsId, out newTestSettings) || newTestSettings == null)
          return TestSettings.QueryById((TestManagementRequestContext) this.m_tmRequestContext, settingsId, projectName);
        TestSettings testSettings = TestSettingsContractConverter.Convert(newTestSettings);
        testSettings.AreaPath = projectName;
        return testSettings;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestSettings))]
    public List<TestSettings> QueryTestSettings(ResultsStoreQuery query, bool omitSettings)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestSettings), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (omitSettings), (object) omitSettings);
        this.EnterMethod(methodInformation);
        List<LegacyTestSettings> newTestSettings;
        if (!this.m_tmRequestContext.LegacyTcmServiceHelper.TryQueryTestSettings(this.m_tmRequestContext.RequestContext, ResultsStoreQueryContractConverter.Convert(query), omitSettings, out newTestSettings) || newTestSettings == null)
          return TestSettings.Query((TestManagementRequestContext) this.m_tmRequestContext, query, omitSettings);
        List<TestSettings> list = newTestSettings.Select<LegacyTestSettings, TestSettings>((Func<LegacyTestSettings, TestSettings>) (testSetting => TestSettingsContractConverter.Convert(testSetting))).ToList<TestSettings>();
        foreach (TestSettings testSettings in list)
          testSettings.AreaPath = query.TeamProjectName;
        return list;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int QueryTestSettingsCount(ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestSettingsCount), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        int? testSettingsCount;
        return this.m_tmRequestContext.LegacyTcmServiceHelper.TryQueryTestSettingsCount(this.m_tmRequestContext.RequestContext, ResultsStoreQueryContractConverter.Convert(query), out testSettingsCount) ? testSettingsCount.GetValueOrDefault() : 0;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    public int CreateTestResolutionState(
      TestResolutionState resolutionState,
      string teamProjectName)
    {
      throw this.HandleException(new Exception(ServerResources.TestResolutionStateDeprecationMessage));
    }

    [WebMethod]
    public void UpdateTestResolutionState(TestResolutionState state, string projectName) => throw this.HandleException(new Exception(ServerResources.TestResolutionStateDeprecationMessage));

    [WebMethod]
    public void DeleteTestResolutionState(int stateId, string projectName) => throw this.HandleException(new Exception(ServerResources.TestResolutionStateDeprecationMessage));

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResolutionState))]
    public List<TestResolutionState> QueryTestResolutionStateById(
      int testResolutionStateId,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResolutionStateById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testResolutionStateId), (object) testResolutionStateId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return TestResolutionState.Query((TestManagementRequestContext) this.m_tmRequestContext, testResolutionStateId, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResolutionState))]
    public List<TestResolutionState> QueryTestResolutionStates(string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResolutionStates), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        this.EnterMethod(methodInformation);
        return TestResolutionState.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, teamProject);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestConfiguration))]
    public List<TestConfiguration> QueryTestConfigurationsForPlan(
      ResultsStoreQuery query,
      int planId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryTestConfigurations", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        this.EnterMethod(methodInformation);
        return TestConfiguration.Query((TestManagementRequestContext) this.m_tmRequestContext, query, planId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (DataCollectorInformation))]
    public List<DataCollectorInformation> RegisterCollectors(
      List<DataCollectorInformation> collectors,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RegisterCollectors), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<DataCollectorInformation>(nameof (collectors), (IList<DataCollectorInformation>) collectors);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        DataCollectorInformation.Register((TestManagementRequestContext) this.m_tmRequestContext, collectors);
        return collectors;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UnregisterCollectors(
      List<DataCollectorInformation> collectors,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UnregisterCollectors), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<DataCollectorInformation>(nameof (collectors), (IList<DataCollectorInformation>) collectors);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        DataCollectorInformation.Unregister((TestManagementRequestContext) this.m_tmRequestContext, collectors);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdateCollectors(
      List<DataCollectorInformation> collectors,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateCollectors), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<DataCollectorInformation>(nameof (collectors), (IList<DataCollectorInformation>) collectors);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        DataCollectorInformation.Update((TestManagementRequestContext) this.m_tmRequestContext, collectors);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (DataCollectorInformation))]
    public List<DataCollectorInformation> QueryCollectors(
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryCollectors), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        return DataCollectorInformation.Query(this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public DataCollectorInformation FindCollector(
      Guid teamProjectCollectionCatalogResourceId,
      string typeUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FindCollector), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        methodInformation.AddParameter(nameof (typeUri), (object) typeUri);
        this.EnterMethod(methodInformation);
        return DataCollectorInformation.Find((TestManagementRequestContext) this.m_tmRequestContext, typeUri);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void RegisterControllers(
      List<TestController> controllers,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        this.CheckIfTestManagementOperationSupported();
        MethodInformation methodInformation = new MethodInformation(nameof (RegisterControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestController>(nameof (controllers), (IList<TestController>) controllers);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        TestController.Register((TestManagementRequestContext) this.m_tmRequestContext, controllers);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UnregisterControllers(
      List<TestController> controllers,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UnregisterControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestController>(nameof (controllers), (IList<TestController>) controllers);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        TestController.Unregister((TestManagementRequestContext) this.m_tmRequestContext, controllers);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdateControllers(
      List<TestController> controllers,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        this.CheckIfTestManagementOperationSupported();
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestController>(nameof (controllers), (IList<TestController>) controllers);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        TestController.Update((TestManagementRequestContext) this.m_tmRequestContext, controllers);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestController))]
    public List<TestController> QueryControllers(Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        return TestController.Query(this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestController))]
    public List<TestController> QueryControllersByGroupId(
      Guid teamProjectCollectionCatalogResourceId,
      string groupId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryControllers", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        methodInformation.AddParameter(nameof (groupId), (object) groupId);
        this.EnterMethod(methodInformation);
        return TestController.Query((TestManagementRequestContext) this.m_tmRequestContext, groupId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestController FindController(
      Guid teamProjectCollectionCatalogResourceId,
      string controllerName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FindController), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        methodInformation.AddParameter(nameof (controllerName), (object) controllerName);
        this.EnterMethod(methodInformation);
        return TestController.Find((TestManagementRequestContext) this.m_tmRequestContext, controllerName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestEnvironment))]
    public List<TestEnvironment> RegisterEnvironments(
      List<TestEnvironment> environments,
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RegisterEnvironments), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestEnvironment>(nameof (environments), (IList<TestEnvironment>) environments);
        methodInformation.AddParameter(nameof (parentName), (object) parentName);
        methodInformation.AddParameter(nameof (parentType), (object) parentType);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        throw new TestManagementAPINotSupportedException("TestManagementWebService.RegisterEnvironments");
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UnregisterEnvironments(
      List<TestEnvironment> environments,
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("UnregisterControllers", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestEnvironment>(nameof (environments), (IList<TestEnvironment>) environments);
        methodInformation.AddParameter(nameof (parentName), (object) parentName);
        methodInformation.AddParameter(nameof (parentType), (object) parentType);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        throw new TestManagementAPINotSupportedException("TestManagementWebService.UnregisterEnvironments");
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<TestEnvironment> UpdateEnvironments(
      List<TestEnvironment> environments,
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateEnvironments), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestEnvironment>(nameof (environments), (IList<TestEnvironment>) environments);
        methodInformation.AddParameter(nameof (parentName), (object) parentName);
        methodInformation.AddParameter(nameof (parentType), (object) parentType);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        throw new TestManagementAPINotSupportedException("TestManagementWebService.UpdateEnvironments");
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestEnvironment))]
    public List<TestEnvironment> QueryEnvironments(
      string parentName,
      EnvironmentParentTypes parentType,
      Guid teamProjectCollectionCatalogResourceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryEnvironments), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parentName), (object) parentName);
        methodInformation.AddParameter(nameof (parentType), (object) parentType);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        return new List<TestEnvironment>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestEnvironment FindEnvironment(
      string parentName,
      EnvironmentParentTypes parentType,
      Guid environmentId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FindEnvironment), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parentName), (object) parentName);
        methodInformation.AddParameter(nameof (parentType), (object) parentType);
        methodInformation.AddParameter(nameof (environmentId), (object) environmentId);
        this.EnterMethod(methodInformation);
        return (TestEnvironment) null;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private void CheckIfTestManagementOperationSupported()
    {
      if (this.m_tmRequestContext.RequestContext.IsFeatureEnabled("LabManagement.Server.BlockLabManagement"))
        throw new NotSupportedException(ServerResources.TestControllerNotSupported);
    }

    [WebMethod]
    [return: XmlArray]
    public List<BuildCoverage> QueryBuildCoverage(string projectName, string buildUri, int flags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildCoverage), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (flags), (object) flags);
        this.EnterMethod(methodInformation);
        if (this.isOnPremiseDeployment())
          return BuildCoverage.Query((TestManagementRequestContext) this.m_tmRequestContext, projectName, buildUri, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverage1;
        return this.m_tmRequestContext.TcmServiceHelper.TryGetBuildCodeCoverage(this.m_tmRequestContext.RequestContext, Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, this.GetBuildIdFromUri(buildUri), flags, out buildCoverage1) ? buildCoverage1.Select<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage, BuildCoverage>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage, BuildCoverage>) (buildCoverage => this.ConvertBuildCoverageToServerOM(buildCoverage))).ToList<BuildCoverage>() : new List<BuildCoverage>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    public List<TestRunCoverage> QueryTestRunCoverage(string projectName, int testRunId, int flags)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunCoverage), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (flags), (object) flags);
        this.EnterMethod(methodInformation);
        if (this.isOnPremiseDeployment())
          return TestRunCoverage.Query((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> runCoverage;
        return this.m_tmRequestContext.TcmServiceHelper.TryGetTestRunCodeCoverage(this.m_tmRequestContext.RequestContext, Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, testRunId, flags, out runCoverage) ? runCoverage.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage, TestRunCoverage>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage, TestRunCoverage>) (testRunCoverage => this.ConvertTestRunCoverageToServerOM(testRunCoverage))).ToList<TestRunCoverage>() : new List<TestRunCoverage>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private bool isOnPremiseDeployment() => this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    private int GetBuildIdFromUri(string buildUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(buildUri);
      int result = 0;
      int.TryParse(artifactId.ToolSpecificId, out result);
      return result;
    }

    private TestRunCoverage ConvertTestRunCoverageToServerOM(Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage testRunCoverage)
    {
      if (testRunCoverage == null)
        return (TestRunCoverage) null;
      TestRunCoverage serverOm = new TestRunCoverage();
      serverOm.TestRunId = Convert.ToInt32(testRunCoverage.TestRun.Id);
      serverOm.LastError = testRunCoverage.LastError;
      CoverageState result;
      if (Enum.TryParse<CoverageState>(testRunCoverage.State, out result))
        serverOm.State = (byte) result;
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage module in testRunCoverage.Modules)
        serverOm.Modules.Add(this.ConvertModuleCoverageToServerOM(module));
      return serverOm;
    }

    private BuildCoverage ConvertBuildCoverageToServerOM(Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage buildCoverage)
    {
      if (buildCoverage == null)
        return (BuildCoverage) null;
      BuildCoverage serverOm = new BuildCoverage();
      CoverageState result;
      if (Enum.TryParse<CoverageState>(buildCoverage.State, out result))
        serverOm.State = (byte) result;
      serverOm.LastError = buildCoverage.LastError;
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage module in buildCoverage.Modules)
        serverOm.Modules.Add(this.ConvertModuleCoverageToServerOM(module));
      serverOm.Configuration = this.ConvertBuildConfigurationToDataContract(buildCoverage.Configuration);
      return serverOm;
    }

    private ModuleCoverage ConvertModuleCoverageToServerOM(Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage moduleCoverage)
    {
      if (moduleCoverage == null)
        return (ModuleCoverage) null;
      ModuleCoverage serverOm = new ModuleCoverage();
      serverOm.BlockCount = moduleCoverage.BlockCount;
      serverOm.BlockData = moduleCoverage.BlockData;
      serverOm.Name = moduleCoverage.Name;
      serverOm.Signature = moduleCoverage.Signature;
      serverOm.SignatureAge = moduleCoverage.SignatureAge;
      serverOm.CoverageFileUrl = moduleCoverage.FileUrl;
      serverOm.Statistics = this.ConvertStatisticsToDataContract(moduleCoverage.Statistics);
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage function in moduleCoverage.Functions)
        serverOm.Functions.Add(this.ConvertFunctionToDataContract(function));
      return serverOm;
    }

    private CoverageStatistics ConvertStatisticsToDataContract(Microsoft.TeamFoundation.TestManagement.WebApi.CoverageStatistics statistics) => new CoverageStatistics()
    {
      BlocksCovered = statistics.BlocksCovered,
      BlocksNotCovered = statistics.BlocksNotCovered,
      LinesCovered = statistics.LinesCovered,
      LinesNotCovered = statistics.LinesNotCovered,
      LinesPartiallyCovered = statistics.LinesPartiallyCovered
    };

    private FunctionCoverage ConvertFunctionToDataContract(Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage function) => new FunctionCoverage()
    {
      Class = function.Class,
      Name = function.Name,
      Namespace = function.Namespace,
      SourceFile = function.SourceFile,
      Statistics = this.ConvertStatisticsToDataContract(function.Statistics)
    };

    private BuildConfiguration ConvertBuildConfigurationToDataContract(
      Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration buildConfiguration)
    {
      return new BuildConfiguration()
      {
        BuildConfigurationId = buildConfiguration.Id,
        BuildUri = buildConfiguration.Uri,
        BuildFlavor = buildConfiguration.Flavor,
        BuildPlatform = buildConfiguration.Platform,
        TeamProjectName = buildConfiguration.Project.Name
      };
    }

    public TestManagementWebService()
    {
      this.RequestContext.ServiceName = "Test Management";
      this.m_tmRequestContext = new TfsTestManagementRequestContext(this.RequestContext);
    }

    [WebMethod]
    [return: XmlArray]
    public List<ImpactedPoint> QueryImpactedPointsForPlan(
      string teamProjectName,
      int planId,
      string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryImpactedPointsForPlan), MethodType.Tool, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        return ImpactedPoint.QueryImpactedPointsForPlan((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, planId, buildUri);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BlockedPointProperties[] BlockTestPoints(TestPoint[] points, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (BlockTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestPoint>(nameof (points), (IList<TestPoint>) points);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return TestPoint.Block2(this.m_tmRequestContext, points, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestPlan CreateTestPlan(
      TestPlan testPlan,
      string teamProjectName,
      TestExternalLink[] links)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestPlan), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testPlan), (object) testPlan);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddArrayParameter<TestExternalLink>(nameof (links), (IList<TestExternalLink>) links);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestPlan>(testPlan, nameof (testPlan), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        testPlan.Description = Compat2013UpdateHelper.ConvertTestPlanDescriptionToHtml(testPlan.Description);
        return testPlan.Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, links, TestPlanSource.Mtm);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteTestPlan(int testPlanId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestPlan), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter("TestPlanId", (object) testPlanId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        throw new TestManagementInvalidOperationException(ServerResources.DeleteTestPlanNotSupported);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPlan))]
    public List<TestPlan> FetchTestPlans(
      IdAndRev[] idsToFetch,
      out List<int> deletedIds,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestPlans), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<IdAndRev>(nameof (idsToFetch), (IList<IdAndRev>) idsToFetch);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        deletedIds = new List<int>();
        return Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, projectName, TestPlan.Fetch((TestManagementRequestContext) this.m_tmRequestContext, idsToFetch, deletedIds, projectName));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> FetchTestPoints(
      string projectName,
      int planId,
      IdAndRev[] idsToFetch,
      string[] testCaseProperties,
      out List<int> deletedIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddArrayParameter<IdAndRev>(nameof (idsToFetch), (IList<IdAndRev>) idsToFetch);
        this.EnterMethod(methodInformation);
        deletedIds = new List<int>();
        List<TestPoint> testPoints = TestPoint.Fetch((TestManagementRequestContext) this.m_tmRequestContext, projectName, planId, idsToFetch, testCaseProperties, deletedIds);
        return Compat2011QU1Helper.Convert(TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, projectName, planId, testPoints));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (SuitePointCount))]
    public List<SuitePointCount> QuerySuitePointCounts(int planId, ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySuitePointCounts), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return TestPlan.QuerySuitePointCounts((TestManagementRequestContext) this.m_tmRequestContext, planId, query);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (int))]
    public List<int> QueryTestCases(string queryText, bool inPlans, string teamProjectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestCases), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (queryText), (object) queryText);
        methodInformation.AddParameter(nameof (inPlans), (object) inPlans);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        return TestPlan.QueryTestCases((TestManagementRequestContext) this.m_tmRequestContext, queryText, inPlans, teamProjectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (SkinnyPlan))]
    public List<SkinnyPlan> QueryTestPlans(ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPlans), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, query.TeamProjectName);
        return TestPlan.Query((TestManagementRequestContext) this.m_tmRequestContext, query, false);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestExternalLink))]
    public List<TestExternalLink> QueryTestPlanLinks(string projectName, int planId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPlanLinks), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        return TestExternalLink.QueryTestPlanLinks(this.m_tmRequestContext, projectName, planId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> QueryTestPoints(
      int planId,
      ResultsStoreQuery query,
      int pageSize,
      string[] testCaseProperties)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddArrayParameter<string>(nameof (testCaseProperties), (IList<string>) testCaseProperties);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, pageSize, query, out List<TestPointStatistic> _, false, testCaseProperties);
        return Compat2011QU1Helper.Convert(TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPointStatistic))]
    public List<TestPointStatistic> QueryTestPointStatistics(int planId, ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointStatistics), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, int.MaxValue, query, out List<TestPointStatistic> _, false, (string[]) null);
        return Compat2011QU1Helper.Convert(TestPointUpdate.GetTestPointStatistics(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPointStatisticsByPivotType))]
    public List<TestPointStatisticsByPivotType> QueryTestPointStatisticsByPivots(
      int planId,
      ResultsStoreQuery query,
      List<TestPointStatisticsQueryPivotType> pivotList)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointStatisticsByPivots), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddArrayParameter<TestPointStatisticsQueryPivotType>(nameof (pivotList), (IList<TestPointStatisticsQueryPivotType>) pivotList);
        List<TestPoint> testPoints1 = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, int.MaxValue, query, out List<TestPointStatistic> _, false, (string[]) null);
        List<TestPoint> testPoints2 = TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints1);
        return Compat2011QU1Helper.Convert(TestPointUpdate.GetTestPointStatisticsByPivotType(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints2, pivotList));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPointStatisticsByPivotType))]
    public List<TestPointStatisticsByPivotType> QueryTestPointStatisticsByPivots2(
      int planId,
      ResultsStoreQuery query,
      List<TestPointStatisticsQueryPivotType> pivotList)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointStatisticsByPivots2), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddArrayParameter<TestPointStatisticsQueryPivotType>(nameof (pivotList), (IList<TestPointStatisticsQueryPivotType>) pivotList);
        List<TestPoint> testPoints1 = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, int.MaxValue, query, out List<TestPointStatistic> _, false, (string[]) null);
        List<TestPoint> testPoints2 = TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints1);
        return Compat2011QU1Helper.Convert(TestPointUpdate.GetTestPointStatisticsByPivotType(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints2, pivotList, true));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> QueryTestPointHistory(int testPointId, int planId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointHistory), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testPointId), (object) testPointId);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints = TestPoint.QueryTestPointHistory((TestManagementRequestContext) this.m_tmRequestContext, testPointId, planId, projectName);
        return Compat2011QU1Helper.Convert(TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, projectName, planId, testPoints));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestPlan UpdateTestPlan(
      TestPlan testPlan,
      string projectName,
      TestExternalLink[] changedLinks)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestPlan), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testPlan), (object) testPlan);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddArrayParameter<TestExternalLink>(nameof (changedLinks), (IList<TestExternalLink>) changedLinks);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestPlan>(testPlan, nameof (testPlan), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        testPlan.Status = Compat2013UpdateHelper.ConvertTcmStateToWorkItemState((TestManagementRequestContext) this.m_tmRequestContext, projectName, testPlan.PlanId, testPlan.State, WitCategoryRefName.TestPlan);
        return Compat2011QU1Helper.Convert((TestManagementRequestContext) this.m_tmRequestContext, testPlan).Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, changedLinks);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties[] UpdateTestPoints(TestPoint[] points, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestPoint>(nameof (points), (IList<TestPoint>) points);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        PlannedResultsTCMServiceHelper tcmServiceHelper = this.m_tmRequestContext.PlannedTestingTCMServiceHelper;
        foreach (IGrouping<int, TestPoint> source in ((IEnumerable<TestPoint>) points).GroupBy<TestPoint, int>((Func<TestPoint, int>) (point => point.PlanId)))
          tcmServiceHelper.UnblockTestPointResultsIfAny((TestManagementRequestContext) this.m_tmRequestContext, source.Select<TestPoint, TestPoint>((Func<TestPoint, TestPoint>) (point => point)).ToList<TestPoint>(), projectFromName.GuidId, source.Key);
        return TestPoint.Update((TestManagementRequestContext) this.m_tmRequestContext, points, projectName, updateResultsInTCM: true);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> QueryTestPointsAndStatistics(
      int planId,
      ResultsStoreQuery query,
      int pageSize,
      string[] testCaseProperties,
      [XmlArray, XmlArrayItem(typeof (TestPointStatistic))] out List<TestPointStatistic> stats)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointsAndStatistics), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddArrayParameter<string>(nameof (testCaseProperties), (IList<string>) testCaseProperties);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, pageSize, query, out stats, true, testCaseProperties);
        List<TestPoint> testPointList = TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints);
        stats = TestPointUpdate.GroupPointsByStatistics(testPointList);
        stats = Compat2011QU1Helper.Convert(stats);
        return Compat2011QU1Helper.Convert(testPointList);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int CreateTestPlanWithRequirements(
      int planId,
      string planName,
      string teamProjectName,
      string areaPath,
      string iteration,
      string description,
      DateTime startDate,
      DateTime endDate,
      Guid owner,
      List<int> requirementIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestPlanWithRequirements), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (planName), (object) planName);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (areaPath), (object) areaPath);
        methodInformation.AddParameter(nameof (iteration), (object) iteration);
        methodInformation.AddParameter(nameof (description), (object) description);
        methodInformation.AddParameter(nameof (startDate), (object) startDate);
        methodInformation.AddParameter(nameof (endDate), (object) endDate);
        methodInformation.AddParameter(nameof (owner), (object) owner);
        methodInformation.AddArrayParameter<int>(nameof (requirementIds), (IList<int>) requirementIds);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        description = Compat2013UpdateHelper.ConvertTestPlanDescriptionToHtml(description);
        return TestPlan.CreateWithRequirements((TestManagementRequestContext) this.m_tmRequestContext, planId, planName, teamProjectName, areaPath, iteration, description, startDate, endDate, owner, requirementIds, TestPlanSource.Mtm);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void CreateTestResults(TestCaseResult[] results, string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestCaseResult>(nameof (results), (IList<TestCaseResult>) results);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        if (results != null && results.Length != 0)
          this.m_tmRequestContext.WorkItemFieldDataHelper.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired((TestManagementRequestContext) this.m_tmRequestContext, Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName), results, false);
        IEnumerable<LegacyTestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) results);
        LegacyTestCaseResult[] array = source != null ? source.ToArray<LegacyTestCaseResult>() : (LegacyTestCaseResult[]) null;
        this.ResultsHelper.CreateTestResults((TestManagementRequestContext) this.m_tmRequestContext, projectName, array);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestRun CreateTestRun(
      TestRun testRun,
      TestSettings settings,
      TestCaseResult[] results,
      string teamProjectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        string str = string.Empty;
        if (testRun != null)
          str = testRun.TestPlanId != 0 ? (!testRun.IsAutomated ? (testRun.Type == (byte) 8 ? "_M1" : "_M") : "_A2") : "_A1";
        MethodInformation methodInformation = new MethodInformation("CreateTestRun2" + str, MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (testRun), (object) testRun);
        methodInformation.AddParameter(nameof (settings), (object) settings);
        methodInformation.AddArrayParameter<TestCaseResult>(nameof (results), (IList<TestCaseResult>) results);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestRun>(testRun, nameof (testRun), "Test Results");
        testRun.ThrowInvalidOperationIfRunHasDtlEnvironment();
        testRun.SourceWorkflow = Compat2011QU1Helper.GetSourceWorkflowForTestRun(testRun);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        if (results != null && results.Length != 0)
          this.m_tmRequestContext.WorkItemFieldDataHelper.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired((TestManagementRequestContext) this.m_tmRequestContext, projectFromName, results, false);
        LegacyTestSettings testSettings = TestSettingsContractConverter.Convert(settings);
        IEnumerable<LegacyTestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) results);
        LegacyTestCaseResult[] array = source != null ? source.ToArray<LegacyTestCaseResult>() : (LegacyTestCaseResult[]) null;
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun1 = TestRunContractConverter.Convert(testRun);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun2 = this.ResultsHelper.CreateTestRun((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, testRun1, array, testSettings);
        if (this.m_tmRequestContext.TestPointOutcomeHelper.IsDualWriteEnabled(this.m_tmRequestContext.RequestContext))
        {
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResultsByRunId = this.ResultsHelper.GetTestResultsByRunId((TestManagementRequestContext) this.m_tmRequestContext, projectFromName.GuidId, testRun2.TestRunId);
          this.m_tmRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.m_tmRequestContext.RequestContext, teamProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testResultsByRunId);
        }
        return Compat2011QU1Helper.Convert(TestRunContractConverter.Convert(testRun2), this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties AbortTestRun(
      int testRunId,
      int revision,
      string projectName,
      int options)
    {
      try
      {
        this.m_tmRequestContext.RequestContext.TraceEnter(1015090, "TestManagement", "WebService", "TestRuns.AbortTestRun");
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (AbortTestRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (revision), (object) revision);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        GuidAndString projectId = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        if (!this.m_tmRequestContext.TestPointOutcomeHelper.IsDualWriteEnabled(this.m_tmRequestContext.RequestContext))
          return UpdatedPropertiesConverter.Convert(this.ResultsHelper.AbortTestRun((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId, revision, options));
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResultsByRunId = this.ResultsHelper.GetTestResultsByRunId((TestManagementRequestContext) this.m_tmRequestContext, projectId.GuidId, testRunId);
        int testPlanId = 0;
        List<int> intList = new List<int>();
        if (testResultsByRunId != null && testResultsByRunId.Count > 0)
        {
          testPlanId = testResultsByRunId[0].TestPlan != null ? int.Parse(testResultsByRunId[0].TestPlan.Id) : 0;
          intList = testResultsByRunId.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>) (res => res.TestPoint == null ? 0 : int.Parse(res.TestPoint.Id))).ToList<int>();
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties = this.ResultsHelper.AbortTestRun((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId, revision, options);
        if (testPlanId > 0 && intList.Any<int>() && !this.m_tmRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          TestResultsQuery testResultsQuery = new TestResultsQuery()
          {
            ResultsFilter = new ResultsFilter()
            {
              AutomatedTestName = string.Empty,
              TestPlanId = testPlanId,
              TestPointIds = (IList<int>) intList
            }
          };
          TestResultsQuery results = (TestResultsQuery) null;
          if (!this.m_tmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
          {
            this.m_tmRequestContext.TcmServiceHelper.TryGetTestResultsByQuery(this.m_tmRequestContext.RequestContext, projectId.GuidId, testResultsQuery, out results);
          }
          else
          {
            TestResultsHttpClient testResultsHttpClient = this.m_tmRequestContext.RequestContext.GetClient<TestResultsHttpClient>();
            results = TestManagementController.InvokeAction<TestResultsQuery>((Func<TestResultsQuery>) (() => testResultsHttpClient.GetTestResultsByQueryAsync(testResultsQuery, projectId.GuidId, (object) null, new CancellationToken())?.Result));
          }
          if (results != null)
          {
            int? count = results.Results?.Count;
            int num = 0;
            if (count.GetValueOrDefault() > num & count.HasValue)
            {
              this.m_tmRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.m_tmRequestContext.RequestContext, projectName, results.Results);
              goto label_12;
            }
          }
          this.m_tmRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeWithoutResult(this.m_tmRequestContext.RequestContext, projectName, testPlanId, (IList<int>) intList);
        }
        else
          this.m_tmRequestContext.RequestContext.Trace(1015763, TraceLevel.Info, "TestManagement", "WebService", "PointId doesn't exist for runid = {0}, so didn't fetch result and updated it", (object) testRunId);
label_12:
        return UpdatedPropertiesConverter.Convert(updatedProperties);
      }
      catch (Exception ex)
      {
        this.m_tmRequestContext.RequestContext.Trace(1015763, TraceLevel.Warning, "TestManagement", "WebService", "Exception: {0}", (object) ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.m_tmRequestContext.RequestContext.TraceLeave(1015763, "TestManagement", "WebService", "TestRuns.AbortTestRun");
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteTestRun(int testRunId, string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        this.LegacyResultsHelper.DeleteTestRun(this.m_tmRequestContext, projectName, new int[1]
        {
          testRunId
        });
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteTestRuns(int[] testRunIds, string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestRuns), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (testRunIds), (IList<int>) testRunIds);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        this.LegacyResultsHelper.DeleteTestRun(this.m_tmRequestContext, projectName, testRunIds);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildConfiguration QueryBuildConfigurationById(int buildConfigurationId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildConfigurationById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildConfigurationId), (object) buildConfigurationId);
        this.EnterMethod(methodInformation);
        return new BuildConfiguration().Query(this.m_tmRequestContext.RequestContext, buildConfigurationId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestActionResult))]
    public List<TestActionResult> QueryTestActionResults(
      TestCaseResultIdentifier identifier,
      [XmlArray, XmlArrayItem(typeof (TestResultParameter))] out List<TestResultParameter> parameters,
      [XmlArray, XmlArrayItem(typeof (TestResultAttachment))] out List<TestResultAttachment> attachments,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestActionResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (identifier), (object) identifier);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        QueryTestActionResultResponse actionResultResponse = this.ResultsHelper.QueryTestActionResults((TestManagementRequestContext) this.m_tmRequestContext, projectName, identifier);
        IEnumerable<TestActionResult> source1 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) actionResultResponse.TestActionResults);
        List<TestActionResult> list1 = source1 != null ? source1.ToList<TestActionResult>() : (List<TestActionResult>) null;
        ref List<TestResultParameter> local1 = ref parameters;
        IEnumerable<TestResultParameter> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) actionResultResponse.TestResultParameters);
        List<TestResultParameter> list2 = source2 != null ? source2.ToList<TestResultParameter>() : (List<TestResultParameter>) null;
        local1 = list2;
        ref List<TestResultAttachment> local2 = ref attachments;
        IEnumerable<TestResultAttachment> source3 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) actionResultResponse.TestAttachments);
        List<TestResultAttachment> list3 = source3 != null ? source3.ToList<TestResultAttachment>() : (List<TestResultAttachment>) null;
        local2 = list3;
        return Compat2011QU1Helper.Convert(list1);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestCaseResult FindTestResultInMultipleProjects(
      int testRunId,
      int testResultId,
      out string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (FindTestResultInMultipleProjects), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (testResultId), (object) testResultId);
        this.EnterMethod(methodInformation);
        return Compat2011QU1Helper.Convert(TestCaseResultContractConverter.Convert(this.ResultsHelper.GetTestResultInMultipleProjects((TestManagementRequestContext) this.m_tmRequestContext, testRunId, testResultId, out projectName)));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResults(
      ResultsStoreQuery query,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        List<LegacyTestCaseResult> testResultsByQuery = this.ResultsHelper.GetTestResultsByQuery((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(query), pageSize, out excessIds1);
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list = source1 != null ? source1.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list;
        IEnumerable<TestCaseResult> source2 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) testResultsByQuery);
        return Compat2011QU1Helper.Convert(source2 != null ? source2.ToList<TestCaseResult>() : (List<TestCaseResult>) null);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> FetchTestResults(
      TestCaseResultIdAndRev[] idsToFetch,
      string projectName,
      bool includeActionResults,
      out List<TestCaseResultIdentifier> deletedIds,
      [XmlArray, XmlArrayItem(typeof (TestActionResult))] out List<TestActionResult> actionResults,
      [XmlArray, XmlArrayItem(typeof (TestResultParameter))] out List<TestResultParameter> parameters,
      [XmlArray, XmlArrayItem(typeof (TestResultAttachment))] out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation("FetchTestResultsIds", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestCaseResultIdAndRev>(nameof (idsToFetch), (IList<TestCaseResultIdAndRev>) idsToFetch);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (includeActionResults), (object) includeActionResults);
        this.EnterMethod(methodInformation);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> source1 = TestCaseResultIdAndRevConverter.Convert((IEnumerable<TestCaseResultIdAndRev>) idsToFetch);
        List<LegacyTestCaseResultIdentifier> webApiDeletedIds;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments;
        List<LegacyTestCaseResult> testCaseResults = this.ResultsHelper.Fetch((TestManagementRequestContext) this.m_tmRequestContext, source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>) null, projectName, includeActionResults, out webApiDeletedIds, out webApiActionResults, out webApiParams, out webApiAttachments);
        ref List<TestActionResult> local1 = ref actionResults;
        IEnumerable<TestActionResult> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) webApiActionResults);
        List<TestActionResult> list1 = source2 != null ? source2.ToList<TestActionResult>() : (List<TestActionResult>) null;
        local1 = list1;
        ref List<TestResultParameter> local2 = ref parameters;
        IEnumerable<TestResultParameter> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) webApiParams);
        List<TestResultParameter> list2 = source3 != null ? source3.ToList<TestResultParameter>() : (List<TestResultParameter>) null;
        local2 = list2;
        ref List<TestResultAttachment> local3 = ref attachments;
        IEnumerable<TestResultAttachment> source4 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) webApiAttachments);
        List<TestResultAttachment> list3 = source4 != null ? source4.ToList<TestResultAttachment>() : (List<TestResultAttachment>) null;
        local3 = list3;
        ref List<TestCaseResultIdentifier> local4 = ref deletedIds;
        IEnumerable<TestCaseResultIdentifier> source5 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) webApiDeletedIds);
        List<TestCaseResultIdentifier> list4 = source5 != null ? source5.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local4 = list4;
        actionResults = Compat2011QU1Helper.Convert(actionResults);
        IEnumerable<TestCaseResult> source6 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) testCaseResults);
        return Compat2011QU1Helper.Convert(source6 != null ? source6.ToList<TestCaseResult>() : (List<TestCaseResult>) null);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRun(
      int testRunId,
      int pageSize,
      string projectName,
      bool includeActionResults,
      out List<TestCaseResultIdentifier> excessIds,
      [XmlArray, XmlArrayItem(typeof (TestActionResult))] out List<TestActionResult> actionResults,
      [XmlArray, XmlArrayItem(typeof (TestResultParameter))] out List<TestResultParameter> parameters,
      [XmlArray, XmlArrayItem(typeof (TestResultAttachment))] out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (includeActionResults), (object) includeActionResults);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> webApiExcessIds;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRun((TestManagementRequestContext) this.m_tmRequestContext, testRunId, pageSize, out webApiExcessIds, projectName, includeActionResults, out webApiActionResults, out webApiParams, out webApiAttachments));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestActionResult> local1 = ref actionResults;
        IEnumerable<TestActionResult> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) webApiActionResults);
        List<TestActionResult> list2 = source2 != null ? source2.ToList<TestActionResult>() : (List<TestActionResult>) null;
        local1 = list2;
        ref List<TestResultParameter> local2 = ref parameters;
        IEnumerable<TestResultParameter> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) webApiParams);
        List<TestResultParameter> list3 = source3 != null ? source3.ToList<TestResultParameter>() : (List<TestResultParameter>) null;
        local2 = list3;
        ref List<TestResultAttachment> local3 = ref attachments;
        IEnumerable<TestResultAttachment> source4 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) webApiAttachments);
        List<TestResultAttachment> list4 = source4 != null ? source4.ToList<TestResultAttachment>() : (List<TestResultAttachment>) null;
        local3 = list4;
        ref List<TestCaseResultIdentifier> local4 = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source5 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) webApiExcessIds);
        List<TestCaseResultIdentifier> list5 = source5 != null ? source5.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local4 = list5;
        actionResults = Compat2011QU1Helper.Convert(actionResults);
        return Compat2011QU1Helper.Convert(list1);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByPoint(
      string projectName,
      int planId,
      int pointId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByPoint), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (pointId), (object) pointId);
        this.EnterMethod(methodInformation);
        IEnumerable<TestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByPoint((TestManagementRequestContext) this.m_tmRequestContext, projectName, planId, pointId));
        return Compat2011QU1Helper.Convert(source != null ? source.ToList<TestCaseResult>() : (List<TestCaseResult>) null);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRunAndOwner(
      int testRunId,
      Guid owner,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRunAndOwner), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (owner), (object) owner);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndOwner((TestManagementRequestContext) this.m_tmRequestContext, testRunId, owner, pageSize, out excessIds1, projectName));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list2;
        return Compat2011QU1Helper.Convert(list1);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRunAndState(
      int testRunId,
      byte state,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRunAndState), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (state), (object) state);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndState((TestManagementRequestContext) this.m_tmRequestContext, testRunId, state, pageSize, out excessIds1, projectName));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list2;
        return Compat2011QU1Helper.Convert(list1);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRunAndOutcome(
      int testRunId,
      byte outcome,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRunAndOutcome), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (outcome), (object) outcome);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndOutcome((TestManagementRequestContext) this.m_tmRequestContext, testRunId, outcome, pageSize, out excessIds1, projectName));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list2;
        return Compat2011QU1Helper.Convert(list1);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRuns(ResultsStoreQuery query, bool includeStatistics)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRuns), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(query), includeStatistics));
        return Compat2011QU1Helper.Convert(TestRun.FilterNotOfType(source != null ? (IEnumerable<TestRun>) source.ToList<TestRun>() : (IEnumerable<TestRun>) null, Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv), this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsInMultipleProjects(ResultsStoreQuery query)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsInMultipleProjects), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.QueryTestRunsInMultipleProjects((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(query)));
        return Compat2011QU1Helper.Convert(TestRun.FilterNotOfType(source != null ? (IEnumerable<TestRun>) source.ToList<TestRun>() : (IEnumerable<TestRun>) null, Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv), this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int QueryTestRunsCount(ResultsStoreQuery query)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsCount), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return this.ResultsHelper.QueryCount((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(query));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsById(string teamProjectName, int testRunId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, testRunId, Guid.Empty, (string) null, teamProjectName));
        return Compat2011QU1Helper.Convert(TestRun.FilterNotOfType(source != null ? (IEnumerable<TestRun>) source.ToList<TestRun>() : (IEnumerable<TestRun>) null, Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv), this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsByBuild(string teamProjectName, string buildUri)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsByBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, Guid.Empty, buildUri, teamProjectName));
        return Compat2011QU1Helper.Convert(TestRun.FilterNotOfType(source != null ? (IEnumerable<TestRun>) source.ToList<TestRun>() : (IEnumerable<TestRun>) null, Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv), this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsByOwner(string teamProjectName, Guid owner)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsByOwner), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (owner), (object) owner);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, owner, (string) null, teamProjectName));
        return Compat2011QU1Helper.Convert(TestRun.FilterNotOfType(source != null ? (IEnumerable<TestRun>) source.ToList<TestRun>() : (IEnumerable<TestRun>) null, Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv), this.m_tmRequestContext);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestRun QueryTestRunByTmiRunId(Guid tmiRunId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunByTmiRunId), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (tmiRunId), (object) tmiRunId);
        this.EnterMethod(methodInformation);
        TestRun run = TestRunContractConverter.Convert(this.ResultsHelper.QueryTestRunByTmiRunId((TestManagementRequestContext) this.m_tmRequestContext, tmiRunId));
        return run != null && !run.RunHasDtlEnvironment ? Compat2011QU1Helper.Convert(run, this.m_tmRequestContext) : (TestRun) null;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<TestRunStatistic> QueryTestRunStatistics(string projectName, int testRunId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunStatistics), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRunStatistic> source = TestRunStatisticConverter.Convert((IEnumerable<LegacyTestRunStatistic>) this.ResultsHelper.QueryTestRunStats((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId));
        return Compat2011QU1Helper.Convert(source != null ? source.ToList<TestRunStatistic>() : (List<TestRunStatistic>) null);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestCaseResult[] ResetTestResults(
      TestCaseResultIdentifier[] identifiers,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (ResetTestResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestCaseResultIdentifier>(nameof (identifiers), (IList<TestCaseResultIdentifier>) identifiers);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper resultsHelper = this.ResultsHelper;
        TfsTestManagementRequestContext tmRequestContext = this.m_tmRequestContext;
        IEnumerable<LegacyTestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) identifiers);
        LegacyTestCaseResultIdentifier[] array = source1 != null ? source1.ToArray<LegacyTestCaseResultIdentifier>() : (LegacyTestCaseResultIdentifier[]) null;
        string projectName1 = projectName;
        IEnumerable<TestCaseResult> source2 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) resultsHelper.ResetTestResults((TestManagementRequestContext) tmRequestContext, array, projectName1));
        return Compat2011QU1Helper.Convert(source2 != null ? source2.ToArray<TestCaseResult>() : (TestCaseResult[]) null);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public ResultUpdateResponse[] UpdateTestResults(
      ResultUpdateRequest[] requests,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<ResultUpdateRequest>(nameof (requests), (IList<ResultUpdateRequest>) requests);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        Compat2011QU1Helper.ValidateCompatibleResultOutcome(requests, projectName, this.m_tmRequestContext);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest> source1 = ResultUpdateRequestConverter.Convert((IEnumerable<ResultUpdateRequest>) Compat2011QU1Helper.Convert(requests));
        IEnumerable<ResultUpdateResponse> source2 = ResultUpdateResponseConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>) this.ResultsHelper.Update((TestManagementRequestContext) this.m_tmRequestContext, source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[]) null, projectName));
        return source2 != null ? source2.ToArray<ResultUpdateResponse>() : (ResultUpdateResponse[]) null;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties UpdateTestRun(
      TestRun testRun,
      TestResultAttachment[] attachments,
      TestResultAttachmentIdentity[] attachmentDeletes,
      out int[] attachmentIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRun), (object) testRun);
        methodInformation.AddArrayParameter<TestResultAttachment>(nameof (attachments), (IList<TestResultAttachment>) attachments);
        methodInformation.AddArrayParameter<TestResultAttachmentIdentity>(nameof (attachmentDeletes), (IList<TestResultAttachmentIdentity>) attachmentDeletes);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun webApiTestRun = TestRunContractConverter.Convert(testRun);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source1 = AttachmentContractConverter.Convert((IEnumerable<TestResultAttachment>) attachments);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] array1 = source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[]) null;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity> source2 = TestResultAttachmentIdentityConverter.Convert((IEnumerable<TestResultAttachmentIdentity>) attachmentDeletes);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] array2 = source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[]) null;
        return UpdatedPropertiesConverter.Convert(this.ResultsHelper.UpdateTestRun((TestManagementRequestContext) this.m_tmRequestContext, projectName, webApiTestRun, array1, array2, out attachmentIds, true));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (int))]
    public List<int> CreateLogEntriesForRun(
      int testRunId,
      TestMessageLogEntry[] logEntries,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation("CreateTestMessageLog", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddArrayParameter<TestMessageLogEntry>(nameof (logEntries), (IList<TestMessageLogEntry>) logEntries);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return this.LegacyResultsHelper.CreateLogEntriesForRun(this.m_tmRequestContext, projectName, testRunId, logEntries);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestMessageLogEntry))]
    public List<TestMessageLogEntry> QueryLogEntriesForRun(
      int testRunId,
      int testMessageLogId,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryLogEntriesForRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (testMessageLogId), (object) testMessageLogId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return this.LegacyResultsHelper.QueryLogEntriesForRun(this.m_tmRequestContext, projectName, testRunId, testMessageLogId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int[][] QueryAssociatedWorkItemsFromResults(
      int[] pointIds,
      int planId,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryAssociatedWorkItemsFromResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (pointIds), (IList<int>) pointIds);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return TestPoint.QueryAssociatedWorkItemsFromResults((TestManagementRequestContext) this.m_tmRequestContext, pointIds, planId, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void CreateAssociatedWorkItems(
      TestCaseResultIdentifier[] identifiers,
      string[] workItemUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateAssociatedWorkItems), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestCaseResultIdentifier>(nameof (identifiers), (IList<TestCaseResultIdentifier>) identifiers);
        methodInformation.AddArrayParameter<string>(nameof (workItemUris), (IList<string>) workItemUris);
        this.EnterMethod(methodInformation);
        this.ResultsHelper.CreateAssociatedWorkItems((TestManagementRequestContext) this.m_tmRequestContext, TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) identifiers), workItemUris);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteAssociatedWorkItems(
      TestCaseResultIdentifier[] identifiers,
      string[] workItemUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteAssociatedWorkItems), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestCaseResultIdentifier>(nameof (identifiers), (IList<TestCaseResultIdentifier>) identifiers);
        methodInformation.AddArrayParameter<string>(nameof (workItemUris), (IList<string>) workItemUris);
        this.EnterMethod(methodInformation);
        this.ResultsHelper.DeleteAssociatedWorkItems((TestManagementRequestContext) this.m_tmRequestContext, TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) identifiers), workItemUris);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void SetTestGroupPermissions(string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetTestGroupPermissions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        TeamProject.SetTestGroupPermissions((TestManagementRequestContext) this.m_tmRequestContext, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    internal Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper ResultsHelper
    {
      get
      {
        if (this.m_resultsHelper == null)
          this.m_resultsHelper = new Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper((TestManagementRequestContext) this.m_tmRequestContext);
        return this.m_resultsHelper;
      }
    }

    internal LegacyResultsHelper LegacyResultsHelper
    {
      get
      {
        if (this.m_legacyResultsHelper == null)
          this.m_legacyResultsHelper = new LegacyResultsHelper();
        return this.m_legacyResultsHelper;
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.TestManagement.Server.Session CreateSession(
      Microsoft.TeamFoundation.TestManagement.Server.Session session,
      TestSettings settings,
      string teamProjectName)
    {
      try
      {
        bool isFeedbackSession = false;
        string str = string.Empty;
        if (session != null)
        {
          if (session.FeedbackId == 0 && session.TestPlanId != 0)
          {
            str = "_X";
            isFeedbackSession = false;
          }
          else
          {
            str = "_F";
            isFeedbackSession = true;
          }
        }
        MethodInformation methodInformation = new MethodInformation(this.AppendXTSuffixStringIfApplicable("CreateTestSession" + str, session), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (session), (object) session);
        methodInformation.AddParameter(nameof (settings), (object) settings);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.Server.Session>(session, nameof (session), this.m_tmRequestContext.RequestContext.ServiceName);
        return session.Create(this.m_tmRequestContext, settings, teamProjectName, isFeedbackSession);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteSessions(int[] sessions, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteSessions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (sessions), (IList<int>) sessions);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.Session.Delete((TestManagementRequestContext) this.m_tmRequestContext, sessions, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (Microsoft.TeamFoundation.TestManagement.Server.Session))]
    public List<Microsoft.TeamFoundation.TestManagement.Server.Session> QuerySessions(
      ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySessions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return Microsoft.TeamFoundation.TestManagement.Server.Session.Query((TestManagementRequestContext) this.m_tmRequestContext, query);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (Microsoft.TeamFoundation.TestManagement.Server.Session))]
    public List<Microsoft.TeamFoundation.TestManagement.Server.Session> QuerySessionsInMultipleProjects(
      ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySessionsInMultipleProjects), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return Microsoft.TeamFoundation.TestManagement.Server.Session.QueryInMultipleProjects((TestManagementRequestContext) this.m_tmRequestContext, query);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int QuerySessionsCount(ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySessionsCount), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return Microsoft.TeamFoundation.TestManagement.Server.Session.QueryCount((TestManagementRequestContext) this.m_tmRequestContext, query);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (Microsoft.TeamFoundation.TestManagement.Server.Session))]
    public List<Microsoft.TeamFoundation.TestManagement.Server.Session> QuerySessionsById(
      string teamProjectName,
      int sessionId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySessionsById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (sessionId), (object) sessionId);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        return Microsoft.TeamFoundation.TestManagement.Server.Session.Query((TestManagementRequestContext) this.m_tmRequestContext, sessionId, Guid.Empty, (string) null, teamProjectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties UpdateSession(
      Microsoft.TeamFoundation.TestManagement.Server.Session session,
      TestResultAttachment[] attachments,
      TestResultAttachmentIdentity[] attachmentDeletes,
      out int[] attachmentIds,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(this.AppendXTSuffixStringIfApplicable(nameof (UpdateSession), session), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (session), (object) session);
        methodInformation.AddArrayParameter<TestResultAttachment>(nameof (attachments), (IList<TestResultAttachment>) attachments);
        methodInformation.AddArrayParameter<TestResultAttachmentIdentity>(nameof (attachmentDeletes), (IList<TestResultAttachmentIdentity>) attachmentDeletes);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        bool isFeedBackSession = false;
        if (session != null)
          isFeedBackSession = session.FeedbackId != 0 || session.TestPlanId == 0;
        UpdatedProperties updatedProperties = new UpdatedProperties();
        updatedProperties.Revision = -2;
        if (session != null)
          updatedProperties = session.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, isFeedBackSession);
        attachmentIds = TestResultAttachment.Create((TestManagementRequestContext) this.m_tmRequestContext, attachments, projectName, true, isFeedBackSession);
        TestResultAttachment.Delete((TestManagementRequestContext) this.m_tmRequestContext, attachmentDeletes, projectName);
        return updatedProperties;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int[][] QueryAssociatedWorkItemsFromSessions(int[] sessionIds, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryAssociatedWorkItemsFromSessions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (sessionIds), (IList<int>) sessionIds);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return Microsoft.TeamFoundation.TestManagement.Server.Session.QueryAssociatedWorkItemsFromSessions((TestManagementRequestContext) this.m_tmRequestContext, sessionIds, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void CreateAssociatedWorkItemsForSessions(int[] identifiers, string[] workItemUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(this.AppendXTSuffixStringIfApplicable("CreateAssociatedWorkItems"), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (identifiers), (IList<int>) identifiers);
        methodInformation.AddArrayParameter<string>(nameof (workItemUris), (IList<string>) workItemUris);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.Session.CreateAssociatedWorkItems((TestManagementRequestContext) this.m_tmRequestContext, identifiers, workItemUris);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteAssociatedWorkItemsForSessions(int[] identifiers, string[] workItemUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("DeleteAssociatedWorkItems", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (identifiers), (IList<int>) identifiers);
        methodInformation.AddArrayParameter<string>(nameof (workItemUris), (IList<string>) workItemUris);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.Session.DeleteAssociatedWorkItems((TestManagementRequestContext) this.m_tmRequestContext, identifiers, workItemUris);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private string AppendXTSuffixStringIfApplicable(string command, Microsoft.TeamFoundation.TestManagement.Server.Session session)
    {
      string str = command;
      if (session != null && session.TestPlanId == -1)
        str = command + "_XTWeb";
      return str;
    }

    private string AppendXTSuffixStringIfApplicable(string command)
    {
      string str = command;
      if (!string.IsNullOrEmpty(this.RequestContext.UserAgent) && this.RequestContext.UserAgent.StartsWith("Mozilla", true, CultureInfo.InvariantCulture))
        str = command + "_XTWeb";
      return str;
    }

    [WebMethod]
    public UpdatedProperties AddCasesToSuite(
      IdAndRev parent,
      TestCaseAndOwner[] testCases,
      int toIndex,
      string projectName,
      [XmlArray, XmlArrayItem(typeof (int))] out List<int> configurationIds,
      [XmlArray, XmlArrayItem(typeof (string))] out List<string> configurationNames)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddCasesToSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parent), (object) parent);
        methodInformation.AddArrayParameter<TestCaseAndOwner>(nameof (testCases), (IList<TestCaseAndOwner>) testCases);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(parent, nameof (parent), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        return ServerTestSuite.CreateEntries((TestManagementRequestContext) this.m_tmRequestContext, parent, testCases, toIndex, projectName, out configurationIds, out configurationNames, TestSuiteSource.Mtm);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties AddSuiteToSuite(
      IdAndRev parent,
      ServerTestSuite suite,
      string teamProjectName,
      out UpdatedProperties newSuiteProperties,
      int toIndex)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddSuiteToSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parent), (object) parent);
        methodInformation.AddParameter(nameof (suite), (object) suite);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        UpdatedProperties updateProperties = this.CreateUpdateProperties(parent, this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<ServerTestSuite>(suite, nameof (suite), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        newSuiteProperties = suite.Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ref updateProperties, toIndex, TestSuiteSource.Mtm);
        return updateProperties;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int CopyTestSuiteEntries(
      string teamProjectName,
      IdAndRev fromSuiteId,
      List<TestSuiteEntry> entries,
      IdAndRev toSuiteId,
      int toIndex)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CopyTestSuiteEntries), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (fromSuiteId), (object) fromSuiteId);
        methodInformation.AddArrayParameter<TestSuiteEntry>(nameof (entries), (IList<TestSuiteEntry>) entries);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(fromSuiteId, nameof (fromSuiteId), this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<IdAndRev>(toSuiteId, nameof (toSuiteId), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        return ServerTestSuite.CopyEntries((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, fromSuiteId.Id, entries, toSuiteId, toIndex);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties DeleteTestSuiteEntries(
      IdAndRev parentSuiteId,
      List<TestSuiteEntry> entries,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestSuiteEntries), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parentSuiteId), (object) parentSuiteId);
        methodInformation.AddArrayParameter<TestSuiteEntry>(nameof (entries), (IList<TestSuiteEntry>) entries);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(parentSuiteId, nameof (parentSuiteId), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        return ServerTestSuite.DeleteEntries(this.m_tmRequestContext, parentSuiteId, entries, projectName);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> FetchTestSuites(
      string teamProjectName,
      IdAndRev[] suiteIds,
      out List<int> deletedIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestSuites), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddArrayParameter<IdAndRev>("suitesIds", (IList<IdAndRev>) suiteIds);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        deletedIds = new List<int>();
        return Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ServerTestSuite.Fetch((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, suiteIds, deletedIds));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> FetchTestSuitesForPlan(string teamProjectName, int planId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestSuitesForPlan), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        return Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ServerTestSuite.FetchTestSuitesForPlan((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, planId, false));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties MoveTestSuiteEntries(
      string teamProjectName,
      IdAndRev fromSuiteId,
      List<TestSuiteEntry> entries,
      IdAndRev toSuiteId,
      out UpdatedProperties newToProps,
      int toIndex)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (MoveTestSuiteEntries), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (fromSuiteId), (object) fromSuiteId);
        methodInformation.AddArrayParameter<TestSuiteEntry>(nameof (entries), (IList<TestSuiteEntry>) entries);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        UpdatedProperties updateProperties = this.CreateUpdateProperties(fromSuiteId, this.m_tmRequestContext.RequestContext.ServiceName);
        newToProps = this.CreateUpdateProperties(toSuiteId, this.m_tmRequestContext.RequestContext.ServiceName);
        ServerTestSuite.MoveEntries((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ref updateProperties, entries, ref newToProps, toIndex);
        return updateProperties;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> QueryTestSuites(
      ResultsStoreQuery query,
      int pageSize,
      out SuiteIdAndType[] excessIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestSuites), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, query.TeamProjectName);
        List<SuiteIdAndType> excessIds1 = new List<SuiteIdAndType>();
        List<ServerTestSuite> suites = ServerTestSuite.Query((TestManagementRequestContext) this.m_tmRequestContext, query, pageSize, excessIds1);
        List<ServerTestSuite> serverTestSuiteList = Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, query.TeamProjectName, suites);
        excessIds = excessIds1.ToArray();
        return serverTestSuiteList;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> QueryTestSuitesByTestCaseId(
      string teamProjectName,
      int testCaseId,
      int pageSize,
      out SuiteIdAndType[] excessIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestSuitesByTestCaseId), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (testCaseId), (object) testCaseId);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        List<SuiteIdAndType> excessIds1 = new List<SuiteIdAndType>();
        List<ServerTestSuite> suites = ServerTestSuite.QueryByTestCaseId((TestManagementRequestContext) this.m_tmRequestContext, testCaseId, teamProjectName, pageSize, excessIds1);
        List<ServerTestSuite> serverTestSuiteList = Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, suites);
        excessIds = excessIds1.ToArray();
        return serverTestSuiteList;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void RepopulateTestSuite(string teamProjectName, int suiteId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RepopulateTestSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (suiteId), (object) suiteId);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        ServerTestSuite.Repopulate((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, suiteId, TestSuiteSource.Mtm);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties SetSuiteEntryConfigurations(
      string projectName,
      IdAndRev suite,
      TestCaseAndOwner[] testCases,
      int[] configIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetSuiteEntryConfigurations), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (suite), (object) suite);
        methodInformation.AddArrayParameter<TestCaseAndOwner>(nameof (testCases), (IList<TestCaseAndOwner>) testCases);
        methodInformation.AddArrayParameter<int>(nameof (configIds), (IList<int>) configIds);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(suite, nameof (suite), this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<TestCaseAndOwner[]>(testCases, nameof (testCases), this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<int[]>(configIds, nameof (configIds), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        return ServerTestSuite.SetSuiteEntryConfigurations((TestManagementRequestContext) this.m_tmRequestContext, projectName, suite, testCases, (IEnumerable<int>) configIds);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void AssignTestPoints(
      string projectName,
      int suiteId,
      TestPointAssignment[] assignments)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AssignTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (suiteId), (object) suiteId);
        methodInformation.AddArrayParameter<TestPointAssignment>(nameof (assignments), (IList<TestPointAssignment>) assignments);
        this.EnterMethod(methodInformation);
        ServerTestSuite.AssignTestPoints((TestManagementRequestContext) this.m_tmRequestContext, projectName, suiteId, assignments);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public UpdatedProperties UpdateTestSuite(ServerTestSuite testSuite, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testSuite), (object) testSuite);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<ServerTestSuite>(testSuite, nameof (testSuite), this.m_tmRequestContext.RequestContext.ServiceName);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        testSuite.Status = Compat2013UpdateHelper.ConvertTcmStateToWorkItemState((TestManagementRequestContext) this.m_tmRequestContext, projectName, testSuite.Id, testSuite.State, WitCategoryRefName.TestSuite);
        return testSuite.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, TestSuiteSource.Mtm, false);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CloneOperationInformation GetCloneOperationInformation(int opId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetCloneOperationInformation), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (opId), (object) opId);
        this.EnterMethod(methodInformation);
        this.m_tmRequestContext.TraceVerbose("WebService", "GetCloneOperationInformation: {0}", (object) opId);
        return ServerTestSuite.GetCloneOperationInformation((TestManagementRequestContext) this.m_tmRequestContext, projectName, opId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int BeginCloneOperation(
      int fromSuiteId,
      int toSuiteId,
      string projectName,
      CloneOptions options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (BeginCloneOperation), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (fromSuiteId), (object) fromSuiteId);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        this.m_tmRequestContext.TraceVerbose("WebService", "BeginCloneOperation: {0}, {1}", (object) fromSuiteId, (object) toSuiteId);
        ArgumentUtility.CheckForNull<CloneOptions>(options, nameof (options), this.m_tmRequestContext.RequestContext.ServiceName);
        return ServerTestSuite.BeginCloneOperation((TestManagementRequestContext) this.m_tmRequestContext, projectName, fromSuiteId, projectName, toSuiteId, options, true);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private UpdatedProperties CreateUpdateProperties(IdAndRev idAndRev, string expectedSeviceName)
    {
      ArgumentUtility.CheckForNull<IdAndRev>(idAndRev, nameof (idAndRev), expectedSeviceName);
      return new UpdatedProperties()
      {
        Revision = idAndRev.Revision,
        Id = idAndRev.Id
      };
    }
  }
}
