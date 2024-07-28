// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CustomRepositoryEntity
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class CustomRepositoryEntity
  {
    public CustomRepositoryEntity()
    {
      this.CollectionId = Guid.Empty;
      this.ProjectName = (string) null;
      this.RepositoryName = (string) null;
      this.RepositoryId = Guid.Empty;
    }

    public CustomRepositoryEntity(
      Guid collectionId,
      string projectName,
      string repositoryName,
      Guid repositoryId,
      string properties,
      Func<string, Type, object> converter)
    {
      this.CollectionId = collectionId;
      this.ProjectName = projectName;
      this.RepositoryName = repositoryName;
      this.RepositoryId = repositoryId;
      if (string.IsNullOrWhiteSpace(properties))
        return;
      if (converter == null)
        throw new ArgumentNullException(nameof (converter));
      this.Properties = (CustomRepositoryProperties) converter(properties, typeof (CustomRepositoryProperties));
    }

    public Guid CollectionId { get; set; }

    public string ProjectName { get; set; }

    public string RepositoryName { get; set; }

    public Guid RepositoryId { get; set; }

    public CustomRepositoryProperties Properties { get; set; }

    internal void WriteTo(
      out Guid collectionId,
      out string projectName,
      out string repositoryName,
      out string properties,
      Func<object, Type, string> converter)
    {
      collectionId = this.CollectionId;
      projectName = this.ProjectName;
      repositoryName = this.RepositoryName;
      properties = converter((object) this.Properties, typeof (CustomRepositoryProperties));
    }
  }
}
