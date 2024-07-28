// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.IPlatformGraphIdentifierConversionService
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph
{
  [DefaultServiceImplementation(typeof (PlatformGraphIdentifierConversionService))]
  internal interface IPlatformGraphIdentifierConversionService : 
    IGraphIdentifierConversionService,
    IVssFrameworkService
  {
    IList<IdentityKeyMap> UpdateIdentityKeyMaps(
      IVssRequestContext organizationContext,
      IList<IdentityKeyMap> keyMaps);

    IdentityKeyMap GetIdentityKeyMapByCuid(IVssRequestContext organizationContext, Guid cuid);

    IdentityKeyMap GetIdentityKeyMapByStorageKey(
      IVssRequestContext organizationContext,
      Guid storageKey);

    void CategorizeKeyMaps(
      IVssRequestContext requestContext,
      IList<IdentityKeyMap> keyMaps,
      out IList<IdentityKeyMap> keyMapsToCreate,
      out IList<IdentityKeyMap> keyMapsToMunge);
  }
}
