// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CsmControllerExceptionHandlerAttribute
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CsmControllerExceptionHandlerAttribute : ExceptionFilterAttribute
  {
    private const string TraceArea = "Commerce";
    private readonly Dictionary<Type, KeyValuePair<HttpStatusCode, string>> exceptionMap;
    private static readonly Guid CommerceServiceInstanceType = new Guid("00000047-0000-8888-8000-000000000000");

    public CsmControllerExceptionHandlerAttribute(int tracePoint)
    {
      this.exceptionMap = this.GetExceptionMap();
      if (this.exceptionMap == null)
        this.exceptionMap = new Dictionary<Type, KeyValuePair<HttpStatusCode, string>>();
      this.TracePoint = tracePoint;
    }

    public Dictionary<Type, KeyValuePair<HttpStatusCode, string>> GetExceptionMap() => new Dictionary<Type, KeyValuePair<HttpStatusCode, string>>()
    {
      {
        typeof (AccountAlreadyLinkedException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "AccountAlreadyLinked")
      },
      {
        typeof (AccountNotLinkedException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, "AccountNotLinked")
      },
      {
        typeof (AccountToBeCreatedAlreadyExistsException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "AccountToBeCreatedAlreadyExistsException")
      },
      {
        typeof (ArgumentException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, "BadArgument")
      },
      {
        typeof (AzureResourceAccountDoesNotExistException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotFound, "ResourceAccountDoesNotExist")
      },
      {
        typeof (AzureResourceAccountMissingException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NoContent, "AzureResourceAccountMissingException")
      },
      {
        typeof (AzureSubscriptionDisabledException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, "AzureSubscriptionDisabled")
      },
      {
        typeof (AzureSubscriptionDoesNotExistException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotFound, "AzureSubscriptionDoesNotExist")
      },
      {
        typeof (AzureSubscriptionNotLinkedToAccountException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "SubscriptionNotLinkedToAccount")
      },
      {
        typeof (CollectionAlreadyExistsException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "CollectionAlreadyExistsException")
      },
      {
        typeof (CollectionCreationLimitsReachedException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "CollectionCreationLimitsReachedException")
      },
      {
        typeof (CollectionNameException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "CollectionNameException")
      },
      {
        typeof (CollectionNotFoundException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotFound, "CollectionNotFoundException")
      },
      {
        typeof (FeatureDisabledException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotFound, "FeatureDisabled")
      },
      {
        typeof (HostDoesNotExistException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotFound, "HostDoesNotExistException")
      },
      {
        typeof (IdentityNotFoundException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, "Unauthorized")
      },
      {
        typeof (InvalidResourceException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, "InvalidResourceException")
      },
      {
        typeof (InvalidOperationException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, "InvalidOperationException")
      },
      {
        typeof (OrganizationBadRequestException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "OrganizationBadRequestException")
      },
      {
        typeof (OrganizationServiceSecurityException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "OrganizationServiceSecurityException")
      },
      {
        typeof (SecurityException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, "Unauthorized")
      },
      {
        typeof (TeamFoundationSecurityServiceException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, "Unauthorized")
      },
      {
        typeof (UnsupportedRegionException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Conflict, "UnsupportedRegionException")
      },
      {
        typeof (UserIsNotAccountOwnerException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, "UserIsNotAccountOwner")
      },
      {
        typeof (VssUnauthorizedException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, "VssUnauthorizedException")
      },
      {
        typeof (UserIsNotSubscriptionAdminException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, "UserIsNotAdmin")
      },
      {
        typeof (NotImplementedException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotImplemented, "NotImplementedException")
      },
      {
        typeof (IdentityMaterializationFailedException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, "IdentityMaterializationFailedException")
      },
      {
        typeof (HostShutdownException),
        new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, "HostShutdownException")
      }
    };

    private int TracePoint { get; }

    public override void OnException(HttpActionExecutedContext actionExecutedContext)
    {
      if ((actionExecutedContext.ActionContext.ControllerContext.Controller is TfsApiController controller ? controller.TfsRequestContext : (IVssRequestContext) null) != null)
      {
        IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
        string controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
        int tracePoint = this.TracePoint;
        string layer = controllerName;
        Exception exception = actionExecutedContext.Exception;
        tfsRequestContext.TraceException(tracePoint, "Commerce", layer, exception);
      }
      actionExecutedContext.Response = this.CreateExceptionResponse(actionExecutedContext);
    }

    private HttpResponseMessage CreateExceptionResponse(
      HttpActionExecutedContext actionExecutedContext)
    {
      Type key = this.exceptionMap.Keys.FirstOrDefault<Type>((Func<Type, bool>) (e => e.IsAssignableFrom(actionExecutedContext.Exception.GetType())));
      KeyValuePair<HttpStatusCode, string> keyValuePair = !(key == (Type) null) ? this.exceptionMap[key] : new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.InternalServerError, "UnknownException");
      var data = new
      {
        Code = keyValuePair.Value,
        Message = keyValuePair.Key == HttpStatusCode.InternalServerError ? "Unknown Exception" : actionExecutedContext.Exception.Message,
        Target = actionExecutedContext.Exception.TargetSite.Name
      };
      return actionExecutedContext.Request.CreateResponse(keyValuePair.Key, data);
    }
  }
}
