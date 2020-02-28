using System;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.Models {
    public class XirrCalculator {
        public double? Calculate(IList<CashFlow> cashFlows, double guess = 0.1) {
            var calculationArguments = cashFlows
                .Select(f => new Tuple<double, double>(f.Value, f.Date.DaysSince1904()))
                .OrderBy(arg => arg.Item2)
                .ToList();

            if (CheckValues(calculationArguments) || CheckDates(calculationArguments)) return null;

            if (calculationArguments.Count == 2) {
                if (calculationArguments[0].Item2 >= calculationArguments[1].Item2)
                    return null;
            }

            double? rate = CalculateRateForLinearSearch(calculationArguments, -0.999999999999999, 1, 1e-3);
            if (rate.HasValue)
                rate = CalculateRateForBisection(calculationArguments, -0.999999999999999, 1);
            else
                rate = CalculateRateForNewton(calculationArguments, guess);

            return rate;
        }

        bool CheckValues(List<Tuple<double, double>> calculationArguments) {
            double benefit = 0;
            double cost = 0;

            for (int i = 0; i < calculationArguments.Count; i++) {
                if (calculationArguments[i].Item1 >= 0)
                    benefit += calculationArguments[i].Item1;
                else
                    cost += calculationArguments[i].Item1;
            }

            return (benefit.IsZero() || cost.IsZero());
        }

        bool CheckDates(List<Tuple<double, double>> calculationArguments) {
            return false;
        }

        double? CalculateRateForBisection(List<Tuple<double, double>> calculationArguments, double rate1, double rate2) {
            double precision = 1e-9;
            double middleRate;
            double npvStart;
            double npvEnd;
            double npvMiddle;
            double? result;

            do {
                middleRate = (rate1 + rate2) / 2;
                result = Npv(calculationArguments, rate1);
                if (!result.HasValue)
                    return result;
                npvStart = result.Value;
                result = Npv(calculationArguments, rate2);
                if (!result.HasValue)
                    return result;
                npvEnd = result.Value;
                result = Npv(calculationArguments, middleRate);
                if (!result.HasValue)
                    return result;
                npvMiddle = result.Value;

                if (npvStart * npvMiddle < 0)
                    rate2 = middleRate;
                else
                    rate1 = middleRate;
            }
            while (Math.Abs(rate1 - rate2) > precision);

            return middleRate;
        }

        double? CalculateRateForLinearSearch(List<Tuple<double, double>> calculationArguments, double leftGuess, double rigthGuess, double delta) {
            double currentRate = leftGuess;

            double? npvValue = Npv(calculationArguments, currentRate);
            if (!npvValue.HasValue)
                return npvValue;
            double npvStart = npvValue.Value;

            while (currentRate <= rigthGuess) {
                npvValue = Npv(calculationArguments, currentRate + delta);
                if (!npvValue.HasValue)
                    return npvValue;
                double npvEnd = npvValue.Value;

                if (npvStart * npvEnd < 0)
                    return (currentRate + delta / 2);

                currentRate += delta;
                npvStart = npvEnd;
            }

            return null;
        }

        double? CalculateRateForNewton(List<Tuple<double, double>> calculationArguments, double guess) {
            int maxIteration = 100;
            double precision = 1e-9;
            double currentRate = guess;
            double nextRate;
            int numberIteration = 0;
            double currentData;
            double previsiousData;
            double firstData = calculationArguments[0].Item2;
            double resultDiffNPV;
            double result2DiffNPV;
            double elementNPV;
            double elementDiffNPV;
            double element2DiffNPV;
            double powerRate;
            double benefit;
            double cost;

            while (numberIteration < maxIteration) {
                previsiousData = calculationArguments[0].Item2;
                resultDiffNPV = 0;
                result2DiffNPV = 0;
                benefit = 0;
                cost = 0;
                powerRate = 1;
                double power;

                for (int i = 0; i < calculationArguments.Count; i++) {
                    double number = calculationArguments[i].Item1;
                    currentData = calculationArguments[i].Item2;

                    power = (currentData - firstData) / 365;
                    powerRate = Math.Pow(Math.Abs(1 + currentRate), power);
                    if (double.IsInfinity(powerRate) || double.IsNaN(powerRate))
                        return null;
                    if (1 + currentRate < 0 && ((currentData - firstData) % 2).IsNotZero())
                        powerRate *= -1;
                    elementNPV = number / powerRate;
                    if (number >= 0)
                        benefit += elementNPV;
                    else
                        cost += elementNPV;
                    if (i > 0) {
                        elementDiffNPV = -elementNPV * power / (1 + currentRate);
                        resultDiffNPV += elementDiffNPV;
                        element2DiffNPV = -elementDiffNPV * (power + 1) / (1 + currentRate);
                        result2DiffNPV += element2DiffNPV;
                    }
                    previsiousData = currentData;
                }

                nextRate = currentRate - (benefit + cost) / resultDiffNPV;
                if (Math.Abs(nextRate - currentRate) < precision)
                    return nextRate;

                currentRate = nextRate;
                numberIteration++;
            }
            return null;

        }

        double? Npv(List<Tuple<double, double>> calculationArguments, double rate) {
            double npv = calculationArguments[0].Item1;
            double powerRate = 1;
            double firstData = calculationArguments[0].Item2;

            for (int i = 1; i < calculationArguments.Count; i++) {
                powerRate = Math.Pow(Math.Abs(1 + rate), (calculationArguments[i].Item2 - firstData) / 365);
                if (double.IsInfinity(powerRate) || double.IsNaN(powerRate))
                    return null;
                if (1 + rate < 0 && ((calculationArguments[i].Item2 - firstData) % 2) != 0)
                    powerRate *= -1;
                npv += calculationArguments[i].Item1 / powerRate;
            }
            return npv;
        }
    }

    public static class DateTimeHelper {
        static readonly DateTime baseDate = new DateTime(1904,1,1);
        public static double DaysSince1904(this DateTime date) {
            TimeSpan difference = date - baseDate;
            if (difference < TimeSpan.Zero) {
                throw new ArgumentException("The specified date must be after 1/1/1904");
            }
            return difference.TotalDays;
        }
    }
}
