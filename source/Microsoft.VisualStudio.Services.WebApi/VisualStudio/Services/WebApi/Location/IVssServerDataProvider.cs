// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.IVssServerDataProvider
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  public interface IVssServerDataProvider : ILocationDataProvider
  {
    bool HasConnected { get; }

    Task<Microsoft.VisualStudio.Services.Identity.Identity> GetAuthorizedIdentityAsync(
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Microsoft.VisualStudio.Services.Identity.Identity> GetAuthenticatedIdentityAsync(
      CancellationToken cancellationToken = default (CancellationToken));

    Task ConnectAsync(ConnectOptions connectOptions, CancellationToken cancellationToken = default (CancellationToken));

    Task DisconnectAsync(CancellationToken cancellationToken = default (CancellationToken));
  }
}
