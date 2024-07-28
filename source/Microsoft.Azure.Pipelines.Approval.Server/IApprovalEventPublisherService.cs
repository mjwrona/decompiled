// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.IApprovalEventPublisherService
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  [DefaultServiceImplementation(typeof (ApprovalEventPublisherService))]
  public interface IApprovalEventPublisherService : IVssFrameworkService
  {
    void NotifyApprovalCompletedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval);

    void NotifyApprovalSkippedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval,
      IdentityRef skippedBy);

    void NotifyApprovalPendingEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval);
  }
}
