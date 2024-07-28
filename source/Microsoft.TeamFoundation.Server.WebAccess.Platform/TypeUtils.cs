// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TypeUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TypeUtils
  {
    private static readonly MethodInfo sm_replaceCollectionMethod = typeof (TypeUtils).GetMethod("ReplaceCollectionImpl", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly MethodInfo sm_replaceDictionaryMethod = typeof (TypeUtils).GetMethod("ReplaceDictionaryImpl", BindingFlags.Static | BindingFlags.NonPublic);

    public static IEnumerable<Type> GetAllTypes(Func<Type, bool> filter) => TypeUtils.GetAllTypes(BuildManager.GetReferencedAssemblies().OfType<Assembly>(), filter);

    public static IEnumerable<Type> GetAllTypes(
      IEnumerable<Assembly> assemblies,
      Func<Type, bool> filter)
    {
      IEnumerable<Type> source = assemblies.SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (assembly =>
      {
        try
        {
          return (IEnumerable<Type>) assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Warning, "WebAccess", TfsTraceLayers.Controller, nameof (GetAllTypes), (Exception) ex);
          return (IEnumerable<Type>) ex.Types;
        }
      }));
      if (filter != null)
        source = source.Where<Type>(filter);
      return source;
    }

    public static bool IsControllerType(Type t) => t != (Type) null && t.IsPublic && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) && !t.IsAbstract && typeof (IController).IsAssignableFrom(t);

    private static bool CheckGenericInterface(Type type, Type interfaceType) => type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType;

    public static Type ExtractGenericInterface(Type queryType, Type interfaceType) => TypeUtils.CheckGenericInterface(queryType, interfaceType) ? queryType : ((IEnumerable<Type>) queryType.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (t => TypeUtils.CheckGenericInterface(t, interfaceType)));

    public static object CreateModelInstance(Type modelType)
    {
      Type type = modelType;
      if (modelType.IsGenericType)
      {
        Type genericTypeDefinition = modelType.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (IDictionary<,>))
          type = typeof (Dictionary<,>).MakeGenericType(modelType.GetGenericArguments());
        else if (genericTypeDefinition == typeof (IEnumerable<>) || genericTypeDefinition == typeof (ICollection<>) || genericTypeDefinition == typeof (IList<>))
          type = typeof (List<>).MakeGenericType(modelType.GetGenericArguments());
      }
      return Activator.CreateInstance(type);
    }

    public static void ReplaceCollection(
      Type collectionItemType,
      object collection,
      object newContents)
    {
      TypeUtils.sm_replaceCollectionMethod.MakeGenericMethod(collectionItemType).Invoke((object) null, new object[2]
      {
        collection,
        newContents
      });
    }

    private static void ReplaceCollectionImpl<T>(ICollection<T> collection, IEnumerable newContents)
    {
      collection.Clear();
      if (newContents == null)
        return;
      foreach (object newContent in newContents)
      {
        if (!(newContent is T obj1))
          obj1 = default (T);
        T obj2 = obj1;
        collection.Add(obj2);
      }
    }

    public static void ReplaceDictionary(
      Type keyType,
      Type valueType,
      object dictionary,
      object newContents)
    {
      TypeUtils.sm_replaceDictionaryMethod.MakeGenericMethod(keyType, valueType).Invoke((object) null, new object[2]
      {
        dictionary,
        newContents
      });
    }

    private static void ReplaceDictionaryImpl<TKey, TValue>(
      IDictionary<TKey, TValue> dictionary,
      IEnumerable<KeyValuePair<object, object>> newContents)
    {
      dictionary.Clear();
      foreach (KeyValuePair<object, object> newContent in newContents)
      {
        TKey key = (TKey) newContent.Key;
        TValue obj = newContent.Value is TValue ? (TValue) newContent.Value : default (TValue);
        dictionary[key] = obj;
      }
    }
  }
}
