// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LeaseLostException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class LeaseLostException : TeamFoundationServiceException
  {
    public LeaseLostException(string leaseName, Guid leaseOwner)
      : base(FrameworkResources.LeaseLostException((object) leaseName, (object) leaseOwner))
    {
    }

    public LeaseLostException(
      string leaseName,
      Guid leaseOwner,
      DateTime leaseObtained,
      Guid processId,
      DateTime leaseExpires)
      : base(FrameworkResources.LeaseExpiredException((object) leaseName, (object) leaseOwner, (object) leaseObtained, (object) processId, (object) leaseExpires))
    {
    }

    public LeaseLostException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(FrameworkResources.LeaseLostException((object) TeamFoundationServiceException.ExtractString(sqlError, "name"), (object) TeamFoundationServiceException.ExtractString(sqlError, "owner")))
    {
    }
  }
}
