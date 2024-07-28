// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildInformation2010Component3
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal class BuildInformation2010Component3 : BuildInformation2010Component2
  {
    public BuildInformation2010Component3() => this.ServiceVersion = ServiceVersion.V3;

    internal override ResultCollection QueryBuildInformation(
      IEnumerable<BuildInfo> builds,
      IList<string> informationTypes)
    {
      this.TraceEnter(0, nameof (QueryBuildInformation));
      this.PrepareStoredProcedure("prc_QueryBuildInformation2");
      this.BindTable<KeyValuePair<int, string>>("@buildUriTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) BuildSqlResourceComponent.UrisToOrderedStringTable(builds.Select<BuildInfo, string>((System.Func<BuildInfo, string>) (x => x.Uri))));
      if (informationTypes.Count > 1 || informationTypes[0] != BuildConstants.Star)
      {
        this.BindTable<string>("@informationTypeTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) informationTypes, false, 256));
        this.BindBoolean("@allTypes", false);
      }
      else
      {
        this.BindTable<string>("@informationTypeTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) null));
        this.BindBoolean("@allTypes", true);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildInformationNode2010>((ObjectBinder<BuildInformationNode2010>) new BuildInformationNode2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryBuildInformation));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildInformationWithConversion(
      IEnumerable<string> uris,
      IList<string> informationTypes)
    {
      this.TraceEnter(0, nameof (QueryBuildInformationWithConversion));
      this.PrepareStoredProcedure("prc_QueryBuildInformationWithConversion2");
      this.BindTable<KeyValuePair<int, string>>("@buildUriTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) BuildSqlResourceComponent.UrisToOrderedStringTable(uris));
      if (informationTypes.Count > 1 || informationTypes[0] != BuildConstants.Star)
      {
        this.BindTable<string>("@informationTypeTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) informationTypes, false, 128));
        this.BindBoolean("@allTypes", false);
      }
      else
      {
        this.BindTable<string>("@informationTypeTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) null));
        this.BindBoolean("@allTypes", true);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildInformationNode2010>((ObjectBinder<BuildInformationNode2010>) new BuildInformationNode2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryBuildInformationWithConversion));
      return resultCollection;
    }
  }
}
