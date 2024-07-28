// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.AuthorizationSecurityClass
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class AuthorizationSecurityClass
  {
    private TeamFoundationIdentityService m_identityService;
    private TeamFoundationEventService m_eventService;
    private LocalSecurityNamespace m_classSecurity;
    private string m_classId;
    private Dictionary<int, ActionDefinition> m_bitMaskToActions = new Dictionary<int, ActionDefinition>();
    private Dictionary<string, int> m_actionIdsToBitMasks = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> m_objectIdToToken = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.ObjectId);
    private Dictionary<string, string> m_tokenToObjectId = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.ObjectId);
    private Dictionary<string, Guid> m_objectIdToProjectId = new Dictionary<string, Guid>((IEqualityComparer<string>) TFStringComparer.ObjectId);
    private ReaderWriterLock m_accessLock = new ReaderWriterLock();

    public AuthorizationSecurityClass(
      IVssRequestContext requestContext,
      string classId,
      LocalSecurityNamespace classSecurity,
      List<SecurityObject> objects)
    {
      this.m_identityService = requestContext.GetService<TeamFoundationIdentityService>();
      this.m_eventService = requestContext.GetService<TeamFoundationEventService>();
      this.m_classId = classId;
      this.m_classSecurity = classSecurity;
      foreach (ActionDefinition action in classSecurity.Description.Actions)
      {
        this.m_actionIdsToBitMasks[action.Name] = action.Bit;
        this.m_bitMaskToActions[action.Bit] = action;
      }
      foreach (SecurityObject securityObject in objects)
      {
        this.m_objectIdToToken[securityObject.ObjectId] = securityObject.SecurityToken;
        this.m_objectIdToProjectId[securityObject.ObjectId] = securityObject.ProjectId;
        this.m_tokenToObjectId[securityObject.SecurityToken] = securityObject.ObjectId;
      }
    }

    public string ClassId => this.m_classId;

    public LocalSecurityNamespace SecurityNamespace => this.m_classSecurity;

    public string[] DisplayableActions
    {
      get
      {
        List<string> stringList = new List<string>();
        foreach (ActionDefinition actionDefinition in this.m_bitMaskToActions.Values)
        {
          if (!StringComparer.OrdinalIgnoreCase.Equals("TRIGGER_EVENT", actionDefinition.Name) && !StringComparer.OrdinalIgnoreCase.Equals("SYNCHRONIZE_READ", actionDefinition.Name))
            stringList.Add(actionDefinition.Name);
        }
        return stringList.ToArray();
      }
    }

    public void UpdateAccessControlEntries(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] aces,
      bool replace)
    {
      string securityToken = this.GetSecurityToken(requestContext, objectId);
      LegacyEleadSecurityUtility.UpdateAccessControlEntries(requestContext, (IVssSecurityNamespace) this.m_classSecurity, securityToken, aces, objectId, replace);
    }

    public void RemoveAccessControlEntry(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry ace)
    {
      string securityToken = this.GetSecurityToken(requestContext, objectId);
      LegacyEleadSecurityUtility.RemoveAccessControlEntry(requestContext, (IVssSecurityNamespace) this.m_classSecurity, securityToken, ace);
    }

    public bool HasPermission(
      IVssRequestContext requestContext,
      string objectId,
      string actionId,
      IdentityDescriptor descriptor)
    {
      string securityToken = this.GetSecurityToken(requestContext, objectId);
      int requestedPermissions;
      if (!this.m_actionIdsToBitMasks.TryGetValue(actionId, out requestedPermissions))
        throw new GroupSecuritySubsystemServiceException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_SECURITYACTIONDOESNOTEXISTERROR((object) actionId));
      requestContext.GetService<TeamFoundationHostManagementService>();
      using (IVssRequestContext userContext = requestContext.CreateUserContext(descriptor))
        return this.m_classSecurity.HasPermission(userContext, securityToken, requestedPermissions);
    }

    public void CheckPermission(
      IVssRequestContext requestContext,
      string objectId,
      string actionId)
    {
      string securityToken = this.GetSecurityToken(requestContext, objectId);
      int requestedPermissions;
      if (!this.m_actionIdsToBitMasks.TryGetValue(actionId, out requestedPermissions))
        throw new GroupSecuritySubsystemServiceException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_SECURITYACTIONDOESNOTEXISTERROR((object) actionId));
      this.m_classSecurity.CheckPermission(requestContext, securityToken, requestedPermissions);
    }

    public IdentityDescriptor GetAdminGroupForObject(
      IVssRequestContext requestContext,
      string objectId)
    {
      if (objectId != null)
      {
        if (!TFStringComparer.ObjectId.Equals(objectId, PermissionNamespaces.Global))
        {
          try
          {
            this.m_accessLock.AcquireReaderLock(-1);
            Guid id;
            if (!this.m_objectIdToProjectId.TryGetValue(objectId, out id))
            {
              this.m_accessLock.ReleaseReaderLock();
              this.CheckForObjectExistence(requestContext, objectId, (string) null);
              this.m_accessLock.AcquireReaderLock(-1);
              this.m_objectIdToProjectId.TryGetValue(objectId, out id);
            }
            if (id == Guid.Empty)
              return (IdentityDescriptor) null;
            string projectUri = ProjectInfo.GetProjectUri(id);
            return IdentityHelper.CreateTeamFoundationDescriptor(this.m_identityService.GetProjectAdminSid(requestContext.Elevate(), projectUri));
          }
          finally
          {
            if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
              this.m_accessLock.ReleaseReaderLock();
          }
        }
      }
      return (IdentityDescriptor) null;
    }

    public Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] ReadAccessControlList(
      IVssRequestContext requestContext,
      string objectId)
    {
      string securityToken = this.GetSecurityToken(requestContext, objectId);
      return LegacyEleadSecurityUtility.GetAuthorizationStyleACEsFromACL(this.m_classSecurity.QueryAccessControlList(requestContext, securityToken, (IEnumerable<IdentityDescriptor>) null, false), this.m_bitMaskToActions);
    }

    public void RegisterObject(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      string objectId,
      string objectClassId,
      string projectUri,
      string parentObjectId)
    {
      Guid projectId;
      if (!this.TryParseProjectId(projectUri, out projectId))
        throw new RegisterObjectNoProjectException(projectUri);
      TeamFoundationIdentity foundationIdentity = this.ReadIdentity(requestContext, descriptor);
      string key = (string) null;
      this.m_classSecurity.GetCurrentSequenceId(requestContext);
      using (AuthorizationComponent authorizationComponent = this.GetAuthorizationComponent(requestContext))
        key = authorizationComponent.RegisterObject(objectId, objectClassId, projectId, parentObjectId, foundationIdentity.TeamFoundationId).GetCurrent<string>().Items[0];
      this.m_classSecurity.OnDataChanged(requestContext);
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        this.m_objectIdToToken[objectId] = key;
        this.m_objectIdToProjectId[objectId] = projectId;
        this.m_tokenToObjectId[key] = objectId;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public List<string> UnregisterObject(IVssRequestContext requestContext, string objectId)
    {
      string securityToken = this.GetSecurityToken(requestContext, objectId);
      this.m_classSecurity.GetCurrentSequenceId(requestContext);
      using (AuthorizationComponent authorizationComponent = this.GetAuthorizationComponent(requestContext))
        authorizationComponent.UnregisterObject(objectId, securityToken);
      this.m_classSecurity.OnDataChanged(requestContext);
      List<string> stringList = new List<string>();
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        foreach (KeyValuePair<string, string> keyValuePair in this.m_objectIdToToken)
        {
          if (TFStringComparer.ObjectId.StartsWith(keyValuePair.Value, securityToken))
            stringList.Add(keyValuePair.Key);
        }
        foreach (string key1 in stringList)
        {
          string key2;
          if (this.m_objectIdToToken.TryGetValue(key1, out key2))
          {
            this.m_objectIdToToken.Remove(key1);
            this.m_tokenToObjectId.Remove(key2);
          }
        }
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
      return stringList;
    }

    public void ResetInheritance(
      IVssRequestContext requestContext,
      string objectId,
      string parentObjectId)
    {
      this.GetSecurityToken(requestContext, objectId);
      List<SecurityObject> securityObjectList = (List<SecurityObject>) null;
      using (AuthorizationComponent authorizationComponent = this.GetAuthorizationComponent(requestContext))
        securityObjectList = authorizationComponent.ResetInheritance(objectId, parentObjectId, this.m_classSecurity.Description.NamespaceId).GetCurrent<SecurityObject>().Items;
      this.m_classSecurity.OnDataChanged(requestContext);
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        foreach (SecurityObject securityObject in securityObjectList)
        {
          this.m_objectIdToToken[securityObject.ObjectId] = securityObject.SecurityToken;
          this.m_tokenToObjectId[securityObject.SecurityToken] = securityObject.ObjectId;
        }
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void SecurityObjectCreated(
      IVssRequestContext requestContext,
      string objectId,
      string securityToken,
      string parentId)
    {
      try
      {
        this.m_classSecurity.OnDataChanged(requestContext);
        this.m_accessLock.AcquireWriterLock(-1);
        this.m_objectIdToToken[objectId] = securityToken;
        this.m_objectIdToProjectId[objectId] = this.m_objectIdToProjectId[parentId];
        this.m_tokenToObjectId[securityToken.TrimEnd(AuthorizationSecurityConstants.SeparatorChar)] = objectId;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void SecurityObjectCreated(
      IVssRequestContext requestContext,
      string objectId,
      string securityToken,
      Guid projectId)
    {
      try
      {
        this.m_classSecurity.OnDataChanged(requestContext);
        this.m_accessLock.AcquireWriterLock(-1);
        this.m_objectIdToToken[objectId] = securityToken;
        this.m_objectIdToProjectId[objectId] = projectId;
        this.m_tokenToObjectId[securityToken.TrimEnd(AuthorizationSecurityConstants.SeparatorChar)] = objectId;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void SecurityObjectDeleted(IVssRequestContext requestContext, string objectId)
    {
      try
      {
        this.m_classSecurity.OnDataChanged(requestContext);
        this.m_accessLock.AcquireWriterLock(-1);
        string key;
        if (this.m_objectIdToToken.TryGetValue(objectId, out key))
        {
          this.m_objectIdToToken.Remove(objectId);
          this.m_tokenToObjectId.Remove(key);
        }
        this.m_objectIdToProjectId.Remove(objectId);
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    internal string GetObjectId(IVssRequestContext requestContext, string securityToken)
    {
      if (securityToken == null)
        return (string) null;
      try
      {
        string objectId;
        if (!this.TryGetFrameworkObjectId(requestContext, securityToken, out objectId))
        {
          this.m_accessLock.AcquireReaderLock(-1);
          securityToken = securityToken.TrimEnd(AuthorizationSecurityConstants.SeparatorChar);
          if (!this.m_tokenToObjectId.TryGetValue(securityToken, out objectId))
          {
            this.m_accessLock.ReleaseReaderLock();
            this.CheckForObjectExistence(requestContext, (string) null, securityToken);
            this.m_accessLock.AcquireReaderLock(-1);
            this.m_tokenToObjectId.TryGetValue(securityToken, out objectId);
          }
        }
        return objectId;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private bool TryGetFrameworkObjectId(
      IVssRequestContext requestContext,
      string securityToken,
      out string objectId)
    {
      if (securityToken.Equals(FrameworkSecurity.TeamProjectCollectionNamespaceToken))
      {
        objectId = PermissionNamespaces.Global;
        return true;
      }
      if (securityToken.StartsWith(PermissionNamespaces.Project))
      {
        objectId = securityToken.TrimEnd(AuthorizationSecurityConstants.SeparatorChar);
        return true;
      }
      objectId = (string) null;
      return false;
    }

    private string GetSecurityToken(IVssRequestContext requestContext, string objectId)
    {
      if (objectId == null)
        return (string) null;
      string securityToken;
      if (!this.TryGetSecurityToken(requestContext, objectId, out securityToken))
        throw new GroupSecuritySubsystemServiceException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_SECURITYOBJECTDOESNOTEXISTERROR((object) objectId));
      return securityToken;
    }

    internal bool TryGetSecurityToken(
      IVssRequestContext requestContext,
      string objectId,
      out string securityToken)
    {
      if (objectId == null)
      {
        securityToken = (string) null;
        return false;
      }
      try
      {
        if (!this.TryGetFrameworkSecurityToken(requestContext, objectId, out securityToken))
        {
          this.m_accessLock.AcquireReaderLock(-1);
          if (!this.m_objectIdToToken.TryGetValue(objectId, out securityToken))
          {
            this.m_accessLock.ReleaseReaderLock();
            this.CheckForObjectExistence(requestContext, objectId, (string) null);
            this.m_accessLock.AcquireReaderLock(-1);
            return this.m_objectIdToToken.TryGetValue(objectId, out securityToken);
          }
        }
        return true;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private bool TryGetFrameworkSecurityToken(
      IVssRequestContext requestContext,
      string objectId,
      out string securityToken)
    {
      if (objectId.Equals(PermissionNamespaces.Global))
      {
        securityToken = FrameworkSecurity.TeamProjectCollectionNamespaceToken;
        return true;
      }
      if (objectId.StartsWith(PermissionNamespaces.Project))
      {
        securityToken = objectId + ":";
        return true;
      }
      securityToken = (string) null;
      return false;
    }

    private void CheckForObjectExistence(
      IVssRequestContext requestContext,
      string objectId,
      string securityToken)
    {
      SecurityObject securityObject;
      using (AuthorizationComponent authorizationComponent = this.GetAuthorizationComponent(requestContext.Elevate()))
        securityObject = authorizationComponent.QueryAuthorizationObjects(objectId, securityToken).GetCurrent<SecurityObject>().Items.FirstOrDefault<SecurityObject>();
      if (securityObject == null)
        return;
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        this.m_objectIdToToken[securityObject.ObjectId] = securityObject.SecurityToken;
        this.m_objectIdToProjectId[securityObject.ObjectId] = securityObject.ProjectId;
        this.m_tokenToObjectId[securityObject.SecurityToken.TrimEnd(AuthorizationSecurityConstants.SeparatorChar)] = securityObject.ObjectId;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    private bool TryParseProjectId(string projectUri, out Guid projectId)
    {
      if (projectUri.Equals(string.Empty))
      {
        projectId = Guid.Empty;
        return true;
      }
      try
      {
        ArtifactId artifactId = LinkingUtilities.DecodeUri(projectUri);
        if (artifactId != null)
        {
          if (artifactId.ToolSpecificId != null)
          {
            projectId = new Guid(artifactId.ToolSpecificId);
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentException _:
          case FormatException _:
label_7:
            throw;
          default:
            OverflowException overflowException = ex as OverflowException;
            goto label_7;
        }
      }
      projectId = Guid.Empty;
      return false;
    }

    private ActionDefinition GetActionForBitMask(int bitMask)
    {
      ActionDefinition actionForBitMask;
      this.m_bitMaskToActions.TryGetValue(bitMask, out actionForBitMask);
      return actionForBitMask;
    }

    private TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      TeamFoundationIdentity identity = this.m_identityService.ReadIdentity(requestContext, descriptor, MembershipQuery.None, ReadIdentityOptions.None);
      if (identity == null)
        throw new IdentityNotFoundException(descriptor);
      this.Validate(requestContext, identity);
      return identity;
    }

    private void Validate(IVssRequestContext requestContext, TeamFoundationIdentity identity)
    {
      if (!identity.IsActive)
        throw new ActiveDirectoryAccessException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_ACTIVE_DIRECTORY_NO_IDENTITY_EXCEPTION());
      if (string.IsNullOrEmpty(identity.GetAttribute("SecurityGroup", (string) null)) && identity.IsContainer && identity.Descriptor.IdentityType == "System.Security.Principal.WindowsIdentity")
        throw new NotASecurityGroupException(identity.DisplayName);
    }

    private AuthorizationComponent GetAuthorizationComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<AuthorizationComponent>();
  }
}
