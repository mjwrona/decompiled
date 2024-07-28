// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IAdminBlobStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IAdminBlobStore : IBlobStore, IVssFrameworkService
  {
    Task<IDictionary<BlobIdentifier, RemoveReferencesResult>> RemoveReferencesWithResultsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds);
  }
}
