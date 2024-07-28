// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublisherComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "PublisherComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[10]
    {
      (IComponentCreator) new ComponentCreator<PublisherComponent>(1, true),
      (IComponentCreator) new ComponentCreator<PublisherComponent2>(2),
      (IComponentCreator) new ComponentCreator<PublisherComponent3>(3),
      (IComponentCreator) new ComponentCreator<PublisherComponent4>(4),
      (IComponentCreator) new ComponentCreator<PublisherComponent5>(5),
      (IComponentCreator) new ComponentCreator<PublisherComponent6>(6),
      (IComponentCreator) new ComponentCreator<PublisherComponent7>(7),
      (IComponentCreator) new ComponentCreator<PublisherComponent8>(8),
      (IComponentCreator) new ComponentCreator<PublisherComponent9>(9),
      (IComponentCreator) new ComponentCreator<PublisherComponent10>(10)
    }, "Publisher");

    static PublisherComponent()
    {
      PublisherComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      PublisherComponent.s_sqlExceptionFactories.Add(270000, new SqlExceptionFactory(typeof (PublisherExistsException)));
      PublisherComponent.s_sqlExceptionFactories.Add(270001, new SqlExceptionFactory(typeof (PublisherDoesNotExistException)));
      PublisherComponent.s_sqlExceptionFactories.Add(270028, new SqlExceptionFactory(typeof (PublisherDomainDoesNotExistException)));
      PublisherComponent.s_sqlExceptionFactories.Add(270029, new SqlExceptionFactory(typeof (DnsTokenNotVerifiedException)));
      PublisherComponent.s_sqlExceptionFactories.Add(270030, new SqlExceptionFactory(typeof (InvalidUpdateOperationException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PublisherComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (PublisherComponent);

    public virtual Publisher CreatePublisher(
      string publisherName,
      string displayName,
      Guid ownerId,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      Guid publisherKey,
      DateTime updateTime)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreatePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (displayName), displayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid(nameof (ownerId), ownerId);
      this.BindInt(nameof (flags), (int) flags);
      this.BindString(nameof (shortDescription), shortDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (longDescription), longDescription, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableGuid(nameof (publisherKey), publisherKey);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<Publisher>((ObjectBinder<Publisher>) new PublisherComponent.PublisherBinder());
        return resultCollection.GetCurrent<Publisher>().Items[0];
      }
    }

    public virtual void DeletePublisher(string publisherName)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeletePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public virtual Publisher QueryPublisher(string publisherName, PublisherQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryPublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt(nameof (flags), (int) flags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryPublisher", this.RequestContext))
      {
        resultCollection.AddBinder<Publisher>((ObjectBinder<Publisher>) new PublisherComponent.PublisherBinder());
        Dictionary<Guid, Publisher> dictionary = this.ProcessPublisherResult(resultCollection, flags);
        return dictionary.Count != 0 ? dictionary.Values.First<Publisher>() : throw new PublisherDoesNotExistException(GalleryWebApiResources.PublisherDoesNotExist());
      }
    }

    public virtual PublisherQueryResult QueryPublishersByQuery(
      List<QueryFilter> filters,
      List<QueryFilterValue> filterValues,
      PublisherQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryPublishersByQuery");
      this.BindQueryFilterTable("queryFilter", (IEnumerable<QueryFilter>) filters);
      this.BindQueryFilterValueTable("queryFilterValue", (IEnumerable<QueryFilterValue>) filterValues);
      this.BindInt(nameof (flags), (int) flags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.QueryExtensionsByQuery", this.RequestContext))
      {
        resultCollection.AddBinder<QueryMatch>((ObjectBinder<QueryMatch>) new QueryMatchBinder());
        List<QueryMatch> items = resultCollection.GetCurrent<QueryMatch>().Items;
        resultCollection.AddBinder<Publisher>((ObjectBinder<Publisher>) new PublisherComponent.PublisherBinder());
        resultCollection.NextResult();
        Dictionary<Guid, Publisher> dictionary = this.ProcessPublisherResult(resultCollection, flags);
        PublisherQueryResult publisherQueryResult = new PublisherQueryResult();
        publisherQueryResult.Results = new List<PublisherFilterResult>();
        foreach (QueryFilter filter in filters)
          publisherQueryResult.Results.Add(new PublisherFilterResult());
        foreach (QueryMatch queryMatch in items)
        {
          PublisherFilterResult result = publisherQueryResult.Results[queryMatch.QueryIndex];
          if (result.Publishers == null)
            result.Publishers = new List<Publisher>();
          result.Publishers.Add(dictionary[queryMatch.ReferenceId]);
        }
        return publisherQueryResult;
      }
    }

    public virtual Publisher UpdatePublisher(
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      Guid publisherKey,
      DateTime updateTime)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdatePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (displayName), displayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableInt(nameof (flags), (int) flags, 1073741824);
      this.BindString(nameof (shortDescription), shortDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (longDescription), longDescription, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableGuid(nameof (publisherKey), publisherKey);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<Publisher>((ObjectBinder<Publisher>) new PublisherComponent.PublisherBinder());
        return resultCollection.GetCurrent<Publisher>().Items[0];
      }
    }

    public virtual void UpdatePublisherPermissions(PublisherAccessControlEntry publisherAce) => throw new NotImplementedException();

    public virtual void ManagePublisherVisibility(
      string publisherName,
      string extensionName,
      Guid identityId,
      bool removeTag)
    {
      throw new NotImplementedException();
    }

    public virtual IReadOnlyList<string> FetchAllPublisherDisplayNames() => throw new NotImplementedException();

    public virtual IReadOnlyList<string> FetchPublisherDisplayNamesHavingExtensions() => throw new NotImplementedException();

    protected Dictionary<Guid, Publisher> ProcessPublisherResult(
      ResultCollection resultCollection,
      PublisherQueryFlags flags)
    {
      List<Publisher> items = resultCollection.GetCurrent<Publisher>().Items;
      Dictionary<Guid, Publisher> dictionary = new Dictionary<Guid, Publisher>();
      foreach (Publisher publisher in items)
        dictionary[publisher.PublisherId] = publisher;
      if ((flags & PublisherQueryFlags.IncludeExtensions) != PublisherQueryFlags.None)
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) new PublishedExtensionBinder());
        resultCollection.NextResult();
        foreach (PublishedExtension publishedExtension in resultCollection.GetCurrent<PublishedExtension>().Items)
        {
          Publisher publisher = dictionary[publishedExtension.Publisher.PublisherId];
          if (publisher.Extensions == null)
            publisher.Extensions = new List<PublishedExtension>();
          publisher.Extensions.Add(publishedExtension);
        }
      }
      return dictionary;
    }

    public virtual List<string> QueryPublishersByVsid(string vsid)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryPublishersByVsid");
      this.BindString(nameof (vsid), vsid, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryPublishersByVsid", this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new PublisherComponent.PublisherNameBinder());
        return resultCollection.GetCurrent<string>().Items;
      }
    }

    internal string GetVSIDByPublisherName(string publisherName)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetVSIDByPublisherName");
      this.BindString(nameof (publisherName), publisherName, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetVSIDByPublisherName", this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new PublisherComponent.VSIDNameBinder());
        List<Guid> items = resultCollection.GetCurrent<Guid>().Items;
        return items.Any<Guid>() ? items[0].ToString() : string.Empty;
      }
    }

    internal Dictionary<Guid, string> GetPublisherIds(List<string> publisherNames)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetPublisherId");
      this.BindStringTable("PublisherNames", (IEnumerable<string>) publisherNames);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetPublisherId", this.RequestContext))
      {
        resultCollection.AddBinder<KeyValuePair<Guid, string>>((ObjectBinder<KeyValuePair<Guid, string>>) new PublisherComponent.GetPublisherIdBinder());
        return resultCollection.GetCurrent<KeyValuePair<Guid, string>>().Items.ToDictionary<KeyValuePair<Guid, string>, Guid, string>((System.Func<KeyValuePair<Guid, string>, Guid>) (pair => pair.Key), (System.Func<KeyValuePair<Guid, string>, string>) (pair => pair.Value));
      }
    }

    internal void InsertSpamPublishers(Dictionary<Guid, string> publishers)
    {
      this.PrepareStoredProcedure("Gallery.prc_AddSpamPublisher");
      this.BindGuidStringTable("SpamPublisher", publishers.Select<KeyValuePair<Guid, string>, Tuple<Guid, string>>((System.Func<KeyValuePair<Guid, string>, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.Key, x.Value))));
      this.ExecuteNonQuery();
    }

    internal class PublisherBinder : ObjectBinder<Publisher>
    {
      protected SqlColumnBinder publisherIdColumn = new SqlColumnBinder("PublisherId");
      protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
      protected SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
      protected SqlColumnBinder flagsColumn = new SqlColumnBinder("Flags");
      protected SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
      protected SqlColumnBinder shortDescriptionColumn = new SqlColumnBinder("ShortDescription");
      protected SqlColumnBinder longDescriptionColumn = new SqlColumnBinder("LongDescription");
      protected SqlColumnBinder publisherKeyColumn = new SqlColumnBinder("PublisherKey");

      protected override Publisher Bind()
      {
        Publisher publisher = new Publisher();
        publisher.PublisherId = this.publisherIdColumn.GetGuid((IDataReader) this.Reader);
        publisher.PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false);
        publisher.DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false);
        publisher.Flags = (PublisherFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
        publisher.LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader);
        publisher.ShortDescription = this.shortDescriptionColumn.GetString((IDataReader) this.Reader, true);
        publisher.LongDescription = this.longDescriptionColumn.GetString((IDataReader) this.Reader, true);
        return publisher;
      }
    }

    internal class PublisherBinder2 : PublisherComponent.PublisherBinder
    {
      protected SqlColumnBinder isVerified = new SqlColumnBinder("IsVerified");
      protected SqlColumnBinder isDnsTokenVerified = new SqlColumnBinder("IsDnsTokenVerified");

      protected override Publisher Bind()
      {
        Publisher publisher = new Publisher();
        publisher.PublisherId = this.publisherIdColumn.GetGuid((IDataReader) this.Reader);
        publisher.PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false);
        publisher.DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false);
        publisher.Flags = (PublisherFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
        publisher.LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader);
        publisher.ShortDescription = this.shortDescriptionColumn.GetString((IDataReader) this.Reader, true);
        publisher.LongDescription = this.longDescriptionColumn.GetString((IDataReader) this.Reader, true);
        publisher.IsDomainVerified = this.isVerified.GetBoolean((IDataReader) this.Reader, false);
        publisher.IsDnsTokenVerified = this.isDnsTokenVerified.GetBoolean((IDataReader) this.Reader, false);
        return publisher;
      }
    }

    protected class PublisherNameBinder : ObjectBinder<string>
    {
      protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");

      protected override string Bind() => this.publisherNameColumn.GetString((IDataReader) this.Reader, false);
    }

    protected class VSIDNameBinder : ObjectBinder<Guid>
    {
      protected SqlColumnBinder SecurityTokenColumn = new SqlColumnBinder("TeamFoundationId");

      protected override Guid Bind() => this.SecurityTokenColumn.GetGuid((IDataReader) this.Reader, false);
    }

    protected class GetPublisherIdBinder : ObjectBinder<KeyValuePair<Guid, string>>
    {
      protected SqlColumnBinder PublisherId = new SqlColumnBinder(nameof (PublisherId));
      protected SqlColumnBinder PublisherName = new SqlColumnBinder(nameof (PublisherName));

      protected override KeyValuePair<Guid, string> Bind() => new KeyValuePair<Guid, string>(this.PublisherId.GetGuid((IDataReader) this.Reader), this.PublisherName.GetString((IDataReader) this.Reader, false));
    }
  }
}
