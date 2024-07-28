// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.DataImportValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  public class DataImportValidator : IDisposable
  {
    public ImportValidationOutcome Outcome;
    private IDisposableReadOnlyList<IImportValidation> m_validations;
    private DataImportValidationContext m_validationContext;

    public IDisposableReadOnlyList<IImportValidation> Validations => this.m_validations;

    public DataImportValidationType Skip { get; set; }

    public IList<string> Errors { get; private set; }

    public IList<string> Warnings { get; private set; }

    public event EventHandler<DataImportActivityCompletedEventArgs> ValidationTypeCompleted;

    public event EventHandler<DataImportActivityCompletedEventArgs> ValidationStarted;

    public event EventHandler<DataImportActivityCompletedEventArgs> ValidationCompleted;

    public DataImportValidator(DataImportValidationContext context)
    {
      ArgumentUtility.CheckForNull<DataImportValidationContext>(context, nameof (context));
      this.m_validations = (IDisposableReadOnlyList<IImportValidation>) null;
      this.m_validationContext = context;
      this.Warnings = (IList<string>) new List<string>();
      this.Errors = (IList<string>) new List<string>();
    }

    public void LoadExtensions(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_validations = vssRequestContext.GetService<IVssExtensionManagementService>().GetExtensions<IImportValidation>(vssRequestContext);
    }

    public void Dispose()
    {
      if (this.m_validations == null)
        return;
      this.m_validations.Dispose();
    }

    public void RunCollectionValidations() => this.Run(DataImportValidationType.Collection | DataImportValidationType.ProjectProcessesMap);

    public void RunMappingFileValidations() => this.Run(DataImportValidationType.IdentityMap);

    public void Run(DataImportValidationType filter = DataImportValidationType.All)
    {
      this.Outcome = ImportValidationOutcome.NotRun;
      this.Warnings.Clear();
      this.Errors.Clear();
      foreach (IGrouping<DataImportValidationType, IImportValidation> source in this.Validations.Where<IImportValidation>((Func<IImportValidation, bool>) (v => (v.Type & filter) != 0)).GroupBy<IImportValidation, DataImportValidationType>((Func<IImportValidation, DataImportValidationType>) (v => v.Type)))
      {
        ImportValidationOutcome val1 = ImportValidationOutcome.NotRun;
        if (this.Skip.HasFlag((Enum) source.Key))
        {
          this.m_validationContext.Logger.Info(string.Format("Skipping validations of type {0}", (object) source.Key));
        }
        else
        {
          IEnumerable<IImportValidation> importValidations = (IEnumerable<IImportValidation>) DataImportValidator.OrderValidations(source.ToList<IImportValidation>());
          ITFLogger logger = this.m_validationContext.Logger;
          FileLogger fileLogger = new FileLogger(Path.Combine(this.m_validationContext.GetItem<string>("Output", "."), source.Key.ToString() + ".log"), FileMode.Append);
          this.m_validationContext.SetLogger((ITFLogger) new AggregateLogger(new ITFLogger[2]
          {
            logger,
            (ITFLogger) fileLogger
          }));
          int num1 = 0;
          foreach (IImportValidation importValidation in importValidations)
          {
            string message = (string) null;
            this.OnValidationStarted(new DataImportActivityCompletedEventArgs()
            {
              Name = importValidation.Name,
              Type = source.Key
            });
            ImportValidationOutcome val2;
            try
            {
              this.m_validationContext.Logger.Info("========================================================");
              this.m_validationContext.Logger.Info("Starting validation '" + importValidation.Name + "' (" + importValidation.GetType().ToString() + ")");
              val2 = importValidation.Run(this.m_validationContext, out message);
            }
            catch (Exception ex)
            {
              val2 = ImportValidationOutcome.Failed;
              if (string.IsNullOrEmpty(message))
                message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format("{0} {1}", (object) importValidation.Name, (object) val2));
              this.m_validationContext.Logger.Error(ex.ToReadableStackTrace());
            }
            switch (val2)
            {
              case ImportValidationOutcome.Warning:
                this.Warnings.Add(message);
                break;
              case ImportValidationOutcome.Failed:
                this.Errors.Add(message);
                break;
            }
            this.Outcome = (ImportValidationOutcome) Math.Max((int) this.Outcome, (int) val2);
            val1 = (ImportValidationOutcome) Math.Max((int) val1, (int) val2);
            DataImportActivityCompletedEventArgs e = new DataImportActivityCompletedEventArgs();
            e.Name = importValidation.Name;
            e.Outcome = val2;
            e.Type = source.Key;
            e.ValidationCount = source.Count<IImportValidation>();
            int num2;
            num1 = num2 = num1 + 1;
            e.ValidationNumber = num2;
            this.OnValidationCompleted(e);
            this.m_validationContext.Logger.Info(string.Format("Validation completed '{0}' with result {1}, message {2}", (object) importValidation.Name, (object) val2, (object) message));
          }
          this.m_validationContext.SetLogger(logger);
          fileLogger?.Dispose();
        }
        this.OnValidationTypeCompleted(new DataImportActivityCompletedEventArgs()
        {
          Name = source.Key.ToString(),
          Outcome = val1
        });
      }
    }

    public static List<IImportValidation> OrderValidations(List<IImportValidation> list)
    {
      List<IImportValidation> importValidationList = new List<IImportValidation>();
      while (!list.IsNullOrEmpty<IImportValidation>())
      {
        IImportValidation importValidation1 = list.First<IImportValidation>();
        list.Remove(importValidation1);
        importValidationList.Add(importValidation1);
        foreach (string str in ((IEnumerable<object>) importValidation1.GetType().GetCustomAttributes(false)).Where<object>((Func<object, bool>) (a => a is DependsOnValidationAttribute)).Select<object, string>((Func<object, string>) (a => ((DependsOnValidationAttribute) a).Type)))
        {
          string type = str;
          IImportValidation importValidation2 = list.FirstOrDefault<IImportValidation>((Func<IImportValidation, bool>) (g => string.Equals(g.GetType().Name, type, StringComparison.OrdinalIgnoreCase)));
          if (importValidation2 != null && !importValidationList.Contains(importValidation2))
          {
            list.Remove(importValidation2);
            importValidationList.Insert(importValidationList.Count - 1, importValidation2);
          }
        }
      }
      return importValidationList;
    }

    public virtual void OnValidationStarted(DataImportActivityCompletedEventArgs e)
    {
      EventHandler<DataImportActivityCompletedEventArgs> validationStarted = this.ValidationStarted;
      if (validationStarted == null)
        return;
      validationStarted((object) this, e);
    }

    public virtual void OnValidationCompleted(DataImportActivityCompletedEventArgs e)
    {
      EventHandler<DataImportActivityCompletedEventArgs> validationCompleted = this.ValidationCompleted;
      if (validationCompleted == null)
        return;
      validationCompleted((object) this, e);
    }

    public virtual void OnValidationTypeCompleted(DataImportActivityCompletedEventArgs e)
    {
      EventHandler<DataImportActivityCompletedEventArgs> validationTypeCompleted = this.ValidationTypeCompleted;
      if (validationTypeCompleted == null)
        return;
      validationTypeCompleted((object) this, e);
    }
  }
}
