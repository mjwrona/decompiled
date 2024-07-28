// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.LazyComponentRegistrationList`2
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization
{
  internal sealed class LazyComponentRegistrationList<TArgument, TComponent> : 
    IEnumerable<Func<TArgument, TComponent>>,
    IEnumerable
  {
    private readonly List<LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration> entries = new List<LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration>();

    public LazyComponentRegistrationList<TArgument, TComponent> Clone()
    {
      LazyComponentRegistrationList<TArgument, TComponent> registrationList = new LazyComponentRegistrationList<TArgument, TComponent>();
      foreach (LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration entry in this.entries)
        registrationList.entries.Add(entry);
      return registrationList;
    }

    public void Add(Type componentType, Func<TArgument, TComponent> factory) => this.entries.Add(new LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration(componentType, factory));

    public void Remove(Type componentType)
    {
      for (int index = 0; index < this.entries.Count; ++index)
      {
        if ((object) this.entries[index].ComponentType == (object) componentType)
        {
          this.entries.RemoveAt(index);
          return;
        }
      }
      throw new KeyNotFoundException(string.Format("A component registration of type '{0}' was not found.", (object) componentType.FullName));
    }

    public int Count => this.entries.Count;

    public IEnumerable<Func<TArgument, TComponent>> InReverseOrder
    {
      get
      {
        for (int i = this.entries.Count - 1; i >= 0; --i)
          yield return this.entries[i].Factory;
      }
    }

    public IRegistrationLocationSelectionSyntax<TComponent> CreateRegistrationLocationSelector(
      Type componentType,
      Func<TArgument, TComponent> factory)
    {
      return (IRegistrationLocationSelectionSyntax<TComponent>) new LazyComponentRegistrationList<TArgument, TComponent>.RegistrationLocationSelector(this, new LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration(componentType, factory));
    }

    public ITrackingRegistrationLocationSelectionSyntax<TComponent> CreateTrackingRegistrationLocationSelector(
      Type componentType,
      Func<TComponent, TArgument, TComponent> factory)
    {
      return (ITrackingRegistrationLocationSelectionSyntax<TComponent>) new LazyComponentRegistrationList<TArgument, TComponent>.TrackingRegistrationLocationSelector(this, new LazyComponentRegistrationList<TArgument, TComponent>.TrackingLazyComponentRegistration(componentType, factory));
    }

    public IEnumerator<Func<TArgument, TComponent>> GetEnumerator() => this.entries.Select<LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration, Func<TArgument, TComponent>>((Func<LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration, Func<TArgument, TComponent>>) (e => e.Factory)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    private int IndexOfRegistration(Type registrationType)
    {
      for (int index = 0; index < this.entries.Count; ++index)
      {
        if ((object) registrationType == (object) this.entries[index].ComponentType)
          return index;
      }
      return -1;
    }

    private void EnsureNoDuplicateRegistrationType(Type componentType)
    {
      if (this.IndexOfRegistration(componentType) != -1)
        throw new InvalidOperationException(string.Format("A component of type '{0}' has already been registered.", (object) componentType.FullName));
    }

    private int EnsureRegistrationExists<TRegistrationType>()
    {
      int num = this.IndexOfRegistration(typeof (TRegistrationType));
      return num != -1 ? num : throw new InvalidOperationException(string.Format("A component of type '{0}' has not been registered.", (object) typeof (TRegistrationType).FullName));
    }

    public sealed class LazyComponentRegistration
    {
      public readonly Type ComponentType;
      public readonly Func<TArgument, TComponent> Factory;

      public LazyComponentRegistration(Type componentType, Func<TArgument, TComponent> factory)
      {
        this.ComponentType = componentType;
        this.Factory = factory;
      }
    }

    public sealed class TrackingLazyComponentRegistration
    {
      public readonly Type ComponentType;
      public readonly Func<TComponent, TArgument, TComponent> Factory;

      public TrackingLazyComponentRegistration(
        Type componentType,
        Func<TComponent, TArgument, TComponent> factory)
      {
        this.ComponentType = componentType;
        this.Factory = factory;
      }
    }

    private class RegistrationLocationSelector : IRegistrationLocationSelectionSyntax<TComponent>
    {
      private readonly LazyComponentRegistrationList<TArgument, TComponent> registrations;
      private readonly LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration newRegistration;

      public RegistrationLocationSelector(
        LazyComponentRegistrationList<TArgument, TComponent> registrations,
        LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration newRegistration)
      {
        this.registrations = registrations;
        this.newRegistration = newRegistration;
      }

      void IRegistrationLocationSelectionSyntax<TComponent>.InsteadOf<TRegistrationType>()
      {
        if ((object) this.newRegistration.ComponentType != (object) typeof (TRegistrationType))
          this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
        this.registrations.entries[this.registrations.EnsureRegistrationExists<TRegistrationType>()] = this.newRegistration;
      }

      void IRegistrationLocationSelectionSyntax<TComponent>.After<TRegistrationType>()
      {
        this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
        this.registrations.entries.Insert(this.registrations.EnsureRegistrationExists<TRegistrationType>() + 1, this.newRegistration);
      }

      void IRegistrationLocationSelectionSyntax<TComponent>.Before<TRegistrationType>()
      {
        this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
        this.registrations.entries.Insert(this.registrations.EnsureRegistrationExists<TRegistrationType>(), this.newRegistration);
      }

      void IRegistrationLocationSelectionSyntax<TComponent>.OnBottom()
      {
        this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
        this.registrations.entries.Add(this.newRegistration);
      }

      void IRegistrationLocationSelectionSyntax<TComponent>.OnTop()
      {
        this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
        this.registrations.entries.Insert(0, this.newRegistration);
      }
    }

    private class TrackingRegistrationLocationSelector : 
      ITrackingRegistrationLocationSelectionSyntax<TComponent>
    {
      private readonly LazyComponentRegistrationList<TArgument, TComponent> registrations;
      private readonly LazyComponentRegistrationList<TArgument, TComponent>.TrackingLazyComponentRegistration newRegistration;

      public TrackingRegistrationLocationSelector(
        LazyComponentRegistrationList<TArgument, TComponent> registrations,
        LazyComponentRegistrationList<TArgument, TComponent>.TrackingLazyComponentRegistration newRegistration)
      {
        this.registrations = registrations;
        this.newRegistration = newRegistration;
      }

      void ITrackingRegistrationLocationSelectionSyntax<TComponent>.InsteadOf<TRegistrationType>()
      {
        if ((object) this.newRegistration.ComponentType != (object) typeof (TRegistrationType))
          this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
        int index = this.registrations.EnsureRegistrationExists<TRegistrationType>();
        Func<TArgument, TComponent> innerComponentFactory = this.registrations.entries[index].Factory;
        this.registrations.entries[index] = new LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration(this.newRegistration.ComponentType, (Func<TArgument, TComponent>) (arg => this.newRegistration.Factory(innerComponentFactory(arg), arg)));
      }
    }
  }
}
