// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex.NuGetMetadataFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex
{
  public static class NuGetMetadataFactory
  {
    public static ProtocolMetadata Create(NuGetPackageMetadata packageMetadata)
    {
      NuGetProtocolMetadata protocolMetadata = new NuGetProtocolMetadata()
      {
        Copyright = packageMetadata.Copyright,
        Language = packageMetadata.Language,
        MinClientVersion = packageMetadata.MinClientVersion,
        ProjectUrl = packageMetadata.ProjectUrl,
        ReleaseNotes = packageMetadata.ReleaseNotes,
        RequireLicenseAcceptance = packageMetadata.RequireLicenseAcceptance,
        Title = packageMetadata.Title,
        IconUrl = packageMetadata.IconUrl,
        IconFile = packageMetadata.IconFile,
        LicenseUrl = packageMetadata.LicenseUrl,
        LicenseExpression = packageMetadata.LicenseExpression,
        LicenseFile = packageMetadata.LicenseFile
      };
      return new ProtocolMetadata()
      {
        SchemaVersion = 1,
        Data = (object) protocolMetadata
      };
    }
  }
}
