// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.Cursor
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Pegasus.Common
{
  [DebuggerDisplay("({Line},{Column})")]
  [Serializable]
  public class Cursor : IEquatable<Cursor>
  {
    public const string LexicalElementsKey = "_lexical";
    private static int previousStateKey = -1;
    private readonly bool inTransition;
    private readonly bool mutable;
    private readonly IDictionary<string, object> state;
    private int stateKey;

    public Cursor(string subject, int location = 0, string fileName = null)
    {
      if (subject == null)
        throw new ArgumentNullException(nameof (subject));
      if (location < 0 || location > subject.Length)
        throw new ArgumentOutOfRangeException(nameof (location));
      this.Subject = subject;
      this.Location = location;
      this.FileName = fileName;
      int line = 1;
      int column = 1;
      bool inTransition = false;
      Cursor.TrackLines(this.Subject, 0, location, ref line, ref column, ref inTransition);
      this.Line = line;
      this.Column = column;
      this.inTransition = inTransition;
      this.state = (IDictionary<string, object>) new Dictionary<string, object>();
      this.stateKey = Cursor.GetNextStateKey();
      this.mutable = false;
    }

    private Cursor(
      string subject,
      int location,
      string fileName,
      int line,
      int column,
      bool inTransition,
      IDictionary<string, object> state,
      int stateKey,
      bool mutable)
    {
      this.Subject = subject;
      this.Location = location;
      this.FileName = fileName;
      this.Line = line;
      this.Column = column;
      this.inTransition = inTransition;
      this.state = state;
      this.stateKey = stateKey;
      this.mutable = mutable;
    }

    public int Column { get; }

    public string FileName { get; }

    public int Line { get; }

    public int Location { get; }

    public int StateKey => this.stateKey;

    public string Subject { get; }

    public object this[string key]
    {
      get
      {
        object obj;
        this.state.TryGetValue(key, out obj);
        return obj;
      }
      set
      {
        if (!this.mutable)
          throw new InvalidOperationException();
        this.stateKey = Cursor.GetNextStateKey();
        this.state[key] = value;
      }
    }

    public static bool operator !=(Cursor left, Cursor right) => !object.Equals((object) left, (object) right);

    public static bool operator ==(Cursor left, Cursor right) => object.Equals((object) left, (object) right);

    public Cursor Advance(int count)
    {
      if (this.mutable)
        throw new InvalidOperationException();
      int line = this.Line;
      int column = this.Column;
      bool inTransition = this.inTransition;
      Cursor.TrackLines(this.Subject, this.Location, count, ref line, ref column, ref inTransition);
      return new Cursor(this.Subject, this.Location + count, this.FileName, line, column, inTransition, this.state, this.stateKey, this.mutable);
    }

    public override bool Equals(object obj) => this.Equals(obj as Cursor);

    public bool Equals(Cursor other) => (object) other != null && this.Location == other.Location && this.Subject == other.Subject && this.FileName == other.FileName && this.stateKey == other.stateKey;

    public override int GetHashCode() => (((1374496523 * -626349353 + this.Subject.GetHashCode()) * -626349353 + this.Location.GetHashCode()) * -626349353 + (this.FileName == null ? 0 : this.FileName.GetHashCode())) * -626349353 + this.stateKey;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method is too expensive to be a property.")]
    public IList<LexicalElement> GetLexicalElements()
    {
      List<LexicalElement> list = (this["_lexical"] as ListNode<LexicalElement>).ToList<LexicalElement>();
      list.Reverse();
      return (IList<LexicalElement>) new ReadOnlyCollection<LexicalElement>((IList<LexicalElement>) list);
    }

    public Cursor Touch() => new Cursor(this.Subject, this.Location, this.FileName, this.Line, this.Column, this.inTransition, this.mutable ? (IDictionary<string, object>) new Dictionary<string, object>(this.state) : this.state, Cursor.GetNextStateKey(), this.mutable);

    public Cursor WithMutability(bool mutable) => mutable || this.mutable ? new Cursor(this.Subject, this.Location, this.FileName, this.Line, this.Column, this.inTransition, (IDictionary<string, object>) new Dictionary<string, object>(this.state), this.stateKey, mutable) : this;

    private static int GetNextStateKey() => Interlocked.Increment(ref Cursor.previousStateKey);

    private static void TrackLines(
      string subject,
      int start,
      int count,
      ref int line,
      ref int column,
      ref bool inTransition)
    {
      if (count == 0)
        return;
      for (int index = 0; index < count; ++index)
      {
        char ch1 = subject[start + index];
        switch (ch1)
        {
          case '\n':
          case '\r':
            if (inTransition)
            {
              inTransition = false;
              ++line;
              column = 1;
              break;
            }
            if (subject.Length <= start + index + 1)
            {
              ++line;
              column = 1;
              break;
            }
            char ch2 = subject[start + index + 1];
            if (ch1 == '\r' && ch2 == '\n' || ch1 == '\n' && ch2 == '\r')
            {
              inTransition = true;
              ++column;
              break;
            }
            ++line;
            column = 1;
            break;
          case '\u2028':
          case '\u2029':
            ++line;
            column = 1;
            break;
          default:
            ++column;
            break;
        }
      }
    }
  }
}
