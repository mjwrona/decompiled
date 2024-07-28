// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.PipelinesEnvironmentsExceptionMapper
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.ServiceModel;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  public static class PipelinesEnvironmentsExceptionMapper
  {
    public static void Map(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<EndpointNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<FormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IndexOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotSupportedException>(HttpStatusCode.BadRequest);
    }
  }
}
