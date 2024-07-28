// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.IQueryProvisioningHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IQueryProvisioningHelper
  {
    IEnumerable<Guid> GetDirtyQueryItems();

    bool IsQueryDeleted(Guid id);

    bool IsQueryNew(Guid id);

    bool IsQueryDirtyShallow(Guid id);

    string GetOwnerIdentifier(Guid id, bool onlyIfChanged = false);

    string GetIdentityType(Guid id, bool onlyIfChanged = false);

    string GetQueryText(Guid id, bool onlyIfChanged = false);

    Guid GetParentId(Guid id, bool onlyIfChanged = false);

    string GetName(Guid id, bool onlyIfChanged = false);

    IEnumerable<QueryAccessControlEntry> GetAccessControlEntries(Guid id);

    bool IsAccessControlListDirty(Guid id);

    bool GetInheritPermissions(Guid id);
  }
}
