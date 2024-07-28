// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MessageQueueHelpers
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class MessageQueueHelpers
  {
    public const string MessageQueuePrefix = "taskagent-";

    public static string GetQueueName(int poolId, int agentId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}-{2}", (object) "taskagent-", (object) poolId, (object) agentId);

    public static bool TryParse(string queueName, out int poolId, out int agentId)
    {
      poolId = 0;
      agentId = 0;
      if (!queueName.StartsWith("taskagent-", StringComparison.OrdinalIgnoreCase))
        return false;
      string[] strArray = queueName.Substring("taskagent-".Length).Split(new string[1]
      {
        "-"
      }, StringSplitOptions.RemoveEmptyEntries);
      return strArray.Length == 2 && int.TryParse(strArray[0], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out poolId) && int.TryParse(strArray[1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out agentId);
    }
  }
}
