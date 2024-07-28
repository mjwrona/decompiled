// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobStoreProviderConstants
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public sealed class BlobStoreProviderConstants
  {
    public static readonly string BlobContainerPrefix = "b-";
    public static readonly string BlobStoreSuffix = ".blob";
    public static readonly string MetadataPrefix = "md";
    public static readonly string DedupMetadataPrefix = "dedup";
    public static readonly string DedupContainerPrefix = "db";
    public static readonly string SessionMetadataPrefix = "session";
  }
}
