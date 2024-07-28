// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationComponent7 : LocationComponent6
  {
    protected override void RemoveServiceDefinitions(IEnumerable<ServiceDefinition> definitions) => this.RemoveServiceDefinitions(definitions, false);

    public override void RemoveServiceDefinitions(
      IEnumerable<ServiceDefinition> definitions,
      bool fullMatch)
    {
      this.PrepareStoredProcedure("prc_RemoveServiceDefinitions");
      this.BindServiceDefinitionTable("@definition", definitions, !fullMatch);
      this.BindGuid("@writerIdentifier", this.Author);
      this.BindBoolean("@requireFullMatch", fullMatch);
      this.ExecuteNonQuery();
    }

    public override ServiceDefinition QueryServiceDefinition(string serviceType, Guid identifier)
    {
      this.PrepareStoredProcedure("prc_QueryServiceDefinition");
      this.BindString("@serviceType", serviceType, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@identifier", identifier);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceDefinition", this.RequestContext);
      resultCollection.AddBinder<ServiceDefinition>((ObjectBinder<ServiceDefinition>) new ServiceDefinitionDataColumns2());
      return resultCollection.GetCurrent<ServiceDefinition>().Items.FirstOrDefault<ServiceDefinition>();
    }
  }
}
