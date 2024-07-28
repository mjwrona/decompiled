// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.EntitySearchResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts
{
  [DataContract]
  public abstract class EntitySearchResponse : SearchSecuredObject
  {
    private List<ErrorData> m_errors = new List<ErrorData>();

    [DataMember(Name = "errors")]
    public IEnumerable<ErrorData> Errors
    {
      get => (IEnumerable<ErrorData>) this.m_errors;
      private set => value.ToList<ErrorData>();
    }

    [DataMember(Name = "filterCategories")]
    public IEnumerable<FilterCategory> FilterCategories { get; set; }

    [DataMember(Name = "suggestions")]
    public IEnumerable<string> Suggestions { get; set; }

    public void AddError(ErrorData errorData) => this.m_errors.Add(errorData);

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<ErrorData> errors = this.Errors;
      this.Errors = errors != null ? (IEnumerable<ErrorData>) errors.Select<ErrorData, ErrorData>((Func<ErrorData, ErrorData>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<ErrorData>() : (IEnumerable<ErrorData>) null;
      IEnumerable<FilterCategory> filterCategories = this.FilterCategories;
      this.FilterCategories = filterCategories != null ? (IEnumerable<FilterCategory>) filterCategories.Select<FilterCategory, FilterCategory>((Func<FilterCategory, FilterCategory>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<FilterCategory>() : (IEnumerable<FilterCategory>) null;
    }
  }
}
