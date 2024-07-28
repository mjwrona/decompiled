// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionReconciliationJobData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemTypeExtensionReconciliationJobData
  {
    public WorkItemTypeExtensionReconciliationJobDetail[] Details { get; set; }

    public Guid ReconciliationWatermark { get; set; }

    public bool IsCancelable { get; set; }

    public bool RefreshTree { get; set; }

    public IdentityDescriptor IdentityDescriptor { get; set; }

    public string StackTrace { get; set; }

    public bool SkipWITChangeDateUpdate { get; set; }
  }
}
