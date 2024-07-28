// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalAddIdentityElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalAddIdentityElement : DalSqlElement
  {
    public void JoinBatch(ElementGroup group, Microsoft.VisualStudio.Services.Identity.Identity identity, bool enableTfidRewriting)
    {
      this.m_elementGroup = group;
      this.m_outputs = 0;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql(" exec ");
      this.AppendSql("@fRollback");
      this.AppendSql(" = dbo.[");
      this.AppendSql("SyncWithBIS_SyncIdentity");
      this.AppendSql("] ");
      this.AppendPartitionIdVariable();
      this.AppendSql(this.SqlBatch.AddParameterUniqueIdentifier(identity.Id));
      this.AppendSql(",");
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(identity.Descriptor.Identifier));
      this.AppendSql(",");
      this.AppendSql(DalSqlElement.InlineInt((int) IdentityConstantsNormalizer.GetBisIdentityType((IVssIdentity) identity)));
      this.AppendSql(",");
      this.AppendSql(DalSqlElement.InlineInt((int) Enum.Parse(typeof (GroupSpecialType), identity.GetProperty<string>("SpecialType", string.Empty))));
      this.AppendSql(",");
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(identity.GetProperty<string>("Domain", string.Empty)));
      this.AppendSql(",");
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(identity.GetProperty<string>("Account", string.Empty)));
      this.AppendSql(",");
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(identity.DisplayName));
      this.AppendSql(",");
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(identity.GetProperty<string>("Mail", string.Empty)));
      this.AppendSql(",");
      this.AppendSql(this.SqlBatch.AddParameterTable<string>((WorkItemTrackingTableValueParameter<string>) new IdentityMembershipTable((IEnumerable<string>) ((IEnumerable<IdentityDescriptor>) identity.Members ?? Enumerable.Empty<IdentityDescriptor>()).Select<IdentityDescriptor, string>((Func<IdentityDescriptor, string>) (item => item.Identifier)).ToList<string>())));
      this.AppendSql(",");
      this.AppendSql("@SyncTime");
      this.AppendSql(",");
      this.AppendSql(DalSqlElement.InlineBit(!identity.IsActive));
      if (this.Version >= 39)
      {
        this.AppendSql(",");
        this.AppendSql(this.SqlBatch.AddParameterInt(enableTfidRewriting ? 1 : 0));
      }
      this.AppendSql("; if (");
      this.AppendSql("@fRollback");
      this.AppendSql(" <> 0) return");
      this.AppendSql(Environment.NewLine);
    }
  }
}
