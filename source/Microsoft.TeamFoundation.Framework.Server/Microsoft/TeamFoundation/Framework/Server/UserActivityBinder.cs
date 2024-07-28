// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserActivityBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class UserActivityBinder : ObjectBinder<UserActivityEntry>
  {
    private SqlColumnBinder identityNameColumn = new SqlColumnBinder("IdentityName");
    private SqlColumnBinder userAgentColumn = new SqlColumnBinder("UserAgent");
    private SqlColumnBinder activityDateColumn = new SqlColumnBinder("ActivityDate");
    private IVssRequestContext m_requestContext;

    public UserActivityBinder(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    protected override UserActivityEntry Bind() => new UserActivityEntry()
    {
      IdentityName = this.identityNameColumn.GetString((IDataReader) this.Reader, false),
      UserAgent = this.userAgentColumn.GetString((IDataReader) this.Reader, false),
      ActivityDate = this.activityDateColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
