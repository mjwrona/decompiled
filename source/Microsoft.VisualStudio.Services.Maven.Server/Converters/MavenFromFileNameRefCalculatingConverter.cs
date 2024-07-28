// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Converters.MavenFromFileNameRefCalculatingConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Converters
{
  public class MavenFromFileNameRefCalculatingConverter : 
    IConverter<IPackageFileRequest<MavenPackageIdentity>, string>,
    IHaveInputType<IPackageFileRequest<MavenPackageIdentity>>,
    IHaveOutputType<string>
  {
    public string Convert(IPackageFileRequest<MavenPackageIdentity> input) => string.Join("/", ((IEnumerable<string>) new string[5]
    {
      "feed",
      input.Feed.Id.ToString(),
      input.PackageId.Name.GroupId,
      input.PackageId.Name.ArtifactId,
      input.FilePath
    }).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x)))).ToLowerInvariant();
  }
}
