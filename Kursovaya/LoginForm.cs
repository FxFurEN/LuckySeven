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
using static System.Windows.Forms.DataFormats;

namespace Kursovaya
{
    public partial class LoginForm : Form
    {
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database.accdb;Persist Security Info=False;";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user_login = textBox1.Text;
            string user_password = textBox2.Text;

            int userId = IsValidUser(user_login, user_password);

            if (userId != -1)
            {
                MessageBox.Show("Авторизация успешна!");
                Form1 mainForm = new Form1(userId);
                this.Hide();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Неверные учетные данные. Пожалуйста, повторите попытку.");
            }
        }
        private int IsValidUser(string user_login, string user_password)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT user_id FROM [User] WHERE user_login = @user_login AND user_password = @user_password";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_login", user_login);
                    command.Parameters.AddWithValue("@user_password", user_password);

                    try
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return -1; // Или другое значение, которое будет обозначать неудачу.
                        }
                    }
                    catch (OleDbException ex)
                    {
                        MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                        return -1; // Или другое значение, которое будет обозначать неудачу.
                    }
                }
            }
        }


        private void label4_Click(object sender, EventArgs e)
        {

            RegisterForm registerForm = new RegisterForm();
            this.Hide();
            registerForm.FormClosed += (s, args) => this.Close();
            registerForm.Show();
        }
    }
}
