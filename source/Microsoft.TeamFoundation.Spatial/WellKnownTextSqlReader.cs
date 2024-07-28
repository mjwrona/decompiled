// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.WellKnownTextSqlReader
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Spatial
{
  internal class WellKnownTextSqlReader : SpatialReader<TextReader>
  {
    private bool allowOnlyTwoDimensions;

    public WellKnownTextSqlReader(SpatialPipeline destination)
      : this(destination, false)
    {
    }

    public WellKnownTextSqlReader(SpatialPipeline destination, bool allowOnlyTwoDimensions)
      : base(destination)
    {
      this.allowOnlyTwoDimensions = allowOnlyTwoDimensions;
    }

    protected override void ReadGeographyImplementation(TextReader input) => new WellKnownTextSqlReader.Parser(input, (TypeWashedPipeline) new TypeWashedToGeographyLongLatPipeline(this.Destination), this.allowOnlyTwoDimensions).Read();

    protected override void ReadGeometryImplementation(TextReader input) => new WellKnownTextSqlReader.Parser(input, (TypeWashedPipeline) new TypeWashedToGeometryPipeline(this.Destination), this.allowOnlyTwoDimensions).Read();

    private class Parser
    {
      private readonly bool allowOnlyTwoDimensions;
      private readonly TextLexerBase lexer;
      private readonly TypeWashedPipeline pipeline;

      public Parser(TextReader reader, TypeWashedPipeline pipeline, bool allowOnlyTwoDimensions)
      {
        this.lexer = (TextLexerBase) new WellKnownTextLexer(reader);
        this.pipeline = pipeline;
        this.allowOnlyTwoDimensions = allowOnlyTwoDimensions;
      }

      public void Read()
      {
        this.ParseSRID();
        this.ParseTaggedText();
      }

      private bool IsTokenMatch(WellKnownTextTokenType type, string text) => this.lexer.CurrentToken.MatchToken((int) type, text, StringComparison.OrdinalIgnoreCase);

      private bool NextToken()
      {
        while (this.lexer.Next())
        {
          if (!this.lexer.CurrentToken.MatchToken(8, string.Empty, StringComparison.Ordinal))
            return true;
        }
        return false;
      }

      private void ParseCollectionText()
      {
        if (this.ReadEmptySet())
          return;
        this.ReadToken(WellKnownTextTokenType.LeftParen, (string) null);
        this.ParseTaggedText();
        while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, (string) null))
          this.ParseTaggedText();
        this.ReadToken(WellKnownTextTokenType.RightParen, (string) null);
      }

      private void ParseLineStringText()
      {
        if (this.ReadEmptySet())
          return;
        this.ReadToken(WellKnownTextTokenType.LeftParen, (string) null);
        this.ParsePoint(true);
        while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, (string) null))
          this.ParsePoint(false);
        this.ReadToken(WellKnownTextTokenType.RightParen, (string) null);
        this.pipeline.EndFigure();
      }

      private void ParseMultiGeoText(SpatialType innerType, Action innerReader)
      {
        if (this.ReadEmptySet())
          return;
        this.ReadToken(WellKnownTextTokenType.LeftParen, (string) null);
        this.pipeline.BeginGeo(innerType);
        innerReader();
        this.pipeline.EndGeo();
        while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, (string) null))
        {
          this.pipeline.BeginGeo(innerType);
          innerReader();
          this.pipeline.EndGeo();
        }
        this.ReadToken(WellKnownTextTokenType.RightParen, (string) null);
      }

      private void ParsePoint(bool firstFigure)
      {
        double coordinate1 = this.ReadDouble();
        double coordinate2 = this.ReadDouble();
        double? coordinate3;
        if (this.TryReadOptionalNullableDouble(out coordinate3) && this.allowOnlyTwoDimensions)
          throw new FormatException(Strings.WellKnownText_TooManyDimensions);
        double? coordinate4;
        if (this.TryReadOptionalNullableDouble(out coordinate4) && this.allowOnlyTwoDimensions)
          throw new FormatException(Strings.WellKnownText_TooManyDimensions);
        if (firstFigure)
          this.pipeline.BeginFigure(coordinate1, coordinate2, coordinate3, coordinate4);
        else
          this.pipeline.LineTo(coordinate1, coordinate2, coordinate3, coordinate4);
      }

      private void ParsePointText()
      {
        if (this.ReadEmptySet())
          return;
        this.ReadToken(WellKnownTextTokenType.LeftParen, (string) null);
        this.ParsePoint(true);
        this.ReadToken(WellKnownTextTokenType.RightParen, (string) null);
        this.pipeline.EndFigure();
      }

      private void ParsePolygonText()
      {
        if (this.ReadEmptySet())
          return;
        this.ReadToken(WellKnownTextTokenType.LeftParen, (string) null);
        this.ParseLineStringText();
        while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, (string) null))
          this.ParseLineStringText();
        this.ReadToken(WellKnownTextTokenType.RightParen, (string) null);
      }

      private void ParseSRID()
      {
        if (this.ReadOptionalToken(WellKnownTextTokenType.Text, "SRID"))
        {
          this.ReadToken(WellKnownTextTokenType.Equals, (string) null);
          this.pipeline.SetCoordinateSystem(new int?(this.ReadInteger()));
          this.ReadToken(WellKnownTextTokenType.Semicolon, (string) null);
        }
        else
          this.pipeline.SetCoordinateSystem(new int?());
      }

      private void ParseTaggedText()
      {
        if (!this.NextToken())
          throw new FormatException(Strings.WellKnownText_UnknownTaggedText((object) string.Empty));
        switch (this.lexer.CurrentToken.Text.ToUpperInvariant())
        {
          case "FULLGLOBE":
            this.pipeline.BeginGeo(SpatialType.FullGlobe);
            this.pipeline.EndGeo();
            break;
          case "GEOMETRYCOLLECTION":
            this.pipeline.BeginGeo(SpatialType.Collection);
            this.ParseCollectionText();
            this.pipeline.EndGeo();
            break;
          case "LINESTRING":
            this.pipeline.BeginGeo(SpatialType.LineString);
            this.ParseLineStringText();
            this.pipeline.EndGeo();
            break;
          case "MULTILINESTRING":
            this.pipeline.BeginGeo(SpatialType.MultiLineString);
            this.ParseMultiGeoText(SpatialType.LineString, new Action(this.ParseLineStringText));
            this.pipeline.EndGeo();
            break;
          case "MULTIPOINT":
            this.pipeline.BeginGeo(SpatialType.MultiPoint);
            this.ParseMultiGeoText(SpatialType.Point, new Action(this.ParsePointText));
            this.pipeline.EndGeo();
            break;
          case "MULTIPOLYGON":
            this.pipeline.BeginGeo(SpatialType.MultiPolygon);
            this.ParseMultiGeoText(SpatialType.Polygon, new Action(this.ParsePolygonText));
            this.pipeline.EndGeo();
            break;
          case "POINT":
            this.pipeline.BeginGeo(SpatialType.Point);
            this.ParsePointText();
            this.pipeline.EndGeo();
            break;
          case "POLYGON":
            this.pipeline.BeginGeo(SpatialType.Polygon);
            this.ParsePolygonText();
            this.pipeline.EndGeo();
            break;
          default:
            throw new FormatException(Strings.WellKnownText_UnknownTaggedText((object) this.lexer.CurrentToken.Text));
        }
      }

      private double ReadDouble()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.ReadToken(WellKnownTextTokenType.Number, (string) null);
        stringBuilder.Append(this.lexer.CurrentToken.Text);
        if (this.ReadOptionalToken(WellKnownTextTokenType.Period, (string) null))
        {
          stringBuilder.Append(".");
          this.ReadToken(WellKnownTextTokenType.Number, (string) null);
          stringBuilder.Append(this.lexer.CurrentToken.Text);
        }
        return double.Parse(stringBuilder.ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
      }

      private bool ReadEmptySet() => this.ReadOptionalToken(WellKnownTextTokenType.Text, "EMPTY");

      private int ReadInteger()
      {
        this.ReadToken(WellKnownTextTokenType.Number, (string) null);
        return XmlConvert.ToInt32(this.lexer.CurrentToken.Text);
      }

      private bool TryReadOptionalNullableDouble(out double? value)
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (this.ReadOptionalToken(WellKnownTextTokenType.Number, (string) null))
        {
          stringBuilder.Append(this.lexer.CurrentToken.Text);
          if (this.ReadOptionalToken(WellKnownTextTokenType.Period, (string) null))
          {
            stringBuilder.Append(".");
            this.ReadToken(WellKnownTextTokenType.Number, (string) null);
            stringBuilder.Append(this.lexer.CurrentToken.Text);
          }
          value = new double?(double.Parse(stringBuilder.ToString(), (IFormatProvider) CultureInfo.InvariantCulture));
          return true;
        }
        value = new double?();
        return this.ReadOptionalToken(WellKnownTextTokenType.Text, "NULL");
      }

      private bool ReadOptionalToken(
        WellKnownTextTokenType expectedTokenType,
        string expectedTokenText)
      {
        LexerToken token;
        while (this.lexer.Peek(out token))
        {
          if (token.MatchToken(8, (string) null, StringComparison.OrdinalIgnoreCase))
          {
            this.lexer.Next();
          }
          else
          {
            if (!token.MatchToken((int) expectedTokenType, expectedTokenText, StringComparison.OrdinalIgnoreCase))
              return false;
            this.lexer.Next();
            return true;
          }
        }
        return false;
      }

      private void ReadToken(WellKnownTextTokenType type, string text)
      {
        if (!this.NextToken() || !this.IsTokenMatch(type, text))
          throw new FormatException(Strings.WellKnownText_UnexpectedToken((object) type, (object) text, (object) this.lexer.CurrentToken));
      }
    }
  }
}
