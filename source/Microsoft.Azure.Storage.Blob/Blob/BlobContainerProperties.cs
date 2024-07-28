// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobContainerProperties
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobContainerProperties
  {
    public string ETag { get; internal set; }

    public bool? HasImmutabilityPolicy { get; internal set; }

    public bool? HasLegalHold { get; internal set; }

    public DateTimeOffset? LastModified { get; internal set; }

    public LeaseStatus LeaseStatus { get; internal set; }

    public LeaseState LeaseState { get; internal set; }

    public LeaseDuration LeaseDuration { get; internal set; }

    public BlobContainerPublicAccessType? PublicAccess { get; internal set; }

    public BlobContainerEncryptionScopeOptions EncryptionScopeOptions { get; internal set; }
  }
}
