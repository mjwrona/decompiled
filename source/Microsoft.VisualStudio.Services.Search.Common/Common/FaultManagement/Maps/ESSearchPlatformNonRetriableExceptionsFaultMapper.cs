// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.ESSearchPlatformNonRetriableExceptionsFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class ESSearchPlatformNonRetriableExceptionsFaultMapper : FaultMapper
  {
    private const string ESTimeOutExceptionMessage = "System.Net.WebException: The operation has timed out";
    private const string ESQuorumNotMetExceptionMessage = "Not enough active copies to meet write consistency of [QUORUM]";
    private const string ESSocketExceptionMessage1 = "Unable to connect to the remote server ---> System.Net.Sockets.SocketException";
    private const string ESSocketExceptionMessage2 = "An error occurred trying to establish a connection with the specified node";
    private const string ESPrimaryShardInactiveMessage = "primary shard is not active Timeout";

    public ESSearchPlatformNonRetriableExceptionsFaultMapper()
      : base("ESSearchPlatformException", IndexerFaultSource.ElasticSearch)
    {
    }

    public override bool IsMatch(Exception ex)
    {
      if (ex == null || !ex.GetType().IsAssignableFrom(typeof (SearchPlatformException)))
        return false;
      return ex.Message.Contains("Unable to connect to the remote server ---> System.Net.Sockets.SocketException") || ex.Message.Contains("An error occurred trying to establish a connection with the specified node") || ex.Message.Contains("System.Net.WebException: The operation has timed out") || ex.Message.Contains("Not enough active copies to meet write consistency of [QUORUM]") || ex.Message.Contains("primary shard is not active Timeout");
    }
  }
}
