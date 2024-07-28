// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionSourceProviderBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildDefinitionSourceProviderBinder : 
    BuildObjectBinder<BuildDefinitionSourceProvider>
  {
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder sourceProviderId = new SqlColumnBinder("SourceProviderId");
    private SqlColumnBinder sourceProviderName = new SqlColumnBinder("Name");
    private SqlColumnBinder sourceProviderFields = new SqlColumnBinder("Fields");
    private SqlColumnBinder lastModified = new SqlColumnBinder("LastModified");

    protected override BuildDefinitionSourceProvider Bind()
    {
      BuildDefinitionSourceProvider definitionSourceProvider = new BuildDefinitionSourceProvider();
      definitionSourceProvider.DefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false);
      definitionSourceProvider.Id = this.sourceProviderId.GetInt32((IDataReader) this.Reader);
      definitionSourceProvider.Name = this.sourceProviderName.GetString((IDataReader) this.Reader, false);
      definitionSourceProvider.Fields.AddRange((IEnumerable<NameValueField>) this.sourceProviderFields.XmlToListOfNameValueField(this.Reader));
      definitionSourceProvider.LastModified = this.lastModified.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      return definitionSourceProvider;
    }
  }
}
