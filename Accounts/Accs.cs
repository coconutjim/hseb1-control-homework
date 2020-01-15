using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Accounts
{
    public partial class Accs : Form
    {
        public Accs()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            toolStripMenuSave.Enabled = false;
        }
        static byte state = 0;
        static bool ifsaved = true;
        static string fileName = null;
        List<BankAccount> BA = new List<BankAccount>();
        List<BankAccount> FiltBA = new List<BankAccount>();
        static StreamWriter writeFile;
        static StreamReader readFile;
        void SaveorAddData(bool a)
        {
            writeFile = new StreamWriter(fileName, a);
            string line;
            string k1 = "SH::=";
            string k2 = "D: ";
            for (int i = 0; i < BA.Count; i++)
            {
                line = BA[i].ToStringValue();
                writeFile.WriteLine(k1 + line.Substring(0, line.IndexOf(' ')));
                writeFile.WriteLine(k2 + line.Remove(0, line.IndexOf(' ') + 1));
            }
            writeFile.Flush();
            writeFile.Close();
            string temp = a ? "Добавление" : "Сохранение";
            temp += String.Format(" выполнено успешно\n(Файл: {0})", fileName);
            MessageBox.Show(temp, "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void ShowElementsForUrgent(bool flag)
        {
            label8.Visible = flag;
            label9.Visible = flag;
            label10.Visible = flag;
            textBoxCDDay.Visible = flag;
            textBoxCDMonth.Visible = flag;
            textBoxCDYear.Visible = flag;
            checkBoxEC.Visible = flag;
        }
        void ShowElementsForRepl(bool flag)
        {
            label11.Visible = flag;
            label22.Visible = flag;
            textBoxMinSum.Visible = flag;
        }
        void ShowElementsForCalc(bool flag)
        {
            label15.Visible = flag;
            label16.Visible = flag;
            label17.Visible = flag;
            label21.Visible = flag;
            textBoxFBDay.Visible = flag;
            textBoxFBMonth.Visible = flag;
            textBoxFBYear.Visible = flag;
            buttonCalc.Visible = flag;
            labelRes.Visible = flag;
            labelRes.Text = "Результат: ";
        }
        void ClearAll()
        {
            textBoxName.Clear();
            textBoxNumber.Clear();
            textBoxODYear.Clear();
            textBoxODMonth.Clear();
            textBoxODDay.Clear();
            textBoxBalance.Clear();
            textBoxPercent.Clear();
            comboBox1.SelectedIndex = 0;
            checkBoxOver.Checked = false;
            if (state == 1)
            {
                textBoxCDDay.Clear();
                textBoxCDMonth.Clear();
                textBoxCDYear.Clear();
                checkBoxEC.Checked = false;
            }
            if (state == 2)
            {
                textBoxCDDay.Clear();
                textBoxCDMonth.Clear();
                textBoxCDYear.Clear();
                checkBoxEC.Checked = false;
                textBoxMinSum.Clear();
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                ClearAll();
                ShowElementsForUrgent(false);
                if (state == 2) ShowElementsForRepl(false);
                checkBoxOver.Visible = true;
                state = 0;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                ClearAll();
                ShowElementsForUrgent(true);
                if (state == 0) checkBoxOver.Visible = false;
                if (state == 2) ShowElementsForRepl(false);
                state = 1;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                ClearAll();
                ShowElementsForUrgent(true);
                ShowElementsForRepl(true);
                if (state == 0) checkBoxOver.Visible = false;
                state = 2;
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            BankAccount o = new BankAccount();
            try
            {
                switch (state)
                {
                    case 0:
                        {
                            DepositAccount obj = new DepositAccount(textBoxName.Text, textBoxNumber.Text,
                                    textBoxODYear.Text, textBoxODMonth.Text, textBoxODDay.Text, textBoxBalance.Text,
                                    textBoxPercent.Text, checkBoxOver.Checked);
                            o = obj;
                            break;
                        }
                    case 1:
                        {
                            UrgentAccount obj = new UrgentAccount(textBoxName.Text, textBoxNumber.Text,
                                    textBoxODYear.Text, textBoxODMonth.Text, textBoxODDay.Text, textBoxBalance.Text,
                                    textBoxPercent.Text, textBoxCDYear.Text, textBoxCDMonth.Text, textBoxCDDay.Text, checkBoxEC.Checked);
                            o = obj;
                            break;
                        }
                    case 2:
                        {
                            ReplenishableUrgentAccount obj = new ReplenishableUrgentAccount(textBoxName.Text, textBoxNumber.Text,
                                    textBoxODYear.Text, textBoxODMonth.Text, textBoxODDay.Text, textBoxBalance.Text,
                                    textBoxPercent.Text, textBoxCDYear.Text, textBoxCDMonth.Text, textBoxCDDay.Text, checkBoxEC.Checked,
                                    textBoxMinSum.Text);
                            o = obj;
                            break;
                        }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка в данных!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (Check(o))
            {
                MessageBox.Show("Такой номер уже есть в списке. Номера не должны повторяться.", "Ошибка в данных!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            BA.Add(o);
            listBox1.Items.Add(o.ToShortString());
            ClearAll();
            ifsaved = false;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != 0)
            {
                BA.Remove(FiltBA[listBox1.SelectedIndex]);
                FiltBA.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
            else
            {
                BA.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
            labelInfo.Text = "info";
            ifsaved = false;
        }

        private void toolStripMenuSaveAsSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "text files (.txt)|*txt";
            saveFileDialog1.Title = "Сохранить данные о счетах в файл";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                fileName = saveFileDialog1.FileName;
                ifsaved = true;
                toolStripMenuSave.Enabled = true;
                SaveorAddData(false);
            }
        }

        private void toolStripMenuSaveAsAdd_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "text files (.txt)|*txt";
            saveFileDialog1.Title = "Добавить данные о счетах в файл";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                fileName = saveFileDialog1.FileName;
                ifsaved = true;
                toolStripMenuSave.Enabled = true;
                SaveorAddData(true);
            }
        }

        private void toolStripMenuSaveSave_Click(object sender, EventArgs e)
        {
            SaveorAddData(false);
            ifsaved = true;
        }

        private void toolStripMenuSaveAdd_Click(object sender, EventArgs e)
        {
            SaveorAddData(true);
            ifsaved = true;
        }

        private void toolStripMenuSave_EnabledChanged(object sender, EventArgs e)
        {
            toolStripMenuSaveSave.Visible = toolStripMenuSave.Enabled;
            toolStripMenuSaveAdd.Visible = toolStripMenuSave.Enabled;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                buttonRemove.Enabled = false;
                labelInfo.Visible = false;
                ShowElementsForCalc(false);
            }
            else
            {
                buttonRemove.Enabled = true;
                ShowElementsForCalc(true);
                labelInfo.Visible = true;
                if (comboBox1.SelectedIndex != 0)
                    labelInfo.Text = ToInfo(FiltBA[listBox1.SelectedIndex]);
                else labelInfo.Text = ToInfo(BA[listBox1.SelectedIndex]);
            }
        }

        private void toolStripMenuOpen_Click(object sender, EventArgs e)
        {
            if (!ifsaved)
            {
                DialogResult res = MessageBox.Show("Текущие изменения в файле не сохранены. Вы хотите сохранить их?",
                    "Предупреждение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    DialogResult resres = MessageBox.Show("Добавить(да) или перезаписать(нет)", "Вид записи",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resres == DialogResult.Yes)
                    {
                        if (toolStripMenuSave.Enabled)
                            toolStripMenuSaveAdd_Click(this, null);
                        else toolStripMenuSaveAsAdd_Click(this, null);
                        ifsaved = true;
                    }
                    if (resres==DialogResult.No)
                    {
                        if (toolStripMenuSave.Enabled)
                            toolStripMenuSaveSave_Click(this, null);
                        else toolStripMenuSaveAsSave_Click(this, null);
                        ifsaved = true;
                    }
                }
                if (res == DialogResult.Cancel)
                {
                    return;
                }
            }
            ClearAll();
            listBox1.Items.Clear();
            BA.Clear();
            openFileDialog1.Filter = "text files (.txt)|*txt";
            openFileDialog1.Title = "Открыть файл с данными";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result==DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                readFile = new StreamReader(fileName);
                string firstLine;
                string secondLine;
                string temp;
                try
                {
                    while (true)
                    {
                        temp = readFile.ReadLine();
                        if (temp == null) break;
                        firstLine = temp.Remove(0, 5);
                        switch (firstLine)
                        {
                            case "Депозитный":
                                {
                                    temp = readFile.ReadLine();
                                    secondLine = temp.Remove(0, temp.IndexOf(' ') + 1);
                                    DepositAccount obj0 = new DepositAccount(secondLine, 6);
                                    if (Check(obj0)) throw new Exception();
                                    BA.Add(obj0);
                                    listBox1.Items.Add(secondLine.Substring(0, secondLine.IndexOf(' ')) + " " + firstLine);
                                    break;
                                }
                            case "Срочный":
                                {
                                    temp = readFile.ReadLine();
                                    secondLine = temp.Remove(0, temp.IndexOf(' ') + 1);
                                    UrgentAccount obj1 = new UrgentAccount(secondLine, 7);
                                    if (Check(obj1)) throw new Exception();
                                    BA.Add(obj1);
                                    listBox1.Items.Add(secondLine.Substring(0, secondLine.IndexOf(' ')) + " " + firstLine);
                                    break;
                                }
                            case "Срочный_пополняемый":
                                {
                                    temp = readFile.ReadLine();
                                    secondLine = temp.Remove(0, temp.IndexOf(' ') + 1);
                                    ReplenishableUrgentAccount obj2 = new ReplenishableUrgentAccount(secondLine, 8);
                                    if (Check(obj2)) throw new Exception();
                                    BA.Add(obj2);
                                    listBox1.Items.Add(secondLine.Substring(0, secondLine.IndexOf(' ')) + " " + firstLine);
                                    break;
                                }
                            default: { throw new Exception(); }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Данные в файле непригодны для интерпретации!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    listBox1.Items.Clear();
                    BA.Clear();
                    toolStripMenuSave.Enabled = false;
                    fileName = "";
                }
                readFile.Close();
                toolStripMenuSave.Enabled = true;
                ifsaved = true;
                buttonRemove.Enabled = false;
                labelInfo.Visible = false;
                ShowElementsForCalc(false);
            }
        }
        string ToInfo(BankAccount obj)
        {
            string line;
            string type = "";
            switch (obj.GetType().ToString())
            {
                case "Accounts.DepositAccount": { type = "Депозитный"; break; }
                case "Accounts.UrgentAccount": { type = "Срочный"; break; }
                case "Accounts.ReplenishableUrgentAccount": { type = "Срочный пополняемый"; break; }
            }
            line = "Фамилия владельца: " + obj.Name + "\nТип счёта: " + type + "\nНомер счёта: " + obj.Number +
                "\nДата открытия: " + obj.OpeningDate.ToShortDateString() +
                "\nСумма на счёте: " + obj.Balance +" рублей"+ "\nПроцентная ставка: " + obj.Percent+" %";
            switch (type)
            {
                case "Депозитный":
                    {
                        DepositAccount obj0 = (DepositAccount)obj;
                        line += "\nНаличие овердрафта: ";
                        line += obj0.Overdraft ? "Есть" : "Нет";
                        break;
                    }
                case "Срочный":
                    {
                        UrgentAccount obj1 = (UrgentAccount)obj;
                        line += "\nДата закрытия: ";
                        line += obj1.ClosingDate.ToShortDateString();
                        line += "\nНаличие даты досрочного закрытия: ";
                        line += obj1.EarlyClosing ? "Есть" : "Нет";
                        break;
                    }
                case "Срочный пополняемый":
                    {
                        ReplenishableUrgentAccount obj2 = (ReplenishableUrgentAccount)obj;
                        line += "\nДата закрытия: ";
                        line += obj2.ClosingDate.ToShortDateString();
                        line += "\nВозможность досрочного закрытия: ";
                        line += obj2.EarlyClosing ? "Есть" : "Нет";
                        line += "\nМинимальная сумма пополнения: ";
                        line += obj2.MinReplSum;
                        line += " руб.";
                        break;
                    }
            }
            return line;
        }
        List<BankAccount> Filter(string filterString)
        {
            return BA.FindAll(ba => ba.ToShortString().Remove(0, ba.ToShortString().IndexOf(' ') + 1) == filterString);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != 0)
            {
                FiltBA = Filter(comboBox1.SelectedItem.ToString());
                listBox1.Items.Clear();
                for (int i = 0; i < FiltBA.Count; i++)
                    listBox1.Items.Add(FiltBA[i].ToShortString());
            }
            else
            {
                listBox1.Items.Clear();
                for (int i = 0; i < BA.Count; i++)
                    listBox1.Items.Add(BA[i].ToShortString());
            }
            labelInfo.Visible=false;
            ShowElementsForCalc(false);
            buttonRemove.Enabled = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripMenuOpen_Click(this, null);
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            toolStripMenuOpen_Click(this, null);
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (toolStripMenuSave.Enabled)
                toolStripMenuSaveSave_Click(this, null);
            else toolStripMenuSaveAsSave_Click(this, null);
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            if (toolStripMenuSave.Enabled)
                toolStripMenuSaveAdd_Click(this, null);
            else toolStripMenuSaveAsAdd_Click(this, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ifsaved)
            {
                DialogResult res = MessageBox.Show("Текущие изменения в файле не сохранены. Вы хотите сохранить их?",
                    "Предупреждение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    DialogResult resres = MessageBox.Show("Добавить(да) или перезаписать(нет)", "Вид записи",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resres == DialogResult.Yes)
                    {
                        if (toolStripMenuSave.Enabled)
                            toolStripMenuSaveAdd_Click(this, null);
                        else toolStripMenuSaveAsAdd_Click(this, null);
                        ifsaved = true;
                    }
                    if (resres == DialogResult.No)
                    {
                        if (toolStripMenuSave.Enabled)
                            toolStripMenuSaveSave_Click(this, null);
                        else toolStripMenuSaveAsSave_Click(this, null);
                        ifsaved = true;
                    }
                }
                if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void toolStripMenuOut_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCalc_Click(object sender, EventArgs e)
        {
            BankAccount obj;
            if (comboBox1.SelectedIndex == 0)
                obj = BA[listBox1.SelectedIndex];
            else obj = FiltBA[listBox1.SelectedIndex];
            string result = "Результат: ";
            try
            {
                switch (obj.GetType().ToString())
                {
                    case "Accounts.DepositAccount":
                        {
                            DepositAccount obj0 = (DepositAccount)obj;
                            result += obj0.CalcBal(obj0.Percent, obj0.Balance, textBoxFBYear.Text, textBoxFBMonth.Text, textBoxFBDay.Text);
                            break;
                        }
                    case "Accounts.UrgentAccount":
                        {
                            UrgentAccount obj1 = (UrgentAccount)obj;
                            result += obj1.CalcBal(obj1.Percent, obj1.Balance, textBoxFBYear.Text, textBoxFBMonth.Text, textBoxFBDay.Text);
                            break;
                        }
                    case "Accounts.ReplenishableUrgentAccount":
                        {
                            ReplenishableUrgentAccount obj2 = (ReplenishableUrgentAccount)obj;
                            result += obj2.CalcBal(obj2.Percent, obj2.Balance, textBoxFBYear.Text, textBoxFBMonth.Text, textBoxFBDay.Text);
                            break;
                        }
                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка в данных!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            labelRes.Text = result + " руб.";
        }

        private void toolStripMenuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа для обработки банковских счетов.\nАвтор: Лев Осипов, студент первого курса НИУ ВШЭ\nКонтактый e-mail: osipov.enterprise@gmail.com", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        bool Check(BankAccount o)
        {
            bool flag = false;
            foreach (BankAccount obj in BA)
                if (String.Compare(obj.Number, o.Number) == 0) flag = true;
            return flag;
        }

        private void Accs_Load(object sender, EventArgs e)
        {

        }

        private void checkBoxOver_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
