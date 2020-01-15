using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts
{
    class BankAccount
    {
        string name;
        string number;
        DateTime openingDate;
        protected string balance;
        protected string percent;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                    if (!((value[i] >= 'A' && value[i] <= 'Z') || (value[i] >= 'a' && value[i] <= 'z') || (value[i] >= 'А' && value[i] <= 'я')))
                        throw new Exception("Ошибка в значении фамилии!\nПамятка: Фамилия не может состоять более чем из 15 символов и должна состоять только из букв.");
                if (value == "" || value.Length > 15 || value.IndexOf(' ') != -1)
                    throw new Exception("Ошибка в значении фамилии!\nПамятка: Фамилия не может состоять более чем из 15 символов и должна состоять только из букв.");
                name = value;
            }
        }
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                int test;
                if (value.Length != 6 || !int.TryParse(value, out test) || test < 0)
                    throw new Exception("Ошибка в значении номера счета!\nПамятка: Счет должен представлять собой шестизначное число.");
                number = value;
            }
        }
        public DateTime OpeningDate
        {
            get
            {
                return openingDate;
            }
            set
            {
                if (value.CompareTo(new DateTime(2000, 10, 10)) < 0 || value.CompareTo(DateTime.Now) > 0)
                    throw new Exception();
                openingDate = value;
            }
        }
        public virtual string Percent { get; set; }
        public virtual string Balance
        {
            get
            {
                return balance;
            }
            set
            {
                double test;
                if (!double.TryParse(value, out test) || test < 0)
                    throw new Exception("Ошибка в значении баланса!\nПамятка: Баланс при этом типе счёта не может быть отрицательным.");
                balance = test.ToString("F3");
            }
        }
        public BankAccount(string name, string num, string ody, string odm, string odd, string bal, string per)
        {
            Name = name; Number = num;
            try
            {
                OpeningDate = new DateTime(int.Parse(ody), int.Parse(odm), int.Parse(odd));
            }
            catch
            {
                throw new Exception("Ошибка в значении даты! Дата должна соответствовать формату ДД-ММ-ГГГГ\nПамятка: 1) Банк был открыт 10.10.2001!\n 2) Дата не может быть позднее текущей");
            }
            Balance = bal; Percent = per;
        }
        public BankAccount(string line, byte dataItems)
        {
            string[] ar = line.Split(' ');
            if (ar.Length != dataItems)
                throw new Exception();
            Name = ar[0];
            Number = ar[1];
            OpeningDate = DateTime.Parse(ar[2]);
            Balance = ar[3];
            Percent = ar[4];
        }
        public BankAccount()
        {
        }
        public virtual string ToStringValue() { return Name; }
        public virtual string ToShortString() { return Name; }
        public virtual string CalcBal(string per, string bal, string fby, string fbm, string fbd) { return Balance; }
    }
}
