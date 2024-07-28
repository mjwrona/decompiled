// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.NuGetApiController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Exceptions;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.Shared.NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers
{
  [FeatureEnabled("Artifact.Features.NuGet")]
  [NuGetClientMessageExceptionFilter]
  public abstract class NuGetApiController : PackagingApiController
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) NuGetExceptionMappings.Mappings;

    public override string TraceArea => "NuGet";

    public override string ActivityLogArea => "NuGet";

    protected override bool ExemptFromGlobalExceptionFormatting { get; } = true;

    protected static void CheckPathArgumentNotNull(object argumentValue, string parameterName = null)
    {
      if (argumentValue == null)
        throw PathPartMissingException.Create(parameterName);
    }

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.NuGet;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage)
    {
      IEnumerable<string> values;
      return requestMessage.Headers.TryGetValues("X-NuGet-Session-Id", out values) ? values.FirstOrDefault<string>() : (string) null;
    }

    protected INuGetVersionsService NuGetVersionsService => this.TfsRequestContext.GetService<INuGetVersionsService>();
  }
}
