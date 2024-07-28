// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmConsumersController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [VersionedApiControllerCustomName(Area = "RM", ResourceName = "consumers")]
  [ClientInternalUseOnly(false)]
  public class RmConsumersController : TfsApiController
  {
    [StaticSafe]
    private static readonly IDictionary<Type, HttpStatusCode> HttpExceptionsMap = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ExecuteServiceEndpointRequestFailedException),
        HttpStatusCode.BadRequest
      }
    };

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is a REST API.")]
    public IEnumerable<string> GetConsumers() => this.TfsRequestContext.GetService<ConsumerService>().QueryConsumers(this.TfsRequestContext);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is a REST API.")]
    public string GetConsumer(string consumerId) => "Call to GetConsumer with ConsumerId " + consumerId;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => RmConsumersController.HttpExceptionsMap;

    public override string ActivityLogArea => "ReleaseManagement";
  }
}
