// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemStateColor : BaseSecuredObject
  {
    public WorkItemStateColor()
    {
    }

    public WorkItemStateColor(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Color { get; set; }

    [DataMember]
    public string Category { get; set; }
  }
}
