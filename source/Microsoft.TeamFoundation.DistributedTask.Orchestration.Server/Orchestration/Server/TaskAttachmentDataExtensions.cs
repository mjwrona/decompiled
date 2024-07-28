// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskAttachmentDataExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class TaskAttachmentDataExtensions
  {
    public static void AddLinks(
      this TaskAttachmentData attachmentData,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string hubName,
      Guid planId)
    {
      Uri resourceUri = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "distributedtask", TaskResourceIds.Attachments, (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubname = hubName,
        planId = planId,
        timelineId = attachmentData.TimelineId,
        recordId = attachmentData.RecordId,
        type = attachmentData.Type,
        name = attachmentData.Name
      });
      attachmentData.Links.AddLink("self", resourceUri.AbsoluteUri);
    }
  }
}
