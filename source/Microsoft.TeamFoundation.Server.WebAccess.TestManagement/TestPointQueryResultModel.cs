// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPointQueryResultModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  [KnownType(typeof (IdentityRef))]
  internal class TestPointQueryResultModel
  {
    [DataMember(Name = "testPoints")]
    public List<TestPointModel> TestPoints { get; set; }

    [DataMember(Name = "columnOptions")]
    public List<TestPointGridDisplayColumn> ColumnOptions { get; set; }

    [DataMember(Name = "columnSortOrder")]
    public ColumnSortOrderModel ColumnSortOrder { get; set; }

    [DataMember(Name = "sortedPointIds")]
    public List<int> SortedPointIds { get; set; }

    [DataMember(Name = "testCaseIds")]
    public List<int> TestCaseIds { get; set; }

    [DataMember(Name = "totalPointsCount")]
    public int TotalPointsCount { get; set; }

    [DataMember(Name = "configurations")]
    public List<TestConfigurationModel> Configurations { get; set; }

    [DataMember(Name = "pagedColumns", EmitDefaultValue = false)]
    public IEnumerable<TestPointGridDisplayColumn> PagedColumns { get; set; }
  }
}
