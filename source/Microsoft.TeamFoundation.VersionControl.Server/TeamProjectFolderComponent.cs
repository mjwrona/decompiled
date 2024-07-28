// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TeamProjectFolderComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class TeamProjectFolderComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<TeamProjectFolderComponent>(1),
      (IComponentCreator) new ComponentCreator<TeamProjectFolderComponent5>(5)
    }, "VCTeamProjectFolder");
    private static readonly SqlMetaData[] typ_ReleaseNoteDefinition = new SqlMetaData[3]
    {
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Required", SqlDbType.Bit),
      new SqlMetaData("DisplayOrder", SqlDbType.Int)
    };
    protected static readonly SqlMetaData[] typ_PermissionTable2 = new SqlMetaData[6]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SecurityToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexableToken", SqlDbType.NVarChar, 350L),
      new SqlMetaData("AllowPermission", SqlDbType.Int),
      new SqlMetaData("DenyPermission", SqlDbType.Int)
    };

    public TeamProjectFolderComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected SqlParameter BindCheckinNoteFieldDefinitionTable(
      string parameterName,
      IEnumerable<CheckinNoteFieldDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<CheckinNoteFieldDefinition>();
      System.Func<CheckinNoteFieldDefinition, SqlDataRecord> selector = (System.Func<CheckinNoteFieldDefinition, SqlDataRecord>) (definition =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamProjectFolderComponent.typ_ReleaseNoteDefinition);
        sqlDataRecord.SetString(0, definition.Name);
        sqlDataRecord.SetBoolean(1, definition.Required);
        sqlDataRecord.SetInt32(2, definition.DisplayOrder);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ReleaseNoteDefinition", rows.Select<CheckinNoteFieldDefinition, SqlDataRecord>(selector));
    }

    public TeamProjectFolderComponent.CreationResult CreateProjectFolder(
      Guid teamFoundationId,
      string teamProjectPath,
      string sourceProjectPath,
      string comment,
      bool exclusiveCheckout,
      bool getLatestOnCheckout,
      CheckinNoteFieldDefinition[] checkinNoteDefinition,
      TeamProjectFolderPermission[] permissions,
      PathLength maxServerPathLength)
    {
      try
      {
        this.VersionControlRequestContext.LatestChangeset = 0;
        TeamProjectFolderComponent.CreationResult projectFolder = new TeamProjectFolderComponent.CreationResult();
        XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture));
        List<DatabaseAccessControlEntry> permissionList = new List<DatabaseAccessControlEntry>();
        if (permissions != null)
        {
          foreach (TeamProjectFolderPermission permission in permissions)
          {
            int allowBits = (int) permission.allowBits;
            int denyBits = (int) permission.denyBits;
            permissionList.Add(new DatabaseAccessControlEntry(permission.IdentityId, teamProjectPath, allowBits, denyBits, false));
          }
        }
        this.PrepareStoredProcedure("prc_CreateTeamProject");
        this.BindCreateTeamProjectParameters(teamFoundationId, teamProjectPath, sourceProjectPath, comment, exclusiveCheckout, getLatestOnCheckout, checkinNoteDefinition, permissionList, maxServerPathLength);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
        resultCollection.AddBinder<int>((ObjectBinder<int>) new CheckinColumns());
        ObjectBinder<int> current = resultCollection.GetCurrent<int>();
        current.MoveNext();
        projectFolder.changesetId = current.Current;
        return projectFolder;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    protected SqlParameter BindPermissionsTable(
      string parameterName,
      char separator,
      IEnumerable<DatabaseAccessControlEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<DatabaseAccessControlEntry>();
      System.Func<DatabaseAccessControlEntry, SqlDataRecord> selector = (System.Func<DatabaseAccessControlEntry, SqlDataRecord>) (ace =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamProjectFolderComponent.typ_PermissionTable2);
        int dataspaceId;
        string token = PermissionTable.AddSeparator(separator, this.ConvertToPathWithProjectGuid(ace.Token, out dataspaceId));
        sqlDataRecord.SetInt32(0, dataspaceId);
        sqlDataRecord.SetGuid(1, ace.SubjectId);
        sqlDataRecord.SetString(2, token);
        sqlDataRecord.SetString(3, PermissionTable.GetIndexableTokenFromToken(token, separator));
        sqlDataRecord.SetInt32(4, ace.Allow);
        sqlDataRecord.SetInt32(5, ace.Deny);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_PermissionTable2", rows.Select<DatabaseAccessControlEntry, SqlDataRecord>(selector));
    }

    protected void BindCreateTeamProjectParameters(
      Guid teamFoundationId,
      string teamProjectPath,
      string sourceProjectPath,
      string comment,
      bool exclusiveCheckout,
      bool getLatestOnCheckout,
      CheckinNoteFieldDefinition[] checkinNoteDefinition,
      List<DatabaseAccessControlEntry> permissionList,
      PathLength maxServerPathLength)
    {
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@teamFoundationId", teamFoundationId);
      string teamProjectName = VersionControlPath.GetTeamProjectName(teamProjectPath);
      Guid projectId = ProjectUtility.GetProjectId(this.RequestContext, teamProjectName, false);
      string parameterValue = "$/" + projectId.ToString() + teamProjectPath.Substring(teamProjectName.Length + "$/".Length);
      this.BindInt("@itemDataspaceId", this.GetDataspaceIdDebug(projectId, teamProjectPath));
      this.BindServerItem("@teamProjectPath", parameterValue, false);
      this.BindDataspaceIdAndServerItem("@sourceItemDataspaceId", "@sourceProjectPath", sourceProjectPath, true);
      this.BindString("@comment", comment, 2048, true, SqlDbType.NVarChar);
      this.BindBoolean("@exclusive", exclusiveCheckout);
      this.BindBoolean("@getLatest", getLatestOnCheckout);
      this.BindCheckinNoteFieldDefinitionTable("@definitionList", (IEnumerable<CheckinNoteFieldDefinition>) checkinNoteDefinition);
      this.BindPermissionsTable("@permissionList", '/', (IEnumerable<DatabaseAccessControlEntry>) permissionList);
      this.BindGuid("@author", this.Author);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
    }

    internal class CreationResult
    {
      public int changesetId;
      public string teamProjectResource;
      public List<string> deletedResources = new List<string>();
    }
  }
}
