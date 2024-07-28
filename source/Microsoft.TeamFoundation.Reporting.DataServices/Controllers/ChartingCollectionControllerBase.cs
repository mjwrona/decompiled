// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Controllers.ChartingCollectionControllerBase
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Controllers
{
  public abstract class ChartingCollectionControllerBase : TfsApiController
  {
    protected static readonly Dictionary<Type, HttpStatusCode> s_CommonHttpExceptions = ControllerHelpers.getCommonHttpExceptions();

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ChartingCollectionControllerBase.s_CommonHttpExceptions;

    public override string ActivityLogArea => "Reporting";
  }
}
