// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalcParameterAttribute.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Service attribute for converting units of measurement in the calculated parameter
// </summary>

namespace Teta.Packages.UnitMeasure.Contracts.Attributes
{
    /// <summary>
    ///  Service attribute for converting units of measurement in the calculated parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
    public class CalcParameterAttribute : Attribute
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="id">calc parameter id</param>
        public CalcParameterAttribute(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Calc parameter id
        /// </summary>
        public int Id { get; set; }
    }
}