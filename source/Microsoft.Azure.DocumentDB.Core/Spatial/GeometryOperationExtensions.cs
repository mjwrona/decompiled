// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.GeometryOperationExtensions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Spatial
{
  public static class GeometryOperationExtensions
  {
    public static double Distance(this Geometry from, Geometry to) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public static bool Within(this Geometry inner, Geometry outer) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public static bool IsValid(this Geometry geometry) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public static GeometryValidationResult IsValidDetailed(this Geometry geometry) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public static bool Intersects(this Geometry geometry1, Geometry geometry2) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);
  }
}
