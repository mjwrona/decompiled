// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.ThirdParty.IStampLocator
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;

namespace Microsoft.Cloud.Metrics.Client.ThirdParty
{
  public interface IStampLocator
  {
    Uri GetStampEndpoint(string subscriptionId, string azureRegion);

    string GetMonitoringAccountName(string subscriptionId, string azureRegion);
  }
}
