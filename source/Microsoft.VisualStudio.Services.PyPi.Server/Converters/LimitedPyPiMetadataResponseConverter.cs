// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Converters.LimitedPyPiMetadataResponseConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Converters
{
  public class LimitedPyPiMetadataResponseConverter : 
    IConverter<IEnumerable<LimitedPyPiMetadata>, LimitedPyPiMetadataResponse>,
    IHaveInputType<IEnumerable<LimitedPyPiMetadata>>,
    IHaveOutputType<LimitedPyPiMetadataResponse>
  {
    public LimitedPyPiMetadataResponse Convert(
      IEnumerable<LimitedPyPiMetadata> limitedPyPiMetadata)
    {
      List<RawLimitedPyPiMetadata> limitedPyPiMetadataList = new List<RawLimitedPyPiMetadata>();
      foreach (LimitedPyPiMetadata limitedPyPiMetadata1 in limitedPyPiMetadata)
        limitedPyPiMetadataList.Add(new RawLimitedPyPiMetadata()
        {
          PackageName = limitedPyPiMetadata1.Identity.Name.DisplayName,
          PackageVersion = limitedPyPiMetadata1.Identity.Version.DisplayVersion,
          RequiresPython = limitedPyPiMetadata1.RequiresPython,
          PackageFiles = (IReadOnlyList<UnstoredPyPiPackageFile>) limitedPyPiMetadata1.PackageFiles.Select<IUnstoredPyPiPackageFile, UnstoredPyPiPackageFile>((Func<IUnstoredPyPiPackageFile, UnstoredPyPiPackageFile>) (file => new UnstoredPyPiPackageFile(file.DistType.ToString(), file.Path, LimitedPyPiMetadataResponseConverter.GetHashes((IUnstoredPackageFile) file), file.SizeInBytes, file.DateAdded))).ToList<UnstoredPyPiPackageFile>()
        });
      return new LimitedPyPiMetadataResponse()
      {
        LimitedPyPiMetadataList = limitedPyPiMetadataList
      };
    }

    private static List<KeyValuePair<string, string>> GetHashes(IUnstoredPackageFile file) => file.Hashes.Select<HashAndType, KeyValuePair<string, string>>((Func<HashAndType, KeyValuePair<string, string>>) (hash => new KeyValuePair<string, string>(hash.HashType.ToString(), hash.Value))).ToList<KeyValuePair<string, string>>();
  }
}
