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
                        conversionMethodName = String.Concat(
                                DatabaseType.Name,
                                "To",
                                PropertyType.Name
                                );
                    }
                    break;
                case TypeConversionDirection.PropertyToDatabase:
                    {
                        conversionMethodName = String.Concat(
                                PropertyType.Name,
                                "To",
                                DatabaseType.Name
                                );
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

        public SubSonicTypeConversionAttribute(Type databaseType, Type propertyType, Type converterClass)
        {
            DatabaseType = databaseType;
            PropertyType = propertyType;
            ConverterClass = converterClass;
        }
    }

    public enum TypeConversionDirection
    {
        DatabaseToProperty,
        PropertyToDatabase
    }
}
