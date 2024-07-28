// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiNotFoundException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class KpiNotFoundException : TeamFoundationServiceException
  {
    public KpiNotFoundException()
    {
    }

    public KpiNotFoundException(string area, string name, string scope)
      : base(FrameworkResources.KpiNotFoundException((object) area, (object) name, (object) scope))
    {
    }

    public KpiNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "area"), TeamFoundationServiceException.ExtractString(sqlError, "name"), TeamFoundationServiceException.ExtractString(sqlError, "scope"))
    {
    }

    protected KpiNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
