// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.IdentityManagementWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "IdentityManagementService", CollectionServiceIdentifier = "1E29861E-76B6-4b1e-BF41-5F868AEA63FE", ConfigurationServiceIdentifier = "3DE26348-00BE-4B82-8E4A-E5AD004CFECD")]
  public class IdentityManagementWebService : FrameworkWebService
  {
    private TeamFoundationIdentityService m_identityManager;

    public IdentityManagementWebService()
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.RequestContext.CheckOnPremisesDeployment(true);
      this.m_identityManager = this.RequestContext.GetService<TeamFoundationIdentityService>();
      this.RequestContext.ServiceName = "Identities";
    }

    [WebMethod]
    public TeamFoundationIdentity[] ReadIdentitiesByDescriptor(
      IdentityDescriptor[] descriptors,
      int queryMembership,
      int options,
      int features,
      [ClientType(typeof (IEnumerable<string>))] string[] propertyNameFilters,
      int propertyScope)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadIdentitiesByDescriptor), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<IdentityDescriptor>(nameof (descriptors), (IList<IdentityDescriptor>) descriptors);
        MembershipQuery membershipQuery = (MembershipQuery) queryMembership;
        methodInformation.AddParameter(nameof (queryMembership), (object) membershipQuery);
        ReadIdentityOptions readIdentityOptions = (ReadIdentityOptions) options;
        methodInformation.AddParameter("readOptions", (object) readIdentityOptions);
        TeamFoundationSupportedFeatures supportedFeatures = (TeamFoundationSupportedFeatures) features;
        IdentityPropertyScope parameterValue = (IdentityPropertyScope) propertyScope;
        methodInformation.AddParameter("supportedFeatures", (object) supportedFeatures);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        methodInformation.AddParameter(nameof (propertyScope), (object) parameterValue);
        this.EnterMethod(methodInformation);
        TeamFoundationIdentity[] identities = this.m_identityManager.ReadIdentities(this.RequestContext, descriptors, membershipQuery, readIdentityOptions, (IEnumerable<string>) propertyNameFilters, parameterValue == IdentityPropertyScope.None ? IdentityPropertyScope.Both : parameterValue);
        this.PrepareForWebServiceSerialization((IList<TeamFoundationIdentity>) identities, supportedFeatures);
        return identities;
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
    public TeamFoundationIdentity[] ReadIdentitiesById(
      Guid[] teamFoundationIds,
      int queryMembership,
      int features,
      int options,
      [ClientType(typeof (IEnumerable<string>))] string[] propertyNameFilters,
      int propertyScope)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadIdentitiesById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (teamFoundationIds), (IList<Guid>) teamFoundationIds);
        MembershipQuery membershipQuery = (MembershipQuery) queryMembership;
        methodInformation.AddParameter(nameof (queryMembership), (object) membershipQuery);
        TeamFoundationSupportedFeatures supportedFeatures = (TeamFoundationSupportedFeatures) features;
        IdentityPropertyScope parameterValue = (IdentityPropertyScope) propertyScope;
        methodInformation.AddParameter("supportedFeatures", (object) supportedFeatures);
        ReadIdentityOptions readIdentityOptions = (ReadIdentityOptions) options;
        methodInformation.AddParameter("readOptions", (object) readIdentityOptions);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        methodInformation.AddParameter(nameof (propertyScope), (object) parameterValue);
        this.EnterMethod(methodInformation);
        TeamFoundationIdentity[] foundationIdentityArray = this.m_identityManager.ReadIdentities(this.RequestContext, teamFoundationIds, membershipQuery, readIdentityOptions, (IEnumerable<string>) propertyNameFilters, parameterValue == IdentityPropertyScope.None ? IdentityPropertyScope.Both : parameterValue);
        if (FavoritesServiceShim.IsFavoritesReadShimNeeded(this.RequestContext, propertyNameFilters, foundationIdentityArray))
          FavoritesServiceShim.OverrideQueriedFavorites(this.RequestContext, propertyNameFilters, foundationIdentityArray);
        this.PrepareForWebServiceSerialization((IList<TeamFoundationIdentity>) foundationIdentityArray, supportedFeatures);
        return foundationIdentityArray;
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
    public TeamFoundationIdentity[][] ReadIdentities(
      int searchFactor,
      string[] factorValues,
      int queryMembership,
      int options,
      int features,
      [ClientType(typeof (IEnumerable<string>))] string[] propertyNameFilters,
      int propertyScope)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadIdentities), MethodType.Normal, EstimatedMethodCost.Low);
        IdentitySearchFactor identitySearchFactor = (IdentitySearchFactor) searchFactor;
        methodInformation.AddParameter(nameof (searchFactor), (object) identitySearchFactor);
        methodInformation.AddArrayParameter<string>(nameof (factorValues), (IList<string>) factorValues);
        MembershipQuery membershipQuery = (MembershipQuery) queryMembership;
        methodInformation.AddParameter(nameof (queryMembership), (object) membershipQuery);
        ReadIdentityOptions readIdentityOptions = (ReadIdentityOptions) options;
        methodInformation.AddParameter("readOptions", (object) readIdentityOptions);
        TeamFoundationSupportedFeatures supportedFeatures = (TeamFoundationSupportedFeatures) features;
        IdentityPropertyScope parameterValue = (IdentityPropertyScope) propertyScope;
        methodInformation.AddParameter("supportedFeatures", (object) supportedFeatures);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        methodInformation.AddParameter(nameof (propertyScope), (object) parameterValue);
        this.EnterMethod(methodInformation);
        TeamFoundationIdentity[][] foundationIdentityArray = this.m_identityManager.ReadIdentities(this.RequestContext, identitySearchFactor, factorValues, membershipQuery, readIdentityOptions, (IEnumerable<string>) propertyNameFilters, parameterValue == IdentityPropertyScope.None ? IdentityPropertyScope.Both : parameterValue);
        for (int index = 0; index < foundationIdentityArray.Length; ++index)
          this.PrepareForWebServiceSerialization((IList<TeamFoundationIdentity>) foundationIdentityArray[index], supportedFeatures);
        return foundationIdentityArray;
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
    public IdentityDescriptor CreateApplicationGroup(
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
        return this.m_identityManager.CreateApplicationGroup(this.RequestContext, projectUri, groupName, groupDescription).Descriptor;
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
    public TeamFoundationIdentity[] ListApplicationGroups(
      string projectUri,
      int options,
      int features,
      [ClientType(typeof (IEnumerable<string>))] string[] propertyNameFilters,
      int propertyScope)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ListApplicationGroups), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        ReadIdentityOptions readIdentityOptions = (ReadIdentityOptions) options;
        methodInformation.AddParameter("readOptions", (object) readIdentityOptions);
        TeamFoundationSupportedFeatures supportedFeatures = (TeamFoundationSupportedFeatures) features;
        IdentityPropertyScope parameterValue = (IdentityPropertyScope) propertyScope;
        methodInformation.AddParameter("supportedFeatures", (object) supportedFeatures);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        methodInformation.AddParameter(nameof (propertyScope), (object) parameterValue);
        this.EnterMethod(methodInformation);
        TeamFoundationIdentity[] identities = this.m_identityManager.ListApplicationGroups(this.RequestContext, projectUri, readIdentityOptions, (IEnumerable<string>) propertyNameFilters, parameterValue == IdentityPropertyScope.None ? IdentityPropertyScope.Both : parameterValue);
        this.PrepareForWebServiceSerialization((IList<TeamFoundationIdentity>) identities, supportedFeatures);
        return identities;
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
      IdentityDescriptor groupDescriptor,
      int groupProperty,
      string newValue)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupDescriptor), (object) groupDescriptor);
        methodInformation.AddParameter(nameof (groupProperty), (object) groupProperty);
        methodInformation.AddParameter(nameof (newValue), (object) newValue);
        this.EnterMethod(methodInformation);
        this.m_identityManager.UpdateApplicationGroup(this.RequestContext, groupDescriptor, (GroupProperty) groupProperty, newValue);
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
    public void DeleteApplicationGroup(IdentityDescriptor groupDescriptor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupDescriptor), (object) groupDescriptor);
        this.EnterMethod(methodInformation);
        this.m_identityManager.DeleteApplicationGroup(this.RequestContext, groupDescriptor);
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
    public void AddMemberToApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddMemberToApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupDescriptor), (object) groupDescriptor);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        this.EnterMethod(methodInformation);
        this.m_identityManager.AddMemberToApplicationGroup(this.RequestContext, groupDescriptor, descriptor);
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
    public void RemoveMemberFromApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveMemberFromApplicationGroup), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (groupDescriptor), (object) groupDescriptor);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        this.EnterMethod(methodInformation);
        this.m_identityManager.RemoveMemberFromApplicationGroup(this.RequestContext, groupDescriptor, descriptor);
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
    public bool IsMember(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (IsMember), MethodType.Normal, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (groupDescriptor), (object) groupDescriptor);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        this.EnterMethod(methodInformation);
        return this.m_identityManager.IsMember(this.RequestContext, groupDescriptor, descriptor);
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
    public bool RefreshIdentity(IdentityDescriptor descriptor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RefreshIdentity), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        this.EnterMethod(methodInformation);
        return this.m_identityManager.RefreshIdentity(this.RequestContext, descriptor);
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
    public string GetScopeName(string scopeId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetScopeName), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (scopeId), (object) scopeId);
        this.EnterMethod(methodInformation);
        string scopeName;
        this.m_identityManager.GetScopeInfo(this.RequestContext, scopeId, out scopeName, out IdentityDescriptor _, out bool _);
        return scopeName;
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

    protected void PrepareForWebServiceSerialization(
      IList<TeamFoundationIdentity> identities,
      TeamFoundationSupportedFeatures supportedFeatures)
    {
      foreach (TeamFoundationIdentity identity in (IEnumerable<TeamFoundationIdentity>) identities)
        identity?.PrepareForWebServiceSerialization(supportedFeatures);
    }
  }
}
