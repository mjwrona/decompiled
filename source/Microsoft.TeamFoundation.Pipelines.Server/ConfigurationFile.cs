// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ConfigurationFile
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class ConfigurationFile
  {
    public ConfigurationFile(
      string path,
      string content,
      bool isBase64Encoded,
      string webUrl = null,
      string objectId = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      ArgumentUtility.CheckForNull<string>(content, nameof (content));
      this.Path = path;
      this.Content = content;
      this.IsBase64Encoded = isBase64Encoded;
      this.WebUrl = webUrl;
      this.ObjectId = objectId;
    }

    public string Path { get; }

    public string Content { get; }

    public bool IsBase64Encoded { get; }

    public string WebUrl { get; }

    public string ObjectId { get; }
  }
}
