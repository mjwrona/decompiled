// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.SerializerBuilder
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;

namespace YamlDotNet.Serialization
{
  public sealed class SerializerBuilder : BuilderSkeleton<SerializerBuilder>
  {
    private ObjectGraphTraversalStrategyFactory objectGraphTraversalStrategyFactory;
    private readonly LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitorFactories;
    private readonly LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>> emissionPhaseObjectGraphVisitorFactories;
    private readonly LazyComponentRegistrationList<IEventEmitter, IEventEmitter> eventEmitterFactories;
    private readonly IDictionary<Type, string> tagMappings = (IDictionary<Type, string>) new Dictionary<Type, string>();
    private int maximumRecursion = 50;

    public SerializerBuilder()
    {
      this.typeInspectorFactories.Add(typeof (CachedTypeInspector), (Func<ITypeInspector, ITypeInspector>) (inner => (ITypeInspector) new CachedTypeInspector(inner)));
      this.typeInspectorFactories.Add(typeof (NamingConventionTypeInspector), (Func<ITypeInspector, ITypeInspector>) (inner => this.namingConvention == null ? inner : (ITypeInspector) new NamingConventionTypeInspector(inner, this.namingConvention)));
      this.typeInspectorFactories.Add(typeof (YamlAttributesTypeInspector), (Func<ITypeInspector, ITypeInspector>) (inner => (ITypeInspector) new YamlAttributesTypeInspector(inner)));
      this.typeInspectorFactories.Add(typeof (YamlAttributeOverridesInspector), (Func<ITypeInspector, ITypeInspector>) (inner => this.overrides == null ? inner : (ITypeInspector) new YamlAttributeOverridesInspector(inner, this.overrides.Clone())));
      this.preProcessingPhaseObjectGraphVisitorFactories = new LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>();
      this.preProcessingPhaseObjectGraphVisitorFactories.Add(typeof (AnchorAssigner), (Func<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>) (typeConverters => (IObjectGraphVisitor<Nothing>) new AnchorAssigner(typeConverters)));
      this.emissionPhaseObjectGraphVisitorFactories = new LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>();
      this.emissionPhaseObjectGraphVisitorFactories.Add(typeof (CustomSerializationObjectGraphVisitor), (Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>) (args => (IObjectGraphVisitor<IEmitter>) new CustomSerializationObjectGraphVisitor(args.InnerVisitor, args.TypeConverters, args.NestedObjectSerializer)));
      this.emissionPhaseObjectGraphVisitorFactories.Add(typeof (AnchorAssigningObjectGraphVisitor), (Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>) (args => (IObjectGraphVisitor<IEmitter>) new AnchorAssigningObjectGraphVisitor(args.InnerVisitor, args.EventEmitter, (IAliasProvider) args.GetPreProcessingPhaseObjectGraphVisitor<AnchorAssigner>())));
      this.emissionPhaseObjectGraphVisitorFactories.Add(typeof (DefaultExclusiveObjectGraphVisitor), (Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>) (args => (IObjectGraphVisitor<IEmitter>) new DefaultExclusiveObjectGraphVisitor(args.InnerVisitor)));
      this.eventEmitterFactories = new LazyComponentRegistrationList<IEventEmitter, IEventEmitter>();
      this.eventEmitterFactories.Add(typeof (TypeAssigningEventEmitter), (Func<IEventEmitter, IEventEmitter>) (inner => (IEventEmitter) new TypeAssigningEventEmitter(inner, false, this.tagMappings)));
      this.objectGraphTraversalStrategyFactory = (ObjectGraphTraversalStrategyFactory) ((typeInspector, typeResolver, typeConverters, maximumRecursion) => (IObjectGraphTraversalStrategy) new FullObjectGraphTraversalStrategy(typeInspector, typeResolver, maximumRecursion, this.namingConvention ?? (INamingConvention) new NullNamingConvention()));
      this.WithTypeResolver((ITypeResolver) new DynamicTypeResolver());
    }

    protected override SerializerBuilder Self => this;

    public SerializerBuilder WithMaximumRecursion(int maximumRecursion)
    {
      this.maximumRecursion = maximumRecursion > 0 ? maximumRecursion : throw new ArgumentOutOfRangeException(nameof (maximumRecursion), string.Format("The maximum recursion specified ({0}) is invalid. It should be a positive integer.", (object) maximumRecursion));
      return this;
    }

    public SerializerBuilder WithEventEmitter<TEventEmitter>(
      Func<IEventEmitter, TEventEmitter> eventEmitterFactory)
      where TEventEmitter : IEventEmitter
    {
      return this.WithEventEmitter<TEventEmitter>(eventEmitterFactory, (Action<IRegistrationLocationSelectionSyntax<IEventEmitter>>) (w => w.OnTop()));
    }

