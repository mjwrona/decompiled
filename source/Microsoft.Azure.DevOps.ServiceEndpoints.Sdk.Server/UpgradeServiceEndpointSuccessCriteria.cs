// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.UpgradeServiceEndpointSuccessCriteria
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class UpgradeServiceEndpointSuccessCriteria
  {
    public static readonly UpgradeServiceEndpointSuccessCriteria MostSuccessful = new UpgradeServiceEndpointSuccessCriteria()
    {
      MaxAttemptCount = 5,
      MinSuccessfulAttempts = 3
    };
    public static readonly UpgradeServiceEndpointSuccessCriteria AtLeastOneSuccessful = new UpgradeServiceEndpointSuccessCriteria()
    {
      MaxAttemptCount = 3,
      MinSuccessfulAttempts = 1
    };

    public int MaxAttemptCount { get; set; } = 1;

    public int MinSuccessfulAttempts { get; set; } = 1;

    public bool Matches(Func<bool> func)
    {
      int maxAttemptCount = this.MaxAttemptCount;
      int num = 0;
      while (maxAttemptCount-- > 0)
      {
        if (func())
        {
          ++num;
          if (num >= this.MinSuccessfulAttempts)
            return true;
        }
      }
      return false;
    }
  }
}
