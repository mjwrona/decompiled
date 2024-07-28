// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.SortOption
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public class SortOption : SearchSecuredV2Object, ICloneable
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

    public override bool Equals(object obj) => (obj as SortOption).Field.Equals(this.Field, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => this.Field.ToUpper().GetHashCode();

    public object Clone() => (object) new SortOption()
    {
      SortOrder = this.SortOrder,
      SortOrderStr = this.SortOrderStr,
      Field = this.Field
    };
  }
}
