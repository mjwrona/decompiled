// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.PlatformProjectPropertyService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class PlatformProjectPropertyService : BaseProjectPropertyService
  {
    private static readonly Dictionary<string, ProjectProperty> s_empty = new Dictionary<string, ProjectProperty>();
    private const string c_area = "Project";
    private const string c_layer = "PlatformProjectPropertyService";

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationPropertyService>().RegisterNotification(systemRequestContext, TeamProjectPropertyConstants.ArtifactKindId, new ArtifactPropertyValueChangedCallback(this.OnProjectPropertiesChanged));
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationPropertyService>().UnregisterNotification(systemRequestContext, TeamProjectPropertyConstants.ArtifactKindId, new ArtifactPropertyValueChangedCallback(this.OnProjectPropertiesChanged));
      base.ServiceEnd(systemRequestContext);
    }

    public IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> GetProperties(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> filters)
    {
      return this.GetProperties(requestContext, projectIds, filters, false);
    }

    public void DeleteAllProperties(IVssRequestContext requestContext, Guid projectId)
    {
      try
      {
        requestContext.GetService<ITeamFoundationPropertyService>().DeleteArtifacts(requestContext.Elevate(), (IEnumerable<ArtifactSpec>) new ArtifactSpec[1]
        {
          TeamProjectUtil.GetProjectPropertySpec(projectId)
        });
      }
      catch (DataspaceNotFoundException ex)
      {
      }
      ProjectPropertyCacheService service = requestContext.GetService<ProjectPropertyCacheService>();
      this.RemoveCache(requestContext, projectId, service.GetCachedPropertyNames(projectId));
    }

    protected override IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> ReadFromSource(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> filters)
    {
      IDataspaceService dataspaceService = requestContext.GetService<IDataspaceService>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<ArtifactSpec> artifactSpecs = projectIds.Where<Guid>((Func<Guid, bool>) (id => dataspaceService.QueryDataspace(requestContext, "Default", id, false) != null)).Select<Guid, ArtifactSpec>(PlatformProjectPropertyService.\u003C\u003EO.\u003C0\u003E__GetProjectPropertySpec ?? (PlatformProjectPropertyService.\u003C\u003EO.\u003C0\u003E__GetProjectPropertySpec = new Func<Guid, ArtifactSpec>(TeamProjectUtil.GetProjectPropertySpec)));
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, artifactSpecs, filters))
      {
        Dictionary<Guid, Dictionary<string, ProjectProperty>> propertiesByProject = new Dictionary<Guid, Dictionary<string, ProjectProperty>>();
        Dictionary<Guid, HashSet<string>> dictionary = new Dictionary<Guid, HashSet<string>>();
        foreach (ArtifactPropertyValue artifactPropertyValue in properties)
        {
          Guid dataspaceIdentifier = artifactPropertyValue.Spec.DataspaceIdentifier;
          Dictionary<string, ProjectProperty> orAddValue1 = propertiesByProject.GetOrAddValue<Guid, Dictionary<string, ProjectProperty>>(dataspaceIdentifier, (Func<Dictionary<string, ProjectProperty>>) (() => new Dictionary<string, ProjectProperty>((IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName)));
          HashSet<string> orAddValue2 = dictionary.GetOrAddValue<Guid, HashSet<string>>(dataspaceIdentifier, (Func<HashSet<string>>) (() => new HashSet<string>((IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName)));
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
          {
            string propertyName = propertyValue.PropertyName;
            ProjectProperty projectProperty;
            if (orAddValue1.TryGetValue(propertyName, out projectProperty))
            {
              this.TraceDuplicateProperties(requestContext, dataspaceIdentifier, projectProperty.Name, propertyName);
              orAddValue1.Remove(propertyName);
              orAddValue2.Add(propertyName);
            }
            else if (orAddValue2.Contains(propertyName))
              this.TraceDuplicateProperties(requestContext, dataspaceIdentifier, propertyName);
            else
              orAddValue1.Add(propertyName, new ProjectProperty(propertyName, propertyValue.Value));
          }
        }
        foreach (Guid key in projectIds.Where<Guid>((Func<Guid, bool>) (id => !propertiesByProject.ContainsKey(id))))
          propertiesByProject.Add(key, PlatformProjectPropertyService.s_empty);
        return (IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>>) propertiesByProject.ToDictionary<KeyValuePair<Guid, Dictionary<string, ProjectProperty>>, Guid, IEnumerable<ProjectProperty>>((Func<KeyValuePair<Guid, Dictionary<string, ProjectProperty>>, Guid>) (pbp => pbp.Key), (Func<KeyValuePair<Guid, Dictionary<string, ProjectProperty>>, IEnumerable<ProjectProperty>>) (pbp => pbp.Value.Values.AsEnumerable<ProjectProperty>()));
      }
    }

    protected override void WriteToSource(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> properties)
    {
      requestContext.GetService<PlatformProjectService>().SetProjectPropertiesInternal(requestContext, projectId, properties);
    }

    private void OnProjectPropertiesChanged(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactPropertyValueKey> propertyValues)
    {
      foreach (IGrouping<Guid, ArtifactPropertyValueKey> source in propertyValues.GroupBy<ArtifactPropertyValueKey, Guid>((Func<ArtifactPropertyValueKey, Guid>) (pv => pv.Spec.DataspaceIdentifier)))
        this.RemoveCache(requestContext, source.Key, source.Select<ArtifactPropertyValueKey, string>((Func<ArtifactPropertyValueKey, string>) (p => p.PropertyName)));
    }

    private void TraceDuplicateProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      params string[] duplicateProperties)
    {
      requestContext.Trace(5500280, TraceLevel.Error, "Project", nameof (PlatformProjectPropertyService), "Project '{0}' has duplicate properties: {1}.", (object) projectId, (object) string.Join(",", duplicateProperties));
    }
  }
}
