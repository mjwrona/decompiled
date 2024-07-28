// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ContributionTypeComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  public class ContributionTypeComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "ContributionTypeComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ContributionTypeComponent>(1)
    }, "ContributionType");

    static ContributionTypeComponent() => ContributionTypeComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ContributionTypeComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (ContributionTypeComponent);

    public virtual void UpdateContributionType(
      Guid extensionId,
      string contributionType,
      string contributionId)
    {
      try
      {
        this.TraceEnter(12061082, "Enter UpdateContributionType");
        ArgumentUtility.CheckStringForNullOrEmpty(contributionType, "resourceName");
        ArgumentUtility.CheckStringForNullOrEmpty(contributionId, "resourceId");
        this.PrepareStoredProcedure("Gallery.prc_AddContributionType");
        this.BindGuid(nameof (extensionId), extensionId);
        this.BindString(nameof (contributionType), contributionType, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        this.BindString(nameof (contributionId), contributionId, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        this.Trace(12061084, TraceLevel.Info, string.Format("Updated ContributionTypes where extensionId={0}, contributionType={1}, contributionId={2}", (object) extensionId, (object) contributionType, (object) contributionId));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(12061084, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061083, "Leave UpdateContributionType");
      }
    }

    public virtual void RemoveContributionTypes()
    {
      try
      {
        this.TraceEnter(12061082, "Enter RemoveContributionTypes");
        this.PrepareStoredProcedure("Gallery.prc_DeleteContributionType");
        this.Trace(12061084, TraceLevel.Info, string.Format("Removed all ContributionTypes"));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(12061084, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061083, "Leave RemoveContributionTypes");
      }
    }
  }
}