    public SerializerBuilder WithEventEmitter<TEventEmitter>(
      Func<IEventEmitter, TEventEmitter> eventEmitterFactory,
      Action<IRegistrationLocationSelectionSyntax<IEventEmitter>> where)
      where TEventEmitter : IEventEmitter
    {
      if (eventEmitterFactory == null)
        throw new ArgumentNullException(nameof (eventEmitterFactory));
      if (where == null)
        throw new ArgumentNullException(nameof (where));
      where(this.eventEmitterFactories.CreateRegistrationLocationSelector(typeof (TEventEmitter), (Func<IEventEmitter, IEventEmitter>) (inner => (IEventEmitter) eventEmitterFactory(inner))));
      return this.Self;
    }

    public SerializerBuilder WithEventEmitter<TEventEmitter>(
      WrapperFactory<IEventEmitter, IEventEmitter, TEventEmitter> eventEmitterFactory,
      Action<ITrackingRegistrationLocationSelectionSyntax<IEventEmitter>> where)
      where TEventEmitter : IEventEmitter
    {
      if (eventEmitterFactory == null)
        throw new ArgumentNullException(nameof (eventEmitterFactory));
      if (where == null)
        throw new ArgumentNullException(nameof (where));
      where(this.eventEmitterFactories.CreateTrackingRegistrationLocationSelector(typeof (TEventEmitter), (Func<IEventEmitter, IEventEmitter, IEventEmitter>) ((wrapped, inner) => (IEventEmitter) eventEmitterFactory(wrapped, inner))));
      return this.Self;
    }

    public SerializerBuilder WithoutEventEmitter<TEventEmitter>() where TEventEmitter : IEventEmitter => this.WithoutEventEmitter(typeof (TEventEmitter));

    public SerializerBuilder WithoutEventEmitter(Type eventEmitterType)
    {
      if ((object) eventEmitterType == null)
        throw new ArgumentNullException(nameof (eventEmitterType));
      this.eventEmitterFactories.Remove(eventEmitterType);
      return this;
    }

    public override SerializerBuilder WithTagMapping(string tag, Type type)
    {
      if (tag == null)
        throw new ArgumentNullException(nameof (tag));
      if ((object) type == null)
        throw new ArgumentNullException(nameof (type));
      string str;
      if (this.tagMappings.TryGetValue(type, out str))
        throw new ArgumentException(string.Format("Type already has a registered tag '{0}' for type '{1}'", (object) str, (object) type.FullName), nameof (type));
      this.tagMappings.Add(type, tag);
      return this;
    }

    public SerializerBuilder WithoutTagMapping(Type type)
    {
      if ((object) type == null)
        throw new ArgumentNullException(nameof (type));
      if (!this.tagMappings.Remove(type))
        throw new KeyNotFoundException(string.Format("Tag for type '{0}' is not registered", (object) type.FullName));
      return this;
    }

    public SerializerBuilder EnsureRoundtrip()
    {
      this.objectGraphTraversalStrategyFactory = (ObjectGraphTraversalStrategyFactory) ((typeInspector, typeResolver, typeConverters, maximumRecursion) => (IObjectGraphTraversalStrategy) new RoundtripObjectGraphTraversalStrategy(typeConverters, typeInspector, typeResolver, maximumRecursion));
      this.WithEventEmitter<TypeAssigningEventEmitter>((Func<IEventEmitter, TypeAssigningEventEmitter>) (inner => new TypeAssigningEventEmitter(inner, true, this.tagMappings)), (Action<IRegistrationLocationSelectionSyntax<IEventEmitter>>) (loc => loc.InsteadOf<TypeAssigningEventEmitter>()));
      return this.WithTypeInspector<ReadableAndWritablePropertiesTypeInspector>((Func<ITypeInspector, ReadableAndWritablePropertiesTypeInspector>) (inner => new ReadableAndWritablePropertiesTypeInspector(inner)), (Action<IRegistrationLocationSelectionSyntax<ITypeInspector>>) (loc => loc.OnBottom()));
    }

    public SerializerBuilder DisableAliases()
    {
      this.preProcessingPhaseObjectGraphVisitorFactories.Remove(typeof (AnchorAssigner));
      this.emissionPhaseObjectGraphVisitorFactories.Remove(typeof (AnchorAssigningObjectGraphVisitor));
      return this;
    }

    public SerializerBuilder EmitDefaults()
    {
      this.emissionPhaseObjectGraphVisitorFactories.Remove(typeof (DefaultExclusiveObjectGraphVisitor));
      return this;
    }

