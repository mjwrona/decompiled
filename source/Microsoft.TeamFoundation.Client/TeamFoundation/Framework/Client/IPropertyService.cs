// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IPropertyService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface IPropertyService
  {
    ArtifactPropertyValue[] GetProperties(ArtifactSpec artifactSpec, string[] propertyNameFilters);

    ArtifactPropertyValue[] GetProperties(
      ArtifactSpec[] artifactSpecs,
      string[] propertyNameFilters);

    ArtifactPropertyValue[] GetProperties(
      ArtifactSpec[] artifactSpecs,
      string[] propertyNameFilters,
      GetPropertiesOptions options);

    void SetProperty(ArtifactSpec artifactSpec, string propertyName, int? value);

    void SetProperty(ArtifactSpec artifactSpec, string propertyName, double? value);

    void SetProperty(ArtifactSpec artifactSpec, string propertyName, string value);

    void SetProperty(ArtifactSpec artifactSpec, string propertyName, DateTime? value);

    void SetProperty(ArtifactSpec artifactSpec, string propertyName, byte[] value);

    void SetProperties(ArtifactPropertyValue artifactPropertyValues);

    void SetProperties(ArtifactPropertyValue[] artifactPropertyValues);
  }
}
