// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.FieldOperatorInfoComparer
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;

namespace Microsoft.AzureAd.Icm.Types
{
  public class FieldOperatorInfoComparer : IEqualityComparer<FieldOperatorInfo>
  {
    public bool Equals(FieldOperatorInfo x, FieldOperatorInfo y) => x.FieldId == y.FieldId && x.OperatorId == y.OperatorId;

    public int GetHashCode(FieldOperatorInfo obj)
    {
      ArgumentCheck.ThrowIfNull((object) obj, "FieldOperatorInfo object is null", nameof (GetHashCode), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\CustomSearch\\FieldOperatorInfoComparer.cs");
      return (int) (((int) obj.FieldId << 10) + obj.OperatorId);
    }
  }
}
