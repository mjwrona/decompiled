// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminProcessController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [ValidateInput(false)]
  [SupportedRouteArea("Admin", NavigationContextLevels.Collection | NavigationContextLevels.Project)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  [RequireInheritedCustomizationEnabled]
  public class AdminProcessController : AdminAreaController
  {
    private const int tracepoint = 910050;

    [HttpGet]
    [TfsTraceFilter(910050, 910060)]
    public ActionResult Index() => (ActionResult) this.View("Process", (object) new ProcessViewModel(this.TfsWebContext)
    {
      CanCreateProjects = this.CanCreateProjects
    });

    [HttpGet]
    [TfsTraceFilter(910070, 910080)]
    public ActionResult ChildProcessCreationDialog() => (ActionResult) this.View();

    [HttpGet]
    [TfsTraceFilter(910110, 910120)]
    public ActionResult MigrateProjectsDialog(Guid targetProcessId)
    {
      ArgumentUtility.CheckForEmptyGuid(targetProcessId, nameof (targetProcessId));
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ITeamFoundationProcessService service1 = tfsRequestContext.GetService<ITeamFoundationProcessService>();
      IWorkItemTrackingProcessService service2 = tfsRequestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor1 = service1.GetProcessDescriptor(tfsRequestContext, targetProcessId);
      ProcessDescriptor processDescriptor2 = (ProcessDescriptor) null;
      if (processDescriptor1.IsDerived)
        processDescriptor2 = service1.GetProcessDescriptor(tfsRequestContext, processDescriptor1.Inherits);
      List<ProjectInfo> source = new List<ProjectInfo>();
      Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
      foreach (ProjectProcessDescriptorMapping descriptorMapping in (IEnumerable<ProjectProcessDescriptorMapping>) service2.GetProjectProcessDescriptorMappings(tfsRequestContext, expectUnmappedProjects: true))
      {
        if (descriptorMapping.Descriptor != null)
        {
          if (processDescriptor1.IsSystem && descriptorMapping.Descriptor.Inherits == processDescriptor1.TypeId)
          {
            source.Add(descriptorMapping.Project);
            dictionary[descriptorMapping.Project.Id] = descriptorMapping.Descriptor.Name;
          }
          else if (processDescriptor2 != null && processDescriptor2.TypeId == descriptorMapping.Descriptor.TypeId)
          {
            source.Add(descriptorMapping.Project);
            dictionary[descriptorMapping.Project.Id] = processDescriptor2.Name;
          }
        }
      }
      return (ActionResult) this.View((object) new MigrateProjectsModel()
      {
        ToSystem = processDescriptor1.IsSystem,
        TargetName = processDescriptor1.Name,
        ProjectIdToProcessNameMap = dictionary,
        Projects = source.OrderBy<ProjectInfo, string>((Func<ProjectInfo, string>) (p => p.Name), (IComparer<string>) TFStringComparer.TeamProjectName).ToArray<ProjectInfo>()
      });
    }

    [HttpGet]
    [TfsTraceFilter(910130, 910140)]
    public ActionResult ProcessSuccessDialog(string processName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(processName, nameof (processName));
      return (ActionResult) this.View((object) new ProcessSuccessModel()
      {
        CanCreateProjects = this.CanCreateProjects,
        ProcessName = processName
      });
    }

    [HttpGet]
    [TfsTraceFilter(910210, 910220)]
    public ActionResult ProcessOverview() => (ActionResult) this.View();

    [HttpGet]
    [TfsTraceFilter(910150, 910160)]
    public ActionResult ProcessFields() => (ActionResult) this.View();

    [HttpGet]
    [TfsTraceFilter(910230, 910240)]
    public ActionResult GetMigrateTargetDialogInfo(Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      IDictionary<Guid, string> processIdToNameMap;
      SelectMigrateTargetModel migrateTargetModel = this.GetSelectMigrateTargetModel(projectId, out processIdToNameMap);
      return (ActionResult) this.Json((object) new
      {
        ToSystem = migrateTargetModel.ToSystem,
        SystemProcessName = migrateTargetModel.SystemProcessName,
        ProcessIdToNameMap = processIdToNameMap.OrderBy<KeyValuePair<Guid, string>, string>((Func<KeyValuePair<Guid, string>, string>) (kvp => kvp.Value)).ToArray<KeyValuePair<Guid, string>>()
      }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(910090, 910100)]
    public ActionResult SelectMigrateTargetDialog(Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (ActionResult) this.View((object) this.GetSelectMigrateTargetModel(projectId, out IDictionary<Guid, string> _));
    }

    private SelectMigrateTargetModel GetSelectMigrateTargetModel(
      Guid projectId,
      out IDictionary<Guid, string> processIdToNameMap)
    {
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ITeamFoundationProcessService service = tfsRequestContext.GetService<ITeamFoundationProcessService>();
      ProjectProcessDescriptorMapping descriptorMapping = tfsRequestContext.GetService<IWorkItemTrackingProcessService>().GetProjectProcessDescriptorMapping(tfsRequestContext, projectId);
      ISet<Guid> disabledProcessTypeIds = service.GetDisabledProcessTypeIds(this.TfsRequestContext);
      bool flag = false;
      string str = string.Empty;
      processIdToNameMap = (IDictionary<Guid, string>) new Dictionary<Guid, string>();
      if (descriptorMapping.Descriptor.IsSystem)
      {
        foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) service.GetProcessDescriptors(tfsRequestContext))
        {
          if (processDescriptor.IsDerived && processDescriptor.Inherits == descriptorMapping.Descriptor.TypeId && !disabledProcessTypeIds.Contains(processDescriptor.TypeId))
          {
            flag = true;
            processIdToNameMap[processDescriptor.TypeId] = processDescriptor.Name;
          }
        }
        str = descriptorMapping.Descriptor.Name;
      }
      else if (descriptorMapping.Descriptor.IsDerived)
      {
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(tfsRequestContext, descriptorMapping.Descriptor.Inherits);
        if (!disabledProcessTypeIds.Contains(processDescriptor.TypeId))
        {
          processIdToNameMap[processDescriptor.TypeId] = processDescriptor.Name;
          flag = true;
        }
        str = processDescriptor.Name;
      }
      return new SelectMigrateTargetModel()
      {
        ProjectName = descriptorMapping.Project.Name,
        SystemProcessName = str,
        CurrentProcessName = descriptorMapping.Descriptor.Name,
        CanChangeProcess = flag
      };
    }
  }
}
