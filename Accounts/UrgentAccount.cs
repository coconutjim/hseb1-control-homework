using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts
{
    class UrgentAccount:BankAccount
    {
        DateTime closingDate;
        public bool EarlyClosing { get; set; }
        public override string Percent
        {
            get
            {
                return percent;
            }
            set
            {
                double test;
                if (!double.TryParse(value, out test) || test > 15 || test < 10)
                    throw new Exception("Ошибка в значении процентной ставки!\nПамятка: При этом типе вклада процентная ставка должна принимать значения от 10 до 15");
                percent = test.ToString("F3");
            }
        }
        public DateTime ClosingDate
        {
            get
            {
                return closingDate;
            }
            set
            {
                if (value.CompareTo(OpeningDate.AddMonths(6)) < 0 || value.CompareTo(OpeningDate.AddYears(10)) > 0)
                    throw new Exception();
                closingDate = value;
            }
        }
        public UrgentAccount(string name, string num, string ody, string odm, string odd, string bal, string per,
            string cdy, string cdm, string cdd, bool ec)
            : base(name, num, ody, odm, odd, bal, per)
        {
            try
            {
                ClosingDate = new DateTime(int.Parse(cdy), int.Parse(cdm), int.Parse(cdd));
            }
            catch
            {
                throw new Exception("Ошибка в значении даты! Дата должна соответствовать формату ДД-ММ-ГГГГ\nПамятка: 1) Минимальный срок счета - 6 месяцев!\n2) Максимальный срок счета - 10 лет!");
            }
            EarlyClosing = ec;
        }
        public UrgentAccount(string line, byte dataItems)
            : base(line, dataItems)
        {
            string[] ar = line.Split(' ');
            ClosingDate = DateTime.Parse(ar[5]);
            EarlyClosing = bool.Parse(ar[6]);
        }
        public override string ToStringValue()
        {
            string res = String.Format("Срочный {0:15} {1} {2} {3} {4} {5} {6}", Name, Number,
                OpeningDate.ToShortDateString(), Balance, Percent, ClosingDate.ToShortDateString(), EarlyClosing);
            return res;
        }
        public override string ToShortString()
        {
            string res = String.Format("{0:60} Срочный", Name);
            return res;
        }
        public override string CalcBal(string per, string bal, string fby, string fbm, string fbd)
        {
            try
            {
                DateTime d = new DateTime(int.Parse(fby), int.Parse(fbm), int.Parse(fbd));
                DateTime now = DateTime.Now;
                if (d.CompareTo(now) < 0 || d.CompareTo(ClosingDate) > 0 || d.CompareTo(OpeningDate) < 0 || d.CompareTo(new DateTime(2050, 01, 01)) > 0)
                    throw new Exception();
                int years = d.Year - now.Year;
                if (d.AddYears(OpeningDate.Year - d.Year).CompareTo(OpeningDate) < 0) years--;
                double newBal = double.Parse(bal);
                for (int i = 0; i < years; i++) newBal *= (double.Parse(per) / 10 + 1);
                return newBal.ToString("F3");
            }
            catch
            {
                throw new Exception("Ошибка в значении даты! Дата должна соответствовать формату ДД-ММ-ГГГГ!\nПамятка: 1) Дата должна быть позднее текущей.\n2) Проверьте, попадает ли дата в интервал времени существования счёта.\n3) Дата не может быть позднее 01.01.2050.");
            }
        }
    }
}
