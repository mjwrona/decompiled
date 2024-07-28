// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.ICollectionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DefaultServiceImplementation(typeof (FrameworkCollectionService))]
  public interface ICollectionService : IVssFrameworkService
  {
    Collection GetCollection(IVssRequestContext context, IEnumerable<string> propertyNames);

    bool UpdateCollectionOwner(IVssRequestContext context, Guid newOwnerId);

    bool RenameCollection(IVssRequestContext context, string newName);

    bool UpdateProperties(IVssRequestContext context, PropertyBag properties);

    bool DeleteProperties(IVssRequestContext context, IEnumerable<string> propertyNames);

    bool ForceUpdateCollectionOwner(
      IVssRequestContext context,
      Guid newOwnerId,
      string forceUpdateReason);

    bool IsEligibleForTakeOver(IVssRequestContext context);

    bool DeleteAvatar(IVssRequestContext context);

    Collection BackfillPreferredGeography(
      IVssRequestContext collectionContext,
      string geographyCode);
  }
}
