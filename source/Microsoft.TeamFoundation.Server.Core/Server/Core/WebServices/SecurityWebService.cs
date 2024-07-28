// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.SecurityWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "SecurityService", CollectionServiceIdentifier = "af3178da-1ec3-4bd0-b245-9f5decdc572e", ConfigurationServiceIdentifier = "AFF1A844-BA7D-4340-8A95-2952524EC778")]
  public class SecurityWebService : FrameworkWebService
  {
    private ITeamFoundationSecurityService m_securityService;
    private ITeamFoundationSecurityService m_localSecurityService;
    private TeamFoundationSecurityService m_rawSecurityService;

    public SecurityWebService()
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.RequestContext.CheckOnPremisesDeployment(true);
      this.m_securityService = (ITeamFoundationSecurityService) this.RequestContext.GetService<SecuredTeamFoundationSecurityService>();
      this.m_localSecurityService = (ITeamFoundationSecurityService) this.RequestContext.GetService<SecuredLocalSecurityService>();
      this.m_rawSecurityService = this.RequestContext.GetService<TeamFoundationSecurityService>();
    }

    [WebMethod]
    public void CreateSecurityNamespace(SecurityNamespaceDescription description)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateSecurityNamespace), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (description), (object) description);
        this.EnterMethod(methodInformation);
        this.m_securityService.CreateSecurityNamespace(this.RequestContext, description);
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
    public void DeleteSecurityNamespace(Guid namespaceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteSecurityNamespace), MethodType.Admin, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        this.EnterMethod(methodInformation);
        this.m_securityService.DeleteSecurityNamespace(this.RequestContext, namespaceId);
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
    public StreamingCollection<SecurityNamespaceDescription> QuerySecurityNamespaces(
      Guid namespaceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySecurityNamespaces), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        this.EnterMethod(methodInformation);
        IEnumerable<SecurityNamespaceDescription> namespaceDescriptions;
        if (Guid.Empty == namespaceId)
        {
          namespaceDescriptions = this.m_localSecurityService.GetSecurityNamespaces(this.RequestContext).Select<IVssSecurityNamespace, SecurityNamespaceDescription>((Func<IVssSecurityNamespace, SecurityNamespaceDescription>) (s => s.Description));
          IVssSecurityNamespace securityNamespace1 = this.m_securityService.GetSecurityNamespace(this.RequestContext, FrameworkSecurity.IdentitiesNamespaceId);
          if (securityNamespace1 != null)
            namespaceDescriptions = namespaceDescriptions.Concat<SecurityNamespaceDescription>((IEnumerable<SecurityNamespaceDescription>) new SecurityNamespaceDescription[1]
            {
              securityNamespace1.Description
            });
          IVssSecurityNamespace securityNamespace2 = this.m_securityService.GetSecurityNamespace(this.RequestContext, AuditLogConstants.SecurityNamespaceId);
          if (securityNamespace2 != null)
            namespaceDescriptions = namespaceDescriptions.Concat<SecurityNamespaceDescription>((IEnumerable<SecurityNamespaceDescription>) new SecurityNamespaceDescription[1]
            {
              securityNamespace2.Description
            });
        }
        else
        {
          IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
          if (securityNamespace != null)
            namespaceDescriptions = (IEnumerable<SecurityNamespaceDescription>) new SecurityNamespaceDescription[1]
            {
              securityNamespace.Description
            };
          else
            namespaceDescriptions = (IEnumerable<SecurityNamespaceDescription>) Array.Empty<SecurityNamespaceDescription>();
        }
        return new StreamingCollection<SecurityNamespaceDescription>(namespaceDescriptions);
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
    public bool RemoveAccessControlList(Guid namespaceId, [ClientType(typeof (IEnumerable<string>))] List<string> tokens, bool recurse)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveAccessControlList), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddArrayParameter<string>(nameof (tokens), (IList<string>) tokens);
        methodInformation.AddParameter(nameof (recurse), (object) recurse);
        this.EnterMethod(methodInformation);
        IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        for (int index = 0; index < tokens.Count; ++index)
          tokens[index] = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace.Unsecured(), tokens[index]);
        return securityNamespace.RemoveAccessControlLists(this.RequestContext, (IEnumerable<string>) tokens, recurse);
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
    public bool RemoveAccessControlEntries(
      Guid namespaceId,
      string token,
      [ClientType(typeof (IEnumerable<IdentityDescriptor>))] List<IdentityDescriptor> identities)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveAccessControlEntries), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddArrayParameter<IdentityDescriptor>(nameof (identities), (IList<IdentityDescriptor>) identities);
        this.EnterMethod(methodInformation);
        IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace.Unsecured(), token);
        return securityNamespace.RemoveAccessControlEntries(this.RequestContext, token, (IEnumerable<IdentityDescriptor>) identities);
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
    public List<AccessControlEntryDetails> SetPermissions(
      Guid namespaceId,
      string token,
      [ClientType(typeof (IEnumerable<AccessControlEntryDetails>))] List<AccessControlEntryDetails> accessControlEntries,
      bool merge)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetPermissions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddArrayParameter<AccessControlEntryDetails>(nameof (accessControlEntries), (IList<AccessControlEntryDetails>) accessControlEntries);
        methodInformation.AddParameter(nameof (merge), (object) merge);
        this.EnterMethod(methodInformation);
        IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace.Unsecured(), token);
        IEnumerable<IAccessControlEntry> source = securityNamespace.SetAccessControlEntries(this.RequestContext, token, accessControlEntries.Select<AccessControlEntryDetails, IAccessControlEntry>((Func<AccessControlEntryDetails, IAccessControlEntry>) (s => s.ToAccessControlEntry())), merge);
        token = securityNamespace.NamespaceExtension.HandleOutgoingToken(this.RequestContext, securityNamespace.Unsecured(), token);
        Func<IAccessControlEntry, AccessControlEntryDetails> selector = (Func<IAccessControlEntry, AccessControlEntryDetails>) (s => new AccessControlEntryDetails(token, s));
        return source.Select<IAccessControlEntry, AccessControlEntryDetails>(selector).ToList<AccessControlEntryDetails>();
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
    public void SetAccessControlList(
      Guid namespaceId,
      [ClientType(typeof (IEnumerable<AccessControlListDetails>))] List<AccessControlListDetails> accessControlLists)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetAccessControlList), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddArrayParameter<AccessControlListDetails>(nameof (accessControlLists), (IList<AccessControlListDetails>) accessControlLists);
        this.EnterMethod(methodInformation);
        IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        foreach (AccessControlListDetails accessControlList in accessControlLists)
          accessControlList.Token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace.Unsecured(), accessControlList.Token);
        securityNamespace.SetAccessControlLists(this.RequestContext, accessControlLists.Select<AccessControlListDetails, IAccessControlList>((Func<AccessControlListDetails, IAccessControlList>) (s => s.ToAccessControlList())));
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
    public List<AccessControlListDetails> QueryPermissions(
      Guid namespaceId,
      string token,
      [ClientType(typeof (IEnumerable<IdentityDescriptor>))] List<IdentityDescriptor> identities,
      bool includeExtendedInfo,
      bool recurse)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryPermissions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddArrayParameter<IdentityDescriptor>(nameof (identities), (IList<IdentityDescriptor>) identities);
        methodInformation.AddParameter(nameof (includeExtendedInfo), (object) includeExtendedInfo);
        methodInformation.AddParameter(nameof (recurse), (object) recurse);
        this.EnterMethod(methodInformation);
        IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace.Unsecured(), token);
        IEnumerable<IAccessControlList> source = securityNamespace.QueryAccessControlLists(this.RequestContext, token, identities == null ? (IEnumerable<IdentityDescriptor>) (IdentityDescriptor[]) null : (IEnumerable<IdentityDescriptor>) identities.ToArray(), includeExtendedInfo, recurse);
        foreach (IAccessControlList accessControlList in source)
          accessControlList.Token = securityNamespace.NamespaceExtension.HandleOutgoingToken(this.RequestContext, securityNamespace.Unsecured(), accessControlList.Token);
        return source.Select<IAccessControlList, AccessControlListDetails>((Func<IAccessControlList, AccessControlListDetails>) (s => new AccessControlListDetails(s))).ToList<AccessControlListDetails>();
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
    public void SetInheritFlag(Guid namespaceId, string token, bool inherits)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("SetInheritance", MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddParameter(nameof (inherits), (object) inherits);
        this.EnterMethod(methodInformation);
        IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace.Unsecured(), token);
        securityNamespace.SetInheritFlag(this.RequestContext, token, inherits);
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
    public AccessControlEntryDetails RemovePermissions(
      Guid namespaceId,
      string token,
      IdentityDescriptor descriptor,
      int permissions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemovePermissions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        methodInformation.AddParameter(nameof (permissions), (object) permissions);
        this.EnterMethod(methodInformation);
        IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        token = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace, token);
        IAccessControlEntry accessControlEntry = securityNamespace.RemovePermissions(this.RequestContext, token, descriptor, permissions);
        token = securityNamespace.NamespaceExtension.HandleOutgoingToken(this.RequestContext, securityNamespace, token);
        return new AccessControlEntryDetails()
        {
          Descriptor = accessControlEntry.Descriptor,
          Allow = accessControlEntry.Allow,
          Deny = accessControlEntry.Deny,
          Token = token
        };
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
    public List<bool> HasPermissionByTokenList(
      Guid namespaceId,
      [ClientType(typeof (IEnumerable<string>))] List<string> tokens,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (HasPermissionByTokenList), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddArrayParameter<string>(nameof (tokens), (IList<string>) tokens);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        methodInformation.AddParameter(nameof (requestedPermissions), (object) requestedPermissions);
        methodInformation.AddParameter(nameof (alwaysAllowAdministrators), (object) alwaysAllowAdministrators);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tokens, nameof (tokens));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
        IVssSecurityNamespace securityNamespace = this.m_rawSecurityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        using (IVssRequestContext requestContext = this.RequestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(this.RequestContext, this.RequestContext.ServiceHost.InstanceId, descriptor, true))
        {
          List<bool> boolList = new List<bool>();
          foreach (string token1 in tokens)
          {
            string token2 = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace, token1);
            boolList.Add(securityNamespace.HasPermission(requestContext, token2, requestedPermissions, alwaysAllowAdministrators));
          }
          return boolList;
        }
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
    public List<bool> HasPermissionByDescriptorList(
      Guid namespaceId,
      string token,
      [ClientType(typeof (IEnumerable<IdentityDescriptor>))] List<IdentityDescriptor> descriptors,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (HasPermissionByDescriptorList), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddArrayParameter<IdentityDescriptor>(nameof (descriptors), (IList<IdentityDescriptor>) descriptors);
        methodInformation.AddParameter(nameof (requestedPermissions), (object) requestedPermissions);
        methodInformation.AddParameter(nameof (alwaysAllowAdministrators), (object) alwaysAllowAdministrators);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
        ArgumentUtility.CheckForNull<string>(token, nameof (token));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) descriptors, nameof (descriptors));
        IVssSecurityNamespace securityNamespace = this.m_rawSecurityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        List<bool> boolList = new List<bool>();
        string token1 = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace, token);
        foreach (IdentityDescriptor descriptor in descriptors)
        {
          using (IVssRequestContext requestContext = this.RequestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(this.RequestContext, this.RequestContext.ServiceHost.InstanceId, descriptor, true))
            boolList.Add(securityNamespace.HasPermission(requestContext, token1, requestedPermissions, alwaysAllowAdministrators));
        }
        return boolList;
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
    public List<bool> HasPermissionByPermissionsList(
      Guid namespaceId,
      string token,
      IdentityDescriptor descriptor,
      [ClientType(typeof (IEnumerable<int>))] List<int> requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (HasPermissionByPermissionsList), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddParameter(nameof (descriptor), (object) descriptor);
        methodInformation.AddArrayParameter<int>(nameof (requestedPermissions), (IList<int>) requestedPermissions);
        methodInformation.AddParameter(nameof (alwaysAllowAdministrators), (object) alwaysAllowAdministrators);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
        ArgumentUtility.CheckForNull<string>(token, nameof (token));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) requestedPermissions, nameof (requestedPermissions));
        IVssSecurityNamespace securityNamespace = this.m_rawSecurityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        List<bool> boolList = new List<bool>();
        string token1 = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace, token);
        using (IVssRequestContext requestContext = this.RequestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(this.RequestContext, this.RequestContext.ServiceHost.InstanceId, descriptor, true))
        {
          foreach (int requestedPermission in requestedPermissions)
            boolList.Add(securityNamespace.HasPermission(requestContext, token1, requestedPermission, alwaysAllowAdministrators));
          return boolList;
        }
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
    public List<bool> HasWritePermission(
      Guid namespaceId,
      string token,
      [ClientType(typeof (IEnumerable<int>))] List<int> requestedPermissions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (HasWritePermission), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (namespaceId), (object) namespaceId);
        methodInformation.AddParameter(nameof (token), (object) token);
        methodInformation.AddArrayParameter<int>(nameof (requestedPermissions), (IList<int>) requestedPermissions);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
        ArgumentUtility.CheckForNull<string>(token, nameof (token));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) requestedPermissions, nameof (requestedPermissions));
        IVssSecurityNamespace securityNamespace = this.m_rawSecurityService.GetSecurityNamespace(this.RequestContext, namespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(namespaceId);
        List<bool> boolList = new List<bool>();
        string token1 = securityNamespace.NamespaceExtension.HandleIncomingToken(this.RequestContext, securityNamespace, token);
        bool flag = securityNamespace.HasWritePermission(this.RequestContext, token1, false);
        foreach (int requestedPermission in requestedPermissions)
          boolList.Add(flag);
        return boolList;
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
