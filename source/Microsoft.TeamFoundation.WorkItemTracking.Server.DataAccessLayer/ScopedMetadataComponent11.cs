// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ScopedMetadataComponent11
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
  internal class ScopedMetadataComponent11 : ScopedMetadataComponent9
  {
    internal override IEnumerable<string> GetAllowedValues(
      int fieldId,
      string projectName,
      IEnumerable<string> workItemTypeNames,
      bool sortById = false,
      bool excludeIdentities = false)
    {
      IEnumerable<string> strings;
      return this.GetAllowedValues((IEnumerable<int>) new int[1]
      {
        fieldId
      }, projectName, workItemTypeNames, sortById, excludeIdentities).TryGetValue(fieldId, out strings) ? strings : Enumerable.Empty<string>();
    }

    internal override IDictionary<int, IEnumerable<string>> GetAllowedValues(
      IEnumerable<int> fieldIds,
      string projectName,
      IEnumerable<string> workItemTypeNames,
      bool sortById = false,
      bool excludeIdentities = false)
    {
      bool metadataFilterEnabled = this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).MetadataFilterEnabled;
      this.PrepareStoredProcedure(nameof (GetAllowedValues));
      this.BindUserSid();
      this.BindBoolean("@metadataFilterEnabled", metadataFilterEnabled);
      this.BindInt32Table("@fieldIds", fieldIds);
      this.BindString("@projectName", projectName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@sortById", sortById);
      this.BindBoolean("@excludeIdentities", excludeIdentities);
      if (metadataFilterEnabled)
      {
        IEnumerable<int> projects;
        this.RequestContext.GetService<UserProjectsCache>().GetUserProjects(this.RequestContext, out projects);
        this.BindInt32Table("@userProjects", projects);
      }
      if (!string.IsNullOrEmpty(projectName))
        this.BindStringTable("@workItemTypeNames", workItemTypeNames);
      return (IDictionary<int, IEnumerable<string>>) WorkItemTrackingResourceComponent.Bind<KeyValuePair<int, string>>(this.ExecuteReader(), (System.Func<IDataReader, KeyValuePair<int, string>>) (reader1 => new KeyValuePair<int, string>(reader1.GetInt32(0), reader1.GetString(1)))).GroupBy<KeyValuePair<int, string>, int, string>((System.Func<KeyValuePair<int, string>, int>) (pair => pair.Key), (System.Func<KeyValuePair<int, string>, string>) (pair => pair.Value)).ToDictionary<IGrouping<int, string>, int, IEnumerable<string>>((System.Func<IGrouping<int, string>, int>) (pair => pair.Key), (System.Func<IGrouping<int, string>, IEnumerable<string>>) (pair => pair.AsEnumerable<string>()));
    }
  }
}
