// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility.MetadataProjectCompatibilityDescriptor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility
{
  public class MetadataProjectCompatibilityDescriptor
  {
    public IDictionary<string, int> CategoryIdsByRefName = (IDictionary<string, int>) new Dictionary<string, int>();
    private ProcessDescriptor m_processDescriptor;
    private bool m_lookedForProcessDescriptor;
    private bool m_lookedForProcessDefinition;
    private ProcessWorkDefinition m_oobProcessDefinition;

    public MetadataProjectCompatibilityDescriptor()
    {
    }

    public MetadataProjectCompatibilityDescriptor(
      ProcessDescriptor processDescriptor,
      IEnumerable<WorkItemTypeCategory> categories)
    {
      this.m_processDescriptor = processDescriptor;
      this.m_lookedForProcessDescriptor = true;
      this.WorkItemTypeCategories = categories;
    }

    public TreeNode ProjectNode { get; set; }

    public IEnumerable<WorkItemTypeCategory> WorkItemTypeCategories { get; }

    public IReadOnlyCollection<MetadataWorkItemTypeCompatibilityDescriptor> TypeDescriptors { get; set; }

    internal bool TryGetProcessDescriptor(
      IVssRequestContext requestContext,
      out ProcessDescriptor processDescriptor)
    {
      if (this.m_lookedForProcessDescriptor)
      {
        processDescriptor = this.m_processDescriptor;
        return processDescriptor != null;
      }
      this.m_lookedForProcessDescriptor = true;
      if (this.m_processDescriptor == null)
      {
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        if (this.ProjectNode == null || !service.TryGetLatestProjectProcessDescriptor(requestContext.Elevate(), this.ProjectNode.ProjectId, out this.m_processDescriptor))
          this.m_processDescriptor = (ProcessDescriptor) null;
      }
      processDescriptor = this.m_processDescriptor;
      return processDescriptor != null;
    }

    internal bool TryGetOobProcessDefinition(
      IVssRequestContext requestContext,
      out ProcessWorkDefinition oobProcessDefinition)
    {
      if (this.m_lookedForProcessDefinition)
      {
        oobProcessDefinition = this.m_oobProcessDefinition;
        return oobProcessDefinition != null;
      }
      this.m_lookedForProcessDefinition = true;
      ProcessDescriptor processDescriptor;
      if (this.TryGetProcessDescriptor(requestContext, out processDescriptor) && !processDescriptor.IsCustom)
      {
        ILegacyWorkItemTrackingProcessService service = requestContext.GetService<ILegacyWorkItemTrackingProcessService>();
        Guid processTypeId = processDescriptor.IsDerived ? processDescriptor.Inherits : processDescriptor.TypeId;
        this.m_oobProcessDefinition = service.GetProcessWorkDefinition(requestContext, processTypeId);
        oobProcessDefinition = this.m_oobProcessDefinition;
        return true;
      }
      oobProcessDefinition = (ProcessWorkDefinition) null;
      return false;
    }
  }
}
