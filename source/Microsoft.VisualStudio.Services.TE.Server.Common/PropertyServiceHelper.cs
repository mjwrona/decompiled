// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.PropertyServiceHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class PropertyServiceHelper : IPropertyServiceHelper
  {
    public void RegisterInternalArtifactKind(TestExecutionRequestContext context, Guid artifactKind)
    {
      try
      {
        context.RequestContext.GetService<ITeamFoundationPropertyService>().CreateArtifactKind(context.RequestContext, new ArtifactKind()
        {
          Kind = artifactKind,
          Flags = ArtifactKindFlags.None,
          DataspaceCategory = "Default",
          IsInternalKind = true,
          IsMonikerBased = true
        });
      }
      catch (TeamFoundationServerException ex)
      {
        context.RequestContext.TraceException(6200000, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, (Exception) ex);
      }
    }

    public void AddOrUpdate(
      TestExecutionRequestContext context,
      Guid artifactKind,
      int artifactId,
      IDictionary<string, object> properties)
    {
      ArtifactSpec artifcactSpec = PropertyServiceHelper.GetArtifcactSpec(artifactKind, artifactId);
      PropertyServiceHelper.AddOrUpdate(context, artifcactSpec, properties);
    }

    public void AddOrUpdate(
      TestExecutionRequestContext context,
      Guid artifactKind,
      string artifactId,
      IDictionary<string, object> properties)
    {
      ArtifactSpec artifactSpec = new ArtifactSpec(artifactKind, artifactId, 0);
      PropertyServiceHelper.AddOrUpdate(context, artifactSpec, properties);
    }

    public IDictionary<string, object> Get(
      TestExecutionRequestContext context,
      Guid artifactKind,
      int artifactId,
      IEnumerable<string> propertyNameFilters)
    {
      ArtifactSpec artifcactSpec = PropertyServiceHelper.GetArtifcactSpec(artifactKind, artifactId);
      return this.Get(context, artifcactSpec, propertyNameFilters);
    }

    public IDictionary<string, object> Get(
      TestExecutionRequestContext context,
      Guid artifactKind,
      string artifactId,
      IEnumerable<string> propertyNameFilters)
    {
      ArtifactSpec artifactSpec = new ArtifactSpec(artifactKind, artifactId, 0);
      return this.Get(context, artifactSpec, propertyNameFilters);
    }

    private IDictionary<string, object> Get(
      TestExecutionRequestContext context,
      ArtifactSpec artifactSpec,
      IEnumerable<string> propertyNameFilters)
    {
      ITeamFoundationPropertyService service = context.RequestContext.GetService<ITeamFoundationPropertyService>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      IVssRequestContext requestContext = context.RequestContext;
      ArtifactSpec artifactSpec1 = artifactSpec;
      IEnumerable<string> propertyNameFilters1 = propertyNameFilters;
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, artifactSpec1, propertyNameFilters1))
      {
        ArtifactPropertyValue artifactPropertyValue = properties.CurrentEnumerable<ArtifactPropertyValue>().FirstOrDefault<ArtifactPropertyValue>();
        if (artifactPropertyValue != null)
        {
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
            dictionary.Add(propertyValue.PropertyName, propertyValue.Value);
        }
      }
      return (IDictionary<string, object>) dictionary;
    }

    public void Delete(TestExecutionRequestContext context, Guid artifactKind, int artifactId)
    {
      ITeamFoundationPropertyService service = context.RequestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifcactSpec = PropertyServiceHelper.GetArtifcactSpec(artifactKind, artifactId);
      IVssRequestContext requestContext = context.RequestContext;
      ArtifactSpec[] artifactSpecArray = new ArtifactSpec[1]
      {
        artifcactSpec
      };
      service.DeleteArtifacts(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecArray);
    }

    public void Delete(TestExecutionRequestContext context, Guid artifactKind, string artifactId)
    {
      ITeamFoundationPropertyService service = context.RequestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifactSpec = new ArtifactSpec(artifactKind, artifactId, 0);
      IVssRequestContext requestContext = context.RequestContext;
      ArtifactSpec[] artifactSpecArray = new ArtifactSpec[1]
      {
        artifactSpec
      };
      service.DeleteArtifacts(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecArray);
    }

    private static void AddOrUpdate(
      TestExecutionRequestContext context,
      ArtifactSpec artifactSpec,
      IDictionary<string, object> properties)
    {
      ITeamFoundationPropertyService service = context.RequestContext.GetService<ITeamFoundationPropertyService>();
      List<PropertyValue> propertyValueList = new List<PropertyValue>();
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
        propertyValueList.Add(new PropertyValue(property.Key, property.Value));
      service.SetProperties(context.RequestContext, artifactSpec, (IEnumerable<PropertyValue>) propertyValueList);
    }

    private static ArtifactSpec GetArtifcactSpec(Guid artifactKind, int artifactId)
    {
      string moniker = string.Format("{0}", (object) artifactId);
      return new ArtifactSpec(artifactKind, moniker, 0);
    }
  }
}
