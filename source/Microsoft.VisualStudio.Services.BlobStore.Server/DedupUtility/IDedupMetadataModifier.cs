// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.IDedupMetadataModifier
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility
{
  public interface IDedupMetadataModifier
  {
    Task<MetadataOperationResult> TryModifyKeepUntilAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IKeepUntil data,
      bool throwError);
  }
}
