// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryOperationsExtensions
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Spatial
{
  public static class GeometryOperationsExtensions
  {
    public static double? Distance(this Geometry operand1, Geometry operand2) => GeometryOperationsExtensions.OperationsFor(operand1, operand2).IfValidReturningNullable<SpatialOperations, double>((Func<SpatialOperations, double>) (ops => ops.Distance(operand1, operand2)));

    public static double? Length(this Geometry operand) => GeometryOperationsExtensions.OperationsFor(operand).IfValidReturningNullable<SpatialOperations, double>((Func<SpatialOperations, double>) (ops => ops.Length(operand)));

    public static bool? Intersects(this Geometry operand1, Geometry operand2) => GeometryOperationsExtensions.OperationsFor(operand1, operand2).IfValidReturningNullable<SpatialOperations, bool>((Func<SpatialOperations, bool>) (ops => ops.Intersects(operand1, operand2)));

    private static SpatialOperations OperationsFor(params Geometry[] operands) => ((IEnumerable<Geometry>) operands).Any<Geometry>((Func<Geometry, bool>) (operand => operand == null)) ? (SpatialOperations) null : operands[0].Creator.VerifyAndGetNonNullOperations();
  }
}
