// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReceiptSecretProviderBase
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public abstract class ReceiptSecretProviderBase : IReceiptSecretProvider, ISecretItemChangeListener
  {
    public ReceiptSecretTandem ReceiptSecrets { get; protected set; }

    public void OnSecretChanged(string keyOfChangedStrongboxItem, string newStrongBoxItemValue)
    {
      lock (this.ReceiptSecrets)
      {
        switch (keyOfChangedStrongboxItem)
        {
          case "PrimaryChunkDedupReceiptKey":
            this.ReceiptSecrets = new ReceiptSecretTandem(newStrongBoxItemValue, this.ReceiptSecrets.SecondarySecret);
            break;
          case "SecondaryChunkDedupReceiptKey":
            this.ReceiptSecrets = new ReceiptSecretTandem(this.ReceiptSecrets.PrimarySecret, newStrongBoxItemValue);
            break;
        }
      }
    }
  }
}
