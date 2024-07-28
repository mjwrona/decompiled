// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.AuthorizationService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Server;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Proxy
{
  internal class AuthorizationService : IAuthorizationService
  {
    private Microsoft.TeamFoundation.Server.AuthorizationService proxy;

    internal AuthorizationService(TfsTeamProjectCollection tfs, string url) => this.proxy = new Microsoft.TeamFoundation.Server.AuthorizationService(tfs, url);

    internal AuthorizationService(string url)
      : this((TfsTeamProjectCollection) null, url)
    {
    }

    public void RegisterObject(
      string objectId,
      string objectClassId,
      string projectUri,
      string parentId)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckObjectClassId(ref objectClassId, nameof (objectClassId));
        SecurityValidation.CheckProjectUri(ref projectUri, true, nameof (projectUri));
        this.proxy.RegisterObject(objectId, objectClassId, projectUri, parentId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void UnregisterObject(string objectId)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        this.proxy.UnregisterObject(objectId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void ResetInheritance(string objectId, string parentObjectId)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckObjectId(ref parentObjectId, true, nameof (parentObjectId));
        SecurityValidation.CheckParentObjectIdNotSelf(objectId, parentObjectId, nameof (parentObjectId));
        this.proxy.ResetInheritance(objectId, parentObjectId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string GetObjectClass(string objectId)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        return this.proxy.GetObjectClass(objectId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string[] ListObjectClasses()
    {
      try
      {
        return this.proxy.ListObjectClasses();
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string[] ListObjectClassActions(string objectClassId)
    {
      try
      {
        SecurityValidation.CheckObjectClassId(ref objectClassId, nameof (objectClassId));
        return this.proxy.ListObjectClassActions(objectClassId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string[] ListLocalizedActionNames(string objectClassId, string[] actionId)
    {
      try
      {
        SecurityValidation.CheckObjectClassId(ref objectClassId, nameof (objectClassId));
        SecurityValidation.CheckActionIdArray(actionId, nameof (actionId));
        return this.proxy.ListLocalizedActionNames(objectClassId, actionId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void AddAccessControlEntry(string objectId, AccessControlEntry ace)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckAccessControlEntry(ace, nameof (ace));
        this.proxy.AddAccessControlEntry(objectId, ace);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void RemoveAccessControlEntry(string objectId, AccessControlEntry ace)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckAccessControlEntry(ace, nameof (ace));
        this.proxy.RemoveAccessControlEntry(objectId, ace);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string GetChangedAccessControlEntries(int sequence_id)
    {
      try
      {
        SecurityValidation.CheckSequenceId(sequence_id, nameof (sequence_id));
        return this.proxy.GetChangedAccessControlEntries(sequence_id);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void ReplaceAccessControlList(string objectId, AccessControlEntry[] acl)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckAccessControlEntryArray(acl, nameof (acl));
        this.proxy.ReplaceAccessControlList(objectId, acl);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public AccessControlEntry[] ReadAccessControlList(string objectId)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        return this.proxy.ReadAccessControlList(objectId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public AccessControlEntry[][] ReadAccessControlLists(string[] objectId)
    {
      try
      {
        SecurityValidation.CheckObjectIdArray(objectId, nameof (objectId));
        return this.proxy.ReadAccessControlLists(objectId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void CheckPermission(string objectId, string actionId)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckActionId(ref actionId, nameof (actionId));
        this.proxy.CheckPermission(objectId, actionId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public bool[] IsPermitted(string[] objectId, string actionId, string userSid)
    {
      try
      {
        SecurityValidation.CheckObjectIdArray(objectId, nameof (objectId));
        SecurityValidation.CheckActionId(ref actionId, nameof (actionId));
        SecurityValidation.CheckSid(ref userSid, nameof (userSid));
        return this.proxy.IsPermittedByObjectList(objectId, actionId, userSid);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public bool[] IsPermitted(string objectId, string[] actionId, string userSid)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckActionIdArray(actionId, nameof (actionId));
        SecurityValidation.CheckSid(ref userSid, nameof (userSid));
        return this.proxy.IsPermittedByActionList(objectId, actionId, userSid);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public bool[] IsPermitted(string objectId, string actionId, string[] userSid)
    {
      try
      {
        SecurityValidation.CheckObjectId(ref objectId, false, nameof (objectId));
        SecurityValidation.CheckActionId(ref actionId, nameof (actionId));
        SecurityValidation.CheckSidArray(userSid, nameof (userSid));
        return this.proxy.IsPermittedBySidList(objectId, actionId, userSid);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }
  }
}
