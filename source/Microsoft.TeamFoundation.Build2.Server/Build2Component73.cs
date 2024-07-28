// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component73
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component73 : Build2Component72
  {
    public override List<BuildDefinition> GetYamlDefinitionsForRepository(
      HashSet<int> dataspaceIds,
      string repositoryId,
      string repositoryType,
      int maxDefinitions)
    {
      using (this.TraceScope(method: nameof (GetYamlDefinitionsForRepository)))
      {
        this.PrepareStoredProcedure("Build.prc_GetYamlDefinitionsForRepository");
        this.BindUniqueInt32Table("@dataspaceIdTable", (IEnumerable<int>) dataspaceIds);
        this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindInt("@maxDefinitions", maxDefinitions);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
          rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
          rc.AddBinder<DesignerSchedule>(this.GetDesignerScheduleBinder());
          List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
          Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
          rc.NextResult();
          foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>())
          {
            BuildDefinition buildDefinition;
            if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
              buildDefinition.Tags.Add(definitionTagData.Tag);
          }
          this.UpdateScheduleTriggers(rc, dictionary);
          return items;
        }
      }
    }
  }
}
