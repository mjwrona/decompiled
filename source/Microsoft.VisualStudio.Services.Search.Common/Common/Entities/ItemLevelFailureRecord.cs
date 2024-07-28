// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ItemLevelFailureRecord
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class ItemLevelFailureRecord : ICloneable
  {
    public long Id { get; set; }

    public string Item { get; set; }

    public int AttemptCount { get; set; } = 1;

    public string Stage { get; set; }

    public string Reason { get; set; }

    public FailureMetadata Metadata { get; set; }

    public RejectionCode RejectionCode { get; set; }

    public object Clone() => Serializers.FromJsonString(Serializers.ToJsonString((object) this, this.GetType()), this.GetType());

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(nameof (ItemLevelFailureRecord));
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create(" [Id : {0},  Item: {1}, AttemptCount : {2}, Reason : {3}, FailureMetadata : {4}, RejectionCode : {5} ]", (object) this.Id, (object) this.Item, (object) this.AttemptCount, (object) this.Reason, (object) this.Metadata, (object) this.RejectionCode)));
      return stringBuilder.ToString();
    }
  }
}
