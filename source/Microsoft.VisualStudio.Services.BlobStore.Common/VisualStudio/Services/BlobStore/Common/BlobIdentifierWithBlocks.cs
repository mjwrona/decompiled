// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [JsonObject(MemberSerialization.OptIn)]
  public sealed class BlobIdentifierWithBlocks : IEquatable<BlobIdentifierWithBlocks>
  {
    private static readonly char[] SplitCharacters = new char[1]
    {
      ','
    };

    [JsonProperty(PropertyName = "identifierValue")]
    [JsonConverter(typeof (ByteArrayAsNumberArrayJsonConvertor))]
    private byte[] identifierValue
    {
      get => this.BlobId?.Bytes;
      set => this.BlobId = new BlobIdentifier(value);
    }

    [JsonProperty(PropertyName = "BlockHashes")]
    public IList<BlobBlockHash> BlockHashes { get; set; }

    public BlobIdentifier BlobId { get; private set; }

    public BlobIdentifierWithBlocks()
    {
    }

    public BlobIdentifierWithBlocks(
      BlobIdentifier blobId,
      IEnumerable<BlobBlockHash> blockIdentifiers)
    {
      this.BlockHashes = (IList<BlobBlockHash>) blockIdentifiers.ToList<BlobBlockHash>();
      this.BlobId = blobId;
      this.Validate();
    }

    public static BlobIdentifierWithBlocks Deserialize(string serialized)
    {
      string[] strArray = serialized.Split(':');
      return new BlobIdentifierWithBlocks(BlobIdentifier.Deserialize(strArray[0]), (IEnumerable<BlobBlockHash>) ((IEnumerable<string>) strArray[1].Split(BlobIdentifierWithBlocks.SplitCharacters, StringSplitOptions.RemoveEmptyEntries)).Select<string, BlobBlockHash>((Func<string, BlobBlockHash>) (idString => new BlobBlockHash(idString))).ToList<BlobBlockHash>());
    }

    public string Serialize() => string.Format("{0}:{1}", (object) this.BlobId.ValueString, (object) string.Join(",", this.BlockHashes.Select<BlobBlockHash, string>((Func<BlobBlockHash, string>) (id => id.HashString))));

    public bool BlocksContainThisId(IEnumerable<BlobBlockHash> blocks) => this.BlockHashes.All<BlobBlockHash>((Func<BlobBlockHash, bool>) (blockHash => blocks.Any<BlobBlockHash>((Func<BlobBlockHash, bool>) (block => block == blockHash))));

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => this.Validate();

    private void Validate()
    {
      if (this.BlobId == (BlobIdentifier) null)
        throw new ArgumentNullException("BlobId");
      BlobIdentifier other = this.BlockHashes != null ? this.BlobId.GetBlobHasher().CalculateBlobIdentifierFromBlobBlockHashes((IEnumerable<BlobBlockHash>) this.BlockHashes) : throw new ArgumentNullException("BlockHashes");
      if (!this.BlobId.Equals(other))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Computed BlobIdentifier '{0}' does not match given one '{1}'.", (object) other, (object) this.BlobId));
    }

    public override bool Equals(object obj) => this.Equals(obj as BlobIdentifierWithBlocks);

    public bool Equals(BlobIdentifierWithBlocks other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && this.BlobId.Equals(other.BlobId);
    }

    public override int GetHashCode() => Microsoft.VisualStudio.Services.Content.Common.EqualityHelper.GetCombinedHashCode((object) this.BlobId, (object) this.GetType());

    public override string ToString() => "BlobWithBlocks:" + this.Serialize();

    public int CompareTo(object obj) => (object) (obj as BlobIdentifierWithBlocks) != null ? this.CompareTo((BlobIdentifierWithBlocks) obj) : throw new ArgumentException("Object is not a BlobIdentifierWithBlocks");

    public int CompareTo(BlobIdentifierWithBlocks other) => !(other == (BlobIdentifierWithBlocks) null) ? string.Compare(this.Serialize(), other.Serialize(), StringComparison.InvariantCultureIgnoreCase) : 1;

    public static bool operator ==(BlobIdentifierWithBlocks x, BlobIdentifierWithBlocks y) => (object) x == null ? (object) y == null : x.Equals(y);

    public static bool operator !=(BlobIdentifierWithBlocks x, BlobIdentifierWithBlocks y) => !(x == y);
  }
}
