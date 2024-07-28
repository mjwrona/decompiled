// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseContainerInMemory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseContainerInMemory : DocumentServiceLeaseContainer
  {
    private readonly ConcurrentDictionary<string, DocumentServiceLease> container;

    public DocumentServiceLeaseContainerInMemory(
      ConcurrentDictionary<string, DocumentServiceLease> container)
    {
      this.container = container;
    }

    public override Task<IReadOnlyList<DocumentServiceLease>> GetAllLeasesAsync() => Task.FromResult<IReadOnlyList<DocumentServiceLease>>((IReadOnlyList<DocumentServiceLease>) this.container.Values.ToList<DocumentServiceLease>().AsReadOnly());

    public override Task<IEnumerable<DocumentServiceLease>> GetOwnedLeasesAsync() => Task.FromResult<IEnumerable<DocumentServiceLease>>(this.container.Values.AsEnumerable<DocumentServiceLease>());
  }
}
