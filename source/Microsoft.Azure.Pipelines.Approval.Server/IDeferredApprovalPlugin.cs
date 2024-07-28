// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.IDeferredApprovalPlugin
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  [InheritedExport]
  public interface IDeferredApprovalPlugin
  {
    void ScheduleDeferredApprovalJob(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid currentUserId,
      ApprovalUpdateParameters updateParameter);
  }
}
