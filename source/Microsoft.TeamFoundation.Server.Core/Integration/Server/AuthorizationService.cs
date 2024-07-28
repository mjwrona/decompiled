// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.AuthorizationService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03", Description = "DevOps Authorization web service")]
  [ClientService(ServiceName = "Authorization", CollectionServiceIdentifier = "6373ee32-aad4-4bf9-9ec8-72201ab1c45c")]
  public class AuthorizationService : IntegrationWebService
  {
    private string c_allowPreDev15ClientsFeatureFlag = "VisualStudio.ProjectService.AllowPreDev15Clients";

    public AuthorizationService() => this.ServiceVersion = 1;

    public AuthorizationService(int serviceVersion) => this.ServiceVersion = serviceVersion;

    protected int ServiceVersion { get; set; }

    [WebMethod]
    public void RegisterObject(
      string objectId,
      string objectClassId,
      string projectUri,
      string parentObjectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RegisterObject), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddParameter(nameof (objectClassId), (object) objectClassId);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (parentObjectId), (object) parentObjectId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckObjectClassId(ref objectClassId, nameof (objectClassId));
        SecurityValidation.CheckProjectUri(ref projectUri, true, nameof (projectUri));
        this.m_integrationHost.SecuredAuthorizationManager.RegisterObject(this.RequestContext, objectId, objectClassId, projectUri, parentObjectId);
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
    public void UnregisterObject(string objectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UnregisterObject), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        this.m_integrationHost.SecuredAuthorizationManager.UnregisterObject(this.RequestContext, objectId);
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
    public void ResetInheritance(string objectId, string parentObejctId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ResetInheritance), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddParameter(nameof (parentObejctId), (object) parentObejctId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckObjectId(ref parentObejctId, true, nameof (parentObejctId));
        SecurityValidation.CheckParentObjectIdNotSelf(objectId, parentObejctId, nameof (parentObejctId));
        this.m_integrationHost.SecuredAuthorizationManager.ResetInheritance(this.RequestContext, objectId, parentObejctId);
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
    public string GetObjectClass(string objectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetObjectClass), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        return this.m_integrationHost.SecuredAuthorizationManager.GetObjectClass(this.RequestContext, objectId);
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
    public string[] ListObjectClasses()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (ListObjectClasses), MethodType.Normal, EstimatedMethodCost.Low));
        return this.m_integrationHost.SecuredAuthorizationManager.ListObjectClasses(this.RequestContext);
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
    public string[] ListObjectClassActions(string objectClassId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ListObjectClassActions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectClassId), (object) objectClassId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectClassId(ref objectClassId, nameof (objectClassId));
        return this.m_integrationHost.SecuredAuthorizationManager.ListObjectClassActions(this.RequestContext, objectClassId);
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
    public string[] ListLocalizedActionNames(string objectClassId, string[] actionId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ListLocalizedActionNames), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectClassId), (object) objectClassId);
        methodInformation.AddArrayParameter<string>(nameof (actionId), (IList<string>) actionId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectClassId(ref objectClassId, nameof (objectClassId));
        SecurityValidation.CheckActionIdArray(actionId, nameof (actionId));
        return this.m_integrationHost.SecuredAuthorizationManager.ListLocalizedActionNames(this.RequestContext, objectClassId, actionId);
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
    public void AddAccessControlEntry(string objectId, Microsoft.Azure.Boards.CssNodes.AccessControlEntry ace)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddAccessControlEntry), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddParameter(nameof (ace), (object) ace);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckAccessControlEntry(ace, nameof (ace));
        this.m_integrationHost.SecuredAuthorizationManager.AddAccessControlEntry(this.RequestContext, objectId, ace);
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
    public void RemoveAccessControlEntry(string objectId, Microsoft.Azure.Boards.CssNodes.AccessControlEntry ace)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveAccessControlEntry), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddParameter(nameof (ace), (object) ace);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckAccessControlEntry(ace, nameof (ace));
        this.m_integrationHost.SecuredAuthorizationManager.RemoveAccessControlEntry(this.RequestContext, objectId, ace);
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
    public string GetChangedAccessControlEntries(int sequence_id)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetChangedAccessControlEntries), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (sequence_id), (object) sequence_id);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckSequenceId(sequence_id, nameof (sequence_id));
        return this.m_integrationHost.SecuredAuthorizationManager.GetChangedAccessControlEntries(this.RequestContext, sequence_id);
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
    public void ReplaceAccessControlList(string objectId, [AllowEmptyArray] Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] acl)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReplaceAccessControlList), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddArrayParameter<Microsoft.Azure.Boards.CssNodes.AccessControlEntry>(nameof (acl), (IList<Microsoft.Azure.Boards.CssNodes.AccessControlEntry>) acl);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckAccessControlEntryArray(acl, nameof (acl));
        this.m_integrationHost.SecuredAuthorizationManager.ReplaceAccessControlList(this.RequestContext, objectId, acl);
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
    public Microsoft.Azure.Boards.CssNodes.AccessControlEntry[][] ReadAccessControlLists(
      string[] objectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadAccessControlLists), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (objectId), (IList<string>) objectId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectIdArray(objectId, nameof (objectId));
        Microsoft.Azure.Boards.CssNodes.AccessControlEntry[][] accessControlEntryArray = new Microsoft.Azure.Boards.CssNodes.AccessControlEntry[objectId.Length][];
        for (int index = 0; index < accessControlEntryArray.Length; ++index)
          accessControlEntryArray[index] = this.m_integrationHost.SecuredAuthorizationManager.ReadAccessControlList(this.RequestContext, objectId[index]);
        return accessControlEntryArray;
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
    public Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] ReadAccessControlList(
      string objectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReadAccessControlList), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        return this.m_integrationHost.SecuredAuthorizationManager.ReadAccessControlList(this.RequestContext, objectId);
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
    public bool[] IsPermittedByObjectList(string[] objectId, string actionId, string userSid)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (IsPermittedByObjectList), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (objectId), (IList<string>) objectId);
        methodInformation.AddParameter(nameof (actionId), (object) actionId);
        methodInformation.AddParameter(nameof (userSid), (object) userSid);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectIdArray(objectId, nameof (objectId));
        SecurityValidation.CheckActionId(ref actionId, nameof (actionId));
        SecurityValidation.CheckSid(ref userSid, nameof (userSid));
        IdentityDescriptor descriptor = (IdentityDescriptor) null;
        TeamFoundationIdentity foundationIdentity = this.RequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(this.RequestContext, IdentitySearchFactor.Identifier, userSid, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
        if (foundationIdentity != null)
          descriptor = foundationIdentity.Descriptor;
        bool[] flagArray = new bool[objectId.Length];
        for (int index = 0; index < flagArray.Length; ++index)
        {
          this.ValidateClientVersion(objectId[index], actionId);
          flagArray[index] = descriptor != (IdentityDescriptor) null && this.m_integrationHost.SecuredAuthorizationManager.IsPermitted(this.RequestContext, objectId[index], actionId, descriptor);
        }
        return flagArray;
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
    public bool[] IsPermittedByActionList(string objectId, string[] actionId, string userSid)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (IsPermittedByActionList), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddArrayParameter<string>(nameof (actionId), (IList<string>) actionId);
        methodInformation.AddParameter(nameof (userSid), (object) userSid);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckActionIdArray(actionId, nameof (actionId));
        SecurityValidation.CheckSid(ref userSid, nameof (userSid));
        IdentityDescriptor descriptor = (IdentityDescriptor) null;
        TeamFoundationIdentity foundationIdentity = this.RequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(this.RequestContext, IdentitySearchFactor.Identifier, userSid, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
        if (foundationIdentity != null)
          descriptor = foundationIdentity.Descriptor;
        bool[] flagArray = new bool[actionId.Length];
        for (int index = 0; index < flagArray.Length; ++index)
        {
          this.ValidateClientVersion(objectId, actionId[index]);
          flagArray[index] = descriptor != (IdentityDescriptor) null && this.m_integrationHost.SecuredAuthorizationManager.IsPermitted(this.RequestContext, objectId, actionId[index], descriptor);
        }
        return flagArray;
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
    public bool[] IsPermittedBySidList(string objectId, string actionId, string[] userSid)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (IsPermittedBySidList), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddParameter(nameof (actionId), (object) actionId);
        methodInformation.AddArrayParameter<string>(nameof (userSid), (IList<string>) userSid);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckActionId(ref actionId, nameof (actionId));
        SecurityValidation.CheckSidArray(userSid, nameof (userSid));
        this.ValidateClientVersion(objectId, actionId);
        bool[] flagArray = new bool[userSid.Length];
        for (int index = 0; index < flagArray.Length; ++index)
        {
          IdentityDescriptor descriptor = (IdentityDescriptor) null;
          TeamFoundationIdentity foundationIdentity = this.RequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(this.RequestContext, IdentitySearchFactor.Identifier, userSid[index], MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
          if (foundationIdentity != null)
            descriptor = foundationIdentity.Descriptor;
          flagArray[index] = descriptor != (IdentityDescriptor) null && this.m_integrationHost.SecuredAuthorizationManager.IsPermitted(this.RequestContext, objectId, actionId, descriptor);
        }
        return flagArray;
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
    public void CheckPermission(string objectId, string actionId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckPermission), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (objectId), (object) objectId);
        methodInformation.AddParameter(nameof (actionId), (object) actionId);
        this.EnterMethod(methodInformation);
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckActionId(ref actionId, nameof (actionId));
        this.ValidateClientVersion(objectId, actionId);
        this.RequestContext.GetService<IntegrationSecurityManager>().CheckObjectPermission(this.RequestContext, objectId, actionId);
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

    internal void ValidateClientVersion(string objectId, string actionId)
    {
      if (this.ServiceVersion >= 5 && this.ServiceVersion < 7 && this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment && !this.RequestContext.IsFeatureEnabled(this.c_allowPreDev15ClientsFeatureFlag) && TFStringComparer.PermissionName.Equals(actionId, "CREATE_PROJECTS"))
        throw new TeamFoundationServiceUnavailableException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INCOMPATIBLE_CLIENT_FOR_PCW());
      if (this.ServiceVersion >= 5)
        return;
      if (TFStringComparer.PermissionName.Equals(actionId, "CREATE_PROJECTS"))
        throw new UnauthorizedAccessException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INCOMPATIBLE_CLIENT_FOR_PCW());
      if (TFStringComparer.PermissionName.Equals(actionId, "DELETE") && TFStringComparer.ObjectId.StartsWith(objectId, PermissionNamespaces.Project))
      {
        if (this.ServiceVersion < 4)
          throw new UnauthorizedAccessException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INCOMPATIBLE_CLIENT_FOR_PROJECTDELETE());
      }
      else if (TFStringComparer.PermissionName.Equals(actionId, "MANAGE_TEMPLATE") || TFStringComparer.PermissionName.Equals(actionId, "ManageLinkTypes"))
        throw new UnauthorizedAccessException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INCOMPATIBLE_CLIENT_FOR_TEMPLATES());
    }
  }
}
