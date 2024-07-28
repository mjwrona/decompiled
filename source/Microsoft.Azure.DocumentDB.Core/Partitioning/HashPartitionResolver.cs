// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Partitioning.HashPartitionResolver
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.Azure.Documents.Partitioning
{
  [Obsolete("Support for IPartitionResolver based classes is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput.")]
  public class HashPartitionResolver : IPartitionResolver, IDisposable
  {
    private ConsistentHashRing consistentHashRing;
    private const int defaultNumberOfVirtualNodesPerCollection = 128;
    private bool isDisposed;

    [JsonIgnore]
    public IHashGenerator HashGenerator { get; private set; }

    public string PartitionKeyPropertyName { get; private set; }

    public IEnumerable<string> CollectionLinks { get; private set; }

    public Func<object, string> PartitionKeyExtractor { get; private set; }

    public int NumberOfVirtualNodesPerCollection { get; private set; }

    public HashPartitionResolver(
      string partitionKeyPropertyName,
      IEnumerable<string> collectionLinks,
      int numberOfVirtualNodesPerCollection = 128,
      IHashGenerator hashGenerator = null)
    {
      if (string.IsNullOrEmpty(partitionKeyPropertyName))
        throw new ArgumentException(nameof (partitionKeyPropertyName));
      if (numberOfVirtualNodesPerCollection <= 0)
        throw new ArgumentException(nameof (numberOfVirtualNodesPerCollection));
      this.PartitionKeyPropertyName = partitionKeyPropertyName;
      this.Initialize(collectionLinks, hashGenerator, numberOfVirtualNodesPerCollection);
    }

    public HashPartitionResolver(
      Func<object, string> partitionKeyExtractor,
      IEnumerable<string> collectionLinks,
      int numberOfVirtualNodesPerCollection = 128,
      IHashGenerator hashGenerator = null)
    {
      if (partitionKeyExtractor == null)
        throw new ArgumentNullException(nameof (partitionKeyExtractor));
      if (numberOfVirtualNodesPerCollection <= 0)
        throw new ArgumentException(nameof (numberOfVirtualNodesPerCollection));
      this.PartitionKeyExtractor = partitionKeyExtractor;
      this.Initialize(collectionLinks, hashGenerator, numberOfVirtualNodesPerCollection);
    }

    private void Initialize(
      IEnumerable<string> collectionLinks,
      IHashGenerator hashGenerator,
      int numberOfVirtualNodesPerCollection)
    {
      this.CollectionLinks = collectionLinks != null ? collectionLinks : throw new ArgumentNullException(nameof (collectionLinks));
      this.HashGenerator = hashGenerator ?? (IHashGenerator) new HashPartitionResolver.Md5HashGenerator();
      this.NumberOfVirtualNodesPerCollection = numberOfVirtualNodesPerCollection;
      this.consistentHashRing = new ConsistentHashRing(this.HashGenerator, this.CollectionLinks, this.CollectionLinks.Count<string>() * this.NumberOfVirtualNodesPerCollection);
    }

    public virtual object GetPartitionKey(object document)
    {
      if (this.PartitionKeyPropertyName != null)
        return PartitionResolverUtils.ExtractPartitionKeyFromDocument(document, this.PartitionKeyPropertyName);
      if (this.PartitionKeyExtractor != null)
        return (object) this.PartitionKeyExtractor(document);
      throw new InvalidOperationException(ClientResources.PartitionKeyExtractError);
    }

    public virtual string ResolveForCreate(object partitionKey) => partitionKey != null ? this.consistentHashRing.GetNode(this.ProcessPartitionKey(partitionKey)) : throw new ArgumentNullException(nameof (partitionKey));

    public virtual IEnumerable<string> ResolveForRead(object partitionKey)
    {
      if (partitionKey == null)
        return this.CollectionLinks;
      return (IEnumerable<string>) new List<string>()
      {
        this.consistentHashRing.GetNode(this.ProcessPartitionKey(partitionKey))
      };
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (this.consistentHashRing.GetHashGenerator() is HashPartitionResolver.Md5HashGenerator hashGenerator)
        hashGenerator.Dispose();
      this.isDisposed = true;
    }

    internal string ProcessPartitionKey(object partitionKey)
    {
      if (partitionKey == null)
        return (string) null;
      return partitionKey is string ? (string) partitionKey : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.UnsupportedPartitionKey, (object) partitionKey.GetType()));
    }

    private class Md5HashGenerator : IHashGenerator, IDisposable
    {
      private HashAlgorithm hashAlgorithm;

      [SuppressMessage("Microsoft.Security.Cryptography", "CA5350:SHA-2 (SHA256, SHA384, SHA512) must be used", Justification = "Secutiry is not a concern here since we need a good hash function only.")]
      public Md5HashGenerator() => this.hashAlgorithm = (HashAlgorithm) MD5.Create();

      public byte[] ComputeHash(byte[] key)
      {
        lock (this)
          return this.hashAlgorithm.ComputeHash(key);
      }

      public void Dispose() => this.hashAlgorithm.Dispose();
    }
  }
}
