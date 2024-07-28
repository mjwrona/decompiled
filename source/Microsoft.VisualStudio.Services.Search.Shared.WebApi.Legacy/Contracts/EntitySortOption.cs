// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.EntitySortOption
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts
{
  [DataContract]
  public class EntitySortOption : SearchSecuredObject, ICloneable
  {
    [DataMember(Name = "field")]
    public string Field { get; set; }

    [DataMember(Name = "sortOrder")]
    public string SortOrderStr
    {
      get
      {
        switch (this.SortOrder)
        {
          case SortOrder.Ascending:
            return "ASC";
          case SortOrder.Descending:
            return "DESC";
          default:
            return (string) null;
        }
      }
      set
      {
        if (string.Equals(value, "ASC", StringComparison.OrdinalIgnoreCase))
          this.SortOrder = SortOrder.Ascending;
        else if (string.Equals(value, "DESC", StringComparison.OrdinalIgnoreCase))
          this.SortOrder = SortOrder.Descending;
        else
          this.SortOrder = SortOrder.Undefined;
      }
    }

    public SortOrder SortOrder { get; set; }

    public override bool Equals(object obj) => (obj as EntitySortOption).Field.Equals(this.Field, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => this.Field.ToUpper().GetHashCode();

    public object Clone() => (object) new EntitySortOption()
    {
      SortOrder = this.SortOrder,
      SortOrderStr = this.SortOrderStr,
      Field = this.Field
    };
  }
}
