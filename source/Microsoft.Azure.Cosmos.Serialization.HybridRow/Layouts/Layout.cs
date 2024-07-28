// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.Layout
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class Layout
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly Layout Empty = SystemSchema.LayoutResolver.Resolve(SystemSchema.EmptySchemaId);
    private readonly LayoutColumn[] topColumns;
    private readonly Dictionary<Utf8String, LayoutColumn> pathMap;
    private readonly Dictionary<string, LayoutColumn> pathStringMap;

    internal Layout(
      string name,
      SchemaId schemaId,
      int numBitmaskBytes,
      int minRequiredSize,
      List<LayoutColumn> columns)
    {
      this.Name = name;
      this.SchemaId = schemaId;
      this.NumBitmaskBytes = numBitmaskBytes;
      this.Size = minRequiredSize;
      this.Tokenizer = new StringTokenizer();
      this.pathMap = new Dictionary<Utf8String, LayoutColumn>(columns.Count, (IEqualityComparer<Utf8String>) SamplingUtf8StringComparer.Default);
      this.pathStringMap = new Dictionary<string, LayoutColumn>(columns.Count);
      this.NumFixed = 0;
      this.NumVariable = 0;
      List<LayoutColumn> layoutColumnList = new List<LayoutColumn>(columns.Count);
      foreach (LayoutColumn column in columns)
      {
        this.Tokenizer.Add(column.Path);
        this.pathMap.Add(column.FullPath, column);
        this.pathStringMap.Add(column.FullPath.ToString(), column);
        if (column.Storage == StorageKind.Fixed)
          this.NumFixed = checked (this.NumFixed + 1);
        else if (column.Storage == StorageKind.Variable)
          this.NumVariable = checked (this.NumVariable + 1);
        if (column.Parent == null)
          layoutColumnList.Add(column);
      }
      this.topColumns = layoutColumnList.ToArray();
    }

    public string Name { get; }

    public SchemaId SchemaId { get; }

    public Layout.ColumnView Columns => new Layout.ColumnView(this.topColumns);

    public int Size { get; }

    public int NumBitmaskBytes { get; }

    public int NumFixed { get; }

    public int NumVariable { get; }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Bug in Analyzer. This is an auto-property not a method.")]
    public StringTokenizer Tokenizer { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    public bool TryFind(UtfAnyString path, out LayoutColumn column)
    {
      if (((UtfAnyString) ref path).IsNull)
      {
        column = (LayoutColumn) null;
        return false;
      }
      return ((UtfAnyString) ref path).IsUtf8 ? this.pathMap.TryGetValue(((UtfAnyString) ref path).ToUtf8String(), out column) : this.pathStringMap.TryGetValue(UtfAnyString.op_Implicit(path), out column);
    }

    public bool TryFind(string path, out LayoutColumn column) => this.pathStringMap.TryGetValue(path, out column);

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("Layout:\n");
      stringBuilder.AppendFormat("\tCount: {0}\n", (object) this.topColumns.Length);
      stringBuilder.AppendFormat("\tFixedSize: {0}\n", (object) this.Size);
      foreach (LayoutColumn topColumn in this.topColumns)
      {
        if (topColumn.Type.IsFixed)
        {
          if (topColumn.Type.IsBool)
            stringBuilder.AppendFormat("\t{0}: {1} @ {2}:{3}:{4}\n", (object) topColumn.FullPath, (object) topColumn.Type.Name, (object) topColumn.Offset, (object) topColumn.NullBit, (object) topColumn.BoolBit);
          else
            stringBuilder.AppendFormat("\t{0}: {1} @ {2}\n", (object) topColumn.FullPath, (object) topColumn.Type.Name, (object) topColumn.Offset);
        }
        else
          stringBuilder.AppendFormat("\t{0}: {1}[{3}] @ {2}\n", (object) topColumn.FullPath, (object) topColumn.Type.Name, (object) topColumn.Offset, (object) topColumn.Size);
      }
      return stringBuilder.ToString();
    }

    public struct ColumnView
    {
      private readonly LayoutColumn[] list;
      private int index;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal ColumnView(LayoutColumn[] list)
      {
        this.list = list;
        this.index = -1;
      }

      public Layout.ColumnView GetEnumerator() => new Layout.ColumnView(this.list);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool MoveNext()
      {
        int num = checked (this.index + 1);
        if (num >= this.list.Length)
          return false;
        this.index = num;
        return true;
      }

      public LayoutColumn Current
      {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.list[this.index];
      }

      public LayoutColumn this[int index]
      {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.list[index];
      }

      public int Length
      {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.list.Length;
      }
    }
  }
}
