// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Partitioning.ConsistentHashRing
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Documents.Partitioning
{
  [Obsolete("Support for classes used with IPartitionResolver is now obsolete.")]
  internal sealed class ConsistentHashRing
  {
    private IHashGenerator hashGenerator;
    private ConsistentHashRing.Partition[] partitions;
    private int totalPartitions;

    public ConsistentHashRing(
      IHashGenerator hashGenerator,
      IEnumerable<string> nodes,
      int totalPartitions)
    {
      if (hashGenerator == null)
        throw new ArgumentNullException("hash");
      int num = nodes != null ? nodes.Count<string>() : throw new ArgumentNullException(nameof (nodes));
      if (totalPartitions < num)
        throw new ArgumentException("The total number of partitions must be at least the number of nodes");
      this.hashGenerator = hashGenerator;
      this.totalPartitions = totalPartitions;
      this.partitions = ConsistentHashRing.ConstructPartitions(this.hashGenerator, nodes, this.totalPartitions);
    }

    public string GetNode(string key) => this.GetNode(this.ComputePartition(key), 0);

    public IHashGenerator GetHashGenerator() => this.hashGenerator;

    private static ConsistentHashRing.Partition[] ConstructPartitions(
      IHashGenerator hashGenerator,
      IEnumerable<string> nodes,
      int totalPartitions)
    {
      int num1 = nodes.Count<string>();
      ConsistentHashRing.Partition[] array = new ConsistentHashRing.Partition[totalPartitions];
      int num2 = totalPartitions / num1;
      int num3 = totalPartitions - num2 * num1;
      int num4 = 0;
      foreach (string node in nodes)
      {
        byte[] hash = hashGenerator.ComputeHash(BitConverter.GetBytes(node.GetHashCode()));
        for (int index = 0; index < num2 + (num3 > 0 ? 1 : 0); ++index)
        {
          array[num4++] = new ConsistentHashRing.Partition(hash, node);
          hash = hashGenerator.ComputeHash(hash);
        }
        --num3;
      }
      Array.Sort<ConsistentHashRing.Partition>(array);
      return array;
    }

    private int ComputePartition(string key) => key != null ? this.ComputePartition(Encoding.UTF8.GetBytes(key)) : throw new ArgumentNullException(nameof (key));

    private int ComputePartition(byte[] key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      int partition = ConsistentHashRing.LowerBound(this.partitions, this.hashGenerator.ComputeHash(key));
      if (partition == this.partitions.Length)
        partition = 0;
      return partition;
    }

    private string GetNode(int partition, int replica) => this.partitions[ConsistentHashRing.SkipRelicas(this.partitions, partition, replica)].Node;

    private static int LowerBound(ConsistentHashRing.Partition[] partitions, byte[] hashValue)
    {
      int num1 = 0;
      int num2 = partitions.Length - num1;
      while (num2 > 0)
      {
        int num3 = num2 / 2;
        int index = num1 + num3;
        if (partitions[index].CompareTo(hashValue) < 0)
        {
          int num4;
          num1 = num4 = index + 1;
          num2 -= num3 + 1;
        }
        else
          num2 = num3;
      }
      return num1;
    }

    private static int SkipRelicas(
      ConsistentHashRing.Partition[] partitions,
      int partition,
      int replica)
    {
      string[] source = new string[replica];
      int index = partition;
      for (; replica > 0; --replica)
      {
        source[source.Length - replica] = partitions[index].Node;
        do
        {
          index = (index + 1) % partitions.Length;
          if (index == partition)
            throw new InvalidOperationException("Not enough nodes for the requested replica");
        }
        while (((IEnumerable<string>) source).Take<string>(source.Length - replica + 1).Contains<string>(partitions[index].Node));
      }
      return index;
    }

    private class Partition : IComparable<ConsistentHashRing.Partition>
    {
      public Partition(byte[] hashValue, string node)
      {
        this.HashValue = hashValue;
        this.Node = node;
      }

      public byte[] HashValue { get; private set; }

      public string Node { get; private set; }

      [SuppressMessage("Microsoft.Usage", "#pw26506")]
      public int CompareTo(ConsistentHashRing.Partition other) => this.CompareTo(other.HashValue);

      public int CompareTo(byte[] otherHashValue) => ConsistentHashRing.Partition.CompareHashValues(this.HashValue, otherHashValue);

      public static int CompareHashValues(byte[] hash1, byte[] hash2)
      {
        if (hash1.Length != hash2.Length)
          throw new ArgumentException("Length does not match", nameof (hash2));
        for (int index = 0; index < hash1.Length; ++index)
        {
          if ((int) hash1[index] < (int) hash2[index])
            return -1;
          if ((int) hash1[index] > (int) hash2[index])
            return 1;
        }
        return 0;
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder(32);
        for (int index = 0; index < this.HashValue.Length; ++index)
          stringBuilder.AppendFormat("{0:x2}", (object) this.HashValue[index]);
        return stringBuilder.ToString();
      }
    }
  }
}
