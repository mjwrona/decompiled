// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSetQueryItemAclElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSetQueryItemAclElement : DalQueryItemElement
  {
    public void JoinBatch(
      ElementGroup group,
      ServerQueryItem item,
      ExtendedAccessControlListData aclData)
    {
      this.SetOutputs(0);
      this.SetGroup(group);
      StringBuilder stringBuilder = new StringBuilder();
      List<string> tokens = new List<string>();
      tokens.Add(item.SecurityToken);
      List<DalAccessControlEntry> permissions = new List<DalAccessControlEntry>();
      foreach (AccessControlEntryData permission in aclData.Permissions)
        permissions.Add(new DalAccessControlEntry(permission.TeamFoundationId, item.SecurityToken, permission.Allow, permission.Deny));
      Func<string, int> dataspaceMapper = (Func<string, int>) (token => this.GetDataspaceIdForSecurityToken(this.SqlBatch.Component, token));
      string str1 = this.SqlBatch.AddParameterTable<string>((WorkItemTrackingTableValueParameter<string>) new TokenTable2(dataspaceMapper, (IEnumerable<string>) tokens, aclData.InheritPermissions, QueryItemSecurityConstants.PathSeparator));
      string str2 = this.SqlBatch.AddParameterTable<DalAccessControlEntry>((WorkItemTrackingTableValueParameter<DalAccessControlEntry>) new PermissionTable2(dataspaceMapper, (string) null, QueryItemSecurityConstants.PathSeparator, (IEnumerable<DalAccessControlEntry>) permissions));
      this.AppendExecProc("prc_pSetAccessControlLists2", DalSqlElement.InlineInt(this.SqlBatch.RequestContext.ServiceHost.PartitionId), DalSqlElement.InlineGuid(QueryItemSecurityConstants.NamespaceGuid), str2, str1, DalSqlElement.InlineBit(false), DalSqlElement.InlineBit(true), this.Param((object) QueryItemSecurityConstants.PathSeparator.ToString()));
    }
  }
}
