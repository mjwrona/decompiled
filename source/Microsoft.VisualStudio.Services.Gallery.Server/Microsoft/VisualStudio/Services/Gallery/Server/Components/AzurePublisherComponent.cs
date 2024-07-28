// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.AzurePublisherComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  public class AzurePublisherComponent : TeamFoundationSqlResourceComponent
  {
    private const string s_area = "AzurePublisherComponent";
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<AzurePublisherComponent>(1),
      (IComponentCreator) new ComponentCreator<AzurePublisherComponent2>(2)
    }, "AzurePublisher");

    static AzurePublisherComponent()
    {
      AzurePublisherComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      AzurePublisherComponent.s_sqlExceptionFactories.Add(270012, new SqlExceptionFactory(typeof (AzurePublisherExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) =>
      {
        object[] strings = ExceptionHelper.ExtractStrings(sqlError, (object) "publisherName", (object) "azurePublisherId");
        return (Exception) new AzurePublisherExistsException(GalleryWebApiResources.AzurePublisherExistsException(strings[0], strings[1]));
      })));
      AzurePublisherComponent.s_sqlExceptionFactories.Add(270013, new SqlExceptionFactory(typeof (AzurePublisherDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) => (Exception) new AzurePublisherDoesNotExistException(GalleryResources.AzurePublisherDoesNotExist(ExceptionHelper.ExtractStrings(sqlError, (object) "publisherName")[0], (object) "https://go.microsoft.com/fwlink/?linkid=823198")))));
    }

    protected override string TraceArea => nameof (AzurePublisherComponent);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) AzurePublisherComponent.s_sqlExceptionFactories;

    public virtual AzurePublisher QueryAssociatedAzurePublisher(string publisherName)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryAzurePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryAzurePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<AzurePublisher>((ObjectBinder<AzurePublisher>) new AzurePublisherComponent.AzurePublisherBinder());
        List<AzurePublisher> items = resultCollection.GetCurrent<AzurePublisher>().Items;
        return items != null && items.Count > 0 ? items.First<AzurePublisher>() : throw new AzurePublisherDoesNotExistException(GalleryResources.AzurePublisherDoesNotExist((object) publisherName, (object) "https://go.microsoft.com/fwlink/?linkid=823198"));
      }
    }

    public virtual AzurePublisher UpdateAzurePublisher(
      string publisherName,
      string azurePublisherId,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateAzurePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindAzurePublisherId(azurePublisherId);
      this.BindGuid(nameof (updatedBy), updatedBy);
      this.BindDateTime("updatedDate", DateTime.UtcNow);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateAzurePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<AzurePublisher>((ObjectBinder<AzurePublisher>) new AzurePublisherComponent.AzurePublisherBinder());
        return resultCollection.GetCurrent<AzurePublisher>().Items[0];
      }
    }

    protected virtual void BindAzurePublisherId(string azurePublisherId) => this.BindGuid(nameof (azurePublisherId), Guid.Parse(azurePublisherId));

    internal class AzurePublisherBinder : ObjectBinder<AzurePublisher>
    {
      protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
      protected SqlColumnBinder azurePublisherIdColumn = new SqlColumnBinder("AzurePublisherId");

      protected override AzurePublisher Bind() => new AzurePublisher(this.publisherNameColumn.GetString((IDataReader) this.Reader, false), this.azurePublisherIdColumn.GetObject((IDataReader) this.Reader).ToString());
    }
  }
}
