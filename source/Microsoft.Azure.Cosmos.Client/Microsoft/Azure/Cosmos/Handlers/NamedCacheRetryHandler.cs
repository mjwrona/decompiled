// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.NamedCacheRetryHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal class NamedCacheRetryHandler : AbstractRetryHandler
  {
    internal override Task<IDocumentClientRetryPolicy> GetRetryPolicyAsync(RequestMessage request) => Task.FromResult<IDocumentClientRetryPolicy>((IDocumentClientRetryPolicy) new InvalidPartitionExceptionRetryPolicy((IDocumentClientRetryPolicy) null));
  }
}
