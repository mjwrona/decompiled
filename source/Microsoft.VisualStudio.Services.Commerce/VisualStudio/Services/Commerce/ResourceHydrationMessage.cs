// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceHydrationMessage
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ResourceHydrationMessage
  {
    public Guid SubscriptionId { get; set; }

    public Guid AccountId { get; set; }

    public string AccountName { get; set; }

    public string ResourceGroup { get; set; }

    public string ResourceRegion { get; set; }

    public HydrationOperationType OperationType { get; set; }

    public List<ServiceDefinition> ServiceInstances { get; set; }

    public void GenerateServiceDefinitions(HostInstanceMapping hostMapping) => this.ServiceInstances = new List<ServiceDefinition>()
    {
      hostMapping.ToServiceDefinition()
    };
  }
}
