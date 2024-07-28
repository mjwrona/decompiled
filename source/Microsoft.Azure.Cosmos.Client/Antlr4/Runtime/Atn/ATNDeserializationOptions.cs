// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ATNDeserializationOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime.Atn
{
  internal class ATNDeserializationOptions
  {
    private static readonly ATNDeserializationOptions defaultOptions = new ATNDeserializationOptions();
    private bool readOnly;
    private bool verifyATN;
    private bool generateRuleBypassTransitions;
    private bool optimize;

    static ATNDeserializationOptions() => ATNDeserializationOptions.defaultOptions.MakeReadOnly();

    public ATNDeserializationOptions()
    {
      this.verifyATN = true;
      this.generateRuleBypassTransitions = false;
      this.optimize = true;
    }

    public ATNDeserializationOptions(ATNDeserializationOptions options)
    {
      this.verifyATN = options.verifyATN;
      this.generateRuleBypassTransitions = options.generateRuleBypassTransitions;
      this.optimize = options.optimize;
    }

    [NotNull]
    public static ATNDeserializationOptions Default => ATNDeserializationOptions.defaultOptions;

    public bool IsReadOnly => this.readOnly;

    public void MakeReadOnly() => this.readOnly = true;

    public bool VerifyAtn
    {
      get => this.verifyATN;
      set
      {
        bool flag = value;
        this.ThrowIfReadOnly();
        this.verifyATN = flag;
      }
    }

    public bool GenerateRuleBypassTransitions
    {
      get => this.generateRuleBypassTransitions;
      set
      {
        bool flag = value;
        this.ThrowIfReadOnly();
        this.generateRuleBypassTransitions = flag;
      }
    }

    public bool Optimize
    {
      get => this.optimize;
      set
      {
        bool flag = value;
        this.ThrowIfReadOnly();
        this.optimize = flag;
      }
    }

    protected internal virtual void ThrowIfReadOnly()
    {
      if (this.IsReadOnly)
        throw new InvalidOperationException("The object is read only.");
    }
  }
}
