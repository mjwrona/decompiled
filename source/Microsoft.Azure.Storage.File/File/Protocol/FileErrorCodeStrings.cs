// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.FileErrorCodeStrings
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

namespace Microsoft.Azure.Storage.File.Protocol
{
  public static class FileErrorCodeStrings
  {
    public static readonly string ShareNotFound = nameof (ShareNotFound);
    public static readonly string ShareAlreadyExists = nameof (ShareAlreadyExists);
    public static readonly string ShareDisabled = nameof (ShareDisabled);
    public static readonly string ShareBeingDeleted = nameof (ShareBeingDeleted);
    public static readonly string DeletePending = nameof (DeletePending);
    public static readonly string ParentNotFound = nameof (ParentNotFound);
    public static readonly string InvalidResourceName = nameof (InvalidResourceName);
    public static readonly string ResourceAlreadyExists = nameof (ResourceAlreadyExists);
    public static readonly string ResourceTypeMismatch = nameof (ResourceTypeMismatch);
    public static readonly string SharingViolation = nameof (SharingViolation);
    public static readonly string CannotDeleteFileOrDirectory = nameof (CannotDeleteFileOrDirectory);
    public static readonly string FileLockConflict = nameof (FileLockConflict);
    public static readonly string ReadOnlyAttribute = nameof (ReadOnlyAttribute);
    public static readonly string ClientCacheFlushDelay = nameof (ClientCacheFlushDelay);
    public static readonly string InvalidFileOrDirectoryPathName = nameof (InvalidFileOrDirectoryPathName);
    public static readonly string ConditionHeadersNotSupported = nameof (ConditionHeadersNotSupported);
  }
}
