// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.LiteralUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.Spatial;
using System.IO;

namespace Microsoft.OData.UriParser
{
  internal static class LiteralUtils
  {
    private static WellKnownTextSqlFormatter Formatter => SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(false);

    internal static Geography ParseGeography(string text)
    {
      using (StringReader input = new StringReader(text))
        return LiteralUtils.Formatter.Read<Geography>((TextReader) input);
    }

    internal static Geometry ParseGeometry(string text)
    {
      using (StringReader input = new StringReader(text))
        return LiteralUtils.Formatter.Read<Geometry>((TextReader) input);
    }

    internal static string ToWellKnownText(Geography instance) => LiteralUtils.Formatter.Write((ISpatial) instance);

    internal static string ToWellKnownText(Geometry instance) => LiteralUtils.Formatter.Write((ISpatial) instance);
  }
}
