// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.RowCursorExtensions
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  public static class RowCursorExtensions
  {
    public static ref RowCursor Clone(in this RowCursor src, out RowCursor dest)
    {
      dest = src;
      return ref dest;
    }

    public static ref RowCursor AsReadOnly(in this RowCursor src, out RowCursor dest)
    {
      dest = src;
      dest.immutable = true;
      return ref dest;
    }

    public static ref RowCursor Find(ref this RowCursor edit, ref RowBuffer row, UtfAnyString path)
    {
      Contract.Requires(!edit.scopeType.IsIndexedScope);
      if (!(edit.cellType is LayoutEndScope))
      {
        while (row.SparseIteratorMoveNext(ref edit))
        {
          if (((UtfAnyString) ref path).Equals(row.ReadSparsePath(ref edit)))
          {
            edit.exists = true;
            break;
          }
        }
      }
      edit.writePath = path;
      edit.writePathToken = new StringToken();
      return ref edit;
    }

    public static ref RowCursor Find(
      ref this RowCursor edit,
      ref RowBuffer row,
      in StringToken pathToken)
    {
      Contract.Requires(!edit.scopeType.IsIndexedScope);
      if (!(edit.cellType is LayoutEndScope))
      {
        while (row.SparseIteratorMoveNext(ref edit))
        {
          if ((long) pathToken.Id == (long) checked ((uint) edit.pathToken))
          {
            edit.exists = true;
            break;
          }
        }
      }
      edit.writePath = Utf8String.op_Implicit(pathToken.Path);
      edit.writePathToken = pathToken;
      return ref edit;
    }

    public static bool MoveNext(ref this RowCursor edit, ref RowBuffer row)
    {
      edit.writePath = new UtfAnyString();
      edit.writePathToken = new StringToken();
      return row.SparseIteratorMoveNext(ref edit);
    }

    public static bool MoveTo(ref this RowCursor edit, ref RowBuffer row, int index)
    {
      edit.writePath = new UtfAnyString();
      edit.writePathToken = new StringToken();
      while (edit.index < index)
      {
        if (!row.SparseIteratorMoveNext(ref edit))
          return false;
      }
      return true;
    }

    public static bool MoveNext(
      ref this RowCursor edit,
      ref RowBuffer row,
      ref RowCursor childScope)
    {
      if (childScope.scopeType != null)
        edit.Skip(ref row, ref childScope);
      return edit.MoveNext(ref row);
    }

    public static void Skip(ref this RowCursor edit, ref RowBuffer row, ref RowCursor childScope)
    {
      Contract.Requires(childScope.start == edit.valueOffset);
      if (!(childScope.cellType is LayoutEndScope))
      {
        while (row.SparseIteratorMoveNext(ref childScope))
          ;
      }
      if (childScope.scopeType.IsSizedScope)
        edit.endOffset = childScope.metaOffset;
      else
        edit.endOffset = checked (childScope.metaOffset + 1);
    }
  }
}
