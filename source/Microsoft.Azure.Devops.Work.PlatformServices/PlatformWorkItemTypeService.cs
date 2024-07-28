// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemTypeService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWorkItemTypeService : IWorkItemTypeRemotableService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      Guid projectId,
      string witNameOrReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(witNameOrReferenceName, nameof (witNameOrReferenceName));
      return WorkItemTypeServiceHelper.GetWorkItemTypeInternal(requestContext, projectId, witNameOrReferenceName);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      string projectName,
      string witNameOrReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
      ArgumentUtility.CheckForNull<string>(witNameOrReferenceName, nameof (witNameOrReferenceName));
      IProjectService service = requestContext.GetService<IProjectService>();
      return WorkItemTypeServiceHelper.GetWorkItemTypeInternal(requestContext, service.GetProjectId(requestContext, projectName), witNameOrReferenceName);
    }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return WorkItemTypeServiceHelper.GetWorkItemTypesInternal(requestContext, projectId);
    }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      string projectName)
    {
      throw new NotImplementedException();
    }
  }
}
