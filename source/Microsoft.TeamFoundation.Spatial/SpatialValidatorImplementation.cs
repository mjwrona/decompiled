// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialValidatorImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class SpatialValidatorImplementation : SpatialPipeline
  {
    internal const double MaxLongitude = 15069.0;
    internal const double MaxLatitude = 90.0;
    private readonly SpatialValidatorImplementation.NestedValidator geographyValidatorInstance = new SpatialValidatorImplementation.NestedValidator();
    private readonly SpatialValidatorImplementation.NestedValidator geometryValidatorInstance = new SpatialValidatorImplementation.NestedValidator();

    public override GeographyPipeline GeographyPipeline => this.geographyValidatorInstance.GeographyPipeline;

    public override GeometryPipeline GeometryPipeline => this.geometryValidatorInstance.GeometryPipeline;

    private class NestedValidator : DrawBoth
    {
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState CoordinateSystem = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.SetCoordinateSystemState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState BeginSpatial = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.BeginGeoState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState PointStart = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.PointStartState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState PointBuilding = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.PointBuildingState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState PointEnd = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.PointEndState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState LineStringStart = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.LineStringStartState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState LineStringBuilding = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.LineStringBuildingState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState LineStringEnd = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.LineStringEndState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState PolygonStart = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.PolygonStartState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState PolygonBuilding = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.PolygonBuildingState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState MultiPoint = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.MultiPointState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState MultiLineString = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.MultiLineStringState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState MultiPolygon = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.MultiPolygonState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState Collection = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.CollectionState();
      private static readonly SpatialValidatorImplementation.NestedValidator.ValidatorState FullGlobe = (SpatialValidatorImplementation.NestedValidator.ValidatorState) new SpatialValidatorImplementation.NestedValidator.FullGlobeState();
      private const int MaxGeometryCollectionDepth = 28;
      private readonly Stack<SpatialValidatorImplementation.NestedValidator.ValidatorState> stack = new Stack<SpatialValidatorImplementation.NestedValidator.ValidatorState>(16);
      private CoordinateSystem validationCoordinateSystem;
      private int ringCount;
      private double initialFirstCoordinate;
      private double initialSecondCoordinate;
      private double mostRecentFirstCoordinate;
      private double mostRecentSecondCoordinate;
      private bool processingGeography;
      private int pointCount;
      private int depth;

      public NestedValidator() => this.InitializeObject();

      protected override CoordinateSystem OnSetCoordinateSystem(CoordinateSystem coordinateSystem)
      {
        SpatialValidatorImplementation.NestedValidator.ValidatorState validatorState = this.stack.Peek();
        this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem);
        if (validatorState == SpatialValidatorImplementation.NestedValidator.CoordinateSystem)
          this.validationCoordinateSystem = coordinateSystem;
        else if (this.validationCoordinateSystem != coordinateSystem)
          throw new FormatException(Strings.Validator_SridMismatch);
        return coordinateSystem;
      }

      protected override SpatialType OnBeginGeography(SpatialType shape)
      {
        this.processingGeography = this.depth <= 0 || this.processingGeography ? true : throw new FormatException(Strings.Validator_UnexpectedGeometry);
        this.BeginShape(shape);
        return shape;
      }

      protected override void OnEndGeography()
      {
        this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.End);
        if (!this.processingGeography)
          throw new FormatException(Strings.Validator_UnexpectedGeometry);
        --this.depth;
      }

      protected override SpatialType OnBeginGeometry(SpatialType shape)
      {
        this.processingGeography = this.depth <= 0 || !this.processingGeography ? false : throw new FormatException(Strings.Validator_UnexpectedGeography);
        this.BeginShape(shape);
        return shape;
      }

      protected override void OnEndGeometry()
      {
        this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.End);
        if (this.processingGeography)
          throw new FormatException(Strings.Validator_UnexpectedGeography);
        --this.depth;
      }

      protected override GeographyPosition OnBeginFigure(GeographyPosition position)
      {
        this.BeginFigure(new Action<double, double, double?, double?>(SpatialValidatorImplementation.NestedValidator.ValidateGeographyPosition), position.Latitude, position.Longitude, position.Z, position.M);
        return position;
      }

      protected override GeometryPosition OnBeginFigure(GeometryPosition position)
      {
        this.BeginFigure(new Action<double, double, double?, double?>(SpatialValidatorImplementation.NestedValidator.ValidateGeometryPosition), position.X, position.Y, position.Z, position.M);
        return position;
      }

      protected override void OnEndFigure() => this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure);

      protected override void OnReset() => this.InitializeObject();

      protected override GeographyPosition OnLineTo(GeographyPosition position)
      {
        if (this.processingGeography)
          SpatialValidatorImplementation.NestedValidator.ValidateGeographyPosition(position.Latitude, position.Longitude, position.Z, position.M);
        this.AddControlPoint(position.Latitude, position.Longitude);
        if (!this.processingGeography)
          throw new FormatException(Strings.Validator_UnexpectedGeometry);
        return position;
      }

      protected override GeometryPosition OnLineTo(GeometryPosition position)
      {
        if (!this.processingGeography)
          SpatialValidatorImplementation.NestedValidator.ValidateGeometryPosition(position.X, position.Y, position.Z, position.M);
        this.AddControlPoint(position.X, position.Y);
        if (this.processingGeography)
          throw new FormatException(Strings.Validator_UnexpectedGeography);
        return position;
      }

      private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

      [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
      private static bool IsPointValid(double first, double second, double? z, double? m)
      {
        if (!SpatialValidatorImplementation.NestedValidator.IsFinite(first) || !SpatialValidatorImplementation.NestedValidator.IsFinite(second) || z.HasValue && !SpatialValidatorImplementation.NestedValidator.IsFinite(z.Value))
          return false;
        return !m.HasValue || SpatialValidatorImplementation.NestedValidator.IsFinite(m.Value);
      }

      private static void ValidateOnePosition(double first, double second, double? z, double? m)
      {
        if (!SpatialValidatorImplementation.NestedValidator.IsPointValid(first, second, z, m))
          throw new FormatException(Strings.Validator_InvalidPointCoordinate((object) first, (object) second, (object) z, (object) m));
      }

      private static void ValidateGeographyPosition(
        double latitude,
        double longitude,
        double? z,
        double? m)
      {
        SpatialValidatorImplementation.NestedValidator.ValidateOnePosition(latitude, longitude, z, m);
        if (!SpatialValidatorImplementation.NestedValidator.IsLatitudeValid(latitude))
          throw new FormatException(Strings.Validator_InvalidLatitudeCoordinate((object) latitude));
        if (!SpatialValidatorImplementation.NestedValidator.IsLongitudeValid(longitude))
          throw new FormatException(Strings.Validator_InvalidLongitudeCoordinate((object) longitude));
      }

      private static void ValidateGeometryPosition(double x, double y, double? z, double? m) => SpatialValidatorImplementation.NestedValidator.ValidateOnePosition(x, y, z, m);

      private static bool IsLatitudeValid(double latitude) => latitude >= -90.0 && latitude <= 90.0;

      private static bool IsLongitudeValid(double longitude) => longitude >= -15069.0 && longitude <= 15069.0;

      private static void ValidateGeographyPolygon(
        int numOfPoints,
        double initialFirstCoordinate,
        double initialSecondCoordinate,
        double mostRecentFirstCoordinate,
        double mostRecentSecondCoordinate)
      {
        if (numOfPoints < 4 || initialFirstCoordinate != mostRecentFirstCoordinate || !SpatialValidatorImplementation.NestedValidator.AreLongitudesEqual(initialSecondCoordinate, mostRecentSecondCoordinate))
          throw new FormatException(Strings.Validator_InvalidPolygonPoints);
      }

      private static void ValidateGeometryPolygon(
        int numOfPoints,
        double initialFirstCoordinate,
        double initialSecondCoordinate,
        double mostRecentFirstCoordinate,
        double mostRecentSecondCoordinate)
      {
        if (numOfPoints < 4 || initialFirstCoordinate != mostRecentFirstCoordinate || initialSecondCoordinate != mostRecentSecondCoordinate)
          throw new FormatException(Strings.Validator_InvalidPolygonPoints);
      }

      private static bool AreLongitudesEqual(double left, double right) => left == right || (left - right) % 360.0 == 0.0;

      private void BeginFigure(
        Action<double, double, double?, double?> validate,
        double x,
        double y,
        double? z,
        double? m)
      {
        validate(x, y, z, m);
        this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure);
        this.pointCount = 0;
        this.TrackPosition(x, y);
      }

      private void BeginShape(SpatialType type)
      {
        ++this.depth;
        switch (type)
        {
          case SpatialType.Point:
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPoint);
            break;
          case SpatialType.LineString:
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginLineString);
            break;
          case SpatialType.Polygon:
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPolygon);
            break;
          case SpatialType.MultiPoint:
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiPoint);
            break;
          case SpatialType.MultiLineString:
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiLineString);
            break;
          case SpatialType.MultiPolygon:
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiPolygon);
            break;
          case SpatialType.Collection:
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginCollection);
            break;
          case SpatialType.FullGlobe:
            if (!this.processingGeography)
              throw new FormatException(Strings.Validator_InvalidType((object) type));
            this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFullGlobe);
            break;
          default:
            throw new FormatException(Strings.Validator_InvalidType((object) type));
        }
      }

      private void AddControlPoint(double first, double second)
      {
        this.Execute(SpatialValidatorImplementation.NestedValidator.PipelineCall.LineTo);
        this.TrackPosition(first, second);
      }

      private void TrackPosition(double first, double second)
      {
        if (this.pointCount == 0)
        {
          this.initialFirstCoordinate = first;
          this.initialSecondCoordinate = second;
        }
        this.mostRecentFirstCoordinate = first;
        this.mostRecentSecondCoordinate = second;
        ++this.pointCount;
      }

      private void Execute(
        SpatialValidatorImplementation.NestedValidator.PipelineCall transition)
      {
        this.stack.Peek().ValidateTransition(transition, this);
      }

      private void InitializeObject()
      {
        this.depth = 0;
        this.initialFirstCoordinate = 0.0;
        this.initialSecondCoordinate = 0.0;
        this.mostRecentFirstCoordinate = 0.0;
        this.mostRecentSecondCoordinate = 0.0;
        this.pointCount = 0;
        this.validationCoordinateSystem = (CoordinateSystem) null;
        this.ringCount = 0;
        this.stack.Clear();
        this.stack.Push(SpatialValidatorImplementation.NestedValidator.CoordinateSystem);
      }

      private void Call(
        SpatialValidatorImplementation.NestedValidator.ValidatorState state)
      {
        if (this.stack.Count > 28)
          throw new FormatException(Strings.Validator_NestingOverflow((object) 28));
        this.stack.Push(state);
      }

      private void Return() => this.stack.Pop();

      private void Jump(
        SpatialValidatorImplementation.NestedValidator.ValidatorState state)
      {
        this.stack.Pop();
        this.stack.Push(state);
      }

      private enum PipelineCall
      {
        SetCoordinateSystem,
        Begin,
        BeginPoint,
        BeginLineString,
        BeginPolygon,
        BeginMultiPoint,
        BeginMultiLineString,
        BeginMultiPolygon,
        BeginCollection,
        BeginFullGlobe,
        BeginFigure,
        LineTo,
        EndFigure,
        End,
      }

      private abstract class ValidatorState
      {
        internal abstract void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator);

        protected static void ThrowExpected(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator.PipelineCall actual)
        {
          throw new FormatException(Strings.Validator_UnexpectedCall((object) transition, (object) actual));
        }

        protected static void ThrowExpected(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition1,
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition2,
          SpatialValidatorImplementation.NestedValidator.PipelineCall actual)
        {
          throw new FormatException(Strings.Validator_UnexpectedCall2((object) transition1, (object) transition2, (object) actual));
        }

        protected static void ThrowExpected(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition1,
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition2,
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition3,
          SpatialValidatorImplementation.NestedValidator.PipelineCall actual)
        {
          throw new FormatException(Strings.Validator_UnexpectedCall2((object) (transition1.ToString() + ", " + (object) transition2), (object) transition3, (object) actual));
        }
      }

      private class SetCoordinateSystemState : 
        SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          if (transition == SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem)
            validator.Call(SpatialValidatorImplementation.NestedValidator.BeginSpatial);
          else
            SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem, transition);
        }
      }

      private class BeginGeoState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPoint:
              validator.Jump(SpatialValidatorImplementation.NestedValidator.PointStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginLineString:
              validator.Jump(SpatialValidatorImplementation.NestedValidator.LineStringStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPolygon:
              validator.Jump(SpatialValidatorImplementation.NestedValidator.PolygonStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiPoint:
              validator.Jump(SpatialValidatorImplementation.NestedValidator.MultiPoint);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiLineString:
              validator.Jump(SpatialValidatorImplementation.NestedValidator.MultiLineString);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiPolygon:
              validator.Jump(SpatialValidatorImplementation.NestedValidator.MultiPolygon);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginCollection:
              validator.Jump(SpatialValidatorImplementation.NestedValidator.Collection);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFullGlobe:
              if (validator.depth != 1)
                throw new FormatException(Strings.Validator_FullGlobeInCollection);
              validator.Jump(SpatialValidatorImplementation.NestedValidator.FullGlobe);
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.Begin, transition);
              break;
          }
        }
      }

      private class PointStartState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          if (transition != SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure)
          {
            if (transition == SpatialValidatorImplementation.NestedValidator.PipelineCall.End)
              validator.Return();
            else
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure, SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
          }
          else
            validator.Jump(SpatialValidatorImplementation.NestedValidator.PointBuilding);
        }
      }

      private class PointBuildingState : 
        SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.LineTo:
              if (validator.pointCount == 0)
                break;
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure, transition);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure:
              if (validator.pointCount == 0)
                SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure, transition);
              validator.Jump(SpatialValidatorImplementation.NestedValidator.PointEnd);
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure, transition);
              break;
          }
        }
      }

      private class PointEndState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          if (transition == SpatialValidatorImplementation.NestedValidator.PipelineCall.End)
            validator.Return();
          else
            SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
        }
      }

      private class LineStringStartState : 
        SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          if (transition != SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure)
          {
            if (transition == SpatialValidatorImplementation.NestedValidator.PipelineCall.End)
              validator.Return();
            else
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure, SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
          }
          else
            validator.Jump(SpatialValidatorImplementation.NestedValidator.LineStringBuilding);
        }
      }

      private class LineStringBuildingState : 
        SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.LineTo:
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure:
              if (validator.pointCount < 2)
                throw new FormatException(Strings.Validator_LineStringNeedsTwoPoints);
              validator.Jump(SpatialValidatorImplementation.NestedValidator.LineStringEnd);
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.LineTo, SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure, transition);
              break;
          }
        }
      }

      private class LineStringEndState : 
        SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          if (transition == SpatialValidatorImplementation.NestedValidator.PipelineCall.End)
            validator.Return();
          else
            SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
        }
      }

      private class PolygonStartState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          if (transition != SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure)
          {
            if (transition == SpatialValidatorImplementation.NestedValidator.PipelineCall.End)
            {
              validator.ringCount = 0;
              validator.Return();
            }
            else
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFigure, SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
          }
          else
            validator.Jump(SpatialValidatorImplementation.NestedValidator.PolygonBuilding);
        }
      }

      private class PolygonBuildingState : 
        SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.LineTo:
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure:
              ++validator.ringCount;
              if (validator.processingGeography)
                SpatialValidatorImplementation.NestedValidator.ValidateGeographyPolygon(validator.pointCount, validator.initialFirstCoordinate, validator.initialSecondCoordinate, validator.mostRecentFirstCoordinate, validator.mostRecentSecondCoordinate);
              else
                SpatialValidatorImplementation.NestedValidator.ValidateGeometryPolygon(validator.pointCount, validator.initialFirstCoordinate, validator.initialSecondCoordinate, validator.mostRecentFirstCoordinate, validator.mostRecentSecondCoordinate);
              validator.Jump(SpatialValidatorImplementation.NestedValidator.PolygonStart);
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.LineTo, SpatialValidatorImplementation.NestedValidator.PipelineCall.EndFigure, transition);
              break;
          }
        }
      }

      private class MultiPointState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem:
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPoint:
              validator.Call(SpatialValidatorImplementation.NestedValidator.PointStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.End:
              validator.Return();
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem, SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPoint, SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
              break;
          }
        }
      }

      private class MultiLineStringState : 
        SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem:
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginLineString:
              validator.Call(SpatialValidatorImplementation.NestedValidator.LineStringStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.End:
              validator.Return();
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem, SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginLineString, SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
              break;
          }
        }
      }

      private class MultiPolygonState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem:
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPolygon:
              validator.Call(SpatialValidatorImplementation.NestedValidator.PolygonStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.End:
              validator.Return();
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem, SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPolygon, SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
              break;
          }
        }
      }

      private class CollectionState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          switch (transition)
          {
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem:
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPoint:
              validator.Call(SpatialValidatorImplementation.NestedValidator.PointStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginLineString:
              validator.Call(SpatialValidatorImplementation.NestedValidator.LineStringStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginPolygon:
              validator.Call(SpatialValidatorImplementation.NestedValidator.PolygonStart);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiPoint:
              validator.Call(SpatialValidatorImplementation.NestedValidator.MultiPoint);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiLineString:
              validator.Call(SpatialValidatorImplementation.NestedValidator.MultiLineString);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginMultiPolygon:
              validator.Call(SpatialValidatorImplementation.NestedValidator.MultiPolygon);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginCollection:
              validator.Call(SpatialValidatorImplementation.NestedValidator.Collection);
              break;
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.BeginFullGlobe:
              throw new FormatException(Strings.Validator_FullGlobeInCollection);
            case SpatialValidatorImplementation.NestedValidator.PipelineCall.End:
              validator.Return();
              break;
            default:
              SpatialValidatorImplementation.NestedValidator.ValidatorState.ThrowExpected(SpatialValidatorImplementation.NestedValidator.PipelineCall.SetCoordinateSystem, SpatialValidatorImplementation.NestedValidator.PipelineCall.Begin, SpatialValidatorImplementation.NestedValidator.PipelineCall.End, transition);
              break;
          }
        }
      }

      private class FullGlobeState : SpatialValidatorImplementation.NestedValidator.ValidatorState
      {
        internal override void ValidateTransition(
          SpatialValidatorImplementation.NestedValidator.PipelineCall transition,
          SpatialValidatorImplementation.NestedValidator validator)
        {
          if (transition != SpatialValidatorImplementation.NestedValidator.PipelineCall.End)
            throw new FormatException(Strings.Validator_FullGlobeCannotHaveElements);
          validator.Return();
        }
      }
    }
  }
}
