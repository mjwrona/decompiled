// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.Controller.AuthorizationExceptionMapper
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;

namespace Microsoft.Azure.Pipelines.Authorization.Server.Controller
{
  public static class AuthorizationExceptionMapper
  {
    public static void Map(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidResourceType>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidOperationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AccessDeniedException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<DefinitionNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
