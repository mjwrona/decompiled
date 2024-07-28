// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagementPropertyHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete]
  public static class HostManagementPropertyHelper
  {
    public static PropertiesCollection FetchExtendedProperties(
      IVssRequestContext requestContext,
      Guid hostId,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.CheckDeploymentRequestContext();
      using (TeamFoundationDataReader properties = requestContext.GetService<TeamFoundationPropertyService>().GetProperties(requestContext, HostManagementPropertyHelper.CreateArtifactSpec(hostId), propertyNameFilters))
      {
        ArtifactPropertyValue artifactPropertyValue = properties.Current<StreamingCollection<ArtifactPropertyValue>>().FirstOrDefault<ArtifactPropertyValue>();
        return artifactPropertyValue == null ? new PropertiesCollection() : new PropertiesCollection((IDictionary<string, object>) artifactPropertyValue.PropertyValues.ToDictionary<PropertyValue, string, object>((Func<PropertyValue, string>) (pv => pv.PropertyName), (Func<PropertyValue, object>) (pv => pv.Value)));
      }
    }

    public static void UpdateServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId,
      IEnumerable<PropertyValue> properties)
    {
      ArgumentUtility.CheckForNull<IEnumerable<PropertyValue>>(properties, nameof (properties));
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<TeamFoundationPropertyService>().SetProperties(requestContext, HostManagementPropertyHelper.CreateArtifactSpec(hostId), properties);
    }

    private static ArtifactSpec CreateArtifactSpec(Guid hostId) => new ArtifactSpec(ArtifactKinds.ServiceHost, hostId.ToByteArray(), 0);
  }
}
