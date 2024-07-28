// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.IResourceAreaService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [DefaultServiceImplementation(typeof (ResourceAreaService))]
  public interface IResourceAreaService : IVssFrameworkService
  {
    IEnumerable<ResourceArea> GetResourceAreas(IVssRequestContext requestContext);

    ResourceArea GetResourceArea(IVssRequestContext requestContext, Guid areaId);

    ResourceArea GetResourceArea(
      IVssRequestContext requestContext,
      Guid areaId,
      bool previewFaultIn);

    IEnumerable<ResourceArea> GetInheritedResourceAreas(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType);

    ResourceArea GetInheritedResourceArea(
      IVssRequestContext requestContext,
      Guid areaId,
      TeamFoundationHostType hostType);

    void RegisterResourceAreas(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      bool registerWithSps = true);

    void RegisterResourceAreas(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      Guid parentIdentifier,
      bool registerWithSps = true);
  }
}
