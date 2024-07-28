// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.DataContractKnownTypesPluginService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class DataContractKnownTypesPluginService : IVssFrameworkService
  {
    private IDisposableReadOnlyList<IndexingProperties> m_indexingPropertiesPlugins;
    private List<Type> m_indexingPropertiesKnownTypes;
    private IDisposableReadOnlyList<TFSEntityAttributes> m_tfsEntityAttributesPlugins;
    private List<Type> m_tfsEntityAttributesKnownTypes;
    private IDisposableReadOnlyList<ChangeEventData> m_changeEventDataPlugins;
    private List<Type> m_changeEventDataKnownTypes;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_indexingPropertiesPlugins = SearchPlatformHelper.GetExtensions<IndexingProperties>(systemRequestContext);
      this.m_indexingPropertiesKnownTypes = new List<Type>(this.m_indexingPropertiesPlugins.Count);
      foreach (object propertiesPlugin in (IEnumerable<IndexingProperties>) this.m_indexingPropertiesPlugins)
        this.m_indexingPropertiesKnownTypes.Add(propertiesPlugin.GetType());
      this.m_tfsEntityAttributesPlugins = SearchPlatformHelper.GetExtensions<TFSEntityAttributes>(systemRequestContext);
      this.m_tfsEntityAttributesKnownTypes = new List<Type>(this.m_tfsEntityAttributesPlugins.Count);
      foreach (object attributesPlugin in (IEnumerable<TFSEntityAttributes>) this.m_tfsEntityAttributesPlugins)
        this.m_tfsEntityAttributesKnownTypes.Add(attributesPlugin.GetType());
      this.m_changeEventDataPlugins = SearchPlatformHelper.GetExtensions<ChangeEventData>(systemRequestContext);
      this.m_changeEventDataKnownTypes = new List<Type>(this.m_changeEventDataPlugins.Count);
      foreach (object changeEventDataPlugin in (IEnumerable<ChangeEventData>) this.m_changeEventDataPlugins)
        this.m_changeEventDataKnownTypes.Add(changeEventDataPlugin.GetType());
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_indexingPropertiesPlugins != null)
      {
        this.m_indexingPropertiesPlugins.Dispose();
        this.m_indexingPropertiesPlugins = (IDisposableReadOnlyList<IndexingProperties>) null;
        this.m_indexingPropertiesKnownTypes = (List<Type>) null;
      }
      if (this.m_tfsEntityAttributesPlugins != null)
      {
        this.m_tfsEntityAttributesPlugins.Dispose();
        this.m_tfsEntityAttributesPlugins = (IDisposableReadOnlyList<TFSEntityAttributes>) null;
        this.m_tfsEntityAttributesKnownTypes = (List<Type>) null;
      }
      if (this.m_changeEventDataPlugins == null)
        return;
      this.m_changeEventDataPlugins.Dispose();
      this.m_changeEventDataPlugins = (IDisposableReadOnlyList<ChangeEventData>) null;
      this.m_changeEventDataKnownTypes = (List<Type>) null;
    }

    public virtual IEnumerable<Type> GetKnownTypes(Type type)
    {
      if (type == typeof (IndexingProperties))
        return (IEnumerable<Type>) this.m_indexingPropertiesKnownTypes;
      if (type == typeof (TFSEntityAttributes))
        return (IEnumerable<Type>) this.m_tfsEntityAttributesKnownTypes;
      return type == typeof (ChangeEventData) ? (IEnumerable<Type>) this.m_changeEventDataKnownTypes : (IEnumerable<Type>) null;
    }
  }
}
