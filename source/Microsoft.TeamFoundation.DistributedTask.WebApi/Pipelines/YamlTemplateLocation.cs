// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.YamlTemplateLocation
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  public sealed class YamlTemplateLocation
  {
    private const string repoPrefix = "repo=";
    private const string filePrefix = "/file=";
    [DataMember(Name = "RepositoryAlias")]
    private string m_repositoryAlias;
    [DataMember(Name = "Path")]
    private string m_path;

    public YamlTemplateLocation()
    {
    }

    public YamlTemplateLocation(string repositoryAlias, string path)
    {
      this.m_repositoryAlias = repositoryAlias;
      this.m_path = this.CononicalizePath(path);
    }

    public string RepositoryAlias => this.m_repositoryAlias;

    public string Path => this.m_path;

    private string CononicalizePath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      path = path.Replace('\\', '/').TrimStart('/');
      return path;
    }

    public static YamlTemplateLocation FromString(string location)
    {
      if (!string.IsNullOrEmpty(location))
      {
        int startIndex = location.StartsWith("repo=", StringComparison.OrdinalIgnoreCase) ? "repo=".Length : -1;
        int length = location.IndexOf("/file=", StringComparison.OrdinalIgnoreCase) - 1;
        if (startIndex > -1 && length > startIndex)
          return new YamlTemplateLocation(Uri.UnescapeDataString(location.Substring(startIndex, length)), Uri.UnescapeDataString(location.Substring(length + "/file=".Length + 1)));
      }
      return (YamlTemplateLocation) null;
    }

    public override string ToString() => "repo=" + Uri.EscapeDataString(this.RepositoryAlias) + "/file=" + Uri.EscapeDataString(this.Path);
  }
}
