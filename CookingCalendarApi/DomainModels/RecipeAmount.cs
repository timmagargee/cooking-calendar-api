using CookingCalendarApi.Enums;
using CookingCalendarApi.Utilities;
using System.Drawing;

namespace CookingCalendarApi.DomainModels
{
    public class RecipeAmount
    {
        public MeasurementType Measurement { get; set; }
        public double? Amount { get; set; }
        public int? Numerator { get; set; }
        public int? Denominator { get; set; }
        public int Servings { get; set; }

        public void MultiplyBy(int factor)
        {
            if(Amount.HasValue)
            {
                Amount *= factor;
            }
            if(HasFraction())
            {
                Numerator *= factor;
                Amount += (Numerator / Denominator);
                Numerator = Numerator % Denominator;
            }
        }

        public void DivideBy(int factor)
        {
            if (Amount.HasValue && !Numerator.HasValue)
            {
                Amount /= factor;
            }
            else if(HasFraction())
            {
                Numerator += (int)Amount.GetValueOrDefault() * Denominator;
                Denominator *= factor;
                Amount = (Numerator / Denominator);
                Numerator = Numerator % Denominator;
            }
        }

        public void Add(RecipeAmount amount)
        {
            if(amount.Measurement != Measurement)
            {
                throw new InvalidOperationException("Measurements are not the same type");
            }
            Amount += amount.Amount;

            if(HasFraction() && amount.HasFraction())
            {
                var numerator = (int)(Numerator * amount.Denominator + amount.Numerator * Denominator)!;
                var denominator = (int)(Denominator * amount.Denominator)!;
                int gcd = FractionUtility.GCD(numerator, denominator);
                Amount += numerator / gcd;
                Numerator = numerator % gcd;
                Denominator = gcd;
            }

        }

        public bool HasFraction()
        {
            return Numerator.HasValue && Denominator.GetValueOrDefault() != 0;
        }

        public double GetSingleAmount()
        {
            var amount = Amount.GetValueOrDefault();
            if (HasFraction())
            {
                amount += (double)Numerator / (double)Denominator;
            }
            return amount;
        }

        public double CollapseAmount()
        {
            Amount = GetSingleAmount();
            Numerator = null;
            Denominator = null;
            return Amount.Value;
        }

        public bool IsMetric()
        {
            return (int)Measurement > 100;
        }

        public void UpdateServings(int servings)
        {
            if (servings != Servings)
            {
                MultiplyBy(servings);
                DivideBy(Servings);
                Servings = servings;
            }
        }

    }

    public class ShoppingIngredient
    {
        public int IngredientId { get; set; }
        public RecipeAmount Amount { get; set; }
    }
}
