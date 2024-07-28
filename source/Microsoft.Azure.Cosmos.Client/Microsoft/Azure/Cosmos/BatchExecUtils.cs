// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchExecUtils
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal static class BatchExecUtils
  {
    private const int BufferSize = 81920;

    public static async Task<Memory<byte>> StreamToMemoryAsync(
      Stream stream,
      CancellationToken cancellationToken)
    {
      byte[] bytes;
      int sum1;
      if (stream.CanSeek)
      {
        ArraySegment<byte> buffer;
        if (stream is MemoryStream memoryStream && memoryStream.GetType() == typeof (MemoryStream) && memoryStream.TryGetBuffer(out buffer))
          return (Memory<byte>) buffer;
        bytes = new byte[stream.Length];
        sum1 = 0;
        while (true)
        {
          int num;
          if ((num = await stream.ReadAsync(bytes, sum1, bytes.Length - sum1, cancellationToken)) > 0)
            sum1 += num;
          else
            break;
        }
        return (Memory<byte>) bytes;
      }
      sum1 = 81920;
      bytes = new byte[sum1];
      using (MemoryStream memoryStream = new MemoryStream(sum1))
      {
        int sum2 = 0;
        while (true)
        {
          int count;
          if ((count = await stream.ReadAsync(bytes, 0, sum1, cancellationToken)) > 0)
          {
            sum2 += count;
            memoryStream.Write(bytes, 0, count);
          }
          else
            break;
        }
        return new Memory<byte>(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
      }
    }

    public static void EnsureValid(
      IReadOnlyList<ItemBatchOperation> operations,
      RequestOptions batchOptions)
    {
      string message = BatchExecUtils.IsValid(operations, batchOptions);
      if (message != null)
        throw new ArgumentException(message);
    }

    internal static string IsValid(
      IReadOnlyList<ItemBatchOperation> operations,
      RequestOptions batchOptions)
    {
      string str1 = (string) null;
      if (operations.Count == 0)
        str1 = ClientResources.BatchNoOperations;
      if (str1 == null && batchOptions != null && (batchOptions.IfMatchEtag != null || batchOptions.IfNoneMatchEtag != null))
        str1 = ClientResources.BatchRequestOptionNotSupported;
      if (str1 == null)
      {
        foreach (ItemBatchOperation operation in (IEnumerable<ItemBatchOperation>) operations)
        {
          object obj1;
          object obj2;
          object obj3;
          if (operation.RequestOptions != null && operation.RequestOptions.Properties != null && operation.RequestOptions.Properties.TryGetValue("x-ms-effective-partition-key", out obj1) | operation.RequestOptions.Properties.TryGetValue("x-ms-effective-partition-key-string", out obj2) | operation.RequestOptions.Properties.TryGetValue("x-ms-documentdb-partitionkey", out obj3))
          {
            byte[] numArray = obj1 as byte[];
            string str2 = obj3 as string;
            if (numArray == null && str2 == null || !(obj2 is string))
              str1 = string.Format(ClientResources.EpkPropertiesPairingExpected, (object) "x-ms-effective-partition-key", (object) "x-ms-effective-partition-key-string");
            if (operation.PartitionKey.HasValue && !operation.RequestOptions.IsEffectivePartitionKeyRouting)
              str1 = ClientResources.PKAndEpkSetTogether;
          }
        }
      }
      return str1;
    }

    public static string GetPartitionKeyRangeId(
      PartitionKey partitionKey,
      PartitionKeyDefinition partitionKeyDefinition,
      CollectionRoutingMap collectionRoutingMap)
    {
      string partitionKeyString = partitionKey.InternalKey.GetEffectivePartitionKeyString(partitionKeyDefinition);
      return collectionRoutingMap.GetRangeByEffectivePartitionKey(partitionKeyString).Id;
    }
  }
}
