// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.FilePath
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class FilePath : IEquatable<FilePath>
  {
    private readonly string m_path;
    private const char c_separator = '/';
    private static readonly string c_separatorString = '/'.ToString();

    public FilePath(string path)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      this.m_path = FilePath.EnsureIsRooted(FilePath.NormalizePath(path));
    }

    public FilePath Folder
    {
      get
      {
        int length = this.m_path.LastIndexOf('/');
        if (length < 0)
          return (FilePath) null;
        if (length == 0)
        {
          if (this.m_path.Equals(FilePath.c_separatorString, StringComparison.OrdinalIgnoreCase))
            return (FilePath) null;
          if (this.m_path.Length > 1)
            return new FilePath(FilePath.c_separatorString);
        }
        return new FilePath(this.m_path.Substring(0, length));
      }
    }

    public FilePath AppendPath(string path)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      string path1 = FilePath.NormalizePath(path).TrimStart('/');
      if (path1.Length == 0)
        return this;
      FilePath filePath = this;
      for (; path1.StartsWith("../"); path1 = path1.Substring(3))
        filePath = filePath?.Folder;
      if (filePath == (FilePath) null)
        return new FilePath(path1);
      return new FilePath(filePath.m_path.TrimEnd('/') + "/" + path1);
    }

    public override string ToString() => this.m_path;

    public bool Equals(FilePath other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || string.Equals(this.m_path, other.m_path, StringComparison.Ordinal);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      FilePath other = obj as FilePath;
      return (object) other != null && this.Equals(other);
    }

    public static bool operator ==(FilePath path1, FilePath path2)
    {
      if ((object) path1 == null && (object) path2 == null)
        return true;
      return (object) path1 != null && (object) path2 != null && path1.Equals(path2);
    }

    public static bool operator !=(FilePath path1, FilePath path2) => !(path1 == path2);

    public override int GetHashCode() => this.m_path.GetHashCode();

    private static string EnsureIsRooted(string path) => path.StartsWith(FilePath.c_separatorString) ? path : "/" + path;

    private static string NormalizePath(string path) => path.Replace('\\', '/');
  }
}
