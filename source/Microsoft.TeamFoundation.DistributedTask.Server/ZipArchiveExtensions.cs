// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ZipArchiveExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class ZipArchiveExtensions
  {
    internal static ZipArchiveEntry GetEntry(
      this ZipArchive zipArchive,
      string fullPath,
      StringComparison comparisonType)
    {
      return zipArchive == null ? (ZipArchiveEntry) null : zipArchive.Entries.Where<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (entry => string.Equals(entry.FullName, fullPath, comparisonType))).FirstOrDefault<ZipArchiveEntry>();
    }
  }
}
