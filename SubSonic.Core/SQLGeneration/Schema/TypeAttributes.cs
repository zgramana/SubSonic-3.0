using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SubSonic.SqlGeneration.Schema
{
    /// <summary>
    /// Provides SubSonic with the ability to de-couple the storage type from POCO type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SubSonicTypeConversionAttribute : Attribute
    {
        /// <summary>
        /// System.Converter class to be used by SubSonic.
        /// </summary>
        internal Type ConverterClass { get; set; }

        /// <summary>
        /// A type that SubSonic knows how to convert into a db type.
        /// </summary>
        internal Type DatabaseType { get; set; }

        /// <summary>
        /// The type of an target object's decorated property.
        /// </summary>
        internal Type PropertyType { get; set; }

        internal MethodInfo ConverterMethod { get; set; }

        public Object Convert(Object item, TypeConversionDirection direction)
        {
            var conversionMethodName = String.Empty;

            switch (direction)
            {
                case TypeConversionDirection.DatabaseToProperty:
                    {
                        conversionMethodName = ConversionToPropertyMethodName;
                    }
                    break;
                case TypeConversionDirection.PropertyToDatabase:
                    {
                        conversionMethodName = ConversionToDatabaseMethodName;
                    }
                    break;
                default:
                    throw new NotImplementedException("Not a valid TypeConversionDirection.");
            }
            
            return ConverterClass.InvokeMember(
                    conversionMethodName,
                    BindingFlags.InvokeMethod,
                    null,
                    null,
                    new[] { item }
                    );
        }

        protected string ConversionToDatabaseMethodName
        {
            get
            {
                return String.Concat(
                        GetSafePropertyName(PropertyType),
                        "To",
                        GetSafePropertyName(DatabaseType)
                        );
            }
        }

        protected string ConversionToPropertyMethodName
        {
            get
            {
                return String.Concat(
                        GetSafePropertyName(DatabaseType),
                        "To",
                        GetSafePropertyName(PropertyType)
                        );
            }
        }

        public SubSonicTypeConversionAttribute(Type databaseType, Type propertyType, Type converterClass)
        {
            DatabaseType = databaseType;
            PropertyType = propertyType;
            ConverterClass = converterClass;
        }
        
        /// <summary>
        /// Generates a type name string that won't invalidate C#'s method naming rules because of generic types.
        /// </summary>
        /// <param name="propType"></param>
        /// <returns>Given typeof(Func<IDictionary<String, String>, Tuple<string,int,bool>>), returns a string like FuncIDictionaryStringStringTupleStringInt32Boolean</returns>
        private string GetSafePropertyName(Type propType)
        {
            if (!propType.IsGenericType) return propType.Name;

            var nameEndIndex = propType.Name.IndexOf('`');
            var safeName = new StringBuilder(propType.Name.Substring(0, nameEndIndex));

            var genericTypeParams = propType.GetGenericArguments();

            foreach (var genericType in genericTypeParams)
            {
                safeName.Append(GetSafePropertyName(genericType));
            }

            return safeName.ToString();
        }
    }

    public enum TypeConversionDirection
    {
        DatabaseToProperty,
        PropertyToDatabase
    }
}
