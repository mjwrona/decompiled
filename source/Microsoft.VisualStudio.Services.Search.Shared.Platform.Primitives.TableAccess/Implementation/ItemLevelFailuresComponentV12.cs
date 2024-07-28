// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV12
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV12 : ItemLevelFailuresComponentV11
  {
    public ItemLevelFailuresComponentV12()
    {
    }

    internal ItemLevelFailuresComponentV12(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override int GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnitId(
      int indexingUnitId,
      List<RejectionCode> rejectionCodesListToExclude,
      int hoursToLookBack)
    {
      if (hoursToLookBack < 0 || indexingUnitId < 1)
        throw new ArgumentException(hoursToLookBack < 0 ? nameof (hoursToLookBack) : nameof (indexingUnitId));
      if (rejectionCodesListToExclude == null)
        rejectionCodesListToExclude = new List<RejectionCode>();
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnit");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.BindTinyIntIds("@rejectionCodesToExclude", (IEnumerable<byte>) rejectionCodesListToExclude.ConvertAll<byte>((Converter<RejectionCode, byte>) (code => (byte) code)));
      this.BindInt("@hoursToLookBack", hoursToLookBack);
      int forIndexingUnitId = (int) this.ExecuteScalar();
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnitId took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return forIndexingUnitId;
    }

    public override IDictionary<string, IDictionary<int, int>> GetCountOfRecordsCreatedBeforeGivenHoursGroupedByEntityAndIndexingUnitId(
      List<RejectionCode> rejectionCodesListToExclude,
      int hoursToLookBack)
    {
      if (hoursToLookBack < 0)
        throw new ArgumentException(nameof (hoursToLookBack));
      if (rejectionCodesListToExclude == null)
        rejectionCodesListToExclude = new List<RejectionCode>();
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_GetCountOfRecordsCreatedBeforeGivenHoursGroupedByEntityAndIndexingUnitId");
      this.BindInt("@hoursToLookBack", hoursToLookBack);
      this.BindTinyIntIds("@rejectionCodesToExclude", (IEnumerable<byte>) rejectionCodesListToExclude.ConvertAll<byte>((Converter<RejectionCode, byte>) (code => (byte) code)));
      IDictionary<string, IDictionary<int, int>> andIndexingUnitId = (IDictionary<string, IDictionary<int, int>>) new Dictionary<string, IDictionary<int, int>>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<string, int, int>>((ObjectBinder<Tuple<string, int, int>>) new ItemLevelFailuresComponent.CountOfRecordsGroupedByEntityAndIndexingUnit());
        ObjectBinder<Tuple<string, int, int>> current = resultCollection.GetCurrent<Tuple<string, int, int>>();
        if (current?.Items != null)
        {
          if (current.Items.Count > 0)
          {
            foreach (Tuple<string, int, int> tuple in current.Items)
            {
              IDictionary<int, int> dictionary;
              if (!andIndexingUnitId.TryGetValue(tuple.Item1, out dictionary))
              {
                dictionary = (IDictionary<int, int>) new Dictionary<int, int>();
                andIndexingUnitId.Add(tuple.Item1, dictionary);
              }
              dictionary.Add(tuple.Item2, tuple.Item3);
            }
          }
        }
      }
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.GetCountOfRecordsCreatedBeforeGivenHoursGroupedByIndexingUnitId took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return andIndexingUnitId;
    }
  }
}
