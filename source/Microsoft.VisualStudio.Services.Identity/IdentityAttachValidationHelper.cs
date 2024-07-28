// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityAttachValidationHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityAttachValidationHelper
  {
    public List<Microsoft.VisualStudio.Services.Identity.Identity> GetSnapshotIdentities(
      ISqlConnectionInfo connectionInfo,
      Guid originalCollectionId,
      string identityType,
      bool readAllIdentities)
    {
      int num = 0;
      DatabasePartitionComponent component = (DatabasePartitionComponent) null;
      if (TeamFoundationResourceManagementService.TryCreateComponentRaw<DatabasePartitionComponent>(connectionInfo, 300, 100, 20, out component, true))
      {
        num = component.QueryOnlyPartition().PartitionId;
        component.Dispose();
      }
      List<Guid> items1;
      using (GroupComponent componentRaw = connectionInfo.CreateComponentRaw<GroupComponent>())
      {
        componentRaw.PartitionId = num;
        using (ResultCollection resultCollection = componentRaw.ReadSnapshot(originalCollectionId, readAllIdentities))
        {
          List<IdentityScope> items2 = resultCollection.GetCurrent<IdentityScope>().Items;
          resultCollection.NextResult();
          List<Microsoft.VisualStudio.Services.Identity.Identity> items3 = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
          resultCollection.NextResult();
          List<GroupMembership> items4 = resultCollection.GetCurrent<GroupMembership>().Items;
          resultCollection.NextResult();
          items1 = resultCollection.GetCurrent<Guid>().Items;
        }
      }
      List<Microsoft.VisualStudio.Services.Identity.Identity> snapshotIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      using (IdentityManagementComponent componentRaw = connectionInfo.CreateComponentRaw<IdentityManagementComponent>())
      {
        componentRaw.PartitionId = num;
        using (ResultCollection resultCollection = componentRaw.ReadIdentities((IEnumerable<IdentityDescriptor>) null, (IEnumerable<Guid>) items1))
        {
          foreach (IdentityManagementComponent.IdentityData identityData in resultCollection.GetCurrent<IdentityManagementComponent.IdentityData>())
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityData.Identity;
            if (identity != null && string.Equals(identity.Descriptor.IdentityType, identityType, StringComparison.OrdinalIgnoreCase))
              snapshotIdentities.Add(identity);
          }
        }
      }
      return snapshotIdentities;
    }
  }
}
