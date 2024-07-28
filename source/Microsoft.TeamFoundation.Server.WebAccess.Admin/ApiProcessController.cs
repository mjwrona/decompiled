// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiProcessController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Serializers;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.Collection | NavigationContextLevels.Project)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class ApiProcessController : AdminAreaController
  {
    [HttpPost]
    [TfsHandleFeatureFlag("WebAccess.Process.ProcessUpload", null)]
    [TfsTraceFilter(210620, 210630)]
    public ActionResult ImportProcessTemplate(
      string callback,
      HttpPostedFileBase templateZipFile,
      bool byPassWarnings = false)
    {
      CommonUtility.CheckCallbackName(callback);
      object obj;
      try
      {
        ArgumentUtility.CheckForNull<HttpPostedFileBase>(templateZipFile, nameof (templateZipFile));
        ProcessUpdateResultModel updateResultModel = this.TfsRequestContext.GetService<IProcessAdminService>().UpdateProcess(this.TfsRequestContext, templateZipFile.InputStream, byPassWarnings);
        bool flag = !updateResultModel.ProcessTemplateValidatorResult.Errors.Any<ProcessTemplateValidatorMessage>() && (!updateResultModel.ProcessTemplateValidatorResult.ConfirmationsNeeded.Any<ProcessTemplateValidatorMessage>() || byPassWarnings);
        obj = flag ? (object) new
        {
          success = flag,
          promoteJobId = updateResultModel.PromoteJobId,
          isNew = updateResultModel.PromoteJobId.Equals(Guid.Empty)
        } : (object) new
        {
          validationFailed = !flag,
          validationResult = this.Json((object) updateResultModel.ProcessTemplateValidatorResult)
        };
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(210621, "AdminProcess", "Process", ex);
        obj = (object) new
        {
          success = false,
          error = ex.ToJson()
        };
      }
      if (string.IsNullOrWhiteSpace(callback))
        return (ActionResult) new EmptyResult();
      string empty = string.Empty;
      string str;
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
      {
        str = "var uploadResult = window.parent[" + JsonConvert.SerializeObject((object) callback) + "];if(uploadResult && uploadResult.isCallback === true && typeof (uploadResult.callback) === 'function'){uploadResult.callback(" + JsonConvert.SerializeObject(obj) + ");}";
      }
      else
      {
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        str = "var uploadResult = window.parent[" + scriptSerializer.Serialize((object) callback) + "];if(uploadResult && uploadResult.isCallback === true && typeof (uploadResult.callback) === 'function'){uploadResult.callback(" + scriptSerializer.Serialize(obj) + ");}";
      }
      return (ActionResult) this.Content("<html><body><script type='text/javascript'>" + str + "</script><body></html>", "text/html");
    }

    [HttpGet]
    [TfsTraceFilter(210630, 210640)]
    public ActionResult GetProcessTemplate(Guid templateTypeId)
    {
      ArgumentUtility.CheckForEmptyGuid(templateTypeId, nameof (templateTypeId));
      this.TfsRequestContext.GetService<IProcessAdminService>();
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, templateTypeId);
      FileStreamResult processTemplate = new FileStreamResult(ProcessPackageFormTransformer.GetProcessPackageContentWithTransformedWebLayout(this.TfsRequestContext, processDescriptor), "application/octet-stream");
      processTemplate.FileDownloadName = processDescriptor.Name + ".zip";
      return (ActionResult) processTemplate;
    }

    [HttpPost]
    [TfsTraceFilter(210640, 210650)]
    public ActionResult DeleteProcessTemplate(Guid templateTypeId)
    {
      ArgumentUtility.CheckForEmptyGuid(templateTypeId, nameof (templateTypeId));
      this.TfsRequestContext.GetService<IProcessAdminService>().DeleteProcess(this.TfsRequestContext, templateTypeId);
      return (ActionResult) new EmptyResult();
    }

    [HttpPost]
    [TfsTraceFilter(210650, 210660)]
    public ActionResult SetDefaultProcessTemplate(Guid templateTypeId)
    {
      ArgumentUtility.CheckForEmptyGuid(templateTypeId, nameof (templateTypeId));
      this.TfsRequestContext.GetService<ITeamFoundationProcessService>().SetProcessAsDefault(this.TfsRequestContext, templateTypeId);
      return (ActionResult) new EmptyResult();
    }

    [HttpGet]
    [TfsTraceFilter(210660, 210670)]
    public ActionResult GetProcesses()
    {
      IProcessAdminService service = this.TfsRequestContext.GetService<IProcessAdminService>();
      return (ActionResult) this.Json((object) new ProcessModel()
      {
        Templates = service.GetProcesses(this.TfsRequestContext)
      }.ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [TfsHandleFeatureFlag("WebAccess.Process.ProcessUpload", null)]
    [TfsTraceFilter(210670, 210680)]
    public ActionResult ProcessTemplateDescription(
      string callback,
      HttpPostedFileBase templateZipFile)
    {
      CommonUtility.CheckCallbackName(callback);
      object obj;
      try
      {
        ArgumentUtility.CheckForNull<HttpPostedFileBase>(templateZipFile, nameof (templateZipFile));
        this.TfsRequestContext.GetService<IProcessTemplateValidatorService>().ValidateTemplateFileSizeLimit(this.TfsRequestContext, templateZipFile.InputStream);
        ITeamFoundationProcessService service = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
        this.TfsRequestContext.GetService<IProcessAdminService>();
        string templateName;
        Guid templateTypeId;
        ProcessAdminService.ExtractNameAndType(templateZipFile.InputStream, out templateName, out templateTypeId);
        if (!string.IsNullOrEmpty(templateName) && templateTypeId != Guid.Empty)
        {
          bool flag = false;
          string str = string.Empty;
          IEnumerable<ProcessDescriptor> source = service.GetProcessDescriptors(this.TfsRequestContext).Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase) || template.TypeId.Equals(templateTypeId)));
          if (source.Any<ProcessDescriptor>())
          {
            if (source.Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.Scope == ProcessScope.Deployment)).Any<ProcessDescriptor>())
              throw new ArgumentException(this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ProcessUpdateBlockedSystemTemplate() : Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ProcessUpdateBlockedSystemTemplateOnPrem());
            if (source.Any<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase) && !template.TypeId.Equals(templateTypeId))))
              throw new ProcessNameConflictException(templateName);
            ProcessDescriptor processDescriptor = source.Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (template => template.TypeId.Equals(templateTypeId))).FirstOrDefault<ProcessDescriptor>();
            if (processDescriptor != null && !processDescriptor.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase))
              str = processDescriptor.Name;
            flag = true;
          }
          obj = (object) new
          {
            extractSuccess = true,
            isTemplateExist = flag,
            TemplateName = templateName,
            ExistingTemplateName = str,
            TemplateTypeId = templateTypeId
          };
        }
        else
        {
          if (templateTypeId.Equals(Guid.Empty))
            throw new ArgumentException(string.Format(AdminServerResources.ImportProcessParameterNull, (object) "process type Id"));
          throw new ArgumentException(string.Format(AdminServerResources.ImportProcessParameterNull, (object) "process name"));
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(210671, "AdminProcess", "Process", ex);
        obj = (object) new
        {
          success = false,
          error = ex.ToJson()
        };
      }
      if (string.IsNullOrWhiteSpace(callback))
        return (ActionResult) new EmptyResult();
      string str1;
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
      {
        str1 = "var uploadResult = window.parent[" + JsonConvert.SerializeObject((object) callback) + "];if(uploadResult && uploadResult.isCallback === true && typeof (uploadResult.callback) === 'function'){uploadResult.callback(" + JsonConvert.SerializeObject(obj) + ");}";
      }
      else
      {
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        str1 = "var uploadResult = window.parent[" + scriptSerializer.Serialize((object) callback) + "];if(uploadResult && uploadResult.isCallback === true && typeof (uploadResult.callback) === 'function'){uploadResult.callback(" + scriptSerializer.Serialize(obj) + ");}";
      }
      return (ActionResult) this.Content("<html><body><script type='text/javascript'>" + str1 + "</script><body></html>", "text/html");
    }

    [HttpGet]
    [TfsTraceFilter(210680, 210690)]
    public ActionResult GetJobProgress(Guid jobId) => (ActionResult) this.Json(AdminDataSource.GetJobProgress(this.TfsRequestContext, jobId), JsonRequestBehavior.AllowGet);

    [HttpPost]
    [TfsHandleFeatureFlag("WebAccess.Process.Hierarchy", null)]
    [TfsTraceFilter(210690, 210700)]
    public ActionResult AddInheritedProcess(
      string name,
      string referenceName,
      string description,
      Guid parentTypeId)
    {
      this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>().CreateInheritedProcess(this.TfsRequestContext, name, referenceName, description, parentTypeId);
      return (ActionResult) new EmptyResult();
    }

    [HttpPost]
    [TfsHandleFeatureFlag("WebAccess.Process.Hierarchy", null)]
    [TfsTraceFilter(210750, 210760)]
    public ActionResult SetProcessIsEnabled(Guid templateTypeId, bool isEnabled)
    {
      ArgumentUtility.CheckForEmptyGuid(templateTypeId, nameof (templateTypeId));
      this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>().EnableDisableProcess(this.TfsRequestContext, templateTypeId, isEnabled);
      return (ActionResult) new EmptyResult();
    }

    [HttpPost]
    [TfsHandleFeatureFlag("WebAccess.Process.Hierarchy", null)]
    [TfsTraceFilter(210700, 210710)]
    public ActionResult UpdateProcessInfo(Guid templateTypeId, string name, string description)
    {
      ITeamFoundationProcessService service1 = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
      IWorkItemTrackingProcessService service2 = this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>();
      if (service1 != null && ApiProcessController.IsUpdateNeeded(service1.GetProcessDescriptor(this.TfsRequestContext, templateTypeId), name, description))
        service2.UpdateProcessNameAndDescription(this.TfsRequestContext, templateTypeId, name, description);
      return (ActionResult) new EmptyResult();
    }

    internal static bool IsUpdateNeeded(
      ProcessDescriptor processTemplateDescriptor,
      string name,
      string description)
    {
      return processTemplateDescriptor.Scope != ProcessScope.Deployment && (!string.IsNullOrEmpty(name) || description != null);
    }

    [HttpPost]
    [TfsHandleFeatureFlag("WebAccess.Process.Hierarchy", null)]
    [TfsTraceFilter(210710, 210720)]
    public ActionResult MigrateProjectsProcess(
      [ModelBinder(typeof (ProcessMigrationModelBinder))] IEnumerable<ProcessMigrationModel> migratingProjects)
    {
      return (ActionResult) this.Json((object) BoardsAdminDataSource.MigrateProjectsProcess(this.TfsRequestContext, migratingProjects), JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsHandleFeatureFlag("WebAccess.Process.Hierarchy", null)]
    [TfsTraceFilter(210730, 210740)]
    public ActionResult GetProcessFieldUsages(Guid processTypeId) => (ActionResult) this.Json((object) AdminDataSource.GetProcessFieldUsages(this.TfsRequestContext, processTypeId), JsonRequestBehavior.AllowGet);

    [HttpGet]
    [TfsHandleFeatureFlag("WebAccess.Process.Hierarchy", null)]
    [OutputCache(CacheProfile = "NoCache")]
    [TfsTraceFilter(210740, 2107450)]
    public ActionResult GetProcess(string projectName)
    {
      ArgumentUtility.CheckForNull<string>(projectName, "projectId");
      IProjectService service1 = this.TfsRequestContext.GetService<IProjectService>();
      Guid processTypeId = new Guid((string) service1.GetProjectProperties(this.TfsRequestContext, service1.GetProjectId(this.TfsRequestContext, projectName), ProcessTemplateIdPropertyNames.ProcessTemplateType).First<ProjectProperty>().Value);
      ITeamFoundationProcessService service2 = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor processDescriptor = service2.GetProcessDescriptor(this.TfsRequestContext, processTypeId);
      ISet<Guid> disabledProcessTypeIds = service2.GetDisabledProcessTypeIds(this.TfsRequestContext);
      Guid defaultProcessTypeId = service2.GetDefaultProcessTypeId(this.TfsRequestContext);
      return (ActionResult) this.Json((object) new ProcessDescriptorModel()
      {
        Id = processDescriptor.RowId,
        TemplateTypeId = processDescriptor.TypeId,
        Name = processDescriptor.Name,
        ReferenceName = processDescriptor.ReferenceName,
        Version = string.Format("{0}.{1}", (object) processDescriptor.Version.Major, (object) processDescriptor.Version.Minor),
        Description = processDescriptor.Description,
        IsDefault = (processDescriptor.TypeId == defaultProcessTypeId),
        IsEnabled = !disabledProcessTypeIds.Contains(processDescriptor.TypeId),
        IsSystemTemplate = (processDescriptor.Scope == ProcessScope.Deployment),
        IsInherited = processDescriptor.IsDerived,
        Status = processDescriptor.ProcessStatus,
        Inherits = processDescriptor.Inherits
      }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult GetPermissions(Guid templateTypeId)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      string str = (string) null;
      try
      {
        ITeamFoundationProcessService service = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(this.TfsRequestContext, templateTypeId);
        flag1 = service.HasProcessPermission(this.TfsRequestContext, 1, processDescriptor);
        flag2 = service.HasProcessPermission(this.TfsRequestContext, 2, processDescriptor);
        flag3 = service.HasProcessPermission(this.TfsRequestContext, 4, processDescriptor);
      }
      catch (Exception ex)
      {
        str = ex.Message;
        TeamFoundationTrace.TraceException(ex);
      }
      return (ActionResult) this.Json((object) new
      {
        editPermission = flag1,
        deletePermission = flag2,
        createPermission = flag3,
        errorMessage = str
      }, JsonRequestBehavior.AllowGet);
    }
  }
}
