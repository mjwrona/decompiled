// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiPage
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi.Shared;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [DataContract]
  public class WikiPage : WikiPageCreateOrUpdateParameters, IBaseResource
  {
    private WikiPage(GitItem item)
    {
      this.InternalItem = item;
      this.GitItemPath = item?.Path;
    }

    [JsonConstructor]
    public WikiPage(
      string path,
      bool isParentPage,
      int order,
      GitItem item = null,
      bool isNonConformant = false)
      : this(item)
    {
      this.Path = this.NormalizePath(path);
      this.IsParentPage = isParentPage;
      this.Order = order;
      this.IsNonConformant = isNonConformant;
      this.SubPages = (IList<WikiPage>) new List<WikiPage>();
    }

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public string Path { get; private set; }

    [DataMember(Name = "order")]
    public int Order { get; set; }

    [DataMember(Name = "isParentPage", EmitDefaultValue = false)]
    public bool IsParentPage { get; private set; }

    [DataMember(Name = "isNonConformant", EmitDefaultValue = false)]
    public bool IsNonConformant { get; private set; }

    [DataMember(Name = "gitItemPath", EmitDefaultValue = false)]
    public string GitItemPath { get; private set; }

    [DataMember(Name = "subPages", EmitDefaultValue = false)]
    public IList<WikiPage> SubPages { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "remoteUrl", EmitDefaultValue = false)]
    public string RemoteUrl { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
    public int? Id { get; set; }

    [IgnoreDataMember]
    public GitItem InternalItem { get; private set; }

    private string NormalizePath(string path) => !string.IsNullOrEmpty(path) ? path.Replace("\\", "/") : (string) null;

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      foreach (WikiSecuredObject subPage in (IEnumerable<WikiPage>) this.SubPages)
        subPage.SetSecuredObject(securedObject);
    }
  }
}
