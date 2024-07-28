// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IServiceConfigurationReader
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal interface IServiceConfigurationReader
  {
    string DatabaseAccountId { get; }

    Uri DatabaseAccountApiEndpoint { get; }

    ReplicationPolicy UserReplicationPolicy { get; }

    ReplicationPolicy SystemReplicationPolicy { get; }

    ConsistencyLevel DefaultConsistencyLevel { get; }

    ReadPolicy ReadPolicy { get; }

    string PrimaryMasterKey { get; }

    string SecondaryMasterKey { get; }

    string PrimaryReadonlyMasterKey { get; }

    string SecondaryReadonlyMasterKey { get; }

    string ResourceSeedKey { get; }

    string SubscriptionId { get; }

    Task InitializeAsync();
  }
}
