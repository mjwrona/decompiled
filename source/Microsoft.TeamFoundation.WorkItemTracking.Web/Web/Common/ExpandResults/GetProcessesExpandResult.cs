// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults.GetProcessesExpandResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults
{
  public class GetProcessesExpandResult
  {
    private readonly ILookup<Guid, ProjectInfo> subscribedProjectsLookup;
    private readonly IDictionary<Guid, ProcessDescriptor> processDescriptors;
    private readonly Guid defaultProcessTypeId;
    private readonly ISet<Guid> disabledProcessTypeIds;

    public GetProcessesExpandResult(
      IVssRequestContext requestContext,
      GetProcessExpandLevel expandLevel)
      : this(requestContext, expandLevel, new Guid?())
    {
    }

    public GetProcessesExpandResult(
      IVssRequestContext requestContext,
      GetProcessExpandLevel expandLevel,
      Guid? processTypeId)
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      this.defaultProcessTypeId = service.GetDefaultProcessTypeId(requestContext);
      this.disabledProcessTypeIds = service.GetDisabledProcessTypeIds(requestContext);
      if (!processTypeId.HasValue)
      {
        this.processDescriptors = (IDictionary<Guid, ProcessDescriptor>) service.GetProcessDescriptors(requestContext).ToDictionary<ProcessDescriptor, Guid, ProcessDescriptor>((Func<ProcessDescriptor, Guid>) (x => x.TypeId), (Func<ProcessDescriptor, ProcessDescriptor>) (x => x));
      }
      else
      {
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, processTypeId.Value);
        this.processDescriptors = (IDictionary<Guid, ProcessDescriptor>) new Dictionary<Guid, ProcessDescriptor>()
        {
          {
            processDescriptor.TypeId,
            processDescriptor
          }
        };
      }
      if (expandLevel != GetProcessExpandLevel.Projects)
        return;
      this.subscribedProjectsLookup = requestContext.GetService<IWorkItemTrackingProcessService>().GetProjectProcessDescriptorMappings(requestContext, expectUnmappedProjects: !WorkItemTrackingFeatureFlags.IsSharedProcessEnabled(requestContext)).Where<ProjectProcessDescriptorMapping>((Func<ProjectProcessDescriptorMapping, bool>) (p => p.Descriptor != null)).ToLookup<ProjectProcessDescriptorMapping, Guid, ProjectInfo>((Func<ProjectProcessDescriptorMapping, Guid>) (x => x.Descriptor.TypeId), (Func<ProjectProcessDescriptorMapping, ProjectInfo>) (x => x.Project));
    }

    public bool IsProcessEnabled(Guid processTypeId) => !this.disabledProcessTypeIds.Contains(processTypeId);

    public bool IsDefaultProcess(Guid processTypeId) => object.Equals((object) processTypeId, (object) this.defaultProcessTypeId);

    public ProcessDescriptor GetProcessDescriptor(Guid processTypeId) => this.processDescriptors[processTypeId];

    public IEnumerable<ProjectInfo> GetSubscribedProjects(Guid processTypeId) => this.subscribedProjectsLookup == null || !this.subscribedProjectsLookup.Contains(processTypeId) ? (IEnumerable<ProjectInfo>) null : this.subscribedProjectsLookup[processTypeId];
  }
}
