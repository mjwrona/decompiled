// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.Controllers.PipelineChecksExceptionMapper
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;

namespace Microsoft.Azure.Pipelines.Checks.Server.Controllers
{
  public static class PipelineChecksExceptionMapper
  {
    public static void Map(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FileIdNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<FormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IndexOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UriFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CheckConfigurationNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<CheckRunExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<CheckRunRequestNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<InvalidCheckConfigurationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CheckSuiteRequestNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<CheckSuiteRequestIdExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<InvalidCheckTypeException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<AccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<CheckSuiteIsAlreadyCompleted>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidCheckRunUpdateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CheckCannotBeRerunException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CheckSuiteBypassException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CheckSuiteUpdateException>(HttpStatusCode.BadRequest);
    }
  }
}
