// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessImportExportController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "processAdmin", ResourceName = "processes", ResourceVersion = 1)]
  public class ProcessImportExportController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5918500;
    private static readonly MediaTypeHeaderValue ApplicationOctet = new MediaTypeHeaderValue("application/octet-stream");

    public override string TraceArea => "processes";

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      int num = this.TfsRequestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload") ? 1 : 0;
      bool flag1 = this.TfsRequestContext.IsFeatureEnabled("WebAccess.Process.XmlTemplateProcess");
      bool flag2 = this.TfsRequestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy");
      bool flag3 = WorkItemTrackingFeatureFlags.IsProjectChangeProcessEnabled(this.TfsRequestContext);
      if (num == 0 && !flag1 | flag2 && !flag3)
        throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.FeatureNotSupportedForInheritedProcessMessage()));
    }

    [TraceFilter(5918510, 5918520)]
    [RequestContentTypeRestriction(AllowJson = false, AllowJsonPatch = false, AllowStream = true, AllowZip = true)]
    [HttpPost]
    [ClientLocationId("29E1F38D-9E9C-4358-86A5-CDF9896A5759")]
    [ClientResponseType(typeof (ProcessImportResult), "ImportProcessTemplate", null)]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientExample("POST__import_process.json", null, null, null)]
    [ActionName("Import")]
    public HttpResponseMessage Import(bool ignoreWarnings = false, bool replaceExistingTemplate = true)
    {
      this.CheckXmlProcessFeatureFlags();
      Stream result = this.Request.Content.ReadAsStreamAsync().Result;
      MemoryStream memoryStream = new MemoryStream();
      result.CopyTo((Stream) memoryStream);
      CheckTemplateExistenceResult templateExistenceResult = this.CheckTemplateExistenceInternal((Stream) memoryStream);
      if (templateExistenceResult.DoesTemplateExist && !replaceExistingTemplate)
        return this.Request.CreateResponse<ProcessImportResult>(HttpStatusCode.PreconditionFailed, new ProcessImportResult()
        {
          Id = templateExistenceResult.ExistingTemplateTypeId,
          ValidationResults = Enumerable.Empty<ValidationIssue>(),
          CheckExistenceResult = templateExistenceResult
        });
      ProcessUpdateResultModel updateResultModel = this.TfsRequestContext.GetService<IProcessAdminService>().UpdateProcess(this.TfsRequestContext, result, ignoreWarnings);
      ValidationIssue[] array1 = updateResultModel.ProcessTemplateValidatorResult.Errors.Select<ProcessTemplateValidatorMessage, ValidationIssue>((Func<ProcessTemplateValidatorMessage, ValidationIssue>) (msg => new ValidationIssue()
      {
        Description = msg.Message,
        File = msg.File,
        Line = msg.LineNumber,
        IssueType = ValidationIssueType.Error,
        HelpLink = msg.HelpLink
      })).ToArray<ValidationIssue>();
      ValidationIssue[] array2 = updateResultModel.ProcessTemplateValidatorResult.ConfirmationsNeeded.Select<ProcessTemplateValidatorMessage, ValidationIssue>((Func<ProcessTemplateValidatorMessage, ValidationIssue>) (msg => new ValidationIssue()
      {
        Description = msg.Message,
        File = msg.File,
        Line = msg.LineNumber,
        IssueType = ValidationIssueType.Warning,
        HelpLink = msg.HelpLink
      })).ToArray<ValidationIssue>();
      ProcessDescriptor descriptor;
      if (this.TfsRequestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(this.TfsRequestContext, updateResultModel.TemplateTypeId, out descriptor))
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("Process", nameof (Import));
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ProcessName", (object) descriptor?.Name);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      IEnumerable<ValidationIssue> source = ignoreWarnings ? (IEnumerable<ValidationIssue>) array1 : ((IEnumerable<ValidationIssue>) array1).Concat<ValidationIssue>((IEnumerable<ValidationIssue>) array2);
      if (!source.Any<ValidationIssue>())
        return this.Request.CreateResponse<ProcessImportResult>(HttpStatusCode.Accepted, new ProcessImportResult()
        {
          Id = updateResultModel.TemplateTypeId,
          PromoteJobId = updateResultModel.PromoteJobId,
          ValidationResults = source,
          CheckExistenceResult = templateExistenceResult
        });
      return this.Request.CreateResponse<ProcessImportResult>(HttpStatusCode.PreconditionFailed, new ProcessImportResult()
      {
        Id = updateResultModel.TemplateTypeId,
        HelpUrl = Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ProcessImportExportController_ErrorsLink(),
        ValidationResults = source,
        CheckExistenceResult = templateExistenceResult
      });
    }

    [TraceFilter(5918530, 5918540)]
    [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
    [RequestContentTypeRestriction(AllowJson = false, AllowJsonPatch = false, AllowStream = true, AllowZip = true)]
    [HttpPost]
    [ClientLocationId("29E1F38D-9E9C-4358-86A5-CDF9896A5759")]
    [ClientResponseType(typeof (CheckTemplateExistenceResult), "CheckTemplateExistence", null)]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ActionName("CheckTemplateExistence")]
    public HttpResponseMessage CheckTemplateExistence()
    {
      this.CheckXmlProcessFeatureFlags();
      return this.Request.CreateResponse<CheckTemplateExistenceResult>(HttpStatusCode.OK, this.CheckTemplateExistenceInternal(this.Request.Content.ReadAsStreamAsync().Result));
    }

    private CheckTemplateExistenceResult CheckTemplateExistenceInternal(Stream processInput)
    {
      string templateName;
      Guid templateTypeId;
      ProcessAdminService.ExtractNameAndType(processInput, out templateName, out templateTypeId);
      if (templateTypeId == Guid.Empty)
        throw new InvalidWorkItemTemplateIdException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ProcessTemplateIdIsNotValid());
      bool flag = false;
      string str = string.Empty;
      IEnumerable<ProcessDescriptor> source = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(this.TfsRequestContext).Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase) || template.TypeId.Equals(templateTypeId)));
      if (source.Any<ProcessDescriptor>())
      {
        if (source.Any<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.Scope == ProcessScope.Deployment)))
          throw new ArgumentException(this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ProcessUpdateBlockedSystemTemplate() : Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ProcessUpdateBlockedSystemTemplateOnPrem());
        if (source.Any<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase) && !template.TypeId.Equals(templateTypeId))))
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ProcessTemplateNameConflict((object) templateName));
        str = source.Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.TypeId.Equals(templateTypeId))).FirstOrDefault<ProcessDescriptor>()?.Name;
        flag = true;
      }
      return new CheckTemplateExistenceResult()
      {
        DoesTemplateExist = flag,
        RequestedTemplateName = templateName,
        ExistingTemplateName = str,
        ExistingTemplateTypeId = templateTypeId
      };
    }

    [TraceFilter(5918500, 5918510)]
    [ClientLocationId("29E1F38D-9E9C-4358-86A5-CDF9896A5759")]
    [ClientResponseType(typeof (Stream), "ExportProcessTemplate", "application/zip")]
    [ActionName("Export")]
    [ClientExample("GET__export_template.json", null, null, null)]
    [HttpGet]
    public HttpResponseMessage Export([FromUri] Guid id)
    {
      this.CheckXmlProcessFeatureFlags();
      Guid guid = id;
      ArgumentUtility.CheckForEmptyGuid(guid, "processTemplateId");
      ProcessDescriptor descriptor;
      if (!this.TfsRequestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(this.TfsRequestContext, guid, out descriptor))
        return this.Request.CreateResponse<ProcessExportResult>(HttpStatusCode.NotFound, new ProcessExportResult()
        {
          Id = guid
        });
      string str = descriptor.Name;
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        str = str.Replace(invalidFileNameChar.ToString(), "_");
      if (string.IsNullOrWhiteSpace(str))
        str = descriptor.RowId.ToString();
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(ProcessPackageFormTransformer.GetProcessPackageContentWithTransformedWebLayout(this.TfsRequestContext, descriptor));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(str + ".zip");
      return response;
    }

    [TraceFilter(5918520, 5918530)]
    [ClientLocationId("29E1F38D-9E9C-4358-86A5-CDF9896A5759")]
    [ClientResponseType(typeof (ProcessPromoteStatus), "ImportProcessTemplateStatus", null)]
    [ActionName("Status")]
    [ClientExample("GET__import_process_status_template.json", null, null, null)]
    [HttpGet]
    public HttpResponseMessage Status([FromUri] Guid id)
    {
      Guid promoteJobId = id;
      ProcessUpdateProgressModel updateProgressModel = this.TfsRequestContext.GetService<IProcessAdminService>().MonitorUpdateProgress(this.TfsRequestContext, promoteJobId);
      if (updateProgressModel == null)
        return this.Request.CreateResponse<ProcessPromoteStatus>(HttpStatusCode.NotFound, new ProcessPromoteStatus()
        {
          Id = promoteJobId
        });
      string str = (string) null;
      if (updateProgressModel.FailedProject != null)
        str = Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ProcessImportExportController_Status_FailedProject((object) updateProgressModel.FailedProject.Id, (object) updateProgressModel.FailedProject.CompletedSteps);
      return this.Request.CreateResponse<ProcessPromoteStatus>(HttpStatusCode.OK, new ProcessPromoteStatus()
      {
        Id = promoteJobId,
        Complete = updateProgressModel.Complete,
        Pending = updateProgressModel.Remaining,
        RemainingRetries = updateProgressModel.RemainingRetries,
        Successful = updateProgressModel.IsSuccessful,
        Message = str
      });
    }

    [TraceFilter(5918530, 5918540)]
    [ClientLocationId("29E1F38D-9E9C-4358-86A5-CDF9896A5759")]
    [ClientResponseType(typeof (Guid), "CloneXmlToInherited", null)]
    [ActionName("CloneXmlToInherited")]
    [HttpPost]
    [FeatureEnabled("WebAccess.Process.ProcessUpload")]
    [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage CloneXmlToInherited(
      Guid SourceProcessId,
      string TargetProcessName,
      string ParentProcessName,
      string processDescription)
    {
      ArgumentUtility.CheckForEmptyGuid(SourceProcessId, nameof (SourceProcessId));
      ArgumentUtility.CheckStringForNullOrEmpty(TargetProcessName, nameof (TargetProcessName));
      ArgumentUtility.CheckStringForNullOrEmpty(TargetProcessName, nameof (ParentProcessName));
      this.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>();
      IWorkItemTrackingProcessService workItemProcessService = this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>();
      IEnumerable<Project> source = this.TfsRequestContext.GetService<WebAccessWorkItemService>().GetProjects(this.TfsRequestContext).Where<Project>((Func<Project, bool>) (proj => workItemProcessService.GetProjectProcessDescriptor(this.TfsRequestContext, proj.Guid).TypeId.Equals(SourceProcessId)));
      Dictionary<string, Dictionary<string, string>> workItemTypeNameToStateNameToStateColors = (Dictionary<string, Dictionary<string, string>>) null;
      if (source != null && source.Any<Project>())
        workItemTypeNameToStateNameToStateColors = this.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStateColors(this.TfsRequestContext, source.First<Project>().Guid).ToDedupedDictionary<KeyValuePair<string, IReadOnlyCollection<WorkItemStateColor>>, string, Dictionary<string, string>>((Func<KeyValuePair<string, IReadOnlyCollection<WorkItemStateColor>>, string>) (dictEntry => dictEntry.Key), (Func<KeyValuePair<string, IReadOnlyCollection<WorkItemStateColor>>, Dictionary<string, string>>) (dictEntry => dictEntry.Value.ToDedupedDictionary<WorkItemStateColor, string, string>((Func<WorkItemStateColor, string>) (witStateColor => witStateColor.Name), (Func<WorkItemStateColor, string>) (witStateColor => witStateColor.Color))));
      Guid inherited = workItemProcessService.CloneHostedXmlProcessToInherited(this.TfsRequestContext, SourceProcessId, TargetProcessName, this.GetSystemProcessTemplateTypeGuidByName(ParentProcessName), processDescription, workItemTypeNameToStateNameToStateColors);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Process", nameof (CloneXmlToInherited));
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add(nameof (TargetProcessName), (object) TargetProcessName);
      data.Add(nameof (ParentProcessName), (object) ParentProcessName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<Guid>(HttpStatusCode.OK, inherited);
    }

    [TraceFilter(5918530, 5918540)]
    [ClientLocationId("29E1F38D-9E9C-4358-86A5-CDF9896A5759")]
    [ClientResponseType(typeof (Guid), "QueuePromoteProjectToProcessJob", null)]
    [ActionName("QueuePromoteProjectToProcessJob")]
    [HttpPost]
    [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
    public HttpResponseMessage QueuePromoteProjectToProcessJob(
      string projectName,
      Guid targetProcessId)
    {
      ArgumentUtility.CheckForEmptyGuid(targetProcessId, nameof (targetProcessId));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ITeamFoundationProcessService service = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor processDescriptor = service.GetProcessDescriptor(this.TfsRequestContext, targetProcessId, bypassCache: true);
      ProjectInfo project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, projectName);
      if (processDescriptor.IsCustom)
        throw new FeatureDisabledException(FrameworkResources.FeatureDisabledError());
      if (!this.TfsRequestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload") && !WorkItemTrackingFeatureFlags.IsProjectChangeProcessEnabled(this.TfsRequestContext))
        throw new FeatureDisabledException(FrameworkResources.FeatureDisabledError());
      if (!this.HasPermissionToChangeProcessOfProject(this.TfsRequestContext, project))
        throw new ProjectProcessChangeAccessDeniedException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InsufficientPermissionToChangeProcessOfProjectExceptionMessage((object) projectName));
      this.RecordTelemetryForPromoteRequest(projectName, targetProcessId, service, processDescriptor, project);
      this.TfsRequestContext.GetService<TeamFoundationProjectPromoteService>().QueuePromoteJob(this.TfsRequestContext, processDescriptor.TypeId, true, new Guid?(project.Id));
      ProcessDescriptor sourceProcess = this.GetSourceProcess(service, project);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Process", "MigrateXmlToInherited");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProjectName", (object) projectName);
      data.Add("OldProcess", (object) sourceProcess?.Name);
      data.Add("NewProcess", (object) processDescriptor.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<Guid>(HttpStatusCode.OK, processDescriptor.TypeId);
    }

    private bool HasPermissionToChangeProcessOfProject(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      string token = projectInfo != null ? TeamProjectSecurityConstants.GetToken(projectInfo.Uri) : throw new ArgumentNullException(nameof (projectInfo));
      return this.HasPermission(requestContext, FrameworkSecurity.TeamProjectNamespaceId, token, TeamProjectPermissions.ChangeProjectsProcess);
    }

    private bool HasPermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int permission)
    {
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId).HasPermission(requestContext, token, permission);
    }

    private void RecordTelemetryForPromoteRequest(
      string projectName,
      Guid targetProcessId,
      ITeamFoundationProcessService processService,
      ProcessDescriptor targetProcess,
      ProjectInfo pInfo)
    {
      try
      {
        ProcessDescriptor sourceProcess = this.GetSourceProcess(processService, pInfo);
        CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add(nameof (projectName), projectName);
        intelligenceData.Add("projectId", pInfo.Id.ToString());
        intelligenceData.Add("sourceProcess.Id", (object) sourceProcess?.TypeId);
        intelligenceData.Add("sourceProcess.Name", sourceProcess?.Name);
        intelligenceData.Add("sourceProcess.Inherits", (object) sourceProcess?.Inherits);
        intelligenceData.Add("sourceProcess.IsSystem", (object) sourceProcess?.IsSystem);
        intelligenceData.Add("sourceProcess.IsDerived", (object) sourceProcess?.IsDerived);
        intelligenceData.Add("sourceProcess.IsCustom", (object) sourceProcess?.IsCustom);
        intelligenceData.Add("targetProcess.Id", (object) targetProcessId);
        intelligenceData.Add("targetProcess.Name", targetProcess?.Name);
        intelligenceData.Add("targetProcess.Inherits", (object) targetProcess?.Inherits);
        intelligenceData.Add("targetProcess.IsSystem", (object) targetProcess?.IsSystem);
        intelligenceData.Add("targetProcess.IsDerived", (object) targetProcess?.IsDerived);
        intelligenceData.Add("targetProcess.IsCustom", (object) targetProcess?.IsCustom);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(tfsRequestContext, nameof (ProcessImportExportController), "QueuePromoteProjectToProcessJob", properties);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(902201, nameof (ProcessImportExportController), nameof (RecordTelemetryForPromoteRequest), ex);
      }
    }

    private Guid GetSystemProcessTemplateTypeGuidByName(string templateType)
    {
      if (string.Equals(templateType, "Agile", StringComparison.OrdinalIgnoreCase))
        return OutOfBoxProcessTemplateIds.Agile;
      if (string.Equals(templateType, "Scrum", StringComparison.OrdinalIgnoreCase))
        return OutOfBoxProcessTemplateIds.Scrum;
      if (string.Equals(templateType, "CMMI", StringComparison.OrdinalIgnoreCase))
        return OutOfBoxProcessTemplateIds.Cmmi;
      return string.Equals(templateType, "Basic", StringComparison.OrdinalIgnoreCase) ? OutOfBoxProcessTemplateIds.Basic : Guid.Empty;
    }

    private void CheckXmlProcessFeatureFlags()
    {
      int num = this.TfsRequestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload") ? 1 : 0;
      bool flag = this.TfsRequestContext.IsFeatureEnabled("WebAccess.Process.XmlTemplateProcess");
      if (num == 0 && !flag)
        throw new FeatureDisabledException(FrameworkResources.FeatureDisabledError());
    }

    private ProcessDescriptor GetSourceProcess(
      ITeamFoundationProcessService processService,
      ProjectInfo pInfo)
    {
      pInfo.PopulateProperties(this.TfsRequestContext, ProcessTemplateIdPropertyNames.ProcessTemplateType);
      IList<ProjectProperty> properties = pInfo.Properties;
      string input = properties != null ? (string) properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => StringComparer.OrdinalIgnoreCase.Equals(p.Name, ProcessTemplateIdPropertyNames.ProcessTemplateType)))?.Value : (string) (object) null;
      ProcessDescriptor sourceProcess = (ProcessDescriptor) null;
      Guid result;
      if (input != null && Guid.TryParse(input, out result))
        sourceProcess = processService.GetProcessDescriptor(this.TfsRequestContext, result, bypassCache: true);
      return sourceProcess;
    }
  }
}
