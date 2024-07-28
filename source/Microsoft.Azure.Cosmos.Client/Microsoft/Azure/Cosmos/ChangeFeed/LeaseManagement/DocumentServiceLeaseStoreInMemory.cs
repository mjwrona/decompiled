// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseStoreInMemory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseStoreInMemory : DocumentServiceLeaseStore
  {
    private bool isInitialized;

    public override Task<bool> IsInitializedAsync() => Task.FromResult<bool>(this.isInitialized);

    public override Task MarkInitializedAsync()
    {
      this.isInitialized = true;
      return Task.CompletedTask;
    }

    public override Task<bool> AcquireInitializationLockAsync(TimeSpan lockTime) => Task.FromResult<bool>(true);

    public override Task<bool> ReleaseInitializationLockAsync() => Task.FromResult<bool>(true);
  }
}
