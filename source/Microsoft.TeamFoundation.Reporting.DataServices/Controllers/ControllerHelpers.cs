// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Controllers.ControllerHelpers
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Controllers
{
  internal class ControllerHelpers
  {
    public static Dictionary<Type, HttpStatusCode> getCommonHttpExceptions() => new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      }
    };

    public static Dictionary<Type, HttpStatusCode> getTransformHttpExceptions() => new Dictionary<Type, HttpStatusCode>((IDictionary<Type, HttpStatusCode>) ControllerHelpers.getCommonHttpExceptions())
    {
      {
        typeof (InvalidTransformOptionsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ProjectDoesNotExistWithNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ChartDataExceedsAllowedLimitsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TransformFilterValueNotFound),
        HttpStatusCode.NotFound
      },
      {
        typeof (ChartScopeProviderNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ChartProviderNotEnabledException),
        HttpStatusCode.NotFound
      },
      {
        typeof (TooManyOptionsPerTransformException),
        HttpStatusCode.BadRequest
      }
    };
  }
}
