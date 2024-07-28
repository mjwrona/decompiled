// Decompiled with JetBrains decompiler
// Type: Nest.BulkAliasDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class BulkAliasDescriptor : 
    RequestDescriptorBase<BulkAliasDescriptor, BulkAliasRequestParameters, IBulkAliasRequest>,
    IBulkAliasRequest,
    IRequest<BulkAliasRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesBulkAlias;

    public BulkAliasDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public BulkAliasDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    IList<IAliasAction> IBulkAliasRequest.Actions { get; set; } = (IList<IAliasAction>) new List<IAliasAction>();

    public BulkAliasDescriptor Add(IAliasAction action) => Fluent.Assign<BulkAliasDescriptor, IBulkAliasRequest, IAliasAction>(this, action, (Action<IBulkAliasRequest, IAliasAction>) ((a, v) => a.Actions.AddIfNotNull<IAliasAction>(v)));

    public BulkAliasDescriptor Add(
      Func<AliasAddDescriptor, IAliasAddAction> addSelector)
    {
      return this.Add(addSelector != null ? (IAliasAction) addSelector(new AliasAddDescriptor()) : (IAliasAction) null);
    }

    public BulkAliasDescriptor Remove(
      Func<AliasRemoveDescriptor, IAliasRemoveAction> removeSelector)
    {
      return this.Add(removeSelector != null ? (IAliasAction) removeSelector(new AliasRemoveDescriptor()) : (IAliasAction) null);
    }

    public BulkAliasDescriptor RemoveIndex(
      Func<AliasRemoveIndexDescriptor, IAliasRemoveIndexAction> removeIndexSelector)
    {
      return this.Add(removeIndexSelector != null ? (IAliasAction) removeIndexSelector(new AliasRemoveIndexDescriptor()) : (IAliasAction) null);
    }
  }
}
