// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.BatchRequestParameters
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class BatchRequestParameters
  {
    public bool HasBypassRules { get; set; }

    public bool UseLegacyIndexHandling { get; set; }

    public bool UseIdentityRef { get; set; }

    public bool ReturnProjectScopedUrl { get; set; }

    public bool HasSuppressNotifications { get; set; }
  }
}
