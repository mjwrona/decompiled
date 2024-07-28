// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProjectionBinder`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ProjectionBinder<T> : ObjectBinder<T>
  {
    private readonly Func<SqlDataReader, T> m_projection;

    public ProjectionBinder(Func<SqlDataReader, T> projection)
    {
      ArgumentUtility.CheckForNull<Func<SqlDataReader, T>>(projection, nameof (projection));
      this.m_projection = projection;
    }

    protected override T Bind() => this.m_projection(this.Reader);
  }
}
