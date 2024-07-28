// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  [DataContract]
  public class CodeResult : SearchSecuredObject
  {
    [DataMember(Name = "fileName")]
    public string Filename { get; set; }

    [DataMember(Name = "path")]
    public string Path { get; set; }

    [DataMember(Name = "hitCount")]
    public int HitCount { get; set; }

    [DataMember(Name = "hits")]
    public IEnumerable<Hit> Hits { get; set; }

    [DataMember(Name = "matches")]
    public IDictionary<string, IEnumerable<Hit>> Matches { get; set; }

    [DataMember(Name = "collection")]
    public string Collection { get; set; }

    [DataMember(Name = "project")]
    public string Project { get; set; }

    [DataMember(Name = "projectId")]
    public string ProjectId { get; set; }

    [DataMember(Name = "repository")]
    public string Repository { get; set; }

    [DataMember(Name = "repositoryId")]
    public string RepositoryID { get; set; }

    [DataMember(Name = "branch")]
    public string Branch { get; set; }

    [DataMember(Name = "versions")]
    public IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version> Versions { get; set; }

    [DataMember(Name = "changeId")]
    public string ChangeId { get; set; }

    [DataMember(Name = "contentId")]
    public string ContentId { get; set; }

    [DataMember(Name = "vcType")]
    public VersionControlType VcType { get; set; }

    public CodeResult(
      string filename,
      string path,
      int hitCount,
      IEnumerable<Hit> hits,
      IDictionary<string, IEnumerable<Hit>> matches,
      string collection,
      string project,
      string repository,
      string repositoryId,
      string branch,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version> versions,
      string changeId,
      string contentId,
      VersionControlType vcType)
    {
      this.Filename = filename;
      this.Path = path;
      this.HitCount = hitCount;
      this.Hits = hits;
      this.Matches = matches;
      this.Collection = collection;
      this.Project = project;
      this.Repository = repository;
      this.RepositoryID = repositoryId;
      this.Branch = branch;
      this.Versions = versions;
      this.ChangeId = changeId;
      this.ContentId = contentId;
      this.VcType = vcType;
    }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}\\{2}\\{3}\\{4}\\{5}\\{6}\\{7}", (object) indentSpacing, (object) this.Collection, (object) this.Project, (object) this.Repository, (object) this.RepositoryID, (object) this.Branch, (object) this.Path, (object) this.Filename);
      sb.AppendLine();
      sb.AppendLine(indentSpacing, "Branches: " + this.Versions.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version, string>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version, string>) (x => x.BranchName)).Aggregate<string>((Func<string, string, string>) ((x, y) => x + "," + y)));
      sb.AppendLine(indentSpacing, "Change ID: " + this.ChangeId);
      sb.AppendLine(indentSpacing, "Content ID: " + this.ContentId);
      sb.AppendLine(indentSpacing, "Version Control Type: " + this.VcType.ToString());
      sb.Append(indentSpacing, "# of hits: ").AppendLine(this.HitCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<Hit> hits = this.Hits;
      this.Hits = hits != null ? (IEnumerable<Hit>) hits.Select<Hit, Hit>((Func<Hit, Hit>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<Hit>() : (IEnumerable<Hit>) null;
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version> versions = this.Versions;
      this.Versions = versions != null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>) versions.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>() : (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>) null;
      foreach (string key in (IEnumerable<string>) this.Matches.Keys.ToList<string>())
      {
        IEnumerable<Hit> match = this.Matches[key];
        IEnumerable<Hit> list = match != null ? (IEnumerable<Hit>) match.Select<Hit, Hit>((Func<Hit, Hit>) (i =>
        {
          i.SetSecuredObject(namespaceId, requiredPermissions, token);
          return i;
        })).ToList<Hit>() : (IEnumerable<Hit>) null;
        this.Matches[key] = list;
      }
    }
  }
}
