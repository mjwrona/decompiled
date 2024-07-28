// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing.RawNpmPackageNameWithFileRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing
{
  public class RawNpmPackageNameWithFileRequestConverter : 
    IConverter<RawNpmPackageNameWithFileRequest, RawPackageRequest>,
    IHaveInputType<RawNpmPackageNameWithFileRequest>,
    IHaveOutputType<RawPackageRequest>
  {
    public RawPackageRequest Convert(RawNpmPackageNameWithFileRequest input)
    {
      string fileName = input.FileName;
      string str = input.UnscopedPackageName + "-";
      int startIndex = fileName.StartsWith(str) && fileName.EndsWith(".tgz") ? str.Length : throw new InvalidRequestException(Resources.Error_InvalidPackageFileName());
      int length = fileName.Length - ".tgz".Length - startIndex;
      string packageVersion = fileName.Substring(startIndex, length);
      return new RawPackageRequest((IFeedRequest) input, RawNpmPackageName.Create(input.PackageScope, input.UnscopedPackageName), packageVersion);
    }
  }
}
