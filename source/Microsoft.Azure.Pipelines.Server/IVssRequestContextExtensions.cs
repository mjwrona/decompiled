// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.IVssRequestContextExtensions
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Server
{
  public static class IVssRequestContextExtensions
  {
    private static readonly Guid PipelinesServiceInstanceType = new Guid("0000005a-0000-8888-8000-000000000000");

    public static bool IsPipelines(this IVssRequestContext requestContext) => requestContext.ServiceInstanceType() == IVssRequestContextExtensions.PipelinesServiceInstanceType;
  }
}
