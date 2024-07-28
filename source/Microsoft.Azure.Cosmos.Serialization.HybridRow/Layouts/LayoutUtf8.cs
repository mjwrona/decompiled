// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutUtf8
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutUtf8 : 
    LayoutType<string>,
    ILayoutUtf8SpanWritable,
    ILayoutType,
    ILayoutUtf8SpanReadable
  {
    internal LayoutUtf8()
      : base(LayoutCode.Utf8, 0)
    {
    }

    public override string Name => "utf8";

    public override bool IsFixed => false;

    public override Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      string value)
    {
      Contract.Requires(value != null);
      return this.WriteFixed(ref b, ref scope, col, Utf8Span.TranscodeUtf16(value));
    }

    public Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      Utf8Span value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      Contract.Requires(col.Size >= 0);
      Contract.Requires(((Utf8Span) ref value).Length == col.Size);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      b.WriteFixedString(checked (scope.start + col.Offset), value);
      b.SetBit(scope.start, col.NullBit);
      return Result.Success;
    }

    public override Result ReadFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out string value)
    {
      Utf8Span utf8Span;
      Result result = this.ReadFixed(ref b, ref scope, col, out utf8Span);
      value = result == Result.Success ? utf8Span.ToString() : (string) null;
      return result;
    }

    public Result ReadFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out Utf8Span value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      Contract.Requires(col.Size >= 0);
      if (!b.ReadBit(scope.start, col.NullBit))
      {
        value = new Utf8Span();
        return Result.NotFound;
      }
      value = b.ReadFixedString(checked (scope.start + col.Offset), col.Size);
      return Result.Success;
    }

    public override Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      string value)
    {
      Contract.Requires(value != null);
      return this.WriteVariable(ref b, ref scope, col, Utf8Span.TranscodeUtf16(value));
    }

    public Result WriteVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      Utf8Span value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      int length = ((Utf8Span) ref value).Length;
      if (col.Size > 0 && length > col.Size)
        return Result.TooBig;
      bool exists = b.ReadBit(scope.start, col.NullBit);
      int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
      int shift;
      b.WriteVariableString(variableValueOffset, value, exists, out shift);
      b.SetBit(scope.start, col.NullBit);
      checked { scope.metaOffset += shift; }
      checked { scope.valueOffset += shift; }
      return Result.Success;
    }

    public override Result ReadVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out string value)
    {
      Utf8Span utf8Span;
      Result result = this.ReadVariable(ref b, ref scope, col, out utf8Span);
      value = result == Result.Success ? utf8Span.ToString() : (string) null;
      return result;
    }

    public Result ReadVariable(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out Utf8Span value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (!b.ReadBit(scope.start, col.NullBit))
      {
        value = new Utf8Span();
        return Result.NotFound;
      }
      int variableValueOffset = b.ComputeVariableValueOffset(scope.layout, scope.start, col.Offset);
      value = b.ReadVariableString(variableValueOffset);
      return Result.Success;
    }

    public override Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      string value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Contract.Requires(value != null);
      return this.WriteSparse(ref b, ref edit, Utf8Span.TranscodeUtf16(value), options);
    }

    public Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      Utf8Span value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, this.TypeArg, options);
      if (result != Result.Success)
        return result;
      b.WriteSparseString(ref edit, value, options);
      return Result.Success;
    }

    public override Result ReadSparse(ref RowBuffer b, ref RowCursor edit, out string value)
    {
      Utf8Span utf8Span;
      Result result = this.ReadSparse(ref b, ref edit, out utf8Span);
      value = result == Result.Success ? utf8Span.ToString() : (string) null;
      return result;
    }

    public Result ReadSparse(ref RowBuffer b, ref RowCursor edit, out Utf8Span value)
    {
      Result result = LayoutType.PrepareSparseRead(ref b, ref edit, this.LayoutCode);
      if (result != Result.Success)
      {
        value = new Utf8Span();
        return result;
      }
      value = b.ReadSparseString(ref edit);
      return Result.Success;
    }
  }
}
