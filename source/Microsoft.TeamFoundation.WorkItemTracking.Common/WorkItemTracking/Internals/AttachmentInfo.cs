// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.AttachmentInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AttachmentInfo : FileLinkInfo
  {
    public FileInfo FileInfo;
    public bool IsUploaded;
    public bool IsLocalCopy;

    public long Length { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastWriteDate { get; set; }

    public AttachmentInfo()
    {
      this.IsUploaded = false;
      this.FileInfo = (FileInfo) null;
      this.ExtId = 0;
      this.Length = 0L;
      this.CreationDate = DateTime.MinValue;
      this.LastWriteDate = DateTime.MinValue;
      this.IsLocalCopy = false;
    }

    public AttachmentInfo(string path)
      : this()
    {
      this.FileInfo = new FileInfo(path);
      this.Length = this.FileInfo.Length;
      this.CreationDate = this.FileInfo.CreationTimeUtc;
      this.LastWriteDate = this.FileInfo.LastWriteTimeUtc;
      this.Attribute = this.FileInfo.Name;
      this.Path = Guid.NewGuid().ToString();
    }

    public AttachmentInfo(AttachmentInfo src)
      : base((FileLinkInfo) src)
    {
      this.FileInfo = src.FileInfo;
      this.Length = src.Length;
      this.CreationDate = src.CreationDate;
      this.LastWriteDate = src.LastWriteDate;
      this.IsUploaded = src.IsUploaded;
      if (!this.IsUploaded)
        this.Path = Guid.NewGuid().ToString();
      this.IsLocalCopy = src.ExtId != 0;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj) => obj != null && obj is AttachmentInfo && base.Equals(obj);
  }
}
