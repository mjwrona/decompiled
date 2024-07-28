// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileProperties
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;

namespace Microsoft.Azure.Storage.File
{
  public sealed class FileProperties
  {
    internal string filePermissionKey;
    internal string filePermissionKeyToSet;
    internal CloudFileNtfsAttributes? ntfsAttributes;
    internal CloudFileNtfsAttributes? ntfsAttributesToSet;
    internal DateTimeOffset? creationTime;
    internal DateTimeOffset? creationTimeToSet;
    internal DateTimeOffset? lastWriteTime;
    internal DateTimeOffset? lastWriteTimeToSet;

    public FileProperties() => this.Length = -1L;

    public FileProperties(FileProperties other)
    {
      CommonUtility.AssertNotNull(nameof (other), (object) other);
      this.ContentType = other.ContentType;
      this.ContentDisposition = other.ContentDisposition;
      this.ContentEncoding = other.ContentEncoding;
      this.ContentLanguage = other.ContentLanguage;
      this.CacheControl = other.CacheControl;
      this.ContentChecksum.MD5 = other.ContentChecksum.MD5;
      this.ContentChecksum.CRC64 = other.ContentChecksum.CRC64;
      this.Length = other.Length;
      this.ETag = other.ETag;
      this.LastModified = other.LastModified;
      this.IsServerEncrypted = other.IsServerEncrypted;
      this.filePermissionKey = other.filePermissionKey;
      this.ntfsAttributes = other.ntfsAttributes;
      this.creationTime = other.creationTime;
      this.lastWriteTime = other.lastWriteTime;
      this.filePermissionKeyToSet = other.filePermissionKeyToSet;
      this.ntfsAttributesToSet = other.ntfsAttributesToSet;
      this.creationTimeToSet = other.creationTimeToSet;
      this.lastWriteTimeToSet = other.lastWriteTimeToSet;
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

    public DateTimeOffset? LastModified { get; internal set; }

    public bool IsServerEncrypted { get; internal set; }

    public string FilePermissionKey
    {
      get => this.filePermissionKeyToSet ?? this.filePermissionKey;
      set => this.filePermissionKeyToSet = value;
    }

    public CloudFileNtfsAttributes? NtfsAttributes
    {
      get => this.ntfsAttributesToSet ?? this.ntfsAttributes;
      set => this.ntfsAttributesToSet = value;
    }

    public DateTimeOffset? CreationTime
    {
      get => this.creationTimeToSet ?? this.creationTime;
      set => this.creationTimeToSet = value;
    }

    public DateTimeOffset? LastWriteTime
    {
      get => this.lastWriteTimeToSet ?? this.lastWriteTime;
      set => this.lastWriteTimeToSet = value;
    }

    public DateTimeOffset? ChangeTime { get; internal set; }

    public string FileId { get; internal set; }

    public string ParentId { get; internal set; }
  }
}
