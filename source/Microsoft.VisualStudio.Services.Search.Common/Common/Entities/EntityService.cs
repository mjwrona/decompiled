// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class EntityService : IEntityService, IVssFrameworkService
  {
    private IDisposableReadOnlyList<IEntityType> m_entityTypes;
    private List<Type> m_entitiesKnownTypes;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_entityTypes = SearchPlatformHelper.GetExtensions<IEntityType>(systemRequestContext);
      this.m_entitiesKnownTypes = new List<Type>(this.m_entityTypes.Count);
      foreach (object entityType in (IEnumerable<IEntityType>) this.m_entityTypes)
        this.m_entitiesKnownTypes.Add(entityType.GetType());
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_entityTypes == null)
        return;
      this.m_entityTypes.Dispose();
      this.m_entityTypes = (IDisposableReadOnlyList<IEntityType>) null;
      this.m_entitiesKnownTypes = (List<Type>) null;
    }

    public IEnumerable<IEntityType> GetEntityTypes() => (IEnumerable<IEntityType>) this.m_entityTypes;

    public IEnumerable<Type> GetEntitiesKnownTypes() => (IEnumerable<Type>) this.m_entitiesKnownTypes;
  }
}
