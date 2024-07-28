// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IServiceConfigurationReader
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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

    bool EnableAuthorization { get; }

    string SubscriptionId { get; }

    Task InitializeAsync();
  }
}
