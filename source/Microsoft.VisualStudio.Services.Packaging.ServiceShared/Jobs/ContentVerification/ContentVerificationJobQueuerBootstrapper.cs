// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ContentVerification.ContentVerificationJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ContentVerification
{
  public class ContentVerificationJobQueuerBootstrapper : IBootstrapper<IFeedJobQueuer>
  {
    private readonly IVssRequestContext requestContext;
    private readonly JobCreationInfo jobCreationInfo;

    public ContentVerificationJobQueuerBootstrapper(
      IVssRequestContext requestContext,
      JobCreationInfo jobCreationInfo)
    {
      this.requestContext = requestContext;
      this.jobCreationInfo = jobCreationInfo;
    }

    public IFeedJobQueuer Bootstrap() => new FeedJobQueuerBasicBootstrapper(this.requestContext, ContentVerificationJobConstants.ContentVerificationJobType, this.jobCreationInfo).Bootstrap();
  }
}
