// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package.PackageSearchResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package
{
  [DataContract]
  public class PackageSearchResponse : SearchSecuredV2Object
  {
    [ClientResponseContent]
    [DataMember(Name = "Content")]
    public PackageSearchResponseContent Content { get; set; }

    [ClientResponseHeader("ActivityId")]
    [DataMember(Name = "ActivityId")]
    public IEnumerable<string> ActivityId { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Content.SetSecuredObject(namespaceId, requiredPermissions, token);
    }
  }
}
