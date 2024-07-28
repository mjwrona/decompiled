// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseManagementPublicControllerBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [TraceFilter]
  public abstract class ReleaseManagementPublicControllerBase : TfsApiController
  {
    public override string ActivityLogArea => "ReleaseManagement";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => ReleaseManagementProjectControllerBase.HttpExceptionsMap;
  }
}
