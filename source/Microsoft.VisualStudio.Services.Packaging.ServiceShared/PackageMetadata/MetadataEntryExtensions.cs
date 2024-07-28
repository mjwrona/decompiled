// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.MetadataEntryExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public static class MetadataEntryExtensions
  {
    public static bool IsDeleted(this IMetadataEntry metadataEntry) => metadataEntry.DeletedDate.HasValue || metadataEntry.PermanentDeletedDate.HasValue;

    public static bool IsPermanentlyDeleted(this IMetadataEntry metadataEntry) => metadataEntry.PermanentDeletedDate.HasValue;

    public static bool IsInRecycleBin(this IMetadataEntry metadataEntry) => metadataEntry.IsDeleted() && !metadataEntry.IsPermanentlyDeleted();
  }
}
