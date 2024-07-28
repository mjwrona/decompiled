// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.ShardEventArgs
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using System;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class ShardEventArgs : EventArgs
  {
    public ShardEventArgs(string tableName, int shardId)
    {
      this.TableName = tableName;
      this.ShardId = shardId;
    }

    public string TableName { get; private set; }

    public int ShardId { get; private set; }
  }
}
