// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.PredictionContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime.Atn
{
  internal abstract class PredictionContext
  {
    public static readonly int EMPTY_RETURN_STATE = int.MaxValue;
    public static readonly EmptyPredictionContext EMPTY = new EmptyPredictionContext();
    private static readonly int INITIAL_HASH = 1;
    private readonly int cachedHashCode;

    protected internal static int CalculateEmptyHashCode() => MurmurHash.Finish(MurmurHash.Initialize(PredictionContext.INITIAL_HASH), 0);

    protected internal static int CalculateHashCode(PredictionContext parent, int returnState) => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(PredictionContext.INITIAL_HASH), (object) parent), returnState), 2);

    protected internal static int CalculateHashCode(PredictionContext[] parents, int[] returnStates)
    {
      int hash = MurmurHash.Initialize(PredictionContext.INITIAL_HASH);
      foreach (PredictionContext parent in parents)
        hash = MurmurHash.Update(hash, (object) parent);
      foreach (int returnState in returnStates)
        hash = MurmurHash.Update(hash, returnState);
      return MurmurHash.Finish(hash, 2 * parents.Length);
    }

    protected internal PredictionContext(int cachedHashCode) => this.cachedHashCode = cachedHashCode;

    public static PredictionContext FromRuleContext(ATN atn, RuleContext outerContext)
    {
      if (outerContext == null)
        outerContext = (RuleContext) ParserRuleContext.EMPTY;
      return outerContext.Parent == null || outerContext == ParserRuleContext.EMPTY ? (PredictionContext) PredictionContext.EMPTY : PredictionContext.FromRuleContext(atn, outerContext.Parent).GetChild(((RuleTransition) atn.states[outerContext.invokingState].Transition(0)).followState.stateNumber);
    }

    public abstract int Size { get; }

    public abstract PredictionContext GetParent(int index);

    public abstract int GetReturnState(int index);

    public virtual bool IsEmpty => this == PredictionContext.EMPTY;

    public virtual bool HasEmptyPath => this.GetReturnState(this.Size - 1) == PredictionContext.EMPTY_RETURN_STATE;

    public override sealed int GetHashCode() => this.cachedHashCode;

    internal static PredictionContext Merge(
      PredictionContext a,
      PredictionContext b,
      bool rootIsWildcard,
      MergeCache mergeCache)
    {
      if (a == b || a.Equals((object) b))
        return a;
      if (a is SingletonPredictionContext && b is SingletonPredictionContext)
        return PredictionContext.MergeSingletons((SingletonPredictionContext) a, (SingletonPredictionContext) b, rootIsWildcard, mergeCache);
      if (rootIsWildcard)
      {
        if (a is EmptyPredictionContext)
          return a;
        if (b is EmptyPredictionContext)
          return b;
      }
      if (a is SingletonPredictionContext)
        a = (PredictionContext) new ArrayPredictionContext((SingletonPredictionContext) a);
      if (b is SingletonPredictionContext)
        b = (PredictionContext) new ArrayPredictionContext((SingletonPredictionContext) b);
      return PredictionContext.MergeArrays((ArrayPredictionContext) a, (ArrayPredictionContext) b, rootIsWildcard, mergeCache);
    }

    public static PredictionContext MergeSingletons(
      SingletonPredictionContext a,
      SingletonPredictionContext b,
      bool rootIsWildcard,
      MergeCache mergeCache)
    {
      if (mergeCache != null)
      {
        PredictionContext predictionContext1 = mergeCache.Get((PredictionContext) a, (PredictionContext) b);
        if (predictionContext1 != null)
          return predictionContext1;
        PredictionContext predictionContext2 = mergeCache.Get((PredictionContext) b, (PredictionContext) a);
        if (predictionContext2 != null)
          return predictionContext2;
      }
      PredictionContext predictionContext3 = PredictionContext.MergeRoot(a, b, rootIsWildcard);
      if (predictionContext3 != null)
      {
        mergeCache?.Put((PredictionContext) a, (PredictionContext) b, predictionContext3);
        return predictionContext3;
      }
      if (a.returnState == b.returnState)
      {
        PredictionContext parent = PredictionContext.Merge(a.parent, b.parent, rootIsWildcard, mergeCache);
        if (parent == a.parent)
          return (PredictionContext) a;
        if (parent == b.parent)
          return (PredictionContext) b;
        PredictionContext predictionContext4 = SingletonPredictionContext.Create(parent, a.returnState);
        mergeCache?.Put((PredictionContext) a, (PredictionContext) b, predictionContext4);
        return predictionContext4;
      }
      int[] returnStates = new int[2];
      PredictionContext[] parents = new PredictionContext[2];
      PredictionContext predictionContext5 = (PredictionContext) null;
      if (a == b || a.parent != null && a.parent.Equals((object) b.parent))
        predictionContext5 = a.parent;
      if (predictionContext5 != null)
      {
        if (a.returnState > b.returnState)
        {
          returnStates[0] = b.returnState;
          returnStates[1] = a.returnState;
        }
        else
        {
          returnStates[0] = a.returnState;
          returnStates[1] = b.returnState;
        }
        parents[0] = predictionContext5;
        parents[1] = predictionContext5;
        PredictionContext predictionContext6 = (PredictionContext) new ArrayPredictionContext(parents, returnStates);
        mergeCache?.Put((PredictionContext) a, (PredictionContext) b, predictionContext6);
        return predictionContext6;
      }
      if (a.returnState > b.returnState)
      {
        returnStates[0] = b.returnState;
        returnStates[1] = a.returnState;
        parents[0] = b.parent;
        parents[1] = a.parent;
      }
      else
      {
        returnStates[0] = a.returnState;
        returnStates[1] = b.returnState;
        parents[0] = a.parent;
        parents[1] = b.parent;
      }
      PredictionContext predictionContext7 = (PredictionContext) new ArrayPredictionContext(parents, returnStates);
      mergeCache?.Put((PredictionContext) a, (PredictionContext) b, predictionContext7);
      return predictionContext7;
    }

    public static PredictionContext MergeArrays(
      ArrayPredictionContext a,
      ArrayPredictionContext b,
      bool rootIsWildcard,
      MergeCache mergeCache)
    {
      if (mergeCache != null)
      {
        PredictionContext predictionContext1 = mergeCache.Get((PredictionContext) a, (PredictionContext) b);
        if (predictionContext1 != null)
          return predictionContext1;
        PredictionContext predictionContext2 = mergeCache.Get((PredictionContext) b, (PredictionContext) a);
        if (predictionContext2 != null)
          return predictionContext2;
      }
      int index1 = 0;
      int index2 = 0;
      int newSize = 0;
      int[] numArray = new int[a.returnStates.Length + b.returnStates.Length];
      PredictionContext[] predictionContextArray = new PredictionContext[a.returnStates.Length + b.returnStates.Length];
      while (index1 < a.returnStates.Length && index2 < b.returnStates.Length)
      {
        PredictionContext parent1 = a.parents[index1];
        PredictionContext parent2 = b.parents[index2];
        if (a.returnStates[index1] == b.returnStates[index2])
        {
          int returnState = a.returnStates[index1];
          if (((returnState != PredictionContext.EMPTY_RETURN_STATE || parent1 != null ? 0 : (parent2 == null ? 1 : 0)) | (parent1 == null || parent2 == null ? (false ? 1 : 0) : (parent1.Equals((object) parent2) ? 1 : 0))) != 0)
          {
            predictionContextArray[newSize] = parent1;
            numArray[newSize] = returnState;
          }
          else
          {
            PredictionContext predictionContext = PredictionContext.Merge(parent1, parent2, rootIsWildcard, mergeCache);
            predictionContextArray[newSize] = predictionContext;
            numArray[newSize] = returnState;
          }
          ++index1;
          ++index2;
        }
        else if (a.returnStates[index1] < b.returnStates[index2])
        {
          predictionContextArray[newSize] = parent1;
          numArray[newSize] = a.returnStates[index1];
          ++index1;
        }
        else
        {
          predictionContextArray[newSize] = parent2;
          numArray[newSize] = b.returnStates[index2];
          ++index2;
        }
        ++newSize;
      }
      if (index1 < a.returnStates.Length)
      {
        for (int index3 = index1; index3 < a.returnStates.Length; ++index3)
        {
          predictionContextArray[newSize] = a.parents[index3];
          numArray[newSize] = a.returnStates[index3];
          ++newSize;
        }
      }
      else
      {
        for (int index4 = index2; index4 < b.returnStates.Length; ++index4)
        {
          predictionContextArray[newSize] = b.parents[index4];
          numArray[newSize] = b.returnStates[index4];
          ++newSize;
        }
      }
      if (newSize < predictionContextArray.Length)
      {
        if (newSize == 1)
        {
          PredictionContext predictionContext = SingletonPredictionContext.Create(predictionContextArray[0], numArray[0]);
          mergeCache?.Put((PredictionContext) a, (PredictionContext) b, predictionContext);
          return predictionContext;
        }
        predictionContextArray = Arrays.CopyOf<PredictionContext>(predictionContextArray, newSize);
        numArray = Arrays.CopyOf<int>(numArray, newSize);
      }
      PredictionContext predictionContext3 = (PredictionContext) new ArrayPredictionContext(predictionContextArray, numArray);
      if (predictionContext3.Equals((object) a))
      {
        mergeCache?.Put((PredictionContext) a, (PredictionContext) b, (PredictionContext) a);
        return (PredictionContext) a;
      }
      if (predictionContext3.Equals((object) b))
      {
        mergeCache?.Put((PredictionContext) a, (PredictionContext) b, (PredictionContext) b);
        return (PredictionContext) b;
      }
      PredictionContext.CombineCommonParents(predictionContextArray);
      mergeCache?.Put((PredictionContext) a, (PredictionContext) b, predictionContext3);
      return predictionContext3;
    }

    protected static void CombineCommonParents(PredictionContext[] parents)
    {
      Dictionary<PredictionContext, PredictionContext> dictionary = new Dictionary<PredictionContext, PredictionContext>();
      for (int index = 0; index < parents.Length; ++index)
      {
        PredictionContext parent = parents[index];
        if (parent != null && !dictionary.ContainsKey(parent))
          dictionary.Put<PredictionContext, PredictionContext>(parent, parent);
      }
      for (int index = 0; index < parents.Length; ++index)
      {
        PredictionContext parent = parents[index];
        if (parent != null)
          parents[index] = dictionary.Get<PredictionContext, PredictionContext>(parent);
      }
    }

    public static PredictionContext MergeRoot(
      SingletonPredictionContext a,
      SingletonPredictionContext b,
      bool rootIsWildcard)
    {
      if (rootIsWildcard)
      {
        if (a == PredictionContext.EMPTY)
          return (PredictionContext) PredictionContext.EMPTY;
        if (b == PredictionContext.EMPTY)
          return (PredictionContext) PredictionContext.EMPTY;
      }
      else
      {
        if (a == PredictionContext.EMPTY && b == PredictionContext.EMPTY)
          return (PredictionContext) PredictionContext.EMPTY;
        if (a == PredictionContext.EMPTY)
        {
          int[] returnStates = new int[2]
          {
            b.returnState,
            PredictionContext.EMPTY_RETURN_STATE
          };
          return (PredictionContext) new ArrayPredictionContext(new PredictionContext[2]
          {
            b.parent,
            null
          }, returnStates);
        }
        if (b == PredictionContext.EMPTY)
        {
          int[] returnStates = new int[2]
          {
            a.returnState,
            PredictionContext.EMPTY_RETURN_STATE
          };
          return (PredictionContext) new ArrayPredictionContext(new PredictionContext[2]
          {
            a.parent,
            null
          }, returnStates);
        }
      }
      return (PredictionContext) null;
    }

    public static PredictionContext GetCachedContext(
      PredictionContext context,
      PredictionContextCache contextCache,
      PredictionContext.IdentityHashMap visited)
    {
      if (context.IsEmpty)
        return context;
      PredictionContext cachedContext1 = visited.Get<PredictionContext, PredictionContext>(context);
      if (cachedContext1 != null)
        return cachedContext1;
      PredictionContext cachedContext2 = contextCache.Get(context);
      if (cachedContext2 != null)
      {
        visited.Put<PredictionContext, PredictionContext>(context, cachedContext2);
        return cachedContext2;
      }
      bool flag = false;
      PredictionContext[] parents = new PredictionContext[context.Size];
      for (int index1 = 0; index1 < parents.Length; ++index1)
      {
        PredictionContext cachedContext3 = PredictionContext.GetCachedContext(context.GetParent(index1), contextCache, visited);
        if (flag || cachedContext3 != context.GetParent(index1))
        {
          if (!flag)
          {
            parents = new PredictionContext[context.Size];
            for (int index2 = 0; index2 < context.Size; ++index2)
              parents[index2] = context.GetParent(index2);
            flag = true;
          }
          parents[index1] = cachedContext3;
        }
      }
      if (!flag)
      {
        contextCache.Add(context);
        visited.Put<PredictionContext, PredictionContext>(context, context);
        return context;
      }
      PredictionContext cachedContext4;
      if (parents.Length == 0)
        cachedContext4 = (PredictionContext) PredictionContext.EMPTY;
      else if (parents.Length == 1)
      {
        cachedContext4 = SingletonPredictionContext.Create(parents[0], context.GetReturnState(0));
      }
      else
      {
        ArrayPredictionContext predictionContext = (ArrayPredictionContext) context;
        cachedContext4 = (PredictionContext) new ArrayPredictionContext(parents, predictionContext.returnStates);
      }
      contextCache.Add(cachedContext4);
      visited.Put<PredictionContext, PredictionContext>(cachedContext4, cachedContext4);
      visited.Put<PredictionContext, PredictionContext>(context, cachedContext4);
      return cachedContext4;
    }

    public virtual PredictionContext GetChild(int returnState) => (PredictionContext) new SingletonPredictionContext(this, returnState);

    public virtual string[] ToStrings(IRecognizer recognizer, int currentState) => this.ToStrings(recognizer, (PredictionContext) PredictionContext.EMPTY, currentState);

    public virtual string[] ToStrings(
      IRecognizer recognizer,
      PredictionContext stop,
      int currentState)
    {
      List<string> stringList = new List<string>();
      int num1 = 0;
      while (true)
      {
        int num2 = 0;
        bool flag = true;
        PredictionContext predictionContext = this;
        int index1 = currentState;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("[");
        int index2;
        for (; !predictionContext.IsEmpty && predictionContext != stop; predictionContext = predictionContext.GetParent(index2))
        {
          index2 = 0;
          if (predictionContext.Size > 0)
          {
            int num3 = 1;
            while (1 << num3 < predictionContext.Size)
              ++num3;
            int num4 = (1 << num3) - 1;
            index2 = num1 >> num2 & num4;
            flag &= index2 >= predictionContext.Size - 1;
            if (index2 < predictionContext.Size)
              num2 += num3;
            else
              goto label_19;
          }
          if (recognizer != null)
          {
            if (stringBuilder.Length > 1)
              stringBuilder.Append(' ');
            ATNState state = recognizer.Atn.states[index1];
            string ruleName = recognizer.RuleNames[state.ruleIndex];
            stringBuilder.Append(ruleName);
          }
          else if (predictionContext.GetReturnState(index2) != PredictionContext.EMPTY_RETURN_STATE && !predictionContext.IsEmpty)
          {
            if (stringBuilder.Length > 1)
              stringBuilder.Append(' ');
            stringBuilder.Append(predictionContext.GetReturnState(index2));
          }
          index1 = predictionContext.GetReturnState(index2);
        }
        stringBuilder.Append("]");
        stringList.Add(stringBuilder.ToString());
        if (flag)
          break;
label_19:
        ++num1;
      }
      return stringList.ToArray();
    }

    internal sealed class IdentityHashMap : Dictionary<PredictionContext, PredictionContext>
    {
      public IdentityHashMap()
        : base((IEqualityComparer<PredictionContext>) PredictionContext.IdentityEqualityComparator.Instance)
      {
      }
    }

    internal sealed class IdentityEqualityComparator : EqualityComparer<PredictionContext>
    {
      public static readonly PredictionContext.IdentityEqualityComparator Instance = new PredictionContext.IdentityEqualityComparator();

      private IdentityEqualityComparator()
      {
      }

      public override int GetHashCode(PredictionContext obj) => obj.GetHashCode();

      public override bool Equals(PredictionContext a, PredictionContext b) => a == b;
    }
  }
}
