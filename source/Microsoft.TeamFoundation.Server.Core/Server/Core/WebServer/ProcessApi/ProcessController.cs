// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServer.ProcessApi.ProcessController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.WebServer.ProcessApi
{
  [VersionedApiControllerCustomName("core", "processes", 1)]
  public class ProcessController : ServerCoreApiController
  {
    [HttpGet]
    [TraceFilter(10005021, 10005022)]
    public IEnumerable<Process> GetProcesses()
    {
      ITeamFoundationProcessService service = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
      IReadOnlyCollection<ProcessDescriptor> processDescriptors = service.GetProcessDescriptors(this.TfsRequestContext);
      Guid defaultProcessId = service.GetDefaultProcessTypeId(this.TfsRequestContext);
      Func<ProcessDescriptor, Process> selector = (Func<ProcessDescriptor, Process>) (descriptor => ProcessController.CreateProcess(this.TfsRequestContext, descriptor, descriptor.TypeId == defaultProcessId));
      return processDescriptors.Select<ProcessDescriptor, Process>(selector);
    }

    [HttpGet]
    [TraceFilter(10005023, 10005024)]
    public Process GetProcessById(Guid processId)
    {
      ITeamFoundationProcessService service = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor processDescriptor = service.GetProcessDescriptor(this.TfsRequestContext, processId);
      Guid defaultProcessTypeId = service.GetDefaultProcessTypeId(this.TfsRequestContext);
      return ProcessController.CreateProcess(this.TfsRequestContext, processDescriptor, processDescriptor.TypeId == defaultProcessTypeId, true);
    }

    private static Process CreateProcess(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor,
      bool isDefault,
      bool includeLinks = false)
    {
      string resourceUrlString = ProcessUrlHelper.GetProjectResourceUrlString(requestContext, descriptor.TypeId);
      ProcessType processType = ProcessType.System;
      if (!descriptor.IsSystem)
        processType = descriptor.IsDerived ? ProcessType.Inherited : ProcessType.Custom;
      Process process1 = new Process();
      process1.Description = descriptor.Description;
      process1.Id = descriptor.TypeId;
      process1.IsDefault = isDefault;
      process1.Name = descriptor.Name;
      process1.Url = resourceUrlString;
      process1.Type = processType;
      Process process2 = process1;
      if (includeLinks)
      {
        ReferenceLinks referenceLinks = new ReferenceLinks();
        referenceLinks.AddLink("self", resourceUrlString);
        process2.Links = referenceLinks;
      }
      return process2;
    }
  }
}
