// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardPositionCollisionException
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [Obsolete("This exception has been deprecated. Please remove all references.", false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public class DashboardPositionCollisionException : TeamFoundationServerException
  {
    public DashboardPositionCollisionException()
      : base(DashboardResources.ErrorDashboardPositionCollision, (Exception) null)
    {
    }

    protected DashboardPositionCollisionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
