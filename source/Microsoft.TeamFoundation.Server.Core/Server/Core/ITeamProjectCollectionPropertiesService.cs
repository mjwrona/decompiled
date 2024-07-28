// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ITeamProjectCollectionPropertiesService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  [DefaultServiceImplementation(typeof (TeamProjectCollectionPropertiesService), typeof (VirtualTeamProjectCollectionPropertiesService))]
  public interface ITeamProjectCollectionPropertiesService : IVssFrameworkService
  {
    TeamProjectCollectionProperties GetCollectionProperties(
      IVssRequestContext requestContext,
      Guid collectionId,
      ServiceHostFilterFlags filterFlags,
      bool returnProcessCustomizationProperty = false);

    TeamProjectCollectionProperties GetCollectionPropertiesCached(
      IVssRequestContext requestContext,
      Guid collectionId,
      bool returnProcessCustomizationProperty = false);

    List<TeamProjectCollectionProperties> GetCollectionProperties(
      IVssRequestContext requestContext,
      ServiceHostFilterFlags filterFlags);

    List<TeamProjectCollectionProperties> GetCollectionPropertiesCached(
      IVssRequestContext requestContext);

    List<TeamProjectCollectionProperties> GetCollectionProperties(
      IVssRequestContext requestContext,
      IList<Guid> collectionIds,
      ServiceHostFilterFlags filterFlags);

    List<TeamProjectCollectionProperties> GetCollectionPropertiesCached(
      IVssRequestContext requestContext,
      IList<Guid> collectionIds);
  }
}
