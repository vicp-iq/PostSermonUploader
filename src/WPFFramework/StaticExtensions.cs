using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Frameworks.Presentation
{
    public static class StaticReflectionExtensions
    {
        /// <summary>
        /// Gets the name of the property being accessed in the <paramref name="propertySelector"/> expression.
        /// </summary>
        /// <typeparam name="T">Implied type of the source object.</typeparam>
        /// <typeparam name="K">Implied type of the member being accessed in <paramref name="expression"/>.</typeparam>
        /// <param name="propertySelector">A member access described as a lambda or delegate function whose body is the member access.</param>
        /// <returns>The name of the property being accessed in the <paramref name="propertySelector"/> expression.</returns>
        public static string GetPropertyName<T, K>(this Expression<Func<T, K>> propertySelector)
        {
            var name = GetProperty(propertySelector).Name;

            return name;
        }

        /// <summary>
        /// Gets a <see cref="PropertyInfo"/> object representing the property access described in <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="MODEL">Implied type of the source object.</typeparam>
        /// <param name="expression">A member access described as a lambda or delegate function whose body is the member access.</param>
        /// <returns>A <see cref="PropertyInfo"/> object describing the property access from <paramref name="expression"/>.</returns>
        public static PropertyInfo GetProperty<MODEL>(Expression<Func<MODEL, object>> expression)
        {
            MemberExpression memberExpression = getMemberExpression(expression);
            return (PropertyInfo)memberExpression.Member;
        }

        /// <summary>
        /// Gets a <see cref="PropertyInfo"/> object representing the property access described in <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="MODEL">Implied type of the source object.</typeparam>
        /// <typeparam name="T">Implied type of the member being accessed in <paramref name="expression"/>.</typeparam>
        /// <param name="expression">A member access described as a lambda or delegate function whose body is the member access.</param>
        /// <returns>A <see cref="PropertyInfo"/> object describing the property access from <paramref name="expression"/>.</returns>
        public static PropertyInfo GetProperty<MODEL, T>(Expression<Func<MODEL, T>> expression)
        {
            MemberExpression memberExpression = getMemberExpression(expression);
            return (PropertyInfo)memberExpression.Member;
        }

        /// <summary>
        /// Gets the <see cref="MemberExpression"/> described by <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="MODEL">Implied type of the source object.</typeparam>
        /// <typeparam name="T">Implied type of the member being accessed in <paramref name="expression"/>.</typeparam>
        /// <param name="expression">A member access described as a lambda or delegate function whose body is the member access.</param>
        /// <returns>A <see cref="MemberExpression"/>.</returns>
        private static MemberExpression getMemberExpression<MODEL, T>(Expression<Func<MODEL, T>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null) throw new ArgumentException("Not a member access", "member");
            return memberExpression;
        }
    }
}