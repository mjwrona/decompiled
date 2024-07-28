// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildInformationComponent2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildInformationComponent2 : BuildInformationComponent
  {
    public BuildInformationComponent2() => this.ServiceVersion = ServiceVersion.V2;

    internal override ResultCollection UpdateBuildInformation(
      Guid projectId,
      string buildUri,
      ICollection<InformationChangeRequest> changes,
      TeamFoundationIdentity requestedBy)
    {
      this.TraceEnter(0, nameof (UpdateBuildInformation));
      this.PrepareStoredProcedure("prc_UpdateBuildInformation");
      this.BindUri("@buildUri", DBHelper.ExtractDbId(buildUri), false);
      this.BindBuildInformationChangeRequestTable("@informationChangeRequestTable", (IEnumerable<InformationChangeRequest>) changes);
      List<InformationField> rows = new List<InformationField>();
      foreach (InformationChangeRequest change in (IEnumerable<InformationChangeRequest>) changes)
      {
        InformationAddRequest informationAddRequest = change as InformationAddRequest;
        InformationEditRequest informationEditRequest = change as InformationEditRequest;
        if (informationAddRequest != null)
          rows.AddRange((IEnumerable<InformationField>) informationAddRequest.Fields);
        else if (informationEditRequest != null)
          rows.AddRange((IEnumerable<InformationField>) informationEditRequest.Fields);
      }
      this.BindBuildInformationFieldTable("@informationFieldTable", (IEnumerable<InformationField>) rows);
      this.BindIdentity("@requestedBy", requestedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildInformationNode>((ObjectBinder<BuildInformationNode>) new BuildInformationBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (UpdateBuildInformation));
      return resultCollection;
    }
  }
}
