// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.CargoApiController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Exceptions;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers
{
  public class CargoApiController : 
    SingleFileProtocolPackagingApiController<CargoPackageName, CargoPackageVersion, CargoPackageIdentity>
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) CargoExceptionMappings.Mappings;

    public override string TraceArea => Protocol.Cargo.CorrectlyCasedName;

    public override string ActivityLogArea => Protocol.Cargo.CorrectlyCasedName;

    protected override string? GetClientSessionIdFrom(HttpRequestMessage requestMessage) => (string) null;

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.Cargo;

    protected override IIdentityResolver<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, SimplePackageFileName> IdentityResolver => (IIdentityResolver<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, SimplePackageFileName>) CargoIdentityResolver.Instance;
  }
}
