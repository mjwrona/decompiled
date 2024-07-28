// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemTypeCategoryService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWorkItemTypeCategoryService : 
    IWorkItemTypeCategoryRemotableService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      Guid projectId,
      string category)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(category, nameof (category));
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory itemTypeCategory = requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategory(requestContext, projectId, category);
      return WorkItemTypeCategoryFactory.Create(requestContext, itemTypeCategory);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      string projectName,
      string category)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
      ArgumentUtility.CheckForNull<string>(category, nameof (category));
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory itemTypeCategory = requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategory(requestContext, projectName, category);
      return WorkItemTypeCategoryFactory.Create(requestContext, itemTypeCategory);
    }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>) requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, projectName).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>) (cat => WorkItemTypeCategoryFactory.Create(requestContext, cat))).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>();
    }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>) requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, projectId).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>) (cat => WorkItemTypeCategoryFactory.Create(requestContext, cat))).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>();
    }
  }
}
