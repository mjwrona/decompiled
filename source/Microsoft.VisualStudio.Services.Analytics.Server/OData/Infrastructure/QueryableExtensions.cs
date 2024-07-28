// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.QueryableExtensions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class QueryableExtensions
  {
    private static Lazy<HashSet<Type>> _disableProjectAttributes = new Lazy<HashSet<Type>>(new Func<HashSet<Type>>(QueryableExtensions.PrepareDisableProjectAttributes));

    private static HashSet<Type> PrepareDisableProjectAttributes() => new HashSet<Type>(AnalyticsContext.GetEntityTypes().Where<Type>((Func<Type, bool>) (t => t.GetCustomAttribute<DisableProjectFilteringAttribute>(true) != null)));

    internal static IQueryable ApplyProjectScopeFilterInternal(
      IQueryable queryable,
      ProjectInfo projectInfo)
    {
      return (IQueryable) typeof (QueryableExtensions).GetMethod("ApplyProjectScopeFilter", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(queryable.ElementType).Invoke((object) null, new object[2]
      {
        (object) queryable,
        (object) projectInfo
      });
    }

    internal static IQueryable<T> ApplyProjectScopeFilter<T>(
      this IQueryable query,
      ProjectInfo project)
      where T : class
    {
      if (project != null)
      {
        if (typeof (Project).IsAssignableFrom(typeof (T)))
          query = (IQueryable) Queryable.Cast<T>(((IQueryable<Project>) query).Where<Project>((Expression<Func<Project, bool>>) (i => i.ProjectSK == project.Id)));
        else if (typeof (IProjectScoped).IsAssignableFrom(typeof (T)))
        {
          if (QueryableExtensions._disableProjectAttributes.Value.Contains(typeof (T)))
            query = (IQueryable) Queryable.Cast<T>(((IQueryable<IProjectScoped>) query).Where<IProjectScoped>((Expression<Func<IProjectScoped, bool>>) (i => i.ProjectSK == (Guid?) project.Id)));
          else
            query = (IQueryable) ((IQueryable<T>) query).Where<T>((Expression<Func<T, bool>>) (wit => true));
        }
      }
      return (IQueryable<T>) query;
    }
  }
}
