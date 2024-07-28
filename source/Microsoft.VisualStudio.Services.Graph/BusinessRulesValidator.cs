// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.BusinessRulesValidator
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class BusinessRulesValidator
  {
    public static void ValidateGraphGroupCreationContext(
      IVssRequestContext context,
      GraphGroupOriginIdCreationContext creationContext)
    {
      ArgumentUtility.CheckForNull<GraphGroupOriginIdCreationContext>(creationContext, nameof (creationContext));
      if (creationContext.OriginId == null)
        throw new GraphBadRequestException(Resources.GroupCreationContextMissingRequiredField((object) "originId"));
    }

    public static void ValidateGraphGroupCreationContext(
      IVssRequestContext context,
      GraphGroupMailAddressCreationContext creationContext)
    {
      ArgumentUtility.CheckForNull<GraphGroupMailAddressCreationContext>(creationContext, nameof (creationContext));
      if (creationContext.MailAddress == null)
        throw new GraphBadRequestException(Resources.GroupCreationContextMissingRequiredField((object) "mailAddress"));
      if (!context.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGraphEnhancedValidation"))
        return;
      ArgumentUtility.CheckEmailAddress(creationContext.MailAddress, "mailAddress");
    }

    public static void ValidateGraphGroupCreationContext(
      IVssRequestContext context,
      GraphGroupVstsCreationContext creationContext)
    {
      ArgumentUtility.CheckForNull<GraphGroupVstsCreationContext>(creationContext, nameof (creationContext));
      if (creationContext.DisplayName == null)
        throw new GraphBadRequestException(Resources.GroupCreationContextMissingRequiredField((object) "displayName"));
      if (context.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGraphEnhancedValidation"))
      {
        string displayName = creationContext.DisplayName;
        string description = creationContext.Description;
        TFCommonUtil.CheckGroupName(ref displayName);
        TFCommonUtil.CheckGroupDescription(ref description);
      }
      if (creationContext.SpecialGroupType == null)
        return;
      SpecialGroupType result;
      if (!EnumUtility.TryParse<SpecialGroupType>(creationContext.SpecialGroupType, out result))
        throw new GraphBadRequestException(Resources.CannotParseParameter((object) creationContext.SpecialGroupType, (object) "specialGroupType"));
      if (result != SpecialGroupType.Generic && !Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(context, context.GetAuthenticatedDescriptor()))
        throw new GraphBadRequestException(Resources.CannotCreateSpecialGroup((object) creationContext.SpecialGroupType));
    }

    public static void ValidateGraphScopeCreationContext(
      IVssRequestContext context,
      GraphScopeCreationContext creationContext)
    {
      ArgumentUtility.CheckForNull<GraphScopeCreationContext>(creationContext, nameof (creationContext));
      if (string.IsNullOrWhiteSpace(creationContext.Name))
        throw new GraphBadRequestException(Resources.ScopeCreationContextMissingRequiredField((object) "displayName"));
      if (!Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(context, context.GetAuthenticatedDescriptor()))
        throw new GraphBadRequestException(Resources.CannotModifyScopes());
      if (!context.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGraphEnhancedValidation"))
        return;
      string name = creationContext.Name;
      string adminGroupName = creationContext.AdminGroupName;
      string groupDescription = creationContext.AdminGroupDescription;
      TFCommonUtil.CheckGroupName(ref name);
      TFCommonUtil.CheckGroupName(ref adminGroupName);
      TFCommonUtil.CheckGroupDescription(ref groupDescription);
    }

    public static void ValidateGraphMemberUpdateContext(
      IVssRequestContext context,
      IGraphMemberOriginIdUpdateContext updateContext)
    {
      ArgumentUtility.CheckForNull<IGraphMemberOriginIdUpdateContext>(updateContext, nameof (updateContext));
      if (updateContext.OriginId == null)
        throw new GraphBadRequestException(Resources.MemberUpdateContextMissingRequiredField((object) "originId"));
    }

    public static void ValidateGraphMemberCreationContext(
      IVssRequestContext context,
      GraphUserPrincipalNameUpdateContext creationContext)
    {
      ArgumentUtility.CheckForNull<GraphUserPrincipalNameUpdateContext>(creationContext, nameof (creationContext));
      if (creationContext.PrincipalName == null)
        throw new GraphBadRequestException(Resources.MemberUpdateContextMissingRequiredField((object) "principalName"));
    }

    public static void ValidateGraphMemberCreationContext(
      IVssRequestContext context,
      IGraphMemberOriginIdCreationContext creationContext)
    {
      ArgumentUtility.CheckForNull<IGraphMemberOriginIdCreationContext>(creationContext, nameof (creationContext));
      if (creationContext.OriginId == null)
        throw new GraphBadRequestException(Resources.MemberCreationContextMissingRequiredField((object) "originId"));
    }

    public static void ValidateGraphMemberCreationContext(
      IVssRequestContext context,
      IGraphMemberPrincipalNameCreationContext creationContext)
    {
      ArgumentUtility.CheckForNull<IGraphMemberPrincipalNameCreationContext>(creationContext, nameof (creationContext));
      if (creationContext.PrincipalName == null)
        throw new GraphBadRequestException(Resources.MemberCreationContextMissingRequiredField((object) "principalName"));
    }

    public static void ValidateGraphMemberCreationContext(
      IVssRequestContext context,
      IGraphMemberMailAddressCreationContext creationContext)
    {
      ArgumentUtility.CheckForNull<IGraphMemberMailAddressCreationContext>(creationContext, nameof (creationContext));
      if (creationContext.MailAddress == null)
        throw new GraphBadRequestException(Resources.MemberCreationContextMissingRequiredField((object) "mailAddress"));
      if (!context.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGraphEnhancedValidation"))
        return;
      ArgumentUtility.CheckEmailAddress(creationContext.MailAddress, "mailAddress");
    }

    public static void ValidateGraphTraversalDepth(IVssRequestContext context, int depth)
    {
      if (depth != 1)
      {
        if (depth == -1)
        {
          IdentityDescriptor authenticatedDescriptor = context.GetAuthenticatedDescriptor();
          if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(context, authenticatedDescriptor))
            return;
        }
        throw new GraphBadRequestException(Resources.TraversalDepthNotSupported((object) depth, (object) nameof (depth)));
      }
    }
  }
}
