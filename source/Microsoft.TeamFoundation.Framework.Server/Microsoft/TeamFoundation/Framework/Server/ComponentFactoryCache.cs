// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ComponentFactoryCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ComponentFactoryCache
  {
    private Dictionary<Type, ComponentFactory> m_componentFactories = new Dictionary<Type, ComponentFactory>();
    private static readonly ComponentFactoryCache s_instance = new ComponentFactoryCache();

    private ComponentFactoryCache()
    {
    }

    public static ComponentFactoryCache Instance => ComponentFactoryCache.s_instance;

    public ComponentFactory GetComponentFactory<TComponent>() where TComponent : class, ISqlResourceComponent, new()
    {
      Type key = typeof (TComponent);
      ComponentFactory componentFactory1;
      if (this.m_componentFactories.TryGetValue(key, out componentFactory1))
        return componentFactory1;
      FieldInfo field = key.GetField("ComponentFactory", BindingFlags.Static | BindingFlags.Public);
      ComponentFactory componentFactory2;
      if (field == (FieldInfo) null)
        componentFactory2 = new ComponentFactory(new IComponentCreator[1]
        {
          (IComponentCreator) new ComponentCreator<TComponent>(0, true)
        }, key.FullName);
      else
        componentFactory2 = !(field.FieldType != typeof (ComponentFactory)) ? (ComponentFactory) field.GetValue((object) null) : throw new ComponentFactoryException(FrameworkResources.InvalidComponentFactoryFieldType((object) key.FullName));
      if (componentFactory2 == null)
        throw new ComponentFactoryException(FrameworkResources.ComponentFactoryFieldIsNull((object) key.FullName));
      this.m_componentFactories = new Dictionary<Type, ComponentFactory>((IDictionary<Type, ComponentFactory>) this.m_componentFactories)
      {
        [key] = componentFactory2
      };
      return componentFactory2;
    }
  }
}
