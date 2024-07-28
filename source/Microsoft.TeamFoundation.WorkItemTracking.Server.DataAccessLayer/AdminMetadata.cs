// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AdminMetadata
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class AdminMetadata
  {
    public IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.TreeAdminData> TreeAdminData { get; set; }

    public IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.FieldAdminData> FieldAdminData { get; set; }

    public IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsagesAdminData> FieldUsagesAdminData { get; set; }

    internal AdminMetadata()
    {
      this.TreeAdminData = (IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.TreeAdminData>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.TreeAdminData>(0).AsReadOnly();
      this.FieldAdminData = (IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.FieldAdminData>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.FieldAdminData>(0).AsReadOnly();
      this.FieldUsagesAdminData = (IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsagesAdminData>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsagesAdminData>(0).AsReadOnly();
    }
  }
}
