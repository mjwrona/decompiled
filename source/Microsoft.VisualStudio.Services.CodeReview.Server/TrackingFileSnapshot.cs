// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.TrackingFileSnapshot
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.VisualStudio.Services.CodeReview.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class TrackingFileSnapshot : IEquatable<TrackingFileSnapshot>
  {
    public int IterationId;
    public int ChangeTrackingId;
    public TrackingVersion Version;
    public string Filename;
    public string FileHash;
    public string SecondaryFilename;
    public HashSet<int> ThreadIds;
    private int FileCount;
    private DiffFile FileData;
    private Encoding FileEncoding;

    public TrackingFileSnapshot(int iterationId, int changeTrackingId, TrackingVersion version)
    {
      this.IterationId = iterationId;
      this.ChangeTrackingId = changeTrackingId;
      this.Version = version;
      this.ThreadIds = new HashSet<int>();
      this.Filename = (string) null;
      this.SecondaryFilename = (string) null;
      this.FileHash = (string) null;
      this.FileData = (DiffFile) null;
      this.FileEncoding = Encoding.Default;
      this.FileCount = 0;
    }

    public void SetFileDataFromStream(Stream fileStream)
    {
      this.FileData = DiffFile.Create(fileStream, this.FileEncoding, this.GetDiffOptions());
      fileStream.Dispose();
    }

    public void SetEncodingFromStream(Stream fileStream, long scanBytes = 1024100)
    {
      Encoding encoding = FileTypeUtil.DetermineEncoding(fileStream, false, Encoding.Default, scanBytes, out bool _);
      fileStream.Dispose();
      this.FileEncoding = encoding == null ? Encoding.Default : encoding;
    }

    public DiffFile PeekFileData() => this.FileData;

    public DiffFile GetFileData()
    {
      DiffFile fileData = this.FileData;
      if (--this.FileCount != 0)
        return fileData;
      this.FileData = (DiffFile) null;
      return fileData;
    }

    public void SetFileData(DiffFile fileData) => this.FileData = fileData;

    public void SetEncoding(Encoding encoding) => this.FileEncoding = encoding;

    public void IncrementFileCount() => ++this.FileCount;

    public void DecrementFileCount() => --this.FileCount;

    public int GetFileCount() => this.FileCount;

    public void AddThread(int id) => this.ThreadIds.Add(id);

    public bool HasFileData() => this.FileData != null;

    public bool IsValid() => !string.IsNullOrEmpty(this.FileHash) && !string.IsNullOrEmpty(this.Filename) && this.FileCount > 0;

    public bool ClearIfInvalid()
    {
      if (this.IsValid())
        return false;
      this.FileData = (DiffFile) null;
      this.FileCount = 0;
      return true;
    }

    public Encoding GetFileEncoding() => this.FileEncoding;

    public DiffOptions GetDiffOptions()
    {
      DiffOptions diffOptions = new DiffOptions();
      diffOptions.SourceLabel = "Original";
      diffOptions.TargetLabel = "Modified";
      diffOptions.UseThirdPartyTool = false;
      diffOptions.OutputType = DiffOutputType.Context;
      diffOptions.Flags |= DiffOptionFlags.IgnoreLeadingAndTrailingWhiteSpace;
      diffOptions.Flags |= DiffOptionFlags.IgnoreEndOfLineDifference;
      diffOptions.Flags |= DiffOptionFlags.IgnoreEndOfFileEndOfLineDifference;
      return diffOptions;
    }

    public object FormatCI() => (object) new
    {
      ChangeTrackingId = this.ChangeTrackingId,
      FileHash = this.FileHash,
      IterationId = this.IterationId,
      ThreadIds = this.ThreadIds,
      Version = this.Version.ToString(),
      FileCount = this.FileCount,
      IsValid = this.IsValid()
    };

    public override int GetHashCode() => this.IterationId.GetHashCode() ^ this.ChangeTrackingId.GetHashCode() ^ this.Version.GetHashCode();

    public bool Equals(TrackingFileSnapshot other) => other != null & this.IterationId == other.IterationId & this.ChangeTrackingId == other.ChangeTrackingId & this.Version.Equals((object) other.Version);

    public override string ToString() => this.IterationId.ToString() + ":" + this.ChangeTrackingId.ToString() + ":" + this.Version.ToString();
  }
}
