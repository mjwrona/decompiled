// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ETag`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Linq;

namespace Microsoft.AspNet.OData.Formatter
{
  public class ETag<TEntity> : ETag
  {
    public ETag() => this.EntityType = typeof (TEntity);

    public override IQueryable ApplyTo(IQueryable query)
    {
      ETag<TEntity>.ValidateQuery(query);
      return base.ApplyTo(query);
    }

    public IQueryable<TEntity> ApplyTo(IQueryable<TEntity> query) => query != null ? (IQueryable<TEntity>) base.ApplyTo((IQueryable) query) : throw Error.ArgumentNull(nameof (query));

    private static void ValidateQuery(IQueryable query)
    {
      if (query == null)
        throw Error.ArgumentNull(nameof (query));
      if (!TypeHelper.IsTypeAssignableFrom(typeof (TEntity), query.ElementType))
        throw Error.Argument(nameof (query), SRResources.CannotApplyETagOfT, (object) typeof (ETag).Name, (object) typeof (TEntity).FullName, (object) typeof (IQueryable).Name, (object) query.ElementType.FullName);
    }
  }
}
