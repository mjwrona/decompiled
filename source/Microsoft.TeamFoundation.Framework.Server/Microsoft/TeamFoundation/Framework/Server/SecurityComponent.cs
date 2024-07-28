// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[16]
    {
      (IComponentCreator) new ComponentCreator<SecurityComponent>(1),
      (IComponentCreator) new ComponentCreator<SecurityComponent2>(2),
      (IComponentCreator) new ComponentCreator<SecurityComponent3>(3),
      (IComponentCreator) new ComponentCreator<SecurityComponent4>(4),
      (IComponentCreator) new ComponentCreator<SecurityComponent5>(5),
      (IComponentCreator) new ComponentCreator<SecurityComponent6>(6),
      (IComponentCreator) new ComponentCreator<SecurityComponent6>(7),
      (IComponentCreator) new ComponentCreator<SecurityComponent8>(8),
      (IComponentCreator) new ComponentCreator<SecurityComponent9>(9),
      (IComponentCreator) new ComponentCreator<SecurityComponent10>(10),
      (IComponentCreator) new ComponentCreator<SecurityComponent11>(11),
      (IComponentCreator) new ComponentCreator<SecurityComponent12>(12),
      (IComponentCreator) new ComponentCreator<SecurityComponent13>(13),
      (IComponentCreator) new ComponentCreator<SecurityComponent14>(14),
      (IComponentCreator) new ComponentCreator<SecurityComponent15>(15),
      (IComponentCreator) new ComponentCreator<SecurityComponent16>(16)
    }, "Security");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static SecurityComponent()
    {
      SecurityComponent.s_sqlExceptionFactories.Add(800009, new SqlExceptionFactory(typeof (SecurityNamespaceAlreadyExistsException)));
      SecurityComponent.s_sqlExceptionFactories.Add(800043, new SqlExceptionFactory(typeof (InvalidSecurityNamespaceException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) SecurityComponent.s_sqlExceptionFactories;

    protected internal ISecurityDataspaceMapper DataspaceMapper { get; internal set; }

    public virtual void CreateSecurityNamespace(
      SecurityComponent.LocalNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_CreateSecurityNamespace");
      this.BindGuid("@namespaceGuid", description.NamespaceId);
      this.BindString("@name", description.Name, 260, false, SqlDbType.NVarChar);
      this.BindString("@displayName", description.DisplayName, 260, false, SqlDbType.NVarChar);
      this.BindString("@databaseCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindInt("@structure", (int) description.Structure);
      this.BindString("@separatorChar", description.Separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindInt("@elementLength", description.ElementLength);
      this.BindInt("@writePermission", description.WritePermission);
      this.BindInt("@readPermission", description.ReadPermission);
      this.BindString("@extensionType", description.ExtensionType, -1, true, SqlDbType.VarChar);
      TeamFoundationDatabaseXmlWriter xmlWriter = new TeamFoundationDatabaseXmlWriter("actions");
      if (description.Actions != null)
      {
        foreach (SecurityComponent.NamespaceAction action in description.Actions)
        {
          xmlWriter.WriteStartElement("a");
          xmlWriter.WriteAttributeString("b", action.Bit.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          xmlWriter.WriteAttributeString("d", action.DisplayName);
          xmlWriter.WriteAttributeString("n", action.Name);
          xmlWriter.WriteEndElement();
        }
      }
      this.BindXml("@actionXML", xmlWriter);
      this.BindGuid("@writerIdentifier", this.Author);
      if (description.SystemBitMask != 0)
        throw new NotSupportedException();
      this.ExecuteNonQuery();
    }

    public virtual void CreateRemoteSecurityNamespace(
      SecurityComponent.RemoteNamespaceDescription description)
    {
      throw new NotImplementedException();
    }

    public virtual void UpdateSecurityNamespace(
      SecurityComponent.LocalNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_UpdateSecurityNamespace");
      this.BindGuid("@namespaceGuid", description.NamespaceId);
      this.BindString("@name", description.Name, 260, false, SqlDbType.NVarChar);
      this.BindString("@displayName", description.DisplayName, 260, false, SqlDbType.NVarChar);
      this.BindString("@databaseCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindInt("@structure", (int) description.Structure);
      this.BindString("@separatorChar", description.Separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindInt("@elementLength", description.ElementLength);
      this.BindInt("@writePermission", description.WritePermission);
      this.BindInt("@readPermission", description.ReadPermission);
      this.BindString("@extensionType", description.ExtensionType, -1, true, SqlDbType.VarChar);
      TeamFoundationDatabaseXmlWriter xmlWriter = new TeamFoundationDatabaseXmlWriter("actions");
      if (description.Actions != null)
      {
        foreach (SecurityComponent.NamespaceAction action in description.Actions)
        {
          xmlWriter.WriteStartElement("a");
          xmlWriter.WriteAttributeString("b", action.Bit.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          xmlWriter.WriteAttributeString("d", action.DisplayName);
          xmlWriter.WriteAttributeString("n", action.Name);
          xmlWriter.WriteEndElement();
        }
      }
      this.BindXml("@actionXML", xmlWriter);
      this.BindGuid("@writerIdentifier", this.Author);
      if (description.SystemBitMask != 0)
        throw new NotSupportedException();
      this.ExecuteNonQuery();
    }

    public virtual void DeleteSecurityNamespace(Guid namespaceId, bool deleteAces = true)
    {
      this.PrepareStoredProcedure("prc_DeleteSecurityNamespace");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindGuid("@writerIdentifier", this.Author);
      if (!deleteAces)
        throw new NotSupportedException();
      this.ExecuteNonQuery();
    }

    public IEnumerable<SecurityComponent.NamespaceDescription> QuerySecurityNamespaces(
      Guid namespaceId = default (Guid))
    {
      SecurityComponent securityComponent = this;
      securityComponent.PrepareStoredProcedure("prc_QuerySecurityNamespaces");
      securityComponent.BindNullableGuid("@namespaceGuid", namespaceId);
      Dictionary<Guid, SecurityComponent.NamespaceDescription> dictionary = new Dictionary<Guid, SecurityComponent.NamespaceDescription>();
      Dictionary<Guid, List<SecurityComponent.NamespaceAction>> actionMap = new Dictionary<Guid, List<SecurityComponent.NamespaceAction>>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) securityComponent.ExecuteReader(), "prc_QuerySecurityNamespaces", securityComponent.RequestContext))
      {
        resultCollection.AddBinder<SecurityComponent.NamespaceDescription>((ObjectBinder<SecurityComponent.NamespaceDescription>) securityComponent.GetNamespaceDescriptionColumns());
        resultCollection.AddBinder<SecurityComponent.NamespaceAction>((ObjectBinder<SecurityComponent.NamespaceAction>) new SecurityComponent.NamespaceActionColumns());
        foreach (SecurityComponent.NamespaceDescription namespaceDescription in resultCollection.GetCurrent<SecurityComponent.NamespaceDescription>().Items)
          dictionary.Add(namespaceDescription.NamespaceId, namespaceDescription);
        resultCollection.NextResult();
        foreach (SecurityComponent.NamespaceAction namespaceAction in resultCollection.GetCurrent<SecurityComponent.NamespaceAction>().Items)
        {
          List<SecurityComponent.NamespaceAction> namespaceActionList;
          if (!actionMap.TryGetValue(namespaceAction.NamespaceId, out namespaceActionList))
          {
            namespaceActionList = new List<SecurityComponent.NamespaceAction>();
            actionMap[namespaceAction.NamespaceId] = namespaceActionList;
          }
          namespaceActionList.Add(namespaceAction);
        }
      }
      foreach (SecurityComponent.NamespaceDescription namespaceDescription1 in dictionary.Values)
      {
        List<SecurityComponent.NamespaceAction> newActions;
        if (namespaceDescription1 is SecurityComponent.LocalNamespaceDescription namespaceDescription2 && actionMap.TryGetValue(namespaceDescription2.NamespaceId, out newActions))
          namespaceDescription2.SetActions((IEnumerable<SecurityComponent.NamespaceAction>) newActions);
        yield return namespaceDescription1;
      }
    }

    protected virtual SecurityComponent.NamespaceDescriptionColumns GetNamespaceDescriptionColumns() => new SecurityComponent.NamespaceDescriptionColumns();

    protected virtual string GetDataspaceCategory(string dataspaceCategory) => dataspaceCategory.Equals("Framework", StringComparison.OrdinalIgnoreCase) ? "Default" : dataspaceCategory;

    protected int GetDataspaceIdForToken(string token)
    {
      Guid dataspaceIdentifier = Guid.Empty;
      if (this.DataspaceMapper != null)
      {
        using (this.AcquireExemptionLock())
          dataspaceIdentifier = this.DataspaceMapper.DataspaceIdentifierFromToken(this.RequestContext, token);
      }
      return this.GetDataspaceId(dataspaceIdentifier);
    }

    public virtual ResultCollection QuerySecurityData(
      Guid namespaceId,
      bool usesInheritInformation,
      int lastSyncId,
      char separator = '\0')
    {
      this.PrepareStoredProcedure("prc_QuerySecurityInfo");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindBoolean("@getInheritInformation", usesInheritInformation);
      this.BindInt("@lastSyncId", lastSyncId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QuerySecurityInfo", this.RequestContext);
      resultCollection.AddBinder<DatabaseAccessControlEntry>((ObjectBinder<DatabaseAccessControlEntry>) this.GetDatabaseAccessControlEntryColumns());
      if (usesInheritInformation)
        resultCollection.AddBinder<string>((ObjectBinder<string>) this.GetNoInheritColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      return resultCollection;
    }

    public int QuerySequenceId(Guid namespaceId)
    {
      using (ResultCollection resultCollection = this.QuerySecurityData(namespaceId, false, int.MaxValue))
      {
        resultCollection.NextResult();
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    public virtual int SetPermissions(
      Guid namespaceId,
      string token,
      IEnumerable<DatabaseAccessControlEntry> permissions,
      bool merge,
      char separator)
    {
      TeamFoundationDatabaseXmlWriter databaseXmlWriter = new TeamFoundationDatabaseXmlWriter(nameof (permissions));
      SecurityComponent.WritePermissionXML(token, permissions, databaseXmlWriter);
      this.PrepareStoredProcedure("prc_SetAccessControlLists");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindXml("@permissionListXML", databaseXmlWriter);
      this.BindXml("@inheritListXML", (string) null);
      this.BindBoolean("@mergePermissions", merge);
      this.BindBoolean("@overwriteACL", false);
      this.BindSeparator("@separator", separator);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    public int SetAccessControlLists(
      Guid namespaceId,
      IEnumerable<IAccessControlList> accessControlLists,
      IEnumerable<DatabaseAccessControlEntry> accessControlEntries,
      char separator,
      SecurityNamespaceStructure structure)
    {
      return this.SetAccessControlLists(namespaceId, accessControlLists, accessControlEntries, separator, structure, false, true);
    }

    public virtual int SetAccessControlLists(
      Guid namespaceId,
      IEnumerable<IAccessControlList> accessControlLists,
      IEnumerable<DatabaseAccessControlEntry> accessControlEntries,
      char separator,
      SecurityNamespaceStructure structure,
      bool mergePermissions,
      bool overwriteAcl)
    {
      TeamFoundationDatabaseXmlWriter xmlWriter = new TeamFoundationDatabaseXmlWriter("inherits");
      if (structure == SecurityNamespaceStructure.Hierarchical)
      {
        foreach (IAccessControlList accessControlList in accessControlLists)
        {
          xmlWriter.WriteStartElement("i");
          xmlWriter.WriteAttributeString("s", accessControlList.Token);
          xmlWriter.WriteAttributeString("i", accessControlList.InheritPermissions ? "1" : "0");
          xmlWriter.WriteEndElement();
        }
      }
      TeamFoundationDatabaseXmlWriter databaseXmlWriter = new TeamFoundationDatabaseXmlWriter("permissions");
      foreach (DatabaseAccessControlEntry accessControlEntry in accessControlEntries)
        SecurityComponent.WritePermissionXML(accessControlEntry.Token, accessControlEntry, databaseXmlWriter);
      this.PrepareStoredProcedure("prc_SetAccessControlLists");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindXml("@permissionListXML", databaseXmlWriter);
      this.BindXml("@inheritListXML", xmlWriter);
      this.BindBoolean("@mergePermissions", mergePermissions);
      this.BindBoolean("@overwriteACL", overwriteAcl);
      this.BindSeparator("@separator", separator);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    public virtual int SetInheritFlag(
      Guid namespaceId,
      string token,
      bool inheritFlag,
      char separator)
    {
      this.PrepareStoredProcedure("prc_SetInheritFlag");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindString("@token", token, 4000, false, SqlDbType.NVarChar);
      this.BindBoolean("@inheritFlag", inheritFlag);
      this.BindSeparator("@separator", separator);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    public virtual int RemovePermissions(
      Guid namespaceId,
      string token,
      IEnumerable<Guid> teamFoundationIds,
      char separator)
    {
      StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w);
      xmlTextWriter.WriteStartDocument();
      xmlTextWriter.WriteStartElement("removes");
      foreach (Guid teamFoundationId in teamFoundationIds)
      {
        xmlTextWriter.WriteStartElement("i");
        xmlTextWriter.WriteAttributeString("t", teamFoundationId.ToString());
        xmlTextWriter.WriteEndElement();
      }
      xmlTextWriter.WriteEndElement();
      xmlTextWriter.WriteEndDocument();
      xmlTextWriter.Flush();
      this.PrepareStoredProcedure("prc_RemoveAccessControlEntry");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindXml("@identityListXML", w.ToString());
      this.BindString("@token", token, 4000, true, SqlDbType.NVarChar);
      this.BindString("@separator", separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveAccessControlEntry", this.RequestContext);
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SecurityComponent.SecurityIdentityColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      resultCollection.NextResult();
      return resultCollection.GetCurrent<int>().First<int>();
    }

    public virtual void RemoveAllAccessControlLists(Guid namespaceId)
    {
      this.PrepareStoredProcedure("prc_RemoveAllAccessControlLists");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public virtual int RemoveAccessControlLists(
      Guid namespaceId,
      IEnumerable<string> tokens,
      bool recurse,
      char separator)
    {
      StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w);
      xmlTextWriter.WriteStartDocument();
      xmlTextWriter.WriteStartElement(nameof (tokens));
      foreach (string token in tokens)
      {
        xmlTextWriter.WriteStartElement("t");
        xmlTextWriter.WriteAttributeString("t", token);
        xmlTextWriter.WriteAttributeString("r", recurse ? "1" : "0");
        xmlTextWriter.WriteEndElement();
      }
      xmlTextWriter.WriteEndElement();
      xmlTextWriter.WriteEndDocument();
      xmlTextWriter.Flush();
      this.PrepareStoredProcedure("prc_RemoveAccessControlList");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindXml("@tokenListXML", w.ToString());
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, true, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveAccessControlList", this.RequestContext);
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SecurityComponent.SecurityIdentityColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      resultCollection.NextResult();
      return resultCollection.GetCurrent<int>().First<int>();
    }

    public virtual int RenameToken(
      Guid namespaceId,
      string originalToken,
      string newToken,
      bool copy,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RenameToken");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindString("@originalToken", originalToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@newToken", newToken, -1, false, SqlDbType.NVarChar);
      this.BindBoolean("@copy", copy);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    public virtual int RenameTokens(
      Guid namespaceId,
      IEnumerable<TokenRename> renameTokens,
      char separator)
    {
      throw new NotImplementedException();
    }

    public virtual ResultCollection RemoveIdentityACEs(
      Guid namespaceId,
      IEnumerable<Guid> teamFoundationIds)
    {
      throw new NotImplementedException();
    }

    public virtual void PurgeSecurityTokenDelta(int purgePeriodInDays) => throw new NotImplementedException();

    public virtual IEnumerable<Guid> RemoveDataspacedACEs(
      IEnumerable<Tuple<Guid, string>> namespaces,
      Guid dataspaceIdentifier)
    {
      return Enumerable.Empty<Guid>();
    }

    protected void BindSeparator(string parameterName, char separator)
    {
      if (separator == char.MinValue)
        this.BindNullValue(parameterName, SqlDbType.NVarChar);
      else
        this.BindString(parameterName, separator.ToString(), 1, false, SqlDbType.NVarChar);
    }

    private static void WritePermissionXML(
      string securityToken,
      IEnumerable<DatabaseAccessControlEntry> permissions,
      TeamFoundationDatabaseXmlWriter xmlPermissionWriter)
    {
      foreach (DatabaseAccessControlEntry permission in permissions)
        SecurityComponent.WritePermissionXML(securityToken, permission, xmlPermissionWriter);
    }

    private static void WritePermissionXML(
      string securityToken,
      DatabaseAccessControlEntry permission,
      TeamFoundationDatabaseXmlWriter xmlPermissionWriter)
    {
      xmlPermissionWriter.WriteStartElement("p");
      xmlPermissionWriter.WriteAttributeString("t", permission.SubjectId.ToString());
      xmlPermissionWriter.WriteAttributeString("s", securityToken);
      xmlPermissionWriter.WriteAttributeString("a", permission.Allow.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      xmlPermissionWriter.WriteAttributeString("d", permission.Deny.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      xmlPermissionWriter.WriteEndElement();
    }

    protected virtual SecurityComponent.DatabaseAccessControlEntryColumns GetDatabaseAccessControlEntryColumns() => new SecurityComponent.DatabaseAccessControlEntryColumns();

    protected virtual SecurityComponent.NoInheritColumns GetNoInheritColumns() => new SecurityComponent.NoInheritColumns();

    public virtual void BulkImportSecurityData(
      Guid namespaceId,
      IEnumerable<IAccessControlList> accessControlLists,
      bool overwriteAcls,
      bool mergePermissions,
      char separator)
    {
      TeamFoundationDatabaseXmlWriter xmlWriter1 = new TeamFoundationDatabaseXmlWriter("inherits");
      TeamFoundationDatabaseXmlWriter xmlWriter2 = new TeamFoundationDatabaseXmlWriter("permissions");
      foreach (IAccessControlList accessControlList in accessControlLists)
      {
        xmlWriter1.WriteStartElement("i");
        xmlWriter1.WriteAttributeString("s", accessControlList.Token);
        xmlWriter1.WriteAttributeString("i", accessControlList.InheritPermissions ? "1" : "0");
        xmlWriter1.WriteEndElement();
        foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
        {
          xmlWriter2.WriteStartElement("p");
          xmlWriter2.WriteAttributeString("t", accessControlEntry.Descriptor.IdentityType);
          xmlWriter2.WriteAttributeString("i", accessControlEntry.Descriptor.Identifier);
          xmlWriter2.WriteAttributeString("s", accessControlList.Token);
          xmlWriter2.WriteAttributeString("a", accessControlEntry.Allow.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          xmlWriter2.WriteAttributeString("d", accessControlEntry.Deny.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          xmlWriter2.WriteEndElement();
        }
      }
      this.PrepareStoredProcedure("prc_SetSnapshotInstanceSecurityData");
      this.BindGuid("@namespaceId", namespaceId);
      this.BindXml("@permissionListXML", xmlWriter2);
      this.BindXml("@inheritListXML", xmlWriter1);
      this.BindBoolean("@mergePermissions", mergePermissions);
      this.BindBoolean("@overwriteACL", mergePermissions);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection BulkExportSecurityData(
      Guid namespaceId,
      string securityToken,
      char separator)
    {
      this.PrepareStoredProcedure("prc_QuerySnapshotInstanceSecurityData");
      this.BindGuid("@namespaceId", namespaceId);
      this.BindString("@securityToken", securityToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QuerySnapshotInstanceSecurityData", this.RequestContext);
      resultCollection.AddBinder<IAccessControlEntry>((ObjectBinder<IAccessControlEntry>) new SecurityComponent.BulkExportACEColumns());
      resultCollection.AddBinder<bool>((ObjectBinder<bool>) new SecurityComponent.BulkExportInheritColumns());
      return resultCollection;
    }

    protected class NamespaceActionColumns : ObjectBinder<SecurityComponent.NamespaceAction>
    {
      private SqlColumnBinder namespaceIdColumn = new SqlColumnBinder("NamespaceId");
      private SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
      private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder bitColumn = new SqlColumnBinder("Bit");

      protected override SecurityComponent.NamespaceAction Bind() => new SecurityComponent.NamespaceAction(this.namespaceIdColumn.GetGuid((IDataReader) this.Reader, false), this.bitColumn.GetInt32((IDataReader) this.Reader), this.nameColumn.GetString((IDataReader) this.Reader, false), this.displayNameColumn.GetString((IDataReader) this.Reader, true));
    }

    protected class NamespaceDescriptionColumns : 
      ObjectBinder<SecurityComponent.NamespaceDescription>
    {
      private SqlColumnBinder namespaceGuidColumn = new SqlColumnBinder("NamespaceGuid");
      private SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
      private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder databaseCategoryColumn = new SqlColumnBinder("DatabaseCategory");
      private SqlColumnBinder structureColumn = new SqlColumnBinder("Structure");
      private SqlColumnBinder separatorColumn = new SqlColumnBinder("Separator");
      private SqlColumnBinder elementLengthColumn = new SqlColumnBinder("ElementLength");
      private SqlColumnBinder writePermissionColumn = new SqlColumnBinder("WritePermission");
      private SqlColumnBinder readPermissionColumn = new SqlColumnBinder("ReadPermission");
      private SqlColumnBinder extensionTypeColumn = new SqlColumnBinder("ExtensionType");

      protected override SecurityComponent.NamespaceDescription Bind() => (SecurityComponent.NamespaceDescription) new SecurityComponent.LocalNamespaceDescription(this.namespaceGuidColumn.GetGuid((IDataReader) this.Reader, false), this.nameColumn.GetString((IDataReader) this.Reader, false), this.displayNameColumn.GetString((IDataReader) this.Reader, true), this.GetDataspaceCategory(this.databaseCategoryColumn.GetString((IDataReader) this.Reader, false)), this.separatorColumn.GetString((IDataReader) this.Reader, false)[0], this.elementLengthColumn.GetInt32((IDataReader) this.Reader), (SecurityNamespaceStructure) this.structureColumn.GetInt32((IDataReader) this.Reader), this.writePermissionColumn.GetInt32((IDataReader) this.Reader), this.readPermissionColumn.GetInt32((IDataReader) this.Reader), Enumerable.Empty<SecurityComponent.NamespaceAction>(), this.extensionTypeColumn.GetString((IDataReader) this.Reader, true), this.GetIsRemotable(), this.GetUseTokenTranslator(), 0);

      protected virtual bool GetIsRemotable() => false;

      protected virtual bool GetUseTokenTranslator() => false;

      protected virtual string GetDataspaceCategory(string dataspaceCategory) => dataspaceCategory.Equals("Framework", StringComparison.OrdinalIgnoreCase) ? "Default" : dataspaceCategory;
    }

    protected class DatabaseAccessControlEntryColumns : ObjectBinder<DatabaseAccessControlEntry>
    {
      private SqlColumnBinder tokenColumn = new SqlColumnBinder("SecurityToken");
      private SqlColumnBinder allowColumn = new SqlColumnBinder("AllowPermission");
      private SqlColumnBinder denyColumn = new SqlColumnBinder("DenyPermission");
      private SqlColumnBinder teamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");
      private SqlColumnBinder deletedColumn = new SqlColumnBinder("Deleted");

      protected override DatabaseAccessControlEntry Bind() => new DatabaseAccessControlEntry(this.teamFoundationIdColumn.GetGuid((IDataReader) this.Reader), this.tokenColumn.GetString((IDataReader) this.Reader, true), this.allowColumn.GetInt32((IDataReader) this.Reader), this.denyColumn.GetInt32((IDataReader) this.Reader), this.deletedColumn.GetBoolean((IDataReader) this.Reader));
    }

    protected class SecurityIdentityColumns : ObjectBinder<Guid>
    {
      private SqlColumnBinder teamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");

      protected override Guid Bind() => this.teamFoundationIdColumn.GetGuid((IDataReader) this.Reader);
    }

    protected class NoInheritColumns : ObjectBinder<string>
    {
      private SqlColumnBinder tokenColumn = new SqlColumnBinder("SecurityToken");

      protected override string Bind() => this.tokenColumn.GetString((IDataReader) this.Reader, true);
    }

    protected class ChangeIdColumns : ObjectBinder<int>
    {
      private SqlColumnBinder changeIdColumn = new SqlColumnBinder("ChangeId");

      protected override int Bind() => this.changeIdColumn.GetInt32((IDataReader) this.Reader, 0);
    }

    protected class BulkExportACEColumns : ObjectBinder<IAccessControlEntry>
    {
      private SqlColumnBinder identityTypeColumn = new SqlColumnBinder("IdentityType");
      private SqlColumnBinder identityIdentifierColumn = new SqlColumnBinder("IdentityIdentifier");
      private SqlColumnBinder allowColumn = new SqlColumnBinder("AllowPermission");
      private SqlColumnBinder denyColumn = new SqlColumnBinder("DenyPermission");

      protected override IAccessControlEntry Bind() => (IAccessControlEntry) new AccessControlEntry(new IdentityDescriptor(this.identityTypeColumn.GetString((IDataReader) this.Reader, false), this.identityIdentifierColumn.GetString((IDataReader) this.Reader, false)), this.allowColumn.GetInt32((IDataReader) this.Reader), this.denyColumn.GetInt32((IDataReader) this.Reader));
    }

    protected class BulkExportInheritColumns : ObjectBinder<bool>
    {
      private SqlColumnBinder inheritsColumn = new SqlColumnBinder("Inherits");

      protected override bool Bind() => this.inheritsColumn.GetBoolean((IDataReader) this.Reader);
    }

    public abstract class NamespaceDescription
    {
      public readonly Guid NamespaceId;
      public readonly string DataspaceCategory;

      protected NamespaceDescription(Guid namespaceId, string dataspaceCategory)
      {
        this.NamespaceId = namespaceId;
        this.DataspaceCategory = dataspaceCategory;
      }
    }

    public class LocalNamespaceDescription : SecurityComponent.NamespaceDescription
    {
      public readonly string Name;
      public readonly string DisplayName;
      public readonly SecurityNamespaceStructure Structure;
      public readonly char Separator;
      public readonly int ElementLength;
      public readonly int WritePermission;
      public readonly int ReadPermission;
      public readonly string ExtensionType;
      public readonly bool IsRemotable;
      public readonly bool UseTokenTranslator;
      public readonly int SystemBitMask;
      private IEnumerable<SecurityComponent.NamespaceAction> m_actions;

      public LocalNamespaceDescription(
        Guid namespaceId,
        string name,
        string displayName,
        string dataspaceCategory,
        char separator,
        int elementLength,
        SecurityNamespaceStructure structure,
        int writePermission,
        int readPermission,
        IEnumerable<SecurityComponent.NamespaceAction> actions,
        string extensionType,
        bool isRemotable,
        bool useTokenTranslator,
        int systemBitMask)
        : base(namespaceId, dataspaceCategory)
      {
        this.Name = name;
        this.DisplayName = displayName;
        this.Structure = structure;
        this.Separator = separator;
        this.ElementLength = elementLength;
        this.WritePermission = writePermission;
        this.ReadPermission = readPermission;
        this.m_actions = actions;
        this.ExtensionType = extensionType;
        this.IsRemotable = isRemotable;
        this.UseTokenTranslator = useTokenTranslator;
        this.SystemBitMask = systemBitMask;
      }

      public IEnumerable<SecurityComponent.NamespaceAction> Actions => this.m_actions;

      public void SetActions(
        IEnumerable<SecurityComponent.NamespaceAction> newActions)
      {
        this.m_actions = newActions;
      }

      public SecurityNamespaceDescription ToPublicType() => new SecurityNamespaceDescription(this.NamespaceId, this.Name, this.DisplayName, this.DataspaceCategory, this.Separator, this.ElementLength, this.Structure, this.WritePermission, this.ReadPermission, new List<ActionDefinition>(this.Actions.Select<SecurityComponent.NamespaceAction, ActionDefinition>((System.Func<SecurityComponent.NamespaceAction, ActionDefinition>) (s => s.ToPublicType()))), this.ExtensionType, this.IsRemotable, this.UseTokenTranslator, this.SystemBitMask);

      public static SecurityComponent.LocalNamespaceDescription FromPublicType(
        SecurityNamespaceDescription publicType)
      {
        return new SecurityComponent.LocalNamespaceDescription(publicType.NamespaceId, publicType.Name, publicType.DisplayName, publicType.DataspaceCategory, publicType.SeparatorValue, publicType.ElementLength, publicType.NamespaceStructure, publicType.WritePermission, publicType.ReadPermission, publicType.Actions != null ? publicType.Actions.Select<ActionDefinition, SecurityComponent.NamespaceAction>((System.Func<ActionDefinition, SecurityComponent.NamespaceAction>) (s => SecurityComponent.NamespaceAction.FromPublicType(s))) : Enumerable.Empty<SecurityComponent.NamespaceAction>(), publicType.ExtensionType, publicType.IsRemotable, publicType.UseTokenTranslator, publicType.SystemBitMask);
      }
    }

    public class RemoteNamespaceDescription : SecurityComponent.NamespaceDescription
    {
      public readonly Guid ServiceOwner;

      public RemoteNamespaceDescription(
        Guid namespaceId,
        string dataspaceCategory,
        Guid serviceOwner)
        : base(namespaceId, dataspaceCategory)
      {
        this.ServiceOwner = serviceOwner;
      }

      public RemoteSecurityNamespaceDescription ToPublicType() => new RemoteSecurityNamespaceDescription(this.NamespaceId, this.ServiceOwner);

      public static SecurityComponent.RemoteNamespaceDescription FromPublicType(
        RemoteSecurityNamespaceDescription publicType)
      {
        return new SecurityComponent.RemoteNamespaceDescription(publicType.NamespaceId, "Default", publicType.ServiceOwner);
      }
    }

    public class NamespaceAction
    {
      public readonly Guid NamespaceId;
      public readonly int Bit;
      public readonly string Name;
      public readonly string DisplayName;

      public NamespaceAction(Guid namespaceId, int bit, string name, string displayName)
      {
        this.NamespaceId = namespaceId;
        this.Bit = bit;
        this.Name = name;
        this.DisplayName = displayName;
      }

      public ActionDefinition ToPublicType() => new ActionDefinition(this.NamespaceId, this.Bit, this.Name, this.DisplayName);

      public static SecurityComponent.NamespaceAction FromPublicType(ActionDefinition publicType) => new SecurityComponent.NamespaceAction(publicType.NamespaceId, publicType.Bit, publicType.Name, publicType.DisplayName);
    }
  }
}
