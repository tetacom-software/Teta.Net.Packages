// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValueLimitsValidationAttribute.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Attribute of validation of limit values for the parameter
// </summary>

namespace Teta.Packages.UnitMeasure.Contracts.Attributes
{
    /// <summary>
    /// Attribute of validation of limit values for the parameter
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Property |
        AttributeTargets.Field |
        AttributeTargets.Parameter)]
    public class PropertyValueLimitsValidationAttribute : Attribute
    {
        /// <summary>
        /// Minimum limit
        /// </summary>
        public double Minimum { get; }

        /// <summary>
        /// Maximum limit
        /// </summary>
        public double Maximum { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="minimum">Min in default unit</param>
        /// <param name="maximum">Max in default unit</param>
        public PropertyValueLimitsValidationAttribute(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
