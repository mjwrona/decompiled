// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Ingestion.MavenCommitOperationDataFromStorablePackageInfoHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Ingestion
{
  public class MavenCommitOperationDataFromStorablePackageInfoHandler : 
    IAsyncHandler<IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo>, MavenCommitOperationData>,
    IHaveInputType<IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo>>,
    IHaveOutputType<MavenCommitOperationData>
  {
    private readonly IProvenanceInfoProvider provenanceInfoProvider;
    private readonly ITimeProvider timeProvider;

    public MavenCommitOperationDataFromStorablePackageInfoHandler(
      IProvenanceInfoProvider provenanceInfoProvider,
      ITimeProvider timeProvider)
    {
      this.provenanceInfoProvider = provenanceInfoProvider;
      this.timeProvider = timeProvider;
    }

    public async Task<MavenCommitOperationData> Handle(
      IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo> request)
    {
      string fileName = request.ProtocolSpecificInfo.FilePath.FileName;
      Stream stream = request.ProtocolSpecificInfo.Stream;
      IStorageId packageStorageId = request.PackageStorageId;
      IEnumerable<HashAndType> hashes = MavenChecksumUtility.ComputeTextChecksums(stream).Select<(MavenHashAlgorithmInfo, string), HashAndType>((Func<(MavenHashAlgorithmInfo, string), HashAndType>) (x => new HashAndType()
      {
        HashType = x.AlgorithmInfo.HashType,
        Value = x.HashValue
      }));
      ImmutableList<MavenPackageFileNew> files = ImmutableList.Create<MavenPackageFileNew>(new MavenPackageFileNew(fileName, packageStorageId, hashes, stream.Length, this.timeProvider.Now));
      byte[] pomBytes = (byte[]) null;
      if (request.ProtocolSpecificInfo.IsPomFile)
      {
        using (MemoryStream destination = new MemoryStream())
        {
          stream.Position = 0L;
          stream.CopyTo((Stream) destination);
          pomBytes = destination.ToArray();
        }
      }
      MavenCommitOperationData commitOperationData = new MavenCommitOperationData(request.PackageId, (IEnumerable<MavenPackageFileNew>) files, pomBytes, await this.provenanceInfoProvider.GetProvenanceInfoAsync(), request.SourceChain);
      files = (ImmutableList<MavenPackageFileNew>) null;
      pomBytes = (byte[]) null;
      return commitOperationData;
    }
  }
}
