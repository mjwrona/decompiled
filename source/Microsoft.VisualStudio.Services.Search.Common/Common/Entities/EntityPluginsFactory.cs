// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityPluginsFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public static class EntityPluginsFactory
  {
    public static IEntityType GetEntityType(
      IVssRequestContext requestContext,
      string entityTypeName)
    {
      IEnumerable<IEntityType> entityTypes = requestContext != null ? requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes() : throw new ArgumentNullException(nameof (requestContext));
      IEntityType entityType1 = (IEntityType) null;
      if (entityTypeName == null)
        throw new ArgumentNullException(nameof (entityTypeName));
      foreach (IEntityType entityType2 in entityTypes)
      {
        if (string.Equals(entityType2.Name, entityTypeName, StringComparison.OrdinalIgnoreCase))
          entityType1 = entityType2;
      }
      return entityType1 != null ? entityType1 : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("No entity type found for the name {0}.", (object) entityTypeName)));
    }

    public static IEntityType GetEntityType(
      IEnumerable<IEntityType> entityTypes,
      string entityTypeName)
    {
      if (entityTypes == null)
        throw new ArgumentNullException(nameof (entityTypes));
      IEntityType entityType1 = (IEntityType) null;
      foreach (IEntityType entityType2 in entityTypes)
      {
        if (string.Equals(entityType2.Name, entityTypeName, StringComparison.OrdinalIgnoreCase))
          entityType1 = entityType2;
      }
      return entityType1 != null ? entityType1 : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("No entity type found for the name {0}.", (object) entityTypeName)));
    }

    public static IEntityType GetEntityType(IVssRequestContext requestContext, int entityTypeID)
    {
      IEnumerable<IEntityType> entityTypes = requestContext != null ? requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes() : throw new ArgumentNullException(nameof (requestContext));
      IEntityType entityType1 = (IEntityType) null;
      foreach (IEntityType entityType2 in entityTypes)
      {
        if (entityType2.ID == entityTypeID)
          entityType1 = entityType2;
      }
      return entityType1 != null ? entityType1 : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("No entity type found for the id {0}", (object) entityTypeID)));
    }

    public static IEntityType GetEntityType(IEnumerable<IEntityType> entityTypes, int entityTypeID)
    {
      if (entityTypes == null)
        throw new ArgumentNullException(nameof (entityTypes));
      IEntityType entityType1 = (IEntityType) null;
      foreach (IEntityType entityType2 in entityTypes)
      {
        if (entityType2.ID == entityTypeID)
          entityType1 = entityType2;
      }
      return entityType1 != null ? entityType1 : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("No entity type found for the id {0}", (object) entityTypeID)));
    }
  }
}
