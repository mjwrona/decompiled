// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PolicyFailureColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PolicyFailureColumns : VersionControlObjectBinder<PolicyFailureInfo>
  {
    private SqlColumnBinder changesetId = new SqlColumnBinder("ChangeSetId");
    private SqlColumnBinder policyName = new SqlColumnBinder("PolicyName");
    private SqlColumnBinder message = new SqlColumnBinder("Message");

    protected override PolicyFailureInfo Bind() => new PolicyFailureInfo()
    {
      changesetId = this.changesetId.GetInt32((IDataReader) this.Reader),
      PolicyName = this.policyName.GetString((IDataReader) this.Reader, false),
      Message = this.message.GetString((IDataReader) this.Reader, false)
    };
  }
}
