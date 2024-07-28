// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
