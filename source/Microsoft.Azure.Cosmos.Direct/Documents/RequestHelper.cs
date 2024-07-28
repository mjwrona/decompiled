// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestHelper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class RequestHelper
  {
    public static ConsistencyLevel GetConsistencyLevelToUse(
      IServiceConfigurationReader serviceConfigReader,
      DocumentServiceRequest request)
    {
      ConsistencyLevel consistencyLevelToUse = serviceConfigReader.DefaultConsistencyLevel;
      string header = request.Headers["x-ms-consistency-level"];
      if (!string.IsNullOrEmpty(header))
      {
        ConsistencyLevel result;
        if (!Enum.TryParse<ConsistencyLevel>(header, out result))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) header, (object) "x-ms-consistency-level"));
        consistencyLevelToUse = result;
      }
      return consistencyLevelToUse;
    }
  }
}
