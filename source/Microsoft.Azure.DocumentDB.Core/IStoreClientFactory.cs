// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IStoreClientFactory
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal interface IStoreClientFactory : IDisposable
  {
    StoreClient CreateStoreClient(
      IAddressResolver addressResolver,
      ISessionContainer sessionContainer,
      IServiceConfigurationReader serviceConfigurationReader,
      IAuthorizationTokenProvider authorizationTokenProvider,
      bool enableRequestDiagnostics = false,
      bool enableReadRequestsFallback = false,
      bool useFallbackClient = true,
      bool useMultipleWriteLocations = false,
      bool detectClientConnectivityIssues = false);
  }
}
