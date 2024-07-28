// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent5 : PropertyComponent4
  {
    internal override void DeleteArtifacts(
      IEnumerable<ArtifactSpec> artifactSpecs,
      ArtifactKind kind,
      PropertiesOptions options)
    {
      using (ArtifactSpecDbPagingManager specDbPagingManager = new ArtifactSpecDbPagingManager(this.RequestContext, PropertiesOptions.None, (PropertyComponent) this))
      {
        specDbPagingManager.EnqueueAll(artifactSpecs);
        specDbPagingManager.Flush();
        if (specDbPagingManager.TotalCount == 0)
          return;
        this.PrepareStoredProcedure("prc_DeleteArtifacts");
        this.BindInt("@kindId", specDbPagingManager.PagedArtifactKind.CompactKindId);
        this.BindGuid("@author", this.Author);
        if (specDbPagingManager.IsPaged)
          this.BindInt("@lobParam", specDbPagingManager.ParameterId);
        else
          this.BindString("@artifactSpecList", specDbPagingManager.ServerItemWriter.ToString(), -1, false, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    internal override ResultCollection GetPropertyValue(
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters,
      ArtifactKind artifactKind,
      PropertiesOptions options)
    {
      ArtifactSpecDbPagingManager specDbPagingManager = (ArtifactSpecDbPagingManager) null;
      try
      {
        if (artifactSpecs != null)
        {
          specDbPagingManager = new ArtifactSpecDbPagingManager(this.RequestContext, options, (PropertyComponent) this);
          specDbPagingManager.EnqueueAll(artifactSpecs);
        }
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
        if (specDbPagingManager != null)
        {
          this.BindBoolean("@containsArtifactWildcards", specDbPagingManager.ContainsWildcards);
          if (specDbPagingManager.IsPaged)
            this.BindInt("@lobParam", specDbPagingManager.ParameterId);
          else
            this.BindString("@artifactSpecList", specDbPagingManager.ServerItemWriter.ToString(), -1, false, SqlDbType.NVarChar);
        }
        ResultCollection propertyValue = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyValueById", this.RequestContext);
        propertyValue.AddBinder<DbArtifactPropertyValue>(this.CreatePropertyValueBinder());
        return propertyValue;
      }
      finally
      {
        specDbPagingManager?.Dispose();
      }
    }

    internal override bool SetPropertyValue(
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues,
      ArtifactKind kind,
      DateTime? changedDate,
      Guid? changedBy)
    {
      this.PrepareStoredProcedure("prc_SetPropertyValue");
      using (ArtifactPropertyValueDbPagingManager valueDbPagingManager = new ArtifactPropertyValueDbPagingManager(this.RequestContext, (PropertyComponent) this))
      {
        valueDbPagingManager.EnqueueAll(artifactPropertyValues);
        if (valueDbPagingManager.TotalCount == 0)
          return false;
        this.BindInt("@kindId", valueDbPagingManager.PagedArtifactKind.CompactKindId);
        this.BindGuid("@author", this.Author);
        if (valueDbPagingManager.IsPaged)
          this.BindInt("@lobParam", valueDbPagingManager.ParameterId);
        else
          this.BindString("@propertyValueList", valueDbPagingManager.ServerItemWriter.ToString(), -1, false, SqlDbType.NVarChar);
        if (changedDate.HasValue)
          this.BindDateTime("@changedDate", changedDate.Value);
        if (changedBy.HasValue)
          this.BindGuid("@changedBy", changedBy.Value);
        this.ExecuteNonQuery();
        return true;
      }
    }
  }
}
