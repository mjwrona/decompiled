// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.UpgradeEnvironmentComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class UpgradeEnvironmentComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<UpgradeEnvironmentComponent>(1)
    }, "UpgradeEnvironment", "ReleaseManagement");

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    public void UpdateEnvironmentsWithQueues(
      Guid projectId,
      IEnumerable<KeyValuePair<int, int>> definitionEnvironmentIdsToQueueIdMap,
      IEnumerable<KeyValuePair<int, int>> releaseEnvironmentIdsToQueueIdMap)
    {
      if (definitionEnvironmentIdsToQueueIdMap == null)
        throw new ArgumentNullException(nameof (definitionEnvironmentIdsToQueueIdMap));
      if (releaseEnvironmentIdsToQueueIdMap == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentIdsToQueueIdMap));
      this.PrepareStoredProcedure("Release.prc_UpdateEnvironmentsToQueues", projectId);
      this.BindKeyValuePairInt32Int32Table("definitionEnvironmentToQueueMap", definitionEnvironmentIdsToQueueIdMap);
      this.BindKeyValuePairInt32Int32Table("releaseEnvironmentToQueueMap", releaseEnvironmentIdsToQueueIdMap);
      this.ExecuteNonQuery();
    }
  }
}
