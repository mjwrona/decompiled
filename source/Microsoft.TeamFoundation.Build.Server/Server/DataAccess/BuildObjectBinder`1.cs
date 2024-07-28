// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildObjectBinder`1
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal abstract class BuildObjectBinder<T> : ObjectBinder<T>
  {
    private BuildIdentityResolver m_identityResolver;
    private BuildSqlResourceComponent m_component;

    public BuildObjectBinder() => this.m_identityResolver = new BuildIdentityResolver();

    public BuildObjectBinder(BuildSqlResourceComponent component)
    {
      this.m_component = component;
      this.m_identityResolver = new BuildIdentityResolver();
    }

    public BuildObjectBinder(SqlDataReader dataReader, string procedureName)
      : base(dataReader, procedureName)
    {
      this.m_identityResolver = new BuildIdentityResolver();
    }

    protected string GetUniqueName(IVssRequestContext requestContext, string identityValue) => this.GetUniqueName(requestContext, identityValue, out string _);

    protected string GetUniqueName(
      IVssRequestContext requestContext,
      string identityValue,
      out string displayName)
    {
      return this.GetUniqueName(requestContext, identityValue, out displayName, out TeamFoundationIdentity _);
    }

    protected string GetUniqueName(
      IVssRequestContext requestContext,
      string identityValue,
      out string displayName,
      out TeamFoundationIdentity identity)
    {
      return this.m_component.GetUniqueName(requestContext, this.m_identityResolver, identityValue, out displayName, out identity);
    }

    protected BuildSqlResourceComponent Component => this.m_component;
  }
}
