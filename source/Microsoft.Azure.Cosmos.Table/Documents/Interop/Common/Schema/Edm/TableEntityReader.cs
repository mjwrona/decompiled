// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.Edm.TableEntityReader
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents.Interop.Common.Schema.Edm
{
  internal sealed class TableEntityReader : ITableEntityReader, IDisposable
  {
    private static List<int> edmTypeValues = new List<int>()
    {
      2,
      1,
      8,
      5,
      0,
      16,
      18,
      9
    };
    private readonly JsonScanner scanner;
    private TableEntityReaderState state;
    private JsonToken currentToken;
    private JsonToken pushedToken;
    private JsonToken currentValue;
    private string currentName;
    private Microsoft.Azure.Documents.Interop.Common.Schema.DataType currentEdmType;
    private bool disposed;

    public TableEntityReader(string json)
    {
      this.scanner = !string.IsNullOrEmpty(json) ? new JsonScanner(json) : throw new ArgumentException(nameof (json));
      this.state = TableEntityReaderState.Initial;
    }

    public string CurrentName
    {
      get
      {
        this.ThrowIfDisposed();
        this.ThrowIfInvalidState("get_CurrentName", TableEntityReaderState.HasValue);
        return this.currentName;
      }
    }

    public Microsoft.Azure.Documents.Interop.Common.Schema.DataType CurrentType
    {
      get
      {
        this.ThrowIfDisposed();
        this.ThrowIfInvalidState("get_CurrentType", TableEntityReaderState.HasValue);
        return this.currentEdmType;
      }
    }

    public void Start()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (Start), new TableEntityReaderState[1]);
      this.Expect(JsonTokenType.BeginObject);
    }

    public void End()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (End), TableEntityReaderState.Done);
    }

    public bool MoveNext()
    {
      this.ThrowIfDisposed();
      if (this.state == TableEntityReaderState.Done)
        return false;
      JsonToken jsonToken1 = this.PopToken();
      if (jsonToken1.Type == JsonTokenType.EndObject)
      {
        this.state = TableEntityReaderState.Done;
        return false;
      }
      if (jsonToken1.Type == JsonTokenType.String)
        this.currentName = jsonToken1.GetStringValue();
      else
        this.ThrowFormatException("Expecting a name but found '{0}'", (object) jsonToken1.Lexeme);
      this.Expect(JsonTokenType.Colon);
      JsonToken jsonToken2 = this.PopToken();
      if (EdmSchemaMapping.IsDocumentDBProperty(this.currentName) || this.currentName == "$pk" || this.currentName == "$id")
      {
        switch (jsonToken2.Type)
        {
          case JsonTokenType.Number:
            this.currentEdmType = Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Double;
            this.currentValue = jsonToken2;
            break;
          case JsonTokenType.String:
            this.currentEdmType = Microsoft.Azure.Documents.Interop.Common.Schema.DataType.String;
            this.currentValue = jsonToken2;
            break;
          default:
            this.ThrowFormatException("Unexpected value type '{0}' for DocumentDB property.", (object) jsonToken2.Type);
            break;
        }
      }
      else
      {
        if (jsonToken2.Type != JsonTokenType.BeginObject)
          this.ThrowFormatException("Value is expected to be an object instead it was '{0}'.", (object) jsonToken2.Type);
        this.currentEdmType = this.ParseEdmType();
      }
      this.TryReadComma();
      this.state = TableEntityReaderState.HasValue;
      return true;
    }

    public string ReadString()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadString), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadString), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.String);
      if (this.currentValue.Type == JsonTokenType.Null)
        return (string) null;
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.String);
      return this.currentValue.GetStringValue();
    }

    public byte[] ReadBinary()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadBinary), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadBinary), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Binary);
      if (this.currentValue.Type == JsonTokenType.Null)
        return (byte[]) null;
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.String);
      return SchemaUtil.StringToBytes(this.currentValue.GetStringValue());
    }

    public bool? ReadBoolean()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadBoolean), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadBoolean), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Boolean);
      if (this.currentValue.Type == JsonTokenType.Null)
        return new bool?();
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.Boolean);
      return new bool?(this.currentValue.GetBooleanValue());
    }

    public DateTime? ReadDateTime()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadDateTime), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadDateTime), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.DateTime);
      if (this.currentValue.Type == JsonTokenType.Null)
        return new DateTime?();
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.String);
      return new DateTime?(SchemaUtil.GetDateTimeFromUtcTicks(long.Parse(this.currentValue.GetStringValue(), (IFormatProvider) CultureInfo.InvariantCulture)));
    }

    public double? ReadDouble()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadDouble), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadDouble), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Double);
      if (this.currentValue.Type == JsonTokenType.Null)
        return new double?();
      if (this.currentValue.Type == JsonTokenType.String)
        return new double?(double.Parse(this.currentValue.GetStringValue(), (IFormatProvider) CultureInfo.InvariantCulture));
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.Number);
      return new double?(this.currentValue.GetDoubleValue());
    }

    public Guid? ReadGuid()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadGuid), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadGuid), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Guid);
      if (this.currentValue.Type == JsonTokenType.Null)
        return new Guid?();
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.String);
      return new Guid?(Guid.Parse(this.currentValue.GetStringValue()));
    }

    public int? ReadInt32()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadInt32), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadInt32), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Int32);
      if (this.currentValue.Type == JsonTokenType.Null)
        return new int?();
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.Number);
      return new int?((int) this.currentValue.GetDoubleValue());
    }

    public long? ReadInt64()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (ReadInt64), TableEntityReaderState.HasValue);
      this.ThrowIfInvalidType(nameof (ReadInt64), Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Int64);
      if (this.currentValue.Type == JsonTokenType.Null)
        return new long?();
      this.EnsureMatchingTypes(this.currentValue.Type, JsonTokenType.String);
      return new long?(long.Parse(this.currentValue.GetStringValue(), (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.disposed = true;
    }

    private Microsoft.Azure.Documents.Interop.Common.Schema.DataType ParseEdmType()
    {
      JsonToken jsonToken = this.PopToken();
      if (jsonToken.Type != JsonTokenType.String || jsonToken.GetStringValue() != "$t")
        this.ThrowFormatException("Expecting type marker but found '{0}'", (object) jsonToken.Type.ToString());
      this.Expect(JsonTokenType.Colon);
      this.Expect(JsonTokenType.Number);
      double doubleValue = this.currentToken.GetDoubleValue();
      if (doubleValue % 1.0 != 0.0 || !TableEntityReader.edmTypeValues.Contains((int) doubleValue))
        this.ThrowFormatException("Invalid Edm type: {0}", (object) doubleValue);
      int edmType = (int) doubleValue;
      this.Expect(JsonTokenType.Comma);
      this.Expect("$v");
      this.Expect(JsonTokenType.Colon);
      this.currentToken = this.PopToken();
      this.currentValue = this.currentToken;
      this.Expect(JsonTokenType.EndObject);
      return (Microsoft.Azure.Documents.Interop.Common.Schema.DataType) edmType;
    }

    private void Expect(JsonTokenType type)
    {
      JsonToken jsonToken = this.PopToken();
      if (jsonToken.Type != type)
        this.ThrowFormatException("Expecting type {0} but found {1}", (object) type, (object) jsonToken.Type);
      this.currentToken = jsonToken;
    }

    private void EnsureMatchingTypes(JsonTokenType type1, JsonTokenType type2)
    {
      if (type1 == type2)
        return;
      this.ThrowFormatException("type should be {0} but found {1}", (object) type1, (object) type2);
    }

    private void Expect(string stringToken)
    {
      this.Expect(JsonTokenType.String);
      if (!(this.currentToken.GetStringValue() != stringToken))
        return;
      this.ThrowFormatException("Expecting token {0} but found {1}", (object) stringToken, (object) this.currentToken.GetStringValue());
    }

    private void PushToken(JsonToken token) => this.pushedToken = token;

    private JsonToken PopToken()
    {
      if (this.pushedToken != null)
      {
        JsonToken pushedToken = this.pushedToken;
        this.pushedToken = (JsonToken) null;
        return pushedToken;
      }
      return this.scanner.ScanNext() ? this.scanner.GetCurrentToken() : throw new Exception("Scanner failed.");
    }

    private bool TryReadComma()
    {
      JsonToken token = this.PopToken();
      if (token.Type == JsonTokenType.Comma)
        return true;
      this.PushToken(token);
      return false;
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(nameof (TableEntityReader));
    }

    private void ThrowIfInvalidState(string methodName, params TableEntityReaderState[] validStates)
    {
      foreach (TableEntityReaderState validState in validStates)
      {
        if (this.state == validState)
          return;
      }
      string str = string.Join(" or ", ((IEnumerable<TableEntityReaderState>) validStates).Select<TableEntityReaderState, string>((Func<TableEntityReaderState, string>) (s => s.ToString())).ToArray<string>());
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} can only be called when state is {1}, actual state is {2}", (object) methodName, (object) str, (object) this.state.ToString()));
    }

    private void ThrowIfInvalidType(string methodName, Microsoft.Azure.Documents.Interop.Common.Schema.DataType expectedType)
    {
      if (this.currentEdmType != expectedType)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} expects current type to be {1}, actual type is {2}", (object) methodName, (object) expectedType.ToString(), (object) this.currentEdmType.ToString()));
    }

    private void ThrowFormatException(string format, params object[] args) => throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));
  }
}
