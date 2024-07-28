// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FlatStore`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FlatStore<T> : ITokenStore<T>
  {
    public readonly StringComparison TokenComparison;
    private readonly List<FlatStore<T>.FlatStoreEntry> m_entries;
    private bool m_sealed;

    public FlatStore(StringComparison tokenComparison)
    {
      this.TokenComparison = tokenComparison;
      this.m_entries = new List<FlatStore<T>.FlatStoreEntry>();
      this.m_sealed = false;
    }

    private FlatStore(FlatStore<T> source)
    {
      source.CheckSealStatus(true);
      this.TokenComparison = source.TokenComparison;
      this.m_entries = new List<FlatStore<T>.FlatStoreEntry>((IEnumerable<FlatStore<T>.FlatStoreEntry>) source.m_entries);
    }

    public int Count => this.m_entries.Count;

    public bool TryGetValue(string token, out T referencedObject)
    {
      int index = this.Seek(token);
      if (index >= 0)
      {
        referencedObject = this.m_entries[index].ReferencedObject;
        return true;
      }
      referencedObject = default (T);
      return false;
    }

    public T GetOrAdd<X>(string token, Func<X, T> valueFactory, X valueFactoryParameter = null)
    {
      int index = this.Seek(token);
      T orAdd;
      if (index >= 0)
      {
        orAdd = this.m_entries[index].ReferencedObject;
      }
      else
      {
        this.CheckSealStatus(false);
        orAdd = valueFactory(valueFactoryParameter);
        this.m_entries.Insert(~index, new FlatStore<T>.FlatStoreEntry(token, orAdd));
      }
      return orAdd;
    }

    public bool HasSubItem(string token) => this.Seek(token) >= 0;

    public bool Remove(string token, bool recurse)
    {
      int index = this.Seek(token);
      if (index < 0)
        return false;
      this.CheckSealStatus(false);
      this.m_entries.RemoveAt(index);
      return true;
    }

    public T this[string token]
    {
      get
      {
        int index = this.Seek(token);
        if (index < 0)
          throw new KeyNotFoundException();
        return this.m_entries[index].ReferencedObject;
      }
      set
      {
        this.CheckSealStatus(false);
        int index = this.Seek(token);
        if (index < 0)
          this.m_entries.Insert(~index, new FlatStore<T>.FlatStoreEntry(token, value));
        else
          this.m_entries[index] = new FlatStore<T>.FlatStoreEntry(this.m_entries[index].Token, value);
      }
    }

    public ITokenStore<T> Copy(IVssRequestContext requestContext) => (ITokenStore<T>) new FlatStore<T>(this);

    public void Clear()
    {
      this.CheckSealStatus(false);
      this.m_entries.Clear();
    }

    public IEnumerable<T> EnumSubTree(string token, bool enumSubTreeRoot)
    {
      if (token == null)
      {
        foreach (FlatStore<T>.FlatStoreEntry entry in this.m_entries)
          yield return entry.ReferencedObject;
      }
      else
      {
        T referencedObject;
        if (enumSubTreeRoot && this.TryGetValue(token, out referencedObject))
          yield return referencedObject;
      }
    }

    public void EnumAndEvaluateParents(
      string token,
      bool includeSparseNodes,
      Func<string, T, string, bool, bool> evaluate)
    {
      T referencedObject;
      if (this.TryGetValue(token, out referencedObject))
      {
        int num1 = evaluate(token, referencedObject, token, true) ? 1 : 0;
      }
      else
      {
        if (!includeSparseNodes)
          return;
        int num2 = evaluate(token, default (T), token, true) ? 1 : 0;
      }
    }

    public bool IsSubItem(string token, string parentToken) => string.Equals(token, parentToken, this.TokenComparison);

    private int Seek(string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      int num1 = 0;
      int num2 = this.m_entries.Count - 1;
      while (num1 <= num2)
      {
        int index = num1 + (num2 - num1 >> 1);
        int num3 = string.Compare(this.m_entries[index].Token, token, this.TokenComparison);
        if (num3 == 0)
          return index;
        if (num3 < 0)
          num1 = index + 1;
        else
          num2 = index - 1;
      }
      return ~num1;
    }

    public void Seal() => this.m_sealed = true;

    private void CheckSealStatus(bool expectedStatus)
    {
      if (this.m_sealed != expectedStatus)
        throw new InvalidOperationException(expectedStatus ? "Source FlatStore needs to be sealed before doing a copy." : "It is illegal to change the source FlatStore directly once it's sealed.");
    }

    private struct FlatStoreEntry
    {
      public readonly string Token;
      public readonly T ReferencedObject;

      public FlatStoreEntry(string token, T value)
      {
        ArgumentUtility.CheckForNull<string>(token, nameof (token));
        this.Token = token;
        this.ReferencedObject = value;
      }
    }
  }
}
