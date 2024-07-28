// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraphClientRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public abstract class MicrosoftGraphClientRequest<T> where T : MicrosoftGraphClientResponse
  {
    public JwtSecurityToken AccessToken { get; set; }

    public abstract override string ToString();

    internal virtual void Validate()
    {
    }

    internal abstract T Execute(IVssRequestContext context, GraphServiceClient graphServiceClient);

    protected IDictionary<K, V> CreateEmptyMapWithIds<K, V>(IEnumerable<K> ids) => (IDictionary<K, V>) ids.Distinct<K>().ToDictionary<K, K, V>((Func<K, K>) (oid => oid), (Func<K, V>) (oid => default (V)));

    public virtual IEnumerable<TEntity> ReadAllPages<TEntity, TPage>(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient,
      TPage firstPage)
      where TEntity : Entity
      where TPage : ICollectionPage<TEntity>
    {
      List<TEntity> entities = new List<TEntity>();
      PageIterator<TEntity> pageIterator = PageIterator<TEntity>.CreatePageIterator((IBaseClient) graphServiceClient, (ICollectionPage<TEntity>) firstPage, (Func<TEntity, bool>) (entity =>
      {
        entities.Add(entity);
        return true;
      }), (Func<IBaseRequest, IBaseRequest>) (req => req));
      context.RunSynchronously(new Func<Task>(pageIterator.IterateAsync));
      return (IEnumerable<TEntity>) entities;
    }
  }
}
