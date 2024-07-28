// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage.BatchPackageFileRequest`1
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage
{
  public class BatchPackageFileRequest<TIdentity> : FeedRequest where TIdentity : IPackageIdentity
  {
    public BatchPackageFileRequest(
      IFeedRequest feedRequest,
      IReadOnlyCollection<IPackageFileRequest<TIdentity>> requests)
      : base(feedRequest)
    {
      this.Requests = requests;
    }

    public IReadOnlyCollection<IPackageFileRequest<TIdentity>> Requests { get; }
  }
}
