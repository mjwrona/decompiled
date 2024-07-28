// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.NormalizedGitPath
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class NormalizedGitPath : IEquatable<NormalizedGitPath>
  {
    private readonly List<ArraySegment<byte>> m_parts;
    private readonly string m_str;
    private string m_withoutLeadingSlash;

    public NormalizedGitPath(string path)
    {
      string[] splitPath = NormalizedGitPath.SplitPath(path);
      this.m_str = NormalizedGitPath.FormatPath(splitPath, true, false);
      byte[] bytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(this.m_str);
      this.m_parts = new List<ArraySegment<byte>>(splitPath.Length);
      int num = 0;
      for (int index = 1; index < bytes.Length; ++index)
      {
        if (bytes[index] == (byte) 47)
        {
          int offset = num + 1;
          int count = index - offset;
          this.m_parts.Add(new ArraySegment<byte>(bytes, offset, count));
          num = index;
        }
      }
      if (bytes.Length <= 1)
        return;
      int offset1 = num + 1;
      int count1 = bytes.Length - offset1;
      this.m_parts.Add(new ArraySegment<byte>(bytes, offset1, count1));
    }

    public bool IsRoot => this.m_str == string.Empty;

    public int Depth => this.m_parts.Count;

    private NormalizedGitPath(List<ArraySegment<byte>> parts, string str)
    {
      this.m_parts = parts;
      this.m_str = str;
    }

    private static string[] SplitPath(string path) => path.Split(new char[1]
    {
      '/'
    }, StringSplitOptions.RemoveEmptyEntries);

    private static string FormatPath(string[] splitPath, bool startsWithSlash, bool endsWithSlash)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < splitPath.Length; ++index)
      {
        if (startsWithSlash || index > 0)
          stringBuilder.Append('/');
        stringBuilder.Append(splitPath[index]);
      }
      if (endsWithSlash)
        stringBuilder.Append("/");
      return stringBuilder.ToString();
    }

    internal static string TEST_FormatPath(
      string[] splitPath,
      bool startsWithSlash,
      bool endsWithSlash)
    {
      return NormalizedGitPath.FormatPath(splitPath, startsWithSlash, endsWithSlash);
    }

    internal NormalizedGitPath GetParent()
    {
      if (this.m_parts.Count == 0)
        throw new InvalidOperationException("no parent path");
      return new NormalizedGitPath(this.m_parts.GetRange(0, this.m_parts.Count - 1), this.m_str.Remove(this.m_str.LastIndexOf('/')));
    }

    public override bool Equals(object obj) => this == obj as NormalizedGitPath;

    public static bool operator ==(NormalizedGitPath a, NormalizedGitPath b)
    {
      if ((object) a != null && (object) b != null)
        return a.m_str == b.m_str;
      return (object) a == null && (object) b == null;
    }

    public static bool operator !=(NormalizedGitPath a, NormalizedGitPath b) => !(a == b);

    public bool Equals(NormalizedGitPath other) => !(other == (NormalizedGitPath) null) && StringComparer.Ordinal.Equals(this.m_str, other.m_str);

    public override string ToString() => this.m_str;

    public string WithoutLeadingSlash
    {
      get
      {
        if (this.m_withoutLeadingSlash == null)
          this.m_withoutLeadingSlash = !(this.m_str == string.Empty) ? this.m_str.Substring(1) : string.Empty;
        return this.m_withoutLeadingSlash;
      }
    }

    public override int GetHashCode() => this.m_str.GetHashCode();

    internal IReadOnlyList<ArraySegment<byte>> Parts => (IReadOnlyList<ArraySegment<byte>>) this.m_parts;
  }
}
