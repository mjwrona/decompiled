// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.StringMinifier
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class StringMinifier : IStringMinifier
  {
    private readonly ConcurrentDictionary<string, string> _stringMinifier = new ConcurrentDictionary<string, string>();
    private readonly ConcurrentDictionary<string, string> _stringMaximizer = new ConcurrentDictionary<string, string>();
    private int _lastMinifiedKey = -1;
    private readonly Func<string, string> _createMinifiedString;

    public StringMinifier() => this._createMinifiedString = new Func<string, string>(this.CreateMinifiedString);

    public string Minify(string fullString) => this._stringMinifier.GetOrAdd(fullString, this._createMinifiedString);

    public string Unminify(string minifiedString)
    {
      string str;
      this._stringMaximizer.TryGetValue(minifiedString, out str);
      return str;
    }

    public void RemoveUnminified(string fullString)
    {
      string key;
      if (!this._stringMinifier.TryRemove(fullString, out key))
        return;
      this._stringMaximizer.TryRemove(key, out string _);
    }

    private string CreateMinifiedString(string fullString)
    {
      string stringFromInt = StringMinifier.GetStringFromInt((uint) Interlocked.Increment(ref this._lastMinifiedKey));
      this._stringMaximizer.TryAdd(stringFromInt, fullString);
      return stringFromInt;
    }

    private static char GetCharFromSixBitInt(uint num)
    {
      if (num < 26U)
        return (char) (num + 65U);
      if (num < 52U)
        return (char) ((int) num - 26 + 97);
      if (num < 62U)
        return (char) ((int) num - 52 + 48);
      if (num == 62U)
        return '_';
      if (num == 63U)
        return ':';
      throw new IndexOutOfRangeException();
    }

    private static string GetStringFromInt(uint num)
    {
      char[] chArray = new char[6];
      int startIndex = 6;
      do
      {
        chArray[--startIndex] = StringMinifier.GetCharFromSixBitInt(num & 63U);
        num >>= 6;
      }
      while (num != 0U);
      return new string(chArray, startIndex, 6 - startIndex);
    }
  }
}
