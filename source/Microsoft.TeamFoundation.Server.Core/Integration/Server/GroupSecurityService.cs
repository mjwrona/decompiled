// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.GroupSecurityService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [Obsolete("GroupSecurityService is obsolete.  Please use the IdentityManagementService or SecurityService instead.", false)]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03", Description = "DevOps Group Security web service")]
  [ClientService(ServiceName = "CommonStructure", CollectionServiceIdentifier = "d9c3f8ff-8938-4193-919b-7588e81cb730")]
  public class GroupSecurityService : IntegrationWebService
  {
    private static readonly RegistryQuery s_deprecatedMethodsEnabled = (RegistryQuery) "/Service/Elead/EnableDeprecatedIdentityMethods";

    [WebMethod]
    public Microsoft.TeamFoundation.Integration.Server.Identity ReadIdentity(
      SearchFactor factor,
      string factorValue,
      QueryMembership queryMembership)
    {
      Microsoft.TeamFoundation.Integration.Server.Identity identity1 = (Microsoft.TeamFoundation.Integration.Server.Identity) null;
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadIdentity), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (factor), (object) factor);
        methodInformation.AddParameter(nameof (factorValue), (object) factorValue);
        methodInformation.AddParameter(nameof (queryMembership), (object) queryMembership);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckFactorAndValue(factor, ref factorValue, nameof (factor), nameof (factorValue));
        SecurityValidation.CheckQueryMembership(queryMembership, nameof (queryMembership));
        IdentityDescriptor sidBySearchFactor = this.RequestContext.GetService<TeamFoundationGroupSecurityService>().GetSidBySearchFactor(this.RequestContext, factor, factorValue, ReadIdentityOptions.None);
        if (sidBySearchFactor != (IdentityDescriptor) null)
        {
          TeamFoundationIdentity identity2 = this.RequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(this.RequestContext, sidBySearchFactor, (MembershipQuery) queryMembership, ReadIdentityOptions.None);
          if (identity2 != null && identity2.IsActive)
            identity1 = Microsoft.TeamFoundation.Integration.Server.Identity.Convert(this.RequestContext, identity2);
        }
        return identity1;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Integration.Server.Identity ReadIdentityFromSource(
      SearchFactor factor,
      string factorValue)
    {
      Microsoft.TeamFoundation.Integration.Server.Identity identity1 = (Microsoft.TeamFoundation.Integration.Server.Identity) null;
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadIdentityFromSource), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (factor), (object) factor);
        methodInformation.AddParameter(nameof (factorValue), (object) factorValue);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckFactorAndValue(factor, ref factorValue, nameof (factor), nameof (factorValue));
        IdentityDescriptor sidBySearchFactor = this.RequestContext.GetService<TeamFoundationGroupSecurityService>().GetSidBySearchFactor(this.RequestContext, factor, factorValue, ReadIdentityOptions.IncludeReadFromSource);
        if (sidBySearchFactor != (IdentityDescriptor) null)
        {
          TeamFoundationIdentity identity2 = this.RequestContext.GetService<TeamFoundationIdentityService>().ReadIdentityFromSource(this.RequestContext, sidBySearchFactor, true);
          if (identity2 != null && identity2.IsActive)
            identity1 = Microsoft.TeamFoundation.Integration.Server.Identity.Convert(this.RequestContext, identity2);
        }
        return identity1;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public bool IsIdentityCached(SearchFactor factor, string factorValue)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (IsIdentityCached), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (factor), (object) factor);
        methodInformation.AddParameter(nameof (factorValue), (object) factorValue);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckFactorAndValue(factor, ref factorValue, nameof (factor), nameof (factorValue));
        IdentityDescriptor sidBySearchFactor = this.RequestContext.GetService<TeamFoundationGroupSecurityService>().GetSidBySearchFactor(this.RequestContext, factor, factorValue, ReadIdentityOptions.None);
        return this.RequestContext.GetService<TeamFoundationIdentityService>().IsIdentityCached(this.RequestContext, sidBySearchFactor);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public string GetChangedIdentities(int sequence_id)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetChangedIdentities), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (sequence_id), (object) sequence_id);
        this.EnterMethod(methodInformation);
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment || !this.AreDeprecatedMethodsEnabled(this.RequestContext))
          throw new DeprecatedIdentityMethodException();
        SecurityValidation.CheckSequenceId(sequence_id, nameof (sequence_id));
        this.RequestContext.GetService<IntegrationSecurityManager>().CheckGlobalPermission(this.RequestContext, "SYNCHRONIZE_READ");
        return this.RequestContext.GetService<TeamFoundationGroupSecurityService>().GetChangedIdentities(this.RequestContext, sequence_id);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public string CreateApplicationGroup(
      string projectUri,
      string groupName,
      string groupDescription)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateApplicationGroup), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (groupName), (object) groupName);
        methodInformation.AddParameter(nameof (groupDescription), (object) groupDescription);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<TeamFoundationIdentityService>().CreateApplicationGroup(this.RequestContext, projectUri, groupName, groupDescription).Descriptor.Identifier;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Integration.Server.Identity[] ListApplicationGroups(
      string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ListApplicationGroups), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        TeamFoundationIdentity[] foundationIdentityArray = this.RequestContext.GetService<TeamFoundationIdentityService>().ListApplicationGroups(this.RequestContext, projectUri, ReadIdentityOptions.None, (IEnumerable<string>) null);
        Microsoft.TeamFoundation.Integration.Server.Identity[] identityArray = new Microsoft.TeamFoundation.Integration.Server.Identity[((ICollection<TeamFoundationIdentity>) foundationIdentityArray).Count];
        int num = 0;
        foreach (TeamFoundationIdentity identity in (IEnumerable<TeamFoundationIdentity>) foundationIdentityArray)
          identityArray[num++] = Microsoft.TeamFoundation.Integration.Server.Identity.Convert(this.RequestContext, identity);
        return identityArray;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdateApplicationGroup(
      string groupSid,
      ApplicationGroupProperty property,
      string newValue)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupSid), (object) groupSid);
        methodInformation.AddParameter(nameof (property), (object) property);
        methodInformation.AddParameter(nameof (newValue), (object) newValue);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<TeamFoundationIdentityService>().UpdateApplicationGroup(this.RequestContext, new IdentityDescriptor("Microsoft.TeamFoundation.Identity", groupSid), (GroupProperty) property, newValue);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteApplicationGroup(string groupSid)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupSid), (object) groupSid);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<TeamFoundationIdentityService>().DeleteApplicationGroup(this.RequestContext, new IdentityDescriptor("Microsoft.TeamFoundation.Identity", groupSid));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void AddMemberToApplicationGroup(string groupSid, string identitySid)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddMemberToApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupSid), (object) groupSid);
        methodInformation.AddParameter(nameof (identitySid), (object) identitySid);
        this.EnterMethod(methodInformation);
        IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(identitySid);
        this.RequestContext.GetService<TeamFoundationIdentityService>().AddMemberToApplicationGroup(this.RequestContext, new IdentityDescriptor("Microsoft.TeamFoundation.Identity", groupSid), descriptorFromSid);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void RemoveMemberFromApplicationGroup(string groupSid, string identitySid)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveMemberFromApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupSid), (object) groupSid);
        methodInformation.AddParameter(nameof (identitySid), (object) identitySid);
        this.EnterMethod(methodInformation);
        IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(identitySid);
        this.RequestContext.GetService<TeamFoundationIdentityService>().RemoveMemberFromApplicationGroup(this.RequestContext, new IdentityDescriptor("Microsoft.TeamFoundation.Identity", groupSid), descriptorFromSid);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public bool IsMember(string groupSid, string identitySid)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (IsMember), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupSid), (object) groupSid);
        methodInformation.AddParameter(nameof (identitySid), (object) identitySid);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<TeamFoundationIdentityService>().IsMember(this.RequestContext, IdentityHelper.CreateDescriptorFromSid(groupSid), IdentityHelper.CreateDescriptorFromSid(identitySid));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    protected bool AreDeprecatedMethodsEnabled(IVssRequestContext requestContext)
    {
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in GroupSecurityService.s_deprecatedMethodsEnabled, false);
    }
  }
}
