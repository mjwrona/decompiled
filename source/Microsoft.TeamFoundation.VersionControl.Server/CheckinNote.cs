// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNote
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class CheckinNote : IValidatable
  {
    private CheckinNoteFieldValue[] m_values;

    internal static void CreateDefinition(
      VersionControlRequestContext versionControlRequestContext,
      string associatedServerItem,
      CheckinNoteFieldDefinition[] checkinNoteFields)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, associatedServerItem);
      if (checkinNoteFields == null)
        checkinNoteFields = Array.Empty<CheckinNoteFieldDefinition>();
      using (CheckinNoteComponent checkinNoteComponent = versionControlRequestContext.VersionControlService.GetCheckinNoteComponent(versionControlRequestContext))
        checkinNoteComponent.CreateDefinition(associatedServerItem, checkinNoteFields);
    }

    internal static List<CheckinNoteFieldDefinition> QueryDefinition(
      VersionControlRequestContext versionControlRequestContext,
      string[] associatedServerItem)
    {
      List<string> associatedServerItemList = new List<string>();
      foreach (string token in associatedServerItem)
      {
        if (versionControlRequestContext.GetRepositorySecurity().HasPermission(versionControlRequestContext.RequestContext, token, 1, false) || versionControlRequestContext.GetRepositorySecurity().HasPermissionForAnyChildren(versionControlRequestContext.RequestContext, token, 1, alwaysAllowAdministrators: false))
          associatedServerItemList.Add(token);
      }
      if (associatedServerItemList.Count == 0)
        return new List<CheckinNoteFieldDefinition>();
      using (CheckinNoteComponent checkinNoteComponent = versionControlRequestContext.VersionControlService.GetCheckinNoteComponent(versionControlRequestContext))
      {
        List<CheckinNoteFieldDefinition> noteFieldDefinitionList = new List<CheckinNoteFieldDefinition>();
        noteFieldDefinitionList.AddRange((IEnumerable<CheckinNoteFieldDefinition>) checkinNoteComponent.QueryDefinition(associatedServerItemList).GetCurrent<CheckinNoteFieldDefinition>().Items);
        return noteFieldDefinitionList;
      }
    }

    internal static void UpdateCheckinNoteFieldName(
      VersionControlRequestContext versionControlRequestContext,
      string path,
      string existingFieldName,
      string newFieldName)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, path);
      using (CheckinNoteComponent checkinNoteComponent = versionControlRequestContext.VersionControlService.GetCheckinNoteComponent(versionControlRequestContext))
        checkinNoteComponent.UpdateCheckinNoteFieldName(existingFieldName, newFieldName);
    }

    internal static List<string> QueryCheckinNoteFieldNames(
      VersionControlRequestContext versionControlRequestContext)
    {
      using (CheckinNoteComponent checkinNoteComponent = versionControlRequestContext.VersionControlService.GetCheckinNoteComponent(versionControlRequestContext))
      {
        List<string> stringList = new List<string>();
        stringList.AddRange((IEnumerable<string>) checkinNoteComponent.QueryCheckinNoteFieldNames().GetCurrent<string>().Items);
        return stringList;
      }
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public CheckinNoteFieldValue[] Values
    {
      get => this.m_values;
      set => this.m_values = value;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.check((IValidatable[]) this.m_values, "Values", true);
      if (this.Values == null || this.Values.Length == 0)
        return;
      List<string> stringList = new List<string>();
      foreach (CheckinNoteFieldValue checkinNoteFieldValue in this.Values)
      {
        if (stringList.Contains(checkinNoteFieldValue.Name))
          throw new DuplicateCheckinNoteFieldException(checkinNoteFieldValue.Name);
        stringList.Add(checkinNoteFieldValue.Name);
      }
    }

    public bool AreMemberWiseEqual(object value)
    {
      if (!(value is CheckinNote checkinNote))
        return false;
      if (this.Values == null && checkinNote.Values == null)
        return true;
      if (this.Values == null || checkinNote.Values == null || this.Values.Length != checkinNote.Values.Length)
        return false;
      Array.Sort<CheckinNoteFieldValue>(this.Values);
      Array.Sort<CheckinNoteFieldValue>(checkinNote.Values);
      for (int index = 0; index < this.Values.Length; ++index)
      {
        CheckinNoteFieldValue checkinNoteFieldValue = this.Values[index];
        CheckinNoteFieldValue other = checkinNote.Values[index];
        if (checkinNoteFieldValue == null && other != null || checkinNoteFieldValue.CompareTo(other) != 0)
          return false;
      }
      return true;
    }
  }
}
