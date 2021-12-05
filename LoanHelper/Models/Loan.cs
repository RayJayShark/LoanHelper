using System;
using System.Collections.Generic;
using System.Linq;

namespace LoanHelper.Models
{
    public class Loan
    {
        public double PrincipalValue { get; set; }
        public double InterestRate { get; set; }
        public InterestFrequency Frequency { get; set; }
        public int NumberOfPeriods { get; set; }
        public double PaymentAmount { get; set; }
        public double EscrowPayment { get; set; }
        /// <summary>
        /// The states of the loan over time
        /// </summary>
        public List<LoanState> LoanStates { get; set; }

        private double? _totalInterestPayed;
        public double TotalInterestPayed
        {
            get
            {
                _totalInterestPayed ??= LoanStates.Sum(state => state.Interest);
                return _totalInterestPayed.Value;
            }
        }

        public enum InterestFrequency
        {
            Annually,
            Monthly
        }

        public Loan()
        {
            LoanStates = new List<LoanState>();
        }
        
        /// <summary>
        /// Calculates new InterestRate based on new InterestFrequency
        /// Example: Changing from Annually to Monthly divides the interest rate by 12
        /// </summary>
        public void ChangeInterestType()
        {
            // No calculation needed
            if (InterestRate == 0) return;
            
            // Changed to annually
            if (Frequency == InterestFrequency.Annually)
            {
                InterestRate *= 12;
            }
            // Changed to monthly
            else
            {
                InterestRate /= 12d;
            }
        }
        
        /// <summary>
        /// Calculates amount of payments when all other info is given
        /// </summary>
        public void CalculatePayment()
        {
            // Check for valid data. Just returns for now.
            if (PrincipalValue <= 0 || NumberOfPeriods <= 0 || InterestRate < 0) return;
            
            // r(PV)/1-(1+r)^-n
            var percentRate = Frequency == InterestFrequency.Annually ? (InterestRate / 100d) / 12d : InterestRate / 100d;
            PaymentAmount = (percentRate * PrincipalValue) / (1 - Math.Pow(1 + percentRate, NumberOfPeriods * -1));
        }
        
        /// <summary>
        /// Calculates the periods required to pay off loan when all other info is given
        /// </summary>
        public void CalculatePeriods()
        {
            // Check for valid data. Just returns for now.
            if (PrincipalValue <= 0 || PaymentAmount <= 0 || InterestRate < 0) return;
            
            // -log(1 - r(PV)/P) / log(1+r)
            var percentRate = Frequency == InterestFrequency.Annually ? (InterestRate / 100d) / 12d : InterestRate / 100d;
            
            // Check number in log for negative to avoid issues
            var innerLog = 1 - ((percentRate * PrincipalValue) / PaymentAmount);
            if (innerLog < 0) return;
            
            NumberOfPeriods = (int) Math.Ceiling(-Math.Log(innerLog) / Math.Log(1 + percentRate));
        }
        
        /// <summary>
        /// Simulates a mortgage and stores the data in LoanStates
        /// </summary>
        public void SimulateMortgage()
        {
            // Check for valid data. Just returns for now.
            if (PrincipalValue <= 0 || PaymentAmount <= 0 || InterestRate < 0 || EscrowPayment <= 0) return;

            // Remove data from previous calculations
            LoanStates = new List<LoanState>();

            // Loop to calculate individual payments
            do
            {
                // Grab last state if available, otherwise use this object's values
                var lastState = LoanStates.LastOrDefault();
                var interest = (lastState?.CurrentPrincipal ?? PrincipalValue) * ((InterestRate / 12d) / 100d);

                LoanStates.Add(new LoanState
                {
                    Interest = interest,
                    CurrentPrincipal = (lastState?.CurrentPrincipal ?? PrincipalValue) - (PaymentAmount - interest - EscrowPayment),
                    PaymentNumber = (lastState?.PaymentNumber ?? 0) + 1
                });
            } while (LoanStates.Last().CurrentPrincipal > 0);
        }
    }
}