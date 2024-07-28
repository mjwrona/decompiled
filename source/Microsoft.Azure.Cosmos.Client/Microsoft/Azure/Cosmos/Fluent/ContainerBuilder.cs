// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.ContainerBuilder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class ContainerBuilder : ContainerDefinition<ContainerBuilder>
  {
    private readonly Database database;
    private readonly CosmosClientContext clientContext;
    private readonly Uri containerUri;
    private UniqueKeyPolicy uniqueKeyPolicy;
    private ConflictResolutionPolicy conflictResolutionPolicy;
    private ChangeFeedPolicy changeFeedPolicy;
    private ClientEncryptionPolicy clientEncryptionPolicy;

    protected ContainerBuilder()
    {
    }

    public ContainerBuilder(Database database, string name, string partitionKeyPath)
    {
      string name1 = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof (name));
      string partitionKeyPath1 = !string.IsNullOrEmpty(partitionKeyPath) ? partitionKeyPath : throw new ArgumentNullException(nameof (partitionKeyPath));
      // ISSUE: explicit constructor call
      base.\u002Ector(name1, partitionKeyPath1);
      this.database = database ?? throw new ArgumentNullException(nameof (database));
      this.clientContext = database.Client.ClientContext;
      this.containerUri = UriFactory.CreateDocumentCollectionUri(this.database.Id, name);
    }

    public UniqueKeyDefinition WithUniqueKey() => new UniqueKeyDefinition(this, (Action<UniqueKey>) (uniqueKey => this.AddUniqueKey(uniqueKey)));

    public ConflictResolutionDefinition WithConflictResolution() => new ConflictResolutionDefinition(this, (Action<ConflictResolutionPolicy>) (conflictPolicy => this.AddConflictResolution(conflictPolicy)));

    internal ChangeFeedPolicyDefinition WithChangeFeedPolicy(TimeSpan retention) => new ChangeFeedPolicyDefinition(this, retention, (Action<ChangeFeedPolicy>) (changeFeedPolicy => this.AddChangeFeedPolicy(changeFeedPolicy)));

    public ClientEncryptionPolicyDefinition WithClientEncryptionPolicy() => new ClientEncryptionPolicyDefinition(this, (Action<ClientEncryptionPolicy>) (clientEncryptionPolicy => this.AddClientEncryptionPolicy(clientEncryptionPolicy)));

    public ClientEncryptionPolicyDefinition WithClientEncryptionPolicy(int policyFormatVersion) => new ClientEncryptionPolicyDefinition(this, (Action<ClientEncryptionPolicy>) (clientEncryptionPolicy => this.AddClientEncryptionPolicy(clientEncryptionPolicy)), policyFormatVersion);

    public async Task<ContainerResponse> CreateAsync(
      ThroughputProperties throughputProperties,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.database.CreateContainerAsync(this.Build(), throughputProperties, cancellationToken: cancellationToken);
    }

    public async Task<ContainerResponse> CreateIfNotExistsAsync(
      ThroughputProperties throughputProperties,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.database.CreateContainerIfNotExistsAsync(this.Build(), throughputProperties, cancellationToken: cancellationToken);
    }

    public async Task<ContainerResponse> CreateAsync(
      int? throughput = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.database.CreateContainerAsync(this.Build(), throughput, cancellationToken: cancellationToken);
    }

    public async Task<ContainerResponse> CreateIfNotExistsAsync(
      int? throughput = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.database.CreateContainerIfNotExistsAsync(this.Build(), throughput, cancellationToken: cancellationToken);
    }

    public new ContainerProperties Build()
    {
      ContainerProperties containerProperties = base.Build();
      if (this.uniqueKeyPolicy != null)
        containerProperties.UniqueKeyPolicy = this.uniqueKeyPolicy;
      if (this.conflictResolutionPolicy != null)
        containerProperties.ConflictResolutionPolicy = this.conflictResolutionPolicy;
      if (this.changeFeedPolicy != null)
        containerProperties.ChangeFeedPolicy = this.changeFeedPolicy;
      if (this.clientEncryptionPolicy != null)
        containerProperties.ClientEncryptionPolicy = this.clientEncryptionPolicy;
      return containerProperties;
    }

    private void AddUniqueKey(UniqueKey uniqueKey)
    {
      if (this.uniqueKeyPolicy == null)
        this.uniqueKeyPolicy = new UniqueKeyPolicy();
      this.uniqueKeyPolicy.UniqueKeys.Add(uniqueKey);
    }

    private void AddConflictResolution(ConflictResolutionPolicy conflictResolutionPolicy)
    {
      if (conflictResolutionPolicy.Mode == ConflictResolutionMode.Custom && !string.IsNullOrEmpty(conflictResolutionPolicy.ResolutionProcedure))
      {
        this.clientContext.ValidateResource(conflictResolutionPolicy.ResolutionProcedure);
        conflictResolutionPolicy.ResolutionProcedure = UriFactory.CreateStoredProcedureUri(this.containerUri.ToString(), conflictResolutionPolicy.ResolutionProcedure).ToString();
      }
      this.conflictResolutionPolicy = conflictResolutionPolicy;
    }

    private void AddChangeFeedPolicy(ChangeFeedPolicy changeFeedPolicy) => this.changeFeedPolicy = changeFeedPolicy;

    private void AddClientEncryptionPolicy(ClientEncryptionPolicy clientEncryptionPolicy) => this.clientEncryptionPolicy = clientEncryptionPolicy;
  }
}
