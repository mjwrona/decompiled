// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusDataContractResolver
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class ServiceBusDataContractResolver : DataContractResolver
  {
    public static DataContractSerializer GetDataContractSerializer(Type type)
    {
      ServiceBusDataContractResolver contractResolver = new ServiceBusDataContractResolver();
      return new DataContractSerializer(type, (IEnumerable<Type>) null, int.MaxValue, false, false, (IDataContractSurrogate) null, (DataContractResolver) contractResolver);
    }

    public override Type ResolveName(
      string typeName,
      string typeNamespace,
      Type declaredType,
      DataContractResolver knownTypeResolver)
    {
      Type type = (Type) null;
      if (knownTypeResolver != null)
        type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, knownTypeResolver);
      if ((Type) null == type)
        type = Assembly.Load(new AssemblyName()
        {
          Name = typeName
        }).GetType(typeNamespace);
      return type;
    }

    public override bool TryResolveType(
      Type type,
      Type declaredType,
      DataContractResolver knownTypeResolver,
      out XmlDictionaryString typeName,
      out XmlDictionaryString typeNamespace)
    {
      bool flag = false;
      if (type.GetCustomAttributes(typeof (ServiceEventObjectAttribute), true).Length != 0)
      {
        string name = new AssemblyName(type.Assembly.ToString()).Name;
        string fullName = type.FullName;
        typeName = new XmlDictionaryString(XmlDictionary.Empty, name, 0);
        typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, fullName, 0);
        flag = true;
      }
      else if (knownTypeResolver != null)
      {
        flag = knownTypeResolver.TryResolveType(type, declaredType, knownTypeResolver, out typeName, out typeNamespace);
      }
      else
      {
        typeName = (XmlDictionaryString) null;
        typeNamespace = (XmlDictionaryString) null;
      }
      return flag;
    }
  }
}
