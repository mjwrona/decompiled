// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.MultiplierHelpers
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class MultiplierHelpers
  {
    private static readonly char[] ValueSeparator = new char[1]
    {
      ','
    };

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "internal")]
    public static Dictionary<string, IDictionary<string, string>> GetMatrix(
      this MultiConfigInput multiplierOptions,
      IDictionary<string, string> variables,
      bool allowNullOrEmptyValues)
    {
      if (multiplierOptions == null)
        return (Dictionary<string, IDictionary<string, string>>) null;
      Dictionary<string, IDictionary<string, string>> matrix = new Dictionary<string, IDictionary<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IDictionary<string, IList<string>> multiplierValues = MultiplierHelpers.GetMultiplierValues(multiplierOptions, variables, allowNullOrEmptyValues);
      if (multiplierValues.Count > 0)
      {
        foreach (IList<KeyValuePair<string, string>> combination in (IEnumerable<IList<KeyValuePair<string, string>>>) MultiplierHelpers.ComputeCombinations(multiplierValues))
        {
          Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) combination)
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
          string key = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Multiplier, (object) string.Join(",", combination.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Value))));
          matrix[key] = (IDictionary<string, string>) dictionary;
        }
      }
      else
      {
        string key = string.Join(",", new string[1]
        {
          multiplierOptions.Multipliers
        });
        matrix[key] = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      }
      return matrix;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "internal")]
    public static IDictionary<string, IList<string>> GetMultiplierValues(
      MultiConfigInput input,
      IDictionary<string, string> variables,
      bool allowNullOrEmptyValues)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      IDictionary<string, IList<string>> multiplierValues1 = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ICollection<string> multiplierVariables = input.GetMultiplierVariables();
      if (multiplierVariables.Count == 0)
        return multiplierValues1;
      IDictionary<string, IList<string>> multiplierValues2 = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string str in (IEnumerable<string>) multiplierVariables)
      {
        if (!string.IsNullOrEmpty(str))
        {
          IList<string> variableValues = (IList<string>) MultiplierHelpers.GetVariableValues(variables, str, allowNullOrEmptyValues);
          if (variableValues.Any<string>())
            multiplierValues2.Add(str, variableValues);
        }
      }
      return multiplierValues2;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "internal")]
    public static IList<IList<KeyValuePair<string, string>>> ComputeCombinations(
      IDictionary<string, IList<string>> multipliers)
    {
      if (multipliers == null)
        throw new ArgumentNullException(nameof (multipliers));
      IList<IList<KeyValuePair<string, string>>> results = (IList<IList<KeyValuePair<string, string>>>) new List<IList<KeyValuePair<string, string>>>();
      KeyValuePair<string, string>[] workingSet = new KeyValuePair<string, string>[multipliers.Count];
      MultiplierHelpers.ComputeCombinations(multipliers, results, workingSet, (IList<string>) multipliers.Keys.ToList<string>(), 0);
      return results;
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "index+1", Justification = "internal code.")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "internal code")]
    public static void ComputeCombinations(
      IDictionary<string, IList<string>> multipliers,
      IList<IList<KeyValuePair<string, string>>> results,
      KeyValuePair<string, string>[] workingSet,
      IList<string> keys,
      int index)
    {
      if (multipliers == null)
        throw new ArgumentNullException(nameof (multipliers));
      if (results == null)
        throw new ArgumentNullException(nameof (results));
      if (workingSet == null)
        throw new ArgumentNullException(nameof (workingSet));
      string key = keys != null ? keys[index] : throw new ArgumentNullException(nameof (keys));
      foreach (string str in (IEnumerable<string>) multipliers[key])
      {
        workingSet[index] = new KeyValuePair<string, string>(key, str);
        if (index == keys.Count - 1)
          results.Add((IList<KeyValuePair<string, string>>) ((IEnumerable<KeyValuePair<string, string>>) workingSet).ToList<KeyValuePair<string, string>>());
        else
          MultiplierHelpers.ComputeCombinations(multipliers, results, workingSet, keys, index + 1);
      }
    }

    private static List<string> GetVariableValues(
      IDictionary<string, string> variables,
      string variableName,
      bool allowNullOrEmptyValues)
    {
      string enumerable;
      if (variables.TryGetValue(variableName, out enumerable))
      {
        if (!enumerable.IsNullOrEmpty<char>())
          return ((IEnumerable<string>) enumerable.Split(MultiplierHelpers.ValueSeparator, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (value => VariableResolutionHelper.ResolveVariableValue(value, variables, (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              variableName,
              (string) null
            }
          }))).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
        if (!allowNullOrEmptyValues)
          throw new InvalidMultiConfigException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidValuesInMultiplierInput, (object) variableName));
      }
      return new List<string>();
    }
  }
}
