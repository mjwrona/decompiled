// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionProviderNotFoundException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class ExternalConnectionProviderNotFoundException : ExternalConnectionExceptionsBase
  {
    public ExternalConnectionProviderNotFoundException(string serviceEndpointType)
      : base("Cannot find provider support service endpoint type '" + serviceEndpointType + "'")
    {
    }
  }
}
