// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNoteComponent3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CheckinNoteComponent3 : CheckinNoteComponent2
  {
    public override void CreateDefinition(
      string associatedServerItem,
      CheckinNoteFieldDefinition[] checkinNoteFields)
    {
      this.PrepareStoredProcedure("prc_CreateReleaseNoteDefinition");
      this.BindDataspaceIdAndServerItem("@itemDataspaceId", "@associatedItem", associatedServerItem, false);
      this.BindCheckinNoteFieldDefinitionTable("@definitionList", (IEnumerable<CheckinNoteFieldDefinition>) checkinNoteFields);
      this.ExecuteNonQuery();
    }

    public override ResultCollection QueryDefinition(List<string> associatedServerItemList)
    {
      List<string> rows = new List<string>(associatedServerItemList.Count);
      foreach (string associatedServerItem in associatedServerItemList)
      {
        string pathWithProjectGuid = this.ConvertToPathWithProjectGuid(associatedServerItem);
        rows.Add(DBPath.ServerToDatabasePath(pathWithProjectGuid));
      }
      this.PrepareStoredProcedure("prc_QueryReleaseNoteDefinition");
      this.BindStringTable("@associatedItems", (IEnumerable<string>) rows);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<CheckinNoteFieldDefinition>((ObjectBinder<CheckinNoteFieldDefinition>) new CheckinNoteFieldDefinitionColumns3((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }
  }
}
