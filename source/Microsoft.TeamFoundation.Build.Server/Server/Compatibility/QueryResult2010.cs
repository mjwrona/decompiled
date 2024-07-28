// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.QueryResult2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [XmlType("QueryResult")]
  public abstract class QueryResult2010
  {
    private List<Failure2010> m_failures = new List<Failure2010>();

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private, PropertyName = "InternalFailures", Direction = ClientPropertySerialization.Bidirectional)]
    public List<Failure2010> Failures => this.m_failures;
  }
}
