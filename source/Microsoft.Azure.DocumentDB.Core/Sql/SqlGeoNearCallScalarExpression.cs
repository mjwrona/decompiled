// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlGeoNearCallScalarExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal class SqlGeoNearCallScalarExpression : SqlScalarExpression
  {
    public const string NearMinimumDistanceName = "@nearMinimumDistance";
    public const string NearMaximumDistanceName = "@nearMaximumDistance";

    public SqlScalarExpression PropertyRef { get; private set; }

    public SqlScalarExpression Geometry { get; set; }

    public uint? NumberOfPoints { get; set; }

    public double MinimumDistance { get; set; }

    public double MaximumDistance { get; set; }

    private SqlGeoNearCallScalarExpression(
      SqlScalarExpression propertyRef,
      SqlScalarExpression geometry,
      uint? num = null,
      double minDistance = 0.0,
      double maxDistance = 10000.0)
      : base(SqlObjectKind.GeoNearCallScalarExpression)
    {
      this.PropertyRef = propertyRef;
      this.Geometry = geometry;
      this.NumberOfPoints = num;
      this.MinimumDistance = Math.Max(0.0, minDistance);
      this.MaximumDistance = Math.Max(0.0, maxDistance);
    }

    public static SqlGeoNearCallScalarExpression Create(
      SqlScalarExpression propertyRef,
      SqlScalarExpression geometry,
      uint? num = null,
      double minDistance = 0.0,
      double maxDistance = 10000.0)
    {
      return new SqlGeoNearCallScalarExpression(propertyRef, geometry, num, minDistance, maxDistance);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlScalarExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlScalarExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(
      SqlScalarExpressionVisitor<T, TResult> visitor,
      T input)
    {
      return visitor.Visit(this, input);
    }
  }
}
