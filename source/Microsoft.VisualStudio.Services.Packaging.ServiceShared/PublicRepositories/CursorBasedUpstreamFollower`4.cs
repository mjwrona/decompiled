// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.CursorBasedUpstreamFollower`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class CursorBasedUpstreamFollower<TCursor, TPackageName, TPackageVersion, TPackageIdentity> : 
    IUpstreamFollower
    where TCursor : class, IComparable<TCursor>, IEquatable<TCursor>
    where TPackageName : class, IPackageName
    where TPackageVersion : class, IPackageVersion
    where TPackageIdentity : class, IPackageIdentity<TPackageName, TPackageVersion>
  {
    private const int MaxTelemetryPackageNames = 10;
    private readonly IPublicRepositoryInterestTracker<TPackageName> interestTracker;
    private readonly IDeploymentPackageUpstreamRefreshJobQueuer jobQueuer;
    private readonly ITracerService tracerService;
    private readonly IBookmarkTokenProvider<NullRequest, TCursor?> bookmarkTokenProvider;
    private readonly IPublicRepositoryWithCursorAssistedInvalidation<TPackageName, TPackageVersion, TCursor> repository;
    private readonly ICursorBasedUpstreamChangeProvider<TCursor, TPackageIdentity> changeProvider;
    private readonly IOrgLevelPackagingSetting<bool> queueUpdateJobsSetting;
    private readonly IOrgLevelPackagingSetting<int> maxPackageInvalidationsSetting;

    public CursorBasedUpstreamFollower(
      IPublicRepositoryInterestTracker<TPackageName> interestTracker,
      IDeploymentPackageUpstreamRefreshJobQueuer jobQueuer,
      ITracerService tracerService,
      IBookmarkTokenProvider<NullRequest, TCursor?> bookmarkTokenProvider,
      IPublicRepositoryWithCursorAssistedInvalidation<TPackageName, TPackageVersion, TCursor> repository,
      ICursorBasedUpstreamChangeProvider<TCursor, TPackageIdentity> changeProvider,
      IOrgLevelPackagingSetting<bool> queueUpdateJobsSetting,
      IOrgLevelPackagingSetting<int> maxPackageInvalidationsSetting)
    {
      this.changeProvider = changeProvider;
      this.queueUpdateJobsSetting = queueUpdateJobsSetting;
      this.maxPackageInvalidationsSetting = maxPackageInvalidationsSetting;
      this.interestTracker = interestTracker;
      this.jobQueuer = jobQueuer;
      this.tracerService = tracerService;
      this.bookmarkTokenProvider = bookmarkTokenProvider;
      this.repository = repository;
    }

    public async Task ProcessRecentUpstreamChangesAsync(UpstreamFollowerTelemetryInfo telemetryInfo)
    {
      CursorBasedUpstreamFollower<TCursor, TPackageName, TPackageVersion, TPackageIdentity> sendInTheThisObject = this;
      ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ProcessRecentUpstreamChangesAsync));
      try
      {
        telemetryInfo.ExitReason = UpstreamFollowerExitReason.Exception;
        HashSet<TPackageName> interestingPackageNamesSet = sendInTheThisObject.GetInterestingPackageNamesSet();
        int? count = interestingPackageNamesSet?.Count;
        telemetryInfo.InterestingPackageNamesCount = count;
        int? nullable = count;
        int num = 0;
        if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        {
          telemetryInfo.ExitReason = UpstreamFollowerExitReason.NoInterest;
          tracer = (ITracerBlock) null;
          interestingPackageNamesSet = (HashSet<TPackageName>) null;
        }
        else
        {
          TCursor since = sendInTheThisObject.LoadCursor();
          telemetryInfo.InitialCursorPosition = since?.ToString();
          IReadOnlyList<ChangedPackage<TCursor, TPackageIdentity>> changes = await sendInTheThisObject.changeProvider.GetChanges(since);
          telemetryInfo.ChangesProvided = new int?(changes.Count);
          if (changes.Count == 0)
          {
            telemetryInfo.ExitReason = UpstreamFollowerExitReason.NoChanges;
            tracer = (ITracerBlock) null;
            interestingPackageNamesSet = (HashSet<TPackageName>) null;
          }
          else
          {
            TCursor newCursor = await sendInTheThisObject.ProcessChangedPackagesAsync((IEnumerable<ChangedPackage<TCursor, TPackageIdentity>>) changes, (ISet<TPackageName>) interestingPackageNamesSet, telemetryInfo, sendInTheThisObject.queueUpdateJobsSetting.Get(), sendInTheThisObject.maxPackageInvalidationsSetting.Get());
            telemetryInfo.FinalCursorPosition = newCursor?.ToString();
            if ((object) newCursor == null)
            {
              tracer = (ITracerBlock) null;
              interestingPackageNamesSet = (HashSet<TPackageName>) null;
            }
            else
            {
              sendInTheThisObject.SaveCursor(newCursor);
              tracer = (ITracerBlock) null;
              interestingPackageNamesSet = (HashSet<TPackageName>) null;
            }
          }
        }
      }
      finally
      {
        tracer?.Dispose();
      }
    }

    private HashSet<TPackageName>? GetInterestingPackageNamesSet()
    {
      try
      {
        return this.interestTracker.GetAllPackagesWithInterestedFeeds(this.repository.WellKnownUpstreamSource).ToHashSet<TPackageName>((IEqualityComparer<TPackageName>) PackageNameComparer.NormalizedName);
      }
      catch (ServiceVersionNotSupportedException ex)
      {
        return (HashSet<TPackageName>) null;
      }
    }

    private async Task<TCursor?> ProcessChangedPackagesAsync(
      IEnumerable<ChangedPackage<TCursor, TPackageIdentity>> changedPackages,
      ISet<TPackageName>? interestingPackageNamesSet,
      UpstreamFollowerTelemetryInfo telemetryInfo,
      bool queueUpdateJobs,
      int maxPackageInvalidations)
    {
      CursorBasedUpstreamFollower<TCursor, TPackageName, TPackageVersion, TPackageIdentity> sendInTheThisObject = this;
      // ISSUE: variable of a compiler-generated type
      CursorBasedUpstreamFollower<TCursor, TPackageName, TPackageVersion, TPackageIdentity>.\u003C\u003Ec__DisplayClass12_0 cDisplayClass120;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass120.interestingPackageNamesSet = interestingPackageNamesSet;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass120.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass120.traceBlock = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ProcessChangedPackagesAsync));
      try
      {
        List<IGrouping<TCursor, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>> list = changedPackages.GroupBy<ChangedPackage<TCursor, TPackageIdentity>, TPackageName>((Func<ChangedPackage<TCursor, TPackageIdentity>, TPackageName>) (x => x.PackageIdentity.Name)).Select<IGrouping<TPackageName, ChangedPackage<TCursor, TPackageIdentity>>, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>((Func<IGrouping<TPackageName, ChangedPackage<TCursor, TPackageIdentity>>, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>) (x => (x.Key, x.OrderBy<ChangedPackage<TCursor, TPackageIdentity>, TCursor>((Func<ChangedPackage<TCursor, TPackageIdentity>, TCursor>) (y => y.CursorPosition)).ToList<ChangedPackage<TCursor, TPackageIdentity>>()))).GroupBy<(TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>), TCursor>((Func<(TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>), TCursor>) (x => x.Changes[0].CursorPosition)).OrderBy<IGrouping<TCursor, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>, TCursor>((Func<IGrouping<TCursor, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>, TCursor>) (x => x.Key)).ToList<IGrouping<TCursor, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>>();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass120.updatedPackageNames = new List<string>(10);
        // ISSUE: reference to a compiler-generated field
        telemetryInfo.LastUpdatedPackageNames = (IReadOnlyCollection<string>) cDisplayClass120.updatedPackageNames;
        TCursor fullyProcessedThroughCursor = default (TCursor);
        foreach (IGrouping<TCursor, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)> grouping in list)
        {
          IGrouping<TCursor, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)> cursorGroup = grouping;
          foreach ((TPackageName packageName, List<ChangedPackage<TCursor, TPackageIdentity>> changedPackageList) in (IEnumerable<(TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>) cursorGroup)
          {
            telemetryInfo.ChangesAttempted += changedPackageList.Count;
            RecordChange(packageName, changedPackageList, ref cDisplayClass120);
            // ISSUE: reference to a compiler-generated method
            bool isInterestingPackage = sendInTheThisObject.\u003CProcessChangedPackagesAsync\u003Eg__AreAnyFeedsInterestedInPackage\u007C12_5(packageName, ref cDisplayClass120);
            try
            {
              await sendInTheThisObject.repository.InvalidatePackageVersionDataAsync(packageName, changedPackageList.Select<ChangedPackage<TCursor, TPackageIdentity>, TPackageVersion>((Func<ChangedPackage<TCursor, TPackageIdentity>, TPackageVersion>) (x => x.PackageIdentity.Version)), changedPackageList.Last<ChangedPackage<TCursor, TPackageIdentity>>().CursorPosition, isInterestingPackage);
            }
            catch (UpstreamNotUpToDateException ex)
            {
              // ISSUE: reference to a compiler-generated field
              cDisplayClass120.traceBlock.TraceException((Exception) ex);
              telemetryInfo.ExitReason = UpstreamFollowerExitReason.UpstreamNotUpToDate;
              return fullyProcessedThroughCursor;
            }
            ++telemetryInfo.PackagesInvalidated;
            if (isInterestingPackage & queueUpdateJobs)
            {
              sendInTheThisObject.jobQueuer.QueuePackageJob((IPackageName) packageName, (IPackageVersion) changedPackageList.Max<ChangedPackage<TCursor, TPackageIdentity>, TPackageVersion>((Func<ChangedPackage<TCursor, TPackageIdentity>, TPackageVersion>) (x => x.PackageIdentity.Version)), sendInTheThisObject.repository.WellKnownUpstreamSource, changedPackageList.Last<ChangedPackage<TCursor, TPackageIdentity>>().IngestionTimestamp);
              ++telemetryInfo.JobsQueued;
            }
            packageName = default (TPackageName);
            changedPackageList = (List<ChangedPackage<TCursor, TPackageIdentity>>) null;
          }
          fullyProcessedThroughCursor = cursorGroup.Key;
          ++telemetryInfo.CursorPositionsProcessed;
          if (telemetryInfo.PackagesInvalidated >= maxPackageInvalidations)
          {
            telemetryInfo.ExitReason = UpstreamFollowerExitReason.ReachedLimit;
            return fullyProcessedThroughCursor;
          }
          cursorGroup = (IGrouping<TCursor, (TPackageName, List<ChangedPackage<TCursor, TPackageIdentity>>)>) null;
        }
        telemetryInfo.ExitReason = UpstreamFollowerExitReason.ReachedCurrent;
        return fullyProcessedThroughCursor;
      }
      finally
      {
        // ISSUE: reference to a compiler-generated field
        if (cDisplayClass120.traceBlock != null)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass120.traceBlock.Dispose();
        }
      }

      void RecordChange(
        TPackageName packageName,
        List<ChangedPackage<TCursor, TPackageIdentity>> changes,
        ref CursorBasedUpstreamFollower<
        #nullable disable
        TCursor, TPackageName, TPackageVersion, TPackageIdentity>.\u003C\u003Ec__DisplayClass12_0 _param3)
      {
        // ISSUE: reference to a compiler-generated field
        _param3.traceBlock.TraceInfoAlways(new string[1]
        {
          "UpstreamFollowerProcessingPackage"
        }, new
        {
          PackageName = packageName.DisplayName,
          ChangeCount = changes.Count
        }.Serialize(true));
        // ISSUE: reference to a compiler-generated field
        if (_param3.updatedPackageNames.Count >= 10)
        {
          // ISSUE: reference to a compiler-generated field
          _param3.updatedPackageNames.RemoveAt(0);
        }
        // ISSUE: reference to a compiler-generated field
        _param3.updatedPackageNames.Add(packageName.DisplayName);
      }
    }

    private void SaveCursor(
    #nullable enable
    TCursor newCursor) => this.bookmarkTokenProvider.StoreToken((NullRequest) null, newCursor);

    private TCursor? LoadCursor() => this.bookmarkTokenProvider.GetToken((NullRequest) null);
  }
}
