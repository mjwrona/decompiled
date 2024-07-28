// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code
{
  [DataContract]
  public class CodeResult : SearchSecuredV2Object
  {
    [DataMember(Name = "fileName")]
    public string Filename { get; set; }

    [DataMember(Name = "path")]
    public string Path { get; set; }

    [DataMember(Name = "matches")]
    public IDictionary<string, IEnumerable<Hit>> Matches { get; set; }

    [DataMember(Name = "collection")]
    public Collection Collection { get; set; }

    [DataMember(Name = "project")]
    public Project Project { get; set; }

    [DataMember(Name = "repository")]
    public Repository Repository { get; set; }

    [DataMember(Name = "versions")]
    public IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version> Versions { get; set; }

    [DataMember(Name = "contentId")]
    public string ContentId { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Collection?.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Project?.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Repository?.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version> versions = this.Versions;
      this.Versions = versions != null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>) versions.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>() : (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>) null;
      IDictionary<string, IEnumerable<Hit>> matches = this.Matches;
      foreach (string key in (matches != null ? (IEnumerable<string>) matches.Keys.ToList<string>() : (IEnumerable<string>) null) ?? Enumerable.Empty<string>())
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
