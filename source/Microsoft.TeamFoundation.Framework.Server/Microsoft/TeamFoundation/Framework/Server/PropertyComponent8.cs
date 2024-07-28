// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent8 : PropertyComponent7
  {
    private static readonly SqlMetaData[] typ_ArtifactPropertySpecWithSequenceTable = new SqlMetaData[6]
    {
      new SqlMetaData("SeqId", SqlDbType.Int),
      new SqlMetaData("Version", SqlDbType.Int),
      new SqlMetaData("ArtifactId", SqlDbType.VarBinary, 64L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Moniker", SqlDbType.NVarChar, 440L),
      new SqlMetaData("RequestedVersion", SqlDbType.Int)
    };
    protected bool m_containsWildCards;

    internal override ResultCollection GetPropertyValue(
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters,
      ArtifactKind artifactKind,
      PropertiesOptions options)
    {
      bool parameterValue = false;
      List<string> rows = new List<string>();
      foreach (string propertyNameFilter in propertyNameFilters)
      {
        bool containsWildcards;
        rows.Add(ArtifactSpec.TranslateSqlWildcards(propertyNameFilter, out bool _, out containsWildcards));
        if (containsWildcards)
          parameterValue = true;
      }
      this.PrepareStoredProcedure("prc_GetPropertyValueById");
      this.BindInt("@kindId", artifactKind.CompactKindId);
      this.BindBoolean("@keepInputOrder", true);
      this.BindBoolean("@containsPropertyWildcards", parameterValue);
      this.BindStringTable("@propertyNameFilterList", (IEnumerable<string>) rows, true);
      this.BindBoolean("@queryAllVersions", (options & PropertiesOptions.AllVersions) == PropertiesOptions.AllVersions);
      if (artifactSpecs != null)
      {
        this.BindTable("@artifactSpecs", "typ_ArtifactPropertySpecWithSequence", this.BindArtifactSpecs(artifactSpecs, options, artifactKind));
        this.BindBoolean("@containsArtifactWildcards", this.m_containsWildCards);
      }
      ResultCollection propertyValue = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyValueById", this.RequestContext);
      propertyValue.AddBinder<DbArtifactPropertyValue>(this.CreatePropertyValueBinder());
      return propertyValue;
    }

    internal override ResultCollection GetPropertyValue(
      ArtifactKind artifactKind,
      IEnumerable<string> propertyNameFilters,
      Guid? dataspaceIdentifier)
    {
      bool parameterValue = false;
      List<string> rows = new List<string>();
      foreach (string propertyNameFilter in propertyNameFilters)
      {
        bool containsWildcards;
        rows.Add(ArtifactSpec.TranslateSqlWildcards(propertyNameFilter, out bool _, out containsWildcards));
        if (containsWildcards)
          parameterValue = true;
      }
      this.PrepareStoredProcedure("prc_GetPropertyValueById");
      this.BindInt("@kindId", artifactKind.CompactKindId);
      this.BindBoolean("@keepInputOrder", true);
      this.BindBoolean("@containsPropertyWildcards", parameterValue);
      this.BindStringTable("@propertyNameFilterList", (IEnumerable<string>) rows, true);
      this.BindBoolean("@queryAllVersions", false);
      if (dataspaceIdentifier.HasValue)
        this.BindDataspaces(new Guid[1]
        {
          dataspaceIdentifier.Value
        });
      ResultCollection propertyValue = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyValueById", this.RequestContext);
      propertyValue.AddBinder<DbArtifactPropertyValue>(this.CreatePropertyValueBinder());
      return propertyValue;
    }

    protected IEnumerable<SqlDataRecord> BindArtifactSpecs(
      IEnumerable<ArtifactSpec> specs,
      PropertiesOptions options,
      ArtifactKind artifactKind)
    {
      Func<ArtifactSpec, int, int, SqlDataRecord> createRecord = (Func<ArtifactSpec, int, int, SqlDataRecord>) ((spec, current, version) =>
      {
        this.ValidateArtifactSpec(spec, options, artifactKind);
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PropertyComponent8.typ_ArtifactPropertySpecWithSequenceTable);
        sqlDataRecord.SetInt32(0, current);
        sqlDataRecord.SetInt32(1, version);
        if (string.IsNullOrEmpty(spec.Moniker))
        {
          sqlDataRecord.SetBytes(2, 0L, spec.Id, 0, spec.Id.Length);
          sqlDataRecord.SetDBNull(4);
        }
        else
        {
          sqlDataRecord.SetDBNull(2);
          sqlDataRecord.SetString(4, spec.Moniker);
        }
        sqlDataRecord.SetInt32(3, this.GetDataspaceId(spec.DataspaceIdentifier));
        if ((options & PropertiesOptions.AllVersions) != PropertiesOptions.AllVersions)
          sqlDataRecord.SetInt32(5, spec.Version);
        else
          sqlDataRecord.SetDBNull(5);
        return sqlDataRecord;
      });
      int ctr = 0;
      foreach (ArtifactSpec spec in specs)
      {
        bool containsWildcards;
        ArtifactSpec translatedSpec = ArtifactSpec.TranslateSqlWildcards(spec, out containsWildcards);
        if (containsWildcards)
          this.m_containsWildCards = true;
        yield return createRecord(translatedSpec, ctr, translatedSpec.Version);
        if ((options & PropertiesOptions.AllVersions) != PropertiesOptions.AllVersions && translatedSpec.Version != 0)
          yield return createRecord(translatedSpec, ctr, 0);
        ++ctr;
        translatedSpec = (ArtifactSpec) null;
      }
    }

    protected void ValidateArtifactSpec(
      ArtifactSpec artifactSpec,
      PropertiesOptions options,
      ArtifactKind kind)
    {
      ArgumentUtility.CheckForNull<ArtifactSpec>(artifactSpec, nameof (artifactSpec));
      if (artifactSpec.Kind != kind.Kind)
        throw new ArtifactKindsMustBeUniformException();
      if (!kind.IsMonikerBased && !string.IsNullOrEmpty(artifactSpec.Moniker))
        throw new TeamFoundationValidationException(FrameworkResources.ArtifactKindMonikerDisallowed((object) kind.Kind), nameof (artifactSpec));
      artifactSpec.Validate(options);
    }
  }
}
