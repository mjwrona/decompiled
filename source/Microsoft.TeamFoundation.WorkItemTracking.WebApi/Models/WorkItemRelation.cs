// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRelation
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemRelation : Link, IEquatable<WorkItemRelation>
  {
    public WorkItemRelation()
    {
    }

    public WorkItemRelation(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public bool Equals(WorkItemRelation other)
    {
      if (other == null || !string.Equals(this.Rel, other.Rel, StringComparison.OrdinalIgnoreCase))
        return false;
      object obj1 = (object) null;
      object obj2 = (object) null;
      if (this.Attributes != null && other.Attributes != null)
      {
        this.Attributes.TryGetValue("id", out obj1);
        other.Attributes.TryGetValue("id", out obj2);
      }
      return string.Equals(this.Url, other.Url, StringComparison.OrdinalIgnoreCase) && string.Equals(obj1?.ToString(), obj2?.ToString(), StringComparison.OrdinalIgnoreCase);
    }
  }
}
