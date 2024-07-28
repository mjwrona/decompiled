// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers.ItemStoreImportSettings
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers
{
  internal class ItemStoreImportSettings : ImportSettings
  {
    public ItemStoreImportSettings(
      string tablePrefix,
      IServicingContext servicingContext,
      IVssRequestContext requestContext,
      IVssRequestContext deploymentContext,
      int sourceDatabaseId)
      : base("Item", tablePrefix, servicingContext, requestContext, deploymentContext, sourceDatabaseId)
    {
    }

    protected override AzureTableUploader CreateAzureTableUploader() => (AzureTableUploader) new ItemAzureTableUploader((ImportSettings) this);
  }
}
