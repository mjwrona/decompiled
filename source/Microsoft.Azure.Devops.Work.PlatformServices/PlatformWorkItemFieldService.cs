// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemFieldService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWorkItemFieldService : IWorkItemFieldsRemotableService, IVssFrameworkService
  {
    public WorkItemField2 GetField(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      string projectName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldNameOrRefName, nameof (fieldNameOrRefName));
      Guid guid = Guid.Empty;
      if (projectName != null)
        guid = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      return WitFieldsHelper.GetWorkItemField2(requestContext, fieldNameOrRefName, projectId: new Guid?(guid));
    }

    public WorkItemField2 GetField(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      Guid? projectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldNameOrRefName, nameof (fieldNameOrRefName));
      return WitFieldsHelper.GetWorkItemField2(requestContext, fieldNameOrRefName, projectId: projectId);
    }

    public List<WorkItemField2> GetFields(
      IVssRequestContext requestContext,
      GetFieldsExpand? expand = null,
      string projectName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      Guid guid = Guid.Empty;
      if (projectName != null)
        guid = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      if (!expand.HasValue)
        expand = new GetFieldsExpand?(GetFieldsExpand.None);
      return WitFieldsHelper.GetWorkItemFields2(requestContext, expand.Value, projectId: new Guid?(guid)).ToList<WorkItemField2>();
    }

    public List<WorkItemField2> GetFields(
      IVssRequestContext requestContext,
      GetFieldsExpand? expand = null,
      Guid? projectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!expand.HasValue)
        expand = new GetFieldsExpand?(GetFieldsExpand.None);
      return WitFieldsHelper.GetWorkItemFields2(requestContext, expand.Value, projectId: projectId).ToList<WorkItemField2>();
    }

    public bool IsUpdatable(IVssRequestContext requestContext, string fieldReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldReferenceName, nameof (fieldReferenceName));
      return WitFieldsHelper.IsFieldUpdatable(requestContext, fieldReferenceName);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
