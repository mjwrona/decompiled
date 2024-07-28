// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.OnDataspaceCreationComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class OnDataspaceCreationComponent : TeamFoundationSqlResourceComponent
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<OnDataspaceCreationComponent>(1, true)
    }, "ReleaseManagementDataspaceCreation", "ReleaseManagement");

    public OnDataspaceCreationComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dataspace", Justification = "dataspace is a valid term here.")]
    public virtual void PopulateCounters(int dataspaceId)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseCounters_Populate");
      this.BindInt("DataspaceId", dataspaceId);
      this.ExecuteNonQuery();
    }

    public virtual void CreateResourcesForNewDataspace(int dataspaceId)
    {
      this.PrepareStoredProcedure("Release.prc_CreateResourcesForNewDataspace");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.ExecuteNonQuery();
    }
  }
}
