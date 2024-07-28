// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.TreeModifications
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  [Flags]
  public enum TreeModifications : long
  {
    None = 0,
    PreserveImportantComments = 1,
    BracketMemberToDotMember = 2,
    NewObjectToObjectLiteral = 4,
    NewArrayToArrayLiteral = 8,
    RemoveEmptyDefaultCase = 16, // 0x0000000000000010
    RemoveEmptyCaseWhenNoDefault = 32, // 0x0000000000000020
    RemoveBreakFromLastCaseBlock = 64, // 0x0000000000000040
    RemoveEmptyFinally = 128, // 0x0000000000000080
    RemoveDuplicateVar = 256, // 0x0000000000000100
    CombineVarStatements = 512, // 0x0000000000000200
    MoveVarIntoFor = 1024, // 0x0000000000000400
    VarInitializeReturnToReturnInitializer = 2048, // 0x0000000000000800
    IfEmptyToExpression = 4096, // 0x0000000000001000
    IfConditionCallToConditionAndCall = 8192, // 0x0000000000002000
    IfElseReturnToReturnConditional = 16384, // 0x0000000000004000
    IfConditionReturnToCondition = 32768, // 0x0000000000008000
    IfConditionFalseToIfNotConditionTrue = 65536, // 0x0000000000010000
    CombineAdjacentStringLiterals = 131072, // 0x0000000000020000
    RemoveUnaryPlusOnNumericLiteral = 262144, // 0x0000000000040000
    ApplyUnaryMinusToNumericLiteral = 524288, // 0x0000000000080000
    MinifyStringLiterals = 1048576, // 0x0000000000100000
    MinifyNumericLiterals = 2097152, // 0x0000000000200000
    RemoveUnusedParameters = 4194304, // 0x0000000000400000
    StripDebugStatements = 8388608, // 0x0000000000800000
    LocalRenaming = 16777216, // 0x0000000001000000
    RemoveFunctionExpressionNames = 33554432, // 0x0000000002000000
    RemoveUnnecessaryLabels = 67108864, // 0x0000000004000000
    RemoveUnnecessaryCCOnStatements = 134217728, // 0x0000000008000000
    DateGetTimeToUnaryPlus = 268435456, // 0x0000000010000000
    EvaluateNumericExpressions = 536870912, // 0x0000000020000000
    SimplifyStringToNumericConversion = 1073741824, // 0x0000000040000000
    PropertyRenaming = 2147483648, // 0x0000000080000000
    RemoveQuotesFromObjectLiteralNames = 8589934592, // 0x0000000200000000
    BooleanLiteralsToNotOperators = 17179869184, // 0x0000000400000000
    IfExpressionsToExpression = 34359738368, // 0x0000000800000000
    CombineAdjacentExpressionStatements = 68719476736, // 0x0000001000000000
    ReduceStrictOperatorIfTypesAreSame = 137438953472, // 0x0000002000000000
    ReduceStrictOperatorIfTypesAreDifferent = 274877906944, // 0x0000004000000000
    MoveFunctionToTopOfScope = 549755813888, // 0x0000008000000000
    CombineVarStatementsToTopOfScope = 1099511627776, // 0x0000010000000000
    IfNotTrueFalseToIfFalseTrue = 2199023255552, // 0x0000020000000000
    MoveInExpressionsIntoForStatement = 4398046511104, // 0x0000040000000000
    InvertIfReturn = 8796093022208, // 0x0000080000000000
    CombineNestedIfs = 17592186044416, // 0x0000100000000000
    CombineEquivalentIfReturns = 35184372088832, // 0x0000200000000000
    ChangeWhileToFor = 70368744177664, // 0x0000400000000000
    InvertIfContinue = 140737488355328, // 0x0000800000000000
    EvaluateLiteralJoins = 281474976710656, // 0x0001000000000000
    RemoveUnusedVariables = 562949953421312, // 0x0002000000000000
    UnfoldCommaExpressionStatements = 1125899906842624, // 0x0004000000000000
    EvaluateLiteralLengths = 2251799813685248, // 0x0008000000000000
    RemoveWindowDotFromTypeOf = 4503599627370496, // 0x0010000000000000
  }
}
