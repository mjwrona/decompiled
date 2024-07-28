// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyCountByScopeBinder
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class PolicyCountByScopeBinder : ObjectBinder<PolicyCountByScopeRecord>
  {
    internal SqlColumnBinder m_scope = new SqlColumnBinder("Scope");
    internal SqlColumnBinder m_policyCount = new SqlColumnBinder("PolicyCount");

    internal PolicyCountByScopeBinder()
    {
    }

    protected override PolicyCountByScopeRecord Bind() => new PolicyCountByScopeRecord(this.m_scope.GetString((IDataReader) this.Reader, false), this.m_policyCount.GetInt32((IDataReader) this.Reader, 0));
  }
}
