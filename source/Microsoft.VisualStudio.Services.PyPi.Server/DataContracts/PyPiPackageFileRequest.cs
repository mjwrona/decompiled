// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.DataContracts.PyPiPackageFileRequest
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;

namespace Microsoft.VisualStudio.Services.PyPi.Server.DataContracts
{
  public class PyPiPackageFileRequest : PackageFileRequest<PyPiPackageIdentity>
  {
    public PyPiPackageFile PackageFile { get; }

    public PyPiPackageFileRequest(
      IFeedRequest feedRequest,
      PyPiPackageIdentity packageId,
      PyPiPackageFile packageFile)
      : base(feedRequest, packageId, packageFile.Path)
    {
      this.PackageFile = packageFile;
    }
  }
}
