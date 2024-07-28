// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityPropertyHelper`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  public abstract class IdentityPropertyHelper<T> where T : class
  {
    private const string s_area = "Identity";
    private const string s_layer = "IdentityPropertyHelper";
    private const string c_FeatureDisableIdentityPropertyHelperEnumerationFix = "VisualStudio.Services.Identity.DisableIdentityPropertyHelperEnumerationFix";

    public void ReadExtendedProperties(
      IVssRequestContext requestContext,
      IEnumerable<T> identities,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableIdentityPropertyHelperEnumerationFix"))
        identities = (IEnumerable<T>) identities.ToList<T>();
      if (propertyScope == IdentityPropertyScope.Both || propertyScope == IdentityPropertyScope.Global)
        this.FetchExtendedProperties(requestContext.To(TeamFoundationHostType.Deployment), identities, propertyNameFilters, IdentityPropertyScope.Global);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || propertyScope != IdentityPropertyScope.Both && propertyScope != IdentityPropertyScope.Local)
        return;
      this.FetchExtendedProperties(requestContext, identities, propertyNameFilters, IdentityPropertyScope.Local);
    }

    public IEnumerable<T> UpdateExtendedProperties(
      IVssRequestContext requestContext,
      IdentityPropertyScope propertyScope,
      IEnumerable<T> identities,
      Func<IVssRequestContext, T, IdentityPropertyScope, string, bool> skipPropertyUpdate = null)
    {
      IEnumerable<T> objs = (IEnumerable<T>) null;
      if (propertyScope == IdentityPropertyScope.Both || propertyScope == IdentityPropertyScope.Global)
        objs = this.UpdateHostSpecificExtendedProperties(requestContext.To(TeamFoundationHostType.Deployment), IdentityPropertyScope.Global, identities, skipPropertyUpdate);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && (propertyScope == IdentityPropertyScope.Both || propertyScope == IdentityPropertyScope.Local))
        objs = this.UpdateHostSpecificExtendedProperties(requestContext, IdentityPropertyScope.Local, identities, skipPropertyUpdate);
      foreach (T identity in identities)
        this.ResetModifiedProperties(propertyScope, identity);
      return objs;
    }

    public void ClearExtendedProperties(
      IVssRequestContext requestContext,
      IdentityPropertyScope propertyScope,
      IEnumerable<T> identities)
    {
      if (propertyScope == IdentityPropertyScope.Both || propertyScope == IdentityPropertyScope.Global)
        this.ClearHostSpecificExtendedProperties(requestContext.To(TeamFoundationHostType.Deployment), IdentityPropertyScope.Global, identities);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || propertyScope != IdentityPropertyScope.Both && propertyScope != IdentityPropertyScope.Local)
        return;
      this.ClearHostSpecificExtendedProperties(requestContext, IdentityPropertyScope.Local, identities);
    }

    internal ArtifactSpec MakeArtifactSpec(IdentityPropertyScope propertyScope, T identity)
    {
      Guid artifactId;
      Guid kind;
      if (propertyScope == IdentityPropertyScope.Global)
      {
        artifactId = this.GetArtifactId(propertyScope, identity);
        kind = ArtifactKinds.Identity;
      }
      else
      {
        artifactId = this.GetArtifactId(propertyScope, identity);
        kind = ArtifactKinds.LocalIdentity;
      }
      return !Guid.Empty.Equals(artifactId) ? new ArtifactSpec(kind, artifactId.ToByteArray(), 0) : throw new ArgumentOutOfRangeException(nameof (identity));
    }

    public List<ArtifactSpec> MakeArtifactSpecs(
      IdentityPropertyScope propertyScope,
      IEnumerable<T> identities)
    {
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>();
      foreach (T identity in identities)
      {
        if ((object) identity != null)
          artifactSpecList.Add(this.MakeArtifactSpec(propertyScope, identity));
      }
      return artifactSpecList;
    }

    private List<ArtifactPropertyValue> MakeArtifactPropertyValues(
      IdentityPropertyScope propertyScope,
      IEnumerable<T> identities,
      IEnumerable<PropertyValue> propertyValues)
    {
      List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
      foreach (T identity in identities)
      {
        if ((object) identity != null)
          artifactPropertyValueList.Add(new ArtifactPropertyValue(this.MakeArtifactSpec(propertyScope, identity), propertyValues));
      }
      return artifactPropertyValueList;
    }

    private void FetchExtendedProperties(
      IVssRequestContext requestContext,
      IEnumerable<T> identities,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      List<ArtifactSpec> artifactSpecList = this.MakeArtifactSpecs(propertyScope, identities);
      if (artifactSpecList.Count <= 0)
        return;
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, propertyNameFilters))
      {
        IEnumerator<T> enumerator = identities.GetEnumerator();
        bool flag = enumerator.MoveNext();
        T current = enumerator.Current;
        foreach (ArtifactPropertyValue artifactPropertyValue in properties)
        {
          for (Guid guid = new Guid(artifactPropertyValue.Spec.Id); (object) current == null || this.GetArtifactId(propertyScope, current) != guid; current = enumerator.Current)
          {
            flag = enumerator.MoveNext();
            if (!flag)
              break;
          }
          if (!flag)
            break;
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
            this.SetProperty(propertyScope, current, propertyValue.PropertyName, propertyValue.Value);
        }
      }
    }

    internal virtual IEnumerable<T> UpdateHostSpecificExtendedProperties(
      IVssRequestContext requestContext,
      IdentityPropertyScope propertyScope,
      IEnumerable<T> identities,
      Func<IVssRequestContext, T, IdentityPropertyScope, string, bool> skipPropertyUpdate = null)
    {
      List<T> objList = new List<T>();
      List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
      StringBuilder stringBuilder = new StringBuilder();
      foreach (T identity1 in identities)
      {
        HashSet<string> modifiedProperties = this.GetModifiedProperties(propertyScope, identity1);
        if (modifiedProperties != null)
        {
          objList.Add(identity1);
          List<PropertyValue> source = new List<PropertyValue>(modifiedProperties.Count);
          foreach (string propertyName in modifiedProperties)
          {
            if (skipPropertyUpdate == null || !skipPropertyUpdate(requestContext, identity1, propertyScope, propertyName))
              source.Add(new PropertyValue(propertyName, this.GetProperty(propertyScope, identity1, propertyName)));
          }
          if (requestContext.IsTracing(80580, TraceLevel.Verbose, "Identity", nameof (IdentityPropertyHelper<T>)))
            stringBuilder.AppendFormat("[Identity: {0}, propertyNames: {1}], ", identity1 is Microsoft.VisualStudio.Services.Identity.Identity identity2 ? (object) identity2.Id.ToString() : (object) (string) null, (object) source.Select<PropertyValue, string>((Func<PropertyValue, string>) (x => x.PropertyName)).Serialize<IEnumerable<string>>());
          artifactPropertyValueList.Add(new ArtifactPropertyValue(this.MakeArtifactSpec(propertyScope, identity1), (IEnumerable<PropertyValue>) source));
        }
      }
      if (stringBuilder.Length > 0)
        requestContext.Trace(80580, TraceLevel.Verbose, "Identity", nameof (IdentityPropertyHelper<T>), "Setting properties - " + stringBuilder.ToString());
      requestContext.GetService<TeamFoundationPropertyService>().SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList);
      return (IEnumerable<T>) objList;
    }

    private void ClearHostSpecificExtendedProperties(
      IVssRequestContext requestContext,
      IdentityPropertyScope propertyScope,
      IEnumerable<T> identities)
    {
      TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
      List<ArtifactSpec> artifactSpecList = this.MakeArtifactSpecs(propertyScope, identities);
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) null))
      {
        foreach (ArtifactPropertyValue artifactPropertyValue in properties)
        {
          List<PropertyValue> propertyValueList = new List<PropertyValue>();
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
          {
            propertyValue.Value = (object) null;
            propertyValueList.Add(propertyValue);
          }
          service.SetProperties(requestContext, artifactPropertyValue.Spec, (IEnumerable<PropertyValue>) propertyValueList);
        }
      }
    }

    protected abstract Guid GetArtifactId(IdentityPropertyScope propertyScope, T identity);

    protected abstract void SetProperty(
      IdentityPropertyScope propertyScope,
      T identity,
      string propertyName,
      object propertyValue);

    protected abstract object GetProperty(
      IdentityPropertyScope propertyScope,
      T identity,
      string propertyName);

    protected abstract HashSet<string> GetModifiedProperties(
      IdentityPropertyScope propertyScope,
      T identity);

    protected abstract void ResetModifiedProperties(IdentityPropertyScope propertyScope, T identity);
  }
}
