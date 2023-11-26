using System;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursovaya
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database.accdb;Persist Security Info=False;";

        private int userId;
        private int balance;
        private int minBet = 50;
        private int maxBet;
        private Random random = new Random();
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private int spinDuration = 3000; // Время вращения в миллисекундах


        public Form1(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadBalanceFromDatabase();
            maxBet = balance;
            UpdateBalancelabel1();
            CheckBalance(); // Вызывается при открытии формы


        }

        // Добавлено событие при загрузке формы
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckBalance();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // Заблокировать кнопку перед началом анимации
            button1.Enabled = false;

            int bet = (int)numericUpDown1.Value;

            if (balance <= 0)
            {
                MessageBox.Show("Вы банкрот!", "Игра окончена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button1.Enabled = false;
                button2.Enabled = true;
                return;
            }

            if (bet < minBet || bet > maxBet)
            {
                MessageBox.Show($"Минимальная ставка {minBet}.", "Invalid bet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                button1.Enabled = true;
                return;
            }

            await SpinNumbersAsync();

            // Разблокировать кнопку перед отображением результатов
            button1.Enabled = true;

            int digit1 = GetRandomDigit();
            int digit2 = GetRandomDigit();
            int digit3 = GetRandomDigit();

            label2.Text = digit1.ToString();
            label3.Text = digit2.ToString();
            label4.Text = digit3.ToString();

            if (digit1 == 7 && digit2 == 7 && digit3 == 7)
            {
                pictureBox1.Visible = true;
                await Task.Delay(1200);
                pictureBox1.Visible = false;
                int winnings = bet * 15;
                balance += winnings;
                MessageBox.Show($"Поздравляем! Вы выиграли {winnings}!", "Выигрыш!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateBalancelabel1();
                UpdateBalanceInDatabase();

            }

            else
            {

                balance -= bet;

                UpdateBalancelabel1();
                UpdateBalanceInDatabase();
                if (balance <= 0)
                {
                    button1.Enabled = false;
                    MessageBox.Show("Вы банкрот! ", "Игра окончена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Разблокировать кнопку после завершения игры
            button1.Enabled = true;

            // Проверить баланс после каждого хода
            CheckBalance();
        }



        private int GetRandomDigit()
        {
            return random.Next(10);
        }

        private async Task SpinNumbersAsync()
        {
            int steps = 20; // Количество шагов анимации
            int interval = spinDuration / steps;

            for (int i = 0; i < steps; i++)
            {
                label2.Text = GetRandomDigit().ToString();
                label3.Text = GetRandomDigit().ToString();
                label4.Text = GetRandomDigit().ToString();

                await Task.Delay(interval);
            }
        }

        private void UpdateBalancelabel1()
        {
            label1.Text = $"Баланс: {balance}";
        }

        private void LoadBalanceFromDatabase()
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT balance FROM [User] WHERE user_id = @userId";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        balance = Convert.ToInt32(result);
                    }
                }
            }
        }

        private void UpdateBalanceInDatabase()
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE [User] SET balance = @balance WHERE user_id = @userId";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@balance", balance);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            balance += 1000;
            UpdateBalanceInDatabase();
            MessageBox.Show("Ваш баланс пополнен на 1000! Теперь вы настоящий миллионер!", "Пополнение баланса", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateBalancelabel1();

            // Проверить баланс после пополнения
            CheckBalance();
        }

        private void CheckBalance()
        {
            // Проверить баланс и заблокировать/разблокировать кнопки
            button1.Enabled = balance > 0;
            button2.Enabled = balance <= 0;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            this.Hide();
            loginForm.FormClosed += (s, args) => this.Close();
            loginForm.Show();
        }
    }
}
