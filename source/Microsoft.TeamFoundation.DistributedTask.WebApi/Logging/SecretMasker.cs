// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Logging.SecretMasker
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Logging
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SecretMasker : ISecretMasker, IDisposable
  {
    private readonly HashSet<ValueSecret> m_originalValueSecrets;
    private readonly HashSet<RegexSecret> m_regexSecrets;
    private readonly HashSet<ValueEncoder> m_valueEncoders;
    private readonly HashSet<ValueSecret> m_valueSecrets;
    private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

    public SecretMasker()
    {
      this.MinSecretLength = 0;
      this.m_originalValueSecrets = new HashSet<ValueSecret>();
      this.m_regexSecrets = new HashSet<RegexSecret>();
      this.m_valueEncoders = new HashSet<ValueEncoder>();
      this.m_valueSecrets = new HashSet<ValueSecret>();
    }

    public SecretMasker(int minSecretLength)
    {
      this.MinSecretLength = minSecretLength;
      this.m_originalValueSecrets = new HashSet<ValueSecret>();
      this.m_regexSecrets = new HashSet<RegexSecret>();
      this.m_valueEncoders = new HashSet<ValueEncoder>();
      this.m_valueSecrets = new HashSet<ValueSecret>();
    }

    private SecretMasker(SecretMasker copy)
    {
      try
      {
        copy.m_lock.EnterReadLock();
        this.MinSecretLength = copy.MinSecretLength;
        this.m_originalValueSecrets = new HashSet<ValueSecret>((IEnumerable<ValueSecret>) copy.m_originalValueSecrets);
        this.m_regexSecrets = new HashSet<RegexSecret>((IEnumerable<RegexSecret>) copy.m_regexSecrets);
        this.m_valueEncoders = new HashSet<ValueEncoder>((IEnumerable<ValueEncoder>) copy.m_valueEncoders);
        this.m_valueSecrets = new HashSet<ValueSecret>((IEnumerable<ValueSecret>) copy.m_valueSecrets);
      }
      finally
      {
        if (copy.m_lock.IsReadLockHeld)
          copy.m_lock.ExitReadLock();
      }
    }

    public int MinSecretLength { get; set; }

    public void AddRegex(string pattern)
    {
      if (string.IsNullOrEmpty(pattern))
        return;
      if (pattern.Length < this.MinSecretLength)
        return;
      try
      {
        this.m_lock.EnterWriteLock();
        this.m_regexSecrets.Add(new RegexSecret(pattern));
      }
      finally
      {
        if (this.m_lock.IsWriteLockHeld)
          this.m_lock.ExitWriteLock();
      }
    }

    public void AddValue(string value)
    {
      if (string.IsNullOrEmpty(value) || value.Length < this.MinSecretLength)
        return;
      List<ValueSecret> valueSecretList = new List<ValueSecret>((IEnumerable<ValueSecret>) new ValueSecret[1]
      {
        new ValueSecret(value)
      });
      ValueEncoder[] array;
      try
      {
        this.m_lock.EnterReadLock();
        if (this.m_originalValueSecrets.Contains(valueSecretList[0]))
          return;
        array = this.m_valueEncoders.ToArray<ValueEncoder>();
      }
      finally
      {
        if (this.m_lock.IsReadLockHeld)
          this.m_lock.ExitReadLock();
      }
      foreach (ValueEncoder valueEncoder in array)
      {
        string str = valueEncoder(value);
        if (!string.IsNullOrEmpty(str) && str.Length >= this.MinSecretLength)
          valueSecretList.Add(new ValueSecret(str));
      }
      try
      {
        this.m_lock.EnterWriteLock();
        this.m_originalValueSecrets.Add(valueSecretList[0]);
        foreach (ValueSecret valueSecret in valueSecretList)
          this.m_valueSecrets.Add(valueSecret);
      }
      finally
      {
        if (this.m_lock.IsWriteLockHeld)
          this.m_lock.ExitWriteLock();
      }
    }

    public void AddValueEncoder(ValueEncoder encoder)
    {
      ValueSecret[] array;
      try
      {
        this.m_lock.EnterReadLock();
        if (this.m_valueEncoders.Contains(encoder))
          return;
        array = this.m_originalValueSecrets.ToArray<ValueSecret>();
      }
      finally
      {
        if (this.m_lock.IsReadLockHeld)
          this.m_lock.ExitReadLock();
      }
      List<ValueSecret> valueSecretList = new List<ValueSecret>();
      foreach (ValueSecret valueSecret in array)
      {
        string str = encoder(valueSecret.m_value);
        if (!string.IsNullOrEmpty(str) && str.Length >= this.MinSecretLength)
          valueSecretList.Add(new ValueSecret(str));
      }
      try
      {
        this.m_lock.EnterWriteLock();
        this.m_valueEncoders.Add(encoder);
        foreach (ValueSecret valueSecret in valueSecretList)
          this.m_valueSecrets.Add(valueSecret);
      }
      finally
      {
        if (this.m_lock.IsWriteLockHeld)
          this.m_lock.ExitWriteLock();
      }
    }

    public ISecretMasker Clone() => (ISecretMasker) new SecretMasker(this);

    public void Dispose()
    {
      this.m_lock?.Dispose();
      this.m_lock = (ReaderWriterLockSlim) null;
    }

    public string MaskSecrets(string input)
    {
      if (string.IsNullOrEmpty(input))
        return string.Empty;
      List<ReplacementPosition> source = new List<ReplacementPosition>();
      try
      {
        this.m_lock.EnterReadLock();
        foreach (RegexSecret regexSecret in this.m_regexSecrets)
          source.AddRange(regexSecret.GetPositions(input));
        foreach (ValueSecret valueSecret in this.m_valueSecrets)
          source.AddRange(valueSecret.GetPositions(input));
      }
      finally
      {
        if (this.m_lock.IsReadLockHeld)
          this.m_lock.ExitReadLock();
      }
      if (source.Count == 0)
        return input;
      List<ReplacementPosition> replacementPositionList = new List<ReplacementPosition>();
      ReplacementPosition replacementPosition1 = (ReplacementPosition) null;
      foreach (ReplacementPosition copy in (IEnumerable<ReplacementPosition>) source.OrderBy<ReplacementPosition, int>((Func<ReplacementPosition, int>) (x => x.Start)))
      {
        if (replacementPosition1 == null)
        {
          replacementPosition1 = new ReplacementPosition(copy);
          replacementPositionList.Add(replacementPosition1);
        }
        else if (copy.Start <= replacementPosition1.End)
        {
          replacementPosition1.Length = Math.Max(replacementPosition1.End, copy.End) - replacementPosition1.Start;
        }
        else
        {
          replacementPosition1 = new ReplacementPosition(copy);
          replacementPositionList.Add(replacementPosition1);
        }
      }
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      foreach (ReplacementPosition replacementPosition2 in replacementPositionList)
      {
        stringBuilder.Append(input.Substring(startIndex, replacementPosition2.Start - startIndex));
        stringBuilder.Append("***");
        startIndex = replacementPosition2.Start + replacementPosition2.Length;
      }
      if (startIndex < input.Length)
        stringBuilder.Append(input.Substring(startIndex));
      return stringBuilder.ToString();
    }

    public void RemoveShortSecretsFromDictionary()
    {
      HashSet<ValueSecret> valueSecretSet = new HashSet<ValueSecret>();
      HashSet<RegexSecret> regexSecretSet = new HashSet<RegexSecret>();
      try
      {
        this.m_lock.EnterReadLock();
        foreach (ValueSecret valueSecret in this.m_valueSecrets)
        {
          if (valueSecret.m_value.Length < this.MinSecretLength)
            valueSecretSet.Add(valueSecret);
        }
        foreach (RegexSecret regexSecret in this.m_regexSecrets)
        {
          if (regexSecret.Pattern.Length < this.MinSecretLength)
            regexSecretSet.Add(regexSecret);
        }
      }
      finally
      {
        if (this.m_lock.IsReadLockHeld)
          this.m_lock.ExitReadLock();
      }
      try
      {
        this.m_lock.EnterWriteLock();
        foreach (ValueSecret valueSecret in valueSecretSet)
          this.m_valueSecrets.Remove(valueSecret);
        foreach (RegexSecret regexSecret in regexSecretSet)
          this.m_regexSecrets.Remove(regexSecret);
        foreach (ValueSecret valueSecret in valueSecretSet)
          this.m_originalValueSecrets.Remove(valueSecret);
      }
      finally
      {
        if (this.m_lock.IsWriteLockHeld)
          this.m_lock.ExitWriteLock();
      }
    }
  }
}
