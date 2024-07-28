// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestResultsService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.Server.TcmService;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2007/02/TCM/TestResults/01", Description = "Test Management Results Service", Name = "TestResultsService")]
  [ClientService(ComponentName = "TestManagement", RegistrationName = "TestManagement", ServiceName = "TestResultsService", CollectionServiceIdentifier = "e26a5f7b-635a-4256-899c-f4949601a857")]
  [WebServiceBinding(Name = "LinkingService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03")]
  [WebServiceBinding(Name = "IProjectMaintenanceBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  public class TestResultsService : TeamFoundationWebService
  {
    private TfsTestManagementRequestContext m_tmRequestContext;
    private LegacyResultsHelper m_legacyResultsHelper;
    private Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper m_resultsHelper;
    private AttachmentsHelper m_attachmentsHelper;
    private AfnStripsHelper m_afnStripsHelper;

    public TestResultsService()
    {
      this.RequestContext.ServiceName = "Test Management";
      this.m_tmRequestContext = new TfsTestManagementRequestContext(this.RequestContext);
    }

    public bool isOnPremiseDeployment() => this.m_tmRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment;

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
        return Compat2010Helper.Convert(this.AfnStripsHelper.GetDefaultAfnStrips(projectName, (IList<int>) testCaseIds).Select<AfnStrip, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>((Func<AfnStrip, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (afnStrip => AfnStripContractConverter.Convert(afnStrip))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>());
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
        return Compat2010Helper.Convert(this.AttachmentsHelper.GetTestAttachmentsByQuery((TestManagementRequestContext) this.m_tmRequestContext, query.TeamProjectName, ResultsStoreQueryContractConverter.Convert(Compat2010Helper.Convert(query))).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>());
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
        return Compat2010Helper.Convert(this.AttachmentsHelper.GetTestAttachments((TestManagementRequestContext) this.m_tmRequestContext, projectName, attachmentId, getSiblingAttachments).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>());
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
        return Compat2010Helper.Convert(this.AttachmentsHelper.GetTestAttachments((TestManagementRequestContext) this.m_tmRequestContext, projectName, identifier.TestRunId, identifier.TestResultId, 0, 0, 0).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>());
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
        return Compat2010Helper.Convert(this.AttachmentsHelper.GetTestAttachments((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId, 0, 0, 0, 0).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>());
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
        this.AfnStripsHelper.UpdateDefaultStrip((TestManagementRequestContext) this.m_tmRequestContext, projectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>) ((IEnumerable<DefaultAfnStripBinding>) afnStripBindingList).Select<DefaultAfnStripBinding, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>((Func<DefaultAfnStripBinding, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>) (binding => AfnStripContractConverter.Convert(Compat2010Helper.Convert(binding)))).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>());
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
        TcmArgumentValidator.CheckNull((object) bugFieldMapping, nameof (bugFieldMapping));
        return Compat2010Helper.Convert(Compat2010Helper.Convert(bugFieldMapping).Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.BugFieldMapping.Query((TestManagementRequestContext) this.m_tmRequestContext, projectName));
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
        TcmArgumentValidator.CheckNull((object) bugFieldMapping, nameof (bugFieldMapping));
        return Compat2010Helper.Convert(Compat2010Helper.Convert(bugFieldMapping).Update((TestManagementRequestContext) this.m_tmRequestContext, projectName));
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
          Microsoft.TeamFoundation.TestManagement.Server.Validator.TranslateBuildUri(buildUri)
        });
        if (this.isOnPremiseDeployment())
          return;
        SoapBuildDeleteEvent payload = new SoapBuildDeleteEvent(Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, buildUri);
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
        TcmArgumentValidator.CheckNull((object) testConfiguration, nameof (testConfiguration));
        return Compat2010Helper.Convert(Compat2010Helper.Convert(testConfiguration).Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName));
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
        Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration.Delete((TestManagementRequestContext) this.m_tmRequestContext, testConfigurationId, projectName);
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration.QueryById((TestManagementRequestContext) this.m_tmRequestContext, testConfigurationId, projectName));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration.Query((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(query), 0));
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
        return Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration.QueryCount((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(query));
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
        TcmArgumentValidator.CheckNull((object) testConfiguration, nameof (testConfiguration));
        return Compat2010Helper.Convert(Compat2010Helper.Convert(testConfiguration).Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, updateInUse, unchangedValues));
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
        TcmArgumentValidator.CheckNull((object) variable, nameof (variable));
        return Compat2010Helper.Convert(variable).Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
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
        TcmArgumentValidator.CheckNull((object) variable, nameof (variable));
        return Compat2010Helper.Convert(variable).Update((TestManagementRequestContext) this.m_tmRequestContext, projectName);
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
        Microsoft.TeamFoundation.TestManagement.Server.TestVariable.Delete((TestManagementRequestContext) this.m_tmRequestContext, variableId, projectName);
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestVariable.QueryById((TestManagementRequestContext) this.m_tmRequestContext, variableId, projectName));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestVariable.QueryTestVariables((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName));
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
        TcmArgumentValidator.CheckNull((object) settings, nameof (settings));
        GuidAndString projectFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
        return this.m_tmRequestContext.LegacyTcmServiceHelper.TryCreateTestSettingsCompat(this.m_tmRequestContext.RequestContext, projectFromName.GuidId, TestSettingsContractConverter.Convert(Compat2010Helper.Convert(settings)), out newUpdateProperties) ? Compat2010Helper.Convert(UpdatedPropertiesConverter.Convert(newUpdateProperties)) : new UpdatedProperties();
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
        TcmArgumentValidator.CheckNull((object) settings, nameof (settings));
        GuidAndString projectFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
        return this.m_tmRequestContext.LegacyTcmServiceHelper.TryUpdateTestSettings(this.m_tmRequestContext.RequestContext, projectFromName.GuidId, TestSettingsContractConverter.Convert(Compat2010Helper.Convert(settings)), out newUpdateProperties) ? Compat2010Helper.Convert(UpdatedPropertiesConverter.Convert(newUpdateProperties)) : new UpdatedProperties();
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
        this.m_tmRequestContext.TcmServiceHelper.TryDeleteTestSettings(this.m_tmRequestContext.RequestContext, Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, settingsId);
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
        if (!this.m_tmRequestContext.LegacyTcmServiceHelper.TryGetTestSettingsCompatById(this.m_tmRequestContext.RequestContext, Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, settingsId, out newTestSettings) || newTestSettings == null)
          return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestSettings.QueryById((TestManagementRequestContext) this.m_tmRequestContext, settingsId, projectName));
        Microsoft.TeamFoundation.TestManagement.Server.TestSettings settings = TestSettingsContractConverter.Convert(newTestSettings);
        settings.AreaPath = projectName;
        return Compat2010Helper.Convert(settings);
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
        if (!this.m_tmRequestContext.LegacyTcmServiceHelper.TryQueryTestSettings(this.m_tmRequestContext.RequestContext, ResultsStoreQueryContractConverter.Convert(Compat2010Helper.Convert(query)), omitSettings, out newTestSettings) || newTestSettings == null)
          return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestSettings.Query((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(query), omitSettings));
        List<Microsoft.TeamFoundation.TestManagement.Server.TestSettings> list = newTestSettings.Select<LegacyTestSettings, Microsoft.TeamFoundation.TestManagement.Server.TestSettings>((Func<LegacyTestSettings, Microsoft.TeamFoundation.TestManagement.Server.TestSettings>) (testSetting => TestSettingsContractConverter.Convert(testSetting))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestSettings>();
        foreach (Microsoft.TeamFoundation.TestManagement.Server.TestSettings testSettings in list)
          testSettings.AreaPath = query.TeamProjectName;
        return Compat2010Helper.Convert(list);
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
        return this.m_tmRequestContext.LegacyTcmServiceHelper.TryQueryTestSettingsCount(this.m_tmRequestContext.RequestContext, ResultsStoreQueryContractConverter.Convert(Compat2010Helper.Convert(query)), out testSettingsCount) ? testSettingsCount.GetValueOrDefault() : 0;
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
    public int CreateTestResolutionState(
      TestResolutionState resolutionState,
      string teamProjectName)
    {
      throw this.HandleException(new Exception(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestResolutionStateDeprecationMessage));
    }

    [WebMethod]
    public void UpdateTestResolutionState(TestResolutionState state, string projectName) => throw this.HandleException(new Exception(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestResolutionStateDeprecationMessage));

    [WebMethod]
    public void DeleteTestResolutionState(int stateId, string projectName) => throw this.HandleException(new Exception(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestResolutionStateDeprecationMessage));

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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState.Query((TestManagementRequestContext) this.m_tmRequestContext, testResolutionStateId, projectName));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, teamProject));
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
        List<Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation> collectors1 = Compat2010Helper.Convert(collectors);
        Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation.Register((TestManagementRequestContext) this.m_tmRequestContext, collectors1);
        return Compat2010Helper.Convert(collectors1);
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
        Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation.Unregister((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(collectors));
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
        Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation.Update((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(collectors));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation.Query(this.m_tmRequestContext));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation.Find((TestManagementRequestContext) this.m_tmRequestContext, typeUri));
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
        MethodInformation methodInformation = new MethodInformation(nameof (RegisterControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestController>(nameof (controllers), (IList<TestController>) controllers);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.TestController.Register((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(controllers));
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
        Microsoft.TeamFoundation.TestManagement.Server.TestController.Unregister((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(controllers));
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
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestController>(nameof (controllers), (IList<TestController>) controllers);
        methodInformation.AddParameter(nameof (teamProjectCollectionCatalogResourceId), (object) teamProjectCollectionCatalogResourceId);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.TestController.Update((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(controllers));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestController.Query(this.m_tmRequestContext));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestController.Query((TestManagementRequestContext) this.m_tmRequestContext, groupId));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestController.Find((TestManagementRequestContext) this.m_tmRequestContext, controllerName));
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
        List<Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment> environments1 = Compat2010Helper.Convert(environments);
        Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment.Register(this.m_tmRequestContext, environments1, parentName, parentType, teamProjectCollectionCatalogResourceId);
        return Compat2010Helper.Convert(environments1);
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
        Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment.Unregister((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(environments), parentName, parentType, teamProjectCollectionCatalogResourceId);
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
        List<Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment> environments1 = Compat2010Helper.Convert(environments);
        Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment.Update((TestManagementRequestContext) this.m_tmRequestContext, environments1, parentName, parentType, teamProjectCollectionCatalogResourceId);
        return Compat2010Helper.Convert(environments1);
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
        List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverage1;
        return this.m_tmRequestContext.TcmServiceHelper.TryGetBuildCodeCoverage(this.m_tmRequestContext.RequestContext, Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, this.GetBuildIdFromUri(buildUri), flags, out buildCoverage1) ? buildCoverage1.Select<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage, BuildCoverage>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage, BuildCoverage>) (buildCoverage => this.ConvertBuildCoverageToServerOM(buildCoverage))).ToList<BuildCoverage>() : new List<BuildCoverage>();
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
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> runCoverage;
        return this.m_tmRequestContext.TcmServiceHelper.TryGetTestRunCodeCoverage(this.m_tmRequestContext.RequestContext, Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName).GuidId, testRunId, flags, out runCoverage) ? runCoverage.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage, TestRunCoverage>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage, TestRunCoverage>) (testRunCoverage => this.ConvertTestRunCoverageToServerOM(testRunCoverage))).ToList<TestRunCoverage>() : new List<TestRunCoverage>();
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.ImpactedPoint.QueryImpactedPointsForPlan((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, planId, Microsoft.TeamFoundation.TestManagement.Server.Validator.TranslateBuildUri(buildUri)));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPoint.Block2(this.m_tmRequestContext, Compat2010Helper.Convert(points), projectName));
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
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        TcmArgumentValidator.CheckNull((object) testPlan, nameof (testPlan));
        return Compat2010Helper.Convert(Compat2010Helper.Convert((TestManagementRequestContext) this.m_tmRequestContext, testPlan).Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, Compat2010Helper.Convert(links), TestPlanSource.Mtm));
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
        throw new TestManagementInvalidOperationException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.DeleteTestPlanNotSupported);
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
        methodInformation.AddParameter(nameof (idsToFetch), (object) idsToFetch);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        deletedIds = new List<int>();
        return Compat2010Helper.Convert(Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, projectName, Microsoft.TeamFoundation.TestManagement.Server.TestPlan.Fetch((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(idsToFetch), deletedIds, projectName)));
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
      out List<int> deletedIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (idsToFetch), (object) idsToFetch);
        this.EnterMethod(methodInformation);
        deletedIds = new List<int>();
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPoints = Microsoft.TeamFoundation.TestManagement.Server.TestPoint.Fetch((TestManagementRequestContext) this.m_tmRequestContext, projectName, planId, Compat2010Helper.Convert(idsToFetch), (string[]) null, deletedIds);
        return Compat2010Helper.Convert(TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, projectName, planId, testPoints));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPlan.QuerySuitePointCounts((TestManagementRequestContext) this.m_tmRequestContext, planId, Compat2010Helper.Convert(query)));
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
        return Microsoft.TeamFoundation.TestManagement.Server.TestPlan.QueryTestCases((TestManagementRequestContext) this.m_tmRequestContext, queryText, inPlans, teamProjectName);
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPlan.Query((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(query), false));
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
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink.QueryTestPlanLinks(this.m_tmRequestContext, projectName, planId));
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
    public List<TestPoint> QueryTestPoints(int planId, ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPoints = Microsoft.TeamFoundation.TestManagement.Server.TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, 0, Compat2010Helper.Convert(query), out List<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic> _, false, (string[]) null);
        return Compat2010Helper.Convert(TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints));
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
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPoints = Microsoft.TeamFoundation.TestManagement.Server.TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, int.MaxValue, Compat2010Helper.Convert(query), out List<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic> _, false, (string[]) null);
        return Compat2010Helper.Convert(TestPointUpdate.GetTestPointStatistics(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints));
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
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPoints = Microsoft.TeamFoundation.TestManagement.Server.TestPoint.QueryTestPointHistory((TestManagementRequestContext) this.m_tmRequestContext, testPointId, planId, projectName);
        return Compat2010Helper.Convert(TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, projectName, planId, testPoints));
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
        TcmArgumentValidator.CheckNull((object) testPlan, nameof (testPlan));
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan1 = Compat2010Helper.Convert((TestManagementRequestContext) this.m_tmRequestContext, testPlan);
        testPlan1.Status = Compat2013UpdateHelper.ConvertTcmStateToWorkItemState((TestManagementRequestContext) this.m_tmRequestContext, projectName, testPlan1.PlanId, testPlan1.State, WitCategoryRefName.TestPlan);
        return Compat2010Helper.Convert(testPlan1.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, Compat2010Helper.Convert(changedLinks)));
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
        GuidAndString projectFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        PlannedResultsTCMServiceHelper tcmServiceHelper1 = this.m_tmRequestContext.PlannedTestingTCMServiceHelper;
        foreach (IGrouping<int, TestPoint> source1 in ((IEnumerable<TestPoint>) points).GroupBy<TestPoint, int>((Func<TestPoint, int>) (point => point.PlanId)))
        {
          PlannedResultsTCMServiceHelper tcmServiceHelper2 = tcmServiceHelper1;
          TfsTestManagementRequestContext tmRequestContext = this.m_tmRequestContext;
          Microsoft.TeamFoundation.TestManagement.Server.TestPoint[] source2 = Compat2010Helper.Convert(source1.Select<TestPoint, TestPoint>((Func<TestPoint, TestPoint>) (point => point)).ToArray<TestPoint>());
          List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> list = source2 != null ? ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>) source2).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>) null;
          Guid guidId = projectFromName.GuidId;
          int key = source1.Key;
          tcmServiceHelper2.UnblockTestPointResultsIfAny((TestManagementRequestContext) tmRequestContext, list, guidId, key);
        }
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPoint.Update((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(points), projectName, updateResultsInTCM: true));
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
        if (results == null || results.Length == 0)
          return;
        Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] results1 = Compat2010Helper.Convert(results);
        this.m_tmRequestContext.WorkItemFieldDataHelper.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired((TestManagementRequestContext) this.m_tmRequestContext, Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName), results1, false);
        IEnumerable<LegacyTestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) results1);
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
    public TestRun CreateTestRun(TestRun testRun, string teamProjectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        string str = string.Empty;
        if (testRun != null)
          str = testRun.TestPlanId != 0 ? (!testRun.IsAutomated ? (testRun.Type == (byte) 8 ? "_M1" : "_M") : "_A2") : "_A1";
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestRun) + str, MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRun), (object) testRun);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestRun>(testRun, nameof (testRun), "Test Results");
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun1 = TestRunContractConverter.Convert(Compat2010Helper.Convert(testRun));
        return Compat2010Helper.Convert(TestRunContractConverter.Convert(this.ResultsHelper.CreateTestRun((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, testRun1, (LegacyTestCaseResult[]) null, (LegacyTestSettings) null)));
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
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (AbortTestRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (revision), (object) revision);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        GuidAndString projectFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        if (!this.m_tmRequestContext.TestPointOutcomeHelper.IsDualWriteEnabled(this.m_tmRequestContext.RequestContext))
          return Compat2010Helper.Convert(UpdatedPropertiesConverter.Convert(this.ResultsHelper.AbortTestRun((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId, revision, options)));
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResultsByRunId = this.ResultsHelper.GetTestResultsByRunId((TestManagementRequestContext) this.m_tmRequestContext, projectFromName.GuidId, testRunId);
        int testPlanId = 0;
        List<int> pointIds = new List<int>();
        if (testResultsByRunId != null && testResultsByRunId.Count > 0)
        {
          testPlanId = int.Parse(testResultsByRunId[0].TestPlan.Id);
          pointIds = testResultsByRunId.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>) (res => res.TestPoint == null ? 0 : int.Parse(res.TestPoint.Id))).ToList<int>();
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties = this.ResultsHelper.AbortTestRun((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId, revision, options);
        TestResultsQuery query = new TestResultsQuery()
        {
          ResultsFilter = new ResultsFilter()
          {
            AutomatedTestName = string.Empty,
            TestPlanId = testPlanId,
            TestPointIds = (IList<int>) pointIds
          }
        };
        TestResultsQuery results = (TestResultsQuery) null;
        if (!this.m_tmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
          this.m_tmRequestContext.TcmServiceHelper.TryGetTestResultsByQuery(this.m_tmRequestContext.RequestContext, projectFromName.GuidId, query, out results);
        else
          results = this.m_tmRequestContext.RequestContext.GetClient<TestResultsHttpClient>().GetTestResultsByQueryAsync(query, projectFromName.GuidId, (object) null, new CancellationToken())?.Result;
        if (results != null)
        {
          int? count = results.Results?.Count;
          int num = 0;
          if (count.GetValueOrDefault() > num & count.HasValue)
          {
            this.m_tmRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.m_tmRequestContext.RequestContext, projectName, results.Results);
            goto label_10;
          }
        }
        this.m_tmRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeWithoutResult(this.m_tmRequestContext.RequestContext, projectName, testPlanId, (IList<int>) pointIds);
label_10:
        return Compat2010Helper.Convert(UpdatedPropertiesConverter.Convert(updatedProperties));
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
        return Compat2010Helper.Convert(new Microsoft.TeamFoundation.TestManagement.Server.BuildConfiguration().Query(this.m_tmRequestContext.RequestContext, buildConfigurationId));
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
        QueryTestActionResultResponse actionResultResponse = this.ResultsHelper.QueryTestActionResults((TestManagementRequestContext) this.m_tmRequestContext, projectName, Compat2010Helper.Convert(identifier));
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> source1 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) actionResultResponse.TestActionResults);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> list = source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) null;
        ref List<TestResultParameter> local1 = ref parameters;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) actionResultResponse.TestResultParameters);
        List<TestResultParameter> testResultParameterList = Compat2010Helper.Convert(source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) null);
        local1 = testResultParameterList;
        ref List<TestResultAttachment> local2 = ref attachments;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> source3 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) actionResultResponse.TestAttachments);
        List<TestResultAttachment> resultAttachmentList = Compat2010Helper.Convert(source3 != null ? source3.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) null);
        local2 = resultAttachmentList;
        return Compat2010Helper.Convert(list);
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
        return Compat2010Helper.Convert(TestCaseResultContractConverter.Convert(this.ResultsHelper.GetTestResultInMultipleProjects((TestManagementRequestContext) this.m_tmRequestContext, testRunId, testResultId, out projectName)));
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
        List<LegacyTestCaseResult> testResultsByQuery = this.ResultsHelper.GetTestResultsByQuery((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(Compat2010Helper.Convert(query)), pageSize, out excessIds1);
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> resultIdentifierList = Compat2010Helper.Convert(source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null);
        local = resultIdentifierList;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source2 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) testResultsByQuery);
        return Compat2010Helper.Convert(source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null);
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
      out List<TestCaseResultIdentifier> deletedIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation("FetchTestResultsIds", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (idsToFetch), (object) idsToFetch);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> source1 = TestCaseResultIdAndRevConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>) Compat2010Helper.Convert(idsToFetch));
        List<LegacyTestCaseResultIdentifier> webApiDeletedIds;
        List<LegacyTestCaseResult> testCaseResults = this.ResultsHelper.Fetch((TestManagementRequestContext) this.m_tmRequestContext, source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>) null, projectName, false, out webApiDeletedIds, out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> _, out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> _, out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> _);
        ref List<TestCaseResultIdentifier> local = ref deletedIds;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) webApiDeletedIds);
        List<TestCaseResultIdentifier> resultIdentifierList = Compat2010Helper.Convert(source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null);
        local = resultIdentifierList;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source3 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) testCaseResults);
        return Compat2010Helper.Convert(source3 != null ? source3.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null);
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
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> webApiExcessIds;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRun((TestManagementRequestContext) this.m_tmRequestContext, testRunId, pageSize, out webApiExcessIds, projectName, false, out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> _, out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> _, out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> _));
        List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> list = source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) webApiExcessIds);
        List<TestCaseResultIdentifier> resultIdentifierList = Compat2010Helper.Convert(source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null);
        local = resultIdentifierList;
        return Compat2010Helper.Convert(list);
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByPoint((TestManagementRequestContext) this.m_tmRequestContext, projectName, planId, pointId));
        return Compat2010Helper.Convert(source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null);
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndOwner((TestManagementRequestContext) this.m_tmRequestContext, testRunId, owner, pageSize, out excessIds1, projectName));
        List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> list = source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> resultIdentifierList = Compat2010Helper.Convert(source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null);
        local = resultIdentifierList;
        return Compat2010Helper.Convert(list);
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndState((TestManagementRequestContext) this.m_tmRequestContext, testRunId, state, pageSize, out excessIds1, projectName));
        List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> list = source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> resultIdentifierList = Compat2010Helper.Convert(source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null);
        local = resultIdentifierList;
        return Compat2010Helper.Convert(list);
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndOutcome((TestManagementRequestContext) this.m_tmRequestContext, testRunId, outcome, pageSize, out excessIds1, projectName));
        List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> list = source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> resultIdentifierList = Compat2010Helper.Convert(source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null);
        local = resultIdentifierList;
        return Compat2010Helper.Convert(list);
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
    public List<TestRun> QueryTestRuns(ResultsStoreQuery query)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRuns), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(Compat2010Helper.Convert(query))));
        return TestRun.FilterNotOfType((IEnumerable<TestRun>) Compat2010Helper.Convert(source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) null), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.QueryTestRunsInMultipleProjects((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(Compat2010Helper.Convert(query))));
        return TestRun.FilterNotOfType((IEnumerable<TestRun>) Compat2010Helper.Convert(source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) null), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
        return this.ResultsHelper.QueryCount((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(Compat2010Helper.Convert(query)));
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, testRunId, Guid.Empty, (string) null, teamProjectName));
        return TestRun.FilterNotOfType((IEnumerable<TestRun>) Compat2010Helper.Convert(source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) null), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, Guid.Empty, buildUri, teamProjectName));
        return TestRun.FilterNotOfType((IEnumerable<TestRun>) Compat2010Helper.Convert(source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) null), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, owner, (string) null, teamProjectName));
        return TestRun.FilterNotOfType((IEnumerable<TestRun>) Compat2010Helper.Convert(source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRun>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) null), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
        return Compat2010Helper.Convert(TestRunContractConverter.Convert(this.ResultsHelper.QueryTestRunByTmiRunId((TestManagementRequestContext) this.m_tmRequestContext, tmiRunId)));
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
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic> source = TestRunStatisticConverter.Convert((IEnumerable<LegacyTestRunStatistic>) this.ResultsHelper.QueryTestRunStats((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId));
        return Compat2010Helper.Convert(source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic>) null);
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
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper resultsHelper = this.ResultsHelper;
        TfsTestManagementRequestContext tmRequestContext = this.m_tmRequestContext;
        IEnumerable<LegacyTestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) Compat2010Helper.Convert(identifiers));
        LegacyTestCaseResultIdentifier[] array = source1 != null ? source1.ToArray<LegacyTestCaseResultIdentifier>() : (LegacyTestCaseResultIdentifier[]) null;
        string projectName1 = projectName;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source2 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) resultsHelper.ResetTestResults((TestManagementRequestContext) tmRequestContext, array, projectName1));
        return Compat2010Helper.Convert(source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>() : (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[]) null);
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
        this.EnterMethod(methodInformation);
        Compat2010Helper.ValidateCompatibleResultOutcome(requests, projectName, this.m_tmRequestContext);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest> source1 = ResultUpdateRequestConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest>) Compat2011QU1Helper.Convert(Compat2010Helper.Convert(requests)));
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse> source2 = ResultUpdateResponseConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>) this.ResultsHelper.Update((TestManagementRequestContext) this.m_tmRequestContext, source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[]) null, projectName));
        return Compat2010Helper.Convert(source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse>() : (Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse[]) null);
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
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun webApiTestRun = TestRunContractConverter.Convert(Compat2010Helper.Convert(testRun));
        Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[] source1 = Compat2010Helper.Convert(attachments);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source2 = AttachmentContractConverter.Convert(source1 != null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) source1).ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>() : (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[]) null);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] array1 = source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[]) null;
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity> source3 = TestResultAttachmentIdentityConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>) Compat2010Helper.Convert(attachmentDeletes));
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] array2 = source3 != null ? source3.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[]) null;
        return Compat2010Helper.Convert(UpdatedPropertiesConverter.Convert(this.ResultsHelper.UpdateTestRun((TestManagementRequestContext) this.m_tmRequestContext, projectName, webApiTestRun, array1, array2, out attachmentIds, true)));
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
        return this.LegacyResultsHelper.CreateLogEntriesForRun(this.m_tmRequestContext, projectName, testRunId, Compat2010Helper.Convert(logEntries));
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
        return Compat2010Helper.Convert(this.LegacyResultsHelper.QueryLogEntriesForRun(this.m_tmRequestContext, projectName, testRunId, testMessageLogId));
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
        return Microsoft.TeamFoundation.TestManagement.Server.TestPoint.QueryAssociatedWorkItemsFromResults((TestManagementRequestContext) this.m_tmRequestContext, pointIds, planId, projectName);
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
        this.ResultsHelper.CreateAssociatedWorkItems((TestManagementRequestContext) this.m_tmRequestContext, TestCaseResultIdentifierConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) Compat2010Helper.Convert(identifiers)), workItemUris);
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
        this.ResultsHelper.DeleteAssociatedWorkItems((TestManagementRequestContext) this.m_tmRequestContext, TestCaseResultIdentifierConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) Compat2010Helper.Convert(identifiers)), workItemUris);
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
        TcmArgumentValidator.CheckNull((object) parent, nameof (parent));
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.CreateEntries((TestManagementRequestContext) this.m_tmRequestContext, new Microsoft.TeamFoundation.TestManagement.Server.IdAndRev(parent.Id, parent.Revision), Compat2010Helper.Convert(testCases), toIndex, projectName, out configurationIds, out configurationNames, TestSuiteSource.Mtm));
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
        TcmArgumentValidator.CheckNull((object) suite, nameof (suite));
        Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties parent1 = Compat2010Helper.Convert(this.CreateUpdateProperties(parent));
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        newSuiteProperties = Compat2010Helper.Convert(Compat2010Helper.Convert(suite).Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ref parent1, toIndex, TestSuiteSource.Mtm));
        return Compat2010Helper.Convert(parent1);
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
        methodInformation.AddParameter(nameof (entries), (object) entries);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        TcmArgumentValidator.CheckNull((object) fromSuiteId, nameof (fromSuiteId));
        TcmArgumentValidator.CheckNull((object) toSuiteId, nameof (toSuiteId));
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        return Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.CopyEntries((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, fromSuiteId.Id, Compat2010Helper.Convert(entries), new Microsoft.TeamFoundation.TestManagement.Server.IdAndRev(toSuiteId.Id, toSuiteId.Revision), toIndex);
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
        methodInformation.AddParameter(nameof (entries), (object) entries);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        TcmArgumentValidator.CheckNull((object) parentSuiteId, nameof (parentSuiteId));
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.DeleteEntries(this.m_tmRequestContext, new Microsoft.TeamFoundation.TestManagement.Server.IdAndRev(parentSuiteId.Id, parentSuiteId.Revision), Compat2010Helper.Convert(entries), projectName));
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
        methodInformation.AddParameter("suitesIds", (object) suiteIds);
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        deletedIds = new List<int>();
        return Compat2010Helper.Convert(Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.FetchWithRepopulate((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, Compat2010Helper.Convert(suiteIds), deletedIds)));
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
        methodInformation.AddParameter(nameof (entries), (object) entries);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties fromSuiteProps = Compat2010Helper.Convert(this.CreateUpdateProperties(fromSuiteId));
        Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties toSuiteProps = Compat2010Helper.Convert(this.CreateUpdateProperties(toSuiteId));
        Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.MoveEntries((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ref fromSuiteProps, Compat2010Helper.Convert(entries), ref toSuiteProps, toIndex);
        newToProps = Compat2010Helper.Convert(toSuiteProps);
        return Compat2010Helper.Convert(fromSuiteProps);
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
        List<Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType> excessIds1 = new List<Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType>();
        List<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite> suites = Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.Query((TestManagementRequestContext) this.m_tmRequestContext, Compat2010Helper.Convert(query), pageSize, excessIds1);
        excessIds = Compat2010Helper.Convert(excessIds1.ToArray());
        return Compat2010Helper.Convert(Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, query.TeamProjectName, suites));
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
        this.EnterMethod(methodInformation);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        List<Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType> excessIds1 = new List<Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType>();
        List<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite> suites = Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.QueryByTestCaseId((TestManagementRequestContext) this.m_tmRequestContext, testCaseId, teamProjectName, pageSize, excessIds1);
        List<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite> list = Compat2013UpdateHelper.ConvertFromWorkItem((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, suites);
        excessIds = Compat2010Helper.Convert(excessIds1.ToArray());
        return Compat2010Helper.Convert(list);
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
        Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.Repopulate((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, suiteId, TestSuiteSource.Mtm);
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
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        TcmArgumentValidator.CheckNull((object) suite, nameof (suite));
        TcmArgumentValidator.CheckNull((object) configIds, nameof (configIds));
        TcmArgumentValidator.CheckNull((object) testCases, nameof (testCases));
        return Compat2010Helper.Convert(Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.SetSuiteEntryConfigurations((TestManagementRequestContext) this.m_tmRequestContext, projectName, new Microsoft.TeamFoundation.TestManagement.Server.IdAndRev(suite.Id, suite.Revision), Compat2010Helper.Convert(testCases), (IEnumerable<int>) configIds));
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
        Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.AssignTestPoints((TestManagementRequestContext) this.m_tmRequestContext, projectName, suiteId, Compat2010Helper.Convert(assignments));
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
        TcmArgumentValidator.CheckNull((object) testSuite, nameof (testSuite));
        Compat2010Helper.ValidateQueryBasedSuite(testSuite, projectName, this.m_tmRequestContext);
        ProcessConfigurationHelper.ValidateProcessConfiguration((TestManagementRequestContext) this.m_tmRequestContext, projectName);
        Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite serverTestSuite = Compat2010Helper.Convert(testSuite);
        serverTestSuite.Status = Compat2013UpdateHelper.ConvertTcmStateToWorkItemState((TestManagementRequestContext) this.m_tmRequestContext, projectName, serverTestSuite.Id, serverTestSuite.State, WitCategoryRefName.TestSuite);
        return Compat2010Helper.Convert(serverTestSuite.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, TestSuiteSource.Mtm, false));
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

    private UpdatedProperties CreateUpdateProperties(IdAndRev idAndRev)
    {
      TcmArgumentValidator.CheckNull((object) idAndRev, nameof (idAndRev));
      return new UpdatedProperties()
      {
        Revision = idAndRev.Revision,
        Id = idAndRev.Id
      };
    }

    [WebMethod]
    [SoapDocumentMethod(Binding = "IProjectMaintenanceBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    [ClientIgnore]
    public bool DeleteProject(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        TeamProject.QueueDelete((TestManagementRequestContext) this.m_tmRequestContext, projectUri);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
      return true;
    }

    [WebMethod]
    [SoapDocumentMethod(Binding = "LinkingService", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03/GetArtifacts")]
    [ClientIgnore]
    public Artifact[] GetArtifacts(string[] artifactUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetArtifacts), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (artifactUris), (IList<string>) artifactUris);
        this.EnterMethod(methodInformation);
        return ArtifactHandler.GetArtifacts((TestManagementRequestContext) this.m_tmRequestContext, artifactUris);
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
  }
}
