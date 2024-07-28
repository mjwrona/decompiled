// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent12
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent12 : DalSqlResourceComponent11
  {
    protected override void BindIdentityCategory(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      int bisIdentityType = (int) IdentityConstantsNormalizer.GetBisIdentityType((IVssIdentity) identity);
      int parameterValue = (int) Enum.Parse(typeof (GroupSpecialType), identity.GetProperty<string>("SpecialType", string.Empty));
      this.BindInt("@objectCategory", bisIdentityType);
      this.BindInt("@objectSpecialType", parameterValue);
    }

    protected override void BindCollectionAndAccountHostIds()
    {
      if (this.RequestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      this.BindString("@collectionHostId", this.RequestContext.ServiceHost.InstanceId.ToString(), 256, false, SqlDbType.NVarChar);
      this.BindString("@accountHostId", this.RequestContext.ServiceHost.ParentServiceHost.InstanceId.ToString(), 256, false, SqlDbType.NVarChar);
    }
  }
}
