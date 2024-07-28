// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IPublisherDomainVerificationService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (PublisherDomainVerificationService))]
  public interface IPublisherDomainVerificationService : IVssFrameworkService
  {
    Guid FetchDomainToken(IVssRequestContext requestContext, string publisherName);

    void VerifyDomainToken(IVssRequestContext requestContext, string publisherName);

    void MarkPublisherDomainAsVerified(IVssRequestContext requestContext, string publisherName);

    void MarkPublisherDomainAsUnverified(IVssRequestContext requestContext, string publisherName);

    List<PublisherDomainModel> FetchVerifiedPublishers(IVssRequestContext requestContext);

    bool IsDomainTokenInDnsRecords(
      IVssRequestContext requestContext,
      Publisher publisher,
      string token = null);
  }
}
