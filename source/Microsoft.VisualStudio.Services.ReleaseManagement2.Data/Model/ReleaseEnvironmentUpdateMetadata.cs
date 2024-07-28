// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseEnvironmentUpdateMetadata
  {
    public ReleaseEnvironmentStatus Status { get; set; }

    public DateTime? ScheduledDeploymentTime { get; set; }

    public string Comment { get; set; }

    public bool AddCommentAsDeploymentIssue { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IDictionary<string, ConfigurationVariableValue> Variables { get; set; }

    public bool IsApprovalScheduledDeploymentUpdate() => this.Status == ReleaseEnvironmentStatus.Undefined;

    public ReleaseEnvironmentUpdateMetadata() => this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
