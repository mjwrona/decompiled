// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.ResourceIds
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class ResourceIds
  {
    public static readonly ApiResourceVersion ApiVersion = new ApiResourceVersion("1.0");
    internal static readonly ApiResourceVersion MultiDomainApiVersion = new ApiResourceVersion("5.1");
    public const string BlobAreaId = "5294EF93-12A1-4D13-8671-9D9D014072C8";
    public const string BlobArea = "blob";
    public const string BlobResourceName = "blobs";
    public const string BlobUrlResourceName = "url";
    public const string BlobBatchResourceName = "blobsbatch";
    public const string ReferenceBatchResourceName = "referencesbatch";
    public static readonly Guid BlobResourceId = new Guid("{D56223DF-8CCD-45C9-89B4-EDDF69240690}");
    public static readonly Guid BlobUrlResourceId = new Guid("{1D1857E7-3F76-4766-AC71-C443AB9093EF}");
    public static readonly Guid ReferenceResourceId = new Guid("{8A483D58-09D3-47D5-8D9F-10DA1BDE704C}");
    public static readonly Guid BlobBatchResourceId = new Guid("{4585DE0E-2CFD-438E-A824-DA53AC2EF0D0}");
    public static readonly Guid ReferenceBatchResourceId = new Guid("{3E5ABB16-5C5B-4C87-9D73-88CB68FA9509}");
    public static readonly Guid MultiDomainBlobResourceId = new Guid("{EE0B79F2-1953-4D0E-84C7-47A3812DAD3F}");
    public static readonly Guid MultiDomainBlobUrlResourceId = new Guid("{99416AEE-B328-4F24-87A6-F005BA1578D9}");
    public static readonly Guid MultiDomainBlobBatchResourceId = new Guid("{42A85FA2-5F01-473F-A281-93FA35A4EB2A}");
    public static readonly Guid MultiDomainReferenceBatchResourceId = new Guid("{41112FB0-004F-45BD-B357-163B51D9DC5D}");
    public const string DedupAreaId = "01E4817C-857E-485C-9401-0334A33200DA";
    public const string DedupArea = "dedup";
    public const string ChunkResourceName = "chunks";
    public const string EchoResourceName = "echo";
    public const string NodeResourceName = "nodes";
    public const string DedupUrlsResourceName = "urls";
    public const string DedupUrlsBatchResourceName = "urlsbatch";
    public const string RootResourceName = "roots";
    public const string ValidationResourceName = "validations";
    public static readonly Guid ChunkResourceId = new Guid("{C8911095-CE13-48E9-B1DC-158C716AA6BA}");
    public static readonly Guid NodeResourceId = new Guid("{53E6E1E0-7444-47EA-93CD-44E6DDF264E6}");
    public static readonly Guid EchoResourceId = new Guid("{40213C1A-2CA1-401F-8A49-E0F59668DFDE}");
    public static readonly Guid DedupUrlsResourceId = new Guid("{3C7526CF-A472-4D43-A44D-2B6D98488ECA}");
    public static readonly Guid DedupUrlsBatchResourceId = new Guid("{89D5AC43-8380-4834-B07E-39E26F441D47}");
    public static readonly Guid RootResourceId = new Guid("{B30D4D8E-D3D7-4C2E-9BB5-F1878D6D2E3A}");
    public static readonly Guid ValidationResourceId = new Guid("{2F154496-2068-4F99-8887-B39B1FB8611D}");
    public const string MultiDomainArea = "domains";
    public const string MultiDomainAreaId = "D7C52D59-024A-4376-A82D-AB6F81122D14";
    public static readonly Guid MultiDomainChunkResourceId = new Guid("{900AA0AF-4B75-496F-8F39-BDF36FAE20CE}");
    public static readonly Guid MultiDomainNodeResourceId = new Guid("{B3887B9B-BC3F-4CBF-8D3A-91F36ABDACA7}");
    public static readonly Guid MultiDomainRootResourceId = new Guid("000B97A0-7804-486E-86A0-26DD73DD6CFB");
    public static readonly Guid MultiDomainDedupUrlsResourceId = new Guid("{C9232A0D-5013-498B-BD91-63905459F44C}");
    public static readonly Guid MultiDomainDedupUrlsBatchResourceId = new Guid("FB07C94C-74ED-45B6-BA48-8217B344A0C5");
    public static readonly Guid MultiDomainDedupBlobsResourceId = new Guid("49B6AF14-4C77-4FBC-B6DE-B877537DA893");
    public const string HostDomainResourceName = "hostdomain";
    public static readonly Guid HostDomainResourceId = new Guid("C0862C01-F74E-47F0-A79A-28D2BD315822");
    public const string ClientToolsAreaId = "3FDA18BA-DFF2-42E6-8D10-C521B23B85FC";
    public const string ClientToolsArea = "clienttools";
    public const string ClientToolsReleaseResourceName = "release";
    public static readonly Guid ClientToolsReleaseResourceId = new Guid("187EC90D-DD1E-4EC6-8C57-937D979261E5");
    public const string ClientSettingsResourceName = "settings";
    public static readonly Guid ClientSettingsResourceId = new Guid("2213DC4F-BF6D-4893-92B2-2EF04C852B40");
    public const string TelemetryAreaId = "7670AA71-46BD-4133-BD39-213FF359D30E";
    public const string TelemetryArea = "pipelineartifactstelemetry";
    public const string TelemetryResourceName = "aiinstrumentationkey";
    public static readonly Guid TelemetryResourceId = new Guid("4CAFE3EF-2526-4AAD-A636-204EE8D2F66B");
    public const string UsageInfoAreaId = "66939471-964E-4475-9EC2-A616D9BD7522";
    public const string UsageInfoArea = "usage";
    public const string UsageInfoMetricsResource = "metrics";
    public static readonly Guid UsageInfoMetricsResourceId = new Guid("{110C51C8-1A45-4CBF-AC4B-B9B7C1F375ED}");
    public const string SessionsCollectionName = "sessions";
    public static readonly Guid SessionsCollectionId = new Guid("E618578C-07D1-454B-9E0E-147927221211");
    public const string PartsCollectionName = "parts";
    public static readonly Guid PartsCollectionId = new Guid("8D18E458-79D9-43E8-8DF4-4B0F41CE7CA8");
    public const string DedupCopyResourceName = "copy";
    public static readonly Guid DedupCopyResourceId = new Guid("11255A61-4C81-4917-8296-94DDB7ED92B6");
  }
}
