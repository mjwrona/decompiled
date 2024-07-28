// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ITeamFoundationPropertyServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ITeamFoundationPropertyServiceExtensions
  {
    public static PropertiesCollection UpdateProperties(
      this ITeamFoundationPropertyService propertyService,
      IVssRequestContext requestContext,
      ArtifactSpec artifactSpec,
      PropertiesCollection properties)
    {
      properties = properties ?? new PropertiesCollection();
      if (properties.Count > 0)
      {
        using (requestContext.CITimer("UpdatePropertiesElapsedMilliseconds"))
          propertyService.SetProperties(requestContext, artifactSpec, properties.Convert());
      }
      using (requestContext.CITimer("GetPropertiesElapsedMilliseconds"))
      {
        using (TeamFoundationDataReader properties1 = propertyService.GetProperties(requestContext, artifactSpec, (IEnumerable<string>) ArtifactPropertyKinds.AllProperties))
        {
          ArtifactPropertyValue artifactPropertyValue = properties1.Current<StreamingCollection<ArtifactPropertyValue>>().FirstOrDefault<ArtifactPropertyValue>();
          return artifactPropertyValue != null ? artifactPropertyValue.PropertyValues.Convert() : new PropertiesCollection();
        }
      }
    }
  }
}
