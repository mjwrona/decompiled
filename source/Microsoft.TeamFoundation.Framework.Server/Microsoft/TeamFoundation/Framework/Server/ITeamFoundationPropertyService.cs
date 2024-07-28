// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationPropertyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationPropertyService))]
  public interface ITeamFoundationPropertyService : IVssFrameworkService
  {
    void CreateArtifactKind(IVssRequestContext requestContext, ArtifactKind artifactKind);

    IEnumerable<ArtifactKind> GetArtifactKinds(IVssRequestContext requestContext);

    ArtifactKind GetArtifactKind(IVssRequestContext requestContext, Guid kind);

    void DeleteArtifactKind(IVssRequestContext requestContext, Guid kind);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      ArtifactSpec artifactSpec,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters,
      PropertiesOptions options);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      IEnumerable<string> propertyNameFilters,
      Guid? dataspaceIdentifier);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      string expression,
      IEnumerable<string> propertyNameFilters);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      string expression,
      IEnumerable<string> propertyNameFilters,
      Guid[] dataspaceIds);

    TeamFoundationDataReader GetProperties(
      IVssRequestContext requestContext,
      Guid kind,
      string expression,
      IEnumerable<string> propertyNameFilters,
      Guid[] dataspaceIdentifiers,
      PropertiesOptions options);

    bool SetProperties(
      IVssRequestContext requestContext,
      ArtifactSpec artifactSpec,
      IEnumerable<PropertyValue> propertyValues);

    bool SetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues);

    bool SetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues,
      DateTime? changedDate,
      Guid? changedBy);

    bool SetProperties(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<PropertyValue> propertyValues);

    int DeleteProperties(
      IVssRequestContext requestContext,
      Guid kind,
      IEnumerable<string> propertyNames,
      int batchSize = 2000,
      int? maxPropertiesToDelete = null);

    void DeleteArtifacts(IVssRequestContext requestContext, IEnumerable<ArtifactSpec> artifactSpecs);

    void DeleteArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactSpec> artifactSpecs,
      PropertiesOptions options);

    void RegisterNotification(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ArtifactPropertyValueChangedCallback callback);

    void UnregisterNotification(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ArtifactPropertyValueChangedCallback callback);
  }
}
