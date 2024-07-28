// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ExportToHtmlSendMailJob
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using System;
using System.Web.Mvc.Async;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class ExportToHtmlSendMailJob : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      ExportToHtmSendMailJobDataModel mailJobDataModel = TeamFoundationSerializationUtility.Deserialize<ExportToHtmSendMailJobDataModel>(jobDefinition.Data);
      TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      TestManagerRequestContext managerRequestContext = (TestManagerRequestContext) null;
      try
      {
        WebApiTeam teamByGuid = requestContext.GetService<ITeamService>().GetTeamByGuid(requestContext, mailJobDataModel.TeamId);
        managerRequestContext = new TestManagerRequestContext(requestContext, mailJobDataModel.ProjectName, mailJobDataModel.CurrentProjectGuid, teamByGuid);
        managerRequestContext.TraceVerbose("JobLayer", "Generating report with following data: PlanId:{0} ,suiteId:{1}, paramtersXml: {2}, dialogSettings:{3}", (object) mailJobDataModel.PlanId, (object) mailJobDataModel.SuiteId, (object) mailJobDataModel.ParametersXml, (object) string.Join(",", mailJobDataModel.DialogSettings.ToArray()));
        string empty = string.Empty;
        string htmlReport = new HtmlDocumentGenerator(managerRequestContext, mailJobDataModel.PlanId, mailJobDataModel.SuiteId, mailJobDataModel.ParametersXml, mailJobDataModel.XsltFileText, mailJobDataModel.DialogSettings).GetHtmlReport();
        string str = empty + mailJobDataModel.NotesText + htmlReport;
        mailJobDataModel.Message.Body = str;
        resultMessage = string.Empty;
        MailSender.BeginSendMail(mailJobDataModel.Message, requestContext, requestContext.IsHosted(), new AsyncManager());
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case MailValidationException _:
          case TeamFoundationServiceException _:
            mailJobDataModel.Message.Body = string.Format(TestManagementServerResources.ExportToHtmlReportSendEmailFailureMessage, (object) ex.Message);
            break;
          default:
            mailJobDataModel.Message.Body = string.Format(TestManagementServerResources.ExportToHtmlReportGenerationFailureMessage, (object) jobDefinition.JobId);
            break;
        }
        mailJobDataModel.Message.Subject = string.Format(TestManagementServerResources.ExportToHtmlReportGenerationFailureSubjectMessage, (object) mailJobDataModel.Message.Subject);
        resultMessage = ex.ToString();
        jobExecutionResult = TeamFoundationJobExecutionResult.Failed;
        if (managerRequestContext != null)
          managerRequestContext.TraceError("JobLayer", "Error generating html report with following data: PlanId:{0} ,suiteId:{1}, paramtersXml: {2}, dialogSettings:{3}. Exception :{4}", (object) mailJobDataModel.PlanId, (object) mailJobDataModel.SuiteId, (object) mailJobDataModel.ParametersXml, (object) string.Join(",", mailJobDataModel.DialogSettings.ToArray()), (object) ex.ToString());
        MailSender.BeginSendMail(mailJobDataModel.Message, requestContext, requestContext.IsHosted(), new AsyncManager());
      }
      return jobExecutionResult;
    }
  }
}
