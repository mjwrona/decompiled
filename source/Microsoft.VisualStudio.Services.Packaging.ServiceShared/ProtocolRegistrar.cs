// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ProtocolRegistrar
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class ProtocolRegistrar : IProtocolRegistrar
  {
    private readonly 
    #nullable disable
    ImmutableDictionary<string, IProtocol> protocolsByExactName;
    private static readonly Lazy<ProtocolRegistrar> InstanceLazy = new Lazy<ProtocolRegistrar>((Func<ProtocolRegistrar>) (() => new ProtocolRegistrar(ProtocolRegistrar.RegisterAllProtocols())));

    public static IProtocolRegistrar Instance => (IProtocolRegistrar) ProtocolRegistrar.InstanceLazy.Value;

    public IReadOnlyList<IProtocol> AllProtocols { get; }

    public IReadOnlyList<BookmarkTokenKey> BookmarkTokenKeys { get; }

    public IReadOnlyList<string> ItemStoreExperienceNames { get; }

    private IReadOnlyDictionary<IProtocol, IRequiredProtocolBootstrappers> Bootstrappers { get; }

    private IReadOnlyDictionary<string, Func<IVssRequestContext, IItemStore>> ItemStoreExperiences { get; }

    private ImmutableDictionary<IProtocol, IIdentityResolver> IdentityResolvers { get; }

    public IItemStore GetItemStoreExperienceOrDefault(
      IVssRequestContext requestContext,
      string name)
    {
      name = name.Trim();
      Func<IVssRequestContext, IItemStore> func = this.ItemStoreExperiences.First<KeyValuePair<string, Func<IVssRequestContext, IItemStore>>>((Func<KeyValuePair<string, Func<IVssRequestContext, IItemStore>>, bool>) (x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase))).Value;
      return func == null ? (IItemStore) null : func(requestContext);
    }

    public IProtocol GetProtocolOrDefault(string name)
    {
      IProtocol protocolOrDefault;
      if (this.protocolsByExactName.TryGetValue(name, out protocolOrDefault))
        return protocolOrDefault;
      name = name.Trim();
      return this.AllProtocols.FirstOrDefault<IProtocol>((Func<IProtocol, bool>) (x => x.CorrectlyCasedName.Equals(name, StringComparison.OrdinalIgnoreCase)));
    }

    public IRequiredProtocolBootstrappers GetBootstrappers(IProtocol protocol) => this.Bootstrappers[protocol];

    public IIdentityResolver GetIdentityResolver(IProtocol protocol)
    {
      IIdentityResolver identityResolver;
      this.IdentityResolvers.TryGetValue(protocol, out identityResolver);
      return identityResolver != null ? identityResolver : throw new NotImplementedException("IdentityResolver is not implemented for Protocol: '" + protocol.ToString() + "' in ProtocolRegistrar");
    }

    private static ProtocolRegistrar.Acceptor RegisterAllProtocols()
    {
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      ProtocolRegistrar.Acceptor acceptor = new ProtocolRegistrar.Acceptor((IEnumerable<Assembly>) assemblies);
      foreach (Assembly assembly in assemblies)
      {
        Type type = assembly.GetType(assembly.GetName().Name + ".PackagingRegistration");
        if (!(type == (Type) null))
        {
          acceptor.RegistrationTypes.Add(type);
          MethodInfo method = type.GetMethod("Register", BindingFlags.Static | BindingFlags.Public, (Binder) null, new Type[1]
          {
            typeof (IProtocolRegistrationAcceptor)
          }, (ParameterModifier[]) null);
          if (!(method == (MethodInfo) null))
          {
            acceptor.RegistrationMethods.Add(method);
            method.Invoke((object) null, new object[1]
            {
              (object) acceptor
            });
          }
        }
      }
      return acceptor;
    }

    private ProtocolRegistrar(ProtocolRegistrar.Acceptor acceptor)
    {
      this.Bootstrappers = (IReadOnlyDictionary<IProtocol, IRequiredProtocolBootstrappers>) acceptor.Protocols.ToImmutable();
      this.AllProtocols = (IReadOnlyList<IProtocol>) this.Bootstrappers.Keys.ToImmutableList<IProtocol>();
      this.protocolsByExactName = this.AllProtocols.ToImmutableDictionary<IProtocol, string>((Func<IProtocol, string>) (x => x.CorrectlyCasedName));
      this.ItemStoreExperiences = (IReadOnlyDictionary<string, Func<IVssRequestContext, IItemStore>>) acceptor.ItemStoreExperiences.ToImmutable();
      this.ItemStoreExperienceNames = (IReadOnlyList<string>) this.ItemStoreExperiences.Keys.ToImmutableList<string>();
      this.RegistrationTypes = acceptor.RegistrationTypes.ToImmutable();
      this.RegistrationMethods = acceptor.RegistrationMethods.ToImmutable();
      this.AssembliesAtLoadTime = acceptor.AssembliesAtLoadTime.ToImmutableList<Assembly>();
      this.BookmarkTokenKeys = (IReadOnlyList<BookmarkTokenKey>) acceptor.BookmarkTokenKeys.ToImmutable();
      this.IdentityResolvers = acceptor.IdentityResolvers.ToImmutable();
    }

    private ImmutableList<Assembly> AssembliesAtLoadTime { get; }

    private ImmutableList<MethodInfo> RegistrationMethods { get; }

    private ImmutableList<Type> RegistrationTypes { get; }

    private class Acceptor : IProtocolRegistrationAcceptor
    {
      public IEnumerable<Assembly> AssembliesAtLoadTime { get; }

      public ImmutableList<Type>.Builder RegistrationTypes { get; } = ImmutableList.CreateBuilder<Type>();

      public ImmutableList<MethodInfo>.Builder RegistrationMethods { get; } = ImmutableList.CreateBuilder<MethodInfo>();

      public Acceptor(IEnumerable<Assembly> assemblies) => this.AssembliesAtLoadTime = (IEnumerable<Assembly>) assemblies.ToImmutableList<Assembly>();

      public ImmutableDictionary<IProtocol, IRequiredProtocolBootstrappers>.Builder Protocols { get; } = ImmutableDictionary.CreateBuilder<IProtocol, IRequiredProtocolBootstrappers>();

      public ImmutableDictionary<string, Func<IVssRequestContext, IItemStore>>.Builder ItemStoreExperiences { get; } = ImmutableDictionary.CreateBuilder<string, Func<IVssRequestContext, IItemStore>>();

      public ImmutableList<BookmarkTokenKey>.Builder BookmarkTokenKeys { get; } = ImmutableList.CreateBuilder<BookmarkTokenKey>();

      public ImmutableDictionary<IProtocol, IIdentityResolver>.Builder IdentityResolvers { get; } = ImmutableDictionary.CreateBuilder<IProtocol, IIdentityResolver>();

      public void RegisterProtocol(
        IProtocol protocol,
        IRequiredProtocolBootstrappers bootstrappers,
        IIdentityResolver identityResolver)
      {
        this.Protocols.Add(protocol, bootstrappers);
        this.RegisterBookmarkTokenKey(protocol.ChangeProcessingBookmarkTokenKey);
        if (protocol.DeleteProcessingBookmarkTokenKey != null)
          this.RegisterBookmarkTokenKey(protocol.DeleteProcessingBookmarkTokenKey);
        this.IdentityResolvers.Add(protocol, identityResolver);
      }

      public void RegisterItemStoreExperience(
        string name,
        Func<IVssRequestContext, IItemStore> itemStoreFactory)
      {
        this.ItemStoreExperiences.Add(name, itemStoreFactory);
      }

      public void RegisterBookmarkTokenKey(BookmarkTokenKey key) => this.BookmarkTokenKeys.Add(key);
    }
  }
}
