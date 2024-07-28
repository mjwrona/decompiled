// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent10
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent10 : IdentityManagementComponent9
  {
    public static readonly IDictionary<string, string> ExtendedPropertyNameMap = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "ExternalId",
        "http://schemas.microsoft.com/identity/claims/objectidentifier"
      },
      {
        "AuthenticationCredentialValidFrom",
        "AuthenticationCredentialValidFrom"
      },
      {
        "ImageId",
        "Microsoft.TeamFoundation.Identity.Image.Id"
      },
      {
        "ImageType",
        "Microsoft.TeamFoundation.Identity.Image.Type"
      },
      {
        "ConfirmedNotificationAddress",
        "ConfirmedNotificationAddress"
      },
      {
        "CustomNotificationAddresses",
        "CustomNotificationAddresses"
      }
    };

    public override IList<Guid> FetchIdentityIdsBatch(
      int batchSize,
      int maxSequenceId = -1,
      bool incudeOnlyClaimsAndBindPendingTypes = false)
    {
      try
      {
        this.TraceEnter(4701700, nameof (FetchIdentityIdsBatch));
        this.PrepareStoredProcedure("prc_FetchIdentityIdsBatch");
        this.BindInt("@batchSize", batchSize);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(4701709, nameof (FetchIdentityIdsBatch));
      }
    }

    public override List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      this.PrepareUpdateIdentitiesStoredProcedure(updates);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity update in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates)
      {
        if (update.Id == Guid.Empty)
          update.Id = Guid.NewGuid();
        string str = IdentityHelper.CleanProviderDisplayName(update.ProviderDisplayName, update.Descriptor);
        if (!string.IsNullOrWhiteSpace(str))
          update.ProviderDisplayName = str;
        if (update.CustomDisplayName != null)
          update.CustomDisplayName = IdentityHelper.CleanCustomDisplayName(update.CustomDisplayName, update.Descriptor, false);
      }
      this.BindIdentityTable5("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindBoolean("@favorCurrentlyActive", favorCurrentlyActive);
      if (updates.Any<Microsoft.VisualStudio.Services.Identity.Identity>((System.Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.GetModifiedProperties() != null && i.GetModifiedProperties().Any<string>())))
        this.BindBoolean("@updateIdentityExtendedProperties", true);
      this.BindIdentityExtensionTable("@identityExtendedProperties", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityChangedData>((ObjectBinder<IdentityChangedData>) new IdentityManagementComponent.UpdateIdentitySummaryColumns());
        resultCollection.AddBinder<Tuple<int, Guid, bool>>((ObjectBinder<Tuple<int, Guid, bool>>) new IdentityManagementComponent.UpdateIdentityColumns());
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
        resultCollection.AddBinder<KeyValuePair<Guid, Guid>>((ObjectBinder<KeyValuePair<Guid, Guid>>) new IdentitiesToTransferColumns());
        resultCollection.AddBinder<DescriptorChange>((ObjectBinder<DescriptorChange>) new IdentityManagementComponent.DescriptorChangeColumns());
        identityChangedData = resultCollection.GetCurrent<IdentityChangedData>().FirstOrDefault<IdentityChangedData>();
        if (identityChangedData == null)
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        resultCollection.NextResult();
        List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        foreach (Tuple<int, Guid, bool> tuple in resultCollection.GetCurrent<Tuple<int, Guid, bool>>())
        {
          Microsoft.VisualStudio.Services.Identity.Identity update = updates[tuple.Item1];
          update.Id = tuple.Item2;
          update.MasterId = tuple.Item2;
          if (tuple.Item3)
            identityList.Add(update);
        }
        resultCollection.NextResult();
        deletedIds = resultCollection.GetCurrent<Guid>().Items;
        resultCollection.NextResult();
        identitiesToTransfer = resultCollection.GetCurrent<KeyValuePair<Guid, Guid>>().Items;
        if (identityChangedData.DescriptorChangeType == DescriptorChangeType.Minor)
        {
          resultCollection.NextResult();
          identityChangedData.DescriptorChanges = resultCollection.GetCurrent<DescriptorChange>().ToArray<DescriptorChange>();
        }
        return identityList;
      }
    }

    public override ResultCollection ReadIdentities(
      IEnumerable<IdentityDescriptor> descriptors,
      IEnumerable<Guid> ids)
    {
      this.PrepareStoredProcedure("prc_ReadIdentities");
      this.BindOrderedDescriptorTable("@descriptors", descriptors, true);
      this.BindOrderedGuidTable("@ids", ids, true);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) new IdentityManagementComponent10.IdentitiesColumns6());
      return resultCollection;
    }

    public override ResultCollection ReadIdentity(
      IdentitySearchFilter searchFactor,
      string factorValue,
      string domain,
      string account,
      int uniqueUserId,
      bool? isGroup)
    {
      this.PrepareStoredProcedure("prc_ReadIdentity");
      this.BindInt("@searchFactor", (int) searchFactor);
      this.BindString("@factorValue", factorValue, 515, false, SqlDbType.NVarChar);
      this.BindString("@domain", domain, 256, true, SqlDbType.NVarChar);
      this.BindString("@account", account, 256, true, SqlDbType.NVarChar);
      this.BindInt("@uniqueUserId", uniqueUserId);
      if (isGroup.HasValue)
        this.BindBoolean("@isGroup", isGroup.Value);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent10.IdentityColumns6());
      return resultCollection;
    }

    protected class IdentitiesColumns6 : ObjectBinder<IdentityManagementComponent.IdentityData>
    {
      private readonly IdentityManagementComponent10.IdentityColumns6 IdentityColumns;
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));

      public IdentitiesColumns6() => this.IdentityColumns = new IdentityManagementComponent10.IdentityColumns6();

      protected override IdentityManagementComponent.IdentityData Bind() => new IdentityManagementComponent.IdentityData()
      {
        Identity = this.IdentityColumns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };
    }

    protected class IdentityColumns6 : IdentityManagementComponent9.IdentityColumns5
    {
      private SqlColumnBinder ResourceVersion = new SqlColumnBinder(nameof (ResourceVersion));
      private SqlColumnBinder MetaTypeId = new SqlColumnBinder(nameof (MetaTypeId));
      private SqlColumnBinder ExternalId = new SqlColumnBinder(nameof (ExternalId));
      private SqlColumnBinder AuthenticationCredentialValidFrom = new SqlColumnBinder(nameof (AuthenticationCredentialValidFrom));
      private SqlColumnBinder ImageId = new SqlColumnBinder(nameof (ImageId));
      private SqlColumnBinder ImageType = new SqlColumnBinder(nameof (ImageType));
      private SqlColumnBinder ConfirmedNotificationAddress = new SqlColumnBinder(nameof (ConfirmedNotificationAddress));
      private SqlColumnBinder CustomNotificationAddresses = new SqlColumnBinder(nameof (CustomNotificationAddresses));

      protected override Microsoft.VisualStudio.Services.Identity.Identity Bind() => this.Bind(this.Reader);

      internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        identity.ResourceVersion = (int) this.ResourceVersion.GetByte((IDataReader) reader, (byte) 2);
        identity.MetaTypeId = (int) this.MetaTypeId.GetByte((IDataReader) reader, byte.MaxValue);
        Guid guid1 = this.ExternalId.GetGuid((IDataReader) reader, true);
        if (guid1 != Guid.Empty)
          identity.SetProperty(IdentityManagementComponent10.ExtendedPropertyNameMap["ExternalId"], (object) guid1.ToString());
        DateTime dateTime = this.AuthenticationCredentialValidFrom.GetDateTime((IDataReader) reader);
        if (dateTime != DateTime.MinValue)
          identity.SetProperty(IdentityManagementComponent10.ExtendedPropertyNameMap["AuthenticationCredentialValidFrom"], (object) dateTime.Ticks);
        Guid guid2 = this.ImageId.GetGuid((IDataReader) reader, true);
        if (guid2 != Guid.Empty)
          identity.SetProperty(IdentityManagementComponent10.ExtendedPropertyNameMap["ImageId"], (object) guid2.ToByteArray());
        string str1 = this.ImageType.GetString((IDataReader) reader, (string) null);
        if (!string.IsNullOrEmpty(str1))
          identity.SetProperty(IdentityManagementComponent10.ExtendedPropertyNameMap["ImageType"], (object) str1);
        string str2 = this.ConfirmedNotificationAddress.GetString((IDataReader) reader, (string) null);
        if (!string.IsNullOrEmpty(str2))
          identity.SetProperty(IdentityManagementComponent10.ExtendedPropertyNameMap["ConfirmedNotificationAddress"], (object) str2);
        string str3 = this.CustomNotificationAddresses.GetString((IDataReader) reader, (string) null);
        if (!string.IsNullOrEmpty(str3))
          identity.SetProperty(IdentityManagementComponent10.ExtendedPropertyNameMap["CustomNotificationAddresses"], (object) str3);
        DateTime? nullable = this.BindMetadataUpdateDate(reader);
        if (nullable.HasValue)
          identity.SetProperty("MetadataUpdateDate", (object) nullable.Value);
        return identity;
      }

      protected virtual DateTime? BindMetadataUpdateDate(SqlDataReader reader) => new DateTime?();

      protected virtual int BindResourceVersion(SqlDataReader reader) => (int) this.ResourceVersion.GetByte((IDataReader) reader, (byte) 2);
    }
  }
}
