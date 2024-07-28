// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.Edm.TableEntityWriter
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Documents.Interop.Common.Schema.Edm
{
  internal sealed class TableEntityWriter : ITableEntityWriter, IDisposable
  {
    private readonly TextWriter textWriter;
    private TableEntityWriterContext context;
    private TableEntityWriterState state;
    private string elemantName;
    private bool disposed;

    public TableEntityWriter(TextWriter writer)
    {
      this.textWriter = writer != null ? writer : throw new ArgumentNullException(nameof (writer));
      this.context = new TableEntityWriterContext();
      this.state = TableEntityWriterState.Initial;
    }

    public void Close()
    {
      if (this.state == TableEntityWriterState.CLosed)
        return;
      this.Flush();
      this.state = TableEntityWriterState.CLosed;
    }

    public void Flush()
    {
      this.ThrowIfDisposed();
      this.textWriter.Flush();
    }

    public void Start()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState("WriteStart", new TableEntityWriterState[1]);
      this.textWriter.Write('{');
      this.state = TableEntityWriterState.Name;
    }

    public void End()
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState("WriteStart", TableEntityWriterState.Name);
      this.textWriter.Write('}');
      this.state = TableEntityWriterState.Done;
    }

    public void WriteName(string name)
    {
      if (name == null)
        throw new ArgumentException(nameof (name));
      if (name.StartsWith(EdmSchemaMapping.SystemPropertiesPrefix, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(nameof (name));
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteName), TableEntityWriterState.Name);
      this.elemantName = !EdmSchemaMapping.IsDocumentDBProperty(name) ? name : EdmSchemaMapping.SystemPropertiesPrefix + name;
      this.state = TableEntityWriterState.Value;
    }

    public void WriteString(string value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteString), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (value == null)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.String);
      else
        this.WriteValue<string>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.String, SchemaUtil.GetQuotedString(value));
      this.UpdateWriterState();
    }

    public void WriteBinary(byte[] value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteBinary), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (value == null)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Binary);
      else
        this.WriteValue<string>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Binary, SchemaUtil.GetQuotedString(SchemaUtil.BytesToString(value)));
      this.UpdateWriterState();
    }

    public void WriteBoolean(bool? value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteBoolean), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (!value.HasValue)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Boolean);
      else
        this.WriteValue<string>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Boolean, value.Value ? "true" : "false");
      this.UpdateWriterState();
    }

    public void WriteDateTime(DateTime? value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteDateTime), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (!value.HasValue)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.DateTime);
      else
        this.WriteValue<string>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.DateTime, SchemaUtil.GetQuotedString(SchemaUtil.GetUtcTicksString(value.Value)));
      this.UpdateWriterState();
    }

    public void WriteDouble(double? value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteDouble), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (!value.HasValue)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Double);
      else
        this.WriteValue<string>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Double, value.Value.ToString("G17", (IFormatProvider) CultureInfo.InvariantCulture));
      this.UpdateWriterState();
    }

    public void WriteGuid(Guid? value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteGuid), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (!value.HasValue)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Guid);
      else
        this.WriteValue<string>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Guid, SchemaUtil.GetQuotedString(value.ToString()));
      this.UpdateWriterState();
    }

    public void WriteInt32(int? value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteInt32), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (!value.HasValue)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Int32);
      else
        this.WriteValue<int>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Int32, value.Value);
      this.UpdateWriterState();
    }

    public void WriteInt64(long? value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfInvalidState(nameof (WriteInt64), TableEntityWriterState.Value);
      this.WriteNameAux(this.elemantName);
      if (!value.HasValue)
        this.WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Int64);
      else
        this.WriteValue<string>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType.Int64, SchemaUtil.GetQuotedString(value.Value.ToString("D20", (IFormatProvider) CultureInfo.InvariantCulture)));
      this.UpdateWriterState();
    }

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.Close();
      this.disposed = true;
    }

    private void WriteNameAux(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (this.context.HasElements)
        this.textWriter.Write(", ");
      this.textWriter.Write(SchemaUtil.GetQuotedString(name));
      this.textWriter.Write(": ");
      this.context.HasElements = true;
    }

    private void UpdateWriterState()
    {
      if (this.state == TableEntityWriterState.Name)
        this.state = TableEntityWriterState.Value;
      else
        this.state = TableEntityWriterState.Name;
    }

    private void WriteNull(Microsoft.Azure.Documents.Interop.Common.Schema.DataType type)
    {
      this.textWriter.Write('{');
      this.textWriter.Write("\"{0}\": {1}", (object) "$t", (object) (int) type);
      this.textWriter.Write(", ");
      this.textWriter.Write("\"{0}\": {1}", (object) "$v", (object) "null");
      this.textWriter.Write('}');
    }

    private void WriteValue<TValue>(Microsoft.Azure.Documents.Interop.Common.Schema.DataType type, TValue value)
    {
      if ((object) value == null)
      {
        this.WriteNull(type);
      }
      else
      {
        this.textWriter.Write('{');
        this.textWriter.Write("\"{0}\": {1}", (object) "$t", (object) (int) type);
        this.textWriter.Write(", ");
        this.textWriter.Write("\"{0}\": {1}", (object) "$v", (object) value);
        this.textWriter.Write('}');
      }
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(nameof (TableEntityWriter));
    }

    private void ThrowIfInvalidState(string methodName, params TableEntityWriterState[] validStates)
    {
      foreach (TableEntityWriterState validState in validStates)
      {
        if (this.state == validState)
          return;
      }
      string str = string.Join(" or ", ((IEnumerable<TableEntityWriterState>) validStates).Select<TableEntityWriterState, string>((Func<TableEntityWriterState, string>) (s => s.ToString())).ToArray<string>());
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} can only be called when state is {1}, actual state is {2}", (object) methodName, (object) str, (object) this.state.ToString()));
    }
  }
}
