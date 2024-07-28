// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens.CommitLogEntryExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens
{
  public static class CommitLogEntryExtensions
  {
    public static CommitLogBookmark GetCommitLogBookmark(this ICommitLogEntry entry)
    {
      long? seq = entry.SequenceNumber != 0L ? new long?(entry.SequenceNumber) : new long?();
      return new CommitLogBookmark(entry.CommitId, seq);
    }

    public static CommitLogBookmark GetNextCommitLogBookmark(this CommitLogEntry entry)
    {
      long? seq = entry.SequenceNumber != 0L ? new long?(entry.SequenceNumber + 1L) : new long?();
      return new CommitLogBookmark(entry.NextCommitId, seq);
    }
  }
}
