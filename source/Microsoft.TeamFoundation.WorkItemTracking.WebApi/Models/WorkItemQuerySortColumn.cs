// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemQuerySortColumn
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemQuerySortColumn : BaseSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public WorkItemFieldReference Field { get; set; }

    [DataMember]
    public bool Descending { get; set; }

    public WorkItemQuerySortColumn()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public WorkItemQuerySortColumn(ISecuredObject securedObject)
      : base(securedObject)
    {
    }
  }
}
