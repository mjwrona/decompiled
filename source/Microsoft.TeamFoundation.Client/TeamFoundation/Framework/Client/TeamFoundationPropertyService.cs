// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationPropertyService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class TeamFoundationPropertyService : IPropertyService
  {
    private readonly PropertyWebService m_propertyWebService;
    private readonly TfsConnection m_teamFoundationServer;
    private static ArtifactPropertyValue[] s_emptyArtifactPropertyValueArray = Array.Empty<ArtifactPropertyValue>();

    internal TeamFoundationPropertyService(TfsConnection teamFoundationServer)
    {
      this.m_teamFoundationServer = teamFoundationServer;
      this.m_propertyWebService = new PropertyWebService(teamFoundationServer);
    }

    public ArtifactPropertyValue[] GetProperties(
      ArtifactSpec artifactSpec,
      string[] propertyNameFilters)
    {
      ArgumentUtility.CheckForNull<ArtifactSpec>(artifactSpec, nameof (artifactSpec));
      return this.GetProperties(new ArtifactSpec[1]
      {
        artifactSpec
      }, propertyNameFilters);
    }

    public ArtifactPropertyValue[] GetProperties(
      ArtifactSpec[] artifactSpecs,
      string[] propertyNameFilters)
    {
      return this.GetProperties(artifactSpecs, propertyNameFilters, GetPropertiesOptions.None);
    }

    public ArtifactPropertyValue[] GetProperties(
      ArtifactSpec[] artifactSpecs,
      string[] propertyNameFilters,
      GetPropertiesOptions options)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactSpecs, nameof (artifactSpecs));
      try
      {
        return this.m_propertyWebService.GetProperties(artifactSpecs, propertyNameFilters, (int) options);
      }
      catch (SoapException ex)
      {
        TeamFoundationPropertyService.HandlePropertyException(ex);
      }
      return TeamFoundationPropertyService.s_emptyArtifactPropertyValueArray;
    }

    public void SetProperty(ArtifactSpec artifactSpec, string propertyName, int? value) => this.InternalSetProperty(artifactSpec, propertyName, (object) value);

    public void SetProperty(ArtifactSpec artifactSpec, string propertyName, double? value) => this.InternalSetProperty(artifactSpec, propertyName, (object) value);

    public void SetProperty(ArtifactSpec artifactSpec, string propertyName, string value) => this.InternalSetProperty(artifactSpec, propertyName, (object) value);

    public void SetProperty(ArtifactSpec artifactSpec, string propertyName, DateTime? value) => this.InternalSetProperty(artifactSpec, propertyName, (object) value);

    public void SetProperty(ArtifactSpec artifactSpec, string propertyName, byte[] value) => this.InternalSetProperty(artifactSpec, propertyName, (object) value);

    internal void InternalSetProperty(ArtifactSpec artifactSpec, string propertyName, object value) => this.SetProperties(new ArtifactPropertyValue[1]
    {
      new ArtifactPropertyValue(artifactSpec, new PropertyValue(propertyName, value))
    });

    public void SetProperties(ArtifactPropertyValue artifactPropertyValue) => this.SetProperties(new ArtifactPropertyValue[1]
    {
      artifactPropertyValue
    });

    public void SetProperties(ArtifactPropertyValue[] artifactPropertyValues)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactPropertyValues, nameof (artifactPropertyValues));
      try
      {
        for (int index = 0; index < artifactPropertyValues.Length; ++index)
        {
          ArgumentUtility.CheckForNull<ArtifactPropertyValue>(artifactPropertyValues[index], string.Format((IFormatProvider) CultureInfo.InvariantCulture, "artifactPropertyValues[{0}]", (object) index));
          artifactPropertyValues[index].PrepareInternalValues();
        }
        List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
        for (int index = 0; index < artifactPropertyValues.Length; ++index)
        {
          ArtifactPropertyValue artifactPropertyValue = artifactPropertyValues[index];
          if (artifactPropertyValue.InternalPropertyValues.Length != 0)
            artifactPropertyValueList.Add(artifactPropertyValue);
        }
        this.m_propertyWebService.SetProperties(artifactPropertyValueList.ToArray());
      }
      catch (SoapException ex)
      {
        TeamFoundationPropertyService.HandlePropertyException(ex);
      }
    }

    private static void HandlePropertyException(SoapException se) => throw new PropertyServiceException(SoapExceptionUtilities.GetExceptionMessage(se));
  }
}
