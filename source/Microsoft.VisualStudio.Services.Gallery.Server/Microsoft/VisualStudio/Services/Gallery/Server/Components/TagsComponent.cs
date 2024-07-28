// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.TagsComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class TagsComponent : TeamFoundationSqlResourceComponent
  {
    private const string s_area = "TagsComponent";
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<TagsComponent>(1)
    }, "Tags");

    protected override string TraceArea => nameof (TagsComponent);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TagsComponent.SqlExceptionFactories;

    public virtual IEnumerable<TagItem> GetTagItemsByTagName(string tagName, byte tagType)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetTagItemsByTagName");
      this.BindString(nameof (tagName), tagName, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte(nameof (tagType), tagType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetTagItemsByTagName", this.RequestContext))
      {
        resultCollection.AddBinder<TagItem>((ObjectBinder<TagItem>) new TagBinder());
        return (IEnumerable<TagItem>) resultCollection.GetCurrent<TagItem>().Items;
      }
    }

    public virtual int DeleteTagItems(string tagName, byte tagType)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteTagItems");
      this.BindString(nameof (tagName), tagName, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte(nameof (tagType), tagType);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
