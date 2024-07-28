// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.TagsSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class TagsSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<TagsSqlComponent>(1)
    }, "ReleaseManagementTags", "ReleaseManagement");

    public virtual IEnumerable<string> GetTags(Guid projectId)
    {
      this.PrepareStoredProcedure("Release.prc_GetTags", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => reader.GetString(0))));
        return (IEnumerable<string>) resultCollection.GetCurrent<string>().Items;
      }
    }

    public virtual IEnumerable<string> AddTags(
      Guid projectId,
      int releaseId,
      IEnumerable<string> tags)
    {
      this.PrepareStoredProcedure("Release.prc_AddReleaseTags", projectId);
      this.BindInt("@releaseId", releaseId);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseTagData>((ObjectBinder<ReleaseTagData>) this.GetTagBinder());
        return (IEnumerable<string>) resultCollection.GetCurrent<ReleaseTagData>().Select<ReleaseTagData, string>((System.Func<ReleaseTagData, string>) (t => t.Tag)).ToList<string>();
      }
    }

    public virtual IEnumerable<string> DeleteTags(
      Guid projectId,
      int releaseId,
      IEnumerable<string> tags)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteReleaseTags", projectId);
      this.BindInt("@releaseId", releaseId);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseTagData>((ObjectBinder<ReleaseTagData>) this.GetTagBinder());
        return (IEnumerable<string>) resultCollection.GetCurrent<ReleaseTagData>().Select<ReleaseTagData, string>((System.Func<ReleaseTagData, string>) (t => t.Tag)).ToList<string>();
      }
    }

    public virtual IEnumerable<string> AddDefinitionTags(
      Guid projectId,
      int definitionId,
      IEnumerable<string> tags)
    {
      this.PrepareStoredProcedure("Release.prc_AddDefinitionTags", projectId);
      this.BindInt("@definitionId", definitionId);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionTagData>((ObjectBinder<DefinitionTagData>) this.GetDefinitionTagBinder());
        return (IEnumerable<string>) resultCollection.GetCurrent<DefinitionTagData>().Select<DefinitionTagData, string>((System.Func<DefinitionTagData, string>) (t => t.Tag)).ToList<string>();
      }
    }

    public virtual IEnumerable<string> DeleteDefinitionTags(
      Guid projectId,
      int definitionId,
      IEnumerable<string> tags)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteDefinitionTags", projectId);
      this.BindInt("@definitionId", definitionId);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionTagData>((ObjectBinder<DefinitionTagData>) this.GetDefinitionTagBinder());
        return (IEnumerable<string>) resultCollection.GetCurrent<DefinitionTagData>().Select<DefinitionTagData, string>((System.Func<DefinitionTagData, string>) (t => t.Tag)).ToList<string>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It may be overriden later")]
    protected virtual ReleaseTagBinder GetTagBinder() => new ReleaseTagBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It may be overriden later")]
    protected virtual DefinitionTagBinder GetDefinitionTagBinder() => new DefinitionTagBinder((ReleaseManagementSqlResourceComponentBase) this);
  }
}
