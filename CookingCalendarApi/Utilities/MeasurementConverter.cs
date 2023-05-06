using CookingCalendarApi.DomainModels;
using CookingCalendarApi.Enums;

namespace CookingCalendarApi.Utilities
{
    public static class MeasurementConverter
    {

        public static RecipeAmount ConvertTo(this RecipeAmount amount, MeasurementType meas)
        {
            if (amount.Measurement != meas)
            {
                switch (amount.Measurement)
                {
                    case MeasurementType.Tsp:
                        switch (meas)
                        {
                            case MeasurementType.Tbl:
                                amount.DivideBy(3);
                                break;
                            case MeasurementType.Cup:
                                amount.DivideBy(32);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Cup);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Tbl:
                        switch (meas)
                        {
                            case MeasurementType.Tsp:
                                amount.MultiplyBy(3);
                                break;
                            case MeasurementType.FlOz:
                                amount.DivideBy(2);
                                break;
                            case MeasurementType.Cup:
                                amount.DivideBy(16);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Cup);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.FlOz:
                        switch (meas)
                        {
                            case MeasurementType.Tbl:
                                amount.MultiplyBy(2);
                                break;
                            case MeasurementType.Cup:
                                amount.DivideBy(8);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Cup);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Cup:
                        switch (meas)
                        {
                            case MeasurementType.Tsp:
                                amount.MultiplyBy(32);
                                break;
                            case MeasurementType.Tbl:
                                amount.MultiplyBy(16);
                                break;
                            case MeasurementType.FlOz:
                                amount.MultiplyBy(8);
                                break;
                            case MeasurementType.Pt:
                                amount.DivideBy(2);
                                break;
                            case MeasurementType.Qt:
                                amount.DivideBy(4);
                                break;
                            case MeasurementType.Gal:
                                amount.DivideBy(16);
                                break;
                            case MeasurementType.Oz:
                                //Not True for all ingredients
                                amount.MultiplyBy(8);
                                break;
                            case MeasurementType.Lb:
                                //Not True for all Ingredients
                                amount.DivideBy(2);
                                break;
                            case MeasurementType.L:
                                amount.Amount = amount.CollapseAmount() / 4.22675;
                                break;
                            default:
                                if (amount.IsMetric())
                                {
                                    amount.ConvertTo(MeasurementType.L);
                                    return amount.ConvertTo(meas);
                                }
                                throw new NotImplementedException("Add more conversions to CUP");
                        }
                        break;
                    case MeasurementType.Pt:
                        switch (meas)
                        {
                            case MeasurementType.Cup:
                                amount.MultiplyBy(2);
                                break;
                            case MeasurementType.Qt:
                                amount.DivideBy(2);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Cup);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Qt:
                        switch (meas)
                        {
                            case MeasurementType.Cup:
                                amount.MultiplyBy(4);
                                break;
                            case MeasurementType.Pt:
                                amount.MultiplyBy(2);
                                break;
                            case MeasurementType.Gal:
                                amount.DivideBy(4);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Cup);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Gal:
                        switch (meas)
                        {
                            case MeasurementType.Cup:
                                amount.MultiplyBy(16);
                                break;
                            case MeasurementType.Qt:
                                amount.MultiplyBy(4);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Cup);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Lb:
                        switch (meas)
                        {
                            case MeasurementType.Oz:
                                amount.MultiplyBy(16);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Oz);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Oz:
                        switch (meas)
                        {
                            case MeasurementType.Lb:
                                amount.DivideBy(16);
                                break;
                            case MeasurementType.Cup:
                                //NOT TRUE FOR ALL INGREDIENTS
                                amount.DivideBy(8);
                                break;
                            case MeasurementType.Kg:
                                amount.Amount = amount.CollapseAmount() / 35.274;
                                break;
                            default:
                                if (amount.IsMetric())
                                {
                                    amount.ConvertTo(MeasurementType.Kg);
                                }
                                else
                                {
                                    amount.ConvertTo(MeasurementType.Cup);
                                }
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.In:
                        switch (meas)
                        {
                            case MeasurementType.Yd:
                                amount.MultiplyBy(36);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.Yd);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Yd:
                        switch (meas)
                        {
                            case MeasurementType.In:
                                amount.DivideBy(36);
                                break;
                            case MeasurementType.M:
                                amount.Amount = amount.CollapseAmount() * 0.9144;
                                break;
                            default:
                                if (amount.IsMetric())
                                {
                                    amount.ConvertTo(MeasurementType.M);
                                    return amount.ConvertTo(meas);
                                }
                                throw new NotImplementedException();
                        }
                        break;
                    case MeasurementType.F:
                        if (meas == MeasurementType.Celcius)
                        {
                            amount.Amount = (amount.CollapseAmount() - 32) * 5 / 9;
                        }
                        break;
                    case MeasurementType.ML:
                        switch (meas)
                        {
                            case MeasurementType.L:
                                amount.DivideBy(1000);
                                break;
                            default: 
                                amount.ConvertTo(MeasurementType.L);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.L:
                        switch (meas)
                        {
                            case MeasurementType.ML:
                                amount.MultiplyBy(1000);
                                break;
                            case MeasurementType.DL:
                                amount.MultiplyBy(10);
                                break;
                            case MeasurementType.Cup:
                                amount.Amount = amount.CollapseAmount() * 4.22675;
                                break;
                            default:
                                if (!amount.IsMetric())
                                {
                                    amount.ConvertTo(MeasurementType.Cup);
                                    return amount.ConvertTo(meas);
                                }
                                throw new NotImplementedException();
                        }
                        break;
                    case MeasurementType.DL:
                        switch (meas)
                        {
                            case MeasurementType.L:
                                amount.DivideBy(10);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.L);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.G:
                        switch (meas)
                        {
                            case MeasurementType.Kg:
                                amount.DivideBy(1000);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.L);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.Kg:
                        switch (meas)
                        {
                            case MeasurementType.G:
                                amount.MultiplyBy(1000);
                                break;
                            case MeasurementType.Oz:
                                amount.Amount = amount.CollapseAmount() * 35.274;
                                break;
                            default:
                                if (!amount.IsMetric())
                                {
                                    amount.ConvertTo(MeasurementType.Oz);
                                    return amount.ConvertTo(meas);
                                }
                                throw new NotImplementedException();
                        }
                        break;
                    case MeasurementType.MM:
                        switch (meas)
                        {
                            case MeasurementType.M:
                                amount.DivideBy(1000);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.M);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.CM:
                        switch (meas)
                        {
                            case MeasurementType.M:
                                amount.DivideBy(100);
                                break;
                            default:
                                amount.ConvertTo(MeasurementType.M);
                                return amount.ConvertTo(meas);
                        }
                        break;
                    case MeasurementType.M:
                        switch (meas)
                        {
                            case MeasurementType.MM:
                                amount.MultiplyBy(1000);
                                break;
                            case MeasurementType.CM:
                                amount.MultiplyBy(100);
                                break;
                            case MeasurementType.Yd:
                                amount.Amount = amount.CollapseAmount() * 1.09361;
                                break;
                            default:
                                if (!amount.IsMetric())
                                {
                                    amount.ConvertTo(MeasurementType.Yd);
                                    return amount.ConvertTo(meas);
                                }
                                throw new NotImplementedException();
                        }
                        break;
                    case MeasurementType.Celcius:
                        if (meas == MeasurementType.F)
                        {
                            amount.Amount = (amount.CollapseAmount() * 9 / 5) + 32;
                        }
                        break;
                    //case MeasurementType.Amount:
                    //    break;
                    default:
                        throw new InvalidOperationException("CANNOT CONVERT");
                }
            }
            if (amount.Amount.HasValue)
            {
                amount.Amount = Math.Round(amount.Amount.Value, 3);
            }
            amount.Measurement = meas;
            return amount;
        }

        public static RecipeAmount ConvertToDefault(this RecipeAmount amount)
        {
            return amount.ConvertTo(GetDefaultMeasurement(amount.Measurement));
        }

        public static MeasurementType GetDefaultMeasurement(MeasurementType measurementType)
        {
            switch (measurementType)
            {
                case MeasurementType.Tsp:
                case MeasurementType.Tbl:
                case MeasurementType.FlOz:
                case MeasurementType.Cup:
                case MeasurementType.Pt:
                case MeasurementType.Qt:
                case MeasurementType.Gal:
                    return MeasurementType.FlOz;
                case MeasurementType.Lb:
                case MeasurementType.Oz:
                    return MeasurementType.Oz;
                case MeasurementType.In:
                case MeasurementType.Yd:
                    return MeasurementType.In;
                case MeasurementType.ML:
                case MeasurementType.L:
                case MeasurementType.DL:
                    return MeasurementType.ML;
                case MeasurementType.G:
                case MeasurementType.Kg:
                    return MeasurementType.G;
                case MeasurementType.MM:
                case MeasurementType.CM:
                case MeasurementType.M:
                    return MeasurementType.MM;
                default:
                    return measurementType;
            }
        }
    }
}
