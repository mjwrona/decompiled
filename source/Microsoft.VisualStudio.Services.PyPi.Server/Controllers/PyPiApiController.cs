// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.PyPiApiController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.Shared.PyPi;
using Microsoft.VisualStudio.Services.PyPi.Server.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers
{
  [FeatureEnabled("Packaging.PyPi.Enabled")]
  public class PyPiApiController : PackagingApiController
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PyPiExceptionMappings.Mappings;

    public override string TraceArea => Protocol.PyPi.CorrectlyCasedName;

    public override string ActivityLogArea => Protocol.PyPi.CorrectlyCasedName;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage) => (string) null;

    protected IPyPiVersionsService PyPiVersionsService => this.TfsRequestContext.GetService<IPyPiVersionsService>();

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.PyPi;
  }
}
