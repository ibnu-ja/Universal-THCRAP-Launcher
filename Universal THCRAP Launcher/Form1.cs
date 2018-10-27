﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Universal_THCRAP_Launcher.Properties;

namespace Universal_THCRAP_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void ErrorAndExit(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        public Configuration Configuration1 { get; private set; } =
            new Configuration(true, 5, false, true, true, ".js", "th");

        private void Form1_Load(object sender, EventArgs e)
        {
            const string msg_Error1 =
                "thcrap_loader.exe couldn't be found.\nMake sure you put the application next to it!";
            const string msg_Error2 =
                "games.js couldn't be found.\nMake sure you run thcrap_configure.exe first!";
            const string msg_Error3 =
                "No config files could be found.\nMake sure you run thcrap_configure.exe first!";

            if (!File.Exists("thcrap_loader.exe")) ErrorAndExit(msg_Error1);
            if (!File.Exists("games.js")) ErrorAndExit(msg_Error2);

            List<string> jsFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.js").ToList();

            for (int i = 0; i < jsFiles.Count; i++)
                jsFiles[i] = jsFiles[i].Replace(Directory.GetCurrentDirectory() + "\\", "");

            jsFiles.Remove("games.js");
            jsFiles.Remove("config.js");

            if (jsFiles.Count == 0) ErrorAndExit(msg_Error3);

            foreach (var item in jsFiles)
            {
                listBox1.Items.Add(item);
            }

            FileStream fs = new FileStream("games.js", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string file = sr.ReadToEnd();
            sr.Close();
            fs.Close();

            Dictionary<string, string> games = JsonConvert.DeserializeObject<Dictionary<string, string>>(file);

            foreach (var item in games)
                listBox2.Items.Add(item.Key);

            listBox1.SelectedIndex = 0;
            listBox2.SelectedIndex = 0;

            //MessageBox.Show((string)listBox1.SelectedItem);

            Configuration1.UthcraplLastConfig = (string) listBox1.SelectedItem;
            Configuration1.UthcraplLastGame = (string) listBox2.SelectedItem;

            //MessageBox.Show(Configuration1.UthcraplLastConfig);

            if (File.Exists("config.js"))
            {
                file = File.ReadAllText("config.js");
                Configuration1 = JsonConvert.DeserializeObject<Configuration>(file);
                checkBox1.Checked = Configuration1.UthcraplExitAfterStartup;
                if (Configuration1.UthcraplLastConfig != null || Configuration1.UthcraplLastGame != null)
                {
                    listBox1.SelectedIndex = listBox1.FindString(Configuration1.UthcraplLastConfig);
                    listBox2.SelectedIndex = listBox2.FindString(Configuration1.UthcraplLastGame);
                }
            }

            UpdateConfigFile();
            Debug.WriteLine("Form1 Loaded");
        }

        private void UpdateConfigFile()
        {
            Configuration1.UthcraplLastConfig = (string) listBox1.SelectedItem;
            Configuration1.UthcraplLastGame = (string) listBox2.SelectedItem;
            string output = JsonConvert.SerializeObject(Configuration1, Formatting.Indented);
            File.WriteAllText("config.js", output);
            Debug.WriteLine("Config file Updated!");
        }

        private void StartThcrap()
        {
            string s = "";
            s += listBox1.SelectedItem;
            s += " ";
            s += listBox2.SelectedItem;
            //MessageBox.Show(args);
            Process process = new Process {StartInfo = {FileName = "thcrap_loader.exe", Arguments = s}};
            process.Start();
            Debug.WriteLine("Starting thcrap with {0}", s);
            if (checkBox1.Checked)
                Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e) => StartThcrap();

        private new void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
                StartThcrap();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateConfigFile();
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            button1.BackgroundImage = Resources.Shinmera_Banner_5_mini_size_hover;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Resources.Shinmera_Banner_5_mini_size;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            button1.Size = new Size(Size.Width - 43, button1.Size.Height);
            splitContainer1.Size = new Size(Size.Width - 38, Size.Height - 154);
            listBox1.Size = new Size(splitContainer1.Panel1.Width - 1, splitContainer1.Panel1.Height - 1);
            listBox2.Size = new Size(splitContainer1.Panel2.Width - 1, splitContainer1.Panel2.Height - 1);
            checkBox1.Location = new Point(checkBox1.Location.X, Size.Height - 88);
            label1.Location = new Point(label1.Location.X, Size.Height - 62);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Tudi20/Universal-THCRAP-Launcher");
        }
    }

#pragma warning disable IDE1006 // Naming Styles
    public class Configuration
    {
        public Configuration(bool background_updates, int time_between_updates, bool update_at_exit, bool update_others,
            bool uthcraplExitAfterStartup, string uthcraplLastConfig, string uthcraplLastGame)
        {
            this.background_updates = background_updates;
            this.time_between_updates = time_between_updates;
            this.update_at_exit = update_at_exit;
            this.update_others = update_others;
            UthcraplExitAfterStartup = uthcraplExitAfterStartup;
            UthcraplLastConfig = uthcraplLastConfig;
            UthcraplLastGame = uthcraplLastGame;
        }

        public bool background_updates { get; set; }
        public int time_between_updates { get; set; }
        public bool update_at_exit { get; set; }
        public bool update_others { get; set; }
        public bool UthcraplExitAfterStartup { get; set; }
        public string UthcraplLastConfig { get; set; }
        public string UthcraplLastGame { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}