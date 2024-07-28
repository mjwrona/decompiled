// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.Controllers.PipelineChecksApiController
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Pipelines.Checks.Server.Controllers
{
  public abstract class PipelineChecksApiController : TfsApiController
  {
    public override string TraceArea => "Pipeline.Checks";

    public override string ActivityLogArea => "Pipeline.Checks";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      PipelineChecksExceptionMapper.Map(exceptionMap);
    }
  }
}
