// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.VersionedObjectFactoryCache
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  internal sealed class VersionedObjectFactoryCache
  {
    private static readonly VersionedObjectFactoryCache s_instance = new VersionedObjectFactoryCache();
    private IDictionary<Type, VersionedObjectFactory> m_factoryDictionary = (IDictionary<Type, VersionedObjectFactory>) new Dictionary<Type, VersionedObjectFactory>();

    public VersionedObjectFactory GetFactory<T>() where T : class, new()
    {
      Type key = typeof (T);
      VersionedObjectFactory factory1;
      if (this.m_factoryDictionary.TryGetValue(key, out factory1))
        return factory1;
      FieldInfo field = key.GetField("VersionedObjectFactory", BindingFlags.Static | BindingFlags.Public);
      VersionedObjectFactory factory2;
      if (field == (FieldInfo) null)
        factory2 = new VersionedObjectFactory(new IVersionedObjectCreator[1]
        {
          (IVersionedObjectCreator) new VersionedObjectCreator<T>(1)
        });
      else if (field.FieldType == typeof (VersionedObjectFactory))
        factory2 = (VersionedObjectFactory) field.GetValue((object) null);
      else
        factory2 = new VersionedObjectFactory(new IVersionedObjectCreator[1]
        {
          (IVersionedObjectCreator) new VersionedObjectCreator<T>(1)
        });
      this.m_factoryDictionary = (IDictionary<Type, VersionedObjectFactory>) new Dictionary<Type, VersionedObjectFactory>(this.m_factoryDictionary)
      {
        [key] = factory2
      };
      return factory2;
    }

    public static VersionedObjectFactoryCache Instance => VersionedObjectFactoryCache.s_instance;
  }
}
