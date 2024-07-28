// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.LazyComponentRegistrationListExtensions
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization
{
  internal static class LazyComponentRegistrationListExtensions
  {
    public static TComponent BuildComponentChain<TComponent>(
      this LazyComponentRegistrationList<TComponent, TComponent> registrations,
      TComponent innerComponent)
    {
      return registrations.InReverseOrder.Aggregate<Func<TComponent, TComponent>, TComponent>(innerComponent, (Func<TComponent, Func<TComponent, TComponent>, TComponent>) ((inner, factory) => factory(inner)));
    }

    public static TComponent BuildComponentChain<TArgument, TComponent>(
      this LazyComponentRegistrationList<TArgument, TComponent> registrations,
      TComponent innerComponent,
      Func<TComponent, TArgument> argumentBuilder)
    {
      return registrations.InReverseOrder.Aggregate<Func<TArgument, TComponent>, TComponent>(innerComponent, (Func<TComponent, Func<TArgument, TComponent>, TComponent>) ((inner, factory) => factory(argumentBuilder(inner))));
    }

    public static List<TComponent> BuildComponentList<TComponent>(
      this LazyComponentRegistrationList<Nothing, TComponent> registrations)
    {
      return registrations.Select<Func<Nothing, TComponent>, TComponent>((Func<Func<Nothing, TComponent>, TComponent>) (factory => factory((Nothing) null))).ToList<TComponent>();
    }

    public static List<TComponent> BuildComponentList<TArgument, TComponent>(
      this LazyComponentRegistrationList<TArgument, TComponent> registrations,
      TArgument argument)
    {
      return registrations.Select<Func<TArgument, TComponent>, TComponent>((Func<Func<TArgument, TComponent>, TComponent>) (factory => factory(argument))).ToList<TComponent>();
    }
  }
}
