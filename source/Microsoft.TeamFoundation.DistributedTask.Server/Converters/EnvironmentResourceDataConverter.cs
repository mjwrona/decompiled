// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.EnvironmentResourceDataConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class EnvironmentResourceDataConverter
  {
    public static EnvironmentResourceReference ToResourceReference(
      this EnvironmentResourceData resourceData)
    {
      return new EnvironmentResourceReference()
      {
        Id = resourceData.Id,
        Name = resourceData.Name,
        Type = resourceData.Type
      };
    }
  }
}
