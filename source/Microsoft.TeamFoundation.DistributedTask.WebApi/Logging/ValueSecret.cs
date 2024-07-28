// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Logging.ValueSecret
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Logging
{
  internal sealed class ValueSecret : ISecret
  {
    internal readonly string m_value;

    public ValueSecret(string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      this.m_value = value;
    }

    public override bool Equals(object obj) => obj is ValueSecret valueSecret && string.Equals(this.m_value, valueSecret.m_value, StringComparison.Ordinal);

    public override int GetHashCode() => this.m_value.GetHashCode();

    public IEnumerable<ReplacementPosition> GetPositions(string input)
    {
      if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(this.m_value))
      {
        int startIndex = 0;
        while (startIndex > -1 && startIndex < input.Length && input.Length - startIndex >= this.m_value.Length)
        {
          startIndex = input.IndexOf(this.m_value, startIndex, StringComparison.Ordinal);
          if (startIndex > -1)
          {
            yield return new ReplacementPosition(startIndex, this.m_value.Length);
            ++startIndex;
          }
        }
      }
    }
  }
}
