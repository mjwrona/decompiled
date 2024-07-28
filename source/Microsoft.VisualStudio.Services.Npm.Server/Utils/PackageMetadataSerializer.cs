// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.PackageMetadataSerializer
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class PackageMetadataSerializer : IPackageMetadataSerializer
  {
    private readonly ITracerService tracerService;

    public PackageMetadataSerializer(ITracerService tracerService) => this.tracerService = tracerService;

    public PackageMetadata DeserializePackageMetadata(
      string packageMetadata,
      NpmPackageName packageName,
      UpstreamSource? upstreamSource)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (DeserializePackageMetadata)))
      {
        ArgumentUtility.CheckForNull<string>(packageMetadata, nameof (packageMetadata));
        using (StringReader stringReader = new StringReader(packageMetadata))
        {
          IReadOnlyList<Exception> errors;
          PackageMetadata packageMetadata1 = PackageJsonUtils.DeserializeNpmJsonDocumentNoThrow<PackageMetadata>((TextReader) stringReader, this.tracerService, packageName.FullName, (string) null, upstreamSource?.Location, nameof (DeserializePackageMetadata), out errors);
          if (packageMetadata1 == null || errors.Count > 0)
          {
            string str = string.Join(Environment.NewLine, errors.Select<Exception, string>((Func<Exception, string>) (x => x.Message)).Prepend<string>(""));
            JsonSerializationException serializationException = new JsonSerializationException(upstreamSource != null ? Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageMetadataUpstream((object) packageName.FullName, (object) upstreamSource.Location, (object) str) : Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageMetadata((object) packageName.FullName, (object) str));
            tracerBlock.TraceException((Exception) serializationException);
            if (packageMetadata1 == null)
              throw serializationException;
          }
          return packageMetadata1;
        }
      }
    }
  }
}
