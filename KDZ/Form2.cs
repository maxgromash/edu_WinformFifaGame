using FootballLibrary;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace KDZ
{
    public partial class Form2 : Form
    {
        #region Объявление необходимых переменных
        Random rand = new Random();
        Player[] userTeam;
        Player[] botTeam;
        int CurrentRound = 1;
        int CurrentReer = 1;
        int userPoints = 0;
        int botPoints = 0;
        int indUserPlayer = -1;
        int indBotPlayer = -1;
        bool isUserAttack = true;
        #endregion

        #region Конструктор для загрузки из сохранения
        /// <summary>
        /// Контрукт сохранения 
        /// </summary>
        /// <param name="userTeam">Команда игрока</param>
        /// <param name="botTeam">Команда бота</param>
        /// <param name="CurrentRound">Текущий раунд</param>
        /// <param name="userPoints">Очки игрока</param>
        /// <param name="botPoints">Очки бота</param>
        /// <param name="isUserAttack">Кто атакует</param>
        public Form2(Player[] userTeam, Player[] botTeam, int CurrentRound, int userPoints, 
            int botPoints, bool isUserAttack)
        {
            CenterToScreen();
            InitializeComponent();
            this.userTeam = userTeam;
            this.botTeam = botTeam;
            this.CurrentRound = CurrentRound;
            this.botPoints = botPoints;
            this.userPoints = userPoints;
            this.isUserAttack = isUserAttack;
            label3.Text = $"Раунд {CurrentRound} : Этап {CurrentReer}";
            label1.Text = $"Счёт {userPoints} : {botPoints}";
            InitButtons();
            if (isUserAttack)
                richTextBox1.AppendText("Выберите атакующего: ");
            else
                richTextBox1.AppendText("Выберите защитника: ");
            SaveToXml();
        }
        #endregion

        #region Основной конструктор
        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="userTeam">Команда игрока</param>
        /// <param name="botTeam">Команда бота</param>
        public Form2(Player[] userTeam, Player[] botTeam)
        {
            InitializeComponent();
            this.userTeam = userTeam;
            this.botTeam = botTeam;
            InitButtons();
            richTextBox1.AppendText("Выберите атакующего: ");
            SaveToXml();
        }
        #endregion

        #region Геймплей
        /// <summary>
        /// Считает очки игрока
        /// </summaryИгрок>
        /// <param name="player"></param>
        /// <returns></returns>
        double CalcPoints(Player player)
        {
            return (((double)(player.Height - player.Weight)) / 10.0) *
                (((double)player.Overall) / Math.Max(player.Overall - player.Potential, 1));
        }

        /// <summary>
        /// Метод определяет, прошел игрок дальше или нет
        /// </summary>
        /// <param name="player">Игрок пользовтеля</param>
        /// <param name="player2">Игрок компьютера</param>
        void WhoWin(Player player, Player player2)
        {
            if (isUserAttack)
            {
                if (CalcPoints(player) > CalcPoints(player2))
                {
                    if (CurrentReer == 3)
                    {
                        isUserAttack = false;
                        richTextBox1.Clear();
                        userPoints++;
                        UpdateTablo("\n\n                                         ГОООООООЛЛЛЛ!\n\n", "Выберите защитника: ");
                    }
                    else
                    {
                        richTextBox1.AppendText("\nВы прошли дальше! Выберите атакующего: ");
                        CurrentReer++;
                        label3.Text = $"Раунд {CurrentRound} : Этап {CurrentReer}";
                    }
                }
                else
                {
                    isUserAttack = false;
                    UpdateTablo("\n\nМЯЧЬ ПОТЕРЯН\n", "Выберите защитника: ");
                }
            }
            else
            {
                if (CalcPoints(player) < CalcPoints(player2))
                {
                    if (CurrentReer == 3)
                    {
                        isUserAttack = true;
                        botPoints++;
                        richTextBox1.Clear();
                        UpdateTablo("\n\n                                          ВАМ ЗАБИЛИ ГОЛ!\n\n", "Выберите  нападающего: ");
                    }
                    else
                    {
                        richTextBox1.AppendText("\nСоверник продвигается вперед!. Выберите защитника: ");
                        CurrentReer++;
                        label3.Text = $"Раунд {CurrentRound} : Этап {CurrentReer}";
                    }
                }
                else
                {
                    isUserAttack = true;
                    UpdateTablo("\n\nМЯЧЬ ПОЛУЧЕН!\n", "Выберите нападающего: ");
                }
            }

        }

        /// <summary>
        /// Метод обновляет табло
        /// </summary>
        /// <param name="s">Текст1 для TextBox</param>
        /// <param name="s2">Текст2 для TextBox</param>
        void UpdateTablo(string s, string s2)
        {
            CurrentRound++;
            CurrentReer = 1;
            label3.Text = $"Раунд {CurrentRound} : Этап {CurrentReer}";
            label1.Text = $"Счёт {userPoints} : {botPoints}";
            richTextBox1.AppendText(s);
            richTextBox1.AppendText(s2);
            if (CurrentRound > 30) EndGame();
            if (CurrentRound < 31) SaveToXml();
        }

        /// <summary>
        /// Обработчик нажатия на игрока
        /// </summary>
        /// <param name="sender">Издатель</param>
        /// <param name="e">Данные</param>
        private void SelectPlayer(object sender, EventArgs e)
        {
            string s = ((Button)sender).Name;
            int n;
            if (s.Length == 8)
            {
                if (s[7] == '1')
                    n = 11;
                else 
                    n = 10;
            }
            else
            {
                n = int.Parse(s[6].ToString());
            }
            indUserPlayer = n - 1;
            richTextBox1.AppendText($"{userTeam[indUserPlayer].ShortName}");
            indBotPlayer = rand.Next(0, 11);
            WhoWin(userTeam[indUserPlayer], botTeam[indBotPlayer]);
        }
        #endregion

        #region Вспомогательные методы
        void InitButtons()
        {
            button1.Text = userTeam[0].Id.ToString() + " " + userTeam[0].ShortName;
            button2.Text = userTeam[1].Id.ToString() + " " + userTeam[1].ShortName;
            button3.Text = userTeam[2].Id.ToString() + " " + userTeam[2].ShortName;
            button4.Text = userTeam[3].Id.ToString() + " " + userTeam[3].ShortName;
            button5.Text = userTeam[4].Id.ToString() + " " + userTeam[4].ShortName;
            button6.Text = userTeam[5].Id.ToString() + " " + userTeam[5].ShortName;
            button7.Text = userTeam[6].Id.ToString() + " " + userTeam[6].ShortName;
            button8.Text = userTeam[7].Id.ToString() + " " + userTeam[7].ShortName;
            button9.Text = userTeam[8].Id.ToString() + " " + userTeam[8].ShortName;
            button10.Text = userTeam[9].Id.ToString() + " " + userTeam[9].ShortName;
            button11.Text = userTeam[10].Id.ToString() + " " + userTeam[10].ShortName;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
        #endregion

        #region Конец игры и сохранения
        void EndGame()
        {
            File.Delete("xmlDoc.xml");
            richTextBox1.Clear();
            if (userPoints > botPoints)
                MessageBox.Show("Вы выйграли!", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (userPoints == botPoints)
                MessageBox.Show("Ничья", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show("Вы проиграли!", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var res = MessageBox.Show("Хотите сыграть еще раз?", "Инструкция", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (res == DialogResult.No)
            {
                Hide();
                Close();
            }
            else
            {
                Hide();
                Form1 game = new Form1();
                game.ShowDialog();
                Close();
            }
        }

        /// <summary>
        /// Метод сохраняет состояние игры в xml
        /// </summary>
        void SaveToXml()
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Football"));
            XElement xmlTree = new XElement("PlayerTeam");
            foreach (var item in userTeam)
            {
                xmlTree.Add(new XElement("Player",
                    new XElement("ID", item.Id),
                    new XElement("URL", item.Url),
                    new XElement("ShortName", item.ShortName),
                    new XElement("LongName", item.LongName),
                    new XElement("Age", item.Age),
                    new XElement("Birthday", item.Birthday),
                    new XElement("Height", item.Height),
                    new XElement("Weight", item.Weight),
                    new XElement("Nationality", item.Nationality),
                    new XElement("Club", item.Club),
                    new XElement("Overall", item.Overall),
                    new XElement("Potential", item.Potential)));
            }
            xmlDoc.Root.Add(xmlTree);
            XElement xmlTreeBot = new XElement("BotTeam");
            foreach (var item in botTeam)
            {
                xmlTreeBot.Add(new XElement("Player",
                    new XElement("ID", item.Id),
                    new XElement("URL", item.Url),
                    new XElement("ShortName", item.ShortName),
                    new XElement("LongName", item.LongName),
                    new XElement("Age", item.Age),
                    new XElement("Birthday", item.Birthday),
                    new XElement("Height", item.Height),
                    new XElement("Weight", item.Weight),
                    new XElement("Nationality", item.Nationality),
                    new XElement("Club", item.Club),
                    new XElement("Overall", item.Overall),
                    new XElement("Potential", item.Potential)));
            }
            xmlDoc.Root.Add(xmlTreeBot);
            xmlDoc.Root.Add(new XElement("UserPoints", userPoints));
            xmlDoc.Root.Add(new XElement("BotPoints", botPoints));
            xmlDoc.Root.Add(new XElement("CurrentRound", CurrentRound));
            xmlDoc.Root.Add(new XElement("IsUserAttack", isUserAttack));
            xmlDoc.Save("xmlDoc.xml");
            
        }
        #endregion

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
