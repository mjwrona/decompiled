// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.AzureEventLogEntry
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  public class AzureEventLogEntry : TableEntity
  {
    public long EventTickCount { get; set; }

    public string DeploymentId { get; set; }

    public string Role { get; set; }

    public string RoleInstance { get; set; }

    public string ProviderGuid { get; set; }

    public string ProviderName { get; set; }

    public int EventId { get; set; }

    public int Level { get; set; }

    public int Pid { get; set; }

    public int Tid { get; set; }

    public string Channel { get; set; }

    public string RawXml { get; set; }
  }
}
