// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectWatermarkColumns2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectWatermarkColumns2 : ProjectWatermarkColumns
  {
    protected override string GetState() => this.stateColumn.GetString((IDataReader) this.Reader, true);
  }
}
