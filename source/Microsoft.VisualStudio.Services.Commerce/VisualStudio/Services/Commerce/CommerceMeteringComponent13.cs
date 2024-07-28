// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent13
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent13 : CommerceMeteringComponent12
  {
    internal override int UpdateCommittedAndCurrentQuantities(
      int meterId,
      byte resourceSeq,
      int committedQuantity,
      int currentQuantity,
      Guid lastUpdatedBy)
    {
      try
      {
        this.TraceEnter(5109004, nameof (UpdateCommittedAndCurrentQuantities));
        this.Trace(5109005, TraceLevel.Info, string.Format("{0}: calling {1}. (CollectionId: {2}, ResourceId: {3}, ResourceSeq: {4}, CommittedQuantity: {5}, CurrentQuantity: {6}, LastUpdatedBy: {7})", (object) nameof (UpdateCommittedAndCurrentQuantities), (object) "prc_UpdateCommittedAndCurrentQuantities", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) meterId, (object) resourceSeq, (object) committedQuantity, (object) currentQuantity, (object) lastUpdatedBy));
        this.PrepareStoredProcedure("prc_UpdateCommittedAndCurrentQuantities");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindInt("@resourceId", meterId);
        this.BindByte("@resourceSeq", resourceSeq);
        this.BindInt("@committedQuantity", committedQuantity);
        this.BindInt("@currentQuantity", currentQuantity);
        this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        this.TraceException(5109007, ex);
        throw;
      }
      finally
      {
        this.Trace(5109008, TraceLevel.Info, string.Format("{0}: Complete. CollectionId: {1}", (object) nameof (UpdateCommittedAndCurrentQuantities), (object) this.ComponentRequestContext.ServiceHost.InstanceId));
        this.TraceLeave(5109009, nameof (UpdateCommittedAndCurrentQuantities));
      }
    }
  }
}
