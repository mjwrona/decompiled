// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.IdentityManagementWebService2
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
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "IdentityManagementService2", CollectionServiceIdentifier = "A4CE4577-B38E-49C8-BDB4-B9C53615E0DA", ConfigurationServiceIdentifier = "6A67CA20-F7B4-4586-B8B6-CB4DA7234919")]
  public class IdentityManagementWebService2 : IdentityManagementWebService
  {
    private TeamFoundationIdentityService m_identityService;

    public IdentityManagementWebService2() => this.m_identityService = this.RequestContext.GetService<TeamFoundationIdentityService>();

    [WebMethod]
    public void AddRecentUser(Guid teamFoundationId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddRecentUser), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamFoundationId), (object) teamFoundationId);
        this.EnterMethod(methodInformation);
        this.m_identityService.AddRecentUser(this.RequestContext, teamFoundationId);
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
    public TeamFoundationIdentity[] GetMostRecentlyUsedUsers(int features)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetMostRecentlyUsedUsers), MethodType.Normal, EstimatedMethodCost.Low);
        TeamFoundationSupportedFeatures supportedFeatures = (TeamFoundationSupportedFeatures) features;
        methodInformation.AddParameter("supportedFeatures", (object) supportedFeatures);
        this.EnterMethod(methodInformation);
        TeamFoundationIdentity[] recentlyUsedUsers = this.m_identityService.GetMostRecentlyUsedUsers(this.RequestContext);
        this.PrepareForWebServiceSerialization((IList<TeamFoundationIdentity>) recentlyUsedUsers, supportedFeatures);
        return recentlyUsedUsers;
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
    public TeamFoundationFilteredIdentitiesList ReadFilteredIdentities(
      string expression,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      int queryMembership,
      int features)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadFilteredIdentities), MethodType.Normal, EstimatedMethodCost.Low);
        MembershipQuery membershipQuery = (MembershipQuery) queryMembership;
        methodInformation.AddParameter(nameof (queryMembership), (object) membershipQuery);
        methodInformation.AddParameter(nameof (expression), (object) expression);
        methodInformation.AddParameter(nameof (suggestedPageSize), (object) suggestedPageSize);
        methodInformation.AddParameter(nameof (lastSearchResult), (object) lastSearchResult);
        methodInformation.AddParameter(nameof (lookForward), (object) lookForward);
        TeamFoundationSupportedFeatures supportedFeatures = (TeamFoundationSupportedFeatures) features;
        methodInformation.AddParameter("supportedFeatures", (object) supportedFeatures);
        this.EnterMethod(methodInformation);
        TeamFoundationFilteredIdentitiesList filteredIdentitiesList = this.m_identityService.ReadFilteredIdentities(this.RequestContext, expression, suggestedPageSize, lastSearchResult, lookForward, membershipQuery);
        this.PrepareForWebServiceSerialization((IList<TeamFoundationIdentity>) filteredIdentitiesList.Items, supportedFeatures);
        return filteredIdentitiesList;
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
    public void SetCustomDisplayName(string customDisplayName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetCustomDisplayName), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (customDisplayName), (object) customDisplayName);
        this.EnterMethod(methodInformation);
        this.m_identityService.SetCustomDisplayName(this.RequestContext, customDisplayName);
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
    public void UpdateIdentityExtendedProperties(
      IdentityDescriptor descriptor,
      StreamingCollection<PropertyValue> updates,
      StreamingCollection<PropertyValue> localUpdates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateIdentityExtendedProperties), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        methodInformation.AddParameter(nameof (updates), (object) updates);
        methodInformation.AddParameter(nameof (localUpdates), (object) localUpdates);
        this.EnterMethod(methodInformation);
        if (updates != null)
          this.m_identityService.UpdateExtendedProperties(this.RequestContext, IdentityPropertyScope.Global, descriptor, (IEnumerable<PropertyValue>) updates);
        if (localUpdates == null)
          return;
        this.m_identityService.UpdateExtendedProperties(this.RequestContext, IdentityPropertyScope.Local, descriptor, (IEnumerable<PropertyValue>) localUpdates);
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
  }
}
