// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ATNSimulator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;
using System;

namespace Antlr4.Runtime.Atn
{
  internal abstract class ATNSimulator
  {
    public static readonly DFAState ERROR = ATNSimulator.InitERROR();
    public readonly ATN atn;
    protected readonly PredictionContextCache sharedContextCache;

    private static DFAState InitERROR() => new DFAState(new ATNConfigSet())
    {
      stateNumber = int.MaxValue
    };

    public ATNSimulator(ATN atn, PredictionContextCache sharedContextCache)
    {
      this.atn = atn;
      this.sharedContextCache = sharedContextCache;
    }

    public abstract void Reset();

    public virtual void ClearDFA() => throw new Exception("This ATN simulator does not support clearing the DFA.");

    protected void ConsoleWriteLine(string format, params object[] arg) => Console.WriteLine(format, arg);

    public PredictionContextCache getSharedContextCache() => this.sharedContextCache;

    public PredictionContext getCachedContext(PredictionContext context)
    {
      if (this.sharedContextCache == null)
        return context;
      lock (this.sharedContextCache)
      {
        PredictionContext.IdentityHashMap visited = new PredictionContext.IdentityHashMap();
        return PredictionContext.GetCachedContext(context, this.sharedContextCache, visited);
      }
    }
  }
}
