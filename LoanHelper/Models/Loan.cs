using System;
using System.Collections.Generic;
using System.Linq;

namespace LoanHelper.Models
{
    public class Loan
    {
        public double PrincipalValue { get; set; }

        private double _interestRate;
        public double InterestRate
        {
            get => _interestRate;
            set
            {
                _interestPercentage = null;
                _interestRate = value;
            }
        }
        public InterestFrequency Frequency { get; set; }

        private double? _interestPercentage;
        /// <summary>
        /// Converts the InterestRate to a percentage to be used in formulas
        /// </summary>
        public double InterestPercentage {
            get
            {
                _interestPercentage ??= Frequency == InterestFrequency.Annually
                    ? (InterestRate / 100d) / 12d
                    : InterestRate / 100d;
                return _interestPercentage.Value;
            }
        }

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
            PaymentAmount = (InterestPercentage * PrincipalValue) / (1 - Math.Pow(1 + InterestPercentage, NumberOfPeriods * -1));
        }
        
        /// <summary>
        /// Calculates the periods required to pay off loan when all other info is given
        /// </summary>
        public void CalculatePeriods()
        {
            // Check for valid data. Just returns for now.
            if (PrincipalValue <= 0 || PaymentAmount <= 0 || InterestRate < 0) return;
            
            
            // -log(1 - r(PV)/P) / log(1+r)
            // Check number in log for negative to avoid issues
            var innerLog = 1 - ((InterestPercentage * PrincipalValue) / PaymentAmount);
            if (innerLog < 0) return;
            
            NumberOfPeriods = (int) Math.Ceiling(-Math.Log(innerLog) / Math.Log(1 + InterestPercentage));
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
                
                // If loan is growing then the loop will be infinite
                if (lastState is not null && lastState.CurrentPrincipal > PrincipalValue)
                {
                    LoanStates = new List<LoanState>();
                    return;
                }
                
                var interest = (lastState?.CurrentPrincipal ?? PrincipalValue) * InterestPercentage;

                LoanStates.Add(new LoanState
                {
                    Interest = interest,
                    CurrentPrincipal = (lastState?.CurrentPrincipal ?? PrincipalValue) - (PaymentAmount - interest - EscrowPayment),
                    PaymentNumber = (lastState?.PaymentNumber ?? 0) + 1
                });
            } while (LoanStates.Last().CurrentPrincipal > 0 || LoanStates.Count > 1000);    // Limit to 1000 states

            // Zero out the last payment
            LoanStates.Last().CurrentPrincipal = 0;
        }
    }
}