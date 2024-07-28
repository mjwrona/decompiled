// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.Catalog.Objects.CatalogObjectUtilities
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Common.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CatalogObjectUtilities
  {
    public static bool TryGetResourceTypeIdentifier(Type catalogObjectType, out Guid id)
    {
      FieldInfo field = catalogObjectType.GetField("ResourceTypeIdentifier", BindingFlags.Static | BindingFlags.Public);
      if (field != (FieldInfo) null && field.FieldType == typeof (Guid))
      {
        id = (Guid) field.GetValue((object) null);
        return true;
      }
      id = Guid.Empty;
      return false;
    }

    public static Guid GetResourceTypeIdentifier(Type catalogObjectType)
    {
      Guid id;
      if (!CatalogObjectUtilities.TryGetResourceTypeIdentifier(catalogObjectType, out id))
        throw new InvalidOperationException("Entity type " + catalogObjectType.FullName + " does not define a static ResourceIdentifierType field of type Guid.");
      return id;
    }

    public static Guid GetResourceTypeIdentifier<T>() => CatalogObjectUtilities.GetResourceTypeIdentifier(typeof (T));

    public static Guid[] GetResourceTypeIdentifierFilter<T>()
    {
      Guid id;
      if (!CatalogObjectUtilities.TryGetResourceTypeIdentifier(typeof (T), out id))
        return (Guid[]) null;
      return new Guid[1]{ id };
    }

    public static ICollection<Guid> GetKnownDescendantTypes<T>() => CatalogObjectUtilities.GetKnownDescendantTypes(typeof (T));

    public static ICollection<Guid> GetKnownDescendantTypes(Type catalogObjectType)
    {
      Queue<Type> source1 = new Queue<Type>();
      IList<Type> typeList = (IList<Type>) new List<Type>();
      IList<Type> source2 = (IList<Type>) new List<Type>();
      source1.Enqueue(catalogObjectType);
      while (source1.Any<Type>())
      {
        Type catalogObjectType1 = source1.Dequeue();
        typeList.Add(catalogObjectType1);
        IEnumerable<Type> knownChildTypes = CatalogObjectUtilities.GetKnownChildTypes(catalogObjectType1);
        if (knownChildTypes != null)
        {
          foreach (Type type in knownChildTypes)
          {
            if (!source2.Contains(type))
              source2.Add(type);
            if (!typeList.Contains(type) && !source1.Contains(type))
              source1.Enqueue(type);
          }
        }
      }
      return !source2.Any<Type>() ? (ICollection<Guid>) null : (ICollection<Guid>) source2.Select<Type, Guid>((Func<Type, Guid>) (r => CatalogObjectUtilities.GetResourceTypeIdentifier(r))).ToArray<Guid>();
    }

    private static IEnumerable<Type> GetKnownChildTypes(Type catalogObjectType)
    {
      FieldInfo field = catalogObjectType.GetField("KnownChildTypes", BindingFlags.Static | BindingFlags.NonPublic);
      return field != (FieldInfo) null ? field.GetValue((object) null) as IEnumerable<Type> : (IEnumerable<Type>) null;
    }

    public static T PropertyValueFromString<T>(string stringValue)
    {
      Type enumType = typeof (T);
      object obj = (object) null;
      if (enumType == typeof (string))
        obj = (object) stringValue;
      else if (enumType == typeof (Uri))
        obj = (object) new Uri(stringValue);
      else if (enumType == typeof (Guid))
        obj = (object) new Guid(stringValue);
      else if (enumType == typeof (bool))
        obj = (object) bool.Parse(stringValue);
      else if (enumType == typeof (int))
        obj = (object) int.Parse(stringValue, (IFormatProvider) CultureInfo.InvariantCulture);
      else if (enumType.IsEnum)
        obj = Enum.Parse(enumType, stringValue);
      return (T) obj;
    }

    public static string PropertyValueToString<T>(T value)
    {
      string str = (string) null;
      if ((object) value != null)
        str = value.ToString();
      return str;
    }
  }
}
