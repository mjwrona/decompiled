// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutColumn
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  [DebuggerDisplay("{FullPath + \": \" + Type.Name + TypeArgs.ToString()}")]
  public sealed class LayoutColumn
  {
    private readonly int size;
    private readonly Utf8String path;
    private readonly Utf8String fullPath;
    private readonly LayoutType type;
    private readonly TypeArgument typeArg;
    private readonly StorageKind storage;
    private readonly LayoutColumn parent;
    private readonly TypeArgumentList typeArgs;
    private readonly LayoutBit nullBit;
    private readonly LayoutBit boolBit;
    private int index;
    private int offset;

    internal LayoutColumn(
      string path,
      LayoutType type,
      TypeArgumentList typeArgs,
      StorageKind storage,
      LayoutColumn parent,
      int index,
      int offset,
      LayoutBit nullBit,
      LayoutBit boolBit,
      int length = 0)
    {
      this.path = Utf8String.TranscodeUtf16(path);
      this.fullPath = Utf8String.TranscodeUtf16(LayoutColumn.GetFullPath(parent, path));
      this.type = type;
      this.typeArgs = typeArgs;
      this.typeArg = new TypeArgument(type, typeArgs);
      this.storage = storage;
      this.parent = parent;
      this.index = index;
      this.offset = offset;
      this.nullBit = nullBit;
      this.boolBit = boolBit;
      this.size = this.typeArg.Type.IsFixed ? type.Size : length;
    }

    public Utf8String Path => this.path;

    public Utf8String FullPath => this.fullPath;

    public LayoutType Type => this.type;

    public StorageKind Storage => this.storage;

    public LayoutColumn Parent => this.parent;

    public TypeArgument TypeArg => this.typeArg;

    public TypeArgumentList TypeArgs => this.typeArgs;

    public int Index => this.index;

    public int Offset
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.offset;
    }

    public LayoutBit NullBit
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.nullBit;
    }

    public LayoutBit BoolBit
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.boolBit;
    }

    public int Size
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.size;
    }

    [DebuggerHidden]
    public T TypeAs<T>() where T : ILayoutType => this.type.TypeAs<T>();

    internal void SetIndex(int index) => this.index = index;

    internal void SetOffset(int offset) => this.offset = offset;

    private static string GetFullPath(LayoutColumn parent, string path)
    {
      if (parent == null)
        return path;
      LayoutCode layoutCode = LayoutCodeTraits.ClearImmutableBit(parent.type.LayoutCode);
      if ((uint) layoutCode <= 42U)
      {
        switch (layoutCode)
        {
          case LayoutCode.ObjectScope:
            break;
          case LayoutCode.ArrayScope:
          case LayoutCode.TypedArrayScope:
          case LayoutCode.TypedMapScope:
            goto label_6;
          default:
            goto label_7;
        }
      }
      else if (layoutCode != LayoutCode.TypedSetScope)
      {
        if (layoutCode != LayoutCode.Schema)
          goto label_7;
      }
      else
        goto label_6;
      return parent.FullPath.ToString() + "." + path;
label_6:
      return parent.FullPath.ToString() + "[]" + path;
label_7:
      Contract.Fail(string.Format("Parent scope type not supported: {0}", (object) parent.type.LayoutCode));
      return (string) null;
    }
  }
}
