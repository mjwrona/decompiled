// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosAccountServiceConfiguration
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosAccountServiceConfiguration : IServiceConfigurationReader
  {
    private Func<Task<AccountProperties>> accountPropertiesTaskFunc { get; }

    internal AccountProperties AccountProperties { get; private set; }

    public CosmosAccountServiceConfiguration(
      Func<Task<AccountProperties>> accountPropertiesTaskFunc)
    {
      this.accountPropertiesTaskFunc = accountPropertiesTaskFunc != null ? accountPropertiesTaskFunc : throw new ArgumentNullException(nameof (accountPropertiesTaskFunc));
    }

    public IDictionary<string, object> QueryEngineConfiguration => this.AccountProperties.QueryEngineConfiguration;

    public string DatabaseAccountId => throw new NotImplementedException();

    public Uri DatabaseAccountApiEndpoint => throw new NotImplementedException();

    public ReplicationPolicy UserReplicationPolicy => this.AccountProperties.ReplicationPolicy;

    public ReplicationPolicy SystemReplicationPolicy => this.AccountProperties.SystemReplicationPolicy;

    public Microsoft.Azure.Documents.ConsistencyLevel DefaultConsistencyLevel => (Microsoft.Azure.Documents.ConsistencyLevel) this.AccountProperties.Consistency.DefaultConsistencyLevel;

    public ReadPolicy ReadPolicy => this.AccountProperties.ReadPolicy;

    public string PrimaryMasterKey => throw new NotImplementedException();

    public string SecondaryMasterKey => throw new NotImplementedException();

    public string PrimaryReadonlyMasterKey => throw new NotImplementedException();

    public string SecondaryReadonlyMasterKey => throw new NotImplementedException();

    public string ResourceSeedKey => throw new NotImplementedException();

    public bool EnableAuthorization => true;

    public string SubscriptionId => throw new NotImplementedException();

    public async Task InitializeAsync()
    {
      if (this.AccountProperties != null)
        return;
      this.AccountProperties = await this.accountPropertiesTaskFunc();
    }
  }
}
