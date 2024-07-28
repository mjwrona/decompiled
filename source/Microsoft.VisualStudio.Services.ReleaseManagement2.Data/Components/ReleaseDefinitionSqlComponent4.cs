// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent4
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseDefinitionSqlComponent4 : ReleaseDefinitionSqlComponent2
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override void SoftDeleteReleaseDefinition(
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId = 0,
      string comment = null,
      bool forceDelete = false)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinition_Delete", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindGuid("modifiedBy", requestorId);
      this.ExecuteNonQuery();
    }
  }
}
