// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.InternalCloudAgentDefinitionBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class InternalCloudAgentDefinitionBinder : ObjectBinder<InternalCloudAgentDefinition>
  {
    private SqlColumnBinder identifier = new SqlColumnBinder("Identifier");
    private SqlColumnBinder imageLabel = new SqlColumnBinder("ImageLabel");
    private SqlColumnBinder isVisible = new SqlColumnBinder("IsVisible");

    protected override InternalCloudAgentDefinition Bind()
    {
      InternalCloudAgentDefinition cloudAgentDefinition = new InternalCloudAgentDefinition();
      cloudAgentDefinition.Identifier = this.identifier.GetString((IDataReader) this.Reader, false);
      cloudAgentDefinition.ImageLabel = this.imageLabel.GetString((IDataReader) this.Reader, false);
      cloudAgentDefinition.IsVisible = this.isVisible.GetBoolean((IDataReader) this.Reader, false);
      return cloudAgentDefinition;
    }
  }
}
