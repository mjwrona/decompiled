// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseStoreManagerBuilder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal class DocumentServiceLeaseStoreManagerBuilder
  {
    private static readonly string IdPkPathName = "/id";
    private static readonly string PartitionKeyPkPathName = "/partitionKey";
    private readonly DocumentServiceLeaseStoreManagerOptions options = new DocumentServiceLeaseStoreManagerOptions();
    private ContainerInternal monitoredContainer;
    private ContainerInternal leaseContainer;
    private RequestOptionsFactory requestOptionsFactory;

    public static async Task<DocumentServiceLeaseStoreManager> InitializeAsync(
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      string leaseContainerPrefix,
      string instanceName)
    {
      ContainerProperties containerPropertiesAsync = await leaseContainer.GetCachedContainerPropertiesAsync(false, (ITrace) NoOpTrace.Singleton, new CancellationToken());
      bool flag1 = containerPropertiesAsync.PartitionKey != null && containerPropertiesAsync.PartitionKey.Paths != null && containerPropertiesAsync.PartitionKey.Paths.Count > 0;
      PartitionKeyDefinition partitionKey = containerPropertiesAsync.PartitionKey;
      int num;
      if (partitionKey == null)
      {
        num = 0;
      }
      else
      {
        bool? isSystemKey = partitionKey.IsSystemKey;
        bool flag2 = true;
        num = isSystemKey.GetValueOrDefault() == flag2 & isSystemKey.HasValue ? 1 : 0;
      }
      bool flag3 = num != 0;
      if (flag1 && !flag3 && (containerPropertiesAsync.PartitionKey.Paths.Count != 1 || !(containerPropertiesAsync.PartitionKey.Paths[0] == DocumentServiceLeaseStoreManagerBuilder.IdPkPathName) && !(containerPropertiesAsync.PartitionKey.Paths[0] == DocumentServiceLeaseStoreManagerBuilder.PartitionKeyPkPathName)))
        throw new ArgumentException("The lease container, if partitioned, must have partition key equal to " + DocumentServiceLeaseStoreManagerBuilder.IdPkPathName + " or " + DocumentServiceLeaseStoreManagerBuilder.PartitionKeyPkPathName + ".");
      RequestOptionsFactory requestOptionsFactory = !flag1 || flag3 ? (RequestOptionsFactory) new SinglePartitionRequestOptionsFactory() : (containerPropertiesAsync.PartitionKey.Paths[0] != DocumentServiceLeaseStoreManagerBuilder.IdPkPathName ? (RequestOptionsFactory) new PartitionedByPartitionKeyCollectionRequestOptionsFactory() : (RequestOptionsFactory) new PartitionedByIdCollectionRequestOptionsFactory());
      return new DocumentServiceLeaseStoreManagerBuilder().WithLeasePrefix(leaseContainerPrefix).WithMonitoredContainer(monitoredContainer).WithLeaseContainer(leaseContainer).WithRequestOptionsFactory(requestOptionsFactory).WithHostName(instanceName).Build();
    }

    private DocumentServiceLeaseStoreManagerBuilder WithMonitoredContainer(
      ContainerInternal monitoredContainer)
    {
      this.monitoredContainer = monitoredContainer ?? throw new ArgumentNullException("leaseContainer");
      return this;
    }

    private DocumentServiceLeaseStoreManagerBuilder WithLeaseContainer(
      ContainerInternal leaseContainer)
    {
      this.leaseContainer = leaseContainer ?? throw new ArgumentNullException(nameof (leaseContainer));
      return this;
    }

    private DocumentServiceLeaseStoreManagerBuilder WithLeasePrefix(string leasePrefix)
    {
      DocumentServiceLeaseStoreManagerOptions options = this.options;
      options.ContainerNamePrefix = leasePrefix ?? throw new ArgumentNullException(nameof (leasePrefix));
      return this;
    }

    private DocumentServiceLeaseStoreManagerBuilder WithRequestOptionsFactory(
      RequestOptionsFactory requestOptionsFactory)
    {
      this.requestOptionsFactory = requestOptionsFactory ?? throw new ArgumentNullException(nameof (requestOptionsFactory));
      return this;
    }

    private DocumentServiceLeaseStoreManagerBuilder WithHostName(string hostName)
    {
      DocumentServiceLeaseStoreManagerOptions options = this.options;
      options.HostName = hostName ?? throw new ArgumentNullException(nameof (hostName));
      return this;
    }

    private DocumentServiceLeaseStoreManager Build()
    {
      if (this.monitoredContainer == null)
        throw new InvalidOperationException("monitoredContainer was not specified");
      if (this.leaseContainer == null)
        throw new InvalidOperationException("leaseContainer was not specified");
      if (this.requestOptionsFactory == null)
        throw new InvalidOperationException("requestOptionsFactory was not specified");
      return (DocumentServiceLeaseStoreManager) new DocumentServiceLeaseStoreManagerCosmos(this.options, this.monitoredContainer, this.leaseContainer, this.requestOptionsFactory);
    }
  }
}
