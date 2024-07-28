// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.AdministrationComponent6
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class AdministrationComponent6 : AdministrationComponent5
  {
    public AdministrationComponent6()
    {
      this.ServiceVersion = ServiceVersion.V6;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection AddBuildControllers(IList<BuildController> controllers)
    {
      this.TraceEnter(0, nameof (AddBuildControllers));
      this.PrepareStoredProcedure("prc_AddBuildControllers");
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      for (int index = 0; index < controllers.Count; ++index)
      {
        controllers[index].Uri = DBHelper.CreateArtifactUri("Controller", index);
        this.Trace(0, TraceLevel.Info, "Created uri for controller '{0}'", (object) controllers[index].Uri);
        properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateEmptyArtifactSpec(BuildPropertyKinds.BuildController, index), (IEnumerable<PropertyValue>) controllers[index].Properties));
      }
      this.BindTable<BuildController>("@buildControllerTable", (TeamFoundationTableValueParameter<BuildController>) new BuildControllerTable2((IEnumerable<BuildController>) controllers));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      this.TraceLeave(0, nameof (AddBuildControllers));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuildControllers(
      IList<BuildControllerUpdateOptions> updates)
    {
      this.TraceEnter(0, nameof (UpdateBuildControllers));
      this.PrepareStoredProcedure("prc_UpdateBuildControllers");
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      foreach (BuildControllerUpdateOptions update in (IEnumerable<BuildControllerUpdateOptions>) updates)
      {
        if ((update.Fields & BuildControllerUpdate.AttachedProperties) == BuildControllerUpdate.AttachedProperties)
          properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateArtifactSpec(update.Uri), (IEnumerable<PropertyValue>) update.AttachedProperties));
      }
      this.BindTable<BuildControllerUpdateOptions>("@buildControllerTable", (TeamFoundationTableValueParameter<BuildControllerUpdateOptions>) new BuildControllerUpdateTable2((IEnumerable<BuildControllerUpdateOptions>) updates));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildControllers));
      return resultCollection;
    }
  }
}
