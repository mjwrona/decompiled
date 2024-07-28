// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IFirstPartyPublisherAccessService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (FirstPartyPublisherAccessService))]
  public interface IFirstPartyPublisherAccessService : IVssFrameworkService
  {
    bool CheckAtMicrosftDotComAccessIfRequired(
      IVssRequestContext requestContext,
      Publisher publisher);

    bool IsMicrosoftEmployee(
      IVssRequestContext requestContext,
      Publisher publisher,
      PublishedExtension existingExtension);

    bool IsMicrosoftPublisher(IVssRequestContext requestContext, Publisher publisher);

    bool IsInternalEmployee(IVssRequestContext requestContext);

    bool IsInternalPartner(IVssRequestContext requestContext);
  }
}
