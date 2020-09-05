using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using FootballLibrary;

namespace KDZ
{
    public partial class Form1 : Form
    {
        #region Объявление необходимых полей
        List<Player> allPlayers = new List<Player>();
        Player[] userTeam = new Player[11];
        Player[] botTeam = new Player[11];
        //Данные для загрузки игры из сохранения
        int CurrentRound = 1;
        int userPoints = 0;
        int botPoints = 0;
        bool isUserAttack = true;
        // Необходимые таблицы для реализации фильтрации
        DataTable dt = new DataTable();
        DataTable currentdt = new DataTable();
        DataTable newdt = new DataTable();
        #endregion

        public Form1()
        {
            InitializeComponent();
            MessageBox.Show("Вы можете отредактировать таблицу по своему усмотрению, после этого нажать кнопку \"Готово\" и " +
                "выбрать игроков. Все внесенные измненения будут видны при очищении параметров фильтрации. Для добавления игрока" 
                + " нажмите сначала на строку с игроком, а потом на кнопку Добавить.", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
            button1.Enabled = false;
            button3.Enabled = false;
            //Восстановление из сохранения, при его наличии
            if (File.Exists("xmlDoc.xml"))
            {
                var res = MessageBox.Show("Хотите продолжить последнюю игру?", "Инструкция", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    if (TakeFromXml() && Checktems())
                    {
                        Hide();
                        Form2 form = new Form2(userTeam, botTeam, CurrentRound, userPoints, botPoints, isUserAttack);
                        form.ShowDialog();
                        Close();
                    }
                    else
                    {      
                        try
                        {
                            if (File.Exists("xmlDoc.xml"))
                                File.Delete("xmlDoc.xml");
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        #region Загрузка сохранания из XML
        /// <summary>
        /// Метод считывает информацию из XML и парсит её
        /// </summary>
        bool TakeFromXml()
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load("xmlDoc.xml");
                Player[] team = new Player[11];
                XmlElement xRoot = xDoc.DocumentElement;
                int k = 0;
                //Цикл по всем элементам xml
                foreach (XmlNode xnode in xRoot)
                {
                    team = new Player[11];
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        Player player = new Player();
                        foreach (XmlNode childnode2 in childnode.ChildNodes)
                        {
                            switch (childnode2.Name)
                            {
                                case "ID":
                                    player.Id = int.Parse(childnode2.InnerText);
                                    break;
                                case "URL":
                                    player.Url = childnode2.InnerText;
                                    break;
                                case "ShortName":
                                    player.ShortName = childnode2.InnerText;
                                    break;
                                case "LongName":
                                    player.LongName = childnode2.InnerText;
                                    break;
                                case "Age":
                                    player.Age = int.Parse(childnode2.InnerText);
                                    break;
                                case "Birthday":
                                    player.Birthday = DateTime.Parse(childnode2.InnerText);
                                    break;
                                case "Height":
                                    player.Height = int.Parse(childnode2.InnerText);
                                    break;
                                case "Weight":
                                    player.Weight = int.Parse(childnode2.InnerText);
                                    break;
                                case "Nationality":
                                    player.Nationality = childnode2.InnerText;
                                    break;
                                case "Club":
                                    player.Club = childnode2.InnerText;
                                    break;
                                case "Overall":
                                    player.Overall = int.Parse(childnode2.InnerText);
                                    break;
                                case "Potential":
                                    player.Potential = int.Parse(childnode2.InnerText);
                                    break;
                            }
                        }
                        team[k] = player;
                        k++;
                        if (k == 11) k = 0;
                    }


                    if (xnode.Name == "PlayerTeam")
                    {
                        userTeam = (Player[])team.Clone();
                    }
                    else if (xnode.Name == "BotTeam")
                    {
                        botTeam = (Player[])team.Clone();
                    }
                    switch (xnode.Name)
                    {
                        case "UserPoints":
                            userPoints = int.Parse(xnode.InnerText);
                            break;
                        case "BotPoints":
                            botPoints = int.Parse(xnode.InnerText);
                            break;
                        case "CurrentRound":
                            CurrentRound = int.Parse(xnode.InnerText);
                            break;
                        case "IsUserAttack":
                            isUserAttack = bool.Parse(xnode.InnerText);
                            break;
                    }
                }
                if (CurrentRound > 30 || CurrentRound < 1 || userPoints > 16 || userPoints<0 || botPoints > 16 || botPoints<0)
                    throw new XmlException();
                if (userPoints > CurrentRound || botPoints > CurrentRound || botPoints + userPoints > CurrentRound)
                    throw new XmlException();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Сохранение не найдено, нажмите ОК, чтобы начать новую игру", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (XmlException)
            {
                MessageBox.Show("Проблемы с файлом сохранения, нажмите ОК, чтобы начать новую игру", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось воостановить игру, нажмите ОК, чтобы начать новую", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод для проверки команд на корректность
        /// </summary>
        /// <returns>Корректны команды или нет</returns>
        bool Checktems()
        {
            try
            {
                for (int i = 0; i < botTeam.Length; i++)
                {
                    if (userTeam[i].Weight == 0 || string.IsNullOrEmpty(userTeam[i].Url) || string.IsNullOrEmpty(userTeam[i].ShortName)
                              || string.IsNullOrEmpty(userTeam[i].Nationality) || string.IsNullOrEmpty(userTeam[i].LongName)
                              || userTeam[i].Id == 0 || userTeam[i].Height == 0 || userTeam[i].Overall == 0 || userTeam[i].Potential == 0
                              || userTeam[i].Weight == 0)
                    {
                        MessageBox.Show("Файл сохранение поврежден!", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }

                    if (botTeam[i].Weight == 0 || string.IsNullOrEmpty(botTeam[i].Url) || string.IsNullOrEmpty(botTeam[i].ShortName)
                                || string.IsNullOrEmpty(botTeam[i].Nationality) || string.IsNullOrEmpty(botTeam[i].LongName)
                                || botTeam[i].Id == 0 || botTeam[i].Height == 0 || botTeam[i].Overall == 0 || botTeam[i].Potential == 0
                                || botTeam[i].Weight == 0)
                    {
                        MessageBox.Show("Файл сохранение поврежден!", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
            } 
            catch (Exception)
            {
                MessageBox.Show("Файл сохранение поврежден!", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        #endregion

        #region Загрузка игроков из таблицы
        private void LoadAllPleyers(object sender, EventArgs e)
        {
            //Добавление столбцов
            dt.Columns.Add("id");
            dt.Columns.Add("url");
            dt.Columns.Add("short name");
            dt.Columns.Add("long name");
            dt.Columns.Add("age");
            dt.Columns.Add("birthday");
            dt.Columns.Add("height");
            dt.Columns.Add("weight");
            dt.Columns.Add("nationality");
            dt.Columns.Add("club");
            dt.Columns.Add("overall");
            dt.Columns.Add("potential");
            //Считывание из файла
            try
            {
                using (var reader = new StreamReader(@"..\..\..\FIFA.csv"))
                {
                    bool f = true;
                    string[] value = new string[13];
                    do
                    {
                        if (f)
                        {
                            var first = reader.ReadLine();
                            f = false;
                            continue;
                        }
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            break;
                        value = line.Split(';');
                        if (!CheckTwice(value[0]))
                            continue;
                        try
                        {
                            dt.Rows.Add(new object[] { value[0], value[1], value[2], value[3], value[4],
                                value[5], value[6], value[7], value[8],value[9], value[10], value[11]});

                            allPlayers.Add(new Player(int.Parse(value[0]), value[1], value[2], value[3], int.Parse(value[4]),
                                DateTime.Parse(value[5]), int.Parse(value[6]), int.Parse(value[7]), value[8], value[9],
                                int.Parse(value[10]), int.Parse(value[11])));
                        }
                        catch (Exception)
                        {
                            throw new Exception();                    
                        }
                    } while (true);
                    if (allPlayers.Count < 22)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
            //Обработка исключений + повтор решений
            catch (ArgumentOutOfRangeException)
            {
                var res = MessageBox.Show("Слишком мало игроков в базе! Повторить попытку?", "Ошибка!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (res == DialogResult.Yes)
                {
                    Hide();
                    try
                    {
                        Form1 f = new Form1();
                        f.ShowDialog();
                    }
                    catch (Exception) { }
                    Close();
                }
                else
                {
                    Hide();
                    Close();
                }
            }
            catch (FileNotFoundException)
            {
                var res = MessageBox.Show("Файл с игроками не найден! Повторить попытку?", "Ошибка!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (res == DialogResult.Yes)
                {
                    Hide();
                    try
                    {
                        Form1 f = new Form1();
                        f.ShowDialog();
                    } catch (Exception) { }
                    Close();
                }
                else
                {
                    Hide();
                    Close();
                }

            }
            catch (Exception)
            {
                var res = MessageBox.Show("Файл с игроками повреждён, или ячейки имеют некорректный формат! Повторить попытку?", "Ошибка!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (res == DialogResult.Yes)
                {
                    Hide();
                    try
                    {
                        Form1 f = new Form1();
                        f.ShowDialog();
                    }
                    catch (Exception) { }
                    Close();
                }
                else
                {
                    Hide();
                    Close();
                }
            }

            prevdt = dt;
            currentdt = dt;
            dataGridView1.DataSource = dt;
            foreach (DataGridViewBand band in dataGridView1.Columns)
            {
                band.ReadOnly = true;
            }
        }
        #endregion

        #region Набор команды
        int countTeamPlayers = 0;
        /// <summary>
        /// Обработчик события, который добавляет выбранного игрока в команду
        /// </summary>
        /// <param name="sender">Издатель</param>
        /// <param name="e">Данные</param>
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToOrderColumns = false;
            int ind = dataGridView1.CurrentRow.Index;
            string idind = dataGridView1[0, dataGridView1.SelectedCells[0].RowIndex].Value.ToString();
            // Нахождение выбранного игрока по id
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (allPlayers[i].Id.ToString() == idind)
                    ind = i;
            }
            // Проверка на повтор игрока
            for (int i = 0; i < countTeamPlayers; i++)
            {
                if (userTeam[i].Id == allPlayers[ind].Id)
                {
                    MessageBox.Show("У вас уже есть такой игрок в команде!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            userTeam[countTeamPlayers] = allPlayers[ind];
            switch (countTeamPlayers)
            {
                case 0:
                    label12.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 1:
                    label13.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 2:
                    label14.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 3:
                    label15.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 4:
                    label16.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 5:
                    label17.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 6:
                    label18.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 7:
                    label19.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 8:
                    label20.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 9:
                    label21.Text = userTeam[countTeamPlayers].ToString();
                    break;
                case 10:
                    label22.Text = userTeam[countTeamPlayers].ToString();
                    break;
            }
            countTeamPlayers++;
            if (countTeamPlayers == 11)
            {
                button1.Enabled = false;
                button3.Enabled = true;
            }
        }
        #endregion

        #region Кнопки изменния навигации и логики
        private void button2_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button2.Enabled = false;
            button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
            CreateBotTeam();
            Form2 game = new Form2(userTeam, botTeam);
            game.ShowDialog();
            Close();
        }
        #endregion

        #region Вспомогательный функции для команд и проверки уникальности

        /// <summary>
        /// Id на совпадения
        /// </summary>
        /// <param name="a">id</param>
        /// <returns>Нашлось совпадение или нет</returns>
        bool CheckTwice(string a)
        {
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (allPlayers[i].Id.ToString() == a)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Метод создаёт команду компьютера
        /// </summary>
        void CreateBotTeam()
        {
            int k = 0;
            for (int i = allPlayers.Count - 1; i >= 0; i--)
            {
                bool f = true;
                for (int j = 0; i < userTeam.Length; i++)
                {
                    if (allPlayers[i].Id == userTeam[j].Id)
                        f = false;
                }
                if (f)
                {
                    botTeam[k] = allPlayers[i];
                    k++;
                    if (k == 4) break;
                }

            }

            for (int i = 0; i < allPlayers.Count ; i++)
            {
                bool f = true;
                for (int j = 0; i < userTeam.Length; i++)
                {
                    if (allPlayers[i].Id == userTeam[j].Id)
                        f = false;
                }
                if (f)
                {
                    botTeam[k] = allPlayers[i];
                    k++;
                    if (k == 11) break;
                }

            }


        }

        #endregion

        #region Измнение полей пользователем
        /// <summary>
        /// Обработчик изменения поля, выбранного пользователем
        /// </summary>
        /// <param name="sender">Издатель</param>
        /// <param name="e">Данные</param>
        private void button4_Click(object sender, EventArgs e)
        {
            int rind = 0;
            string idind = dataGridView1[0, dataGridView1.SelectedCells[0].RowIndex].Value.ToString();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (allPlayers[i].Id.ToString() == idind)
                    rind = i;
            }
            int cind = dataGridView1.SelectedCells[0].ColumnIndex;
            int res = 0;

            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Поле не может быть пустым!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Отдельная обработка id
            if (cind == 0 && !int.TryParse(textBox4.Text, out res) || cind == 0 && res >=1000000 || cind == 0 && res < 1)
            {
                MessageBox.Show("ID должны быть целым числом от 1 до 999999", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (cind == 0)
            {
                foreach (var i in allPlayers)
                {
                    if (textBox4.Text == i.Id.ToString())
                    {
                        MessageBox.Show("Параметр \"ID\" должен быть уникальным!", "Ошибка!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                allPlayers[rind].Id = res;
                dt.Rows[rind].SetField(cind, textBox4.Text);
                dataGridView1.CurrentCell.Value = textBox4.Text;
                return;
            }

            
            if (cind == 0 || cind == 4 || cind == 6 || cind == 7 || cind == 10 || cind == 11)
                if (int.TryParse(textBox4.Text, out res) && res > 0 && res<=100 || cind == 6 && int.TryParse(textBox4.Text, out res) && res > 100 && res <= 200)
                {
                    switch (cind)
                    {
                        case 0:
                            allPlayers[rind].Id = res;
                            break;
                        case 4:
                            allPlayers[rind].Age = res;
                            break;
                        case 6:
                            allPlayers[rind].Height = res;
                            break;
                        case 7:
                            allPlayers[rind].Weight = res;
                            break;
                        case 10:
                            allPlayers[rind].Overall = res;
                            break;
                        case 11:
                            allPlayers[rind].Potential = res;
                            break;
                    }
                    dt.Rows[rind].SetField(cind, textBox4.Text);
                    dataGridView1.CurrentCell.Value = textBox4.Text;
                    return;
                }
                else
                {
                    if (cind == 6)
                    {
                        MessageBox.Show("Параметр, который вы хотите изменить, должен быть целым числом от 1 до 250!",
                       "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    MessageBox.Show("Параметр, который вы хотите изменить, должен быть целым числом от 1 до 100!",
                        "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            DateTime s;
            if (cind == 5)
            {
                if (DateTime.TryParse(textBox4.Text, out s))
                {
                    allPlayers[rind].Birthday = s;
                    dt.Rows[rind].SetField(cind, textBox4.Text);
                    dataGridView1.CurrentCell.Value = textBox4.Text;
                    return;
                }
                else
                {
                    MessageBox.Show("Неверный формат даты!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            switch (cind)
            {
                case 1:
                    allPlayers[rind].Url = textBox4.Text;
                    break;
                case 2:
                    allPlayers[rind].ShortName = textBox4.Text;
                    break;
                case 3:
                    allPlayers[rind].LongName = textBox4.Text;
                    break;
                case 8:
                    allPlayers[rind].Nationality = textBox4.Text;
                    break;
                case 9:
                    allPlayers[rind].Club = textBox4.Text;
                    break;
            }
            dt.Rows[rind].SetField(cind, textBox4.Text);
            dataGridView1.CurrentCell.Value = textBox4.Text;
        }
        #endregion

        #region Фильтрация
       /// <summary>
       /// Метод отчищает поля фильтрации
       /// </summary>
       /// <param name="sender">Издатель</param>
       /// <param name="e">Данные</param>
        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        int prevind = 0;
        int newind = 0;
        //Вспомогательная таблицу 
        DataTable prevdt;
        /// <summary>
        /// Обработчик фильтрации
        /// </summary>
        /// <param name="sender">Издатель</param>
        /// <param name="e">Данные</param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            int ind = int.Parse(((TextBox)sender).Name[7].ToString());
            int curCollumn = 0;
            TextBox tx = new TextBox();

            switch (ind)
            {
                case 1:
                    curCollumn = 10;
                    tx = textBox1;
                    break;
                case 2:
                    curCollumn = 8;
                    tx = textBox2;
                    break;
                case 3:
                    curCollumn = 11;
                    tx = textBox3;
                    break;

            }
            if (prevind == 0)
            {
                prevind = ind;
                newind = ind;
            }
            else
            {
                prevind = newind;
                newind = ind;
            }
            if (prevind == ind)
                currentdt = prevdt;
            else
                prevdt = currentdt;


            AddCollums();
            for (int i = 0; i < currentdt.Rows.Count; i++)
            {
                if (currentdt.Rows[i].ItemArray[curCollumn].ToString() == tx.Text)
                {
                    object[] ar = currentdt.Rows[i].ItemArray;
                    newdt.Rows.Add(ar);
                }
            }
            currentdt = newdt;
            if (string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                dataGridView1.DataSource = prevdt;
                currentdt = prevdt;
            }
            else
                dataGridView1.DataSource = currentdt;

            if (string.IsNullOrEmpty(textBox1.Text) && string.IsNullOrEmpty(textBox2.Text) && string.IsNullOrEmpty(textBox3.Text))
            {
                prevdt = dt;
                currentdt = dt;
                dataGridView1.DataSource = currentdt;
            }


        }
        /// <summary>
        /// Вспомогательная функция для оъявления стобцов таблицы
        /// </summary>
        void AddCollums()
        {
            newdt = new DataTable();
            newdt.Columns.Add("id");
            newdt.Columns.Add("url");
            newdt.Columns.Add("short name");
            newdt.Columns.Add("long name");
            newdt.Columns.Add("age");
            newdt.Columns.Add("birthday");
            newdt.Columns.Add("height");
            newdt.Columns.Add("weight");
            newdt.Columns.Add("nationality");
            newdt.Columns.Add("club");
            newdt.Columns.Add("overall");
            newdt.Columns.Add("potential");
        }
        #endregion
    }
}

/* Альтернативное решение
 * Чтение файла csvReader.
 * Сохранение в xml через сериализацию, а не вручную.
 */
