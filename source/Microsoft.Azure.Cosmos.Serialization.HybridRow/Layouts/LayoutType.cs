// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutType
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  [DebuggerDisplay("{Name}")]
  public abstract class LayoutType : ILayoutType
  {
    internal const int BitsPerByte = 8;
    private static readonly LayoutType[] CodeIndex = new LayoutType[71];
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutInt8 Int8 = new LayoutInt8();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutInt16 Int16 = new LayoutInt16();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutInt32 Int32 = new LayoutInt32();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutInt64 Int64 = new LayoutInt64();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUInt8 UInt8 = new LayoutUInt8();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUInt16 UInt16 = new LayoutUInt16();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUInt32 UInt32 = new LayoutUInt32();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUInt64 UInt64 = new LayoutUInt64();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutVarInt VarInt = new LayoutVarInt();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutVarUInt VarUInt = new LayoutVarUInt();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutFloat32 Float32 = new LayoutFloat32();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutFloat64 Float64 = new LayoutFloat64();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutFloat128 Float128 = new LayoutFloat128();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutDecimal Decimal = new LayoutDecimal();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutNull Null = new LayoutNull();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutBoolean Boolean = new LayoutBoolean(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutBoolean BooleanFalse = new LayoutBoolean(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutDateTime DateTime = new LayoutDateTime();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUnixDateTime UnixDateTime = new LayoutUnixDateTime();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutGuid Guid = new LayoutGuid();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutMongoDbObjectId MongoDbObjectId = new LayoutMongoDbObjectId();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUtf8 Utf8 = new LayoutUtf8();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutBinary Binary = new LayoutBinary();
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutObject Object = new LayoutObject(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutObject ImmutableObject = new LayoutObject(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutArray Array = new LayoutArray(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutArray ImmutableArray = new LayoutArray(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedArray TypedArray = new LayoutTypedArray(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedArray ImmutableTypedArray = new LayoutTypedArray(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedSet TypedSet = new LayoutTypedSet(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedSet ImmutableTypedSet = new LayoutTypedSet(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedMap TypedMap = new LayoutTypedMap(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedMap ImmutableTypedMap = new LayoutTypedMap(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTuple Tuple = new LayoutTuple(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTuple ImmutableTuple = new LayoutTuple(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedTuple TypedTuple = new LayoutTypedTuple(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTypedTuple ImmutableTypedTuple = new LayoutTypedTuple(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTagged Tagged = new LayoutTagged(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTagged ImmutableTagged = new LayoutTagged(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTagged2 Tagged2 = new LayoutTagged2(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutTagged2 ImmutableTagged2 = new LayoutTagged2(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutNullable Nullable = new LayoutNullable(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutNullable ImmutableNullable = new LayoutNullable(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUDT UDT = new LayoutUDT(false);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    public static readonly LayoutUDT ImmutableUDT = new LayoutUDT(true);
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable.")]
    internal static readonly LayoutEndScope EndScope = new LayoutEndScope();
    public readonly LayoutCode LayoutCode;
    public readonly bool Immutable;
    public readonly int Size;

    internal LayoutType(LayoutCode code, bool immutable, int size)
    {
      this.LayoutCode = code;
      this.Immutable = immutable;
      this.Size = size;
      LayoutType.CodeIndex[(int) code] = this;
    }

    public abstract string Name { get; }

    public abstract bool IsFixed { get; }

    public bool AllowVariable => !this.IsFixed;

    public virtual bool IsBool => false;

    public virtual bool IsNull => false;

    public virtual bool IsVarint => false;

    [DebuggerHidden]
    public T TypeAs<T>() where T : ILayoutType => (T) this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static LayoutType FromCode(LayoutCode code) => LayoutType.CodeIndex[(int) code];

    internal static Result PrepareSparseDelete(
      ref RowBuffer b,
      ref RowCursor edit,
      LayoutCode code)
    {
      if (edit.scopeType.IsFixedArity)
        return Result.TypeConstraint;
      if (edit.immutable)
        return Result.InsufficientPermissions;
      return edit.exists && LayoutCodeTraits.Canonicalize(edit.cellType.LayoutCode) != code ? Result.TypeMismatch : Result.Success;
    }

    internal static Result PrepareSparseWrite(
      ref RowBuffer b,
      ref RowCursor edit,
      TypeArgument typeArg,
      UpdateOptions options)
    {
      if (edit.immutable || edit.scopeType.IsUniqueScope && !edit.deferUniqueIndex)
        return Result.InsufficientPermissions;
      if (edit.scopeType.IsFixedArity && !(edit.scopeType is LayoutNullable))
      {
        if (edit.index < edit.scopeTypeArgs.Count && !typeArg.Equals(edit.scopeTypeArgs[edit.index]))
          return Result.TypeConstraint;
      }
      else if (edit.scopeType is LayoutTypedMap)
      {
        if (!(typeArg.Type is LayoutTypedTuple) || !typeArg.TypeArgs.Equals(edit.scopeTypeArgs))
          return Result.TypeConstraint;
      }
      else if (edit.scopeType.IsTypedScope && !typeArg.Equals(edit.scopeTypeArgs[0]))
        return Result.TypeConstraint;
      if (options == UpdateOptions.InsertAt && edit.scopeType.IsFixedArity)
        return Result.TypeConstraint;
      if (options == UpdateOptions.InsertAt && !edit.scopeType.IsFixedArity)
        edit.exists = false;
      if (options == UpdateOptions.Update && !edit.exists)
        return Result.NotFound;
      return options == UpdateOptions.Insert && edit.exists ? Result.Exists : Result.Success;
    }

    internal static Result PrepareSparseRead(ref RowBuffer b, ref RowCursor edit, LayoutCode code)
    {
      if (!edit.exists)
        return Result.NotFound;
      return LayoutCodeTraits.Canonicalize(edit.cellType.LayoutCode) != code ? Result.TypeMismatch : Result.Success;
    }

    internal static Result PrepareSparseMove(
      ref RowBuffer b,
      ref RowCursor destinationScope,
      LayoutScope destinationCode,
      TypeArgument elementType,
      ref RowCursor srcEdit,
      UpdateOptions options,
      out RowCursor dstEdit)
    {
      Contract.Requires(destinationScope.scopeType == destinationCode);
      Contract.Requires(destinationScope.index == 0, "Can only insert into a edit at the root");
      Result result = LayoutType.PrepareSparseDelete(ref b, ref srcEdit, elementType.Type.LayoutCode);
      if (result != Result.Success)
      {
        dstEdit = new RowCursor();
        return result;
      }
      if (!srcEdit.exists)
      {
        dstEdit = new RowCursor();
        return Result.NotFound;
      }
      if (destinationScope.immutable)
      {
        b.DeleteSparse(ref srcEdit);
        dstEdit = new RowCursor();
        return Result.InsufficientPermissions;
      }
      if (!srcEdit.cellTypeArgs.Equals(elementType.TypeArgs))
      {
        b.DeleteSparse(ref srcEdit);
        dstEdit = new RowCursor();
        return Result.TypeConstraint;
      }
      if (options == UpdateOptions.InsertAt)
      {
        b.DeleteSparse(ref srcEdit);
        dstEdit = new RowCursor();
        return Result.TypeConstraint;
      }
      dstEdit = b.PrepareSparseMove(ref destinationScope, ref srcEdit);
      if (options == UpdateOptions.Update && !dstEdit.exists)
      {
        b.DeleteSparse(ref srcEdit);
        dstEdit = new RowCursor();
        return Result.NotFound;
      }
      if (options != UpdateOptions.Insert || !dstEdit.exists)
        return Result.Success;
      b.DeleteSparse(ref srcEdit);
      dstEdit = new RowCursor();
      return Result.Exists;
    }

    internal virtual int CountTypeArgument(TypeArgumentList value) => 1;

    internal virtual int WriteTypeArgument(ref RowBuffer row, int offset, TypeArgumentList value)
    {
      row.WriteSparseTypeCode(offset, this.LayoutCode);
      return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static TypeArgument ReadTypeArgument(
      ref RowBuffer row,
      int offset,
      out int lenInBytes)
    {
      LayoutType type = row.ReadSparseTypeCode(offset);
      int lenInBytes1;
      TypeArgumentList typeArgs = type.ReadTypeArgumentList(ref row, checked (offset + 1), out lenInBytes1);
      lenInBytes = checked (1 + lenInBytes1);
      return new TypeArgument(type, typeArgs);
    }

    internal virtual TypeArgumentList ReadTypeArgumentList(
      ref RowBuffer row,
      int offset,
      out int lenInBytes)
    {
      lenInBytes = 0;
      return TypeArgumentList.Empty;
    }
  }
}
