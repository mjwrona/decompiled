// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobProperties
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobProperties
  {
    public BlobProperties() => this.Length = -1L;

    public BlobProperties(BlobProperties other)
    {
      CommonUtility.AssertNotNull(nameof (other), (object) other);
      this.BlobType = other.BlobType;
      this.ContentType = other.ContentType;
      this.ContentDisposition = other.ContentDisposition;
      this.ContentEncoding = other.ContentEncoding;
      this.ContentLanguage = other.ContentLanguage;
      this.CacheControl = other.CacheControl;
      this.ContentChecksum.MD5 = other.ContentChecksum.MD5;
      this.ContentChecksum.CRC64 = other.ContentChecksum.CRC64;
      this.Length = other.Length;
      this.ETag = other.ETag;
      this.Created = other.Created;
      this.LastModified = other.LastModified;
      this.PageBlobSequenceNumber = other.PageBlobSequenceNumber;
      this.AppendBlobCommittedBlockCount = other.AppendBlobCommittedBlockCount;
      this.IsServerEncrypted = other.IsServerEncrypted;
      this.EncryptionScope = other.EncryptionScope;
      this.IsIncrementalCopy = other.IsIncrementalCopy;
      this.PremiumPageBlobTier = other.PremiumPageBlobTier;
      this.StandardBlobTier = other.StandardBlobTier;
      this.RehydrationStatus = other.RehydrationStatus;
      this.BlobTierLastModifiedTime = other.BlobTierLastModifiedTime;
      this.DeletedTime = other.DeletedTime;
      this.RemainingDaysBeforePermanentDelete = other.RemainingDaysBeforePermanentDelete;
    }

    public string CacheControl { get; set; }

    public string ContentDisposition { get; set; }

    public string ContentEncoding { get; set; }

    public string ContentLanguage { get; set; }

    public long Length { get; internal set; }

    public string ContentMD5
    {
      get => this.ContentChecksum.MD5;
      set => this.ContentChecksum.MD5 = value;
    }

    internal Checksum ContentChecksum { get; set; } = new Checksum();

    public string ContentType { get; set; }

    public string ETag { get; internal set; }

    public DateTimeOffset? Created { get; internal set; }

    public DateTimeOffset? LastModified { get; internal set; }

    public BlobType BlobType { get; internal set; }

    public LeaseStatus LeaseStatus { get; internal set; }

    public LeaseState LeaseState { get; internal set; }

    public LeaseDuration LeaseDuration { get; internal set; }

    public long? PageBlobSequenceNumber { get; internal set; }

    public int? AppendBlobCommittedBlockCount { get; internal set; }

    public bool IsServerEncrypted { get; internal set; }

    public string EncryptionScope { get; internal set; }

    public bool IsIncrementalCopy { get; internal set; }

    public Microsoft.Azure.Storage.Blob.StandardBlobTier? StandardBlobTier { get; internal set; }

    public Microsoft.Azure.Storage.Blob.RehydrationStatus? RehydrationStatus { get; internal set; }

    public Microsoft.Azure.Storage.Blob.PremiumPageBlobTier? PremiumPageBlobTier { get; internal set; }

    public bool? BlobTierInferred { get; internal set; }

    public DateTimeOffset? BlobTierLastModifiedTime { get; internal set; }

    public DateTimeOffset? DeletedTime { get; internal set; }

    public int? RemainingDaysBeforePermanentDelete { get; internal set; }
  }
}
