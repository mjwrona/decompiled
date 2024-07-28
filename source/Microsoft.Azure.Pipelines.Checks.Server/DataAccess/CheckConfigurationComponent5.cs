// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckConfigurationComponent5
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckConfigurationComponent5 : CheckConfigurationComponent4
  {
    public override List<CheckConfiguration> GetCheckConfigurationsOnResources(
      List<Resource> resources,
      bool includeDisabledChecks,
      bool includeSettings = false,
      bool includeDeletedChecks = false)
    {
      IEnumerable<string> rows = resources.Select<Resource, string>((System.Func<Resource, string>) (resource => resource.GetScopeString()));
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckConfigurationsOnResources)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetPolicyAssignmentsByScopes");
        this.BindStringTable("@scopesTable", rows, true, 256);
        this.BindBoolean("@includeSettings", includeSettings);
        this.BindBoolean("@includeDeletedChecks", includeDeletedChecks);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().Items;
        }
      }
    }
  }
}
