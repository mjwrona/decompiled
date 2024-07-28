// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.PermissionsControllerBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  public abstract class PermissionsControllerBase : TfsApiController
  {
    [NonAction]
    public IEnumerable<bool> HasPermission(
      Guid securityNamespaceId,
      IEnumerable<string> tokens,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tokens, nameof (tokens));
      IVssSecurityNamespace securityNamespace = this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId);
      if (securityNamespace == null)
        throw new InvalidSecurityNamespaceException(securityNamespaceId);
      IRequestActor userActor = this.TfsRequestContext.GetUserActor();
      IEnumerable<IRequestActor> actors;
      if (userActor == null)
        actors = Enumerable.Empty<IRequestActor>();
      else
        actors = (IEnumerable<IRequestActor>) new IRequestActor[1]
        {
          userActor
        };
      return securityNamespace.HasPermissionOnActors(this.TfsRequestContext, actors, tokens, requestedPermissions, alwaysAllowAdministrators);
    }

    [HttpDelete]
    [ClientExample("DELETE__permissions__securityNamespaceId__4__token-_token1__descriptor-_descriptor_.json", null, null, null)]
    public Microsoft.VisualStudio.Services.Security.AccessControlEntry RemovePermission(
      Guid securityNamespaceId,
      string descriptor,
      int permissions = 0,
      string token = "")
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckStringForNullOrEmpty(descriptor, nameof (descriptor));
      IdentityDescriptor descriptorFromString = IdentityParser.GetDescriptorFromString(descriptor);
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptorFromString, nameof (descriptor));
      return SecurityConverter.Convert((this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId) ?? throw new InvalidSecurityNamespaceException(securityNamespaceId)).RemovePermissions(this.TfsRequestContext, token, descriptorFromString, permissions));
    }

    public override string TraceArea => "SecurityService";

    public override string ActivityLogArea => "Framework";

    [JsonConverter(typeof (PermissionsControllerBase.HasPermissionResultJsonConverter))]
    public struct HasPermissionResult : ISecuredObject
    {
      public readonly bool Result;

      public HasPermissionResult(bool result) => this.Result = result;

      public Guid NamespaceId => SecuritySecurityConstants.NamespaceId;

      public int RequiredPermissions => 1;

      public string GetToken() => "";
    }

    private class HasPermissionResultJsonConverter : VssSecureJsonConverter
    {
      public override bool CanConvert(Type objectType) => typeof (PermissionsControllerBase.HasPermissionResult).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        throw new NotImplementedException();
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        base.WriteJson(writer, value, serializer);
        writer.WriteValue(((PermissionsControllerBase.HasPermissionResult) value).Result);
      }
    }
  }
}
