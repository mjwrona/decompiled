// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.ProcessModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class ProcessModelFactory
  {
    internal static ProcessModel Create(
      IVssRequestContext requestContext,
      Guid processTypeId,
      GetProcessesExpandResult expandResult)
    {
      ProcessDescriptor processDescriptor = expandResult.GetProcessDescriptor(processTypeId);
      bool isDefault = expandResult.IsDefaultProcess(processTypeId);
      bool isEnabled = expandResult.IsProcessEnabled(processTypeId);
      return new ProcessModel()
      {
        TypeId = processDescriptor.TypeId,
        ReferenceName = processDescriptor.ReferenceName,
        Name = processDescriptor.Name,
        Description = processDescriptor.Description,
        Properties = ProcessModelFactory.GetProcessProperties(processDescriptor, isDefault, isEnabled),
        Projects = ProcessModelFactory.GetProjectReferences(processTypeId, expandResult)
      };
    }

    internal static ProcessInfo CreateProcessInfo(
      IVssRequestContext requestContext,
      Guid processTypeId,
      GetProcessesExpandResult expandResult)
    {
      ProcessDescriptor processDescriptor = expandResult.GetProcessDescriptor(processTypeId);
      bool flag1 = expandResult.IsDefaultProcess(processTypeId);
      bool flag2 = expandResult.IsProcessEnabled(processTypeId);
      return new ProcessInfo()
      {
        TypeId = processDescriptor.TypeId,
        ReferenceName = processDescriptor.ReferenceName,
        Name = processDescriptor.Name,
        Description = processDescriptor.Description,
        CustomizationType = processDescriptor.IsDerived ? CustomizationType.Inherited : (processDescriptor.IsSystem ? CustomizationType.System : CustomizationType.Custom),
        ParentProcessTypeId = processDescriptor.Inherits,
        IsEnabled = flag2,
        IsDefault = flag1,
        Projects = ProcessModelFactory.GetProjectReferences(processTypeId, expandResult)
      };
    }

    private static IEnumerable<ProjectReference> GetProjectReferences(
      Guid processTypeId,
      GetProcessesExpandResult expandResult)
    {
      IEnumerable<ProjectInfo> subscribedProjects = expandResult.GetSubscribedProjects(processTypeId);
      return subscribedProjects == null ? (IEnumerable<ProjectReference>) null : subscribedProjects.Select<ProjectInfo, ProjectReference>((Func<ProjectInfo, ProjectReference>) (p => new ProjectReference()
      {
        Id = p.Id,
        Name = p.Name,
        Url = p.Uri,
        Description = p.Description
      }));
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessProperties GetProcessProperties(
      ProcessDescriptor descriptor,
      bool isDefault,
      bool isEnabled)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessProperties()
      {
        IsDefault = isDefault,
        ParentProcessTypeId = descriptor.Inherits,
        IsEnabled = isEnabled,
        Version = string.Format("{0}.{1}", (object) descriptor.Version.Major, (object) descriptor.Version.Minor),
        Class = ProcessModelFactory.GetProcessClass(descriptor)
      };
    }

    private static ProcessClass GetProcessClass(ProcessDescriptor descriptor)
    {
      if (descriptor.IsSystem)
        return ProcessClass.System;
      return descriptor.IsDerived ? ProcessClass.Derived : ProcessClass.Custom;
    }
  }
}
