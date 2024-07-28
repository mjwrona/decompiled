// Decompiled with JetBrains decompiler
// Type: Nest.ProcessorsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ProcessorsDescriptor : DescriptorPromiseBase<ProcessorsDescriptor, IList<IProcessor>>
  {
    public ProcessorsDescriptor()
      : base((IList<IProcessor>) new List<IProcessor>())
    {
    }

    public ProcessorsDescriptor Attachment<T>(
      Func<AttachmentProcessorDescriptor<T>, IAttachmentProcessor> selector)
      where T : class
    {
      return this.Assign<Func<AttachmentProcessorDescriptor<T>, IAttachmentProcessor>>(selector, (Action<IList<IProcessor>, Func<AttachmentProcessorDescriptor<T>, IAttachmentProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new AttachmentProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Append<T>(
      Func<AppendProcessorDescriptor<T>, IAppendProcessor> selector)
      where T : class
    {
      return this.Assign<Func<AppendProcessorDescriptor<T>, IAppendProcessor>>(selector, (Action<IList<IProcessor>, Func<AppendProcessorDescriptor<T>, IAppendProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new AppendProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Csv<T>(
      Func<CsvProcessorDescriptor<T>, ICsvProcessor> selector)
      where T : class
    {
      return this.Assign<Func<CsvProcessorDescriptor<T>, ICsvProcessor>>(selector, (Action<IList<IProcessor>, Func<CsvProcessorDescriptor<T>, ICsvProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new CsvProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Convert<T>(
      Func<ConvertProcessorDescriptor<T>, IConvertProcessor> selector)
      where T : class
    {
      return this.Assign<Func<ConvertProcessorDescriptor<T>, IConvertProcessor>>(selector, (Action<IList<IProcessor>, Func<ConvertProcessorDescriptor<T>, IConvertProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new ConvertProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Date<T>(
      Func<DateProcessorDescriptor<T>, IDateProcessor> selector)
      where T : class
    {
      return this.Assign<Func<DateProcessorDescriptor<T>, IDateProcessor>>(selector, (Action<IList<IProcessor>, Func<DateProcessorDescriptor<T>, IDateProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new DateProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor DateIndexName<T>(
      Func<DateIndexNameProcessorDescriptor<T>, IDateIndexNameProcessor> selector)
      where T : class
    {
      return this.Assign<Func<DateIndexNameProcessorDescriptor<T>, IDateIndexNameProcessor>>(selector, (Action<IList<IProcessor>, Func<DateIndexNameProcessorDescriptor<T>, IDateIndexNameProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new DateIndexNameProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor DotExpander<T>(
      Func<DotExpanderProcessorDescriptor<T>, IDotExpanderProcessor> selector)
      where T : class
    {
      return this.Assign<Func<DotExpanderProcessorDescriptor<T>, IDotExpanderProcessor>>(selector, (Action<IList<IProcessor>, Func<DotExpanderProcessorDescriptor<T>, IDotExpanderProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new DotExpanderProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Enrich<T>(
      Func<EnrichProcessorDescriptor<T>, IEnrichProcessor> selector)
      where T : class
    {
      return this.Assign<Func<EnrichProcessorDescriptor<T>, IEnrichProcessor>>(selector, (Action<IList<IProcessor>, Func<EnrichProcessorDescriptor<T>, IEnrichProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new EnrichProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Fail(
      Func<FailProcessorDescriptor, IFailProcessor> selector)
    {
      return this.Assign<Func<FailProcessorDescriptor, IFailProcessor>>(selector, (Action<IList<IProcessor>, Func<FailProcessorDescriptor, IFailProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new FailProcessorDescriptor()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Foreach<T>(
      Func<ForeachProcessorDescriptor<T>, IForeachProcessor> selector)
      where T : class
    {
      return this.Assign<Func<ForeachProcessorDescriptor<T>, IForeachProcessor>>(selector, (Action<IList<IProcessor>, Func<ForeachProcessorDescriptor<T>, IForeachProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new ForeachProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor GeoIp<T>(
      Func<GeoIpProcessorDescriptor<T>, IGeoIpProcessor> selector)
      where T : class
    {
      return this.Assign<Func<GeoIpProcessorDescriptor<T>, IGeoIpProcessor>>(selector, (Action<IList<IProcessor>, Func<GeoIpProcessorDescriptor<T>, IGeoIpProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new GeoIpProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Grok<T>(
      Func<GrokProcessorDescriptor<T>, IGrokProcessor> selector)
      where T : class
    {
      return this.Assign<Func<GrokProcessorDescriptor<T>, IGrokProcessor>>(selector, (Action<IList<IProcessor>, Func<GrokProcessorDescriptor<T>, IGrokProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new GrokProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Gsub<T>(
      Func<GsubProcessorDescriptor<T>, IGsubProcessor> selector)
      where T : class
    {
      return this.Assign<Func<GsubProcessorDescriptor<T>, IGsubProcessor>>(selector, (Action<IList<IProcessor>, Func<GsubProcessorDescriptor<T>, IGsubProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new GsubProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Join<T>(
      Func<JoinProcessorDescriptor<T>, IJoinProcessor> selector)
      where T : class
    {
      return this.Assign<Func<JoinProcessorDescriptor<T>, IJoinProcessor>>(selector, (Action<IList<IProcessor>, Func<JoinProcessorDescriptor<T>, IJoinProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new JoinProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Lowercase<T>(
      Func<LowercaseProcessorDescriptor<T>, ILowercaseProcessor> selector)
      where T : class
    {
      return this.Assign<Func<LowercaseProcessorDescriptor<T>, ILowercaseProcessor>>(selector, (Action<IList<IProcessor>, Func<LowercaseProcessorDescriptor<T>, ILowercaseProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new LowercaseProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Remove<T>(
      Func<RemoveProcessorDescriptor<T>, IRemoveProcessor> selector)
      where T : class
    {
      return this.Assign<Func<RemoveProcessorDescriptor<T>, IRemoveProcessor>>(selector, (Action<IList<IProcessor>, Func<RemoveProcessorDescriptor<T>, IRemoveProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new RemoveProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Rename<T>(
      Func<RenameProcessorDescriptor<T>, IRenameProcessor> selector)
      where T : class
    {
      return this.Assign<Func<RenameProcessorDescriptor<T>, IRenameProcessor>>(selector, (Action<IList<IProcessor>, Func<RenameProcessorDescriptor<T>, IRenameProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new RenameProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Script(
      Func<ScriptProcessorDescriptor, IScriptProcessor> selector)
    {
      return this.Assign<Func<ScriptProcessorDescriptor, IScriptProcessor>>(selector, (Action<IList<IProcessor>, Func<ScriptProcessorDescriptor, IScriptProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new ScriptProcessorDescriptor()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Set<T>(
      Func<SetProcessorDescriptor<T>, ISetProcessor> selector)
      where T : class
    {
      return this.Assign<Func<SetProcessorDescriptor<T>, ISetProcessor>>(selector, (Action<IList<IProcessor>, Func<SetProcessorDescriptor<T>, ISetProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new SetProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Sort<T>(
      Func<SortProcessorDescriptor<T>, ISortProcessor> selector)
      where T : class
    {
      return this.Assign<Func<SortProcessorDescriptor<T>, ISortProcessor>>(selector, (Action<IList<IProcessor>, Func<SortProcessorDescriptor<T>, ISortProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new SortProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Split<T>(
      Func<SplitProcessorDescriptor<T>, ISplitProcessor> selector)
      where T : class
    {
      return this.Assign<Func<SplitProcessorDescriptor<T>, ISplitProcessor>>(selector, (Action<IList<IProcessor>, Func<SplitProcessorDescriptor<T>, ISplitProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new SplitProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Trim<T>(
      Func<TrimProcessorDescriptor<T>, ITrimProcessor> selector)
      where T : class
    {
      return this.Assign<Func<TrimProcessorDescriptor<T>, ITrimProcessor>>(selector, (Action<IList<IProcessor>, Func<TrimProcessorDescriptor<T>, ITrimProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new TrimProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Uppercase<T>(
      Func<UppercaseProcessorDescriptor<T>, IUppercaseProcessor> selector)
      where T : class
    {
      return this.Assign<Func<UppercaseProcessorDescriptor<T>, IUppercaseProcessor>>(selector, (Action<IList<IProcessor>, Func<UppercaseProcessorDescriptor<T>, IUppercaseProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new UppercaseProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Json<T>(
      Func<JsonProcessorDescriptor<T>, IJsonProcessor> selector)
      where T : class
    {
      return this.Assign<Func<JsonProcessorDescriptor<T>, IJsonProcessor>>(selector, (Action<IList<IProcessor>, Func<JsonProcessorDescriptor<T>, IJsonProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new JsonProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor UserAgent<T>(
      Func<UserAgentProcessorDescriptor<T>, IUserAgentProcessor> selector)
      where T : class
    {
      return this.Assign<Func<UserAgentProcessorDescriptor<T>, IUserAgentProcessor>>(selector, (Action<IList<IProcessor>, Func<UserAgentProcessorDescriptor<T>, IUserAgentProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new UserAgentProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Kv<T>(
      Func<KeyValueProcessorDescriptor<T>, IKeyValueProcessor> selector)
      where T : class
    {
      return this.Assign<Func<KeyValueProcessorDescriptor<T>, IKeyValueProcessor>>(selector, (Action<IList<IProcessor>, Func<KeyValueProcessorDescriptor<T>, IKeyValueProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new KeyValueProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor UrlDecode<T>(
      Func<UrlDecodeProcessorDescriptor<T>, IUrlDecodeProcessor> selector)
      where T : class
    {
      return this.Assign<Func<UrlDecodeProcessorDescriptor<T>, IUrlDecodeProcessor>>(selector, (Action<IList<IProcessor>, Func<UrlDecodeProcessorDescriptor<T>, IUrlDecodeProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new UrlDecodeProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Bytes<T>(
      Func<BytesProcessorDescriptor<T>, IBytesProcessor> selector)
      where T : class
    {
      return this.Assign<Func<BytesProcessorDescriptor<T>, IBytesProcessor>>(selector, (Action<IList<IProcessor>, Func<BytesProcessorDescriptor<T>, IBytesProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new BytesProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Dissect<T>(
      Func<DissectProcessorDescriptor<T>, IDissectProcessor> selector)
      where T : class
    {
      return this.Assign<Func<DissectProcessorDescriptor<T>, IDissectProcessor>>(selector, (Action<IList<IProcessor>, Func<DissectProcessorDescriptor<T>, IDissectProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new DissectProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Drop(
      Func<DropProcessorDescriptor, IDropProcessor> selector)
    {
      return this.Assign<Func<DropProcessorDescriptor, IDropProcessor>>(selector, (Action<IList<IProcessor>, Func<DropProcessorDescriptor, IDropProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new DropProcessorDescriptor()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor SetSecurityUser<T>(
      Func<SetSecurityUserProcessorDescriptor<T>, ISetSecurityUserProcessor> selector)
      where T : class
    {
      return this.Assign<Func<SetSecurityUserProcessorDescriptor<T>, ISetSecurityUserProcessor>>(selector, (Action<IList<IProcessor>, Func<SetSecurityUserProcessorDescriptor<T>, ISetSecurityUserProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new SetSecurityUserProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Pipeline(
      Func<PipelineProcessorDescriptor, IPipelineProcessor> selector)
    {
      return this.Assign<Func<PipelineProcessorDescriptor, IPipelineProcessor>>(selector, (Action<IList<IProcessor>, Func<PipelineProcessorDescriptor, IPipelineProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new PipelineProcessorDescriptor()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Circle<T>(
      Func<CircleProcessorDescriptor<T>, ICircleProcessor> selector)
      where T : class
    {
      return this.Assign<Func<CircleProcessorDescriptor<T>, ICircleProcessor>>(selector, (Action<IList<IProcessor>, Func<CircleProcessorDescriptor<T>, ICircleProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new CircleProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor UriParts<T>(
      Func<UriPartsProcessorDescriptor<T>, IUriPartsProcessor> selector)
      where T : class
    {
      return this.Assign<Func<UriPartsProcessorDescriptor<T>, IUriPartsProcessor>>(selector, (Action<IList<IProcessor>, Func<UriPartsProcessorDescriptor<T>, IUriPartsProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new UriPartsProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Fingerprint<T>(
      Func<FingerprintProcessorDescriptor<T>, IFingerprintProcessor> selector)
      where T : class
    {
      return this.Assign<Func<FingerprintProcessorDescriptor<T>, IFingerprintProcessor>>(selector, (Action<IList<IProcessor>, Func<FingerprintProcessorDescriptor<T>, IFingerprintProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new FingerprintProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor NetworkCommunityId<T>(
      Func<NetworkCommunityIdProcessorDescriptor<T>, INetworkCommunityIdProcessor> selector)
      where T : class
    {
      return this.Assign<Func<NetworkCommunityIdProcessorDescriptor<T>, INetworkCommunityIdProcessor>>(selector, (Action<IList<IProcessor>, Func<NetworkCommunityIdProcessorDescriptor<T>, INetworkCommunityIdProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new NetworkCommunityIdProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor NetworkDirection<T>(
      Func<NetworkDirectionProcessorDescriptor<T>, INetworkDirectionProcessor> selector)
      where T : class
    {
      return this.Assign<Func<NetworkDirectionProcessorDescriptor<T>, INetworkDirectionProcessor>>(selector, (Action<IList<IProcessor>, Func<NetworkDirectionProcessorDescriptor<T>, INetworkDirectionProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new NetworkDirectionProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor RegisteredDomain<T>(
      Func<RegisteredDomainProcessorDescriptor<T>, IRegisteredDomainProcessor> selector)
      where T : class
    {
      return this.Assign<Func<RegisteredDomainProcessorDescriptor<T>, IRegisteredDomainProcessor>>(selector, (Action<IList<IProcessor>, Func<RegisteredDomainProcessorDescriptor<T>, IRegisteredDomainProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new RegisteredDomainProcessorDescriptor<T>()) : (IProcessor) null)));
    }

    public ProcessorsDescriptor Inference<T>(
      Func<InferenceProcessorDescriptor<T>, IInferenceProcessor> selector)
      where T : class
    {
      return this.Assign<Func<InferenceProcessorDescriptor<T>, IInferenceProcessor>>(selector, (Action<IList<IProcessor>, Func<InferenceProcessorDescriptor<T>, IInferenceProcessor>>) ((a, v) => a.AddIfNotNull<IProcessor>(v != null ? (IProcessor) v(new InferenceProcessorDescriptor<T>()) : (IProcessor) null)));
    }
  }
}
