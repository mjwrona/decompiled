// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceDefinitionDataColumns4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceDefinitionDataColumns4 : ServiceDefinitionDataColumns3
  {
    private SqlColumnBinder resourceVersion = new SqlColumnBinder("ResourceVersion");
    private SqlColumnBinder minVersion = new SqlColumnBinder("MinVersion");
    private SqlColumnBinder maxVersion = new SqlColumnBinder("MaxVersion");
    private SqlColumnBinder releasedVersion = new SqlColumnBinder("ReleasedVersion");

    protected override ServiceDefinition Bind()
    {
      ServiceDefinition serviceDefinition = base.Bind();
      serviceDefinition.ResourceVersion = this.resourceVersion.GetInt32((IDataReader) this.Reader, 0);
      string version1 = this.minVersion.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(version1))
        serviceDefinition.MinVersion = new Version(version1);
      string version2 = this.maxVersion.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(version2))
        serviceDefinition.MaxVersion = new Version(version2);
      string version3 = this.releasedVersion.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(version3))
        serviceDefinition.ReleasedVersion = new Version(version3);
      return serviceDefinition;
    }
  }
}
