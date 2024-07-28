// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent39
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent39 : ReleaseDefinitionSqlComponent38
  {
    public override IEnumerable<ReleaseDefinition> ListAllReleaseDefinitions(Guid projectId)
    {
      this.PrepareStoredProcedure("Release.prc_QueryAllReleaseDefinitions", projectId);
      List<ShallowReference> items1;
      List<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping> items2;
      List<ReleaseDefinition> items3;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowReference>((ObjectBinder<ShallowReference>) new ShallowReferenceBinder());
        resultCollection.AddBinder<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping>((ObjectBinder<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping>) new ReleaseDefinitionSqlComponent39.ReleaseDefinitionIdFolderIdBinder());
        resultCollection.AddBinder<ReleaseDefinition>((ObjectBinder<ReleaseDefinition>) this.GetReleaseDefinitionBinder());
        items1 = resultCollection.GetCurrent<ShallowReference>().Items;
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping>().Items;
        resultCollection.NextResult();
        items3 = resultCollection.GetCurrent<ReleaseDefinition>().Items;
      }
      Dictionary<int, string> dictionary1 = items1.ToDictionary<ShallowReference, int, string>((System.Func<ShallowReference, int>) (e => e.Id), (System.Func<ShallowReference, string>) (e => PathHelper.DBPathToServerPath(e.Name)));
      Dictionary<int, int> dictionary2 = items2.ToDictionary<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping, int, int>((System.Func<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping, int>) (e => e.ReleaseDefinitionId), (System.Func<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping, int>) (e => e.FolderId));
      foreach (ReleaseDefinition releaseDefinition in items3)
      {
        int key = dictionary2.ContainsKey(releaseDefinition.Id) ? dictionary2[releaseDefinition.Id] : 0;
        if (key > 0 && dictionary1.ContainsKey(key))
          releaseDefinition.Path = dictionary1[key];
      }
      return ReleaseDefinitionSqlComponent21.SortDefinitionsOnQueryOrder((IList<ReleaseDefinition>) items3, ReleaseDefinitionQueryOrder.IdAscending);
    }

    private class ReleaseDefinitionIdFolderIdBinder : 
      ObjectBinder<ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping>
    {
      private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
      private SqlColumnBinder folderId = new SqlColumnBinder("FolderId");

      protected override ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping Bind() => new ReleaseDefinitionSqlComponent39.ReleaseDefinitionFolderMapping()
      {
        ReleaseDefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader),
        FolderId = this.folderId.GetInt32((IDataReader) this.Reader)
      };
    }

    private class ReleaseDefinitionFolderMapping
    {
      public int ReleaseDefinitionId { get; set; }

      public int FolderId { get; set; }
    }
  }
}
