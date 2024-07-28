// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ColumnSortOrderModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class ColumnSortOrderModel
  {
    [DataMember(Name = "index")]
    public string Index { get; set; }

    [DataMember(Name = "order")]
    public string Order { get; set; }

    public override string ToString() => !string.IsNullOrWhiteSpace(this.Index) && !string.IsNullOrWhiteSpace(this.Order) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) this.Index, (object) this.Order) : string.Empty;

    public static ColumnSortOrderModel Default => new ColumnSortOrderModel()
    {
      Index = CoreFieldReferenceNames.Id,
      Order = "asc"
    };

    public static ColumnSortOrderModel FromString(string sortOrder)
    {
      if (string.IsNullOrEmpty(sortOrder))
        return ColumnSortOrderModel.Default;
      string[] strArray = sortOrder.Split('=');
      if (strArray.Length != 2)
        return ColumnSortOrderModel.Default;
      return new ColumnSortOrderModel()
      {
        Index = strArray[0],
        Order = strArray[1]
      };
    }
  }
}
