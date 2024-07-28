// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutMongoDbObjectId
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutMongoDbObjectId : LayoutType<Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId>
  {
    internal LayoutMongoDbObjectId()
      : base(LayoutCode.MongoDbObjectId, 12)
    {
    }

    public override string Name => "mongodbobjectid";

    public override bool IsFixed => true;

    public override Result WriteFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (scope.immutable)
        return Result.InsufficientPermissions;
      b.WriteMongoDbObjectId(checked (scope.start + col.Offset), value);
      b.SetBit(scope.start, col.NullBit);
      return Result.Success;
    }

    public override Result ReadFixed(
      ref RowBuffer b,
      ref RowCursor scope,
      LayoutColumn col,
      out Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId value)
    {
      Contract.Requires(scope.scopeType is LayoutUDT);
      if (!b.ReadBit(scope.start, col.NullBit))
      {
        value = new Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId();
        return Result.NotFound;
      }
      value = b.ReadMongoDbObjectId(checked (scope.start + col.Offset));
      return Result.Success;
    }

    public override Result WriteSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId value,
      UpdateOptions options = UpdateOptions.Upsert)
    {
      Result result = LayoutType.PrepareSparseWrite(ref b, ref edit, this.TypeArg, options);
      if (result != Result.Success)
        return result;
      b.WriteSparseMongoDbObjectId(ref edit, value, options);
      return Result.Success;
    }

    public override Result ReadSparse(
      ref RowBuffer b,
      ref RowCursor edit,
      out Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId value)
    {
      Result result = LayoutType.PrepareSparseRead(ref b, ref edit, this.LayoutCode);
      if (result != Result.Success)
      {
        value = new Microsoft.Azure.Cosmos.Serialization.HybridRow.MongoDbObjectId();
        return result;
      }
      value = b.ReadSparseMongoDbObjectId(ref edit);
      return Result.Success;
    }
  }
}
