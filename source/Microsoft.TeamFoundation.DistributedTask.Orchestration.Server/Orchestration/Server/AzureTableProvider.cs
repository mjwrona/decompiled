// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.AzureTableProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class AzureTableProvider : AzureTableStorageProvider
  {
    public AzureTableProvider(IVssRequestContext requestContext, string connectionString)
      : base(requestContext, connectionString, Enumerable.Empty<string>())
    {
    }

    protected override CloudTable GetCloudTable(string tableName, bool allowAdd = false) => this.TableClient.GetTableReference(tableName);
  }
}
