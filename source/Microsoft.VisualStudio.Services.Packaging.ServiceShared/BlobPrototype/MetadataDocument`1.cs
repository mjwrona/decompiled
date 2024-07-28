// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MetadataDocument`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MetadataDocument<TMetadataEntry> : IMetadataDocument where TMetadataEntry : class, IMetadataEntry
  {
    public MetadataDocument(List<TMetadataEntry> entries = null, IMetadataDocumentProperties properties = null)
    {
      this.Entries = entries ?? new List<TMetadataEntry>();
      this.Properties = properties;
    }

    public List<TMetadataEntry> Entries { get; }

    IReadOnlyList<IMetadataEntry> IMetadataDocument.Entries => (IReadOnlyList<IMetadataEntry>) this.Entries;

    public IMetadataDocumentProperties Properties { get; set; }
  }
}
