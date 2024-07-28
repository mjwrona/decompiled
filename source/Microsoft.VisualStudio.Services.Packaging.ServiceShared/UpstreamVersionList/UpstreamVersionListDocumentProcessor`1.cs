// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList.UpstreamVersionListDocumentProcessor`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList
{
  public class UpstreamVersionListDocumentProcessor<TPackageVersion> : 
    IAggregationDocumentProcessor<UpstreamVersionListFile<TPackageVersion>>,
    IEmptyDocumentProvider<UpstreamVersionListFile<TPackageVersion>>
    where TPackageVersion : IPackageVersion
  {
    private readonly IConverter<string, TPackageVersion> versionParser;

    public UpstreamVersionListDocumentProcessor(IConverter<string, TPackageVersion> versionParser) => this.versionParser = versionParser;

    public UpstreamVersionListFile<TPackageVersion> GetEmptyDocument() => UpstreamVersionListFile<TPackageVersion>.Empty;

    public UpstreamVersionListFile<TPackageVersion> Deserialize(byte[] buffer) => JsonConvert.DeserializeObject<UpstreamVersionListFile<TPackageVersion>.Stored>(Encoding.UTF8.GetString(buffer)).Unpack(new Func<string, TPackageVersion>(this.versionParser.Convert));

    public byte[] Serialize(UpstreamVersionListFile<TPackageVersion> doc) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) doc.Pack()));

    public void NotifySaved(UpstreamVersionListFile<TPackageVersion> doc)
    {
    }
  }
}
