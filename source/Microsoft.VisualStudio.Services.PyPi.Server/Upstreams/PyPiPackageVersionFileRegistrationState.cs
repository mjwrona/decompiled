// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiPackageVersionFileRegistrationState
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using System;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public record PyPiPackageVersionFileRegistrationState(
    string Filename,
    ImmutableArray<HashAndType> Digests,
    long Size,
    DateTime UploadTime,
    PyPiDistType? Packagetype,
    string Url,
    bool? HasSig,
    string? RequiresPython,
    bool? IsYanked,
    string? YankedReason)
  ;
}
