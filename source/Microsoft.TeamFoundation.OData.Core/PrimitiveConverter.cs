// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.PrimitiveConverter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal sealed class PrimitiveConverter
  {
    private static readonly IPrimitiveTypeConverter geographyTypeConverter = (IPrimitiveTypeConverter) new GeographyTypeConverter();
    private static readonly IPrimitiveTypeConverter geometryTypeConverter = (IPrimitiveTypeConverter) new GeometryTypeConverter();
    private static readonly PrimitiveConverter primitiveConverter = new PrimitiveConverter(new KeyValuePair<Type, IPrimitiveTypeConverter>[16]
    {
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeographyPoint), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeographyLineString), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeographyPolygon), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeographyCollection), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeographyMultiPoint), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeographyMultiLineString), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeographyMultiPolygon), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (Geography), PrimitiveConverter.geographyTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeometryPoint), PrimitiveConverter.geometryTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeometryLineString), PrimitiveConverter.geometryTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeometryPolygon), PrimitiveConverter.geometryTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeometryCollection), PrimitiveConverter.geometryTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeometryMultiPoint), PrimitiveConverter.geometryTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeometryMultiLineString), PrimitiveConverter.geometryTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (GeometryMultiPolygon), PrimitiveConverter.geometryTypeConverter),
      new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (Geometry), PrimitiveConverter.geometryTypeConverter)
    });
    private readonly Dictionary<Type, IPrimitiveTypeConverter> spatialPrimitiveTypeConverters;

    internal PrimitiveConverter(
      KeyValuePair<Type, IPrimitiveTypeConverter>[] spatialPrimitiveTypeConverters)
    {
      this.spatialPrimitiveTypeConverters = new Dictionary<Type, IPrimitiveTypeConverter>((IEqualityComparer<Type>) EqualityComparer<Type>.Default);
      foreach (KeyValuePair<Type, IPrimitiveTypeConverter> primitiveTypeConverter in spatialPrimitiveTypeConverters)
        this.spatialPrimitiveTypeConverters.Add(primitiveTypeConverter.Key, primitiveTypeConverter.Value);
    }

    internal static PrimitiveConverter Instance => PrimitiveConverter.primitiveConverter;

    internal void WriteJsonLight(object instance, IJsonWriter jsonWriter)
    {
      IPrimitiveTypeConverter primitiveTypeConverter;
      this.TryGetConverter(instance.GetType(), out primitiveTypeConverter);
      primitiveTypeConverter.WriteJsonLight(instance, jsonWriter);
    }

    private bool TryGetConverter(Type type, out IPrimitiveTypeConverter primitiveTypeConverter)
    {
      if (typeof (ISpatial).IsAssignableFrom(type))
      {
        KeyValuePair<Type, IPrimitiveTypeConverter> keyValuePair = new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof (object), (IPrimitiveTypeConverter) null);
        foreach (KeyValuePair<Type, IPrimitiveTypeConverter> primitiveTypeConverter1 in this.spatialPrimitiveTypeConverters)
        {
          if (primitiveTypeConverter1.Key.IsAssignableFrom(type) && keyValuePair.Key.IsAssignableFrom(primitiveTypeConverter1.Key))
            keyValuePair = primitiveTypeConverter1;
        }
        primitiveTypeConverter = keyValuePair.Value;
        return keyValuePair.Value != null;
      }
      primitiveTypeConverter = (IPrimitiveTypeConverter) null;
      return false;
    }
  }
}
