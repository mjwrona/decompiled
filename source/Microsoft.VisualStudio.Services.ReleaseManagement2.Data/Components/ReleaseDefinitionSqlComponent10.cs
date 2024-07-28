// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent10
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseDefinitionSqlComponent10 : ReleaseDefinitionSqlComponent9
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override ReleaseDefinition GetReleaseDefinition(
      Guid projectId,
      int releaseDefinitionId,
      bool includeDeleted = false,
      bool isDefaultToLatestArtifactVersionEnabled = false,
      bool includeLastRelease = false)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseDefinition", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindBoolean(nameof (includeDeleted), includeDeleted);
      return this.GetReleaseDefinitionObject(isDefaultToLatestArtifactVersionEnabled, includeLastRelease);
    }

    protected override void BindIsDeletedFilter(bool isDeletedFilter) => this.BindBoolean("isDeleted", isDeletedFilter);

    protected override void BindIncludeTriggers(bool includeTriggers) => this.BindBoolean(nameof (includeTriggers), includeTriggers);

    protected override void BindMaxModifiedTime(DateTime? modifiedTime) => this.BindNullableDateTime("maxModifiedTime", modifiedTime);

    protected override void AddTriggersBinder(ResultCollection resultCollection)
    {
      if (resultCollection == null)
        throw new ArgumentNullException(nameof (resultCollection));
      resultCollection.AddBinder<ReleaseTriggerBase>((ObjectBinder<ReleaseTriggerBase>) this.GetReleaseTriggerBinder());
    }

    protected override IEnumerable<ReleaseTriggerBase> GetReleaseTriggers(
      ResultCollection resultCollection)
    {
      if (resultCollection == null)
        throw new ArgumentNullException(nameof (resultCollection));
      resultCollection.NextResult();
      return (IEnumerable<ReleaseTriggerBase>) resultCollection.GetCurrent<ReleaseTriggerBase>().Items;
    }
  }
}
