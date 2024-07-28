// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.RoutingService
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class RoutingService : IRoutingService, IVssFrameworkService
  {
    private readonly ConcurrentDictionary<string, IRoutingDataProvider> m_entityTypeToRoutingProvider;

    internal RoutingService() => this.m_entityTypeToRoutingProvider = new ConcurrentDictionary<string, IRoutingDataProvider>();

    public string GetRouting(IndexingExecutionContext indexingExecutionContext, string item) => throw new NotImplementedException();

    public List<ShardAssignmentDetails> AssignIndex(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSizeEstimates)
    {
      return this.GetRoutingDataProvider(indexingExecutionContext).AssignIndex(indexingExecutionContext, indexingUnitsWithSizeEstimates);
    }

    public List<ShardAssignmentDetails> AssignShards(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      return this.GetRoutingDataProvider(indexingExecutionContext).AssignShards(indexingExecutionContext, indexingUnitsWithSize);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal virtual IRoutingDataProvider GetRoutingDataProvider(
      IndexingExecutionContext indexingExecutionContext)
    {
      IEntityType entityType = indexingExecutionContext.IndexingUnit.EntityType;
      IRoutingDataProvider instance;
      if (!this.m_entityTypeToRoutingProvider.TryGetValue(entityType.Name, out instance))
      {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          try
          {
            foreach (TypeInfo definedType in assembly.DefinedTypes)
            {
              if (typeof (IRoutingDataProvider).IsAssignableFrom((Type) definedType) && !definedType.IsAbstract && !definedType.IsInterface)
              {
                PropertyInfo property = definedType.GetProperty("EntityType", BindingFlags.Static | BindingFlags.Public);
                if (property != (PropertyInfo) null && ((IEntityType) property.GetValue((object) null)).Equals((object) entityType))
                {
                  BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
                  instance = (IRoutingDataProvider) Activator.CreateInstance((Type) definedType, bindingAttr, (Binder) null, new object[1]
                  {
                    (object) string.Empty
                  }, (CultureInfo) null);
                  this.m_entityTypeToRoutingProvider.TryAdd(entityType.Name, instance);
                  return instance;
                }
              }
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
      return instance != null ? instance : throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find an implementation of IRoutingDataProvider for EntityType {0}.", (object) entityType)));
    }
  }
}
