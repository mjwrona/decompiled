// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.ESStateYellowFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class ESStateYellowFaultMapper : FaultMapper
  {
    public ESStateYellowFaultMapper()
      : base("ESStateYellow", IndexerFaultSource.ElasticSearch)
    {
    }

    public override bool IsMatch(Exception ex) => ex != null && ex.GetType().IsAssignableFrom(typeof (SearchPlatformException)) && "The state of the Elastic Search Cluster is yellow.".Equals(ex.Message, StringComparison.OrdinalIgnoreCase);
  }
}
