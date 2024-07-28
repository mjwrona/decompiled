// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ZipPackageReader
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class ZipPackageReader : ITaskPackageReader, IDisposable
  {
    private readonly bool m_ownsPackage;
    private readonly IDictionary<string, string> m_lookup = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ZipPackageReader(ZipArchive archive, bool ownsPackage, Stream originalStream = null)
      : this(archive, string.Empty, ownsPackage, originalStream)
    {
    }

    public ZipPackageReader(
      ZipArchive archive,
      string folder,
      bool ownsPackage,
      Stream originalStream = null)
    {
      string str;
      if (folder == null)
        str = (string) null;
      else
        str = folder.Replace('\\', '/').Trim('/');
      if (str == null)
        str = string.Empty;
      folder = str;
      if (!string.IsNullOrEmpty(folder))
        folder += "/";
      this.Archive = archive;
      if (this.Archive?.Entries != null)
      {
        foreach (ZipArchiveEntry entry in this.Archive.Entries)
        {
          string fullName = entry.FullName;
          this.m_lookup[this.Normalize(entry.FullName)] = fullName;
        }
      }
      this.Folder = folder;
      this.m_ownsPackage = ownsPackage;
      this.OriginalStream = originalStream;
    }

    public ZipArchive Archive { get; }

    public string Folder { get; }

    public Stream OriginalStream { get; private set; }

    public ITaskPackageReader CreateReader(string path) => (ITaskPackageReader) new ZipPackageReader(this.Archive, path, false, this.OriginalStream);

    public void Dispose()
    {
      if (this.m_ownsPackage)
        this.Archive?.Dispose();
      this.OriginalStream?.Dispose();
      this.OriginalStream = (Stream) null;
    }

    public bool Exists(string path) => this.m_lookup.ContainsKey(this.AbsolutePath(path));

    public IEnumerable<string> GetEntries()
    {
      string normalizedFolder = this.Normalize(this.Folder);
      return this.m_lookup.Keys.Where<string>((Func<string, bool>) (x => x.StartsWith(normalizedFolder, StringComparison.OrdinalIgnoreCase) && !x.EndsWith("/") && !x.EndsWith("\\"))).Select<string, string>((Func<string, string>) (x => x.Remove(0, this.Folder.Length).TrimStart('/')));
    }

    public Stream GetStream(string path) => this.Archive.GetEntry(this.m_lookup[this.AbsolutePath(path)], StringComparison.OrdinalIgnoreCase).Open();

    private string AbsolutePath(string path) => string.IsNullOrEmpty(this.Folder) ? path : this.Folder + path;

    private string Normalize(string str) => str.Replace('\\', '/');
  }
}
