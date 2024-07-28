// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityUpgradeHandler
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityUpgradeHandler : IdentityAttachHandler
  {
    private readonly Dictionary<string, string> m_groupMap = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.IdentityDescriptor);

    public IdentityUpgradeHandler(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Framework.Server.ServicingContext servicingContext,
      Dictionary<string, IIdentityProvider> syncAgents,
      PlatformIdentityStore identityStore)
      : base(requestContext, (IServicingContext) servicingContext, (IDictionary<string, IIdentityProvider>) syncAgents, identityStore)
    {
    }

    protected override void PreTransferSteps()
    {
      base.PreTransferSteps();
      using (IdentityUpgradeHandler.IdentityUpgradeComponent component = this.RequestContext.CreateComponent<IdentityUpgradeHandler.IdentityUpgradeComponent>())
        component.UpdateSchema();
    }

    protected override void ReadSnapshotFromSource(
      out IEnumerable<IdentityScope> scopes,
      out IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      out IEnumerable<GroupMembership> memberships,
      out IEnumerable<Guid> identityIds)
    {
      using (IdentityUpgradeHandler.IdentityUpgradeComponent component = this.RequestContext.CreateComponent<IdentityUpgradeHandler.IdentityUpgradeComponent>())
      {
        List<IdentityScope> items1;
        List<Microsoft.VisualStudio.Services.Identity.Identity> items2;
        List<GroupMembership> items3;
        using (ResultCollection resultCollection = component.ReadSnapshot())
        {
          items1 = resultCollection.GetCurrent<IdentityScope>().Items;
          resultCollection.NextResult();
          items2 = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
          resultCollection.NextResult();
          items3 = resultCollection.GetCurrent<GroupMembership>().Items;
          resultCollection.NextResult();
          identityIds = (IEnumerable<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
        IdentityDomain identityDomain1 = new IdentityDomain(this.RequestContext);
        Dictionary<Guid, IdentityDescriptor> dictionary1 = new Dictionary<Guid, IdentityDescriptor>();
        IdentityDescriptor everyoneGroup = GroupWellKnownIdentityDescriptors.EveryoneGroup;
        IdentityDescriptor descriptor = identityDomain1.MapFromWellKnownIdentifier(everyoneGroup);
        Dictionary<Guid, Guid> dictionary2 = new Dictionary<Guid, Guid>(items1.Count);
        for (int index = 0; index < items1.Count; ++index)
        {
          IdentityScope identityScope = items1[index];
          identityScope.ParentId = this.HostDomain.DomainId;
          identityScope.SecuringHostId = this.HostDomain.DomainId;
          Guid guid = Guid.NewGuid();
          dictionary2.Add(identityScope.Id, guid);
          identityScope.Id = guid;
          IdentityDescriptor identityDescriptor = IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup, identityScope.Id);
          Guid id = Guid.NewGuid();
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity1.Id = id;
          identity1.Descriptor = identityDescriptor;
          identity1.ProviderDisplayName = FrameworkResources.ProjectValidUsersGroupName();
          identity1.IsActive = true;
          identity1.UniqueUserId = 0;
          identity1.IsContainer = true;
          identity1.Members = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
          identity1.MemberOf = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
          identity1.MasterId = id;
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
          identity2.SetProperty("SchemaClassName", (object) "Group");
          identity2.SetProperty("Domain", (object) IdentityHelper.GetScopeUri(identityScope.LocalScopeId, identityScope.ScopeType));
          identity2.SetProperty("Account", (object) FrameworkResources.ProjectValidUsersGroupName());
          identity2.SetProperty("Description", (object) FrameworkResources.ProjectValidUsersGroupDescription());
          identity2.SetProperty("DN", (object) string.Empty);
          identity2.SetProperty("Mail", (object) string.Empty);
          identity2.SetProperty("SecurityGroup", (object) "SecurityGroup");
          identity2.SetProperty("SpecialType", (object) "EveryoneApplicationGroup");
          items2.Add(identity2);
          GroupMembership groupMembership = new GroupMembership(Guid.Empty, id, descriptor);
          items3.Add(groupMembership);
          dictionary1.Add(identityScope.Id, identityDescriptor);
        }
        Dictionary<Guid, IdentityDescriptor> dictionary3 = new Dictionary<Guid, IdentityDescriptor>();
        for (int index = 0; index < items2.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = items2[index];
          string property1 = identity.GetProperty<string>("Domain", string.Empty);
          Guid result1;
          if (!string.IsNullOrEmpty(property1) && LinkingUtilities.IsUriWellFormed(property1) && Guid.TryParse(LinkingUtilities.DecodeUri(property1).ToolSpecificId, out result1))
          {
            Guid guid = dictionary2[result1];
            identity.SetProperty("ScopeId", (object) guid);
            IdentityDescriptor identityDescriptor1;
            if (dictionary1.TryGetValue(guid, out identityDescriptor1))
              dictionary3.Add(identity.MasterId, identityDescriptor1);
            IdentityDescriptor identityDescriptor2 = (IdentityDescriptor) null;
            string property2 = identity.GetProperty<string>("SpecialType", string.Empty);
            IdentityDomain identityDomain2 = new IdentityDomain(guid, string.Empty);
            if (string.Equals(property2, "AdministrativeApplicationGroup", StringComparison.Ordinal))
            {
              identityDescriptor2 = identityDomain2.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
            }
            else
            {
              string result2;
              if (identityDomain2.MapIfSidFromDifferentHost(identity.Descriptor.Identifier, SidIdentityHelper.WellKnownDomainSid + "-1", out result2))
                identityDescriptor2 = new IdentityDescriptor("Microsoft.TeamFoundation.Identity", result2);
            }
            if (identityDescriptor2 != (IdentityDescriptor) null)
            {
              this.m_groupMap.Add(identity.Descriptor.Identifier, identityDescriptor2.Identifier);
              identity.Descriptor = identityDescriptor2;
            }
          }
        }
        for (int index = 0; index < items3.Count; ++index)
        {
          GroupMembership groupMembership = items3[index];
          string str;
          if (this.m_groupMap.TryGetValue(groupMembership.Descriptor.Identifier, out str))
            groupMembership.Descriptor.Identifier = str;
          IdentityDescriptor identityDescriptor;
          if (dictionary3.TryGetValue(groupMembership.Id, out identityDescriptor) && IdentityDescriptorComparer.Instance.Equals(groupMembership.Descriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup))
            groupMembership.Descriptor = identityDescriptor;
        }
        groups = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) items2;
        scopes = (IEnumerable<IdentityScope>) items1;
        memberships = (IEnumerable<GroupMembership>) items3;
      }
    }

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IList<Guid> identityIds)
    {
      using (IdentityUpgradeHandler.IdentityUpgradeComponent component = this.RequestContext.CreateComponent<IdentityUpgradeHandler.IdentityUpgradeComponent>())
      {
        using (ResultCollection resultCollection = component.ReadIdentities(identityIds))
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
      }
    }

    protected override void UpdateIdentityMap(IEnumerable<KeyValuePair<Guid, Guid>> mappings)
    {
    }

    public override IdentityDescriptor MapIfTeamFoundationType(IdentityDescriptor descriptor)
    {
      if (string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
      {
        bool flag = false;
        string result = descriptor.Identifier;
        if (this.m_groupMap.TryGetValue(descriptor.Identifier, out result))
          flag = true;
        if (!flag)
          flag = this.HostDomain.MapIfSidFromDifferentHost(descriptor.Identifier, SidIdentityHelper.WellKnownDomainSid, out result);
        if (flag)
          descriptor = new IdentityDescriptor("Microsoft.TeamFoundation.Identity", result);
        descriptor = this.MapInactiveTargetGroupDescriptor(descriptor);
      }
      return descriptor;
    }

    private class IdentityUpgradeComponent : TeamFoundationSqlResourceComponent
    {
      internal void UpdateSchema()
      {
        string sqlStatement = "  IF NOT EXISTS (\n                            SELECT  *\n                            FROM    sys.columns\n                            WHERE   object_id = OBJECT_ID(N'tbl_security_identity_cache')\n                                    AND name = 'tf_id'\n                        )\n                        BEGIN\n                            ALTER TABLE tbl_security_identity_cache\n                            ADD tf_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()\n                        END\n\n                        IF NOT EXISTS (\n                            SELECT  *\n                            FROM    sys.indexes\n                            WHERE   object_id = OBJECT_ID(N'tbl_security_identity_cache')\n                                    AND name = 'ix_security_identity_cache_tf_id'\n                        )\n                        BEGIN\n                            CREATE UNIQUE INDEX ix_security_identity_cache_tf_id\n                            ON tbl_security_identity_cache(tf_id)\n                        END";
        this.PrepareSqlBatch(sqlStatement.Length);
        this.AddStatement(sqlStatement);
        this.ExecuteNonQuery();
      }

      internal ResultCollection ReadSnapshot()
      {
        string sqlStatement = "  -- All Scopes\n                        SELECT  proj.[uri],\n                                cssp.project_name AS scope_name,\n                                grp.[sid],\n                                cssp.project_id AS scope_id\n                        FROM    [dbo].[tbl_security_projects] proj\n                        JOIN    tbl_projects cssp\n                        ON      'vstfs:///Classification/TeamProject/' + CAST(cssp.project_id AS VARCHAR(128)) = proj.uri\n                        JOIN    [dbo].[tbl_gss_groups] grp\n                        ON      proj.pid = grp.pid\n                        WHERE   grp.special_type = 1\n\n                        -- All TF Groups\n                        SELECT  grp.[sid],\n                                proj.[uri] as project_uri,\n                                grp.[special_type],\n                                grp.[display_name],\n                                grp.[description],\n                                sic.[tf_id]\n                        FROM    [dbo].[tbl_gss_groups] grp\n                        JOIN    [dbo].[tbl_security_identity_cache] sic\n                        ON      sic.[sid] = grp.[sid]\n                        JOIN    [dbo].[tbl_security_projects] proj\n                        ON      proj.pid = grp.pid\n\n                        -- Memberships\n                        SELECT  mem.[parent_group_sid],\n                                sic.[tf_id] AS [member_id]\n                        FROM    [dbo].[tbl_gss_group_membership] mem\n                        JOIN    [dbo].[tbl_security_identity_cache] sic\n                        ON      sic.[sid] = mem.[member_sid]\n\n                        -- All external identities / inactive TF groups\n                        SELECT  ids.[tf_id]\n                        FROM    [dbo].[tbl_security_identity_cache] ids\n                        WHERE   ids.[type] <> 4\n                                OR ids.[deleted] = 1";
        this.PrepareSqlBatch(sqlStatement.Length);
        this.AddStatement(sqlStatement);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<IdentityScope>((ObjectBinder<IdentityScope>) new IdentityUpgradeHandler.IdentityUpgradeComponent.IdentityUpgradeProjectScopeColumns());
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityUpgradeHandler.IdentityUpgradeComponent.GroupIdentityColumns());
        resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new IdentityUpgradeHandler.IdentityUpgradeComponent.GroupMembershipColumns());
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityUpgradeHandler.IdentityUpgradeComponent.IdentityIdColumns());
        return resultCollection;
      }

      internal ResultCollection ReadIdentities(IList<Guid> identityIds)
      {
        string sqlStatement = "      DECLARE @status         INT\n                            DECLARE @rowcount       INT\n                            DECLARE @docHandle      INT\n                            DECLARE @pendingRows    INT\n\n                            CREATE TABLE #list (\n                                [id]\tUNIQUEIDENTIFIER,\n                                [allow] BIT DEFAULT 0\n\n                                PRIMARY KEY ([id])\n                            )\n\n                            -- Parse the list into our local temporary table\n                            EXEC sp_xml_preparedocument @docHandle OUTPUT, @id_list\n\n                            INSERT  #list ([id])\n                            SELECT  DISTINCT\n                                    input.[g]\n                            FROM    OPENXML(@docHandle, N'/ids/g')\n                                    WITH (\n                                        g UNIQUEIDENTIFIER\n                                    ) input\n\n                            SELECT @status = @@ERROR,\n                                   @pendingRows = @@ROWCOUNT\n\n                            EXEC sp_xml_removedocument @docHandle\n\n                            SELECT  ids.[sid],\n                                    ids.[type],\n                                    ids.[display_name],\n                                    ids.[description],\n                                    ids.[domain],\n                                    ids.[account_name],\n                                    ids.[distinguished_name],\n                                    ids.[mail_address],\n                                    ids.[special_type],\n                                    ids.[deleted],\n                                    ids.[tf_id]\n                            FROM    #list l\n                            JOIN    [dbo].[tbl_security_identity_cache] ids\n                            ON      l.[id] = ids.[tf_id]";
        using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          using (XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w))
          {
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteStartElement("ids");
            foreach (Guid identityId in (IEnumerable<Guid>) identityIds)
            {
              xmlTextWriter.WriteStartElement("g");
              xmlTextWriter.WriteAttributeString("g", identityId.ToString());
              xmlTextWriter.WriteEndElement();
            }
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.Flush();
            this.PrepareSqlBatch(sqlStatement.Length);
            this.AddStatement(sqlStatement);
            this.BindXml("@id_list", w.ToString());
          }
        }
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityUpgradeHandler.IdentityUpgradeComponent.IdentityColumns());
        return resultCollection;
      }

      internal class GroupMembershipColumns : ObjectBinder<GroupMembership>
      {
        private SqlColumnBinder parent_group_sid = new SqlColumnBinder(nameof (parent_group_sid));
        private SqlColumnBinder member_id = new SqlColumnBinder(nameof (member_id));

        protected override GroupMembership Bind()
        {
          Guid guid = this.member_id.GetGuid((IDataReader) this.Reader);
          IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(this.parent_group_sid.GetString((IDataReader) this.Reader, false));
          return new GroupMembership(Guid.Empty, guid, foundationDescriptor);
        }
      }

      internal class IdentityColumns : ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>
      {
        private SqlColumnBinder sid = new SqlColumnBinder(nameof (sid));
        private SqlColumnBinder type = new SqlColumnBinder(nameof (type));
        private SqlColumnBinder display_name = new SqlColumnBinder(nameof (display_name));
        private SqlColumnBinder description = new SqlColumnBinder(nameof (description));
        private SqlColumnBinder domain = new SqlColumnBinder(nameof (domain));
        private SqlColumnBinder account_name = new SqlColumnBinder(nameof (account_name));
        private SqlColumnBinder distinguished_name = new SqlColumnBinder(nameof (distinguished_name));
        private SqlColumnBinder mail_address = new SqlColumnBinder(nameof (mail_address));
        private SqlColumnBinder special_type = new SqlColumnBinder(nameof (special_type));
        private SqlColumnBinder deleted = new SqlColumnBinder(nameof (deleted));
        private SqlColumnBinder tf_id = new SqlColumnBinder(nameof (tf_id));

        protected override Microsoft.VisualStudio.Services.Identity.Identity Bind()
        {
          IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(this.sid.GetString((IDataReader) this.Reader, false));
          IdentityType identityType = (IdentityType) this.type.GetByte((IDataReader) this.Reader);
          string str1 = this.display_name.GetString((IDataReader) this.Reader, false);
          string str2 = this.description.GetString((IDataReader) this.Reader, false);
          string str3 = this.domain.GetString((IDataReader) this.Reader, true) ?? string.Empty;
          string str4 = this.account_name.GetString((IDataReader) this.Reader, false);
          string str5 = this.distinguished_name.GetString((IDataReader) this.Reader, false);
          string str6 = this.mail_address.GetString((IDataReader) this.Reader, false);
          SpecialGroupType specialGroupType = (SpecialGroupType) this.special_type.GetByte((IDataReader) this.Reader);
          bool flag = !this.deleted.GetBoolean((IDataReader) this.Reader);
          Guid guid = this.tf_id.GetGuid((IDataReader) this.Reader);
          Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity.Id = guid;
          identity.Descriptor = descriptorFromSid;
          identity.ProviderDisplayName = str1;
          identity.CustomDisplayName = (string) null;
          identity.IsActive = flag;
          identity.UniqueUserId = 0;
          identity.IsContainer = identityType == IdentityType.WindowsGroup;
          identity.Members = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
          identity.MemberOf = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
          identity.MasterId = guid;
          identity.SetProperty("SchemaClassName", identityType == IdentityType.WindowsGroup ? (object) "Group" : (object) "User");
          identity.SetProperty("Domain", (object) str3);
          identity.SetProperty("Account", (object) str4);
          identity.SetProperty("Description", (object) str2);
          identity.SetProperty("DN", (object) str5);
          identity.SetProperty("Mail", (object) str6);
          identity.SetProperty("SecurityGroup", (object) "SecurityGroup");
          identity.SetProperty("SpecialType", (object) specialGroupType.ToString());
          return identity;
        }
      }

      private class IdentityUpgradeProjectScopeColumns : ObjectBinder<IdentityScope>
      {
        private SqlColumnBinder uri = new SqlColumnBinder(nameof (uri));
        private SqlColumnBinder name = new SqlColumnBinder("scope_name");
        private SqlColumnBinder adminSid = new SqlColumnBinder("sid");
        private SqlColumnBinder scopeId = new SqlColumnBinder("scope_id");

        internal IdentityUpgradeProjectScopeColumns()
        {
        }

        protected override IdentityScope Bind()
        {
          Guid guid = this.scopeId.GetGuid((IDataReader) this.Reader);
          return new IdentityScope()
          {
            Id = guid,
            LocalScopeId = guid,
            ScopeType = GroupScopeType.TeamProject,
            Name = DBPath.DatabaseToUserPath(this.name.GetString((IDataReader) this.Reader, false)),
            Administrators = IdentityHelper.CreateTeamFoundationDescriptor(this.adminSid.GetString((IDataReader) this.Reader, false)),
            IsGlobal = false,
            IsActive = true
          };
        }
      }

      private class GroupIdentityColumns : ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>
      {
        private SqlColumnBinder sid = new SqlColumnBinder(nameof (sid));
        private SqlColumnBinder project_uri = new SqlColumnBinder(nameof (project_uri));
        private SqlColumnBinder special_type = new SqlColumnBinder(nameof (special_type));
        private SqlColumnBinder display_name = new SqlColumnBinder(nameof (display_name));
        private SqlColumnBinder description = new SqlColumnBinder(nameof (description));
        private SqlColumnBinder tf_id = new SqlColumnBinder(nameof (tf_id));

        protected override Microsoft.VisualStudio.Services.Identity.Identity Bind()
        {
          Guid guid = this.tf_id.GetGuid((IDataReader) this.Reader);
          string str1 = this.display_name.GetString((IDataReader) this.Reader, false);
          string str2 = this.project_uri.GetString((IDataReader) this.Reader, true) ?? string.Empty;
          string str3 = this.description.GetString((IDataReader) this.Reader, true);
          ApplicationGroupSpecialType int32 = (ApplicationGroupSpecialType) this.special_type.GetInt32((IDataReader) this.Reader);
          IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(this.sid.GetString((IDataReader) this.Reader, false));
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity1.Id = guid;
          identity1.Descriptor = descriptorFromSid;
          identity1.ProviderDisplayName = str1;
          identity1.CustomDisplayName = (string) null;
          identity1.IsActive = true;
          identity1.UniqueUserId = 0;
          identity1.IsContainer = true;
          identity1.Members = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
          identity1.MemberOf = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
          identity1.MasterId = guid;
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
          identity2.SetProperty("SchemaClassName", (object) "Group");
          if (string.IsNullOrEmpty(str2))
            identity2.SetProperty("GlobalScope", (object) "GlobalScope");
          identity2.SetProperty("Domain", (object) str2);
          identity2.SetProperty("Account", (object) str1);
          identity2.SetProperty("Description", (object) str3);
          identity2.SetProperty("DN", (object) string.Empty);
          identity2.SetProperty("Mail", (object) string.Empty);
          identity2.SetProperty("SecurityGroup", (object) "SecurityGroup");
          identity2.SetProperty("SpecialType", (object) int32.ToString());
          return identity2;
        }
      }

      protected class IdentityIdColumns : ObjectBinder<Guid>
      {
        private SqlColumnBinder id = new SqlColumnBinder("tf_id");

        protected override Guid Bind() => this.id.GetGuid((IDataReader) this.Reader);
      }
    }
  }
}
