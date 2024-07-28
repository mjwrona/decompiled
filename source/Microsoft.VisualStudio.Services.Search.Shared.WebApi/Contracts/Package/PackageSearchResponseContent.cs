// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package.PackageSearchResponseContent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package
{
  [DataContract]
  public class PackageSearchResponseContent : EntitySearchResponse
  {
    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "results")]
    public IEnumerable<PackageResult> Results { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<PackageResult> results = this.Results;
      this.Results = results != null ? (IEnumerable<PackageResult>) results.Select<PackageResult, PackageResult>((Func<PackageResult, PackageResult>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<PackageResult>() : (IEnumerable<PackageResult>) null;
    }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(Extensions.GetIndentSpacing(indentLevel), "Package results:").Append(this.Results.ToString());
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
