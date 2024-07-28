// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ServerQueryEngine
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ServerQueryEngine : QueryEngine
  {
    private TestManagementRequestContext m_requestContext;

    internal ServerQueryEngine(TestManagementRequestContext context, ResultsStoreQuery query)
      : this(context, query, (List<Tuple<Type, string, string>>) null)
    {
    }

    internal ServerQueryEngine(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      List<Tuple<Type, string, string>> tables)
      : this(context, query, (Func<string, string, string>) null, tables)
    {
    }

    internal ServerQueryEngine(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      Func<string, string, string> conditionNodeValueTranslator,
      List<Tuple<Type, string, string>> tables)
      : base(query, context.UserTeamFoundationId.ToString(), conditionNodeValueTranslator, context.RequestContext.ServiceName, tables)
    {
      this.m_requestContext = context;
      this.Prepare();
      this.m_requestContext.TestManagementHost.Replicator.UpdateCss(this.m_requestContext);
    }

    protected override string GenerateWhereClause(bool isMultipleProjects, int dataspaceId = 0)
    {
      string empty = string.Empty;
      string whereClause = base.GenerateWhereClause(isMultipleProjects, dataspaceId);
      return string.IsNullOrWhiteSpace(whereClause) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WHERE PartitionId={0}", (object) this.m_requestContext.RequestContext.ServiceHost.PartitionId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} AND PartitionId={1}", (object) whereClause, (object) this.m_requestContext.RequestContext.ServiceHost.PartitionId);
    }

    protected override string[] GetGroupMembership(string groupId)
    {
      this.m_requestContext.TraceVerbose("QueryEngine", "GetGroupMembership started - {0}", (object) groupId);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> parameter1 = this.m_requestContext.IdentityService.ReadIdentities(this.m_requestContext.RequestContext, (IList<Guid>) new Guid[1]
      {
        new Guid(groupId)
      }, QueryMembership.Expanded, (IEnumerable<string>) null);
      this.m_requestContext.IfNullThenTraceAndDebugFail("QueryEngine", (object) parameter1, "groups");
      if (parameter1[0] == null)
        return Array.Empty<string>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> parameter2 = this.m_requestContext.IdentityService.ReadIdentities(this.m_requestContext.RequestContext, (IList<IdentityDescriptor>) parameter1[0].Members.ToArray<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null);
      this.m_requestContext.IfNullThenTraceAndDebugFail("QueryEngine", (object) parameter2, "members");
      string[] groupMembership = new string[parameter2.Count];
      for (int index = 0; index < parameter2.Count; ++index)
      {
        if (parameter2[index] != null)
          groupMembership[index] = parameter2[index].Id.ToString();
      }
      this.m_requestContext.TraceVerbose("QueryEngine", "GetGroupMembership ended");
      return groupMembership;
    }
  }
}
