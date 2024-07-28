// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.DefaultQuerySettings
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;

namespace Microsoft.AspNet.OData.Query
{
  public class DefaultQuerySettings
  {
    private int? _maxTop = new int?(0);

    public bool EnableExpand { get; set; }

    public bool EnableSelect { get; set; }

    public bool EnableCount { get; set; }

    public bool EnableOrderBy { get; set; }

    public bool EnableFilter { get; set; }

    public int? MaxTop
    {
      get => this._maxTop;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num = 0;
          if (nullable.GetValueOrDefault() < num & nullable.HasValue)
            throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 0);
        }
        this._maxTop = value;
      }
    }

    public bool EnableSkipToken { get; set; }
  }
}
