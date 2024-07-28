// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.IBlobstoreEventGridClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.EventGrid.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [DefaultServiceImplementation(typeof (BlobstoreEventGridClient))]
  public interface IBlobstoreEventGridClient : IVssFrameworkService
  {
    Task PublishEventsAsync(IList<EventGridEvent> events, CancellationToken cancellationToken);
  }
}
