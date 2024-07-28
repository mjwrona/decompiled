// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent3
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent3 : OrchestrationComponent2
  {
    public override OrchestrationHubDescription CreateHub(OrchestrationHubDescription description)
    {
      this.PrepareStoredProcedure("prc_AddOrchestrationHub");
      this.BindString("@hubType", description.HubType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@hubName", description.HubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindByte("@compressionStyle", (byte) description.CompressionSettings.Style);
      this.BindNullableInt("@compressionThreshold", description.CompressionSettings.ThresholdInBytes, 0);
      this.BindInt("@maxConcurrentActivities", description.MaxConcurrentActivities);
      this.BindInt("@maxConcurrentOrchestrations", description.MaxConcurrentOrchestrations);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationHubDescription>((ObjectBinder<OrchestrationHubDescription>) new OrchestrationHubDescriptionBinder2());
        return resultCollection.GetCurrent<OrchestrationHubDescription>().First<OrchestrationHubDescription>();
      }
    }

    public override IEnumerable<OrchestrationHubDescription> GetHubs(string hubName)
    {
      this.PrepareStoredProcedure("prc_GetOrchestrationHub");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationHubDescription>((ObjectBinder<OrchestrationHubDescription>) new OrchestrationHubDescriptionBinder2());
        return (IEnumerable<OrchestrationHubDescription>) resultCollection.GetCurrent<OrchestrationHubDescription>().Items;
      }
    }

    public override OrchestrationHubDescription UpdateHub(string hubName, string newHubName)
    {
      this.PrepareStoredProcedure("prc_UpdateOrchestrationHub");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@newHubName", newHubName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationHubDescription>((ObjectBinder<OrchestrationHubDescription>) new OrchestrationHubDescriptionBinder2());
        return resultCollection.GetCurrent<OrchestrationHubDescription>().FirstOrDefault<OrchestrationHubDescription>();
      }
    }
  }
}
