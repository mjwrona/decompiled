// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent13
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent13 : PropertyComponent12
  {
    internal override ResultCollection GetPropertyValue(
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters,
      ArtifactKind artifactKind,
      PropertiesOptions options)
    {
      bool wasTranslated;
      bool containsWildcards1;
      ResultCollection propertyValue;
      if (artifactSpecs.Count<ArtifactSpec>() == 1 && propertyNameFilters.Count<string>() == 1)
      {
        this.PrepareStoredProcedure("prc_GetPropertyValueByArtifact");
        this.BindInt("@kindId", artifactKind.CompactKindId);
        ArtifactSpec artifactSpec = artifactSpecs.FirstOrDefault<ArtifactSpec>();
        if ((options & PropertiesOptions.AllVersions) != PropertiesOptions.AllVersions)
          this.BindInt("@version", artifactSpec.Version);
        else
          this.BindNullValue("@version", SqlDbType.Int);
        bool containsWildcards2 = false;
        if (string.IsNullOrEmpty(artifactSpec.Moniker))
        {
          this.BindBinary("@artifactId", artifactSpec.Id, SqlDbType.VarBinary);
          this.BindNullValue("@moniker", SqlDbType.NVarChar);
        }
        else
        {
          artifactSpec = ArtifactSpec.TranslateSqlWildcards(artifactSpec, out containsWildcards2);
          this.BindString("@moniker", artifactSpec.Moniker, 440, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          this.BindNullValue("@artifactId", SqlDbType.VarBinary);
        }
        this.BindInt("@dataspaceId", this.GetDataspaceId(artifactSpec.DataspaceIdentifier));
        this.BindBoolean("@containsArtifactWildcards", containsWildcards2);
        this.BindString("@propertyName", ArtifactSpec.TranslateSqlWildcards(propertyNameFilters.First<string>(), out wasTranslated, out containsWildcards1), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        propertyValue = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyValueByArtifact", this.RequestContext);
      }
      else
      {
        List<string> rows = new List<string>();
        foreach (string propertyNameFilter in propertyNameFilters)
          rows.Add(ArtifactSpec.TranslateSqlWildcards(propertyNameFilter, out wasTranslated, out containsWildcards1));
        this.PrepareStoredProcedure("prc_GetPropertyValueByArtifacts");
        this.BindInt("@kindId", artifactKind.CompactKindId);
        this.BindStringTable("@propertyNameFilterList", (IEnumerable<string>) rows, true);
        this.BindBoolean("@queryAllVersions", (options & PropertiesOptions.AllVersions) == PropertiesOptions.AllVersions);
        this.BindTable("@artifactSpecs", "typ_ArtifactPropertySpecWithSequence", this.BindArtifactSpecs(artifactSpecs, options, artifactKind));
        this.BindBoolean("@containsArtifactWildcards", this.m_containsWildCards);
        propertyValue = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyValueByArtifacts", this.RequestContext);
      }
      propertyValue.AddBinder<DbArtifactPropertyValue>(this.CreatePropertyValueBinder());
      return propertyValue;
    }

    internal override ResultCollection GetPropertyValue(
      ArtifactKind artifactKind,
      IEnumerable<string> propertyNameFilters,
      Guid? dataspaceIdentifier)
    {
      List<string> rows = new List<string>();
      foreach (string propertyNameFilter in propertyNameFilters)
        rows.Add(ArtifactSpec.TranslateSqlWildcards(propertyNameFilter, out bool _, out bool _));
      this.PrepareStoredProcedure("prc_GetPropertyValueByName");
      this.BindInt("@kindId", artifactKind.CompactKindId);
      if (dataspaceIdentifier.HasValue)
        this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier.Value));
      else
        this.BindNullValue("@dataspaceId", SqlDbType.Int);
      this.BindStringTable("@propertyNameFilterList", (IEnumerable<string>) rows, true);
      this.BindBoolean("@monikerBased", artifactKind.IsMonikerBased);
      ResultCollection propertyValue = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyValueByName", this.RequestContext);
      propertyValue.AddBinder<DbArtifactPropertyValue>(this.CreatePropertyValueBinder());
      return propertyValue;
    }
  }
}
