// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiChangelogCursorBasedUpstreamChangeProvider
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public class PyPiChangelogCursorBasedUpstreamChangeProvider : 
    ICursorBasedUpstreamChangeProvider<PyPiChangelogCursor, PyPiPackageIdentity>
  {
    private readonly IPublicUpstreamPyPiClient client;

    public PyPiChangelogCursorBasedUpstreamChangeProvider(IPublicUpstreamPyPiClient client) => this.client = client;

    public async Task<IReadOnlyList<ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>>> GetChanges(
      PyPiChangelogCursor? since)
    {
      if ((object) since == null)
        since = new PyPiChangelogCursor(await this.client.ChangelogLastSerial());
      return (IReadOnlyList<ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>>) await this.client.ChangelogSinceSerial(since);
    }
  }
}
