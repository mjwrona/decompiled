// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.LibraryWorkItemsData
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  [ClientIncludeModel]
  public class LibraryWorkItemsData
  {
    public LibraryWorkItemsData()
    {
      this.WorkItems = (IList<WorkItemDetails>) new List<WorkItemDetails>();
      this.HasMoreElements = false;
    }

    [DataMember(IsRequired = true, Name = "WorkItems")]
    public IList<WorkItemDetails> WorkItems { get; set; }

    [DataMember(IsRequired = false, Name = "WorkItemIds")]
    public IList<int> WorkItemIds { get; set; }

    [DataMember(IsRequired = true, Name = "HasMoreElements")]
    public bool HasMoreElements { get; set; }

    [DataMember(IsRequired = true, Name = "ExceededWorkItemQueryLimit")]
    public bool ExceededWorkItemQueryLimit { get; set; }

    [DataMember(IsRequired = false, Name = "ContinuationToken", EmitDefaultValue = false)]
    public string ContinuationToken { get; set; }

    [DataMember(IsRequired = true, Name = "ReturnCode")]
    public LibraryTestCasesDataReturnCode ReturnCode { get; set; }

    [DataMember(IsRequired = false, Name = "ColumnOptions")]
    public List<string> ColumnOptions { get; set; }
  }
}
