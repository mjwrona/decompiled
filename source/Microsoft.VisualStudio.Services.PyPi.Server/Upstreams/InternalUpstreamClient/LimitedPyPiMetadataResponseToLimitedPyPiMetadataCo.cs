// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient.LimitedPyPiMetadataResponseToLimitedPyPiMetadataConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient
{
  public class LimitedPyPiMetadataResponseToLimitedPyPiMetadataConverter : 
    IConverter<LimitedPyPiMetadataResponse, List<LimitedPyPiMetadata>>,
    IHaveInputType<LimitedPyPiMetadataResponse>,
    IHaveOutputType<List<LimitedPyPiMetadata>>
  {
    public List<LimitedPyPiMetadata> Convert(
      LimitedPyPiMetadataResponse limitedPyPiMetadataResponse)
    {
      List<LimitedPyPiMetadata> limitedPyPiMetadataList = new List<LimitedPyPiMetadata>();
      foreach (RawLimitedPyPiMetadata limitedPyPiMetadata in limitedPyPiMetadataResponse.LimitedPyPiMetadataList)
        limitedPyPiMetadataList.Add(ConvertVersion(limitedPyPiMetadata));
      return limitedPyPiMetadataList;

      static LimitedPyPiMetadata ConvertVersion(RawLimitedPyPiMetadata metadata) => new LimitedPyPiMetadata(PyPiIdentityResolver.Instance.ResolvePackageIdentity(metadata.PackageName, metadata.PackageVersion), metadata.RequiresPython, metadata.PackageFiles.Select<UnstoredPyPiPackageFile, PyPiPackageFile>(LimitedPyPiMetadataResponseToLimitedPyPiMetadataConverter.\u003C\u003EO.\u003C0\u003E__ConvertFile ?? (LimitedPyPiMetadataResponseToLimitedPyPiMetadataConverter.\u003C\u003EO.\u003C0\u003E__ConvertFile = new Func<UnstoredPyPiPackageFile, PyPiPackageFile>(ConvertFile))).ToImmutableArray<PyPiPackageFile>().CastArray<IUnstoredPyPiPackageFile>());

      static PyPiPackageFile ConvertFile(UnstoredPyPiPackageFile file) => new PyPiPackageFile(file.Path, (IStorageId) null, (IReadOnlyCollection<HashAndType>) file.HashTypeAndValueCollection.Select<KeyValuePair<string, string>, HashAndType>(LimitedPyPiMetadataResponseToLimitedPyPiMetadataConverter.\u003C\u003EO.\u003C1\u003E__ConvertHash ?? (LimitedPyPiMetadataResponseToLimitedPyPiMetadataConverter.\u003C\u003EO.\u003C1\u003E__ConvertHash = new Func<KeyValuePair<string, string>, HashAndType>(ConvertHash))).ToImmutableArray<HashAndType>(), file.Size, file.DateAdded, (PyPiDistType) Enum.Parse(typeof (PyPiDistType), file.PyPiDistType));

      static HashAndType ConvertHash(KeyValuePair<string, string> hash) => new HashAndType()
      {
        HashType = (HashType) Enum.Parse(typeof (HashType), hash.Key),
        Value = hash.Value
      };
    }
  }
}
