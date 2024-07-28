// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.UpdatedPropertiesConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class UpdatedPropertiesConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties)
    {
      if (updatedProperties == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties()
      {
        Id = updatedProperties.Id,
        Revision = updatedProperties.Revision,
        LastUpdated = updatedProperties.LastUpdated,
        LastUpdatedBy = updatedProperties.LastUpdatedBy,
        LastUpdatedByName = updatedProperties.LastUpdatedByName
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties Convert(
      Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties updatedProperties)
    {
      if (updatedProperties == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties()
      {
        Id = updatedProperties.Id,
        Revision = updatedProperties.Revision,
        LastUpdated = updatedProperties.LastUpdated,
        LastUpdatedBy = updatedProperties.LastUpdatedBy,
        LastUpdatedByName = updatedProperties.LastUpdatedByName
      };
    }
  }
}
