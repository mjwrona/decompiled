// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher.IContentStitcherTokenService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher
{
  [DefaultServiceImplementation(typeof (ContentStitcherTokenService))]
  public interface IContentStitcherTokenService : IVssFrameworkService
  {
    Task<AuthenticationResult> GetAccessTokenAsync(IVssRequestContext requestContext);
  }
}
