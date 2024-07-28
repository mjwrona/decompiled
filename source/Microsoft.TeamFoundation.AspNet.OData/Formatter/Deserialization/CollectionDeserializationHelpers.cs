// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.CollectionDeserializationHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  internal static class CollectionDeserializationHelpers
  {
    private static readonly Type[] _emptyTypeArray = new Type[0];
    private static readonly object[] _emptyObjectArray = new object[0];
    private static readonly MethodInfo _toArrayMethodInfo = typeof (Enumerable).GetMethod("ToArray");

    public static void AddToCollection(
      this IEnumerable items,
      IEnumerable collection,
      Type elementType,
      Type resourceType,
      string propertyName,
      Type propertyType)
    {
      MethodInfo addMethod = (MethodInfo) null;
      if (!(collection is IList list))
      {
        addMethod = collection.GetType().GetMethod("Add", new Type[1]
        {
          elementType
        });
        if (addMethod == (MethodInfo) null)
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CollectionShouldHaveAddMethod, (object) propertyType.FullName, (object) propertyName, (object) resourceType.FullName));
      }
      else if (list.GetType().IsArray)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.GetOnlyCollectionCannotBeArray, (object) propertyName, (object) resourceType.FullName));
      items.AddToCollectionCore(collection, elementType, list, addMethod);
    }

    public static void AddToCollection(
      this IEnumerable items,
      IEnumerable collection,
      Type elementType,
      string paramName,
      Type paramType)
    {
      MethodInfo addMethod = (MethodInfo) null;
      if (!(collection is IList list))
      {
        addMethod = collection.GetType().GetMethod("Add", new Type[1]
        {
          elementType
        });
        if (addMethod == (MethodInfo) null)
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CollectionParameterShouldHaveAddMethod, (object) paramType, (object) paramName));
      }
      items.AddToCollectionCore(collection, elementType, list, addMethod);
    }

    private static void AddToCollectionCore(
      this IEnumerable items,
      IEnumerable collection,
      Type elementType,
      IList list,
      MethodInfo addMethod)
    {
      bool isNonstandardEdmPrimitive;
      EdmLibHelpers.IsNonstandardEdmPrimitive(elementType, out isNonstandardEdmPrimitive);
      foreach (object obj1 in items)
      {
        object obj2 = obj1;
        if (isNonstandardEdmPrimitive && obj2 != null)
          obj2 = EdmPrimitiveHelpers.ConvertPrimitiveValue(obj2, elementType);
        if (list != null)
          list.Add(obj2);
        else
          addMethod.Invoke((object) collection, new object[1]
          {
            obj2
          });
      }
    }

    public static void Clear(this IEnumerable collection, string propertyName, Type resourceType)
    {
      MethodInfo method = collection.GetType().GetMethod(nameof (Clear), CollectionDeserializationHelpers._emptyTypeArray);
      if (method == (MethodInfo) null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CollectionShouldHaveClearMethod, (object) collection.GetType().FullName, (object) propertyName, (object) resourceType.FullName));
      method.Invoke((object) collection, CollectionDeserializationHelpers._emptyObjectArray);
    }

    public static bool TryCreateInstance(
      Type collectionType,
      IEdmCollectionTypeReference edmCollectionType,
      Type elementType,
      out IEnumerable instance)
    {
      if (collectionType == typeof (EdmComplexObjectCollection))
      {
        instance = (IEnumerable) new EdmComplexObjectCollection(edmCollectionType);
        return true;
      }
      if (collectionType == typeof (EdmEntityObjectCollection))
      {
        instance = (IEnumerable) new EdmEntityObjectCollection(edmCollectionType);
        return true;
      }
      if (collectionType == typeof (EdmEnumObjectCollection))
      {
        instance = (IEnumerable) new EdmEnumObjectCollection(edmCollectionType);
        return true;
      }
      if (collectionType.IsGenericType())
      {
        Type genericTypeDefinition = collectionType.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (IEnumerable<>) || genericTypeDefinition == typeof (ICollection<>) || genericTypeDefinition == typeof (IList<>))
        {
          instance = Activator.CreateInstance(typeof (List<>).MakeGenericType(elementType)) as IEnumerable;
          return true;
        }
      }
      if (collectionType.IsArray)
      {
        instance = Activator.CreateInstance(typeof (List<>).MakeGenericType(elementType)) as IEnumerable;
        return true;
      }
      if (collectionType.GetConstructor(Type.EmptyTypes) != (ConstructorInfo) null && !TypeHelper.IsAbstract(collectionType))
      {
        instance = Activator.CreateInstance(collectionType) as IEnumerable;
        return true;
      }
      instance = (IEnumerable) null;
      return false;
    }

    public static IEnumerable ToArray(IEnumerable value, Type elementType) => CollectionDeserializationHelpers._toArrayMethodInfo.MakeGenericMethod(elementType).Invoke((object) null, new object[1]
    {
      (object) value
    }) as IEnumerable;
  }
}
