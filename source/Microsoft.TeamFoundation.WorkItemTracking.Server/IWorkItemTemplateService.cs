// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IWorkItemTemplateService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (WorkItemTemplateService))]
  public interface IWorkItemTemplateService : IVssFrameworkService
  {
    WorkItemTemplate CreateTemplate(IVssRequestContext requestContext, WorkItemTemplate template);

    void UpdateTemplate(IVssRequestContext requestContext, WorkItemTemplate template);

    WorkItemTemplate GetTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid templateId);

    IEnumerable<WorkItemTemplateDescriptor> GetTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId);

    IEnumerable<WorkItemTemplateDescriptor> GetTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string workItemTypeName);

    void DeleteTemplate(IVssRequestContext requestContext, Guid projectId, Guid templateId);

    void DeleteAllTemplatesInProject(IVssRequestContext requestContext, Guid projectId);

    void DeleteTemplatesByOwner(IVssRequestContext requestContext, Guid projectId, Guid ownerId);
  }
}
