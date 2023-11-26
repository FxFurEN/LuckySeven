using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursovaya
{
    public partial class RegisterForm : Form
    {
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database.accdb;Persist Security Info=False;";

        public RegisterForm()
        {
            InitializeComponent();
        }


        private void btnRegister_Click(object sender, EventArgs e)
        {
            string user_login = textBox1.Text;
            string user_password = textBox2.Text;

            if (RegisterUser(user_login, user_password))
            {
                MessageBox.Show("Регистрация успешна!");
                LoginForm loginForm = new LoginForm();
                this.Hide();
                loginForm.FormClosed += (s, args) => this.Close();
                loginForm.Show();
            }
            else
            {
                MessageBox.Show("Не удалось зарегистрировать пользователя. Пожалуйста, повторите попытку.");
            }
        }

        private bool RegisterUser(string user_login, string user_password)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();

                // Проверка, существует ли аккаунт с указанным логином
                string checkQuery = "SELECT COUNT(*) FROM [User] WHERE user_login = @user_login";
                using (OleDbCommand checkCommand = new OleDbCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@user_login", user_login);
                    int existingAccountsCount = (int)checkCommand.ExecuteScalar();

                    if (existingAccountsCount > 0)
                    {
                        // Аккаунт с указанным логином уже существует
                        MessageBox.Show("Пользователь с таким логином уже существует. Выберите другой логин.", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // Если аккаунта с указанным логином нет, добавляем новый аккаунт
                string insertQuery = "INSERT INTO [User] (user_login, user_password) VALUES (@user_login, @user_password)";
                using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@user_login", user_login);
                    command.Parameters.AddWithValue("@user_password", user_password);
                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return true; // Успешная регистрация
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (OleDbException ex)
                    {
                        MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        private void label4_Click_1(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            this.Hide();
            loginForm.FormClosed += (s, args) => this.Close();
            loginForm.Show();
        }
    }
}
