// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CounterComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CounterComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<CounterComponent>(1),
      (IComponentCreator) new ComponentCreator<CounterComponent2>(2),
      (IComponentCreator) new ComponentCreator<CounterComponent3>(3),
      (IComponentCreator) new ComponentCreator<CounterComponent4>(4),
      (IComponentCreator) new ComponentCreator<CounterComponent5>(5),
      (IComponentCreator) new ComponentCreator<CounterComponent5>(6)
    }, "Counter");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static CounterComponent()
    {
      CounterComponent.s_sqlExceptionFactories.Add(800095, new SqlExceptionFactory(typeof (CounterNotPopulatedException)));
      CounterComponent.s_sqlExceptionFactories.Add(800107, new SqlExceptionFactory(typeof (NoDataspaceProvidedWhenIsolated)));
      CounterComponent.s_sqlExceptionFactories.Add(800108, new SqlExceptionFactory(typeof (CounterNotPopulatedException)));
    }

    public CounterComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual long? ReserveCounterIds(
      string counterName,
      long countToReserve,
      Guid dataspaceIdentifier,
      bool isolated)
    {
      this.PrepareStoredProcedure("prc_CounterGetNext");
      this.BindString("@counterName", counterName, 128, false, SqlDbType.NVarChar);
      this.BindInt("@countToReserve", checked ((int) countToReserve));
      this.BindDataspace(dataspaceIdentifier);
      this.BindIsolated(isolated);
      object obj = this.ExecuteScalar();
      return obj is DBNull ? new long?() : new long?((long) obj);
    }

    public virtual bool IsIsolated() => throw new NotImplementedException();

    protected virtual void BindDataspace(Guid dataspaceIdentifier)
    {
    }

    protected virtual void BindIsolated(bool isolated)
    {
    }

    public virtual void SetIsolated(bool isolated)
    {
    }

    public virtual void PopulateCounter(
      string counterName,
      long nextCounterValue,
      Guid dataspaceIdentifier,
      bool isolated)
    {
      throw new NotImplementedException();
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) CounterComponent.s_sqlExceptionFactories;
  }
}
