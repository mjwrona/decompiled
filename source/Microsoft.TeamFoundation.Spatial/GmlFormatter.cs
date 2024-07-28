// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GmlFormatter
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Spatial
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the accepted name in the industry")]
  public abstract class GmlFormatter : SpatialFormatter<XmlReader, XmlWriter>
  {
    protected GmlFormatter(SpatialImplementation creator)
      : base(creator)
    {
    }

    public static GmlFormatter Create() => SpatialImplementation.CurrentImplementation.CreateGmlFormatter();
  }
}
