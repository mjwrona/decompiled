// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.Controllers.ApprovalExceptionMapper
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;

namespace Microsoft.Azure.Pipelines.Approval.Server.Controllers
{
  public static class ApprovalExceptionMapper
  {
    public static void Map(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IndexOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UriFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ApprovalExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<ApprovalNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ApprovalIdNotProvidedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ApprovalAlreadyCompletedException>(HttpStatusCode.Conflict);
    }
  }
}
