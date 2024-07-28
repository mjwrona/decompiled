// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.WikiResult
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class WikiResult
  {
    [DataMember(Name = "fileName")]
    public string Filename { get; set; }

    [DataMember(Name = "path")]
    public string Path { get; set; }

    [DataMember(Name = "hits")]
    public IEnumerable<WikiHitSnippet> Hits { get; set; }

    [DataMember(Name = "collection")]
    public string CollectionName { get; set; }

    [DataMember(Name = "collectionUrl")]
    public string CollectionUrl { get; set; }

    public string ProjectId { get; set; }

    [DataMember(Name = "project")]
    public string ProjectName { get; set; }

    [DataMember(Name = "repository")]
    public string RepositoryName { get; set; }

    [DataMember(Name = "repositoryId")]
    public string RepositoryID { get; set; }

    [DataMember(Name = "wiki")]
    public string WikiName { get; set; }

    [DataMember(Name = "wikiId")]
    public string WikiId { get; set; }

    [DataMember(Name = "wikiVersion")]
    public string WikiVersion { get; set; }

    [DataMember(Name = "mappedPath")]
    public string MappedPath { get; set; }

    [DataMember(Name = "contentId")]
    public string ContentId { get; set; }

    [DataMember(Name = "lastUpdated")]
    public DateTime LastUpdated { get; set; }

    [DataMember(Name = "visibility")]
    public string Visibility { get; set; }

    public WikiResult(
      string filename,
      string path,
      IEnumerable<WikiHitSnippet> hitSnippets,
      string collection,
      string collectionUrl,
      string projectId,
      string projectName,
      string repository,
      string repositoryId,
      string wikiName,
      string wikiId,
      string wikiVersion,
      string mappedPath,
      string contentId,
      DateTime lastUpdated,
      string visibility)
    {
      this.Filename = filename;
      this.Path = path;
      this.Hits = hitSnippets;
      this.CollectionName = collection;
      this.CollectionUrl = collectionUrl;
      this.ProjectId = projectId;
      this.ProjectName = projectName;
      this.RepositoryName = repository;
      this.RepositoryID = repositoryId;
      this.WikiName = wikiName;
      this.WikiId = wikiId;
      this.WikiVersion = wikiVersion;
      this.MappedPath = mappedPath;
      this.ContentId = contentId;
      this.LastUpdated = lastUpdated;
      this.Visibility = visibility;
    }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.AppendFormat("{0}\\{1}\\{2}\\{3}\\{4}\\{5}\\{6}\\{7}\\{8}\\{9}\\{10}\\{11}\\{12}\\{13}", (object) indentSpacing, (object) this.CollectionName, (object) this.ProjectId, (object) this.ProjectName, (object) this.RepositoryName, (object) this.RepositoryID, (object) this.WikiName, (object) this.WikiId, (object) this.WikiVersion, (object) this.MappedPath, (object) this.Path, (object) this.Filename, (object) this.LastUpdated.ToString("o"), (object) this.Visibility);
      sb.AppendLine();
      sb.AppendLine(indentSpacing, "Content ID: " + this.ContentId);
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
