// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IProjectFeatureProvisioningDetails
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IProjectFeatureProvisioningDetails
  {
    ProcessDescriptor ProcessTemplateDescriptor { get; }

    string ProcessTemplateDescriptorName { get; }

    Guid ProcessTemplateDescriptorRowId { get; }

    string ProjectUri { get; }

    bool IsValid { get; }

    bool IsRecommended { get; }

    IEnumerable<ProjectProvisioningIssue> Issues { get; }
  }
}
