// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ScopedMetadataComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ScopedMetadataComponent5 : ScopedMetadataComponent4
  {
    internal override IEnumerable<string> GetAllowedValues(
      int fieldId,
      string projectName,
      IEnumerable<string> workItemTypeNames,
      bool sortById = false,
      bool excludeIdentities = false)
    {
      bool metadataFilterEnabled = this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).MetadataFilterEnabled;
      this.PrepareStoredProcedure(nameof (GetAllowedValues));
      this.BindUserSid();
      this.BindBoolean("@metadataFilterEnabled", metadataFilterEnabled);
      this.BindInt("@fieldId", fieldId);
      this.BindString("@projectName", projectName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@sortById", sortById);
      if (metadataFilterEnabled)
      {
        IEnumerable<int> projects;
        this.RequestContext.GetService<UserProjectsCache>().GetUserProjects(this.RequestContext, out projects);
        this.BindInt32Table("@userProjects", projects);
      }
      if (!string.IsNullOrEmpty(projectName))
        this.BindStringTable("@workItemTypeNames", workItemTypeNames);
      return (IEnumerable<string>) WorkItemTrackingResourceComponent.Bind<string>(this.ExecuteReader(), (System.Func<IDataReader, string>) (reader1 => reader1.GetString(0))).ToList<string>();
    }
  }
}
