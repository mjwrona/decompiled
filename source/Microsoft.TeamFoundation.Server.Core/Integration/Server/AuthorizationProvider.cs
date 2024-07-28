// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.AuthorizationProvider
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
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Integration.Server
{
  public sealed class AuthorizationProvider : IAuthorizationProvider, IVssFrameworkService
  {
    private bool m_cacheFresh;
    private Dictionary<string, AuthorizationSecurityClass> m_objectIdToClassMappings;
    private Dictionary<string, AuthorizationSecurityClass> m_classIdToClassMappings;
    private Dictionary<Guid, AuthorizationSecurityClass> m_namespaceIdToClassMappings;
    private ReaderWriterLock m_accessLock = new ReaderWriterLock();
    private static readonly IReadOnlyDictionary<Guid, string> s_namespaceIdToClass = (IReadOnlyDictionary<Guid, string>) new Dictionary<Guid, string>()
    {
      {
        FrameworkSecurity.TeamProjectCollectionNamespaceId,
        "NAMESPACE"
      },
      {
        FrameworkSecurity.TeamProjectNamespaceId,
        "PROJECT"
      },
      {
        FrameworkSecurity.EventSubscriptionNamespaceId,
        "EVENT_SUBSCRIPTION"
      },
      {
        AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid,
        "CSS_NODE"
      },
      {
        AuthorizationSecurityConstants.IterationNodeSecurityGuid,
        "ITERATION_NODE"
      }
    };

    internal AuthorizationProvider()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", EleadSqlNotifications.CSSNodeReparented, new SqlNotificationHandler(this.OnCSSNodeReparented), true);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", EleadSqlNotifications.CSSNodeReparented, new SqlNotificationHandler(this.OnCSSNodeReparented), true);

    public string GetObjectClass(IVssRequestContext requestContext, string objectId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting GetObjectClass('{0}')", (object) objectId);
      this.EnsureDataLoaded(requestContext);
      return this.GetSecurityClassFromObjectId(requestContext, objectId).ClassId;
    }

    public string[] ListObjectClassActions(IVssRequestContext requestContext, string objectClassId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting ListObjectClassActions('{0}')", (object) objectClassId);
      this.EnsureDataLoaded(requestContext);
      return (this.TryGetSecurityClassFromClassId(objectClassId) ?? throw new GroupSecuritySubsystemServiceException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_CLASSIDDOESNOTEXISTERROR((object) objectClassId))).DisplayableActions;
    }

    public string[] ListLocalizedActionNames(string objectClassId, string[] actionId) => LegacyEleadSecurityUtility.ListLocalizedActionNames(objectClassId, actionId);

    public void ReplaceAccessControlList(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] aces)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting ReplaceAccessControlList('{0}', aces[])", (object) objectId);
      this.EnsureDataLoaded(requestContext);
      this.GetSecurityClassFromObjectId(requestContext, objectId).UpdateAccessControlEntries(requestContext, objectId, aces, true);
    }

    public void AddAccessControlEntry(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry ace)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting AddAccessControlEntry('{0}', {1})", (object) objectId, (object) ace.ToString());
      this.EnsureDataLoaded(requestContext);
      this.GetSecurityClassFromObjectId(requestContext, objectId).UpdateAccessControlEntries(requestContext, objectId, new Microsoft.Azure.Boards.CssNodes.AccessControlEntry[1]
      {
        ace
      }, false);
    }

    public void AddAccessControlEntries(
      IVssRequestContext requestContext,
      string objectId,
      IEnumerable<Microsoft.Azure.Boards.CssNodes.AccessControlEntry> aces)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting AddAccessControlEntries('{0}')", (object) objectId);
      this.EnsureDataLoaded(requestContext);
      this.GetSecurityClassFromObjectId(requestContext, objectId).UpdateAccessControlEntries(requestContext, objectId, aces.ToArray<Microsoft.Azure.Boards.CssNodes.AccessControlEntry>(), false);
    }

    public void RemoveAccessControlEntry(
      IVssRequestContext requestContext,
      string objectId,
      Microsoft.Azure.Boards.CssNodes.AccessControlEntry ace)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting RemoveAccessControlEntry('{0}', {1})", (object) objectId, (object) ace.ToString());
      this.EnsureDataLoaded(requestContext);
      this.GetSecurityClassFromObjectId(requestContext, objectId).RemoveAccessControlEntry(requestContext, objectId, ace);
    }

    public string GetChangedAccessControlEntries(
      IVssRequestContext requestContext,
      int startSequenceId)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting GetChangedAccessControlEntries('{0}')", (object) startSequenceId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        int newer;
        List<SecurityChange> items;
        using (AuthorizationComponent authorizationComponent = this.GetAuthorizationComponent(requestContext))
        {
          using (ResultCollection changedAcls = authorizationComponent.SecurityGetChangedAcls(startSequenceId))
          {
            newer = changedAcls.GetCurrent<int>().Items[0];
            changedAcls.NextResult();
            items = changedAcls.GetCurrent<SecurityChange>().Items;
          }
        }
        Dictionary<Guid, IdentityDescriptor> dictionary1 = new Dictionary<Guid, IdentityDescriptor>();
        foreach (SecurityChange securityChange in items)
        {
          if (securityChange.TeamFoundationId != Guid.Empty)
            dictionary1[securityChange.TeamFoundationId] = (IdentityDescriptor) null;
        }
        TeamFoundationIdentityService service = requestContext.Elevate().GetService<TeamFoundationIdentityService>();
        Guid[] array = dictionary1.Keys.ToArray<Guid>();
        IVssRequestContext requestContext1 = requestContext;
        Guid[] teamFoundationIds = array;
        TeamFoundationIdentity[] foundationIdentityArray = service.ReadIdentities(requestContext1, teamFoundationIds);
        for (int index = 0; index < foundationIdentityArray.Length; ++index)
        {
          TeamFoundationIdentity foundationIdentity = foundationIdentityArray[index];
          if (foundationIdentity != null)
            dictionary1[foundationIdentity.TeamFoundationId] = foundationIdentity.Descriptor;
          else
            TeamFoundationTrace.Warning("Tfid {0} could not be found. Some security changes will be skipped.", (object) array[index]);
        }
        StringBuilder output = new StringBuilder(1000 + items.Count * 250);
        XmlWriter xmlWriter = XmlWriter.Create(output);
        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("SecurityChanges");
        xmlWriter.WriteAttributeString("MaxSequence", newer.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        xmlWriter.WriteAttributeString("fMore", "0");
        xmlWriter.WriteStartElement("ACES");
        this.m_accessLock.AcquireReaderLock(-1);
        Dictionary<string, IAccessControlList> dictionary2 = new Dictionary<string, IAccessControlList>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (SecurityChange securityChange in items)
        {
          if (!(securityChange.TeamFoundationId == Guid.Empty) && !(dictionary1[securityChange.TeamFoundationId] == (IdentityDescriptor) null))
          {
            AuthorizationSecurityClass idToClassMapping = this.m_namespaceIdToClassMappings[securityChange.NamespaceId];
            if (idToClassMapping.SecurityNamespace.GetCurrentSequenceId(requestContext).IsSupersededBy((TokenStoreSequenceId) (long) newer))
              idToClassMapping.SecurityNamespace.OnDataChanged(requestContext);
            string objectId = idToClassMapping.GetObjectId(requestContext, securityChange.Token);
            if (string.IsNullOrEmpty(objectId) && (securityChange.NamespaceId == AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid || securityChange.NamespaceId == AuthorizationSecurityConstants.IterationNodeSecurityGuid))
            {
              string[] separator = new string[1]
              {
                LinkingUtilities.VSTFS
              };
              string[] strArray = securityChange.Token.Split(separator, StringSplitOptions.None);
              objectId = LinkingUtilities.VSTFS + strArray[strArray.Length - 1].TrimEnd(AuthorizationSecurityConstants.SeparatorChar);
            }
            if (!string.IsNullOrEmpty(objectId))
            {
              string key = securityChange.NamespaceId.ToString() + "~" + securityChange.Token;
              IAccessControlList accessControlList;
              if (!dictionary2.TryGetValue(key, out accessControlList))
              {
                accessControlList = idToClassMapping.SecurityNamespace.QueryAccessControlList(requestContext, securityChange.Token, (IEnumerable<IdentityDescriptor>) null, false);
                dictionary2[key] = accessControlList;
              }
              IdentityDescriptor descriptor = dictionary1[securityChange.TeamFoundationId];
              IAccessControlEntry accessControlEntry = accessControlList.QueryAccessControlEntry(descriptor);
              foreach (ActionDefinition action in idToClassMapping.SecurityNamespace.Description.Actions)
              {
                bool deny = (accessControlEntry.Deny & action.Bit) != 0;
                bool flag = (accessControlEntry.Allow & action.Bit) != 0;
                if (deny | flag)
                {
                  AuthorizationProvider.WriteACEXML(xmlWriter, objectId, descriptor, action, deny, false);
                }
                else
                {
                  AuthorizationProvider.WriteACEXML(xmlWriter, objectId, descriptor, action, true, true);
                  AuthorizationProvider.WriteACEXML(xmlWriter, objectId, descriptor, action, false, true);
                }
              }
            }
          }
        }
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
        return output.ToString();
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private static void WriteACEXML(
      XmlWriter xmlWriter,
      string objectId,
      IdentityDescriptor descriptor,
      ActionDefinition action,
      bool deny,
      bool deleted)
    {
      xmlWriter.WriteStartElement("ACE");
      xmlWriter.WriteAttributeString("ObjectId", objectId);
      xmlWriter.WriteAttributeString("SID", descriptor.Identifier);
      xmlWriter.WriteAttributeString("ActionId", action.Name);
      xmlWriter.WriteAttributeString("Deny", deny.ToString());
      xmlWriter.WriteAttributeString("Deleted", deleted.ToString());
      xmlWriter.WriteEndElement();
    }

    public bool IsPermitted(
      IVssRequestContext requestContext,
      string objectId,
      string actionId,
      IdentityDescriptor descriptor)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting IsPermitted('{0}', '{1}', '{2}')", (object) objectId, (object) actionId, (object) descriptor);
      this.EnsureDataLoaded(requestContext);
      return this.GetSecurityClassFromObjectId(requestContext, objectId).HasPermission(requestContext, objectId, actionId, descriptor);
    }

    public void EnsurePermitted(
      IVssRequestContext requestContext,
      string objectId,
      string actionId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting EnsurePermitted('{0}', '{1}', '{2}')", (object) objectId, (object) actionId, (object) requestContext.UserContext);
      this.EnsureDataLoaded(requestContext);
      this.GetSecurityClassFromObjectId(requestContext, objectId).CheckPermission(requestContext, objectId, actionId);
    }

    public Microsoft.Azure.Boards.CssNodes.AccessControlEntry[] ReadAccessControlList(
      IVssRequestContext requestContext,
      string objectId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting ReadAccessControlList('{0}')", (object) objectId);
      this.EnsureDataLoaded(requestContext);
      return this.GetSecurityClassFromObjectId(requestContext, objectId).ReadAccessControlList(requestContext, objectId);
    }

    public string[] ListObjectClasses(IVssRequestContext requestContext)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting ReadAccessControlList()");
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        List<string> stringList = new List<string>();
        foreach (string key in this.m_classIdToClassMappings.Keys)
          stringList.Add(key);
        return stringList.ToArray();
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public void RegisterObject(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      string objectId,
      string objectClassId,
      string projectUri,
      string parentObjectId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting RegisterObject('{0}', '{1}', '{2}', '{3}', '{4}')", (object) descriptor, (object) objectId, (object) objectClassId, projectUri == null ? (object) "(null)" : (object) projectUri, parentObjectId == null ? (object) "(null)" : (object) parentObjectId);
      this.EnsureDataLoaded(requestContext);
      AuthorizationSecurityClass classFromClassId = this.TryGetSecurityClassFromClassId(objectClassId);
      if (classFromClassId == null)
        throw new AuthorizationSubsystemServiceException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_REGISTEROBJECTNOCLASSERROR((object) objectClassId));
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        if (this.m_objectIdToClassMappings.ContainsKey(objectId))
          throw new RegisterObjectExistsException(objectId);
        this.m_accessLock.ReleaseReaderLock();
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
      try
      {
        classFromClassId.RegisterObject(requestContext, descriptor, objectId, objectClassId, projectUri, parentObjectId);
        this.m_accessLock.AcquireWriterLock(-1);
        if (this.m_objectIdToClassMappings.ContainsKey(objectId))
          throw new RegisterObjectExistsException(objectId);
        this.m_objectIdToClassMappings.Add(objectId, this.m_classIdToClassMappings[objectClassId]);
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void UnregisterObject(IVssRequestContext requestContext, string objectId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting UnregisterObject('{0}')", (object) objectId);
      this.EnsureDataLoaded(requestContext);
      List<string> stringList = this.GetSecurityClassFromObjectId(requestContext, objectId).UnregisterObject(requestContext, objectId);
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        foreach (string key in stringList)
          this.m_objectIdToClassMappings.Remove(key);
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void ResetInheritance(
      IVssRequestContext requestContext,
      string objectId,
      string parentObjectId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting ResetInheritance('{0}', '{1})", (object) objectId, parentObjectId == null ? (object) "(null)" : (object) parentObjectId);
      this.EnsureDataLoaded(requestContext);
      this.GetSecurityClassFromObjectId(requestContext, objectId).ResetInheritance(requestContext, objectId, parentObjectId);
    }

    public void SecurityObjectCreatedByParentId(
      IVssRequestContext requestContext,
      string parentId,
      string objectId,
      string securityToken)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        AuthorizationSecurityClass idToClassMapping = this.m_objectIdToClassMappings[parentId];
        this.m_objectIdToClassMappings[objectId] = idToClassMapping;
        idToClassMapping.SecurityObjectCreated(requestContext, objectId, securityToken, parentId);
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void SecurityObjectCreatedByClassId(
      IVssRequestContext requestContext,
      string classId,
      string objectId,
      string securityToken,
      Guid projectId)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        AuthorizationSecurityClass idToClassMapping = this.m_classIdToClassMappings[classId];
        this.m_objectIdToClassMappings[objectId] = idToClassMapping;
        idToClassMapping.SecurityObjectCreated(requestContext, objectId, securityToken, projectId);
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    internal int GetCurrentSequenceId(IVssRequestContext requestContext)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        return checked ((int) this.m_classIdToClassMappings["NAMESPACE"].SecurityNamespace.GetCurrentSequenceId(requestContext).ToScalarForRestReply());
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public void SecurityObjectDeleted(IVssRequestContext requestContext, string objectId)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        AuthorizationSecurityClass authorizationSecurityClass;
        if (!this.m_objectIdToClassMappings.TryGetValue(objectId, out authorizationSecurityClass))
          return;
        this.m_objectIdToClassMappings.Remove(objectId);
        authorizationSecurityClass.SecurityObjectDeleted(requestContext, objectId);
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void ClearMemoryCache(IVssRequestContext requestContext, string securityClass)
    {
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        if (this.m_classIdToClassMappings == null)
          return;
        if (string.IsNullOrEmpty(securityClass))
        {
          foreach (AuthorizationSecurityClass authorizationSecurityClass in this.m_classIdToClassMappings.Values)
            authorizationSecurityClass.SecurityNamespace.OnDataChanged(requestContext);
        }
        else
        {
          AuthorizationSecurityClass authorizationSecurityClass;
          if (!this.m_classIdToClassMappings.TryGetValue(securityClass, out authorizationSecurityClass))
            return;
          authorizationSecurityClass.SecurityNamespace.OnDataChanged(requestContext);
        }
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
        this.m_cacheFresh = false;
      }
    }

    public string GetObjectId(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string securityToken)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        return this.m_namespaceIdToClassMappings[namespaceId].GetObjectId(requestContext, securityToken);
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public string GetSecurityToken(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string objectId)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        string securityToken;
        Guid result;
        if (!this.m_namespaceIdToClassMappings[namespaceId].TryGetSecurityToken(requestContext, objectId, out securityToken) && (namespaceId == AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid || namespaceId == AuthorizationSecurityConstants.IterationNodeSecurityGuid) && LinkingUtilities.IsUriWellFormed(objectId) && Guid.TryParse(LinkingUtilities.DecodeUri(objectId).ToolSpecificId, out result))
          securityToken = requestContext.GetService<CommonStructureService>().GetNodeSecurityToken(requestContext, result);
        return securityToken;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public IdentityDescriptor GetAdminGroupForObjectId(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string objectId)
    {
      this.EnsureDataLoaded(requestContext);
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        return this.m_namespaceIdToClassMappings[namespaceId].GetAdminGroupForObject(requestContext, objectId);
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private AuthorizationSecurityClass TryGetSecurityClassFromClassId(string classId)
    {
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        AuthorizationSecurityClass authorizationSecurityClass;
        return !this.m_classIdToClassMappings.TryGetValue(classId, out authorizationSecurityClass) ? (AuthorizationSecurityClass) null : authorizationSecurityClass;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private AuthorizationSecurityClass GetSecurityClassFromObjectId(
      IVssRequestContext requestContext,
      string objectId)
    {
      try
      {
        AuthorizationSecurityClass securityClass;
        if (!this.TryGetFrameworkSecurityClass(requestContext, objectId, out securityClass))
        {
          this.m_accessLock.AcquireReaderLock(-1);
          if (!this.m_objectIdToClassMappings.TryGetValue(objectId, out securityClass))
          {
            this.m_accessLock.ReleaseReaderLock();
            this.EnsureSpecificObjectLoaded(requestContext, objectId);
            this.m_accessLock.AcquireReaderLock(-1);
            if (!this.m_objectIdToClassMappings.TryGetValue(objectId, out securityClass))
              throw new SecurityObjectDoesNotExistException(objectId);
          }
        }
        return securityClass;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private bool TryGetFrameworkSecurityClass(
      IVssRequestContext requestContext,
      string objectId,
      out AuthorizationSecurityClass securityClass)
    {
      if (objectId.Equals(PermissionNamespaces.Global))
      {
        securityClass = this.m_classIdToClassMappings["NAMESPACE"];
        return true;
      }
      if (objectId.StartsWith(PermissionNamespaces.Project))
      {
        securityClass = this.m_classIdToClassMappings["PROJECT"];
        return true;
      }
      securityClass = (AuthorizationSecurityClass) null;
      return false;
    }

    private void EnsureSpecificObjectLoaded(
      IVssRequestContext requestContext,
      string specificObjectId)
    {
      if (!this.m_cacheFresh)
      {
        this.EnsureDataLoaded(requestContext);
      }
      else
      {
        if (string.IsNullOrEmpty(specificObjectId))
          return;
        try
        {
          this.m_accessLock.AcquireWriterLock(-1);
          SecurityObject securityObject;
          using (AuthorizationComponent authorizationComponent = this.GetAuthorizationComponent(requestContext.Elevate()))
            securityObject = authorizationComponent.QueryAuthorizationObjects(specificObjectId, (string) null).GetCurrent<SecurityObject>().Items.FirstOrDefault<SecurityObject>();
          if (securityObject == null)
            return;
          AuthorizationSecurityClass idToClassMapping = this.m_classIdToClassMappings[securityObject.ClassId];
          idToClassMapping.SecurityObjectCreated(requestContext, securityObject.ObjectId, securityObject.SecurityToken, securityObject.ProjectId);
          this.m_objectIdToClassMappings[securityObject.ObjectId] = idToClassMapping;
        }
        finally
        {
          if (this.m_accessLock.IsWriterLockHeld)
            this.m_accessLock.ReleaseWriterLock();
        }
      }
    }

    private void EnsureDataLoaded(IVssRequestContext requestContext)
    {
      if (this.m_cacheFresh)
        return;
      try
      {
        LocalSecurityService service = requestContext.Elevate().GetService<LocalSecurityService>();
        this.m_accessLock.AcquireWriterLock(-1);
        if (this.m_cacheFresh)
          return;
        this.m_objectIdToClassMappings = new Dictionary<string, AuthorizationSecurityClass>((IEqualityComparer<string>) TFStringComparer.ObjectId);
        this.m_classIdToClassMappings = new Dictionary<string, AuthorizationSecurityClass>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_namespaceIdToClassMappings = new Dictionary<Guid, AuthorizationSecurityClass>();
        List<SecurityObject> items;
        using (AuthorizationComponent authorizationComponent = this.GetAuthorizationComponent(requestContext.Elevate()))
          items = authorizationComponent.QueryAuthorizationObjects((string) null, (string) null).GetCurrent<SecurityObject>().Items;
        Dictionary<string, List<SecurityObject>> dictionary = new Dictionary<string, List<SecurityObject>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (SecurityObject securityObject in items)
        {
          List<SecurityObject> securityObjectList;
          if (!dictionary.TryGetValue(securityObject.ClassId, out securityObjectList))
          {
            securityObjectList = new List<SecurityObject>();
            dictionary[securityObject.ClassId] = securityObjectList;
          }
          securityObjectList.Add(securityObject);
        }
        foreach (KeyValuePair<Guid, string> keyValuePair in (IEnumerable<KeyValuePair<Guid, string>>) AuthorizationProvider.s_namespaceIdToClass)
        {
          string str = keyValuePair.Value;
          LocalSecurityNamespace securityNamespace = service.GetSecurityNamespace(requestContext.Elevate(), keyValuePair.Key);
          List<SecurityObject> objects;
          if (!dictionary.TryGetValue(str, out objects))
            objects = new List<SecurityObject>();
          AuthorizationSecurityClass authorizationSecurityClass = new AuthorizationSecurityClass(requestContext.Elevate(), str, securityNamespace, objects);
          this.m_classIdToClassMappings[authorizationSecurityClass.ClassId] = authorizationSecurityClass;
          this.m_namespaceIdToClassMappings[securityNamespace.Description.NamespaceId] = authorizationSecurityClass;
          foreach (SecurityObject securityObject in objects)
            this.m_objectIdToClassMappings[securityObject.ObjectId] = authorizationSecurityClass;
        }
        this.m_cacheFresh = true;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    private AuthorizationComponent GetAuthorizationComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<AuthorizationComponent>();

    private void OnCSSNodeReparented(IVssRequestContext requestContext, NotificationEventArgs args) => this.m_cacheFresh = false;
  }
}
