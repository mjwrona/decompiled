// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationComponent3 : LocationComponent2
  {
    public override bool SaveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.PrepareStoredProcedure("prc_SaveServiceDefinitions2");
      this.BindServiceHostId(false);
      this.BindServiceDefinitionTable("@definition", serviceDefinitions);
      this.BindLocationMappingTable("@locationMapping", serviceDefinitions);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
      return true;
    }

    protected override void RemoveServiceDefinitions(IEnumerable<ServiceDefinition> definitions)
    {
      this.PrepareStoredProcedure("prc_RemoveServiceDefinitions2");
      this.BindServiceHostId(false);
      this.BindServiceDefinitionTable("@definition", definitions, true);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    protected override void BindServiceHostId(bool alwaysBind)
    {
      if (alwaysBind)
      {
        this.BindGuid("@hostId", this.ServiceHostId);
      }
      else
      {
        Guid parameterValue = Guid.Empty;
        if (this.ServiceHostId != Guid.Empty && this.RequestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application))
          parameterValue = this.ServiceHostId;
        this.BindGuid("@hostId", parameterValue);
      }
    }
  }
}
