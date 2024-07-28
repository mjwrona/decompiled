// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.IAuthorizationService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Server
{
  public interface IAuthorizationService
  {
    void RegisterObject(
      string objectId,
      string objectClassId,
      string projectUri,
      string parentObjectId);

    void UnregisterObject(string objectId);

    void ResetInheritance(string objectId, string parentObjectId);

    string[] ListObjectClasses();

    string GetObjectClass(string objectId);

    string[] ListObjectClassActions(string objectClassId);

    string[] ListLocalizedActionNames(string objectClassId, string[] actionId);

    void AddAccessControlEntry(string objectId, AccessControlEntry ace);

    void RemoveAccessControlEntry(string objectId, AccessControlEntry ace);

    string GetChangedAccessControlEntries(int sequenceId);

    void ReplaceAccessControlList(string objectId, AccessControlEntry[] acl);

    AccessControlEntry[] ReadAccessControlList(string objectId);

    AccessControlEntry[][] ReadAccessControlLists(string[] objectId);

    void CheckPermission(string objectId, string actionId);

    bool[] IsPermitted(string[] objectId, string actionId, string userSid);

    bool[] IsPermitted(string objectId, string[] actionId, string userSid);

    bool[] IsPermitted(string objectId, string actionId, string[] userSid);
  }
}