    public SerializerBuilder JsonCompatible() => this.WithTypeConverter((IYamlTypeConverter) new GuidConverter(true), (Action<IRegistrationLocationSelectionSyntax<IYamlTypeConverter>>) (w => w.InsteadOf<GuidConverter>())).WithEventEmitter<JsonEventEmitter>((Func<IEventEmitter, JsonEventEmitter>) (inner => new JsonEventEmitter(inner)), (Action<IRegistrationLocationSelectionSyntax<IEventEmitter>>) (loc => loc.InsteadOf<TypeAssigningEventEmitter>()));

    public SerializerBuilder WithPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>(
      TObjectGraphVisitor objectGraphVisitor)
      where TObjectGraphVisitor : IObjectGraphVisitor<Nothing>
    {
      return this.WithPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>(objectGraphVisitor, (Action<IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<Nothing>>>) (w => w.OnTop()));
    }

    public SerializerBuilder WithPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>(
      TObjectGraphVisitor objectGraphVisitor,
      Action<IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<Nothing>>> where)
      where TObjectGraphVisitor : IObjectGraphVisitor<Nothing>
    {
      if ((object) objectGraphVisitor == null)
        throw new ArgumentNullException(nameof (objectGraphVisitor));
      if (where == null)
        throw new ArgumentNullException(nameof (where));
      where(this.preProcessingPhaseObjectGraphVisitorFactories.CreateRegistrationLocationSelector(typeof (TObjectGraphVisitor), (Func<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>) (_ => (IObjectGraphVisitor<Nothing>) objectGraphVisitor)));
      return this;
    }

    public SerializerBuilder WithPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>(
      WrapperFactory<IObjectGraphVisitor<Nothing>, TObjectGraphVisitor> objectGraphVisitorFactory,
      Action<ITrackingRegistrationLocationSelectionSyntax<IObjectGraphVisitor<Nothing>>> where)
      where TObjectGraphVisitor : IObjectGraphVisitor<Nothing>
    {
      if (objectGraphVisitorFactory == null)
        throw new ArgumentNullException(nameof (objectGraphVisitorFactory));
      if (where == null)
        throw new ArgumentNullException(nameof (where));
      where(this.preProcessingPhaseObjectGraphVisitorFactories.CreateTrackingRegistrationLocationSelector(typeof (TObjectGraphVisitor), (Func<IObjectGraphVisitor<Nothing>, IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>) ((wrapped, _) => (IObjectGraphVisitor<Nothing>) objectGraphVisitorFactory(wrapped))));
      return this;
    }

    public SerializerBuilder WithoutPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>() where TObjectGraphVisitor : IObjectGraphVisitor<Nothing> => this.WithoutPreProcessingPhaseObjectGraphVisitor(typeof (TObjectGraphVisitor));

    public SerializerBuilder WithoutPreProcessingPhaseObjectGraphVisitor(Type objectGraphVisitorType)
    {
      if ((object) objectGraphVisitorType == null)
        throw new ArgumentNullException(nameof (objectGraphVisitorType));
      this.preProcessingPhaseObjectGraphVisitorFactories.Remove(objectGraphVisitorType);
      return this;
    }

    public SerializerBuilder WithObjectGraphTraversalStrategyFactory(
      ObjectGraphTraversalStrategyFactory objectGraphTraversalStrategyFactory)
    {
      this.objectGraphTraversalStrategyFactory = objectGraphTraversalStrategyFactory;
      return this;
    }

    public SerializerBuilder WithEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>(
      Func<EmissionPhaseObjectGraphVisitorArgs, TObjectGraphVisitor> objectGraphVisitorFactory)
      where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
    {
      return this.WithEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>(objectGraphVisitorFactory, (Action<IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<IEmitter>>>) (w => w.OnTop()));
    }

    public SerializerBuilder WithEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>(
      Func<EmissionPhaseObjectGraphVisitorArgs, TObjectGraphVisitor> objectGraphVisitorFactory,
      Action<IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<IEmitter>>> where)
      where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
    {
      if (objectGraphVisitorFactory == null)
        throw new ArgumentNullException(nameof (objectGraphVisitorFactory));
      if (where == null)
        throw new ArgumentNullException(nameof (where));
      where(this.emissionPhaseObjectGraphVisitorFactories.CreateRegistrationLocationSelector(typeof (TObjectGraphVisitor), (Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>) (args => (IObjectGraphVisitor<IEmitter>) objectGraphVisitorFactory(args))));
      return this;
    }

