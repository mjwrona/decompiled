// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.ContainerDefinition`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public abstract class ContainerDefinition<T> where T : ContainerDefinition<T>
  {
    private readonly string containerName;
    private readonly string partitionKeyPath;
    private int? defaultTimeToLive;
    private IndexingPolicy indexingPolicy;
    private string timeToLivePropertyPath;
    private PartitionKeyDefinitionVersion? partitionKeyDefinitionVersion;

    public ContainerDefinition()
    {
    }

    internal ContainerDefinition(string name, string partitionKeyPath = null)
    {
      this.containerName = name;
      this.partitionKeyPath = partitionKeyPath;
    }

    public T WithPartitionKeyDefinitionVersion(
      PartitionKeyDefinitionVersion partitionKeyDefinitionVersion)
    {
      this.partitionKeyDefinitionVersion = new PartitionKeyDefinitionVersion?(partitionKeyDefinitionVersion);
      return (T) this;
    }

    public T WithDefaultTimeToLive(TimeSpan defaultTtlTimeSpan)
    {
      this.defaultTimeToLive = new int?((int) defaultTtlTimeSpan.TotalSeconds);
      return (T) this;
    }

    public T WithDefaultTimeToLive(int defaultTtlInSeconds)
    {
      this.defaultTimeToLive = defaultTtlInSeconds >= -1 ? new int?(defaultTtlInSeconds) : throw new ArgumentOutOfRangeException(nameof (defaultTtlInSeconds));
      return (T) this;
    }

    public T WithTimeToLivePropertyPath(string propertyPath)
    {
      this.timeToLivePropertyPath = !string.IsNullOrEmpty(propertyPath) ? propertyPath : throw new ArgumentNullException(nameof (propertyPath));
      return (T) this;
    }

    public IndexingPolicyDefinition<T> WithIndexingPolicy()
    {
      if (this.indexingPolicy != null)
        throw new NotSupportedException();
      return new IndexingPolicyDefinition<T>((T) this, (Action<IndexingPolicy>) (indexingPolicy => this.WithIndexingPolicy(indexingPolicy)));
    }

    public ContainerProperties Build()
    {
      ContainerProperties containerProperties = new ContainerProperties(this.containerName, this.partitionKeyPath);
      if (this.indexingPolicy != null)
        containerProperties.IndexingPolicy = this.indexingPolicy;
      if (this.defaultTimeToLive.HasValue)
        containerProperties.DefaultTimeToLive = new int?(this.defaultTimeToLive.Value);
      if (this.timeToLivePropertyPath != null)
        containerProperties.TimeToLivePropertyPath = this.timeToLivePropertyPath;
      if (this.partitionKeyDefinitionVersion.HasValue)
        containerProperties.PartitionKeyDefinitionVersion = new PartitionKeyDefinitionVersion?(this.partitionKeyDefinitionVersion.Value);
      containerProperties.ValidateRequiredProperties();
      return containerProperties;
    }

    private void WithIndexingPolicy(IndexingPolicy indexingPolicy) => this.indexingPolicy = indexingPolicy;
  }
}
