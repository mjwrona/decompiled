// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommercePropertyStore
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommercePropertyStore
  {
    public bool HasPropertyKind(IVssRequestContext requestContext, Guid propertyKind)
    {
      this.ValidateRequestContext(requestContext);
      try
      {
        return requestContext.GetService<ITeamFoundationPropertyService>().GetArtifactKind(requestContext, propertyKind) != null;
      }
      catch (PropertyServiceException ex)
      {
      }
      return false;
    }

    public void CreatePropertyKind(
      IVssRequestContext requestContext,
      Guid propertyKind,
      string description,
      bool monikerBased = false,
      bool internalKind = false)
    {
      this.ValidateRequestContext(requestContext);
      ArtifactKind artifactKind = new ArtifactKind()
      {
        Kind = propertyKind,
        Description = description,
        IsInternalKind = internalKind,
        IsMonikerBased = monikerBased,
        DataspaceCategory = "Default"
      };
      requestContext.GetService<ITeamFoundationPropertyService>().CreateArtifactKind(requestContext, artifactKind);
    }

    public void DeletePropertyKind(IVssRequestContext requestContext, Guid propertyKind)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.GetService<ITeamFoundationPropertyService>().DeleteArtifactKind(requestContext, propertyKind);
    }

    public PropertiesCollection GetProperties(
      IVssRequestContext requestContext,
      Guid propertyKind,
      IEnumerable<string> propertyNames = null)
    {
      this.ValidateRequestContext(requestContext);
      PropertiesCollection properties1 = new PropertiesCollection();
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifactSpec1 = this.MakeArtifactSpec(requestContext, propertyKind);
      IVssRequestContext requestContext1 = requestContext;
      ArtifactSpec artifactSpec2 = artifactSpec1;
      IEnumerable<string> propertyNameFilters = propertyNames;
      using (TeamFoundationDataReader properties2 = service.GetProperties(requestContext1, artifactSpec2, propertyNameFilters))
      {
        if (properties2 == null)
          return properties1;
        foreach (ArtifactPropertyValue artifactPropertyValue in properties2)
        {
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
            properties1.Add(propertyValue.PropertyName, (object) propertyValue.Value.ToString());
        }
      }
      return properties1;
    }

    public void UpdateProperties(
      IVssRequestContext requestContext,
      Guid propertyKind,
      PropertiesCollection properties)
    {
      this.ValidateRequestContext(requestContext);
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      if (properties.IsNullOrEmpty<KeyValuePair<string, object>>())
        return;
      ArtifactSpec artifactSpec = this.MakeArtifactSpec(requestContext, propertyKind);
      service.SetProperties(requestContext, artifactSpec, properties.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (x => new PropertyValue(x.Key, x.Value))));
    }

    public void DeleteProperties(
      IVssRequestContext requestContext,
      Guid propertyKind,
      IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(requestContext);
      if (propertyNames.IsNullOrEmpty<string>())
        return;
      requestContext.GetService<ITeamFoundationPropertyService>().DeleteProperties(requestContext, propertyKind, propertyNames);
    }

    private ArtifactSpec MakeArtifactSpec(IVssRequestContext requestContext, Guid propertyKind) => new ArtifactSpec(propertyKind, requestContext.ServiceHost.InstanceId.ToByteArray(), 0);

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }
  }
}
