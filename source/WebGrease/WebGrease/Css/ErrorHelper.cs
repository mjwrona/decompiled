// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ErrorHelper
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebGrease.Css
{
  internal static class ErrorHelper
  {
    internal static IEnumerable<string> DedupeCSSErrors(this AggregateException aggEx)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (Exception innerException in aggEx.InnerExceptions)
      {
        if (innerException is RecognitionException recognitionException)
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1}): run-time error CSS1000: {2}", new object[3]
          {
            (object) recognitionException.Line,
            (object) recognitionException.CharPositionInLine,
            (object) recognitionException.Message
          });
          stringSet.Add(str);
        }
      }
      return (IEnumerable<string>) stringSet;
    }

    internal static IEnumerable<BuildWorkflowException> CreateBuildErrors(
      this AggregateException aggEx,
      string fileName)
    {
      return aggEx.InnerExceptions.OfType<RecognitionException>().CreateBuildErrors(fileName);
    }

    internal static IEnumerable<BuildWorkflowException> CreateBuildErrors(
      this IEnumerable<RecognitionException> exceptions,
      string fileName)
    {
      return exceptions.Where<RecognitionException>((Func<RecognitionException, bool>) (ex => ex != null)).Distinct<RecognitionException>((IEqualityComparer<RecognitionException>) new ErrorHelper.ErrorDeduper()).Select<RecognitionException, BuildWorkflowException>((Func<RecognitionException, BuildWorkflowException>) (ex => new BuildWorkflowException(ex.Message, "CSS", "CSS1000", (string) null, fileName, ex.Line, ex.CharPositionInLine, 0, 0, (Exception) ex)));
    }

    private class ErrorDeduper : IEqualityComparer<RecognitionException>
    {
      public bool Equals(RecognitionException x, RecognitionException y) => x.Line == y.Line && x.CharPositionInLine == y.CharPositionInLine && x.Message == y.Message;

      public int GetHashCode(RecognitionException obj) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", new object[3]
      {
        (object) obj.Line,
        (object) obj.CharPositionInLine,
        (object) obj.Message
      }).GetHashCode();
    }
  }
}
