// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleasePropertyExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleasePropertyExtensions
  {
    public static readonly Guid Release = new Guid("{9DA2EDC3-D038-46A2-A380-E745EE66CB5B}");

    public static IList<PropertyValue> GetReleasePropertyValues(
      IVssRequestContext requestContext,
      int releaseId,
      Guid dataspaceIdentifier,
      IEnumerable<string> propertiesFilter)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      List<PropertyValue> releasePropertyValues = new List<PropertyValue>();
      IVssRequestContext requestContext1 = requestContext;
      ArtifactSpec artifactSpec = ReleasePropertyExtensions.CreateArtifactSpec(releaseId, dataspaceIdentifier);
      IEnumerable<string> propertyNameFilters = propertiesFilter;
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpec, propertyNameFilters))
      {
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
          releasePropertyValues.AddRange((IEnumerable<PropertyValue>) current.PropertyValues);
      }
      return (IList<PropertyValue>) releasePropertyValues;
    }

    public static ArtifactSpec CreateArtifactSpec(int releaseId, Guid dataspaceIdentifier) => new ArtifactSpec(ReleasePropertyExtensions.Release, releaseId, 0, dataspaceIdentifier);
  }
}
