// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseStoreManagerCosmos
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseStoreManagerCosmos : DocumentServiceLeaseStoreManager
  {
    private readonly DocumentServiceLeaseStore leaseStore;
    private readonly DocumentServiceLeaseManager leaseManager;
    private readonly DocumentServiceLeaseCheckpointer leaseCheckpointer;
    private readonly DocumentServiceLeaseContainer leaseContainer;

    public DocumentServiceLeaseStoreManagerCosmos(
      DocumentServiceLeaseStoreManagerOptions options,
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      RequestOptionsFactory requestOptionsFactory)
      : this(options, monitoredContainer, leaseContainer, requestOptionsFactory, (DocumentServiceLeaseUpdater) new DocumentServiceLeaseUpdaterCosmos((Container) leaseContainer))
    {
    }

    internal DocumentServiceLeaseStoreManagerCosmos(
      DocumentServiceLeaseStoreManagerOptions options,
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      RequestOptionsFactory requestOptionsFactory,
      DocumentServiceLeaseUpdater leaseUpdater)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      if (options.ContainerNamePrefix == null)
        throw new ArgumentNullException("ContainerNamePrefix");
      if (string.IsNullOrEmpty(options.HostName))
        throw new ArgumentNullException("HostName");
      if (monitoredContainer == null)
        throw new ArgumentNullException(nameof (monitoredContainer));
      if (leaseContainer == null)
        throw new ArgumentNullException(nameof (leaseContainer));
      if (requestOptionsFactory == null)
        throw new ArgumentException(nameof (requestOptionsFactory));
      if (leaseUpdater == null)
        throw new ArgumentException(nameof (leaseUpdater));
      this.leaseStore = (DocumentServiceLeaseStore) new DocumentServiceLeaseStoreCosmos((Container) leaseContainer, options.ContainerNamePrefix, requestOptionsFactory);
      this.leaseManager = (DocumentServiceLeaseManager) new DocumentServiceLeaseManagerCosmos(monitoredContainer, leaseContainer, leaseUpdater, options, requestOptionsFactory);
      this.leaseCheckpointer = (DocumentServiceLeaseCheckpointer) new DocumentServiceLeaseCheckpointerCore(leaseUpdater, requestOptionsFactory);
      this.leaseContainer = (DocumentServiceLeaseContainer) new DocumentServiceLeaseContainerCosmos((Container) leaseContainer, options);
    }

    public override DocumentServiceLeaseStore LeaseStore => this.leaseStore;

    public override DocumentServiceLeaseManager LeaseManager => this.leaseManager;

    public override DocumentServiceLeaseCheckpointer LeaseCheckpointer => this.leaseCheckpointer;

    public override DocumentServiceLeaseContainer LeaseContainer => this.leaseContainer;
  }
}
