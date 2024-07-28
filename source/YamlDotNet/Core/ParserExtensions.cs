// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.ParserExtensions
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;
using System.IO;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
  public static class ParserExtensions
  {
    public static T Expect<T>(this IParser parser) where T : ParsingEvent
    {
      T obj = parser.Allow<T>();
      if ((object) obj == null)
      {
        ParsingEvent current = parser.Current;
        throw new YamlException(current.Start, current.End, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Expected '{0}', got '{1}' (at {2}).", new object[3]
        {
          (object) typeof (T).Name,
          (object) current.GetType().Name,
          (object) current.Start
        }));
      }
      return obj;
    }

    public static bool Accept<T>(this IParser parser) where T : ParsingEvent
    {
      if (parser.Current == null && !parser.MoveNext())
        throw new EndOfStreamException();
      return parser.Current is T;
    }

    public static T Allow<T>(this IParser parser) where T : ParsingEvent
    {
      if (!parser.Accept<T>())
        return default (T);
      T current = (T) parser.Current;
      parser.MoveNext();
      return current;
    }

    public static T Peek<T>(this IParser parser) where T : ParsingEvent => !parser.Accept<T>() ? default (T) : (T) parser.Current;

    public static void SkipThisAndNestedEvents(this IParser parser)
    {
      int num = 0;
      do
      {
        num += parser.Peek<ParsingEvent>().NestingIncrease;
        parser.MoveNext();
      }
      while (num > 0);
    }
  }
}
