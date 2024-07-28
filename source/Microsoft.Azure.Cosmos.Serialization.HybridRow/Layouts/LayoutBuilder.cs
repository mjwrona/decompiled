// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutBuilder
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  internal sealed class LayoutBuilder
  {
    private readonly string name;
    private readonly SchemaId schemaId;
    private LayoutBit.Allocator bitallocator;
    private List<LayoutColumn> fixedColumns;
    private int fixedCount;
    private int fixedSize;
    private List<LayoutColumn> varColumns;
    private int varCount;
    private List<LayoutColumn> sparseColumns;
    private int sparseCount;
    private Stack<LayoutColumn> scope;

    public LayoutBuilder(string name, SchemaId schemaId)
    {
      this.name = name;
      this.schemaId = schemaId;
      this.Reset();
    }

    private LayoutColumn Parent => this.scope.Count == 0 ? (LayoutColumn) null : this.scope.Peek();

    public void AddFixedColumn(string path, LayoutType type, bool nullable, int length = 0)
    {
      Contract.Requires(length >= 0);
      Contract.Requires(!type.IsVarint);
      LayoutColumn layoutColumn;
      if (type.IsNull)
      {
        Contract.Requires(nullable);
        LayoutBit nullBit = this.bitallocator.Allocate();
        layoutColumn = new LayoutColumn(path, type, TypeArgumentList.Empty, StorageKind.Fixed, this.Parent, this.fixedCount, 0, nullBit, LayoutBit.Invalid);
      }
      else if (type.IsBool)
      {
        LayoutBit nullBit = nullable ? this.bitallocator.Allocate() : LayoutBit.Invalid;
        LayoutBit boolBit = this.bitallocator.Allocate();
        layoutColumn = new LayoutColumn(path, type, TypeArgumentList.Empty, StorageKind.Fixed, this.Parent, this.fixedCount, 0, nullBit, boolBit);
      }
      else
      {
        LayoutBit nullBit = nullable ? this.bitallocator.Allocate() : LayoutBit.Invalid;
        layoutColumn = new LayoutColumn(path, type, TypeArgumentList.Empty, StorageKind.Fixed, this.Parent, this.fixedCount, this.fixedSize, nullBit, LayoutBit.Invalid, length);
        checked { this.fixedSize += type.IsFixed ? type.Size : length; }
      }
      checked { ++this.fixedCount; }
      this.fixedColumns.Add(layoutColumn);
    }

    public void AddVariableColumn(string path, LayoutType type, int length = 0)
    {
      Contract.Requires(length >= 0);
      Contract.Requires(type.AllowVariable);
      LayoutColumn layoutColumn = new LayoutColumn(path, type, TypeArgumentList.Empty, StorageKind.Variable, this.Parent, this.varCount, this.varCount, this.bitallocator.Allocate(), LayoutBit.Invalid, length);
      checked { ++this.varCount; }
      this.varColumns.Add(layoutColumn);
    }

    public void AddSparseColumn(string path, LayoutType type)
    {
      LayoutColumn layoutColumn = new LayoutColumn(path, type, TypeArgumentList.Empty, StorageKind.Sparse, this.Parent, this.sparseCount, -1, LayoutBit.Invalid, LayoutBit.Invalid);
      checked { ++this.sparseCount; }
      this.sparseColumns.Add(layoutColumn);
    }

    public void AddObjectScope(string path, LayoutType type)
    {
      LayoutColumn layoutColumn = new LayoutColumn(path, type, TypeArgumentList.Empty, StorageKind.Sparse, this.Parent, this.sparseCount, -1, LayoutBit.Invalid, LayoutBit.Invalid);
      checked { ++this.sparseCount; }
      this.sparseColumns.Add(layoutColumn);
      this.scope.Push(layoutColumn);
    }

    public void EndObjectScope()
    {
      Contract.Requires(this.scope.Count > 0);
      this.scope.Pop();
    }

    public void AddTypedScope(string path, LayoutType type, TypeArgumentList typeArgs)
    {
      LayoutColumn layoutColumn = new LayoutColumn(path, type, typeArgs, StorageKind.Sparse, this.Parent, this.sparseCount, -1, LayoutBit.Invalid, LayoutBit.Invalid);
      checked { ++this.sparseCount; }
      this.sparseColumns.Add(layoutColumn);
    }

    public Layout Build()
    {
      int numBytes = this.bitallocator.NumBytes;
      int fixedCount = this.fixedCount;
      List<LayoutColumn> columns = new List<LayoutColumn>(checked (this.fixedColumns.Count + this.varColumns.Count));
      foreach (LayoutColumn fixedColumn in this.fixedColumns)
      {
        fixedColumn.SetOffset(checked (fixedColumn.Offset + numBytes));
        columns.Add(fixedColumn);
      }
      foreach (LayoutColumn varColumn in this.varColumns)
      {
        varColumn.SetIndex(checked (varColumn.Index + fixedCount));
        columns.Add(varColumn);
      }
      columns.AddRange((IEnumerable<LayoutColumn>) this.sparseColumns);
      Layout layout = new Layout(this.name, this.schemaId, this.bitallocator.NumBytes, checked (this.fixedSize + numBytes), columns);
      this.Reset();
      return layout;
    }

    private void Reset()
    {
      this.bitallocator = new LayoutBit.Allocator();
      this.fixedSize = 0;
      this.fixedCount = 0;
      this.fixedColumns = new List<LayoutColumn>();
      this.varCount = 0;
      this.varColumns = new List<LayoutColumn>();
      this.sparseCount = 0;
      this.sparseColumns = new List<LayoutColumn>();
      this.scope = new Stack<LayoutColumn>();
    }
  }
}
