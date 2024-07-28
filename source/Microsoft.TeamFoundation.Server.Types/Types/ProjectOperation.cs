// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectOperation
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Types
{
  public class ProjectOperation : BaseAuditJobData
  {
    public Guid ProjectId { get; set; }

    public long Revision { get; set; }

    public Guid OperationId { get; set; }

    public List<ProjectOperationProperty> Properties { get; set; }

    public ProjectOperation() => this.Properties = new List<ProjectOperationProperty>();

    public override string ToString() => string.Format("[{0}, {1}, {2}]", (object) this.ProjectId, (object) this.Revision, (object) this.OperationId);

    public bool HasProperty(string name) => this.Properties != null && this.Properties.Any<ProjectOperationProperty>((Func<ProjectOperationProperty, bool>) (prop => string.Equals(prop.Name, name)));

    public T GetPropertyValueWithDefault<T>(string propertyName, T defaultValue)
    {
      if (this.Properties != null)
      {
        ProjectOperationProperty operationProperty = this.Properties.FirstOrDefault<ProjectOperationProperty>((Func<ProjectOperationProperty, bool>) (prop => string.Equals(prop.Name, propertyName, StringComparison.Ordinal)));
        if (operationProperty != null && operationProperty.Value is T)
          return (T) operationProperty.Value;
      }
      return defaultValue;
    }
  }
}
