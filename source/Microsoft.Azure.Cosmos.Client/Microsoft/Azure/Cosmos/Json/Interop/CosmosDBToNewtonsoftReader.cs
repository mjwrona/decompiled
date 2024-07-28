// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.Interop.CosmosDBToNewtonsoftReader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Cosmos.Json.Interop
{
  internal sealed class CosmosDBToNewtonsoftReader : Newtonsoft.Json.JsonReader
  {
    private static readonly object Null = (object) null;
    private static readonly object False = (object) false;
    private static readonly object True = (object) true;
    private readonly IJsonReader jsonReader;

    public CosmosDBToNewtonsoftReader(IJsonReader jsonReader) => this.jsonReader = jsonReader ?? throw new ArgumentNullException(nameof (jsonReader));

    public override bool Read()
    {
      bool flag = this.jsonReader.Read();
      if (!flag)
      {
        this.SetToken(JsonToken.None);
        return false;
      }
      JsonTokenType currentTokenType = this.jsonReader.CurrentTokenType;
      JsonToken newToken;
      object obj;
      switch (currentTokenType)
      {
        case JsonTokenType.BeginArray:
          newToken = JsonToken.StartArray;
          obj = CosmosDBToNewtonsoftReader.Null;
          break;
        case JsonTokenType.EndArray:
          newToken = JsonToken.EndArray;
          obj = CosmosDBToNewtonsoftReader.Null;
          break;
        case JsonTokenType.BeginObject:
          newToken = JsonToken.StartObject;
          obj = CosmosDBToNewtonsoftReader.Null;
          break;
        case JsonTokenType.EndObject:
          newToken = JsonToken.EndObject;
          obj = CosmosDBToNewtonsoftReader.Null;
          break;
        case JsonTokenType.String:
          newToken = JsonToken.String;
          obj = (object) this.jsonReader.GetStringValue().ToString();
          break;
        case JsonTokenType.Number:
          Number64 numberValue = this.jsonReader.GetNumberValue();
          if (numberValue.IsInteger)
          {
            obj = (object) Number64.ToLong(numberValue);
            newToken = JsonToken.Integer;
            break;
          }
          obj = (object) Number64.ToDouble(numberValue);
          newToken = JsonToken.Float;
          break;
        case JsonTokenType.True:
          newToken = JsonToken.Boolean;
          obj = CosmosDBToNewtonsoftReader.True;
          break;
        case JsonTokenType.False:
          newToken = JsonToken.Boolean;
          obj = CosmosDBToNewtonsoftReader.False;
          break;
        case JsonTokenType.Null:
          newToken = JsonToken.Null;
          obj = CosmosDBToNewtonsoftReader.Null;
          break;
        case JsonTokenType.FieldName:
          newToken = JsonToken.PropertyName;
          obj = (object) this.jsonReader.GetStringValue().ToString();
          break;
        case JsonTokenType.Int8:
          newToken = JsonToken.Integer;
          obj = (object) this.jsonReader.GetInt8Value();
          break;
        case JsonTokenType.Int16:
          newToken = JsonToken.Integer;
          obj = (object) this.jsonReader.GetInt16Value();
          break;
        case JsonTokenType.Int32:
          newToken = JsonToken.Integer;
          obj = (object) this.jsonReader.GetInt32Value();
          break;
        case JsonTokenType.Int64:
          newToken = JsonToken.Integer;
          obj = (object) this.jsonReader.GetInt64Value();
          break;
        case JsonTokenType.UInt32:
          newToken = JsonToken.Integer;
          obj = (object) this.jsonReader.GetUInt32Value();
          break;
        case JsonTokenType.Float32:
          newToken = JsonToken.Float;
          obj = (object) this.jsonReader.GetFloat32Value();
          break;
        case JsonTokenType.Float64:
          newToken = JsonToken.Float;
          obj = (object) this.jsonReader.GetFloat64Value();
          break;
        case JsonTokenType.Guid:
          newToken = JsonToken.String;
          obj = (object) this.jsonReader.GetGuidValue().ToString();
          break;
        case JsonTokenType.Binary:
          newToken = JsonToken.Bytes;
          obj = (object) this.jsonReader.GetBinaryValue().ToArray();
          break;
        default:
          throw new ArgumentException(string.Format("Unexpected jsonTokenType: {0}", (object) currentTokenType));
      }
      this.SetToken(newToken, obj);
      return flag;
    }

    public override byte[] ReadAsBytes() => throw new NotImplementedException();

    public override DateTime? ReadAsDateTime()
    {
      this.Read();
      if (this.jsonReader.CurrentTokenType == JsonTokenType.EndArray)
        return new DateTime?();
      DateTime dateTime = DateTime.Parse(UtfAnyString.op_Implicit(this.jsonReader.GetStringValue()));
      this.SetToken(JsonToken.Date, (object) dateTime);
      return new DateTime?(dateTime);
    }

    public override DateTimeOffset? ReadAsDateTimeOffset()
    {
      this.Read();
      if (this.jsonReader.CurrentTokenType == JsonTokenType.EndArray)
        return new DateTimeOffset?();
      DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(UtfAnyString.op_Implicit(this.jsonReader.GetStringValue()));
      this.SetToken(JsonToken.Date, (object) dateTimeOffset);
      return new DateTimeOffset?(dateTimeOffset);
    }

    public override Decimal? ReadAsDecimal()
    {
      double? nullable1 = this.ReadNumberValue();
      Decimal? nullable2 = nullable1.HasValue ? new Decimal?((Decimal) nullable1.GetValueOrDefault()) : new Decimal?();
      if (nullable2.HasValue)
        this.SetToken(JsonToken.Float, (object) nullable2);
      return nullable2;
    }

    public override int? ReadAsInt32()
    {
      double? nullable1 = this.ReadNumberValue();
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      if (nullable2.HasValue)
        this.SetToken(JsonToken.Integer, (object) nullable2);
      return nullable2;
    }

    public override string ReadAsString()
    {
      this.Read();
      if (this.jsonReader.CurrentTokenType == JsonTokenType.EndArray)
        return (string) null;
      string str = UtfAnyString.op_Implicit(this.jsonReader.GetStringValue());
      this.SetToken(JsonToken.String, (object) str);
      return str;
    }

    private double? ReadNumberValue()
    {
      this.Read();
      return this.jsonReader.CurrentTokenType == JsonTokenType.EndArray ? new double?() : new double?(Number64.ToDouble(this.jsonReader.GetNumberValue()));
    }
  }
}
