// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.AccessCondition
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;

namespace Microsoft.Azure.Storage
{
  public sealed class AccessCondition
  {
    private DateTimeOffset? ifModifiedSinceDateTime;
    private DateTimeOffset? ifNotModifiedSinceDateTime;

    public string IfMatchETag { get; set; }

    public string IfNoneMatchETag { get; set; }

    public string IfMatchContentCrc { get; set; }

    public string IfNoneMatchContentCrc { get; set; }

    public DateTimeOffset? IfModifiedSinceTime
    {
      get => this.ifModifiedSinceDateTime;
      set => this.ifModifiedSinceDateTime = value.HasValue ? new DateTimeOffset?(value.Value.ToUniversalTime()) : value;
    }

    public DateTimeOffset? IfNotModifiedSinceTime
    {
      get => this.ifNotModifiedSinceDateTime;
      set => this.ifNotModifiedSinceDateTime = value.HasValue ? new DateTimeOffset?(value.Value.ToUniversalTime()) : value;
    }

    public long? IfMaxSizeLessThanOrEqual { get; set; }

    public long? IfAppendPositionEqual { get; set; }

    public long? IfSequenceNumberLessThanOrEqual { get; set; }

    public long? IfSequenceNumberLessThan { get; set; }

    public long? IfSequenceNumberEqual { get; set; }

    public string LeaseId { get; set; }

    internal bool IsConditional
    {
      get
      {
        if (string.IsNullOrEmpty(this.IfMatchETag) && string.IsNullOrEmpty(this.IfNoneMatchETag))
        {
          DateTimeOffset? modifiedSinceTime = this.IfModifiedSinceTime;
          if (!modifiedSinceTime.HasValue)
          {
            modifiedSinceTime = this.IfNotModifiedSinceTime;
            return modifiedSinceTime.HasValue;
          }
        }
        return true;
      }
    }

    internal bool IsIfNotExists => string.Equals("*", this.IfNoneMatchETag, StringComparison.Ordinal);

    internal AccessCondition RemoveIsIfNotExistsCondition()
    {
      if (this.IsIfNotExists)
        this.IfNoneMatchETag = (string) null;
      return this;
    }

    public AccessCondition Clone() => (AccessCondition) this.MemberwiseClone();

    public static AccessCondition GenerateEmptyCondition() => new AccessCondition();

    public static AccessCondition GenerateIfNotExistsCondition() => new AccessCondition()
    {
      IfNoneMatchETag = "*"
    };

    public static AccessCondition GenerateIfExistsCondition() => new AccessCondition()
    {
      IfMatchETag = "*"
    };

    public static AccessCondition GenerateIfMatchCondition(string etag) => new AccessCondition()
    {
      IfMatchETag = etag
    };

    public static AccessCondition GenerateIfModifiedSinceCondition(DateTimeOffset modifiedTime) => new AccessCondition()
    {
      IfModifiedSinceTime = new DateTimeOffset?(modifiedTime)
    };

    public static AccessCondition GenerateIfNoneMatchCondition(string etag) => new AccessCondition()
    {
      IfNoneMatchETag = etag
    };

    public static AccessCondition GenerateIfNotModifiedSinceCondition(DateTimeOffset modifiedTime) => new AccessCondition()
    {
      IfNotModifiedSinceTime = new DateTimeOffset?(modifiedTime)
    };

    public static AccessCondition GenerateIfMaxSizeLessThanOrEqualCondition(long maxSize) => new AccessCondition()
    {
      IfMaxSizeLessThanOrEqual = new long?(maxSize)
    };

    public static AccessCondition GenerateIfAppendPositionEqualCondition(long appendPosition) => new AccessCondition()
    {
      IfAppendPositionEqual = new long?(appendPosition)
    };

    public static AccessCondition GenerateIfSequenceNumberLessThanOrEqualCondition(
      long sequenceNumber)
    {
      return new AccessCondition()
      {
        IfSequenceNumberLessThanOrEqual = new long?(sequenceNumber)
      };
    }

    public static AccessCondition GenerateIfSequenceNumberLessThanCondition(long sequenceNumber) => new AccessCondition()
    {
      IfSequenceNumberLessThan = new long?(sequenceNumber)
    };

    public static AccessCondition GenerateIfSequenceNumberEqualCondition(long sequenceNumber) => new AccessCondition()
    {
      IfSequenceNumberEqual = new long?(sequenceNumber)
    };

    public static AccessCondition GenerateLeaseCondition(string leaseId) => new AccessCondition()
    {
      LeaseId = leaseId
    };

    internal static AccessCondition CloneConditionWithETag(
      AccessCondition accessCondition,
      string etag)
    {
      return new AccessCondition()
      {
        IfMatchETag = etag,
        LeaseId = accessCondition?.LeaseId
      };
    }
  }
}
