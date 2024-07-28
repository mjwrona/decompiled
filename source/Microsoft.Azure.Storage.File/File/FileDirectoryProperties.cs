// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileDirectoryProperties
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;

namespace Microsoft.Azure.Storage.File
{
  public sealed class FileDirectoryProperties
  {
    internal string filePermissionKey;
    internal string filePermissionKeyToSet;
    internal CloudFileNtfsAttributes? ntfsAttributes;
    internal CloudFileNtfsAttributes? ntfsAttributesToSet;
    internal DateTimeOffset? creationTime;
    internal DateTimeOffset? creationTimeToSet;
    internal DateTimeOffset? lastWriteTime;
    internal DateTimeOffset? lastWriteTimeToSet;

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

    public string DirectoryId { get; internal set; }

    public string ParentId { get; internal set; }
  }
}
