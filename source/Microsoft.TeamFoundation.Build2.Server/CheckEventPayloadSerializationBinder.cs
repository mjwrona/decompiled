// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CheckEventPayloadSerializationBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class CheckEventPayloadSerializationBinder : ISerializationBinder
  {
    private static readonly IDictionary<Type, string> s_knownTypes;
    private static readonly IList<Type> s_unsupportedAssignableTypes = (IList<Type>) new List<Type>()
    {
      typeof (IDynamicMetaObjectProvider),
      typeof (CollectionBase),
      typeof (ICollection),
      typeof (IDisposable),
      typeof (INotifyPropertyChanged),
      typeof (ISupportInitialize)
    };
    private static readonly IList<Type> s_supportedAssignableTypes = (IList<Type>) new List<Type>()
    {
      typeof (TimelineRecord)
    };

    static CheckEventPayloadSerializationBinder()
    {
      CheckEventPayloadSerializationBinder.s_knownTypes = (IDictionary<Type, string>) new Dictionary<Type, string>();
      foreach (Type objectType in (IEnumerable<Type>) new List<Type>()
      {
        typeof (GithubJobsCreateEvent),
        typeof (GithubJobCompletedEvent),
        typeof (GithubBuildSkippedEvent)
      })
        CheckEventPayloadSerializationBinder.DetectKnownCheckEventTypes(CheckEventPayloadSerializationBinder.s_knownTypes, objectType, false);
    }

    public static void DetectKnownCheckEventTypes(
      IDictionary<Type, string> knownTypes,
      Type objectType,
      bool isExplicitlySupportedParentType)
    {
      string name = Assembly.GetAssembly(objectType)?.GetName().Name;
      if (name == null)
        throw new InvalidOperationException("Assembly name cannot be resolved.");
      if (!knownTypes.TryAdd<Type, string>(objectType, name))
        return;
      bool isExplicitlySupportedParentType1 = isExplicitlySupportedParentType || CheckEventPayloadSerializationBinder.IsExplicitlySupportedType(objectType);
      if (!isExplicitlySupportedParentType1 && CheckEventPayloadSerializationBinder.IsUnsupportedType(objectType))
        throw new InvalidOperationException(string.Format("Unsupported {0} type encountered during detection of known types.", (object) objectType));
      foreach (PropertyInfo property in objectType.GetProperties())
      {
        Type propertyType = property.PropertyType;
        if (!CheckEventPayloadSerializationBinder.IsSystemNamespace(propertyType))
          CheckEventPayloadSerializationBinder.DetectKnownCheckEventTypes(knownTypes, propertyType, isExplicitlySupportedParentType1);
      }
    }

    private static bool IsExplicitlySupportedType(Type type) => CheckEventPayloadSerializationBinder.SupportedAssignableTypes.Any<Type>((Func<Type, bool>) (supportedAssignableType => supportedAssignableType.Equals(type)));

    private static bool IsUnsupportedType(Type type)
    {
      int num1 = type.Equals(typeof (object)) ? 1 : 0;
      bool unsupportedType = CheckEventPayloadSerializationBinder.IsAssignableToUnsupportedType(type);
      bool flag = CheckEventPayloadSerializationBinder.IsUntypedCollection(type);
      int num2 = unsupportedType ? 1 : 0;
      return (num1 | num2 | (flag ? 1 : 0)) != 0;
    }

    private static bool IsAssignableToUnsupportedType(Type type) => CheckEventPayloadSerializationBinder.UnsupportedAssignableTypes.Any<Type>((Func<Type, bool>) (unsupportedAssignableType => unsupportedAssignableType.IsAssignableFrom(type)));

    private static bool IsUntypedCollection(Type type)
    {
      bool flag = false;
      if (typeof (IEnumerable).IsAssignableFrom(type))
        flag = (type.GenericTypeArguments.Length != 0 ? 1 : 0) == 0 | ((IEnumerable<Type>) type.GenericTypeArguments).Any<Type>((Func<Type, bool>) (arg => arg.Equals(typeof (object))));
      return flag;
    }

    private static bool IsSystemNamespace(Type type) => type.Namespace == "System";

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
      KeyValuePair<Type, string> assignableTypePair = CheckEventPayloadSerializationBinder.GetAssignableTypePair(serializedType);
      if (!assignableTypePair.Equals((object) null))
      {
        assemblyName = assignableTypePair.Value;
        typeName = assignableTypePair.Key.FullName;
      }
      else
      {
        assemblyName = (string) null;
        typeName = (string) null;
      }
    }

    public Type BindToType(string assemblyName, string typeName)
    {
      KeyValuePair<Type, string> knownTypePairByName = CheckEventPayloadSerializationBinder.GetKnownTypePairByName(typeName);
      string str = !knownTypePairByName.Equals((object) null) ? knownTypePairByName.Value : throw new InvalidOperationException("Assembly of check event payload type not found!");
      if (str != null && str.Equals(assemblyName))
        return knownTypePairByName.Key;
    }

    private static KeyValuePair<Type, string> GetKnownTypePairByName(string serializedTypeName)
    {
      Type serializedType = Type.GetType(serializedTypeName);
      if (serializedType == (Type) null)
      {
        IEnumerable<KeyValuePair<Type, string>> source = CheckEventPayloadSerializationBinder.s_knownTypes.Where<KeyValuePair<Type, string>>((Func<KeyValuePair<Type, string>, bool>) (type => CheckEventPayloadSerializationBinder.IsSerializedTypeKnown(type.Key, serializedTypeName)));
        serializedType = source.Any<KeyValuePair<Type, string>>() ? source.First<KeyValuePair<Type, string>>().Key : throw new InvalidOperationException("Check event payload type not found!");
      }
      return CheckEventPayloadSerializationBinder.GetAssignableTypePair(serializedType);
    }

    private static KeyValuePair<Type, string> GetAssignableTypePair(Type serializedType)
    {
      IEnumerable<KeyValuePair<Type, string>> source = CheckEventPayloadSerializationBinder.s_knownTypes.Where<KeyValuePair<Type, string>>((Func<KeyValuePair<Type, string>, bool>) (type => CheckEventPayloadSerializationBinder.IsSerializedTypeAssignable(type.Key, serializedType)));
      return source.Any<KeyValuePair<Type, string>>() ? source.Last<KeyValuePair<Type, string>>() : throw new InvalidOperationException("Check event payload type not assignable!");
    }

    private static bool IsSerializedTypeKnown(Type knownType, string serializedTypeName) => (object) knownType != null && knownType.FullName != null && knownType.FullName.Equals(serializedTypeName);

    private static bool IsSerializedTypeAssignable(Type knownType, Type serializedType) => knownType.IsAssignableFrom(serializedType);

    public static IDictionary<Type, string> KnownTypes => CheckEventPayloadSerializationBinder.s_knownTypes;

    public static IList<Type> UnsupportedAssignableTypes => CheckEventPayloadSerializationBinder.s_unsupportedAssignableTypes;

    public static IList<Type> SupportedAssignableTypes => CheckEventPayloadSerializationBinder.s_supportedAssignableTypes;
  }
}
