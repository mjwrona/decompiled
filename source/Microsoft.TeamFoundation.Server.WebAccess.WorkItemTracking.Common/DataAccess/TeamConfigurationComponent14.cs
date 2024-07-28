// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent14
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent14 : TeamConfigurationComponent13
  {
    internal override IDictionary<Guid, IDictionary<string, bool>> GetTeamBacklogVisibilitiesForProject(
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetTeamConfigurationProperties");
      this.BindDataspaceId(projectId);
      this.BindString("@propertyName", "BacklogVisibilities", 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      IList<TeamConfigurationPropertyRow> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamConfigurationPropertyRow>((ObjectBinder<TeamConfigurationPropertyRow>) new FullTeamConfigurationPropertyRowBinder());
        items = (IList<TeamConfigurationPropertyRow>) resultCollection.GetCurrent<TeamConfigurationPropertyRow>().Items;
      }
      IDictionary<Guid, IDictionary<string, bool>> visibilitiesForProject = (IDictionary<Guid, IDictionary<string, bool>>) new Dictionary<Guid, IDictionary<string, bool>>();
      foreach (TeamConfigurationPropertyRow row in (IEnumerable<TeamConfigurationPropertyRow>) items)
      {
        if (!visibilitiesForProject.ContainsKey(row.TeamId))
        {
          IDictionary<string, bool> settingsProperty = (IDictionary<string, bool>) new Dictionary<string, bool>();
          TeamConfigurationComponent.DeserializeAndFillTeamSettingsProperty<bool>(settingsProperty, row);
          visibilitiesForProject[row.TeamId] = settingsProperty;
        }
      }
      return visibilitiesForProject;
    }
  }
}
