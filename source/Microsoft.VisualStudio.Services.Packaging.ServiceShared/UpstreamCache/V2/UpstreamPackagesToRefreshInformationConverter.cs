// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamPackagesToRefreshInformationConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class UpstreamPackagesToRefreshInformationConverter : 
    IUpstreamPackagesToRefreshInformationConverter
  {
    private readonly IConverter<string, IPackageName> packageNameConverter;
    private readonly UpstreamMetadataRefreshJobSplittingConfiguration jobSplittingConfiguration;
    private readonly IRegistryService registryService;

    public UpstreamPackagesToRefreshInformationConverter(
      IConverter<string, IPackageName> packageNameConverter,
      UpstreamMetadataRefreshJobSplittingConfiguration jobSplittingConfiguration,
      IRegistryService registryService)
    {
      this.packageNameConverter = packageNameConverter;
      this.jobSplittingConfiguration = jobSplittingConfiguration;
      this.registryService = registryService;
    }

    public IEnumerable<UpstreamPackagesToRefreshInformation> GetListOfUpstreamPackagesToRefreshInformation(
      FeedCore feed,
      UpstreamMetadataCacheInfo info,
      IProtocol protocol)
    {
      FeedRequest feedRequest = new FeedRequest(feed, protocol);
      int maxNumberOfJobs = this.jobSplittingConfiguration.GetMaxNumberOfJobs((IFeedRequest) feedRequest);
      int higherLimitOfWork = this.jobSplittingConfiguration.GetHigherLimitOfWork((IFeedRequest) feedRequest);
      List<IPackageName> list1 = info.PackageNames.OrderBy<IPackageName, IPackageName>((Func<IPackageName, IPackageName>) (x => x), (IComparer<IPackageName>) PackageNameComparer.NormalizedName).ToList<IPackageName>();
      int num1 = Math.Min((int) Math.Ceiling((double) UpstreamPackageUtils.GetUnitsOfWork(this.registryService, list1.Count<IPackageName>(), feed, protocol) / (double) higherLimitOfWork), maxNumberOfJobs);
      if (num1 < 1)
        return (IEnumerable<UpstreamPackagesToRefreshInformation>) new List<UpstreamPackagesToRefreshInformation>()
        {
          new UpstreamPackagesToRefreshInformation(feed.Id, (IPackageName) null, (IPackageName) null)
        };
      int num2 = (int) Math.Ceiling((double) list1.Count<IPackageName>() / (double) num1);
      int count1 = num2 + 1;
      List<UpstreamPackagesToRefreshInformation> source = new List<UpstreamPackagesToRefreshInformation>();
      for (int count2 = 0; count2 < list1.Count<IPackageName>(); count2 += num2)
      {
        List<IPackageName> list2 = list1.Skip<IPackageName>(count2).Take<IPackageName>(count1).ToList<IPackageName>();
        IPackageName firstPackage = list2.First<IPackageName>();
        IPackageName lastPackage = list2.Last<IPackageName>();
        if (source.Count<UpstreamPackagesToRefreshInformation>() == 0)
          firstPackage = (IPackageName) null;
        if (source.Count<UpstreamPackagesToRefreshInformation>() == num1 - 1)
          lastPackage = (IPackageName) null;
        UpstreamPackagesToRefreshInformation refreshInformation = new UpstreamPackagesToRefreshInformation(feed.Id, firstPackage, lastPackage);
        source.Add(refreshInformation);
        if (source.Count<UpstreamPackagesToRefreshInformation>() == num1)
          break;
      }
      return (IEnumerable<UpstreamPackagesToRefreshInformation>) source;
    }

    public IEnumerable<IPackageName> FindPackagesToRefresh(
      ISet<IPackageName> packageNames,
      UpstreamPackagesToRefreshInformation packageInformation)
    {
      List<IPackageName> list = packageNames.OrderBy<IPackageName, IPackageName>((Func<IPackageName, IPackageName>) (x => x), (IComparer<IPackageName>) PackageNameComparer.NormalizedName).ToList<IPackageName>();
      IPackageName startPackageNameNormalized = packageInformation.FirstPackageDisplayName != null ? this.packageNameConverter.Convert(packageInformation.FirstPackageDisplayName) : (IPackageName) null;
      IPackageName endPackageNameNormalized = packageInformation.LastPackageDisplayName != null ? this.packageNameConverter.Convert(packageInformation.LastPackageDisplayName) : (IPackageName) null;
      IEnumerable<IPackageName> source = startPackageNameNormalized != null ? list.SkipWhile<IPackageName>((Func<IPackageName, bool>) (x => PackageNameComparer.NormalizedName.Compare(x, startPackageNameNormalized) < 0)) : (IEnumerable<IPackageName>) list;
      return endPackageNameNormalized == null ? source : source.TakeWhile<IPackageName>((Func<IPackageName, bool>) (x => PackageNameComparer.NormalizedName.Compare(x, endPackageNameNormalized) < 0));
    }
  }
}
