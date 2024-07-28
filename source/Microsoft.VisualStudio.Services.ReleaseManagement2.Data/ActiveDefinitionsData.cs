// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.ActiveDefinitionsData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  public class ActiveDefinitionsData
  {
    public List<ReleaseEnvironmentStep> PendingApprovals { get; set; }

    public List<int> ActiveDefinitionIds { get; set; }

    public List<int> RecentDefinitionIds { get; set; }

    public List<ReleaseDefinition> Definitions { get; set; }

    public List<Deployment> Deployments { get; set; }

    public List<int> DefinitionsWithApprovalPendingOnUser { get; set; }

    public List<int> DefinitionsWithDeploymentRequestedByUser { get; set; }

    public List<int> DefinitionsWithApprovalCompletedByUser { get; set; }
  }
}
