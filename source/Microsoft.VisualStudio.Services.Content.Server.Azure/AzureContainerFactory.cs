// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureContainerFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class AzureContainerFactory : 
    ISecretItemChangeListener,
    IAzureBlobContainerFactory,
    IDisposable
  {
    public Microsoft.Azure.Storage.RetryPolicies.LocationMode? LocationMode { get; set; }

    public abstract ICloudBlobContainer CreateContainerReference(
      string containerName,
      bool enableTracing);

    public abstract void OnSecretChanged(
      string keyOfChangedStrongboxItem,
      string newStrongBoxItemValue);

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose() => this.Dispose(true);
  }
}
