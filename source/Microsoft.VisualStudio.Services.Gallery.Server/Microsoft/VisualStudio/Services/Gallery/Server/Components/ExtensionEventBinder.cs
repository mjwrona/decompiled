// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionEventBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionEventBinder : ObjectBinder<ExtensionEvent>
  {
    private SqlColumnBinder idColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
    private SqlColumnBinder statisticDateColumn = new SqlColumnBinder("StatisticDate");
    private SqlColumnBinder propertiesColumn = new SqlColumnBinder("Properties");

    protected override ExtensionEvent Bind() => new ExtensionEvent()
    {
      Id = this.idColumn.GetInt64((IDataReader) this.Reader),
      Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
      StatisticDate = this.statisticDateColumn.GetDateTime((IDataReader) this.Reader),
      Properties = JObject.Parse(this.propertiesColumn.GetString((IDataReader) this.Reader, true))
    };
  }
}
