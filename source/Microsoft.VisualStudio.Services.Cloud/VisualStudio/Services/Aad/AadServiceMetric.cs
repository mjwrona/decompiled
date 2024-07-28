// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServiceMetric
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Aad
{
  internal class AadServiceMetric : TimedCiEvent
  {
    internal AadServiceMetric(IVssRequestContext context, [CallerMemberName] string operation = "")
      : base(context, "VisualStudio.Services.Aad", operation)
    {
      this.Result = AadServiceMetric.ResultValue.Failure;
    }

    internal AadServiceMetric.ResultValue Result
    {
      set => this[nameof (Result)] = (object) EnumUtility.ToString<AadServiceMetric.ResultValue>(value);
    }

    internal AadServiceMetric.CauseValue Cause
    {
      set => this[nameof (Cause)] = (object) EnumUtility.ToString<AadServiceMetric.CauseValue>(value);
    }

    internal enum Key
    {
      Result,
      Cause,
    }

    internal enum ResultValue
    {
      Success,
      Error,
      Failure,
    }

    internal enum CauseValue
    {
      Authentication,
      ObjectNotFound,
    }
  }
}
