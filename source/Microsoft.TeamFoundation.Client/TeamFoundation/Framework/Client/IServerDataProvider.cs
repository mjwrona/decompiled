// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IServerDataProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IServerDataProvider : ILocationService
  {
    Guid InstanceId { get; }

    Guid CachedInstanceId { get; }

    Guid CatalogResourceId { get; }

    ServerCapabilities ServerCapabilities { get; }

    string ServerVersion { get; }

    string ClientCacheDirectoryForInstance { get; }

    string ClientVolatileCacheDirectoryForInstance { get; }

    string ClientCacheDirectoryForUser { get; }

    TeamFoundationIdentity AuthorizedIdentity { get; }

    TeamFoundationIdentity AuthenticatedIdentity { get; }

    bool HasAuthenticated { get; }

    void EnsureAuthenticated();

    void Authenticate();

    string FindServerLocation(Guid serverGuid);

    void Connect(ConnectOptions connectOptions);

    void Disconnect();

    void ReactToPossibleServerUpdate(int serverLastChangeId);
  }
}
