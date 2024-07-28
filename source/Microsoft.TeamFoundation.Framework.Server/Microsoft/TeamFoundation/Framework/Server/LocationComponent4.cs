// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationComponent4 : LocationComponent3
  {
    public LocationComponent4() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override bool SaveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.PrepareStoredProcedure("prc_SaveServiceDefinitions");
      this.BindServiceDefinitionTable("@definition", serviceDefinitions);
      this.BindLocationMappingTable("@locationMapping", serviceDefinitions);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
      return true;
    }

    protected override void RemoveServiceDefinitions(IEnumerable<ServiceDefinition> definitions)
    {
      this.PrepareStoredProcedure("prc_RemoveServiceDefinitions");
      this.BindServiceDefinitionTable("@definition", definitions, true);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    protected override void BindServiceHostId(bool alwaysBind)
    {
    }
  }
}
