﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class SodaMachine
    {
        //Member Variables (Has A)
        private List<Coin> _register;
        private List<Can> _inventory;

        //Constructor (Spawner)
        public SodaMachine()
        {
            _register = new List<Coin>();
            _inventory = new List<Can>();
            FillInventory();
            FillRegister();
        }

        //Member Methods (Can Do)

        //A method to fill the sodamachines register with coin objects.
        public void FillRegister()
        {
            for (int i = 0; i < 20; i++)
            {               
                _register.Add(new Quarter());
            }
            for (int i = 0; i < 10; i++)
            {
                _register.Add(new Dime());
            }
            for (int i = 0; i < 20; i++)
            {
                _register.Add(new Nickel());
            }
            for (int i = 0; i < 50; i++)
            {
                _register.Add(new Penny());
            }
        }
        //A method to fill the sodamachines inventory with soda can objects.
        public void FillInventory()
        {
            for (int i = 0; i < 20; i++)
            {
                _inventory.Add(new RootBeer());
            }
            for (int i = 0; i < 20; i++)
            {
                _inventory.Add(new Cola());
            }
            for (int i = 0; i < 20; i++)
            {
                _inventory.Add(new OrangeSoda());
            }
        }
        //Method to be called to start a transaction.
        //Takes in a customer which can be passed freely to which ever method needs it.
        public void BeginTransaction(Customer customer)
        {
            bool willProceed = UserInterface.DisplayWelcomeInstructions(_inventory);
            if (willProceed)
            {
                Transaction(customer);
            }
        }
        
        //This is the main transaction logic think of it like "runGame".  This is where the user will be prompted for the desired soda.
        //grab the desired soda from the inventory.
        //get payment from the user.
        //pass payment to the calculate transaction method to finish up the transaction based on the results.
        private void Transaction(Customer customer)
        {
            string selectSoda = UserInterface.SodaSelection(_inventory);
            Can selectSodaFromInventory = GetSodaFromInventory(selectSoda);
            List<Coin> payment = customer.GatherCoinsFromWallet(selectSodaFromInventory);
            CalculateTransaction(payment, selectSodaFromInventory, customer);
        }
        //Gets a soda from the inventory based on the name of the soda.
        private Can GetSodaFromInventory(string nameOfSoda)
        {
            foreach (Can can in _inventory)
            {
                if (can.Name == nameOfSoda)
                {
                    return can;
                }
            }
            return null;
        }

        private bool CheckSodaFromInventory(string nameOfSoda)
        {
            foreach (Can can in _inventory)
            {
                if (can.Name == nameOfSoda)
                {
                    return true;
                }
            }
            return false;
        }

        //This is the main method for calculating the result of the transaction.
        //It takes in the payment from the customer, the soda object they selected, and the customer who is purchasing the soda.
        //This is the method that will determine the following:
        //If the payment is greater than the price of the soda, and if the sodamachine has enough change to return: Dispense soda, and change to the customer.
        //If the payment is greater than the cost of the soda, but the machine does not have ample change: Dispense payment back to the customer.
        //If the payment is exact to the cost of the soda:  Dispense soda.
        //If the payment does not meet the cost of the soda: dispense payment back to the customer.
        private void CalculateTransaction(List<Coin> payment, Can chosenSoda, Customer customer)
        {
            double paymentTotal = TotalCoinValue(payment);
            double registerSum = TotalCoinValue(_register);
            double changeNeeded = Math.Round(DetermineChange(paymentTotal, chosenSoda.Price), 2);

           if (paymentTotal > chosenSoda.Price && registerSum > changeNeeded)
            {
                DepositCoinsIntoRegister(payment);
                customer.AddCanToBackpack(GetSodaFromInventory(chosenSoda.Name));
                customer.AddCoinsIntoWallet(GatherChange(changeNeeded));
                UserInterface.EndMessage(chosenSoda.Name, changeNeeded);
            }
           else if (paymentTotal > chosenSoda.Price && registerSum < changeNeeded)
            {
                customer.AddCoinsIntoWallet(payment);
            }
           else if (paymentTotal == chosenSoda.Price)
            {
                DepositCoinsIntoRegister(payment);
                customer.AddCanToBackpack(GetSodaFromInventory(chosenSoda.Name));
                UserInterface.EndMessage(chosenSoda.Name, changeNeeded);
            }
           else if (paymentTotal < chosenSoda.Price)
            {
                customer.AddCoinsIntoWallet(payment);
            }                    
        }
        //Takes in the value of the amount of change needed.
        //Attempts to gather all the required coins from the sodamachine's register to make change.
        //Returns the list of coins as change to despense.
        //If the change cannot be made, return null.
        public List<Coin> GatherChange(double changeValue)
        {
            List<Coin> gatherChange = new List<Coin>();
            double changeToDispense = TotalCoinValue(gatherChange);
            double changeNeeded = changeValue - changeToDispense;
            while (changeToDispense > changeValue)
            {
                if (changeValue >= 0.25 && RegisterHasCoin("Quarter"))
                {
                    gatherChange.Add(GetCoinFromRegister("Quarter"));
                }
                else if (changeValue >= 0.10 && RegisterHasCoin("Dime"))
                {
                    gatherChange.Add(GetCoinFromRegister("Dime"));
                }
                else if (changeValue >= 0.05 && RegisterHasCoin("Nickel"))
                {
                    gatherChange.Add(GetCoinFromRegister("Nickel"));
                }
                else if (changeValue >= 0.10 && RegisterHasCoin("Penny"))
                {
                    gatherChange.Add(GetCoinFromRegister("Penny"));
                }
            }
            return gatherChange;
        }
        //Reusable method to check if the register has a coin of that name.
        //If it does have one, return true.  Else, false.
        private bool RegisterHasCoin(string name)
        {
           foreach (Coin coin in _register)
            {
                if (coin.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        //Reusable method to return a coin from the register.
        //Returns null if no coin can be found of that name.
        private Coin GetCoinFromRegister(string name)
        {
            foreach (Coin coin in _register)
            {
                if (coin.Name == name)
                {
                    return coin;
                }
            }
            return null;
        }
        //Takes in the total payment amount and the price of can to return the change amount.
        private double DetermineChange(double totalPayment, double canPrice)
        {
            double changeAmount = totalPayment - canPrice;
            return changeAmount;
        }
        
        //Takes in a list of coins to return the total value of the coins as a double.
        private double TotalCoinValue(List<Coin> payment)
        {
            double totalValue = 0;
            for (int i = 0; i < payment.Count; i++)
            {
                totalValue += payment[i].Value;
            }
            return totalValue;
        }
        //Puts a list of coins into the soda machines register.
        private void DepositCoinsIntoRegister(List<Coin> coins)
        {
           foreach (Coin coin in coins)
            {
                _register.Add(coin);
            }
        }
    }
}
