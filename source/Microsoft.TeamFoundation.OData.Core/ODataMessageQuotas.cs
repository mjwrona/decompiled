// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessageQuotas
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  public sealed class ODataMessageQuotas
  {
    private int maxPartsPerBatch;
    private int maxOperationsPerChangeset;
    private int maxNestingDepth;
    private long maxReceivedMessageSize;

    public ODataMessageQuotas()
    {
      this.maxPartsPerBatch = 100;
      this.maxOperationsPerChangeset = 1000;
      this.maxNestingDepth = 100;
      this.maxReceivedMessageSize = 1048576L;
    }

    public ODataMessageQuotas(ODataMessageQuotas other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageQuotas>(other, nameof (other));
      this.maxPartsPerBatch = other.maxPartsPerBatch;
      this.maxOperationsPerChangeset = other.maxOperationsPerChangeset;
      this.maxNestingDepth = other.maxNestingDepth;
      this.maxReceivedMessageSize = other.maxReceivedMessageSize;
    }

    public int MaxPartsPerBatch
    {
      get => this.maxPartsPerBatch;
      set
      {
        ExceptionUtils.CheckIntegerNotNegative(value, nameof (MaxPartsPerBatch));
        this.maxPartsPerBatch = value;
      }
    }

    public int MaxOperationsPerChangeset
    {
      get => this.maxOperationsPerChangeset;
      set
      {
        ExceptionUtils.CheckIntegerNotNegative(value, nameof (MaxOperationsPerChangeset));
        this.maxOperationsPerChangeset = value;
      }
    }

    public int MaxNestingDepth
    {
      get => this.maxNestingDepth;
      set
      {
        ExceptionUtils.CheckIntegerPositive(value, nameof (MaxNestingDepth));
        this.maxNestingDepth = value;
      }
    }

    public long MaxReceivedMessageSize
    {
      get => this.maxReceivedMessageSize;
      set
      {
        ExceptionUtils.CheckLongPositive(value, nameof (MaxReceivedMessageSize));
        this.maxReceivedMessageSize = value;
      }
    }
  }
}
