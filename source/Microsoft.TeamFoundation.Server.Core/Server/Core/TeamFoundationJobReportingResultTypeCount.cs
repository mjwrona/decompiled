// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationJobReportingResultTypeCount
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationJobReportingResultTypeCount
  {
    public TeamFoundationJobResult ResultTypeId { get; set; }

    public int Count { get; set; }

    public string ResultTypeName => this.ResultTypeId.ToString();

    public override string ToString() => string.Format("Count: '{1}'{0}ResultTypeId: '{2}'{0}ResultTypeName: '{3}'{0}", (object) Environment.NewLine, (object) this.Count, (object) this.ResultTypeId, (object) this.ResultTypeName);
  }
}
