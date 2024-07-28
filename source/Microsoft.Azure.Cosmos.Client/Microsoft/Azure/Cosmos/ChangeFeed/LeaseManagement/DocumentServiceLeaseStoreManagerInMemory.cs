// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseStoreManagerInMemory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Concurrent;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseStoreManagerInMemory : DocumentServiceLeaseStoreManager
  {
    private readonly DocumentServiceLeaseStore leaseStore;
    private readonly DocumentServiceLeaseManager leaseManager;
    private readonly DocumentServiceLeaseCheckpointer leaseCheckpointer;
    private readonly DocumentServiceLeaseContainer leaseContainer;

    public DocumentServiceLeaseStoreManagerInMemory()
      : this(new ConcurrentDictionary<string, DocumentServiceLease>())
    {
    }

    internal DocumentServiceLeaseStoreManagerInMemory(
      ConcurrentDictionary<string, DocumentServiceLease> container)
      : this((DocumentServiceLeaseUpdater) new DocumentServiceLeaseUpdaterInMemory(container), container)
    {
    }

    internal DocumentServiceLeaseStoreManagerInMemory(
      DocumentServiceLeaseUpdater leaseUpdater,
      ConcurrentDictionary<string, DocumentServiceLease> container)
    {
      if (leaseUpdater == null)
        throw new ArgumentException(nameof (leaseUpdater));
      this.leaseStore = (DocumentServiceLeaseStore) new DocumentServiceLeaseStoreInMemory();
      this.leaseManager = (DocumentServiceLeaseManager) new DocumentServiceLeaseManagerInMemory(leaseUpdater, container);
      this.leaseCheckpointer = (DocumentServiceLeaseCheckpointer) new DocumentServiceLeaseCheckpointerCore(leaseUpdater, (RequestOptionsFactory) new PartitionedByIdCollectionRequestOptionsFactory());
      this.leaseContainer = (DocumentServiceLeaseContainer) new DocumentServiceLeaseContainerInMemory(container);
    }

    public override DocumentServiceLeaseStore LeaseStore => this.leaseStore;

    public override DocumentServiceLeaseManager LeaseManager => this.leaseManager;

    public override DocumentServiceLeaseCheckpointer LeaseCheckpointer => this.leaseCheckpointer;

    public override DocumentServiceLeaseContainer LeaseContainer => this.leaseContainer;
  }
}
