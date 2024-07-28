// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.RowCursor
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  [DebuggerDisplay("{ToString()}")]
  public struct RowCursor
  {
    internal Layout layout;
    internal LayoutScope scopeType;
    internal TypeArgumentList scopeTypeArgs;
    internal bool immutable;
    internal bool deferUniqueIndex;
    internal int start;
    internal bool exists;
    internal UtfAnyString writePath;
    internal StringToken writePathToken;
    internal int pathOffset;
    internal int pathToken;
    internal int metaOffset;
    internal LayoutType cellType;
    internal int valueOffset;
    internal int endOffset;
    internal int count;
    internal int index;
    internal TypeArgumentList cellTypeArgs;

    public static RowCursor Create(ref RowBuffer row)
    {
      SchemaId schemaId = row.ReadSchemaId(1);
      Layout layout = row.Resolver.Resolve(schemaId);
      int variableValueOffset = row.ComputeVariableValueOffset(layout, 5, layout.NumVariable);
      return new RowCursor()
      {
        layout = layout,
        scopeType = (LayoutScope) LayoutType.UDT,
        scopeTypeArgs = new TypeArgumentList(schemaId),
        start = 5,
        metaOffset = variableValueOffset,
        valueOffset = variableValueOffset
      };
    }

    public static ref RowCursor Create(ref RowBuffer row, out RowCursor cursor)
    {
      SchemaId schemaId = row.ReadSchemaId(1);
      Layout layout = row.Resolver.Resolve(schemaId);
      int variableValueOffset = row.ComputeVariableValueOffset(layout, 5, layout.NumVariable);
      cursor = new RowCursor()
      {
        layout = layout,
        scopeType = (LayoutScope) LayoutType.UDT,
        scopeTypeArgs = new TypeArgumentList(schemaId),
        start = 5,
        metaOffset = variableValueOffset,
        valueOffset = variableValueOffset
      };
      return ref cursor;
    }

    public static ref RowCursor CreateForAppend(ref RowBuffer row, out RowCursor cursor)
    {
      SchemaId schemaId = row.ReadSchemaId(1);
      Layout layout = row.Resolver.Resolve(schemaId);
      cursor = new RowCursor()
      {
        layout = layout,
        scopeType = (LayoutScope) LayoutType.UDT,
        scopeTypeArgs = new TypeArgumentList(schemaId),
        start = 5,
        metaOffset = row.Length,
        valueOffset = row.Length
      };
      return ref cursor;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ulong Token
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => checked ((ulong) this.pathToken);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Index
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.index;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Immutable
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.immutable;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public LayoutType ScopeType
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (LayoutType) this.scopeType;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TypeArgumentList ScopeTypeArgs
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.scopeTypeArgs;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Layout Layout
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.layout;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TypeArgument TypeArg
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new TypeArgument(this.cellType, this.cellTypeArgs);
    }

    public override string ToString()
    {
      try
      {
        if (this.scopeType == null)
          return "<Invalid>";
        TypeArgument typeArgument1 = this.scopeType == null || this.scopeType is LayoutEndScope ? new TypeArgument() : new TypeArgument((LayoutType) this.scopeType, this.scopeTypeArgs);
        TypeArgument typeArgument2 = this.cellType == null || this.cellType is LayoutEndScope ? new TypeArgument() : new TypeArgument(this.cellType, this.cellTypeArgs);
        string str = !((UtfAnyString) ref this.writePath).IsNull ? this.writePath.ToString() : this.index.ToString();
        return string.Format("{0}[{1}] : {2}@{3}/{4}", (object) typeArgument1, (object) str, (object) typeArgument2, (object) this.metaOffset, (object) this.valueOffset) + (this.immutable ? " immutable" : string.Empty);
      }
      catch
      {
        return "<???>";
      }
    }
  }
}
