using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.Practices.Prism.ViewModel;

namespace Frameworks.Presentation
{
    public class ViewModel : NotificationObject, IDataErrorInfo
    {
        protected TProperty Get<TEntity, TProperty>(TEntity entity, Func<TEntity, TProperty> propertyAccessor) where TEntity : class
        {
            var result = default(TProperty);

            if (entity != null && propertyAccessor != null)
            {
                result = propertyAccessor(entity);
            }

            return result;
        }

        public string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return PropertySupport.ExtractPropertyName<T>(propertyExpression);
        }

        public string GetPropertyName<TSource, TReturn>(Expression<Func<TSource, TReturn>> propertyExpression)
        {
            return propertyExpression.GetPropertyName();
        }

        public bool IsProperty<T>(string propertyName, Expression<Func<T>> propertyExpression)
        {
            return string.Equals(propertyName, GetPropertyName(propertyExpression));
        }

        protected void RaiseAllPropertyChanged()
        {
            RaisePropertyChanged(string.Empty);
        }

        private Dictionary<string, string> errors;
        protected Dictionary<string, string> Errors
        {
            get
            {
                if (errors == null)
                {
                    errors = new Dictionary<string, string>();
                }
                return errors;
            }

            set { errors = value; }
        }

        //private ValidationResult validationResult;

        //public ValidationResult ValidationResult
        //{
        //    private get { return validationResult; }
        //    set
        //    {
        //        validationResult = value;
        //        Errors = value.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessage);
        //        error = string.Join(Environment.NewLine, value.Errors.Select(x => x.ErrorMessage));
        //    }
        //}

        public bool IsValid
        {
            get
            {
                //if (ValidationResult == null)
                //{
                return true;
                //}

                //return ValidationResult.IsValid;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result;

                Errors.TryGetValue(columnName, out result);

                return result;
            }
        }

        private string error;
        public string Error
        {
            get { return error; }
        }
    }
}