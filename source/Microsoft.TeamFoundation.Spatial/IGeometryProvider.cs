// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.IGeometryProvider
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public interface IGeometryProvider
  {
    [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Not following the event-handler pattern")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
    event Action<Geometry> ProduceGeometry;

    Geometry ConstructedGeometry { get; }
  }
}
