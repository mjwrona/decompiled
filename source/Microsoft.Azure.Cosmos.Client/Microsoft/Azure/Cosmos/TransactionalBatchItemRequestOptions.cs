// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.TransactionalBatchItemRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;

namespace Microsoft.Azure.Cosmos
{
  public class TransactionalBatchItemRequestOptions : RequestOptions
  {
    public Microsoft.Azure.Cosmos.IndexingDirective? IndexingDirective { get; set; }

    public bool? EnableContentResponseOnWrite { get; set; }

    internal static TransactionalBatchItemRequestOptions FromItemRequestOptions(
      ItemRequestOptions itemRequestOptions)
    {
      if (itemRequestOptions == null)
        return (TransactionalBatchItemRequestOptions) null;
      TransactionalBatchItemRequestOptions itemRequestOptions1 = new TransactionalBatchItemRequestOptions();
      itemRequestOptions1.IndexingDirective = itemRequestOptions.IndexingDirective;
      itemRequestOptions1.IfMatchEtag = itemRequestOptions.IfMatchEtag;
      itemRequestOptions1.IfNoneMatchEtag = itemRequestOptions.IfNoneMatchEtag;
      itemRequestOptions1.Properties = itemRequestOptions.Properties;
      itemRequestOptions1.EnableContentResponseOnWrite = itemRequestOptions.EnableContentResponseOnWrite;
      itemRequestOptions1.IsEffectivePartitionKeyRouting = itemRequestOptions.IsEffectivePartitionKeyRouting;
      return itemRequestOptions1;
    }

    internal virtual Result WriteRequestProperties(ref RowWriter writer, bool pkWritten)
    {
      if (this.Properties == null)
        return Result.Success;
      object obj1;
      if (this.Properties.TryGetValue("x-ms-binary-id", out obj1) && obj1 is byte[] numArray1)
      {
        Result result = writer.WriteBinary(UtfAnyString.op_Implicit("binaryId"), numArray1);
        if (result != Result.Success)
          return result;
      }
      object obj2;
      if (this.Properties.TryGetValue("x-ms-effective-partition-key", out obj2) && obj2 is byte[] numArray2)
      {
        Result result = writer.WriteBinary(UtfAnyString.op_Implicit("effectivePartitionKey"), numArray2);
        if (result != Result.Success)
          return result;
      }
      object obj3;
      if (!pkWritten && this.Properties.TryGetValue("x-ms-documentdb-partitionkey", out obj3) && obj3 is string str)
      {
        Result result = writer.WriteString(UtfAnyString.op_Implicit("partitionKey"), str);
        if (result != Result.Success)
          return result;
      }
      object obj4;
      int result1;
      if (this.Properties.TryGetValue("x-ms-time-to-live-in-seconds", out obj4) && obj4 is string s && int.TryParse(s, out result1))
      {
        Result result2 = writer.WriteInt32(UtfAnyString.op_Implicit("timeToLiveInSeconds"), result1);
        if (result2 != Result.Success)
          return result2;
      }
      return Result.Success;
    }

    internal virtual int GetRequestPropertiesSerializationLength()
    {
      if (this.Properties == null)
        return 0;
      int serializationLength = 0;
      object obj1;
      if (this.Properties.TryGetValue("x-ms-binary-id", out obj1) && obj1 is byte[] numArray1)
        serializationLength += numArray1.Length;
      object obj2;
      if (this.Properties.TryGetValue("x-ms-effective-partition-key", out obj2) && obj2 is byte[] numArray2)
        serializationLength += numArray2.Length;
      return serializationLength;
    }
  }
}
