// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SearchApiController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class SearchApiController : TfsProjectApiController
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      [typeof (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.InvalidQueryException)] = HttpStatusCode.BadRequest,
      [typeof (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException)] = HttpStatusCode.BadRequest
    };

    public override string TraceArea => "SearchRestService";
  }
}
