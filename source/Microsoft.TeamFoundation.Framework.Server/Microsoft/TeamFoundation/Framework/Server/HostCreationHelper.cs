// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostCreationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostCreationHelper
  {
    public static bool CheckIfCollectionNameExists(
      IVssRequestContext deploymentContext,
      string collectionName)
    {
      if (HostNameResolver.TryGetCollectionServiceHostId(deploymentContext, collectionName, out Guid _))
        return true;
      deploymentContext.GetService<NameResolutionStore>().DeleteEntries(deploymentContext, (IEnumerable<NameResolutionEntry>) new NameResolutionEntry[2]
      {
        new NameResolutionEntry("Collection", collectionName),
        new NameResolutionEntry("GlobalCollection", collectionName)
      });
      return false;
    }
  }
}
