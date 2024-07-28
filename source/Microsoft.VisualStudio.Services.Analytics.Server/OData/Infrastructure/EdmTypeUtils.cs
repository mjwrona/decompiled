// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.EdmTypeUtils
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class EdmTypeUtils
  {
    private static readonly EdmCoreModel _coreModel = EdmCoreModel.Instance;
    private static readonly Dictionary<Type, IEdmPrimitiveType> _builtInTypesMapping = ((IEnumerable<KeyValuePair<Type, IEdmPrimitiveType>>) new KeyValuePair<Type, IEdmPrimitiveType>[42]
    {
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (string), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (bool), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (bool?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (byte), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Byte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (byte?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Byte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Decimal), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Decimal?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (double), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Double)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (double?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Double)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Guid), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Guid)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Guid?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Guid)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (short), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int16)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (short?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int16)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (int), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (int?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (long), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (long?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (sbyte), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.SByte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (sbyte?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.SByte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (float), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Single)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (float?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Single)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (byte[]), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Binary)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Stream), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Stream)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTimeOffset), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTimeOffset?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeSpan), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Duration)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeSpan?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Duration)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Date), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Date)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Date?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Date)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeOfDay), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeOfDay?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ushort), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ushort?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (uint), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (uint?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ulong), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ulong?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (char[]), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (char), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (char?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTime), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTime?), EdmTypeUtils.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset))
    }).ToDictionary<KeyValuePair<Type, IEdmPrimitiveType>, Type, IEdmPrimitiveType>((Func<KeyValuePair<Type, IEdmPrimitiveType>, Type>) (kvp => kvp.Key), (Func<KeyValuePair<Type, IEdmPrimitiveType>, IEdmPrimitiveType>) (kvp => kvp.Value));

    internal static bool IsTypeOf(IEdmTypeReference typeReference, Type type)
    {
      if (typeReference == null)
        return false;
      // ISSUE: explicit non-virtual call
      return type.Name == (typeReference.Definition is IEdmPrimitiveType definition1 ? definition1.Name : (string) null) || type.Name == (typeReference.Definition is EdmEntityType definition2 ? __nonvirtual (definition2.Name) : (string) null);
    }

    internal static Type GetType(IEdmTypeReference typeReference) => typeReference.Definition is EdmEntityType definition ? EdmTypeUtils.GetType(definition) : (Type) null;

    internal static Type GetType(IEdmType type)
    {
      if (type.TypeKind == EdmTypeKind.Entity)
        return EdmTypeUtils.GetType(type as EdmEntityType);
      // ISSUE: explicit non-virtual call
      return type.TypeKind == EdmTypeKind.Collection ? EdmTypeUtils.GetType(type is EdmCollectionType edmCollectionType ? __nonvirtual (edmCollectionType.ElementType) : (IEdmTypeReference) null) : (Type) null;
    }

    internal static Type GetType(EdmEntityType type) => Type.GetType(type.Namespace + "." + type.Name);

    private static IEdmPrimitiveType GetPrimitiveType(EdmPrimitiveTypeKind primitiveKind) => EdmTypeUtils._coreModel.GetPrimitiveType(primitiveKind);

    public static IEdmPrimitiveType GetEdmPrimitiveTypeOrNull(Type clrType)
    {
      IEdmPrimitiveType edmPrimitiveType;
      return !EdmTypeUtils._builtInTypesMapping.TryGetValue(clrType, out edmPrimitiveType) ? (IEdmPrimitiveType) null : edmPrimitiveType;
    }

    public static IEdmPrimitiveTypeReference GetEdmPrimitiveTypeReferenceOrNull(Type clrType)
    {
      IEdmPrimitiveType primitiveTypeOrNull = EdmTypeUtils.GetEdmPrimitiveTypeOrNull(clrType);
      return primitiveTypeOrNull == null ? (IEdmPrimitiveTypeReference) null : EdmTypeUtils._coreModel.GetPrimitive(primitiveTypeOrNull.PrimitiveKind, EdmTypeUtils.IsNullable(clrType));
    }

    public static bool IsNullable(Type type) => !EdmTypeUtils.IsValueType(type) || Nullable.GetUnderlyingType(type) != (Type) null;

    public static bool IsValueType(Type clrType) => clrType.IsValueType;

    public static EdmEnumType GetEdmEnumType(Type type)
    {
      if (!type.IsEnum)
        return (EdmEnumType) null;
      EdmEnumType edmEnumType = new EdmEnumType(type.Namespace, type.Name, false);
      foreach (Enum @enum in Enum.GetValues(type))
      {
        long int64 = Convert.ToInt64((object) @enum, (IFormatProvider) CultureInfo.InvariantCulture);
        edmEnumType.AddMember(@enum.ToString(), (IEdmEnumMemberValue) new EdmEnumMemberValue(int64));
      }
      return edmEnumType;
    }
  }
}
