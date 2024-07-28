// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.FeatureEnabledConstants
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E6307531-8252-47C3-B21C-ECA66F38ED4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.dll

namespace Microsoft.VisualStudio.Services.ItemStore.Server
{
  public static class FeatureEnabledConstants
  {
    public const string EnableEmptyBlobReferenceOptimization = "ItemStore.Features.EnableEmptyBlobReferenceOptimization";
    public const string AllowUndoSoftDelete = "ItemStore.Features.Internal.AllowUndoSoftDelete";
    public const string AzureBlobTelemetry = "Blobstore.Features.AzureBlobTelemetry";
    public const string BulkRemoveBlobRefs = "ItemStore.Features.BulkRemoveBlobRefs";
  }
}
