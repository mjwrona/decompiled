// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.IWorkItemTypeRemotableService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  [DefaultServiceImplementation("Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemTypeService, Microsoft.Azure.Devops.Work.PlatformServices")]
  public interface IWorkItemTypeRemotableService : IVssFrameworkService
  {
    WorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      Guid projectId,
      string witNameOrReferenceName);

    WorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      string projectName,
      string witNameOrReferenceName);

    IEnumerable<WorkItemType> GetWorkItemTypes(IVssRequestContext requestContext, Guid projectId);

    IEnumerable<WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      string projectName);
  }
}
