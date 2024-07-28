// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing.NpmPackageIdentityParsingConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing
{
  public class NpmPackageIdentityParsingConverter : 
    IConverter<IRawPackageRequest, NpmPackageIdentity>,
    IHaveInputType<IRawPackageRequest>,
    IHaveOutputType<NpmPackageIdentity>
  {
    public NpmPackageIdentity Convert(IRawPackageRequest input)
    {
      NpmPackageName name = new NpmPackageName(input.PackageName);
      SemanticVersion version1;
      if (!NpmVersionUtils.TryParseNpmPackageVersion(input.PackageVersion, out version1))
        throw new InvalidPackageException(Resources.Error_InvalidPackageVersion((object) input.PackageVersion));
      SemanticVersion version2 = version1;
      return new NpmPackageIdentity(name, version2);
    }
  }
}
