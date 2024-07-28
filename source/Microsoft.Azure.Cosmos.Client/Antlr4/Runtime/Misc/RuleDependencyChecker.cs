// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.RuleDependencyChecker
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Antlr4.Runtime.Misc
{
  internal class RuleDependencyChecker
  {
    private const BindingFlags AllDeclaredStaticMembers = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags AllDeclaredMembers = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly HashSet<string> checkedAssemblies = new HashSet<string>();
    private static readonly Dependents ImplementedDependents = Dependents.Self | Dependents.Parents | Dependents.Children | Dependents.Ancestors | Dependents.Descendants;

    public static void CheckDependencies(Assembly assembly)
    {
      if (RuleDependencyChecker.IsChecked(assembly))
        return;
      IList<Type> typesToCheck = RuleDependencyChecker.GetTypesToCheck(assembly);
      ArrayList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>> arrayList = new ArrayList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>();
      foreach (Type clazz in (IEnumerable<Type>) typesToCheck)
        arrayList.AddRange((IEnumerable<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>) RuleDependencyChecker.GetDependencies(clazz));
      if (arrayList.Count > 0)
      {
        IDictionary<Type, IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>> dictionary = (IDictionary<Type, IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>>) new Dictionary<Type, IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>>();
        foreach (Tuple<RuleDependencyAttribute, ICustomAttributeProvider> tuple in (List<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>) arrayList)
        {
          Type recognizer = tuple.Item1.Recognizer;
          IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>> tupleList;
          if (!dictionary.TryGetValue(recognizer, out tupleList))
          {
            tupleList = (IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>) new ArrayList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>();
            dictionary[recognizer] = tupleList;
          }
          tupleList.Add(tuple);
        }
        foreach (KeyValuePair<Type, IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>> keyValuePair in (IEnumerable<KeyValuePair<Type, IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>>>) dictionary)
          RuleDependencyChecker.CheckDependencies(keyValuePair.Value, keyValuePair.Key);
      }
      RuleDependencyChecker.MarkChecked(assembly);
    }

    private static IList<Type> GetTypesToCheck(Assembly assembly) => (IList<Type>) assembly.GetTypes();

    private static bool IsChecked(Assembly assembly)
    {
      lock (RuleDependencyChecker.checkedAssemblies)
        return RuleDependencyChecker.checkedAssemblies.Contains(assembly.FullName);
    }

    private static void MarkChecked(Assembly assembly)
    {
      lock (RuleDependencyChecker.checkedAssemblies)
        RuleDependencyChecker.checkedAssemblies.Add(assembly.FullName);
    }

    private static void CheckDependencies(
      IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>> dependencies,
      Type recognizerType)
    {
      string[] ruleNames = RuleDependencyChecker.GetRuleNames(recognizerType);
      int[] ruleVersions = RuleDependencyChecker.GetRuleVersions(recognizerType, ruleNames);
      RuleDependencyChecker.RuleRelations ruleRelations = RuleDependencyChecker.ExtractRuleRelations(recognizerType);
      StringBuilder errors = new StringBuilder();
      foreach (Tuple<RuleDependencyAttribute, ICustomAttributeProvider> dependency in (IEnumerable<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>) dependencies)
      {
        if (dependency.Item1.Recognizer.IsAssignableFrom(recognizerType))
        {
          int rule = dependency.Item1.Rule;
          if (rule < 0 || rule >= ruleVersions.Length)
          {
            string str = string.Format("Rule dependency on unknown rule {0}@{1} in {2}", (object) dependency.Item1.Rule, (object) dependency.Item1.Version, (object) dependency.Item1.Recognizer.ToString());
            errors.AppendLine(dependency.Item2.ToString());
            errors.AppendLine(str);
          }
          else
          {
            Dependents dependents = Dependents.Self | dependency.Item1.Dependents;
            RuleDependencyChecker.ReportUnimplementedDependents(errors, dependency, dependents);
            BitSet bitSet = new BitSet();
            int val1 = RuleDependencyChecker.CheckDependencyVersion(errors, dependency, ruleNames, ruleVersions, rule, (string) null);
            if ((dependents & Dependents.Parents) != Dependents.None)
            {
              BitSet parent = ruleRelations.parents[dependency.Item1.Rule];
              for (int index = parent.NextSetBit(0); index >= 0; index = parent.NextSetBit(index + 1))
              {
                if (index >= 0 && index < ruleVersions.Length && !bitSet.Get(index))
                {
                  bitSet.Set(index);
                  int val2 = RuleDependencyChecker.CheckDependencyVersion(errors, dependency, ruleNames, ruleVersions, index, "parent");
                  val1 = Math.Max(val1, val2);
                }
              }
            }
            if ((dependents & Dependents.Children) != Dependents.None)
            {
              BitSet child = ruleRelations.children[dependency.Item1.Rule];
              for (int index = child.NextSetBit(0); index >= 0; index = child.NextSetBit(index + 1))
              {
                if (index >= 0 && index < ruleVersions.Length && !bitSet.Get(index))
                {
                  bitSet.Set(index);
                  int val2 = RuleDependencyChecker.CheckDependencyVersion(errors, dependency, ruleNames, ruleVersions, index, "child");
                  val1 = Math.Max(val1, val2);
                }
              }
            }
            if ((dependents & Dependents.Ancestors) != Dependents.None)
            {
              BitSet ancestors = ruleRelations.GetAncestors(dependency.Item1.Rule);
              for (int index = ancestors.NextSetBit(0); index >= 0; index = ancestors.NextSetBit(index + 1))
              {
                if (index >= 0 && index < ruleVersions.Length && !bitSet.Get(index))
                {
                  bitSet.Set(index);
                  int val2 = RuleDependencyChecker.CheckDependencyVersion(errors, dependency, ruleNames, ruleVersions, index, "ancestor");
                  val1 = Math.Max(val1, val2);
                }
              }
            }
            if ((dependents & Dependents.Descendants) != Dependents.None)
            {
              BitSet descendants = ruleRelations.GetDescendants(dependency.Item1.Rule);
              for (int index = descendants.NextSetBit(0); index >= 0; index = descendants.NextSetBit(index + 1))
              {
                if (index >= 0 && index < ruleVersions.Length && !bitSet.Get(index))
                {
                  bitSet.Set(index);
                  int val2 = RuleDependencyChecker.CheckDependencyVersion(errors, dependency, ruleNames, ruleVersions, index, "descendant");
                  val1 = Math.Max(val1, val2);
                }
              }
            }
            int version = dependency.Item1.Version;
            if (version > val1)
            {
              string str = string.Format("Rule dependency version mismatch: {0} has maximum dependency version {1} (expected {2}) in {3}", (object) ruleNames[dependency.Item1.Rule], (object) val1, (object) version, (object) dependency.Item1.Recognizer.ToString());
              errors.AppendLine(dependency.Item2.ToString());
              errors.AppendLine(str);
            }
          }
        }
      }
      if (errors.Length > 0)
        throw new InvalidOperationException(errors.ToString());
    }

    private static void ReportUnimplementedDependents(
      StringBuilder errors,
      Tuple<RuleDependencyAttribute, ICustomAttributeProvider> dependency,
      Dependents dependents)
    {
      Dependents dependents1 = dependents & ~RuleDependencyChecker.ImplementedDependents;
      if (dependents1 == Dependents.None)
        return;
      string str = string.Format("Cannot validate the following dependents of rule {0}: {1}", (object) dependency.Item1.Rule, (object) dependents1);
      errors.AppendLine(str);
    }

    private static int CheckDependencyVersion(
      StringBuilder errors,
      Tuple<RuleDependencyAttribute, ICustomAttributeProvider> dependency,
      string[] ruleNames,
      int[] ruleVersions,
      int relatedRule,
      string relation)
    {
      string ruleName = ruleNames[dependency.Item1.Rule];
      string str1 = relation != null ? string.Format("rule {0} ({1} of {2})", (object) ruleNames[relatedRule], (object) relation, (object) ruleName) : ruleName;
      int version = dependency.Item1.Version;
      int ruleVersion = ruleVersions[relatedRule];
      if (ruleVersion > version)
      {
        string str2 = string.Format("Rule dependency version mismatch: {0} has version {1} (expected <= {2}) in {3}", (object) str1, (object) ruleVersion, (object) version, (object) dependency.Item1.Recognizer.ToString());
        errors.AppendLine(dependency.Item2.ToString());
        errors.AppendLine(str2);
      }
      return ruleVersion;
    }

    private static int[] GetRuleVersions(Type recognizerClass, string[] ruleNames)
    {
      int[] ruleVersions = new int[ruleNames.Length];
      foreach (FieldInfo field in recognizerClass.GetFields())
      {
        if (field.IsStatic & field.FieldType == typeof (int))
        {
          if (field.Name.StartsWith("RULE_"))
          {
            try
            {
              string name = field.Name.Substring("RULE_".Length);
              if (name.Length != 0)
              {
                if (char.IsLower(name[0]))
                {
                  int index = (int) field.GetValue((object) null);
                  if (index >= 0)
                  {
                    if (index < ruleVersions.Length)
                    {
                      MethodInfo ruleMethod = RuleDependencyChecker.GetRuleMethod(recognizerClass, name);
                      if (!(ruleMethod == (MethodInfo) null))
                      {
                        RuleVersionAttribute customAttribute = (RuleVersionAttribute) Attribute.GetCustomAttribute((MemberInfo) ruleMethod, typeof (RuleVersionAttribute));
                        int version = customAttribute != null ? customAttribute.Version : 0;
                        ruleVersions[index] = version;
                      }
                    }
                  }
                }
              }
            }
            catch (ArgumentException ex)
            {
              throw;
            }
            catch (MemberAccessException ex)
            {
              throw;
            }
          }
        }
      }
      return ruleVersions;
    }

    private static MethodInfo GetRuleMethod(Type recognizerClass, string name)
    {
      foreach (MethodInfo method in recognizerClass.GetMethods())
      {
        if (method.Name.Equals(name) && Attribute.IsDefined((MemberInfo) method, typeof (RuleVersionAttribute)))
          return method;
      }
      return (MethodInfo) null;
    }

    private static string[] GetRuleNames(Type recognizerClass) => (string[]) recognizerClass.GetField("ruleNames").GetValue((object) null);

    public static IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>> GetDependencies(
      Type clazz)
    {
      IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>> result = (IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>) new ArrayList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>>();
      RuleDependencyChecker.GetElementDependencies(RuleDependencyChecker.AsCustomAttributeProvider((ICustomAttributeProvider) clazz), result);
      foreach (ConstructorInfo constructor in clazz.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
      {
        RuleDependencyChecker.GetElementDependencies(RuleDependencyChecker.AsCustomAttributeProvider((ICustomAttributeProvider) constructor), result);
        foreach (ICustomAttributeProvider parameter in constructor.GetParameters())
          RuleDependencyChecker.GetElementDependencies(RuleDependencyChecker.AsCustomAttributeProvider(parameter), result);
      }
      foreach (ICustomAttributeProvider field in clazz.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        RuleDependencyChecker.GetElementDependencies(RuleDependencyChecker.AsCustomAttributeProvider(field), result);
      foreach (MethodInfo method in clazz.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
      {
        RuleDependencyChecker.GetElementDependencies(RuleDependencyChecker.AsCustomAttributeProvider((ICustomAttributeProvider) method), result);
        if (method.ReturnParameter != null)
          RuleDependencyChecker.GetElementDependencies(RuleDependencyChecker.AsCustomAttributeProvider((ICustomAttributeProvider) method.ReturnParameter), result);
        foreach (ICustomAttributeProvider parameter in method.GetParameters())
          RuleDependencyChecker.GetElementDependencies(RuleDependencyChecker.AsCustomAttributeProvider(parameter), result);
      }
      return result;
    }

    private static void GetElementDependencies(
      ICustomAttributeProvider annotatedElement,
      IList<Tuple<RuleDependencyAttribute, ICustomAttributeProvider>> result)
    {
      foreach (RuleDependencyAttribute customAttribute in annotatedElement.GetCustomAttributes(typeof (RuleDependencyAttribute), true))
        result.Add(Tuple.Create<RuleDependencyAttribute, ICustomAttributeProvider>(customAttribute, annotatedElement));
    }

    private static RuleDependencyChecker.RuleRelations ExtractRuleRelations(Type recognizer)
    {
      string serializedAtn = RuleDependencyChecker.GetSerializedATN(recognizer);
      if (serializedAtn == null)
        return (RuleDependencyChecker.RuleRelations) null;
      ATN atn = new ATNDeserializer().Deserialize(serializedAtn.ToCharArray());
      RuleDependencyChecker.RuleRelations ruleRelations = new RuleDependencyChecker.RuleRelations(atn.ruleToStartState.Length);
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        if (state.epsilonOnlyTransitions)
        {
          foreach (Transition transition in state.transitions)
          {
            if (transition.TransitionType == TransitionType.RULE)
            {
              RuleTransition ruleTransition = (RuleTransition) transition;
              ruleRelations.AddRuleInvocation(state.ruleIndex, ruleTransition.target.ruleIndex);
            }
          }
        }
      }
      return ruleRelations;
    }

    private static string GetSerializedATN(Type recognizerClass)
    {
      FieldInfo field = recognizerClass.GetField("_serializedATN", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (field != (FieldInfo) null)
        return (string) field.GetValue((object) null);
      return recognizerClass.BaseType != (Type) null ? RuleDependencyChecker.GetSerializedATN(recognizerClass.BaseType) : (string) null;
    }

    private RuleDependencyChecker()
    {
    }

    protected static ICustomAttributeProvider AsCustomAttributeProvider(ICustomAttributeProvider obj) => obj;

    private sealed class RuleRelations
    {
      public readonly BitSet[] parents;
      public readonly BitSet[] children;

      public RuleRelations(int ruleCount)
      {
        this.parents = new BitSet[ruleCount];
        for (int index = 0; index < ruleCount; ++index)
          this.parents[index] = new BitSet();
        this.children = new BitSet[ruleCount];
        for (int index = 0; index < ruleCount; ++index)
          this.children[index] = new BitSet();
      }

      public bool AddRuleInvocation(int caller, int callee)
      {
        if (caller < 0 || this.children[caller].Get(callee))
          return false;
        this.children[caller].Set(callee);
        this.parents[callee].Set(caller);
        return true;
      }

      public BitSet GetAncestors(int rule)
      {
        BitSet ancestors = new BitSet();
        ancestors.Or(this.parents[rule]);
        int num;
        do
        {
          num = ancestors.Cardinality();
          for (int index = ancestors.NextSetBit(0); index >= 0; index = ancestors.NextSetBit(index + 1))
            ancestors.Or(this.parents[index]);
        }
        while (ancestors.Cardinality() != num);
        return ancestors;
      }

      public BitSet GetDescendants(int rule)
      {
        BitSet descendants = new BitSet();
        descendants.Or(this.children[rule]);
        int num;
        do
        {
          num = descendants.Cardinality();
          for (int index = descendants.NextSetBit(0); index >= 0; index = descendants.NextSetBit(index + 1))
            descendants.Or(this.children[index]);
        }
        while (descendants.Cardinality() != num);
        return descendants;
      }
    }
  }
}
