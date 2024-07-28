// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.IApprovalDetailsProvider
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  [InheritedExport]
  public interface IApprovalDetailsProvider : IApprovalOwner
  {
    IList<Guid> FilterApprovalsWithRequiredPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<Guid, JObject> approvalIdsToContext,
      ApprovalPermissions permission);
  }
}
