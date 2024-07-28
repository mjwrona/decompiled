// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.EnvironmentsApiController
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  public abstract class EnvironmentsApiController : TfsApiController
  {
    public override string TraceArea => "Environments";

    public override string ActivityLogArea => "Environments";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      PipelinesEnvironmentsExceptionMapper.Map(exceptionMap);
    }
  }
}
