// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.UrlHostResolutionServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class UrlHostResolutionServiceFacade : IUrlHostResolutionService
  {
    private readonly IVssRequestContext requestContext;
    private readonly Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService urlHostResolutionService;

    public UrlHostResolutionServiceFacade(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService urlHostResolutionService)
    {
      this.requestContext = requestContext;
      this.urlHostResolutionService = urlHostResolutionService;
    }

    public Uri GetHostUri(Guid hostId, Guid serviceIdentifier = default (Guid)) => this.urlHostResolutionService.GetHostUri(this.requestContext, hostId, serviceIdentifier);
  }
}
