// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.JsonToken
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Documents.Interop.Common.Schema
{
  internal class JsonToken
  {
    private static readonly JsonToken BeginObjectToken = new JsonToken(JsonTokenType.BeginObject, "{");
    private static readonly JsonToken EndObjectToken = new JsonToken(JsonTokenType.EndObject, "}");
    private static readonly JsonToken BeginArrayToken = new JsonToken(JsonTokenType.BeginArray, "[");
    private static readonly JsonToken EndArrayToken = new JsonToken(JsonTokenType.EndArray, "]");
    private static readonly JsonToken ColonToken = new JsonToken(JsonTokenType.Colon, ":");
    private static readonly JsonToken CommaToken = new JsonToken(JsonTokenType.Comma, ",");
    private static readonly JsonToken NullToken = new JsonToken(JsonTokenType.Null, "null");
    private static readonly JsonToken.NumberJsonToken NaNToken = new JsonToken.NumberJsonToken(double.NaN, nameof (NaN));
    private static readonly JsonToken.NumberJsonToken InfinityToken = new JsonToken.NumberJsonToken(double.PositiveInfinity, nameof (Infinity));
    private static readonly JsonToken.BooleanJsonToken TrueToken = new JsonToken.BooleanJsonToken(true, "true");
    private static readonly JsonToken.BooleanJsonToken FalseToken = new JsonToken.BooleanJsonToken(false, "false");
    private readonly JsonTokenType type;
    private readonly string lexeme;

    protected JsonToken(JsonTokenType type, string lexeme)
    {
      this.type = type;
      this.lexeme = lexeme;
    }

    public JsonTokenType Type => this.type;

    public string Lexeme => this.lexeme;

    public virtual double GetDoubleValue() => throw new InvalidCastException();

    public virtual long GetInt64Value() => throw new InvalidCastException();

    public virtual string GetStringValue() => throw new InvalidCastException();

    public virtual bool GetBooleanValue() => throw new InvalidCastException();

    public static JsonToken BeginObject => JsonToken.BeginObjectToken;

    public static JsonToken EndObject => JsonToken.EndObjectToken;

    public static JsonToken BeginArray => JsonToken.BeginArrayToken;

    public static JsonToken EndArray => JsonToken.EndArrayToken;

    public static JsonToken Colon => JsonToken.ColonToken;

    public static JsonToken Comma => JsonToken.CommaToken;

    public static JsonToken Null => JsonToken.NullToken;

    public static JsonToken True => (JsonToken) JsonToken.TrueToken;

    public static JsonToken False => (JsonToken) JsonToken.FalseToken;

    public static JsonToken NaN => (JsonToken) JsonToken.NaNToken;

    public static JsonToken Infinity => (JsonToken) JsonToken.InfinityToken;

    public static JsonToken NumberToken(double value, string lexeme) => (JsonToken) new JsonToken.NumberJsonToken(value, lexeme);

    public static JsonToken StringToken(string value, string lexeme) => (JsonToken) new JsonToken.StringJsonToken(value, lexeme);

    private sealed class BooleanJsonToken : JsonToken
    {
      private readonly bool value;

      public BooleanJsonToken(bool value, string lexeme)
        : base(JsonTokenType.Boolean, lexeme)
      {
        this.value = value;
      }

      public override bool GetBooleanValue() => this.value;
    }

    private sealed class NumberJsonToken : JsonToken
    {
      private readonly double value;

      public NumberJsonToken(double value, string lexeme)
        : base(JsonTokenType.Number, lexeme)
      {
        if (lexeme == null)
          throw new ArgumentNullException(nameof (lexeme));
        this.value = value;
      }

      public override double GetDoubleValue() => this.value;

      public override long GetInt64Value() => Convert.ToInt64(this.value);
    }

    private sealed class StringJsonToken : JsonToken
    {
      private readonly string value;

      public StringJsonToken(string value, string lexeme)
        : base(JsonTokenType.String, lexeme)
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        if (lexeme == null)
          throw new ArgumentNullException(nameof (lexeme));
        this.value = value;
      }

      public override string GetStringValue() => this.value;
    }
  }
}
