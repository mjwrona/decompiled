// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IProjectProvisioningContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IProjectProvisioningContext : IProjectMetadata
  {
    void AddWorkItemTypeCategory(WorkItemTypeCategory category);

    void AddWorkItemType(WorkItemTypeMetadata workItemType);

    void AddProcessConfiguration(ProjectProcessConfiguration processConfiguration);

    IProjectMetadata ProcessTemplate { get; }

    void ReportIssue(ProjectProvisioningIssue issue);

    bool ValidateOnly { get; }
  }
}
