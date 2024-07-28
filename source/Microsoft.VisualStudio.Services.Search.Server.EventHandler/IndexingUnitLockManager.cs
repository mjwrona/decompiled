// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitLockManager
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  internal class IndexingUnitLockManager
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "IndexingUnitChangeEventHandler";

    internal IDataAccessFactory DataAccessFactory { get; set; }

    public IndexingUnitLockManager()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal IndexingUnitLockManager(IDataAccessFactory dataAccessFactory) => this.DataAccessFactory = dataAccessFactory;

    internal virtual LockStatus AcquireNecessaryLocks(
      ExecutionContext executionContext,
      int indexingUnitId,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> changeEvents)
    {
      Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (AcquireNecessaryLocks));
      try
      {
        if (changeEvents == null || !changeEvents.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Received null or empty {0} parameter.", (object) nameof (changeEvents))));
        IList<LockDetails> lockingRequirements = this.GetLockingRequirements(executionContext, indexingUnitId);
        if (lockingRequirements == null || !lockingRequirements.Any<LockDetails>())
        {
          Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("No valid locking requirements were obtained. Hence returning LockStatus accordingly")));
          return new LockStatus(false, string.Empty);
        }
        LockStatus lockStatus = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess().AcquireLockAndMarkEventsAsQueued(executionContext.RequestContext, lockingRequirements, changeEvents);
        Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("Attempt to acquire locks on {0} and change event ids : ({1}) completed with Status: {2}.", (object) LockDetails.ToString(lockingRequirements), (object) string.Join<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(",", changeEvents), (object) lockStatus)));
        return lockStatus;
      }
      finally
      {
        Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (AcquireNecessaryLocks));
      }
    }

    [Info("InternalForTestPurpose")]
    internal virtual IList<LockDetails> GetLockingRequirements(
      ExecutionContext executionContext,
      int indexingUnitId)
    {
      Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetLockingRequirements));
      IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, indexingUnitId);
      if (indexingUnit == null)
        return (IList<LockDetails>) new List<LockDetails>();
      Guid guid = indexingUnit.AssociatedJobId.Value;
      IList<LockDetails> source = (IList<LockDetails>) new List<LockDetails>();
      LockDetails lockDetails1 = new LockDetails(indexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture), LockMode.Exclusive, guid.ToString());
      source.Add(lockDetails1);
      int indexingUnitId1 = indexingUnit.IndexingUnitId;
      int parentIndexingUnitId = indexingUnit.ParentUnitId;
      IndexingUnitProvider indexingUnitProvider = IndexingUnitProviderFactory.GetInstance().GetIndexingUnitProvider(indexingUnitDataAccess);
      while (parentIndexingUnitId != -1)
      {
        LockDetails lockDetails2 = new LockDetails(parentIndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture), LockMode.Shared, guid.ToString());
        source.Add(lockDetails2);
        int indexingUnitId2 = parentIndexingUnitId;
        parentIndexingUnitId = -1;
        if (!indexingUnitProvider.TryGetParentIndexingUnitId(executionContext.RequestContext, indexingUnitId2, out parentIndexingUnitId))
          return (IList<LockDetails>) new List<LockDetails>();
      }
      List<LockDetails> list = source.Reverse<LockDetails>().ToList<LockDetails>();
      Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetLockingRequirements));
      return (IList<LockDetails>) list;
    }
  }
}
