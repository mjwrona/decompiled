// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeQueryResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  [DataContract]
  public class CodeQueryResponse : EntitySearchResponse
  {
    [DataMember(Name = "query")]
    public SearchQuery Query { get; set; }

    [DataMember(Name = "results")]
    public CodeResults Results { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "indentLevel+1", Justification = "indentLevel is not likely to exceed int.Max range.")]
    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.AppendLine(indentSpacing, "Search query:").Append(this.Query.ToString(indentLevel + 1));
      sb.AppendLine(indentSpacing, "Code results:").Append(this.Results.ToString(indentLevel + 1));
      sb.AppendLine(indentSpacing, "Filter categories:");
      if (this.FilterCategories != null)
      {
        foreach (FilterCategory filterCategory in this.FilterCategories)
          sb.Append(filterCategory.ToString(indentLevel + 1));
      }
      if (this.Errors != null)
      {
        sb.AppendLine(indentSpacing, "Errors:");
        foreach (ErrorData error in this.Errors)
          sb.Append(error.ToString(indentLevel + 1));
      }
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Query.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Results.SetSecuredObject(namespaceId, requiredPermissions, token);
    }
  }
}
