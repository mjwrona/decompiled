// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonObjectState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Json
{
  internal sealed class JsonObjectState
  {
    private const int JsonMaxNestingDepth = 256;
    private readonly bool readMode;
    private readonly byte[] nestingStackBitmap;
    private int nestingStackIndex;
    private JsonObjectState.JsonObjectContext currentContext;

    public JsonObjectState(bool readMode)
    {
      this.readMode = readMode;
      this.nestingStackBitmap = new byte[32];
      this.nestingStackIndex = -1;
      this.CurrentTokenType = JsonTokenType.NotStarted;
      this.currentContext = JsonObjectState.JsonObjectContext.None;
    }

    public int CurrentDepth => this.nestingStackIndex + 1;

    public JsonTokenType CurrentTokenType { get; private set; }

    public bool IsPropertyExpected => this.CurrentTokenType != JsonTokenType.FieldName && this.currentContext == JsonObjectState.JsonObjectContext.Object;

    public bool InArrayContext => this.currentContext == JsonObjectState.JsonObjectContext.Array;

    public bool InObjectContext => this.currentContext == JsonObjectState.JsonObjectContext.Object;

    private JsonObjectState.JsonObjectContext RetrieveCurrentContext
    {
      get
      {
        if (this.nestingStackIndex < 0)
          return JsonObjectState.JsonObjectContext.None;
        return ((int) this.nestingStackBitmap[this.nestingStackIndex / 8] & (int) this.Mask) != 0 ? JsonObjectState.JsonObjectContext.Object : JsonObjectState.JsonObjectContext.Array;
      }
    }

    private byte Mask => (byte) (1 << this.nestingStackIndex % 8);

    public void RegisterToken(JsonTokenType jsonTokenType)
    {
      switch (jsonTokenType)
      {
        case JsonTokenType.BeginArray:
          this.RegisterBeginArray();
          break;
        case JsonTokenType.EndArray:
          this.RegisterEndArray();
          break;
        case JsonTokenType.BeginObject:
          this.RegisterBeginObject();
          break;
        case JsonTokenType.EndObject:
          this.RegisterEndObject();
          break;
        case JsonTokenType.String:
        case JsonTokenType.Number:
        case JsonTokenType.True:
        case JsonTokenType.False:
        case JsonTokenType.Null:
        case JsonTokenType.Int8:
        case JsonTokenType.Int16:
        case JsonTokenType.Int32:
        case JsonTokenType.Int64:
        case JsonTokenType.UInt32:
        case JsonTokenType.Float32:
        case JsonTokenType.Float64:
        case JsonTokenType.Guid:
        case JsonTokenType.Binary:
          this.RegisterValue(jsonTokenType);
          break;
        case JsonTokenType.FieldName:
          this.RegisterFieldName();
          break;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.UnexpectedJsonTokenType, (object) jsonTokenType));
      }
    }

    private void Push(bool isArray)
    {
      if (this.nestingStackIndex + 1 >= 256)
        throw new InvalidOperationException(RMResources.JsonMaxNestingExceeded);
      ++this.nestingStackIndex;
      if (isArray)
      {
        this.nestingStackBitmap[this.nestingStackIndex / 8] &= ~this.Mask;
        this.currentContext = JsonObjectState.JsonObjectContext.Array;
      }
      else
      {
        this.nestingStackBitmap[this.nestingStackIndex / 8] |= this.Mask;
        this.currentContext = JsonObjectState.JsonObjectContext.Object;
      }
    }

    private void RegisterValue(JsonTokenType jsonTokenType)
    {
      if (this.currentContext == JsonObjectState.JsonObjectContext.Object && this.CurrentTokenType != JsonTokenType.FieldName)
        throw new JsonMissingPropertyException();
      this.CurrentTokenType = this.currentContext != JsonObjectState.JsonObjectContext.None || this.CurrentTokenType == JsonTokenType.NotStarted ? jsonTokenType : throw new JsonPropertyArrayOrObjectNotStartedException();
    }

    private void RegisterBeginArray()
    {
      this.RegisterValue(JsonTokenType.BeginArray);
      this.Push(true);
    }

    public void RegisterEndArray()
    {
      if (this.currentContext != JsonObjectState.JsonObjectContext.Array)
      {
        if (this.readMode)
          throw new JsonUnexpectedEndArrayException();
        throw new JsonArrayNotStartedException();
      }
      --this.nestingStackIndex;
      this.CurrentTokenType = JsonTokenType.EndArray;
      this.currentContext = this.RetrieveCurrentContext;
    }

    private void RegisterBeginObject()
    {
      this.RegisterValue(JsonTokenType.BeginObject);
      this.Push(false);
    }

    public void RegisterEndObject()
    {
      if (this.currentContext != JsonObjectState.JsonObjectContext.Object)
      {
        if (this.readMode)
          throw new JsonUnexpectedEndObjectException();
        throw new JsonObjectNotStartedException();
      }
      if (this.CurrentTokenType == JsonTokenType.FieldName)
      {
        if (this.readMode)
          throw new JsonUnexpectedEndObjectException();
        throw new JsonNotCompleteException();
      }
      --this.nestingStackIndex;
      this.CurrentTokenType = JsonTokenType.EndObject;
      this.currentContext = this.RetrieveCurrentContext;
    }

    public void RegisterFieldName()
    {
      if (this.currentContext != JsonObjectState.JsonObjectContext.Object)
        throw new JsonObjectNotStartedException();
      this.CurrentTokenType = this.CurrentTokenType != JsonTokenType.FieldName ? JsonTokenType.FieldName : throw new JsonPropertyAlreadyAddedException();
    }

    private enum JsonObjectContext
    {
      None,
      Array,
      Object,
    }
  }
}
