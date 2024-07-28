// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceControllerBase
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
using System.Net;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class CommerceControllerBase : TfsApiController
  {
    internal static Dictionary<Type, HttpStatusCode> httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static CommerceControllerBase()
    {
      CommerceControllerBase.httpExceptions.Add(typeof (AccountAlreadyLinkedException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (AccountNotLinkedException), HttpStatusCode.BadRequest);
      CommerceControllerBase.httpExceptions.Add(typeof (AccountToBeCreatedAlreadyExistsException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (AzureResourceAccountDoesNotExistException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (AzureResourceAccountMissingException), HttpStatusCode.NoContent);
      CommerceControllerBase.httpExceptions.Add(typeof (AzureSubscriptionDisabledException), HttpStatusCode.Forbidden);
      CommerceControllerBase.httpExceptions.Add(typeof (AzureSubscriptionDoesNotExistException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (AzureSubscriptionNotLinkedToAccountException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (CollectionAlreadyExistsException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (CollectionCreationLimitsReachedException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (CollectionNameException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (CollectionNotFoundException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (CommerceInvalidSubscriptionSqlException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (CommerceSecurityException), HttpStatusCode.Unauthorized);
      CommerceControllerBase.httpExceptions.Add(typeof (FeatureDisabledException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (HostDoesNotExistException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (InvalidResourceException), HttpStatusCode.BadRequest);
      CommerceControllerBase.httpExceptions.Add(typeof (InvalidOperationException), HttpStatusCode.BadRequest);
      CommerceControllerBase.httpExceptions.Add(typeof (OrganizationBadRequestException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (OrganizationServiceSecurityException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (RequestCanceledException), HttpStatusCode.RequestTimeout);
      CommerceControllerBase.httpExceptions.Add(typeof (UnsupportedRegionException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (UserContextNullReferenceException), HttpStatusCode.Forbidden);
      CommerceControllerBase.httpExceptions.Add(typeof (UserIsNotAccountOwnerException), HttpStatusCode.Forbidden);
      CommerceControllerBase.httpExceptions.Add(typeof (InvalidUserTypeException), HttpStatusCode.Conflict);
      CommerceControllerBase.httpExceptions.Add(typeof (UserIsNotSubscriptionAdminException), HttpStatusCode.Forbidden);
      CommerceControllerBase.httpExceptions.Add(typeof (OfferMeterNotFoundException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (VssUnauthorizedException), HttpStatusCode.Unauthorized);
      CommerceControllerBase.httpExceptions.Add(typeof (IdentityNotFoundException), HttpStatusCode.NotFound);
      CommerceControllerBase.httpExceptions.Add(typeof (AccessCheckException), HttpStatusCode.Forbidden);
      CommerceControllerBase.httpExceptions.Add(typeof (IdentityMaterializationFailedException), HttpStatusCode.BadRequest);
    }

    public override string ActivityLogArea => "Commerce";

    internal abstract string Layer { get; }

    internal string Area => "Commerce";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) CommerceControllerBase.httpExceptions;
  }
}
