// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemTrackingOutOfBoxFieldsCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  public class WorkItemTrackingOutOfBoxFieldsCache : 
    VssMemoryCacheService<Guid, IReadOnlyCollection<ProcessFieldDefinition>>
  {
    public IReadOnlyCollection<ProcessFieldDefinition> GetOutOfBoxFieldsForProcess(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      IReadOnlyCollection<ProcessFieldDefinition> source = (IReadOnlyCollection<ProcessFieldDefinition>) null;
      if (!this.TryGetValue(requestContext, processTypeId, out source))
      {
        ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, processTypeId);
        if (processDescriptor.IsDerived)
          processDescriptor = service.GetProcessDescriptor(requestContext, processDescriptor.Inherits);
        if (!processDescriptor.IsSystem)
          throw new ProcessOutOfBoxFieldsNotFound(processTypeId);
        source = requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, processDescriptor).ProcessFields;
        if (source != null)
          this.Set(requestContext, processTypeId, source);
      }
      return source == null ? (IReadOnlyCollection<ProcessFieldDefinition>) null : (IReadOnlyCollection<ProcessFieldDefinition>) source.Select<ProcessFieldDefinition, ProcessFieldDefinition>((Func<ProcessFieldDefinition, ProcessFieldDefinition>) (d => d.Clone())).ToList<ProcessFieldDefinition>();
    }
  }
}
