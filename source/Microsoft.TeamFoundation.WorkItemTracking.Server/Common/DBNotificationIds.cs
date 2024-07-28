// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.DBNotificationIds
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class DBNotificationIds
  {
    public static readonly Guid WorkItemTrackingProvisionedMetadataChanged = new Guid("891321DE-3E0E-47AA-898C-4141262734B7");
    public static readonly Guid ProcessWorkItemMetadataDeleted = new Guid("940A67AC-C5D9-4490-9557-47818BFD8937");
    public static readonly Guid WorkItemStateDefinitionModified = new Guid("9B4EF705-8408-4B4D-A575-BC0FE81304A6");
    public static readonly Guid WorkItemTypeBehaviorReferenceModified = new Guid("9C2ACAD5-16AF-457D-8E68-E12B0437871F");
    public static readonly Guid WorkItemTemplatesModified = new Guid("449F1C62-777D-482E-9373-18E0A2B1A4DF");
    public static readonly Guid WorkItemTemplatesModifiedByWorkItemTypeRename = new Guid("FB33B12C-5847-4702-858D-13A87CE63A46");
  }
}
