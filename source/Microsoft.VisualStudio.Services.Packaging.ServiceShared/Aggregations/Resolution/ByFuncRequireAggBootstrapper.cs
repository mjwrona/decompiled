// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.ByFuncRequireAggBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public static class ByFuncRequireAggBootstrapper
  {
    public static IRequireAggBootstrapper<TBootstrapped> For<TBootstrapped, TAgg1>(
      Func<TAgg1, TBootstrapped> bootstrapperFunc)
      where TBootstrapped : class
      where TAgg1 : class
    {
      return (IRequireAggBootstrapper<TBootstrapped>) new ByFuncRequireAggBootstrapper.Impl<TBootstrapped, TAgg1>(bootstrapperFunc);
    }

    public static IRequireAggBootstrapper<TBootstrapped> For<TBootstrapped, TAgg1, TAgg2>(
      Func<TAgg1, TAgg2, TBootstrapped> bootstrapperFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
    {
      return (IRequireAggBootstrapper<TBootstrapped>) new ByFuncRequireAggBootstrapper.Impl<TBootstrapped, TAgg1, TAgg2>(bootstrapperFunc);
    }

    public static IRequireAggBootstrapper<TBootstrapped> For<TBootstrapped, TAgg1, TAgg2, TAgg3>(
      Func<TAgg1, TAgg2, TAgg3, TBootstrapped> bootstrapperFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      return (IRequireAggBootstrapper<TBootstrapped>) new ByFuncRequireAggBootstrapper.Impl<TBootstrapped, TAgg1, TAgg2, TAgg3>(bootstrapperFunc);
    }

    private class Impl<TBootstrapped, TAgg1> : RequireAggBootstrapper<TBootstrapped, TAgg1>
      where TBootstrapped : class
      where TAgg1 : class
    {
      private readonly Func<TAgg1, TBootstrapped> bootstrapperFunc;

      public Impl(Func<TAgg1, TBootstrapped> bootstrapperFunc) => this.bootstrapperFunc = bootstrapperFunc;

      protected override TBootstrapped Bootstrap(TAgg1 agg1) => this.bootstrapperFunc(agg1);
    }

    private class Impl<TBootstrapped, TAgg1, TAgg2> : 
      RequireAggBootstrapper<TBootstrapped, TAgg1, TAgg2>
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
    {
      private readonly Func<TAgg1, TAgg2, TBootstrapped> bootstrapperFunc;

      public Impl(Func<TAgg1, TAgg2, TBootstrapped> bootstrapperFunc) => this.bootstrapperFunc = bootstrapperFunc;

      protected override TBootstrapped Bootstrap(TAgg1 agg1, TAgg2 agg2) => this.bootstrapperFunc(agg1, agg2);
    }

    private class Impl<TBootstrapped, TAgg1, TAgg2, TAgg3> : 
      RequireAggBootstrapper<TBootstrapped, TAgg1, TAgg2, TAgg3>
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      private readonly Func<TAgg1, TAgg2, TAgg3, TBootstrapped> bootstrapperFunc;

      public Impl(
        Func<TAgg1, TAgg2, TAgg3, TBootstrapped> bootstrapperFunc)
      {
        this.bootstrapperFunc = bootstrapperFunc;
      }

      protected override TBootstrapped Bootstrap(TAgg1 agg1, TAgg2 agg2, TAgg3 agg3) => this.bootstrapperFunc(agg1, agg2, agg3);
    }
  }
}
