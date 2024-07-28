// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.IGlobalEndpointManager
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.Azure.Cosmos.Routing
{
  internal interface IGlobalEndpointManager : IDisposable
  {
    ReadOnlyCollection<Uri> ReadEndpoints { get; }

    ReadOnlyCollection<Uri> WriteEndpoints { get; }

    int PreferredLocationCount { get; }

    Uri ResolveServiceEndpoint(DocumentServiceRequest request);

    string GetLocation(Uri endpoint);

    void MarkEndpointUnavailableForRead(Uri endpoint);

    void MarkEndpointUnavailableForWrite(Uri endpoint);

    bool CanUseMultipleWriteLocations(DocumentServiceRequest request);

    void InitializeAccountPropertiesAndStartBackgroundRefresh(AccountProperties databaseAccount);

    Task RefreshLocationAsync(bool forceRefresh = false);
  }
}
