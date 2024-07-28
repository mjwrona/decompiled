// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.IInternalLocationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [DefaultServiceImplementation(typeof (LocationService), typeof (VirtualLocationService))]
  internal interface IInternalLocationService : 
    ILocationService,
    IVssFrameworkService,
    ILocationDataProvider
  {
    void OnLocationDataChanged(
      IVssRequestContext requestContext,
      LocationDataKind kind,
      ILocationCacheManager<string> cacheManager = null);

    IEnumerable<ServiceDefinition> FindNonInheritedDefinitions(IVssRequestContext requestContext);

    IEnumerable<string> GetActiveProviderUrls(IVssRequestContext requestContext);
  }
}
