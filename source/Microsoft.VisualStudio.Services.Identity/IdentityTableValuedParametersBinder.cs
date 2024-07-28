// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityTableValuedParametersBinder
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityTableValuedParametersBinder
  {
    public virtual SqlParameter BindIdentityTable(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable2();
    }

    public virtual SqlParameter BindIdentityTable2(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable3();
    }

    public virtual SqlParameter BindIdentityTable3(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable4();
    }

    public virtual SqlParameter BindIdentityTable4(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable5();
    }

    public virtual SqlParameter BindIdentityTable5(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable6();
    }

    public virtual SqlParameter BindIdentityTable6(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable7();
    }

    public virtual SqlParameter BindIdentityTable7(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable8();
    }

    public virtual SqlParameter BindIdentityTable9(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate).BindTableTyp_IdentityTable9();
    }

    public virtual SqlParameter BindIdentityTable10(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate,
      IdentityBindingConfig bindingConfig)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate, bindingConfig).BindTableTyp_IdentityTable10();
    }

    public virtual SqlParameter BindIdentityTable12(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      HashSet<string> propertiesToUpdate,
      IdentityBindingConfig bindingConfig)
    {
      return IdentityTableValuedParametersBinder.IdentityTableVersionHelper.Given(component, parameterName, rows, propertiesToUpdate, bindingConfig).BindTableTyp_IdentityTable12();
    }

    private class IdentityTableVersionHelper : TableValuedParameterVersionHelper<Microsoft.VisualStudio.Services.Identity.Identity>
    {
      private readonly HashSet<string> propertiesToUpdate;
      private readonly IdentityBindingConfig bindingConfig;
      private static readonly SqlMetaData[] typ_IdentityTable2 = new SqlMetaData[12]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Type", SqlDbType.VarChar, 64L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int)
      };
      private static readonly SqlMetaData[] typ_IdentityTable3 = new SqlMetaData[13]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Type", SqlDbType.VarChar, 64L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L)
      };
      private static readonly SqlMetaData[] typ_IdentityTable4 = new SqlMetaData[14]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Type", SqlDbType.VarChar, 64L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 50L)
      };
      private static readonly SqlMetaData[] typ_IdentityTable5 = new SqlMetaData[15]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Type", SqlDbType.VarChar, 64L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 50L),
        new SqlMetaData("ComplianceValidated", SqlDbType.DateTime)
      };
      private static readonly SqlMetaData[] typ_IdentityTable6 = new SqlMetaData[15]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("Type", SqlDbType.VarChar, 64L),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 100L),
        new SqlMetaData("ComplianceValidated", SqlDbType.DateTime)
      };
      private static readonly SqlMetaData[] typ_IdentityTable7 = new SqlMetaData[15]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("TypeId", SqlDbType.TinyInt),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 100L),
        new SqlMetaData("ComplianceValidated", SqlDbType.DateTime)
      };
      private static readonly SqlMetaData[] typ_IdentityTable8 = new SqlMetaData[14]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("TypeId", SqlDbType.TinyInt),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 100L)
      };
      private static readonly SqlMetaData[] typ_IdentityTable9 = new SqlMetaData[15]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("TypeId", SqlDbType.TinyInt),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 100L),
        new SqlMetaData("MetadataUpdateDate", SqlDbType.DateTime)
      };
      private static readonly SqlMetaData[] typ_IdentityTable10 = new SqlMetaData[16]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("TypeId", SqlDbType.TinyInt),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 100L),
        new SqlMetaData("MetadataUpdateDate", SqlDbType.DateTime),
        new SqlMetaData("DirectoryAlias", SqlDbType.NVarChar, 256L)
      };
      private static readonly SqlMetaData[] typ_IdentityTable12 = new SqlMetaData[18]
      {
        new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
        new SqlMetaData("TypeId", SqlDbType.TinyInt),
        new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ProviderDisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Description", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("Domain", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AccountName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DistinguishedName", SqlDbType.NVarChar, 1024L),
        new SqlMetaData("MailAddress", SqlDbType.NVarChar, 256L),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("OrderId", SqlDbType.Int),
        new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Puid", SqlDbType.VarChar, 100L),
        new SqlMetaData("MetadataUpdateDate", SqlDbType.DateTime),
        new SqlMetaData("DirectoryAlias", SqlDbType.NVarChar, 256L),
        new SqlMetaData("SocialType", SqlDbType.TinyInt),
        new SqlMetaData("SocialIdentifier", SqlDbType.NVarChar, 256L)
      };

      private IdentityTableVersionHelper(
        TeamFoundationSqlResourceComponent component,
        string parameterName,
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
        HashSet<string> propertiesToUpdate,
        IdentityBindingConfig bindingConfig)
        : base(component, parameterName, rows)
      {
        this.propertiesToUpdate = propertiesToUpdate;
        this.bindingConfig = bindingConfig;
      }

      private static void SetGroup(Microsoft.VisualStudio.Services.Identity.Identity identity, SqlDataRecord record, int index)
      {
        string property = identity.GetProperty<string>("SchemaClassName", (string) null);
        if (!string.IsNullOrEmpty(property))
        {
          if (property.Equals("Group", StringComparison.OrdinalIgnoreCase))
          {
            record.SetBoolean(index, true);
          }
          else
          {
            if (!property.Equals("User", StringComparison.OrdinalIgnoreCase))
              return;
            record.SetBoolean(index, false);
          }
        }
        else
          record.SetDBNull(index);
      }

      private void BindForTyp_IdentityTable2(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_Initial(record, identity, counter);
        record.SetString(1, identity.Descriptor.IdentityType);
      }

      private void BindForTyp_Initial(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        record.SetString(0, identity.Descriptor.Identifier);
        Action<string, Action<BindStringBehavior>, int> action = (Action<string, Action<BindStringBehavior>, int>) ((property, setAction, index) =>
        {
          if (this.propertiesToUpdate == null)
            setAction(BindStringBehavior.EmptyStringToNull);
          else if (this.propertiesToUpdate.Contains(property))
            setAction(BindStringBehavior.Unchanged);
          else
            record.SetDBNull(index);
        });
        Guid masterId = identity.MasterId == Guid.Empty ? identity.Id : identity.MasterId;
        if (this.propertiesToUpdate == null || !this.propertiesToUpdate.Contains("Descriptor"))
          action("MasterId", (Action<BindStringBehavior>) (bindBehavior => record.SetNullableGuid(2, masterId)), 2);
        else
          record.SetGuid(2, masterId);
        action("ProviderDisplayName", (Action<BindStringBehavior>) (bindBehavior => record.SetString(3, identity.ProviderDisplayName, bindBehavior)), 3);
        action("CustomDisplayName", (Action<BindStringBehavior>) (bindBehavior => record.SetString(4, identity.CustomDisplayName, BindStringBehavior.Unchanged)), 4);
        action("IdentityAttributeTags.Description", (Action<BindStringBehavior>) (bindBehavior => record.SetString(5, identity.GetProperty<string>("Description", (string) null), bindBehavior)), 5);
        action("IdentityAttributeTags.Domain", (Action<BindStringBehavior>) (bindBehavior => record.SetString(6, identity.GetProperty<string>("Domain", (string) null), bindBehavior)), 6);
        action("IdentityAttributeTags.AccountName", (Action<BindStringBehavior>) (bindBehavior => record.SetString(7, identity.GetProperty<string>("Account", (string) null), bindBehavior)), 7);
        action("IdentityAttributeTags.DistinguishedName", (Action<BindStringBehavior>) (bindBehavior => record.SetString(8, identity.GetProperty<string>("DN", (string) null), bindBehavior)), 8);
        action("IdentityAttributeTags.MailAddress", (Action<BindStringBehavior>) (bindBehavior => record.SetString(9, identity.GetProperty<string>("Mail", (string) null), bindBehavior)), 9);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.SetGroup(identity, record, 10);
        record.SetInt32(11, counter);
      }

      private void BindForTyp_IdentityTable3(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_IdentityTable2(record, identity, counter);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_Alias(record, identity);
      }

      private static void BindForTyp_Alias(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity) => record.SetNullableString(12, identity.GetProperty<string>("Alias", (string) null));

      private void BindForTyp_IdentityTable4(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_IdentityTable3(record, identity, counter);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_Puid(record, identity);
      }

      private static void BindForTyp_Puid(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity) => record.SetNullableString(13, identity.GetProperty<string>("PUID", (string) null));

      private void BindForTyp_IdentityTable5(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_IdentityTable4(record, identity, counter);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_ComplianceValidated(record, identity);
      }

      private static void BindForTyp_ComplianceValidated(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity) => record.SetNullableDateTime(14, identity.GetProperty<DateTime>("ComplianceValidated", DateTime.MinValue));

      private void BindForTyp_IdentityTable6(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_IdentityTable5(record, identity, counter);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_PuidStorageFormat(record, identity);
      }

      private static void BindForTyp_PuidStorageFormat(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity)
      {
        string str = identity.GetProperty<string>("PUID", (string) null);
        if (string.IsNullOrWhiteSpace(str) && identity.IsClaims && identity.GetProperty<string>("Domain", string.Empty).Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase) && identity.Descriptor.Identifier.Length == 25 && identity.Descriptor.Identifier.Substring(16).Equals("@Live.com", StringComparison.OrdinalIgnoreCase))
        {
          str = identity.Descriptor.Identifier.Substring(0, 16);
          identity.SetProperty("PUID", (object) str);
        }
        record.SetNullableString(13, str);
      }

      private void BindForTyp_IdentityTable7(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_Initial(record, identity, counter);
        record.SetByte(1, IdentityTypeMapper.Instance.GetTypeIdFromName(identity.Descriptor.IdentityType));
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_Alias(record, identity);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_ComplianceValidated(record, identity);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_PuidStorageFormat(record, identity);
      }

      private void BindForTyp_IdentityTable8(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_Initial(record, identity, counter);
        record.SetByte(1, IdentityTypeMapper.Instance.GetTypeIdFromName(identity.Descriptor.IdentityType));
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_Alias(record, identity);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_PuidStorageFormat(record, identity);
      }

      private void BindForTyp_IdentityTable9(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_Initial(record, identity, counter);
        record.SetByte(1, IdentityTypeMapper.Instance.GetTypeIdFromName(identity.Descriptor.IdentityType));
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_Alias(record, identity);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_PuidStorageFormat(record, identity);
        record.SetNullableDateTime(14, identity.GetProperty<DateTime>("MetadataUpdateDate", DateTime.MinValue));
      }

      private void BindForTyp_IdentityTable10(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_IdentityTable9(record, identity, counter);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_DirectoryAlias(record, identity, this.bindingConfig.DirectoryAliasFeatureEnabled && !this.bindingConfig.UseAccountNameAsDirectoryAlias);
      }

      private static void BindForTyp_DirectoryAlias(
        SqlDataRecord record,
        Microsoft.VisualStudio.Services.Identity.Identity identity,
        bool updateDirectoryAlias)
      {
        if (!updateDirectoryAlias)
          return;
        record.SetNullableString(15, identity.GetProperty<string>("DirectoryAlias", (string) null));
      }

      private void BindForTyp_IdentityTable12(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity, int counter)
      {
        this.BindForTyp_IdentityTable10(record, identity, counter);
        IdentityTableValuedParametersBinder.IdentityTableVersionHelper.BindForTyp_SocialIdentifier(record, identity, this.bindingConfig.SocialIdentifierEnabled);
      }

      private static void BindForTyp_SocialIdentifier(
        SqlDataRecord record,
        Microsoft.VisualStudio.Services.Identity.Identity identity,
        bool updateSocialIdentifier)
      {
        if (!updateSocialIdentifier)
          return;
        if (identity.SocialDescriptor != new SocialDescriptor())
        {
          SocialDescriptor socialDescriptor = identity.SocialDescriptor;
          if (!string.IsNullOrEmpty(socialDescriptor.SocialType))
          {
            socialDescriptor = identity.SocialDescriptor;
            if (socialDescriptor.SocialType != "ukn")
            {
              socialDescriptor = identity.SocialDescriptor;
              if (!string.IsNullOrEmpty(socialDescriptor.Identifier))
              {
                SqlDataRecord sqlDataRecord = record;
                SocialTypeMapper instance = SocialTypeMapper.Instance;
                socialDescriptor = identity.SocialDescriptor;
                string socialType = socialDescriptor.SocialType;
                int socialIdFromName = (int) instance.GetSocialIdFromName(socialType);
                sqlDataRecord.SetByte(16, (byte) socialIdFromName);
                SqlDataRecord record1 = record;
                socialDescriptor = identity.SocialDescriptor;
                string identifier = socialDescriptor.Identifier;
                record1.SetNullableString(17, identifier);
                return;
              }
            }
          }
        }
        record.SetNullableByte(16, new byte?());
        record.SetNullableString(17, (string) null);
      }

      public static IdentityTableValuedParametersBinder.IdentityTableVersionHelper Given(
        TeamFoundationSqlResourceComponent component,
        string parameterName,
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
        HashSet<string> propertiesToUpdate)
      {
        IdentityBindingConfig bindingConfig = new IdentityBindingConfig(false, -1, true, false, true);
        return new IdentityTableValuedParametersBinder.IdentityTableVersionHelper(component, parameterName, rows, propertiesToUpdate, bindingConfig);
      }

      public static IdentityTableValuedParametersBinder.IdentityTableVersionHelper Given(
        TeamFoundationSqlResourceComponent component,
        string parameterName,
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
        HashSet<string> propertiesToUpdate,
        IdentityBindingConfig bindingConfig)
      {
        return new IdentityTableValuedParametersBinder.IdentityTableVersionHelper(component, parameterName, rows, propertiesToUpdate, bindingConfig);
      }

      public SqlParameter BindTableTyp_IdentityTable2() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable2, "typ_IdentityTable2", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable2));

      public SqlParameter BindTableTyp_IdentityTable3() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable3, "typ_IdentityTable3", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable3));

      public SqlParameter BindTableTyp_IdentityTable4() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable4, "typ_IdentityTable4", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable4));

      public SqlParameter BindTableTyp_IdentityTable5() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable5, "typ_IdentityTable5", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable5));

      public SqlParameter BindTableTyp_IdentityTable6() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable6, "typ_IdentityTable6", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable6));

      public SqlParameter BindTableTyp_IdentityTable7() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable7, "typ_IdentityTable7", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable7));

      public SqlParameter BindTableTyp_IdentityTable8() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable8, "typ_IdentityTable8", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable8));

      public SqlParameter BindTableTyp_IdentityTable9() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable9, "typ_IdentityTable9", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable9));

      public SqlParameter BindTableTyp_IdentityTable10() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable10, "typ_IdentityTable10", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable10));

      public SqlParameter BindTableTyp_IdentityTable12() => this.BindTable(IdentityTableValuedParametersBinder.IdentityTableVersionHelper.typ_IdentityTable12, "typ_IdentityTable12", new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity, int>(this.BindForTyp_IdentityTable12));
    }
  }
}
