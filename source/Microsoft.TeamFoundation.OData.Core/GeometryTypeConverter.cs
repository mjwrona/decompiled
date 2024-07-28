// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.GeometryTypeConverter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using Microsoft.Spatial;
using System.IO;
using System.Xml;

namespace Microsoft.OData
{
  internal sealed class GeometryTypeConverter : IPrimitiveTypeConverter
  {
    public void WriteAtom(object instance, XmlWriter writer) => ((Geometry) instance).SendTo((GeometryPipeline) GmlFormatter.Create().CreateWriter(writer));

    public void WriteAtom(object instance, TextWriter writer) => ((Geometry) instance).SendTo((GeometryPipeline) WellKnownTextSqlFormatter.Create().CreateWriter(writer));

    public void WriteJsonLight(object instance, IJsonWriter jsonWriter)
    {
      IGeoJsonWriter writer = (IGeoJsonWriter) new GeoJsonWriterAdapter(jsonWriter);
      ((Geometry) instance).SendTo((GeometryPipeline) GeoJsonObjectFormatter.Create().CreateWriter(writer));
    }
  }
}
