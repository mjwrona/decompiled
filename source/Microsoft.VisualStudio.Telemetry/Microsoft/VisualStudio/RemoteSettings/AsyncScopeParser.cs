// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.AsyncScopeParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class AsyncScopeParser
  {
    private readonly string expression;
    private readonly Stack<AsyncScopeParser.AsyncOperand> output = new Stack<AsyncScopeParser.AsyncOperand>();
    private readonly Stack<AsyncScopeParser.Operator> operators = new Stack<AsyncScopeParser.Operator>();
    private readonly IDictionary<string, IScopeFilterProvider> providedFilters;
    private readonly Regex stringRegex = new Regex("'(.*)?'");
    private int expressionIndex;

    internal AsyncScopeParser(
      string expression,
      IDictionary<string, IScopeFilterProvider> providedFilters)
    {
      expression.RequiresArgumentNotNull<string>(nameof (expression));
      providedFilters.RequiresArgumentNotNull<IDictionary<string, IScopeFilterProvider>>(nameof (providedFilters));
      this.expression = expression;
      this.providedFilters = providedFilters;
    }

    public async Task<bool> RunAsync()
    {
      while (this.expressionIndex < this.expression.Length)
      {
        switch (this.expression[this.expressionIndex])
        {
          case '\t':
          case ' ':
            ++this.expressionIndex;
            continue;
          case '!':
          case '&':
          case '(':
          case ')':
          case '<':
          case '=':
          case '>':
          case '|':
            await this.ParseOperatorAsync(this.ScanOperator());
            continue;
          default:
            this.ParseOperand(this.ScanOperand());
            continue;
        }
      }
      while (this.operators.Count != 0)
        await this.EvaluateOutputAsync().ConfigureAwait(false);
      if (this.output.Count != 1)
        throw new ScopeParserException("Operand and operator count mismatch");
      return await this.output.Peek().ToBoolAsync().ConfigureAwait(false);
    }

    private async Task ParseOperatorAsync(AsyncScopeParser.Operator op)
    {
      if (op.IdValue == AsyncScopeParser.Operator.Id.Opn)
        this.operators.Push(op);
      else if (op.IdValue == AsyncScopeParser.Operator.Id.Cls)
      {
        while (this.operators.Count != 0 && this.operators.Peek().IdValue != AsyncScopeParser.Operator.Id.Opn)
          await this.EvaluateOutputAsync().ConfigureAwait(false);
        if (this.operators.Count == 0)
          return;
        this.operators.Pop();
      }
      else
      {
        while (this.operators.Count != 0 && ((int) op.Priority < (int) this.operators.Peek().Priority || (int) op.Priority == (int) this.operators.Peek().Priority && op.FixityValue == AsyncScopeParser.Operator.Fixity.Left))
          await this.EvaluateOutputAsync().ConfigureAwait(false);
        this.operators.Push(op);
      }
    }

    private void ParseOperand(AsyncScopeParser.AsyncOperand op) => this.output.Push(op);

    private AsyncScopeParser.Operator ScanOperator()
    {
      string key = this.expression[this.expressionIndex++].ToString();
      if (this.expressionIndex < this.expression.Length)
      {
        char ch = this.expression[this.expressionIndex];
        switch (ch)
        {
          case '&':
          case '=':
          case '|':
            key += ch.ToString();
            ++this.expressionIndex;
            break;
        }
      }
      AsyncScopeParser.Operator @operator;
      if (!AsyncScopeParser.Operator.StrMap.TryGetValue(key, out @operator))
        throw new ScopeParserException("Invalid operator: " + key);
      return @operator;
    }

    private AsyncScopeParser.AsyncOperand ScanOperand()
    {
      Func<char, bool> isAlphabet = (Func<char, bool>) (c =>
      {
        if (c >= 'A' && c <= 'Z')
          return true;
        return c >= 'a' && c <= 'z';
      });
      Func<char, bool> isNumeral = (Func<char, bool>) (n => n >= '0' && n <= '9');
      Func<char, bool> isSpecial = (Func<char, bool>) (c => c == '.' || c == '_' || c == '\'');
      Func<char, bool> predicate = (Func<char, bool>) (c => isAlphabet(c) || isNumeral(c) || isSpecial(c));
      string str = new string(this.expression.Skip<char>(this.expressionIndex).TakeWhile<char>(predicate).ToArray<char>());
      this.expressionIndex += str.Length;
      if (str.Length == 0)
        throw new ScopeParserException("Missing operand: " + this.GetRestOfExpression());
      double result;
      if (double.TryParse(str, out result))
        return (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.DoubleOperand(result);
      Match match = this.stringRegex.Match(str);
      if (match.Success)
        return (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.StringOperand(match.Groups[1].Value);
      string subkey = (string) null;
      string[] strArray = str.Split('.');
      string key;
      if (strArray.Length == 2)
      {
        key = strArray[0];
        subkey = strArray[1];
      }
      else
        key = str;
      IScopeFilterProvider provider;
      if (this.providedFilters.TryGetValue(key, out provider))
      {
        ISingleValueScopeFilterProvider<ScopeValue> singleValue = provider as ISingleValueScopeFilterProvider<ScopeValue>;
        if (singleValue != null)
        {
          if (subkey != null)
            throw new ScopeParserException("Filter has subkey, but only single-value provider");
          return (AsyncScopeParser.AsyncOperand) AsyncScopeParser.CastAppropiateSingle(provider) ?? (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.LazyOperand((Func<AsyncScopeParser.AsyncOperand>) (() => singleValue.Provide().GetAsyncOperand()));
        }
        IMultiValueScopeFilterProvider<ScopeValue> multiValue = provider as IMultiValueScopeFilterProvider<ScopeValue>;
        if (multiValue != null)
        {
          if (subkey == null)
            throw new ScopeParserException("Filter has no subkey, yet multi-value provider");
          return (AsyncScopeParser.AsyncOperand) AsyncScopeParser.CastAppropiateMulti(provider, subkey) ?? (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.LazyOperand((Func<AsyncScopeParser.AsyncOperand>) (() => multiValue.Provide(subkey).GetAsyncOperand()));
        }
      }
      return (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.InvalidOperand("Could not find provided scope with name: " + key);
    }

    private async Task EvaluateOutputAsync()
    {
      AsyncScopeParser.Operator @operator = this.operators.Count != 0 ? this.operators.Pop() : throw new ScopeParserException("Missing operator(s): " + this.GetRestOfExpression());
      Stack<AsyncScopeParser.AsyncOperand> asyncOperandStack;
      if (@operator.TypeValue == AsyncScopeParser.Operator.Type.Unary && this.output.Count >= 1)
      {
        AsyncScopeParser.AsyncOperand asyncOperand = this.output.Pop();
        asyncOperandStack = this.output;
        asyncOperandStack.Push(await @operator.TheFuncDelegateAsync(asyncOperand, asyncOperand));
        asyncOperandStack = (Stack<AsyncScopeParser.AsyncOperand>) null;
      }
      else
      {
        if (@operator.TypeValue != AsyncScopeParser.Operator.Type.Binary || this.output.Count < 2)
          throw new ScopeParserException("Missing operand(s): " + this.GetRestOfExpression());
        AsyncScopeParser.AsyncOperand b = this.output.Pop();
        AsyncScopeParser.AsyncOperand a = this.output.Pop();
        asyncOperandStack = this.output;
        asyncOperandStack.Push(await @operator.TheFuncDelegateAsync(a, b));
        asyncOperandStack = (Stack<AsyncScopeParser.AsyncOperand>) null;
      }
    }

    private string GetRestOfExpression() => this.expression.Substring(this.expressionIndex);

    private static AsyncScopeParser.LazyOperand CastAppropiateSingle(IScopeFilterProvider provider)
    {
      ISingleValueScopeFilterAsyncProvider<BoolScopeValue> result = provider as ISingleValueScopeFilterAsyncProvider<BoolScopeValue>;
      if (result != null)
        return new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () => (await result.ProvideAsync().ConfigureAwait(false)).GetAsyncOperand()));
      ISingleValueScopeFilterAsyncProvider<DoubleScopeValue> result2 = provider as ISingleValueScopeFilterAsyncProvider<DoubleScopeValue>;
      if (result2 != null)
        return new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () => (await result2.ProvideAsync().ConfigureAwait(false)).GetAsyncOperand()));
      ISingleValueScopeFilterAsyncProvider<StringScopeValue> result3 = provider as ISingleValueScopeFilterAsyncProvider<StringScopeValue>;
      return result3 != null ? new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () => (await result3.ProvideAsync().ConfigureAwait(false)).GetAsyncOperand())) : (AsyncScopeParser.LazyOperand) null;
    }

    private static AsyncScopeParser.LazyOperand CastAppropiateMulti(
      IScopeFilterProvider provider,
      string subkey)
    {
      IMultiValueScopeFilterAsyncProvider<BoolScopeValue> result = provider as IMultiValueScopeFilterAsyncProvider<BoolScopeValue>;
      if (result != null)
        return new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () => (await result.ProvideAsync(subkey).ConfigureAwait(false)).GetAsyncOperand()));
      IMultiValueScopeFilterAsyncProvider<DoubleScopeValue> result2 = provider as IMultiValueScopeFilterAsyncProvider<DoubleScopeValue>;
      if (result2 != null)
        return new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () => (await result2.ProvideAsync(subkey).ConfigureAwait(false)).GetAsyncOperand()));
      IMultiValueScopeFilterAsyncProvider<StringScopeValue> result3 = provider as IMultiValueScopeFilterAsyncProvider<StringScopeValue>;
      return result3 != null ? new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () => (await result3.ProvideAsync(subkey).ConfigureAwait(false)).GetAsyncOperand())) : (AsyncScopeParser.LazyOperand) null;
    }

    internal class InvalidOperand : AsyncScopeParser.AsyncOperand
    {
      private string errorMessage;

      public InvalidOperand(string errorMessage) => this.errorMessage = errorMessage;

      public override Task<bool> ToBoolAsync() => throw new ScopeParserException(this.errorMessage);

      public override Task<int> CompareToAsync(AsyncScopeParser.AsyncOperand a) => throw new ScopeParserException(this.errorMessage);
    }

    internal class BoolOperand : AsyncScopeParser.AsyncOperand
    {
      public BoolOperand(bool value) => this.value = Task.FromResult<object>((object) value);

      public override async Task<bool> ToBoolAsync() => (bool) await this.Value().ConfigureAwait(false);

      public override async Task<int> CompareToAsync(AsyncScopeParser.AsyncOperand a)
      {
        AsyncScopeParser.BoolOperand boolOperand = this;
        object aValue = await a.Value().ConfigureAwait(false);
        if (!(aValue is bool))
          throw new ScopeParserException("Trying to compare two different types");
        int async = ((bool) await boolOperand.Value().ConfigureAwait(false)).CompareTo((bool) aValue);
        aValue = (object) null;
        return async;
      }
    }

    internal class LazyOperand : AsyncScopeParser.AsyncOperand
    {
      private Lazy<Task<AsyncScopeParser.AsyncOperand>> lazy;

      public override async Task<object> Value() => await (await this.lazy.Value.ConfigureAwait(false)).Value().ConfigureAwait(false);

      public LazyOperand(Func<Task<AsyncScopeParser.AsyncOperand>> value) => this.lazy = new Lazy<Task<AsyncScopeParser.AsyncOperand>>(value);

      public LazyOperand(Func<AsyncScopeParser.AsyncOperand> value) => this.lazy = new Lazy<Task<AsyncScopeParser.AsyncOperand>>((Func<Task<AsyncScopeParser.AsyncOperand>>) (() => Task.FromResult<AsyncScopeParser.AsyncOperand>(value())));

      public override async Task<int> CompareToAsync(AsyncScopeParser.AsyncOperand a) => await (await this.lazy.Value).CompareToAsync(a).ConfigureAwait(false);

      public override async Task<bool> ToBoolAsync() => await (await this.lazy.Value).ToBoolAsync().ConfigureAwait(false);
    }

    internal class StringOperand : AsyncScopeParser.AsyncOperand
    {
      public StringOperand(string value) => this.value = Task.FromResult<object>((object) value);

      public override async Task<bool> ToBoolAsync() => await this.Value().ConfigureAwait(false) != null;

      public override async Task<int> CompareToAsync(AsyncScopeParser.AsyncOperand a)
      {
        AsyncScopeParser.StringOperand stringOperand = this;
        object aValue = await a.Value().ConfigureAwait(false);
        if (!(aValue is string))
          throw new ScopeParserException("Trying to compare two different types");
        int async = ((string) await stringOperand.Value().ConfigureAwait(false)).CompareTo((string) aValue);
        aValue = (object) null;
        return async;
      }
    }

    internal class DoubleOperand : AsyncScopeParser.AsyncOperand
    {
      public DoubleOperand(double value) => this.value = Task.FromResult<object>((object) value);

      public override async Task<bool> ToBoolAsync() => (double) await this.Value().ConfigureAwait(false) != 0.0;

      public override async Task<int> CompareToAsync(AsyncScopeParser.AsyncOperand a)
      {
        AsyncScopeParser.DoubleOperand doubleOperand = this;
        object aValue = await a.Value().ConfigureAwait(false);
        if (!(a is AsyncScopeParser.DoubleOperand))
          throw new ScopeParserException("Trying to compare two different types");
        int async = ((double) await doubleOperand.Value().ConfigureAwait(false)).CompareTo(aValue);
        aValue = (object) null;
        return async;
      }
    }

    internal abstract class AsyncOperand
    {
      protected Task<object> value;

      public virtual Task<object> Value() => this.value;

      public abstract Task<int> CompareToAsync(AsyncScopeParser.AsyncOperand a);

      public async Task<bool> EqualsAsync(AsyncScopeParser.AsyncOperand y) => await this.CompareToAsync(y).ConfigureAwait(false) == 0;

      public abstract Task<bool> ToBoolAsync();

      public static AsyncScopeParser.AsyncOperand AndAsync(
        AsyncScopeParser.AsyncOperand a,
        AsyncScopeParser.AsyncOperand b)
      {
        return (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () =>
        {
          if (await a.ToBoolAsync().ConfigureAwait(false))
          {
            if (await b.ToBoolAsync().ConfigureAwait(false))
              return (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(true);
          }
          return (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(false);
        }));
      }

      public static AsyncScopeParser.AsyncOperand OrAsync(
        AsyncScopeParser.AsyncOperand a,
        AsyncScopeParser.AsyncOperand b)
      {
        return (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.LazyOperand((Func<Task<AsyncScopeParser.AsyncOperand>>) (async () => await a.ToBoolAsync().ConfigureAwait(false) ? (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(true) : (!await b.ToBoolAsync().ConfigureAwait(false) ? (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(false) : (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(true))));
      }

      public static async Task<AsyncScopeParser.AsyncOperand> Not(AsyncScopeParser.AsyncOperand a) => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(!await a.ToBoolAsync().ConfigureAwait(false));
    }

    internal struct Operator
    {
      internal AsyncScopeParser.Operator.Id IdValue;
      internal ushort Priority;
      internal AsyncScopeParser.Operator.Fixity FixityValue;
      internal AsyncScopeParser.Operator.Type TypeValue;
      internal AsyncScopeParser.Operator.FuncDelegateAsync TheFuncDelegateAsync;
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Non = (AsyncScopeParser.Operator.FuncDelegateAsync) ((a, b) => Task.FromResult<AsyncScopeParser.AsyncOperand>((AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(false)));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Not = (AsyncScopeParser.Operator.FuncDelegateAsync) ((a, b) => AsyncScopeParser.AsyncOperand.Not(a));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Gt = (AsyncScopeParser.Operator.FuncDelegateAsync) (async (a, b) => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(await a.CompareToAsync(b).ConfigureAwait(false) > 0));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Lt = (AsyncScopeParser.Operator.FuncDelegateAsync) (async (a, b) => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(await a.CompareToAsync(b).ConfigureAwait(false) < 0));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Gte = (AsyncScopeParser.Operator.FuncDelegateAsync) (async (a, b) => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(await a.CompareToAsync(b).ConfigureAwait(false) >= 0));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Lte = (AsyncScopeParser.Operator.FuncDelegateAsync) (async (a, b) => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(await a.CompareToAsync(b).ConfigureAwait(false) <= 0));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Eq = (AsyncScopeParser.Operator.FuncDelegateAsync) (async (a, b) => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(await a.EqualsAsync(b).ConfigureAwait(false)));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Neq = (AsyncScopeParser.Operator.FuncDelegateAsync) (async (a, b) => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(!await a.EqualsAsync(b).ConfigureAwait(false)));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync And = (AsyncScopeParser.Operator.FuncDelegateAsync) ((a, b) => Task.FromResult<AsyncScopeParser.AsyncOperand>(AsyncScopeParser.AsyncOperand.AndAsync(a, b)));
      internal static readonly AsyncScopeParser.Operator.FuncDelegateAsync Or = (AsyncScopeParser.Operator.FuncDelegateAsync) ((a, b) => Task.FromResult<AsyncScopeParser.AsyncOperand>(AsyncScopeParser.AsyncOperand.OrAsync(a, b)));
      internal static readonly IDictionary<string, AsyncScopeParser.Operator> StrMap = (IDictionary<string, AsyncScopeParser.Operator>) new Dictionary<string, AsyncScopeParser.Operator>()
      {
        {
          "(",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Opn,
            Priority = (ushort) 0,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Unary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Non
          }
        },
        {
          ")",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Cls,
            Priority = (ushort) 0,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Unary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Non
          }
        },
        {
          "!",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Not,
            Priority = (ushort) 80,
            FixityValue = AsyncScopeParser.Operator.Fixity.Right,
            TypeValue = AsyncScopeParser.Operator.Type.Unary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Not
          }
        },
        {
          ">",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Gt,
            Priority = (ushort) 64,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Gt
          }
        },
        {
          "<",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Lt,
            Priority = (ushort) 64,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Lt
          }
        },
        {
          ">=",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Gte,
            Priority = (ushort) 64,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Gte
          }
        },
        {
          "<=",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Lte,
            Priority = (ushort) 64,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Lte
          }
        },
        {
          "==",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Eq,
            Priority = (ushort) 48,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Eq
          }
        },
        {
          "!=",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Neq,
            Priority = (ushort) 48,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Neq
          }
        },
        {
          "&&",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.And,
            Priority = (ushort) 32,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.And
          }
        },
        {
          "||",
          new AsyncScopeParser.Operator()
          {
            IdValue = AsyncScopeParser.Operator.Id.Or,
            Priority = (ushort) 16,
            FixityValue = AsyncScopeParser.Operator.Fixity.Left,
            TypeValue = AsyncScopeParser.Operator.Type.Binary,
            TheFuncDelegateAsync = AsyncScopeParser.Operator.Or
          }
        }
      };

      internal delegate Task<AsyncScopeParser.AsyncOperand> FuncDelegateAsync(
        AsyncScopeParser.AsyncOperand a,
        AsyncScopeParser.AsyncOperand b);

      internal enum Id
      {
        Non,
        Opn,
        Cls,
        Not,
        And,
        Or,
        Eq,
        Neq,
        Gt,
        Lt,
        Gte,
        Lte,
      }

      internal enum Fixity
      {
        Left,
        Right,
      }

      internal enum Type
      {
        Unary,
        Binary,
      }
    }
  }
}