    public SerializerBuilder WithEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>(
      WrapperFactory<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>, TObjectGraphVisitor> objectGraphVisitorFactory,
      Action<ITrackingRegistrationLocationSelectionSyntax<IObjectGraphVisitor<IEmitter>>> where)
      where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
    {
      if (objectGraphVisitorFactory == null)
        throw new ArgumentNullException(nameof (objectGraphVisitorFactory));
      if (where == null)
        throw new ArgumentNullException(nameof (where));
      where(this.emissionPhaseObjectGraphVisitorFactories.CreateTrackingRegistrationLocationSelector(typeof (TObjectGraphVisitor), (Func<IObjectGraphVisitor<IEmitter>, EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>) ((wrapped, args) => (IObjectGraphVisitor<IEmitter>) objectGraphVisitorFactory(wrapped, args))));
      return this;
    }

    public SerializerBuilder WithoutEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>() where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter> => this.WithoutEmissionPhaseObjectGraphVisitor(typeof (TObjectGraphVisitor));

    public SerializerBuilder WithoutEmissionPhaseObjectGraphVisitor(Type objectGraphVisitorType)
    {
      if ((object) objectGraphVisitorType == null)
        throw new ArgumentNullException(nameof (objectGraphVisitorType));
      this.emissionPhaseObjectGraphVisitorFactories.Remove(objectGraphVisitorType);
      return this;
    }

    public ISerializer Build() => (ISerializer) Serializer.FromValueSerializer(this.BuildValueSerializer());

    public IValueSerializer BuildValueSerializer()
    {
      IEnumerable<IYamlTypeConverter> typeConverters = this.BuildTypeConverters();
      return (IValueSerializer) new SerializerBuilder.ValueSerializer(this.objectGraphTraversalStrategyFactory(this.BuildTypeInspector(), this.typeResolver, typeConverters, this.maximumRecursion), this.eventEmitterFactories.BuildComponentChain<IEventEmitter>((IEventEmitter) new WriterEventEmitter()), typeConverters, this.preProcessingPhaseObjectGraphVisitorFactories.Clone(), this.emissionPhaseObjectGraphVisitorFactories.Clone());
    }

    private class ValueSerializer : IValueSerializer
    {
      private readonly IObjectGraphTraversalStrategy traversalStrategy;
      private readonly IEventEmitter eventEmitter;
      private readonly IEnumerable<IYamlTypeConverter> typeConverters;
      private readonly LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitorFactories;
      private readonly LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>> emissionPhaseObjectGraphVisitorFactories;

      public ValueSerializer(
        IObjectGraphTraversalStrategy traversalStrategy,
        IEventEmitter eventEmitter,
        IEnumerable<IYamlTypeConverter> typeConverters,
        LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitorFactories,
        LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>> emissionPhaseObjectGraphVisitorFactories)
      {
        this.traversalStrategy = traversalStrategy;
        this.eventEmitter = eventEmitter;
        this.typeConverters = typeConverters;
        this.preProcessingPhaseObjectGraphVisitorFactories = preProcessingPhaseObjectGraphVisitorFactories;
        this.emissionPhaseObjectGraphVisitorFactories = emissionPhaseObjectGraphVisitorFactories;
      }

      public void SerializeValue(IEmitter emitter, object value, Type type)
      {
        Type type1 = (object) type != null ? type : (value != null ? value.GetType() : typeof (object));
        Type type2 = type;
        if ((object) type2 == null)
          type2 = typeof (object);
        Type staticType = type2;
        ObjectDescriptor graph = new ObjectDescriptor(value, type1, staticType);
        List<IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitors = this.preProcessingPhaseObjectGraphVisitorFactories.BuildComponentList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>(this.typeConverters);
        foreach (IObjectGraphVisitor<Nothing> visitor in preProcessingPhaseObjectGraphVisitors)
          this.traversalStrategy.Traverse<Nothing>((IObjectDescriptor) graph, visitor, (Nothing) null);
        ObjectSerializer nestedObjectSerializer = (ObjectSerializer) ((v, t) => this.SerializeValue(emitter, v, t));
        IObjectGraphVisitor<IEmitter> visitor1 = this.emissionPhaseObjectGraphVisitorFactories.BuildComponentChain<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>((IObjectGraphVisitor<IEmitter>) new EmittingObjectGraphVisitor(this.eventEmitter), (Func<IObjectGraphVisitor<IEmitter>, EmissionPhaseObjectGraphVisitorArgs>) (inner => new EmissionPhaseObjectGraphVisitorArgs(inner, this.eventEmitter, (IEnumerable<IObjectGraphVisitor<Nothing>>) preProcessingPhaseObjectGraphVisitors, this.typeConverters, nestedObjectSerializer)));
        this.traversalStrategy.Traverse<IEmitter>((IObjectDescriptor) graph, visitor1, emitter);
      }
    }
  }
}
