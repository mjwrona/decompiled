// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlAzureReason
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class SqlAzureReason
  {
    public SqlAzureThrottlingMode ThrottlingMode { get; internal set; }

    public SqlAzureResourceType MildResourceType { get; internal set; }

    public SqlAzureResourceType SignificantResourceType { get; internal set; }

    public override string ToString() => string.Format("ThrottlingMode = {0}, MildResourceType = {1}, SignificantResourceType = {2}", (object) this.ThrottlingMode, (object) this.MildResourceType, (object) this.SignificantResourceType);
  }
}
