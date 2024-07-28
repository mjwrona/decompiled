// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalUpdateQueryItemHashElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalUpdateQueryItemHashElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      ServerQueryItem item,
      IVssRequestContext requestContext)
    {
      this.SetOutputs(0);
      this.SetGroup(group);
      object obj = string.IsNullOrEmpty(item.New.QueryText) ? DalSqlElement.SQL_NULL : (object) CommonWITUtils.GetSha1HashString(CommonWITUtils.NormalizeWiql(WiqlTransformUtils.TransformNamesToIds(requestContext, item.New.QueryText, false)));
      this.AppendExecProc("prc_UpdateQueryItemHash", DalSqlElement.InlineInt(this.SqlBatch.RequestContext.ServiceHost.PartitionId), DalSqlElement.InlineGuid(item.Id), this.Param(obj));
    }
  }
}
