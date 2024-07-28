// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseWebHookSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseWebHookSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ReleaseWebHookSqlComponent>(1)
    }, "ReleaseManagementWebHook", "ReleaseManagement");

    public virtual WebHook CreateWebHook(WebHook webHook)
    {
      if (webHook == null)
        throw new ArgumentNullException(nameof (webHook));
      this.PrepareStoredProcedure("Release.prc_AddReleaseWebHook");
      this.BindString("artifactTypeId", webHook.ArtifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("normalizedArtifactDefinitionIdentifier", webHook.UniqueArtifactIdentifier, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("webHookId", webHook.WebHookId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WebHook>((ObjectBinder<WebHook>) this.GetReleaseWebHookBinder());
        return resultCollection.GetCurrent<WebHook>().Items.FirstOrDefault<WebHook>();
      }
    }

    public virtual WebHook GetReleaseWebHook(Guid webHookId, bool includeSubscriptions)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseWebHook");
      this.BindGuid(nameof (webHookId), webHookId);
      this.BindBoolean(nameof (includeSubscriptions), includeSubscriptions);
      return this.GetReleaseWebHook(includeSubscriptions);
    }

    public virtual WebHook GetReleaseWebHook(
      string normalizedArtifactDefinitionIdentifier,
      bool includeSubscriptions)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseWebHook");
      this.BindString(nameof (normalizedArtifactDefinitionIdentifier), normalizedArtifactDefinitionIdentifier, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean(nameof (includeSubscriptions), includeSubscriptions);
      return this.GetReleaseWebHook(includeSubscriptions);
    }

    public virtual void DeleteReleaseWebHook(Guid webHookId)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteReleaseWebHook");
      this.BindGuid(nameof (webHookId), webHookId);
      this.ExecuteNonQuery();
    }

    public void AddWebHookSubscription(Guid webHookId, string uniqueSourceIdentifier)
    {
      this.PrepareStoredProcedure("Release.prc_AddReleaseWebHookSubscription");
      this.BindGuid(nameof (webHookId), webHookId);
      this.BindString(nameof (uniqueSourceIdentifier), uniqueSourceIdentifier, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void DeleteWebHookSubscription(Guid webHookId, string uniqueSourceIdentifier)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteReleaseWebHookSubscription");
      this.BindGuid(nameof (webHookId), webHookId);
      this.BindString(nameof (uniqueSourceIdentifier), uniqueSourceIdentifier, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This need to be a virtual method for subtype to override.")]
    protected virtual ReleaseWebHookBinder GetReleaseWebHookBinder() => new ReleaseWebHookBinder((ReleaseManagementSqlResourceComponentBase) this);

    private WebHook GetReleaseWebHook(bool includeSubscriptions)
    {
      SqlColumnBinder uniqueSourceIdentifier = new SqlColumnBinder("UniqueSourceIdentifier");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WebHook>((ObjectBinder<WebHook>) this.GetReleaseWebHookBinder());
        WebHook releaseWebHook = resultCollection.GetCurrent<WebHook>().Items.FirstOrDefault<WebHook>();
        if (includeSubscriptions && releaseWebHook != null)
        {
          resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => uniqueSourceIdentifier.GetString(reader, false))));
          resultCollection.NextResult();
          foreach (string artifactUniqueSourceIdentifier in resultCollection.GetCurrent<string>().Items)
            releaseWebHook.Subscriptions.Add((IWebHookSubscription) new ReleaseWebHookSubscription(artifactUniqueSourceIdentifier));
        }
        return releaseWebHook;
      }
    }
  }
}
