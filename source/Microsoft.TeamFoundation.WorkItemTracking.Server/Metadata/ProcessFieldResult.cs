// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessFieldResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessFieldResult : IEquatable<ProcessFieldResult>
  {
    public string ReferenceName { get; set; }

    public string Name { get; set; }

    public InternalFieldType Type { get; set; }

    public string Description { get; set; }

    public bool IsSystem { get; set; }

    public bool IsBehaviorField { get; set; }

    public Guid ProcessId { get; set; }

    public Guid? PickListId { get; set; }

    public bool IsIdentity { get; set; }

    public bool IsLocked { get; set; }

    public bool Equals(ProcessFieldResult other)
    {
      if (other == null)
        return false;
      return this == other || TFStringComparer.WorkItemFieldReferenceName.Equals(this.ReferenceName, other.ReferenceName);
    }

    public override int GetHashCode()
    {
      string referenceName = this.ReferenceName;
      return referenceName == null ? 0 : referenceName.GetHashCode();
    }
  }
}
