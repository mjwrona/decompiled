// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.AttachmentFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal static class AttachmentFactory
  {
    internal static AttachmentReference Create(
      WorkItemTrackingRequestContext witContext,
      Guid projectId,
      Guid attachmentId,
      ISecuredObject securedObject,
      string fileName = null,
      bool returnProjectScopedUrl = true)
    {
      Guid? projectId1 = new Guid?();
      AttachmentReference attachmentReference;
      if (projectId != Guid.Empty)
      {
        projectId1 = new Guid?(projectId);
        attachmentReference = new AttachmentReference(securedObject);
      }
      else
        attachmentReference = new AttachmentReference();
      attachmentReference.Id = attachmentId;
      attachmentReference.Url = WitUrlHelper.GetAttachmentUrl(witContext, projectId1, attachmentId, fileName, returnProjectScopedUrl);
      return attachmentReference;
    }
  }
}
