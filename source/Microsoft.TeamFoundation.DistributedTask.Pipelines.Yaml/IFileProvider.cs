// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.IFileProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IFileProvider
  {
    RepositoryResource Repository { get; }

    string GetDirectoryName(string path);

    string GetFileContent(string path);

    string GetFileContentByUuid(string uuid);

    string GetFileName(string path);

    string GetFileUuid(string path);

    string ResolvePath(string defaultRoot, string path);
  }
}
